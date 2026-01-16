using Domain.Entities;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.ViewModels
{
    public class UnitAddEdit : BaseAuditable
    {
        [Required]
        public string Code { get; set; }

        [Required]
        public string UnitName { get; set; }
        public long BuildingId { get; set; }
        public long FloorId { get; set; }
        public long OccupancyStatusId { get; set; }

        public bool IsActive { get; set; } = true;
    }
}
