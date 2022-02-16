namespace ASC.Web.Api.Controllers.Security;

public class LoginController :BaseSecurityController
{
    public LoginController(PermissionContext permissionContext, 
        TenantExtra tenantExtra, 
        TenantManager tenantManager, 
        MessageService messageService, 
        LoginEventsRepository loginEventsRepository, 
        AuditEventsRepository auditEventsRepository,
        AuditReportCreator auditReportCreator,
        SettingsManager settingsManager) : base(permissionContext, tenantExtra, tenantManager, messageService, loginEventsRepository, auditEventsRepository, auditReportCreator, settingsManager)
    {
    }

    [Read("audit/login/last")]
    public IEnumerable<EventWrapper> GetLastLoginEvents()
    {
        if (!SetupInfo.IsVisibleSettings(nameof(ManagementType.LoginHistory)))
        {
            throw new BillingException(Resource.ErrorNotAllowedOption, "Audit");
        }

        _permissionContext.DemandPermissions(SecutiryConstants.EditPortalSettings);

        return _loginEventsRepository.GetLast(_tenantManager.GetCurrentTenant().TenantId, 20).Select(x => new EventWrapper(x));
    }

    [Create("audit/login/report")]
    public object CreateLoginHistoryReport()
    {
        _permissionContext.DemandPermissions(SecutiryConstants.EditPortalSettings);

        var tenantId = _tenantManager.GetCurrentTenant().TenantId;

        if (!_tenantExtra.GetTenantQuota().Audit || !SetupInfo.IsVisibleSettings(nameof(ManagementType.LoginHistory)))
            throw new BillingException(Resource.ErrorNotAllowedOption, "Audit");

        var settings = _settingsManager.LoadForTenant<TenantAuditSettings>(_tenantManager.GetCurrentTenant().TenantId);

        var to = DateTime.UtcNow;
        var from = to.Subtract(TimeSpan.FromDays(settings.LoginHistoryLifeTime));

        var reportName = string.Format(AuditReportResource.LoginHistoryReportName + ".csv", from.ToShortDateString(), to.ToShortDateString());
        var events = _loginEventsRepository.Get(tenantId, from, to);
        var result = _auditReportCreator.CreateCsvReport(events, reportName);

        _messageService.Send(MessageAction.LoginHistoryReportDownloaded);
        return result;
    }
}
