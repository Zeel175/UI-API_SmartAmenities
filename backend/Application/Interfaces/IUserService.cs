using Domain.Entities; 
using Microsoft.AspNetCore.Identity;
using System.Collections.Generic;
using System.Threading.Tasks;
using Domain.ViewModels;

namespace Application.Interfaces
{
    public interface IUserService
    {
        Task<User> GetUserByIdAsync(string id);
        Task<User> GetUserById(long id);// Updated to User
        Task<IEnumerable<User>> GetAllUsersAsync();

        Task<IdentityResult> CreateUserAsync(UserAddEdit userAddEdit);// Updated to User
        Task UpdateUserAsync(User user);  // Updated to User
        Task DeleteUserAsync(string id);

        Task<PaginatedList<User>> GetPagedUsersAsync(int pageIndex, int pageSize);

        Task<User> GetUserByEmailAsync(string email);  // Updated to User
        Task<IdentityResult> AssignRoleToUserAsync(User user, string role);  // Updated to User
        Task<IdentityResult> RemoveRoleFromUserAsync(User user, string role);  // Updated to User
        Task<bool> CheckPasswordAsync(User user, string password);  // Updated to User
        Task<IdentityResult> ChangePasswordAsync(User user, string currentPassword, string newPassword);  // Updated to User
        Task<IdentityResult> ResetPasswordAsync(User user, string token, string newPassword);  // Updated to User
        Task<string> GeneratePasswordResetTokenAsync(User user);  // Updated to User
        Task<bool> IsUserInRoleAsync(User user, string role);  // Updated to User
        Task<IList<string>> GetUserRolesAsync(User user);  // Updated to User
        Task<string> GenerateEmailConfirmationTokenAsync(User user);  // Updated to User
        Task<IdentityResult> ConfirmEmailAsync(User user, string token);  // Updated to User
        Task<int> CountUsersAsync();

        Task<bool> IsUserActiveAsync(string id);  // Existing
        Task SetUserActiveStatusAsync(string id, bool isActive);  // Existing

    }
}
