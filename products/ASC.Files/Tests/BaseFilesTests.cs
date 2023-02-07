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



using ASC.Files.Core.EF;
using ASC.MessagingSystem.Core;
using ASC.MessagingSystem.EF.Context;
using ASC.Web.Core;
using ASC.Webhooks.Core.EF.Context;

using Microsoft.EntityFrameworkCore;

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

        builder.ConfigureServices(services =>
        {
            services.AddBaseDbContext<UserDbContext>();
            services.AddBaseDbContext<FilesDbContext>();
            services.AddBaseDbContext<MessagesContext>();
            services.AddBaseDbContext<WebhooksDbContext>();
            services.AddBaseDbContext<TenantDbContext>();
            services.AddBaseDbContext<CoreDbContext>();

            var DIHelper = new DIHelper();
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
        var args = new Dictionary<string, string>
                {
                    { "ConnectionStrings:default:connectionString", BaseFilesTests.TestConnection },
                    { "migration:enabled", "true" },
                    { "core:products:folder", Path.Combine("..", "..", "..", "products") },
                    { "web:hub:internal", "" }
                };

        var host = new FilesApplication(args);
        Migrate(host.Services, "ASC.Migrations.MySql");

        host = new FilesApplication(args);
        Migrate(host.Services, Assembly.GetExecutingAssembly().GetName().Name);

        Scope = host.Services.CreateScope();

        //var tenantManager = Scope.ServiceProvider.GetService<TenantManager>();
        //var tenant = tenantManager.GetTenant(1);
        //tenantManager.SetCurrentTenant(tenant);
    }

    [OneTimeTearDown]
    public void DropDb()
    {
        var context = Scope.ServiceProvider.GetService<IDbContextFactory<UserDbContext>>();
        context.CreateDbContext().Database.EnsureDeleted();

        try
        {
            Directory.Delete(Path.Combine(Path.Combine("..", "..", "..", "..", "..", "..", "Data.Test")), true);
        }
        catch { }
    }

    private void Migrate(IServiceProvider serviceProvider, string testAssembly)
    {
        using var scope = serviceProvider.CreateScope();

        var configuration = scope.ServiceProvider.GetService<IConfiguration>();
        configuration["testAssembly"] = testAssembly;

        using var db = scope.ServiceProvider.GetService<UserDbContext>();
        db.Database.Migrate();

        using var filesDb = scope.ServiceProvider.GetService<FilesDbContext>();
        filesDb.Database.Migrate();

        using var messagesDb = scope.ServiceProvider.GetService<MessagesContext>();
        messagesDb.Database.Migrate();

        using var webHookDb = scope.ServiceProvider.GetService<WebhooksDbContext>();
        webHookDb.Database.Migrate();

        using var tenantDb = scope.ServiceProvider.GetService<TenantDbContext>();
        tenantDb.Database.Migrate();

        using var coreDb = scope.ServiceProvider.GetService<CoreDbContext>();
        coreDb.Database.Migrate();
    }
}

public partial class BaseFilesTests
{
    private readonly JsonSerializerOptions _options;
    protected UserManager _userManager;
    private HttpClient _client;
    private readonly string _baseAddress;
    private string _cookie;

    public static readonly string TestConnection = string.Format("Server=localhost;Database=onlyoffice_test.{0};User ID=root;Password=root;Pooling=true;Character Set=utf8;AutoEnlist=false;SSL Mode=none;AllowPublicKeyRetrieval=True", DateTime.Now.Ticks);

    public BaseFilesTests()
    {
        _options = new JsonSerializerOptions()
        {
            AllowTrailingCommas = true,
            PropertyNameCaseInsensitive = true
        };

        _options.Converters.Add(new ApiDateTimeConverter());
        _options.Converters.Add(new FileEntryWrapperConverter());
        _options.Converters.Add(new FileShareConverter());
        _baseAddress = @$"http://localhost:{new Random().Next(5000, 6000)}/api/2.0/files/";
    }

    [OneTimeSetUp]
    public async Task OneTimeSetup()
    {
        var host = new FilesApplication(new Dictionary<string, string>
            {
                { "ConnectionStrings:default:connectionString", TestConnection },
                { "migration:enabled", "true" },
                { "web:hub:internal", "" },
                { "$STORAGE_ROOT", Path.Combine("..", "..", "..", "Data.Test") },
                { "log:dir", Path.Combine("..", "..", "..", "Logs", "Test") },
            })
             .WithWebHostBuilder(a => { });

        _client = host.CreateClient(new WebApplicationFactoryClientOptions()
        {
            BaseAddress = new Uri(_baseAddress)
        });

        var scope = host.Services.CreateScope();

        _userManager = scope.ServiceProvider.GetService<UserManager>();

        var tenantManager = scope.ServiceProvider.GetService<TenantManager>();
        var tenant = tenantManager.GetTenant(1);
        tenantManager.SetCurrentTenant(tenant);

        var _cookiesManager = scope.ServiceProvider.GetService<CookiesManager>();
        var action = MessageAction.LoginSuccessViaApi;
        _cookie = _cookiesManager.AuthenticateMeAndSetCookies(tenant.Id, tenant.OwnerId, action);

        _client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", _cookie);
        _client.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));
        _client.DefaultRequestHeaders.TryAddWithoutValidation("Content-Type", "application/json;");

        await _client.GetAsync("/");
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

    protected Task<T> GetAsync<T>(string url)
    {
        return SendAsync<T>(HttpMethod.Get, url);
    }

    protected Task<T> PostAsync<T>(string url, object data = null)
    {
        return SendAsync<T>(HttpMethod.Post, url, data);
    }

    protected Task<T> PutAsync<T>(string url, object data = null)
    {
        return SendAsync<T>(HttpMethod.Put, url, data);
    }

    protected Task<T> DeleteAsync<T>(string url, object data = null)
    {
        return SendAsync<T>(HttpMethod.Delete, url, data);
    }

    private protected Task<SuccessApiResponse> DeleteAsync(string url, object data = null)
    {
        return SendAsync(HttpMethod.Delete, url, data);
    }

    protected async Task<List<FileOperationResult>> WaitLongOperation()
    {
        List<FileOperationResult> statuses = null;

        while (true)
        {
            statuses = await GetAsync<List<FileOperationResult>>("fileops");

            if (statuses.TrueForAll(r => r.Finished))
            {
                break;
            }
            await Task.Delay(100);
        }

        return statuses;
    }

    protected void CheckStatuses(List<FileOperationResult> statuses)
    {
        Assert.IsTrue(statuses.Count > 0);
        Assert.IsTrue(statuses.TrueForAll(r => string.IsNullOrEmpty(r.Error)));
    }

    protected async Task<T> SendAsync<T>(HttpMethod method, string url, object data = null)
    {
        var result = await SendAsync(method, url, data);

        if (result.Response is JsonElement jsonElement)
        {
            return jsonElement.Deserialize<T>(_options);
        }
        throw new Exception("can't parsing result");
    }

    protected async Task<SuccessApiResponse> SendAsync(HttpMethod method, string url, object data = null)
    {
        _client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", _cookie);

        var request = new HttpRequestMessage
        {
            RequestUri = new Uri(_baseAddress + url),
            Method = method,
        };

        if (data != null)
        {
            request.Content = JsonContent.Create(data);
        }

        var response = await _client.SendAsync(request);

        return await response.Content.ReadFromJsonAsync<SuccessApiResponse>();
    }
}
