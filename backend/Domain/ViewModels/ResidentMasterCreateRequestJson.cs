using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.ViewModels
{
    public class ResidentMasterCreateRequestJson
    {
        [Required, MaxLength(200)]
        public string ParentFirstName { get; set; } = default!;

        [Required, MaxLength(200)]
        public string ParentLastName { get; set; } = default!;

        [MaxLength(200)]
        public string? Email { get; set; }

        [MaxLength(50)]
        public string? Mobile { get; set; }

        public string? CountryCode { get; set; }
        public string? FaceId { get; set; }
        public string? FaceUrl { get; set; }
        public string? FingerId { get; set; }
        public string? CardId { get; set; }
        public string? QrId { get; set; }

        public string? Password { get; set; }
        public bool IsResident { get; set; }

        public List<long> UnitIds { get; set; } = new();
        public List<ResidentFamilyMemberCreateRequestJson> FamilyMembers { get; set; } = new();

        // ✅ NEW (Parent only)
        public string? ProfilePhoto { get; set; }                 // file path (already uploaded)
        public List<string> DocumentFilePaths { get; set; } = new(); // multiple file paths
    }

    public class ResidentFamilyMemberCreateRequestJson
    {
        [Required, MaxLength(200)]
        public string FirstName { get; set; } = default!;

        [Required, MaxLength(200)]
        public string LastName { get; set; } = default!;

        [MaxLength(200)]
        public string? Email { get; set; }

        [MaxLength(50)]
        public string? Mobile { get; set; }

        public string? FaceId { get; set; }
        public string? FaceUrl { get; set; }
        public string? FingerId { get; set; }
        public string? CardId { get; set; }
        public string? QrId { get; set; }

        public bool IsResident { get; set; }

        public List<long> UnitIds { get; set; } = new();
    }

}
