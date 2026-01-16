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
    public class PropertyController : ControllerBase
    {
        private readonly IPropertyService _propertyService;

        public PropertyController(IPropertyService propertyService)
        {
            _propertyService = propertyService;
        }

        [Route("AddProperty")]
        [HttpPost]
        public async Task<IActionResult> CreatePropertyAsync(PropertyAddEdit property)
        {
            var response = await _propertyService.CreatePropertyAsync(property);
            return Ok(response);
        }
        [HttpGet("GetAllProperty/paged")]
        public async Task<IActionResult> GetAllProperty(int pageIndex, int pageSize)
        {
            if (pageIndex < 1 || pageSize < 1)
            {
                return BadRequest("Page index and page size must be greater than zero.");
            }
            var data = await _propertyService.GetAllPropertiesAsync(pageIndex, pageSize);
            return Ok(data);
        }

        [HttpGet("GetByIdAsync")]
        public async Task<IActionResult> Get(long id)
        {
            var property = await _propertyService.GetPropertyByIdAsync(id);
            if (property == null)
            {
                return NotFound();
            }
            return Ok(property);
        }

        [Route("EditProperty")]
        [HttpPost]
        public async Task<IActionResult> EditProperty(PropertyAddEdit property)
        {
            var response = await _propertyService.UpdatePropertyAsync(property);
            return Ok(response);
        }

        [HttpDelete("DeleteProperty")]
        public async Task<IActionResult> DeletePropertyAsync(long id)
        {
            //long userId = 1;
            //User.Identity.GetUserId();
            await _propertyService.DeletePropertyAsync(id);
            return NoContent();
        }
        [HttpGet("GetAllPropertyBasic")]
        public async Task<IActionResult> GetAllPropertyBasic()
        {
            var data = await _propertyService.GetAllPropertiesBasicAsync();
            return Ok(data);
        }






    }
}