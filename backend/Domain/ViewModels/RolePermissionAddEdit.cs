using System;

namespace Domain.ViewModels
{
    public class RolePermissionAddEdit
    {
        public long Id { get; set; }                // Unique identifier for the role-permission mapping
        public long RoleId { get; set; }            // Foreign key for the Role
        public long PermissionId { get; set; }      // Foreign key for the Permission
        public bool HasMasterAccess { get; set; }   // Indicates if the role has master access
        public string ActionName { get; set; }    // Added to match old model
    }
}
