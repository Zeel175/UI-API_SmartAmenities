using Domain.ViewModels;

namespace Application.Interfaces
{
    public interface IAmenityMasterService
    {
        Task<InsertResponseModel> CreateAmenityAsync(AmenityMasterAddEdit amenity);
        Task<InsertResponseModel> UpdateAmenityAsync(AmenityMasterAddEdit amenity);
        Task DeleteAmenityAsync(long id);
        Task<AmenityMasterAddEdit?> GetAmenityByIdAsync(long id);
        Task<PaginatedList<AmenityMasterList>> GetAmenitiesAsync(int pageIndex, int pageSize);
    }
}
