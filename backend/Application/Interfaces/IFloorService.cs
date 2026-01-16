using Domain.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Interfaces
{
    public interface IFloorService
    {
        Task<PaginatedList<FloorList>> GetAllFloorsAsync(int pageIndex, int pageSize);
        Task<InsertResponseModel> CreateFloorAsync(FloorAddEdit floor);
        Task<FloorAddEdit> GetFloorByIdAsync(long id);
        Task<InsertResponseModel> UpdateFloorAsync(FloorAddEdit floor);
        Task DeleteFloorAsync(long id);
        Task<List<object>> GetAllFloorBasicAsync();
    }
}
