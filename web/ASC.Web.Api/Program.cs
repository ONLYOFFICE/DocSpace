using System.Collections.Generic;
using System.IO;
using ASC.Api.Core;
using ASC.Api.Core.Auth;
using ASC.Api.Core.Middleware;
using ASC.Api.Settings;
using ASC.Common;
using ASC.Common.Logging;
using ASC.Web.Api.Controllers;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;


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
                    .AddEnvironmentVariables()
                    .AddCommandLine(args);
            }).ConfigureServices((hostContext, services) => {

                services.AddAuthentication("cookie")
                        .AddScheme<AuthenticationSchemeOptions, CookieAuthHandler>("cookie", a => { })
                        .AddScheme<AuthenticationSchemeOptions, ConfirmAuthHandler>("confirm", a => { });
                var diHelper = new DIHelper(services);

                diHelper
                    .AddConfirmAuthHandler()
                    .AddCookieAuthHandler()
                    .AddCultureMiddleware()
                    .AddIpSecurityFilter()
                    .AddPaymentFilter()
                    .AddProductSecurityFilter()
                    .AddTenantStatusFilter();

                diHelper.AddNLogManager("ASC.Api", "ASC.Web");

                diHelper
                    .AddAuthenticationController()
                    .AddModulesController()
                    .AddPortalController()
                    .AddSettingsController()
                    .AddSmtpSettingsController();
            });
    }
}
