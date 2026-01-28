using Domain.Entities;
using System;

namespace Domain.ViewModels
{
    public class AmenitySlotTemplateTimeList : BaseAuditable
    {
        public long SlotTemplateId { get; set; }
        public TimeSpan StartTime { get; set; }
        public TimeSpan EndTime { get; set; }
        public int? CapacityPerSlot { get; set; }
        public decimal? SlotCharge { get; set; }
        public bool IsChargeable { get; set; }
        public string? ChargeType { get; set; }
        public decimal? BaseRate { get; set; }
        public decimal? SecurityDeposit { get; set; }
        public bool RefundableDeposit { get; set; }
        public bool TaxApplicable { get; set; }
        public long? TaxCodeId { get; set; }
        public decimal? TaxPercentage { get; set; }
        public bool IsActive { get; set; }
    }
}
