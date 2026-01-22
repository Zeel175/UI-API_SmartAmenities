using Domain.Entities;
using System;

namespace Domain.ViewModels
{
    public class AmenitySlotTemplateList : BaseAuditable
    {
        public long AmenityId { get; set; }
        public string AmenityName { get; set; }
        public string DayOfWeek { get; set; }
        public TimeSpan StartTime { get; set; }
        public TimeSpan EndTime { get; set; }
        public int SlotDurationMinutes { get; set; }
        public int? BufferTimeMinutes { get; set; }
        public int? CapacityPerSlot { get; set; }
        public bool IsActive { get; set; }
    }
}
