using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

using ASC.Common;
using ASC.Common.Caching;
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
                        .AddEnvironmentVariables()
                        .AddCommandLine(args);
                })
                .ConfigureServices((hostContext, services) =>
                {
                    var diHelper = new DIHelper(services);

                    diHelper.AddNLogManager("ASC.Notify", "ASC.Notify.Messages");
                    diHelper.TryAdd(typeof(ICacheNotify<>), typeof(KafkaCache<>));

                    services.Configure<NotifyServiceCfg>(hostContext.Configuration.GetSection("notify"));

                    diHelper.TryAdd(typeof(CommonLinkUtilitySettings));
                    diHelper.TryAdd(typeof(IConfigureOptions<CommonLinkUtilitySettings>), typeof(ConfigureCommonLinkUtilitySettings));
                    diHelper.TryAdd<NotifyServiceLauncher>();

                    diHelper.TryAdd<JabberSender>();
                    diHelper.TryAdd<SmtpSender>();
                    diHelper.TryAdd<AWSSender>(); // fix private

                    services.AddHostedService<NotifyServiceLauncher>();

                    services.AddAutofac(hostContext.Configuration, hostContext.HostingEnvironment.ContentRootPath);

                    var a = $"{string.Join(",", diHelper.Singleton.OrderBy(r => r).ToArray())},{string.Join(",", diHelper.Scoped.OrderBy(r => r).ToArray())},{string.Join(",", diHelper.Transient.OrderBy(r => r).ToArray())}";
                    var b = 0;
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
