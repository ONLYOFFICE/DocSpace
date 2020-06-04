using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

using ASC.Common;
using ASC.Common.Caching;
using ASC.Common.DependencyInjection;
using ASC.Common.Logging;
using ASC.ElasticSearch;
using ASC.Feed.Aggregator;
using ASC.Web.Files.Core.Search;
using ASC.Web.Files.Utils;

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace ASC.Files.Service
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
                    var diHelper = new DIHelper(services);
                    diHelper.AddNLogManager("ASC.Files");
                    services.AddHostedService<ServiceLauncher>();
                    diHelper
                        .AddServiceLauncher()
                        .AddFileConverterService()
                        .AddKafkaService()
                        .AddFactoryIndexerFileService()
                        .AddFactoryIndexerFolderService();

                    diHelper.AddNLogManager("ASC.Feed.Agregator");
                    services.AddHostedService<FeedAggregatorService>();
                    diHelper
                        .AddFeedAggregatorService();

                    services.AddAutofac(hostContext.Configuration, hostContext.HostingEnvironment.ContentRootPath, true, false, "search.json", "feed.json");
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
