using Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace WebAPI.Controllers
{
    [AllowAnonymous]
    [Route("api/[controller]")]
    [ApiController]
    public class PermissionController : ControllerBase
    {
        private readonly IPermissionService _permissionService;

        public PermissionController(IPermissionService permissionService)
        {
            _permissionService = permissionService;
        }

        /// <summary>
        /// Get all active permissions.
        /// </summary>
        /// <returns>List of active permissions.</returns>
        [HttpGet("GetAllPermissions")]
        public async Task<IActionResult> GetAllPermissions()
        {
            try
            {
                var data = _permissionService.GetAllPermissionsAsync(); // No need for ToListAsync() as GetAllPermissionsAsync returns IQueryable
                return Ok(data);
            }
            catch (Exception ex)
            {
                // Log the exception (implementation depends on your logging setup)
                // e.g., _logger.LogError(ex, "An error occurred while fetching permissions.");

                // Return a generic error response
                return StatusCode(StatusCodes.Status500InternalServerError, "An error occurred while processing your request.");
            }
        }
    }
}
