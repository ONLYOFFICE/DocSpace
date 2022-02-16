namespace ASC.Web.Api.Controllers.Portal;

public class UserController : BasePortalController
{
    public UserController(IOptionsMonitor<ILog> options,
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

    [Read("users/{userID}")]
    public UserInfo GetUser(Guid userID)
    {
        return _userManager.GetUsers(userID);
    }

    [Read("users/invite/{employeeType}")]
    public object GeInviteLink(EmployeeType employeeType)
    {
        if (!_webItemSecurity.IsProductAdministrator(WebItemManager.PeopleProductID, _authContext.CurrentAccount.ID))
        {
            throw new SecurityException("Method not available");
        }

        return _commonLinkUtility.GetConfirmationUrl(string.Empty, ConfirmType.LinkInvite, (int)employeeType)
                + $"&emplType={employeeType:d}";
    }
}