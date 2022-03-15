namespace ASC.Data.Backup.Services;

[Scope]
public class BackupService : IBackupService
{
    private readonly ILog _logger;
    private readonly BackupStorageFactory _backupStorageFactory;
    private readonly BackupWorker _backupWorker;
    private readonly BackupRepository _backupRepository;
    private readonly ConfigurationExtension _configuration;

    public BackupService(
        IOptionsMonitor<ILog> options,
        BackupStorageFactory backupStorageFactory,
        BackupWorker backupWorker,
        BackupRepository backupRepository,
        ConfigurationExtension configuration)
    {
        _logger = options.CurrentValue;
        _backupStorageFactory = backupStorageFactory;
        _backupWorker = backupWorker;
        _backupRepository = backupRepository;
        _configuration = configuration;
    }

    public void StartBackup(StartBackupRequest request)
    {
        var progress = _backupWorker.StartBackup(request);
        if (!string.IsNullOrEmpty(progress.Error))
        {
            throw new FaultException();
        }
    }

    public void DeleteBackup(Guid id)
    {
        var backupRecord = _backupRepository.GetBackupRecord(id);
        _backupRepository.DeleteBackupRecord(backupRecord.Id);

        var storage = _backupStorageFactory.GetBackupStorage(backupRecord);
        if (storage == null)
        {
            return;
        }

        storage.Delete(backupRecord.StoragePath);
    }

    public void DeleteAllBackups(int tenantId)
    {
        foreach (var backupRecord in _backupRepository.GetBackupRecordsByTenantId(tenantId))
        {
            try
            {
                _backupRepository.DeleteBackupRecord(backupRecord.Id);
                var storage = _backupStorageFactory.GetBackupStorage(backupRecord);
                if (storage == null)
                {
                    continue;
                }

                storage.Delete(backupRecord.StoragePath);
            }
            catch (Exception error)
            {
                _logger.Warn("error while removing backup record: {0}", error);
            }
        }
    }

    public List<BackupHistoryRecord> GetBackupHistory(int tenantId)
    {
        var backupHistory = new List<BackupHistoryRecord>();
        foreach (var record in _backupRepository.GetBackupRecordsByTenantId(tenantId))
        {
            var storage = _backupStorageFactory.GetBackupStorage(record);
            if (storage == null)
            {
                continue;
            }

            if (storage.IsExists(record.StoragePath))
            {
                backupHistory.Add(new BackupHistoryRecord
                {
                    Id = record.Id,
                    FileName = record.Name,
                    StorageType = record.StorageType,
                    CreatedOn = record.CreatedOn,
                    ExpiresOn = record.ExpiresOn
                });
            }
            else
            {
                _backupRepository.DeleteBackupRecord(record.Id);
            }
        }
        return backupHistory;
    }

    public void StartTransfer(StartTransferRequest request)
    {
        var progress = _backupWorker.StartTransfer(request.TenantId, request.TargetRegion, request.BackupMail, request.NotifyUsers);
        if (!string.IsNullOrEmpty(progress.Error))
        {
            throw new FaultException();
        }
    }

    public void StartRestore(StartRestoreRequest request)
    {
        if (request.StorageType == BackupStorageType.Local)
        {
            if (string.IsNullOrEmpty(request.FilePathOrId) || !File.Exists(request.FilePathOrId))
            {
                throw new FileNotFoundException();
            }
        }

        if (!request.BackupId.Equals(Guid.Empty))
        {
            var backupRecord = _backupRepository.GetBackupRecord(request.BackupId);
            if (backupRecord == null)
            {
                throw new FileNotFoundException();
            }

            request.FilePathOrId = backupRecord.StoragePath;
            request.StorageType = backupRecord.StorageType;
            request.StorageParams = JsonConvert.DeserializeObject<Dictionary<string, string>>(backupRecord.StorageParams);
        }

        var progress = _backupWorker.StartRestore(request);
        if (!string.IsNullOrEmpty(progress.Error))
        {
            throw new FaultException();
        }
    }

    public BackupProgress GetBackupProgress(int tenantId)
    {
        return _backupWorker.GetBackupProgress(tenantId);
    }

    public BackupProgress GetTransferProgress(int tenantId)
    {
        return _backupWorker.GetTransferProgress(tenantId);
    }

    public BackupProgress GetRestoreProgress(int tenantId)
    {
        return _backupWorker.GetRestoreProgress(tenantId);
    }

    public string GetTmpFolder()
    {
        return _backupWorker.TempFolder;
    }

    public List<TransferRegion> GetTransferRegions()
    {
        var settings = _configuration.GetSetting<BackupSettings>("backup");

        return settings.WebConfigs.Elements.Select(configElement =>
        {
            var config = Utils.ConfigurationProvider.Open(PathHelper.ToRootedConfigPath(configElement.Path));
            var baseDomain = config.AppSettings.Settings["core:base-domain"].Value;

            return new TransferRegion
            {
                Name = configElement.Region,
                BaseDomain = baseDomain,
                IsCurrentRegion = configElement.Region.Equals(settings.WebConfigs.CurrentRegion, StringComparison.InvariantCultureIgnoreCase)
            };
        })
        .ToList();
    }

    public void CreateSchedule(CreateScheduleRequest request)
    {
        _backupRepository.SaveBackupSchedule(
            new BackupSchedule()
            {
                TenantId = request.TenantId,
                Cron = request.Cron,
                BackupMail = request.BackupMail,
                BackupsStored = request.NumberOfBackupsStored,
                StorageType = request.StorageType,
                StorageBasePath = request.StorageBasePath,
                StorageParams = JsonConvert.SerializeObject(request.StorageParams)
            });
    }

    public void DeleteSchedule(int tenantId)
    {
        _backupRepository.DeleteBackupSchedule(tenantId);
    }

    public ScheduleResponse GetSchedule(int tenantId)
    {
        var schedule = _backupRepository.GetBackupSchedule(tenantId);
        if (schedule != null)
        {
            var tmp = new ScheduleResponse
            {
                StorageType = schedule.StorageType,
                StorageBasePath = schedule.StorageBasePath,
                BackupMail = schedule.BackupMail,
                NumberOfBackupsStored = schedule.BackupsStored,
                Cron = schedule.Cron,
                LastBackupTime = schedule.LastBackupTime,
                StorageParams = JsonConvert.DeserializeObject<Dictionary<string, string>>(schedule.StorageParams)
            };

            return tmp;
        }
        else
        {
            return null;
        }
    }
}
