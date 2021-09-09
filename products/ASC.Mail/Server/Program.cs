
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

using ASC.Api.Core;

using Autofac.Extensions.DependencyInjection;

using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;

namespace ASC.Mail
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
                    webBuilder.UseStartup<Startup>();
                })
                .ConfigureAppConfiguration((hostingContext, config) =>
                {
                    var buided = config.Build();
                    var path = buided["pathToConf"];
                    if (!Path.IsPathRooted(path))
                    {
                        path = Path.GetFullPath(Path.Combine(hostingContext.HostingEnvironment.ContentRootPath, path));
                    }

                    config.SetBasePath(path);
                    var env = hostingContext.Configuration.GetValue("ENVIRONMENT", "Production");
                    config
                    .AddJsonFile("appsettings.json")
                    .AddJsonFile($"appsettings.{env}.json", true)
                    .AddJsonFile("storage.json")
                    .AddJsonFile($"storage.{env}.json")
                    .AddJsonFile("kafka.json")
                    .AddJsonFile($"kafka.{env}.json", true)
                    .AddJsonFile("mail.json")
                    .AddJsonFile($"mail.{env}.json")
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
