using Application.Interfaces;
using Domain.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace WebAPI.Controller
{
    [AllowAnonymous]
    [Route("api/[controller]")]
    [ApiController]
    public class AmenityUnitMasterController : ControllerBase
    {
        private readonly IAmenityUnitService _unitService;

        public AmenityUnitMasterController(IAmenityUnitService unitService)
        {
            _unitService = unitService;
        }

        [Route("AddAmenityUnit")]
        [HttpPost]
        public async Task<IActionResult> CreateAmenityUnitAsync(AmenityUnitAddEdit unit)
        {
            var response = await _unitService.CreateAmenityUnitAsync(unit);
            return Ok(response);
        }

        [Route("EditAmenityUnit")]
        [HttpPost]
        public async Task<IActionResult> EditAmenityUnitAsync(AmenityUnitAddEdit unit)
        {
            var response = await _unitService.UpdateAmenityUnitAsync(unit);
            return Ok(response);
        }

        [HttpGet("GetAllAmenityUnit/paged")]
        public async Task<IActionResult> GetAllAmenityUnit(int pageIndex, int pageSize)
        {
            if (pageIndex < 1 || pageSize < 1)
            {
                return BadRequest("Page index and page size must be greater than zero.");
            }
            var data = await _unitService.GetAmenityUnitsAsync(pageIndex, pageSize);
            return Ok(data);
        }

        [HttpGet("GetAllAmenityUnit")]
        public async Task<IActionResult> GetAllAmenityUnit()
        {
            var data = await _unitService.GetAmenityUnitsAsync();
            return Ok(data);
        }

        [HttpGet("GetByAmenityId")]
        public async Task<IActionResult> GetByAmenityId(long amenityId)
        {
            if (amenityId <= 0)
            {
                return BadRequest("AmenityId must be greater than zero.");
            }

            var data = await _unitService.GetAmenityUnitsByAmenityIdAsync(amenityId);
            return Ok(data);
        }

        [HttpGet("GetByAmenityIdWithSlots")]
        public async Task<IActionResult> GetByAmenityIdWithSlots(long amenityId)
        {
            if (amenityId <= 0)
            {
                return BadRequest("AmenityId must be greater than zero.");
            }

            var data = await _unitService.GetAmenityUnitsWithSlotsByAmenityIdAsync(amenityId);
            return Ok(data);
        }

        [HttpGet("GetByIdAsync")]
        public async Task<IActionResult> Get(long id)
        {
            var unit = await _unitService.GetAmenityUnitByIdAsync(id);
            if (unit == null)
            {
                return NotFound();
            }
            return Ok(unit);
        }

        [HttpDelete("DeleteAmenityUnit")]
        public async Task<IActionResult> DeleteAmenityUnit(long id)
        {
            await _unitService.DeleteAmenityUnitAsync(id);
            return NoContent();
        }
    }
}
