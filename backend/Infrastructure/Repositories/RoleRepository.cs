using Domain.Entities;
using Domain.Interfaces;
using Infrastructure.Context;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Repositories
{

    public class RoleRepository : GenericRepository<Role>, IRoleRepository
    {
        public RoleRepository(AppDbContext context) : base(context)
        {
        }

        public IQueryable<Role> GetAllRole()
        {
            return _context.Roles
                .Include(wc => wc.RolePermissions) // Ensure the navigation property is included
            .Where(wc => wc.IsActive);
        }

        public async Task<Role> GetByIdAsync(long id)
        {
            return await _context.Roles
                .Include(w => w.RolePermissions)
                    .ThenInclude(d => d.Permission)  // Include WeightCheckSubDetails
                .SingleOrDefaultAsync(w => w.Id == id);
        }

        // Implement methods specific to Property if needed
    }
}
