using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using ASC.Common.Caching;
using ASC.Data.Backup.Contracts;
using ASC.Data.Backup.Service;

namespace ASC.Data.Backup.Listerners
{
    public class BackupListener
    {
        private ICacheNotify<DeleteSchedule> CacheDeleteSchedule { get; }
        private BackupService BackupService { get; }

        public BackupListener(ICacheNotify<DeleteSchedule> cacheDeleteSchedule, BackupService backupService)
        {
            CacheDeleteSchedule = cacheDeleteSchedule;
            BackupService = backupService;
        }

        public void Start()
        {
            CacheDeleteSchedule.Subscribe((n) => DeleteScheldure(n), CacheNotifyAction.Insert);
        }

        public void Stop()
        {
            CacheDeleteSchedule.Unsubscribe(CacheNotifyAction.Insert);
        }

        public void DeleteScheldure(DeleteSchedule deleteSchedule)
        {
            BackupService.DeleteSchedule(deleteSchedule.TenantId);
        }
    }
}
