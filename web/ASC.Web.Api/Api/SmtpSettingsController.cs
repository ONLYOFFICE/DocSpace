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

namespace ASC.Api.Settings;

///<summary>
/// SMTP settings API.
///</summary>
[Scope]
[DefaultRoute]
[ApiController]
public class SmtpSettingsController : ControllerBase
{
    private readonly PermissionContext _permissionContext;
    private readonly CoreConfiguration _coreConfiguration;
    private readonly CoreBaseSettings _coreBaseSettings;
    private readonly SecurityContext _securityContext;
    private readonly IMapper _mapper;
    private readonly SmtpOperation _smtpOperation;
    private readonly TenantManager _tenantManager;


    public SmtpSettingsController(
        PermissionContext permissionContext,
        CoreConfiguration coreConfiguration,
        CoreBaseSettings coreBaseSettings,
        IMapper mapper,
        SecurityContext securityContext,
        SmtpOperation smtpOperation,
        TenantManager tenantManager)
    {
        _permissionContext = permissionContext;
        _coreConfiguration = coreConfiguration;
        _coreBaseSettings = coreBaseSettings;
        _mapper = mapper;
        _securityContext = securityContext;
        _smtpOperation = smtpOperation;
        _tenantManager = tenantManager;
    }


    /// <summary>
    /// Returns the current portal SMTP settings.
    /// </summary>
    /// <short>
    /// Get the SMTP settings
    /// </short>
    /// <category>SMTP settings</category>
    /// <returns type="ASC.Web.Api.ApiModel.ResponseDto.SmtpSettingsDto, ASC.Web.Api">SMTP settings</returns>
    /// <path>api/2.0/smtpsettings/smtp</path>
    /// <httpMethod>GET</httpMethod>
    [HttpGet("smtp")]
    public async Task<SmtpSettingsDto> GetSmtpSettingsAsync()
    {
        CheckSmtpPermissions();

        var current = await _coreConfiguration.GetDefaultSmtpSettingsAsync();

        if (current.IsDefaultSettings && !_coreBaseSettings.Standalone)
        {
            current = SmtpSettings.Empty;
        }

        var settings = _mapper.Map<SmtpSettings, SmtpSettingsDto>(current);
        settings.CredentialsUserPassword = "";

        return settings;
    }

    /// <summary>
    /// Saves the SMTP settings for the current portal.
    /// </summary>
    /// <short>
    /// Save the SMTP settings
    /// </short>
    /// <category>SMTP settings</category>
    /// <param type="ASC.Web.Api.ApiModel.ResponseDto.SmtpSettingsDto, ASC.Web.Api" name="inDto">SMTP settings</param>
    /// <returns type="ASC.Web.Api.ApiModel.ResponseDto.SmtpSettingsDto, ASC.Web.Api">SMTP settings</returns>
    /// <path>api/2.0/smtpsettings/smtp</path>
    /// <httpMethod>POST</httpMethod>
    [HttpPost("smtp")]
    public async Task<SmtpSettingsDto> SaveSmtpSettingsAsync(SmtpSettingsDto inDto)
    {
        CheckSmtpPermissions();

        //TODO: Add validation check

        ArgumentNullException.ThrowIfNull(inDto);

        await _permissionContext.DemandPermissionsAsync(SecutiryConstants.EditPortalSettings);

        var settingConfig = ToSmtpSettingsConfig(inDto);

        await _coreConfiguration.SetSmtpSettingsAsync(settingConfig);

        var settings = _mapper.Map<SmtpSettings, SmtpSettingsDto>(settingConfig);
        settings.CredentialsUserPassword = "";

        return settings;
    }

    private SmtpSettings ToSmtpSettingsConfig(SmtpSettingsDto inDto)
    {
        var settingsConfig = new SmtpSettings(
            inDto.Host,
            inDto.Port ?? SmtpSettings.DefaultSmtpPort,
            inDto.SenderAddress,
            inDto.SenderDisplayName)
        {
            EnableSSL = inDto.EnableSSL,
            EnableAuth = inDto.EnableAuth
        };

        if (inDto.EnableAuth)
        {
            settingsConfig.SetCredentials(inDto.CredentialsUserName, inDto.CredentialsUserPassword);
        }

        return settingsConfig;
    }

    /// <summary>
    /// Resets the SMTP settings of the current portal.
    /// </summary>
    /// <short>
    /// Reset the SMTP settings
    /// </short>
    /// <category>SMTP settings</category>
    /// <returns type="ASC.Web.Api.ApiModel.ResponseDto.SmtpSettingsDto, ASC.Web.Api">Default SMTP settings</returns>
    /// <path>api/2.0/smtpsettings/smtp</path>
    /// <httpMethod>DELETE</httpMethod>
    [HttpDelete("smtp")]
    public async Task<SmtpSettingsDto> ResetSmtpSettingsAsync()
    {
        CheckSmtpPermissions();

        if (!(await _coreConfiguration.GetDefaultSmtpSettingsAsync()).IsDefaultSettings)
        {
            await _permissionContext.DemandPermissionsAsync(SecutiryConstants.EditPortalSettings);
            await _coreConfiguration.SetSmtpSettingsAsync(null);
        }

        var current = await _coreConfiguration.GetDefaultSmtpSettingsAsync();

        if (current.IsDefaultSettings && !_coreBaseSettings.Standalone)
        {
            current = SmtpSettings.Empty;
        }

        var settings = _mapper.Map<SmtpSettings, SmtpSettingsDto>(current);
        settings.CredentialsUserPassword = "";

        return settings;
    }

    // <summary>
    // Tests the SMTP settings for the current portal (sends test message to the user email).
    // </summary>
    // <short>
    // Test the SMTP settings
    // </short>
    // <category>SMTP settings</category>
    // <returns type="ASC.Api.Settings.Smtp.SmtpOperationStatusRequestsDto, ASC.Web.Api">SMTP operation status</returns>
    // <path>api/2.0/smtpsettings/smtp/test</path>
    // <httpMethod>GET</httpMethod>
    [HttpGet("smtp/test")]
    public async Task<SmtpOperationStatusRequestsDto> TestSmtpSettings()
    {
        CheckSmtpPermissions();

        var settings = _mapper.Map<SmtpSettings, SmtpSettingsDto>(await _coreConfiguration.GetDefaultSmtpSettingsAsync());

        var tenant = _tenantManager.GetCurrentTenant();

        _smtpOperation.StartSmtpJob(settings, tenant, _securityContext.CurrentAccount.ID);

        return _smtpOperation.GetStatus(tenant);
    }

    // <summary>
    // Returns the SMTP test process status.
    // </summary>
    // <short>
    // Get the SMTP test process status
    // </short>
    // <category>SMTP settings</category>
    // <returns type="ASC.Api.Settings.Smtp.SmtpOperationStatusRequestsDto, ASC.Web.Api">SMTP operation status</returns>
    // <path>api/2.0/smtpsettings/smtp/test/status</path>
    // <httpMethod>GET</httpMethod>
    [HttpGet("smtp/test/status")]
    public SmtpOperationStatusRequestsDto GetSmtpOperationStatus()
    {
        CheckSmtpPermissions();

        return _smtpOperation.GetStatus(_tenantManager.GetCurrentTenant());
    }

    private static void CheckSmtpPermissions()
    {
        if (!SetupInfo.IsVisibleSettings(nameof(ManagementType.SmtpSettings)))
        {
            throw new BillingException(Resource.ErrorNotAllowedOption, "Smtp");
        }
    }
}