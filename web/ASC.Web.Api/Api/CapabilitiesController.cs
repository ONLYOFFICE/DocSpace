namespace ASC.Web.Api.Controllers;

[DefaultRoute]
[ApiController]
[AllowAnonymous]
public class CapabilitiesController : ControllerBase
{
    private readonly CoreBaseSettings _coreBaseSettings;
    private readonly TenantManager _tenantManager;
    private readonly ProviderManager _providerManager;
    private readonly IConfiguration _configuration;
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly ILog _log;


    public CapabilitiesController(
        CoreBaseSettings coreBaseSettings,
        TenantManager tenantManager,
        ProviderManager providerManager,
        IConfiguration configuration,
        IHttpContextAccessor httpContextAccessor,
        IOptionsMonitor<ILog> options)
    {
        _coreBaseSettings = coreBaseSettings;
        _tenantManager = tenantManager;
        _providerManager = providerManager;
        _configuration = configuration;
        _httpContextAccessor = httpContextAccessor;
        _log = options.CurrentValue;
    }

    ///<summary>
    ///Returns the information about portal capabilities
    ///</summary>
    ///<short>
    ///Get portal capabilities
    ///</short>
    ///<returns>CapabilitiesData</returns>
    [Read(Check = false)] //NOTE: this method doesn't requires auth!!!  //NOTE: this method doesn't check payment!!!
    public CapabilitiesWrapper GetPortalCapabilities()
    {
        var result = new CapabilitiesWrapper
        {
            LdapEnabled = false,
            Providers = null,
            SsoLabel = string.Empty,
            SsoUrl = string.Empty
        };

        try
        {
            if (SetupInfo.IsVisibleSettings(nameof(ManagementType.LdapSettings))
                && (!_coreBaseSettings.Standalone
                    || _tenantManager.GetTenantQuota(_tenantManager.GetCurrentTenant().TenantId).Ldap))
            {
                //var settings = SettingsManager.Load<LdapSettings>();

                //result.LdapEnabled = settings.EnableLdapAuthentication;
                result.LdapEnabled = false;
            }
        }
        catch (Exception ex)
        {
            _log.Error(ex.Message);
        }

        try
        {
            result.Providers = ProviderManager.AuthProviders.Where(loginProvider =>
            {
                var provider = _providerManager.GetLoginProvider(loginProvider);
                return provider != null && provider.IsEnabled;
            })
            .ToList();
        }
        catch (Exception ex)
        {
            _log.Error(ex.Message);
        }

        try
        {
            if (SetupInfo.IsVisibleSettings(nameof(ManagementType.SingleSignOnSettings))
                && _tenantManager.GetTenantQuota(_tenantManager.GetCurrentTenant().TenantId).Sso)
            {
                //var settings = SettingsManager.Load<SsoSettingsV2>();

                //if (settings.EnableSso)
                //{
                var uri = _httpContextAccessor.HttpContext.Request.GetUrlRewriter();

                var configUrl = _configuration["web:sso:saml:login:url"] ?? "";

                result.SsoUrl = $"{uri.Scheme}://{uri.Host}{((uri.Port == 80 || uri.Port == 443) ? "" : ":" + uri.Port)}{configUrl}";
                result.SsoLabel = string.Empty;
                //    result.SsoLabel = settings.SpLoginLabel;
                //}
            }
        }
        catch (Exception ex)
        {
            _log.Error(ex.Message);
        }

        return result;
    }
}