/*
 *
 * (c) Copyright Ascensio System Limited 2010-2021
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

using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

using ASC.Api.Core;
using ASC.Api.Documents;
using ASC.Api.Projects.Wrappers;
using ASC.Api.Utils;
using ASC.Common.Logging;
using ASC.Core;
using ASC.Core.Common.Settings;
using ASC.Core.Tenants;
using ASC.CRM.Core.Dao;
using ASC.MessagingSystem;
using ASC.Projects;
using ASC.Projects.Classes;
using ASC.Projects.Core.Domain;
using ASC.Projects.Core.Domain.Reports;
using ASC.Projects.Engine;
using ASC.Projects.Model;
using ASC.Projects.Model.Reports;
using ASC.Web.Api.Routing;
using ASC.Web.Core.Files;
using ASC.Web.Core.Users;
using ASC.Web.Core.Utility;
using ASC.Web.Files.Services.DocumentService;
using ASC.Web.Studio.Core.Notify;
using ASC.Web.Studio.Utility;

using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;

namespace ASC.Api.Projects
{
    public class ReportsController : BaseProjectController
    {
        public ReportsController(SecurityContext securityContext, ProjectSecurity projectSecurity, ApiContext context, EngineFactory engineFactory, TenantUtil tenantUtil, DisplayUserSettingsHelper displayUserSettingsHelper, CommonLinkUtility commonLinkUtility, UserPhotoManager userPhotoManager, MessageService messageService, MessageTarget messageTarget, ModelHelper modelHelper, FileWrapperHelper fileWrapperHelper, IOptionsMonitor<ILog> options, DaoFactory factory, ReportHelper reportHelper, DocbuilderReportsUtility docbuilderReportsUtility, IHttpContextAccessor httpContextAccessor, TenantManager tenantManager, ReportTemplateHelper reportTemplateHelper, FilesLinkUtility filesLinkUtility, CustomStatusHelper customStatusHelper, IServiceProvider serviceProvider, SettingsManager settingsManager, HtmlUtility htmlUtility, NotifyConfiguration notifyConfiguration) : base(securityContext, projectSecurity, context, engineFactory, tenantUtil, displayUserSettingsHelper, commonLinkUtility, userPhotoManager, messageService, messageTarget, modelHelper, fileWrapperHelper, options, factory, reportHelper, docbuilderReportsUtility, httpContextAccessor, tenantManager, reportTemplateHelper, filesLinkUtility, customStatusHelper, serviceProvider, settingsManager, htmlUtility, notifyConfiguration)
        {
        }

        [Create(@"report/create")]
        public ReportState CheckReportData(string uri)
        {
            ChechDocBuilder();

            ReportState state;

            ReportHelper.TryCreateReport(uri.Split('?')[1], out state);

            return state;
        }

        [Read(@"report/status")]
        public ReportState GetReportStatus()
        {
            ChechDocBuilder();

            return DocbuilderReportsUtility.Status(ReportOrigin.Projects, HttpContextAccessor, TenantManager.GetCurrentTenant().TenantId, SecurityContext.CurrentAccount.ID);
        }

        [Read(@"report/terminate")]
        public void TerminateReport()
        {
            ChechDocBuilder();

            DocbuilderReportsUtility.Terminate(ReportOrigin.Projects, TenantManager.GetCurrentTenant().TenantId, SecurityContext.CurrentAccount.ID);
        }

        [Create(@"report")]
        public ReportTemplateWrapper SaveReportTemplate(ModelSaveReport model)
        {
            ProjectSecurity.DemandAuthentication();

            if (model.Name == null || model.Name.Trim().Length == 0) throw new ArgumentNullException("name");

            var filter = new TaskFilter
            {
                TagId = model.Tag,
                DepartmentId = model.Departament,
                UserId = model.UserId,
                TimeInterval = model.ReportTimeInterval,
                FromDate = model.FromDate,
                ToDate = model.ToDate,
                ViewType = model.ViewType,
                NoResponsible = model.NoResponsible
            };

            if (model.Project != 0)
            {
                filter.ProjectIds.Add(model.Project);
            }

            if (model.Status != null)
            {
                filter.TaskStatuses.Add((TaskStatus)model.Status);
            }

            var template = ReportTemplateHelper.GetReportTemplate(model.ReportType);
            template.Filter = filter;

            SaveOrUpdateTemplate(template, model.Name, model.Period, model.PeriodItem, model.Hour, model.AutoGenerated);
            MessageService.Send(MessageAction.ReportTemplateCreated, MessageTarget.Create(template.Id), template.Name);
            
            return ModelHelper.GetReportTemplateWrapper(template);
        }

        [Update(@"report/{reportid:int}")]
        public ReportTemplateWrapper UpdateReportTemplate(int reportid, ModelUpdateReport model)
        {
            ProjectSecurity.DemandAuthentication();

            var filter = new TaskFilter
            {
                TagId = model.Tag,
                DepartmentId = model.Departament,
                UserId = model.UserId,
                TimeInterval = model.ReportTimeInterval,
                FromDate = model.FromDate,
                ToDate = model.ToDate,
                ViewType = model.ViewType,
                NoResponsible = model.NoResponsible
            };

            if (model.Project != 0)
            {
                filter.ProjectIds.Add(model.Project);
            }

            if (model.Status != null)
            {
                filter.TaskStatuses.Add((TaskStatus)model.Status);
            }

            var template = EngineFactory.GetReportEngine().GetTemplate(reportid);
            template.Filter = filter;

            SaveOrUpdateTemplate(template, model.Name, model.Period, model.PeriodItem, model.Hour, model.AutoGenerated);
            MessageService.Send(MessageAction.ReportTemplateUpdated, MessageTarget.Create(template.Id), template.Name);

            return ModelHelper.GetReportTemplateWrapper(template);
        }

        [Read(@"report/{reportid:int}")]
        public ReportTemplateWrapper GetReportTemplate(int reportid)
        {
            ProjectSecurity.DemandAuthentication();
            return ModelHelper.GetReportTemplateWrapper(EngineFactory.GetReportEngine().GetTemplate(reportid).NotFoundIfNull());
        }

        [Delete(@"report/{reportid:int}")]
        public ReportTemplateWrapper DeleteReportTemplate(int reportid)
        {
            ProjectSecurity.DemandAuthentication();


            var reportTemplate = EngineFactory.GetReportEngine().GetTemplate(reportid).NotFoundIfNull();

            EngineFactory.GetReportEngine().DeleteTemplate(reportid);
            MessageService.Send(MessageAction.ReportTemplateDeleted, MessageTarget.Create(reportTemplate.Id), reportTemplate.Name);

            return ModelHelper.GetReportTemplateWrapper(reportTemplate);
        }

        [Read(@"report/files")]
        public IEnumerable<FileWrapper<int>> GetGeneratedReports()
        {
            ChechDocBuilder();

            var fileIds = EngineFactory.GetReportEngine().Get().Select(r => r.FileId).ToList();

            return EngineFactory.GetFileEngine().GetFiles(fileIds).Select(r => FileWrapperHelper.GetFileWrapper(r)).OrderByDescending(r => r.Id).ToList();
        }

        [Delete(@"report/files/{fileid:int}")]
        public ReportFile RemoveGeneratedReport(int fileid)
        {
            ProjectSecurity.DemandAuthentication();

            var report = EngineFactory.GetReportEngine().GetByFileId(fileid).NotFoundIfNull();

            EngineFactory.GetReportEngine().Remove(report);

            return report;
        }

        private void ChechDocBuilder()
        {
            ProjectSecurity.DemandAuthentication();
            if (string.IsNullOrEmpty(FilesLinkUtility.DocServiceDocbuilderUrl))
                throw ProjectSecurity.CreateSecurityException();
        }

        private void SaveOrUpdateTemplate(ReportTemplate template, string title, string period, int periodItem, int hour, bool autoGenerated)
        {
            template.Name = HttpUtility.HtmlEncode(title);
            switch (period)
            {
                case "day":
                    template.Cron = string.Format("0 0 {0} * * ?", hour);
                    break;
                case "week":
                    template.Cron = string.Format("0 0 {0} ? * {1}", hour, periodItem);
                    break;
                case "month":
                    template.Cron = string.Format("0 0 {0} {1} * ?", hour, periodItem);
                    break;
                default:
                    template.Cron = string.Format("0 0 {0} * * ?", 12);
                    break;
            }
            template.AutoGenerated = autoGenerated;
            EngineFactory.GetReportEngine().SaveTemplate(template);
        }
    }
}