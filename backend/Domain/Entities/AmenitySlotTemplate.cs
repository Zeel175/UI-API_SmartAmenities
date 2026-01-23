using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Collections.Generic;

namespace Domain.Entities
{
    [Table("adm_AmenitySlotTemplate")]
    public class AmenitySlotTemplate : BaseAuditable
    {
        [Required, ForeignKey(nameof(AmenityMaster))]
        public long AmenityId { get; set; }

        [ForeignKey(nameof(AmenityUnit))]
        public long? AmenityUnitId { get; set; }

        [Required]
        [MaxLength(10)]
        public string DayOfWeek { get; set; } = default!;

        [Required]
        public int SlotDurationMinutes { get; set; }

        public int? BufferTimeMinutes { get; set; }

        public TimeSpan? StartTime { get; set; }

        public TimeSpan? EndTime { get; set; }

        public int? CapacityPerSlot { get; set; }

        public bool IsActive { get; set; } = true;

        public AmenityMaster AmenityMaster { get; set; } = default!;

        public AmenityUnit? AmenityUnit { get; set; }

        public ICollection<AmenitySlotTemplateTime> SlotTimes { get; set; } = new List<AmenitySlotTemplateTime>();
    }
}
