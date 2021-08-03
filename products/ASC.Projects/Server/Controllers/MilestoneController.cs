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
using ASC.Api.Documents;
using ASC.Api.Projects.Wrappers;
using ASC.Api.Utils;
using ASC.Common.Logging;
using ASC.Common.Web;
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
using ASC.Projects.Model.Milestones;
using ASC.Web.Api.Routing;
using ASC.Web.Core.Files;
using ASC.Web.Core.Users;
using ASC.Web.Files.Services.DocumentService;
using ASC.Web.Studio.Utility;

using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;

namespace ASC.Api.Projects
{
    public class MilestoneController : BaseProjectController
    {
        public MilestoneController(SecurityContext securityContext, ProjectSecurity projectSecurity, ApiContext context, EngineFactory engineFactory, TenantUtil tenantUtil, DisplayUserSettingsHelper displayUserSettingsHelper, CommonLinkUtility commonLinkUtility, UserPhotoManager userPhotoManager, MessageService messageService, MessageTarget messageTarget, ModelHelper modelHelper, FileWrapperHelper fileWrapperHelper, IOptionsMonitor<ILog> options, DaoFactory factory, ReportHelper reportHelper, DocbuilderReportsUtility docbuilderReportsUtility, IHttpContextAccessor httpContextAccessor, TenantManager tenantManager, ReportTemplateHelper reportTemplateHelper, FilesLinkUtility filesLinkUtility, CustomStatusHelper customStatusHelper, IServiceProvider serviceProvider, SettingsManager settingsManager) : base(securityContext, projectSecurity, context, engineFactory, tenantUtil, displayUserSettingsHelper, commonLinkUtility, userPhotoManager, messageService, messageTarget, modelHelper, fileWrapperHelper, options, factory, reportHelper, docbuilderReportsUtility, httpContextAccessor, tenantManager, reportTemplateHelper, filesLinkUtility, customStatusHelper, serviceProvider, settingsManager)
        {
        }

        [Read(@"milestone")]
        public IEnumerable<MilestoneWrapper> GetMilestones()
        {
            return EngineFactory.GetMilestoneEngine().GetUpcomingMilestones((int)Context.Count).Select(m=> ModelHelper.GetMilestoneWrapper(m)).ToList();
        }

        [Read(@"milestone/filter")]
        public IEnumerable<MilestoneWrapper> GetMilestonesByFilter(ModelMilestoneByFilter model)
        {
            var filter = CreateFilter(EntityType.Milestone);
            filter.UserId = model.MilestoneResponsible;
            filter.ParticipantId = model.TaskResponsible;
            filter.TagId = model.Tag;
            filter.FromDate = model.DeadlineStart;
            filter.ToDate = model.DeadlineStop;
            filter.LastId = model.LastId;
            filter.MyProjects = model.MyProjects;

            if (model.ProjectId != 0)
            {
                filter.ProjectIds.Add(model.ProjectId);
            }

            if (model.Status != null)
            {
                filter.MilestoneStatuses.Add((MilestoneStatus)model.Status);
            }

            Context.SetTotalCount(EngineFactory.GetMilestoneEngine().GetByFilterCount(filter));

            return EngineFactory.GetMilestoneEngine().GetByFilter(filter).NotFoundIfNull().Select(m=> ModelHelper.GetMilestoneWrapper(m)).ToList();
        }

        [Read(@"milestone/late")]
        public IEnumerable<MilestoneWrapper> GetLateMilestones()
        {
            return EngineFactory.GetMilestoneEngine().GetLateMilestones((int)Context.Count).Select(m=> ModelHelper.GetMilestoneWrapper(m)).ToList();
        }

        [Read(@"milestone/{year}/{month}/{day}")]
        public IEnumerable<MilestoneWrapper> GetMilestonesByDeadLineFull(int year, int month, int day)
        {
            var milestones = EngineFactory.GetMilestoneEngine().GetByDeadLine(new DateTime(year, month, day));
            return milestones.Select(m => ModelHelper.GetMilestoneWrapper(m)).ToList();
        }

        [Read(@"milestone/{year}/{month}")]
        public IEnumerable<MilestoneWrapper> GetMilestonesByDeadLineMonth(int year, int month)
        {
            var milestones = EngineFactory.GetMilestoneEngine().GetByDeadLine(new DateTime(year, month, DateTime.DaysInMonth(year, month)));
            return milestones.Select(m => ModelHelper.GetMilestoneWrapper(m)).ToList();
        }

        [Read(@"milestone/{id:int}")]
        public MilestoneWrapper GetMilestoneById(int id)
        {
            if (!EngineFactory.GetMilestoneEngine().IsExists(id)) throw new ItemNotFoundException();
            return ModelHelper.GetMilestoneWrapper(EngineFactory.GetMilestoneEngine().GetByID(id));
        }

        [Read(@"milestone/{id:int}/task")]
        public IEnumerable<TaskWrapper> GetMilestoneTasks(int id)
        {
            if (!EngineFactory.GetMilestoneEngine().IsExists(id)) throw new ItemNotFoundException();
            return EngineFactory.GetTaskEngine().GetMilestoneTasks(id).Select(t=> ModelHelper.GetTaskWrapper(t)).ToList();
        }

        [Update(@"milestone/{id:int}")]
        public MilestoneWrapper UpdateMilestone(int id, ModelMilestoneUpdate model)
        {
            var milestone = EngineFactory.GetMilestoneEngine().GetByID(id).NotFoundIfNull();
            ProjectSecurity.DemandEdit(milestone);

            milestone.Description = Update.IfNotEmptyAndNotEquals(milestone.Description, model.Description);
            milestone.Title = Update.IfNotEmptyAndNotEquals(milestone.Title, model.Title);
            milestone.DeadLine = Update.IfNotEmptyAndNotEquals(milestone.DeadLine, model.Deadline);
            milestone.Responsible = Update.IfNotEmptyAndNotEquals(milestone.Responsible, model.Responsible);

            if (model.IsKey.HasValue)
                milestone.IsKey = model.IsKey.Value;

            if (model.IsNotify.HasValue)
                milestone.IsNotify = model.IsNotify.Value;

            if (model.ProjectID != 0)
            {
                var project = EngineFactory.GetProjectEngine().GetByID(model.ProjectID).NotFoundIfNull();
                milestone.Project = project;
            }

            EngineFactory.GetMilestoneEngine().SaveOrUpdate(milestone, model.NotifyResponsible);
            MessageService.Send(MessageAction.MilestoneUpdated, MessageTarget.Create(milestone.ID), milestone.Project.Title, milestone.Title);

            return ModelHelper.GetMilestoneWrapper(milestone);
        }

        [Update(@"milestone/{id:int}/status")]
        public MilestoneWrapper UpdateMilestone(int id, MilestoneStatus status)
        {
            var milestone = EngineFactory.GetMilestoneEngine().GetByID(id).NotFoundIfNull();

            EngineFactory.GetMilestoneEngine().ChangeStatus(milestone, status);
            MessageService.Send(MessageAction.MilestoneUpdatedStatus, MessageTarget.Create(milestone.ID), milestone.Project.Title, milestone.Title, LocalizedEnumConverter.ConvertToString(milestone.Status));

            return ModelHelper.GetMilestoneWrapper(milestone);
        }

        [Delete(@"milestone/{id:int}")]
        public MilestoneWrapper DeleteMilestone(int id)
        {
            var milestone = EngineFactory.GetMilestoneEngine().GetByID(id).NotFoundIfNull();

            EngineFactory.GetMilestoneEngine().Delete(milestone);
            MessageService.Send(MessageAction.MilestoneDeleted, MessageTarget.Create(milestone.ID), milestone.Project.Title, milestone.Title);

            return ModelHelper.GetMilestoneWrapper(milestone);
        }

        [Delete(@"milestone")]
        public IEnumerable<MilestoneWrapper> DeleteMilestones(int[] ids)
        {
            var result = new List<MilestoneWrapper>(ids.Length);

            foreach (var id in ids)
            {
                try
                {
                    result.Add(DeleteMilestone(id));
                }
                catch (Exception)
                {

                }
            }

            return result;
        }
    }
}