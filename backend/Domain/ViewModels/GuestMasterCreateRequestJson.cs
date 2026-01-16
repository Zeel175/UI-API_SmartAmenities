using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.ViewModels
{
    public class GuestMasterCreateRequestJson
    {
        [Required, MaxLength(200)]
        public string FirstName { get; set; } = default!;

        [MaxLength(200)]
        public string? LastName { get; set; }

        [MaxLength(200)]
        public string? Email { get; set; }

        [MaxLength(50)]
        public string? Mobile { get; set; }
        [MaxLength(10)]
        public string? CountryCode { get; set; }
        [Required]
        public long UnitId { get; set; }

        [MaxLength(200)]
        public string? CardId { get; set; }
        public string? QrId { get; set; }
        public bool IsActive { get; set; }

        public DateTime? FromDateTime { get; set; }
        public DateTime? ToDateTime { get; set; }
    }
}
