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
using System.Data;
using System.Linq;
using ASC.Core;
using ASC.Core.Common.EF;
using ASC.Core.Common.EF.Context;
using ASC.Projects.EF;
using ASC.Core.Tenants;
using ASC.ElasticSearch;
using ASC.Projects.Core.DataInterfaces;
using ASC.Projects.Core.Domain;
using ASC.Core.Common.Settings;
using ASC.Projects.Core.Domain.Reports;
using ASC.Common;

namespace ASC.Projects.Data.DAO
{
    /*internal class CachedTaskDao : TaskDao
    {
        private readonly HttpRequestDictionary<DbTask> taskCache = new HttpRequestDictionary<DbTask>("task");

        public CachedTaskDao(int tenantID) : base(tenantID)
        {
        }

        public override void Delete(DbTask task)
        {
            ResetCache(task.ID);
            base.Delete(task);
        }

        public override DbTask GetById(int id)
        {
            return taskCache.Get(id.ToString(CultureInfo.InvariantCulture), () => GetBaseById(id));
        }

        private DbTask GetBaseById(int id)
        {
            return base.GetById(id);
        }

        public override DbTask Update(DbTask task)
        {
            if (task != null)
            {
                ResetCache(task.ID);
            }
            return base.Update(task);
        }

        private void ResetCache(int taskId)
        {
            taskCache.Reset(taskId.ToString(CultureInfo.InvariantCulture));
        }
    }

    */
    [Scope]
    public class TaskDao : BaseDao, ITaskDao
    {
        private TenantUtil TenantUtil { get; set; }
        private FactoryIndexer<DbTask> FactoryIndexerTask { get; set; }
        private FactoryIndexer<DbSubtask> FactoryIndexerSubTask { get; set; }
        private SettingsManager SettingsManager { get; set; }
        private FilterHelper FilterHelper { get; set; }
        private IDaoFactory DaoFactory { get; set; }

        public TaskDao(SecurityContext securityContext, DbContextManager<WebProjectsContext> dbContextManager, TenantUtil tenantUtil, FactoryIndexer<DbTask> factoryIndexerTask, FactoryIndexer<DbSubtask> factoryIndexerSubTask, IDaoFactory daoFactory, SettingsManager settingsManager, FilterHelper filterHelper, TenantManager tenantManager) : base(securityContext, dbContextManager, tenantManager)
        {
            TenantUtil = tenantUtil;
            FactoryIndexerTask = factoryIndexerTask;
            FactoryIndexerSubTask = factoryIndexerSubTask;
            SettingsManager = settingsManager;
            FilterHelper = filterHelper;
            DaoFactory = daoFactory;
        }


        #region ITaskDao

        public List<Task> GetAll()
        {
            var query = CreateQuery();
            var tasks = query.GroupBy(q => new {q.Task, q.Project}, q => q.TasksResponsible.ResponsibleId)
                .Select(q => ToTask(new QueryTask() { Task = q.Key.Task, Project = q.Key.Project}, Concat(q.ToList())));
            
            return tasks.ToList();
        }

        private string Concat(List<string> strings)
        {
            return string.Join(",", strings.Distinct());
        }

        public List<Task> GetByProject(int projectId, TaskStatus? status, Guid participant)
        {
            var query = CreateQuery().Join(WebProjectsContext.Milestone,
                q => q.Task.MilestoneId,
                m => m.Id,
                (q, m) => new QueryTask
                {
                    Task = q.Task,
                    Project = q.Project,
                    TasksResponsible = q.TasksResponsible,
                    Milestone = q.Milestone
                }).Where(q => q.Milestone.TenantId == q.Task.TenantId && q.Task.ProjectId == projectId);

            if (status.HasValue)
            {
                  query = query.Where(q=> (TaskStatus)q.Task.Status == status.Value);
            }

            if (!participant.Equals(Guid.Empty))
            {
                query = query.Where(q=> 
                WebProjectsContext.Subtask.Where(s=> s.TenantId == q.Task.TenantId && s.TaskId == q.Task.Id && (TaskStatus)s.Status == TaskStatus.Open && s.ResponsibleId == participant.ToString()).Any() 
                && WebProjectsContext.TasksResponsible.Where(tr=> tr.TenantId == q.Task.TenantId && tr.TaskId == q.Task.Id && tr.ResponsibleId == participant.ToString()).Any());
            }

            return query.GroupBy(q => new { q.Task, q.Project, q.Milestone }, q => q.TasksResponsible.ResponsibleId)
                .OrderBy(q => q.Key.Task.SortOrder)
                .ThenBy(q => q.Key.Milestone.Status)
                .ThenBy(q => q.Key.Milestone.Deadline)
                .ThenBy(q => q.Key.Milestone.Id)
                .ThenBy(q => q.Key.Task.Status)
                .ThenBy(q => q.Key.Task.Priority)
                .ThenBy(q => q.Key.Task.CreateOn)
                .Select(q => ToTask(new QueryTask() { Task = q.Key.Task, Project = q.Key.Project }, Concat(q.ToList())))
                .ToList();
        }

        public List<Task> GetByFilter(TaskFilter filter, bool isAdmin, bool checkAccess)
        {
            var query = CreateQuery().Join(WebProjectsContext.Milestone,
                q => q.Task.MilestoneId,
                m => m.Id,
                (q, m) => new QueryTask
                {
                    Task = q.Task,
                    Project = q.Project,
                    TasksResponsible = q.TasksResponsible,
                    Milestone = q.Milestone
                }).Where(q => q.Milestone.TenantId == q.Task.MilestoneId);

            if (filter.Max > 0 && filter.Max < 150000)
            {
                query = query.Skip((int)filter.Offset);
                query = query.Take((int)filter.Max);
            }

            if (!string.IsNullOrEmpty(filter.SortBy))
            {
                var sortColumns = filter.SortColumns["Task"];
                sortColumns.Remove(filter.SortBy);

               // query.OrderBy(GetSortFilter(filter.SortBy, filter.SortOrder), filter.SortOrder);//todo

                foreach (var sort in sortColumns.Keys)
                {
                  //  query.OrderBy(GetSortFilter(sort, sortColumns[sort]), sortColumns[sort]);
                }
            }

            query = CreateQueryFilter(query, filter, isAdmin, checkAccess);

            var tasks = query.GroupBy(q => new { q.Task, q.Project }, q => q.TasksResponsible.ResponsibleId)
                .Select(q => ToTask(new QueryTask() { Task = q.Key.Task, Project = q.Key.Project }, Concat(q.ToList())));

            return tasks.ToList();
        }

        public TaskFilterCountOperationResult GetByFilterCount(TaskFilter filter, bool isAdmin, bool checkAccess)
        {
            var query = WebProjectsContext.Task.Join(WebProjectsContext.Project,
                t => t.ProjectId,
                p => p.Id,
                (t, p) => new QueryTask
                {
                    Task = t,
                    Project = p
                }).Where(q => q.Task.TenantId == q.Project.TenantId && q.Task.TenantId == Tenant);

            query = CreateQueryFilterCount(query, filter, isAdmin, checkAccess);

            var queryCount = query.GroupBy(q => q.Task.Status)
                .ToDictionary(q=>q.Key, q=> q.Count());

            var tasksOpen = queryCount.Where(row => row.Key != (int)TaskStatus.Closed).Sum(row => row.Value);
            //that's right. open its not closed.
            int tasksClosed;
            queryCount.TryGetValue((int)TaskStatus.Closed, out tasksClosed);
            return new TaskFilterCountOperationResult { TasksOpen = tasksOpen, TasksClosed = tasksClosed };
        }

        public IEnumerable<TaskFilterCountOperationResult> GetByFilterCountForStatistic(TaskFilter filter, bool isAdmin, bool checkAccess)
        {
            var result = new List<TaskFilterCountOperationResult>();
            var query = WebProjectsContext.Task.Join(WebProjectsContext.Project,
                t => t.ProjectId,
                p => p.Id,
                (t, p) => new
                {
                    Task = t,
                    Project = p
                }).Join(WebProjectsContext.TasksResponsible,
                q => q.Task.Id,
                tr => tr.TaskId,
                (q, tr) => new QueryTask
                {
                    Task = q.Task,
                    Project = q.Project,
                    TasksResponsible = tr
                }).Where(q => q.Task.TenantId == Tenant && q.Task.TenantId == q.TasksResponsible.TenantId && q.Task.TenantId == q.Project.TenantId);

            if (filter.HasUserId)
            {
                query = query.Where(q => FilterHelper.GetUserIds(filter).Contains(q.TasksResponsible.ResponsibleId));
            }
            else
            {
                query = query.Where(q => q.TasksResponsible.ResponsibleId != Guid.Empty.ToString());
            }

            filter.UserId = Guid.Empty;

            query = CreateQueryFilterCount(query, filter, isAdmin, checkAccess);


            if (filter.ParticipantId.HasValue && !filter.ParticipantId.Value.Equals(Guid.Empty))
            {
                query = query.Where(q => q.TasksResponsible.ResponsibleId == filter.ParticipantId.Value.ToString());
            }

            var queryCount = query.GroupBy(q => new 
                { 
                    Id = q.TasksResponsible.ResponsibleId,
                    Status = q.Task.Status
                })
                .Select(q=> new
                {
                    Id = q.Key.Id,
                    Count = q.Count(),
                    Status = q.Key.Status
                }).GroupBy(q=> q.Id);

            foreach (var r in queryCount)
            {
                var tasksOpen = r.Where(row => row.Status != (int)TaskStatus.Closed).Sum(row => row.Count);
                var tasksClosed = r.Where(row => row.Status == (int)TaskStatus.Closed).Sum(row => row.Count);
                result.Add(new TaskFilterCountOperationResult { UserId = ToGuid(r.Key), TasksOpen = tasksOpen, TasksClosed = tasksClosed });
            }

            return result;
        }

        public List<Tuple<Guid, int, int>> GetByFilterCountForReport(TaskFilter filter, bool isAdmin, bool checkAccess)
        {
            filter = (TaskFilter)filter.Clone();

            var query = WebProjectsContext.Task.Join(WebProjectsContext.Project,
                t => t.ProjectId,
                p => p.Id,
                (t, p) => new QueryTask
                {
                    Task = t,
                    Project = p
                }).Where(q => q.Task.TenantId == q.Project.TenantId && q.Task.TenantId == Tenant && q.Task.CreateOn >= FilterHelper.GetFromDate(filter) && q.Task.CreateOn <= FilterHelper.GetToDate(filter));

            if (filter.HasUserId)
            {
                query = query.Where(q => FilterHelper.GetUserIds(filter).Contains(q.Task.CreateBy));
                filter.UserId = Guid.Empty;
                filter.DepartmentId = Guid.Empty;
            }

            query = CreateQueryFilterCount(query, filter, isAdmin, checkAccess);

            var queryCount = query.GroupBy(q => new { q.Task.CreateBy, q.Task.ProjectId });

            return queryCount.ToList().ConvertAll(b => new Tuple<Guid, int, int>(Guid.Parse(b.Key.CreateBy), b.Key.ProjectId, b.Count()));
        }

        public List<Task> GetByResponsible(Guid responsibleId, IEnumerable<TaskStatus> statuses)
        {
            var query = CreateQuery().Join(WebProjectsContext.Subtask,
                q=> q.Task.Id,
                s=> s.TaskId,
                (q,s)=> new
                {
                    Query = q,
                    Subtask = s
                }).Where(q=> q.Subtask.TenantId == Tenant && q.Subtask.ResponsibleId == responsibleId.ToString() && (q.Subtask.Status == null ? -1 : q.Subtask.Status) == (int)TaskStatus.Open || q.Query.TasksResponsible.ResponsibleId == responsibleId.ToString());

            if (statuses != null && statuses.Any())
            {
                var status = statuses.First();
                query = query.Where(q => (TaskStatus)q.Query.Task.Status == status);
            }

            return query.Select(q => q.Query)
                .GroupBy(q => new {q.Task, q.Project}, q => q.TasksResponsible.ResponsibleId)
                .OrderBy(q => q.Key.Task.SortOrder)
                .ThenBy(q => q.Key.Task.Status)
                .ThenBy(q => q.Key.Task.Priority)
                .ThenBy(q => q.Key.Task.CreateOn)
                .Select(q => ToTask(new QueryTask() { Task = q.Key.Task, Project = q.Key.Project }, Concat(q.ToList())))
                .ToList();
        }

        public List<Task> GetMilestoneTasks(int milestoneId)
        {
            return CreateQuery().Where(q => q.Task.MilestoneId == milestoneId)
                .GroupBy(q => new { q.Task, q.Project }, q => q.TasksResponsible.ResponsibleId)
                .OrderBy(q => q.Key.Task.SortOrder)
                .ThenBy(q => q.Key.Task.Status)
                .ThenBy(q => q.Key.Task.Priority)
                .ThenBy(q => q.Key.Task.CreateOn)
                .Select(q => ToTask(new QueryTask() { Task = q.Key.Task, Project = q.Key.Project }, Concat(q.ToList())))
                .ToList();
        }

        public List<Task> GetById(ICollection<int> ids)
        {
            return CreateQuery()
                .Where(q => ids.ToArray().Contains(q.Task.Id))
                .GroupBy(q => new { q.Task, q.Project }, q => q.TasksResponsible.ResponsibleId)
                .Select(q => ToTask(new QueryTask() { Task = q.Key.Task, Project = q.Key.Project }, Concat(q.ToList())))
                .ToList();
        }

        public virtual Task GetById(int id)
        {
            return CreateQuery()
                .Where(q => q.Task.Id == id)
                .GroupBy(q => new { q.Task, q.Project }, q => q.TasksResponsible.ResponsibleId)
                .Select(q => ToTask(new QueryTask() { Task = q.Key.Task, Project = q.Key.Project }, Concat(q.ToList())))
                .SingleOrDefault();
        }

        public bool IsExists(int id)
        {
            var count = WebProjectsContext.Task.Where(t => t.Id == id).Count();
            return 0 < count;
        }

        public List<object[]> GetTasksForReminder(DateTime deadline)
        {
            var deadlineDate = deadline.Date;
            return WebProjectsContext.Task.Join(WebProjectsContext.Project,
                t => t.ProjectId,
                p => p.Id,
                (t, p) => new QueryTask()
                {
                    Task = t,
                    Project = p
                }).Where(q => q.Task.TenantId == q.Project.TenantId && q.Task.Deadline >= deadline.AddDays(-1) && q.Task.Deadline <= deadline.AddDays(1) && (TaskStatus)q.Task.Status == TaskStatus.Open && (ProjectStatus)q.Project.Status == ProjectStatus.Open)
                .Select(q => new object[] { q.Task.TenantId, q.Task.Id, q.Task.Deadline })
                .ToList();
        }

        public Task Create(Task task)
        {
            task.CreateOn = TenantUtil.DateTimeToUtc(task.CreateOn);
            var db = ToDbTask(task);
            WebProjectsContext.Task.Add(db);

            WebProjectsContext.SaveChanges();

            if (task.Responsibles.Any())
            {
                var resps = WebProjectsContext.TasksResponsible.Where(tr => task.Responsibles.Contains(ToGuid(tr.ResponsibleId))).ToList();
                WebProjectsContext.TasksResponsible.AddRange(resps);
            }

            WebProjectsContext.SaveChanges();
            task.ID = db.Id;
            return task;
        }

        public virtual Task Update(Task task)
        {
            task.LastModifiedOn = TenantUtil.DateTimeToUtc(task.LastModifiedOn);
            task.StatusChangedOn = TenantUtil.DateTimeToUtc(task.StatusChangedOn);
            var db = ToDbTask(task);
            WebProjectsContext.Task.Add(db);

            var responsiblesDelete = WebProjectsContext.TasksResponsible.Where(tr => tr.TaskId == task.ID).ToList();
            WebProjectsContext.TasksResponsible.RemoveRange(responsiblesDelete);

            if (task.Responsibles.Any())
            {
                var resps = WebProjectsContext.TasksResponsible.Where(tr => task.Responsibles.Contains(ToGuid(tr.ResponsibleId))).ToList();
                WebProjectsContext.TasksResponsible.AddRange(resps);
            }

            WebProjectsContext.SaveChanges();

            return task;
        }

        public virtual void Delete(Task task)
        {
            var id = task.ID;
            task.Links.ForEach(RemoveLink);
            task.SubTasks.ForEach(subTask => DaoFactory.GetSubtaskDao().Delete(subTask.ID));
            var comments = WebProjectsContext.Comment.Where(c => c.TargetUniqId == task.UniqID).ToList();
            WebProjectsContext.Comment.RemoveRange(comments);
            var taskResponsibles = WebProjectsContext.TasksResponsible.Where(tr => tr.TaskId == id).ToList();
            WebProjectsContext.TasksResponsible.RemoveRange(taskResponsibles);
            var db = WebProjectsContext.Task.Where(t => t.Id == task.ID).SingleOrDefault();
            WebProjectsContext.Task.Remove(db);

            WebProjectsContext.SaveChanges();
        }

        #region Recurrence

        public List<object[]> GetRecurrence(DateTime date)
        {
            return WebProjectsContext.Task.Join(WebProjectsContext.TaskRecurrence,
                t => t.Id,
                tr => tr.TaskId,
                (t, tr) => new
                {
                    Task = t,
                    TaskRecurrence = tr
                }).Where(q => q.TaskRecurrence.StartDate >= date && q.TaskRecurrence.EndDate <= date && (TaskStatus)q.Task.Status == TaskStatus.Open)
                .Select(q=> new object[]{q.TaskRecurrence.TenantId, q.TaskRecurrence.TaskId })
                .ToList();
        }

        public void SaveRecurrence(Task task, string cron, DateTime startDate, DateTime endDate)
        {
            var taskRecurrence = new DbTaskRecurrence()
            {
                TaskId = task.ID,
                Cron = cron,
                StartDate = startDate,
                EndDate = endDate
            };
            WebProjectsContext.TaskRecurrence.Add(taskRecurrence);
            WebProjectsContext.SaveChanges();
        }

        public void DeleteReccurence(int taskId)
        {
            var taskRecurrence = WebProjectsContext.TaskRecurrence.Where(tr => tr.TaskId == taskId).SingleOrDefault();
            WebProjectsContext.TaskRecurrence.Remove(taskRecurrence);
            WebProjectsContext.SaveChanges();
        }

        #endregion

        #region Link

        public void AddLink(TaskLink taskLink)
        {
            DbLink link = new DbLink()
            {
                TaskId = taskLink.DependenceTaskId,
                ParentId = taskLink.ParentTaskId,
                LinkType = (int)taskLink.LinkType
            };
            WebProjectsContext.Link.Add(link);
            WebProjectsContext.SaveChanges();
        }

        public void RemoveLink(TaskLink taskLink)
        {
            var links = WebProjectsContext.Link.Where(l => l.TaskId == taskLink.DependenceTaskId && l.ParentId == taskLink.ParentTaskId ||
            l.TaskId == taskLink.ParentTaskId && l.ParentId == taskLink.DependenceTaskId).ToList();
            WebProjectsContext.Link.RemoveRange(links); 

            WebProjectsContext.SaveChanges();
        }

        public bool IsExistLink(TaskLink taskLink)
        {
            var links = WebProjectsContext.Link.Where(l => l.TaskId == taskLink.DependenceTaskId && l.ParentId == taskLink.ParentTaskId ||
            l.TaskId == taskLink.ParentTaskId && l.ParentId == taskLink.DependenceTaskId);

            return links.Count() > 0;
        }

        public IEnumerable<TaskLink> GetLinks(int taskID)
        {
            return WebProjectsContext.Link.Where(l => l.TaskId == taskID || l.ParentId == taskID)
                .Select(l=> ToTaskLink(l))
                .ToList();
        }

        public IEnumerable<TaskLink> GetLinks(List<Task> tasks)
        {
            return WebProjectsContext.Link.Where(l => tasks.Select(r => r.ID).ToList().Contains(l.TaskId) || tasks.Select(r => r.ID).ToList().Contains(l.ParentId))
                .Select(l => ToTaskLink(l))
                .ToList();
        }

        #endregion

        #endregion

        #region Private

        private IQueryable<QueryTask> CreateQuery()
        {
            return WebProjectsContext.Task.Join(WebProjectsContext.Project,
                t => t.ProjectId,
                p => p.Id,
                (t, p) => new
                {
                    Task = t,
                    Project = p
                })
                .Join(WebProjectsContext.TasksResponsible,
                q => q.Task.Id,
                tr => tr.TaskId,
                (q, tr) => new QueryTask()
                {
                    Task = q.Task,
                    Project = q.Project,
                    TasksResponsible = tr
                }).Where(q => q.Task.TenantId == Tenant && q.Project.TenantId == q.Task.TenantId && q.Task.TenantId == q.TasksResponsible.TenantId);
        }

        private IQueryable<QueryTask> CreateQueryFilterBase(IQueryable<QueryTask> query, TaskFilter filter)
        {
            if (filter.ProjectIds.Count != 0)
            {
               query = query.Where(q=> filter.ProjectIds.Contains(q.Task.Id));
            }
            else
            {
                if (SettingsManager.Load<ProjectsCommonSettings>().HideEntitiesInPausedProjects)
                {
                    query = query.Where(q => (ProjectStatus)q.Project.Status == ProjectStatus.Paused);
                }

                if (filter.MyProjects)
                {
                    query = query.Join(WebProjectsContext.Participant,
                        q => q.Project.Id,
                        p => p.ProjectId,
                        (q, p) => new
                        {
                            Query = q,
                            Participant = p
                        }).Where(q => q.Participant.Removed == 0 && q.Participant.Tenant == q.Query.Task.TenantId && q.Participant.ParticipantId == CurrentUserID.ToString())
                        .Select(q=> q.Query);
                }
            }

            if (filter.TagId != 0)
            {
                if (filter.TagId == -1)
                {
                    query = query.Join(WebProjectsContext.TagToProject,
                        q => q.Task.ProjectId,
                        tp => tp.ProjectId,
                        (q, tp) => new
                        {
                            Query = q,
                            TagToProject = tp
                        }).Where(q => q.TagToProject.TagId == null)
                        .Select(q=> q.Query); 
                }
                else
                {
                    query = query.Join(WebProjectsContext.TagToProject,
                        q => q.Task.ProjectId,
                        tp => tp.ProjectId,
                        (q, tp) => new
                        {
                            Query = q,
                            TagToProject = tp
                        }).Where(q => q.TagToProject.TagId == filter.TagId)
                        .Select(q => q.Query);
                }
            }

            if (filter.Substatus.HasValue)
            {
                var substatus = filter.Substatus.Value;
                if (substatus > -1)
                {
                    query = query.Join(WebProjectsContext.Status,
                        q => substatus,
                        s => s.Id,
                        (q, s) => new
                        {
                            Query = q,
                            Status = s
                        }).Where(q => q.Status.TenantId == q.Query.Task.TenantId && q.Query.Task.StatusId == substatus || q.Query.Task.StatusId == null && q.Query.Task.Status == q.Status.StatusType && q.Status.IsDefault == 1)
                        .Select(q=> q.Query);
                }
                else
                {
                    query = query.Where(q => q.Task.Status == -substatus && q.Task.StatusId == null);

                }
            }
            else if (filter.TaskStatuses.Count != 0)
            {
                var status = filter.TaskStatuses.First();
                query = query.Where(q => (TaskStatus)q.Task.Status == status);
            }

            if (!filter.UserId.Equals(Guid.Empty))
            {
                query = query.Where(q => q.Task.CreateBy == filter.UserId.ToString());
            }

            if (!filter.DepartmentId.Equals(Guid.Empty) || (filter.ParticipantId.HasValue && !filter.ParticipantId.Value.Equals(Guid.Empty)))
            {
                query.Where(q=> IsExistSubtask(q, filter) || IsExistResponsible(q, filter));
            }

            if (!string.IsNullOrEmpty(filter.SearchText))
            {
                List<int> taskIds;
                if (FactoryIndexerTask.TrySelectIds(s => s.MatchAll(filter.SearchText), out taskIds))
                {
                    IReadOnlyCollection<DbSubtask> subtaskIds;
                    if (FactoryIndexerSubTask.TrySelect(s => s.MatchAll(filter.SearchText), out subtaskIds))
                    {
                        taskIds.AddRange(subtaskIds.Select(r => r.TaskId).ToList());
                    }
                    query = query.Where(q => taskIds.Contains(q.Task.Id));
                }
                else
                {
                    query = query.Where(q => q.Task.Title.Contains(filter.SearchText));
                }
            }

            return query;
        }
        private bool IsExistSubtask(QueryTask q, TaskFilter filter)
        {
            var existSubtask = WebProjectsContext.Subtask.Where(s => s.TenantId == q.Task.TenantId && q.Task.Id == s.TaskId && (TaskStatus)s.Status == TaskStatus.Open);

            if (!filter.DepartmentId.Equals(Guid.Empty))
            {
                existSubtask = existSubtask.Join(WebProjectsContext.UserGroup,
                    s => s.ResponsibleId,
                    u => u.UserId.ToString(),
                    (s, u) => new
                    {
                        Subtask = s,
                        UserGroup = u
                    }).Where(q => q.UserGroup.Removed == false && q.Subtask.TenantId == q.UserGroup.Tenant && q.UserGroup.GroupId == filter.DepartmentId)
                    .Select(q=> q.Subtask);
            }

            if (filter.ParticipantId.HasValue && !filter.ParticipantId.Value.Equals(Guid.Empty))
            {
                existSubtask = existSubtask.Where(s => s.ResponsibleId == filter.ParticipantId.Value.ToString());
            }
            return existSubtask.Any();
        }

        private bool IsExistResponsible(QueryTask q, TaskFilter filter)
        {
            var existResponsible = WebProjectsContext.TasksResponsible.Where(tr => tr.TenantId == q.Task.TenantId && q.Task.Id == tr.TaskId);

            if (!filter.DepartmentId.Equals(Guid.Empty))
            {
                existResponsible = existResponsible.Join(WebProjectsContext.UserGroup,
                    tr => tr.ResponsibleId,
                    u => u.UserId.ToString(),
                    (tr, u) => new
                    {
                        TasksResponsible = tr,
                        UserGroup = u
                    }).Where(q => q.UserGroup.Removed == false && q.UserGroup.Tenant == q.TasksResponsible.TenantId && q.UserGroup.GroupId == filter.DepartmentId)
                    .Select(q=> q.TasksResponsible);
            }

            if (filter.ParticipantId.HasValue && !filter.ParticipantId.Value.Equals(Guid.Empty))
            {
                existResponsible = existResponsible.Where(tr => tr.ResponsibleId == filter.ParticipantId.Value.ToString());
            }
            return existResponsible.Any();
        }

        private IQueryable<QueryTask> CreateQueryFilterCount(IQueryable<QueryTask> query, TaskFilter filter, bool isAdmin, bool checkAccess)
        {
            var minDateTime = DateTime.MinValue;
            var maxDateTime = DateTime.MaxValue;
            var minDateTimeString = DateTime.MinValue.ToString("yyyy-MM-dd HH:mm:ss");

            query = CreateQueryFilterBase(query, filter);

            if (filter.Milestone.HasValue)
            {
                query = query.Where(q => q.Task.MilestoneId == filter.Milestone);
            }
            else if (filter.MyMilestones)
            {
                if (!filter.MyProjects)
                {
                    query = query.Join(WebProjectsContext.Participant,
                        q => q.Project.Id,
                        p => p.ProjectId,
                        (q, p) => new
                        {
                            Query = q,
                            Participant = p
                        }).Where(q => q.Participant.Removed == 0 && q.Participant.Tenant == q.Query.Task.TenantId && q.Participant.ParticipantId == CurrentUserID.ToString())
                        .Select(q=> q.Query);
                }
                query = query.Where(q =>
                WebProjectsContext.Milestone.Where(m => q.Task.MilestoneId == m.Id && m.TenantId == q.Task.TenantId).Any());
            }

            if (filter.ParticipantId.HasValue && filter.ParticipantId.Value.Equals(Guid.Empty))
            {
                query = query.Where(q =>
                !WebProjectsContext.TasksResponsible.Where(tr => q.Task.Id == tr.TaskId && q.Task.TenantId == tr.TenantId).Any());
            }

            var hasFromDate = !filter.FromDate.Equals(minDateTime) && !filter.FromDate.Equals(maxDateTime);
            var hasToDate = !filter.ToDate.Equals(minDateTime) && !filter.ToDate.Equals(maxDateTime);

            if (hasFromDate && hasToDate)
            {
                query = query.Where(q => q.Task.Deadline >= TenantUtil.DateTimeFromUtc(filter.FromDate) && q.Task.Deadline <= TenantUtil.DateTimeFromUtc(filter.ToDate)
                || WebProjectsContext.Milestone.Where(m => m.Id == q.Task.MilestoneId && q.Task.TenantId == m.Id && m.Deadline >= TenantUtil.DateTimeFromUtc(filter.FromDate) && m.Deadline <= TenantUtil.DateTimeFromUtc(filter.ToDate)).Any()
                && q.Task.Deadline == minDateTime);
            }
            else if (hasFromDate)
            {
                query = query.Where(q => q.Task.Deadline >= TenantUtil.DateTimeFromUtc(filter.FromDate)
                || WebProjectsContext.Milestone.Where(m => m.Id == q.Task.MilestoneId && m.TenantId == q.Task.TenantId && m.Deadline >= TenantUtil.DateTimeFromUtc(filter.FromDate) && q.Task.Deadline == minDateTime).Any());
            }
            else if (hasToDate)
            {
                query = query.Where(q => q.Task.Deadline == minDateTime && q.Task.Deadline <= TenantUtil.DateTimeFromUtc(filter.ToDate)
                || WebProjectsContext.Milestone.Where(m => m.Id == q.Task.MilestoneId && m.TenantId == q.Task.TenantId && m.Deadline <= TenantUtil.DateTimeFromUtc(filter.ToDate) && m.Deadline != minDateTime && q.Task.Deadline == minDateTime).Any());
            }

            query = CheckSecurity(query, filter, isAdmin, checkAccess);

            return query;
        }

        private IQueryable<QueryTask> CreateQueryFilter(IQueryable<QueryTask> query, TaskFilter filter, bool isAdmin, bool checkAccess)
        {
            query = CreateQueryFilterBase(query, filter);

            if (filter.ParticipantId.HasValue && filter.ParticipantId.Value.Equals(Guid.Empty))
            {
                query = query.Where(q => q.TasksResponsible.TaskId == null);
            }

            if (filter.Milestone.HasValue)
            {
                query = query.Where(q => q.Task.MilestoneId == filter.Milestone);
            }
            else if (filter.MyMilestones)
            {
                if (!filter.MyProjects)
                {
                    query = query.Join(WebProjectsContext.Participant,
                        q => q.Project.Id,
                        p => p.ProjectId,
                        (q, p) => new
                        {
                            Query = q,
                            Participant = p
                        }).Where(q => q.Participant.Removed == 0 && q.Participant.Tenant == q.Query.Task.TenantId && q.Participant.ParticipantId == CurrentUserID.ToString())
                        .Select(q => q.Query);
                }

                query = query.Where(q => q.Milestone.Id == 0);
            }

            if (!filter.FromDate.Equals(DateTime.MinValue) && !filter.FromDate.Equals(DateTime.MaxValue))
            {
                //query.Where(Exp.Ge(GetSortFilter("deadline", true), TenantUtil.DateTimeFromUtc(filter.FromDate)));
            }

            if (!filter.ToDate.Equals(DateTime.MinValue) && !filter.ToDate.Equals(DateTime.MaxValue))
            {
               // query.Where(Exp.Le(GetSortFilter("deadline", true), TenantUtil.DateTimeFromUtc(filter.ToDate)));
            }

            CheckSecurity(query, filter, isAdmin, checkAccess);

            return query;
        }



        private static TaskLink ToTaskLink(DbLink link)
        {
            return new TaskLink
            {
                DependenceTaskId = link.TaskId,
                ParentTaskId = link.ParentId,
                LinkType = (TaskLinkType)link.LinkType
            };
        }

        private static string GetSortFilter(string sortBy, bool sortOrder)
        {
            if (sortBy != "deadline") return "t." + sortBy;

            var sortDate = sortOrder ? DateTime.MaxValue.ToString("yyyy-MM-dd HH:mm:ss") : DateTime.MinValue.ToString("yyyy-MM-dd HH:mm:ss");
            return string.Format("COALESCE(COALESCE(NULLIF(t.deadline,'{0}'),m.deadline), '{1}')",
                                 DateTime.MinValue.ToString("yyyy-MM-dd HH:mm:ss"), sortDate);

        }

        private IQueryable<QueryTask> CheckSecurity(IQueryable<QueryTask> query, TaskFilter filter, bool isAdmin, bool checkAccess)
        {
            if (checkAccess)
            {
                query = query.Where(q => q.Project.Private == 0);
                return query;
            }

            if (isAdmin) return query;

            var queryP = query.Join(WebProjectsContext.Participant,
                q => q.Task.ProjectId,
                p => p.ProjectId,
                (q, p) => new
                {
                    Query = q,
                    Participant = p
                }).Where(q => q.Participant.ParticipantId == CurrentUserID.ToString() && q.Participant.Tenant == q.Query.Task.TenantId);
            
            query = queryP.Where(q => q.Query.Project.Private == 0 || q.Participant.Security != null && q.Participant.Removed == 0
            && (WebProjectsContext.TasksResponsible.Where(tr => tr.TaskId == q.Query.Task.Id && tr.TenantId == q.Query.
            Task.TenantId && tr.ResponsibleId == CurrentUserID.ToString()).Any()
            || q.Participant.Security == (int)ProjectTeamSecurity.Tasks && q.Query.Task.MilestoneId == 0 || q.Participant.Security == (int)ProjectTeamSecurity.Milestone))
                .Select(q=> q.Query);
            return query;
        }

        private Task ToTask(QueryTask query, string responsibles)
        {
            var deadline = query.Task.Deadline;
            var startDate = query.Task.StartDate;
            var task = new Task
            {
                Project = query.Project != null ? DaoFactory.GetProjectDao().ToProject(query.Project) : null,
                ID = query.Task.Id,
                Title = query.Task.Title,
                CreateBy = ToGuid(query.Task.CreateBy),
                CreateOn = TenantUtil.DateTimeFromUtc(query.Task.CreateOn),
                LastModifiedBy = ToGuid(query.Task.LastModifiedBy),
                LastModifiedOn = TenantUtil.DateTimeFromUtc(query.Task.LastModifiedOn),
                Description = query.Task.Description,
                Priority = (TaskPriority)query.Task.Priority,
                Status = (TaskStatus)query.Task.Status,
                Milestone = query.Task.MilestoneId.GetValueOrDefault(),
                SortOrder = query.Task.SortOrder,
                Deadline = !deadline.Equals(DateTime.MinValue) ? DateTime.SpecifyKind(deadline, DateTimeKind.Local) : default(DateTime),
                StartDate = !startDate.Equals(DateTime.MinValue) ? DateTime.SpecifyKind(startDate, DateTimeKind.Local) : default(DateTime),
                Progress = query.Task.Progress,
                Responsibles = !string.IsNullOrEmpty(responsibles) ? new List<Guid>(responsibles.Split(',').Select(ToGuid)) : new List<Guid>(),
                SubTasks = new List<Subtask>(),
                CustomTaskStatus = query.Task.StatusId.GetValueOrDefault()
            };

            return task;
        }

        public DbTask ToDbTask(Task task)
        {
            return new DbTask
            {
                Id = task.ID,
                Title = task.Title,
                CreateBy = task.CreateBy.ToString(),
                CreateOn = TenantUtil.DateTimeFromUtc(task.CreateOn),
                LastModifiedBy = task.LastModifiedBy.ToString(),
                LastModifiedOn = TenantUtil.DateTimeFromUtc(task.LastModifiedOn),
                Description = task.Description,
                Priority = (int)task.Priority,
                Status = (int)task.Status,
                MilestoneId = task.Milestone,
                SortOrder = task.SortOrder,
                Deadline = task.Deadline,
                StartDate = task.StartDate,
                Progress = task.Progress,
                StatusId = task.CustomTaskStatus
            };
        }

        #endregion
    }

    public class QueryTask
    {
        public DbTask Task { get; set; }
        public DbProject Project { get; set; }
        public DbTasksResponsible TasksResponsible { get; set; }
        public DbMilestone Milestone { get; set; }
    }
}
