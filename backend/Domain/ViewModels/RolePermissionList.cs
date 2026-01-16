using System;

namespace Domain.ViewModels
{
    public class RolePermissionList
    {
        public long Id { get; set; }
        public long RoleId { get; set; }
        public string RoleName { get; set; } // Assuming you want to include role name
        public long PermissionId { get; set; }
        public string PermissionCode { get; set; }
        public string PermissionName { get; set; }
        public bool HasMasterAccess { get; set; }
    }
}

