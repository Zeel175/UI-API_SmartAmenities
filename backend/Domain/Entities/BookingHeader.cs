using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Domain.Entities
{
    [Table("adm_BookingHeader")]
    public class BookingHeader : BaseAuditable
    {
        [Required, ForeignKey(nameof(AmenityMaster))]
        public long AmenityId { get; set; }

        [Required, ForeignKey(nameof(Society))]
        public long SocietyId { get; set; }

        [Required]
        [MaxLength(50)]
        public string BookingNo { get; set; } = default!;

        [Required]
        public DateTime BookingDate { get; set; }

        [MaxLength(500)]
        public string? Remarks { get; set; }

        [Required]
        [MaxLength(50)]
        public string Status { get; set; } = "Draft";

        public long? ResidentUserId { get; set; }

        public long? FlatId { get; set; }

        [MaxLength(200)]
        public string? ResidentNameSnapshot { get; set; }

        [MaxLength(30)]
        public string? ContactNumberSnapshot { get; set; }

        public bool? IsChargeableSnapshot { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal? AmountBeforeTax { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal? TaxAmount { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal? DepositAmount { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal? DiscountAmount { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal? ConvenienceFee { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal? TotalPayable { get; set; }

        public bool? RequiresApprovalSnapshot { get; set; }

        public long? ApprovedBy { get; set; }

        public DateTime? ApprovedOn { get; set; }

        [MaxLength(500)]
        public string? RejectionReason { get; set; }

        public long? CancelledBy { get; set; }

        public DateTime? CancelledOn { get; set; }

        [MaxLength(500)]
        public string? CancellationReason { get; set; }

        [MaxLength(50)]
        public string? RefundStatus { get; set; }

        public AmenityMaster AmenityMaster { get; set; } = default!;
        public Property Society { get; set; } = default!;
        public ICollection<BookingUnit> BookingUnits { get; set; } = new List<BookingUnit>();
    }
}
