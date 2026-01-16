using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Entities
{
    public class ResidentProfilePhoto : BaseAuditable
    {
        [Required]
        [MaxLength(255)]
        public string FileName { get; set; }

        [Required]
        [MaxLength(500)]
        public string FilePath { get; set; }

        [MaxLength(100)]
        public string ContentType { get; set; }

        public bool IsActive { get; set; }
    }
}
