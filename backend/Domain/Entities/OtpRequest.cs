using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Entities
{
    public class OtpRequest : BaseAuditable  // use your existing auditable base
    {
       // public long Id { get; set; }

        public string ContactNumber { get; set; } = null!;

        // For now store OTP directly as you requested
        public string Otp { get; set; } = null!;

        // Recommended fields (useful later)
        public DateTime ExpiresAt { get; set; }
        public bool IsUsed { get; set; } = false;
    }
}
