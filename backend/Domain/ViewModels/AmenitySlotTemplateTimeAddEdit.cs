using Domain.Entities;
using System;

namespace Domain.ViewModels
{
    public class AmenitySlotTemplateTimeAddEdit : BaseAuditable
    {
        public long Id { get; set; }
        public long SlotTemplateId { get; set; }
        public TimeSpan StartTime { get; set; }
        public TimeSpan EndTime { get; set; }
        public int? CapacityPerSlot { get; set; }
        public decimal? SlotCharge { get; set; }
        public bool IsActive { get; set; }
    }
}
