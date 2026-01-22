using Application.Interfaces;
using Domain.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Hosting;

namespace WebAPI.Controller
{
    [AllowAnonymous]
    [Route("api/[controller]")]
    [ApiController]
    public class AmenityMasterController : ControllerBase
    {
        private readonly IAmenityMasterService _amenityService;
        private readonly IAmenityDocumentService _amenityDocumentService;
        private readonly IWebHostEnvironment _env;

        public AmenityMasterController(
            IAmenityMasterService amenityService,
            IAmenityDocumentService amenityDocumentService,
            IWebHostEnvironment env)
        {
            _amenityService = amenityService;
            _amenityDocumentService = amenityDocumentService;
            _env = env;
        }

        [Route("AddAmenity")]
        [HttpPost]
        public async Task<IActionResult> CreateAmenityAsync([FromForm] AmenityMasterAddEdit amenity)
        {
            var response = await _amenityService.CreateAmenityAsync(amenity);
            if (response.Id > 0 && amenity.Documents?.Count > 0)
            {
                await _amenityDocumentService.SaveDocumentsAsync(
                    response.Id,
                    amenity.Documents,
                    _env.WebRootPath);
            }
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

        [HttpGet("GetAllAmenityBasic")]
        public async Task<IActionResult> GetAllAmenityBasic()
        {
            var data = await _amenityService.GetAmenityBasicAsync();
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
        public async Task<IActionResult> EditAmenity([FromForm] AmenityMasterAddEdit amenity)
        {
            var response = await _amenityService.UpdateAmenityAsync(amenity);
            if (amenity.Id > 0 && amenity.Documents?.Count > 0)
            {
                await _amenityDocumentService.SaveDocumentsAsync(
                    amenity.Id,
                    amenity.Documents,
                    _env.WebRootPath);
            }
            return Ok(response);
        }

        [HttpDelete("DeleteAmenity")]
        public async Task<IActionResult> DeleteAmenity(long id)
        {
            await _amenityService.DeleteAmenityAsync(id);
            return NoContent();
        }

        [HttpDelete("Documents/Delete")]
        public async Task<IActionResult> DeleteAmenityDocument(long documentId)
        {
            var userId = 1;
            await _amenityDocumentService.DeleteDocumentAsync(documentId, userId);
            return NoContent();
        }
    }
}
