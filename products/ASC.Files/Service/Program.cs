using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

using ASC.Api.Core;
using ASC.Common;
using ASC.Common.Caching;
using ASC.Common.DependencyInjection;
using ASC.Common.Utils;
using ASC.ElasticSearch;
using ASC.Feed.Aggregator;
using ASC.Files.ThumbnailBuilder;
using ASC.Web.Files.Core.Search;

using Autofac;
using Autofac.Extensions.DependencyInjection;

using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace ASC.Files.Service
{
    public class Program
    {
        public async static Task Main(string[] args)
        {
            var host = CreateHostBuilder(args).Build();

            await host.RunAsync();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .UseSystemd()
                .UseWindowsService()
                .UseServiceProviderFactory(new AutofacServiceProviderFactory())
                .ConfigureWebHostDefaults(webBuilder => webBuilder.UseStartup<BaseWorkerStartup>())
                .ConfigureAppConfiguration((hostContext, config) =>
                {
                    var buided = config.Build();
                    var path = buided["pathToConf"];
                    if (!Path.IsPathRooted(path))
                    {
                        path = Path.GetFullPath(CrossPlatform.PathCombine(hostContext.HostingEnvironment.ContentRootPath, path));
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
                        .AddJsonFile($"notify.{env}.json", true)
                        .AddJsonFile("kafka.json")
                        .AddJsonFile($"kafka.{env}.json", true)
                        .AddJsonFile("elastic.json", true)
                        .AddEnvironmentVariables()
                        .AddCommandLine(args);
                })
                .ConfigureServices((hostContext, services) =>
                {
                    services.AddMemoryCache();

                    var diHelper = new DIHelper(services);

                    diHelper.TryAdd(typeof(ICacheNotify<>), typeof(KafkaCache<>));

                    diHelper.RegisterProducts(hostContext.Configuration, hostContext.HostingEnvironment.ContentRootPath);
                    services.AddHostedService<ServiceLauncher>();
                    diHelper.TryAdd<ServiceLauncher>();

                    services.AddHostedService<FeedAggregatorService>();
                    diHelper.TryAdd<FeedAggregatorService>();

                    services.AddHostedService<Launcher>();
                    diHelper.TryAdd<Launcher>();

                    //diHelper.TryAdd<FileConverter>();
                    diHelper.TryAdd<FactoryIndexerFile>();
                    diHelper.TryAdd<FactoryIndexerFolder>();
                })
                .ConfigureContainer<ContainerBuilder>((context, builder) =>
                {
                    builder.Register(context.Configuration, true, false, "search.json", "feed.json");
                })
            .ConfigureNLogLogging();
    }
}
