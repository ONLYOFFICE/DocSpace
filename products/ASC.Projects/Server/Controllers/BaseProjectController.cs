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

using ASC.Api.Core;
using ASC.Api.Core.Convention;
using ASC.Api.Documents;
using ASC.Api.Utils;
using ASC.Common;
using ASC.Common.Logging;
using ASC.Core;
using ASC.Core.Tenants;
using ASC.MessagingSystem;
using ASC.Projects.Calendars;
using ASC.Projects.Core.Domain;
using ASC.Projects.Core.Domain.Reports;
using ASC.Projects.Engine;
using ASC.Web.Api.Routing;
using ASC.Web.Core.Calendars;
using ASC.Web.Core.Files;
using ASC.Web.Core.Users;
using ASC.Web.Files.Services.DocumentService;
using ASC.Web.Studio.Utility;

using Autofac;

using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.DependencyInjection;
using ASC.Api.Projects.Wrappers;
using ASC.Core.Common.Settings;
using ASC.Projects;
using ASC.Projects.Classes;
using ASC.Projects.Model;
using ASC.Web.Core.Utility;
using ASC.Web.Studio.Core.Notify;

namespace ASC.Api.Projects
{
    [Scope]
    [DefaultRoute]
    [ApiController]
    [ControllerName("project")]
    public abstract class BaseProjectController : ControllerBase
    {
        protected ProjectSecurity ProjectSecurity { get; private set; }
        protected SecurityContext SecurityContext { get; set; }
        protected ApiContext Context { get; set; }
        protected TenantUtil TenantUtil { get; set; }
        protected DisplayUserSettingsHelper DisplayUserSettingsHelper { get; set; }
        protected CommonLinkUtility CommonLinkUtility { get; set; }
        protected UserPhotoManager UserPhotoManager { get; set; }
        protected MessageService MessageService { get; set; }
        protected MessageTarget MessageTarget { get; set; }
        protected ModelHelper ModelHelper { get; set; }
        protected ILog Log { get; set; }
        protected ASC.CRM.Core.Dao.DaoFactory CRMDaoFactory { get; set; }
        protected ReportHelper ReportHelper { get; set; }
        protected DocbuilderReportsUtility DocbuilderReportsUtility { get; set; }
        protected IHttpContextAccessor HttpContextAccessor { get; set; }
        protected TenantManager TenantManager { get; set; }
        protected ReportTemplateHelper ReportTemplateHelper { get; set; }
        protected FilesLinkUtility FilesLinkUtility { get; set; }
        protected CustomStatusHelper CustomStatusHelper { get; set; }
        protected IServiceProvider ServiceProvider { get; set; }
        protected SettingsManager SettingsManager { get; set; }
        protected FileWrapperHelper FileWrapperHelper { get; set; }
        protected EngineFactory EngineFactory { get; set; }
        protected HtmlUtility HtmlUtility { get; set; }
        public BaseProjectController(SecurityContext securityContext,
            ProjectSecurity projectSecurity,
            ApiContext context,
            EngineFactory engineFactory,
            TenantUtil tenantUtil,
            DisplayUserSettingsHelper displayUserSettingsHelper,
            CommonLinkUtility commonLinkUtility,
            UserPhotoManager userPhotoManager,
            MessageService messageService,
            MessageTarget messageTarget,
            ModelHelper modelHelper, 
            FileWrapperHelper fileWrapperHelper,
            IOptionsMonitor<ILog> options,
            ASC.CRM.Core.Dao.DaoFactory factory,
            ReportHelper reportHelper,
            DocbuilderReportsUtility docbuilderReportsUtility,
            IHttpContextAccessor httpContextAccessor,
            TenantManager tenantManager, 
            ReportTemplateHelper reportTemplateHelper, 
            FilesLinkUtility filesLinkUtility, 
            CustomStatusHelper customStatusHelper,
            IServiceProvider serviceProvider, 
            SettingsManager settingsManager,
            HtmlUtility htmlUtility,
            NotifyConfiguration notifyConfiguration)
        {
            SecurityContext = securityContext;
            ProjectSecurity = projectSecurity;
            Context = context;
            TenantUtil = tenantUtil;
            DisplayUserSettingsHelper = displayUserSettingsHelper;
            CommonLinkUtility = commonLinkUtility;
            UserPhotoManager = userPhotoManager;
            MessageService = messageService;
            MessageTarget = messageTarget;
            ModelHelper = modelHelper;
            FileWrapperHelper = fileWrapperHelper;
            Log = options.CurrentValue;
            CRMDaoFactory = factory;
            ReportHelper = reportHelper;
            DocbuilderReportsUtility = docbuilderReportsUtility;
            HttpContextAccessor = httpContextAccessor;
            TenantManager = tenantManager;
            ReportTemplateHelper = reportTemplateHelper;
            FilesLinkUtility = filesLinkUtility;
            CustomStatusHelper = customStatusHelper;
            ServiceProvider = serviceProvider;
            SettingsManager = settingsManager;
            EngineFactory = engineFactory;
            HtmlUtility = htmlUtility;
            notifyConfiguration.Configure();
        }

        public List<BaseCalendar> GetUserCalendars(Guid userId)
        {
            var cals = new List<BaseCalendar>();
            var projects = EngineFactory.GetProjectEngine().GetByParticipant(userId);

            if (projects != null)
            {
                var team = EngineFactory.GetProjectEngine().GetTeam(projects.Select(r => r.ID).ToList());

                foreach (var project in projects)
                {
                    var p = project;

                    var sharingOptions = new SharingOptions();
                    foreach (var participant in team.Where(r => r.ProjectID == p.ID))
                    {
                        sharingOptions.PublicItems.Add(new SharingOptions.PublicItem
                        {
                            Id = participant.ID,
                            IsGroup = false
                        });
                    }

                    var index = project.ID % CalendarColors.BaseColors.Count;
                    var calenar = ServiceProvider.GetService<ProjectCalendar>();
                    
                    cals.Add(calenar.Init(
                        project,
                        CalendarColors.BaseColors[index].BackgroudColor,
                        CalendarColors.BaseColors[index].TextColor,
                        sharingOptions, false));
                }
            }

            var folowingProjects = EngineFactory.GetProjectEngine().GetFollowing(userId);
            if (folowingProjects != null)
            {
                var team = EngineFactory.GetProjectEngine().GetTeam(folowingProjects.Select(r => r.ID).ToList());

                foreach (var project in folowingProjects)
                {
                    var p = project;

                    if (projects != null && projects.Any(proj => proj.ID == p.ID)) continue;

                    var sharingOptions = new SharingOptions();
                    sharingOptions.PublicItems.Add(new SharingOptions.PublicItem { Id = userId, IsGroup = false });
                    foreach (var participant in team.Where(r => r.ProjectID == p.ID))
                    {
                        sharingOptions.PublicItems.Add(new SharingOptions.PublicItem
                        {
                            Id = participant.ID,
                            IsGroup = false
                        });
                    }

                    var index = p.ID % CalendarColors.BaseColors.Count;
                    var calenar = ServiceProvider.GetService<ProjectCalendar>();
                    cals.Add(calenar.Init(
                        p,
                        CalendarColors.BaseColors[index].BackgroudColor,
                        CalendarColors.BaseColors[index].TextColor,
                        sharingOptions, true));
                }
            }

            return cals;
        }

        public TaskFilter CreateFilter(EntityType entityType)
        {
            var filter = new TaskFilter
            {
                SortOrder = !Context.SortDescending,
                SearchText = Context.FilterValue,
                Offset = Context.StartIndex,
                Max = Context.Count
            };

            if (!string.IsNullOrEmpty(Context.SortBy))
            {
                var type = entityType.ToString();
                var sortColumns = filter.SortColumns.ContainsKey(type) ? filter.SortColumns[type] : null;
                if (sortColumns != null && sortColumns.Any())
                    filter.SortBy = sortColumns.ContainsKey(Context.SortBy) ? Context.SortBy : sortColumns.First().Key;
            }

            Context.SetDataFiltered().SetDataPaginated().SetDataSorted();

            return filter;
        }

        [Read(@"message/{messageid:int}/files")]
        public IEnumerable<FileWrapper<int>> GetMessageFiles(int messageid)
        {
            var message = EngineFactory.GetMessageEngine().GetByID(messageid).NotFoundIfNull();

            ProjectSecurity.DemandReadFiles(message.Project);

            return EngineFactory.GetMessageEngine().GetFiles(message).Select(f => FileWrapperHelper.GetFileWrapper(f));
        }

        [Read(@"task/{taskid:int}/files")]
        public IEnumerable<FileWrapper<int>> GetTaskFiles(int taskid)
        {
            var task = EngineFactory.GetTaskEngine().GetByID(taskid).NotFoundIfNull();

            ProjectSecurity.DemandReadFiles(task.Project);

            return EngineFactory.GetTaskEngine().GetFiles(task).Select(f=> FileWrapperHelper.GetFileWrapper(f));
        }

        [Delete(@"task/{taskid:int}/files")]
        public TaskWrapper DetachFileFromTask(int taskid, int fileid)
        {

            var task = EngineFactory.GetTaskEngine().GetByID(taskid).NotFoundIfNull();

            ProjectSecurity.DemandReadFiles(task.Project);

            var file = EngineFactory.GetFileEngine().GetFile(fileid).NotFoundIfNull();
            EngineFactory.GetTaskEngine().DetachFile(task, fileid);
            MessageService.Send(MessageAction.TaskDetachedFile, MessageTarget.Create(task.ID), task.Project.Title, task.Title, file.Title);

            return ModelHelper.GetTaskWrapper(task);
        }

        [Delete(@"task/{taskid:int}/filesmany")]
        public TaskWrapper DetachFileFromTask(int taskid, List<int> files)
        {

            var task = EngineFactory.GetTaskEngine().GetByID(taskid).NotFoundIfNull();

            ProjectSecurity.DemandReadFiles(task.Project);

            var filesList = files.ToList();
            var attachments = new List<Files.Core.File<int>>();
            foreach (var fileid in filesList)
            {
                var file = EngineFactory.GetFileEngine().GetFile(fileid).NotFoundIfNull();
                attachments.Add(file);
                EngineFactory.GetTaskEngine().AttachFile(task, file.ID, true);
            }

            MessageService.Send(MessageAction.TaskDetachedFile, MessageTarget.Create(task.ID), task.Project.Title, task.Title, attachments.Select(x => x.Title));

            return ModelHelper.GetTaskWrapper(task);
        }

        [Create(@"task/{taskid:int}/files")]
        public TaskWrapper UploadFilesToTask(int taskid, IEnumerable<int> files)
        {
            var task = EngineFactory.GetTaskEngine().GetByID(taskid).NotFoundIfNull();

            ProjectSecurity.DemandReadFiles(task.Project);

            var filesList = files.ToList();
            var attachments = new List<Files.Core.File<int>>();
            foreach (var fileid in filesList)
            {
                var file = EngineFactory.GetFileEngine().GetFile(fileid).NotFoundIfNull();
                attachments.Add(file);
                EngineFactory.GetTaskEngine().AttachFile(task, file.ID, true);
            }

            MessageService.Send(MessageAction.TaskAttachedFiles, MessageTarget.Create(task.ID), task.Project.Title, task.Title, attachments.Select(x => x.Title));

            return ModelHelper.GetTaskWrapper(task);
        }

        [Create(@"message/{messageid:int}/files")]
        public MessageWrapper UploadFilesToMessage(int messageid, IEnumerable<int> files)
        {
            var discussion = EngineFactory.GetMessageEngine().GetByID(messageid).NotFoundIfNull();
            ProjectSecurity.DemandReadFiles(discussion.Project);

            var filesList = files.ToList();
            var attachments = new List<Files.Core.File<int>>();
            foreach (var fileid in filesList)
            {
                var file = EngineFactory.GetFileEngine().GetFile(fileid).NotFoundIfNull();
                attachments.Add(file);
                EngineFactory.GetMessageEngine().AttachFile(discussion, file.ID, true);
            }

            MessageService.Send(MessageAction.DiscussionAttachedFiles, MessageTarget.Create(discussion.ID), discussion.Project.Title, discussion.Title, attachments.Select(x => x.Title));

            return ModelHelper.GetMessageWrapper(discussion);
        }

        [Delete(@"message/{messageid:int}/files")]
        public MessageWrapper DetachFileFromMessage(int messageid, int fileid)
        {
            var discussion = EngineFactory.GetMessageEngine().GetByID(messageid).NotFoundIfNull();
            ProjectSecurity.DemandReadFiles(discussion.Project);

            var file = EngineFactory.GetFileEngine().GetFile(fileid).NotFoundIfNull();

            EngineFactory.GetMessageEngine().DetachFile(discussion, fileid);
            MessageService.Send(MessageAction.DiscussionDetachedFile, MessageTarget.Create(discussion.ID), discussion.Project.Title, discussion.Title, file.Title);

            return ModelHelper.GetMessageWrapper(discussion);
        }

        [Delete(@"message/{messageid:int}/filesmany")]
        public MessageWrapper DetachFileFromMessage(int messageid, IEnumerable<int> files)
        {
            var discussion = EngineFactory.GetMessageEngine().GetByID(messageid).NotFoundIfNull();
            ProjectSecurity.DemandReadFiles(discussion.Project);

            var filesList = files.ToList();
            var attachments = new List<Files.Core.File<int>>();
            foreach (var fileid in filesList)
            {
                var file = EngineFactory.GetFileEngine().GetFile(fileid).NotFoundIfNull();
                attachments.Add(file);
                EngineFactory.GetMessageEngine().DetachFile(discussion, fileid);
            }

            MessageService.Send(MessageAction.DiscussionDetachedFile, MessageTarget.Create(discussion.ID), discussion.Project.Title, discussion.Title, attachments.Select(x => x.Title));

            return ModelHelper.GetMessageWrapper(discussion);
        }

        internal TaskWrapper GetTask(Task task)
        {
            if (task.Milestone == 0) return ModelHelper.GetTaskWrapper(task);

            var milestone = EngineFactory.GetMilestoneEngine().GetByID(task.Milestone, false);
            return ModelHelper.GetTaskWrapper(task, milestone);
        }
    }
}