using System;

using ASC.Api.Core;
using ASC.Common;
using ASC.Common.Logging;
using ASC.Common.Threading.Progress;
using ASC.Data.Backup.Controllers;
using ASC.Data.Backup.Service;

using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.AspNetCore.Hosting;
using ASC.Common.Threading.Workers;

namespace ASC.Data.Backup
{
    public class Startup : BaseStartup
    {

        public Startup(IConfiguration configuration, IHostEnvironment hostEnvironment):base(configuration, hostEnvironment)
        {

        }
        public override void ConfigureServices(IServiceCollection services) {
            var diHelper = new DIHelper(services);

            diHelper.AddBackupServiceLauncher()
            .AddBackupController();
            LogParams = new string[] { "ASC.Data.Backup" };
            services.AddHostedService<BackupServiceLauncher>();
            diHelper.AddProgressQueue<BaseBackupProgressItem>(1, (int)TimeSpan.FromMinutes(5).TotalMilliseconds, true, false, 0);
            addcontrollers = true;
            base.ConfigureServices(services);
        }
        public override void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            base.Configure(app, env);
        }
    }
}
