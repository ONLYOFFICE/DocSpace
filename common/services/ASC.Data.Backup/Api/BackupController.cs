// (c) Copyright Ascensio System SIA 2010-2022
//
// This program is a free software product.
// You can redistribute it and/or modify it under the terms
// of the GNU Affero General Public License (AGPL) version 3 as published by the Free Software
// Foundation. In accordance with Section 7(a) of the GNU AGPL its Section 15 shall be amended
// to the effect that Ascensio System SIA expressly excludes the warranty of non-infringement of
// any third-party rights.
//
// This program is distributed WITHOUT ANY WARRANTY, without even the implied warranty
// of MERCHANTABILITY or FITNESS FOR A PARTICULAR  PURPOSE. For details, see
// the GNU AGPL at: http://www.gnu.org/licenses/agpl-3.0.html
//
// You can contact Ascensio System SIA at Lubanas st. 125a-25, Riga, Latvia, EU, LV-1021.
//
// The  interactive user interfaces in modified source and object code versions of the Program must
// display Appropriate Legal Notices, as required under Section 5 of the GNU AGPL version 3.
//
// Pursuant to Section 7(b) of the License you must retain the original Product logo when
// distributing the program. Pursuant to Section 7(e) we decline to grant you any rights under
// trademark law for use of our trademarks.
//
// All the Product's GUI elements, including illustrations and icon sets, as well as technical writing
// content are licensed under the terms of the Creative Commons Attribution-ShareAlike 4.0
// International. See the License terms at http://creativecommons.org/licenses/by-sa/4.0/legalcode

namespace ASC.Data.Backup.Controllers;

[Scope]
[DefaultRoute]
[ApiController]
public class BackupController : ControllerBase
{
    private readonly BackupAjaxHandler _backupHandler;
    private readonly IEventBus _eventBus;
    private readonly Guid _currentUserId;
    private readonly int _tenantId;

    public BackupController(
        BackupAjaxHandler backupAjaxHandler,
        TenantManager tenantManager,
        SecurityContext securityContext,
        IEventBus eventBus)
    {
        _currentUserId = securityContext.CurrentAccount.ID;
        _tenantId = tenantManager.GetCurrentTenant().Id;
        _backupHandler = backupAjaxHandler;
        _eventBus = eventBus;
    }
    /// <summary>
    /// Returns the backup schedule of the current portal
    /// </summary>
    /// <category>Backup</category>
    /// <returns>Backup Schedule</returns>
    [HttpGet("getbackupschedule")]
    public BackupAjaxHandler.Schedule GetBackupSchedule()
    {
        return _backupHandler.GetSchedule();
    }

    /// <summary>
    /// Create the backup schedule of the current portal
    /// </summary>
    /// <param name="storageType">Storage type</param>
    /// <param name="storageParams">Storage parameters</param>
    /// <param name="backupsStored">Max of the backup's stored copies</param>
    /// <param name="cronParams">Cron parameters</param>
    /// <category>Backup</category>
    [HttpPost("createbackupschedule")]
    public bool CreateBackupSchedule(BackupScheduleDto backupSchedule)
    {
        var storageType = backupSchedule.StorageType == null ? BackupStorageType.Documents : (BackupStorageType)Int32.Parse(backupSchedule.StorageType);
        var storageParams = backupSchedule.StorageParams == null ? new Dictionary<string, string>() : backupSchedule.StorageParams.ToDictionary(r => r.Key.ToString(), r => r.Value.ToString());
        var backupStored = backupSchedule.BackupsStored == null ? 0 : Int32.Parse(backupSchedule.BackupsStored);
        var cron = new CronParams()
        {
            Period = backupSchedule.CronParams.Period == null ? BackupPeriod.EveryDay : (BackupPeriod)Int32.Parse(backupSchedule.CronParams.Period),
            Hour = backupSchedule.CronParams.Hour == null ? 0 : Int32.Parse(backupSchedule.CronParams.Hour),
            Day = backupSchedule.CronParams.Day == null ? 0 : Int32.Parse(backupSchedule.CronParams.Day),
        };
        _backupHandler.CreateSchedule(storageType, storageParams, backupStored, cron);
        return true;
    }

    /// <summary>
    /// Delete the backup schedule of the current portal
    /// </summary>
    /// <category>Backup</category>
    [HttpDelete("deletebackupschedule")]
    public bool DeleteBackupSchedule()
    {
        _backupHandler.DeleteSchedule();

        return true;
    }

    /// <summary>
    /// Start a backup of the current portal
    /// </summary>
    /// <param name="storageType">Storage Type</param>
    /// <param name="storageParams">Storage Params</param>
    /// <category>Backup</category>
    /// <returns>Backup Progress</returns>
    [AllowNotPayment]
    [HttpPost("startbackup")]
    public BackupProgress StartBackup(BackupDto backup)
    {
        var storageType = backup.StorageType == null ? BackupStorageType.Documents : (BackupStorageType)Int32.Parse(backup.StorageType);
        var storageParams = backup.StorageParams == null ? new Dictionary<string, string>() : backup.StorageParams.ToDictionary(r => r.Key.ToString(), r => r.Value.ToString());

        _eventBus.Publish(new BackupRequestIntegrationEvent(
             tenantId: _tenantId,
             storageParams: storageParams,
             storageType: storageType,
             createBy: _currentUserId
        ));

        return _backupHandler.GetBackupProgress();
    }

    /// <summary>
    /// Returns the progress of the started backup
    /// </summary>
    /// <category>Backup</category>
    /// <returns>Backup Progress</returns>
    [AllowNotPayment]
    [HttpGet("getbackupprogress")]
    public BackupProgress GetBackupProgress()
    {
        return _backupHandler.GetBackupProgress();
    }

    /// <summary>
    /// Returns the backup history of the started backup
    /// </summary>
    /// <category>Backup</category>
    /// <returns>Backup History</returns>
    [HttpGet("getbackuphistory")]
    public async Task<List<BackupHistoryRecord>> GetBackupHistory()
    {
        return await _backupHandler.GetBackupHistory();
    }

    /// <summary>
    /// Delete the backup with the specified id
    /// </summary>
    /// <category>Backup</category>
    [HttpDelete("deletebackup/{id}")]
    public async Task<bool> DeleteBackup(Guid id)
    {
        await _backupHandler.DeleteBackup(id);
        return true;
    }

    /// <summary>
    /// Delete all backups of the current portal
    /// </summary>
    /// <category>Backup</category>
    /// <returns>Backup History</returns>
    [HttpDelete("deletebackuphistory")]
    public async Task<bool> DeleteBackupHistory()
    {
        await _backupHandler.DeleteAllBackups();
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
    [HttpPost("startrestore")]
    public BackupProgress StartBackupRestore(BackupRestoreDto backupRestore)
    {
        var storageParams = backupRestore.StorageParams == null ? new Dictionary<string, string>() : backupRestore.StorageParams.ToDictionary(r => r.Key.ToString(), r => r.Value.ToString());

        _eventBus.Publish(new BackupRestoreRequestIntegrationEvent(
                             tenantId: _tenantId,
                             createBy: _currentUserId,
                             storageParams: storageParams,
                             storageType: (BackupStorageType)Int32.Parse(backupRestore.StorageType.ToString()),
                             notify: backupRestore.Notify,
                             backupId: backupRestore.BackupId
                        ));


        return _backupHandler.GetBackupProgress();
    }

    /// <summary>
    /// Returns the progress of the started restore
    /// </summary>
    /// <category>Backup</category>
    /// <returns>Restore Progress</returns>
    [HttpGet("getrestoreprogress")]  //NOTE: this method doesn't check payment!!!
    [AllowAnonymous]
    [AllowNotPayment]
    public BackupProgress GetRestoreProgress()
    {
        return _backupHandler.GetRestoreProgress();
    }

    ///<visible>false</visible>
    [HttpGet("backuptmp")]
    public object GetTempPath()
    {
        return _backupHandler.GetTmpFolder();
    }
}
