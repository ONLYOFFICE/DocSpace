using System.IO;
using System.Threading.Tasks;
using ASC.Common.DependencyInjection;
using ASC.Common.Utils;
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
                    config
                        .AddJsonFile("appsettings.json")
                        .AddJsonFile($"appsettings.{hostContext.HostingEnvironment.EnvironmentName}.json", true)
                        .AddJsonFile("autofac.json")
                        .AddJsonFile("storage.json")
                        .AddJsonFile("notify.json")
                        .AddJsonFile("kafka.json");
                })
                .ConfigureServices((hostContext, services) =>
                {
                    var serviceProvider = services.BuildServiceProvider();
                    ConfigurationManager.Init(serviceProvider);
                    CommonServiceProvider.Init(serviceProvider);

                    services.AddAutofac(hostContext.Configuration, hostContext.HostingEnvironment.ContentRootPath);

                    services.AddWebItemManager();

                    var c = ConfigurationManager.GetSetting<NotifyServiceCfg>("notify");
                    c.Init();
                    services.AddSingleton(c);
                    services.AddSingleton<DbWorker>();
                    services.AddSingleton<NotifyCleaner>();
                    services.AddSingleton<NotifySender>();
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
