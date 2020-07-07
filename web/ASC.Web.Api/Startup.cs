
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
using Microsoft.AspNetCore.Hosting;

namespace ASC.Web.Api
{
    public class Startup : BaseStartup
    {
        public Startup(IConfiguration configuration, IHostEnvironment hostEnvironment): base(configuration, hostEnvironment)
        {

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
            LogParams = new string[] { "ASC.Api", "ASC.Web" };
            diHelper
                .AddAuthenticationController()
                .AddModulesController()
                .AddPortalController()
                .AddSettingsController()
                .AddSmtpSettingsController();
            addcontrollers = true;
            confirmAddScheme = true;
            base.ConfigureServices(services);
            services.AddAutofac(Configuration, HostEnvironment.ContentRootPath);

        }
        public override void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            base.Configure(app, env);
        }
    }
}
