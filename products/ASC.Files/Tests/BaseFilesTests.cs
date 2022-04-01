using System;
using System.Collections.Generic;
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
using ASC.Files.Api;
using ASC.Files.Core.ApiModels.RequestDto;
using ASC.Files.Core.ApiModels.ResponseDto;
using ASC.Files.Helpers;
using ASC.Files.Tests.Infrastructure;
using ASC.Web.Files.Classes;
using ASC.Web.Files.Services.WCFService;
using ASC.Web.Files.Services.WCFService.FileOperations;
using ASC.Web.Files.Utils;

using Autofac;

using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

using NUnit.Framework;
using NUnit.Framework.Internal;

namespace ASC.Files.Tests
{
    class FilesApplication : WebApplicationFactory<Program>
    {
        private readonly Dictionary<string, string> _args;

        public FilesApplication(Dictionary<string, string> args)
        {
            _args = args;
        }

        protected override void ConfigureWebHost(IWebHostBuilder builder)
        {
            foreach (var s in _args)
            {
                builder.UseSetting(s.Key, s.Value);
            }

            builder.ConfigureAppConfiguration((context, a) =>
            {
                (a.Sources[0] as ChainedConfigurationSource).Configuration["pathToConf"] = a.Build()["pathToConf"];
            });

            builder.ConfigureServices(services =>
            {
                var DIHelper = new ASC.Common.DIHelper();
                DIHelper.Configure(services);
                foreach (var a in Assembly.Load("ASC.Files").GetTypes().Where(r => r.IsAssignableTo<ControllerBase>()))
                {
                    DIHelper.TryAdd(a);
                }
            });

            base.ConfigureWebHost(builder);
        }
    }

    [SetUpFixture]
    public class MySetUpClass
    {
        protected IServiceScope Scope { get; set; }

        [OneTimeSetUp]
        public void CreateDb()
        {
            var host = new FilesApplication(new Dictionary<string, string>
                    {
                        { "pathToConf", Path.Combine("..","..", "..", "config") },
                        { "ConnectionStrings:default:connectionString", BaseFilesTests.TestConnection },
                        { "migration:enabled", "true" },
                        { "core:products:folder", Path.Combine("..","..", "..", "products") },
                        { "web:hub::internal", "" }
                    })
                .WithWebHostBuilder(builder =>
                {
                });

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
        protected TagsController TagsController { get; set; }
        protected SecurityControllerHelper<int> SecurityControllerHelper { get; set; }
        protected FilesControllerHelper<int> FilesControllerHelper { get; set; }
        protected OperationControllerHelper<int> OperationControllerHelper { get; set; }
        protected FoldersControllerHelper<int> FoldersControllerHelper { get; set; }
        protected GlobalFolderHelper GlobalFolderHelper { get; set; }
        protected FileStorageService<int> FileStorageService { get; set; }
        protected FileDtoHelper FileDtoHelper { get; set; }
        protected EntryManager EntryManager { get; set; }
        protected UserManager UserManager { get; set; }
        protected Tenant CurrentTenant { get; set; }
        protected SecurityContext SecurityContext { get; set; }
        protected UserOptions UserOptions { get; set; }
        protected IServiceScope scope { get; set; }

        public const string TestConnection = "Server=localhost;Database=onlyoffice_test;User ID=root;Password=root;Pooling=true;Character Set=utf8;AutoEnlist=false;SSL Mode=none;AllowPublicKeyRetrieval=True";

        public virtual Task SetUp()
        {
            var host = new FilesApplication(new Dictionary<string, string>
                {
                    { "pathToConf", Path.Combine("..","..", "..", "config") },
                    { "ConnectionStrings:default:connectionString", TestConnection },
                    { "migration:enabled", "true" },
                    { "web:hub:internal", "" }
                })
                 .WithWebHostBuilder(a => { });

            scope = host.Services.CreateScope();

            var tenantManager = scope.ServiceProvider.GetService<TenantManager>();
            var tenant = tenantManager.GetTenant(1);
            tenantManager.SetCurrentTenant(tenant);
            CurrentTenant = tenant;

            FileDtoHelper = scope.ServiceProvider.GetService<FileDtoHelper>();
            EntryManager = scope.ServiceProvider.GetService<EntryManager>();
            TagsController = scope.ServiceProvider.GetService<TagsController>();
            SecurityControllerHelper = scope.ServiceProvider.GetService<SecurityControllerHelper<int>>();
            OperationControllerHelper = scope.ServiceProvider.GetService<OperationControllerHelper<int>>();
            FoldersControllerHelper = scope.ServiceProvider.GetService<FoldersControllerHelper<int>>();
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
            await FoldersControllerHelper.DeleteFolder(folder, false, true);
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

        public BatchRequestDto GetBatchModel(string text)
        {
            var json = text;

            var jsonDocument = JsonDocument.Parse(json);
            var root = jsonDocument.RootElement;
            var folderIds = root[0].GetProperty("folderIds").EnumerateArray().ToList();
            var fileIds = root[1].GetProperty("fileIds").EnumerateArray().ToList();
            var destFolderdId = root[2];

            var batchModel = new BatchRequestDto
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
