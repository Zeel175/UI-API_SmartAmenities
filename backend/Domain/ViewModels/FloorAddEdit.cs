using Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.ViewModels
{
    public class FloorAddEdit : BaseAuditable
    {
        public long Id { get; set; }
        public string Code { get; set; }
        public string FloorName { get; set; }
        public long BuildingId { get; set; }
        public bool IsActive { get; set; }
    }
}
