using Domain.Entities;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

[Table("adm_Unit")]
public class Unit : BaseAuditable
{
    [Required]
    public string Code { get; set; }

    [Required]
    public string UnitName { get; set; }
    [Required, ForeignKey(nameof(Building))] public long BuildingId { get; set; }
    [Required, ForeignKey(nameof(Floor))] public long FloorId { get; set; }

    // NEW: FK to GroupCode
    [Required, ForeignKey(nameof(OccupancyStatus))]
    public long OccupancyStatusId { get; set; }

    public bool IsActive { get; set; } = true;
    public Building Building { get; set; }
    public Floor Floor { get; set; }
    public GroupCode OccupancyStatus { get; set; }
}
