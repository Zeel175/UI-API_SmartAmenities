using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.ViewModels
{
    public class PermissionAssign 
    {
        public string Code { get; set; }
        public string Name { get; set; }
        public int DisplayOrder { get; set; }
        public bool IsList { get; set; }
        public bool IsAdd { get; set; }
        public bool IsEdit { get; set; }
        public bool IsDelete { get; set; }
        public bool? IsExport { get; set; }
        public bool IsAdmin { get; set; }
    }
}
