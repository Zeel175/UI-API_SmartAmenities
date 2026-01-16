using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;


namespace Domain.ViewModels
{
    public class ResidentMasterAddEdit
    {
        public long Id { get; set; }

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

        public string FaceId { get; set; }

        public string FingerId { get; set; }

        public string CardId { get; set; }

        public string QrId { get; set; }
        public string? QrCodeValue { get; set; }
        public string? QrCodeImagePath { get; set; }
        public string Password { get; set; }

        public bool IsActive { get; set; }
        public bool IsResident { get; set; }
        public string ProfilePhoto { get; set; }   // existing photo (for edit)
        public IFormFile ProfilePhotoFile { get; set; } // new upload
        public List<long> UnitIds { get; set; } = new();
        public List<ResidentFamilyMemberAddEdit> FamilyMembers { get; set; } = new();
        //public List<IFormFile> Documents { get; set; } = new();

        [JsonIgnore]
        public List<IFormFile>? Documents { get; set; }

        [JsonPropertyName("documents")]
        public List<ResidentDocumentDto> DocumentDetails { get; set; } = new();

    }
}
