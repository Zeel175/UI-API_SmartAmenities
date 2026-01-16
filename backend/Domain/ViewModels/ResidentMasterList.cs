using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.ViewModels
{
    public class ResidentMasterList
    {
        public long Id { get; set; }
        public string Code { get; set; }
        public string ParentFirstName { get; set; }
        public string ParentLastName { get; set; }
        public string Email { get; set; }
        public string Mobile { get; set; }
        public string? CountryCode { get; set; }
        public string FaceId { get; set; }
        public string FingerId { get; set; }
        public string CardId { get; set; }
        public string QrId { get; set; }
        public string? QrCodeValue { get; set; }
        public string? QrCodeImagePath { get; set; }
        public bool IsActive { get; set; }
        public bool IsResident { get; set; }
        public List<long> UnitIds { get; set; }
        public List<ResidentFamilyMemberList> FamilyMembers { get; set; }
    }
}
