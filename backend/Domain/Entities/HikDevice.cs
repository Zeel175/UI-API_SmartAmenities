using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Entities
{
    public sealed class HikDevice
    {
        public int Id { get; set; }
        public string DevIndex { get; set; } = "";   // Unique key
        public string? DevName { get; set; }
        public string? IpAddress { get; set; }
        public int? PortNo { get; set; }
        public string? DevMode { get; set; }
        public string? DevType { get; set; }
        public string? DevStatus { get; set; }
        public bool? ActiveStatus { get; set; }
        public string? DevVersion { get; set; }
        public string? ProtocolType { get; set; }
        public int? VideoChannelNum { get; set; }

        public DateTime LastSyncedAt { get; set; }
        public string? RawJson { get; set; } // optional (good for debugging)
    }

}
