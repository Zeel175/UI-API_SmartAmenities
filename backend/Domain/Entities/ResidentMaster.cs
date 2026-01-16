using Domain.Entities.Domain.Entities;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Entities
{
    public class ResidentMaster : BaseAuditable
    {
        [Required]
        [MaxLength(50)]
        public string Code { get; set; }

        [Required]
        [MaxLength(200)]
        public string ParentFirstName { get; set; }

        [Required]
        [MaxLength(200)]
        public string ParentLastName { get; set; }

        [MaxLength(200)]
        public string Email { get; set; }

        [MaxLength(50)]
        public string Mobile { get; set; }
        [MaxLength(10)]
        public string? CountryCode { get; set; }

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
        public string Password { get; set; }

        public bool IsActive { get; set; }
        public bool IsResident { get; set; }

        [MaxLength(255)]
        public string ProfilePhoto { get; set; }
        public bool HasFace { get; set; }
        public bool HasFingerprint { get; set; }
        public DateTime? LastBiometricSyncUtc { get; set; }
        public ICollection<ResidentMasterUnit> ParentUnits { get; set; }
        public ICollection<ResidentFamilyMember> FamilyMembers { get; set; }
        public ICollection<ResidentDocument> Documents { get; set; }

    }
}
