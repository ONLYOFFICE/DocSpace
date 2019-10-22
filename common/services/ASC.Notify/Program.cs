using System.IO;
using System.Threading.Tasks;
using ASC.Common.DependencyInjection;
using ASC.Notify.Config;
using ASC.Web.Core;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace ASC.Notify
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
                        .AddJsonFile("appsettings.json")
                        .AddJsonFile($"appsettings.{env}.json", true)
                        .AddJsonFile("autofac.json")
                        .AddJsonFile("storage.json")
                        .AddJsonFile("notify.json")
                        .AddJsonFile("kafka.json")
                        .AddJsonFile($"kafka.{env}.json", true);
                })
                .ConfigureServices((hostContext, services) =>
                {
                    services.AddAutofac(hostContext.Configuration, hostContext.HostingEnvironment.ContentRootPath);
                    services.AddWebItemManager();

                    var serviceProvider = services.BuildServiceProvider();
                    var c = hostContext.Configuration.GetSetting<NotifyServiceCfg>("notify");
                    c.Init();
                    services.AddSingleton(c);
                    services.AddSingleton<DbWorker>();
                    services.AddSingleton<NotifyCleaner>();
                    services.AddSingleton<NotifySender>();
                    services.AddSingleton<NotifyService>();
                    services.AddHostedService<NotifyServiceLauncher>();
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
