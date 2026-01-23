using Domain.Entities;
using System;
using System.Collections.Generic;

namespace Domain.ViewModels
{
    public class AmenitySlotTemplateAddEdit : BaseAuditable
    {
        public long Id { get; set; }
        public long AmenityId { get; set; }
        public string DayOfWeek { get; set; }
        public int SlotDurationMinutes { get; set; }
        public int? BufferTimeMinutes { get; set; }
        public bool IsActive { get; set; }
        public List<AmenitySlotTemplateTimeAddEdit> SlotTimes { get; set; } = new();
    }
}
