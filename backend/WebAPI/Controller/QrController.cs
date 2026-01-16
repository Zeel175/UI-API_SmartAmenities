using Application.Services;
using Domain.Entities;
using Domain.Entities.Domain.Entities;
using Domain.ViewModels;
using Infrastructure.Context;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace WebAPI.Controller
{
    [AllowAnonymous]
    [ApiController]
    [Route("api/[controller]")]
    public class QrController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly ResidentMasterService _residentSvc; // or an IQrService
        private readonly GuestMasterService _guestSvc;

        public QrController(AppDbContext context, ResidentMasterService residentSvc, GuestMasterService guestSvc)
        {
            _context = context;
            _residentSvc = residentSvc;
            _guestSvc = guestSvc;
        }

        [HttpPost("Resolve")]
        public async Task<IActionResult> Resolve([FromBody] QrResolveRequest req, CancellationToken ct)
        {
            if (string.IsNullOrWhiteSpace(req?.Value))
                return BadRequest(new QrResolveResponse { IsSuccess = false, Message = "QR value is required." });

            var scanned = req.Value.Trim();

            // Try decrypt using your key; if decrypt fails, treat it as plain (helps if you ever disable encryption)
            string plain;
            try
            {
                plain = _guestSvc.GetType().GetMethod("DecryptQrValue", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance) != null
                        ? (string)_guestSvc.GetType().GetMethod("DecryptQrValue", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)!.Invoke(_guestSvc, new object[] { scanned })
                        : scanned;
            }
            catch
            {
                try { plain = (string)_residentSvc.GetType().GetMethod("DecryptQrValue", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)!.Invoke(_residentSvc, new object[] { scanned }); }
                catch { plain = scanned; }
            }

            var parts = plain.Split('|');

            // Resident parent: QrId|Code|ResidentId|UnitList  => 4 parts
            if (parts.Length == 4 && long.TryParse(parts[2], out var residentId))
            {
                var qrId = parts[0];
                var resident = await _context.Set<ResidentMaster>()
                    .AsNoTracking()
                    .FirstOrDefaultAsync(x => x.Id == residentId && x.IsActive, ct);

                if (resident == null || !string.Equals(resident.QrId, qrId, StringComparison.OrdinalIgnoreCase))
                    return BadRequest(new QrResolveResponse { IsSuccess = false, Message = "Invalid resident QR." });

                return Ok(new QrResolveResponse
                {
                    IsSuccess = true,
                    Type = "Resident",
                    ResidentId = resident.Id,
                    QrId = resident.QrId,
                    Message = "Resident QR verified."
                });
            }

            // Family: QrId|ResidentCode|ResidentId|FamilyId|UnitList => 5 parts
            if (parts.Length == 5 && long.TryParse(parts[3], out var familyId))
            {
                var qrId = parts[0];
                var member = await _context.Set<ResidentFamilyMember>()
                    .AsNoTracking()
                    .FirstOrDefaultAsync(x => x.Id == familyId && x.IsActive, ct);

                if (member == null || !string.Equals(member.QrId, qrId, StringComparison.OrdinalIgnoreCase))
                    return BadRequest(new QrResolveResponse { IsSuccess = false, Message = "Invalid family QR." });

                return Ok(new QrResolveResponse
                {
                    IsSuccess = true,
                    Type = "FamilyMember",
                    FamilyMemberId = member.Id,
                    ResidentId = member.ResidentMasterId,
                    QrId = member.QrId,
                    Message = "Family member QR verified."
                });
            }

            // Guest: QrId|Code|GuestId|UnitId|From|To => 6 parts
            if (parts.Length == 6 && long.TryParse(parts[2], out var guestId))
            {
                var qrId = parts[0];
                var guest = await _context.Set<GuestMaster>()
                    .AsNoTracking()
                    .FirstOrDefaultAsync(x => x.Id == guestId && x.IsActive, ct);

                if (guest == null || !string.Equals(guest.QrId, qrId, StringComparison.OrdinalIgnoreCase))
                    return BadRequest(new QrResolveResponse { IsSuccess = false, Message = "Invalid guest QR." });

                // optional: validity check
                var now = DateTime.Now;
                if (guest.FromDateTime.HasValue && now < guest.FromDateTime.Value)
                    return BadRequest(new QrResolveResponse { IsSuccess = false, Message = "Guest QR not active yet." });

                if (guest.ToDateTime.HasValue && now > guest.ToDateTime.Value)
                    return BadRequest(new QrResolveResponse { IsSuccess = false, Message = "Guest QR expired." });

                return Ok(new QrResolveResponse
                {
                    IsSuccess = true,
                    Type = "Guest",
                    GuestId = guest.Id,
                    QrId = guest.QrId,
                    Message = "Guest QR verified."
                });
            }

            return BadRequest(new QrResolveResponse { IsSuccess = false, Message = "QR format not recognized." });
        }
    }

}
