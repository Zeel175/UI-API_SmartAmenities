using System.Collections.Generic;
using Domain.ViewModels;

namespace Application.Interfaces
{
    public interface IAmenityUnitService
    {
        Task<InsertResponseModel> CreateAmenityUnitAsync(AmenityUnitAddEdit unit);
        Task<InsertResponseModel> UpdateAmenityUnitAsync(AmenityUnitAddEdit unit);
        Task DeleteAmenityUnitAsync(long id);
        Task<AmenityUnitAddEdit?> GetAmenityUnitByIdAsync(long id);
        Task<PaginatedList<AmenityUnitList>> GetAmenityUnitsAsync(int pageIndex, int pageSize);
        Task<IReadOnlyList<AmenityUnitList>> GetAmenityUnitsAsync();
        Task<IReadOnlyList<AmenityUnitList>> GetAmenityUnitsByAmenityIdAsync(long amenityId);
    }
}
