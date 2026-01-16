using System;

namespace Domain.ViewModels
{
    public class PermissionList
    {
        public long Id { get; set; }               // Changed to long to match the updated model
        public string Code { get; set; }          // Kept as is
        public string Name { get; set; }          // Kept as is
        public bool IsDefault { get; set; }       // Added to match the updated model
        public long PermissionTypeId { get; set; } 
        // Added to match the updated model
        public int DisplayOrder { get; set; }     // Renamed from Priority to DisplayOrder
    }
}
