using Domain.Entities.Domain.Entities;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Entities
{
    public class ResidentUserMap : BaseAuditable
    {
        [ForeignKey(nameof(ResidentMaster))]
        public long? ResidentMasterId { get; set; }
        public ResidentMaster ResidentMaster { get; set; }

        [ForeignKey(nameof(ResidentFamilyMember))]
        public long? ResidentFamilyMemberId { get; set; }
        public ResidentFamilyMember ResidentFamilyMember { get; set; }

        [ForeignKey(nameof(User))]
        public long UserId { get; set; }
        public User User { get; set; }

        public bool IsActive { get; set; }
    }
}
