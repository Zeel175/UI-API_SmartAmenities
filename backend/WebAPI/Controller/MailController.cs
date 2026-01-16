using Application.Interfaces;
using Domain.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace WebAPI.Controller
{
    [AllowAnonymous]
    [Route("api/[controller]")]
    [ApiController]
    public class MailController : ControllerBase
    {
        private readonly IMailService _mailService;

        public MailController(IMailService mailService)
        {
            _mailService = mailService;
        }

        [HttpPost("Email")]
        public async Task<IActionResult> SendEmail([FromBody] EmailData emailConfig)
        {
            if (emailConfig == null) return BadRequest(new { message = "Invalid email configuration." });

            await _mailService.SendEmailAsync(emailConfig);
            return Ok(new { message = "Email sent successfully." });   // <-- JSON, not plain text
        }

    }
}
