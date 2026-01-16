using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Domain.Entities
{
    [Table("adm_Floor")]
    public class Floor : BaseAuditable
    {
        [Required]
        public string Code { get; set; }

        [Required]
        public string FloorName { get; set; }

        [Required, ForeignKey(nameof(Building))]
        public long BuildingId { get; set; }
        public bool IsActive { get; set; } = true;
        public Building Building { get; set; }
        public ICollection<Unit> Units { get; set; }
    }
}
