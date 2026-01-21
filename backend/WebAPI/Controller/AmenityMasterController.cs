using Application.Interfaces;
using Domain.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace WebAPI.Controller
{
    [AllowAnonymous]
    [Route("api/[controller]")]
    [ApiController]
    public class AmenityMasterController : ControllerBase
    {
        private readonly IAmenityMasterService _amenityService;

        public AmenityMasterController(IAmenityMasterService amenityService)
        {
            _amenityService = amenityService;
        }

        [Route("AddAmenity")]
        [HttpPost]
        public async Task<IActionResult> CreateAmenityAsync(AmenityMasterAddEdit amenity)
        {
            var response = await _amenityService.CreateAmenityAsync(amenity);
            return Ok(response);
        }

        [HttpGet("GetAllAmenity/paged")]
        public async Task<IActionResult> GetAllAmenity(int pageIndex, int pageSize)
        {
            if (pageIndex < 1 || pageSize < 1)
            {
                return BadRequest("Page index and page size must be greater than zero.");
            }
            var data = await _amenityService.GetAmenitiesAsync(pageIndex, pageSize);
            return Ok(data);
        }

        [HttpGet("GetByIdAsync")]
        public async Task<IActionResult> Get(long id)
        {
            var amenity = await _amenityService.GetAmenityByIdAsync(id);
            if (amenity == null)
            {
                return NotFound();
            }
            return Ok(amenity);
        }

        [Route("EditAmenity")]
        [HttpPost]
        public async Task<IActionResult> EditAmenity(AmenityMasterAddEdit amenity)
        {
            var response = await _amenityService.UpdateAmenityAsync(amenity);
            return Ok(response);
        }

        [HttpDelete("DeleteAmenity")]
        public async Task<IActionResult> DeleteAmenity(long id)
        {
            await _amenityService.DeleteAmenityAsync(id);
            return NoContent();
        }
    }
}
