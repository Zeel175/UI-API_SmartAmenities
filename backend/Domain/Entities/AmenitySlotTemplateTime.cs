using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Domain.Entities
{
    [Table("adm_AmenitySlotTemplateTime")]
    public class AmenitySlotTemplateTime : BaseAuditable
    {
        [Required, ForeignKey(nameof(AmenitySlotTemplate))]
        public long SlotTemplateId { get; set; }

        [Required]
        public TimeSpan StartTime { get; set; }

        [Required]
        public TimeSpan EndTime { get; set; }

        public int? CapacityPerSlot { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal? SlotCharge { get; set; }

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

        public bool IsActive { get; set; } = true;

        public AmenitySlotTemplate AmenitySlotTemplate { get; set; } = default!;
    }
}
