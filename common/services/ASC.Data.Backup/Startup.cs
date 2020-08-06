using System;

using ASC.Api.Core;
using ASC.Common;
using ASC.Common.Threading.Workers;
using ASC.Data.Backup.Controllers;
using ASC.Data.Backup.Service;

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace ASC.Data.Backup
{
    public class Startup : BaseStartup
    {
        public override string[] LogParams { get => new string[] { "ASC.Data.Backup" }; }

        public Startup(IConfiguration configuration, IHostEnvironment hostEnvironment) : base(configuration, hostEnvironment)
        {

        }

        public override void ConfigureServices(IServiceCollection services)
        {
            var diHelper = new DIHelper(services);

            diHelper
                .AddBackupServiceLauncher()
                .AddBackupController()
                .AddProgressQueue<BaseBackupProgressItem>(1, (int)TimeSpan.FromMinutes(5).TotalMilliseconds, true, false, 0);

            services.AddHostedService<BackupServiceLauncher>();
            base.ConfigureServices(services);
        }
    }
}
