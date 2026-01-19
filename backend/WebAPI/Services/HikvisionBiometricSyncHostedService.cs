using Application.Helper;
using Application.Interfaces;
using Infrastructure.Context;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

namespace WebAPI.Services
{
    public sealed class HikvisionBiometricSyncHostedService : BackgroundService
    {
        private readonly IServiceScopeFactory _scopeFactory;
        private readonly ILogger<HikvisionBiometricSyncHostedService> _logger;
        private readonly BiometricSyncOptions _options;

        public HikvisionBiometricSyncHostedService(
            IServiceScopeFactory scopeFactory,
            ILogger<HikvisionBiometricSyncHostedService> logger,
            IOptions<BiometricSyncOptions> options)
        {
            _scopeFactory = scopeFactory;
            _logger = logger;
            _options = options.Value;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            if (_options.IntervalSeconds <= 0)
            {
                _logger.LogWarning("Biometric sync hosted service disabled because IntervalSeconds is {IntervalSeconds}", _options.IntervalSeconds);
                return;
            }

            using var timer = new PeriodicTimer(TimeSpan.FromSeconds(_options.IntervalSeconds));

            while (!stoppingToken.IsCancellationRequested &&
                   await timer.WaitForNextTickAsync(stoppingToken))
            {
                try
                {
                    _logger.LogInformation(
                        "Biometric sync cycle started. IntervalSeconds={IntervalSeconds}, MinResyncMinutes={MinResyncMinutes}, MaxUsersPerCycle={MaxUsersPerCycle}",
                        _options.IntervalSeconds,
                        _options.MinResyncMinutes,
                        _options.MaxUsersPerCycle);

                    using var scope = _scopeFactory.CreateScope();
                    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
                    var syncService = scope.ServiceProvider.GetRequiredService<IHikvisionSyncService>();

                    var residentQuery =
                        from r in db.ResidentMasters
                        join ru in db.ResidentMasterUnits on r.Id equals ru.ResidentMasterId
                        join u in db.Units on ru.UnitId equals u.Id
                        where r.IsActive
                            && ru.IsActive
                        orderby r.LastBiometricSyncUtc ?? DateTime.MinValue
                        select new { EmployeeNo = r.Code, BuildingId = (int?)u.BuildingId };

                    var residents = await residentQuery
                        .Distinct()
                        .Take(_options.MaxUsersPerCycle)
                        .ToListAsync(stoppingToken);

                    var remaining = Math.Max(0, _options.MaxUsersPerCycle - residents.Count);

                    var familyMembers = new List<(string EmployeeNo, int? BuildingId)>();
                    if (remaining > 0)
                    {
                        var familyQuery =
                            from f in db.ResidentFamilyMembers
                            join fu in db.ResidentFamilyMemberUnits on f.Id equals fu.ResidentFamilyMemberId
                            join u in db.Units on fu.UnitId equals u.Id
                            where f.IsActive
                                && fu.IsActive
                                orderby f.LastBiometricSyncUtc ?? DateTime.MinValue
                            select new { EmployeeNo = f.Code, BuildingId = (int?)u.BuildingId };

                        var family = await familyQuery
                            .Distinct()
                            .Take(remaining)
                            .ToListAsync(stoppingToken);

                        familyMembers.AddRange(family.Select(item => (item.EmployeeNo, item.BuildingId)));
                    }

                    foreach (var resident in residents)
                    {
                        if (!string.IsNullOrWhiteSpace(resident.EmployeeNo))
                            await syncService.SyncUserBiometricStatusAsync(resident.EmployeeNo, resident.BuildingId, null, stoppingToken);
                    }

                    foreach (var member in familyMembers)
                    {
                        if (!string.IsNullOrWhiteSpace(member.EmployeeNo))
                            await syncService.SyncUserBiometricStatusAsync(member.EmployeeNo, member.BuildingId, null, stoppingToken);
                    }

                    _logger.LogInformation(
                        "Biometric sync cycle completed. Residents={ResidentCount}, FamilyMembers={FamilyCount}",
                        residents.Count,
                        familyMembers.Count);
                }
                catch (OperationCanceledException)
                {
                    return;
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Biometric sync cycle failed.");
                }
            }
        }
    }
}
