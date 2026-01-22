using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Domain.Entities
{
    public class AmenityDocument : BaseAuditable
    {
        public long? AmenityMasterId { get; set; }

        [ForeignKey(nameof(AmenityMasterId))]
        public AmenityMaster AmenityMaster { get; set; }

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
