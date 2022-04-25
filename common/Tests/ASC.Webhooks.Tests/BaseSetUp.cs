using System;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Reflection;
using System.Threading.Tasks;

using ASC.Common;
using ASC.Common.Caching;
using ASC.Common.Utils;
using ASC.Core.Common.EF;
using ASC.Webhooks.Core;
using ASC.Webhooks.Core.Dao;
using ASC.Webhooks.Service;

using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

using NUnit.Framework;

namespace ASC.Webhooks.Tests
{
    public class BaseSetUp
    {
        private IServiceProvider serviceProvider;
        protected IHost host;
        protected WebhookSender webhookSender;
        protected RequestHistory requestHistory;
        protected Settings settings;
        protected IHttpClientFactory httpClientFactory;
        protected string TestConnection = "Server=localhost;Database=onlyoffice_test;User ID=dev;Password=dev;Pooling=true;Character Set=utf8;AutoEnlist=false;SSL Mode=none;AllowPublicKeyRetrieval=True";
        protected static int port = 8867;

        [OneTimeSetUp]
        public async Task CreateDb()
        {
            var args = new string[] {
                "--pathToConf", Path.Combine( "..", "..", "..", "..", "..", "..", "config"),
                "--ConnectionStrings:default:connectionString", TestConnection,
                "--migration:enabled", "true"};

            await StartHost(args);

            Migrate(serviceProvider);
            Migrate(serviceProvider, Assembly.GetExecutingAssembly().GetName().Name);
        }

        [OneTimeTearDown]
        public async Task DropDb()
        {
            var scope = serviceProvider.CreateScope();
            var context = scope.ServiceProvider.GetService<DbContextManager<WebhooksDbContext>>();
            context.Value.Database.EnsureDeleted();
            await host.StopAsync();
        }

        private async Task StartHost(string[] args)
        {
            host = await Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder
                    .UseKestrel(options =>
                    {
                        options.Listen(IPAddress.Loopback, port);
                    })
                    .ConfigureServices(services =>
                    {
                        services.AddControllers();
                        services.AddMemoryCache();
                        services.AddHttpClient();

                        var dIHelper = new DIHelper();
                        dIHelper.Configure(services);
                        dIHelper.TryAdd<DbWorker>();
                        dIHelper.TryAdd<TestController>();
                        dIHelper.TryAdd<WebhookSender>();
                        dIHelper.TryAdd(typeof(ICacheNotify<>), typeof(KafkaCacheNotify<>));
                    })
                    .Configure(app =>
                    {
                        app.UseRouting();
                        app.UseEndpoints(endpoints =>
                        {
                            endpoints.MapControllers();
                        });
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
                    .AddCommandLine(args);
                })
                .StartAsync();

            serviceProvider = host.Services;
            webhookSender = serviceProvider.GetService<WebhookSender>();
            requestHistory = serviceProvider.GetService<RequestHistory>();
            settings = serviceProvider.GetService<Settings>();
            httpClientFactory = serviceProvider.GetService<IHttpClientFactory>();
        }

        private void Migrate(IServiceProvider serviceProvider, string testAssembly = null)
        {

            if (!string.IsNullOrEmpty(testAssembly))
            {
                var configuration = serviceProvider.GetService<IConfiguration>();
                configuration["testAssembly"] = testAssembly;
            }

            using var db = serviceProvider.GetService<DbContextManager<WebhooksDbContext>>();
            db.Value.Migrate();
        }
    }
}
