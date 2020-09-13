using ASC.Api.Documents;
using ASC.Common.Security.Authentication;
using ASC.Core;
using ASC.Core.Common.EF;
using ASC.Core.Tenants;
using ASC.Files;
using ASC.Web.Files.Classes;
using ASC.Web.Files.Services.WCFService.FileOperations;

using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

using NUnit.Framework;

using System;
using System.Collections.Generic;
using System.IO;

namespace ASC.Tests.ASC.Files.Tests
{
    [TestFixture]
    public class FilesControllerTests
    {
        private FilesController FilesController { get; set; }
        private TestServer TestServer { get; set; }
        private GlobalFolderHelper GlobalFolderHelper { get; set; }
        private Tenant CurrentTenant { get; set; }
        private User User { get; set; }
        private IAccount Account { get; set; }
        private SecurityContext SecurityContext { get; set; }

        [SetUp]
        public void SetUp()
        {
            TestServer = new TestServer(new WebHostBuilder()
                .UseStartup<Startup>()
                .ConfigureAppConfiguration((hostingContext, config) =>
                {
                    Configure(hostingContext, config);
                }));

            FilesController = TestServer.Services.GetService<FilesController>();
            GlobalFolderHelper = TestServer.Services.GetService<GlobalFolderHelper>();
            SecurityContext = TestServer.Services.GetService<SecurityContext>();
            User = GetUser();
            Account = GetAccount();
            CurrentTenant = SetAndGetCurrentTenant();

            SecurityContext.AuthenticateMe(Account);
        }

        [TestCase("test")]
        public void CreateFileReturnsFileWrapperTest(string fileTitle)
        {
            var folder = FilesController.CreateFolder(GlobalFolderHelper.FolderMy, "folder");
            Assert.IsNotNull(folder);

            var fileWrapper = FilesController.CreateFile(GlobalFolderHelper.FolderMy, fileTitle);

            Assert.IsNotNull(fileWrapper);
            Assert.AreEqual(fileTitle, fileWrapper.Title);
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

        private User GetUser()
        {
            return new User { Id = Guid.NewGuid() };
        }

        private IAccount GetAccount()
        {
            return new Account(User.Id, "maks", true);
        }

        [TearDown]
        public void TearDown()
        {
            TestServer.Dispose();
        }
    }
}
