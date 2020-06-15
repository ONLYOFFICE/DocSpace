using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

using ASC.Common;
using ASC.Common.DependencyInjection;
using ASC.Common.Logging;
using ASC.Common.Threading.Progress;
using ASC.Data.Backup.Service;

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace ASC.Data.Backup
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var host = Host.CreateDefaultBuilder(args)
                .ConfigureAppConfiguration((hostContext, config) =>
                {
                    var buided = config.Build();
                    var path = buided["pathToConf"];
                    if (!Path.IsPathRooted(path))
                    {
                        path = Path.GetFullPath(Path.Combine(hostContext.HostingEnvironment.ContentRootPath, path));
                    }
                    config.SetBasePath(path);
                    var env = hostContext.Configuration.GetValue("ENVIRONMENT", "Production");
                    config
                        .AddInMemoryCollection(new Dictionary<string, string>
                            {
                                {"pathToConf", path }
                            }
                        )
                        .AddJsonFile("appsettings.json")
                        .AddJsonFile($"appsettings.{env}.json", true)
                        .AddJsonFile($"appsettings.services.json", true)
                        .AddJsonFile("storage.json")
                        .AddJsonFile("notify.json")
                        .AddJsonFile("backup.json")
                        .AddJsonFile("kafka.json")
                        .AddJsonFile($"kafka.{env}.json", true)
                        .AddEnvironmentVariables();
                })
                .ConfigureServices((hostContext, services) =>
                {
                    var diHelper = new DIHelper(services);

                    diHelper.AddBackupServiceLauncher();
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

                    services.AddAutofac(hostContext.Configuration, hostContext.HostingEnvironment.ContentRootPath, false);
                })
                .UseConsoleLifetime()
                .Build();

            using (host)
            {
                // Start the host
                await host.StartAsync();

                // Wait for the host to shutdown
                await host.WaitForShutdownAsync();
            }
        }
    }
}
