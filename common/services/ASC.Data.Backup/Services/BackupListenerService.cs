
using System;
using System.Threading;
using System.Threading.Tasks;

using ASC.Common;
using ASC.Common.Caching;
using ASC.Data.Backup.Contracts;

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace ASC.Data.Backup.Services
{
    [Singletone]
    internal sealed class BackupListenerService : IHostedService
    {
        private readonly ICacheNotify<DeleteSchedule> _cacheDeleteSchedule;
        private readonly IServiceScopeFactory _scopeFactory;
    
        public BackupListenerService(ICacheNotify<DeleteSchedule> cacheDeleteSchedule,
            IServiceScopeFactory scopeFactory)
        {
            _cacheDeleteSchedule = cacheDeleteSchedule;
            _scopeFactory = scopeFactory;
        }

        public void DeleteScheldure(DeleteSchedule deleteSchedule)
        {
            using (var scope = _scopeFactory.CreateScope())
            {
                var backupService = scope.ServiceProvider.GetService<BackupService>();

                backupService.DeleteSchedule(deleteSchedule.TenantId);
            }
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            _cacheDeleteSchedule.Subscribe((n) => DeleteScheldure(n), CacheNotifyAction.Insert);

            return Task.CompletedTask;
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            _cacheDeleteSchedule.Unsubscribe(CacheNotifyAction.Insert);

            return Task.CompletedTask;
        }
    }
}
