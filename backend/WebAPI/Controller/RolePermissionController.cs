using Application.Helper;
using Application.Interfaces;
using Application.Services;
using Domain.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace WebAPI.Controllers
{
    [AllowAnonymous]
    [Route("api/[controller]")]
    [ApiController]
  //  [Authorize]
    public class RolePermissionController : ControllerBase
    {
        private readonly IRolePermissionService _rolePermissionService;

        public RolePermissionController(IRolePermissionService rolePermissionService)
        {
            _rolePermissionService = rolePermissionService;
        }

        [HttpPost("AddRolePermission")]
        public async Task<IActionResult> AddRolePermission(RoleAddEdit model)
        {
            var response = await _rolePermissionService.AddRolePermissionAsync(model, User.Identity.GetUserId());
            return Ok(response);
        }

        [HttpPost("UpdateRolePermission")]
        public async Task<IActionResult> UpdateRolePermission(RoleAddEdit model)
        {
            var response = await _rolePermissionService.UpdateRolePermissionAsync(model, User.Identity.GetUserId());
            return Ok(response);
        }

        [HttpGet("GetAllRolePermissions")]
        public async Task<IActionResult> GetAllRolePermissions()
        {
            var data =  _rolePermissionService.GetAllRolePermissionsAsync();
            return Ok(data);
        }

        [HttpGet("GetById/{id}")]
        public async Task<IActionResult> GetRolePermissionById(long id)
        {
            var data = await _rolePermissionService.GetRolePermissionByIdAsync(id);
            if (data == null)
                return NotFound();
            return Ok(data);
        }

        [HttpDelete("Delete/{id}")]
        public async Task<IActionResult> DeleteRolePermission(long id)
        {
            long userId = User.Identity.GetUserId();
            await _rolePermissionService.DeleteRolePermissionAsync(id, userId);
            return NoContent();
        }
    }
}
