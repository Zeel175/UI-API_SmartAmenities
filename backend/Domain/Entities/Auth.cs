using Microsoft.AspNetCore.Identity;
using System;
using System.ComponentModel.DataAnnotations;

namespace Domain.Entities
{
    // Data Transfer Objects
    public class LoginDto
    {
        public string Username { get; set; }
        public string Password { get; set; }
    }

    public class ChangePasswordDto
    {
        public string CurrentPassword { get; set; }
        public string NewPassword { get; set; }
    }

    public class ResetPasswordDto
    {
        public string Token { get; set; }
        public string NewPassword { get; set; }
    }
    public class LoginWithIdentifierDto
    {
        public string Identifier { get; set; }
        public string Password { get; set; }
    }
    // Identity User Class
    public class User : IdentityUser<long>
    {
        public string Name { get; set; }
        public bool IsActive { get; set; } = true;
        public bool IsGuest { get; set; }
        public DateTime CreatedDate { get; set; } = DateTime.Now;
        public long? CreatedBy { get; set; }
        public DateTime? ModifiedDate { get; set; }
        public long? ModifiedBy { get; set; }

        // No need for additional constructors unless required for specific logic
        // The base class IdentityUser<long> provides Id and other properties
    }
}
