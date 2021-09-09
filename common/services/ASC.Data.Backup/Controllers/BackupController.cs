
using System;
using System.Collections.Generic;
using System.Linq;

using ASC.Common;
using ASC.Core;
using ASC.Data.Backup.Contracts;
using ASC.Data.Backup.ModelApi;
using ASC.Data.Backup.Models;
using ASC.Web.Api.Routing;
using ASC.Web.Studio.Utility;

using Microsoft.AspNetCore.Mvc;

using static ASC.Data.Backup.BackupAjaxHandler;

namespace ASC.Data.Backup.Controllers
{
    [Scope]
    [DefaultRoute]
    [ApiController]
    public class BackupController
    {
        private BackupAjaxHandler BackupHandler { get; }
        private CoreBaseSettings CoreBaseSettings { get; }
        private TenantExtra TenantExtra { get; }

        public BackupController(
            BackupAjaxHandler backupAjaxHandler,
            CoreBaseSettings coreBaseSettings,
            TenantExtra tenantExtra)
        {
            BackupHandler = backupAjaxHandler;
            CoreBaseSettings = coreBaseSettings;
            TenantExtra = tenantExtra;
        }
        /// <summary>
        /// Returns the backup schedule of the current portal
        /// </summary>
        /// <category>Backup</category>
        /// <returns>Backup Schedule</returns>
        [Read("getbackupschedule")]
        public BackupAjaxHandler.Schedule GetBackupSchedule()
        {
            if (CoreBaseSettings.Standalone)
            {
                TenantExtra.DemandControlPanelPermission();
            }

            return BackupHandler.GetSchedule();
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
        public bool CreateBackupScheduleFromBody([FromBody]BackupSchedule backupSchedule)
        {
            return CreateBackupSchedule(backupSchedule);
        }

        [Create("createbackupschedule")]
        [Consumes("application/x-www-form-urlencoded")]
        public bool CreateBackupScheduleFromForm([FromForm]BackupSchedule backupSchedule)
        {
            return CreateBackupSchedule(backupSchedule);
        }

        private bool CreateBackupSchedule(BackupSchedule backupSchedule)
        {
            if (CoreBaseSettings.Standalone)
            {
                TenantExtra.DemandControlPanelPermission();
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
            BackupHandler.CreateSchedule(storageType, storageParams, backupStored, cron, backupSchedule.BackupMail);
            return true;
        }

        /// <summary>
        /// Delete the backup schedule of the current portal
        /// </summary>
        /// <category>Backup</category>
        [Delete("deletebackupschedule")]
        public bool DeleteBackupSchedule()
        {
            if (CoreBaseSettings.Standalone)
            {
                TenantExtra.DemandControlPanelPermission();
            }

            BackupHandler.DeleteSchedule();

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
        public BackupProgress StartBackupFromBody([FromBody]Models.Backup backup)
        {
            return StartBackup(backup);
        }

        [Create("startbackup")]
        [Consumes("application/x-www-form-urlencoded")]
        public BackupProgress StartBackupFromForm([FromForm]Models.Backup backup)
        {
            return StartBackup(backup);
        }

        private BackupProgress StartBackup(Models.Backup backup)
        {
            if (CoreBaseSettings.Standalone)
            {
                TenantExtra.DemandControlPanelPermission();
            }
            var storageType = backup.StorageType == null ? BackupStorageType.Documents : (BackupStorageType)Int32.Parse(backup.StorageType);
            var storageParams = backup.StorageParams == null ? new Dictionary<string, string>() : backup.StorageParams.ToDictionary(r => r.Key.ToString(), r => r.Value.ToString());
            BackupHandler.StartBackup(storageType, storageParams, backup.BackupMail);
            return BackupHandler.GetBackupProgress();
        }

        /// <summary>
        /// Returns the progress of the started backup
        /// </summary>
        /// <category>Backup</category>
        /// <returns>Backup Progress</returns>
        [Read("getbackupprogress")]
        public BackupProgress GetBackupProgress()
        {
            if (CoreBaseSettings.Standalone)
            {
                TenantExtra.DemandControlPanelPermission();
            }

            return BackupHandler.GetBackupProgress();
        }

        /// <summary>
        /// Returns the backup history of the started backup
        /// </summary>
        /// <category>Backup</category>
        /// <returns>Backup History</returns>
        [Read("getbackuphistory")]
        public List<BackupHistoryRecord> GetBackupHistory()
        {
            if (CoreBaseSettings.Standalone)
            {
                TenantExtra.DemandControlPanelPermission();
            }

            return BackupHandler.GetBackupHistory();
        }

        /// <summary>
        /// Delete the backup with the specified id
        /// </summary>
        /// <category>Backup</category>
        [Delete("deletebackup/{id}")]
        public bool DeleteBackup(Guid id)
        {
            if (CoreBaseSettings.Standalone)
            {
                TenantExtra.DemandControlPanelPermission();
            }

            BackupHandler.DeleteBackup(id);
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
            if (CoreBaseSettings.Standalone)
            {
                TenantExtra.DemandControlPanelPermission();
            }

            BackupHandler.DeleteAllBackups();
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
        public BackupProgress StartBackupRestoreFromBody([FromBody]BackupRestore backupRestore)
        {
            return StartBackupRestore(backupRestore);
        }

        [Create("startrestore")]
        [Consumes("application/x-www-form-urlencoded")]
        public BackupProgress StartBackupRestoreFromForm([FromForm]BackupRestore backupRestore)
        {
            return StartBackupRestore(backupRestore);
        }

        private BackupProgress StartBackupRestore(BackupRestore backupRestore)
        {
            if (CoreBaseSettings.Standalone)
            {
                TenantExtra.DemandControlPanelPermission();
            }
            var storageParams = backupRestore.StorageParams == null ? new Dictionary<string, string>() : backupRestore.StorageParams.ToDictionary(r => r.Key.ToString(), r => r.Value.ToString());
            BackupHandler.StartRestore(backupRestore.BackupId, (BackupStorageType)Int32.Parse(backupRestore.StorageType.ToString()), storageParams, backupRestore.Notify);
            return BackupHandler.GetBackupProgress();
        }

        /// <summary>
        /// Returns the progress of the started restore
        /// </summary>
        /// <category>Backup</category>
        /// <returns>Restore Progress</returns>
        [Read("getrestoreprogress", true)]  //NOTE: this method doesn't check payment!!!
        public BackupProgress GetRestoreProgress()
        {
            if (CoreBaseSettings.Standalone)
            {
                TenantExtra.DemandControlPanelPermission();
            }

            return BackupHandler.GetRestoreProgress();
        }

        ///<visible>false</visible>
        [Read("backuptmp")]
        public object GetTempPath()
        {
            if (CoreBaseSettings.Standalone)
            {
                TenantExtra.DemandControlPanelPermission();
            }

            return BackupHandler.GetTmpFolder();
        }
    }
}
