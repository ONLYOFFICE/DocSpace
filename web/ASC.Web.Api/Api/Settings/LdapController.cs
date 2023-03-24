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

namespace ASC.Web.Api.Controllers.Settings;

public class LdapController : BaseSettingsController
{
    private Tenant Tenant { get { return ApiContext.Tenant; } }

    private readonly SettingsManager _settingsManager;
    private readonly TenantManager _tenantManager;
    private readonly LdapNotifyService _ldapNotifyHelper;
    private readonly LdapSaveSyncOperation _ldapSaveSyncOperation;
    private readonly AuthContext _authContext;
    private readonly PermissionContext _permissionContext;
    private readonly CoreBaseSettings _coreBaseSettings;
    private readonly IMapper _mapper;

    public LdapController(
        ApiContext apiContext,
        WebItemManager webItemManager,
        IMemoryCache memoryCache,
        SettingsManager settingsManager,
        TenantManager tenantManager,
        LdapNotifyService ldapNotifyHelper,
        LdapSaveSyncOperation ldapSaveSyncOperation,
        AuthContext authContext,
        PermissionContext permissionContext,
        CoreBaseSettings coreBaseSettings,
        IHttpContextAccessor httpContextAccessor,
        IMapper mapper) : base(apiContext, memoryCache, webItemManager, httpContextAccessor)
    {
        _settingsManager = settingsManager;
        _tenantManager = tenantManager;
        _ldapNotifyHelper = ldapNotifyHelper;
        _ldapSaveSyncOperation = ldapSaveSyncOperation;
        _authContext = authContext;
        _permissionContext = permissionContext;
        _coreBaseSettings = coreBaseSettings;
        _mapper = mapper;
    }

    /// <summary>
    /// Returns the current portal LDAP settings.
    /// </summary>
    /// <short>
    /// Get the LDAP settings
    /// </short>
    /// <category>LDAP</category>
    /// <returns>LDAP settings</returns>
    [HttpGet("ldap")]
    public async Task<LdapSettingsDto> GetLdapSettingsAsync()
    {
        await CheckLdapPermissionsAsync();

        var settings = await _settingsManager.LoadAsync<LdapSettings>();

        settings = settings.Clone() as LdapSettings; // clone LdapSettings object for clear password (potencial AscCache.Memory issue)

        if (settings == null)
        {
            settings = new LdapSettings().GetDefault();
            return _mapper.Map<LdapSettings, LdapSettingsDto>(settings);
        }

        settings.Password = null;
        settings.PasswordBytes = null;

        if (settings.IsDefault)
            return _mapper.Map<LdapSettings, LdapSettingsDto>(settings);

        var defaultSettings = settings.GetDefault();

        if (settings.Equals(defaultSettings))
            settings.IsDefault = true;

        return _mapper.Map<LdapSettings, LdapSettingsDto>(settings);
    }

    /// <summary>
    /// Returns the LDAP autosynchronous cron expression of the current portal if it exists.
    /// </summary>
    /// <short>
    /// Get the LDAP cron expression
    /// </short>
    /// <category>LDAP</category>
    /// <returns>Cron expression or null</returns>
    [HttpGet("ldap/cron")]
    public async Task<LdapCronSettingsDto> GetLdapCronSettingsAsync()
    {
        await CheckLdapPermissionsAsync();

        var settings = await _settingsManager.LoadAsync<LdapCronSettings>();

        if (settings == null)
            settings = new LdapCronSettings().GetDefault();

        if (string.IsNullOrEmpty(settings.Cron))
            return null;

        return _mapper.Map<LdapCronSettings, LdapCronSettingsDto>(settings);
    }

    /// <summary>
    /// Sets the LDAP autosynchronous cron expression of the current portal.
    /// </summary>
    /// <short>
    /// Set the LDAP cron expression
    /// </short>
    /// <category>LDAP</category>
    /// <param name="cron">Cron expression</param>
    /// 
    [HttpPost("ldap/cron")]
    public async Task SetLdapCronSettingsAsync(LdapCronRequestDto ldapCronRequest)
    {
        await CheckLdapPermissionsAsync();

        var cron = ldapCronRequest.Cron;

        if (!string.IsNullOrEmpty(cron))
        {
            new CronExpression(cron); // validate

            if (!(await _settingsManager.LoadAsync<LdapSettings>()).EnableLdapAuthentication)
            {
                throw new Exception(Resource.LdapSettingsErrorCantSaveLdapSettings);
            }
        }

        var settings = await _settingsManager.LoadAsync<LdapCronSettings>();

        if (settings == null)
            settings = new LdapCronSettings();

        settings.Cron = cron;
        await _settingsManager.SaveAsync(settings);

        var t = await _tenantManager.GetCurrentTenantAsync();
        if (!string.IsNullOrEmpty(cron))
        {
            _ldapNotifyHelper.UnregisterAutoSync(t);
            _ldapNotifyHelper.RegisterAutoSync(t, cron);
        }
        else
        {
            _ldapNotifyHelper.UnregisterAutoSync(t);
        }
    }

    /// <summary>
    /// Starts synchronizing users and groups by LDAP.
    /// </summary>
    /// <short>
    /// Synchronize by LDAP
    /// </short>
    /// <category>LDAP</category>
    /// <returns>Operation status</returns>
    [HttpGet("ldap/sync")]
    public async Task<LdapStatusDto> SyncLdapAsync()
    {
        await CheckLdapPermissionsAsync();

        var ldapSettings = await _settingsManager.LoadAsync<LdapSettings>();

        var userId = _authContext.CurrentAccount.ID.ToString();

        var result = await _ldapSaveSyncOperation.SyncLdapAsync(ldapSettings, Tenant, userId);

        return _mapper.Map<LdapOperationStatus, LdapStatusDto>(result);
    }

    /// <summary>
    /// Starts the process of collecting preliminary changes on the portal during the synchronization process according to the selected LDAP settings.
    /// </summary>
    /// <short>
    /// Test the LDAP synchronization
    /// </short>
    /// <category>LDAP</category>
    /// <returns>Operation status</returns>
    [HttpGet("ldap/sync/test")]
    public async Task<LdapStatusDto> TestLdapSync()
    {
        await CheckLdapPermissionsAsync();

        var ldapSettings = await _settingsManager.LoadAsync<LdapSettings>();

        var result = await _ldapSaveSyncOperation.TestLdapSyncAsync(ldapSettings, Tenant);

        return _mapper.Map<LdapOperationStatus, LdapStatusDto>(result);
    }

    /// <summary>
    /// Saves the LDAP settings specified in the request and starts importing/synchronizing users and groups by LDAP.
    /// </summary>
    /// <short>
    /// Save the LDAP settings
    /// </short>
    /// <category>LDAP</category>
    /// <param name="settings">LDAP settings in the serialized string format</param>
    /// <param name="acceptCertificate">Specifies if the errors of checking certificates are allowed (true) or not (false)</param>
    /// <returns>Operation status</returns>
    [HttpPost("ldap")]
    public async Task<LdapStatusDto> SaveLdapSettingsAsync(LdapRequestsDto ldapRequestsDto)
    {
        var ldapSettings = _mapper.Map<LdapRequestsDto, LdapSettings>(ldapRequestsDto);

        await CheckLdapPermissionsAsync();

        if (!ldapSettings.EnableLdapAuthentication)
        {
            await SetLdapCronSettingsAsync(null);
        }

        var userId = _authContext.CurrentAccount.ID.ToString();

        var result = await _ldapSaveSyncOperation.SaveLdapSettingsAsync(ldapSettings, Tenant, userId);

        return _mapper.Map<LdapOperationStatus, LdapStatusDto>(result);
    }

    /// <summary>
    /// Starts the process of collecting preliminary changes on the portal during the saving process according to the LDAP settings.
    /// </summary>
    /// <short>
    /// Test the LDAP saving process
    /// </short>
    /// <category>LDAP</category>
    /// <param name="settings">LDAP settings in the serialized string format</param>
    /// <param name="acceptCertificate">Specifies if the errors of checking certificates are allowed (true) or not (false)</param>
    /// <returns>Operation status</returns>
    [HttpPost("ldap/save/test")]
    public async Task<LdapStatusDto> TestLdapSaveAsync(LdapSettings ldapSettings)
    {
        await CheckLdapPermissionsAsync();

        var userId = _authContext.CurrentAccount.ID.ToString();

        var result = await _ldapSaveSyncOperation.TestLdapSaveAsync(ldapSettings, Tenant, userId);

        return _mapper.Map<LdapOperationStatus, LdapStatusDto>(result);
    }

    /// <summary>
    /// Returns the LDAP synchronization process status.
    /// </summary>
    /// <short>
    /// Get the LDAP synchronization process status
    /// </short>
    /// <category>LDAP</category>
    /// <returns>Operation status</returns>
    [HttpGet("ldap/status")]
    public async Task<LdapStatusDto> GetLdapOperationStatusAsync()
    {
        await CheckLdapPermissionsAsync();

        var result = _ldapSaveSyncOperation.ToLdapOperationStatus(Tenant.Id);

        return _mapper.Map<LdapOperationStatus, LdapStatusDto>(result);
    }

    /// <summary>
    /// Returns the LDAP default settings.
    /// </summary>
    /// <short>
    /// Get the LDAP default settings
    /// </short>
    /// <category>LDAP</category>
    /// <returns>LDAP default settings</returns>
    [HttpGet("ldap/default")]
    public async Task<LdapSettingsDto> GetDefaultLdapSettingsAsync()
    {
        await CheckLdapPermissionsAsync();

        var settings = new LdapSettings().GetDefault();

        return _mapper.Map<LdapSettings, LdapSettingsDto>(settings);
    }

    private async Task CheckLdapPermissionsAsync()
    {
        await _permissionContext.DemandPermissionsAsync(SecutiryConstants.EditPortalSettings);

        if (!_coreBaseSettings.Standalone
            && (!SetupInfo.IsVisibleSettings(ManagementType.LdapSettings.ToString())
                || !(await _tenantManager.GetCurrentTenantQuotaAsync()).Ldap))
        {
            throw new BillingException(Resource.ErrorNotAllowedOption, "Ldap");
        }
    }
}
