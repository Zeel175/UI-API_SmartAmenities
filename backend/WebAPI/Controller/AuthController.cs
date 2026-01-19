using Application.Interfaces;
using Domain.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity.Data;
using Microsoft.AspNetCore.Mvc;
using System;

namespace WebAPI.Controller
{
    [Route("api/[controller]")]
    [Route("api/account")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;

        public AuthController(IAuthService authService)
        {
            _authService = authService;
        }


        [AllowAnonymous]
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginDto request)
        {
            var response = await _authService.LoginAsync(request.Username, request.Password);
            if (response.IsSuccess)
            {
                return Ok(response);
            }
            if (string.Equals(response.Message, "User not found", StringComparison.OrdinalIgnoreCase))
            {
                return NotFound(response);
            }
            return Unauthorized(response);
        }
        [AllowAnonymous]
        [HttpPost("logout")]
        public async Task<IActionResult> Logout()
        {
            await _authService.LogoutAsync();
            return NoContent();
        }

        [HttpPost("{id}/change-password")]
        public async Task<IActionResult> ChangePassword(string id, [FromBody] ChangePasswordDto changePasswordDto)
        {
            var result = await _authService.ChangePasswordAsync(id, changePasswordDto.CurrentPassword, changePasswordDto.NewPassword);
            if (result.Succeeded)
                return NoContent();

            return BadRequest(result.Errors);
        }

        [HttpPost("{id}/reset-password")]
        public async Task<IActionResult> ResetPassword(string id, [FromBody] ResetPasswordDto resetPasswordDto)
        {
            var result = await _authService.ResetPasswordAsync(id, resetPasswordDto.Token, resetPasswordDto.NewPassword);
            if (result.Succeeded)
                return NoContent();

            return BadRequest(result.Errors);
        }
        [AllowAnonymous]
        [HttpPost("login-identifier")]
        public async Task<IActionResult> LoginWithIdentifier([FromBody] LoginWithIdentifierDto request)
        {
            var response = await _authService.LoginWithIdentifierAsync(request.Identifier, request.Password);
            if (response.IsSuccess)
            {
                return Ok(response);
            }
            if (string.Equals(response.Message, "User not found", StringComparison.OrdinalIgnoreCase))
            {
                return NotFound(response);
            }
            if (string.Equals(response.Message, "Invalid username or password", StringComparison.OrdinalIgnoreCase))
            {
                return BadRequest(new { error = "InvalidCredentials", message = response.Message });
            }
            return Unauthorized(response);
        }
    }
}
