using System;
using System.Linq;
using System.Reflection;
using System.Text.Json;
using System.Threading;

using ASC.Common.Security.Authentication;
using ASC.Core;
using ASC.Core.Common.EF;
using ASC.Core.Common.EF.Context;
using ASC.Core.Tenants;
using ASC.Core.Users;
using ASC.Files.Helpers;
using ASC.Files.Model;
using ASC.Files.Tests.Infrastructure;
using ASC.Web.Files.Classes;
using ASC.Web.Files.Services.WCFService;
using ASC.Web.Files.Services.WCFService.FileOperations;

using Microsoft.AspNetCore.TestHost;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace ASC.Files.Tests
{
    public class BaseFilesTests
    {
        protected FilesControllerHelper<int> FilesControllerHelper { get; set; }
        protected TestServer TestServer { get; set; }

        public BaseDbContext baseDbContext;


        protected GlobalFolderHelper GlobalFolderHelper { get; set; }
        protected FileStorageService<int> FileStorageService { get; set; }
        protected UserManager UserManager { get; set; }
        protected Tenant CurrentTenant { get; set; }
        protected UserInfo User { get; set; }
        protected SecurityContext SecurityContext { get; set; }
        protected UserOptions UserOptions { get; set; }
        protected IAccount Account { get; set; }
        protected IConfiguration Configuration { get; set; }
        protected IServiceScope scope { get; set; }

        const string con = "Server=localhost;Database=onlyoffice_test;User ID = root; Password=root;Pooling=true;";
        public virtual void SetUp()
        {
            var host = Program.CreateHostBuilder(new string[] {
                "--pathToConf" ,"..\\..\\..\\..\\..\\..\\config",
                "--ConnectionStrings:default:connectionString", con,
                "--migration:enabled", "true" }).Build();

            Migrate(host.Services);

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
           
            
            SecurityContext.AuthenticateMe(CurrentTenant.OwnerId);
        }
        
        public void DeleteFolder(int folder, bool deleteAfter, bool DeleteEmmediatly)
        {
            FilesControllerHelper.DeleteFolder(folder, deleteAfter, DeleteEmmediatly);
            while (true)
            {
                var statuses = FileStorageService.GetTasksStatuses();

                if (statuses.TrueForAll(r => r.Finished))
                    break;
                Thread.Sleep(100);
            }
        }
        public void DeleteFile(int file, bool deleteAfter, bool DeleteEmmediatly)
        {
            FilesControllerHelper.DeleteFile(file, deleteAfter, DeleteEmmediatly);
            while (true)
            {
                var statuses = FileStorageService.GetTasksStatuses();

                if (statuses.TrueForAll(r => r.Finished))
                    break;
                Thread.Sleep(100);
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
        
        public virtual void TearDown()
        {
            var context = scope.ServiceProvider.GetService<DbContextManager<TenantDbContext>>();
            context.Value.Database.EnsureDeleted();
        }

        private void Migrate(IServiceProvider serviceProvider)
        {
            using var scope = serviceProvider.CreateScope();

            var configuration = scope.ServiceProvider.GetService<IConfiguration>();

            TenantMigrate();

            configuration["testAssembly"] = Assembly.GetExecutingAssembly().GetName().Name;
            TenantMigrate();
            configuration["testAssembly"] = "";

            void TenantMigrate()
            {
                using (var db = scope.ServiceProvider.GetService<DbContextManager<TenantDbContext>>())
                {
                    db.Value.Migrate();
                }
            }
        }
    }
}
