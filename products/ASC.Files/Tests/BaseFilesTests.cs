using ASC.Common.Security.Authentication;
using ASC.Core;
using ASC.Core.Tenants;
using ASC.Core.Users;
using ASC.Files.Helpers;
using ASC.Files.Model;
using ASC.Files.Tests.Infrastructure;
using ASC.Web.Files.Classes;
using ASC.Web.Files.Services.WCFService.FileOperations;

using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;

namespace ASC.Files.Tests
{
    public class BaseFilesTests<T>
    {
        protected FilesControllerHelper<T> FilesControllerHelper { get; set; }
        protected TestServer TestServer { get; set; }
        protected GlobalFolderHelper GlobalFolderHelper { get; set; }
        protected UserManager UserManager { get; set; }
        protected Tenant CurrentTenant { get; set; }
        protected UserInfo User { get; set; }
        protected SecurityContext SecurityContext { get; set; }
        protected UserOptions UserOptions { get; set; }
        protected IAccount Account { get; set; }
        protected IConfiguration Configuration { get; set; }

        public virtual void SetUp()
        {
            TestServer = new TestServer(new WebHostBuilder()
                .UseStartup<Startup>()
                .ConfigureServices(service =>
                {
                    service.Configure<UserOptions>(Configuration.GetSection(UserOptions.User));
                })
                .ConfigureAppConfiguration((hostingContext, config) =>
                {
                    Configure(hostingContext, config);
                }));

            FilesControllerHelper = TestServer.Services.GetService<FilesControllerHelper<T>>();
            GlobalFolderHelper = TestServer.Services.GetService<GlobalFolderHelper>();
            UserManager = TestServer.Services.GetService<UserManager>();
            SecurityContext = TestServer.Services.GetService<SecurityContext>();
            UserOptions = TestServer.Services.GetService<IOptions<UserOptions>>().Value;
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
            var tenant = tenantManager.GetTenant(UserOptions.TenantId);
            tenantManager.SetCurrentTenant(tenant);

            return tenantManager.CurrentTenant;
        }

        private UserInfo GetUser()
        {
            var user = UserManager.GetUsers(UserOptions.Id);
            return user;
        }

        private IAccount GetAccount()
        {
            return new Account(CurrentTenant.OwnerId, "di.vahomik22@gmail.com", true);
        }

        public BatchModel GetBatchModel(string text)
        {
            var json = text;

            var jsonDocument = JsonDocument.Parse(json);
            var root = jsonDocument.RootElement;
            var folderIds = root[0].GetProperty("folderIds").EnumerateArray().ToList();
            var fileIds = root[1].GetProperty("fileIds").EnumerateArray().ToList();
            var destFolderdId = root[2];

            var batchModel = new BatchModel
            {
                FolderIds = folderIds,
                FileIds = fileIds,
                DestFolderId = destFolderdId,
                DeleteAfter = false,
                ConflictResolveType = FileConflictResolveType.Overwrite
            };

            return batchModel;
        }

        public virtual void TearDown()
        {
            TestServer.Dispose();
        }
    }
}
