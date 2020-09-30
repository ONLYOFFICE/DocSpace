using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

using ASC.Common;
using ASC.Common.DependencyInjection;
using ASC.Common.Logging;
using ASC.Core.Common;
using ASC.Core.Notify.Senders;
using ASC.Notify.Config;

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
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
                    _ = config.SetBasePath(path);
                    var env = hostContext.Configuration.GetValue("ENVIRONMENT", "Production");
                    _ = config
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
                        .AddEnvironmentVariables()
                        .AddCommandLine(args);
                })
                .ConfigureServices((hostContext, services) =>
                {
                    var diHelper = new DIHelper(services);

                    _ = diHelper.AddNLogManager("ASC.Notify", "ASC.Notify.Messages");

                    _ = services.Configure<NotifyServiceCfg>(hostContext.Configuration.GetSection("notify"));
                    _ = diHelper.AddSingleton<IConfigureOptions<NotifyServiceCfg>, ConfigureNotifyServiceCfg>();

                    _ = diHelper.TryAddSingleton<CommonLinkUtilitySettings>();
                    _ = diHelper.AddSingleton<IConfigureOptions<CommonLinkUtilitySettings>, ConfigureCommonLinkUtilitySettings>();

                    _ = diHelper.AddNotifyServiceLauncher();
                    _ = services.AddHostedService<NotifyServiceLauncher>();

                    _ = diHelper
                    .AddJabberSenderService()
                    .AddSmtpSenderService()
                    .AddAWSSenderService();

                    _ = services.AddAutofac(hostContext.Configuration, hostContext.HostingEnvironment.ContentRootPath);
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
