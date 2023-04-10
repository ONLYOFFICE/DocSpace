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

public class SsoController : BaseSettingsController
{
    private readonly TenantManager _tenantManager;
    private readonly SettingsManager _settingsManager;
    private readonly PermissionContext _permissionContext;
    private readonly CoreBaseSettings _coreBaseSettings;
    private readonly UserManager _userManager;
    private readonly MessageService _messageService;

    public SsoController(
        TenantManager tenantManager,
        ApiContext apiContext,
        WebItemManager webItemManager,
        IMemoryCache memoryCache,
        IHttpContextAccessor httpContextAccessor,
        SettingsManager settingsManager,
        PermissionContext permissionContext,
        CoreBaseSettings coreBaseSettings,
        UserManager userManager,
        MessageService messageService) : base(apiContext, memoryCache, webItemManager, httpContextAccessor)
    {
        _tenantManager = tenantManager;
        _settingsManager = settingsManager;
        _permissionContext = permissionContext;
        _coreBaseSettings = coreBaseSettings;
        _userManager = userManager;
        _messageService = messageService;
    }

    /// <summary>
    /// Returns the current portal SSO settings.
    /// </summary>
    /// <short>
    /// Get the SSO settings
    /// </short>
    /// <category>SSO</category>
    /// <returns>SSO settings</returns>
    [HttpGet("ssov2")]
    public async Task<SsoSettingsV2> GetSsoSettingsV2()
    {
        await CheckSsoPermissionsAsync();

        var settings = await _settingsManager.LoadAsync<SsoSettingsV2>();

        if (string.IsNullOrEmpty(settings.SpLoginLabel))
        {
            settings.SpLoginLabel = SsoSettingsV2.SSO_SP_LOGIN_LABEL;
        }

        return settings;
    }

    /// <summary>
    /// Returns the default portal SSO settings.
    /// </summary>
    /// <short>
    /// Get the default SSO settings
    /// </short>
    /// <category>SSO</category>
    /// <returns>Default SSO settings</returns>
    [HttpGet("ssov2/default")]
    public async Task<SsoSettingsV2> GetDefaultSsoSettingsV2Async()
    {
        await CheckSsoPermissionsAsync();
        return _settingsManager.GetDefault<SsoSettingsV2>();
    }

    /// <summary>
    /// Returns the constants of the SSO settings.
    /// </summary>
    /// <short>
    /// Get the constants of the SSO settings
    /// </short>
    /// <category>SSO</category>
    /// <returns>Constants of the SSO settings</returns>
    [HttpGet("ssov2/constants")]
    public object GetSsoSettingsV2Constants()
    {
        return new
        {
            SsoNameIdFormatType = new SsoNameIdFormatType(),
            SsoBindingType = new SsoBindingType(),
            SsoSigningAlgorithmType = new SsoSigningAlgorithmType(),
            SsoEncryptAlgorithmType = new SsoEncryptAlgorithmType(),
            SsoSpCertificateActionType = new SsoSpCertificateActionType(),
            SsoIdpCertificateActionType = new SsoIdpCertificateActionType()
        };
    }

    /// <summary>
    /// Saves the SSO settings for the current portal.
    /// </summary>
    /// <short>
    /// Save the SSO settings
    /// </short>
    /// <category>SSO</category>
    /// <param name="serializeSettings">Serialized SSO settings</param>
    /// <returns>SSO settings</returns>
    [HttpPost("ssov2")]
    public async Task<SsoSettingsV2> SaveSsoSettingsV2Async(SsoSettingsRequestsDto model)
    {
        await CheckSsoPermissionsAsync();

        var serializeSettings = model.SerializeSettings;

        if (string.IsNullOrEmpty(serializeSettings))
        {
            throw new ArgumentException(Resource.SsoSettingsCouldNotBeNull);
        }

        var options = new JsonSerializerOptions
        {
            AllowTrailingCommas = true,
            PropertyNameCaseInsensitive = true
        };

        var settings = JsonSerializer.Deserialize<SsoSettingsV2>(serializeSettings, options);

        if (settings == null)
        {
            throw new ArgumentException(Resource.SsoSettingsCouldNotBeNull);
        }

        if (string.IsNullOrWhiteSpace(settings.IdpSettings.EntityId))
        {
            throw new Exception(Resource.SsoSettingsInvalidEntityId);
        }

        if (string.IsNullOrWhiteSpace(settings.IdpSettings.SsoUrl) || !CheckUri(settings.IdpSettings.SsoUrl))
        {
            throw new Exception(string.Format(Resource.SsoSettingsInvalidBinding, "SSO " + settings.IdpSettings.SsoBinding));
        }

        if (!string.IsNullOrWhiteSpace(settings.IdpSettings.SloUrl) && !CheckUri(settings.IdpSettings.SloUrl))
        {
            throw new Exception(string.Format(Resource.SsoSettingsInvalidBinding, "SLO " + settings.IdpSettings.SloBinding));
        }

        if (string.IsNullOrWhiteSpace(settings.FieldMapping.FirstName) ||
            string.IsNullOrWhiteSpace(settings.FieldMapping.LastName) ||
            string.IsNullOrWhiteSpace(settings.FieldMapping.Email))
        {
            throw new Exception(Resource.SsoSettingsInvalidMapping);
        }

        if (string.IsNullOrEmpty(settings.SpLoginLabel))
        {
            settings.SpLoginLabel = SsoSettingsV2.SSO_SP_LOGIN_LABEL;
        }
        else if (settings.SpLoginLabel.Length > 100)
        {
            settings.SpLoginLabel = settings.SpLoginLabel.Substring(0, 100);
        }

        if (!await _settingsManager.SaveAsync(settings))
        {
            throw new Exception(Resource.SsoSettingsCantSaveSettings);
        }

        if (!settings.EnableSso)
        {
            await ConverSsoUsersToOrdinaryAsync();
        }

        var messageAction = settings.EnableSso ? MessageAction.SSOEnabled : MessageAction.SSODisabled;

        await _messageService.SendAsync(messageAction);

        return settings;
    }

    /// <summary>
    /// Resets the SSO settings of the current portal.
    /// </summary>
    /// <short>
    /// Reset the SSO settings
    /// </short>
    /// <category>SSO</category>
    /// <returns>Default SSO settings</returns>
    [HttpDelete("ssov2")]
    public async Task<SsoSettingsV2> ResetSsoSettingsV2Async()
    {
        await CheckSsoPermissionsAsync();

        var defaultSettings = _settingsManager.GetDefault<SsoSettingsV2>();

        if (!await _settingsManager.SaveAsync(defaultSettings))
        {
            throw new Exception(Resource.SsoSettingsCantSaveSettings);
        }

        await ConverSsoUsersToOrdinaryAsync();

        await _messageService.SendAsync(MessageAction.SSODisabled);

        return defaultSettings;
    }

    private async Task ConverSsoUsersToOrdinaryAsync()
    {
        var ssoUsers = (await _userManager.GetUsersAsync()).Where(u => u.IsSSO()).ToList();

        if (!ssoUsers.Any())
        {
            return;
        }

        foreach (var existingSsoUser in ssoUsers)
        {
            existingSsoUser.SsoNameId = null;
            existingSsoUser.SsoSessionId = null;

            existingSsoUser.ConvertExternalContactsToOrdinary();

            await _userManager.UpdateUserInfoAsync(existingSsoUser);
        }
    }

    private static bool CheckUri(string uriName)
    {
        return Uri.TryCreate(uriName, UriKind.Absolute, out var uriResult) && (uriResult.Scheme == Uri.UriSchemeHttp || uriResult.Scheme == Uri.UriSchemeHttps);
    }

    private async Task CheckSsoPermissionsAsync()
    {
        await _permissionContext.DemandPermissionsAsync(SecutiryConstants.EditPortalSettings);

        if (!_coreBaseSettings.Standalone
            && (!SetupInfo.IsVisibleSettings(ManagementType.SingleSignOnSettings.ToString())
                || !(await _tenantManager.GetCurrentTenantQuotaAsync()).Sso))
        {
            throw new BillingException(Resource.ErrorNotAllowedOption, "Sso");
        }
    }

}