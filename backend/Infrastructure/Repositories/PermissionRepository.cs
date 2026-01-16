using Domain.Entities;
using Domain.Interfaces;
using Infrastructure.Context;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Repositories
{
    public class PermissionRepository : GenericRepository<Permission>, IPermissionRepository
    {
        public PermissionRepository(AppDbContext context) : base(context)
        {
        }

        public IQueryable<Permission> GetAllPermission()
        {
            return _context.Permissions
                .Where(rp => rp.IsActive); // You can adjust the filter based on your needs
        }

        public Permission GetSingle(Expression<Func<Permission, bool>> filter)
        {
            return _context.Set<Permission>().Where(filter).FirstOrDefault();
        }

        // Implement methods specific to Property if needed
    }
}
