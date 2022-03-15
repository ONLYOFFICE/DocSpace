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


    [Read("smtp")]
    public SmtpSettingsDto GetSmtpSettings()
    {
        CheckSmtpPermissions();

        var settings = _mapper.Map<SmtpSettings, SmtpSettingsDto>(_coreConfiguration.SmtpSettings);
        settings.CredentialsUserPassword = "";

        return settings;
    }

    [Create("smtp")]
    public SmtpSettingsDto SaveSmtpSettingsFromBody([FromBody] SmtpSettingsDto inDto)
    {
        return SaveSmtpSettings(inDto);
    }

    [Create("smtp")]
    [Consumes("application/x-www-form-urlencoded")]
    public SmtpSettingsDto SaveSmtpSettingsFromForm([FromForm] SmtpSettingsDto inDto)
    {
        return SaveSmtpSettings(inDto);
    }

    private SmtpSettingsDto SaveSmtpSettings(SmtpSettingsDto inDto)
    {
        CheckSmtpPermissions();

        //TODO: Add validation check

        ArgumentNullException.ThrowIfNull(inDto);

        _permissionContext.DemandPermissions(SecutiryConstants.EditPortalSettings);

        var settingConfig = ToSmtpSettingsConfig(inDto);

        _coreConfiguration.SmtpSettings = settingConfig;

        var settings = _mapper.Map<SmtpSettings, SmtpSettingsDto>(settingConfig);
        settings.CredentialsUserPassword = "";

        return settings;
    }

    [Delete("smtp")]
    public SmtpSettingsDto ResetSmtpSettings()
    {
        CheckSmtpPermissions();

        if (!_coreConfiguration.SmtpSettings.IsDefaultSettings)
        {
            _permissionContext.DemandPermissions(SecutiryConstants.EditPortalSettings);
            _coreConfiguration.SmtpSettings = null;
        }

        var current = _coreBaseSettings.Standalone ? _coreConfiguration.SmtpSettings : SmtpSettings.Empty;

        var settings = _mapper.Map<SmtpSettings, SmtpSettingsDto>(current);
        settings.CredentialsUserPassword = "";

        return settings;
    }

    //[Read("smtp/test")]
    //public SmtpOperationStatus TestSmtpSettings()
    //{
    //    CheckSmtpPermissions();

    //    var settings = ToSmtpSettings(CoreConfiguration.SmtpSettings);

    //    //add resolve
    //    var smtpTestOp = new SmtpOperation(settings, Tenant.Id, SecurityContext.CurrentAccount.ID, UserManager, SecurityContext, TenantManager, Configuration);

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