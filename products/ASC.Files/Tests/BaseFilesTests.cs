using ASC.Common.Security.Authentication;
using ASC.Core;
using ASC.Core.Tenants;
using ASC.Core.Users;
using ASC.Files.Helpers;
using ASC.Web.Files.Classes;

using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

using System;
using System.Collections.Generic;
using System.IO;

namespace ASC.Files.Tests
{
    public class BaseFilesTests<T>
    {
        private string userId;
        private int tenantId;

        protected FilesControllerHelper<T> FilesControllerHelper { get; set; }
        protected TestServer TestServer { get; set; }
        protected GlobalFolderHelper GlobalFolderHelper { get; set; }
        protected Tenant CurrentTenant { get; set; }
        protected UserInfo User { get; set; }
        protected SecurityContext SecurityContext { get; set; }
        protected IAccount Account { get; set; }
        protected IConfiguration Configuration { get; set; } = new ConfigurationBuilder().Build();

        public virtual void SetUp()
        {
            TestServer = new TestServer(new WebHostBuilder()
                .UseStartup<Startup>()
                .ConfigureAppConfiguration((hostingContext, config) =>
                {
                    Configure(hostingContext, config);
                }));

            userId = Configuration["user:userId"];
            tenantId = int.Parse(Configuration["user:tenantId"]);

            FilesControllerHelper = TestServer.Services.GetService<FilesControllerHelper<T>>();
            GlobalFolderHelper = TestServer.Services.GetService<GlobalFolderHelper>();
            SecurityContext = TestServer.Services.GetService<SecurityContext>();
            CurrentTenant = SetAndGetCurrentTenant();
            User = GetUser();
            Account = GetAccount();

            SecurityContext.AuthenticateMe(Account);
        }

        private void Configure(WebHostBuilderContext hostingContext, IConfigurationBuilder config)
        {
            var path = "..\\..\\..\\..\\..\\..\\config";

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
                .AddJsonFile("filesTests.json")
                .AddEnvironmentVariables();

            Configuration = config.Build();
        }

        private Tenant SetAndGetCurrentTenant()
        {
            var tenantManager = TestServer.Services.GetService<TenantManager>();
            var tenant = tenantManager.GetTenant(tenantId);
            tenantManager.SetCurrentTenant(tenant);

            return tenantManager.CurrentTenant;
        }

        private UserInfo GetUser()
        {
            var userManager = TestServer.Services.GetService<UserManager>();
            var user = userManager.GetUsers(new Guid(userId));

            return user;
        }

        private IAccount GetAccount()
        {
            return new Account(User.ID, "maks", true);
        }

        public virtual void TearDown()
        {
            TestServer.Dispose();
        }
    }
}
