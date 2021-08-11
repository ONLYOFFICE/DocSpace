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
using ASC.Projects.Model.Tasks;
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
    public class TasksController :BaseProjectController
    {
        public TasksController(SecurityContext securityContext, ProjectSecurity projectSecurity, ApiContext context, EngineFactory engineFactory, TenantUtil tenantUtil, DisplayUserSettingsHelper displayUserSettingsHelper, CommonLinkUtility commonLinkUtility, UserPhotoManager userPhotoManager, MessageService messageService, MessageTarget messageTarget, ModelHelper modelHelper, FileWrapperHelper fileWrapperHelper, IOptionsMonitor<ILog> options, DaoFactory factory, ReportHelper reportHelper, DocbuilderReportsUtility docbuilderReportsUtility, IHttpContextAccessor httpContextAccessor, TenantManager tenantManager, ReportTemplateHelper reportTemplateHelper, FilesLinkUtility filesLinkUtility, CustomStatusHelper customStatusHelper, IServiceProvider serviceProvider, SettingsManager settingsManager, HtmlUtility htmlUtility, NotifyConfiguration notifyConfiguration) : base(securityContext, projectSecurity, context, engineFactory, tenantUtil, displayUserSettingsHelper, commonLinkUtility, userPhotoManager, messageService, messageTarget, modelHelper, fileWrapperHelper, options, factory, reportHelper, docbuilderReportsUtility, httpContextAccessor, tenantManager, reportTemplateHelper, filesLinkUtility, customStatusHelper, serviceProvider, settingsManager, htmlUtility, notifyConfiguration)
        {
        }

        [Read(@"task/@self")]
        public IEnumerable<TaskWrapper> GetMyTasks()
        {
            return EngineFactory.GetTaskEngine()
                .GetByResponsible(SecurityContext.CurrentAccount.ID)
                .Select(t=> ModelHelper.GetTaskWrapper(t))
                .ToList();
        }

        [Read(@"task/@self/{status:regex(notaccept|open|closed|disable|unclassified|notinmilestone)}")]
        public IEnumerable<TaskWrapper> GetMyTasks(TaskStatus status)
        {
            return EngineFactory.GetTaskEngine().GetByResponsible(SecurityContext.CurrentAccount.ID, status)
                .Select(t => ModelHelper.GetTaskWrapper(t))
                .ToList();
        }

        [Read(@"task/{taskid:int}")]
        public TaskWrapperFull GetTask(int taskid)
        {
            var task = EngineFactory.GetTaskEngine().GetByID(taskid).NotFoundIfNull();
            var commentsCount = EngineFactory.GetCommentEngine().Count(task);
            var isSubscribed = EngineFactory.GetTaskEngine().IsSubscribed(task);
            var milestone = EngineFactory.GetMilestoneEngine().GetByID(task.Milestone, false);
            var timeSpend = EngineFactory.GetTimeTrackingEngine().GetByTask(task.ID).Sum(r => r.Hours);
            var project = ModelHelper.GetProjectWrapperFull(task.Project, EngineFactory.GetFileEngine().GetRoot(task.Project.ID));
            var files = EngineFactory.GetTaskEngine().GetFiles(task).Select(f=>FileWrapperHelper.GetFileWrapper(f));
            var comments = EngineFactory.GetCommentEngine().GetComments(task);
            var filteredComments = comments.Where(r => r.Parent.Equals(Guid.Empty)).Select(x => ModelHelper.GetCommentInfo(comments, x, task)).ToList();
            return ModelHelper.GetTaskWrapperFull(task, milestone, project, files, filteredComments, commentsCount, isSubscribed, timeSpend);
        }

        [Read(@"task")]
        public IEnumerable<TaskWrapper> GetTask(ModelGetTasks model)
        {
            var tasks = EngineFactory.GetTaskEngine().GetByID(model.TaskId.ToList()).NotFoundIfNull();
            return tasks.Select(x=> ModelHelper.GetTaskWrapper(x)).ToList();
        }

        [Read(@"task/filter")]
        public IEnumerable<TaskWrapper> GetTaskByFilter(ModelTaskByfilter model)
        {
            var filter = CreateFilter(EntityType.Task);
            filter.DepartmentId = model.Departament;
            filter.ParticipantId = model.Participant;
            filter.UserId = model.Creator;
            filter.Milestone = model.Nomilestone ? 0 : model.Milestone;
            filter.FromDate = model.DeadlineStart;
            filter.ToDate = model.DeadlineStop;
            filter.TagId = model.Tag;
            filter.LastId = model.LastId;
            filter.MyProjects = model.MyProjects;
            filter.MyMilestones = model.MyMilestones;
            filter.Follow = model.Follow;
            filter.Substatus = model.Substatus;

            if (model.Projectid != 0)
                filter.ProjectIds.Add(model.Projectid);

            if (model.Status != null)
                filter.TaskStatuses.Add((TaskStatus)model.Status);

            var filterResult = EngineFactory.GetTaskEngine().GetByFilter(filter).NotFoundIfNull();

            Context.SetTotalCount(filterResult.FilterCount.TasksTotal);

            ProjectSecurity.GetTaskSecurityInfo(filterResult.FilterResult);

            return filterResult.FilterResult.Select(x=> ModelHelper.GetTaskWrapper(x)).ToList();
        }

        
        [Read(@"task/filter/simple")]
        public IEnumerable<SimpleTaskWrapper> GetSimpleTaskByFilter(ModelTaskSimpleByFilter model)
        {
            var filter = CreateFilter(EntityType.Task);
            filter.DepartmentId = model.Departament;
            filter.ParticipantId = model.Participant;
            filter.UserId = model.Creator;
            filter.Milestone = model.Milestone;
            filter.FromDate = model.DeadlineStart;
            filter.ToDate = model.DeadlineStop;
            filter.TagId = model.Tag;
            filter.LastId = model.LastId;
            filter.MyProjects = model.MyProjects;
            filter.MyMilestones = model.MyMilestones;
            filter.Follow = model.Follow;

            if (model.ProjectId != 0)
                filter.ProjectIds.Add(model.ProjectId);

            if (model.Status != null)
                filter.TaskStatuses.Add((TaskStatus)model.Status);

            var filterResult = EngineFactory.GetTaskEngine().GetByFilter(filter).NotFoundIfNull();

            Context.SetTotalCount(filterResult.FilterCount.TasksTotal);

            return filterResult.FilterResult.Select(r => ModelHelper.GetSimpleTaskWrapper(r));
        }

        [Update(@"task/{taskid:int}/status")]
        public TaskWrapperFull UpdateTask(int taskid, ModelUpdateTask model)
        {
            var task = EngineFactory.GetTaskEngine().GetByID(taskid).NotFoundIfNull();

            var customStatus = EngineFactory.GetStatusEngine().GetWithDefaults().FirstOrDefault(r => r.Id == model.StatusId) ??
                                        CustomStatusHelper.GetDefaults().First(r => r.StatusType == model.Status);

            EngineFactory.GetTaskEngine().ChangeStatus(task, customStatus);
            MessageService.Send(MessageAction.TaskUpdatedStatus, MessageTarget.Create(task.ID), task.Project.Title, task.Title, LocalizedEnumConverter.ConvertToString(task.Status));

            return GetTask(taskid);
        }

        [Update(@"task/status")]
        public IEnumerable<TaskWrapperFull> UpdateTasks(ModelUpdateTasks model)
        {
            var result = new List<TaskWrapperFull>(model.TaskIds.Length);

            foreach (var taskId in model.TaskIds)
            {
                try
                {
                    var modelUpdateTask = new ModelUpdateTask()
                    {
                        Status = model.Status,
                        StatusId = model.StatusId
                    };
                    result.Add(UpdateTask(taskId, modelUpdateTask));
                }
                catch (Exception e)
                {
                    Log.Error("UpdateTasks", e);
                }
            }

            return result;
        }

        [Update(@"task/{taskid:int}/milestone")]
        public TaskWrapperFull UpdateTask(int taskid, ModelUpdateMilestone model)
        {
            if (model.MilestoneId < 0) throw new ArgumentNullException("milestoneid");

            var task = EngineFactory.GetTaskEngine().GetByID(taskid).NotFoundIfNull();

            var milestone = EngineFactory.GetMilestoneEngine().GetByID(model.MilestoneId);

            EngineFactory.GetTaskEngine().MoveToMilestone(task, model.MilestoneId);
            if (milestone != null)
            {
                MessageService.Send(MessageAction.TaskMovedToMilestone, MessageTarget.Create(task.ID), task.Project.Title, milestone.Title, task.Title);
            }
            else
            {
                MessageService.Send(MessageAction.TaskUnlinkedMilestone, MessageTarget.Create(task.ID), task.Project.Title, task.Title);
            }

            return GetTask(taskid);
        }

        [Update(@"task/milestone")]
        public IEnumerable<TaskWrapperFull> UpdateTasks(ModelUpdateTasksMilestone model)
        {
            if (model.MilestoneId < 0) throw new ArgumentNullException("milestoneid");

            var result = new List<TaskWrapperFull>(model.TaskIds.Length);

            foreach (var taskid in model.TaskIds)
            {
                try
                {
                    result.Add(UpdateTask(taskid, new ModelUpdateMilestone() { MilestoneId = model.MilestoneId}));
                }
                catch (Exception)
                {

                }
            }

            return result;
        }

        [Create(@"task/{copyFrom:int}/copy")]
        public TaskWrapper CopyTask(int copyFrom, ModelCopyTask model)
        {
            if (string.IsNullOrEmpty(model.Title)) throw new ArgumentException(@"title can't be empty", "title");

            var copyFromTask = EngineFactory.GetTaskEngine().GetByID(copyFrom).NotFoundIfNull();
            var project = EngineFactory.GetProjectEngine().GetByID(model.ProjectId).NotFoundIfNull();

            if (!EngineFactory.GetMilestoneEngine().IsExists(model.Milestoneid) && model.Milestoneid > 0)
            {
                throw new ItemNotFoundException("Milestone not found");
            }

            var team = EngineFactory.GetProjectEngine().GetTeam(project.ID);
            var teamIds = team.Select(r => r.ID).ToList();

            if (model.Responsibles.Any(responsible => !teamIds.Contains(responsible)))
            {
                throw new ArgumentException(@"responsibles", "responsibles");
            }

            var task = new Task
            {
                CreateBy = SecurityContext.CurrentAccount.ID,
                CreateOn = TenantUtil.DateTimeNow(),
                Deadline = model.Deadline,
                Description = model.Description ?? "",
                Priority = model.Priority,
                Status = TaskStatus.Open,
                Title = model.Title,
                Project = project,
                Milestone = model.Milestoneid,
                Responsibles = new List<Guid>(model.Responsibles.Distinct()),
                StartDate = model.StartDate
            };

            EngineFactory.GetTaskEngine().SaveOrUpdate(task, null, model.Notify);

            if (model.CopySubtasks)
            {
                EngineFactory.GetTaskEngine().CopySubtasks(copyFromTask, task, team);
            }

            if (model.CopyFiles)
            {
                EngineFactory.GetTaskEngine().CopyFiles(copyFromTask, task);
            }

            if (model.RemoveOld)
            {
                EngineFactory.GetTaskEngine().Delete(copyFromTask);
            }

            MessageService.Send(MessageAction.TaskCreated, MessageTarget.Create(task.ID), project.Title, task.Title);

            return GetTask(task);
        }

        [Update(@"task/{taskid:int}")]
        public TaskWrapperFull UpdateProjectTask(int taskid, ModelUpdateProjectTask model)
        {
            var task = EngineFactory.GetTaskEngine().GetByID(taskid).NotFoundIfNull();

            if (string.IsNullOrEmpty(model.Title)) throw new ArgumentException(@"title can't be empty", "title");

            if (!EngineFactory.GetMilestoneEngine().IsExists(model.Milestoneid) && model.Milestoneid > 0)
            {
                throw new ItemNotFoundException("Milestone not found");
            }

            var distinctResponsibles = new List<Guid>(model.Responsibles.Distinct());

            var hasChanges = !(task.Responsibles.Count == distinctResponsibles.Count && task.Responsibles.All(distinctResponsibles.Contains));

            task.Responsibles = distinctResponsibles;

            task.Deadline = Update.IfNotEquals(TenantUtil.DateTimeToUtc(task.Deadline), model.Deadline);
            task.Description = Update.IfNotEquals(task.Description, model.Description);

            if (model.Priority.HasValue)
            {
                task.Priority = Update.IfNotEquals(task.Priority, model.Priority.Value);
            }

            task.Title = Update.IfNotEmptyAndNotEquals(task.Title, model.Title);
            task.Milestone = Update.IfNotEquals(task.Milestone, model.Milestoneid);
            task.StartDate = Update.IfNotEquals(TenantUtil.DateTimeToUtc(task.StartDate), model.StartDate);

            if (model.ProjectID.HasValue)
            {
                if (task.Project.ID != model.ProjectID.Value)
                {
                    var project = EngineFactory.GetProjectEngine().GetByID(model.ProjectID.Value).NotFoundIfNull();
                    task.Project = project;
                    hasChanges = true;
                }
            }

            if (model.Progress.HasValue)
            {
                task.Progress = Update.IfNotEquals(task.Progress, model.Progress.Value);
            }

            if (hasChanges)
            {
                EngineFactory.GetTaskEngine().SaveOrUpdate(task, null, model.Notify);
            }

            if (model.Status.HasValue)
            {
                var newStatus = CustomStatusHelper.GetDefaults().First(r => r.StatusType == model.Status.Value);

                if (task.Status != newStatus.StatusType || task.CustomTaskStatus != newStatus.Id)
                {
                    hasChanges = true;
                    EngineFactory.GetTaskEngine().ChangeStatus(task, newStatus);
                }
            }

            if (hasChanges)
            {
                MessageService.Send(MessageAction.TaskUpdated, MessageTarget.Create(task.ID), task.Project.Title, task.Title);
            }

            return GetTask(taskid);
        }

        [Delete(@"task/{taskid:int}")]
        public TaskWrapper DeleteTask(int taskid)
        {
            var task = EngineFactory.GetTaskEngine().GetByID(taskid).NotFoundIfNull();

            EngineFactory.GetTaskEngine().Delete(task);
            MessageService.Send(MessageAction.TaskDeleted, MessageTarget.Create(task.ID), task.Project.Title, task.Title);

            return ModelHelper.GetTaskWrapper(task);
        }


        [Delete(@"task")]
        public IEnumerable<TaskWrapper> DeleteTasks(ModelDeleteTask model)
        {
            var result = new List<TaskWrapper>(model.TaskIds.Length);

            foreach (var taskId in model.TaskIds)
            {
                try
                {
                    result.Add(DeleteTask(taskId));
                }
                catch (Exception)
                {

                }
            }

            return result;
        }

        [Read(@"task/{taskid:int}/comment")]
        public IEnumerable<CommentWrapper> GetTaskComments(int taskid)
        {
            var task = EngineFactory.GetTaskEngine().GetByID(taskid).NotFoundIfNull();
            return EngineFactory.GetCommentEngine().GetComments(task).Select(x =>ModelHelper.GetCommentWrapper(x, task));
        }


        [Create(@"task/{taskid:int}/comment")]
        public CommentWrapper AddTaskComments(int taskid, ModelAddTaskComments model)
        {
            if (string.IsNullOrEmpty(model.Content)) throw new ArgumentException(@"Comment text is empty", model.Content);
            if (model.ParentId != Guid.Empty && EngineFactory.GetCommentEngine().GetByID(model.ParentId) == null) throw new ItemNotFoundException("parent comment not found");

            var comment = new Comment
            {
                Content = model.Content,
                TargetUniqID = ProjectEntity.BuildUniqId<Task>(taskid),
                CreateBy = SecurityContext.CurrentAccount.ID,
                CreateOn = TenantUtil.DateTimeNow()
            };

            if (model.ParentId != Guid.Empty)
            {
                comment.Parent = model.ParentId;
            }

            var task = EngineFactory.GetCommentEngine().GetEntityByTargetUniqId(comment).NotFoundIfNull();

            EngineFactory.GetCommentEngine().SaveOrUpdateComment(task, comment);

            MessageService.Send(MessageAction.TaskCommentCreated, MessageTarget.Create(comment.ID), task.Project.Title, task.Title);

            return ModelHelper.GetCommentWrapper(comment, task);
        }
        
        [Read(@"task/{taskid:int}/notify")]
        public TaskWrapper NotifyTaskResponsible(int taskid)
        {
            var task = EngineFactory.GetTaskEngine().GetByID(taskid).NotFoundIfNull();

            EngineFactory.GetTaskEngine().SendReminder(task);

            return ModelHelper.GetTaskWrapper(task);
        }

        [Update(@"task/{taskid:int}/subscribe")]
        public TaskWrapper SubscribeToTask(int taskid)
        {
            var task = EngineFactory.GetTaskEngine().GetByID(taskid).NotFoundIfNull();

            ProjectSecurity.DemandAuthentication();

            EngineFactory.GetTaskEngine().Follow(task);
            MessageService.Send(MessageAction.TaskUpdatedFollowing, MessageTarget.Create(task.ID), task.Project.Title, task.Title);

            return ModelHelper.GetTaskWrapper(task);
        }

        [Read(@"task/{taskid:int}/subscribe")]
        public bool IsSubscribeToTask(int taskid)
        {
            var task = EngineFactory.GetTaskEngine().GetByID(taskid).NotFoundIfNull();

            ProjectSecurity.DemandAuthentication();

            return EngineFactory.GetTaskEngine().IsSubscribed(task);
        }

        [Create(@"task/{parentTaskId:int}/link")]
        public TaskWrapper AddLink(int parentTaskId, ModelAddLink model)
        {
            var dependentTask = EngineFactory.GetTaskEngine().GetByID(model.DependenceTaskId).NotFoundIfNull();
            var parentTask = EngineFactory.GetTaskEngine().GetByID(parentTaskId).NotFoundIfNull();

            EngineFactory.GetTaskEngine().AddLink(parentTask, dependentTask, model.LinkType);
            MessageService.Send(MessageAction.TasksLinked, MessageTarget.Create(new[] { parentTask.ID, dependentTask.ID }), parentTask.Project.Title, parentTask.Title, dependentTask.Title);

            return ModelHelper.GetTaskWrapper(dependentTask);
        }

        [Delete(@"task/{taskid:int}/link")]
        public TaskWrapper RemoveLink(int dependenceTaskId, int parentTaskId)//todo
        {
            var dependentTask = EngineFactory.GetTaskEngine().GetByID(dependenceTaskId).NotFoundIfNull();
            var parentTask = EngineFactory.GetTaskEngine().GetByID(parentTaskId).NotFoundIfNull();

            EngineFactory.GetTaskEngine().RemoveLink(dependentTask, parentTask);
            MessageService.Send(MessageAction.TasksUnlinked, MessageTarget.Create(new[] { parentTask.ID, dependentTask.ID }), parentTask.Project.Title, parentTask.Title, dependentTask.Title);

            return ModelHelper.GetTaskWrapper(dependentTask);
        }

        [Create(@"task/{taskid:int}")]
        public SubtaskWrapper AddSubtask(int taskid, ModelAddSubtask model)
        {
            if (string.IsNullOrEmpty(model.Title)) throw new ArgumentException(@"title can't be empty", "title");
            var task = EngineFactory.GetTaskEngine().GetByID(taskid).NotFoundIfNull();

            if (task.Status == TaskStatus.Closed) throw new ArgumentException(@"task can't be closed");

            var subtask = new Subtask
            {
                Responsible = model.Responsible,
                Task = task.ID,
                Status = TaskStatus.Open,
                Title = model.Title
            };

            subtask = EngineFactory.GetSubtaskEngine().SaveOrUpdate(subtask, task);
            MessageService.Send(MessageAction.SubtaskCreated, MessageTarget.Create(subtask.ID), task.Project.Title, task.Title, subtask.Title);

            return ModelHelper.GetSubtaskWrapper(subtask, task);
        }

        [Create(@"task/{taskid:int}/{subtaskid:int}/copy")]
        public SubtaskWrapper CopySubtask(int taskid, int subtaskid)
        {
            var task = EngineFactory.GetTaskEngine().GetByID(taskid).NotFoundIfNull();

            var subtask = EngineFactory.GetSubtaskEngine().GetById(subtaskid).NotFoundIfNull();

            var team = EngineFactory.GetProjectEngine().GetTeam(task.Project.ID);

            var newSubtask = EngineFactory.GetSubtaskEngine().Copy(subtask, task, team);

            return ModelHelper.GetSubtaskWrapper(newSubtask, task);
        }

        [Update(@"task/{taskid:int}/{subtaskid:int}")]
        public SubtaskWrapper UpdateSubtask(int taskid, int subtaskid, ModelUpdateSubtask model)
        {
            if (string.IsNullOrEmpty(model.Title)) throw new ArgumentException(@"title can't be empty", "title");
            var task = EngineFactory.GetTaskEngine().GetByID(taskid).NotFoundIfNull();
            var subtask = task.SubTasks.Find(r => r.ID == subtaskid).NotFoundIfNull();

            var hasChanges = subtask.Responsible == model.Responsible && subtask.Title == model.Title ? false : true;

            subtask.Responsible = Update.IfNotEquals(subtask.Responsible, model.Responsible);
            subtask.Title = Update.IfNotEmptyAndNotEquals(subtask.Title, model.Title);

            

            if (hasChanges)
            {
                EngineFactory.GetSubtaskEngine().SaveOrUpdate(subtask, task);
                MessageService.Send(MessageAction.SubtaskUpdated, MessageTarget.Create(subtask.ID), task.Project.Title, task.Title, subtask.Title);
            }

            return ModelHelper.GetSubtaskWrapper(subtask, task);
        }

        [Delete(@"task/{taskid:int}/{subtaskid:int}")]
        public SubtaskWrapper DeleteSubtask(int taskid, int subtaskid)
        {
            var task = EngineFactory.GetTaskEngine().GetByID(taskid).NotFoundIfNull();
            var subtask = task.SubTasks.Find(r => r.ID == subtaskid).NotFoundIfNull();

            EngineFactory.GetSubtaskEngine().Delete(subtask, task);
            MessageService.Send(MessageAction.SubtaskDeleted, MessageTarget.Create(subtask.ID), task.Project.Title, task.Title, subtask.Title);

            return ModelHelper.GetSubtaskWrapper(subtask, task);
        }
        
        [Update(@"task/{taskid:int}/{subtaskid:int}/status")]
        public SubtaskWrapper UpdateSubtask(int taskid, int subtaskid, ModelStatus model)
        {
            var task = EngineFactory.GetTaskEngine().GetByID(taskid).NotFoundIfNull();
            var subtask = task.SubTasks.Find(r => r.ID == subtaskid).NotFoundIfNull();
            ProjectSecurity.DemandEdit(task, subtask);

            EngineFactory.GetSubtaskEngine().ChangeStatus(task, subtask, model.Status);
            MessageService.Send(MessageAction.SubtaskUpdatedStatus, MessageTarget.Create(subtask.ID), task.Project.Title, task.Title, subtask.Title, LocalizedEnumConverter.ConvertToString(subtask.Status));

            return ModelHelper.GetSubtaskWrapper(subtask, task);
        }
    }
}