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

[Scope]
[DefaultRoute]
[ApiController]
public class SmtpSettingsController : ControllerBase
{
    private readonly PermissionContext _permissionContext;
    private readonly CoreConfiguration _coreConfiguration;
    private readonly CoreBaseSettings _coreBaseSettings;
    private readonly IMapper _mapper;


    public SmtpSettingsController(
        PermissionContext permissionContext,
        CoreConfiguration coreConfiguration,
        CoreBaseSettings coreBaseSettings,
        IMapper mapper)
    {
        _permissionContext = permissionContext;
        _coreConfiguration = coreConfiguration;
        _coreBaseSettings = coreBaseSettings;
        _mapper = mapper;
    }


    [HttpGet("smtp")]
    public async Task<SmtpSettingsDto> GetSmtpSettingsAsync()
    {
        CheckSmtpPermissions();

        var settings = _mapper.Map<SmtpSettings, SmtpSettingsDto>(await _coreConfiguration.GetSmtpSettingsAsync());
        settings.CredentialsUserPassword = "";

        return settings;
    }

    [HttpPost("smtp")]
    public async Task<SmtpSettingsDto> SaveSmtpSettingsAsync( SmtpSettingsDto inDto)
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

    [HttpDelete("smtp")]
    public async Task<SmtpSettingsDto> ResetSmtpSettingsAsync()
    {
        CheckSmtpPermissions();

        if (!(await _coreConfiguration.GetSmtpSettingsAsync()).IsDefaultSettings)
        {
            await _permissionContext.DemandPermissionsAsync(SecutiryConstants.EditPortalSettings);
            await _coreConfiguration.SetSmtpSettingsAsync(null);
        }

        var current = _coreBaseSettings.Standalone ? await _coreConfiguration.GetSmtpSettingsAsync() : SmtpSettings.Empty;

        var settings = _mapper.Map<SmtpSettings, SmtpSettingsDto>(current);
        settings.CredentialsUserPassword = "";

        return settings;
    }

    //[HttpGet("smtp/test")]
    //public SmtpOperationStatus TestSmtpSettings()
    //{
    //    CheckSmtpPermissions();

    //    var settings = ToSmtpSettings(CoreConfiguration.SmtpSettings);

    //    //add resolve
    //    var smtpTestOp = new SmtpOperation(settings, Tenant.Id, SecurityContext.CurrentAccount.ID, UserManager, SecurityContext, TenantManager, Configuration);

    //    SMTPTasks.QueueTask(smtpTestOp.RunJob, smtpTestOp.GetDistributedTask());

    //    return ToSmtpOperationStatus();
    //}

    //[HttpGet("smtp/test/status")]
    //public SmtpOperationStatus GetSmtpOperationStatus()
    //{
    //    CheckSmtpPermissions();

    //    return ToSmtpOperationStatus();
    //}

    //private SmtpOperationStatus ToSmtpOperationStatus()
    //{
    //    var operations = SMTPTasks.GetTasks().ToList();

    //    foreach (var o in operations)
    //    {
    //        if (!string.IsNullOrEmpty(o.InstanseId) &&
    //            Process.GetProcesses().Any(p => p.Id == int.Parse(o.InstanseId)))
    //            continue;

    //        o.SetProperty(SmtpOperation.PROGRESS, 100);
    //        SMTPTasks.RemoveTask(o.Id);
    //    }

    //    var operation =
    //        operations
    //            .FirstOrDefault(t => t.GetProperty<int>(SmtpOperation.OWNER) == Tenant.Id);

    //    if (operation == null)
    //    {
    //        return null;
    //    }

    //    if (DistributedTaskStatus.Running < operation.Status)
    //    {
    //        operation.SetProperty(SmtpOperation.PROGRESS, 100);
    //        SMTPTasks.RemoveTask(operation.Id);
    //    }

    //    var result = new SmtpOperationStatus
    //    {
    //        Id = operation.Id,
    //        Completed = operation.GetProperty<bool>(SmtpOperation.FINISHED),
    //        Percents = operation.GetProperty<int>(SmtpOperation.PROGRESS),
    //        Status = operation.GetProperty<string>(SmtpOperation.RESULT),
    //        Error = operation.GetProperty<string>(SmtpOperation.ERROR),
    //        Source = operation.GetProperty<string>(SmtpOperation.SOURCE)
    //    };

    //    return result;
    //}

    public static SmtpSettings ToSmtpSettingsConfig(SmtpSettingsDto inDto)
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
    private static void CheckSmtpPermissions()
    {
        if (!SetupInfo.IsVisibleSettings(nameof(ManagementType.SmtpSettings)))
        {
            throw new BillingException(Resource.ErrorNotAllowedOption, "Smtp");
        }
    }
}