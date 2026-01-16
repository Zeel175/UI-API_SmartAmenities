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
    public class GuestMasterController : ControllerBase
    {
        private readonly IGuestMasterService _guestMasterService;

        public GuestMasterController(IGuestMasterService guestMasterService)
        {
            _guestMasterService = guestMasterService;
        }

        [HttpPost("CreateGuest")]
        [Consumes("application/json")]
        public async Task<IActionResult> CreateAsync([FromBody] List<GuestMasterCreateRequestJson> request)
        {
            if (request == null || request.Count == 0)
            {
                return BadRequest("At least one guest is required.");
            }

            var responses = new List<InsertResponseModel>();

            foreach (var guestRequest in request)
            {
                var guest = new GuestMasterAddEdit
                {
                    FirstName = guestRequest.FirstName,
                    LastName = guestRequest.LastName,
                    Email = guestRequest.Email,
                    Mobile = guestRequest.Mobile,
                    CountryCode = guestRequest.CountryCode,
                    UnitId = guestRequest.UnitId,
                    CardId = guestRequest.CardId,  // ✅ IMPORTANT
                    QrId = guestRequest.QrId,
                    IsActive = guestRequest.IsActive,
                    FromDateTime = guestRequest.FromDateTime,
                    ToDateTime = guestRequest.ToDateTime
                };

                var response = await _guestMasterService.CreateGuestAsync(guest);

                if (response.Id <= 0)
                {
                    return StatusCode(500, response);
                }

                responses.Add(response);
            }

            return Ok(responses);
        }
        [HttpGet("List/paged")]
        public async Task<IActionResult> GetPagedAsync(int pageIndex, int pageSize)
        {
            if (pageIndex < 1 || pageSize < 1)
            {
                return BadRequest("Page index and page size must be greater than zero.");
            }

            var residents = await _guestMasterService.GetGuestsAsync(pageIndex, pageSize);
            return Ok(residents);
        }
        [HttpGet("GetAllGuest")]
        public async Task<IActionResult> GetAllGuest()
        {
            var guests = await _guestMasterService.GetAllGuestsAsync();
            return Ok(guests);
        }

        [HttpGet("GetById")]
        public async Task<IActionResult> GetById(long id)
        {
            var guest = await _guestMasterService.GetGuestByIdAsync(id);
            if (guest == null)
            {
                return NotFound();
            }

            return Ok(guest);
        }
        [HttpPost("EditGuest")]
        public async Task<IActionResult> EditGuest(GuestMasterAddEdit guest)
        {
            var response = await _guestMasterService.UpdateGuestAsync(guest);
            return Ok(response);
        }

        [HttpDelete("DeleteGuest")]
        public async Task<IActionResult> DeleteGuest(long id)
        {
            await _guestMasterService.DeleteGuestAsync(id);
            return NoContent();
        }
        [HttpGet("List/by-resident")]
        public async Task<IActionResult> GetByResident(long? residentMasterId, long? residentFamilyMemberId)
        {
            if (residentMasterId == null && residentFamilyMemberId == null)
            {
                return BadRequest("residentMasterId or residentFamilyMemberId is required.");
            }

            var guests = await _guestMasterService.GetGuestsByResidentAsync(residentMasterId, residentFamilyMemberId);
            return Ok(guests);
        }
        [HttpGet("{guestId:long}/qr-code-value")]
        [ProducesResponseType(typeof(QrCodeDecryptionResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> GetGuestDecryptedQr(long guestId, CancellationToken ct)
        {
            var result = await _guestMasterService.DecryptGuestQrCodeValueAsync(guestId, ct);

            if (!result.IsSuccess)
                return BadRequest(result);

            return Ok(result);
        }
        [HttpGet("by-qrid")]
        public async Task<IActionResult> GetByQrId([FromQuery] string qrId, CancellationToken ct)
        {
            var guest = await _guestMasterService.GetGuestByQrIdAsync(qrId, ct);
            if (guest == null) return NotFound("Guest not found for this QrId.");
            return Ok(guest);
        }

    }
}
