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

using IFactoryFileDao = ASC.Files.Core.IDaoFactory;

namespace ASC.Projects.Engine
{
    [Scope]
    public class ProjectEngine
    {
        public TaskEngine TaskEngine { get; set; }
        public MilestoneEngine MilestoneEngine { get; set; }
        public MessageEngine MessageEngine { get; set; }
        public FileEngine FileEngine { get; set; }
        public TimeTrackingEngine TimeTrackingEngine { get; set; }
        public NotifyClient NotifyClient { get; set; }
        public IProjectDao ProjectDao { get; set; }
        public IParticipantDao ParticipantDao { get; set; }
        public ProjectSecurity ProjectSecurity { get; set; }
        public bool DisableNotifications { get; set; }

        public Func<Project, bool> CanReadDelegate;
        public FactoryIndexer<DbProject> FactoryIndexer;
        public IFactoryFileDao FactoryFileDao;
        public SecurityContext SecurityContext;
        public TenantUtil TenantUtil;
        public ParticipantHelper ParticipantHelper;

        public ProjectEngine(IFactoryFileDao factoryFileDao, SecurityContext securityContext, TenantUtil tenantUtil, ParticipantHelper participantHelper, NotifyClient notifyClient, IDaoFactory daoFactory, EngineFactory engineFactory)
        {
            CanReadDelegate = CanRead;
            FactoryFileDao = factoryFileDao;
            SecurityContext = securityContext;
            TenantUtil = tenantUtil;
            ParticipantHelper = participantHelper;
            NotifyClient = notifyClient;
            ProjectDao = daoFactory.GetProjectDao();
            ParticipantDao = daoFactory.GetParticipantDao();
            TaskEngine = engineFactory.GetTaskEngine();
            MilestoneEngine = engineFactory.GetMilestoneEngine();
            MessageEngine = engineFactory.GetMessageEngine();
            FileEngine = engineFactory.GetFileEngine();
            TimeTrackingEngine = engineFactory.GetTimeTrackingEngine();
        }

        public ProjectEngine Init(bool disableNotificationParameter)
        {
            DisableNotifications = disableNotificationParameter;
            return this;
        }

        #region Get Projects

        public IEnumerable<Project> GetAll()
        {
            return ProjectDao.GetAll(null, 0).Where(CanReadDelegate);
        }

        public IEnumerable<Project> GetAll(ProjectStatus status, int max)
        {
            return ProjectDao.GetAll(status, max)
                .Where(CanReadDelegate);
        }

        public IEnumerable<Project> GetLast(ProjectStatus status, int max)
        {
            var offset = 0;
            var lastProjects = new List<Project>();

            do
            {
                var projects = ProjectDao.GetLast(status, offset, max);

                if (!projects.Any())
                    return lastProjects;

                lastProjects.AddRange(projects.Where(CanReadDelegate));
                offset = offset + max;
            } while (lastProjects.Count < max);

            return lastProjects.Count == max ? lastProjects : lastProjects.GetRange(0, max);
        }

        public IEnumerable<Project> GetOpenProjectsWithTasks(Guid participantId)
        {
            return ProjectDao.GetOpenProjectsWithTasks(participantId).Where(CanReadDelegate).ToList();
        }

        public IEnumerable<Project> GetByParticipant(Guid participant)
        {
            return ProjectDao.GetByParticipiant(participant, ProjectStatus.Open).Where(CanReadDelegate).ToList();
        }

        public IEnumerable<Project> GetByFilter(TaskFilter filter)
        {
            return ProjectDao.GetByFilter(filter, ProjectSecurity.CurrentUserAdministrator, ProjectSecurity.IsPrivateDisabled);
        }

        public int GetByFilterCount(TaskFilter filter)
        {
            return ProjectDao.GetByFilterCount(filter, ProjectSecurity.CurrentUserAdministrator, ProjectSecurity.IsPrivateDisabled);
        }

        public IEnumerable<Project> GetFollowing(Guid participant)
        {
            return ProjectDao.GetFollowing(participant).Where(CanReadDelegate).ToList();
        }

        public Project GetByID(int projectID)
        {
            return GetByID(projectID, true);
        }

        public Project GetByID(int projectID, bool checkSecurity)
        {
            var project = ProjectDao.GetById(projectID);

            if (!checkSecurity)
                return project;

            return CanRead(project) ? project : null;
        }

        public Project GetFullProjectByID(int projectID)
        {
            var project = ProjectDao.GetById(projectID);

            if (!CanRead(project)) return null;

            var filter = new TaskFilter
            {
                ProjectIds = new List<int> { projectID },
                MilestoneStatuses = new List<MilestoneStatus> { MilestoneStatus.Open }
            };
            var taskCount = TaskEngine.GetByFilterCount(filter);

            project.MilestoneCount = MilestoneEngine.GetByFilterCount(filter);
            project.TaskCount = taskCount.TasksOpen;
            project.TaskCountTotal = taskCount.TasksTotal;
            project.DiscussionCount = MessageEngine.GetByFilterCount(filter);

            var folderDao = FactoryFileDao.GetFolderDao<int>();
            var folderId = FileEngine.GetRoot(projectID);
            project.DocumentsCount = folderDao.GetItemsCount(folderId);

            project.TimeTrackingTotal = TimeTrackingEngine.GetTotalByProject(projectID);
            project.ParticipantCount = GetTeam(projectID).Count();


            return project;
        }

        public IEnumerable<Project> GetByID(List<int> projectIDs, bool checkSecurity = true)
        {
            var projects = ProjectDao.GetById(projectIDs);
            if (checkSecurity)
            {
                projects = projects.Where(CanReadDelegate).ToList();
            }
            return projects;
        }

        public bool IsExists(int projectID)
        {
            return ProjectDao.IsExists(projectID);
        }

        private bool CanRead(Project project)
        {
            return ProjectSecurity.CanRead(project);
        }

        public DateTime GetMaxLastModified()
        {
            return ProjectDao.GetMaxLastModified();
        }

        #endregion

        #region Order

        public void SetTaskOrder(Project project, string order)
        {
            ProjectDao.SetTaskOrder(project.ID, order);
        }

        public string GetTaskOrder(Project project)
        {
            return ProjectDao.GetTaskOrder(project.ID);
        }

        #endregion

        #region Get Counts

        public virtual int CountOpen()
        {
            return Count(ProjectStatus.Open);
        }

        public int Count(ProjectStatus? status = null)
        {
            var filter = new TaskFilter();

            if (status.HasValue)
            {
                filter.ProjectStatuses = new List<ProjectStatus> { status.Value };
            }

            return GetByFilterCount(filter);
        }

        public int GetTaskCount(int projectId, TaskStatus? taskStatus)
        {
            return GetTaskCount(new List<int> { projectId }, taskStatus).First();
        }

        public IEnumerable<int> GetTaskCount(List<int> projectId, TaskStatus? taskStatus)
        {
            return ProjectDao.GetTaskCount(projectId, taskStatus, ProjectSecurity.CurrentUserAdministrator);
        }

        public int GetMilestoneCount(int projectId, params MilestoneStatus[] milestoneStatus)
        {
            return ProjectDao.GetMilestoneCount(projectId, milestoneStatus);
        }

        public int GetMessageCount(int projectId)
        {
            return ProjectDao.GetMessageCount(projectId);
        }

        #endregion

        #region Save, Delete

        public Project SaveOrUpdate(Project project, bool notifyManager)
        {
            return SaveOrUpdate(project, notifyManager, false);
        }

        public virtual Project SaveOrUpdate(Project project, bool notifyManager, bool isImport)
        {
            if (project == null) throw new ArgumentNullException("project");

            // check guest responsible
            if (ProjectSecurity.IsVisitor(project.Responsible))
            {
                ProjectSecurity.CreateGuestSecurityException();
            }

            project.LastModifiedBy = SecurityContext.CurrentAccount.ID;
            project.LastModifiedOn = TenantUtil.DateTimeNow();

            if (project.ID == 0)
            {
                if (project.CreateBy == default(Guid)) project.CreateBy = SecurityContext.CurrentAccount.ID;
                if (project.CreateOn == default(DateTime)) project.CreateOn = TenantUtil.DateTimeNow();

                ProjectSecurity.DemandCreate<Project>(null);

                ProjectDao.Create(project);

                _ = FactoryIndexer.IndexAsync(ProjectDao.ToDbProject(project));
            }
            else
            {
                var oldProject = ProjectDao.GetById(project.ID);
                ProjectSecurity.DemandEdit(oldProject);

                var db = ProjectDao.Update(project);

                if (!project.Private) ResetTeamSecurity(oldProject);

                _ = FactoryIndexer.UpdateAsync(ProjectDao.ToDbProject(project));
            }

            if (notifyManager && !DisableNotifications && !project.Responsible.Equals(SecurityContext.CurrentAccount.ID))
                NotifyClient.SendAboutResponsibleByProject(project.Responsible, project);

            return project;
        }

        public Project ChangeStatus(Project project, ProjectStatus status)
        {
            if (project == null) throw new ArgumentNullException("project");
            ProjectSecurity.DemandEdit(project);

            project.LastModifiedBy = SecurityContext.CurrentAccount.ID;
            project.LastModifiedOn = TenantUtil.DateTimeNow();
            project.StatusChangedOn = DateTime.Now;
            project.Status = status;

            ProjectDao.Update(project);

            return project;
        }

        public virtual void Delete(int projectId)
        {
            var project = GetByID(projectId);
            if (project == null) return;

            ProjectSecurity.DemandEdit(project);

            FileEngine.RemoveRoot(projectId);

            List<int> messages, tasks;
            ProjectDao.Delete(projectId, out messages, out tasks);

            NotifyClient.SendAboutProjectDeleting(new HashSet<Guid> { project.Responsible }, project);

            MessageEngine.UnSubscribeAll(messages.Select(r => new Message { Project = project, ID = r }).ToList());
            TaskEngine.UnSubscribeAll(tasks.Select(r => new Task { Project = project, ID = r }).ToList());

            _ = FactoryIndexer.DeleteAsync(ProjectDao.ToDbProject(project));
        }

        #endregion

        #region Contacts

        public IEnumerable<Project> GetProjectsByContactID(int contactId)
        {
            return ProjectDao.GetByContactID(contactId).Where(CanReadDelegate);
        }

        public void AddProjectContact(int projectId, int contactId)
        {
            ProjectDao.AddProjectContact(projectId, contactId);
        }

        public void DeleteProjectContact(int projectId, int contactId)
        {
            ProjectDao.DeleteProjectContact(projectId, contactId);
        }

        #endregion

        #region Team

        public IEnumerable<Participant> GetTeam(int projectId)
        {
            var project = GetByID(projectId);
            return ProjectDao.GetTeam(project).Where(r => !r.UserInfo.Equals(ASC.Core.Users.Constants.LostUser)).ToList();
        }

        public IEnumerable<Participant> GetProjectTeamExcluded(int projectId)
        {
            var project = GetByID(projectId);
            return ProjectDao.GetTeam(project, true).Where(r => !r.UserInfo.Equals(ASC.Core.Users.Constants.LostUser)).ToList();
        }

        public IEnumerable<Participant> GetTeam(List<int> projectIds)
        {
            var projects = GetByID(projectIds);
            return ProjectDao.GetTeam(projects).Where(r => !r.UserInfo.Equals(ASC.Core.Users.Constants.LostUser)).ToList();
        }

        public bool IsInTeam(int project, Guid participant)
        {
            return ProjectDao.IsInTeam(project, participant);
        }

        public bool IsFollow(int project, Guid participant)
        {
            return ProjectDao.IsFollow(project, participant);
        }

        public void AddToTeam(Project project, Participant participant, bool sendNotification)
        {
            if (participant == null) throw new ArgumentNullException("participant");

            AddToTeam(project, participant.ID, sendNotification);
        }

        public void AddToTeam(Project project, Guid participant, bool sendNotification)
        {
            if (project == null) throw new ArgumentNullException("project");

            ProjectSecurity.DemandEditTeam(project);
            ProjectDao.AddToTeam(project.ID, participant);

            if (!DisableNotifications && sendNotification && !project.Responsible.Equals(participant) && participant != SecurityContext.CurrentAccount.ID)
                NotifyClient.SendInvaiteToProjectTeam(participant, project);
        }

        public void RemoveFromTeam(Project project, Participant participant, bool sendNotification)
        {
            if (participant == null) throw new ArgumentNullException("participant");

            RemoveFromTeam(project, participant.ID, sendNotification);
        }

        public void RemoveFromTeam(Project project, Guid participant, bool sendNotification)
        {
            if (project == null) throw new ArgumentNullException("project");
            if (participant == null) throw new ArgumentNullException("participant");

            ProjectSecurity.DemandEditTeam(project);
            ProjectDao.RemoveFromTeam(project.ID, participant);

            if (!DisableNotifications && sendNotification)
                NotifyClient.SendRemovingFromProjectTeam(participant, project);
        }

        public void UpdateTeam(Project project, IEnumerable<Guid> participants, bool notify)
        {
            if (project == null) throw new ArgumentNullException("project");
            if (participants == null) throw new ArgumentNullException("participants");

            ProjectSecurity.DemandEditTeam(project);

            var newTeam = participants.Select(p => ParticipantHelper.InitParticipant(new Participant(p))).ToList();
            var oldTeam = GetTeam(project.ID);

            var removeFromTeam = oldTeam.Where(p => !newTeam.Contains(p) && p.ID != project.Responsible).ToList();
            var inviteToTeam = new List<Participant>();

            foreach (var participant in newTeam.Where(participant => !oldTeam.Contains(participant)))
            {
                ParticipantDao.RemoveFromFollowingProjects(project.ID, participant.ID);
                inviteToTeam.Add(participant);
            }

            foreach (var participant in inviteToTeam)
            {
                AddToTeam(project, participant, notify);
            }

            foreach (var participant in removeFromTeam)
            {
                RemoveFromTeam(project, participant, notify);
            }
        }

        public void SetTeamSecurity(Project project, Participant participant, ProjectTeamSecurity teamSecurity, bool visible)
        {
            if (participant == null) throw new ArgumentNullException("participant");

            SetTeamSecurity(project, participant.ID, teamSecurity, visible);
        }

        public void SetTeamSecurity(Project project, Guid participant, ProjectTeamSecurity teamSecurity, bool visible)
        {
            if (project == null) throw new ArgumentNullException("project");

            ProjectSecurity.DemandEditTeam(project);

            var security = ProjectDao.GetTeamSecurity(project.ID, participant);
            if (visible)
            {
                if (security != ProjectTeamSecurity.None) security ^= teamSecurity;
            }
            else
            {
                security |= teamSecurity;
            }
            ProjectDao.SetTeamSecurity(project.ID, participant, security);
        }

        public void SetTeamSecurity(Project project, Guid participant, ProjectTeamSecurity teamSecurity)
        {
            if (project == null) throw new ArgumentNullException("project");

            ProjectSecurity.DemandEditTeam(project);

            ProjectDao.SetTeamSecurity(project.ID, participant, teamSecurity);
        }

        public void SetTeamSecurity(int projectId, Participant participant)
        {
            ProjectDao.SetTeamSecurity(projectId, participant.ID, participant.ProjectTeamSecurity);
        }

        public void ResetTeamSecurity(Project project)
        {
            if (project == null) throw new ArgumentNullException("project");

            ProjectSecurity.DemandEditTeam(project);

            var participant = GetTeam(project.ID);

            foreach (var part in participant)
            {
                ProjectDao.SetTeamSecurity(project.ID, part.ID, ProjectTeamSecurity.None);
            }

        }

        public bool GetTeamSecurity(Project project, Participant participant, ProjectTeamSecurity teamSecurity)
        {
            if (project == null) throw new ArgumentNullException("project");
            if (participant == null) throw new ArgumentNullException("participant");

            var security = ProjectDao.GetTeamSecurity(project.ID, participant.ID);
            return (security & teamSecurity) != teamSecurity;
        }

        public ProjectTeamSecurity GetTeamSecurity(Project project, Guid participant)
        {
            if (project == null) throw new ArgumentNullException("project");

            return ProjectDao.GetTeamSecurity(project.ID, participant);
        }

        public IEnumerable<ParticipantFull> GetTeamUpdates(DateTime from, DateTime to)
        {
            return ProjectDao.GetTeamUpdates(from, to).Where(x => CanRead(x.Project));
        }

        public DateTime GetTeamMaxLastModified()
        {
            return ProjectDao.GetTeamMaxLastModified();
        }

        #endregion
    }
}
