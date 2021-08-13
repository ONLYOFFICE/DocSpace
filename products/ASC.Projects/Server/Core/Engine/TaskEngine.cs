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
 * Pursuant to Section 7 � 3(b) of the GNU GPL you must retain the original ONLYOFFICE logo which contains 
 * relevant author attributions when distributing the software. If the display of the logo in its graphic 
 * form is not reasonably feasible for technical reasons, you must include the words "Powered by ONLYOFFICE" 
 * in every copy of the program you distribute. 
 * Pursuant to Section 7 � 3(e) we decline to grant you any rights under trademark law for use of our trademarks.
 *
*/

using System;
using System.Collections.Generic;
using System.Linq;

using ASC.Common;
using ASC.Core;
using ASC.Core.Tenants;
using ASC.ElasticSearch;
using ASC.Projects.Core.DataInterfaces;
using ASC.Projects.Core.Domain;
using ASC.Projects.Core.Engine;
using ASC.Projects.Core.Services.NotifyService;
using ASC.Projects.EF;


namespace ASC.Projects.Engine
{
    [Scope]
    public class TaskEngine : ProjectEntityEngine
    {
        private TenantUtil TenantUtil { get; set; }
        private FactoryIndexer<DbTask> FactoryIndexer { get; set; }
        private IDaoFactory DaoProjectFactory { get; set; }
        private readonly Func<Task, bool> canReadDelegate;

        public TaskEngine(FactoryIndexer<DbTask> factoryIndexer, TenantUtil tenantUtil, SecurityContext securityContext, Files.Core.IDaoFactory daoFactory, NotifySource notifySource, IDaoFactory daoProjectFactory, EngineFactory engineFactory, NotifyClient notifyClient, ProjectSecurity projectSecurity)
            : base(securityContext, daoFactory, notifySource, engineFactory, projectSecurity, notifyClient)
        {
            canReadDelegate = CanRead;
            FactoryIndexer = factoryIndexer;
            TenantUtil = tenantUtil;
            DaoProjectFactory = daoProjectFactory;
            Init(NotifyConstants.Event_NewCommentForTask);
        }

        #region Get Tasks

        public IEnumerable<Task> GetAll()
        {
            return DaoProjectFactory.GetTaskDao().GetAll().Where(canReadDelegate);
        }

        public IEnumerable<Task> GetByProject(int projectId, TaskStatus? status, Guid participant)
        {
            var listTask = DaoProjectFactory.GetTaskDao().GetByProject(projectId, status, participant).Where(canReadDelegate).ToList();
            DaoProjectFactory.GetSubtaskDao().GetSubtasksForTasks(ref listTask);
            return listTask;
        }

        public TaskFilterOperationResult GetByFilter(TaskFilter filter)
        {
            if (filter.Offset < 0 || filter.Max < 0)
                return null;

            var isAdmin = ProjectSecurity.IsAdministrator(SecurityContext.CurrentAccount.ID);
            var anyOne = ProjectSecurity.IsPrivateDisabled;
            var count = DaoProjectFactory.GetTaskDao().GetByFilterCount(filter, isAdmin, anyOne);

            var filterLimit = filter.Max;
            var filterOffset = filter.Offset;

            if (filterOffset > count.TasksTotal)
                return new TaskFilterOperationResult(count); //there are some records but we cant see them due to offset

            var taskList = new List<Task>();
            if (filter.HasTaskStatuses)
            {
                taskList = DaoProjectFactory.GetTaskDao().GetByFilter(filter, isAdmin, anyOne);
            }
            else if (filterOffset > count.TasksOpen && count.TasksClosed != 0)
            {
                filter.TaskStatuses.Add(TaskStatus.Closed);
                filter.SortBy = "status_changed";
                filter.SortOrder = false;
                filter.Offset = filterOffset - count.TasksOpen;
                taskList = DaoProjectFactory.GetTaskDao().GetByFilter(filter, isAdmin, anyOne);
            }
            else
            {
                //TODO: to one sql query using UNION ALL
                if (count.TasksOpen != 0)
                {
                    filter.TaskStatuses.Add(TaskStatus.Open);
                    taskList = DaoProjectFactory.GetTaskDao().GetByFilter(filter, isAdmin, anyOne);
                }

                if (taskList.Count < filterLimit && count.TasksClosed != 0)
                {
                    filter.TaskStatuses.Clear();
                    filter.TaskStatuses.Add(TaskStatus.Closed);
                    //filter.SortBy = "status_changed";
                    //filter.SortOrder = false;
                    filter.Offset = 0;
                    filter.Max = filterLimit - taskList.Count;
                    taskList.AddRange(DaoProjectFactory.GetTaskDao().GetByFilter(filter, isAdmin, anyOne));
                }
            }

            filter.Offset = filterOffset;
            filter.Max = filterLimit;
            filter.TaskStatuses.Clear();

            DaoProjectFactory.GetSubtaskDao().GetSubtasksForTasks(ref taskList);

            var taskLinks = DaoProjectFactory.GetTaskDao().GetLinks(taskList);

            Func<Task, int> idSelector = task => task.ID;
            Func<Task, IEnumerable<TaskLink>, Task> resultSelector = (task, linksCol) =>
                                                          {
                                                              task.Links.AddRange(linksCol);
                                                              return task;
                                                          };

            taskList = taskList.GroupJoin(taskLinks, idSelector, link => link.DependenceTaskId, resultSelector).ToList();
            taskList = taskList.GroupJoin(taskLinks, idSelector, link => link.ParentTaskId, resultSelector).ToList();

            return new TaskFilterOperationResult(taskList, count);
        }

        public TaskFilterCountOperationResult GetByFilterCount(TaskFilter filter)
        {
            return DaoProjectFactory.GetTaskDao().GetByFilterCount(filter, ProjectSecurity.CurrentUserAdministrator, ProjectSecurity.IsPrivateDisabled);
        }

        public List<Tuple<Guid, int, int>> GetByFilterCountForReport(TaskFilter filter)
        {
            return DaoProjectFactory.GetTaskDao().GetByFilterCountForReport(filter, ProjectSecurity.CurrentUserAdministrator, ProjectSecurity.IsPrivateDisabled);
        }

        public IEnumerable<TaskFilterCountOperationResult> GetByFilterCountForStatistic(TaskFilter filter)
        {
            return DaoProjectFactory.GetTaskDao().GetByFilterCountForStatistic(filter, ProjectSecurity.CurrentUserAdministrator, ProjectSecurity.IsPrivateDisabled);
        }

        public IEnumerable<Task> GetByResponsible(Guid responsibleId, params TaskStatus[] statuses)
        {
            var listTask = DaoProjectFactory.GetTaskDao().GetByResponsible(responsibleId, statuses).Where(canReadDelegate).ToList();
            DaoProjectFactory.GetSubtaskDao().GetSubtasksForTasks(ref listTask);
            return listTask;
        }

        public IEnumerable<Task> GetMilestoneTasks(int milestoneId)
        {
            var listTask = DaoProjectFactory.GetTaskDao().GetMilestoneTasks(milestoneId).Where(canReadDelegate).ToList();
            DaoProjectFactory.GetSubtaskDao().GetSubtasksForTasks(ref listTask);
            return listTask;
        }

        public override ProjectEntity GetEntityByID(int id)
        {
            return GetByID(id);
        }

        public Task GetByID(int id)
        {
            return GetByID(id, true);
        }

        public Task GetByID(int id, bool checkSecurity)
        {
            var task = DaoProjectFactory.GetTaskDao().GetById(id);

            if (task != null)
            {
                task.SubTasks = DaoProjectFactory.GetSubtaskDao().GetSubtasks(task.ID);
                task.Links = DaoProjectFactory.GetTaskDao().GetLinks(task.ID).ToList();
            }

            if (!checkSecurity)
                return task;

            return CanRead(task) ? task : null;
        }

        public IEnumerable<Task> GetByID(ICollection<int> ids)
        {
            var listTask = DaoProjectFactory.GetTaskDao().GetById(ids).Where(canReadDelegate).ToList();
            DaoProjectFactory.GetSubtaskDao().GetSubtasksForTasks(ref listTask);
            return listTask;

        }

        public bool IsExists(int id)
        {
            return DaoProjectFactory.GetTaskDao().IsExists(id);
        }

        private bool CanRead(Task task)
        {
            return ProjectSecurity.CanRead(task);
        }

        #endregion

        #region Save, Delete, Notify

        public Task SaveOrUpdate(Task task, IEnumerable<int> attachedFileIds, bool notifyResponsible, bool isImport = false)
        {
            if (task == null) throw new ArgumentNullException("task");
            if (task.Project == null) throw new Exception("task.Project");

            var milestone = task.Milestone != 0 ? DaoProjectFactory.GetMilestoneDao().GetById(task.Milestone) : null;
            var milestoneResponsible = milestone != null ? milestone.Responsible : Guid.Empty;

            var removeResponsibles = new List<Guid>();
            var inviteToResponsibles = new List<Guid>();

            task.Responsibles.RemoveAll(r => r.Equals(Guid.Empty));

            if (task.Deadline.Kind != DateTimeKind.Local && task.Deadline != DateTime.MinValue)
                task.Deadline = TenantUtil.DateTimeFromUtc(task.Deadline);

            if (task.StartDate.Kind != DateTimeKind.Local && task.StartDate != DateTime.MinValue)
                task.StartDate = TenantUtil.DateTimeFromUtc(task.StartDate);

            var isNew = task.ID == default(int); //Task is new

            if (isNew)
            {
                foreach (var responsible in task.Responsibles)
                {
                    if (ProjectSecurity.IsVisitor(responsible))
                    {
                        ProjectSecurity.CreateGuestSecurityException();
                    }

                    if (!ProjectSecurity.IsInTeam(task.Project, responsible))
                    {
                        ProjectSecurity.CreateSecurityException();
                    }
                }

                if (task.CreateBy == default(Guid)) task.CreateBy = SecurityContext.CurrentAccount.ID;
                if (task.CreateOn == default(DateTime)) task.CreateOn = TenantUtil.DateTimeNow();

                ProjectSecurity.DemandCreate<Task>(task.Project);

                task = DaoProjectFactory.GetTaskDao().Create(task);

                inviteToResponsibles.AddRange(task.Responsibles.Distinct());
            }
            else
            {
                var oldTask = DaoProjectFactory.GetTaskDao().GetById(new[] { task.ID }).FirstOrDefault();

                if (oldTask == null) throw new ArgumentNullException("task");
                ProjectSecurity.DemandEdit(oldTask);

                var newResponsibles = task.Responsibles.Distinct().ToList();
                var oldResponsibles = oldTask.Responsibles.Distinct().ToList();

                foreach (var responsible in newResponsibles.Except(oldResponsibles))
                {
                    if (ProjectSecurity.IsVisitor(responsible))
                    {
                        ProjectSecurity.CreateGuestSecurityException();
                    }

                    if (!ProjectSecurity.IsInTeam(task.Project, responsible))
                    {
                        ProjectSecurity.CreateSecurityException();
                    }
                }

                removeResponsibles.AddRange(oldResponsibles.Where(p => !newResponsibles.Contains(p)));
                inviteToResponsibles.AddRange(newResponsibles.Where(participant => !oldResponsibles.Contains(participant)));

                task.LastModifiedBy = SecurityContext.CurrentAccount.ID;
                task.LastModifiedOn = TenantUtil.DateTimeNow();

                task = DaoProjectFactory.GetTaskDao().Update(task);
            }

            _ = FactoryIndexer.IndexAsync(DaoProjectFactory.GetTaskDao().ToDbTask(task));

            if (attachedFileIds != null && attachedFileIds.Any())
            {
                foreach (var attachedFileId in attachedFileIds)
                {
                    AttachFile(task, attachedFileId);
                }
            }

            var senders = new HashSet<Guid>(task.Responsibles) { task.Project.Responsible, milestoneResponsible, task.CreateBy };
            senders.Remove(Guid.Empty);

            foreach (var subscriber in senders)
            {
                Subscribe(task, subscriber);
            }

            inviteToResponsibles.RemoveAll(r => r.Equals(Guid.Empty));
            removeResponsibles.RemoveAll(r => r.Equals(Guid.Empty));

            NotifyTask(task, inviteToResponsibles, removeResponsibles, isNew, notifyResponsible);

            return task;
        }

        public Task ChangeStatus(Task task, CustomTaskStatus newStatus)
        {
            ProjectSecurity.DemandEdit(task);

            if (task == null) throw new ArgumentNullException("task");
            if (task.Project == null) throw new Exception("Project can't be null.");
            if (task.Project.Status == ProjectStatus.Closed) throw new Exception(EngineResource.ProjectClosedError);

            if (task.Status == newStatus.StatusType && task.CustomTaskStatus == newStatus.Id) return task;

            var status = EngineFactory.GetStatusEngine().Get().FirstOrDefault(r => r.Id == newStatus.Id);
            var cannotChange =
                status != null &&
                status.Available.HasValue && !status.Available.Value &&
                task.CreateBy != SecurityContext.CurrentAccount.ID &&
                task.Project.Responsible != SecurityContext.CurrentAccount.ID &&
                !ProjectSecurity.CurrentUserAdministrator;

            if (cannotChange)
            {
                ProjectSecurity.CreateSecurityException();
            }


            var senders = GetSubscribers(task);

            if (newStatus.StatusType == TaskStatus.Closed && !DisableNotifications && senders.Count != 0)
                NotifyClient.SendAboutTaskClosing(senders, task);

            if (newStatus.StatusType == TaskStatus.Open && !DisableNotifications && senders.Count != 0)
                NotifyClient.SendAboutTaskResumed(senders, task);

            task.Status = newStatus.StatusType;
            task.CustomTaskStatus = newStatus.Id < 0 ? null : (int?)newStatus.Id;
            task.LastModifiedBy = SecurityContext.CurrentAccount.ID;
            task.LastModifiedOn = TenantUtil.DateTimeNow();
            task.StatusChangedOn = TenantUtil.DateTimeNow();

            //subtask
            if (newStatus.StatusType == TaskStatus.Closed)
            {
                if (!task.Responsibles.Any())
                    task.Responsibles.Add(SecurityContext.CurrentAccount.ID);

                DaoProjectFactory.GetSubtaskDao().CloseAllSubtasks(task);
                foreach (var subTask in task.SubTasks)
                {
                    subTask.Status = TaskStatus.Closed;
                }
            }

            return DaoProjectFactory.GetTaskDao().Update(task);
        }

        public Task CopySubtasks(Task @from, Task to, IEnumerable<Participant> team)
        {
            if (from.Status == TaskStatus.Closed) return to;

            var subTasks = DaoProjectFactory.GetSubtaskDao().GetSubtasks(@from.ID);

            to.SubTasks = new List<Subtask>();

            foreach (var subtask in subTasks)
            {
                to.SubTasks.Add(EngineFactory.GetSubtaskEngine().Copy(subtask, to, team));
            }

            return to;
        }

        public Task CopyFiles(Task from, Task to)
        {
            if (from.Project.ID != to.Project.ID) return to;

            var files = GetFiles(from);

            foreach (var file in files)
            {
                AttachFile(to, file.ID);
            }

            return to;
        }

        public Task MoveToMilestone(Task task, int milestoneID)
        {
            ProjectSecurity.DemandEdit(task);

            if (task == null) throw new ArgumentNullException("task");
            if (task.Project == null) throw new Exception("Project can be null.");

            var newMilestone = milestoneID != 0;
            var milestone = DaoProjectFactory.GetMilestoneDao().GetById(newMilestone ? milestoneID : task.Milestone);

            var senders = GetSubscribers(task);

            if (!DisableNotifications && senders.Count != 0)
                NotifyClient.SendAboutTaskRemoved(senders, task, milestone, newMilestone);

            task.Milestone = milestoneID;
            task.LastModifiedBy = SecurityContext.CurrentAccount.ID;
            task.LastModifiedOn = TenantUtil.DateTimeNow();

            return DaoProjectFactory.GetTaskDao().Update(task);
        }

        public void NotifyTask(Task task, IEnumerable<Guid> inviteToResponsibles, IEnumerable<Guid> removeResponsibles, bool isNew, bool notifyResponsible)
        {
            if (DisableNotifications) return;

            var senders = GetSubscribers(task);
            senders = senders.FindAll(r => !inviteToResponsibles.Contains(new Guid(r.ID)) && !removeResponsibles.Contains(new Guid(r.ID)));

            if (senders.Any())
            {
                if (isNew)
                {
                    NotifyClient.SendAboutTaskCreating(senders, task);
                }
                else
                {
                    NotifyClient.SendAboutTaskEditing(senders, task);
                }
            }

            if (notifyResponsible)
                NotifyResponsible(task, inviteToResponsibles.ToList(), removeResponsibles.ToList());
        }

        private void NotifyResponsible(Task task, List<Guid> inviteToResponsibles, List<Guid> removeResponsibles)
        {
            if (DisableNotifications) return;

            if (inviteToResponsibles.Any())
                NotifyClient.SendAboutResponsibleByTask(inviteToResponsibles, task);

            if (removeResponsibles.Any())
                NotifyClient.SendAboutRemoveResponsibleByTask(removeResponsibles, task);
        }

        public void SendReminder(Task task)
        {
            //Don't send anything if notifications are disabled
            if (DisableNotifications || task.Responsibles == null || !task.Responsibles.Any()) return;

            NotifyClient.SendReminderAboutTask(task.Responsibles.Where(r => !r.Equals(SecurityContext.CurrentAccount.ID)).Distinct(), task);
        }


        public void Delete(Task task)
        {
            if (task == null) throw new ArgumentNullException("task");

            ProjectSecurity.DemandDelete(task);
            DaoProjectFactory.GetTaskDao().Delete(task);

            var recipients = GetSubscribers(task);

            if (recipients.Count != 0)
            {
                NotifyClient.SendAboutTaskDeleting(recipients, task);
            }

            UnSubscribeAll(task);

            _ = FactoryIndexer.DeleteAsync(DaoProjectFactory.GetTaskDao().ToDbTask(task));
        }

        #endregion

        #region Link

        public void AddLink(Task parentTask, Task dependentTask, TaskLinkType linkType)
        {
            CheckLink(parentTask, dependentTask, linkType);

            var link = new TaskLink
            {
                ParentTaskId = parentTask.ID,
                DependenceTaskId = dependentTask.ID,
                LinkType = linkType
            };

            if (DaoProjectFactory.GetTaskDao().IsExistLink(link))
                throw new Exception("link already exist");

            ProjectSecurity.DemandEdit(dependentTask);
            ProjectSecurity.DemandEdit(parentTask);

            parentTask.Links.Add(link);
            dependentTask.Links.Add(link);

            DaoProjectFactory.GetTaskDao().AddLink(link);
        }

        public void RemoveLink(Task dependentTask, Task parentTask)
        {
            ProjectSecurity.DemandEdit(dependentTask);

            DaoProjectFactory.GetTaskDao().RemoveLink(new TaskLink { DependenceTaskId = dependentTask.ID, ParentTaskId = parentTask.ID });
            dependentTask.Links.RemoveAll(r => r.ParentTaskId == parentTask.ID && r.DependenceTaskId == dependentTask.ID);
            parentTask.Links.RemoveAll(r => r.ParentTaskId == parentTask.ID && r.DependenceTaskId == dependentTask.ID);
        }

        private static void CheckLink(Task parentTask, Task dependentTask)
        {
            if (parentTask == null) throw new ArgumentNullException("parentTask");
            if (dependentTask == null) throw new ArgumentNullException("dependentTask");

            if (parentTask.ID == dependentTask.ID)
            {
                throw new Exception("it is impossible to create a link between one and the same task");
            }

            /*            if (parentTask.Status == TaskStatus.Closed || dependentTask.Status == TaskStatus.Closed)
                        {
                            throw new Exception("Such link don't be created. Task closed.");
                        }*/

            if (parentTask.Milestone != dependentTask.Milestone)
            {
                throw new Exception("Such link don't be created. Different Milestones");
            }

        }

        private static void CheckLink(Task parentTask, Task dependentTask, TaskLinkType linkType)
        {
            CheckLink(parentTask, dependentTask);

            switch (linkType)
            {
                case TaskLinkType.End:
                    if ((parentTask.Deadline.Equals(DateTime.MinValue) && parentTask.Milestone == 0) || (dependentTask.Deadline.Equals(DateTime.MinValue) && dependentTask.Milestone == 0))
                    {
                        throw new Exception("Such link don't be created. Incorrect task link type.");
                    }
                    break;

                case TaskLinkType.EndStart:
                    if ((parentTask.Deadline.Equals(DateTime.MinValue) && parentTask.Milestone == 0))
                    {
                        throw new Exception("Such link don't be created. Incorrect task link type.");
                    }
                    break;
            }
        }

        #endregion
    }
}
