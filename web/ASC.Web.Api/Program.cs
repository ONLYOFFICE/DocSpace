using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Configuration;
using System.IO;

namespace ASC.Web.Api
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
            .ConfigureAppConfiguration((hostingContext, config) => {
                var buided = config.Build();
                var path = buided["pathToConf"];
                if (!Path.IsPathRooted(path))
                {
                    path = Path.GetFullPath(Path.Combine(hostingContext.HostingEnvironment.ContentRootPath, path));
                }

                config.SetBasePath(path);
                config
                    .AddJsonFile("appsettings.json")
                    .AddJsonFile($"appsettings.{hostingContext.HostingEnvironment.EnvironmentName}.json", true)
                    .AddJsonFile("autofac.json")
                    .AddJsonFile("autofac.products.json")
                    .AddJsonFile("storage.json");
            });
    }
}
