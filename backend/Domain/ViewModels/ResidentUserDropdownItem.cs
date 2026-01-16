using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.ViewModels
{
    public class ResidentUserDropdownItem
    {
        public long Id { get; set; }
        public string Name { get; set; }
        public string Type { get; set; }
        public long? ResidentMasterId { get; set; }
        public long? ResidentFamilyMemberId { get; set; }
    }
}
