using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Domain.Entities
{
    [Table("adm_AmenityUnitMaster")]
    public class AmenityUnit : BaseAuditable
    {
        [Required, ForeignKey(nameof(AmenityMaster))]
        public long AmenityId { get; set; }

        [Required]
        [MaxLength(200)]
        public string UnitName { get; set; } = default!;

        [MaxLength(50)]
        public string? UnitCode { get; set; }

        [ForeignKey(nameof(Device))]
        public int? DeviceId { get; set; }

        [MaxLength(100)]
        public string? DeviceUserName { get; set; }

        [MaxLength(100)]
        public string? DevicePassword { get; set; }

        [Required]
        [MaxLength(50)]
        public string Status { get; set; } = "Active";

        [MaxLength(200)]
        public string? ShortDescription { get; set; }

        public string? LongDescription { get; set; }

        public bool IsChargeable { get; set; }

        [MaxLength(20)]
        public string? ChargeType { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal? BaseRate { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal? SecurityDeposit { get; set; }

        public bool RefundableDeposit { get; set; }

        public bool TaxApplicable { get; set; }

        public long? TaxCodeId { get; set; }

        [Column(TypeName = "decimal(5,2)")]
        public decimal? TaxPercentage { get; set; }

        public AmenityMaster AmenityMaster { get; set; } = default!;
        public HikDevice? Device { get; set; }

        public ICollection<AmenityUnitFeature> Features { get; set; } = new List<AmenityUnitFeature>();
    }
}
