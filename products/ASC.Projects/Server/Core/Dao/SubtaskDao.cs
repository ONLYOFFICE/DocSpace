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
using ASC.Common;

namespace ASC.Projects.Data.DAO
{/*
    internal class CachedSubtaskDao : SubtaskDao
    {
        private readonly HttpRequestDictionary<DbSubtask> _subtaskCache = new HttpRequestDictionary<DbSubtask>("subtask");

        public CachedSubtaskDao() : base()
        {
        }

        public override void Delete(int id)
        {
            ResetCache(id);
            base.Delete(id);
        }

        public override DbSubtask GetById(int id)
        {
            return _subtaskCache.Get(id.ToString(CultureInfo.InvariantCulture), () => GetBaseById(id));
        }

        private DbSubtask GetBaseById(int id)
        {
            return base.GetById(id);
        }

        public override DbSubtask Save(DbSubtask subtask)
        {
            if (subtask != null)
            {
                ResetCache(subtask.ID);
            }
            return base.Save(subtask);
        }

        private void ResetCache(int subtaskId)
        {
            _subtaskCache.Reset(subtaskId.ToString(CultureInfo.InvariantCulture));
        }
    }*/
    [Scope]
    public class SubtaskDao : BaseDao, ISubtaskDao
    {
        private TenantUtil TenantUtil { get; set; }

        public SubtaskDao(SecurityContext securityContext, DbContextManager<WebProjectsContext> dbContextManager, TenantUtil tenantUtil, TenantManager tenantManager) : base(securityContext, dbContextManager, tenantManager)
        {
            TenantUtil = tenantUtil;
        }

        public List<Subtask> GetSubtasks(int taskid)
        {
            var query = WebProjectsContext.Subtask.Where(s => s.TaskId == taskid && s.TenantId == Tenant);
            return OrderQuery(query)
                .ToList()
                .ConvertAll(q => ToSubTask(q));
        }

        public void GetSubtasksForTasks(ref List<Task> tasks)
        {
            var tmpTask = tasks;
            var taskIds = tmpTask.Select(t => t.ID).ToArray();

            var subtasks = WebProjectsContext.Subtask.Where(s => s.TenantId == Tenant && taskIds.Contains(s.TaskId))
                .ToList()
                .ConvertAll(s => ToSubTask(s));

            tasks = tasks
                .GroupJoin(subtasks, task => task.ID, subtask => subtask.Task, (task, subtaskCol) =>
                {
                    task.SubTasks.AddRange(subtaskCol.ToList());
                    return task;
                }).ToList();
        }

        public virtual Subtask GetById(int id)
        {
            var query =  WebProjectsContext.Subtask
                .Where(s => s.TenantId == Tenant && s.Id == id)
                .SingleOrDefault();

            return query == null ? null : ToSubTask(query);
        }

        public List<Subtask> GetById(ICollection<int> ids)
        {
            var query = WebProjectsContext.Subtask
                .Where(s => s.TenantId == Tenant && ids.Contains(s.Id));

            return OrderQuery(query)
            .ToList()
            .ConvertAll(q => ToSubTask(q));
        }

        public List<Subtask> GetUpdates(DateTime from, DateTime to)
        {
            var query = WebProjectsContext.Subtask.Where(s => (s.CreateOn >= from && s.CreateOn <= to) || (s.LastModifiedOn >= from && s.LastModifiedOn <= to) || (s.StatusChanged >= from && s.StatusChanged <= to));
            return OrderQuery(query)
            .ToList()
            .ConvertAll(q => ToSubTask(q));
        }

        public List<Subtask> GetByResponsible(Guid id, TaskStatus? status = null)
        {
            var query = WebProjectsContext.Subtask.Where(s => s.ResponsibleId == id.ToString());

            if (status.HasValue)
            {
                query = query.Where(s => (TaskStatus)s.Status == status.Value);
            }


            return OrderQuery(query)
            .ToList()
            .ConvertAll(q => ToSubTask(q));
        }

        public int GetSubtaskCount(int taskid, params TaskStatus[] statuses)
        {
            var query = WebProjectsContext.Subtask.Where(s => s.TaskId == taskid);
            if (statuses != null && 0 < statuses.Length)
            {
                query = query.Where(s => statuses.Contains((TaskStatus)s.Status));
            }
            return query.Count();
        }

        public virtual Subtask SaveOrUpdate(Subtask subtask)
        {
            if (WebProjectsContext.Subtask.Where(s => s.Id == subtask.ID).Any())
            {
                var dbSubtask = WebProjectsContext.Subtask.Where(s => s.Id == subtask.ID).SingleOrDefault();
                dbSubtask.Title = subtask.Title;
                dbSubtask.ResponsibleId = subtask.Responsible.ToString();
                dbSubtask.Status = (int)subtask.Status;
                dbSubtask.CreateBy = subtask.CreateBy.ToString();
                dbSubtask.LastModifiedBy = subtask.LastModifiedBy.ToString();
                dbSubtask.TaskId = subtask.Task;
                dbSubtask.TenantId = Tenant;
                dbSubtask.CreateOn = TenantUtil.DateTimeToUtc(dbSubtask.CreateOn);
                dbSubtask.LastModifiedOn = TenantUtil.DateTimeToUtc(dbSubtask.LastModifiedOn);
                dbSubtask.StatusChanged = TenantUtil.DateTimeToUtc(dbSubtask.StatusChanged);
                WebProjectsContext.Subtask.Update(dbSubtask);
                WebProjectsContext.SaveChanges();
                return subtask;
            }
            else
            {
                var dbSubtask = ToDbSubTask(subtask);
                dbSubtask.CreateOn = TenantUtil.DateTimeToUtc(dbSubtask.CreateOn);
                dbSubtask.LastModifiedOn = TenantUtil.DateTimeToUtc(dbSubtask.LastModifiedOn);
                dbSubtask.StatusChanged = TenantUtil.DateTimeToUtc(dbSubtask.StatusChanged);
                WebProjectsContext.Subtask.Add(dbSubtask);
                WebProjectsContext.SaveChanges();
                subtask.ID = dbSubtask.Id;
                return subtask;
            }
            
        }

        public void CloseAllSubtasks(Task task)
        {
            var query = WebProjectsContext.Subtask.Where(s => (TaskStatus)s.Status == TaskStatus.Open && s.TaskId == task.ID).ToList();
            WebProjectsContext.Subtask.UpdateRange(query);
            WebProjectsContext.SaveChanges();
        }

        public virtual void Delete(int id)
        {
            var query = WebProjectsContext.Subtask.Where(s => s.Id == id).SingleOrDefault();
            WebProjectsContext.Remove(query);
            WebProjectsContext.SaveChanges();
        }

        private IOrderedQueryable<DbSubtask> OrderQuery(IQueryable<DbSubtask> subtasks)
        {
            return subtasks
                .OrderBy(s => s.Status)
                .ThenBy(s => (int)s.Status == 1 ? s.CreateOn : s.StatusChanged);
        }

        private Subtask ToSubTask(DbSubtask subtask)
        {
            return new Subtask
            {
                ID = subtask.Id,
                Title = subtask.Title,
                Responsible = ToGuid(subtask.ResponsibleId),
                Status = (TaskStatus)subtask.Status,
                CreateBy = ToGuid(subtask.CreateBy),
                CreateOn = TenantUtil.DateTimeFromUtc(subtask.CreateOn),
                LastModifiedBy = ToGuid(subtask.LastModifiedBy),
                LastModifiedOn = TenantUtil.DateTimeFromUtc(subtask.LastModifiedOn),
                Task = subtask.TaskId
            };
        }

        public DbSubtask ToDbSubTask(Subtask subtask)
        {
            return new DbSubtask
            {
                Id = subtask.ID,
                Title = subtask.Title,
                ResponsibleId = subtask.Responsible.ToString(),
                Status = (int)subtask.Status,
                CreateBy = subtask.CreateBy.ToString(),
                CreateOn = TenantUtil.DateTimeFromUtc(subtask.CreateOn),
                LastModifiedBy = subtask.LastModifiedBy.ToString(),
                LastModifiedOn = TenantUtil.DateTimeFromUtc(subtask.LastModifiedOn),
                TaskId = subtask.Task,
                TenantId = Tenant
            };
        }
    }
}
