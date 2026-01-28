using System;

namespace Domain.ViewModels
{
    public class BookingSlotAvailability
    {
        public DateTime SlotStartDateTime { get; set; }
        public DateTime SlotEndDateTime { get; set; }
        public int CapacityPerSlot { get; set; }
        public int AvailableCapacity { get; set; }
        public decimal? SlotCharge { get; set; }
        public bool IsChargeable { get; set; }
        public string? ChargeType { get; set; }
        public decimal? BaseRate { get; set; }
        public decimal? SecurityDeposit { get; set; }
        public bool RefundableDeposit { get; set; }
        public bool TaxApplicable { get; set; }
        public long? TaxCodeId { get; set; }
        public decimal? TaxPercentage { get; set; }
    }
}
