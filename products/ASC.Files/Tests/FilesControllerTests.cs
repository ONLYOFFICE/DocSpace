using ASC.Api.Core;
using ASC.Api.Documents;
using ASC.Core;
using ASC.Core.Common.Configuration;
using ASC.Core.Tenants;
using ASC.Files;
using ASC.Files.Core;
using ASC.Files.Helpers;
using ASC.MessagingSystem;
using ASC.Web.Core;
using ASC.Web.Core.Files;
using ASC.Web.Files.Classes;
using ASC.Web.Files.Configuration;
using ASC.Web.Files.Helpers;
using ASC.Web.Files.Services.DocumentService;
using ASC.Web.Files.Services.WCFService;
using ASC.Web.Files.Services.WCFService.FileOperations;
using ASC.Web.Files.Utils;
using ASC.Web.Studio.Utility;

using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

using NUnit.Framework;

using System.Collections.Generic;
using System.IO;

namespace ASC.Tests.ASC.Files.Tests
{
    [TestFixture]
    public class FilesControllerTests
    {
        private FilesController FilesController { get; set; }
        private TestServer TestServer { get; set; }
        private GlobalFolder GlobalFolder { get; set; }
        private Tenant CurrentTenant { get; set; }

        [SetUp]
        public void SetUp()
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

            GlobalFolder = TestServer.Services.GetService<GlobalFolder>();
        }

        [TestCase(1, "test")]
        public void CreateFileReturnsFileWrapperTest(int id, string fileTitle)
        {
            CurrentTenant = SetAndGetCurrentTenant();

            var fileMarker = TestServer.Services.GetService<FileMarker>();
            var daoFactory = TestServer.Services.GetService<IDaoFactory>();
            var folder = FilesController.CreateFolder(GlobalFolder.GetFolderMy(fileMarker, daoFactory), "testFolder");
            Assert.IsNotNull(folder);


            //var fileWrapperInt = FilesController.CreateFile(id, fileTitle);

            //Assert.IsNotNull(fileWrapperInt);
            //Assert.AreEqual(fileTitle, fileWrapperInt.Title);
        }

        [TestCase(1, false, true)]
        [Ignore("")]
        public void DeleteFileTest(int field, bool deleteAfter, bool immediately)
        {
            FilesController.DeleteFile(field, deleteAfter, immediately);

            var statuses = FilesController.GetOperationStatuses();

            FileOperationWraper status = null;

            foreach (var item in statuses)
            {
                if (item.OperationType == FileOperationType.Delete)
                {
                    status = item;
                }
            }

            Assert.IsNotNull(status);
        }

        private void Configure(WebHostBuilderContext hostingContext, IConfigurationBuilder config)
        {
            var builder = new ConfigurationBuilder()
                    .AddJsonFile("appsettings.json")
                    .Build();

            var path = builder["pathToConf"];

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
            var tenant = GetTenant();

            var tenantManager = TestServer.Services.GetService<TenantManager>();
            tenantManager.SetCurrentTenant(tenant);

            return tenantManager.CurrentTenant;
        }

        private Tenant GetTenant()
        {
            return new Tenant();
        }

        [TearDown]
        public void TearDown()
        {
            TestServer.Dispose();
        }
    }
}
