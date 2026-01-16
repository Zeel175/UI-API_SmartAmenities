using Domain.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Interfaces
{
    public interface IUnitService
    {
        Task<InsertResponseModel> CreateUnitAsync(UnitAddEdit unit);
        Task<InsertResponseModel> UpdateUnitAsync(UnitAddEdit unit);
        Task DeleteUnitAsync(long id);

        Task<PaginatedList<UnitList>> GetAllUnitsAsync(int pageIndex, int pageSize);
        Task<List<object>> GetAllUnitBasicAsync();
        Task<UnitAddEdit> GetUnitByIdAsync(long id);
        Task<List<object>> GetUnitsByBuildingAsync(long buildingId);
        //// Optional
        //Task<List<object>> GetUnitsByBuildingFloorAsync(long buildingId, long floorId);
    }
}
