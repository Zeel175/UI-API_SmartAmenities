namespace Application.Helper
{
    public sealed class BiometricSyncOptions
    {
        public int IntervalSeconds { get; set; } = 300;
        public int MaxUsersPerCycle { get; set; } = 50;
        public int MinResyncMinutes { get; set; } = 30;
    }
}
