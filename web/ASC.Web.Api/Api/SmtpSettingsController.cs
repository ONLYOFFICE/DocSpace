/*
 *
 * (c) Copyright Ascensio System Limited 2010-2018
 *
 * This program is freeware. You can redistribute it and/or modify it under the terms of the GNU 
 * General Public License (GPL) version 3 as published by the Free Software Foundation (https://www.gnu.org/copyleft/gpl.html). 
 * In accordance with Section 7(a) of the GNU GPL its Section 15 shall be amended to the effect that 
 * Ascensio System SIA expressly excludes the warranty of non-infringement of any third-party rights.
 *
 * THIS PROGRAM IS DISTRIBUTED WITHOUT ANY WARRANTY; WITHOUT EVEN THE IMPLIED WARRANTY OF MERCHANTABILITY OR
 * FITNESS FOR A PARTICULAR PURPOSE. For more details, see GNU GPL at https://www.gnu.org/copyleft/gpl.html
 *
 * You can contact Ascensio System SIA by email at sales@onlyoffice.com
 *
 * The interactive user interfaces in modified source and object code versions of ONLYOFFICE must display 
 * Appropriate Legal Notices, as required under Section 5 of the GNU GPL version 3.
 *
 * Pursuant to Section 7 § 3(b) of the GNU GPL you must retain the original ONLYOFFICE logo which contains 
 * relevant author attributions when distributing the software. If the display of the logo in its graphic 
 * form is not reasonably feasible for technical reasons, you must include the words "Powered by ONLYOFFICE" 
 * in every copy of the program you distribute. 
 * Pursuant to Section 7 § 3(e) we decline to grant you any rights under trademark law for use of our trademarks.
 *
*/

namespace ASC.Api.Settings;

[Scope]
[DefaultRoute]
[ApiController]
public class SmtpSettingsController : ControllerBase
{
    private readonly PermissionContext _permissionContext;
    private readonly CoreConfiguration _coreConfiguration;
    private readonly CoreBaseSettings _coreBaseSettings;


    public SmtpSettingsController(
        PermissionContext permissionContext,
        CoreConfiguration coreConfiguration,
        CoreBaseSettings coreBaseSettings)
    {
        _permissionContext = permissionContext;
        _coreConfiguration = coreConfiguration;
        _coreBaseSettings = coreBaseSettings;
    }


    [Read("smtp")]
    public SmtpSettingsResponseDto GetSmtpSettings()
    {
        CheckSmtpPermissions();

        var settings = ToSmtpSettings(_coreConfiguration.SmtpSettings, true);

        return settings;
    }

    [Create("smtp")]
    public SmtpSettingsResponseDto SaveSmtpSettingsFromBody([FromBody]SmtpSettingsResponseDto smtpSettings)
    {
        return SaveSmtpSettings(smtpSettings);
    }

    [Create("smtp")]
    [Consumes("application/x-www-form-urlencoded")]
    public SmtpSettingsResponseDto SaveSmtpSettingsFromForm([FromForm] SmtpSettingsResponseDto smtpSettings)
    {
        return SaveSmtpSettings(smtpSettings);
    }

    private SmtpSettingsResponseDto SaveSmtpSettings(SmtpSettingsResponseDto smtpSettings)
    {
        CheckSmtpPermissions();

        //TODO: Add validation check

        if (smtpSettings == null)
            throw new ArgumentNullException(nameof(smtpSettings));

        _permissionContext.DemandPermissions(SecutiryConstants.EditPortalSettings);

        var settingConfig = ToSmtpSettingsConfig(smtpSettings);

        _coreConfiguration.SmtpSettings = settingConfig;

        var settings = ToSmtpSettings(settingConfig, true);

        return settings;
    }

    [Delete("smtp")]
    public SmtpSettingsResponseDto ResetSmtpSettings()
    {
        CheckSmtpPermissions();

        if (!_coreConfiguration.SmtpSettings.IsDefaultSettings)
        {
            _permissionContext.DemandPermissions(SecutiryConstants.EditPortalSettings);
            _coreConfiguration.SmtpSettings = null;
        }

        var current = _coreBaseSettings.Standalone ? _coreConfiguration.SmtpSettings : SmtpSettings.Empty;

        return ToSmtpSettings(current, true);
    }

    //[Read("smtp/test")]
    //public SmtpOperationStatus TestSmtpSettings()
    //{
    //    CheckSmtpPermissions();

    //    var settings = ToSmtpSettings(CoreConfiguration.SmtpSettings);

    //    //add resolve
    //    var smtpTestOp = new SmtpOperation(settings, Tenant.TenantId, SecurityContext.CurrentAccount.ID, UserManager, SecurityContext, TenantManager, Configuration);

    //    SMTPTasks.QueueTask(smtpTestOp.RunJob, smtpTestOp.GetDistributedTask());

    //    return ToSmtpOperationStatus();
    //}

    //[Read("smtp/test/status")]
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
    //            .FirstOrDefault(t => t.GetProperty<int>(SmtpOperation.OWNER) == Tenant.TenantId);

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

    public static SmtpSettings ToSmtpSettingsConfig(SmtpSettingsResponseDto settingsWrapper)
    {
        var settingsConfig = new SmtpSettings(
            settingsWrapper.Host,
            settingsWrapper.Port ?? SmtpSettings.DefaultSmtpPort,
            settingsWrapper.SenderAddress,
            settingsWrapper.SenderDisplayName)
        {
            EnableSSL = settingsWrapper.EnableSSL,
            EnableAuth = settingsWrapper.EnableAuth
        };

        if (settingsWrapper.EnableAuth)
        {
            settingsConfig.SetCredentials(settingsWrapper.CredentialsUserName, settingsWrapper.CredentialsUserPassword);
        }

        return settingsConfig;
    }

    private static SmtpSettingsResponseDto ToSmtpSettings(SmtpSettings settingsConfig, bool hidePassword = false)
    {
        return new SmtpSettingsResponseDto
        {
            Host = settingsConfig.Host,
            Port = settingsConfig.Port,
            SenderAddress = settingsConfig.SenderAddress,
            SenderDisplayName = settingsConfig.SenderDisplayName,
            CredentialsUserName = settingsConfig.CredentialsUserName,
            CredentialsUserPassword = hidePassword ? "" : settingsConfig.CredentialsUserPassword,
            EnableSSL = settingsConfig.EnableSSL,
            EnableAuth = settingsConfig.EnableAuth
        };
    }

    private static void CheckSmtpPermissions()
    {
        if (!SetupInfo.IsVisibleSettings(nameof(ManagementType.SmtpSettings)))
        {
            throw new BillingException(Resource.ErrorNotAllowedOption, "Smtp");
        }
    }
}