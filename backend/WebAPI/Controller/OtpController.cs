using Application.Interfaces;
using Domain.Interfaces;
using Domain.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace SmartAmenities_API.Controllers
{
    [AllowAnonymous]
    [Route("api/[controller]")]
    [ApiController]
    public class OtpController : ControllerBase
    {
        private readonly IOtpService _otpService;

        public OtpController(IOtpService otpService)
        {
            _otpService = otpService;
        }

        [HttpPost("generate")]
        public async Task<IActionResult> Generate([FromBody] GenerateOtpRequest request)
        {
            var result = await _otpService.GenerateOtpAsync(request);

            if (!result.Success)
                return BadRequest(result);

            return Ok(result);
        }
        [HttpPost("verify")]
        public async Task<IActionResult> Verify([FromBody] VerifyOtpRequest request)
        {
            var result = await _otpService.VerifyOtpAsync(request);

            if (!result.IsSuccess)
            {
                if (string.Equals(result.Message, "User not found", System.StringComparison.OrdinalIgnoreCase))
                {
                    return NotFound(result);
                }
                if (string.Equals(result.Message, "Invalid username or password", System.StringComparison.OrdinalIgnoreCase))
                {
                    return BadRequest(new { error = "InvalidCredentials", message = result.Message });
                }

                return BadRequest(result);
            }
            return Ok(result);
        }
    }
}
