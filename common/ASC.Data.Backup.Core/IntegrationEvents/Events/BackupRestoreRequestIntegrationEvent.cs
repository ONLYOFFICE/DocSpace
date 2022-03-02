namespace ASC.Data.Backup.Core.IntegrationEvents.Events;

public record BackupRestoreRequestIntegrationEvent : IntegrationEvent
{
    public BackupRestoreRequestIntegrationEvent(BackupStorageType storageType, 
                                  int tenantId,
                                  Guid createBy,
                                  Dictionary<string, string> storageParams,
                                  bool notify,
                                  string backupId
                                  ) : base(createBy, tenantId)
    {
        StorageType = storageType;
        StorageParams = storageParams;
        Notify = notify;
        BackupId = backupId;
    }

    public bool Notify { get; set; }
    public string BackupId { get; set; }
    public BackupStorageType StorageType { get; private init; }
    public Dictionary<string, string> StorageParams { get; private init; }
   
}

