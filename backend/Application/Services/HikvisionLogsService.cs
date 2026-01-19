using Application.Interfaces;
using Domain.ViewModels;
using Infrastructure.Context;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Net.Http;
using System.Text.Json;
using Microsoft.IdentityModel.Tokens;
using Application.Helper;

namespace Application.Services
{
    public class HikvisionLogsService : IHikvisionLogsService
    {
        private readonly AppDbContext _db;
        private readonly ISecretProtector _secretProtector;

        public HikvisionLogsService(AppDbContext db, ISecretProtector secretProtector)
        {
            _db = db;
            _secretProtector = secretProtector;
        }

        public async Task<object> GetLogsAsync(HikvisionLogsRequest request)
        {
            // 1) Fetch device details for building/unit (change table names as per your DB)
                        var device = await (
                from b in _db.Buildings
                join d in _db.HikDevices on b.DeviceId equals d.Id
                where b.Id == request.BuildingId && b.IsActive
                select new
                {
                    DeviceIp = d.IpAddress,              // <-- change column name if different
                    DeviceUserName = b.DeviceUserName,  // <-- change if different
                    DevicePassword = b.DevicePassword   // <-- change if different
                }
            ).FirstOrDefaultAsync();

            if (device == null)
                return new { Code = "404", Message = "Hikvision device not configured for this building." };

            if (string.IsNullOrWhiteSpace(device.DeviceIp))
                return new { Code = "400", Message = "Device IP is missing in hik_Devices configuration." };

            var baseUrl = $"http://{device.DeviceIp}";
            var url = $"{baseUrl}/ISAPI/AccessControl/AcsEvent?format=json";

            string decryptedPassword = _secretProtector.Unprotect(device.DevicePassword);
            // 2) Create Digest-auth HttpClient
            var handler = new HttpClientHandler
            {
                Credentials = new NetworkCredential(device.DeviceUserName, decryptedPassword),
                PreAuthenticate = false
            };

            using var httpClient = new HttpClient(handler)
            {
                Timeout = TimeSpan.FromSeconds(30)
            };

            // 3) Decide which employees to fetch
            var selectedEmployeeNos = request.EmployeeNos?
    .Where(x => !string.IsNullOrWhiteSpace(x))
    .Select(x => x.Trim())
    .Distinct()
    .ToList() ?? new List<string>();

            // B) Resolve from UserIds (Residents) and GuestIds
            if (request.UserIds != null && request.UserIds.Count > 0)
            {
                selectedEmployeeNos.AddRange(await ResolveEmployeeNosFromUserIdsAsync(request.UserIds, request.UnitId));
            }

            if (request.GuestIds != null && request.GuestIds.Count > 0)
            {
                selectedEmployeeNos.AddRange(await ResolveEmployeeNosFromGuestIdsAsync(request.GuestIds, request.UnitId));
            }

            selectedEmployeeNos = selectedEmployeeNos
                .Where(x => !string.IsNullOrWhiteSpace(x))
                .Select(x => x.Trim())
                .Distinct()
                .ToList();

            // C) AllUsers only when checkbox true AND no selected users
            var fetchAllUsers = request.AllUsers && selectedEmployeeNos.Count == 0;

            // Validation
            if (!fetchAllUsers && selectedEmployeeNos.Count == 0)
                return new { Code = "400", Message = "No valid employee codes found for selected userIds." }; 
            
            var allEvents = new List<AcsEventInfo>();
            var tz = GetIstTimeZone();
            var startLocal = TimeZoneInfo.ConvertTime(request.Start, tz);
            var endLocal = TimeZoneInfo.ConvertTime(request.End, tz);
            if (fetchAllUsers)
            {
                var events = await FetchAcsEventsAsync(httpClient, url, startLocal, endLocal, null, request.PicEnable);
                allEvents.AddRange(events);
            }
            else
            {
                foreach (var emp in selectedEmployeeNos)
                {
                    var events = await FetchAcsEventsAsync(httpClient, url, startLocal, endLocal, emp, request.PicEnable);
                    events = events.Where(e => e.employeeNoString == emp).ToList();
                    allEvents.AddRange(events);
                }
            }

            // 6) De-duplicate by serialNo (unique id)
            var dedup = allEvents
                .GroupBy(e => e.serialNo)
                .Select(g => g.First())
                .OrderBy(e => e.time) // or serialNo
                .ToList();

            return new
            {
                Code = "200",
                Total = dedup.Count,
                Data = dedup
            };
        }
        private async Task<List<string>> ResolveEmployeeNosFromUserIdsAsync(List<long> userIds, long? unitId)
        {
            if (userIds == null || userIds.Count == 0)
                return new List<string>();

            // Resident codes (UserId -> ResidentUserMap -> ResidentMaster -> Code)
            var residentCodesQuery =
                from m in _db.ResidentUserMaps
                where m.ResidentMasterId.HasValue
                    && m.IsActive
                    && userIds.Contains(m.UserId)
                join r in _db.ResidentMasters
                    on m.ResidentMasterId.Value equals r.Id
                select new { r.Id, r.Code };

            // Family codes (UserId -> ResidentUserMap -> ResidentFamilyMember -> Code)
            var familyCodesQuery =
                from m in _db.ResidentUserMaps
                where m.ResidentFamilyMemberId.HasValue
                    && m.IsActive
                    && userIds.Contains(m.UserId)
                join f in _db.ResidentFamilyMembers
                    on m.ResidentFamilyMemberId.Value equals f.Id
                select new { f.Id, f.Code };

            if (unitId.HasValue)
            {
                residentCodesQuery =
                    from r in residentCodesQuery
                    join u in _db.ResidentMasterUnits on r.Id equals u.ResidentMasterId
                    where u.IsActive && u.UnitId == unitId.Value
                    select r;

                familyCodesQuery =
                    from f in familyCodesQuery
                    join u in _db.ResidentFamilyMemberUnits on f.Id equals u.ResidentFamilyMemberId
                    where u.IsActive && u.UnitId == unitId.Value
                    select f;
            }

            // Merge + clean
            var mappedCodesQuery = residentCodesQuery.Select(r => r.Code)
                .Concat(familyCodesQuery.Select(f => f.Code));

            var codes = await mappedCodesQuery
                .Where(c => !string.IsNullOrWhiteSpace(c))
                .Select(c => c.Trim())
                .Distinct()
                .ToListAsync();

            if (codes.Count > 0)
                return codes;

            // Fallback: userIds are ResidentMaster/FamilyMember IDs (no mapping rows)
            var directResidentCodesQuery =
                from r in _db.ResidentMasters
                where r.IsActive && userIds.Contains(r.Id)
                select new { r.Id, r.Code };

            var directFamilyCodesQuery =
                from f in _db.ResidentFamilyMembers
                where f.IsActive && userIds.Contains(f.Id)
                select new { f.Id, f.Code };

            if (unitId.HasValue)
            {
                directResidentCodesQuery =
                    from r in directResidentCodesQuery
                    join u in _db.ResidentMasterUnits on r.Id equals u.ResidentMasterId
                    where u.IsActive && u.UnitId == unitId.Value
                    select r;

                directFamilyCodesQuery =
                    from f in directFamilyCodesQuery
                    join u in _db.ResidentFamilyMemberUnits on f.Id equals u.ResidentFamilyMemberId
                    where u.IsActive && u.UnitId == unitId.Value
                    select f;
            }

            var directCodesQuery = directResidentCodesQuery.Select(r => r.Code)
                .Concat(directFamilyCodesQuery.Select(f => f.Code));

            var directCodes = await directCodesQuery
                .Where(c => !string.IsNullOrWhiteSpace(c))
                .Select(c => c.Trim())
                .Distinct()
                .ToListAsync();

            return directCodes;
        }

        private async Task<List<string>> ResolveEmployeeNosFromGuestIdsAsync(List<long> guestIds, long? unitId)
        {
            if (guestIds == null || guestIds.Count == 0)
            {
                return new List<string>();
            }

            var query = _db.GuestMasters
                .AsNoTracking()
                .Where(guest => guest.IsActive && guestIds.Contains(guest.Id));

            if (unitId.HasValue)
            {
                query = query.Where(guest => guest.UnitId == unitId.Value);
            }

            return await query
                .Select(guest => guest.Code)
                .Where(code => !string.IsNullOrWhiteSpace(code))
                .Select(code => code.Trim())
                .Distinct()
                .ToListAsync();
        }

        private static TimeZoneInfo GetIstTimeZone()
        {
            try { return TimeZoneInfo.FindSystemTimeZoneById("Asia/Kolkata"); }          // Linux
            catch { return TimeZoneInfo.FindSystemTimeZoneById("India Standard Time"); } // Windows
        }

        private async Task<List<AcsEventInfo>> FetchAcsEventsAsync(
    HttpClient httpClient,
    string url,
    DateTimeOffset start,
    DateTimeOffset end,
    string employeeNo,
    bool picEnable)
        {
            var list = new List<AcsEventInfo>();
            var searchId = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds().ToString();
            int position = 0;

            while (true)
            {
                // ✅ Exact JSON structure required by Hikvision
                var requestBody = new
                {
                    AcsEventCond = new
                    {
                        searchID = searchId,
                        searchResultPosition = position,
                        maxResults = 200,
                        major = 5,
                        minor = 0,
                        startTime = start.ToString("yyyy-MM-ddTHH:mm:sszzz"),
                        endTime = end.ToString("yyyy-MM-ddTHH:mm:sszzz"),
                        employeeNoString = string.IsNullOrEmpty(employeeNo) ? null : employeeNo,
                        picEnable = picEnable
                    }
                };

                var json = System.Text.Json.JsonSerializer.Serialize(
                    requestBody,
                    new JsonSerializerOptions
                    {
                        DefaultIgnoreCondition = System.Text.Json.Serialization.JsonIgnoreCondition.WhenWritingNull
                    }

                );

                using var content = new StringContent(json, Encoding.UTF8, "application/json");
                using var resp = await httpClient.PostAsync(url, content);
                var respText = await resp.Content.ReadAsStringAsync();

                if (!resp.IsSuccessStatusCode)
                    throw new Exception($"Hikvision error {(int)resp.StatusCode}: {respText}");

                var parsed = System.Text.Json.JsonSerializer.Deserialize<AcsEventRoot>(
                    respText,
                    new System.Text.Json.JsonSerializerOptions { PropertyNameCaseInsensitive = true }
                );

                var events = parsed?.AcsEvent?.InfoList ?? new List<AcsEventInfo>();
                list.AddRange(events);

                var status = parsed?.AcsEvent?.responseStatusStrg;
                var count = parsed?.AcsEvent?.numOfMatches ?? 0;

                if (!string.Equals(status, "MORE", StringComparison.OrdinalIgnoreCase))
                    break;

                position += count;
            }

            return list;
        }

    }
}
