using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using ASC.Core.Common.Contracts;

namespace ASC.Web.Api.Models
{
    public class BackupRestore
    {
       public string BackupId { get; set; }
        public BackupStorageType StorageType { get; set; }
        public Dictionary<string, string> StorageParams { get; set; }
        public bool Notify { get; set; }
    }
}
