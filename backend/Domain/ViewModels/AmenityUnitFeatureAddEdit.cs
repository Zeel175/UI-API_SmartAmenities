namespace Domain.ViewModels
{
    public class AmenityUnitFeatureAddEdit
    {
        public long? FeatureId { get; set; }
        public string FeatureName { get; set; } = string.Empty;
        public bool IsActive { get; set; } = true;
    }
}
