

using ASC.Common;
using ASC.Common.Threading.Progress;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using ASC.Common.Logging;
using static ASC.Data.Backup.Service.BackupWorker;
using ASC.Data.Backup.Tasks.Modules;

namespace ASC.Data.Backup
{
    public class Startup
    {
        public IConfiguration Configuration { get; }
        public IHostEnvironment HostEnvironment { get; }

        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public void ConfigureServices(IServiceCollection services)
        {

            var diHelper = new DIHelper(services);

            diHelper.AddBackupHelperService()
                .AddBackupManager()
                .AddDbBackupProvider()
                .AddDbHelper()
                .AddFileBackupProviderService()
                .AddNotifyHelperService()
                .AddHelpers()
                .AddModuleProvider()
                .AddBackupAjaxHandler();
            diHelper.AddNLogManager("ASC.Data.Backup");

            diHelper.Configure<ProgressQueue<BackupProgressItem>>(r =>
            {
                r.workerCount = 1;
                r.waitInterval = (int)TimeSpan.FromMinutes(5).TotalMilliseconds;
                r.removeAfterCompleted = true;
                r.stopAfterFinsih = false;
                r.errorCount = 0;
            });
            diHelper.Configure<ProgressQueue<RestoreProgressItem>>(r =>
            {
                r.workerCount = 1;
                r.waitInterval = (int)TimeSpan.FromMinutes(5).TotalMilliseconds;
                r.removeAfterCompleted = true;
                r.stopAfterFinsih = false;
                r.errorCount = 0;
            });
            diHelper.Configure<ProgressQueue<TransferProgressItem>>(r =>
            {
                r.workerCount = 1;
                r.waitInterval = (int)TimeSpan.FromMinutes(5).TotalMilliseconds;
                r.removeAfterCompleted = true;
                r.stopAfterFinsih = false;
                r.errorCount = 0;
            });
            diHelper.Configure<ProgressQueue<ScheduledProgressItem>>(r =>
            {
                r.workerCount = 1;
                r.waitInterval = (int)TimeSpan.FromMinutes(5).TotalMilliseconds;
                r.removeAfterCompleted = true;
                r.stopAfterFinsih = false;
                r.errorCount = 0;
            });
        }

        public void Configure(IApplicationBuilder app)
        {
           
        }
    }
}
