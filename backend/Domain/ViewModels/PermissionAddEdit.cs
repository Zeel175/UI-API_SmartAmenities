using System;

namespace Domain.ViewModels
{
    public class PermissionAddEdit
    {
        public long Id { get; set; }               // Changed to long to match the old model
        public string Code { get; set; }          // Kept as is
        public string Name { get; set; }          // Kept as is
        public bool IsDefault { get; set; }       // Added to match old model
        public long PermissionTypeId { get; set; }  // Added to match old model
        public int DisplayOrder { get; set; }     // Renamed from Priority to DisplayOrder to match old model
        public string Controller { get; set; }    // Added to match old model
        public string ActionName { get; set; }    // Added to match old model
    }
}
