using Application.Interfaces;
using Domain.Entities;
using Infrastructure.Context;          // AppDbContext
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
    public class AuthService : IAuthService
    {
        private readonly SignInManager<User> _signInManager;
        private readonly UserManager<User> _userManager;
        private readonly IJwtTokenService _jwtTokenService;
        private readonly RoleManager<Role> _roleManager;
        private readonly AppDbContext _db;
        private readonly ILogger<AuthService> _logger;

        public AuthService(
            SignInManager<User> signInManager,
            UserManager<User> userManager,
            IJwtTokenService jwtTokenService,
            RoleManager<Role> roleManager,
            AppDbContext db,
            ILogger<AuthService> logger)
        {
            _signInManager = signInManager;
            _userManager = userManager;
            _jwtTokenService = jwtTokenService;
            _roleManager = roleManager;
            _db = db;
            _logger = logger;
        }

        public async Task<LoginResponse> LoginAsync(string username, string password)
        {
            var user = await _userManager.FindByNameAsync(username);
            return await BuildLoginResponseAsync(user, password);
        }

        public async Task<LoginResponse> LoginWithIdentifierAsync(string identifier, string password)
        {
            var user = await _userManager.FindByNameAsync(identifier);
            if (user == null)
            {
                user = await _userManager.Users.FirstOrDefaultAsync(u => u.PhoneNumber == identifier);
            }

            return await BuildLoginResponseAsync(user, password);
        }

        //public async Task LogoutAsync()
        //{
        //    // Optional: only useful if you ever use cookies elsewhere.
        //    await _signInManager.SignOutAsync();
        //}

        //public async Task<IdentityResult> ChangePasswordAsync(string userId, string currentPassword, string newPassword)
        //{
        //    var user = await _userManager.FindByIdAsync(userId);
        //    if (user == null)
        //        return IdentityResult.Failed(new IdentityError { Description = "User not found" });

        //    return await _userManager.ChangePasswordAsync(user, currentPassword, newPassword);
        //}

        //public async Task<IdentityResult> ResetPasswordAsync(string userId, string token, string newPassword)
        //{
        //    var user = await _userManager.FindByIdAsync(userId);
        //    if (user == null)
        //        return IdentityResult.Failed(new IdentityError { Description = "User not found" });

        //    return await _userManager.ResetPasswordAsync(user, token, newPassword);
        //}

        //private async Task<List<Role>> GetRoleDetailsAsync(IList<string> roleNames)
        //{
        //    var roleDetails = new List<Role>();
        //    foreach (var roleName in roleNames)
        //    {
        //        var role = await _roleManager.FindByNameAsync(roleName);
        //        if (role != null)
        //            roleDetails.Add(role);
        //    }
        //    return roleDetails;
        //}

        private async Task<LoginResponse> BuildLoginResponseAsync(User user, string password)
        {
            var response = new LoginResponse();

            // 1) Find user first (avoid null issues)
            //var user = await _userManager.FindByNameAsync(username);
            if (user == null)
            {
                response.IsSuccess = false;
                response.Message = "Invalid username or password";
                return response;
            }

            // 2) IMPORTANT: do NOT create cookie in API (IIS-safe)
            var check = await _signInManager.CheckPasswordSignInAsync(user, password, lockoutOnFailure: false);
            if (!check.Succeeded)
            {
                response.IsSuccess = false;
                response.Message = "Invalid username or password";
                return response;
            }

            // 3) Create JWT
            var token = await _jwtTokenService.GenerateTokenAsync(user.UserName, user.Id);

            // 4) Roles
            var roleNames = await _userManager.GetRolesAsync(user);
            var roleDetails = await GetRoleDetailsAsync(roleNames);

            var identityRoles = roleDetails.Select(r => new IdentityRole<long>
            {
                Id = r.Id,
                Name = r.Name,
                NormalizedName = r.NormalizedName,
                ConcurrencyStamp = r.ConcurrencyStamp
            }).ToList();

            // 5) Permissions from DB (NO HttpClient, NO Issuer)
            List<string> permissions;
            try
            {
                permissions = await GetUserPermissionsFromDbAsync(identityRoles.Select(x => x.Id).ToList());
            }
            catch (Exception ex)
            {
                // Never crash login because of permissions
                _logger.LogError(ex, "Permissions load failed for user {UserId}", user.Id);
                permissions = new List<string>();
            }

            // Populate response
            response.Id = user.Id;
            response.Token = token;
            response.DisplayName = user.UserName;
            response.ExpireAt = DateTime.UtcNow.AddHours(1);
            response.IsAdmin = roleNames.Contains("Admin");
            response.IsSuccess = true;
            response.Message = "Login successful";
            response.Role = identityRoles;
            response.Permissions = permissions;
            var residentMap = await _db.ResidentUserMaps
                .AsNoTracking()
                .Where(map => map.UserId == user.Id && map.IsActive)
                .OrderByDescending(map => map.CreatedDate)
                .FirstOrDefaultAsync();

            response.ResidentMasterId = residentMap?.ResidentMasterId;
            response.ResidentFamilyMemberId = residentMap?.ResidentFamilyMemberId;
            var unitSummaries = await GetUnitSummariesAsync(residentMap?.ResidentMasterId, residentMap?.ResidentFamilyMemberId);
            response.UnitIds = unitSummaries.Select(unit => unit.UnitId).ToList();
            response.Units = unitSummaries;
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
            return response;
        }

        public async Task LogoutAsync()
        {
            // Optional: only useful if you ever use cookies elsewhere.
            await _signInManager.SignOutAsync();
        }

        public async Task<IdentityResult> ChangePasswordAsync(string userId, string currentPassword, string newPassword)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
                return IdentityResult.Failed(new IdentityError { Description = "User not found" });

            return await _userManager.ChangePasswordAsync(user, currentPassword, newPassword);
        }

        public async Task<IdentityResult> ResetPasswordAsync(string userId, string token, string newPassword)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
                return IdentityResult.Failed(new IdentityError { Description = "User not found" });

            return await _userManager.ResetPasswordAsync(user, token, newPassword);
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

        // ✅ DB based permissions (adjust DbSet names if yours differ)
        private async Task<List<string>> GetUserPermissionsFromDbAsync(List<long> roleIds)
        {
            var typeMap = GetPermissionTypeMapping();

            // These DbSets must exist: _db.RolePermissions and _db.Permissions
            // If your DbSet names differ, rename them accordingly.
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
