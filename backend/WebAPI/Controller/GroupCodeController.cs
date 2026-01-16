using Application.Helper;
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
    public class GroupCodeController : ControllerBase
    {
        private readonly IGroupCodeService _groupCodeService;

        public GroupCodeController(IGroupCodeService groupCodeService)
        {
            _groupCodeService = groupCodeService;
        }
        [Route("AddGroupCode")]
        [HttpPost]
        public async Task<IActionResult> CreateGroupCodeAsync(GroupCodeAddEdit groupCode)
        {
            var response = await _groupCodeService.CreateGroupCodeAsync(groupCode);
            return Ok(response);
        }

        [HttpGet("GetAllGroupCodeBasic")]
        public async Task<IActionResult> GetAllGroupCodeBasic()
        {
            var data = await _groupCodeService.GetAllGroupCodeBasicAsync();
            return Ok(data);
        }

        [HttpGet("GetAllGroupCodes/paged")]
        public async Task<IActionResult> GetAllGroupCodes(int pageIndex, int pageSize)
        {
            if (pageIndex < 1 || pageSize < 1)
            {
                return BadRequest("Page index and page size must be greater than zero.");
            }
            var data = await _groupCodeService.GetAllGroupCodesAsync(pageIndex, pageSize);
            return Ok(data);
        }

        [HttpGet("list/{groupName}")]
        public async Task<IActionResult> GetGroupCodeByGroupName(string groupName)
        {
            var data = await _groupCodeService.GetGroupCodeByGroupName(groupName);
            if (data != null)
            {
                return Ok(data);
            }
            return BadRequest();
        }

        [HttpPut("Activate/{id}/{isActive}")]
        public async Task<IActionResult> Activate(long id, bool isActive)
        {
            //int userId = User.Identity.GetUserId();

            var data = await _groupCodeService.Activate(id, isActive);
            if (data.IsSuccess)
            {
                return Ok(data);
            }
            return BadRequest(data);
        }
        [HttpGet("GetByIdAsync")]
        public async Task<IActionResult> Get(long id)
        {
            var company = await _groupCodeService.GetGroupCodeByIdAsync(id);
            if (company == null)
            {
                return NotFound();
            }
            return Ok(company);
        }

        [Route("EditGroupCode")]
        [HttpPost]
        public async Task<IActionResult> EditGroupCode(GroupCodeAddEdit groupCode)
        {
            var response = await _groupCodeService.UpdateGroupCodeAsync(groupCode);
            return Ok(response);
        }
    }
}
