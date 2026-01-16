using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.ViewModels
{
    public class VerifyOtpRequest
    {
        public string ContactNumber { get; set; } = null!;
        public string Otp { get; set; } = null!;
    }
}
