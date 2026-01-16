using Hangfire;
using Microsoft.AspNetCore.Mvc;
using NutemHangfire.Services;

namespace NutemHangfire.Controllers
{
    public class BackgroundJobController : Controller
    {
        public IBackgroundJobClient _backgroundJobs;
        public BackgroundJobService _backgroundJobService;

        public BackgroundJobController(IBackgroundJobClient backgroundJobs, BackgroundJobService backgroundJobService)
        {
            _backgroundJobs = backgroundJobs;
            _backgroundJobService = backgroundJobService;
        }

        public IActionResult Start()
        {
            RecurringJob.AddOrUpdate("Fetch Production Orders daily in the morning at 04:00 AM ", () => _backgroundJobService.GetProductionOrders(), "00 04 * * *", TimeZoneInfo.Local);

            return Ok();
        }
    }
}
