using System;

using ASC.Api.Core;
using ASC.Common;
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
            base.ConfigureServices(services);
            DIHelper.AddProgressQueue<BaseBackupProgressItem>(1, (int)TimeSpan.FromMinutes(5).TotalMilliseconds, true, false, 0);

            services.AddHostedService<BackupServiceLauncher>();
        }
    }
}
