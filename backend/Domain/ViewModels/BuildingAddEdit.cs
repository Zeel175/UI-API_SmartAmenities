using Domain.Entities;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.ViewModels
{
    public class BuildingAddEdit : BaseAuditable
    {

        [Required]
        public string Code { get; set; }
        [Required]
        public string BuildingName { get; set; }
        public long PropertyId { get; set; }
        public int? DeviceId { get; set; }
        public string? DeviceUserName { get; set; }
        public string? DevicePassword { get; set; }
        public bool IsActive { get; set; }
    }
}
