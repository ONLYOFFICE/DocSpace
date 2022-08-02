// (c) Copyright Ascensio System SIA 2010-2022
//
// This program is a free software product.
// You can redistribute it and/or modify it under the terms
// of the GNU Affero General Public License (AGPL) version 3 as published by the Free Software
// Foundation. In accordance with Section 7(a) of the GNU AGPL its Section 15 shall be amended
// to the effect that Ascensio System SIA expressly excludes the warranty of non-infringement of
// any third-party rights.
//
// This program is distributed WITHOUT ANY WARRANTY, without even the implied warranty
// of MERCHANTABILITY or FITNESS FOR A PARTICULAR  PURPOSE. For details, see
// the GNU AGPL at: http://www.gnu.org/licenses/agpl-3.0.html
//
// You can contact Ascensio System SIA at Lubanas st. 125a-25, Riga, Latvia, EU, LV-1021.
//
// The  interactive user interfaces in modified source and object code versions of the Program must
// display Appropriate Legal Notices, as required under Section 5 of the GNU AGPL version 3.
//
// Pursuant to Section 7(b) of the License you must retain the original Product logo when
// distributing the program. Pursuant to Section 7(e) we decline to grant you any rights under
// trademark law for use of our trademarks.
//
// All the Product's GUI elements, including illustrations and icon sets, as well as technical writing
// content are licensed under the terms of the Creative Commons Attribution-ShareAlike 4.0
// International. See the License terms at http://creativecommons.org/licenses/by-sa/4.0/legalcode

using System;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Reflection;
using System.Threading.Tasks;

using ASC.Common;
using ASC.Common.Caching;
using ASC.Common.Utils;
using ASC.Webhooks.Core;
using ASC.Webhooks.Core.EF.Context;
using ASC.Webhooks.Service;

using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
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
            var context = scope.ServiceProvider.GetService<IDbContextFactory<WebhooksDbContext>>().CreateDbContext();
            context.Database.EnsureDeleted();
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

            using var db = serviceProvider.GetService<IDbContextFactory<WebhooksDbContext>>().CreateDbContext();
            db.Database.Migrate();
        }
    }
}
