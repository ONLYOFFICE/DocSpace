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

///<summary>
/// Portal information access.
///</summary>
///<name>portal</name>
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
    private readonly IUrlShortener _urlShortener;
    private readonly AuthContext _authContext;
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
    private readonly EmailValidationKeyProvider _emailValidationKeyProvider;
    private readonly StudioSmsNotificationSettingsHelper _studioSmsNotificationSettingsHelper;
    private readonly TfaAppAuthSettingsHelper _tfaAppAuthSettingsHelper;
    private readonly IMapper _mapper;
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly QuotaHelper _quotaHelper;
    private readonly CspSettingsHelper _cspSettingsHelper;
    private readonly IEventBus _eventBus;

    public PortalController(
        ILogger<PortalController> logger,
        ApiContext apiContext,
        UserManager userManager,
        TenantManager tenantManager,
        ITariffService tariffService,
        CommonLinkUtility commonLinkUtility,
        IUrlShortener urlShortener,
        AuthContext authContext,
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
        DisplayUserSettingsHelper displayUserSettingsHelper,
        EmailValidationKeyProvider emailValidationKeyProvider,
        StudioSmsNotificationSettingsHelper studioSmsNotificationSettingsHelper,
        TfaAppAuthSettingsHelper tfaAppAuthSettingsHelper,
        IMapper mapper,
        IHttpContextAccessor httpContextAccessor,
        QuotaHelper quotaHelper,
        IEventBus eventBus,
        CspSettingsHelper cspSettingsHelper)
    {
        _log = logger;
        _apiContext = apiContext;
        _userManager = userManager;
        _tenantManager = tenantManager;
        _tariffService = tariffService;
        _commonLinkUtility = commonLinkUtility;
        _urlShortener = urlShortener;
        _authContext = authContext;
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
        _emailValidationKeyProvider = emailValidationKeyProvider;
        _studioSmsNotificationSettingsHelper = studioSmsNotificationSettingsHelper;
        _tfaAppAuthSettingsHelper = tfaAppAuthSettingsHelper;
        _mapper = mapper;
        _httpContextAccessor = httpContextAccessor;
        _quotaHelper = quotaHelper;
        _cspSettingsHelper = cspSettingsHelper;
        _eventBus = eventBus;
    }

    /// <summary>
    /// Returns the current portal.
    /// </summary>
    /// <short>
    /// Get a portal
    /// </short>
    /// <category>Settings</category>
    /// <returns type="ASC.Web.Api.ApiModels.ResponseDto.TenantDto, ASC.Web.Api">Current portal information</returns>
    /// <path>api/2.0/portal</path>
    /// <httpMethod>GET</httpMethod>
    [AllowNotPayment]
    [HttpGet("")]
    public TenantDto Get()
    {
        return _mapper.Map<TenantDto>(Tenant);
    }

    /// <summary>
    /// Returns a user with the ID specified in the request from the current portal.
    /// </summary>
    /// <short>
    /// Get a user by ID
    /// </short>
    /// <category>Users</category>
    /// <param type="System.Guid, System" method="url" name="userID">User ID</param>
    /// <returns type="ASC.Core.Users.UserInfo, ASC.Core.Common">User information</returns>
    /// <path>api/2.0/portal/users/{userID}</path>
    /// <httpMethod>GET</httpMethod>
    [HttpGet("users/{userID}")]
    public async Task<UserInfo> GetUserAsync(Guid userID)
    {
        return await _userManager.GetUsersAsync(userID);
    }

    /// <summary>
    /// Returns an invitation link for joining the portal.
    /// </summary>
    /// <short>
    /// Get an invitation link
    /// </short>
    /// <param type="ASC.Core.Users.EmployeeType, ASC.Core.Common" method="url" name="employeeType">Employee type (All, RoomAdmin, User, DocSpaceAdmin)</param>
    /// <category>Users</category>
    /// <returns type="System.Object, System">Invitation link</returns>
    /// <path>api/2.0/portal/users/invite/{employeeType}</path>
    /// <httpMethod>GET</httpMethod>
    [HttpGet("users/invite/{employeeType}")]
    public async Task<object> GeInviteLinkAsync(EmployeeType employeeType)
    {
        var currentUser = await _userManager.GetUsersAsync(_authContext.CurrentAccount.ID);

        if ((employeeType == EmployeeType.DocSpaceAdmin && !currentUser.IsOwner(await _tenantManager.GetCurrentTenantAsync()))
            || !await _permissionContext.CheckPermissionsAsync(new UserSecurityProvider(Guid.Empty, employeeType), ASC.Core.Users.Constants.Action_AddRemoveUser))
        {
            return string.Empty;
        }

        var link = await _commonLinkUtility.GetConfirmationEmailUrlAsync(string.Empty, ConfirmType.LinkInvite, (int)employeeType, _authContext.CurrentAccount.ID)
                + $"&emplType={employeeType:d}";

        return await _urlShortener.GetShortenLinkAsync(link);
    }

    /// <summary>
    /// Returns a link specified in the request in the shortened format.
    /// </summary>
    /// <short>Get a shortened link</short>
    /// <category>Settings</category>
    /// <param type="ASC.Web.Api.ApiModel.RequestsDto.ShortenLinkRequestsDto, ASC.Web.Api" name="inDto">Shortened link request parameters</param>
    /// <returns type="System.Object, System">Shortened link</returns>
    /// <path>api/2.0/portal/getshortenlink</path>
    /// <httpMethod>PUT</httpMethod>
    [HttpPut("getshortenlink")]
    public async Task<object> GetShortenLinkAsync(ShortenLinkRequestsDto inDto)
    {
        try
        {
            return await _urlShortener.GetShortenLinkAsync(inDto.Link);
        }
        catch (Exception ex)
        {
            _log.ErrorGetShortenLink(ex);
            return inDto.Link;
        }
    }

    /// <summary>
    /// Returns an extra tenant license for the portal.
    /// </summary>
    /// <short>
    /// Get an extra tenant license
    /// </short>
    /// <category>Quota</category>
    /// <param type="System.Boolean, System" name="refresh">Specifies whether the tariff will be refreshed</param>
    /// <returns type="ASC.Web.Api.ApiModels.ResponseDto, ASC.Web.Api">Extra tenant license information</returns>
    /// <path>api/2.0/portal/tenantextra</path>
    /// <httpMethod>GET</httpMethod>
    [AllowNotPayment, AllowAnonymous]
    [HttpGet("tenantextra")]
    public async Task<TenantExtraDto> GetTenantExtra(bool refresh)
    {
        var result = new TenantExtraDto
        {
            CustomMode = _coreBaseSettings.CustomMode,
            Opensource = _tenantExtra.Opensource,
            Enterprise = _tenantExtra.Enterprise,
            EnableTariffPage = //TenantExtra.EnableTarrifSettings - think about hide-settings for opensource
                (!_coreBaseSettings.Standalone || !string.IsNullOrEmpty(_licenseReader.LicensePath))
                && string.IsNullOrEmpty(_setupInfo.AmiMetaUrl)
                && !_coreBaseSettings.CustomMode
        };



        if (_authContext.IsAuthenticated)
        {
            result.Tariff = await _tenantExtra.GetCurrentTariffAsync(refresh);
            result.Quota = await _quotaHelper.GetCurrentQuotaAsync(refresh);
            result.NotPaid = await _tenantExtra.IsNotPaidAsync();
            result.LicenseAccept = _settingsManager.LoadForDefaultTenant<TariffSettings>().LicenseAcceptSetting;
            result.DocServerUserQuota = await _documentServiceLicense.GetLicenseQuotaAsync();
            result.DocServerLicense = await _documentServiceLicense.GetLicenseAsync();
        }

        return result;
    }


    /// <summary>
    /// Returns the used space of the current portal.
    /// </summary>
    /// <short>
    /// Get the used portal space
    /// </short>
    /// <category>Quota</category>
    /// <returns type="System.Double, System">Used portal space</returns>
    /// <path>api/2.0/portal/usedspace</path>
    /// <httpMethod>GET</httpMethod>
    [HttpGet("usedspace")]
    public async Task<double> GetUsedSpaceAsync()
    {
        return Math.Round(
            (await _tenantManager.FindTenantQuotaRowsAsync(Tenant.Id))
                        .Where(q => !string.IsNullOrEmpty(q.Tag) && new Guid(q.Tag) != Guid.Empty)
                        .Sum(q => q.Counter) / 1024f / 1024f / 1024f, 2);
    }


    /// <summary>
    /// Returns a number of portal users.
    /// </summary>
    /// <short>
    /// Get a number of portal users
    /// </short>
    /// <category>Users</category>
    /// <returns type="System.Int64, System">Number of portal users</returns>
    /// <path>api/2.0/portal/userscount</path>
    /// <httpMethod>GET</httpMethod>
    [HttpGet("userscount")]
    public async Task<long> GetUsersCountAsync()
    {
        return _coreBaseSettings.Personal ? 1 : (await _userManager.GetUserNamesAsync(EmployeeStatus.Active)).Length;
    }

    /// <summary>
    /// Returns the current portal tariff.
    /// </summary>
    /// <short>
    /// Get a portal tariff
    /// </short>
    /// <category>Quota</category>
    /// <param type="System.Boolean, System" name="refresh">Specifies whether the tariff will be refreshed</param>
    /// <returns type="ASC.Core.Billing.Tariff, ASC.Core.Common">Current portal tariff</returns>
    /// <path>api/2.0/portal/tariff</path>
    /// <httpMethod>GET</httpMethod>
    [AllowNotPayment]
    [HttpGet("tariff")]
    public async Task<Tariff> GetTariffAsync(bool refresh)
    {
        return await _tariffService.GetTariffAsync(Tenant.Id, refresh: refresh);
    }

    /// <summary>
    /// Returns the current portal quota.
    /// </summary>
    /// <short>
    /// Get a portal quota
    /// </short>
    /// <category>Quota</category>
    /// <returns type="ASC.Core.Tenants.TenantQuota, ASC.Core.Common">Current portal quota</returns>
    /// <path>api/2.0/portal/quota</path>
    /// <httpMethod>GET</httpMethod>
    [AllowNotPayment]
    [HttpGet("quota")]
    public async Task<TenantQuota> GetQuotaAsync()
    {
        return await _tenantManager.GetTenantQuotaAsync(Tenant.Id);
    }

    /// <summary>
    /// Returns the recommended quota for the current portal.
    /// </summary>
    /// <short>
    /// Get the recommended quota
    /// </short>
    /// <category>Quota</category>
    /// <returns type="ASC.Core.Tenants.TenantQuota, ASC.Core.Common">Recommended portal quota</returns>
    /// <path>api/2.0/portal/quota/right</path>
    /// <httpMethod>GET</httpMethod>
    [HttpGet("quota/right")]
    public async Task<TenantQuota> GetRightQuotaAsync()
    {
        var usedSpace = await GetUsedSpaceAsync();
        var needUsersCount = await GetUsersCountAsync();

        return (await _tenantManager.GetTenantQuotasAsync()).OrderBy(r => r.Price)
                            .FirstOrDefault(quota =>
                                            quota.CountUser > needUsersCount
                                            && quota.MaxTotalSize > usedSpace);
    }


    /// <summary>
    /// Returns the full absolute path to the current portal.
    /// </summary>
    /// <short>
    /// Get a path to the portal
    /// </short>
    /// <category>Settings</category>
    /// <param type="System.String, System" name="virtualPath">Portal virtual path</param>
    /// <returns type="System.Object, System">Portal path</returns>
    /// <path>api/2.0/portal/path</path>
    /// <httpMethod>GET</httpMethod>
    [HttpGet("path")]
    public object GetFullAbsolutePath(string virtualPath)
    {
        return _commonLinkUtility.GetFullAbsolutePath(virtualPath);
    }

    /// <summary>
    /// Returns a thumbnail of the bookmark URL specified in the request.
    /// </summary>
    /// <short>
    /// Get a bookmark thumbnail
    /// </short>
    /// <category>Settings</category>
    /// <param type="System.String, System" name="url">Bookmark URL</param>
    /// <returns type="Microsoft.AspNetCore.Mvc.FileResult, Microsoft.AspNetCore.Mvc">Thumbnail</returns>
    /// <path>api/2.0/portal/thumb</path>
    /// <httpMethod>GET</httpMethod>
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

    /// <summary>
    /// Marks a gift message as read.
    /// </summary>
    /// <short>
    /// Mark a gift message as read
    /// </short>
    /// <category>Users</category>
    /// <returns></returns>
    /// <path>api/2.0/portal/present/mark</path>
    /// <httpMethod>POST</httpMethod>
    [HttpPost("present/mark")]
    public async Task MarkPresentAsReadedAsync()
    {
        try
        {
            var settings = await _settingsManager.LoadForCurrentUserAsync<OpensourceGiftSettings>();
            settings.Readed = true;
            await _settingsManager.SaveForCurrentUserAsync(settings);
        }
        catch (Exception ex)
        {
            _log.ErrorMarkPresentAsReaded(ex);
        }
    }

    /// <summary>
    /// Registers the mobile app installation.
    /// </summary>
    /// <short>
    /// Register the mobile app installation
    /// </short>
    /// <category>Settings</category>
    /// <param type="ASC.Web.Api.ApiModel.RequestsDto.MobileAppRequestsDto, ASC.Web.Api" name="inDto">Mobile app request parameters</param>
    /// <returns></returns>
    /// <path>api/2.0/portal/mobile/registration</path>
    /// <httpMethod>POST</httpMethod>
    [HttpPost("mobile/registration")]
    public async Task RegisterMobileAppInstallAsync(MobileAppRequestsDto inDto)
    {
        var currentUser = await _userManager.GetUsersAsync(_securityContext.CurrentAccount.ID);
        await _mobileAppInstallRegistrator.RegisterInstallAsync(currentUser.Email, inDto.Type);
    }

    /// <summary>
    /// Registers the mobile app installation by mobile app type.
    /// </summary>
    /// <short>
    /// Register the mobile app installation by mobile app type
    /// </short>
    /// <category>Settings</category>
    /// <param type="ASC.Core.Common.Notify.Push.MobileAppType, ASC.Core.Common" name="type">Mobile app type (IosProjects, AndroidProjects, IosDocuments, AndroidDocuments, or DesktopEditor)</param>
    /// <returns></returns>
    /// <path>api/2.0/portal/mobile/registration</path>
    /// <httpMethod>POST</httpMethod>
    /// <visible>false</visible>
    [HttpPost("mobile/registration")]
    public async Task RegisterMobileAppInstallAsync(MobileAppType type)
    {
        var currentUser = await _userManager.GetUsersAsync(_securityContext.CurrentAccount.ID);
        await _mobileAppInstallRegistrator.RegisterInstallAsync(currentUser.Email, type);
    }

    /// <summary>
    /// Updates a portal name with a new one specified in the request.
    /// </summary>
    /// <short>Update a portal name</short>
    /// <category>Settings</category>
    /// <param type="ASC.Web.Api.ApiModels.RequestsDto.PortalRenameRequestsDto, ASC.Web.Api" name="inDto">Request parameters for portal renaming</param>
    /// <returns type="System.Object, System">Confirmation email about authentication to the portal with a new name</returns>
    /// <path>api/2.0/portal/portalrename</path>
    /// <httpMethod>PUT</httpMethod>
    /// <visible>false</visible>
    [HttpPut("portalrename")]
    public async Task<object> UpdatePortalName(PortalRenameRequestsDto inDto)
    {
        if (!SetupInfo.IsVisibleSettings(nameof(ManagementType.PortalSecurity)))
        {
            throw new BillingException(Resource.ErrorNotAllowedOption, "PortalRename");
        }

        if (_coreBaseSettings.Personal)
        {
            throw new Exception(Resource.ErrorAccessDenied);
        }

        await _permissionContext.DemandPermissionsAsync(SecutiryConstants.EditPortalSettings);

        var alias = inDto.Alias;
        if (string.IsNullOrEmpty(alias))
        {
            throw new ArgumentException(nameof(alias));
        }

        var tenant = await _tenantManager.GetCurrentTenantAsync();
        var user = await _userManager.GetUsersAsync(_securityContext.CurrentAccount.ID);

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
                await _tenantManager.CheckTenantAddressAsync(newAlias.Trim());
            }


            if (!string.IsNullOrEmpty(_apiSystemHelper.ApiCacheUrl))
            {
                await _apiSystemHelper.AddTenantToCacheAsync(newAlias, user.Id);
            }

            var oldDomain = tenant.GetTenantDomain(_coreSettings);
            tenant.Alias = alias;
            tenant = await _tenantManager.SaveTenantAsync(tenant);
            _tenantManager.SetCurrentTenant(tenant);

            await _cspSettingsHelper.RenameDomain(oldDomain, tenant.GetTenantDomain(_coreSettings));

            if (!string.IsNullOrEmpty(_apiSystemHelper.ApiCacheUrl))
            {
                await _apiSystemHelper.RemoveTenantFromCacheAsync(oldAlias, user.Id);
            }

            if (!localhost || string.IsNullOrEmpty(tenant.MappedDomain))
            {
                await _studioNotifyService.PortalRenameNotifyAsync(tenant, oldVirtualRootPath, oldAlias);
            }
        }
        else
        {
            return string.Empty;
        }

        var rewriter = _httpContextAccessor.HttpContext.Request.Url();
        return string.Format("{0}{1}{2}{3}/{4}",
                                rewriter?.Scheme ?? Uri.UriSchemeHttp,
                                Uri.SchemeDelimiter,
                                tenant.GetTenantDomain(_coreSettings),
                                rewriter != null && !rewriter.IsDefaultPort ? $":{rewriter.Port}" : "",
                                _commonLinkUtility.GetConfirmationUrlRelative(tenant.Id, user.Email, ConfirmType.Auth)
               );
    }

    /// <summary>
    /// Deletes the current portal immediately.
    /// </summary>
    /// <short>Delete a portal immediately</short>
    /// <category>Settings</category>
    /// <returns></returns>
    /// <path>api/2.0/portal/deleteportalimmediately</path>
    /// <httpMethod>DELETE</httpMethod>
    [HttpDelete("deleteportalimmediately")]
    public async Task DeletePortalImmediatelyAsync()
    {
        var tenant = await _tenantManager.GetCurrentTenantAsync();

        if (_securityContext.CurrentAccount.ID != tenant.OwnerId)
        {
            throw new Exception(Resource.ErrorAccessDenied);
        }

        await _tenantManager.RemoveTenantAsync(tenant.Id);

        if (!string.IsNullOrEmpty(_apiSystemHelper.ApiCacheUrl))
        {
            await _apiSystemHelper.RemoveTenantFromCacheAsync(tenant.Alias, _securityContext.CurrentAccount.ID);
        }

        try
        {
            if (!_securityContext.IsAuthenticated)
            {
                await _securityContext.AuthenticateMeWithoutCookieAsync(ASC.Core.Configuration.Constants.CoreSystem);
            }
            await _messageService.SendAsync(MessageAction.PortalDeleted);
        }
        finally
        {
            _securityContext.Logout();
        }
    }

    /// <summary>
    /// Sends the instructions to suspend the current portal.
    /// </summary>
    /// <short>Send suspension instructions</short>
    /// <category>Settings</category>
    /// <returns></returns>
    /// <path>api/2.0/portal/suspend</path>
    /// <httpMethod>POST</httpMethod>
    [AllowNotPayment]
    [HttpPost("suspend")]
    public async Task SendSuspendInstructionsAsync()
    {
        await _permissionContext.DemandPermissionsAsync(SecutiryConstants.EditPortalSettings);

        if (_securityContext.CurrentAccount.ID != Tenant.OwnerId)
        {
            throw new Exception(Resource.ErrorAccessDenied);
        }

        var owner = await _userManager.GetUsersAsync(Tenant.OwnerId);
        var suspendUrl = await _commonLinkUtility.GetConfirmationEmailUrlAsync(owner.Email, ConfirmType.PortalSuspend);
        var continueUrl = await _commonLinkUtility.GetConfirmationEmailUrlAsync(owner.Email, ConfirmType.PortalContinue);

        await _studioNotifyService.SendMsgPortalDeactivationAsync(Tenant, suspendUrl, continueUrl);

        await _messageService.SendAsync(MessageAction.OwnerSentPortalDeactivationInstructions, _messageTarget.Create(owner.Id), owner.DisplayUserName(false, _displayUserSettingsHelper));
    }

    /// <summary>
    /// Sends the instructions to remove the current portal.
    /// </summary>
    /// <short>Send removal instructions</short>
    /// <category>Settings</category>
    /// <returns></returns>
    /// <path>api/2.0/portal/delete</path>
    /// <httpMethod>POST</httpMethod>
    [AllowNotPayment]
    [HttpPost("delete")]
    public async Task SendDeleteInstructionsAsync()
    {
        await _permissionContext.DemandPermissionsAsync(SecutiryConstants.EditPortalSettings);

        if (_securityContext.CurrentAccount.ID != Tenant.OwnerId)
        {
            throw new Exception(Resource.ErrorAccessDenied);
        }

        var owner = await _userManager.GetUsersAsync(Tenant.OwnerId);

        var showAutoRenewText = !_coreBaseSettings.Standalone &&
                        (await _tariffService.GetPaymentsAsync(Tenant.Id)).Any() &&
                        !(await _tenantManager.GetCurrentTenantQuotaAsync()).Trial;

        await _studioNotifyService.SendMsgPortalDeletionAsync(Tenant, await _commonLinkUtility.GetConfirmationEmailUrlAsync(owner.Email, ConfirmType.PortalRemove), showAutoRenewText);

        await _messageService.SendAsync(MessageAction.OwnerSentPortalDeleteInstructions, _messageTarget.Create(owner.Id), owner.DisplayUserName(false, _displayUserSettingsHelper));
    }

    /// <summary>
    /// Restores the current portal.
    /// </summary>
    /// <short>Restore a portal</short>
    /// <category>Settings</category>
    /// <returns></returns>
    /// <path>api/2.0/portal/continue</path>
    /// <httpMethod>PUT</httpMethod>
    [AllowSuspended]
    [HttpPut("continue")]
    [Authorize(AuthenticationSchemes = "confirm", Roles = "PortalContinue")]
    public async Task ContinuePortalAsync()
    {
        Tenant.SetStatus(TenantStatus.Active);
        await _tenantManager.SaveTenantAsync(Tenant);
    }

    /// <summary>
    /// Deactivates the current portal.
    /// </summary>
    /// <short>Deactivate a portal</short>
    /// <category>Settings</category>
    /// <returns></returns>
    /// <path>api/2.0/portal/suspend</path>
    /// <httpMethod>PUT</httpMethod>
    [HttpPut("suspend")]
    [Authorize(AuthenticationSchemes = "confirm", Roles = "PortalSuspend")]
    public async Task SuspendPortalAsync()
    {
        Tenant.SetStatus(TenantStatus.Suspended);
        await _tenantManager.SaveTenantAsync(Tenant);
        await _messageService.SendAsync(MessageAction.PortalDeactivated);
    }

    /// <summary>
    /// Deletes the current portal.
    /// </summary>
    /// <short>Delete a portal</short>
    /// <category>Settings</category>
    /// <returns type="System.Object, System">URL to the feedback form about removing a portal</returns>
    /// <path>api/2.0/portal/delete</path>
    /// <httpMethod>DELETE</httpMethod>
    [AllowNotPayment]
    [HttpDelete("delete")]
    [Authorize(AuthenticationSchemes = "confirm", Roles = "PortalRemove")]
    public async Task<object> DeletePortalAsync()
    {
        if (_securityContext.CurrentAccount.ID != Tenant.OwnerId)
        {
            throw new Exception(Resource.ErrorAccessDenied);
        }

        await _tenantManager.RemoveTenantAsync(Tenant.Id);

        if (!string.IsNullOrEmpty(_apiSystemHelper.ApiCacheUrl))
        {
            await _apiSystemHelper.RemoveTenantFromCacheAsync(Tenant.Alias, _securityContext.CurrentAccount.ID);
        }

        var owner = await _userManager.GetUsersAsync(Tenant.OwnerId);
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
                await _securityContext.AuthenticateMeAsync(ASC.Core.Configuration.Constants.CoreSystem);
                authed = true;
            }

            await _messageService.SendAsync(MessageAction.PortalDeleted);

        }
        finally
        {
            if (authed)
            {
                _securityContext.Logout();
            }
        }

        _eventBus.Publish(new RemovePortalIntegrationEvent(_securityContext.CurrentAccount.ID, Tenant.Id));

        await _studioNotifyService.SendMsgPortalDeletionSuccessAsync(owner, redirectLink);

        return redirectLink;
    }

    /// <summary>
    /// Sends congratulations to the user after registering the portal.
    /// </summary>
    /// <short>Send congratulations</short>
    /// <category>Users</category>
    /// <param type="ASC.Web.Api.ApiModels.RequestsDto.SendCongratulationsDto, ASC.Web.Api" name="inDto">Congratulations request parameters</param>
    /// <returns></returns>
    /// <path>api/2.0/portal/sendcongratulations</path>
    /// <httpMethod>POST</httpMethod>
    [AllowAnonymous]
    [HttpPost("sendcongratulations")]
    public async Task SendCongratulationsAsync([FromQuery] SendCongratulationsDto inDto)
    {
        var authInterval = TimeSpan.FromHours(1);
        var checkKeyResult = await _emailValidationKeyProvider.ValidateEmailKeyAsync(inDto.Userid.ToString() + ConfirmType.Auth, inDto.Key, authInterval);

        switch (checkKeyResult)
        {
            case ValidationResult.Ok:
                var currentUser = await _userManager.GetUsersAsync(inDto.Userid);

                await _studioNotifyService.SendCongratulationsAsync(currentUser);
                await _studioNotifyService.SendRegDataAsync(currentUser);

                if (!SetupInfo.IsSecretEmail(currentUser.Email))
                {
                    if (_setupInfo.TfaRegistration == "sms")
                    {
                        _studioSmsNotificationSettingsHelper.Enable = true;
                    }
                    else if (_setupInfo.TfaRegistration == "code")
                    {
                        _tfaAppAuthSettingsHelper.Enable = true;
                    }
                }
                break;
            default:
                throw new SecurityException("Access Denied.");
        }
    }
}