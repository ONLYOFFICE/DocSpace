


using System;
using System.Linq;
using System.Web;

using ASC.Common.Logging;
using ASC.Core;
using ASC.Core.Common.Settings;
using ASC.FederatedLogin.LoginProviders;
using ASC.Web.Api.Models;
using ASC.Web.Api.Routing;
using ASC.Web.Studio.Core;
using ASC.Web.Studio.Utility;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;

namespace ASC.Web.Api.Controllers
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
                if (SetupInfo.IsVisibleSettings(nameof(ManagementType.LdapSettings))
                    && (!CoreBaseSettings.Standalone
                        || TenantManager.GetTenantQuota(TenantManager.GetCurrentTenant().TenantId).Ldap))
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
                if (SetupInfo.IsVisibleSettings(nameof(ManagementType.SingleSignOnSettings))
                    && TenantManager.GetTenantQuota(TenantManager.GetCurrentTenant().TenantId).Sso)
                {
                    //var settings = SettingsManager.Load<SsoSettingsV2>();

                    //if (settings.EnableSso)
                    //{
                    var uri = HttpContextAccessor.HttpContext.Request.GetUrlRewriter();

                    var configUrl = Configuration["web:sso:saml:login:url"] ?? "";

                    result.SsoUrl = $"{uri.Scheme}://{uri.Host}{((uri.Port == 80 || uri.Port == 443) ? "" : ":" + uri.Port)}{configUrl}";
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
