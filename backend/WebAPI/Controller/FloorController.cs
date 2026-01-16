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
    public class FloorController : ControllerBase
    {
        private readonly IFloorService _floorService;

        public FloorController(IFloorService floorService)
        {
            _floorService = floorService;
        }

        [Route("AddFloor")]
        [HttpPost]
        public async Task<IActionResult> CreateFloorAsync(FloorAddEdit floor)
        {
            var response = await _floorService.CreateFloorAsync(floor);
            return Ok(response);
        }

        [HttpGet("GetAllFloorBasic")]
        public async Task<IActionResult> GetAllFloorBasic()
        {
            var data = await _floorService.GetAllFloorBasicAsync();
            return Ok(data);
        }

        [HttpGet("GetAllFloor/paged")]
        public async Task<IActionResult> GetAllFloor(int pageIndex, int pageSize)
        {
            if (pageIndex < 1 || pageSize < 1)
            {
                return BadRequest("Page index and page size must be greater than zero.");
            }
            var data = await _floorService.GetAllFloorsAsync(pageIndex, pageSize);
            return Ok(data);
        }

        [HttpGet("GetByIdAsync")]
        public async Task<IActionResult> Get(long id)
        {
            var floor = await _floorService.GetFloorByIdAsync(id);
            if (floor == null)
            {
                return NotFound();
            }
            return Ok(floor);
        }

        [Route("EditFloor")]
        [HttpPost]
        public async Task<IActionResult> EditFloor(FloorAddEdit floor)
        {
            var response = await _floorService.UpdateFloorAsync(floor);
            return Ok(response);
        }

        [HttpDelete("DeleteFloor")]
        public async Task<IActionResult> DeleteFloor(long id)
        {
            await _floorService.DeleteFloorAsync(id);
            return NoContent();
        }
    }
}
