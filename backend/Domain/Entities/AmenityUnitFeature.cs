using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Domain.Entities
{
    [Table("adm_AmenityUnitFeature")]
    public class AmenityUnitFeature : BaseAuditable
    {
        [Required, ForeignKey(nameof(AmenityUnit))]
        public long AmenityUnitId { get; set; }

        public long? FeatureId { get; set; }

        [Required]
        [MaxLength(200)]
        public string FeatureName { get; set; } = default!;

        public bool IsActive { get; set; } = true;

        public AmenityUnit AmenityUnit { get; set; } = default!;
    }
}
