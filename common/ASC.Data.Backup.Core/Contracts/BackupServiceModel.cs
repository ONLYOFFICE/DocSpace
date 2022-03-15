namespace ASC.Data.Backup.Contracts;

public enum BackupStorageType
{
    Documents = 0,
    ThridpartyDocuments = 1,
    CustomCloud = 2,
    Local = 3,
    DataStore = 4,
    ThirdPartyConsumer = 5
}

public class StartBackupRequest
{
    public int TenantId { get; set; }
    public Guid UserId { get; set; }
    public bool BackupMail { get; set; }
    public BackupStorageType StorageType { get; set; }
    public string StorageBasePath { get; set; }
    public Dictionary<string, string> StorageParams { get; set; }
}

public class BackupHistoryRecord
{
    public Guid Id { get; set; }
    public string FileName { get; set; }
    public BackupStorageType StorageType { get; set; }
    public DateTime CreatedOn { get; set; }
    public DateTime ExpiresOn { get; set; }
}

public class StartTransferRequest
{
    public int TenantId { get; set; }
    public string TargetRegion { get; set; }
    public bool NotifyUsers { get; set; }
    public bool BackupMail { get; set; }
}

public class TransferRegion
{
    public string Name { get; set; }
    public string BaseDomain { get; set; }
    public bool IsCurrentRegion { get; set; }
}

public class StartRestoreRequest
{
    public int TenantId { get; set; }
    public Guid BackupId { get; set; }
    public BackupStorageType StorageType { get; set; }
    public string FilePathOrId { get; set; }
    public bool NotifyAfterCompletion { get; set; }
    public Dictionary<string, string> StorageParams { get; set; }
}

public class CreateScheduleRequest : StartBackupRequest
{
    public string Cron { get; set; }
    public int NumberOfBackupsStored { get; set; }
}

public class ScheduleResponse
{
    public BackupStorageType StorageType { get; set; }
    public string StorageBasePath { get; set; }
    public bool BackupMail { get; set; }
    public int NumberOfBackupsStored { get; set; }
    public string Cron { get; set; }
    public DateTime LastBackupTime { get; set; }
    public Dictionary<string, string> StorageParams { get; set; }
}
