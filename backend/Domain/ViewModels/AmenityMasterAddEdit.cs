using System;
using System.Collections.Generic;
using Domain.Entities;
using Microsoft.AspNetCore.Http;

namespace Domain.ViewModels
{
    public class AmenityMasterAddEdit : BaseAuditable
    {
        public long Id { get; set; }
        public string Name { get; set; }
        public string Type { get; set; }
        public string? Description { get; set; }
        public long BuildingId { get; set; }
        public long FloorId { get; set; }
        public string? Location { get; set; }
        public string Status { get; set; }
        public int? MaxCapacity { get; set; }
        public int? MaxBookingsPerDayPerFlat { get; set; }
        public int? MaxActiveBookingsPerFlat { get; set; }
        public int? MinAdvanceBookingHours { get; set; }
        public int? MinAdvanceBookingDays { get; set; }
        public int? MaxAdvanceBookingDays { get; set; }
        public bool BookingSlotRequired { get; set; }
        public int? SlotDurationMinutes { get; set; }
        public int? BufferTimeMinutes { get; set; }
        public bool AllowMultipleSlotsPerBooking { get; set; }
        public bool RequiresApproval { get; set; }
        public bool AllowGuests { get; set; }
        public int? MaxGuestsAllowed { get; set; }
        public string? AvailableDays { get; set; }
        public TimeSpan? OpenTime { get; set; }
        public TimeSpan? CloseTime { get; set; }
        public bool HolidayBlocked { get; set; }
        public string? MaintenanceSchedule { get; set; }
        public bool IsChargeable { get; set; }
        public string? ChargeType { get; set; }
        public decimal? BaseRate { get; set; }
        public decimal? SecurityDeposit { get; set; }
        public bool RefundableDeposit { get; set; }
        public bool TaxApplicable { get; set; }
        public long? TaxCodeId { get; set; }
        public decimal? TaxPercentage { get; set; }
        public string? TermsAndConditions { get; set; }
        public List<AmenityDocumentDto> DocumentDetails { get; set; } = new();
        public List<IFormFile>? Documents { get; set; }
    }
}
