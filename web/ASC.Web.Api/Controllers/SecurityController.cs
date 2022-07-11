using System;
using System.Collections.Generic;
using System.Linq;

using ASC.Api.Security;
using ASC.AuditTrail;
using ASC.AuditTrail.Data;
using ASC.Common;
using ASC.Core;
using ASC.Core.Billing;
using ASC.Core.Common.Settings;
using ASC.Core.Tenants;
using ASC.MessagingSystem;
using ASC.Web.Api.Routing;
using ASC.Web.Core.PublicResources;
using ASC.Web.Studio.Core;
using ASC.Web.Studio.Utility;

using Microsoft.AspNetCore.Mvc;

namespace ASC.Web.Api.Controllers
{
    [Scope]
    [DefaultRoute]
    [ApiController]
    public class SecurityController : ControllerBase
    {
        private PermissionContext PermissionContext { get; }
        private CoreBaseSettings CoreBaseSettings { get; }
        private TenantExtra TenantExtra { get; }
        private TenantManager TenantManager { get; }
        private MessageService MessageService { get; }
        private LoginEventsRepository LoginEventsRepository { get; }
        private AuditEventsRepository AuditEventsRepository { get; }
        private AuditReportCreator AuditReportCreator { get; }
        private SettingsManager SettingsManager { get; }

        public SecurityController(
            PermissionContext permissionContext,
            CoreBaseSettings coreBaseSettings,
            TenantExtra tenantExtra,
            TenantManager tenantManager,
            MessageService messageService,
            LoginEventsRepository loginEventsRepository,
            AuditEventsRepository auditEventsRepository,
            AuditReportCreator auditReportCreator,
            SettingsManager settingsManager)
        {
            PermissionContext = permissionContext;
            CoreBaseSettings = coreBaseSettings;
            TenantExtra = tenantExtra;
            TenantManager = tenantManager;
            MessageService = messageService;
            LoginEventsRepository = loginEventsRepository;
            AuditEventsRepository = auditEventsRepository;
            AuditReportCreator = auditReportCreator;
            SettingsManager = settingsManager;
        }

        [Read("audit/login/last")]
        public IEnumerable<EventWrapper> GetLastLoginEvents()
        {
            if (!SetupInfo.IsVisibleSettings(nameof(ManagementType.LoginHistory)))
            {
                throw new BillingException(Resource.ErrorNotAllowedOption, "Audit");
            }

            PermissionContext.DemandPermissions(SecutiryConstants.EditPortalSettings);

            return LoginEventsRepository.GetLast(TenantManager.GetCurrentTenant().TenantId, 20).Select(x => new EventWrapper(x));
        }

        [Read("audit/events/last")]
        public IEnumerable<EventWrapper> GetLastAuditEvents()
        {
            if (!SetupInfo.IsVisibleSettings(nameof(ManagementType.AuditTrail)))
            {
                throw new BillingException(Resource.ErrorNotAllowedOption, "Audit");
            }

            PermissionContext.DemandPermissions(SecutiryConstants.EditPortalSettings);

            return AuditEventsRepository.GetLast(TenantManager.GetCurrentTenant().TenantId, 20).Select(x => new EventWrapper(x));
        }

        [Create("audit/login/report")]
        public object CreateLoginHistoryReport()
        {
            PermissionContext.DemandPermissions(SecutiryConstants.EditPortalSettings);

            var tenantId = TenantManager.GetCurrentTenant().TenantId;

            if (!TenantExtra.GetTenantQuota().Audit || !SetupInfo.IsVisibleSettings(nameof(ManagementType.LoginHistory)))
                throw new BillingException(Resource.ErrorNotAllowedOption, "Audit");

            var settings = SettingsManager.LoadForTenant<TenantAuditSettings>(TenantManager.GetCurrentTenant().TenantId);

            var to = DateTime.UtcNow;
            var from = to.Subtract(TimeSpan.FromDays(settings.LoginHistoryLifeTime));

            var reportName = string.Format(AuditReportResource.LoginHistoryReportName + ".csv", from.ToShortDateString(), to.ToShortDateString());
            var events = LoginEventsRepository.Get(tenantId, from, to);
            var result = AuditReportCreator.CreateCsvReport(events, reportName);

            MessageService.Send(MessageAction.LoginHistoryReportDownloaded);
            return result;
        }

        [Create("audit/events/report")]
        public object CreateAuditTrailReport()
        {
            PermissionContext.DemandPermissions(SecutiryConstants.EditPortalSettings);

            var tenantId = TenantManager.GetCurrentTenant().TenantId;

            if (!TenantExtra.GetTenantQuota().Audit || !SetupInfo.IsVisibleSettings(nameof(ManagementType.AuditTrail)))
                throw new BillingException(Resource.ErrorNotAllowedOption, "Audit");

            var settings = SettingsManager.LoadForTenant<TenantAuditSettings>(TenantManager.GetCurrentTenant().TenantId);

            var to = DateTime.UtcNow;
            var from = to.Subtract(TimeSpan.FromDays(settings.AuditTrailLifeTime));

            var reportName = string.Format(AuditReportResource.AuditTrailReportName + ".csv", from.ToString("MM.dd.yyyy"), to.ToString("MM.dd.yyyy"));

            var events = AuditEventsRepository.Get(tenantId, from, to);
            var result = AuditReportCreator.CreateCsvReport(events, reportName);

            MessageService.Send(MessageAction.AuditTrailReportDownloaded);
            return result;
        }

        [Read("audit/settings/lifetime")]
        public TenantAuditSettings GetAuditSettings()
        {
            if (!SetupInfo.IsVisibleSettings(nameof(ManagementType.LoginHistory)))
            {
                throw new BillingException(Resource.ErrorNotAllowedOption, "Audit");
            }

            PermissionContext.DemandPermissions(SecutiryConstants.EditPortalSettings);

            return SettingsManager.LoadForTenant<TenantAuditSettings>(TenantManager.GetCurrentTenant().TenantId);
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
            if (!TenantExtra.GetTenantQuota().Audit || !SetupInfo.IsVisibleSettings(nameof(ManagementType.LoginHistory)))
                throw new BillingException(Resource.ErrorNotAllowedOption, "Audit");

            PermissionContext.DemandPermissions(SecutiryConstants.EditPortalSettings);

            if (wrapper.settings.LoginHistoryLifeTime <= 0 || wrapper.settings.LoginHistoryLifeTime > TenantAuditSettings.MaxLifeTime)
                throw new ArgumentException("LoginHistoryLifeTime");

            if (wrapper.settings.AuditTrailLifeTime <= 0 || wrapper.settings.AuditTrailLifeTime > TenantAuditSettings.MaxLifeTime)
                throw new ArgumentException("AuditTrailLifeTime");

            SettingsManager.SaveForTenant(wrapper.settings, TenantManager.GetCurrentTenant().TenantId);
            MessageService.Send(MessageAction.AuditSettingsUpdated);

            return wrapper.settings;
        }
    }
}
