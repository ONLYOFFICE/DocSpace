
using ASC.Api.Core;
using ASC.Api.Settings;
using ASC.Common;
using ASC.Common.DependencyInjection;
using ASC.Web.Api.Controllers;

using Autofac;

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace ASC.Web.Api
{
    public class Startup : BaseStartup
    {
        public override string[] LogParams { get => new string[] { "ASC.Api", "ASC.Web" }; }
        public override bool ConfirmAddScheme { get => true; }

        public Startup(IConfiguration configuration, IHostEnvironment hostEnvironment) : base(configuration, hostEnvironment)
        {

        }

        public override void ConfigureServices(IServiceCollection services)
        {
            base.ConfigureServices(services);

            services.AddMemoryCache();

            services.AddOptions();

            DIHelper
                .AddAuthenticationController()
                .AddModulesController()
                .AddPortalController()
                .AddSettingsController()
                .AddSecurityController()
                .AddSmtpSettingsController();
        }

        public void ConfigureContainer(ContainerBuilder builder)
        {
            builder.Register(Configuration, HostEnvironment.ContentRootPath);
        }
    }
}
