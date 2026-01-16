using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.ViewModels
{
    public class QrResolveRequest
    {
        public string Value { get; set; }
    }

    public class QrResolveResponse
    {
        public bool IsSuccess { get; set; }
        public string Message { get; set; }
        public string Type { get; set; } // Resident | FamilyMember | Guest
        public long? ResidentId { get; set; }
        public long? FamilyMemberId { get; set; }
        public long? GuestId { get; set; }
        public string QrId { get; set; }
    }
}
