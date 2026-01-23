using Domain.Entities;
using System;
using System.Collections.Generic;

namespace Domain.ViewModels
{
    public class AmenitySlotTemplateList : BaseAuditable
    {
        public long AmenityId { get; set; }
        public long? AmenityUnitId { get; set; }
        public string AmenityName { get; set; }
        public string DayOfWeek { get; set; }
        public int SlotDurationMinutes { get; set; }
        public int? BufferTimeMinutes { get; set; }
        public bool IsActive { get; set; }
        public List<AmenitySlotTemplateTimeList> SlotTimes { get; set; } = new();
    }
}
