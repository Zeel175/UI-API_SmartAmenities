using System.Collections.Generic;

namespace Domain.ViewModels
{
    public class AmenityUnitWithSlotsList : AmenityUnitList
    {
        public List<AmenitySlotTemplateList> Slots { get; set; } = new();
    }
}
