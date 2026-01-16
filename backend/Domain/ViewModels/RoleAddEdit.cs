using Domain.Entities;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Domain.ViewModels
{
    public class RoleAddEdit
    {
        public RoleAddEdit()
        {
            Permissions = new List<PermissionAssign>();
        }

        public long Id { get; set; }

        [Required(ErrorMessage = "Name is required")]
        public string Name { get; set; }

        public string NormalizedName { get; set; }

        public string PermissionData { get; set; }

        public List<PermissionAssign> Permissions { get; set; }

        public virtual ICollection<RolePermissionAddEdit> RolePermissions { get; set; }
    }
}
