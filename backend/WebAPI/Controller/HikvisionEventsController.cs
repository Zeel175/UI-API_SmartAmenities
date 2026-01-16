using Application.Helper;
using Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace WebAPI.Controller
{
    [AllowAnonymous]
    [Route("api/[controller]")]
    [ApiController]
    public class HikvisionEventsController : ControllerBase
    {
        private readonly IHikvisionSyncService _sync;
        private readonly ILogger<HikvisionEventsController> _logger;

        public HikvisionEventsController(IHikvisionSyncService sync, ILogger<HikvisionEventsController> logger)
        {
            _sync = sync;
            _logger = logger;
        }
        [AllowAnonymous]
        [HttpPost("events")]
        public async Task<IActionResult> ReceiveEvent([FromQuery] int? buildingId = null)
        {
            // Read raw body safely
            string raw;
            using (var reader = new StreamReader(Request.Body, System.Text.Encoding.UTF8, leaveOpen: true))
            {
                raw = await reader.ReadToEndAsync();
            }

            var logPath = Path.Combine(AppContext.BaseDirectory, "hik_events.log");

            var headers = string.Join("\n", Request.Headers.Select(h => $"{h.Key}: {h.Value}"));

            var text =
                $"\n===== {DateTime.Now:O} =====\n" +
                $"RemoteIP: {HttpContext.Connection.RemoteIpAddress}\n" +
                $"Method: {Request.Method}\n" +
                $"Path: {Request.Path}\n" +
                $"ContentType: {Request.ContentType}\n" +
                $"ContentLength: {Request.ContentLength}\n" +
                $"Headers:\n{headers}\n" +
                $"Body:\n{(string.IsNullOrWhiteSpace(raw) ? "(EMPTY BODY)" : raw)}\n";

            System.IO.File.AppendAllText(logPath, text);

            var employeeNo = HikvisionEventParser.TryGetEmployeeNo(raw);
            if (!string.IsNullOrWhiteSpace(employeeNo))
            {
                var remoteIpAddress = HttpContext.Connection.RemoteIpAddress;
                var remoteIp = remoteIpAddress?.IsIPv4MappedToIPv6 == true
                    ? remoteIpAddress.MapToIPv4().ToString()
                    : remoteIpAddress?.ToString();
                try
                {
                    await _sync.SyncUserBiometricStatusAsync(employeeNo, buildingId, remoteIp, HttpContext.RequestAborted);
                }
                catch (Exception ex)
                {
                    _logger.LogWarning(ex, "Failed to sync Hikvision biometric status for {EmployeeNo}.", employeeNo);
                }
            }

            return Ok(new { Status = "OK" });
        }

    }
}
