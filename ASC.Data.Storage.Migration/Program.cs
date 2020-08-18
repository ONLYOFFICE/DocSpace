using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

using ASC.Common;
using ASC.Common.DependencyInjection;
using ASC.Common.Logging;
using ASC.Core.Common;
using ASC.Core.Notify.Senders;

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;

namespace ASC.Data.Storage.Migration
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            Host.CreateDefaultBuilder(args)
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

                    diHelper.AddNLogManager("ASC.Data.Storage.Migration");

                    diHelper.TryAddSingleton<CommonLinkUtilitySettings>();

                    diHelper
                    .AddJabberSenderService()
                    .AddSmtpSenderService()
                    .AddAWSSenderService();

                    services.AddAutofac(hostContext.Configuration, hostContext.HostingEnvironment.ContentRootPath);
                })
                .UseConsoleLifetime()
                .Build()
                .Run();
        }
    }
}
