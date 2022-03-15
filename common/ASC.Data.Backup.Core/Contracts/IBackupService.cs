namespace ASC.Data.Backup.Contracts;

[ServiceContract]
public interface IBackupService
{
    BackupProgress GetBackupProgress(int tenantId);
    BackupProgress GetRestoreProgress(int tenantId);
    BackupProgress GetTransferProgress(int tenantId);
    List<BackupHistoryRecord> GetBackupHistory(int tenantId);
    List<TransferRegion> GetTransferRegions();
    ScheduleResponse GetSchedule(int tenantId);
    string GetTmpFolder();
    void CreateSchedule(CreateScheduleRequest request);
    void DeleteAllBackups(int tenantId);
    void DeleteBackup(Guid backupId);
    void DeleteSchedule(int tenantId);
    void StartBackup(StartBackupRequest request);
    void StartRestore(StartRestoreRequest request);
    void StartTransfer(StartTransferRequest request);
}
