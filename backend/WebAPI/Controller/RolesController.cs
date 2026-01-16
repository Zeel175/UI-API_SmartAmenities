// WebAPI/Controllers/RolesController.cs
using Application.Interfaces;
using Application.Services;
using Domain.Entities;
using Domain.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using System.Threading.Tasks;

namespace WebAPI.Controllers
{
    [AllowAnonymous]
    [ApiController]
    [Route("api/[controller]")]
    [Route("api/role")]

   // [Authorize(Roles = "Admin")]
    public class RolesController : ControllerBase
    {
        private readonly IRoleService _roleService;
      //  private readonly UserManager<User> _userManager;

        public RolesController(IRoleService roleService)
        {
            _roleService = roleService;
           // _userManager = userManager;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var roles = await _roleService.GetAllRolesAsync();
            return Ok(roles);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> Get(string id)
        {
            var role = await _roleService.GetRoleByIdAsync(id);
            if (role == null) return NotFound();
            return Ok(role);
        }

        //[HttpGet("{id}")]
        //public async Task<IActionResult> Get(string id)
        //{
        //    var role = await _roleService.GetRoleById(id);
        //    if (role == null) return NotFound();
        //    return Ok(role);
        //}

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] Role role)
        {
            await _roleService.CreateRoleAsync(role);
            return CreatedAtAction(nameof(Get), new { id = role.Id }, role);
        }

        //[HttpPost]
        //public async Task<IActionResult> Add(RoleAddEdit role)
        //{
        //   // int userId = User.Identity.GetUserId();
        //    await _roleService.CreateRoleAsync(role);
        //    return CreatedAtAction(nameof(Get), new { id = role.Id }, role);
        //}

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(long id, [FromBody] Role role)
        {
            
            if (id != role.Id) return BadRequest();
            await _roleService.UpdateRoleAsync(role);
            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(string id)
        {
            await _roleService.DeleteRoleAsync(id);
            return NoContent();
        }
    }
}


