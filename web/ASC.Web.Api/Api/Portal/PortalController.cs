namespace ASC.Web.Api.Controllers.Portal;

public class PortalController: BasePortalController
{
    public PortalController(IOptionsMonitor<ILog> options,
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

    [Read("")]
    public Tenant Get()
    {
        return Tenant;
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

    [Read("tariff")]
    public Tariff GetTariff()
    {
        return _paymentManager.GetTariff(Tenant.TenantId);
    }

    [Read("userscount")]
    public long GetUsersCount()
    {
        return _coreBaseSettings.Personal ? 1 : _userManager.GetUserNames(EmployeeStatus.Active).Length;
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
}
