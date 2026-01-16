using Domain.Entities;
using Domain.ViewModels;
using Infrastructure.Context;
using Infrastructure.Integrations.Hikvision;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace Application.Services
{
    public sealed class DeviceSyncService
    {
        private readonly HikvisionClient _hik;
        private readonly AppDbContext _db;

        public DeviceSyncService(HikvisionClient hik, AppDbContext db)
        {
            _hik = hik;
            _db = db;
        }

        public async Task<int> SyncDevicesAsync(CancellationToken ct = default)
        {
            var response = await _hik.GetDeviceListAsync(ct);

            var list = response?.SearchResult?.MatchList ?? new List<MatchItem>();
            var now = DateTime.UtcNow;

            int upserts = 0;

            foreach (var item in list)
            {
                var d = item.Device;
                if (d?.DevIndex is null) continue;

                var existing = await _db.HikDevices
                    .FirstOrDefaultAsync(x => x.DevIndex == d.DevIndex, ct);

                if (existing == null)
                {
                    existing = new HikDevice { DevIndex = d.DevIndex };
                    _db.HikDevices.Add(existing);
                }

                existing.DevName = d.DevName;
                existing.IpAddress = d.ISAPIParams?.Address;
                existing.PortNo = d.ISAPIParams?.PortNo;
                existing.DevMode = d.DevMode;
                existing.DevType = d.DevType;
                existing.DevStatus = d.DevStatus;
                existing.ActiveStatus = d.ActiveStatus;
                existing.DevVersion = d.DevVersion;
                existing.ProtocolType = d.ProtocolType;
                existing.VideoChannelNum = d.VideoChannelNum;
                existing.LastSyncedAt = now;

                // optional: store raw payload per device
                existing.RawJson = JsonSerializer.Serialize(d);

                upserts++;
            }

            await _db.SaveChangesAsync(ct);
            return upserts;
        }
        public async Task<List<HikDeviceDto>> GetDevicesFromDbAsync(CancellationToken ct = default)
        {
            return await _db.HikDevices
                .AsNoTracking()
                .OrderByDescending(x => x.LastSyncedAt)
                .Select(x => new HikDeviceDto
                {
                    Id = x.Id, // ✅ add this
                    DevIndex = x.DevIndex,
                    DevName = x.DevName,
                    IpAddress = x.IpAddress,
                    PortNo = x.PortNo,
                    DevMode = x.DevMode,
                    DevType = x.DevType,
                    DevStatus = x.DevStatus,
                    ActiveStatus = x.ActiveStatus,
                    DevVersion = x.DevVersion,
                    ProtocolType = x.ProtocolType,
                    VideoChannelNum = x.VideoChannelNum,
                    LastSyncedAt = x.LastSyncedAt
                })
                .ToListAsync(ct);
        }

        public async Task<HikDeviceDto?> GetDeviceFromDbAsync(string devIndex, CancellationToken ct = default)
        {
            return await _db.HikDevices
                .AsNoTracking()
                .Where(x => x.DevIndex == devIndex)
                .Select(x => new HikDeviceDto
                {
                    Id = x.Id, // ✅ add this
                    DevIndex = x.DevIndex,
                    DevName = x.DevName,
                    IpAddress = x.IpAddress,
                    PortNo = x.PortNo,
                    DevMode = x.DevMode,
                    DevType = x.DevType,
                    DevStatus = x.DevStatus,
                    ActiveStatus = x.ActiveStatus,
                    DevVersion = x.DevVersion,
                    ProtocolType = x.ProtocolType,
                    VideoChannelNum = x.VideoChannelNum,
                    LastSyncedAt = x.LastSyncedAt
                })
                .FirstOrDefaultAsync(ct);
        }
    }
}
