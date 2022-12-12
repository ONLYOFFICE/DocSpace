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


using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace ASC.Web.Api.Tests;

class ApiApplication : WebApplicationFactory<Program>
{
    private readonly Dictionary<string, string> _args;

    public ApiApplication(Dictionary<string, string> args)
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
            services.AddBaseDbContext<TenantDbContext>();
            services.AddBaseDbContext<WebstudioDbContext>();

            var DIHelper = new ASC.Common.DIHelper();
            DIHelper.Configure(services);
            foreach (var a in Assembly.Load("ASC.Web.Api").GetTypes().Where(r => r.IsAssignableTo<ControllerBase>()))
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
    protected IServiceScope _scope;

    [OneTimeSetUp]
    public void CreateDb()
    {
        var args = new Dictionary<string, string>
                {
                    { "ConnectionStrings:default:connectionString", BaseApiTests.TestConnection },
                    { "migration:enabled", "true" },
                    { "web:hub::internal", "" },
                    { "disableLdapNotifyService", "true" }
                };

        var host = new ApiApplication(args);
        Migrate(host.Services, "ASC.Migrations.MySql");

        host = new ApiApplication(args);
        Migrate(host.Services, Assembly.GetExecutingAssembly().GetName().Name);

        _scope = host.Services.CreateScope();
    }

    [OneTimeTearDown]
    public void DropDb()
    {
        var context = _scope.ServiceProvider.GetService<IDbContextFactory<UserDbContext>>().CreateDbContext();
        context.Database.EnsureDeleted();
    }


    private void Migrate(IServiceProvider serviceProvider, string testAssembly )
    {
        using var scope = serviceProvider.CreateScope();

        var configuration = scope.ServiceProvider.GetService<IConfiguration>();
        configuration["testAssembly"] = testAssembly;

        using var userDbContext = scope.ServiceProvider.GetService<UserDbContext>();
        userDbContext.Database.Migrate();

        using var tenantDbContext = scope.ServiceProvider.GetService<TenantDbContext>();
        tenantDbContext.Database.Migrate();
        
        using var webstudioDbContext = scope.ServiceProvider.GetService<WebstudioDbContext>();
        webstudioDbContext.Database.Migrate();
    }
}

class BaseApiTests
{
    protected ILogger _log;
    protected UserManager _userManager;
    protected Tenant _currentTenant;
    protected SecurityContext _securityContext;
    protected UserOptions _userOptions;
    protected IServiceScope _scope;
    protected SettingsManager _settingsManager;
    protected DbWebstudioSettings _dbWebStudioSettings;
    protected FirstTimeTenantSettings _firstTimeTenantSettings;

    public const string TestConnection = "Server=localhost;Database=onlyoffice_test;User ID=root;Password=root;Pooling=true;Character Set=utf8;AutoEnlist=false;SSL Mode=none;AllowPublicKeyRetrieval=True";
    public virtual void SetUp()
    {
        var host = new ApiApplication(new Dictionary<string, string>
            {
                { "ConnectionStrings:default:connectionString", TestConnection },
                { "migration:enabled", "true" },
                { "web:hub:internal", "" },
                { "disableLdapNotifyService", "true" }
            })
        .WithWebHostBuilder(a => { });

        _scope = host.Services.CreateScope();

        var tenantManager = _scope.ServiceProvider.GetService<TenantManager>();
        var tenant = tenantManager.GetTenant(1);
        tenantManager.SetCurrentTenant(tenant);
        _currentTenant = tenant;

        _firstTimeTenantSettings = _scope.ServiceProvider.GetService<FirstTimeTenantSettings>();
        _settingsManager = _scope.ServiceProvider.GetService<SettingsManager>();
        _dbWebStudioSettings = _scope.ServiceProvider.GetService<DbWebstudioSettings>();
        _userManager = _scope.ServiceProvider.GetService<UserManager>();
        _securityContext = _scope.ServiceProvider.GetService<SecurityContext>();
        _userOptions = _scope.ServiceProvider.GetService<IOptions<UserOptions>>().Value;
        _log = _scope.ServiceProvider.GetService<ILogger<BaseApiTests>>();
        _securityContext.AuthenticateMe(_currentTenant.OwnerId);
    }


}
