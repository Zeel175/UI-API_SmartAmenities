using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Domain.Entities
{
    [Table("adm_BookingSlot")]
    public class BookingSlot : BaseAuditable
    {
        [Required, ForeignKey(nameof(BookingHeader))]
        public long BookingId { get; set; }

        [Required, ForeignKey(nameof(BookingUnit))]
        public long BookingUnitId { get; set; }

        [Required, ForeignKey(nameof(AmenityMaster))]
        public long AmenityId { get; set; }

        [Required, ForeignKey(nameof(AmenityUnit))]
        public long AmenityUnitId { get; set; }

        [Required]
        public DateTime SlotStartDateTime { get; set; }

        [Required]
        public DateTime SlotEndDateTime { get; set; }

        [Required]
        [MaxLength(20)]
        public string SlotStatus { get; set; } = "Reserved";

        public bool CheckInRequired { get; set; }

        public DateTime? CheckInTime { get; set; }

        public DateTime? CheckOutTime { get; set; }

        public BookingHeader BookingHeader { get; set; } = default!;

        public BookingUnit BookingUnit { get; set; } = default!;

        public AmenityMaster AmenityMaster { get; set; } = default!;

        public AmenityUnit AmenityUnit { get; set; } = default!;
    }
}
