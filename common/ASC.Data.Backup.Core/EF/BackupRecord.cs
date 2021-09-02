using System;
using System.ComponentModel.DataAnnotations.Schema;

using ASC.Core.Common.EF;
using ASC.Data.Backup.Contracts;

namespace ASC.Data.Backup.EF.Model
{
    [Table("backup_backup")]
    public class BackupRecord : BaseEntity
    {
        public Guid Id { get; set; }

        [Column("tenant_id")]
        public int TenantId { get; set; }

        [Column("is_scheduled")]
        public bool IsScheduled { get; set; }

        public string Name { get; set; }

        public string Hash { get; set; }

        [Column("storage_type")]
        public BackupStorageType StorageType { get; set; }

        [Column("storage_base_path")]
        public string StorageBasePath { get; set; }

        [Column("storage_path")]
        public string StoragePath { get; set; }

        [Column("created_on")]
        public DateTime CreatedOn { get; set; }

        [Column("expires_on")]
        public DateTime ExpiresOn { get; set; }

        [Column("storage_params")]
        public string StorageParams { get; set; }

        public override object[] GetKeys()
        {
            return new object[] { Id };
        }
    }

}
