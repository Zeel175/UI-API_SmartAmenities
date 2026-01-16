using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Drawing;

namespace Domain.Entities
{
    [Table("adm_Building")]
    public class Building : BaseAuditable
    {

        [Required]
        public string Code { get; set; }

        [Required]
        public string BuildingName { get; set; }

        [Required, ForeignKey(nameof(Property))]
        public long PropertyId { get; set; }
        [ForeignKey(nameof(Device))]
        public int? DeviceId { get; set; }
        public string? DeviceUserName { get; set; }
        public string? DevicePassword { get; set; }
        public bool IsActive { get; set; } = true;

        // Navigation
        public Property Property { get; set; }
        public HikDevice? Device { get; set; }
        public ICollection<Floor> Floors { get; set; }
        public ICollection<Zone> Zones { get; set; }
        public ICollection<Unit> Units
        {
            get; set;
        }
    }
}
