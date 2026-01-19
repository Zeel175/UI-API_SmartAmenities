using Application.Interfaces;
using Domain.Entities;
using Domain.Interfaces;
using Domain.ViewModels;
using Infrastructure.Context;
using Infrastructure.Integrations.Hikvision;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime;
using System.Threading.Tasks;

namespace Application.Services
{
    public class OtpService : IOtpService
    {
        private readonly AppDbContext _db;
        private readonly UserManager<User> _userManager;
        private readonly IJwtTokenService _jwtTokenService;
        private readonly RoleManager<Role> _roleManager;
        private readonly HikvisionClient _hikvisionClient;
        private readonly ISecretProtector _secretProtector;
        private readonly ILogger<OtpService> _logger;

        public OtpService(
           AppDbContext db,
           UserManager<User> userManager,
           IJwtTokenService jwtTokenService,
           RoleManager<Role> roleManager,
           HikvisionClient hikvisionClient,
           ISecretProtector secretProtector,
           ILogger<OtpService> logger)
        {
            _db = db;
            _userManager = userManager;
            _jwtTokenService = jwtTokenService;
            _roleManager = roleManager;
            _hikvisionClient = hikvisionClient;
            _secretProtector = secretProtector;
            _logger = logger;
        }

        public async Task<GenerateOtpResponse> GenerateOtpAsync(GenerateOtpRequest request)
        {
            if (string.IsNullOrWhiteSpace(request.ContactNumber))
            {
                return new GenerateOtpResponse
                {
                    Success = false,
                    Message = "ContactNumber is required.",
                    IsLogin = false
                };
            }

            // Normalize contact number (basic)
            var contact = request.ContactNumber.Trim();

            // Generate 6-digit OTP
            var otp = new Random().Next(100000, 999999).ToString();

            // Optional: expire after 5 minutes
            var expiresAt = DateTime.UtcNow.AddMinutes(5);

            // Save OTP in DB
            var otpEntity = new OtpRequest
            {
                ContactNumber = contact,
                Otp = otp,
                ExpiresAt = expiresAt,
                IsUsed = false,

                // If your AuditableEntity needs these, set them here
                // CreatedAt = DateTime.UtcNow,
                // CreatedBy = ...
            };

            _db.OtpRequests.Add(otpEntity);
            await _db.SaveChangesAsync();

            // After save: check contact exists in User Master
            // Adjust field name depending on your User entity:
            // - If you use Identity default -> PhoneNumber
            // - If you have custom -> ContactNumber / MobileNo etc.
            var userExists = await _userManager.Users.AnyAsync(u => u.PhoneNumber == contact);

            return new GenerateOtpResponse
            {
                Success = true,
                Message = "OTP generated successfully.",
                ContactNumber = contact,
                IsLogin = userExists,
                Otp = otp // for now only (since no SMS)
            };
        }
        public async Task<VerifyOtpResponse> VerifyOtpAsync(VerifyOtpRequest request)
        {
            if (string.IsNullOrWhiteSpace(request.ContactNumber))
            {
                return new VerifyOtpResponse
                {
                    IsSuccess = false,
                    Message = "ContactNumber is required.",
                    IsLogin = false
                };
            }

            if (string.IsNullOrWhiteSpace(request.Otp))
            {
                return new VerifyOtpResponse
                {
                    IsSuccess = false,
                    Message = "Otp is required.",
                    IsLogin = false
                };
            }

            var contact = request.ContactNumber.Trim();
            var otp = request.Otp.Trim();

            var otpEntity = await _db.OtpRequests
                .Where(o => o.ContactNumber == contact && o.Otp == otp)
                .OrderByDescending(o => o.CreatedDate)
                .FirstOrDefaultAsync();

            if (otpEntity == null)
            {
                return new VerifyOtpResponse
                {
                    IsSuccess = false,
                    Message = "Invalid OTP.",
                    IsLogin = false
                };
            }

            if (otpEntity.IsUsed)
            {
                return new VerifyOtpResponse
                {
                    IsSuccess = false,
                    Message = "OTP already used.",
                    IsLogin = false
                };
            }

            if (otpEntity.ExpiresAt < DateTime.UtcNow)
            {
                return new VerifyOtpResponse
                {
                    IsSuccess = false,
                    Message = "OTP expired.",
                    IsLogin = false
                };
            }

            otpEntity.IsUsed = true;
            await _db.SaveChangesAsync();

            var user = await _userManager.Users.FirstOrDefaultAsync(u => u.PhoneNumber == contact);
            var response = new VerifyOtpResponse
            {
                IsSuccess = true,
                Message = "OTP verified.",
                IsLogin = user != null
            };

            if (user == null)
            {
                response.IsSuccess = false;
                response.Message = "User not found";
                return response;
            }

            var token = await _jwtTokenService.GenerateTokenAsync(user.UserName, user.Id);
            var roleNames = await _userManager.GetRolesAsync(user);
            var roleDetails = await GetRoleDetailsAsync(roleNames);

            var identityRoles = roleDetails.Select(r => new IdentityRole<long>
            {
                Id = r.Id,
                Name = r.Name,
                NormalizedName = r.NormalizedName,
                ConcurrencyStamp = r.ConcurrencyStamp
            }).ToList();

            List<string> permissions;
            try
            {
                permissions = await GetUserPermissionsFromDbAsync(identityRoles.Select(x => x.Id).ToList());
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Permissions load failed for user {UserId}", user.Id);
                permissions = new List<string>();
            }
            var residentMap = await _db.ResidentUserMaps
                .AsNoTracking()
                .Where(map => map.UserId == user.Id && map.IsActive)
                .OrderByDescending(map => map.CreatedDate)
                .FirstOrDefaultAsync();

            response.ResidentMasterId = residentMap?.ResidentMasterId;
            response.ResidentFamilyMemberId = residentMap?.ResidentFamilyMemberId;
            await PopulateBiometricStatusAsync(response, response.ResidentMasterId, response.ResidentFamilyMemberId);
            var unitSummaries = await GetUnitSummariesAsync(residentMap?.ResidentMasterId, residentMap?.ResidentFamilyMemberId);
            response.UnitIds = unitSummaries.Select(unit => unit.UnitId).ToList();
            response.Units = unitSummaries;
            response.Id = user.Id;
            response.Token = token;
            response.DisplayName = user.UserName;
            response.ExpireAt = DateTime.UtcNow.AddHours(1);
            response.IsAdmin = roleNames.Contains("Admin");
            response.Role = identityRoles;
            response.Permissions = permissions;
            response.User = new User
            {
                Id = user.Id,
                Name = user.Name,
                UserName = user.UserName,
                Email = user.Email,
                PhoneNumber = user.PhoneNumber
            };
            if (response.ResidentMasterId.HasValue)
            {
                response.ProfilePhoto = await _db.ResidentMasters
                    .AsNoTracking()
                    .Where(r => r.Id == response.ResidentMasterId.Value)
                    .Select(r => r.ProfilePhoto)
                    .FirstOrDefaultAsync();

                response.Documents = await _db.ResidentDocuments
                    .AsNoTracking()
                    .Where(d => d.ResidentMasterId == response.ResidentMasterId.Value && d.IsActive)
                    .OrderBy(d => d.FileName)
                    .ToListAsync();
            }

            await TryAttachFaceImageAsync(response);
            response.UserType = await ResolveUserTypeAsync(user);

            return response;
        }

        private async Task TryAttachFaceImageAsync(LoginResponse response)
        {
            if (response.ResidentMasterId.HasValue)
            {
                var residentData = await _db.ResidentMasters
                    .AsNoTracking()
                    .Where(r => r.Id == response.ResidentMasterId.Value)
                    .Select(r => new { r.FaceUrl, r.Id })
                    .FirstOrDefaultAsync();

                if (residentData == null || string.IsNullOrWhiteSpace(residentData.FaceUrl))
                    return;

                var unitIds = await _db.ResidentMasterUnits
                    .AsNoTracking()
                    .Where(u => u.ResidentMasterId == residentData.Id && u.IsActive)
                    .Select(u => u.UnitId)
                    .ToListAsync();

                await TryAttachFaceImageAsync(response, residentData.FaceUrl, unitIds);
                return;
            }

            if (response.ResidentFamilyMemberId.HasValue)
            {
                var memberData = await _db.ResidentFamilyMembers
                    .AsNoTracking()
                    .Where(m => m.Id == response.ResidentFamilyMemberId.Value)
                    .Select(m => new { m.FaceUrl, m.Id })
                    .FirstOrDefaultAsync();

                if (memberData == null || string.IsNullOrWhiteSpace(memberData.FaceUrl))
                    return;

                var unitIds = await _db.ResidentFamilyMemberUnits
                    .AsNoTracking()
                    .Where(u => u.ResidentFamilyMemberId == memberData.Id && u.IsActive)
                    .Select(u => u.UnitId)
                    .ToListAsync();

                await TryAttachFaceImageAsync(response, memberData.FaceUrl, unitIds);
            }
        }

        private async Task TryAttachFaceImageAsync(LoginResponse response, string faceUrl, List<long> unitIds)
        {
            if (string.IsNullOrWhiteSpace(faceUrl) || unitIds == null || unitIds.Count == 0)
                return;

            var buildingId = await _db.Set<Unit>()
                .AsNoTracking()
                .Where(unit => unitIds.Contains(unit.Id))
                .Select(unit => unit.BuildingId)
                .FirstOrDefaultAsync();

            if (buildingId == 0)
                return;

            var result = await TryDownloadFaceImageAsync(buildingId, faceUrl);
            if (result == null || result.Value.Data.Length == 0)
                return;

            response.FaceImageBase64 = Convert.ToBase64String(result.Value.Data);
            response.FaceImageContentType = result.Value.ContentType;
        }

        private async Task<(byte[] Data, string? ContentType)?> TryDownloadFaceImageAsync(
            long buildingId,
            string faceUrl)
        {
            if (string.IsNullOrWhiteSpace(faceUrl))
                return null;

            var device = await (
                from b in _db.Buildings
                join d in _db.HikDevices on b.DeviceId equals d.Id
                where b.IsActive && b.Id == buildingId
                select new
                {
                    d.IpAddress,
                    d.PortNo,
                    d.DevIndex,
                    b.DeviceUserName,
                    b.DevicePassword
                }).FirstOrDefaultAsync();

            if (device == null
                || string.IsNullOrWhiteSpace(device.IpAddress)
                || string.IsNullOrWhiteSpace(device.DeviceUserName)
                || string.IsNullOrWhiteSpace(device.DevicePassword))
            {
                return null;
            }

            var password = _secretProtector.Unprotect(device.DevicePassword);

            return await _hikvisionClient.DownloadFaceImageAsync(
                device.IpAddress,
                device.PortNo ?? 80,
                device.DeviceUserName,
                password,
                faceUrl,
                CancellationToken.None);
        }

        private async Task PopulateBiometricStatusAsync(LoginResponse response, long? residentMasterId, long? residentFamilyMemberId)
        {
            if (residentMasterId.HasValue)
            {
                var status = await _db.ResidentMasters
                    .AsNoTracking()
                    .Where(r => r.Id == residentMasterId.Value)
                    .Select(r => new { r.HasFace, r.HasFingerprint, r.LastBiometricSyncUtc })
                    .FirstOrDefaultAsync();

                if (status != null)
                {
                    response.HasFace = status.HasFace;
                    response.HasFingerprint = status.HasFingerprint;
                    response.LastBiometricSyncUtc = status.LastBiometricSyncUtc;
                }

                return;
            }

            if (residentFamilyMemberId.HasValue)
            {
                var status = await _db.ResidentFamilyMembers
                    .AsNoTracking()
                    .Where(f => f.Id == residentFamilyMemberId.Value)
                    .Select(f => new { f.HasFace, f.HasFingerprint, f.LastBiometricSyncUtc })
                    .FirstOrDefaultAsync();

                if (status != null)
                {
                    response.HasFace = status.HasFace;
                    response.HasFingerprint = status.HasFingerprint;
                    response.LastBiometricSyncUtc = status.LastBiometricSyncUtc;
                }
            }
        }

        private async Task<string?> ResolveUserTypeAsync(User user)
        {
            if (user.IsGuest)
            {
                return "guest";
            }

            var map = await _db.ResidentUserMaps
                .AsNoTracking()
                .Where(m => m.UserId == user.Id && m.IsActive)
                .OrderByDescending(m => m.CreatedDate)
                .FirstOrDefaultAsync();

            if (map?.ResidentMasterId != null)
            {
                return "resident";
            }

            if (map?.ResidentFamilyMemberId != null)
            {
                return "familyMember";
            }

            return "resident";
        }

        private async Task<List<Role>> GetRoleDetailsAsync(IList<string> roleNames)
        {
            var roleDetails = new List<Role>();
            foreach (var roleName in roleNames)
            {
                var role = await _roleManager.FindByNameAsync(roleName);
                if (role != null)
                    roleDetails.Add(role);
            }
            return roleDetails;
        }

        private async Task<List<string>> GetUserPermissionsFromDbAsync(List<long> roleIds)
        {
            var typeMap = GetPermissionTypeMapping();

            var permissions = await _db.RolePermissionMaps
                .Where(rp => roleIds.Contains(rp.RoleId))
                .Join(_db.Permissions,
                      rp => rp.PermissionId,
                      p => p.Id,
                      (rp, p) => p)
                .Distinct()
                .AsNoTracking()
                .ToListAsync();

            return permissions
                .Select(p =>
                {
                    var typeName = typeMap.TryGetValue(p.PermissionTypeId, out var t) ? t : "Unknown";
                    return $"{p.Name} ({p.Code}) - {typeName}";
                })
                .Distinct()
                .ToList();
        }

        private Dictionary<long, string> GetPermissionTypeMapping()
        {
            return new Dictionary<long, string>
            {
                { 72L, "Add" },
                { 73L, "Edit" },
                { 74L, "View" },
                { 75L, "Delete" },
                { 76L, "Export" }
            };
        }
        private async Task<List<UnitInfo>> GetUnitSummariesAsync(long? residentMasterId, long? residentFamilyMemberId)
        {
            var units = new List<UnitInfo>();

            // 🔹 1. Fetch ResidentMaster units if applicable
            if (residentMasterId.HasValue)
            {
                var residentUnits = await _db.ResidentMasterUnits
                    .AsNoTracking()
                    .Where(x => x.ResidentMasterId == residentMasterId.Value)
                    .Join(
                        _db.Units.AsNoTracking(),
                        rm => rm.UnitId,
                        u => u.Id,
                        (rm, u) => new UnitInfo
                        {
                            UnitId = u.Id,
                            UnitName = u.UnitName
                        })
                    .ToListAsync();

                units.AddRange(residentUnits);
            }

            // 🔹 2. Fetch FamilyMember units if applicable
            if (residentFamilyMemberId.HasValue)
            {
                var familyUnits = await _db.ResidentFamilyMemberUnits
                    .AsNoTracking()
                    .Where(x => x.ResidentFamilyMemberId == residentFamilyMemberId.Value)
                    .Join(
                        _db.Units.AsNoTracking(),
                        fm => fm.UnitId,
                        u => u.Id,
                        (fm, u) => new UnitInfo
                        {
                            UnitId = u.Id,
                            UnitName = u.UnitName
                        })
                    .ToListAsync();

                units.AddRange(familyUnits);
            }

            // 🔹 3. Remove duplicates and sort
            var distinctUnits = units
                .GroupBy(u => u.UnitId)
                .Select(g => g.First())
                .OrderBy(u => u.UnitName)
                .ToList();

            return distinctUnits;
        }
    }
}
