using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;

namespace Domain.Entities
{
    public class Role : IdentityRole<long> // Note the <long> here
    {
        public Role()
        {
            CreatedOn = DateTime.Now;
            ModifiedOn = DateTime.Now;
            IsActive = true;

            RolePermissions = new List<RolePermissionMap>();
        }

        public bool IsActive { get; set; }
        public DateTime CreatedOn { get; set; }
        public long? CreatedBy { get; set; }
        public DateTime? ModifiedOn { get; set; }
        public long? ModifiedBy { get; set; }

        public virtual ICollection<RolePermissionMap> RolePermissions { get; set; }
    }
}
