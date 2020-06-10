using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using ASC.Core.Common.Contracts;

namespace ASC.Web.Api.Models
{
    public class Backup
    {
        public BackupStorageType StorageType { get; set; }
        public bool BackupMail { get; set; }
        public Dictionary<string, string> StorageParams { get; set; }
    }
}
