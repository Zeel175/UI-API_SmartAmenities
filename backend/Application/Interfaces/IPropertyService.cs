using Domain.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Interfaces
{
    public interface IPropertyService
    {
        Task<PaginatedList<PropertyList>> GetAllPropertiesAsync(int pageIndex, int pageSize);
        Task<InsertResponseModel> CreatePropertyAsync(PropertyAddEdit property); // Create a new property
        Task<PropertyAddEdit> GetPropertyByIdAsync(long id);
        Task<InsertResponseModel> UpdatePropertyAsync(PropertyAddEdit property);
        Task DeletePropertyAsync(long id);
        Task<List<object>> GetAllPropertiesBasicAsync();

    }
}
