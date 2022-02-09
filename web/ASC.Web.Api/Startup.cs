﻿namespace ASC.Web.Api
{
    public class Startup : BaseStartup
    {
        public override bool ConfirmAddScheme { get => true; }

        public Startup(IConfiguration configuration, IHostEnvironment hostEnvironment) : base(configuration, hostEnvironment)
        {

        }

        public override void ConfigureServices(IServiceCollection services)
        {
            base.ConfigureServices(services);

            services.AddMemoryCache();

            DIHelper.TryAdd<AuthenticationController>();
            DIHelper.TryAdd<ModulesController>();
            DIHelper.TryAdd<PortalController>();
            DIHelper.TryAdd<SettingsController>();
            DIHelper.TryAdd<SecurityController>();
            DIHelper.TryAdd<SmtpSettingsController>();
            DIHelper.TryAdd<ThirdPartyController>();
        }
    }
}
