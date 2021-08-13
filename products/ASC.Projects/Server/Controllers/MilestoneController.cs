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
using ASC.Web.Core.Utility;
using ASC.Web.Files.Services.DocumentService;
using ASC.Web.Studio.Core.Notify;
using ASC.Web.Studio.Utility;

using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;

namespace ASC.Api.Projects
{
    public class MilestoneController : BaseProjectController
    {
        public MilestoneController(SecurityContext securityContext, ProjectSecurity projectSecurity, ApiContext context, EngineFactory engineFactory, TenantUtil tenantUtil, DisplayUserSettingsHelper displayUserSettingsHelper, CommonLinkUtility commonLinkUtility, UserPhotoManager userPhotoManager, MessageService messageService, MessageTarget messageTarget, ModelHelper modelHelper, FileWrapperHelper fileWrapperHelper, IOptionsMonitor<ILog> options, DaoFactory factory, ReportHelper reportHelper, DocbuilderReportsUtility docbuilderReportsUtility, IHttpContextAccessor httpContextAccessor, TenantManager tenantManager, ReportTemplateHelper reportTemplateHelper, FilesLinkUtility filesLinkUtility, CustomStatusHelper customStatusHelper, IServiceProvider serviceProvider, SettingsManager settingsManager, HtmlUtility htmlUtility, NotifyConfiguration notifyConfiguration) : base(securityContext, projectSecurity, context, engineFactory, tenantUtil, displayUserSettingsHelper, commonLinkUtility, userPhotoManager, messageService, messageTarget, modelHelper, fileWrapperHelper, options, factory, reportHelper, docbuilderReportsUtility, httpContextAccessor, tenantManager, reportTemplateHelper, filesLinkUtility, customStatusHelper, serviceProvider, settingsManager, htmlUtility, notifyConfiguration)
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
        public MilestoneWrapper UpdateMilestone(int id, ModelUpdateStatus model)
        {
            var milestone = EngineFactory.GetMilestoneEngine().GetByID(id).NotFoundIfNull();

            EngineFactory.GetMilestoneEngine().ChangeStatus(milestone, model.Status);
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