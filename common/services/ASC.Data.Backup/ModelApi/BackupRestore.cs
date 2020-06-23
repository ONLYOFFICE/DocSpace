using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using ASC.Data.Backup.Contracts;

namespace ASC.Data.Backup.Models
{
    public class BackupRestore
    {
       public string BackupId { get; set; }
        public BackupStorageType StorageType { get; set; }
        public Dictionary<string, string> StorageParams { get; set; }
        public bool Notify { get; set; }
    }
}
