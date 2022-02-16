using SecurityContext = ASC.Core.SecurityContext;

namespace ASC.Web.Api.Controllers.Portal;

[Scope]
[DefaultRoute]
[ApiController]
[ControllerName("portal")]
public class BasePortalController : ControllerBase
{
    internal Tenant Tenant { get { return _apiContext.Tenant; } }
    internal readonly ApiContext _apiContext;
    internal readonly UserManager _userManager;
    internal readonly TenantManager _tenantManager;
    internal readonly PaymentManager _paymentManager;
    internal readonly CommonLinkUtility _commonLinkUtility;
    internal readonly UrlShortener _urlShortener;
    internal readonly AuthContext _authContext;
    internal readonly WebItemSecurity _webItemSecurity;
    internal readonly SecurityContext _securityContext;
    internal readonly SettingsManager _settingsManager;
    internal readonly IMobileAppInstallRegistrator _mobileAppInstallRegistrator;
    internal readonly IConfiguration _configuration;
    internal readonly CoreBaseSettings _coreBaseSettings;
    internal readonly LicenseReader _licenseReader;
    internal readonly SetupInfo _setupInfo;
    internal readonly DocumentServiceLicense _documentServiceLicense;
    internal readonly TenantExtra _tenantExtra;
    internal readonly ILog _log;
    internal readonly IHttpClientFactory _clientFactory;


    public BasePortalController(
        IOptionsMonitor<ILog> options,
        ApiContext apiContext,
        UserManager userManager,
        TenantManager tenantManager,
        PaymentManager paymentManager,
        CommonLinkUtility commonLinkUtility,
        UrlShortener urlShortener,
        AuthContext authContext,
        WebItemSecurity webItemSecurity,
        SecurityContext securityContext,
        SettingsManager settingsManager,
        IMobileAppInstallRegistrator mobileAppInstallRegistrator,
        TenantExtra tenantExtra,
        IConfiguration configuration,
        CoreBaseSettings coreBaseSettings,
        LicenseReader licenseReader,
        SetupInfo setupInfo,
        DocumentServiceLicense documentServiceLicense,
        IHttpClientFactory clientFactory
        )
    {
        _log = options.CurrentValue;
        _apiContext = apiContext;
        _userManager = userManager;
        _tenantManager = tenantManager;
        _paymentManager = paymentManager;
        _commonLinkUtility = commonLinkUtility;
        _urlShortener = urlShortener;
        _authContext = authContext;
        _webItemSecurity = webItemSecurity;
        _securityContext = securityContext;
        _settingsManager = settingsManager;
        _mobileAppInstallRegistrator = mobileAppInstallRegistrator;
        _configuration = configuration;
        _coreBaseSettings = coreBaseSettings;
        _licenseReader = licenseReader;
        _setupInfo = setupInfo;
        _documentServiceLicense = documentServiceLicense;
        _tenantExtra = tenantExtra;
        _clientFactory = clientFactory;
    }
}