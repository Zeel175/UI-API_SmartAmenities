using Domain.Entities;
using Domain.ViewModels;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Interfaces
{
    public interface IRoleService
    {
        Task<IEnumerable<Role>> GetAllRolesAsync();
        Task<Role> GetRoleByIdAsync(string roleId);
      //  Task<RoleAddEdit> GetRoleById(string id);
        Task CreateRoleAsync(Role role);
       // Task CreateRoleAsync(RoleAddEdit role);
        Task UpdateRoleAsync(Role role);
        Task DeleteRoleAsync(string roleId);

    }
}
