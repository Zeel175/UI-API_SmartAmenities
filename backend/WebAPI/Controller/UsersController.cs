// WebAPI/Controllers/UsersController.cs
using Application.Interfaces;
using Application.Services;
using Domain.Entities;
using Domain.Interfaces;
using Domain.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace WebAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [AllowAnonymous]
    //[Route("api/account")]
    // [Authorize]
    public class UsersController : ControllerBase
    {
        private readonly IUserService _userService;
       // private readonly IUserRepository _userRepo;

        public UsersController(IUserService userService)
        {
            _userService = userService;
          //  _userRepo = userRepository;
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> Get(string id)
        {
            var user = await _userService.GetUserByIdAsync(id);
            if (user == null) return NotFound();
            return Ok(user);
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var users = await _userService.GetAllUsersAsync();
            return Ok(users);
        }


        //[HttpPost]
        //public async Task<IActionResult> Create([FromBody] User user)
        //{
        //    await _userService.CreateUserAsync(user);
        //    return CreatedAtAction(nameof(Get), new { id = user.Id }, user);
        //}

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] UserAddEdit userAddEdit)
        {
            var result = await _userService.CreateUserAsync(userAddEdit);
            if (!result.Succeeded)
            {
                return BadRequest(result.Errors);
            }
            // Optionally, retrieve the created user and return it
            var user = await _userService.GetUserByEmailAsync(userAddEdit.Email);
            return CreatedAtAction(nameof(Get), new { id = user.Id }, user);
        }



        [HttpPut("{id}")]
        public async Task<IActionResult> Update(long id, [FromBody] User user)
        {
            if (id != user.Id) return BadRequest();
            await _userService.UpdateUserAsync(user);
            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(string id)
        {
            var user = await _userService.GetUserByIdAsync(id);

            if (user == null)
            {
                return NotFound("User not found.");
            }

            // Perform soft delete by updating the IsActive flag
            await _userService.DeleteUserAsync(id);
            return Ok(new { message = "User deleted (soft delete)." });
        }


        [HttpGet("email/{email}")]
        public async Task<IActionResult> GetUserByEmail(string email)
        {
            var user = await _userService.GetUserByEmailAsync(email);
            if (user == null)
            {
                return NotFound();
            }
            return Ok(user);
        }

        [HttpPost("{id}/assign-role")]
        public async Task<IActionResult> AssignRoleToUser(long id, string role)
        {
            var user = await _userService.GetUserById(id);
            if (user == null)
            {
                return NotFound();
            }
            var result = await _userService.AssignRoleToUserAsync(user, role);
            if (!result.Succeeded)
            {
                return BadRequest(result.Errors);
            }
            return NoContent();
        }

        [HttpPost("{id}/remove-role")]
        public async Task<IActionResult> RemoveRoleFromUser(long id, string role)
        {
            var user = await _userService.GetUserById(id);
            if (user == null)
            {
                return NotFound();
            }
            var result = await _userService.RemoveRoleFromUserAsync(user, role);
            if (!result.Succeeded)
            {
                return BadRequest(result.Errors);
            }
            return NoContent();
        }

        [HttpPost("{id}/check-password")]
        public async Task<IActionResult> CheckPassword(string id, string password)
        {
            var user = await _userService.GetUserByIdAsync(id);
            if (user == null)
            {
                return NotFound();
            }
            var result = await _userService.CheckPasswordAsync(user, password);
            if (!result)
            {
                return Unauthorized();
            }
            return Ok();
        }

        [HttpPost("{id}/change-password")]
        public async Task<IActionResult> ChangePassword(string id, string currentPassword, string newPassword)
        {
            var user = await _userService.GetUserByIdAsync(id);
            if (user == null)
            {
                return NotFound();
            }
            var result = await _userService.ChangePasswordAsync(user, currentPassword, newPassword);
            if (!result.Succeeded)
            {
                return BadRequest(result.Errors);
            }
            return NoContent();
        }

        [HttpPost("{id}/reset-password")]
        public async Task<IActionResult> ResetPassword(string id, string token, string newPassword)
        {
            var user = await _userService.GetUserByIdAsync(id);
            if (user == null)
            {
                return NotFound();
            }
            var result = await _userService.ResetPasswordAsync(user, token, newPassword);
            if (!result.Succeeded)
            {
                return BadRequest(result.Errors);
            }
            return NoContent();
        }

        [HttpGet("{id}/generate-email-confirmation-token")]
        public async Task<IActionResult> GenerateEmailConfirmationToken(string id)
        {
            var user = await _userService.GetUserByIdAsync(id);
            if (user == null)
            {
                return NotFound();
            }
            var token = await _userService.GenerateEmailConfirmationTokenAsync(user);
            return Ok(token);
        }

        [HttpPost("{id}/confirm-email")]
        public async Task<IActionResult> ConfirmEmail(string id, string token)
        {
            var user = await _userService.GetUserByIdAsync(id);
            if (user == null)
            {
                return NotFound();
            }
            var result = await _userService.ConfirmEmailAsync(user, token);
            if (!result.Succeeded)
            {
                return BadRequest(result.Errors);
            }
            return NoContent();
        }

        [HttpGet("count")]
        public async Task<IActionResult> CountUsers()
        {
            var count = await _userService.CountUsersAsync();
            return Ok(count);
        }

        [HttpGet("{id}/generate-password-reset-token")]
        public async Task<IActionResult> GeneratePasswordResetToken(string id)
        {
            var user = await _userService.GetUserByIdAsync(id);
            if (user == null)
            {
                return NotFound();
            }
            var token = await _userService.GeneratePasswordResetTokenAsync(user);
            return Ok(token);
        }

        [HttpGet("{id}/is-in-role")]
        public async Task<IActionResult> IsUserInRole(string id, string role)
        {
            var user = await _userService.GetUserByIdAsync(id);
            if (user == null)
            {
                return NotFound();
            }

            var isInRole = await _userService.IsUserInRoleAsync(user, role);
            return Ok(isInRole);
        }

        [HttpGet("{id}/roles")]
        public async Task<IActionResult> GetUserRoles(string id)
        {
            var user = await _userService.GetUserByIdAsync(id);
            if (user == null)
            {
                return NotFound();
            }

            var roles = await _userService.GetUserRolesAsync(user);
            return Ok(roles);
        }

        // GET: api/user/{id}/is-user-active
        [HttpGet("{id}/is-user-active")]
        public async Task<IActionResult> IsUserActive(string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                return BadRequest("User ID cannot be null or empty.");
            }

            var isActive = await _userService.IsUserActiveAsync(id);
            return Ok(isActive);
        }

        // PUT: api/user/{id}/set-user-active-status
        [HttpPut("{id}/set-user-active-status")]
        public async Task<IActionResult> SetUserActiveStatus(string id, [FromBody] bool isActive)
        {
            if (string.IsNullOrEmpty(id))
            {
                return BadRequest("User ID cannot be null or empty.");
            }

            var user = await _userService.GetUserByIdAsync(id);
            if (user == null)
            {
                return NotFound("User not found.");
            }

            await _userService.SetUserActiveStatusAsync(id, isActive);
            return NoContent();
        }
        [HttpGet("paged")]
        public async Task<IActionResult> GetPaged([FromQuery] int pageIndex = 1, [FromQuery] int pageSize = 10)
        {
            if (pageIndex < 1 || pageSize < 1)
            {
                return BadRequest("Page index and page size must be greater than zero.");
            }

            var paginatedUsers = await _userService.GetPagedUsersAsync(pageIndex, pageSize);
            return Ok(paginatedUsers);
        }

    }
}
