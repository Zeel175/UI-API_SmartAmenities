using Application.Interfaces;
using Domain.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;

namespace WebAPI.Controller
{
    [AllowAnonymous]
    [Route("api/[controller]")]
    [ApiController]
    public class AmenitySlotTemplateController : ControllerBase
    {
        private readonly IAmenitySlotTemplateService _slotTemplateService;

        public AmenitySlotTemplateController(IAmenitySlotTemplateService slotTemplateService)
        {
            _slotTemplateService = slotTemplateService;
        }

        [Route("AddSlotTemplate")]
        [HttpPost]
        public async Task<IActionResult> CreateSlotTemplateAsync(AmenitySlotTemplateAddEdit template)
        {
            var response = await _slotTemplateService.CreateSlotTemplateAsync(template);
            return Ok(response);
        }

        [Route("AddSlotTemplate/bulk")]
        [HttpPost]
        public async Task<IActionResult> CreateSlotTemplatesAsync(List<AmenitySlotTemplateAddEdit> templates)
        {
            var response = await _slotTemplateService.CreateSlotTemplatesAsync(templates);
            return Ok(response);
        }

        [HttpGet("GetAllSlotTemplate/paged")]
        public async Task<IActionResult> GetAllSlotTemplate(int pageIndex, int pageSize)
        {
            if (pageIndex < 1 || pageSize < 1)
            {
                return BadRequest("Page index and page size must be greater than zero.");
            }
            var data = await _slotTemplateService.GetSlotTemplatesAsync(pageIndex, pageSize);
            return Ok(data);
        }

        [HttpGet("GetByIdAsync")]
        public async Task<IActionResult> Get(long id)
        {
            var template = await _slotTemplateService.GetSlotTemplateByIdAsync(id);
            if (template == null)
            {
                return NotFound();
            }
            return Ok(template);
        }

        [Route("EditSlotTemplate")]
        [HttpPost]
        public async Task<IActionResult> EditSlotTemplate(AmenitySlotTemplateAddEdit template)
        {
            var response = await _slotTemplateService.UpdateSlotTemplateAsync(template);
            return Ok(response);
        }

        [HttpDelete("DeleteSlotTemplate")]
        public async Task<IActionResult> DeleteSlotTemplate(long id)
        {
            await _slotTemplateService.DeleteSlotTemplateAsync(id);
            return NoContent();
        }
    }
}
