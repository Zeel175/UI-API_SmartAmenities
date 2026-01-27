using System;
using Domain.Entities;

namespace Domain.ViewModels
{
    public class BookingHeaderAddEdit : BaseAuditable
    {
        public long Id { get; set; }
        public long AmenityId { get; set; }
        public long SocietyId { get; set; }
        public string? BookingNo { get; set; }
        public DateTime BookingDate { get; set; }
        public string? Remarks { get; set; }
        public string Status { get; set; } = "Draft";
        public long? ResidentUserId { get; set; }
        public long? FlatId { get; set; }
        public string? ResidentNameSnapshot { get; set; }
        public string? ContactNumberSnapshot { get; set; }
        public bool? IsChargeableSnapshot { get; set; }
        public decimal? AmountBeforeTax { get; set; }
        public decimal? TaxAmount { get; set; }
        public decimal? DepositAmount { get; set; }
        public decimal? DiscountAmount { get; set; }
        public decimal? ConvenienceFee { get; set; }
        public decimal? TotalPayable { get; set; }
        public bool? RequiresApprovalSnapshot { get; set; }
        public long? ApprovedBy { get; set; }
        public DateTime? ApprovedOn { get; set; }
        public string? RejectionReason { get; set; }
        public long? CancelledBy { get; set; }
        public DateTime? CancelledOn { get; set; }
        public string? CancellationReason { get; set; }
        public string? RefundStatus { get; set; }
    }
}
