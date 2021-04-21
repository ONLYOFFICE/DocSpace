using System.Collections.Generic;
using System.IO;

using ASC.Common.DependencyInjection;
using ASC.Common.Utils;

using Autofac;
using Autofac.Extensions.DependencyInjection;

using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;

namespace ASC.CRM
{
    public class Program
    {
        public static void Main(string[] args)
        {
            CreateHostBuilder(args).Build().Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args)
        {
            return Host.CreateDefaultBuilder(args)
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
                        path = Path.GetFullPath(CrossPlatform.PathCombine(hostingContext.HostingEnvironment.ContentRootPath, path));
                    }

                    config.SetBasePath(path);
                    config
                    .AddJsonFile("appsettings.json")
                    .AddJsonFile($"appsettings.{hostingContext.HostingEnvironment.EnvironmentName}.json", true)
                    .AddJsonFile("storage.json")
                    .AddJsonFile("kafka.json")
                    .AddJsonFile($"kafka.{hostingContext.HostingEnvironment.EnvironmentName}.json", true)
                    .AddEnvironmentVariables()
                    .AddCommandLine(args)
                    .AddInMemoryCollection(new Dictionary<string, string>
                    {
                                        {"pathToConf", path}
                    });
                })
                .ConfigureContainer<ContainerBuilder>((context, builder) =>
                {
                    builder.Register(context.Configuration, true, false);
                });


            //if (!FilesIntegration.IsRegisteredFileSecurityProvider("crm", "crm_common"))
            //{
            //    FilesIntegration.RegisterFileSecurityProvider("crm", "crm_common", new FileSecurityProvider());
            //}

            ////Register prodjects' calendar events
            //CalendarManager.Instance.RegistryCalendarProvider(userid =>
            //{
            //    if (WebItemSecurity.IsAvailableForUser(WebItemManager.CRMProductID, userid))
            //    {
            //        return new List<BaseCalendar> { new CRMCalendar(userid) };
            //    }
            //    return new List<BaseCalendar>();
            //});


        }
    }
}
