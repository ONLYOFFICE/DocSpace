using System.Collections.Generic;
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
                        .AddJsonFile("kafka.json")
                        .AddJsonFile($"kafka.{env}.json", true)
                        .AddEnvironmentVariables();
                })
                .ConfigureServices((hostContext, services) =>
                {
                    services.AddNLogManager("ASC.Notify", "ASC.Notify.Messages");

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

                    services.AddAutofac(hostContext.Configuration, hostContext.HostingEnvironment.ContentRootPath);
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
