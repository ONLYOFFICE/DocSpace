using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;

namespace ASC.Web.Studio
{
    public class Program
    {
        public static void Main(string[] args)
        {
            CreateWebHostBuilder(args).Build().Run();
        }

        public static IHostBuilder CreateWebHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(w=>
                {
                    w.UseStartup<Startup>();
                })
                .ConfigureAppConfiguration((hostingContext, config) => {
                    config
                        .AddJsonFile("autofac.json")
                        .AddJsonFile("storage.json");
                });
    }
}
