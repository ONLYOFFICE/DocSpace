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
    /// <returns>LDAP settings: enable LDAP authentication or not, start TLS or not, enable SSL or not, send welcome email or not, server name, user name, port number, user filter, login attribute, LDAP settings mapping, access rights, user is a group member or not, group name, user attribute, group filter, group attribute, group name attribute, authentication is enabled or not, login, password, accept certificate or not</returns>
    /// <path>api/2.0/settings/ldap</path>
    /// <httpMethod>GET</httpMethod>
    [HttpGet("ldap")]
    public LdapSettingsDto GetLdapSettings()
    {
        CheckLdapPermissions();

        var settings = _settingsManager.Load<LdapSettings>();

        settings = settings.Clone() as LdapSettings; // clone LdapSettings object for clear password (potencial AscCache.Memory issue)

        if (settings == null)
        {
            settings = new LdapSettings().GetDefault();
            return _mapper.Map<LdapSettings, LdapSettingsDto>(settings);
        }

        settings.Password = null;
        settings.PasswordBytes = null;

        if (settings.IsDefault)
            return _mapper.Map<LdapSettings, LdapSettingsDto>(settings); ;

        var defaultSettings = settings.GetDefault();

        if (settings.Equals(defaultSettings))
            settings.IsDefault = true;

        return _mapper.Map<LdapSettings, LdapSettingsDto>(settings);
    }

    /// <summary>
    /// Returns the LDAP autosynchronous cron expression for the current portal if it exists.
    /// </summary>
    /// <short>
    /// Get the LDAP cron expression
    /// </short>
    /// <category>LDAP</category>
    /// <returns>LDAP cron settings: cron expression</returns>
    /// <path>api/2.0/settings/ldap/cron</path>
    /// <httpMethod>GET</httpMethod>
    [HttpGet("ldap/cron")]
    public LdapCronSettingsDto GetLdapCronSettings()
    {
        CheckLdapPermissions();

        var settings = _settingsManager.Load<LdapCronSettings>();

        if (settings == null)
            settings = new LdapCronSettings().GetDefault();

        if (string.IsNullOrEmpty(settings.Cron))
            return null;

        return _mapper.Map<LdapCronSettings, LdapCronSettingsDto>(settings);
    }

    /// <summary>
    /// Sets the LDAP autosynchronous cron expression to the current portal.
    /// </summary>
    /// <short>
    /// Set the LDAP cron expression
    /// </short>
    /// <category>LDAP</category>
    /// <path>api/2.0/settings/ldap/cron</path>
    /// <param type="ASC.Web.Api.ApiModels.RequestsDto.LdapCronRequestDto, ASC.Web.Api.ApiModels.RequestsDto" name="ldapCronRequest">LDAP cron request parameters: Cron (string) - cron expression</param>
    /// <httpMethod>POST</httpMethod>
    /// <returns></returns>
    [HttpPost("ldap/cron")]
    public void SetLdapCronSettingsFromBody(LdapCronRequestDto ldapCronRequest)
    {
        SetLdapCronSettings(ldapCronRequest);
    }

    private void SetLdapCronSettings(LdapCronRequestDto ldapCronRequest)
    {
        CheckLdapPermissions();

        var cron = ldapCronRequest.Cron;

        if (!string.IsNullOrEmpty(cron))
        {
            new CronExpression(cron); // validate

            if (!_settingsManager.Load<LdapSettings>().EnableLdapAuthentication)
            {
                throw new Exception(Resource.LdapSettingsErrorCantSaveLdapSettings);
            }
        }

        var settings = _settingsManager.Load<LdapCronSettings>();

        if (settings == null)
            settings = new LdapCronSettings();

        settings.Cron = cron;
        _settingsManager.Save(settings);

        var t = _tenantManager.GetCurrentTenant();
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
    /// Synchronizes the portal data with the new information from the LDAP server.
    /// </summary>
    /// <short>
    /// Synchronize with LDAP server
    /// </short>
    /// <category>LDAP</category>
    /// <path>api/2.0/settings/ldap/sync</path>
    /// <httpMethod>GET</httpMethod>
    /// <returns>LDAP operation status: completed or not, ID, status, error, warning, percentage of completion, certificate confirmation request, source, operation type</returns>
    [HttpGet("ldap/sync")]
    public LdapStatusDto SyncLdap()
    {
        CheckLdapPermissions();

        var ldapSettings = _settingsManager.Load<LdapSettings>();

        var userId = _authContext.CurrentAccount.ID.ToString();

        var result = _ldapSaveSyncOperation.SyncLdap(ldapSettings, Tenant, userId);

        return _mapper.Map<LdapOperationStatus, LdapStatusDto>(result);
    }

    /// <summary>
    /// Starts the process of collecting preliminary changes on the portal during the synchronization process according to the selected LDAP settings.
    /// </summary>
    /// <short>
    /// Test the LDAP synchronization
    /// </short>
    /// <category>LDAP</category>
    /// <path>api/2.0/settings/ldap/sync/test</path>
    /// <httpMethod>GET</httpMethod>
    /// <returns>LDAP operation status: completed or not, ID, status, error, warning, percentage of completion, certificate confirmation request, source, operation type</returns>
    [HttpGet("ldap/sync/test")]
    public LdapStatusDto TestLdapSync()
    {
        CheckLdapPermissions();

        var ldapSettings = _settingsManager.Load<LdapSettings>();

        var result = _ldapSaveSyncOperation.TestLdapSync(ldapSettings, Tenant);

        return _mapper.Map<LdapOperationStatus, LdapStatusDto>(result);
    }

    /// <summary>
    /// Saves the LDAP settings specified in the request and starts importing/synchronizing users and groups by LDAP.
    /// </summary>
    /// <short>
    /// Save the LDAP settings
    /// </short>
    /// <category>LDAP</category>
    /// <param name="ldapRequestsDto">LDAP settings</param>
    /// <returns>LDAP operation status: completed or not, ID, status, error, warning, percentage of completion, certificate confirmation request, source, operation type</returns>
    /// <path>api/2.0/settings/ldap</path>
    /// <httpMethod>POST</httpMethod>
    [HttpPost("ldap")]
    public LdapStatusDto SaveLdapSettings(LdapRequestsDto ldapRequestsDto)
    {
        var ldapSettings = _mapper.Map<LdapRequestsDto, LdapSettings>(ldapRequestsDto);

        CheckLdapPermissions();

        if (!ldapSettings.EnableLdapAuthentication)
        {
            SetLdapCronSettings(null);
        }

        var userId = _authContext.CurrentAccount.ID.ToString();

        var result = _ldapSaveSyncOperation.SaveLdapSettings(ldapSettings, Tenant, userId);

        return _mapper.Map<LdapOperationStatus, LdapStatusDto>(result);
    }

    /// <summary>
    /// Starts the process of saving LDAP settings and collecting preliminary changes on the portal according to them.
    /// </summary>
    /// <short>
    /// Test the LDAP saving process
    /// </short>
    /// <category>LDAP</category>
    /// <param type="ASC.ActiveDirectory.Base.Settings.LdapSettings, ASC.ActiveDirectory.Base.Settings" name="ldapSettings">LDAP settings: <![CDATA[
    /// <ul>
    ///     <li><b>LdapMapping</b> (Dictionary&lt;MappingFields, string&gt;) - LDAP settings mapping (FirstNameAttribute, SecondNameAttribute, BirthDayAttribute, GenderAttribute, MobilePhoneAttribute, MailAttribute, TitleAttribute, LocationAttribute, AvatarAttribute, AdditionalPhone, AdditionalMobilePhone, AdditionalMail, Skype, UserQuotaLimit),</li>
    ///     <li><b>AccessRights</b> (Dictionary&lt;AccessRight, string&gt;) - accecss rights (FullAccess, Documents, Projects, CRM, Community, People, Mail).</li>
    /// </ul>
    /// ]]></param>
    /// <path>api/2.0/settings/ldap/save/test</path>
    /// <httpMethod>POST</httpMethod>
    /// <returns>LDAP operation status: completed or not, ID, status, error, warning, percentage of completion, certificate confirmation request, source, operation type</returns>
    [HttpPost("ldap/save/test")]
    public LdapStatusDto TestLdapSave(LdapSettings ldapSettings)
    {
        CheckLdapPermissions();

        var userId = _authContext.CurrentAccount.ID.ToString();

        var result = _ldapSaveSyncOperation.TestLdapSave(ldapSettings, Tenant, userId);

        return _mapper.Map<LdapOperationStatus, LdapStatusDto>(result);
    }

    /// <summary>
    /// Returns the LDAP synchronization process status.
    /// </summary>
    /// <short>
    /// Get the LDAP synchronization process status
    /// </short>
    /// <category>LDAP</category>
    /// <returns>LDAP operation status: completed or not, ID, status, error, warning, percentage of completion, certificate confirmation request, source, operation type</returns>
    /// <path>api/2.0/settings/ldap/status</path>
    /// <httpMethod>GET</httpMethod>
    [HttpGet("ldap/status")]
    public LdapStatusDto GetLdapOperationStatus()
    {
        CheckLdapPermissions();

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
    /// <returns>LDAP default settings: enable LDAP authentication or not, start TLS or not, enable SSL or not, send welcome email or not, server name, user name, port number, user filter, login attribute, LDAP settings mapping, access rights, user is a group member or not, group name, user attribute, group filter, group attribute, group name attribute, authentication is enabled or not, login, password, accept certificate or not</returns>
    /// <path>api/2.0/settings/ldap/default</path>
    /// <httpMethod>GET</httpMethod>
    [HttpGet("ldap/default")]
    public LdapSettingsDto GetDefaultLdapSettings()
    {
        CheckLdapPermissions();

        var settings = new LdapSettings().GetDefault();

        return _mapper.Map<LdapSettings, LdapSettingsDto>(settings);
    }

    private void CheckLdapPermissions()
    {
        _permissionContext.DemandPermissions(SecutiryConstants.EditPortalSettings);

        if (!_coreBaseSettings.Standalone
            && (!SetupInfo.IsVisibleSettings(ManagementType.LdapSettings.ToString())
                || !_tenantManager.GetCurrentTenantQuota().Ldap))
        {
            throw new BillingException(Resource.ErrorNotAllowedOption, "Ldap");
        }
    }
}
