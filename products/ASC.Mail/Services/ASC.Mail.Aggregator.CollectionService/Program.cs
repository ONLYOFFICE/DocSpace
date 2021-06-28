﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Runtime.InteropServices;
using System.Threading.Tasks;

using ASC.Api.Core;
using ASC.Common;
using ASC.Common.Caching;
using ASC.Common.DependencyInjection;
using ASC.Common.Logging;
using ASC.Common.Utils;
using ASC.Mail.Aggregator.CollectionService.Console;

using Autofac;
using Autofac.Extensions.DependencyInjection;

using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace ASC.Mail.Aggregator.CollectionService
{
    class Program
    {
        public async static Task Main(string[] args)
        {
#if DEBUG
            Debugger.Launch();
#endif
            var host = CreateHostBuilder(args).Build();

            await host.RunAsync();

        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .UseSystemd()
                .UseWindowsService()
                .UseServiceProviderFactory(new AutofacServiceProviderFactory())
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    var builder = webBuilder.UseStartup<BaseWorkerStartup>();

                    builder.ConfigureKestrel((hostingContext, serverOptions) =>
                    {
                        var kestrelConfig = hostingContext.Configuration.GetSection("Kestrel");

                        if (!kestrelConfig.Exists()) return;

                        var unixSocket = kestrelConfig.GetValue<string>("ListenUnixSocket");

                        if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
                        {
                            if (!String.IsNullOrWhiteSpace(unixSocket))
                            {
                                unixSocket = String.Format(unixSocket, hostingContext.HostingEnvironment.ApplicationName.Replace("ASC.", "").Replace(".", ""));

                                serverOptions.ListenUnixSocket(unixSocket);
                            }
                        }
                    });
                })
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
                        .AddJsonFile("appsettings.json")
                        .AddJsonFile($"appsettings.{env}.json", true)
                        .AddJsonFile("storage.json")
                        .AddJsonFile($"storage.{env}.json")
                        .AddJsonFile("notify.json")
                        .AddJsonFile("backup.json")
                        .AddJsonFile("kafka.json")
                        .AddJsonFile("mail.json")
                        .AddJsonFile($"mail.{env}.json")
                        .AddJsonFile("elastic.json")
                        .AddJsonFile($"elastic.{env}.json")
                        .AddJsonFile($"kafka.{env}.json", true)
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
                LogNLogExtension.ConfigureLog(diHelper, "ASC.Mail.Aggregator", "ASC.Mail.MainThread", "ASC.Mail.Stat", "ASC.Mail.MailboxEngine");
                diHelper.TryAdd(typeof(ICacheNotify<>), typeof(KafkaCache<>));
                services.AddSingleton(new ConsoleParser(args));
                diHelper.TryAdd<AggregatorServiceLauncher>();
                services.AddHostedService<AggregatorServiceLauncher>();
                services.Configure<HostOptions>(opts => opts.ShutdownTimeout = TimeSpan.FromSeconds(15));

            })
            .ConfigureContainer<ContainerBuilder>((context, builder) =>
            {
                builder.Register(context.Configuration, false, false);
            });
    }
}
