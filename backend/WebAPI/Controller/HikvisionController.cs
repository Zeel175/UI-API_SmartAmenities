using Application.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace WebAPI.Controller
{
    [AllowAnonymous]
    [Route("api/[controller]")]
    [ApiController]
    public sealed class HikvisionController : ControllerBase
    {
        private readonly DeviceSyncService _sync;

        public HikvisionController(DeviceSyncService sync) => _sync = sync;

        [HttpPost("sync-devices")]
        public async Task<IActionResult> SyncDevices(CancellationToken ct)
        {
            try
            {
                var count = await _sync.SyncDevicesAsync(ct);
                return Ok(new { message = "Synced devices", count });
            }
            catch (Exception ex)
            {
                return StatusCode(502, new { message = ex.Message }); // 502 = bad gateway (upstream failed)
            }
        }
        // ✅ Read from DB
        [HttpGet("devices")]
        public async Task<IActionResult> GetDevices(CancellationToken ct)
        {
            var devices = await _sync.GetDevicesFromDbAsync(ct);
            return Ok(devices);
        }

        [HttpGet("devices/{devIndex}")]
        public async Task<IActionResult> GetDevice(string devIndex, CancellationToken ct)
        {
            var device = await _sync.GetDeviceFromDbAsync(devIndex, ct);
            if (device == null) return NotFound(new { message = "Device not found" });
            return Ok(device);
        }
    }
}
