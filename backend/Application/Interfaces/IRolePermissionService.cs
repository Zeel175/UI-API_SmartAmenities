using Domain.ViewModels;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Application.Interfaces
{
    public interface IRolePermissionService
    {
        IQueryable<RoleList> GetAllRolePermissionsAsync();
        Task<RoleAddEdit> GetRolePermissionByIdAsync(long id);
        Task<long> AddRolePermissionAsync(RoleAddEdit model, long userId);
        Task<long> UpdateRolePermissionAsync(RoleAddEdit model, long userId);
        Task DeleteRolePermissionAsync(long id, long userId);
    }
}
