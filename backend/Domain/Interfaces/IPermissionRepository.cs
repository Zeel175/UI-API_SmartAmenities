using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Interfaces
{
    public interface IPermissionRepository : IGenericRepository<Permission>
    {
        IQueryable<Permission> GetAllPermission();

        Permission GetSingle(Expression<Func<Permission, bool>> filter);

    }
}
