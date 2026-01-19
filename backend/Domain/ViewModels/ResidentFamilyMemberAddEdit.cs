using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.ViewModels
{
    public class ResidentFamilyMemberAddEdit : IValidatableObject
    {
        public long Id { get; set; }

        public long ResidentMasterId { get; set; }

        public List<long> UnitIds { get; set; } = new();

        [MaxLength(200)]
        public string FirstName { get; set; }

        [MaxLength(200)]
        public string LastName { get; set; }

        public string Email { get; set; }

        public string Mobile { get; set; }
        public string ProfilePhoto { get; set; }
        public string Code { get; set; }

        public IFormFile ProfilePhotoFile { get; set; }

        public string FaceId { get; set; }

        public string? FaceUrl { get; set; }

        public string FingerId { get; set; }

        public string CardId { get; set; }

        public string QrId { get; set; }
        public string? QrCodeValue { get; set; }
        public string? QrCodeImagePath { get; set; }
        public bool HasFace { get; set; }
        public bool HasFingerprint { get; set; }
        public DateTime? LastBiometricSyncUtc { get; set; }
        public bool IsActive { get; set; }
        public bool IsResident { get; set; }

        public bool IsEmpty()
        {
            return Id == 0
                && string.IsNullOrWhiteSpace(FirstName)
                && string.IsNullOrWhiteSpace(LastName)
                && string.IsNullOrWhiteSpace(Email)
                && string.IsNullOrWhiteSpace(Mobile)
                && string.IsNullOrWhiteSpace(FaceId)
                && string.IsNullOrWhiteSpace(FaceUrl)
                && string.IsNullOrWhiteSpace(FingerId)
                && string.IsNullOrWhiteSpace(CardId)
                && string.IsNullOrWhiteSpace(QrId)
                && (UnitIds == null || UnitIds.Count == 0);
        }

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            if (IsEmpty())
            {
                yield break;
            }

            if (string.IsNullOrWhiteSpace(FirstName))
            {
                yield return new ValidationResult("The FirstName field is required.", new[] { nameof(FirstName) });
            }

            if (string.IsNullOrWhiteSpace(LastName))
            {
                yield return new ValidationResult("The LastName field is required.", new[] { nameof(LastName) });
            }
        }
    }
}
