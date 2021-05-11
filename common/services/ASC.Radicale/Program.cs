using ASC.Common;
using ASC.Common.Caching;
using ASC.Common.Logging;
using ASC.Radicale;

using Autofac;
using Autofac.Extensions.DependencyInjection;

using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

using ASC.Common.DependencyInjection;

namespace ASC.Radicale
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var host = Host.CreateDefaultBuilder(args)
                .UseServiceProviderFactory(new AutofacServiceProviderFactory())
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
                        .AddJsonFile("storage.json")
                        .AddJsonFile("kafka.json")
                        .AddJsonFile($"appsettings.services.json", true)
                        //.AddJsonFile("radicale.json")
                        .AddJsonFile($"kafka.{env}.json", true)
                        .AddJsonFile($"appsettings.{env}.json", true)
                        .AddJsonFile($"radicale.{env}.json", true)
                        .AddEnvironmentVariables()
                        .AddCommandLine(args)
                        .AddInMemoryCollection(new Dictionary<string, string>
                            {
                                {"pathToConf", path }
                            }
                        );
                })
                .ConfigureServices((hostContext, services) =>
                {
                    services.AddMemoryCache();
                    var diHelper = new DIHelper(services);
                    diHelper.TryAdd(typeof(ICacheNotify<>), typeof(KafkaCache<>));
                    diHelper.RegisterProducts(hostContext.Configuration, hostContext.HostingEnvironment.ContentRootPath);

                    LogNLogExtension.ConfigureLog(diHelper, "ASC.Radicale");
                    services.AddHostedService<RadicaleServiceLauncher>();
                    diHelper.TryAdd<RadicaleServiceLauncher>();
                })
                .ConfigureContainer<ContainerBuilder>((context, builder) =>
                {
                    builder.Register(context.Configuration, false, false);
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
