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

using ASC.Common;
using ASC.Core;
using ASC.Core.Tenants;
using ASC.ElasticSearch;
using ASC.Projects.Core.DataInterfaces;
using ASC.Projects.Core.Domain;
using ASC.Projects.Core.Services.NotifyService;
using ASC.Projects.EF;


namespace ASC.Projects.Engine
{
    [Scope]
    public class SubtaskEngine : ProjectEntityEngine
    {
        private FactoryIndexer<DbSubtask> FactoryIndexer { get; set; }
        private TenantUtil TenantUtil { get; set; }
        private ISubtaskDao SubtaskDao { get; set; }
        private ITaskDao TaskDao { get; set; }

        public SubtaskEngine(FactoryIndexer<DbSubtask> factoryIndexer, SecurityContext securityContext, Files.Core.IDaoFactory daoFactory, NotifySource notifySource, TenantUtil tenantUtil, IDaoFactory daoProjectFactory, NotifyClient notifyClient) : base(securityContext, daoFactory, notifySource, notifyClient)
        {
            FactoryIndexer = factoryIndexer;
            TenantUtil = tenantUtil;
            TaskDao = daoProjectFactory.GetTaskDao();
            SubtaskDao = daoProjectFactory.GetSubtaskDao();
            NotifyClient = notifyClient;
        }

        public SubtaskEngine Init(bool disableNotifications)
        {
            Init(NotifyConstants.Event_NewCommentForTask, disableNotifications);
            return this;
        }

        #region get 

        public List<Task> GetByDate(DateTime from, DateTime to)
        {
            var subtasks = SubtaskDao.GetUpdates(from, to).ToDictionary(x => x.Task, x => x);
            var ids = subtasks.Select(x => x.Value.Task).Distinct().ToList();
            var tasks = TaskDao.GetById(ids);
            foreach (var task in tasks)
            {
                Subtask subtask;
                subtasks.TryGetValue(task.ID, out subtask);
                task.SubTasks.Add(subtask);
            }
            return tasks;
        }

        public List<Task> GetByResponsible(Guid id, TaskStatus? status = null)
        {
            var subtasks = SubtaskDao.GetByResponsible(id, status);
            var ids = subtasks.Select(x => x.Task).Distinct().ToList();
            var tasks = TaskDao.GetById(ids);
            foreach (var task in tasks)
            {
                task.SubTasks.AddRange(subtasks.FindAll(r => r.Task == task.ID));
            }
            return tasks;
        }

        public int GetSubtaskCount(int taskid, params TaskStatus[] statuses)
        {
            return SubtaskDao.GetSubtaskCount(taskid, statuses);
        }

        public int GetSubtaskCount(int taskid)
        {
            return SubtaskDao.GetSubtaskCount(taskid, null);
        }

        public Subtask GetById(int id)
        {
            return SubtaskDao.GetById(id);
        }

        #endregion

        #region Actions 

        public Subtask ChangeStatus(Task task, Subtask subtask, TaskStatus newStatus)
        {
            if (subtask == null) throw new Exception("subtask.Task");
            if (task == null) throw new ArgumentNullException("task");
            if (task.Status == TaskStatus.Closed) throw new Exception("task can't be closed");

            if (subtask.Status == newStatus) return subtask;

            ProjectSecurity.DemandEdit(task, subtask);

            subtask.Status = newStatus;
            subtask.LastModifiedBy = SecurityContext.CurrentAccount.ID;
            subtask.LastModifiedOn = TenantUtil.DateTimeNow();
            subtask.StatusChangedOn = TenantUtil.DateTimeNow();

            if (subtask.Responsible.Equals(Guid.Empty))
                subtask.Responsible = SecurityContext.CurrentAccount.ID;

            var senders = GetSubscribers(task);

            if (task.Status != TaskStatus.Closed && newStatus == TaskStatus.Closed && !DisableNotifications && senders.Count != 0)
                NotifyClient.SendAboutSubTaskClosing(senders, task, subtask);

            if (task.Status != TaskStatus.Closed && newStatus == TaskStatus.Open && !DisableNotifications && senders.Count != 0)
                NotifyClient.SendAboutSubTaskResumed(senders, task, subtask);

            return SubtaskDao.Save(subtask);
        }

        public Subtask SaveOrUpdate(Subtask subtask, Task task)
        {
            if (subtask == null) throw new Exception("subtask.Task");
            if (task == null) throw new ArgumentNullException("task");
            if (task.Status == TaskStatus.Closed) throw new Exception("task can't be closed");

            // check guest responsible
            if (ProjectSecurity.IsVisitor(subtask.Responsible))
            {
                ProjectSecurity.CreateGuestSecurityException();
            }

            var isNew = subtask.ID == default(int); //Task is new
            var oldResponsible = Guid.Empty;

            subtask.LastModifiedBy = SecurityContext.CurrentAccount.ID;
            subtask.LastModifiedOn = TenantUtil.DateTimeNow();

            if (isNew)
            {
                if (subtask.CreateBy == default(Guid)) subtask.CreateBy = SecurityContext.CurrentAccount.ID;
                if (subtask.CreateOn == default(DateTime)) subtask.CreateOn = TenantUtil.DateTimeNow();

                ProjectSecurity.DemandEdit(task);
                subtask = SubtaskDao.Save(subtask);
            }
            else
            {
                var oldSubtask = SubtaskDao.GetById(new[] { subtask.ID }).First();

                if (oldSubtask == null) throw new ArgumentNullException("subtask");

                oldResponsible = oldSubtask.Responsible;

                //changed task
                ProjectSecurity.DemandEdit(task, oldSubtask);
                subtask = SubtaskDao.Save(subtask);
            }

            NotifySubtask(task, subtask, isNew, oldResponsible);

            var senders = new HashSet<Guid> { subtask.Responsible, subtask.CreateBy };
            senders.Remove(Guid.Empty);

            foreach (var sender in senders)
            {
                Subscribe(task, sender);
            }

            _ = FactoryIndexer.IndexAsync(SubtaskDao.ToDbSubTask(subtask));

            return subtask;
        }

        public Subtask Copy(Subtask from, Task task, IEnumerable<Participant> team)
        {
            var subtask = new Subtask
            {
                ID = default(int),
                CreateBy = SecurityContext.CurrentAccount.ID,
                CreateOn = TenantUtil.DateTimeNow(),
                Task = task.ID,
                Title = from.Title,
                Status = from.Status
            };

            if (team.Any(r => r.ID == from.Responsible))
            {
                subtask.Responsible = from.Responsible;
            }

            return SaveOrUpdate(subtask, task);
        }

        private void NotifySubtask(Task task, Subtask subtask, bool isNew, Guid oldResponsible)
        {
            //Don't send anything if notifications are disabled
            if (DisableNotifications) return;

            var recipients = GetSubscribers(task);

            if (!subtask.Responsible.Equals(Guid.Empty) && (isNew || !oldResponsible.Equals(subtask.Responsible)))
            {
                NotifyClient.SendAboutResponsibleBySubTask(subtask, task);
                recipients.RemoveAll(r => r.ID.Equals(subtask.Responsible.ToString()));
            }

            if (isNew)
            {
                NotifyClient.SendAboutSubTaskCreating(recipients, task, subtask);
            }
            else
            {
                NotifyClient.SendAboutSubTaskEditing(recipients, task, subtask);
            }
        }

        public void Delete(Subtask subtask, Task task)
        {
            if (subtask == null) throw new ArgumentNullException("subtask");
            if (task == null) throw new ArgumentNullException("task");

            ProjectSecurity.DemandEdit(task, subtask);
            SubtaskDao.Delete(subtask.ID);

            var recipients = GetSubscribers(task);

            if (recipients.Any())
            {
                NotifyClient.SendAboutSubTaskDeleting(recipients, task, subtask);
            }
        }

        #endregion
    }
}
