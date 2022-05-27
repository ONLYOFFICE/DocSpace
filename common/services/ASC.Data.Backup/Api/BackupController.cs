namespace ASC.Data.Backup.Controllers;

[Scope]
[DefaultRoute]
[ApiController]
public class BackupController
{
    private readonly BackupAjaxHandler _backupHandler;
    private readonly CoreBaseSettings _coreBaseSettings;
    private readonly TenantExtra _tenantExtra;

    public BackupController(
        BackupAjaxHandler backupAjaxHandler,
        CoreBaseSettings coreBaseSettings,
        TenantExtra tenantExtra)
    {
        _backupHandler = backupAjaxHandler;
        _coreBaseSettings = coreBaseSettings;
        _tenantExtra = tenantExtra;
    }
    /// <summary>
    /// Returns the backup schedule of the current portal
    /// </summary>
    /// <category>Backup</category>
    /// <returns>Backup Schedule</returns>
    [Read("getbackupschedule")]
    public BackupAjaxHandler.Schedule GetBackupSchedule()
    {
        if (_coreBaseSettings.Standalone)
        {
            _tenantExtra.DemandControlPanelPermission();
        }

        return _backupHandler.GetSchedule();
    }

    /// <summary>
    /// Create the backup schedule of the current portal
    /// </summary>
    /// <param name="storageType">Storage type</param>
    /// <param name="storageParams">Storage parameters</param>
    /// <param name="backupsStored">Max of the backup's stored copies</param>
    /// <param name="cronParams">Cron parameters</param>
    /// <param name="backupMail">Include mail in the backup</param>
    /// <category>Backup</category>
    [Create("createbackupschedule")]
    public bool CreateBackupScheduleFromBody([FromBody] BackupScheduleDto backupSchedule)
    {
        return CreateBackupSchedule(backupSchedule);
    }

    [Create("createbackupschedule")]
    [Consumes("application/x-www-form-urlencoded")]
    public bool CreateBackupScheduleFromForm([FromForm] BackupScheduleDto backupSchedule)
    {
        return CreateBackupSchedule(backupSchedule);
    }

    private bool CreateBackupSchedule(BackupScheduleDto backupSchedule)
    {
        if (_coreBaseSettings.Standalone)
        {
            _tenantExtra.DemandControlPanelPermission();
        }
        var storageType = backupSchedule.StorageType == null ? BackupStorageType.Documents : (BackupStorageType)Int32.Parse(backupSchedule.StorageType);
        var storageParams = backupSchedule.StorageParams == null ? new Dictionary<string, string>() : backupSchedule.StorageParams.ToDictionary(r => r.Key.ToString(), r => r.Value.ToString());
        var backupStored = backupSchedule.BackupsStored == null ? 0 : Int32.Parse(backupSchedule.BackupsStored);
        var cron = new CronParams()
        {
            Period = backupSchedule.CronParams.Period == null ? BackupPeriod.EveryDay : (BackupPeriod)Int32.Parse(backupSchedule.CronParams.Period),
            Hour = backupSchedule.CronParams.Hour == null ? 0 : Int32.Parse(backupSchedule.CronParams.Hour),
            Day = backupSchedule.CronParams.Day == null ? 0 : Int32.Parse(backupSchedule.CronParams.Day),
        };
        _backupHandler.CreateSchedule(storageType, storageParams, backupStored, cron, backupSchedule.BackupMail);
        return true;
    }

    /// <summary>
    /// Delete the backup schedule of the current portal
    /// </summary>
    /// <category>Backup</category>
    [Delete("deletebackupschedule")]
    public bool DeleteBackupSchedule()
    {
        if (_coreBaseSettings.Standalone)
        {
            _tenantExtra.DemandControlPanelPermission();
        }

        _backupHandler.DeleteSchedule();

        return true;
    }

    /// <summary>
    /// Start a backup of the current portal
    /// </summary>
    /// <param name="storageType">Storage Type</param>
    /// <param name="storageParams">Storage Params</param>
    /// <param name="backupMail">Include mail in the backup</param>
    /// <category>Backup</category>
    /// <returns>Backup Progress</returns>
    [Create("startbackup")]
    public BackupProgress StartBackupFromBody([FromBody] BackupDto backup)
    {
        return StartBackup(backup);
    }

    [Create("startbackup")]
    [Consumes("application/x-www-form-urlencoded")]
    public BackupProgress StartBackupFromForm([FromForm] BackupDto backup)
    {
        return StartBackup(backup);
    }

    private BackupProgress StartBackup(BackupDto backup)
    {
        if (_coreBaseSettings.Standalone)
        {
            _tenantExtra.DemandControlPanelPermission();
        }
        var storageType = backup.StorageType == null ? BackupStorageType.Documents : (BackupStorageType)Int32.Parse(backup.StorageType);
        var storageParams = backup.StorageParams == null ? new Dictionary<string, string>() : backup.StorageParams.ToDictionary(r => r.Key.ToString(), r => r.Value.ToString());
        _backupHandler.StartBackup(storageType, storageParams, backup.BackupMail);
        return _backupHandler.GetBackupProgress();
    }

    /// <summary>
    /// Returns the progress of the started backup
    /// </summary>
    /// <category>Backup</category>
    /// <returns>Backup Progress</returns>
    [Read("getbackupprogress")]
    public BackupProgress GetBackupProgress()
    {
        if (_coreBaseSettings.Standalone)
        {
            _tenantExtra.DemandControlPanelPermission();
        }

        return _backupHandler.GetBackupProgress();
    }

    /// <summary>
    /// Returns the backup history of the started backup
    /// </summary>
    /// <category>Backup</category>
    /// <returns>Backup History</returns>
    [Read("getbackuphistory")]
    public List<BackupHistoryRecord> GetBackupHistory()
    {
        if (_coreBaseSettings.Standalone)
        {
            _tenantExtra.DemandControlPanelPermission();
        }

        return _backupHandler.GetBackupHistory();
    }

    /// <summary>
    /// Delete the backup with the specified id
    /// </summary>
    /// <category>Backup</category>
    [Delete("deletebackup/{id}")]
    public bool DeleteBackup(Guid id)
    {
        if (_coreBaseSettings.Standalone)
        {
            _tenantExtra.DemandControlPanelPermission();
        }

        _backupHandler.DeleteBackup(id);
        return true;
    }

    /// <summary>
    /// Delete all backups of the current portal
    /// </summary>
    /// <category>Backup</category>
    /// <returns>Backup History</returns>
    [Delete("deletebackuphistory")]
    public bool DeleteBackupHistory()
    {
        if (_coreBaseSettings.Standalone)
        {
            _tenantExtra.DemandControlPanelPermission();
        }

        _backupHandler.DeleteAllBackups();
        return true;
    }

    /// <summary>
    /// Start a data restore of the current portal
    /// </summary>
    /// <param name="backupId">Backup Id</param>
    /// <param name="storageType">Storage Type</param>
    /// <param name="storageParams">Storage Params</param>
    /// <param name="notify">Notify about backup to users</param>
    /// <category>Backup</category>
    /// <returns>Restore Progress</returns>
    [Create("startrestore")]
    public BackupProgress StartBackupRestoreFromBody([FromBody] BackupRestoreDto backupRestore)
    {
        return StartBackupRestore(backupRestore);
    }

    [Create("startrestore")]
    [Consumes("application/x-www-form-urlencoded")]
    public BackupProgress StartBackupRestoreFromForm([FromForm] BackupRestoreDto backupRestore)
    {
        return StartBackupRestore(backupRestore);
    }

    private BackupProgress StartBackupRestore(BackupRestoreDto backupRestore)
    {
        if (_coreBaseSettings.Standalone)
        {
            _tenantExtra.DemandControlPanelPermission();
        }
        var storageParams = backupRestore.StorageParams == null ? new Dictionary<string, string>() : backupRestore.StorageParams.ToDictionary(r => r.Key.ToString(), r => r.Value.ToString());
        _backupHandler.StartRestore(backupRestore.BackupId, (BackupStorageType)Int32.Parse(backupRestore.StorageType.ToString()), storageParams, backupRestore.Notify);
        return _backupHandler.GetBackupProgress();
    }

    /// <summary>
    /// Returns the progress of the started restore
    /// </summary>
    /// <category>Backup</category>
    /// <returns>Restore Progress</returns>
    [Read("getrestoreprogress", true)]  //NOTE: this method doesn't check payment!!!
    public BackupProgress GetRestoreProgress()
    {
        if (_coreBaseSettings.Standalone)
        {
            _tenantExtra.DemandControlPanelPermission();
        }

        return _backupHandler.GetRestoreProgress();
    }

    ///<visible>false</visible>
    [Read("backuptmp")]
    public object GetTempPath()
    {
        if (_coreBaseSettings.Standalone)
        {
            _tenantExtra.DemandControlPanelPermission();
        }

        return _backupHandler.GetTmpFolder();
    }

    ///<visible>false</visible>
    [Read("enablerestore")]
    public bool EnableRestore()
    {
        try
        {
            if (_coreBaseSettings.Standalone)
            {
                _tenantExtra.DemandControlPanelPermission();
            }
            _backupHandler.DemandPermissionsRestore();
            return true;
        }
        catch
        {
            return false;
        }
    }

    ///<visible>false</visible>
    [Read("enableAutoBackup")]
    public bool EnableAutoBackup()
    {
        try
        {
            if (_coreBaseSettings.Standalone)
            {
                _tenantExtra.DemandControlPanelPermission();
            }
            _backupHandler.DemandPermissionsAutoBackup();
            return true;
        }
        catch
        {
            return false;
        }
    }
}
