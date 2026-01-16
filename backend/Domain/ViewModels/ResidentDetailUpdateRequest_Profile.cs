using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.ViewModels
{
    public class ResidentDetailUpdateRequest_Profile
    {
        public string Type { get; set; } = default!;
        public long? ActorUserId { get; set; }
        public ResidentProfilePatchDto? Resident { get; set; }
        public FamilyMemberProfilePatchDto? FamilyMember { get; set; }
    }

    public class ResidentProfilePatchDto
    {
        public long Id { get; set; }

        public string? ParentFirstName { get; set; }
        public string? ParentLastName { get; set; }
        public string? Email { get; set; }
        public string? Mobile { get; set; }
        public string? CountryCode { get; set; }
        public string? FaceId { get; set; }
        public string? FingerId { get; set; }

        public string? CardId { get; set; }
        public bool? IsActive { get; set; }
        public bool? IsResident { get; set; }
        public List<long>? UnitIds { get; set; }

        public string? ProfilePhoto { get; set; }
    }

    public class FamilyMemberProfilePatchDto
    {
        public long Id { get; set; }
        public long? ResidentMasterId { get; set; }

        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public string? Email { get; set; }
        public string? Mobile { get; set; }
        public string? FaceId { get; set; }
        public string? FingerId { get; set; }

        public string? CardId { get; set; }
        public bool? IsActive { get; set; }
        public bool? IsResident { get; set; }
        public List<long>? UnitIds { get; set; }

        public string? ProfilePhoto { get; set; }
    }

}
