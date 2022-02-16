namespace ASC.Web.Api.Controllers.Security;

public class SecurityController: BaseSecurityController
{
    public SecurityController(PermissionContext permissionContext,
        TenantExtra tenantExtra,
        TenantManager tenantManager,
        MessageService messageService,
        LoginEventsRepository loginEventsRepository,
        AuditEventsRepository auditEventsRepository,
        AuditReportCreator auditReportCreator,
        SettingsManager settingsManager) : base(permissionContext, tenantExtra, tenantManager, messageService, loginEventsRepository, auditEventsRepository, auditReportCreator, settingsManager)
    {
    }

    [Create("audit/settings/lifetime")]
    public TenantAuditSettings SetAuditSettingsFromBody([FromBody] TenantAuditSettingsWrapper wrapper)
    {
        return SetAuditSettings(wrapper);
    }

    [Create("audit/settings/lifetime")]
    [Consumes("application/x-www-form-urlencoded")]
    public TenantAuditSettings SetAuditSettingsFromForm([FromForm] TenantAuditSettingsWrapper wrapper)
    {
        return SetAuditSettings(wrapper);
    }

    private TenantAuditSettings SetAuditSettings(TenantAuditSettingsWrapper wrapper)
    {
        if (!_tenantExtra.GetTenantQuota().Audit || !SetupInfo.IsVisibleSettings(nameof(ManagementType.LoginHistory)))
            throw new BillingException(Resource.ErrorNotAllowedOption, "Audit");

        _permissionContext.DemandPermissions(SecutiryConstants.EditPortalSettings);

        if (wrapper.settings.LoginHistoryLifeTime <= 0 || wrapper.settings.LoginHistoryLifeTime > TenantAuditSettings.MaxLifeTime)
            throw new ArgumentException("LoginHistoryLifeTime");

        if (wrapper.settings.AuditTrailLifeTime <= 0 || wrapper.settings.AuditTrailLifeTime > TenantAuditSettings.MaxLifeTime)
            throw new ArgumentException("AuditTrailLifeTime");

        _settingsManager.SaveForTenant(wrapper.settings, _tenantManager.GetCurrentTenant().TenantId);
        _messageService.Send(MessageAction.AuditSettingsUpdated);

        return wrapper.settings;
    }
}
