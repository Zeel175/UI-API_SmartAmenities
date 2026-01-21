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

        public Building Building { get; set; } = default!;
        public Floor Floor { get; set; } = default!;
    }
}
