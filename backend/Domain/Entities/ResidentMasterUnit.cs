using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Entities
{
    public class ResidentMasterUnit : BaseAuditable
    {
        [ForeignKey(nameof(ResidentMaster))]
        public long ResidentMasterId { get; set; }
        public ResidentMaster ResidentMaster { get; set; }

        [ForeignKey(nameof(Unit))]
        public long UnitId { get; set; }
        public Unit Unit { get; set; }

        public bool IsActive { get; set; }
    }
}
