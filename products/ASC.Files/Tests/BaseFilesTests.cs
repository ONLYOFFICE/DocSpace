using ASC.Common.Security.Authentication;
using ASC.Core;
using ASC.Core.Tenants;
using ASC.Core.Users;
using ASC.Files.Helpers;
using ASC.Files.Tests.Infrastructure;
using ASC.Web.Files.Classes;

using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

using System.Collections.Generic;
using System.IO;

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
        protected DocumentsOptions DocumentsOptions { get; set; } = new DocumentsOptions();
        protected IAccount Account { get; set; }
        protected IConfiguration Configuration { get; set; } = new ConfigurationBuilder().Build();

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

            DocumentsOptions.FolderOptions.CreateItems = Configuration.GetSection("Folder:CreateItems").Get<CreateItems>();
            DocumentsOptions.FolderOptions.GetItems = Configuration.GetSection("Folder:GetItems").Get<GetItems>();
            DocumentsOptions.FolderOptions.GetInfoItems = Configuration.GetSection("Folder:GetInfoItems").Get<GetInfoItems>();
            DocumentsOptions.FolderOptions.RenameItems = Configuration.GetSection("Folder:RenameItems").Get<RenameItems>();
            DocumentsOptions.FolderOptions.DeleteItems = Configuration.GetSection("Folder:DeleteItems").Get<DeleteItems>();

            DocumentsOptions.FileOptions.CreateItems = Configuration.GetSection("File:CreateItems").Get<CreateItems>();
            DocumentsOptions.FileOptions.GetInfoItems = Configuration.GetSection("File:GetInfoItems").Get<GetInfoItems>();
            DocumentsOptions.FileOptions.UpdateItems = Configuration.GetSection("File:UpdateItems").Get<UpdateItems>();
            DocumentsOptions.FileOptions.DeleteItems = Configuration.GetSection("File:DeleteItems").Get<DeleteItems>();
        }

        private Tenant SetAndGetCurrentTenant()
        {
            var tenantManager = TestServer.Services.GetService<TenantManager>();
            tenantManager.SetCurrentTenant(UserOptions.TenantId);

            return tenantManager.CurrentTenant;
        }

        private UserInfo GetUser()
        {
            var user = UserManager.GetUsers(UserOptions.Id);

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
