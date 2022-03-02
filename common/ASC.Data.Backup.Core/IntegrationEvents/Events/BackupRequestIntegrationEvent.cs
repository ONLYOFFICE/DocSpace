namespace ASC.Data.Backup.Core.IntegrationEvents.Events;

public record BackupRequestIntegrationEvent : IntegrationEvent
{
    public BackupRequestIntegrationEvent(BackupStorageType storageType, 
                                  int tenantId,
                                  Guid createBy,
                                  Dictionary<string, string> storageParams,
                                  bool backupMail,
                                  bool isScheduled = false,
                                  int backupsStored = 0,
                                  string storageBasePath = "") : base(createBy, tenantId)
    {
        StorageType = storageType;
        StorageParams = storageParams;
        BackupMail = backupMail;
        IsScheduled = isScheduled;
        BackupsStored = backupsStored;
        StorageBasePath = storageBasePath;
    }

    public BackupStorageType StorageType { get; private init; }
    public Dictionary<string, string> StorageParams { get; private init; }
    public bool BackupMail { get; private init; }
    public bool IsScheduled { get; private init; }
    public int BackupsStored { get; private init; }
    public string StorageBasePath { get; private init; }
}

