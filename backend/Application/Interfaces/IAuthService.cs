using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Interfaces
{
    public interface IAuthService
    {
        Task<LoginResponse> LoginAsync(string username, string password);
        Task<LoginResponse> LoginWithIdentifierAsync(string identifier, string password);
        Task LogoutAsync();
        Task<IdentityResult> ChangePasswordAsync(string userId, string currentPassword, string newPassword);
        Task<IdentityResult> ResetPasswordAsync(string userId, string token, string newPassword);
    }
}
