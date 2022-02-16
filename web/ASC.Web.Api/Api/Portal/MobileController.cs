namespace ASC.Web.Api.Controllers.Portal;

public class MobileController : BasePortalController
{
    public MobileController(IOptionsMonitor<ILog> options,
        ApiContext apiContext, 
        UserManager userManager,
        TenantManager tenantManager,
        PaymentManager paymentManager,
        CommonLinkUtility commonLinkUtility, 
        UrlShortener urlShortener,
        AuthContext authContext,
        WebItemSecurity webItemSecurity,
        ASC.Core.SecurityContext securityContext,
        SettingsManager settingsManager,
        IMobileAppInstallRegistrator mobileAppInstallRegistrator,
        TenantExtra tenantExtra,
        IConfiguration configuration,
        CoreBaseSettings coreBaseSettings,
        LicenseReader licenseReader,
        SetupInfo setupInfo, 
        DocumentServiceLicense documentServiceLicense,
        IHttpClientFactory clientFactory) : base(options, apiContext, userManager, tenantManager, paymentManager, commonLinkUtility, urlShortener, authContext, webItemSecurity, securityContext, settingsManager, mobileAppInstallRegistrator, tenantExtra, configuration, coreBaseSettings, licenseReader, setupInfo, documentServiceLicense, clientFactory)
    {
    }

    [Create("mobile/registration")]
    public void RegisterMobileAppInstallFromBody([FromBody] MobileAppDto model)
    {
        var currentUser = _userManager.GetUsers(_securityContext.CurrentAccount.ID);
        _mobileAppInstallRegistrator.RegisterInstall(currentUser.Email, model.Type);
    }

    [Create("mobile/registration")]
    [Consumes("application/x-www-form-urlencoded")]
    public void RegisterMobileAppInstallFromForm([FromForm] MobileAppDto model)
    {
        var currentUser = _userManager.GetUsers(_securityContext.CurrentAccount.ID);
        _mobileAppInstallRegistrator.RegisterInstall(currentUser.Email, model.Type);
    }

    [Create("mobile/registration")]
    public void RegisterMobileAppInstall(MobileAppType type)
    {
        var currentUser = _userManager.GetUsers(_securityContext.CurrentAccount.ID);
        _mobileAppInstallRegistrator.RegisterInstall(currentUser.Email, type);
    }
}
