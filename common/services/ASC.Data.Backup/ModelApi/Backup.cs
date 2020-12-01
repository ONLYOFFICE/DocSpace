
using System.Collections.Generic;

using ASC.Api.Collections;

namespace ASC.Data.Backup.Models
{
    public class Backup
    {

        public string StorageType { get; set; }
        public bool BackupMail { get; set; }
        public IEnumerable<ItemKeyValuePair<object, object>> StorageParams { get; set; }
    }
}
