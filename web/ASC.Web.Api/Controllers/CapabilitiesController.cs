﻿namespace ASC.Web.Api.Controllers
{
    [DefaultRoute]
    [ApiController]
    [AllowAnonymous]
    public class CapabilitiesController : ControllerBase
    {
        private SetupInfo SetupInfo { get; }
        private CoreBaseSettings CoreBaseSettings { get; }
        private TenantManager TenantManager { get; }
        private SettingsManager SettingsManager { get; }
        private ProviderManager ProviderManager { get; }
        private IConfiguration Configuration { get; }
        private IHttpContextAccessor HttpContextAccessor { get; }
        private ILog Log { get; }


        public CapabilitiesController(
            SetupInfo setupInfo,
            CoreBaseSettings coreBaseSettings,
            TenantManager tenantManager,
            SettingsManager settingsManager,
            ProviderManager providerManager,
            IConfiguration configuration,
            IHttpContextAccessor httpContextAccessor,
            IOptionsMonitor<ILog> options)
        {
            SetupInfo = setupInfo;
            CoreBaseSettings = coreBaseSettings;
            TenantManager = tenantManager;
            SettingsManager = settingsManager;
            ProviderManager = providerManager;
            Configuration = configuration;
            HttpContextAccessor = httpContextAccessor;
            Log = options.CurrentValue;
        }

        ///<summary>
        ///Returns the information about portal capabilities
        ///</summary>
        ///<short>
        ///Get portal capabilities
        ///</short>
        ///<returns>CapabilitiesData</returns>
        [Read(Check = false)] //NOTE: this method doesn't requires auth!!!  //NOTE: this method doesn't check payment!!!
        public CapabilitiesData GetPortalCapabilities()
        {
            var result = new CapabilitiesData
            {
                LdapEnabled = false,
                Providers = null,
                SsoLabel = string.Empty,
                SsoUrl = string.Empty
            };

            try
            {
                if (SetupInfo.IsVisibleSettings(ManagementType.LdapSettings.ToString())
                    && (!CoreBaseSettings.Standalone
                        || TenantManager.GetTenantQuota(TenantManager.GetCurrentTenant().Id).Ldap))
                {
                    //var settings = SettingsManager.Load<LdapSettings>();

                    //result.LdapEnabled = settings.EnableLdapAuthentication;
                    result.LdapEnabled = false;
                }
            }
            catch (Exception ex)
            {
                Log.Error(ex.Message);
            }

            try
            {
                result.Providers = ProviderManager.AuthProviders.Where(loginProvider =>
                {
                    var provider = ProviderManager.GetLoginProvider(loginProvider);
                    return provider != null && provider.IsEnabled;
                })
                .ToList();
            }
            catch (Exception ex)
            {
                Log.Error(ex.Message);
            }

            try
            {
                if (SetupInfo.IsVisibleSettings(ManagementType.SingleSignOnSettings.ToString())
                    && TenantManager.GetTenantQuota(TenantManager.GetCurrentTenant().Id).Sso)
                {
                    //var settings = SettingsManager.Load<SsoSettingsV2>();

                    //if (settings.EnableSso)
                    //{
                    var uri = HttpContextAccessor.HttpContext.Request.GetUrlRewriter();

                    var configUrl = Configuration["web:sso:saml:login:url"] ?? "";

                    result.SsoUrl = string.Format("{0}://{1}{2}{3}", uri.Scheme, uri.Host,
                                                  (uri.Port == 80 || uri.Port == 443) ? "" : ":" + uri.Port, configUrl);
                    result.SsoLabel = string.Empty;
                    //    result.SsoLabel = settings.SpLoginLabel;
                    //}
                }
            }
            catch (Exception ex)
            {
                Log.Error(ex.Message);
            }

            return result;
        }
    }
}
