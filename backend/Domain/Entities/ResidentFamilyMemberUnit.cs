using Domain.Entities.Domain.Entities;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Entities
{
    public class ResidentFamilyMemberUnit : BaseAuditable
    {
        [ForeignKey(nameof(ResidentFamilyMember))]
        public long ResidentFamilyMemberId { get; set; }
        public ResidentFamilyMember ResidentFamilyMember { get; set; }

        [ForeignKey(nameof(Unit))]
        public long UnitId { get; set; }
        public Unit Unit { get; set; }

        public bool IsActive { get; set; }
    }
}
