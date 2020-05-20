using ASC.Core.Common.Contracts;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace ASC.Data.Backup.EF.Model
{
    [Table("backup_backup")]
    public class BackupRecord
    {
        public Guid Id { get; set; }
        [Column("tenant_id")]
        public int TenantId { get; set; }
        [Column("is_scheduled")]
        public bool IsScheduled { get; set; }
        public string Name { get; set; }
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
        public Dictionary<string, string> StorageParams { get; set; }

    }

}
