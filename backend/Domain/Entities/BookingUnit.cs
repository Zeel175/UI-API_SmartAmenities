using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Domain.Entities
{
    [Table("adm_BookingUnit")]
    public class BookingUnit : BaseAuditable
    {
        [Required, ForeignKey(nameof(BookingHeader))]
        public long BookingId { get; set; }

        [Required, ForeignKey(nameof(AmenityUnit))]
        public long AmenityUnitId { get; set; }

        [MaxLength(200)]
        public string? UnitNameSnapshot { get; set; }

        public int? CapacityReserved { get; set; }

        public BookingHeader BookingHeader { get; set; } = default!;
        public AmenityUnit AmenityUnit { get; set; } = default!;
        public ICollection<BookingSlot> BookingSlots { get; set; } = new List<BookingSlot>();
    }
}
