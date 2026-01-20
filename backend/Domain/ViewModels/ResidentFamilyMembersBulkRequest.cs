using System.Collections.Generic;

namespace Domain.ViewModels
{
    public class ResidentFamilyMembersBulkRequest
    {
        public List<ResidentFamilyMemberAddEdit> FamilyMembers { get; set; } = new();
    }
}
