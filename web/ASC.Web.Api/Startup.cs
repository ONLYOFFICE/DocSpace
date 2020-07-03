
using ASC.Api.Core;
using ASC.Api.Core.Auth;
using ASC.Api.Core.Middleware;
using ASC.Api.Settings;
using ASC.Common;
using ASC.Common.Logging;
using ASC.Common.DependencyInjection;
using ASC.Web.Api.Controllers;

using Autofac.Extensions.DependencyInjection;

using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace ASC.Web.Api
{
    public class Startup
    {
        public IConfiguration Configuration { get; }
        public IHostEnvironment HostEnvironment { get; }

        public Startup(IConfiguration configuration, IHostEnvironment hostEnvironment)
        {
            Configuration = configuration;
            HostEnvironment = hostEnvironment;
        }
        public void ConfigureServices(IServiceCollection services)
        {
            var diHelper = new DIHelper(services);

            diHelper
                .AddConfirmAuthHandler()
                .AddCookieAuthHandler()
                .AddCultureMiddleware()
                .AddIpSecurityFilter()
                .AddPaymentFilter()
                .AddProductSecurityFilter()
                .AddTenantStatusFilter();

            diHelper.AddNLogManager("ASC.Api", "ASC.Web");

            diHelper
                .AddAuthenticationController()
                .AddModulesController()
                .AddPortalController()
                .AddSettingsController()
                .AddSmtpSettingsController();
            GeneralStartup.ConfigureServices(services, true);
            services.AddAutofac(Configuration, HostEnvironment.ContentRootPath);

        }
        public void Configure(IApplicationBuilder app)
        {
            GeneralStartup.Configure(app);
        }
    }
}
