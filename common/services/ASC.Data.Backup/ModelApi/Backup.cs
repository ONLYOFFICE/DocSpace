
using System.Collections.Generic;
using ASC.Data.Backup.Contracts;

namespace ASC.Data.Backup.Models
{
    public class Backup
    {
        public BackupStorageType StorageType { get; set; }
        public bool BackupMail { get; set; }
        public Dictionary<string, string> StorageParams { get; set; }
    }
}
