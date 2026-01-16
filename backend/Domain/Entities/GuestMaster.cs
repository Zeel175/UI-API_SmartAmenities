using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Entities
{
    public class GuestMaster : BaseAuditable
    {
        [Required]
        [MaxLength(50)]
        public string Code { get; set; } = default!;

        [Required]
        [MaxLength(200)]
        public string FirstName { get; set; } = default!;

        // ✅ Now nullable (DB allows NULL)
        [MaxLength(200)]
        public string? LastName { get; set; }

        [MaxLength(200)]
        public string? Email { get; set; }

        // ✅ Now NOT NULL in DB, so keep required + default
        [Required]
        [MaxLength(50)]
        public string Mobile { get; set; } = string.Empty;

        [MaxLength(10)]
        public string? CountryCode { get; set; }

        [Required, ForeignKey(nameof(Unit))]
        public long UnitId { get; set; }

        public Unit Unit { get; set; } = default!;

        [MaxLength(20)]
        public string? CardId { get; set; }

        [MaxLength(200)]
        public string? QrId { get; set; }

        public string? QrCode { get; set; }

        public bool IsActive { get; set; }

        public DateTime? FromDateTime { get; set; }
        public DateTime? ToDateTime { get; set; }

        public string? QrCodeValue { get; set; }
        public string? QrCodeImagePath { get; set; }
    }

}
