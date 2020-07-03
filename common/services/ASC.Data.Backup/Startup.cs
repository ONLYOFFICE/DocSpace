using System;

using ASC.Api.Core;
using ASC.Common;
using ASC.Common.Logging;
using ASC.Common.Threading.Progress;
using ASC.Data.Backup.Controllers;
using ASC.Data.Backup.Service;
using ASC.Common.DependencyInjection;
using Autofac.Extensions.DependencyInjection;

using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace ASC.Data.Backup
{
    public class Startup
    {
        public IConfiguration Configuration { get; }
        public IHostEnvironment HostEnvironment { get; }

        public Startup(IConfiguration configuration, IHostEnvironment hostEnvironment)
        {
            Configuration = configuration;
            HostEnvironment = hostEnvironment;
        }
        public void ConfigureServices(IServiceCollection services) {
            var diHelper = new DIHelper(services);

            diHelper.AddBackupServiceLauncher()
            .AddBackupController();
            diHelper.AddNLogManager("ASC.Data.Backup");
            services.AddHostedService<BackupServiceLauncher>();
            diHelper.Configure<ProgressQueue<BaseBackupProgressItem>>(r =>
            {
                r.workerCount = 1;
                r.waitInterval = (int)TimeSpan.FromMinutes(5).TotalMilliseconds;
                r.removeAfterCompleted = true;
                r.stopAfterFinsih = false;
                r.errorCount = 0;
            });

           
            GeneralStartup.ConfigureServices(services, false);
            services.AddAutofac(Configuration, HostEnvironment.ContentRootPath);

        }
        public void Configure(IApplicationBuilder app)
        {
            GeneralStartup.Configure(app);
        }
    }
}
