using Domain.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Interfaces
{
    public interface IResidentMasterService
    {
        Task<InsertResponseModel> CreateResidentAsync(ResidentMasterAddEdit resident);
        Task<InsertResponseModel> UpdateResidentAsync(ResidentMasterAddEdit resident);
        //Task<InsertResponseModel> UpdateResidentDetailsAsync(ResidentDetailUpdateRequest request);
        Task DeleteResidentAsync(long id);
        Task<PaginatedList<ResidentMasterList>> GetResidentsAsync(int pageIndex, int pageSize);
        Task<ResidentMasterAddEdit> GetResidentByIdAsync(long id, bool includeFamilyMembers = false);
       Task<InsertResponseModel> UpdateResidentDetailsAsync(ResidentDetailUpdateRequest_Profile request);
        Task<ResidentDetailResponse> GetResidentDetailsAsync(long? residentMasterId, long? residentFamilyMemberId);
        Task<IReadOnlyList<ResidentFamilyMemberList>> GetFamilyMembersByResidentIdAsync(long residentMasterId);
        Task<bool> UpdateResidentProfilePhotoAsync(long residentId, string photoPath, long userId);
        Task<bool> UpdateFamilyMemberProfilePhotoAsync(long familyMemberId, string photoPath, long userId);
        Task<IReadOnlyList<ResidentUserDropdownItem>> GetResidentUsersByUnitAsync(long unitId);
    }
}
