namespace ASC.Data.Backup.Storage;

public interface IBackupRepository
{
    BackupRecord GetBackupRecord(Guid id);
    BackupRecord GetBackupRecord(string hash, int tenant);
    BackupSchedule GetBackupSchedule(int tenantId);
    List<BackupRecord> GetBackupRecordsByTenantId(int tenantId);
    List<BackupRecord> GetExpiredBackupRecords();
    List<BackupRecord> GetScheduledBackupRecords();
    List<BackupSchedule> GetBackupSchedules();
    void DeleteBackupRecord(Guid id);
    void DeleteBackupSchedule(int tenantId);
    void SaveBackupRecord(BackupRecord backupRecord);
    void SaveBackupSchedule(BackupSchedule schedule);
}
