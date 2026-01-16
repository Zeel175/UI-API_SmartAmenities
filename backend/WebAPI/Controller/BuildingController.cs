using Application.Interfaces;
using Application.Services;
using Domain.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace WebAPI.Controller
{
    [AllowAnonymous]
    [Route("api/[controller]")]
    [ApiController]
    public class BuildingController : ControllerBase
    {
        private readonly IBuildingService _buildingService;

        public BuildingController(IBuildingService buildingService)
        {
            _buildingService = buildingService;
        }

        [HttpPost("AddBuilding")]
        public async Task<IActionResult> CreateBuildingAsync(BuildingAddEdit building)
        {
            var response = await _buildingService.CreateBuildingAsync(building);
            return Ok(response);
        }

        [HttpGet("GetAllBuilding/paged")]
        public async Task<IActionResult> GetAllBuilding(int pageIndex, int pageSize)
        {
            if (pageIndex < 1 || pageSize < 1)
            {
                return BadRequest("Page index and page size must be greater than zero.");
            }

            var data = await _buildingService.GetAllBuildingsAsync(pageIndex, pageSize);
            return Ok(data);
        }

        [HttpGet("GetAllBuildingBasic")]
        public async Task<IActionResult> GetAllPropertyBasic()
        {
            var data = await _buildingService.GetAllBuildingBasicAsync();
            return Ok(data);
        }

        [HttpGet("GetByIdAsync")]
        public async Task<IActionResult> Get(long id)
        {
            var building = await _buildingService.GetBuildingByIdAsync(id);
            if (building == null)
            {
                return NotFound();
            }
            return Ok(building);
        }

        [HttpPost("EditBuilding")]
        public async Task<IActionResult> EditBuilding(BuildingAddEdit building)
        {
            var response = await _buildingService.UpdateBuildingAsync(building);
            return Ok(response);
        }

        [HttpDelete("DeleteBuilding")]
        public async Task<IActionResult> DeleteBuildingAsync(long id)
        {
            await _buildingService.DeleteBuildingAsync(id);
            return NoContent();
        }
    
    }
}
