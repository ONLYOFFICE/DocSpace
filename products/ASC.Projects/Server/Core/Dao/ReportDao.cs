/*
 *
 * (c) Copyright Ascensio System Limited 2010-2021
 * 
 * Licensed under the Apache License, Version 2.0 (the "License");
 * you may not use this file except in compliance with the License.
 * You may obtain a copy of the License at
 * http://www.apache.org/licenses/LICENSE-2.0
 * Unless required by applicable law or agreed to in writing, software
 * distributed under the License is distributed on an "AS IS" BASIS,
 * WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 * See the License for the specific language governing permissions and
 * limitations under the License.
 *
*/


using System;
using System.Collections.Generic;
using System.Linq;

using ASC.Core;
using ASC.Core.Common.EF;
using ASC.Core.Common.EF.Context;
using ASC.Projects.EF;
using ASC.Core.Tenants;
using ASC.Projects.Core.DataInterfaces;
using ASC.Projects.Core.Domain;
using ASC.Projects.Core.Domain.Reports;
using ASC.Common;

namespace ASC.Projects.Data.DAO
{
    [Scope]
    public class ReportDao : BaseDao, IReportDao
    {
        private TenantUtil TenantUtil { get; set; }
        private TenantManager TenantManager { get; set; }
        private ReportTemplateHelper ReportTemplateHelper { get; set; }

        public ReportDao(SecurityContext securityContext, DbContextManager<WebProjectsContext> dbContextManager, TenantUtil tenantUtil, TenantManager tenantManager, ReportTemplateHelper reportTemplateHelper)
            : base(securityContext, dbContextManager, tenantManager)
        {
            TenantUtil = tenantUtil;
            TenantManager = tenantManager;
            ReportTemplateHelper = reportTemplateHelper;
        }


        public List<ReportTemplate> GetTemplates(Guid userId)
        {
            return WebProjectsContext.ReportTemplate
                .Where(rt => rt.CreateBy == userId.ToString())
                .OrderBy(rt => rt.Name)
                .Select(rt=> ToTemplate(rt))
                .ToList();
        }

        public List<ReportTemplate> GetAutoTemplates()
        {
            return WebProjectsContext.ReportTemplate
                .Where(rt => rt.Auto == 1 && rt.TenantId == null)
                .OrderBy(rt => rt.TenantId)
                .Select(rt => ToTemplate(rt))
                .ToList();
        }

        public ReportTemplate GetTemplate(int id)
        {
            return WebProjectsContext.ReportTemplate
                .Where(rt => rt.Id == id && rt.TenantId == null)
                .Select(rt => ToTemplate(rt))
                .SingleOrDefault();
        }

        public ReportTemplate SaveTemplate(ReportTemplate template)
        {
            var dbTemplate = ToDbTemplate(template);
            if (dbTemplate == null) throw new ArgumentNullException("template");
            if (dbTemplate.CreateOn == default(DateTime)) dbTemplate.CreateOn = DateTime.Now;
            if (dbTemplate.CreateBy.Equals(Guid.Empty)) dbTemplate.CreateBy = CurrentUserID.ToString();

            dbTemplate.CreateOn = TenantUtil.DateTimeToUtc(dbTemplate.CreateOn);
            dbTemplate.TenantId = Tenant;
            WebProjectsContext.ReportTemplate.Add(dbTemplate);
            WebProjectsContext.SaveChanges();

            return ToTemplate(dbTemplate);
        }

        public void DeleteTemplate(int id)
        {
            var rt = WebProjectsContext.ReportTemplate.Where(rt => rt.Id == id).SingleOrDefault();
            WebProjectsContext.ReportTemplate.Remove(rt);
            WebProjectsContext.SaveChanges();
        }



        public ReportFile Save(ReportFile reportFile)
        {
            var dbReport = ToDbReportFile(reportFile);
            if (dbReport == null) throw new ArgumentNullException("reportFile");
            if (dbReport.CreateOn == default(DateTime)) dbReport.CreateOn = DateTime.Now;
            if (dbReport.CreateBy.Equals(Guid.Empty)) dbReport.CreateBy = CurrentUserID.ToString();

            dbReport.CreateOn = TenantUtil.DateTimeToUtc(dbReport.CreateOn);
            dbReport.TenantId = Tenant;

            WebProjectsContext.Report.Add(dbReport);
            WebProjectsContext.SaveChanges();

            return ToReportFile(dbReport);
        }

        public IEnumerable<ReportFile> Get()
        {
            return WebProjectsContext.Report
                .Where(r => r.CreateBy == CurrentUserID.ToString())
                .Select(r => ToReportFile(r))
                .OrderBy(r => r.CreateOn).ToList();
        }
        public ReportFile GetByFileId(int id)
        {
            return WebProjectsContext.Report
                .Where(r => r.FileId == id && r.CreateBy == CurrentUserID.ToString())
                .Select(r => ToReportFile(r))
                .SingleOrDefault();
        }

        public void Remove(ReportFile report)
        {
            WebProjectsContext.Report.Remove(WebProjectsContext.Report.Where(r=> r.Id == report.Id).SingleOrDefault());
            WebProjectsContext.SaveChanges();
        }

        private ReportTemplate ToTemplate(DbReportTemplate dbTemplate)
        {
            var tenant = TenantManager.GetTenant(dbTemplate.TenantId.GetValueOrDefault());

            if (tenant == null) return null;

            var template = ReportTemplateHelper.GetReportTemplate((ReportType)dbTemplate.Type);
            template.Id = dbTemplate.Id.GetValueOrDefault();
            template.Name = dbTemplate.Name;
            template.Filter = dbTemplate.Filter != null ? TaskFilter.FromXml(dbTemplate.Filter) : new TaskFilter();
            template.Cron = dbTemplate.Cron;
            template.CreateBy = ToGuid(dbTemplate.CreateBy);
            template.CreateOn = TenantUtil.DateTimeFromUtc(tenant.TimeZone, Convert.ToDateTime(dbTemplate.CreateOn));
            template.Tenant = tenant.TenantId;
            template.AutoGenerated = Convert.ToBoolean(dbTemplate.Auto);
            return template;
        }

        private ReportFile ToReportFile(DbReport report)
        {
            var template = new ReportFile
            {
                Id = report.Id,
                ReportType = (ReportType)report.Type,
                Name = report.Name,
                FileId = report.FileId,
                CreateBy = ToGuid(report.CreateBy),
                CreateOn = TenantUtil.DateTimeFromUtc(TenantManager.GetCurrentTenant().TimeZone, Convert.ToDateTime(report.CreateOn))
            };
            return template;
        }

        private DbReportTemplate ToDbTemplate(ReportTemplate template)
        {
            var tenant = TenantManager.GetTenant(template.Tenant);

            if (tenant == null) return null;

            var dbTemplate = new DbReportTemplate()
            {
                Id = template.Id,
                Type = (int)template.ReportType,
                Name = template.Name,
                Filter = template.Filter.ToXml(),
                Cron = template.Cron,
                CreateBy = template.CreateBy.ToString(),
                CreateOn = TenantUtil.DateTimeFromUtc(tenant.TimeZone, Convert.ToDateTime(template.CreateOn)),
                TenantId = tenant.TenantId,
                Auto = Convert.ToInt32(template.AutoGenerated),
            };
            return dbTemplate;
        }

        private DbReport ToDbReportFile(ReportFile report)
        {
            var template = new DbReport
            {
                Id = report.Id,
                Type = (int)report.ReportType,
                Name = report.Name,
                FileId = report.FileId,
                CreateBy = report.CreateBy.ToString(),
                CreateOn = TenantUtil.DateTimeFromUtc(TenantManager.GetCurrentTenant().TimeZone, Convert.ToDateTime(report.CreateOn))
            };
            return template;
        }
    }
}
