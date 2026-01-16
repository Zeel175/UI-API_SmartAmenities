using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.ViewModels
{
    public class GenerateOtpResponse
    {
        public bool Success { get; set; }
        public string Message { get; set; } = "";
        public bool IsLogin { get; set; }
        public string ContactNumber { get; set; } = "";

        // Since no SMS service now, returning OTP helps testing
        public string? Otp { get; set; }
    }
}
