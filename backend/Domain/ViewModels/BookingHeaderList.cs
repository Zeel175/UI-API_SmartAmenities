using System;
using Domain.Entities;

namespace Domain.ViewModels
{
    public class BookingHeaderList : BaseAuditable
    {
        public long AmenityId { get; set; }
        public string AmenityName { get; set; }
        public long SocietyId { get; set; }
        public string SocietyName { get; set; }
        public string BookingNo { get; set; }
        public DateTime BookingDate { get; set; }
        public string? Remarks { get; set; }
        public string Status { get; set; }
        public string? ResidentNameSnapshot { get; set; }
        public string? ContactNumberSnapshot { get; set; }
        public decimal? TotalPayable { get; set; }
    }
}
