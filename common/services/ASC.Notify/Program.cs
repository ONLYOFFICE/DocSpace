using System.IO;
using System.Threading.Tasks;

using ASC.Common.DependencyInjection;
using ASC.Common.Logging;
using ASC.Core.Common;
using ASC.Core.Notify.Senders;
using ASC.Notify.Config;

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;

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

                    services.Configure<LogNLog>(r => r.Name = "ASC");
                    services.Configure<LogNLog>("ASC", r => r.Name = "ASC");
                    services.Configure<LogNLog>("ASC.Notify", r => r.Name = "ASC");
                    services.Configure<LogNLog>("ASC.Notify.Messages", r => r.Name = "ASC.Notify.Messages");

                    services.TryAddSingleton(typeof(ILog), typeof(LogNLog));

                    services.Configure<NotifyServiceCfg>(hostContext.Configuration.GetSection("notify"));
                    services.AddSingleton<IConfigureOptions<NotifyServiceCfg>, ConfigureNotifyServiceCfg>();

                    services.TryAddSingleton<CommonLinkUtilitySettings>();
                    services.AddSingleton<IConfigureOptions<CommonLinkUtilitySettings>, ConfigureCommonLinkUtilitySettings>();

                    services.AddNotifyServiceLauncher();
                    services.AddHostedService<NotifyServiceLauncher>();

                    services
                    .AddJabberSenderService()
                    .AddSmtpSenderService()
                    .AddAWSSenderService();
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
