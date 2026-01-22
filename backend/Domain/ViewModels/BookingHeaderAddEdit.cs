using System;
using Domain.Entities;

namespace Domain.ViewModels
{
    public class BookingHeaderAddEdit : BaseAuditable
    {
        public long Id { get; set; }
        public long AmenityId { get; set; }
        public long SocietyId { get; set; }
        public string? BookingNo { get; set; }
        public DateTime BookingDate { get; set; }
        public string? Remarks { get; set; }
        public string Status { get; set; } = "Draft";
    }
}
