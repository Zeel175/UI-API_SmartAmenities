namespace Domain.ViewModels
{
    public class BookingUnitAddEdit
    {
        public long Id { get; set; }
        public long BookingId { get; set; }
        public long AmenityUnitId { get; set; }
        public string? UnitNameSnapshot { get; set; }
        public int? CapacityReserved { get; set; }
        public BookingSlotAddEdit? BookingSlot { get; set; }
    }
}
