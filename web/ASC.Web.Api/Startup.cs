namespace ASC.Web.Api
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
            DIHelper.TryAdd<PhoneController>();

            DIHelper.TryAdd<PortalController>();
            DIHelper.TryAdd<MobileController>();
            DIHelper.TryAdd<UserController>();

           
            DIHelper.TryAdd<SettingsController>();
            DIHelper.TryAdd<CustomNavigationController>();
            DIHelper.TryAdd<CustomSchemasController>();
            DIHelper.TryAdd<GreetingSettingsController>();
            DIHelper.TryAdd<IpRestrictionsController>();
            DIHelper.TryAdd<LicenseController>();
            DIHelper.TryAdd<MessageSettingsController>();
            DIHelper.TryAdd<OwnerController>();
            DIHelper.TryAdd<Controllers.Settings.SecurityController>();
            DIHelper.TryAdd<StorageController>();
            DIHelper.TryAdd<TfaappController>();
            DIHelper.TryAdd<TipsController>();
            DIHelper.TryAdd<VersionController>();
            DIHelper.TryAdd<WebhooksController>();
            DIHelper.TryAdd<WhitelabelController>();

            DIHelper.TryAdd<Controllers.Security.SecurityController>();
            DIHelper.TryAdd<EventsController>();
            DIHelper.TryAdd<LoginController>();

            DIHelper.TryAdd<SmtpSettingsController>();
            DIHelper.TryAdd<ThirdPartyController>();
            DIHelper.TryAdd<ModulesController>();
        }
    }
}
