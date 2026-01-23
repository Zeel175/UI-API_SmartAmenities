using System.Collections.Generic;
using Domain.Entities;

namespace Domain.ViewModels
{
    public class AmenityUnitAddEdit : BaseAuditable
    {
        public long Id { get; set; }
        public long AmenityId { get; set; }
        public string UnitName { get; set; } = string.Empty;
        public string? UnitCode { get; set; }
        public string Status { get; set; } = "Active";
        public string? ShortDescription { get; set; }
        public string? LongDescription { get; set; }
        public bool IsChargeable { get; set; }
        public string? ChargeType { get; set; }
        public decimal? BaseRate { get; set; }
        public decimal? SecurityDeposit { get; set; }
        public bool RefundableDeposit { get; set; }
        public bool TaxApplicable { get; set; }
        public long? TaxCodeId { get; set; }
        public decimal? TaxPercentage { get; set; }
        public List<AmenityUnitFeatureAddEdit> Features { get; set; } = new();
    }
}
