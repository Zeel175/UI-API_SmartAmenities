using Infrastructure.Integrations.Hikvision;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace Application.Services
{
    public sealed class HikvisionAlertStreamWorker : BackgroundService
    {
        private readonly ILogger<HikvisionAlertStreamWorker> _logger;

        public HikvisionAlertStreamWorker(ILogger<HikvisionAlertStreamWorker> logger)
        {
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("HikvisionAlertStreamWorker started");

            var devices = new[]
            {
        new DeviceInfo("192.168.1.187", 80, "admin", "Prama@12", 2),
    };

            var tasks = devices.Select(d => ListenDeviceForever(d, stoppingToken));
            await Task.WhenAll(tasks);
        }

        private async Task ListenDeviceForever(DeviceInfo d, CancellationToken ct)
        {
            while (!ct.IsCancellationRequested)
            {
                try
                {
                    await ListenOnce(d, ct);
                }
                catch (Exception ex)
                {
                    _logger.LogWarning(ex, "alertStream disconnected for {Ip}. Reconnecting...", d.Ip);
                    await Task.Delay(TimeSpan.FromSeconds(5), ct);
                }
            }
        }

        private async Task ListenOnce(DeviceInfo d, CancellationToken ct)
        {
            _logger.LogInformation("Connecting to {Ip}:{Port}", d.Ip, d.Port);

            var handler = new HttpClientHandler
            {
                Credentials = new NetworkCredential(d.Username, d.Password),
                PreAuthenticate = false
            };

            using var http = new HttpClient(handler)
            {
                Timeout = Timeout.InfiniteTimeSpan
            };

            var url = $"http://{d.Ip}:{d.Port}/ISAPI/Event/notification/alertStream";
            using var req = new HttpRequestMessage(HttpMethod.Get, url);

            using var resp = await http.SendAsync(req, HttpCompletionOption.ResponseHeadersRead, ct);
            _logger.LogInformation("Response {StatusCode} from {Ip}", (int)resp.StatusCode, d.Ip);
            resp.EnsureSuccessStatusCode();

            var contentType = resp.Content.Headers.ContentType?.ToString() ?? "";
            _logger.LogInformation("alertStream Content-Type: {ContentType}", contentType);

            await using var stream = await resp.Content.ReadAsStreamAsync(ct);

            // multipart boundary ho to multipart parse, warna plain xml parse
            var boundary = ExtractBoundary(contentType);
            if (!string.IsNullOrWhiteSpace(boundary))
            {
                _logger.LogInformation("Connected (multipart) to alertStream {Ip}", d.Ip);
                await ReadMultipartLoop(stream, boundary, d, ct);
            }
            else
            {
                _logger.LogInformation("Connected (plain) to alertStream {Ip}", d.Ip);
                await ReadPlainXmlLoop(stream, d, ct);
            }
        }

        private async Task ReadMultipartLoop(Stream stream, string boundary, DeviceInfo d, CancellationToken ct)
        {
            // NOTE: alertStream me XML ke saath kabhi-kabhi binary parts (images) bhi aa sakte hain. :contentReference[oaicite:6]{index=6}
            // Access control terminals me aksar XML hi aata hai, but safe parsing rakho.

            var reader = new StreamReader(stream, Encoding.UTF8);
            var boundaryLine = "--" + boundary;

            while (!ct.IsCancellationRequested)
            {
                var line = await reader.ReadLineAsync();
                if (line == null) break;

                if (!line.StartsWith(boundaryLine, StringComparison.Ordinal)) continue;

                // part headers
                string? contentType = null;
                int? contentLength = null;

                while (true)
                {
                    var h = await reader.ReadLineAsync();
                    if (h == null) return;
                    if (h.Length == 0) break; // blank line => body starts

                    if (h.StartsWith("Content-Type:", StringComparison.OrdinalIgnoreCase))
                        contentType = h.Split(':', 2)[1].Trim();
                    if (h.StartsWith("Content-Length:", StringComparison.OrdinalIgnoreCase)
                        && int.TryParse(h.Split(':', 2)[1].Trim(), out var len))
                        contentLength = len;
                }

                // body
                if (contentLength is null)
                {
                    // fallback: agar content-length na ho, to next boundary tak read karna padega (complex)
                    continue;
                }

                var buffer = new char[contentLength.Value];
                var read = 0;
                while (read < buffer.Length)
                {
                    var n = await reader.ReadAsync(buffer, read, buffer.Length - read);
                    if (n == 0) break;
                    read += n;
                }

                var body = new string(buffer, 0, read).Trim();
                _logger.LogInformation("Connected to alertStream {Ip}", d.Ip);
                // XML event
                if (contentType?.Contains("xml", StringComparison.OrdinalIgnoreCase) == true
                    && body.Contains("EventNotificationAlert", StringComparison.OrdinalIgnoreCase))
                {
                    _logger.LogInformation("Event from Building {BuildingId} / {Ip}: {Xml}",
                        d.BuildingId, d.Ip, body);

                    // TODO:
                    // 1) XML parse -> eventType/eventState/dateTime/employeeNo etc
                    // 2) DB me raw event + flags update (HasFace/HasFingerprint) trigger
                }
            }
        }

        private static string? ExtractBoundary(string contentType)
        {
            // e.g. "multipart/mixed; boundary=boundary"
            var parts = contentType.Split(';', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
            var b = parts.FirstOrDefault(p => p.StartsWith("boundary=", StringComparison.OrdinalIgnoreCase));
            return b?.Split('=', 2)[1].Trim().Trim('"');
        }

        private sealed record DeviceInfo(string Ip, int Port, string Username, string Password, int BuildingId);

        private async Task ReadPlainXmlLoop(Stream stream, DeviceInfo d, CancellationToken ct)
        {
            using var reader = new StreamReader(stream, Encoding.UTF8);
            var sb = new StringBuilder();
            bool collecting = false;

            while (!ct.IsCancellationRequested)
            {
                var line = await reader.ReadLineAsync();
                if (line == null) break;

                // heartbeat ignore
                if (!collecting && (line == "1" || string.IsNullOrWhiteSpace(line)))
                    continue;

                if (!collecting && line.Contains("<EventNotificationAlert", StringComparison.OrdinalIgnoreCase))
                {
                    collecting = true;
                    sb.Clear();
                }

                if (collecting)
                {
                    sb.AppendLine(line);

                    if (line.Contains("</EventNotificationAlert>", StringComparison.OrdinalIgnoreCase))
                    {
                        collecting = false;
                        var xml = sb.ToString();
                        await HandleEventXmlAsync(xml, d, ct);
                    }
                }
            }
        }
        private async Task HandleEventXmlAsync(string xml, DeviceInfo d, CancellationToken ct)
        {
            // 1) raw xml log file
            await File.AppendAllTextAsync("hik_events.log",
                $"[{DateTime.UtcNow:O}] Building={d.BuildingId} Ip={d.Ip}\n{xml}\n\n", ct);

            // 2) parse basic fields
            var (eventType, employeeNo) = ParseEvent(xml);

            _logger.LogInformation("EVENT: eventType={EventType}, employeeNo={EmployeeNo}, buildingId={BuildingId}",
                eventType, employeeNo, d.BuildingId);

            // 3) DB update (aapka logic)
            await UpdateBiometricFlagsAsync(employeeNo, d.BuildingId, ct);
        }
        private static (string? eventType, string? employeeNo) ParseEvent(string xml)
        {
            var doc = XDocument.Parse(xml);

            string? eventType = doc.Descendants().FirstOrDefault(x => x.Name.LocalName == "eventType")?.Value;
            string? employeeNo = doc.Descendants().FirstOrDefault(x => x.Name.LocalName == "employeeNo")?.Value;

            return (eventType, employeeNo);
        }
        private async Task UpdateBiometricFlagsAsync(string? employeeNo, int buildingId, CancellationToken ct)
        {
            if (string.IsNullOrWhiteSpace(employeeNo))
                return;

            // TODO: yaha aap apne HikvisionClient se check karoge
            bool hasFace = await _hikvisionClient.FaceExistsAsync(buildingId, employeeNo, ct);
            bool hasFp = await _hikvisionClient.FingerprintExistsAsync(buildingId, employeeNo, ct);

            // ✅ DB update: ResidentMaster / ResidentFamilyMember / User table me
            await _residentRepo.UpdateBiometricFlagsAsync(employeeNo, hasFace, hasFp, ct);
        }

    }
}
