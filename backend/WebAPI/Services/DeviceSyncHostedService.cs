using Application.Helper;
using Application.Services;
using Microsoft.Extensions.Options;

namespace WebAPI.Services
{
    public sealed class DeviceSyncHostedService : BackgroundService
    {
        private readonly IServiceScopeFactory _scopeFactory;
        private readonly ILogger<DeviceSyncHostedService> _logger;
        private readonly DeviceSyncOptions _options;

        public DeviceSyncHostedService(
            IServiceScopeFactory scopeFactory,
            ILogger<DeviceSyncHostedService> logger,
            IOptions<DeviceSyncOptions> options)
        {
            _scopeFactory = scopeFactory;
            _logger = logger;
            _options = options.Value;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            if (_options.IntervalSeconds <= 0)
            {
                _logger.LogWarning("Device sync hosted service disabled because IntervalSeconds is {IntervalSeconds}", _options.IntervalSeconds);
                return;
            }

            using var timer = new PeriodicTimer(TimeSpan.FromSeconds(_options.IntervalSeconds));

            while (!stoppingToken.IsCancellationRequested &&
                   await timer.WaitForNextTickAsync(stoppingToken))
            {
                try
                {
                    using var scope = _scopeFactory.CreateScope();
                    var syncService = scope.ServiceProvider.GetRequiredService<DeviceSyncService>();
                    var updated = await syncService.SyncDevicesAsync(stoppingToken);
                    _logger.LogInformation("Background device sync completed. Updated {Count} devices.", updated);
                }
                catch (OperationCanceledException)
                {
                    return;
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Background device sync failed.");
                }
            }
        }
    }
}
