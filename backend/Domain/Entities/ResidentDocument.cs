using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Entities
{
    public class ResidentDocument : BaseAuditable
    {
        public long? ResidentMasterId { get; set; }

        [ForeignKey(nameof(ResidentMasterId))]
        public ResidentMaster ResidentMaster { get; set; }

        [Required]
        [MaxLength(255)]
        public string FileName { get; set; }   // original name

        [Required]
        [MaxLength(500)]
        public string FilePath { get; set; }   // /uploads/residents/docs/xxx.pdf

        [MaxLength(100)]
        public string ContentType { get; set; } // application/pdf, image/png

        public bool IsActive { get; set; }
    }
}
