using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.InteropServices;
using System.Threading.Tasks;

using ASC.Api.Core;
using ASC.Common.Utils;

using Autofac.Extensions.DependencyInjection;

using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;

namespace ASC.Data.Backup
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
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    var builder = webBuilder.UseStartup<Startup>();

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
                        .AddJsonFile("notify.json")
                        .AddJsonFile($"notify.{env}.json", true)
                        .AddJsonFile("backup.json")
                        .AddJsonFile("kafka.json")
                        .AddJsonFile($"kafka.{env}.json", true)
                        .AddEnvironmentVariables()
                        .AddInMemoryCollection(new Dictionary<string, string>
                            {
                                {"pathToConf", path }
                            }
                        );
                })
            .ConfigureNLogLogging();

    }
}
