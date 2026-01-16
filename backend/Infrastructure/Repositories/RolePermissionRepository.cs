using Domain.Entities;
using Domain.Interfaces;
using Infrastructure.Context;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;

namespace Infrastructure.Repositories
{
    public class RolePermissionRepository : GenericRepository<RolePermissionMap>, IRolePermissionRepository
    {
        public RolePermissionRepository(AppDbContext context) : base(context)
        {
        }

        public IQueryable<RolePermissionMap> GetAllRolePermissionsWithRelatedInfo()
        {
            return _context.RolePermissionMaps
                .Include(rp => rp.Role) // Include related Role
                .Include(rp => rp.Permission) // Include related Permission
                .Where(rp => rp.HasMasterAccess); // You can adjust the filter based on your needs
        }

        public async Task<RolePermissionMap> GetByIdAsync(long id)
        {
            return await _context.RolePermissionMaps
                .Include(rp => rp.Role) // Include related Role
                .Include(rp => rp.Permission) // Include related Permission
                .SingleOrDefaultAsync(rp => rp.Id == id);
        }
    }
}
