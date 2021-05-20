
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

using ASC.Core.Common.EF;
using ASC.Data.Backup.Contracts;

namespace ASC.Data.Backup.EF.Model
{
    [Table("backup_schedule")]
    public class BackupSchedule : BaseEntity
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
        public string StorageParams { get; set; }

        public override object[] GetKeys()
        {
            return new object[] { TenantId };
        }
    }

}
