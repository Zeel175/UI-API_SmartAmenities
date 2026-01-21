using Domain.Entities;

namespace Domain.ViewModels
{
    public class AmenityMasterAddEdit : BaseAuditable
    {
        public long Id { get; set; }
        public string Name { get; set; }
        public string Type { get; set; }
        public string? Description { get; set; }
        public long BuildingId { get; set; }
        public long FloorId { get; set; }
        public string? Location { get; set; }
        public string Status { get; set; }
    }
}
