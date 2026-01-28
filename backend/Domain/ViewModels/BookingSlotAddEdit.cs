using System;

namespace Domain.ViewModels
{
    public class BookingSlotAddEdit
    {
        public long Id { get; set; }
        public long BookingId { get; set; }
        public long BookingUnitId { get; set; }
        public long AmenityId { get; set; }
        public long AmenityUnitId { get; set; }
        public DateTime SlotStartDateTime { get; set; }
        public DateTime SlotEndDateTime { get; set; }
        public string SlotStatus { get; set; } = "Reserved";
        public bool CheckInRequired { get; set; }
        public DateTime? CheckInTime { get; set; }
        public DateTime? CheckOutTime { get; set; }
    }
}
