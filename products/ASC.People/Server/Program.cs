using System;
using System.Collections.Generic;
using System.IO;
using ASC.Api.Core;
using ASC.Api.Core.Auth;
using ASC.Api.Core.Middleware;
using ASC.Common;
using ASC.Common.Logging;
using ASC.Common.Threading.Progress;
using ASC.Common.Threading.Workers;
using ASC.Data.Reassigns;
using ASC.Employee.Core.Controllers;
using ASC.Web.Core.Users;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace ASC.People
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
            }).ConfigureServices((hostContext, services)=>
            {

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

                diHelper.Configure<WorkerQueue<ResizeWorkerItem>>(r =>
                {
                    r.workerCount = 2;
                    r.waitInterval = (int)TimeSpan.FromSeconds(30).TotalMilliseconds;
                    r.errorCount = 1;
                    r.stopAfterFinsih = true;
                });

                diHelper.Configure<ProgressQueue<ReassignProgressItem>>(r =>
                {
                    r.workerCount = 1;
                    r.waitInterval = (int)TimeSpan.FromMinutes(5).TotalMilliseconds;
                    r.removeAfterCompleted = true;
                    r.stopAfterFinsih = false;
                    r.errorCount = 0;
                });

                diHelper.Configure<ProgressQueue<RemoveProgressItem>>(r =>
                {
                    r.workerCount = 1;
                    r.waitInterval = (int)TimeSpan.FromMinutes(5).TotalMilliseconds;
                    r.removeAfterCompleted = true;
                    r.stopAfterFinsih = false;
                    r.errorCount = 0;
                });

                diHelper.AddNLogManager("ASC.Api", "ASC.Web");

                diHelper
                    .AddPeopleController()
                    .AddGroupController();
            })
            ;
    }
}
