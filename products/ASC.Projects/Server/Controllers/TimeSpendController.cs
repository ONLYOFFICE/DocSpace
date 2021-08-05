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
using ASC.Projects.Model.TimeSpends;
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
    public class TimeSpendController : BaseProjectController
    {
        public TimeSpendController(SecurityContext securityContext, ProjectSecurity projectSecurity, ApiContext context, EngineFactory engineFactory, TenantUtil tenantUtil, DisplayUserSettingsHelper displayUserSettingsHelper, CommonLinkUtility commonLinkUtility, UserPhotoManager userPhotoManager, MessageService messageService, MessageTarget messageTarget, ModelHelper modelHelper, FileWrapperHelper fileWrapperHelper, IOptionsMonitor<ILog> options, DaoFactory factory, ReportHelper reportHelper, DocbuilderReportsUtility docbuilderReportsUtility, IHttpContextAccessor httpContextAccessor, TenantManager tenantManager, ReportTemplateHelper reportTemplateHelper, FilesLinkUtility filesLinkUtility, CustomStatusHelper customStatusHelper, IServiceProvider serviceProvider, SettingsManager settingsManager, HtmlUtility htmlUtility, NotifyConfiguration notifyConfiguration) : base(securityContext, projectSecurity, context, engineFactory, tenantUtil, displayUserSettingsHelper, commonLinkUtility, userPhotoManager, messageService, messageTarget, modelHelper, fileWrapperHelper, options, factory, reportHelper, docbuilderReportsUtility, httpContextAccessor, tenantManager, reportTemplateHelper, filesLinkUtility, customStatusHelper, serviceProvider, settingsManager, htmlUtility, notifyConfiguration)
        {
        }

        [Read(@"time/filter")]
        public IEnumerable<TimeWrapper> GetTaskTimeByFilter(ModelTimeByFilter model)
        {
            var filter = CreateFilter(EntityType.TimeSpend);
            filter.DepartmentId = model.Departament;
            filter.UserId = model.Participant;
            filter.FromDate = model.CreatedStart;
            filter.ToDate = model.CreatedStop;
            filter.TagId = model.Tag;
            filter.LastId = model.LastId;
            filter.MyProjects = model.MyProjects;
            filter.MyMilestones = model.MyMilestones;
            filter.Milestone = model.Milestone;

            if (model.Projectid != 0)
                filter.ProjectIds.Add(model.Projectid);

            if (model.Status.HasValue)
                filter.PaymentStatuses.Add(model.Status.Value);

            Context.SetTotalCount(EngineFactory.GetTimeTrackingEngine().GetByFilterCount(filter));

            return EngineFactory.GetTimeTrackingEngine().GetByFilter(filter).NotFoundIfNull().Select(t=> ModelHelper.GetTimeWrapper(t));
        }

        [Read(@"time/filter/total")]
        public float GetTotalTaskTimeByFilter(ModelTimeByFilter model)
        {
            var filter = CreateFilter(EntityType.TimeSpend);
            filter.DepartmentId = model.Departament;
            filter.UserId = model.Participant;
            filter.FromDate = model.CreatedStart;
            filter.ToDate = model.CreatedStop;
            filter.TagId = model.Tag;
            filter.LastId = model.LastId;
            filter.MyProjects = model.MyProjects;
            filter.MyMilestones = model.MyMilestones;
            filter.Milestone = model.Milestone;

            if (model.Projectid != 0)
                filter.ProjectIds.Add(model.Projectid);

            if (model.Status.HasValue)
                filter.PaymentStatuses.Add(model.Status.Value);

            return EngineFactory.GetTimeTrackingEngine().GetByFilterTotal(filter);
        }

        [Read(@"task/{taskid:int}/time")]
        public IEnumerable<TimeWrapper> GetTaskTime(int taskid)
        {
            if (!EngineFactory.GetTaskEngine().IsExists(taskid)) throw new ItemNotFoundException();
            var times = EngineFactory.GetTimeTrackingEngine().GetByTask(taskid).NotFoundIfNull();
            Context.SetTotalCount(times.Count);
            return times.Select(t=> ModelHelper.GetTimeWrapper(t));
        }
        
        [Create(@"task/{taskid:int}/time")]
        public TimeWrapper AddTaskTime(int taskid, ModelAddTime model)
        {
            if (model.Date == DateTime.MinValue) throw new ArgumentException("date can't be empty");
            if (model.PersonId == Guid.Empty) throw new ArgumentException("person can't be empty");

            var task = EngineFactory.GetTaskEngine().GetByID(taskid);

            if (task == null) throw new ItemNotFoundException();

            if (!EngineFactory.GetProjectEngine().IsExists(model.ProjectId)) throw new ItemNotFoundException("project");

            var ts = new TimeSpend
            {
                Date = model.Date.Date,
                Person = model.PersonId,
                Hours = model.Hours,
                Note = model.Note,
                Task = task,
                CreateBy = SecurityContext.CurrentAccount.ID
            };

            ts = EngineFactory.GetTimeTrackingEngine().SaveOrUpdate(ts);
            MessageService.Send(MessageAction.TaskTimeCreated, MessageTarget.Create(ts.ID), task.Project.Title, task.Title, ts.Note);

            return ModelHelper.GetTimeWrapper(ts);
        }

        [Update(@"time/{timeid:int}")]
        public TimeWrapper UpdateTime(int timeid, ModelUpdateTime model)
        {
            if (model.Date == DateTime.MinValue) throw new ArgumentException("date can't be empty");
            if (model.PersonId == Guid.Empty) throw new ArgumentException("person can't be empty");


            var time = EngineFactory.GetTimeTrackingEngine().GetByID(timeid).NotFoundIfNull();

            time.Date = model.Date.Date;
            time.Person = model.PersonId;
            time.Hours = model.Hours;
            time.Note = model.Note;

            EngineFactory.GetTimeTrackingEngine().SaveOrUpdate(time);
            MessageService.Send(MessageAction.TaskTimeUpdated, MessageTarget.Create(time.ID), time.Task.Project.Title, time.Task.Title, time.Note);

            return ModelHelper.GetTimeWrapper(time);
        }

        [Update(@"time/times/status")]
        public List<TimeWrapper> UpdateTimes(ModelUpdateStatus model)
        {
            var times = new List<TimeWrapper>();

            foreach (var timeid in model.TimeIds)
            {
                var time = EngineFactory.GetTimeTrackingEngine().GetByID(timeid).NotFoundIfNull();
                EngineFactory.GetTimeTrackingEngine().ChangePaymentStatus(time, model.Status);
                times.Add(ModelHelper.GetTimeWrapper(time));
            }

            MessageService.Send(MessageAction.TaskTimesUpdatedStatus, MessageTarget.Create(model.TimeIds), times.Select(t => t.RelatedTaskTitle), LocalizedEnumConverter.ConvertToString(model.Status));

            return times;
        }

        [Delete(@"time/times/remove")]
        public List<TimeWrapper> DeleteTaskTimes(int[] timeids)
        {
            var listDeletedTimers = new List<TimeWrapper>();
            foreach (var timeid in timeids.Distinct())
            {
                var time = EngineFactory.GetTimeTrackingEngine().GetByID(timeid).NotFoundIfNull();

                EngineFactory.GetTimeTrackingEngine().Delete(time);
                listDeletedTimers.Add(ModelHelper.GetTimeWrapper(time));
            }

            MessageService.Send(MessageAction.TaskTimesDeleted, MessageTarget.Create(timeids), listDeletedTimers.Select(t => t.RelatedTaskTitle));

            return listDeletedTimers;
        }
    }
}