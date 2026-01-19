using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.ViewModels
{
    public class HikvisionLogsRequest
    {
        public DateTimeOffset Start { get; set; }
        public DateTimeOffset End { get; set; }

        public int BuildingId { get; set; }
        public int? UnitId { get; set; }

        // NEW: UI sends these
        public List<long> UserIds { get; set; } = new();
        public List<long> GuestIds { get; set; } = new();

        // Optional: keep this for direct testing / Postman
        public List<string> EmployeeNos { get; set; } = new();

        public bool AllUsers { get; set; } = false;
        public bool PicEnable { get; set; } = true;
    }

}
