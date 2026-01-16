using Application.Interfaces;
using Domain.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace WebAPI.Controller
{
    [AllowAnonymous]
    [ApiController]
    [Route("api/[controller]")]
    public class HikvisionLogsController : ControllerBase
    {
        private readonly IHikvisionLogsService _service;

        public HikvisionLogsController(IHikvisionLogsService service)
        {
            _service = service;
        }

        [HttpPost("Search")]
        public async Task<IActionResult> Search([FromBody] HikvisionLogsRequest request)
        {
            if (request == null) return BadRequest("Request is required.");
            if (request.Start >= request.End) return BadRequest("Start must be less than End.");

            var result = await _service.GetLogsAsync(request);
            return Ok(result);
        }
    }
}
