using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Domain.Entities
{
    [Table("adm_BookingHeader")]
    public class BookingHeader : BaseAuditable
    {
        [Required, ForeignKey(nameof(AmenityMaster))]
        public long AmenityId { get; set; }

        [Required, ForeignKey(nameof(Society))]
        public long SocietyId { get; set; }

        [Required]
        [MaxLength(50)]
        public string BookingNo { get; set; } = default!;

        [Required]
        public DateTime BookingDate { get; set; }

        [MaxLength(500)]
        public string? Remarks { get; set; }

        [Required]
        [MaxLength(50)]
        public string Status { get; set; } = "Draft";

        public AmenityMaster AmenityMaster { get; set; } = default!;
        public Property Society { get; set; } = default!;
    }
}
