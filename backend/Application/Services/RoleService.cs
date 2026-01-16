// Application/Services/RoleService.cs
using System.Collections.Generic;
using System.Threading.Tasks;
using Application.Interfaces;
using Domain.Entities;
using Domain.ViewModels;
using Microsoft.AspNetCore.Identity;

namespace Application.Services
{
    public class RoleService : IRoleService
    {
        private readonly RoleManager<Role> _roleManager;

        public RoleService(RoleManager<Role> roleManager)
        {
            _roleManager = roleManager;
        }

        public async Task<IEnumerable<Role>> GetAllRolesAsync()
        {
            return _roleManager.Roles;
        }

        public async Task<Role> GetRoleByIdAsync(string roleId)
        {
            return await _roleManager.FindByIdAsync(roleId);
        }

        public async Task<RoleAddEdit> GetRoleById(long roleId)
        {
            var role = await _roleManager.FindByIdAsync(roleId.ToString());
            if (role == null) return null;
            return new RoleAddEdit
            {
                Id = role.Id,
                Name = role.Name,
                Permissions = new List<PermissionAssign>()
            };
        }

        public async Task CreateRoleAsync(Role role)
        {
            await _roleManager.CreateAsync(role);
        }

        //public async Task CreateRoleAsync(RoleAddEdit roleAddEdit)
        //{

        //    var role = new Role
        //    {
        //        Name = roleAddEdit.Name,
        //        RolePermissions = roleAddEdit.per
        //    };

        //    await _roleManager.CreateAsync(role);
        //    roleAddEdit.Id = role.Id;
        //    await _roleManager.CreateAsync(role);
        //}

        public async Task UpdateRoleAsync(Role role)
        {
            var existingRole = await _roleManager.FindByIdAsync(role.Id.ToString());
            if (existingRole != null)
            {
                existingRole.Name = role.Name;
                existingRole.NormalizedName = role.NormalizedName;
                await _roleManager.UpdateAsync(existingRole);
            }
        }

        public async Task DeleteRoleAsync(string roleId)
        {
            var role = await _roleManager.FindByIdAsync(roleId);
            if (role != null)
            {
                await _roleManager.DeleteAsync(role);
            }
        }
    }
}
