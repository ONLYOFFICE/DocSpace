namespace ASC.Data.Backup.Core.IntegrationEvents.Events;

[ProtoContract]
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

    [ProtoMember(1)]
    public bool Notify { get; set; }

    [ProtoMember(2)]
    public string BackupId { get; set; }

    [ProtoMember(3)]
    public BackupStorageType StorageType { get; private init; }

    [ProtoMember(4)]
    public Dictionary<string, string> StorageParams { get; private init; }   
}

