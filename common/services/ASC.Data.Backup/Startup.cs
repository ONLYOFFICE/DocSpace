

using System;

using ASC.Common;
using ASC.Common.Logging;
using ASC.Common.Threading.Progress;
using ASC.Data.Backup.Service;

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

        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public void ConfigureServices(IServiceCollection services)
        {

            var diHelper = new DIHelper(services);

            diHelper.AddBackupServiceLauncher();
            diHelper.AddNLogManager("ASC.Data.Backup");

            diHelper.Configure<ProgressQueue<IProgressItem>>(r =>
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
