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
using System.Globalization;
using System.IO;
using System.Linq;

using ASC.Core;
using ASC.Core.Tenants;
using ASC.Core.Users;
using ASC.Projects.Core.Domain;
using ASC.Projects.Core.Domain.Reports;
using ASC.Projects.Engine;
using ASC.Web.Core.Helpers;
using ASC.Web.Files.Services.DocumentService;
using Microsoft.Extensions.DependencyInjection;
using ASC.Web.Studio.Utility;

using Autofac;

using Newtonsoft.Json;
using ASC.Web.Core.Users;
using Microsoft.AspNetCore.Http;
using ASC.Common;
using ASC.Projects.Resources;

namespace ASC.Projects.Classes
{
    public class Report
    {
        internal ExtendedReportType ExtendedReportType { get; set; }
        public TaskFilter Filter { get; set; }

        public ReportType ReportType
        {
            get { return ExtendedReportType.ReportType; }
        }

        public ReportInfo ReportInfo
        {
            get { return ExtendedReportType.ReportInfo; }
        }

        internal Report(ExtendedReportType reportType, TaskFilter filter)
        {
            ExtendedReportType = reportType;
            Filter = filter;
        }
    }

    [Scope]
    public class ReportHelper
    {
        private string GetDocbuilderMasterTemplate { get; set; }
        private TenantUtil TenantUtil { get; set; }
        private SecurityContext SecurityContext { get; set; }
        private IServiceProvider ServiceProvider { get; set; }
        private UserManager UserManager { get; set; }
        private TenantManager TenantManager { get; set; }
        private IHttpContextAccessor HttpContextAccessor { get; set; }
        private DocbuilderReportsUtility DocbuilderReportsUtility { get; set; }
        private DisplayUserSettingsHelper DisplayUserSettingsHelper { get; set; }
        private ReportTemplateHelper ReportTemplateHelper { get; set; }

        public ReportHelper(TenantUtil tenantUtil,
            SecurityContext securityContext,
            IServiceProvider serviceProvider,
            UserManager userManager,
            TenantManager tenantManager,
            IHttpContextAccessor httpContextAccessor,
            DocbuilderReportsUtility docbuilderReportsUtility,
            DisplayUserSettingsHelper displayUserSettingsHelper)
        {
            TenantUtil = tenantUtil;
            GetDocbuilderMasterTemplate = GetDocbuilderTemplate("master", -1);
            SecurityContext = securityContext;
            ServiceProvider = serviceProvider;
            UserManager = userManager;
            TenantManager = tenantManager;
            HttpContextAccessor = httpContextAccessor;
            DocbuilderReportsUtility = docbuilderReportsUtility;
            DisplayUserSettingsHelper = displayUserSettingsHelper;
        }

        public string GetFileName(Report report)
        {
            var date = TenantUtil.DateTimeNow();
            return string.Format("{0} ({1} {2}).xlsx",
                           report.ExtendedReportType.ReportFileName.Replace(' ', '_'),
                           date.ToShortDateString(),
                           date.ToShortTimeString());
        }

        private string GetDocbuilderTemplate(string fileName, int v)
        {
            var assembly = System.Reflection.Assembly.GetExecutingAssembly();
            using (var stream = assembly.GetManifestResourceStream(string.Format("ASC.Web.Projects.DocbuilderTemplates.{0}_{1}.docbuilder", fileName, v)) ??
                assembly.GetManifestResourceStream(string.Format("ASC.Web.Projects.DocbuilderTemplates.{0}.docbuilder", fileName)))
            {
                if (stream != null)
                {
                    using (var sr = new StreamReader(stream))
                    {
                        return sr.ReadToEnd();
                    }
                }
            }
            throw new Exception(ReportResource.ErrorCreatingScript);
        }

        private List<object[]> GetDocbuilderData(int templateID, Report report)
        {
            PrepareFilter(templateID, report);
            return report.ExtendedReportType.BuildDocbuilderReport(report.Filter).ToList();
        }

        private void PrepareFilter(int templateID, Report report)
        {
            if (templateID != 0 && !report.Filter.FromDate.Equals(DateTime.MinValue))
            {
                var interval = report.Filter.ToDate.DayOfYear - report.Filter.FromDate.DayOfYear;

                switch (report.ExtendedReportType.ReportType)
                {
                    case ReportType.TasksByUsers:
                    case ReportType.TasksByProjects:
                    {
                        report.Filter.FromDate = TenantUtil.DateTimeNow().Date.AddDays(-interval);
                        report.Filter.ToDate = TenantUtil.DateTimeNow().Date;
                    }
                    break;
                    case ReportType.MilestonesNearest:
                    {
                        report.Filter.FromDate = TenantUtil.DateTimeNow().Date;
                        report.Filter.ToDate = TenantUtil.DateTimeNow().Date.AddDays(interval);
                    }
                    break;
                }
            }
        }

        public Report CreateNewReport(ReportType reportType, TaskFilter filter)
        {
            switch (reportType)
            {
                case ReportType.MilestonesExpired:
                    return new Report(ServiceProvider.GetService<MilestonesExpiredReport>(), filter);

                case ReportType.MilestonesNearest:
                    return new Report(ServiceProvider.GetService<MilestonesNearestReport>(), filter);

                case ReportType.ProjectsList:
                    return new Report(ServiceProvider.GetService<ProjectsListReport>(), filter);

                case ReportType.ProjectsWithoutActiveMilestones:
                    return new Report(ServiceProvider.GetService<ProjectsWithoutActiveMilestonesReport>(), filter);

                case ReportType.ProjectsWithoutActiveTasks:
                    return new Report(ServiceProvider.GetService<ProjectsWithoutActiveTasksReport>(), filter);

                case ReportType.TasksByProjects:
                    return new Report(ServiceProvider.GetService<TasksByProjectsReport>(), filter);

                case ReportType.TasksByUsers:
                    return new Report(ServiceProvider.GetService<TasksByUsersReport>(), filter);

                case ReportType.TasksExpired:
                    return new Report(ServiceProvider.GetService<TasksExpiredReport>(), filter);

                case ReportType.TimeSpend:
                    return new Report(ServiceProvider.GetService<TimeSpendReport>(), filter);

                case ReportType.UsersWithoutActiveTasks:
                    return new Report(ServiceProvider.GetService<UsersWithoutActiveTasksReport>(), filter);

                case ReportType.UsersWorkload:
                    return new Report(ServiceProvider.GetService<UsersWorkloadReport>(), filter);

                case ReportType.UsersActivity:
                    return new Report(ServiceProvider.GetService<UsersActivityReport>(), filter);

                case ReportType.EmptyReport:
                    return new Report(ServiceProvider.GetService<EmptyReport>(), filter);

            }

            throw new Exception("There is not report Type");
        }

        public bool TryCreateReport(string query, out ReportState state)
        {
            var p = ReportFilterSerializer.GetParameterFromUri(query, "reportType");
            if (string.IsNullOrEmpty(p))
                throw new Exception(ReportResource.ErrorParse);

            ReportType reportType;
            if (!Enum.TryParse(p, out reportType))
                throw new Exception(ReportResource.ErrorParse);

            var filter = ReportFilterSerializer.FromUri(query);

            var template = ReportTemplateHelper.GetReportTemplate(reportType);
            template.Id = -1;
            template.Filter = filter;
            template.CreateBy = SecurityContext.CurrentAccount.ID;

            return TryCreateReportFromTemplate(template, ReportTemplateHelper.SaveDocbuilderReport, null, out state);
        }

        public bool TryCreateReportFromTemplate(ReportTemplate template, Action<ReportState, string> callback, object obj, out ReportState state, bool auto = false)
        {
            var report = CreateNewReport(template.ReportType, template.Filter);
            template.Name = GetFileName(report);

            var data = GetDocbuilderData(template.Id, report);

            var dataJson = JsonConvert.SerializeObject(data);
            var columnsJson = JsonConvert.SerializeObject(report.ExtendedReportType.ColumnsName);
            var filterJson = JsonConvert.SerializeObject(template.Filter);

            var userCulture = UserManager.GetUsers(template.CreateBy).GetCulture();
            var reportInfoJson = JsonConvert.SerializeObject(new Dictionary<string, object>
            {
                { "Title", report.ReportInfo.Title },
                { "CreatedText", ReportResource.ReportCreated },
                { "CreatedAt", TenantUtil.DateTimeNow().ToString("M/d/yyyy", CultureInfo.InvariantCulture) },
                { "CreatedBy", ProjectsFilterResource.By + " " + UserManager.GetUsers(template.CreateBy).DisplayUserName(false, DisplayUserSettingsHelper) },
                { "DateFormat", userCulture.DateTimeFormat.ShortDatePattern }
            });

            var tmpFileName = DocbuilderReportsUtility.TmpFileName;

            var script = GetDocbuilderMasterTemplate
                .Replace("${outputFilePath}", tmpFileName)
                .Replace("${reportData}", dataJson)
                .Replace("${reportColumn}", columnsJson)
                .Replace("${reportFilter}", filterJson)
                .Replace("${reportInfo}", reportInfoJson.Replace("\"", "\\\""))
                .Replace("${templateBody}", GetDocbuilderTemplate(report.ReportType.ToString(), report.Filter.ViewType));

            var reportStateData = new ReportStateData(
                GetFileName(report),
                tmpFileName,
                script,
                (int)template.ReportType,
                auto ? ReportOrigin.ProjectsAuto : ReportOrigin.Projects,
                callback,
                obj,
                TenantManager.GetCurrentTenant().TenantId,
                SecurityContext.CurrentAccount.ID);
            state = new ReportState(ServiceProvider, reportStateData, HttpContextAccessor);

            if (data.Count == 0)
            {
                state.Exception = ReportResource.ErrorEmptyData;
                state.Status = ReportStatus.Failed;
                return false;
            }

            DocbuilderReportsUtility.Enqueue(state);

            return true;
        }
    }

    [Transient]
    public abstract class ExtendedReportType
    {
        public ReportType ReportType { get; private set; }
        public abstract string[] ColumnsName { get; }
        public abstract ReportInfo ReportInfo { get; }
        public abstract string ReportFileName { get; }
        public abstract IEnumerable<object[]> BuildDocbuilderReport(TaskFilter filter);
        protected string VirtualRoot
        {
            get { return CommonLinkUtility.VirtualRoot != "/" ? CommonLinkUtility.VirtualRoot : string.Empty; }
        }
        protected string VirtulaRootPath
        {
            get { return CommonLinkUtility.ServerRootPath + VirtualRoot; }
        }

        internal CommonLinkUtility CommonLinkUtility { get; set; }
        internal UserManager UserManager { get; set; }
        internal DisplayUserSettingsHelper DisplayUserSettingsHelper { get; set; }
        internal TenantUtil TenantUtil { get; set; }
        internal CustomNamingPeople CustomNamingPeople { get; set; }
        internal FilterHelper FilerHelper { get; set; }
        internal EngineFactory EngineFactory { get; set; }

        protected ExtendedReportType(CommonLinkUtility commonLinkUtility,
            EngineFactory engineFactory,
            UserManager userManager,
            DisplayUserSettingsHelper displayUserSettingsHelper,
            TenantUtil tenantUtil,
            CustomNamingPeople customNamingPeople)
        {
            CommonLinkUtility = commonLinkUtility;
            UserManager = userManager;
            DisplayUserSettingsHelper = displayUserSettingsHelper;
            TenantUtil = tenantUtil;
            CustomNamingPeople = customNamingPeople;
            EngineFactory = engineFactory;
        }

        protected void Init(ReportType reportType)
        {
            ReportType = reportType;
        }

    }

    [Transient]
    class MilestonesReport : ExtendedReportType
    {
        protected string[] MileColumns
        {
            get
            {
                return new[]
                           {
                               ProjectResource.Project,
                               MilestoneResource.Milestone,
                               ReportResource.DeadLine,
                               VirtulaRootPath
                           };
            }
        }

        public MilestonesReport(CommonLinkUtility commonLinkUtility,
            EngineFactory engineFactory,
            UserManager userManager,
            DisplayUserSettingsHelper displayUserSettingsHelper,
            TenantUtil tenantUtil,
            CustomNamingPeople customNamingPeople)
            : base(commonLinkUtility,
                  engineFactory,
                  userManager,
                  displayUserSettingsHelper,
                  tenantUtil, 
                  customNamingPeople)
        {
        }

        public override IEnumerable<object[]> BuildDocbuilderReport(TaskFilter filter)
        {
            return EngineFactory.GetMilestoneEngine()
                .GetByFilter(filter)
                .OrderBy(r => r.Project.Title)
                .Select(r => new object[]
                {
                    r.Project.Title, UserManager.GetUsers(r.Project.Responsible).DisplayUserName(false, DisplayUserSettingsHelper),
                    r.Title, UserManager.GetUsers(r.Responsible).DisplayUserName(false, DisplayUserSettingsHelper), r.DeadLine.ToString("d"),
                    (int)((DateTime.Now - r.DeadLine).TotalDays)
                });
        }

        public override string[] ColumnsName
        {
            get
            {
                return new[]
                {
                    MilestoneResource.Milestone,
                    MilestoneResource.Responsible,
                    MilestoneResource.MilestoneDeadline,
                    MilestoneResource.Overdue + ", " + MilestoneResource.Days
                };
            }
        }

        public override ReportInfo ReportInfo
        {
            get { throw new NotImplementedException(); }
        }

        public override string ReportFileName
        {
            get
            {
                return ReportResource.ReportLateMilestones_Title;
            }
        }
    }

    [Transient]
    class MilestonesExpiredReport : MilestonesReport
    {
        public MilestonesExpiredReport(CommonLinkUtility commonLinkUtility,
            EngineFactory engineFactory,
            UserManager userManager,
            DisplayUserSettingsHelper displayUserSettingsHelper,
            TenantUtil tenantUtil,
            CustomNamingPeople customNamingPeople)
            : base(commonLinkUtility,
                  engineFactory,
                  userManager,
                  displayUserSettingsHelper,
                  tenantUtil,
                  customNamingPeople)
        {
            Init(ReportType.MilestonesExpired);
        }

        public override IEnumerable<object[]> BuildDocbuilderReport(TaskFilter filter)
        {
            filter.ToDate = TenantUtil.DateTimeNow();
            filter.MilestoneStatuses = new List<MilestoneStatus> { MilestoneStatus.Open };

            return base.BuildDocbuilderReport(filter);
        }

        public override ReportInfo ReportInfo
        {
            get
            {
                return new ReportInfo(
                        String.Format(ReportResource.ReportLateMilestones_Description, "<ul>", "</ul>", "<li>", "</li>"),
                        ReportResource.ReportLateMilestones_Title,
                        MileColumns);
            }
        }

        public override string ReportFileName
        {
            get { return ReportResource.ReportLateMilestones_Title; }
        }
    }

    [Transient]
    class MilestonesNearestReport : MilestonesReport
    {
        public MilestonesNearestReport(CommonLinkUtility commonLinkUtility,
            EngineFactory engineFactory,
            UserManager userManager,
            DisplayUserSettingsHelper displayUserSettingsHelper,
            TenantUtil tenantUtil,
            CustomNamingPeople customNamingPeople)
            : base(commonLinkUtility,
                  engineFactory,
                  userManager,
                  displayUserSettingsHelper,
                  tenantUtil,
                  customNamingPeople)
        {
            Init(ReportType.MilestonesNearest);
        }

        public override IEnumerable<object[]> BuildDocbuilderReport(TaskFilter filter)
        {
            filter.MilestoneStatuses = new List<MilestoneStatus> { MilestoneStatus.Open };

            return EngineFactory.GetMilestoneEngine()
                .GetByFilter(filter)
                .OrderBy(r => r.Project.Title)
                .Where(r => ((r.DeadLine - DateTime.Now).TotalDays) > 0)
                .Select(r => new object[]
                {
                    r.Project.Title, UserManager.GetUsers(r.Project.Responsible).DisplayUserName(false, DisplayUserSettingsHelper),
                    r.Title, UserManager.GetUsers(r.Responsible).DisplayUserName(false, DisplayUserSettingsHelper), r.DeadLine.ToString("d"),
                    (int)((r.DeadLine - DateTime.Now).TotalDays)
                });
        }

        public override string[] ColumnsName
        {
            get
            {
                return new[]
                {
                    MilestoneResource.Milestone,
                    MilestoneResource.Responsible,
                    MilestoneResource.MilestoneDeadline,
                    MilestoneResource.Next + ", " + MilestoneResource.Days
                };
            }
        }

        public override ReportInfo ReportInfo
        {
            get
            {
                return new ReportInfo(String.Format(ReportResource.ReportUpcomingMilestones_Description, "<ul>", "</ul>", "<li>", "</li>"),
                        ReportResource.ReportUpcomingMilestones_Title,
                        MileColumns);
            }
        }

        public override string ReportFileName
        {
            get { return ReportResource.ReportUpcomingMilestones_Title; }
        }
    }

    [Transient]
    class ProjectsListReport : ExtendedReportType
    {
        protected string[] ProjColumns
        {
            get
            {
                return new[]
                           {
                               ProjectsCommonResource.Title,
                               ProjectResource.ProjectLeader,
                               ProjectsCommonResource.Status,
                               GrammaticalResource.MilestoneGenitivePlural,
                               GrammaticalResource.TaskGenitivePlural,
                               ReportResource.Participiants,
                               ReportResource.ClickToSortByThisColumn,
                               CommonLinkUtility.ServerRootPath,
                               VirtualRoot
                           };
            }
        }

        public ProjectsListReport(CommonLinkUtility commonLinkUtility,
            EngineFactory engineFactory,
            UserManager userManager,
            DisplayUserSettingsHelper displayUserSettingsHelper,
            TenantUtil tenantUtil,
            CustomNamingPeople customNamingPeople) 
            : this(commonLinkUtility,
                  engineFactory,
                  userManager,
                  displayUserSettingsHelper,
                  tenantUtil,
                  customNamingPeople,
                  ReportType.ProjectsList)
        {
        }

        public ProjectsListReport(CommonLinkUtility commonLinkUtility,
            EngineFactory engineFactory,
            UserManager userManager,
            DisplayUserSettingsHelper displayUserSettingsHelper,
            TenantUtil tenantUtil,
            CustomNamingPeople customNamingPeople,
            ReportType reportType)
            : base(commonLinkUtility,
                  engineFactory,
                  userManager,
                  displayUserSettingsHelper,
                  tenantUtil,
                  customNamingPeople)
        {
            Init(reportType);
        }


        public override IEnumerable<object[]> BuildDocbuilderReport(TaskFilter filter)
        {
            filter.SortBy = "title";
            filter.SortOrder = true;

            var result = EngineFactory.GetProjectEngine()
                .GetByFilter(filter)
                .Select(r => new object[]
                {
                    r.Title, UserManager.GetUsers(r.Responsible).DisplayUserName(false, DisplayUserSettingsHelper),
                    LocalizedEnumConverter.ConvertToString(r.Status),
                    r.MilestoneCount, r.TaskCount, r.ParticipantCount
                });

            result = result.OrderBy(r => (string)r[1]);

            return result;
        }

        public override string[] ColumnsName
        {
            get
            {
                return new[]
                {
                    ProjectsCommonResource.Title,
                    ProjectResource.ProjectLeader,
                    ProjectsCommonResource.Status,
                    GrammaticalResource.MilestoneGenitivePlural,
                    GrammaticalResource.TaskGenitivePlural,
                    ReportResource.Participiants,
                    LocalizedEnumConverter.ConvertToString(ProjectStatus.Open)
                };
            }
        }

        public override ReportInfo ReportInfo
        {
            get
            {
                return new ReportInfo(
                    String.Format(ReportResource.ReportProjectList_Description, "<ul>", "</ul>", "<li>", "</li>"),
                    ReportResource.ReportProjectList_Title,
                    ProjColumns);
            }
        }

        public override string ReportFileName
        {
            get { return ReportResource.ReportProjectList_Title; }
        }
    }

    [Transient]
    class ProjectsWithoutActiveMilestonesReport : ProjectsListReport
    {
        public ProjectsWithoutActiveMilestonesReport(CommonLinkUtility commonLinkUtility,
            EngineFactory engineFactory,
            UserManager userManager,
            DisplayUserSettingsHelper displayUserSettingsHelper,
            TenantUtil tenantUtil,
            CustomNamingPeople customNamingPeople)
            : base(commonLinkUtility,
                  engineFactory,
                  userManager,
                  displayUserSettingsHelper,
                  tenantUtil,
                  customNamingPeople)
        {
            Init(ReportType.UsersWithoutActiveTasks);
        }


        public override IEnumerable<object[]> BuildDocbuilderReport(TaskFilter filter)
        {
            return base.BuildDocbuilderReport(filter).Where(r => (int)r[3] == 0);
        }

        public override ReportInfo ReportInfo
        {
            get
            {
                return new ReportInfo(String.Format(ReportResource.ReportProjectsWithoutActiveMilestones_Description, "<ul>", "</ul>", "<li>", "</li>"),
                    ReportResource.ReportProjectsWithoutActiveMilestones_Title,
                    ProjColumns);
            }
        }

        public override string ReportFileName
        {
            get { return ReportResource.ReportProjectsWithoutActiveMilestones_Title; }
        }
    }

    [Transient]
    class ProjectsWithoutActiveTasksReport : ProjectsListReport
    {
        public ProjectsWithoutActiveTasksReport(CommonLinkUtility commonLinkUtility,
            EngineFactory engineFactory,
            UserManager userManager,
            DisplayUserSettingsHelper displayUserSettingsHelper,
            TenantUtil tenantUtil,
            CustomNamingPeople customNamingPeople)
            : base(commonLinkUtility,
                  engineFactory,
                  userManager,
                  displayUserSettingsHelper,
                  tenantUtil,
                  customNamingPeople)
        {
            Init(ReportType.ProjectsWithoutActiveTasks);
        }

        public override IEnumerable<object[]> BuildDocbuilderReport(TaskFilter filter)
        {
            return base.BuildDocbuilderReport(filter).Where(r => (int)r[4] == 0);
        }

        public override ReportInfo ReportInfo
        {
            get
            {
                return new ReportInfo(String.Format(ReportResource.ReportProjectsWithoutActiveTasks_Description, "<ul>", "</ul>", "<li>", "</li>"),
                    ReportResource.ReportProjectsWithoutActiveTasks_Title,
                    ProjColumns);
            }
        }

        public override string ReportFileName
        {
            get { return ReportResource.ReportProjectsWithoutActiveTasks_Title; }
        }
    }

    [Transient]
    class TasksReport : ExtendedReportType
    {
        public TasksReport(CommonLinkUtility commonLinkUtility,
            EngineFactory engineFactory,
            UserManager userManager,
            DisplayUserSettingsHelper displayUserSettingsHelper,
            TenantUtil tenantUtil,
            CustomNamingPeople customNamingPeople,
            ReportType reportType)
            : base(commonLinkUtility,
                  engineFactory,
                  userManager,
                  displayUserSettingsHelper,
                  tenantUtil,
                  customNamingPeople)
        {
            Init(reportType);
        }
        protected string[] TaskColumns
        {
            get
            {
                return new[]
                           {

                               ProjectResource.Project,
                               MilestoneResource.Milestone,
                               TaskResource.Task,
                               TaskResource.TaskResponsible,
                               ProjectsCommonResource.Status,
                               TaskResource.UnsortedTask,
                               ReportResource.DeadLine,
                               ReportResource.NoMilestonesAndTasks,
                               CommonLinkUtility.ServerRootPath,
                               VirtualRoot
                           };
            }
        }

        public override IEnumerable<object[]> BuildDocbuilderReport(TaskFilter filter)
        {
            if (!filter.UserId.Equals(Guid.Empty))
            {
                filter.ParticipantId = filter.UserId;
                filter.UserId = Guid.Empty;
            }

            var createdFrom = filter.FromDate;
            var createdTo = filter.ToDate;

            filter.FromDate = DateTime.MinValue;
            filter.ToDate = DateTime.MinValue;

            filter.SortBy = "deadline";
            filter.SortOrder = true;
            var tasks = EngineFactory.GetTaskEngine().GetByFilter(filter)
                    .FilterResult.OrderBy(r => r.Project.Title)
                    .ToList();

            filter.FromDate = createdFrom;
            filter.ToDate = createdTo;

            if (!filter.FromDate.Equals(DateTime.MinValue) && !filter.ToDate.Equals(DateTime.MinValue))
                tasks =
                    tasks.Where(r => r.CreateOn.Date >= filter.FromDate && r.CreateOn.Date <= filter.ToDate)
                        .ToList();

            if (!filter.NoResponsible)
                tasks = tasks.Where(r => r.Responsibles.Any()).ToList();

            var projects = tasks.Select(r => r.Project).Distinct();
            var result = new List<object[]>();

            foreach (var proj in projects)
                result.Add(new object[] {
                    new object[] { proj.Title, LocalizedEnumConverter.ConvertToString(proj.Status),
                        UserManager.GetUsers(proj.Responsible).DisplayUserName(false, DisplayUserSettingsHelper), proj.CreateOn.ToString("d"), proj.Description },
                    tasks.Where(r => r.Project.ID == proj.ID).Select(r => new object[]
                    {
                        r.Title,
                        r.MilestoneDesc != null ? r.MilestoneDesc.Title : "",
                        string.Join(", ", r.Responsibles.Select(x => UserManager.GetUsers(x).DisplayUserName(false, DisplayUserSettingsHelper))),
                        r.Deadline.Equals(DateTime.MinValue) ? ( r.MilestoneDesc == null ? "" : r.MilestoneDesc.DeadLine.ToString("d") ) : r.Deadline.ToString("d"),
                        LocalizedEnumConverter.ConvertToString(r.Status)
                    })
                });

            return result;
        }

        public override string[] ColumnsName
        {
            get
            {
                return new[]
                {
                    ProjectsCommonResource.Status,
                    ProjectResource.ProjectLeader,
                    TaskResource.CreatingDate,
                    ProjectsCommonResource.Description,

                    TaskResource.Task,
                    GrammaticalResource.ResponsibleNominativePlural,
                    MilestoneResource.MilestoneDeadline,
                    ProjectsCommonResource.Status,
                    LocalizedEnumConverter.ConvertToString(TaskStatus.Open)
                };
            }
        }

        public override ReportInfo ReportInfo
        {
            get { throw new NotImplementedException(); }
        }

        public override string ReportFileName
        {
            get { throw new NotImplementedException(); }
        }
    }

    [Transient]
    class TasksByProjectsReport : TasksReport
    {
        public TasksByProjectsReport(CommonLinkUtility commonLinkUtility,
            EngineFactory engineFactory,
            UserManager userManager,
            DisplayUserSettingsHelper displayUserSettingsHelper,
            TenantUtil tenantUtil,
            CustomNamingPeople customNamingPeople)
            : base(commonLinkUtility,
                  engineFactory,
                  userManager,
                  displayUserSettingsHelper,
                  tenantUtil,
                  customNamingPeople,
                  ReportType.TasksByProjects)
        {
        }

        public override ReportInfo ReportInfo
        {
            get
            {
                return new ReportInfo(
                    String.Format(ReportResource.ReportTaskList_Description, "<ul>", "</ul>", "<li>", "</li>"),
                    ReportResource.ReportTaskList_Title,
                    TaskColumns);
            }
        }

        public override string ReportFileName
        {
            get { return ReportResource.ReportTaskList_Title; }
        }
    }

    [Transient]
    class TasksByUsersReport : TasksReport
    {
        public TasksByUsersReport(CommonLinkUtility commonLinkUtility,
            EngineFactory engineFactory,
            UserManager userManager,
            DisplayUserSettingsHelper displayUserSettingsHelper,
            TenantUtil tenantUtil,
            CustomNamingPeople customNamingPeople)
            : base(commonLinkUtility,
                  engineFactory,
                  userManager,
                  displayUserSettingsHelper,
                  tenantUtil,
                  customNamingPeople,
                  ReportType.TasksByUsers)
        {
        }

        public override ReportInfo ReportInfo
        {
            get
            {
                return new ReportInfo(
                    String.Format(ReportResource.ReportUserTasks_Description, "<ul>", "</ul>", "<li>", "</li>"),
                    ReportResource.ReportUserTasks_Title,
                    TaskColumns);
            }
        }

        public override IEnumerable<object[]> BuildDocbuilderReport(TaskFilter filter)
        {
            if (!filter.UserId.Equals(Guid.Empty))
            {
                filter.ParticipantId = filter.UserId;
                filter.UserId = Guid.Empty;
            }

            var createdFrom = filter.FromDate;
            var createdTo = filter.ToDate;

            filter.FromDate = DateTime.MinValue;
            filter.ToDate = DateTime.MinValue;

            filter.SortBy = "deadline";
            filter.SortOrder = true;

            var tasks = EngineFactory.GetTaskEngine().GetByFilter(filter)
                    .FilterResult.OrderBy(r => r.Project.Title)
                    .ToList();

            filter.FromDate = createdFrom;
            filter.ToDate = createdTo;

            if (!filter.FromDate.Equals(DateTime.MinValue) && !filter.ToDate.Equals(DateTime.MinValue))
                tasks =
                    tasks.Where(r => r.CreateOn.Date >= filter.FromDate && r.CreateOn.Date <= filter.ToDate)
                        .ToList();

            var result = new List<object[]>();

            var users = filter.ParticipantId.HasValue ? new List<Guid> { filter.ParticipantId.Value } : tasks.SelectMany(r => r.Responsibles).Distinct();
            foreach (var user in users)
            {
                var userTasks = tasks.Where(r => r.Responsibles.Contains(user));
                var userProj = userTasks.Select(r => r.Project);
                var projData = new List<object[]>();
                foreach (var pr in userProj)
                {
                    var prTasks = userTasks.Where(r => r.Project == pr);
                    projData.Add(new object[] { pr.Title, UserManager.GetUsers(pr.Responsible).DisplayUserName(false, DisplayUserSettingsHelper),
                    prTasks.Select(r => new object[] { r.Title,
                        r.Deadline.Equals(DateTime.MinValue) ? ( r.MilestoneDesc == null ? "" : r.MilestoneDesc.DeadLine.ToString("d") ) : r.Deadline.ToString("d"),
                        LocalizedEnumConverter.ConvertToString(r.Status),
                        LocalizedEnumConverter.ConvertToString(r.Priority), r.StartDate.Equals(DateTime.MinValue) ? "" : r.StartDate.ToString("d") })});
                }

                result.Add(new object[] { UserManager.GetUsers(user).DisplayUserName(false, DisplayUserSettingsHelper), projData });
            }

            return result;
        }

        public override string[] ColumnsName
        {
            get
            {
                return new[]
                {
                    ProjectResource.ProjectLeader,
                    TaskResource.Task,
                    MilestoneResource.MilestoneDeadline,
                    ProjectsCommonResource.Status,
                    TaskResource.Priority,
                    TaskResource.TaskStartDate,
                    LocalizedEnumConverter.ConvertToString(TaskStatus.Open)
                };
            }
        }

        public override string ReportFileName
        {
            get { return ReportResource.ReportUserTasks_Title; }
        }
    }

    [Transient]
    class TasksExpiredReport : ExtendedReportType
    {
        public TasksExpiredReport(CommonLinkUtility commonLinkUtility,
            EngineFactory engineFactory,
            UserManager userManager,
            DisplayUserSettingsHelper displayUserSettingsHelper,
            TenantUtil tenantUtil,
            CustomNamingPeople customNamingPeople)
            : base(commonLinkUtility,
                  engineFactory,
                  userManager,
                  displayUserSettingsHelper,
                  tenantUtil,
                  customNamingPeople)
        {
            Init(ReportType.TasksExpired);
        }

        public override IEnumerable<object[]> BuildDocbuilderReport(TaskFilter filter)
        {
            filter.FromDate = new DateTime(1970, 1, 1);
            filter.ToDate = TenantUtil.DateTimeNow();
            filter.TaskStatuses.Add(TaskStatus.Open);
            var tasks = EngineFactory.GetTaskEngine().GetByFilter(filter)
                    .FilterResult.OrderBy(r => r.Project.Title)
                    .ToList();

            var projects = tasks.Select(r => r.Project).Distinct();

            var result = new List<object[]>();

            foreach (var pr in projects)
                result.Add(new object[] { pr.Title, UserManager.GetUsers(pr.Responsible).DisplayUserName(false, DisplayUserSettingsHelper),
                    tasks.Where(r => r.Project.ID == pr.ID).Select(r => new object[]
            {
                r.Title,
                string.Join(", ", r.Responsibles.Select(x => UserManager.GetUsers(x).DisplayUserName(false, DisplayUserSettingsHelper))),
                r.Deadline.Equals(DateTime.MinValue) ? ( r.MilestoneDesc == null ? "" : r.MilestoneDesc.DeadLine.ToString("d") ) : r.Deadline.ToString("d"),
                (int)((DateTime.Now - (DateTime.MinValue.Equals(r.Deadline) ? r.MilestoneDesc.DeadLine : r.Deadline)).TotalDays)
            })});

            return result;
        }

        public override string[] ColumnsName
        {
            get
            {
                return new[]
                {
                    ProjectResource.ProjectLeader,

                    TaskResource.Task,
                    GrammaticalResource.ResponsibleNominativePlural,
                    MilestoneResource.MilestoneDeadline,
                    MilestoneResource.Overdue + ", " + MilestoneResource.Days
                };
            }
        }

        public override ReportInfo ReportInfo
        {
            get
            {
                var taskExpiredColumns = new[]
                {
                    ProjectResource.Project,
                    MilestoneResource.Milestone,
                    TaskResource.Task,
                    TaskResource.TaskResponsible,
                    ProjectsCommonResource.Status,
                    TaskResource.UnsortedTask,
                    TaskResource.DeadLine,
                    ReportResource.NoMilestonesAndTasks,
                    CommonLinkUtility.ServerRootPath,
                    VirtualRoot
                };

                return new ReportInfo(
                    String.Format(ReportResource.ReportLateTasks_Description, "<ul>", "</ul>", "<li>", "</li>"),
                    ReportResource.ReportLateTasks_Title,
                    taskExpiredColumns);
            }
        }

        public override string ReportFileName
        {
            get { return ReportResource.ReportLateTasks_Title; }
        }
    }

    [Transient]
    class UsersWithoutActiveTasksReport : ExtendedReportType
    {
        public UsersWithoutActiveTasksReport(CommonLinkUtility commonLinkUtility,
            EngineFactory engineFactory,
            UserManager userManager,
            DisplayUserSettingsHelper displayUserSettingsHelper,
            TenantUtil tenantUtil,
            CustomNamingPeople customNamingPeople)
            : base(commonLinkUtility,
                  engineFactory,
                  userManager,
                  displayUserSettingsHelper,
                  tenantUtil,
                  customNamingPeople)
        {
            Init(ReportType.UsersWithoutActiveTasks);
        }

        public override IEnumerable<object[]> BuildDocbuilderReport(TaskFilter filter)
        {
            if (filter.ProjectIds.Count == 0)
            {
                filter.ProjectIds = EngineFactory.GetTagEngine().GetTagProjects(filter.TagId).ToList();
            }

            var result = EngineFactory.GetReportEngine().BuildUsersWithoutActiveTasks(filter);

            result = result.OrderBy(r => (string)r[0]).ToList();

            return result;
        }

        public override string[] ColumnsName
        {
            get
            {
                return new[]
                {
                    ReportResource.CsvColumnUserName,
                    ResourceEnumConverter.ConvertToString(TaskStatus.Open),
                    ResourceEnumConverter.ConvertToString(TaskStatus.Closed),
                    ProjectsCommonResource.Total
                };
            }
        }

        public override ReportInfo ReportInfo
        {
            get
            {
                var userColumns = new[]
                {
                    ReportResource.User,
                    "Not Accept",
                    ResourceEnumConverter.ConvertToString(TaskStatus.Open),
                    ReportResource.ActiveTasks,
                    ResourceEnumConverter.ConvertToString(TaskStatus.Closed),
                    ProjectsCommonResource.Total,
                    ReportResource.ClickToSortByThisColumn,
                    CommonLinkUtility.ServerRootPath
                };

                return new ReportInfo(
                    String.Format(ReportResource.ReportEmployeesWithoutActiveTasks_Description, "<ul>", "</ul>", "<li>",
                        "</li>"),
                    CustomNamingPeople.Substitute<ReportResource>("ReportEmployeesWithoutActiveTasks_Title")
                        .HtmlEncode(),
                    userColumns);
            }
        }

        public override string ReportFileName
        {
            get
            {
                return CustomNamingPeople.Substitute<ReportResource>("ReportEmployeesWithoutActiveTasks_Title").HtmlEncode();
            }
        }
    }

    [Transient]
    class UsersWorkloadReport : ExtendedReportType
    {
        public UsersWorkloadReport(CommonLinkUtility commonLinkUtility,
            EngineFactory engineFactory,
            UserManager userManager,
            DisplayUserSettingsHelper displayUserSettingsHelper,
            TenantUtil tenantUtil,
            CustomNamingPeople customNamingPeople)
            : base(commonLinkUtility,
                  engineFactory,
                  userManager,
                  displayUserSettingsHelper,
                  tenantUtil,
                  customNamingPeople)
        {
            Init(ReportType.UsersWorkload);
        }

        public override IEnumerable<object[]> BuildDocbuilderReport(TaskFilter filter)
        {
            if (filter.TagId != 0 && filter.ProjectIds.Count == 0)
            {
                filter.ProjectIds = EngineFactory.GetTagEngine().GetTagProjects(filter.TagId).ToList();
            }

            var result = EngineFactory.GetReportEngine().BuildUsersWorkload(filter);

            result = result.OrderBy(r => (string)r[0]).ToList();

            return result;
        }

        public override string[] ColumnsName
        {
            get
            {
                return new[]
                {
                    ReportResource.CsvColumnUserName,
                    ResourceEnumConverter.ConvertToString(TaskStatus.Open),
                    ResourceEnumConverter.ConvertToString(TaskStatus.Closed),
                    ProjectsCommonResource.Total
                };
            }
        }

        public override ReportInfo ReportInfo
        {
            get
            {
                var userColumns = new[]
                {
                    ReportResource.User,
                    "Not Accept",
                    ResourceEnumConverter.ConvertToString(TaskStatus.Open),
                    ReportResource.ActiveTasks,
                    ResourceEnumConverter.ConvertToString(TaskStatus.Closed),
                    ProjectsCommonResource.Total,
                    ReportResource.ClickToSortByThisColumn,
                    CommonLinkUtility.ServerRootPath
                };

                return new ReportInfo(
                    String.Format(ReportResource.ReportEmployment_Description, "<ul>", "</ul>", "<li>", "</li>"),
                    ReportResource.ReportEmployment_Title,
                    userColumns);
            }
        }

        public override string ReportFileName
        {
            get { return ReportResource.ReportEmployment_Title; }
        }
    }

    [Transient]
    class TimeSpendReport : ExtendedReportType
    {
        public TimeSpendReport(CommonLinkUtility commonLinkUtility,
            EngineFactory engineFactory,
            UserManager userManager,
            DisplayUserSettingsHelper displayUserSettingsHelper,
            TenantUtil tenantUtil,
            CustomNamingPeople customNamingPeople,
            FilterHelper filterHelper)
            : base(commonLinkUtility,
                  engineFactory,
                  userManager,
                  displayUserSettingsHelper,
                  tenantUtil,
                  customNamingPeople)
        {

            FilerHelper = filterHelper;
            Init(ReportType.TimeSpend);
        }

        public override IEnumerable<object[]> BuildDocbuilderReport(TaskFilter filter)
        {
            filter.FromDate = FilerHelper.GetFromDate(filter, true);
            filter.ToDate = FilerHelper.GetToDate(filter,true);

            IEnumerable<object[]> taskTime = new List<object[]>();
            switch (filter.ViewType)
            {
                case 0:
                    taskTime = EngineFactory.GetTimeTrackingEngine().GetByFilter(filter)
                        .Select(r =>
                            new object[]
                            {
                                UserManager.GetUsers(r.Person).DisplayUserName(false, DisplayUserSettingsHelper),
                                r.Hours, 0, r.PaymentStatus, r.PaymentStatus == PaymentStatus.NotChargeable ? "+" : ""
                            });

                    taskTime = taskTime.GroupBy(r => (string)r[0], (a, b) =>
                        {
                            var enumerable = b as IList<object[]> ?? b.ToList();
                            var data = (object[])enumerable.First().Clone();
                            data[1] = enumerable.Sum(c => (float)c[1]);
                            data[2] = enumerable.Where(r => (PaymentStatus)r[3] == PaymentStatus.Billed).Sum(c => (float)c[1]);
                            return data;
                        });
                    return taskTime.OrderBy(r => (string)r[0]);

                case 1:
                    taskTime = EngineFactory.GetTimeTrackingEngine().GetByFilter(filter)
                        .Select(r =>
                            new object[]
                            {
                                UserManager.GetUsers(r.Person).DisplayUserName(false, DisplayUserSettingsHelper),
                                r.Hours, 0, r.PaymentStatus, r.PaymentStatus == PaymentStatus.NotChargeable ? "+" : "",
                                r.Task.Project, r.Task.Title
                            });

                    taskTime = taskTime.Select(r =>
                    {
                        if ((PaymentStatus)r[3] == PaymentStatus.Billed) r[2] = r[1];
                        return r;
                    });

                    var users = taskTime.GroupBy(x => x[0]).Select(x => new object[] { x.Key, x.ToList()
                        .Select(r => new object[] { r[1], r[2], r[3], r[4], r[5], r[6] })});


                    var result = new List<object[]>();
                    foreach (var user in users) // user = [string, []]
                    {
                        var userTasks = (IEnumerable<object[]>)user[1];

                        var groupedUserTasks = userTasks.GroupBy(x => x[4]).Select(r => new object[] {
                        new object[] { ((Project)r.Key).Title, ProjectsCommonResource.Status + ": " + LocalizedEnumConverter.ConvertToString(((Project)r.Key).Status),
                        ProjectResource.ProjectLeader + ": " + UserManager.GetUsers(((Project)r.Key).Responsible).DisplayUserName(false, DisplayUserSettingsHelper),
                        TaskResource.CreatingDate + ": " + ((Project)r.Key).CreateOn.ToString("d"),
                        ((Project)r.Key).Description != "" ? ProjectsCommonResource.Description + ": " + ((Project)r.Key).Description : ""
                        }, r.ToList().Select(y => new object[] { y[0], y[1], y[2], y[3], y[5] } )});

                        result.Add(new object[] { user[0], groupedUserTasks });
                    }

                    return result;

                case 2:
                    taskTime = EngineFactory.GetTimeTrackingEngine().GetByFilter(filter)
                        .Select(r =>
                            new object[]
                            {
                                UserManager.GetUsers(r.Person).DisplayUserName(false, DisplayUserSettingsHelper),
                                r.Hours, 0, r.PaymentStatus, r.PaymentStatus == PaymentStatus.NotChargeable ? "+" : "",
                                r.Task.Project.Title
                            });

                    return taskTime.GroupBy(x => (string)x[5]).Select(x => new object[] {x.Key, x.GroupBy(r => (string)r[0], (a, b) =>
                        {
                            var enumerable = b as IList<object[]> ?? b.ToList();
                            var data = (object[])enumerable.First().Clone();
                            data[1] = enumerable.Sum(c => (float)c[1]);
                            data[2] = enumerable.Where(r => (PaymentStatus)r[3] == PaymentStatus.Billed).Sum(c => (float)c[1]);
                            return data;
                        })
                    }).OrderBy(x => (string)x[0]);

                default:
                    throw new Exception(ProjectsCommonResource.NoData);
            }
        }

        public override string[] ColumnsName
        {
            get
            {
                return new[]
                {
                    ReportResource.CsvColumnUserName,
                    TaskResource.Task,
                    ProjectsCommonResource.Total,
                    ProjectsCommonResource.SpentTotally,
                    ReportResource.Billed,
                    TimeTrackingResource.PaymentStatusNotChargeable,
                    TimeTrackingResource.ShortHours,
                    TimeTrackingResource.ShortMinutes
                };
            }
        }

        public override ReportInfo ReportInfo
        {
            get
            {
                return new ReportInfo(
                    String.Format(ReportResource.ReportTimeSpendSummary_Description, "<ul>", "</ul>", "<li>", "</li>"),
                    ReportResource.ReportTimeSpendSummary_Title,
                    new[]
                    {
                        ReportResource.User, ProjectsCommonResource.SpentTotally,
                        ProjectsCommonResource.SpentBilledTotally,
                        ReportResource.ClickToSortByThisColumn, CommonLinkUtility.ServerRootPath, VirtualRoot,
                        TimeTrackingResource.ShortHours, TimeTrackingResource.ShortMinutes
                    });
            }
        }

        public override string ReportFileName
        {
            get { return ReportResource.ReportTimeSpendSummary_Title; }
        }
    }

    [Transient]
    class UsersActivityReport : ExtendedReportType
    {
        public UsersActivityReport(CommonLinkUtility commonLinkUtility,
            EngineFactory engineFactory,
            UserManager userManager,
            DisplayUserSettingsHelper displayUserSettingsHelper,
            TenantUtil tenantUtil,
            CustomNamingPeople customNamingPeople)
            : base(commonLinkUtility,
                  engineFactory,
                  userManager,
                  displayUserSettingsHelper,
                  tenantUtil,
                  customNamingPeople)
        {
            Init(ReportType.UsersActivity);
        }

        public override IEnumerable<object[]> BuildDocbuilderReport(TaskFilter filter)
        {
            var result = EngineFactory.GetReportEngine().BuildUsersActivity(filter);
            return result
                .OrderBy(r => (string)r[1])
                .ToList();
        }

        public override string[] ColumnsName
        {
            get
            {
                return new[]
                {
                    ReportResource.CsvColumnUserName,
                    TaskResource.Tasks,
                    MilestoneResource.Milestones,
                    MessageResource.Messages,
                    ProjectsCommonResource.Total
                };
            }
        }

        public override ReportInfo ReportInfo
        {
            get
            {
                return new ReportInfo(
                    String.Format(ReportResource.ReportUserActivity_Descripton, "<ul>", "</ul>", "<li>", "</li>"),
                    ReportResource.ReportUserActivity_Title,
                    new[]
                    {
                        ReportResource.User, TaskResource.Tasks, MilestoneResource.Milestones, MessageResource.Messages,
                        ProjectsFileResource.Files, ProjectsCommonResource.Total, ReportResource.ClickToSortByThisColumn,
                        CommonLinkUtility.ServerRootPath
                    });
            }
        }

        public override string ReportFileName
        {
            get { return ReportResource.ReportUserActivity_Title; }
        }
    }

    [Transient]
    class EmptyReport : ExtendedReportType
    {
        public EmptyReport(CommonLinkUtility commonLinkUtility,
            EngineFactory engineFactory,
            UserManager userManager,
            DisplayUserSettingsHelper displayUserSettingsHelper,
            TenantUtil tenantUtil,
            CustomNamingPeople customNamingPeople)
            : base(commonLinkUtility,
                  engineFactory,
                  userManager,
                  displayUserSettingsHelper,
                  tenantUtil,
                  customNamingPeople)
        {
            Init(ReportType.EmptyReport);
        }

        public override IEnumerable<object[]> BuildDocbuilderReport(TaskFilter filter)
        {
            return new List<object[]>();
        }

        public override string[] ColumnsName
        {
            get { return new[] { ProjectsCommonResource.NoData }; }
        }

        public override ReportInfo ReportInfo
        {
            get { return new ReportInfo("", "", new[] { ProjectsCommonResource.NoData }); }
        }

        public override string ReportFileName
        {
            get { return "EmptyReport"; }
        }
    }
}