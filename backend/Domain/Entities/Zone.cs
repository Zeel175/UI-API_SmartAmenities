using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Entities
{
    [Table("adm_Zone")]
    public class Zone : BaseAuditable
    {
        [Required]
        public string Code { get; set; }

        [Required]
        public string ZoneName { get; set; }

        public string Description { get; set; }

        [ForeignKey(nameof(Building))]
        public long? BuildingId { get; set; }

        public bool IsActive { get; set; } = true;

        public Building? Building { get; set; }
    }
}
