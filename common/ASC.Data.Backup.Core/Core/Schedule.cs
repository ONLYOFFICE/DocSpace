namespace ASC.Data.Backup
{
    [Scope]
    public class Schedule
    {
        private readonly TenantManager _tenantManager;
        private readonly IOptionsMonitor<ILog> _options;
        private readonly TenantUtil _tenantUtil;

        public Schedule(IOptionsMonitor<ILog> options, TenantManager tenantManager, TenantUtil tenantUtil)
        {
            _options = options;
            _tenantManager = tenantManager;
            _tenantUtil = tenantUtil;
        }

        public bool IsToBeProcessed(BackupSchedule backupSchedule)
        {
            try
            {
                var cron = new CronExpression(backupSchedule.Cron);
                var tenant = _tenantManager.GetTenant(backupSchedule.TenantId);
                var tenantTimeZone = tenant.TimeZone;
                var culture = tenant.GetCulture();
                Thread.CurrentThread.CurrentCulture = culture;

                var lastBackupTime = backupSchedule.LastBackupTime.Equals(default)
                    ? DateTime.UtcNow.Date.AddSeconds(-1)
                    : _tenantUtil.DateTimeFromUtc(tenantTimeZone, backupSchedule.LastBackupTime);

                var nextBackupTime = cron.GetTimeAfter(lastBackupTime);

                if (!nextBackupTime.HasValue) return false;
                var now = _tenantUtil.DateTimeFromUtc(tenantTimeZone, DateTime.UtcNow);
                return nextBackupTime <= now;
            }
            catch (Exception e)
            {
                var log = _options.CurrentValue;
                log.Error("Schedule " + backupSchedule.TenantId, e);
                return false;
            }
        }
    }
}
