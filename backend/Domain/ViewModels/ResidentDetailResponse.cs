using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.ViewModels
{
    public class ResidentDetailResponse
    {
        public string Type { get; set; }
        public ResidentMasterAddEdit Resident { get; set; }
        public ResidentFamilyMemberAddEdit FamilyMember { get; set; }
    }
}
