using System.Collections.Generic;

using ASC.Api.Collections;

namespace ASC.Data.Backup.Models
{
    public class BackupRestore
    {
        public string BackupId { get; set; }
        public object StorageType { get; set; }
        public IEnumerable<ItemKeyValuePair<object, object>> StorageParams { get; set; }
        public bool Notify { get; set; }
    }
}
