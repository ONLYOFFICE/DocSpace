using System.Threading.Tasks;

using ASC.Api.Core;

using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;

namespace ASC.Web.HealthChecks.UI
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
                .ConfigureWebHostDefaults(webBuilder => webBuilder.UseStartup<Startup>());
    }
}
