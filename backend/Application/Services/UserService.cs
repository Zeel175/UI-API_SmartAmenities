using Application.Interfaces;
using Domain.Entities;  // Ensure this namespace is correct
using Microsoft.AspNetCore.Identity;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Domain.ViewModels;
using Infrastructure.Repositories;
using Infrastructure.Context;

namespace Application.Services
{
    public class UserService : IUserService
    {
        private readonly UserManager<User> _userManager; // Change to User
        private readonly IClaimAccessorService _claimAccessorService;
        private readonly AppDbContext _context;

        public UserService(UserManager<User> userManager, IClaimAccessorService claimAccessorService,
             AppDbContext context)
        {
            _userManager = userManager;
            _claimAccessorService = claimAccessorService;
            _context = context;
        }

        public async Task<IEnumerable<User>> GetAllUsersAsync()
        {
            return await _userManager.Users.Where(user => user.IsActive).ToListAsync();  // Ensure async execution
        }

       

        public async Task<User> GetUserByIdAsync(string id)
        {
            return await _userManager.FindByIdAsync(id);
        }

        public async Task<User> GetUserById(long id)
        {
            return await _userManager.FindByIdAsync(id.ToString());
        }

        //public async Task CreateUserAsync(User user)
        //{
        //    await _userManager.CreateAsync(user);
        //}

        public async Task<IdentityResult> CreateUserAsync(UserAddEdit userAddEdit)
        {
            long loggedinuserId = _claimAccessorService.GetUserId();
            // Create a User instance from UserAddEdit
            var user = new User
            {
                UserName = userAddEdit.UserName, // Or use other fields as necessary
                Email = userAddEdit.Email, // Assuming username and email are the same
                Name = userAddEdit.Name,
                PhoneNumber = userAddEdit.PhoneNumber,
                IsActive = userAddEdit.IsActive,
                CreatedDate = DateTime.Now,
                CreatedBy = loggedinuserId,
              //  ModifiedDate = userAddEdit.ModifiedDate,
              //  ModifiedBy = userAddEdit.ModifiedBy
            };

            // Audit Logging
            var auditLogs = AuditHelper.GenerateAuditLogs(null, user, "User", loggedinuserId.ToString(), "Insert");
            await _context.AuditLog.AddRangeAsync(auditLogs);
            await _context.SaveChangesAsync();

            // Use UserManager to create the user with password
            return await _userManager.CreateAsync(user, userAddEdit.Password);
        }




        public async Task UpdateUserAsync(User user)
        {
            long loggedinuserId = _claimAccessorService.GetUserId();
            var existingUser = await _userManager.FindByIdAsync(user.Id.ToString());
            if (existingUser != null)
            {
                existingUser.UserName = user.UserName;
                existingUser.Email = user.Email;
                existingUser.Name = user.Name;
                existingUser.PhoneNumber = user.PhoneNumber;
                existingUser.IsActive = user.IsActive; // Update IsActive
                existingUser.ModifiedDate = DateTime.Now;
                existingUser.ModifiedBy = loggedinuserId;
                // Update other properties as needed

                // Audit Logging
                var auditLogs = AuditHelper.GenerateAuditLogs(existingUser, user, "User", loggedinuserId.ToString(), "Update");
                await _context.AuditLog.AddRangeAsync(auditLogs);
                await _context.SaveChangesAsync();

                await _userManager.UpdateAsync(existingUser);
            }
        }

        public async Task DeleteUserAsync(string id)
        {
            long loggedinuserId = _claimAccessorService.GetUserId();
            var user = await _userManager.FindByIdAsync(id);
            if (user != null)
            {
                // Set IsActive to false (soft delete)
                user.IsActive = false;
                user.ModifiedBy = loggedinuserId;
                user.ModifiedDate = DateTime.Now;

                // Audit Logging
                var auditLogs = AuditHelper.GenerateAuditLogs(user, null, "User", loggedinuserId.ToString(), "Delete");
                await _context.AuditLog.AddRangeAsync(auditLogs);
                await _context.SaveChangesAsync();

                // Update the user in the database
                await _userManager.UpdateAsync(user);
            }
        }

        public async Task<User> GetUserByEmailAsync(string email)
        {
            return await _userManager.FindByEmailAsync(email);
        }

        public async Task<IdentityResult> AssignRoleToUserAsync(User user, string role)
        {
            if (await _userManager.IsInRoleAsync(user, role))
            {
                // User is already in the role
                return IdentityResult.Success; // Or return a custom result indicating success
            }

            return await _userManager.AddToRoleAsync(user, role);
        }


        public async Task<IdentityResult> RemoveRoleFromUserAsync(User user, string role)
        {
            return await _userManager.RemoveFromRoleAsync(user, role);
        }

        public async Task<bool> CheckPasswordAsync(User user, string password)
        {
            return await _userManager.CheckPasswordAsync(user, password);
        }

        public async Task<IdentityResult> ChangePasswordAsync(User user, string currentPassword, string newPassword)
        {
            return await _userManager.ChangePasswordAsync(user, currentPassword, newPassword);
        }

        public async Task<IdentityResult> ResetPasswordAsync(User user, string token, string newPassword)
        {
            return await _userManager.ResetPasswordAsync(user, token, newPassword);
        }

        public async Task<string> GeneratePasswordResetTokenAsync(User user)
        {
            return await _userManager.GeneratePasswordResetTokenAsync(user);
        }

        public async Task<bool> IsUserInRoleAsync(User user, string role)
        {
            return await _userManager.IsInRoleAsync(user, role);
        }

        public async Task<IList<string>> GetUserRolesAsync(User user)
        {
            return await _userManager.GetRolesAsync(user);
        }

        public async Task<string> GenerateEmailConfirmationTokenAsync(User user)
        {
            return await _userManager.GenerateEmailConfirmationTokenAsync(user);
        }

        public async Task<IdentityResult> ConfirmEmailAsync(User user, string token)
        {
            return await _userManager.ConfirmEmailAsync(user, token);
        }

        public async Task<int> CountUsersAsync()
        {
            return await _userManager.Users.CountAsync();
        }

        // New methods for IsActive field
        public async Task<bool> IsUserActiveAsync(string id)
        {
            var user = await _userManager.FindByIdAsync(id);
            return user?.IsActive ?? false;
        }

        public async Task SetUserActiveStatusAsync(string id, bool isActive)
        {
            var user = await _userManager.FindByIdAsync(id);
            if (user != null)
            {
                user.IsActive = isActive;
                await _userManager.UpdateAsync(user);
            }
        }

        public async Task<PaginatedList<User>> GetPagedUsersAsync(int pageIndex, int pageSize)
        {
            var query = _userManager.Users.Where(user => user.IsActive);
            var totalCount = await query.CountAsync();
            var users = await query
                .Skip((pageIndex - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return new PaginatedList<User>(users, totalCount, pageIndex, pageSize);
        }
    }
}
