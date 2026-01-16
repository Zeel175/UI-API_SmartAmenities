using Domain.ViewModels;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Application.Interfaces
{
    public interface IBuildingService
    {
        Task<InsertResponseModel> CreateBuildingAsync(BuildingAddEdit building);
        Task<InsertResponseModel> UpdateBuildingAsync(BuildingAddEdit building);
        Task DeleteBuildingAsync(long id);
        Task<BuildingAddEdit> GetBuildingByIdAsync(long id);
        Task<PaginatedList<BuildingList>> GetAllBuildingsAsync(int pageIndex, int pageSize);
        Task<List<object>> GetAllBuildingBasicAsync();
    }
}
