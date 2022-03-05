namespace ASC.Data.Backup.Core.IntegrationEvents.Events;

[ProtoContract]
public record BackupRequestIntegrationEvent : IntegrationEvent
{
    private BackupRequestIntegrationEvent() :base()
    {

    }

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

    [ProtoMember(1)]
    public BackupStorageType StorageType { get; private init; }

    [ProtoMember(2)]
    public Dictionary<string, string> StorageParams { get; private init; }

    [ProtoMember(3)]
    public bool BackupMail { get; private init; }

    [ProtoMember(4)]
    public bool IsScheduled { get; private init; }

    [ProtoMember(5)]
    public int BackupsStored { get; private init; }

    [ProtoMember(6)]
    public string StorageBasePath { get; private init; }
}

