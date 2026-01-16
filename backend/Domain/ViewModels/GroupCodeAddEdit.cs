using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.ViewModels
{
    public class GroupCodeAddEdit 
    {
        public int Id { get; set; }
        public string Code { get; set; }
        [Required(ErrorMessage = "Name is required")]
        public string Name { get; set; }
        [Required(ErrorMessage = "Group Name is required")]
        public string GroupName { get; set; }
        [Required(ErrorMessage = "Priority is required")]
        public int Priority { get; set; }
        public string Value { get; set; }
    }
}
