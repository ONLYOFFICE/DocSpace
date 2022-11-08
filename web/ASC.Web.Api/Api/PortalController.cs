// (c) Copyright Ascensio System SIA 2010-2022
//
// This program is a free software product.
// You can redistribute it and/or modify it under the terms
// of the GNU Affero General Public License (AGPL) version 3 as published by the Free Software
// Foundation. In accordance with Section 7(a) of the GNU AGPL its Section 15 shall be amended
// to the effect that Ascensio System SIA expressly excludes the warranty of non-infringement of
// any third-party rights.
//
// This program is distributed WITHOUT ANY WARRANTY, without even the implied warranty
// of MERCHANTABILITY or FITNESS FOR A PARTICULAR  PURPOSE. For details, see
// the GNU AGPL at: http://www.gnu.org/licenses/agpl-3.0.html
//
// You can contact Ascensio System SIA at Lubanas st. 125a-25, Riga, Latvia, EU, LV-1021.
//
// The  interactive user interfaces in modified source and object code versions of the Program must
// display Appropriate Legal Notices, as required under Section 5 of the GNU AGPL version 3.
//
// Pursuant to Section 7(b) of the License you must retain the original Product logo when
// distributing the program. Pursuant to Section 7(e) we decline to grant you any rights under
// trademark law for use of our trademarks.
//
// All the Product's GUI elements, including illustrations and icon sets, as well as technical writing
// content are licensed under the terms of the Creative Commons Attribution-ShareAlike 4.0
// International. See the License terms at http://creativecommons.org/licenses/by-sa/4.0/legalcode

namespace ASC.Web.Api.Controllers;

[Scope]
[DefaultRoute]
[ApiController]
public class PortalController : ControllerBase
{
    protected Tenant Tenant { get { return _apiContext.Tenant; } }

    private readonly ApiContext _apiContext;
    protected readonly UserManager _userManager;
    protected readonly TenantManager _tenantManager;
    protected readonly ITariffService _tariffService;
    private readonly CommonLinkUtility _commonLinkUtility;
    private readonly UrlShortener _urlShortener;
    private readonly AuthContext _authContext;
    private readonly WebItemSecurity _webItemSecurity;
    protected readonly SecurityContext _securityContext;
    private readonly SettingsManager _settingsManager;
    private readonly IMobileAppInstallRegistrator _mobileAppInstallRegistrator;
    private readonly IConfiguration _configuration;
    private readonly CoreBaseSettings _coreBaseSettings;
    private readonly LicenseReader _licenseReader;
    private readonly SetupInfo _setupInfo;
    private readonly DocumentServiceLicense _documentServiceLicense;
    private readonly TenantExtra _tenantExtra;
    private readonly ILogger<PortalController> _log;
    private readonly IHttpClientFactory _clientFactory;
    private readonly ApiSystemHelper _apiSystemHelper;
    private readonly CoreSettings _coreSettings;
    private readonly PermissionContext _permissionContext;
    private readonly StudioNotifyService _studioNotifyService;
    private readonly MessageService _messageService;
    private readonly MessageTarget _messageTarget;
    private readonly DisplayUserSettingsHelper _displayUserSettingsHelper;

    public PortalController(
        ILogger<PortalController> logger,
        ApiContext apiContext,
        UserManager userManager,
        TenantManager tenantManager,
        ITariffService tariffService,
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
        IHttpClientFactory clientFactory,
        ApiSystemHelper apiSystemHelper,
        CoreSettings coreSettings,
        PermissionContext permissionContext,
        StudioNotifyService studioNotifyService,
        MessageService messageService,
        MessageTarget messageTarget,
        DisplayUserSettingsHelper displayUserSettingsHelper
        )
    {
        _log = logger;
        _apiContext = apiContext;
        _userManager = userManager;
        _tenantManager = tenantManager;
        _tariffService = tariffService;
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
        _apiSystemHelper = apiSystemHelper;
        _coreSettings = coreSettings;
        _permissionContext = permissionContext;
        _studioNotifyService = studioNotifyService;
        _messageService = messageService;
        _messageTarget = messageTarget;
        _displayUserSettingsHelper = displayUserSettingsHelper;
    }

    [HttpGet("")]
    public Tenant Get()
    {
        return Tenant;
    }

    [HttpGet("users/{userID}")]
    public UserInfo GetUser(Guid userID)
    {
        return _userManager.GetUsers(userID);
    }

    [HttpGet("users/invite/{employeeType}")]
    public object GeInviteLink(EmployeeType employeeType)
    {
        if (!_webItemSecurity.IsProductAdministrator(WebItemManager.PeopleProductID, _authContext.CurrentAccount.ID))
        {
            throw new SecurityException("Method not available");
        }

        return _commonLinkUtility.GetConfirmationEmailUrl(string.Empty, ConfirmType.LinkInvite, (int)employeeType, _authContext.CurrentAccount.ID)
                + $"&emplType={employeeType:d}";
    }

    [HttpPut("getshortenlink")]
    public async Task<object> GetShortenLinkAsync(ShortenLinkRequestsDto inDto)
    {
        try
        {
            return await _urlShortener.Instance.GetShortenLinkAsync(inDto.Link);
        }
        catch (Exception ex)
        {
            _log.ErrorGetShortenLink(ex);
            return inDto.Link;
        }
    }

    [HttpGet("tenantextra")]
    public async Task<object> GetTenantExtraAsync()
    {
        return new
        {
            customMode = _coreBaseSettings.CustomMode,
            opensource = _tenantExtra.Opensource,
            enterprise = _tenantExtra.Enterprise,
            tariff = _tenantExtra.GetCurrentTariff(),
            quota = _tenantManager.GetCurrentTenantQuota(),
            notPaid = _tenantExtra.IsNotPaid(),
            licenseAccept = _settingsManager.LoadForCurrentUser<TariffSettings>().LicenseAcceptSetting,
            enableTariffPage = //TenantExtra.EnableTarrifSettings - think about hide-settings for opensource
                (!_coreBaseSettings.Standalone || !string.IsNullOrEmpty(_licenseReader.LicensePath))
                && string.IsNullOrEmpty(_setupInfo.AmiMetaUrl)
                && !_coreBaseSettings.CustomMode,
            DocServerUserQuota = await _documentServiceLicense.GetLicenseQuotaAsync(),
            DocServerLicense = await _documentServiceLicense.GetLicenseAsync()
        };
    }


    [HttpGet("usedspace")]
    public double GetUsedSpace()
    {
        return Math.Round(
            _tenantManager.FindTenantQuotaRows(Tenant.Id)
                        .Where(q => !string.IsNullOrEmpty(q.Tag) && new Guid(q.Tag) != Guid.Empty)
                        .Sum(q => q.Counter) / 1024f / 1024f / 1024f, 2);
    }


    [HttpGet("userscount")]
    public long GetUsersCount()
    {
        return _coreBaseSettings.Personal ? 1 : _userManager.GetUserNames(EmployeeStatus.Active).Length;
    }

    [AllowNotPayment]
    [HttpGet("tariff")]
    public Tariff GetTariff()
    {
        return _tariffService.GetTariff(Tenant.Id);
    }

    [AllowNotPayment]
    [HttpGet("quota")]
    public TenantQuota GetQuota()
    {
        return _tenantManager.GetTenantQuota(Tenant.Id);
    }

    [HttpGet("quota/right")]
    public TenantQuota GetRightQuota()
    {
        var usedSpace = GetUsedSpace();
        var needUsersCount = GetUsersCount();

        return _tenantManager.GetTenantQuotas().OrderBy(r => r.Price)
                            .FirstOrDefault(quota =>
                                            quota.CountUser > needUsersCount
                                            && quota.MaxTotalSize > usedSpace);
    }


    [HttpGet("path")]
    public object GetFullAbsolutePath(string virtualPath)
    {
        return _commonLinkUtility.GetFullAbsolutePath(virtualPath);
    }

    [HttpGet("thumb")]
    public FileResult GetThumb(string url)
    {
        if (!_securityContext.IsAuthenticated || _configuration["bookmarking:thumbnail-url"] == null)
        {
            return null;
        }

        url = url.Replace("&amp;", "&");
        url = WebUtility.UrlEncode(url);

        var request = new HttpRequestMessage
        {
            RequestUri = new Uri(string.Format(_configuration["bookmarking:thumbnail-url"], url))
        };

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

    [HttpPost("present/mark")]
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
            _log.ErrorMarkPresentAsReaded(ex);
        }
    }

    [HttpPost("mobile/registration")]
    public void RegisterMobileAppInstall(MobileAppRequestsDto inDto)
    {
        var currentUser = _userManager.GetUsers(_securityContext.CurrentAccount.ID);
        _mobileAppInstallRegistrator.RegisterInstall(currentUser.Email, inDto.Type);
    }

    [HttpPost("mobile/registration")]
    public void RegisterMobileAppInstall(MobileAppType type)
    {
        var currentUser = _userManager.GetUsers(_securityContext.CurrentAccount.ID);
        _mobileAppInstallRegistrator.RegisterInstall(currentUser.Email, type);
    }

    /// <summary>
    /// Updates a portal name with a new one specified in the request.
    /// </summary>
    /// <short>Update a portal name</short>
    /// <param name="alias">New portal name</param>
    /// <returns>Message about renaming a portal</returns>
    ///<visible>false</visible>
    [HttpPut("portalrename")]
    public async Task<object> UpdatePortalName(PortalRenameRequestsDto model)
    {
        if (!SetupInfo.IsVisibleSettings(nameof(ManagementType.PortalSecurity)))
        {
            throw new BillingException(Resource.ErrorNotAllowedOption, "PortalRename");
        }

        if (_coreBaseSettings.Personal)
        {
            throw new Exception(Resource.ErrorAccessDenied);
        }

        _permissionContext.DemandPermissions(SecutiryConstants.EditPortalSettings);

        var alias = model.Alias;
        if (string.IsNullOrEmpty(alias))
        {
            throw new ArgumentException(nameof(alias));
        }

        var tenant = _tenantManager.GetCurrentTenant();
        var user = _userManager.GetUsers(_securityContext.CurrentAccount.ID);

        var localhost = _coreSettings.BaseDomain == "localhost" || tenant.Alias == "localhost";

        var newAlias = alias.ToLowerInvariant();
        var oldAlias = tenant.Alias;
        var oldVirtualRootPath = _commonLinkUtility.GetFullAbsolutePath("~").TrimEnd('/');

        if (!string.Equals(newAlias, oldAlias, StringComparison.InvariantCultureIgnoreCase))
        {
            if (!string.IsNullOrEmpty(_apiSystemHelper.ApiSystemUrl))
            {
                await _apiSystemHelper.ValidatePortalNameAsync(newAlias, user.Id);
            }
            else
            {
                _tenantManager.CheckTenantAddress(newAlias.Trim());
            }


            if (!string.IsNullOrEmpty(_apiSystemHelper.ApiCacheUrl))
            {
                await _apiSystemHelper.AddTenantToCacheAsync(newAlias, user.Id);
            }

            tenant.Alias = alias;
            tenant = _tenantManager.SaveTenant(tenant);


            if (!string.IsNullOrEmpty(_apiSystemHelper.ApiCacheUrl))
            {
                await _apiSystemHelper.RemoveTenantFromCacheAsync(oldAlias, user.Id);
            }

            if (!localhost || string.IsNullOrEmpty(tenant.MappedDomain))
            {
                _studioNotifyService.PortalRenameNotify(tenant, oldVirtualRootPath);
            }
        }
        else
        {
            return string.Empty;
        }

        return _commonLinkUtility.GetConfirmationEmailUrl(user.Email, ConfirmType.Auth);
    }

    [HttpDelete("deleteportalimmediately")]
    public async Task DeletePortalImmediately()
    {
        var tenant = _tenantManager.GetCurrentTenant();

        if (_securityContext.CurrentAccount.ID != tenant.OwnerId)
        {
            throw new Exception(Resource.ErrorAccessDenied);
        }

        _tenantManager.RemoveTenant(tenant.Id);

        if (!string.IsNullOrEmpty(_apiSystemHelper.ApiCacheUrl))
        {
            await _apiSystemHelper.RemoveTenantFromCacheAsync(tenant.Alias, _securityContext.CurrentAccount.ID);
        }

        try
        {
            if (!_securityContext.IsAuthenticated)
            {
                _securityContext.AuthenticateMeWithoutCookie(ASC.Core.Configuration.Constants.CoreSystem);
            }

            _messageService.Send(MessageAction.PortalDeleted);
        }
        finally
        {
            _securityContext.Logout();
        }
    }

    [HttpPost("suspend")]
    public void SendSuspendInstructions()
    {
        _permissionContext.DemandPermissions(SecutiryConstants.EditPortalSettings);

        var owner = _userManager.GetUsers(Tenant.OwnerId);
        var suspendUrl = _commonLinkUtility.GetConfirmationEmailUrl(owner.Email, ConfirmType.PortalSuspend);
        var continueUrl = _commonLinkUtility.GetConfirmationEmailUrl(owner.Email, ConfirmType.PortalContinue);

        _studioNotifyService.SendMsgPortalDeactivation(Tenant, suspendUrl, continueUrl);

        _messageService.Send(MessageAction.OwnerSentPortalDeactivationInstructions, _messageTarget.Create(owner.Id), owner.DisplayUserName(false, _displayUserSettingsHelper));
    }

    [HttpPost("delete")]
    public void SendDeleteInstructions()
    {
        _permissionContext.DemandPermissions(SecutiryConstants.EditPortalSettings);
        var owner = _userManager.GetUsers(Tenant.OwnerId);

        var showAutoRenewText = !_coreBaseSettings.Standalone &&
                        _tariffService.GetPayments(Tenant.Id).Any() &&
                        !_tenantManager.GetCurrentTenantQuota().Trial;

        _studioNotifyService.SendMsgPortalDeletion(Tenant, _commonLinkUtility.GetConfirmationEmailUrl(owner.Email, ConfirmType.PortalRemove), showAutoRenewText);

        _messageService.Send(MessageAction.OwnerSentPortalDeleteInstructions, _messageTarget.Create(owner.Id), owner.DisplayUserName(false, _displayUserSettingsHelper));
    }

    [AllowSuspended]
    [HttpPut("continue")]
    [Authorize(AuthenticationSchemes = "confirm", Roles = "PortalContinue")]
    public void ContinuePortal()
    {
        Tenant.SetStatus(TenantStatus.Active);
        _tenantManager.SaveTenant(Tenant);
    }

    [HttpPut("suspend")]
    [Authorize(AuthenticationSchemes = "confirm", Roles = "PortalSuspend")]
    public void SuspendPortal()
    {
        Tenant.SetStatus(TenantStatus.Suspended);
        _tenantManager.SaveTenant(Tenant);
        _messageService.Send(MessageAction.PortalDeactivated);
    }

    [HttpDelete("delete")]
    [Authorize(AuthenticationSchemes = "confirm", Roles = "PortalRemove")]
    public async Task<object> DeletePortal()
    {
        _tenantManager.RemoveTenant(Tenant.Id);

        if (!string.IsNullOrEmpty(_apiSystemHelper.ApiCacheUrl))
        {
            await _apiSystemHelper.RemoveTenantFromCacheAsync(Tenant.Alias, _securityContext.CurrentAccount.ID);
        }

        var owner = _userManager.GetUsers(Tenant.OwnerId);
        var redirectLink = _setupInfo.TeamlabSiteRedirect + "/remove-portal-feedback-form.aspx#";
        var parameters = Convert.ToBase64String(Encoding.UTF8.GetBytes("{\"firstname\":\"" + owner.FirstName +
                                                                                "\",\"lastname\":\"" + owner.LastName +
                                                                                "\",\"alias\":\"" + Tenant.Alias +
                                                                                "\",\"email\":\"" + owner.Email + "\"}"));

        redirectLink += HttpUtility.UrlEncode(parameters);

        var authed = false;
        try
        {
            if (!_securityContext.IsAuthenticated)
            {
                _securityContext.AuthenticateMe(ASC.Core.Configuration.Constants.CoreSystem);
                authed = true;
            }

            _messageService.Send(MessageAction.PortalDeleted);

        }
        finally
        {
            if (authed) _securityContext.Logout();
        }

        _studioNotifyService.SendMsgPortalDeletionSuccess(owner, redirectLink);

        return redirectLink;
    }
}