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



namespace ASC.Files.Tests;

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
            foreach (var a in Assembly.Load("ASC.Files").GetTypes().Where(r => r.IsAssignableTo<ControllerBase>() && !r.IsAbstract))
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
        var context = Scope.ServiceProvider.GetService<DbContextManager<UserDbContext>>();
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

        using var filesDb = scope.ServiceProvider.GetService<DbContextManager<Core.EF.FilesDbContext>>();
        filesDb.Value.Migrate();
    }
}

public class BaseFilesTests
{
    protected ILog _log;
    protected TagsController<int> _tagsController;
    protected SecurityControllerHelper<int> _securityControllerHelper;
    protected FilesControllerHelper<int> _filesControllerHelper;
    protected OperationControllerHelper<int> _operationControllerHelper;
    protected FoldersControllerHelper<int> _foldersControllerHelper;
    protected GlobalFolderHelper _globalFolderHelper;
    protected FileStorageService<int> _fileStorageService;
    protected FileDtoHelper _fileDtoHelper;
    protected EntryManager _entryManager;
    protected UserManager _userManager;
    protected Tenant _currentTenant;
    protected SecurityContext _securityContext;
    protected UserOptions _userOptions;
    protected IServiceScope _scope;
    protected HttpClient _client;

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

        _client = host.CreateClient(new WebApplicationFactoryClientOptions()
        {
            BaseAddress = new Uri(@"http://localhost:5007/api/2.0/files/")
        });

        _scope = host.Services.CreateScope();

        var tenantManager = _scope.ServiceProvider.GetService<TenantManager>();
        var tenant = tenantManager.GetTenant(1);
        tenantManager.SetCurrentTenant(tenant);
        _currentTenant = tenant;

        _fileDtoHelper = _scope.ServiceProvider.GetService<FileDtoHelper>();
        _entryManager = _scope.ServiceProvider.GetService<EntryManager>();
        _tagsController = _scope.ServiceProvider.GetService<TagsController<int>>();
        _securityControllerHelper = _scope.ServiceProvider.GetService<SecurityControllerHelper<int>>();
        _operationControllerHelper = _scope.ServiceProvider.GetService<OperationControllerHelper<int>>();
        _foldersControllerHelper = _scope.ServiceProvider.GetService<FoldersControllerHelper<int>>();
        _filesControllerHelper = _scope.ServiceProvider.GetService<FilesControllerHelper<int>>();
        _globalFolderHelper = _scope.ServiceProvider.GetService<GlobalFolderHelper>();
        _userManager = _scope.ServiceProvider.GetService<UserManager>();
        _securityContext = _scope.ServiceProvider.GetService<SecurityContext>();
        _userOptions = _scope.ServiceProvider.GetService<IOptions<UserOptions>>().Value;
        _fileStorageService = _scope.ServiceProvider.GetService<FileStorageService<int>>();
        _log = _scope.ServiceProvider.GetService<IOptionsMonitor<ILog>>().CurrentValue;

        var cookie = _securityContext.AuthenticateMe(_currentTenant.OwnerId);
        _client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", cookie);
        _client.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));
        _client.DefaultRequestHeaders.TryAddWithoutValidation("Content-Type", "application/json;");

        return Task.CompletedTask;
    }

    public async Task DeleteFolderAsync(int folder)
    {
        await _foldersControllerHelper.DeleteFolder(folder, false, true);
        while (true)
        {
            var statuses = _fileStorageService.GetTasksStatuses();

            if (statuses.TrueForAll(r => r.Finished))
                break;
            await Task.Delay(100);
        }
    }
    public async Task DeleteFileAsync(int file)
    {
        await _filesControllerHelper.DeleteFileAsync(file, false, true);
        while (true)
        {
            var statuses = _fileStorageService.GetTasksStatuses();

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
