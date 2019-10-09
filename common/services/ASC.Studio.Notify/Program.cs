using System.IO;
using System.Threading.Tasks;
using ASC.Common.DependencyInjection;
using ASC.Common.Logging;
using ASC.Common.Utils;
using ASC.Data.Storage.Configuration;
using ASC.Notify;
using ASC.Web.Core;
using ASC.Web.Studio.Core.Notify;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace ASC.Studio.Notify
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
                        .AddJsonFile($"appsettings.services.json", true)
                        .AddJsonFile("autofac.json")
                        .AddJsonFile("autofac.products.json")
                        .AddJsonFile("storage.json")
                        .AddJsonFile("notify.json")
                        .AddJsonFile("kafka.json")
                        .AddJsonFile($"kafka.{env}.json", true);
                })
                .ConfigureServices((hostContext, services) =>
                {
                    services.AddAutofac(hostContext.Configuration, hostContext.HostingEnvironment.ContentRootPath);
                    services.AddWebItemManager();
                    services.AddSingleton<StudioNotifyServiceSender>();
                    services.AddHostedService<ServiceLauncher>();
                    services.AddHttpContextAccessor()
                            .AddStorage(hostContext.Configuration)
                            .AddLogManager();

                    var serviceProvider = services.BuildServiceProvider();
                    ConfigurationManager.Init(serviceProvider);
                    CommonServiceProvider.Init(serviceProvider);
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
