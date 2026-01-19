using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.ViewModels
{
    public class ResidentFamilyMemberList
    {
        public long Id { get; set; }
        public long ResidentMasterId { get; set; }
        public List<long> UnitIds { get; set; }
        public List<ResidentFamilyMemberUnitList> Units { get; set; }
        public string FirstName { get; set; }
        public string Code { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string Mobile { get; set; }
        public string FaceId { get; set; }
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
    }
}
