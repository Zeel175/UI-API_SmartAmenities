using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.ViewModels
{
    public class PermissionTypeStruct
    {
        public struct PermissionTypeConstant
        {
            public const long List = 74;
            public const long Add = 72;
            public const long Edit = 73;
            public const long Delete = 75;
            public const long Export = 76;
        }
    }
}
