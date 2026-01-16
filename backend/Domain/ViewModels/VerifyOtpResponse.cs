using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.ViewModels
{
    public class VerifyOtpResponse : LoginResponse
    {
        public bool IsLogin { get; set; }
        public string? UserType { get; set; }
    }
}
