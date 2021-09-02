
using System;

using ASC.Common;
using ASC.Common.Caching;
using ASC.Data.Backup.Contracts;
using ASC.Data.Backup.Service;

using Microsoft.Extensions.DependencyInjection;

namespace ASC.Data.Backup.Listerners
{
    [Singletone]
    public class BackupListener
    {
        private ICacheNotify<DeleteSchedule> CacheDeleteSchedule { get; }
        private IServiceProvider ServiceProvider { get; }

        public BackupListener(ICacheNotify<DeleteSchedule> cacheDeleteSchedule, IServiceProvider serviceProvider)
        {
            CacheDeleteSchedule = cacheDeleteSchedule;
            ServiceProvider = serviceProvider;
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
            using var scope = ServiceProvider.CreateScope();
            var backupService = scope.ServiceProvider.GetService<BackupService>();
            backupService.DeleteSchedule(deleteSchedule.TenantId);
        }
    }
}
