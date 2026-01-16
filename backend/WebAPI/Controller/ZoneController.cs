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
    public class ZoneController : ControllerBase
    {
        private readonly IZoneService _zoneService;

        public ZoneController(IZoneService zoneService)
        {
            _zoneService = zoneService;
        }

        [Route("AddZone")]
        [HttpPost]
        public async Task<IActionResult> CreateZoneAsync(ZoneAddEdit zone)
        {
            var response = await _zoneService.CreateZoneAsync(zone);
            return Ok(response);
        }

        [HttpGet("GetAllZoneBasic")]
        public async Task<IActionResult> GetAllZoneBasic()
        {
            var data = await _zoneService.GetAllZoneBasicAsync();
            return Ok(data);
        }

        [HttpGet("GetAllZone/paged")]
        public async Task<IActionResult> GetAllZone(int pageIndex, int pageSize)
        {
            if (pageIndex < 1 || pageSize < 1)
            {
                return BadRequest("Page index and page size must be greater than zero.");
            }
            var data = await _zoneService.GetAllZonesAsync(pageIndex, pageSize);
            return Ok(data);
        }

        [HttpGet("GetByIdAsync")]
        public async Task<IActionResult> Get(long id)
        {
            var zone = await _zoneService.GetZoneByIdAsync(id);
            if (zone == null)
            {
                return NotFound();
            }
            return Ok(zone);
        }

        [Route("EditZone")]
        [HttpPost]
        public async Task<IActionResult> EditZone([FromBody] ZoneAddEdit zone)
        {
            var response = await _zoneService.UpdateZoneAsync(zone);
            return Ok(response);
        }


        [HttpDelete("DeleteZone")]
        public async Task<IActionResult> DeleteZone(long id)
        {
            await _zoneService.DeleteZoneAsync(id);
            return NoContent();
        }
    }
}
