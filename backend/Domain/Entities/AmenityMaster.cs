using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Domain.Entities
{
    [Table("adm_AmenityMaster")]
    public class AmenityMaster : BaseAuditable
    {
        [Required]
        [MaxLength(200)]
        public string Name { get; set; } = default!;

        [MaxLength(50)]
        public string? Code { get; set; }

        [Required]
        [MaxLength(50)]
        public string Type { get; set; } = default!;

        [MaxLength(500)]
        public string? Description { get; set; }

        [Required, ForeignKey(nameof(Building))]
        public long BuildingId { get; set; }

        [Required, ForeignKey(nameof(Floor))]
        public long FloorId { get; set; }

        [MaxLength(500)]
        public string? Location { get; set; }

        [Required]
        [MaxLength(50)]
        public string Status { get; set; } = "Active";

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

        [MaxLength(50)]
        public string? AvailableDays { get; set; }

        public TimeSpan? OpenTime { get; set; }

        public TimeSpan? CloseTime { get; set; }

        public bool HolidayBlocked { get; set; }

        [MaxLength(500)]
        public string? MaintenanceSchedule { get; set; }

        public bool IsChargeable { get; set; }

        [MaxLength(20)]
        public string? ChargeType { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal? BaseRate { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal? SecurityDeposit { get; set; }

        public bool RefundableDeposit { get; set; }

        public bool TaxApplicable { get; set; }

        public long? TaxCodeId { get; set; }

        [Column(TypeName = "decimal(5,2)")]
        public decimal? TaxPercentage { get; set; }

        public string? TermsAndConditions { get; set; }

        public ICollection<AmenityDocument> Documents { get; set; } = new List<AmenityDocument>();

        public Building Building { get; set; } = default!;
        public Floor Floor { get; set; } = default!;
    }
}
