using System;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text.Json;
using System.Threading.Tasks;

using ASC.Common.Logging;
using ASC.Core;
using ASC.Core.Common.EF;
using ASC.Core.Common.EF.Context;
using ASC.Core.Tenants;
using ASC.Files.Helpers;
using ASC.Files.Model;
using ASC.Files.Tests.Infrastructure;
using ASC.Web.Files.Classes;
using ASC.Web.Files.Services.WCFService;
using ASC.Web.Files.Services.WCFService.FileOperations;

using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

using NUnit.Framework;
using NUnit.Framework.Internal;

namespace ASC.Files.Tests
{
    [SetUpFixture]
    public class MySetUpClass
    {
        protected IServiceScope Scope { get; set; }

        [OneTimeSetUp]
        public void CreateDb()
        {
            var host = Program.CreateHostBuilder(new string[] {
                "--pathToConf", Path.Combine("..", "..", "..", "..","..", "..", "config"),
                "--ConnectionStrings:default:connectionString", BaseFilesTests.TestConnection,
                "--migration:enabled", "true",
                "--core:products:folder", Path.Combine("..", "..", "..", "..","..", "..", "products")}).Build();

            Migrate(host.Services);
            Migrate(host.Services, Assembly.GetExecutingAssembly().GetName().Name);

            Scope = host.Services.CreateScope();
        }

        [OneTimeTearDown]
        public void DropDb()
        {
            var context = Scope.ServiceProvider.GetService<DbContextManager<TenantDbContext>>();
            context.Value.Database.EnsureDeleted();
        }

        private void Migrate(IServiceProvider serviceProvider, string testAssembly = null)
        {
            using var scope = serviceProvider.CreateScope();

            if (!string.IsNullOrEmpty(testAssembly))
            {
                var configuration = scope.ServiceProvider.GetService<IConfiguration>();
                configuration["testAssembly"] = testAssembly;
            }

            using var db = scope.ServiceProvider.GetService<DbContextManager<UserDbContext>>();
            db.Value.Migrate();
        }
    }

    public class BaseFilesTests
    {
        protected ILog Log { get; set; }
        protected FilesControllerHelper<int> FilesControllerHelper { get; set; }
        protected GlobalFolderHelper GlobalFolderHelper { get; set; }
        protected FileStorageService<int> FileStorageService { get; set; }
        protected UserManager UserManager { get; set; }
        protected Tenant CurrentTenant { get; set; }
        protected SecurityContext SecurityContext { get; set; }
        protected UserOptions UserOptions { get; set; }
        protected IServiceScope scope { get; set; }

        public const string TestConnection = "Server=localhost;Database=onlyoffice_test;User ID=root;Password=root;Pooling=true;Character Set=utf8;AutoEnlist=false;SSL Mode=none;AllowPublicKeyRetrieval=True";

        public virtual Task SetUp()
        {
            var host = Program.CreateHostBuilder(new string[] {
                "--pathToConf" , Path.Combine("..", "..", "..", "..","..", "..", "config"),
                "--ConnectionStrings:default:connectionString", TestConnection,
                 "--migration:enabled", "true",
                 "--web:hub:internal", "",
            })
                .Build();

            scope = host.Services.CreateScope();

            var tenantManager = scope.ServiceProvider.GetService<TenantManager>();
            var tenant = tenantManager.GetTenant(1);
            tenantManager.SetCurrentTenant(tenant);
            CurrentTenant = tenant;

            FilesControllerHelper = scope.ServiceProvider.GetService<FilesControllerHelper<int>>();
            GlobalFolderHelper = scope.ServiceProvider.GetService<GlobalFolderHelper>();
            UserManager = scope.ServiceProvider.GetService<UserManager>();
            SecurityContext = scope.ServiceProvider.GetService<SecurityContext>();
            UserOptions = scope.ServiceProvider.GetService<IOptions<UserOptions>>().Value;
            FileStorageService = scope.ServiceProvider.GetService<FileStorageService<int>>();
            Log = scope.ServiceProvider.GetService<IOptionsMonitor<ILog>>().CurrentValue;

            SecurityContext.AuthenticateMe(CurrentTenant.OwnerId);
            return Task.CompletedTask;
        }

        public async Task DeleteFolderAsync(int folder)
        {
            await FilesControllerHelper.DeleteFolder(folder, false, true);
            while (true)
            {
                var statuses = FileStorageService.GetTasksStatuses();

                if (statuses.TrueForAll(r => r.Finished))
                    break;
                await Task.Delay(100);
            }
        }
        public async Task DeleteFileAsync(int file)
        {
            await FilesControllerHelper.DeleteFileAsync(file, false, true);
            while (true)
            {
                var statuses = FileStorageService.GetTasksStatuses();

                if (statuses.TrueForAll(r => r.Finished))
                    break;
                await Task.Delay(100);
            }
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
    }
}
