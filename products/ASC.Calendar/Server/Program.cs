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

namespace ASC.Calendar
{
    public class Program
    {
        public static async Task Main(string[] args)
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
                        serverOptions.Limits.MaxRequestBodySize = 100 * 1024 * 1024;
                        serverOptions.Limits.MaxRequestBufferSize = 100 * 1024 * 1024;
                        serverOptions.Limits.MinRequestBodyDataRate = null;
                        serverOptions.Limits.MinResponseDataRate = null;

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
            .ConfigureAppConfiguration((hostingContext, config) =>
            {
                var buided = config.Build();
                var path = buided["pathToConf"];
                if (!Path.IsPathRooted(path))
                {
                    path = Path.GetFullPath(CrossPlatform.PathCombine(hostingContext.HostingEnvironment.ContentRootPath, path));
                }

                config.SetBasePath(path);
                config
                    .AddJsonFile("appsettings.json")
                    .AddJsonFile($"appsettings.{hostingContext.HostingEnvironment.EnvironmentName}.json", true)
                    .AddJsonFile("storage.json")
                    .AddJsonFile("kafka.json")
                    .AddJsonFile($"kafka.{hostingContext.HostingEnvironment.EnvironmentName}.json", true)
                    .AddJsonFile("rabbitmq.json")
                    .AddJsonFile($"rabbitmq.{hostingContext.HostingEnvironment.EnvironmentName}.json", true)
                    .AddJsonFile("redis.json")
                    .AddJsonFile($"redis.{hostingContext.HostingEnvironment.EnvironmentName}.json", true)
                    .AddEnvironmentVariables()
                    .AddCommandLine(args)
                    .AddInMemoryCollection(new Dictionary<string, string>
                    {
                        {"pathToConf", path}
                    });
            })
            .ConfigureNLogLogging();
    }
}
