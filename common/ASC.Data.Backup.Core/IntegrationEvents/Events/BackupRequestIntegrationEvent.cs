using System;
using System.Collections.Generic;

using ASC.Data.Backup.Contracts;
using ASC.EventBus.Events;

namespace ASC.Data.Backup.Core.IntegrationEvents.Events;

public record BackupRequestIntegrationEvent : IntegrationEvent
{
    public BackupRequestIntegrationEvent(BackupStorageType storageType, 
                                  int tenantId,
                                  Guid createBy,
                                  Dictionary<string, string> storageParams,
                                  bool backupMail) : base(createBy, tenantId)
    {
        StorageType = storageType;
        StorageParams = storageParams;
        BackupMail = backupMail;
    }

    public BackupStorageType StorageType { get; private init; }
    public Dictionary<string, string> StorageParams { get; private init; }
    public bool BackupMail { get; private init; }
}
