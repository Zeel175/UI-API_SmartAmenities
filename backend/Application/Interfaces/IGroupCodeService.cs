using Domain.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Interfaces
{
    public interface IGroupCodeService
    {
        Task<InsertResponseModel> CreateGroupCodeAsync(GroupCodeAddEdit groupCode); // Create a new groupCode
        Task<PaginatedList<GroupCodeList>> GetAllGroupCodesAsync(int pageIndex, int pageSize);
        Task<ResponseModel> Activate(long id, bool isActive);
        Task<GroupCodeAddEdit> GetGroupCodeByIdAsync(long id);
        Task<InsertResponseModel> UpdateGroupCodeAsync(GroupCodeAddEdit groupCode);
        Task<List<object>> GetAllGroupCodeBasicAsync();
        Task<IList<SelectListModel>> GetGroupCodeByGroupName(string GroupName);

    }
}
