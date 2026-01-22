using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Domain.Entities
{
    [Table("adm_AmenitySlotTemplate")]
    public class AmenitySlotTemplate : BaseAuditable
    {
        [Required, ForeignKey(nameof(AmenityMaster))]
        public long AmenityId { get; set; }

        [Required]
        [MaxLength(10)]
        public string DayOfWeek { get; set; } = default!;

        [Required]
        public TimeSpan StartTime { get; set; }

        [Required]
        public TimeSpan EndTime { get; set; }

        [Required]
        public int SlotDurationMinutes { get; set; }

        public int? BufferTimeMinutes { get; set; }

        public int? CapacityPerSlot { get; set; }

        public bool IsActive { get; set; } = true;

        public AmenityMaster AmenityMaster { get; set; } = default!;
    }
}
