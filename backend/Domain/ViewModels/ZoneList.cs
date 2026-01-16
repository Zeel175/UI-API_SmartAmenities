using Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.ViewModels
{
    public class ZoneList : BaseAuditable
    {
        public string Code { get; set; }
        public string ZoneName { get; set; }
        public string Description { get; set; }
        public long? BuildingId { get; set; }
        public string BuildingName { get; set; }
        public bool IsActive { get; set; }
    }
}
