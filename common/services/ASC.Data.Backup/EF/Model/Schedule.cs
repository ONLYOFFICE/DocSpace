
using ASC.Common.Logging;
using ASC.Core;
using ASC.Core.Common.Contracts;
using ASC.Core.Tenants;
using ASC.Notify.Cron;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Threading;

namespace ASC.Data.Backup.EF.Model
{
    [Table("backup_schedule")]
    public class Schedule
    {
        [Key]
        [Column("tenant_id")]
        public int TenantId { get; set; }
        [Column("backup_mail")]
        public bool BackupMail { get; set; }
        public string Cron { get; set; }
        [Column("backups_stored")]
        public int BackupsStored { get; set; }
        [Column("storage_type")]
        public BackupStorageType StorageType { get; set; }
        [Column("storage_base_path")]
        public string StorageBasePath { get; set; }
        [Column("last_backup_time")]
        public DateTime LastBackupTime { get; set; }
        [Column("storage_params")]
        public Dictionary<string, string> StorageParams { get; internal set; }

        public TenantManager tenantManager { get; }
        public IOptionsMonitor<ILog> options { get; }
        public TenantUtil tenantUtil { get; }
        public BackupHelper backupHelper { get; }

        public Schedule(IOptionsMonitor<ILog> options, int tenantId, TenantManager tenantManager, TenantUtil tenantUtil, BackupHelper backupHelper)
        {
            this.options = options;
            TenantId = tenantId;
            this.tenantManager = tenantManager;
            this.tenantUtil = tenantUtil;
            this.backupHelper = backupHelper;
        }
        public bool IsToBeProcessed()
        {
            try
            {
                if (backupHelper.ExceedsMaxAvailableSize(TenantId)) throw new Exception("Backup file exceed " + TenantId);

                var cron = new CronExpression(Cron);
                var tenant = tenantManager.GetTenant(TenantId);
                var tenantTimeZone = tenant.TimeZone;
                var culture = tenant.GetCulture();
                Thread.CurrentThread.CurrentCulture = culture;

                var lastBackupTime = LastBackupTime.Equals(default(DateTime))
                    ? DateTime.UtcNow.Date.AddSeconds(-1)
                    : tenantUtil.DateTimeFromUtc(tenantTimeZone, LastBackupTime);

                var nextBackupTime = cron.GetTimeAfter(lastBackupTime);

                if (!nextBackupTime.HasValue) return false;
                var now = tenantUtil.DateTimeFromUtc(tenantTimeZone, DateTime.UtcNow);
                return nextBackupTime <= now;
            }
            catch (Exception e)
            {
                var log = options.CurrentValue;
                log.Error("Schedule " + TenantId, e);
                return false;
            }
        }
    }

}
