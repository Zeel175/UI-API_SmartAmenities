using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Entities
{
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    namespace Domain.Entities
    {
        public class ResidentFamilyMember : BaseAuditable
        {
            [ForeignKey(nameof(ResidentMaster))]
            public long ResidentMasterId { get; set; }
            public ResidentMaster ResidentMaster { get; set; }

            [Required]
            [MaxLength(200)]
            public string FirstName { get; set; }
            [MaxLength(50)]
            public string Code { get; set; }
            [Required]
            [MaxLength(200)]
            public string LastName { get; set; }

            [MaxLength(200)]
            public string Email { get; set; }

            [MaxLength(50)]
            public string Mobile { get; set; }

            [MaxLength(500)]
            public string ProfilePhoto { get; set; }

            [MaxLength(200)]
            public string FaceId { get; set; }

            [MaxLength(200)]
            public string FingerId { get; set; }

            [MaxLength(200)]
            public string CardId { get; set; }

            [MaxLength(200)]
            public string QrId { get; set; }
            public string? QrCodeValue { get; set; }
            [MaxLength(500)]
            public string? QrCodeImagePath { get; set; }
            public bool IsActive { get; set; }
            public bool IsResident { get; set; }
            public bool HasFace { get; set; }
            public bool HasFingerprint { get; set; }
            public DateTime? LastBiometricSyncUtc { get; set; }

            public ICollection<ResidentFamilyMemberUnit> MemberUnits { get; set; }
        }
    }
}
