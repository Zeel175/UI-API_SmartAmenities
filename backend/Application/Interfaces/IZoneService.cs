using Domain.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Interfaces
{
    public interface IZoneService
    {
        Task<PaginatedList<ZoneList>> GetAllZonesAsync(int pageIndex, int pageSize);
        Task<InsertResponseModel> CreateZoneAsync(ZoneAddEdit zone);
        Task<ZoneAddEdit> GetZoneByIdAsync(long id);
        Task<InsertResponseModel> UpdateZoneAsync(ZoneAddEdit zone);
        Task DeleteZoneAsync(long id);
        Task<List<object>> GetAllZoneBasicAsync();
    }
}
