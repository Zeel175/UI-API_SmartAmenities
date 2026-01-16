using Domain.Entities;
using System.Linq;
using System.Threading.Tasks;

namespace Domain.Interfaces
{
    public interface IRolePermissionRepository : IGenericRepository<RolePermissionMap>
    {
        IQueryable<RolePermissionMap> GetAllRolePermissionsWithRelatedInfo();
        Task<RolePermissionMap> GetByIdAsync(long id);
    }
}
