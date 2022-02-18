using SecurityContext = ASC.Core.SecurityContext;

namespace ASC.Web.Api.Controllers;

[Scope]
[DefaultRoute]
[ApiController]
public class PortalController : ControllerBase
{
    private Tenant Tenant { get { return _apiContext.Tenant; } }

    private readonly ApiContext _apiContext;
    private readonly UserManager _userManager;
    private readonly TenantManager _tenantManager;
    private readonly PaymentManager _paymentManager;
    private readonly CommonLinkUtility _commonLinkUtility;
    private readonly UrlShortener _urlShortener;
    private readonly AuthContext _authContext;
    private readonly WebItemSecurity _webItemSecurity;
    private readonly SecurityContext _securityContext;
    private readonly SettingsManager _settingsManager;
    private readonly IMobileAppInstallRegistrator _mobileAppInstallRegistrator;
    private readonly IConfiguration _configuration;
    private readonly CoreBaseSettings _coreBaseSettings;
    private readonly LicenseReader _licenseReader;
    private readonly SetupInfo _setupInfo;
    private readonly DocumentServiceLicense _documentServiceLicense;
    private readonly TenantExtra _tenantExtra;
    private readonly ILog _log;
    private readonly IHttpClientFactory _clientFactory;


    public PortalController(
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

    [Read("")]
    public Tenant Get()
    {
        return Tenant;
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

    [Update("getshortenlink")]
    public object GetShortenLink(ShortenLinkDto model)
    {
        try
        {
            return _urlShortener.Instance.GetShortenLink(model.Link);
        }
        catch (Exception ex)
        {
            _log.Error("getshortenlink", ex);
            return model.Link;
        }
    }

    [Read("tenantextra")]
    public object GetTenantExtra()
    {
        return new
        {
            customMode = _coreBaseSettings.CustomMode,
            opensource = _tenantExtra.Opensource,
            enterprise = _tenantExtra.Enterprise,
            tariff = _tenantExtra.GetCurrentTariff(),
            quota = _tenantExtra.GetTenantQuota(),
            notPaid = _tenantExtra.IsNotPaid(),
            licenseAccept = _settingsManager.LoadForCurrentUser<TariffSettings>().LicenseAcceptSetting,
            enableTariffPage = //TenantExtra.EnableTarrifSettings - think about hide-settings for opensource
                (!_coreBaseSettings.Standalone || !string.IsNullOrEmpty(_licenseReader.LicensePath))
                && string.IsNullOrEmpty(_setupInfo.AmiMetaUrl)
                && !_coreBaseSettings.CustomMode,
            DocServerUserQuota = _documentServiceLicense.GetLicenseQuota(),
            DocServerLicense = _documentServiceLicense.GetLicense()
        };
    }


    [Read("usedspace")]
    public double GetUsedSpace()
    {
        return Math.Round(
            _tenantManager.FindTenantQuotaRows(Tenant.TenantId)
                        .Where(q => !string.IsNullOrEmpty(q.Tag) && new Guid(q.Tag) != Guid.Empty)
                        .Sum(q => q.Counter) / 1024f / 1024f / 1024f, 2);
    }


    [Read("userscount")]
    public long GetUsersCount()
    {
        return _coreBaseSettings.Personal ? 1 : _userManager.GetUserNames(EmployeeStatus.Active).Length;
    }

    [Read("tariff")]
    public Tariff GetTariff()
    {
        return _paymentManager.GetTariff(Tenant.TenantId);
    }

    [Read("quota")]
    public TenantQuota GetQuota()
    {
        return _tenantManager.GetTenantQuota(Tenant.TenantId);
    }

    [Read("quota/right")]
    public TenantQuota GetRightQuota()
    {
        var usedSpace = GetUsedSpace();
        var needUsersCount = GetUsersCount();

        return _tenantManager.GetTenantQuotas().OrderBy(r => r.Price)
                            .FirstOrDefault(quota =>
                                            quota.ActiveUsers > needUsersCount
                                            && quota.MaxTotalSize > usedSpace
                                            && !quota.Year);
    }


    [Read("path")]
    public object GetFullAbsolutePath(string virtualPath)
    {
        return _commonLinkUtility.GetFullAbsolutePath(virtualPath);
    }

    [Read("thumb")]
    public FileResult GetThumb(string url)
    {
        if (!_securityContext.IsAuthenticated || _configuration["bookmarking:thumbnail-url"] == null)
        {
            return null;
        }

        url = url.Replace("&amp;", "&");
        url = WebUtility.UrlEncode(url);

        var request = new HttpRequestMessage();
        request.RequestUri = new Uri(string.Format(_configuration["bookmarking:thumbnail-url"], url));

        var httpClient = _clientFactory.CreateClient();
        using var response = httpClient.Send(request);
        using var stream = response.Content.ReadAsStream();
        var bytes = new byte[stream.Length];
        stream.Read(bytes, 0, (int)stream.Length);

        string type;
        if (response.Headers.TryGetValues("Content-Type", out var values))
        {
            type = values.First();
        }
        else
        {
            type = "image/png";
        }
        return File(bytes, type);
    }

    [Create("present/mark")]
    public void MarkPresentAsReaded()
    {
        try
        {
            var settings = _settingsManager.LoadForCurrentUser<OpensourceGiftSettings>();
            settings.Readed = true;
            _settingsManager.SaveForCurrentUser(settings);
        }
        catch (Exception ex)
        {
            _log.Error("MarkPresentAsReaded", ex);
        }
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