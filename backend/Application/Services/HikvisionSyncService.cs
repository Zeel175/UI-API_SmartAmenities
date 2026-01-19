using Application.Helper;
using Application.Interfaces;
using Infrastructure.Context;
using Infrastructure.Integrations.Hikvision;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;

namespace Application.Services
{
    public class HikvisionSyncService : IHikvisionSyncService
    {
        private readonly AppDbContext _db;
        private readonly HikvisionClient _hikvisionClient;
        private readonly ISecretProtector _secretProtector;
        private readonly ILogger<HikvisionSyncService> _logger;

        public HikvisionSyncService(
            AppDbContext db,
            HikvisionClient hikvisionClient,
            ISecretProtector secretProtector,
            ILogger<HikvisionSyncService> logger)
        {
            _db = db;
            _hikvisionClient = hikvisionClient;
            _secretProtector = secretProtector;
            _logger = logger;
        }

        public async Task SyncUserBiometricStatusAsync(
            string employeeNo,
            int? buildingId,
            string? deviceIp,
            CancellationToken ct = default)
        {
            if (string.IsNullOrWhiteSpace(employeeNo))
                return;

            var device = await ResolveDeviceAsync(buildingId, deviceIp, ct);
            if (device == null)
            {
                _logger.LogWarning("Hikvision sync skipped: device not resolved (buildingId={BuildingId}, deviceIp={DeviceIp})", buildingId, deviceIp);
                return;
            }

            _logger.LogInformation(
                "Hikvision sync started for {EmployeeNo}. buildingId={BuildingId}, deviceIp={DeviceIp}",
                employeeNo,
                buildingId,
                device.IpAddress);

            try
            {
                var status = await _hikvisionClient.GetUserBiometricStatusAsync(
                    device.IpAddress,
                    device.Port,
                    device.UserName,
                    device.Password,
                    employeeNo,
                    device.DevIndex,
                    ct);

                if (status == null)
                {
                    _logger.LogWarning("Hikvision sync skipped: no status returned for {EmployeeNo}.", employeeNo);
                    return;
                }

                _logger.LogInformation(
                    "Hikvision status received for {EmployeeNo}. HasFace={HasFace}, HasFingerprint={HasFingerprint}, HasCard={HasCard}",
                    employeeNo,
                    status.HasFace,
                    status.HasFingerprint,
                    status.HasCard);

                await ApplyBiometricStatusAsync(employeeNo, status, device, ct);
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Hikvision sync failed for {EmployeeNo}.", employeeNo);
            }
        }

        private async Task ApplyBiometricStatusAsync(
            string employeeNo,
            HikvisionBiometricStatus status,
            DeviceSyncInfo device,
            CancellationToken ct)
        {
            var resident = await _db.ResidentMasters
                .FirstOrDefaultAsync(x => x.Code == employeeNo, ct);

            if (resident != null)
            {
                var updates = new List<string>();
                if (status.HasFace && string.IsNullOrWhiteSpace(resident.FaceId))
                {
                    resident.FaceId = status.FaceId ?? "ENROLLED";
                    updates.Add("FaceId");
                }
                else if (!status.HasFace && !string.IsNullOrWhiteSpace(resident.FaceId))
                {
                    resident.FaceId = null;
                    updates.Add("FaceId");
                }

                if (status.HasFace && !string.IsNullOrWhiteSpace(status.FaceUrl))
                {
                    if (!string.Equals(resident.FaceUrl, status.FaceUrl, StringComparison.OrdinalIgnoreCase))
                    {
                        resident.FaceUrl = status.FaceUrl;
                        updates.Add("FaceUrl");
                    }
                }
                else if (!status.HasFace && !string.IsNullOrWhiteSpace(resident.FaceUrl))
                {
                    resident.FaceUrl = null;
                    updates.Add("FaceUrl");
                }

                if (status.HasFingerprint && string.IsNullOrWhiteSpace(resident.FingerId))
                {
                    resident.FingerId = status.FingerprintId ?? "ENROLLED";
                    updates.Add("FingerId");
                }
                else if (!status.HasFingerprint && !string.IsNullOrWhiteSpace(resident.FingerId))
                {
                    resident.FingerId = null;
                    updates.Add("FingerId");
                }

                // Keep CardId/QrId stable after creation. Do not override from device sync.

                resident.HasFace = status.HasFace;
                resident.HasFingerprint = status.HasFingerprint;
                resident.LastBiometricSyncUtc = DateTime.UtcNow;

                await _db.SaveChangesAsync(ct);
                _logger.LogInformation(
                    updates.Count == 0
                        ? "Hikvision sync applied for resident {EmployeeNo} with no field updates."
                        : "Hikvision sync applied for resident {EmployeeNo}. UpdatedFields={UpdatedFields}",
                    employeeNo,
                    updates.Count == 0 ? Array.Empty<string>() : updates.ToArray());
                return;
            }

            var family = await _db.ResidentFamilyMembers
                .FirstOrDefaultAsync(x => x.Code == employeeNo, ct);

            if (family != null)
            {
                var updates = new List<string>();
                if (status.HasFace && string.IsNullOrWhiteSpace(family.FaceId))
                {
                    family.FaceId = status.FaceId ?? "ENROLLED";
                    updates.Add("FaceId");
                }
                else if (!status.HasFace && !string.IsNullOrWhiteSpace(family.FaceId))
                {
                    family.FaceId = null;
                    updates.Add("FaceId");
                }

                if (status.HasFace && !string.IsNullOrWhiteSpace(status.FaceUrl))
                {
                    if (!string.Equals(family.FaceUrl, status.FaceUrl, StringComparison.OrdinalIgnoreCase))
                    {
                        family.FaceUrl = status.FaceUrl;
                        updates.Add("FaceUrl");
                    }
                }
                else if (!status.HasFace && !string.IsNullOrWhiteSpace(family.FaceUrl))
                {
                    family.FaceUrl = null;
                    updates.Add("FaceUrl");
                }

                if (status.HasFingerprint && string.IsNullOrWhiteSpace(family.FingerId))
                {
                    family.FingerId = status.FingerprintId ?? "ENROLLED";
                    updates.Add("FingerId");
                }
                else if (!status.HasFingerprint && !string.IsNullOrWhiteSpace(family.FingerId))
                {
                    family.FingerId = null;
                    updates.Add("FingerId");
                }

                // Keep CardId/QrId stable after creation. Do not override from device sync.

                family.HasFace = status.HasFace;
                family.HasFingerprint = status.HasFingerprint;
                family.LastBiometricSyncUtc = DateTime.UtcNow;

                await _db.SaveChangesAsync(ct);
                _logger.LogInformation(
                    updates.Count == 0
                        ? "Hikvision sync applied for family member {EmployeeNo} with no field updates."
                        : "Hikvision sync applied for family member {EmployeeNo}. UpdatedFields={UpdatedFields}",
                    employeeNo,
                    updates.Count == 0 ? Array.Empty<string>() : updates.ToArray());
            }
        }

        private async Task<DeviceSyncInfo?> ResolveDeviceAsync(int? buildingId, string? deviceIp, CancellationToken ct)
        {
            var query =
                from b in _db.Buildings
                join d in _db.HikDevices on b.DeviceId equals d.Id
                where b.IsActive
                select new
                {
                    b.Id,
                    d.IpAddress,
                    d.PortNo,
                    b.DeviceUserName,
                    b.DevicePassword,
                    d.DevIndex
                };

            if (buildingId.HasValue)
                query = query.Where(x => x.Id == buildingId.Value);
            else if (!string.IsNullOrWhiteSpace(deviceIp))
                query = query.Where(x => x.IpAddress == deviceIp);
            else
                return null;

            var device = await query.FirstOrDefaultAsync(ct);
            if (device == null)
                return null;

            if (string.IsNullOrWhiteSpace(device.IpAddress)
                || string.IsNullOrWhiteSpace(device.DeviceUserName)
                || string.IsNullOrWhiteSpace(device.DevicePassword))
                return null;

            var password = _secretProtector.Unprotect(device.DevicePassword);

            return new DeviceSyncInfo(
                device.IpAddress,
                device.PortNo ?? 80,
                device.DeviceUserName,
                password,
                device.DevIndex);
        }

        private sealed record DeviceSyncInfo(
            string IpAddress,
            int Port,
            string UserName,
            string Password,
            string? DevIndex);
    }
}
