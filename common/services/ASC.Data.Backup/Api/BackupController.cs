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

/// <summary>
/// Backup API.
/// </summary>
/// <name>backup</name>
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
    /// Returns the backup schedule of the current portal.
    /// </summary>
    /// <short>Get the backup schedule</short>
    /// <returns>Backup schedule: storage type, storage parameters, cron parameters, maximum number of the stored backup copies, last backup creation time</returns>
    /// <httpMethod>GET</httpMethod>
    /// <path>api/2.0/backup/getbackupschedule</path>
    [HttpGet("getbackupschedule")]
    public BackupAjaxHandler.Schedule GetBackupSchedule()
    {
        return _backupHandler.GetSchedule();
    }

    /// <summary>
    /// Creates the backup schedule of the current portal with the parameters specified in the request.
    /// </summary>
    /// <short>Create the backup schedule</short>
    /// <param type="ASC.Data.Backup.ApiModels.BackupScheduleDto, ASC.Data.Backup.ApiModels" name="backupSchedule">Backup schedule parameters: <![CDATA[
    /// <ul>
    ///     <li><b>StorageType</b> (string) - storage type,</li>
    ///     <li><b>StorageParams</b> (IEnumerable&lt;ItemKeyValuePair&lt;object, object&gt;&gt;) - storage parameters,</li>
    ///     <li><b>BackupsStored</b> (string) - maximum number of the stored backup copies,</li>
    ///     <li><b>CronParams</b> (Cron) - cron parameters:</li>
    ///     <ul>
    ///         <li><b>Period</b> (string) - period,</li>
    ///         <li><b>Hour</b> (string) - hour,</li>
    ///         <li><b>Day</b> (string) - day.</li>
    ///     </ul>
    /// </ul>
    /// ]]></param>
    /// <returns>Boolean value: true if the operation is successful</returns>
    /// <httpMethod>POST</httpMethod>
    /// <path>api/2.0/backup/createbackupschedule</path>
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
    /// Deletes the backup schedule of the current portal.
    /// </summary>
    /// <short>Delete the backup schedule</short>
    /// <returns>Boolean value: true if the operation is successful</returns>
    /// <httpMethod>DELETE</httpMethod>
    /// <path>api/2.0/backup/deletebackupschedule</path>
    [HttpDelete("deletebackupschedule")]
    public bool DeleteBackupSchedule()
    {
        _backupHandler.DeleteSchedule();

        return true;
    }

    /// <summary>
    /// Starts the backup of the current portal with the parameters specified in the request.
    /// </summary>
    /// <short>Start the backup</short>
    /// <param type="ASC.Data.Backup.ApiModels.BackupDto, ASC.Data.Backup.ApiModels" name="backup">Backup parameters: <![CDATA[
    /// <ul>
    ///     <li><b>StorageType</b> (string) - storage type,</li>
    ///     <li><b>StorageParams</b> (IEnumerable&lt;ItemKeyValuePair&lt;object, object&gt;&gt;) - storage parameters.</li>
    /// </ul>
    /// ]]></param>
    /// <returns>Backup progress: completed or not, progress percentage, error, tenant ID, backup progress item (Backup, Restore, Transfer), link</returns>
    /// <httpMethod>POST</httpMethod>
    /// <path>api/2.0/backup/startbackup</path>
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
    /// Returns the progress of the started backup.
    /// </summary>
    /// <short>Get the backup progress</short>
    /// <returns>Backup progress: completed or not, progress percentage, error, tenant ID, backup progress item (Backup, Restore, Transfer), link</returns>
    /// <httpMethod>GET</httpMethod>
    /// <path>api/2.0/backup/getbackupprogress</path>
    [AllowNotPayment]
    [HttpGet("getbackupprogress")]
    public BackupProgress GetBackupProgress()
    {
        return _backupHandler.GetBackupProgress();
    }

    /// <summary>
    /// Returns the history of the started backup.
    /// </summary>
    /// <short>Get the backup history</short>
    /// <returns>List of backup history records: backup ID, file name, storage type, creation date, expiration date</returns>
    /// <httpMethod>GET</httpMethod>
    /// <path>api/2.0/backup/getbackuphistory</path>
    [HttpGet("getbackuphistory")]
    public async Task<List<BackupHistoryRecord>> GetBackupHistory()
    {
        return await _backupHandler.GetBackupHistory();
    }

    /// <summary>
    /// Deletes the backup with the ID specified in the request.
    /// </summary>
    /// <short>Delete the backup</short>
    /// <param type="System.Guid, System" name="id">Backup ID</param>
    /// <returns>Boolean value: true if the operation is successful</returns>
    /// <httpMethod>DELETE</httpMethod>
    /// <path>api/2.0/backup/deletebackup/{id}</path>
    [HttpDelete("deletebackup/{id}")]
    public async Task<bool> DeleteBackup(Guid id)
    {
        await _backupHandler.DeleteBackup(id);
        return true;
    }

    /// <summary>
    /// Deletes the backup history of the current portal.
    /// </summary>
    /// <short>Delete the backup history</short>
    /// <returns>Boolean value: true if the operation is successful</returns>
    /// <httpMethod>DELETE</httpMethod>
    /// <path>api/2.0/backup/deletebackuphistory</path>
    [HttpDelete("deletebackuphistory")]
    public async Task<bool> DeleteBackupHistory()
    {
        await _backupHandler.DeleteAllBackups();
        return true;
    }

    /// <summary>
    /// Starts the data restoring process of the current portal with the parameters specified in the request.
    /// </summary>
    /// <short>Start the restoring process</short>
    /// <param type="ASC.Data.Backup.ApiModels.BackupRestoreDto, ASC.Data.Backup.ApiModels" name="backupRestore">Restoring parameters: <![CDATA[
    /// <ul>
    ///     <li><b>BackupId</b> (string) - backup ID,</li>
    ///     <li><b>StorageType</b> (object) - storage type,</li>
    ///     <li><b>StorageParams</b> (IEnumerable&lt;ItemKeyValuePair&lt;object, object&gt;&gt;) - storage parameters,</li>
    ///     <li><b>Notify</b> (bool) - notifies users about portal restoring process or not.</li>
    /// </ul>
    /// ]]></param>
    /// <returns>Backup progress: completed or not, progress percentage, error, tenant ID, backup progress item (Backup, Restore, Transfer), link</returns>
    /// <httpMethod>POST</httpMethod>
    /// <path>api/2.0/backup/startrestore</path>
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
    /// Returns the progress of the started restoring process.
    /// </summary>
    /// <short>Get the restoring progress</short>
    /// <returns>Backup progress: completed or not, progress percentage, error, tenant ID, backup progress item (Backup, Restore, Transfer), link</returns>
    /// <httpMethod>GET</httpMethod>
    /// <path>api/2.0/backup/getrestoreprogress</path>
    /// <requiresAuthorization>false</requiresAuthorization>
    [HttpGet("getrestoreprogress")]  //NOTE: this method doesn't check payment!!!
    [AllowAnonymous]
    [AllowNotPayment]
    public BackupProgress GetRestoreProgress()
    {
        return _backupHandler.GetRestoreProgress();
    }

    /// <summary>
    /// Returns a path to the temporary folder with the stored backup.
    /// </summary>
    /// <short>Get the temporary backup folder</short>
    /// <returns>Path to the temporary folder with the stored backup</returns>
    /// <httpMethod>GET</httpMethod>
    /// <path>api/2.0/backup/backuptmp</path>
    ///<visible>false</visible>
    [HttpGet("backuptmp")]
    public object GetTempPath()
    {
        return _backupHandler.GetTmpFolder();
    }
}
