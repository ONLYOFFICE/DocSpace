namespace ASC.Data.Backup.Storage;

[Scope]
public class BackupRepository : IBackupRepository
{
    private BackupsContext _backupContext => _lazyBackupContext.Value;
    private readonly Lazy<BackupsContext> _lazyBackupContext;

    public BackupRepository(DbContextManager<BackupsContext> dbContactManager)
    {
        _lazyBackupContext = new Lazy<BackupsContext>(() => dbContactManager.Value);
    }

    public void SaveBackupRecord(BackupRecord backup)
    {
        _backupContext.AddOrUpdate(r => r.Backups, backup);
        _backupContext.SaveChanges();
    }

    public BackupRecord GetBackupRecord(Guid id)
    {
        return _backupContext.Backups.Find(id);
    }

    public BackupRecord GetBackupRecord(string hash, int tenant)
    {
        return _backupContext.Backups.AsNoTracking().AsQueryable().SingleOrDefault(b => b.Hash == hash && b.TenantId == tenant);
    }

    public List<BackupRecord> GetExpiredBackupRecords()
    {
        return _backupContext.Backups.AsNoTracking().Where(b => b.ExpiresOn != DateTime.MinValue && b.ExpiresOn <= DateTime.UtcNow).ToList();
    }

    public List<BackupRecord> GetScheduledBackupRecords()
    {
        return _backupContext.Backups.AsNoTracking().AsQueryable().Where(b => b.IsScheduled == true).ToList();
    }

    public List<BackupRecord> GetBackupRecordsByTenantId(int tenantId)
    {
        return _backupContext.Backups.AsNoTracking().AsQueryable().Where(b => b.TenantId == tenantId).ToList();
    }

    public void DeleteBackupRecord(Guid id)
    {
        var backup = _backupContext.Backups.Find(id);

        if (backup != null)
        {
            _backupContext.Backups.Remove(backup);
            _backupContext.SaveChanges();
        }
    }

    public void SaveBackupSchedule(BackupSchedule schedule)
    {
        _backupContext.AddOrUpdate(r => r.Schedules, schedule);
        _backupContext.SaveChanges();
    }

    public void DeleteBackupSchedule(int tenantId)
    {
        var shedule = _backupContext.Schedules.AsQueryable().Where(s => s.TenantId == tenantId).ToList();

        _backupContext.Schedules.RemoveRange(shedule);
        _backupContext.SaveChanges();
    }

    public List<BackupSchedule> GetBackupSchedules()
    {
        var query = _backupContext.Schedules.AsQueryable().Join(_backupContext.Tenants,
            s => s.TenantId,
            t => t.Id,
            (s, t) => new { schedule = s, tenant = t })
            .Where(q => q.tenant.Status == TenantStatus.Active)
            .Select(q => q.schedule);

        return query.ToList();
    }

    public BackupSchedule GetBackupSchedule(int tenantId)
    {
        return _backupContext.Schedules.AsNoTracking().SingleOrDefault(s => s.TenantId == tenantId);
    }
}
