using Application.Interfaces;
using Domain.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace WebAPI.Controller
{
    [AllowAnonymous]
    [Route("api/[controller]")]
    [ApiController]
    public class UnitController : ControllerBase
    {
        private readonly IUnitService _unitService;

        public UnitController(IUnitService unitService)
        {
            _unitService = unitService;
        }

        [HttpPost("AddUnit")]
        public async Task<IActionResult> CreateUnitAsync(UnitAddEdit unit)
        {
            var response = await _unitService.CreateUnitAsync(unit);
            return Ok(response);
        }

        [HttpGet("GetAllUnit/paged")]
        public async Task<IActionResult> GetAllUnit(int pageIndex, int pageSize)
        {
            if (pageIndex < 1 || pageSize < 1)
                return BadRequest("Page index and page size must be greater than zero.");

            var data = await _unitService.GetAllUnitsAsync(pageIndex, pageSize);
            return Ok(data);
        }

        [HttpGet("GetAllUnitBasic")]
        public async Task<IActionResult> GetAllUnitBasic()
        {
            var data = await _unitService.GetAllUnitBasicAsync();
            return Ok(data);
        }

        [HttpGet("GetByIdAsync")]
        public async Task<IActionResult> Get(long id)
        {
            var unit = await _unitService.GetUnitByIdAsync(id);
            if (unit == null)
                return NotFound();

            return Ok(unit);
        }

        [HttpPost("EditUnit")]
        public async Task<IActionResult> EditUnit(UnitAddEdit unit)
        {
            var response = await _unitService.UpdateUnitAsync(unit);
            return Ok(response);
        }

        [HttpDelete("DeleteUnit")]
        public async Task<IActionResult> DeleteUnitAsync(long id)
        {
            await _unitService.DeleteUnitAsync(id);
            return NoContent();
        }
        [HttpGet("GetUnitsByBuilding")]
        public async Task<IActionResult> GetUnitsByBuilding(long buildingId)
        {
            var data = await _unitService.GetUnitsByBuildingAsync(buildingId);
            return Ok(data);
        }
    }
}
