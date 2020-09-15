using ASC.Api.Core;
using ASC.Api.Documents;
using ASC.Common.Security.Authentication;
using ASC.Core;
using ASC.Core.Common.Configuration;
using ASC.Core.Tenants;
using ASC.Core.Users;
using ASC.Files.Helpers;
using ASC.MessagingSystem;
using ASC.Web.Core;
using ASC.Web.Core.Files;
using ASC.Web.Files.Classes;
using ASC.Web.Files.Configuration;
using ASC.Web.Files.Helpers;
using ASC.Web.Files.Services.DocumentService;
using ASC.Web.Files.Services.WCFService;
using ASC.Web.Files.Utils;
using ASC.Web.Studio.Utility;

using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

using System;
using System.Collections.Generic;
using System.IO;

namespace ASC.Files.Tests
{
    public class BaseTests
    {
        private const string userId = "cb52f8a6-d564-11ea-a9b8-0a0027000010";
        private const int tenantId = 1;

        protected FilesController FilesController { get; set; }
        protected TestServer TestServer { get; set; }
        protected GlobalFolderHelper GlobalFolderHelper { get; set; }
        protected Tenant CurrentTenant { get; set; }
        protected UserInfo User { get; set; }
        protected IAccount Account { get; set; }
        protected SecurityContext SecurityContext { get; set; }

        public virtual void SetUp()
        {
            TestServer = new TestServer(new WebHostBuilder()
                .UseStartup<Startup>()
                .ConfigureAppConfiguration((hostingContext, config) =>
                {
                    Configure(hostingContext, config);
                }));

            var apiContext = TestServer.Services.GetService<ApiContext>();
            var filesControllerHelperString = TestServer.Services.GetService<FilesControllerHelper<string>>();
            var filesControllerHelperInt = TestServer.Services.GetService<FilesControllerHelper<int>>();
            var fileStorageService = TestServer.Services.GetService<FileStorageService<string>>();
            var fileStorageServiceInt = TestServer.Services.GetService<FileStorageService<int>>();
            var globalFolderHelper = TestServer.Services.GetService<GlobalFolderHelper>();
            var filesSettingsHelper = TestServer.Services.GetService<FilesSettingsHelper>();
            var filesLinkUtility = TestServer.Services.GetService<FilesLinkUtility>();
            var securityContext = TestServer.Services.GetService<SecurityContext>();
            var folderWrapperHelper = TestServer.Services.GetService<FolderWrapperHelper>();
            var fileOperationWraperHelper = TestServer.Services.GetService<FileOperationWraperHelper>();
            var entryManager = TestServer.Services.GetService<EntryManager>();
            var userManager = TestServer.Services.GetService<UserManager>();
            var webItemSecurity = TestServer.Services.GetService<WebItemSecurity>();
            var coreBaseSettings = TestServer.Services.GetService<CoreBaseSettings>();
            var thirdpartyConfiguration = TestServer.Services.GetService<ThirdpartyConfiguration>();
            var messageService = TestServer.Services.GetService<MessageService>();
            var commonLinkUtility = TestServer.Services.GetService<CommonLinkUtility>();
            var documentServiceConnector = TestServer.Services.GetService<DocumentServiceConnector>();
            var folderContentWrapperHelper = TestServer.Services.GetService<FolderContentWrapperHelper>();
            var wordpressToken = TestServer.Services.GetService<WordpressToken>();
            var wordpressHelper = TestServer.Services.GetService<WordpressHelper>();
            var consumerFactory = TestServer.Services.GetService<ConsumerFactory>();
            var easyBibHelper = TestServer.Services.GetService<EasyBibHelper>();
            var productEntryPoint = TestServer.Services.GetService<ProductEntryPoint>();

            FilesController = new FilesController(
                apiContext,
                filesControllerHelperString,
                filesControllerHelperInt,
                fileStorageService,
                fileStorageServiceInt,
                globalFolderHelper,
                filesSettingsHelper,
                filesLinkUtility,
                securityContext,
                folderWrapperHelper,
                fileOperationWraperHelper,
                entryManager,
                userManager,
                webItemSecurity,
                coreBaseSettings,
                thirdpartyConfiguration,
                messageService,
                commonLinkUtility,
                documentServiceConnector,
                folderContentWrapperHelper,
                wordpressToken,
                wordpressHelper,
                consumerFactory,
                easyBibHelper,
                productEntryPoint);

            GlobalFolderHelper = TestServer.Services.GetService<GlobalFolderHelper>();
            SecurityContext = TestServer.Services.GetService<SecurityContext>();
            CurrentTenant = SetAndGetCurrentTenant();
            User = GetUser();
            Account = GetAccount();

            SecurityContext.AuthenticateMe(Account);
        }

        private void Configure(WebHostBuilderContext hostingContext, IConfigurationBuilder config)
        {
            var path = "..\\..\\..\\config";

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
                .AddEnvironmentVariables();
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
