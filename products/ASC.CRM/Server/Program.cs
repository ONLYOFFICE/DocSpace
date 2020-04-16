using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace ASC.CRM
{
    public class Program
    {
        public static void Main(string[] args)
        {
            CreateHostBuilder(args).Build().Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
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
                config
                    .AddInMemoryCollection(new Dictionary<string, string>
                    {
                        {"pathToConf", path}
                    })
                    .AddJsonFile("appsettings.json")
                    .AddJsonFile($"appsettings.{hostingContext.HostingEnvironment.EnvironmentName}.json", true)
                    .AddJsonFile("storage.json")
                    .AddJsonFile("kafka.json")
                    .AddJsonFile($"kafka.{hostingContext.HostingEnvironment.EnvironmentName}.json", true)
                    .AddEnvironmentVariables();
            });
    }
}
