using Domain.Entities;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.ViewModels
{
    public class UnitList : BaseAuditable
    {
        public string Code { get; set; }
        public string UnitName { get; set; }
        public long BuildingId { get; set; }
        public string BuildingName { get; set; }
        public long FloorId { get; set; }
        public string FloorName { get; set; }
        public long OccupancyStatusId { get; set; }
        public string OccupancyStatusName { get; set; }
        public bool IsActive { get; set; } = true;
    }
}
