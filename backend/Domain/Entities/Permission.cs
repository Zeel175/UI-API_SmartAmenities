using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace Domain.Entities
{
    [Table("adm_Permission")]
    public class Permission : BaseAuditable
    {
        public Permission()
        {
            RolePermissions = new List<RolePermissionMap>();
        }

        //public int Id { get; set; }               // Unique identifier for the permission
        public string Code { get; set; }          // Permission code (similar to old entity)
        public string Name { get; set; }          // Name of the permission
        public bool IsDefault { get; set; }       // Whether this permission is default
        public long PermissionTypeId { get; set; } 
        public int DisplayOrder { get; set; }     // Order in which permissions are displayed
        public string Controller { get; set; }    // Controller associated with the permission
        public string ActionName { get; set; }    // Action within the controller
        public bool IsActive { get; set; } = true;  // Status of the permission

        // Relationships
        public virtual ICollection<RolePermissionMap> RolePermissions { get; set; }  // Role-Permission mapping
        /*public virtual MastersEntity PermissionTypeMasters { get; set; }*/      // Link to permission type master
    }
}
