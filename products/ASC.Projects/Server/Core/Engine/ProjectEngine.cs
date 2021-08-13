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
using ASC.Projects.Core.Services.NotifyService;
using ASC.Projects.EF;

using IFactoryFileDao = ASC.Files.Core.IDaoFactory;

namespace ASC.Projects.Engine
{
    [Scope]
    public class ProjectEngine
    {
        public NotifyClient NotifyClient { get; set; }
        public ProjectSecurity ProjectSecurity { get; set; }
        public bool DisableNotifications { get; set; }
        public Func<Project, bool> CanReadDelegate { get; set; }
        public FactoryIndexer<DbProject> FactoryIndexer { get; set; }
        public IFactoryFileDao FactoryFileDao { get; set; }
        public SecurityContext SecurityContext { get; set; }
        public TenantUtil TenantUtil { get; set; }
        public ParticipantHelper ParticipantHelper { get; set; }
        public EngineFactory EngineFactory { get; set; }
        public IDaoFactory DaoFactory { get; set; }

        public ProjectEngine(IFactoryFileDao factoryFileDao, SecurityContext securityContext, TenantUtil tenantUtil, ParticipantHelper participantHelper, NotifyClient notifyClient, IDaoFactory daoFactory, EngineFactory engineFactory, ProjectSecurity projectSecurity, FactoryIndexer<DbProject> factoryIndexer)
        {
            CanReadDelegate = CanRead;
            FactoryFileDao = factoryFileDao;
            SecurityContext = securityContext;
            TenantUtil = tenantUtil;
            ParticipantHelper = participantHelper;
            NotifyClient = notifyClient;
            EngineFactory = engineFactory;
            DaoFactory = daoFactory;
            ProjectSecurity = projectSecurity;
            FactoryIndexer = factoryIndexer;
        }


        #region Get Projects

        public IEnumerable<Project> GetAll()
        {
            return DaoFactory.GetProjectDao().GetAll(null, 0).Where(CanReadDelegate);
        }

        public IEnumerable<Project> GetAll(ProjectStatus status, int max)
        {
            return DaoFactory.GetProjectDao().GetAll(status, max)
                .Where(CanReadDelegate);
        }

        public IEnumerable<Project> GetLast(ProjectStatus status, int max)
        {
            var offset = 0;
            var lastProjects = new List<Project>();

            do
            {
                var projects = DaoFactory.GetProjectDao().GetLast(status, offset, max);

                if (!projects.Any())
                    return lastProjects;

                lastProjects.AddRange(projects.Where(CanReadDelegate));
                offset = offset + max;
            } while (lastProjects.Count < max);

            return lastProjects.Count == max ? lastProjects : lastProjects.GetRange(0, max);
        }

        public IEnumerable<Project> GetOpenProjectsWithTasks(Guid participantId)
        {
            return DaoFactory.GetProjectDao().GetOpenProjectsWithTasks(participantId).Where(CanReadDelegate).ToList();
        }

        public IEnumerable<Project> GetByParticipant(Guid participant)
        {
            return DaoFactory.GetProjectDao().GetByParticipiant(participant, ProjectStatus.Open).Where(CanReadDelegate).ToList();
        }

        public IEnumerable<Project> GetByFilter(TaskFilter filter)
        {
            return DaoFactory.GetProjectDao().GetByFilter(filter, ProjectSecurity.CurrentUserAdministrator, ProjectSecurity.IsPrivateDisabled);
        }

        public int GetByFilterCount(TaskFilter filter)
        {
            return DaoFactory.GetProjectDao().GetByFilterCount(filter, ProjectSecurity.CurrentUserAdministrator, ProjectSecurity.IsPrivateDisabled);
        }

        public IEnumerable<Project> GetFollowing(Guid participant)
        {
            return DaoFactory.GetProjectDao().GetFollowing(participant).Where(CanReadDelegate).ToList();
        }

        public Project GetByID(int projectID)
        {
            return GetByID(projectID, true);
        }

        public Project GetByID(int projectID, bool checkSecurity)
        {
            var project = DaoFactory.GetProjectDao().GetById(projectID);

            if (!checkSecurity)
                return project;

            return CanRead(project) ? project : null;
        }

        public Project GetFullProjectByID(int projectID)
        {
            var project = DaoFactory.GetProjectDao().GetById(projectID);

            if (!CanRead(project)) return null;

            var filter = new TaskFilter
            {
                ProjectIds = new List<int> { projectID },
                MilestoneStatuses = new List<MilestoneStatus> { MilestoneStatus.Open }
            };
            var taskCount = EngineFactory.GetTaskEngine().GetByFilterCount(filter);

            project.MilestoneCount = EngineFactory.GetMilestoneEngine().GetByFilterCount(filter);
            project.TaskCount = taskCount.TasksOpen;
            project.TaskCountTotal = taskCount.TasksTotal;
            project.DiscussionCount = EngineFactory.GetMessageEngine().GetByFilterCount(filter);

            var folderDao = FactoryFileDao.GetFolderDao<int>();
            var folderId = EngineFactory.GetFileEngine().GetRoot(projectID);
            project.DocumentsCount = folderDao.GetItemsCount(folderId);

            project.TimeTrackingTotal = EngineFactory.GetTimeTrackingEngine().GetTotalByProject(projectID);
            project.ParticipantCount = GetTeam(projectID).Count();


            return project;
        }

        public IEnumerable<Project> GetByID(List<int> projectIDs, bool checkSecurity = true)
        {
            var projects = DaoFactory.GetProjectDao().GetById(projectIDs);
            if (checkSecurity)
            {
                projects = projects.Where(CanReadDelegate).ToList();
            }
            return projects;
        }

        public bool IsExists(int projectID)
        {
            return DaoFactory.GetProjectDao().IsExists(projectID);
        }

        private bool CanRead(Project project)
        {
            return ProjectSecurity.CanRead(project);
        }

        public DateTime GetMaxLastModified()
        {
            return DaoFactory.GetProjectDao().GetMaxLastModified();
        }

        #endregion

        #region Order

        public void SetTaskOrder(Project project, string order)
        {
            DaoFactory.GetProjectDao().SetTaskOrder(project.ID, order);
        }

        public string GetTaskOrder(Project project)
        {
            return DaoFactory.GetProjectDao().GetTaskOrder(project.ID);
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
            return DaoFactory.GetProjectDao().GetTaskCount(projectId, taskStatus, ProjectSecurity.CurrentUserAdministrator);
        }

        public int GetMilestoneCount(int projectId, params MilestoneStatus[] milestoneStatus)
        {
            return DaoFactory.GetProjectDao().GetMilestoneCount(projectId, milestoneStatus);
        }

        public int GetMessageCount(int projectId)
        {
            return DaoFactory.GetProjectDao().GetMessageCount(projectId);
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

                DaoFactory.GetProjectDao().Create(project);

                _ = FactoryIndexer.IndexAsync(DaoFactory.GetProjectDao().ToDbProject(project));
            }
            else
            {
                var oldProject = DaoFactory.GetProjectDao().GetById(project.ID);
                ProjectSecurity.DemandEdit(oldProject);

                DaoFactory.GetProjectDao().Update(project);

                if (!project.Private) ResetTeamSecurity(oldProject);

                _ = FactoryIndexer.UpdateAsync(DaoFactory.GetProjectDao().ToDbProject(project));
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

            DaoFactory.GetProjectDao().Update(project);

            return project;
        }

        public virtual void Delete(int projectId)
        {
            var project = GetByID(projectId);
            if (project == null) return;

            ProjectSecurity.DemandEdit(project);

            EngineFactory.GetFileEngine().RemoveRoot(projectId);

            List<int> messages, tasks;
            DaoFactory.GetProjectDao().Delete(projectId, out messages, out tasks);

            NotifyClient.SendAboutProjectDeleting(new HashSet<Guid> { project.Responsible }, project);

            EngineFactory.GetMessageEngine().UnSubscribeAll(messages.Select(r => new Message { Project = project, ID = r }).ToList());
            EngineFactory.GetTaskEngine().UnSubscribeAll(tasks.Select(r => new Task { Project = project, ID = r }).ToList());

            _ = FactoryIndexer.DeleteAsync(DaoFactory.GetProjectDao().ToDbProject(project));
        }

        #endregion

        #region Contacts

        public IEnumerable<Project> GetProjectsByContactID(int contactId)
        {
            return DaoFactory.GetProjectDao().GetByContactID(contactId).Where(CanReadDelegate);
        }

        public void AddProjectContact(int projectId, int contactId)
        {
            DaoFactory.GetProjectDao().AddProjectContact(projectId, contactId);
        }

        public void DeleteProjectContact(int projectId, int contactId)
        {
            DaoFactory.GetProjectDao().DeleteProjectContact(projectId, contactId);
        }

        #endregion

        #region Team

        public IEnumerable<Participant> GetTeam(int projectId)
        {
            var project = GetByID(projectId);
            return DaoFactory.GetProjectDao().GetTeam(project).Where(r => !r.UserInfo.Equals(ASC.Core.Users.Constants.LostUser)).ToList();
        }

        public IEnumerable<Participant> GetProjectTeamExcluded(int projectId)
        {
            var project = GetByID(projectId);
            return DaoFactory.GetProjectDao().GetTeam(project, true).Where(r => !r.UserInfo.Equals(ASC.Core.Users.Constants.LostUser)).ToList();
        }

        public IEnumerable<Participant> GetTeam(List<int> projectIds)
        {
            var projects = GetByID(projectIds);
            return DaoFactory.GetProjectDao().GetTeam(projects).Where(r => !r.UserInfo.Equals(ASC.Core.Users.Constants.LostUser)).ToList();
        }

        public bool IsInTeam(int project, Guid participant)
        {
            return DaoFactory.GetProjectDao().IsInTeam(project, participant);
        }

        public bool IsFollow(int project, Guid participant)
        {
            return DaoFactory.GetProjectDao().IsFollow(project, participant);
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
            DaoFactory.GetProjectDao().AddToTeam(project.ID, participant);

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
            DaoFactory.GetProjectDao().RemoveFromTeam(project.ID, participant);

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
                DaoFactory.GetParticipantDao().RemoveFromFollowingProjects(project.ID, participant.ID);
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

            var security = DaoFactory.GetProjectDao().GetTeamSecurity(project.ID, participant);
            if (visible)
            {
                if (security != ProjectTeamSecurity.None) security ^= teamSecurity;
            }
            else
            {
                security |= teamSecurity;
            }
            DaoFactory.GetProjectDao().SetTeamSecurity(project.ID, participant, security);
        }

        public void SetTeamSecurity(Project project, Guid participant, ProjectTeamSecurity teamSecurity)
        {
            if (project == null) throw new ArgumentNullException("project");

            ProjectSecurity.DemandEditTeam(project);

            DaoFactory.GetProjectDao().SetTeamSecurity(project.ID, participant, teamSecurity);
        }

        public void SetTeamSecurity(int projectId, Participant participant)
        {
            DaoFactory.GetProjectDao().SetTeamSecurity(projectId, participant.ID, participant.ProjectTeamSecurity);
        }

        public void ResetTeamSecurity(Project project)
        {
            if (project == null) throw new ArgumentNullException("project");

            ProjectSecurity.DemandEditTeam(project);

            var participant = GetTeam(project.ID);

            foreach (var part in participant)
            {
                DaoFactory.GetProjectDao().SetTeamSecurity(project.ID, part.ID, ProjectTeamSecurity.None);
            }

        }

        public bool GetTeamSecurity(Project project, Participant participant, ProjectTeamSecurity teamSecurity)
        {
            if (project == null) throw new ArgumentNullException("project");
            if (participant == null) throw new ArgumentNullException("participant");

            var security = DaoFactory.GetProjectDao().GetTeamSecurity(project.ID, participant.ID);
            return (security & teamSecurity) != teamSecurity;
        }

        public ProjectTeamSecurity GetTeamSecurity(Project project, Guid participant)
        {
            if (project == null) throw new ArgumentNullException("project");

            return DaoFactory.GetProjectDao().GetTeamSecurity(project.ID, participant);
        }

        public IEnumerable<ParticipantFull> GetTeamUpdates(DateTime from, DateTime to)
        {
            return DaoFactory.GetProjectDao().GetTeamUpdates(from, to).Where(x => CanRead(x.Project));
        }

        public DateTime GetTeamMaxLastModified()
        {
            return DaoFactory.GetProjectDao().GetTeamMaxLastModified();
        }

        #endregion
    }
}
