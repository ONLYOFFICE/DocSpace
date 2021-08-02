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
using System.Globalization;
using System.Linq;

using ASC.Collections;
using ASC.Core;
using ASC.Core.Common.EF;
using ASC.Core.Common.EF.Context;
using ASC.Projects.EF;
using ASC.Core.Tenants;
using ASC.ElasticSearch;
using ASC.Projects.Core.DataInterfaces;
using ASC.Projects.Core.Domain;

using Newtonsoft.Json.Linq;
using Microsoft.AspNetCore.Http;
using ASC.Common;

namespace ASC.Projects.Data.DAO
{
    /*
    internal class CachedProjectDao : ProjectDao
    {
        private readonly HttpRequestDictionary<DbProject> projectCache = new HttpRequestDictionary<DbProject>("project");

        public CachedProjectDao(int tenantId)
            : base()
        {
        }

        public override void Delete(int projectId, out List<int> messages, out List<int> tasks)
        {
            ResetCache(projectId);
            base.Delete(projectId, out messages, out tasks);
        }

        public override void RemoveFromTeam(int projectId, Guid participantId)
        {
            ResetCache(projectId);
            base.RemoveFromTeam(projectId, participantId);
        }

        public override DbProject Update(DbProject project)
        {
            if (project != null)
            {
                ResetCache(project.ID);
            }
            return base.Update(project);
        }

        public override DbProject GetById(int projectId)
        {
            return projectCache.Get(projectId.ToString(CultureInfo.InvariantCulture), () => GetBaseById(projectId));
        }

        private DbProject GetBaseById(int projectId)
        {
            return base.GetById(projectId);
        }

        public override void AddToTeam(int projectId, Guid participantId)
        {
            ResetCache(projectId);
            base.AddToTeam(projectId, participantId);
        }

        private void ResetCache(int projectId)
        {
            projectCache.Reset(projectId.ToString(CultureInfo.InvariantCulture));
        }

    }
    */
    [Scope]
    public class ProjectDao : BaseDao, IProjectDao
    {
        private HttpRequestDictionary<TeamCacheItem> TeamCache { get; set; }
        private HttpRequestDictionary<List<Guid>> FollowCache { get; set; }
        private TenantUtil TenantUtil { get; set; }
        private FactoryIndexer<DbProject> FactoryIndexer { get; set; }
        private ParticipantHelper ParticipantHelper { get; set; }

        public ProjectDao(SecurityContext securityContext, DbContextManager<WebProjectsContext> dbContextManager, TenantUtil tenantUtil, FactoryIndexer<DbProject> factoryIndexer, ParticipantHelper participantHelper, IHttpContextAccessor accessor, TenantManager tenantManager) : base(securityContext, dbContextManager, tenantManager)
        {
            TenantUtil = tenantUtil;
            FactoryIndexer = factoryIndexer;
            ParticipantHelper = participantHelper;
            TeamCache = new HttpRequestDictionary<TeamCacheItem>(accessor?.HttpContext, "ProjectDao-TeamCacheItem");
            FollowCache = new HttpRequestDictionary<List<Guid>>(accessor?.HttpContext, "ProjectDao-FollowCache");
        }


        public List<Project> GetAll(ProjectStatus? status, int max)
        {
            return WebProjectsContext.Project
                .Where(p => (ProjectStatus)p.Status == status)
                .Take(max)
                .Select(q=> ToProject(q))
                .ToList();
        }

        public List<Project> GetLast(ProjectStatus? status, int offset, int max)
        {
            return WebProjectsContext.Project
                .Where(p => status == null || (ProjectStatus)p.Status == status)
                .Skip(offset)
                .Take(max)
                .Select(q => ToProject(q))
                .ToList();
        }

        public List<Project> GetOpenProjectsWithTasks(Guid participantId)
        {
            var query = WebProjectsContext.Project.Join(WebProjectsContext.Task,
                p => p.Id,
                t => t.ProjectId,
                (p, t) => new
                {
                    Project = p,
                    Task = t
                }).Where(q => q.Project.TenantId == Tenant && q.Task.TenantId == Tenant && (ProjectStatus)q.Project.Status == ProjectStatus.Open);
            if (!participantId.Equals(Guid.Empty))
            {
                query = query.Join(WebProjectsContext.Participant,
                    q => q.Project.Id,
                    ppp => ppp.ProjectId,
                    (q, ppp) => new
                    {
                        Project = q.Project,
                        Participant = ppp,
                        Task = q.Task
                    }).Where(q => q.Participant.Tenant == Tenant && q.Participant.ParticipantId == participantId.ToString() && q.Participant.Removed == 0)
                    .Select(q=> new
                    {
                        Project = q.Project,
                        Task = q.Task
                    });
            }

            return query
                .GroupBy(q => new { q.Project.Id, q.Project })
                .OrderBy(q => q.Key.Project.Title)
                .Select(q=> ToProject(q.Key.Project))
                .ToList();
        }

        public DateTime GetMaxLastModified()
        {
            return TenantUtil.DateTimeFromUtc(WebProjectsContext.Project.Max(p=> p.LastModifiedOn));
        }

        public void UpdateLastModified(int projectId)
        {
            var project = WebProjectsContext.Project.Where(p => p.Id == projectId).SingleOrDefault();
            project.LastModifiedBy = CurrentUserID.ToString();
            project.LastModifiedOn = DateTime.UtcNow;
            WebProjectsContext.Project.Update(project);
            WebProjectsContext.SaveChanges();
        }

        public List<Project> GetByParticipiant(Guid participantId, ProjectStatus status)
        {
            return WebProjectsContext.Project.Join(WebProjectsContext.Participant,
                p => p.Id,
                ppp => ppp.ProjectId,
                (p, ppp) => new
                {
                    Project = p,
                    Participant = ppp
                }).Where(q => q.Participant.Removed == 0 && q.Participant.Tenant == Tenant && q.Project.TenantId == Tenant && (ProjectStatus)q.Project.Status == status && q.Participant.ParticipantId == participantId.ToString())
                .OrderBy(q => q.Project.Title)
                .Select(q=> ToProject(q.Project))
                .ToList();
        }

        public List<Project> GetByFilter(TaskFilter filter, bool isAdmin, bool checkAccess)
        {
            var partQuery = WebProjectsContext.Participant.Join(WebProjectsContext.User,
                ppp => ppp.ParticipantId,
                u => u.Id.ToString(),
                (ppp, u) => new
                {
                    Participant = ppp,
                    User = u
                }).Where(q => q.Participant.Tenant == Tenant && q.User.Tenant == Tenant);

            var millQuery = WebProjectsContext.Milestone
                .Where(m => m.TenantId == Tenant && (MilestoneStatus)m.Status == MilestoneStatus.Open);

            var allTaskQuery = WebProjectsContext.Task
                .Where(t => t.TenantId == Tenant);

            var openTaskQuery = WebProjectsContext.Task
                .Where(t => t.TenantId == Tenant && (TaskStatus)t.Status == TaskStatus.Open);

            var query = WebProjectsContext.Project
                .Where(p => p.TenantId == Tenant)
                .Select(p => new QueryProject
                {
                    Project = p,
                    TaskCount = openTaskQuery.Where(t => t.ProjectId == p.Id).Count(),
                    TaskCountTotal = allTaskQuery.Where(t => t.ProjectId == p.Id).Count(),
                    MilestoneCount = millQuery.Where(m => m.ProjectId == p.Id).Count(),
                    ParticipantCount = partQuery.Where(q => q.Participant.ProjectId == p.Id).Count(),
                });

            if (filter.Max > 0 && filter.Max < 150000)
            {
                query = query.Skip((int)filter.Offset);
                query = query.Take((int)filter.Max * 2);
            }

            query = CreateQueryFilter(query, filter, isAdmin, checkAccess);

            if (!string.IsNullOrEmpty(filter.SortBy))
            {
                var sortColumns = filter.SortColumns["Project"];
                sortColumns.Remove(filter.SortBy);

                // query = query.OrderBy("p." + filter.SortBy, filter.SortOrder);todo

                foreach (var sort in sortColumns.Keys)
                {
                    // query.OrderBy("p." + sort, sortColumns[sort]);todo
                }
            }

            var sortedQuery = query.OrderBy(q => q.Project.Status == 2 ? 1 : (q.Project.Status == 1 ? 2 : 0)).ThenBy(q=> q.Project.Id);

            return sortedQuery
                .Select(q=> ToProjectFull(q))
                .ToList();
        }

        public int GetByFilterCount(TaskFilter filter, bool isAdmin, bool checkAccess)
        {
            var query = WebProjectsContext.Project.Where(p => p.TenantId == Tenant)
                .Select(p=> new QueryProject() { Project = p});

            query = CreateQueryFilter(query, filter, isAdmin, checkAccess);

            return query.Count();
        }

        private IQueryable<QueryProject> CreateQueryFilter(IQueryable<QueryProject> query, TaskFilter filter, bool isAdmin, bool checkAccess)
        {
            if (filter.TagId != 0)
            {
                if (filter.TagId == -1)
                {
                    query = query.Join(WebProjectsContext.TagToProject,
                        q => q.Project.Id,
                        tp => tp.ProjectId,
                        (q, tp) => new
                        {
                            QueryProject = q,
                            TagToProject = tp
                        }).Where(q => q.TagToProject.TagId == null)
                        .Select(q=> q.QueryProject);
                }
                else
                {
                    query = query.Join(WebProjectsContext.TagToProject,
                        q => q.Project.Id,
                        tp => tp.ProjectId,
                        (q, tp) => new
                        {
                            QueryProject = q,
                            TagToProject = tp
                        }).Where(q => q.TagToProject.TagId == filter.TagId)
                        .Select(q => q.QueryProject);
                }
            }

            if (filter.HasUserId || (filter.ParticipantId.HasValue && !filter.ParticipantId.Equals(Guid.Empty)))
            {
                query = query.Where(q => IsExistParticipant(q, filter));
            }

            if (filter.UserId != Guid.Empty)
            {
                query = query.Where(q => q.Project.ResponsibleId == filter.UserId.ToString());
            }

            if (filter.Follow)
            {
                query = query.Join(WebProjectsContext.FollowingProject,
                    q => q.Project.Id,
                    fp => fp.ProjectId,
                    (q, fp) => new
                    {
                        Query = q,
                        FollowingProject = fp
                    }).Where(q => q.FollowingProject.ParticipantId == CurrentUserID.ToString())
                    .Select(q=> q.Query);
            }

            if (filter.ProjectStatuses.Any())
            {
                query = query.Where(q => filter.ProjectStatuses.Contains((ProjectStatus)q.Project.Status));
            }

            if (!string.IsNullOrEmpty(filter.SearchText))
            {
                List<int> projIds;
                if (FactoryIndexer.TrySelectIds(s => s.MatchAll(filter.SearchText), out projIds))
                {
                    query = query.Where(q => projIds.Contains(q.Project.Id));
                }
                else
                {
                    query = query.Where(q => q.Project.Title.Contains(filter.SearchText));
                }
            }

            if (checkAccess)
            {
                query = query.Where(q => q.Project.Private == 0);
            }
            else if (!isAdmin)
            {
                query = query.Where(q => q.Project.Private == 0 || q.Project.ResponsibleId == CurrentUserID.ToString() || q.Project.Private == 1 &&
                WebProjectsContext.Participant.Where(p => p.ProjectId == q.Project.Id && p.Removed == 0 && p.ParticipantId == CurrentUserID.ToString() && p.Tenant == q.Project.TenantId).Any()
                );
            }

            return query;
        }
        private bool IsExistParticipant(QueryProject q, TaskFilter filter)
        {
            var query = WebProjectsContext.Participant.Where(p => q.Project.Id == p.ProjectId && p.Removed == 0 && p.Tenant == q.Project.TenantId);

            if (filter.DepartmentId != Guid.Empty)
            {
                query = query.Join(WebProjectsContext.UserGroup,
                    p => p.ParticipantId,
                    u => u.UserId.ToString(),
                    (p, u) => new
                    {
                        Participant = p,
                        UserGroup = u
                    }).Where(q => q.UserGroup.Removed == false && q.UserGroup.Tenant == q.Participant.Tenant && q.UserGroup.GroupId == filter.DepartmentId)
                    .Select(q=> q.Participant);
            }

            if (filter.ParticipantId.HasValue && !filter.ParticipantId.Equals(Guid.Empty))
            {
                query = query.Where(p => p.ParticipantId == filter.ParticipantId.ToString());
            }
            return query.Any();
        }

        public List<Project> GetFollowing(Guid participantId)
        {
            return WebProjectsContext.Project.Join(WebProjectsContext.FollowingProject,
                p => p.Id,
                fp => fp.ProjectId,
                (p, fp) => new
                {
                    Project = p,
                    FollowingProject = fp
                }).Where(q => q.FollowingProject.ParticipantId == participantId.ToString() && (ProjectStatus)q.Project.Status == ProjectStatus.Open)
                .OrderBy(q => q.Project.CreateOn)
                .Select(q=> ToProject(q.Project))
                .ToList();
        }

        public bool IsFollow(int projectId, Guid participantId)
        {
            var users = FollowCache[projectId.ToString(CultureInfo.InvariantCulture)];
            if (users == null)
            {
                users = WebProjectsContext.FollowingProject
                    .Where(fp => fp.ProjectId == projectId)
                    .Select(fp=> ToGuid(fp.ParticipantId))
                    .ToList();
                FollowCache.Add(projectId.ToString(CultureInfo.InvariantCulture), users);
            }

            return users.Contains(participantId);
        }

        public virtual Project GetById(int projectId)
        {
            return WebProjectsContext.Project
                .Where(p => p.Id == projectId)
                .Select(p=> ToProject(p))
                .SingleOrDefault();
        }

        public List<Project> GetById(List<int> projectIDs)
        {
            return WebProjectsContext.Project
                .Where(p => projectIDs.Contains(p.Id))
                .Select(p => ToProject(p))
                .ToList();
        }

        public bool IsExists(int projectId)
        {
            var count = WebProjectsContext.Project.Where(p => p.Id == projectId).ToList().Count();
            return 0 < count;
        }

        public List<Project> GetByContactID(int contactId)
        {
            var projectIds = WebProjectsContext.CrmToProject.Where(crm => crm.ContactId == contactId)
                .Select(crm => crm.ProjectId).ToList();

            if (!projectIds.Any()) return new List<Project>(0);

            var milestoneCountQuery = WebProjectsContext.Milestone
                .Where(m => (MilestoneStatus)m.Status == MilestoneStatus.Open && m.TenantId == Tenant);

            var taskCountQuery = WebProjectsContext.Task
                .Where(t => (TaskStatus)t.Status == TaskStatus.Open && t.TenantId == Tenant);

            var participantCountQuery = WebProjectsContext.Participant
                .Where(p => p.Removed == 0 && p.Tenant == Tenant);

            var query = WebProjectsContext.Project
                .Where(p=> projectIds.Contains(p.Id) && p.TenantId == Tenant)
                .Select(p=> new QueryProject()
                {
                    Project = p,
                    MilestoneCount = milestoneCountQuery.Where(m=> m.ProjectId == p.Id).Count(),
                    TaskCount = taskCountQuery.Where(t=> t.ProjectId == p.Id).Count(),
                    ParticipantCount = participantCountQuery.Where(par=> par.ProjectId == p.Id).Count()
                }); 

            return query
                .Select(q=> ToProjectFull(q))
                .ToList();
        }

        public void AddProjectContact(int projectID, int contactID)
        {
            var crmToProject = new DbCrmToProject();
            crmToProject.ContactId = contactID;
            crmToProject.ProjectId = projectID;
            WebProjectsContext.CrmToProject.Add(crmToProject);
            WebProjectsContext.SaveChanges();
        }

        public void DeleteProjectContact(int projectID, int contactID)
        {
            var crmToProject = WebProjectsContext.CrmToProject.Where(crm => crm.ProjectId == projectID && crm.ContactId == contactID).SingleOrDefault();
            WebProjectsContext.Remove(crmToProject);
            WebProjectsContext.SaveChanges();
        }

        public int Count()
        {
            return WebProjectsContext.Project
                .Count();
        }

        public List<int> GetTaskCount(List<int> projectId, TaskStatus? taskStatus, bool isAdmin)
        {
            var query = WebProjectsContext.Task
                .Where(t => t.TenantId == Tenant && projectId.Contains(t.ProjectId));

            if (taskStatus.HasValue)
            {
                query = query.Where(t=> (TaskStatus)t.Status == taskStatus.Value);
            }

            if (!isAdmin)
            {
                query = query.Join(WebProjectsContext.Project,
                    t => t.ProjectId,
                    p => p.Id,
                    (t, p) => new
                    {
                        Task = t,
                        Project = p
                    })
                    .Join(WebProjectsContext.TasksResponsible,
                    q => q.Task.Id,
                    ttr => ttr.TaskId,
                    (q, ttr) => new
                    {
                        Task = q.Task,
                        Project = q.Project,
                        TasksToResponsible = ttr
                    })
                    .Join(WebProjectsContext.Participant,
                    q => q.Project.Id,
                    par => par.ProjectId,
                    (q, par) => new
                    {
                        Task = q.Task,
                        Project = q.Project,
                        TasksToResponsible = q.TasksToResponsible,
                        Participant = par
                    })
                    .Where(q => q.Project.TenantId == Tenant && q.TasksToResponsible.TenantId == Tenant && q.TasksToResponsible.ResponsibleId == CurrentUserID.ToString() && q.Participant.Tenant == Tenant && q.Participant.Removed == 0 && q.Participant.ParticipantId == CurrentUserID.ToString() && q.TasksToResponsible.ResponsibleId != null && q.Participant.Security != null && (ProjectTeamSecurity)q.Participant.Security <= ProjectTeamSecurity.Tasks)
                    .Select(q=> q.Task);
            }
            var result = query
                .GroupBy(t=> t.ProjectId)
                .Select(t=> t.Count())
                .ToList();

            return result;
        }

        public int GetMessageCount(int projectId)
        {
            return WebProjectsContext.Message.Where(m => m.ProjectId == projectId).Count();
        }

        public int GetTotalTimeCount(int projectId)
        {
            return WebProjectsContext.TimeTracking.Where(tt => tt.ProjectId == projectId).Count();
        }

        public int GetMilestoneCount(int projectId, params MilestoneStatus[] statuses)
        {
            var query = WebProjectsContext.Milestone.Where(m => m.ProjectId == projectId);
            if (statuses.Any())
            {
                query = query.Where(m => statuses.Contains((MilestoneStatus)m.Status));
            }

            return query.Count();
        }

        public Project Create(Project project)
        {
            project.CreateOn = TenantUtil.DateTimeFromUtc(TenantUtil.DateTimeToUtc(project.CreateOn));
            project.LastModifiedOn = TenantUtil.DateTimeFromUtc(TenantUtil.DateTimeToUtc(project.LastModifiedOn));
            var dbProject = ToDbProject(project);
            WebProjectsContext.Project.Add(dbProject);
            WebProjectsContext.SaveChanges();

            project.ID = dbProject.Id;
            return project;
        }

        public virtual Project Update(Project project)
        {
            project.LastModifiedOn = TenantUtil.DateTimeFromUtc(TenantUtil.DateTimeToUtc(project.LastModifiedOn));
            WebProjectsContext.Update(project);
            WebProjectsContext.SaveChanges();

            return project;
        }

        public virtual void Delete(int projectId, out List<int> messages, out List<int> tasks)
        {
            var messagesQuery = WebProjectsContext.Message.Where(m => m.ProjectId == projectId).ToList();
            messages = messagesQuery.ConvertAll(m => m.Id);

            var milestones = WebProjectsContext.Milestone.Where(m => m.ProjectId == projectId).ToList();

            var tasksQuery = WebProjectsContext.Task.Where(t => t.ProjectId == projectId).ToList();
            tasks = tasksQuery.ConvertAll(t => t.Id);

            if (messagesQuery.Any())
            {
                WebProjectsContext.Message.RemoveRange(messagesQuery);
                var comments = WebProjectsContext.Comment.Where(c => messagesQuery.ConvertAll(m => "Message_" + m.Id).Contains(c.TargetUniqId)).ToList();
                WebProjectsContext.Comment.RemoveRange(comments);
            }
            if (milestones.Any())
            {
                WebProjectsContext.Milestone.RemoveRange(milestones);
                var comments = WebProjectsContext.Comment.Where(c => milestones.ConvertAll(m => "Milestone_" + m.Id).Contains(c.TargetUniqId)).ToList();
                WebProjectsContext.Comment.RemoveRange(comments);
            }
            if (tasksQuery.Any())
            {
                WebProjectsContext.Task.RemoveRange(tasksQuery);

                var comments = WebProjectsContext.Comment.Where(c => milestones.ConvertAll(m => "Task_" + m.Id).Contains(c.TargetUniqId)).ToList();
                WebProjectsContext.Comment.RemoveRange(comments);
                var tasksOrder = WebProjectsContext.TasksOrder.Where(to => to.ProjectId == projectId).ToList();
                WebProjectsContext.TasksOrder.RemoveRange(tasksOrder);
                var tasksResp = WebProjectsContext.TasksResponsible.Where(tr => tasksQuery.ConvertAll(t=> t.Id).Contains(tr.TaskId.GetValueOrDefault())).ToList();
                WebProjectsContext.TasksResponsible.RemoveRange(tasksResp);
                var subtasks = WebProjectsContext.Subtask.Where(s => tasksQuery.ConvertAll(t => t.Id).Contains(s.TaskId)).ToList();
                WebProjectsContext.Subtask.RemoveRange(subtasks);
            }
            var participant = WebProjectsContext.Participant.Where(p => p.ProjectId == projectId && p.Tenant == Tenant).ToList();
            WebProjectsContext.Participant.RemoveRange(participant);
            var followingProject = WebProjectsContext.FollowingProject.Where(fp => fp.ProjectId == projectId).ToList();
            WebProjectsContext.FollowingProject.RemoveRange(followingProject);
            var Tag = WebProjectsContext.TagToProject.Where(tp => tp.ProjectId == projectId).ToList();
            WebProjectsContext.TagToProject.RemoveRange(Tag);
            var TimeTracking = WebProjectsContext.TimeTracking.Where(tt => tt.ProjectId == projectId).ToList();
            WebProjectsContext.TimeTracking.RemoveRange(TimeTracking);
            var projects = WebProjectsContext.Project.Where(p => p.Id == projectId).ToList();
            WebProjectsContext.Project.RemoveRange(projects);

            var tagsId = WebProjectsContext.TagToProject.Select(tp=> tp.TagId).ToList();
            var tags = WebProjectsContext.Tag.Where(t => !tagsId.Contains(t.Id)).ToList();
            WebProjectsContext.Tag.RemoveRange(tags);
            WebProjectsContext.SaveChanges();
        }


        public virtual void AddToTeam(int projectId, Guid participantId)
        {
            var participant = new DbParticipant()
            {
                ProjectId = projectId,
                ParticipantId = participantId.ToString(),
                Created = DateTime.UtcNow,
                Updated = DateTime.UtcNow,
                Removed = 0,
                Tenant = Tenant
            };
            WebProjectsContext.Participant.Add(participant);
            WebProjectsContext.SaveChanges();

            UpdateLastModified(projectId);

            var key = string.Format("{0}|{1}", projectId, participantId);
            var item = TeamCache.Get(key, () => new TeamCacheItem(true, ProjectTeamSecurity.None));
            if (item != null) item.InTeam = true;
        }

        public virtual void RemoveFromTeam(int projectId, Guid participantId)
        {
            var query = WebProjectsContext.Participant
                .Where(p => p.ProjectId == projectId && p.ParticipantId == participantId.ToString() && p.Tenant == Tenant)
                .SingleOrDefault();
            query.Removed = 0;
            query.Updated = DateTime.UtcNow;

            WebProjectsContext.Update(query);

            UpdateLastModified(projectId);

            var key = string.Format("{0}|{1}", projectId, participantId);
            var item = TeamCache.Get(key, () => new TeamCacheItem(true, ProjectTeamSecurity.None));
            if (item != null) item.InTeam = false;
        }

        public bool IsInTeam(int projectId, Guid participantId)
        {
            return GetTeamItemFromCacheOrLoad(projectId, participantId).InTeam;
        }

        public List<Participant> GetTeam(Project project, bool withExcluded = false)
        {
            if (project == null) return new List<Participant>();

            var query = WebProjectsContext.Participant.Join(WebProjectsContext.Project,
                par => par.ProjectId,
                p => p.Id,
                (par, p) => new
                {
                    Participant = par,
                    Project = p
                }).Where(q => q.Participant.Tenant == Tenant && q.Project.TenantId == Tenant && q.Participant.ProjectId == project.ID)
                .Select(q => new QueryParticipant()
                {
                    Participant = q.Participant,
                    IsManager = q.Participant.ProjectId == q.Project.Id
                });

            if (!withExcluded)
            {
                query = query.Where(q => q.Participant.Removed == 0);
            }

            return query
                .Select(q=> ToParticipant(q))
                .ToList();
        }

        public List<Participant> GetTeam(IEnumerable<Project> projects)
        {
             return WebProjectsContext.Participant.Join(WebProjectsContext.Project,
                par => par.ProjectId,
                p => p.Id,
                (par, p) => new
                {
                    Participant = par,
                    Project = p
                }).Where(q => q.Project.TenantId == Tenant && q.Participant.Tenant == Tenant && projects.Select(r => r.ID).ToArray().Contains(q.Participant.ProjectId) && q.Participant.Removed == 0)
                .Select(q => new QueryParticipant()
                {
                    Participant = q.Participant,
                    IsManager = q.Participant.ProjectId == q.Project.Id
                })
                .Distinct()
                .Select(q=> ToParticipant(q))
                .ToList();
        }

        public List<ParticipantFull> GetTeamUpdates(DateTime from, DateTime to)
        {
            return WebProjectsContext.Project.Join(WebProjectsContext.Participant,
                p => p.Id,
                par => par.ProjectId,
                (p, par) => new QueryParticipant
                {
                    Project = p,
                    Participant = par
                }).Where(q => q.Participant.Created >= from && q.Participant.Created <= to && q.Participant.Updated >= from && q.Participant.Updated <= to)
                .Select(q => ToParticipantFull(q))
                .ToList();
        }

        public DateTime GetTeamMaxLastModified()
        {
            return WebProjectsContext.Participant.Where(p => p.Tenant == Tenant)
                .Select(p => p.Updated)
                .Max();
        }

        public void SetTeamSecurity(int projectId, Guid participantId, ProjectTeamSecurity teamSecurity)
        {
            var participan = WebProjectsContext.Participant.Where(p => p.Tenant == Tenant && p.ProjectId == projectId && p.ParticipantId == participantId.ToString()).SingleOrDefault();

            participan.Updated = DateTime.UtcNow;
            participan.Security = (int)teamSecurity;
            WebProjectsContext.Participant.Update(participan);
            WebProjectsContext.SaveChanges();

            var key = string.Format("{0}|{1}", projectId, participantId);
            var item = TeamCache.Get(key);
            if (item != null) TeamCache[key].Security = teamSecurity;
        }

        public ProjectTeamSecurity GetTeamSecurity(int projectId, Guid participantId)
        {
            return GetTeamItemFromCacheOrLoad(projectId, participantId).Security;
        }

        private TeamCacheItem GetTeamItemFromCacheOrLoad(int projectId, Guid participantId)
        {
            var key = string.Format("{0}|{1}", projectId, participantId);

            var item = TeamCache.Get(key);
            if (item != null) return item;

            item = TeamCache.Get(string.Format("{0}|{1}", 0, participantId));
            if (item != null) return new TeamCacheItem(false, ProjectTeamSecurity.None);
            var projectList = WebProjectsContext.Participant.Where(p => p.Tenant == Tenant && p.ParticipantId == participantId.ToString() && p.Removed == 0).ToList();

            var teamCacheItem = new TeamCacheItem(true, ProjectTeamSecurity.None);
            TeamCache.Add(string.Format("{0}|{1}", 0, participantId), teamCacheItem);

            foreach (var prj in projectList)
            {
                teamCacheItem = new TeamCacheItem(true, (ProjectTeamSecurity)Convert.ToInt32(prj.Security));
                key = string.Format("{0}|{1}", prj.Security, participantId);
                TeamCache.Add(key, teamCacheItem);
            }

            var currentProject = projectList.Find(r => Convert.ToInt32(r.ProjectId) == projectId);
            teamCacheItem = new TeamCacheItem(currentProject != null,
                                                currentProject != null
                                                    ? (ProjectTeamSecurity)Convert.ToInt32(currentProject.Security)
                                                    : ProjectTeamSecurity.None);
            key = string.Format("{0}|{1}", projectId, participantId);
            TeamCache.Add(key, teamCacheItem);
            return teamCacheItem;
        }


        public void SetTaskOrder(int projectID, string order)
        {
            var taskOrder = new DbTasksOrder();
            taskOrder.ProjectId = projectID;
            taskOrder.TaskOrder = order;
            WebProjectsContext.TasksOrder.Add(taskOrder);
            
            try
            {
                var orderJson = JObject.Parse(order);
                var newTaskOrder = orderJson["tasks"].Select(r => r.Value<int>()).ToList();

                for (var i = 0; i < newTaskOrder.Count; i++)
                {
                    var task = WebProjectsContext.Task.Where(t => t.ProjectId == projectID && t.Id == newTaskOrder[1]).SingleOrDefault();
                    task.SortOrder = i;
                    WebProjectsContext.Update(task);
                }
            }
            finally
            {
                WebProjectsContext.SaveChanges();
            }
        }

        public string GetTaskOrder(int projectID)
        {
            return WebProjectsContext.TasksOrder.Where(t => t.ProjectId == projectID)
                .Select(t=> t.TaskOrder).FirstOrDefault();
        }
        private class TeamCacheItem
        {
            public bool InTeam { get; set; }

            public ProjectTeamSecurity Security { get; set; }

            public TeamCacheItem(bool inteam, ProjectTeamSecurity security)
            {
                InTeam = inteam;
                Security = security;
            }
        }

        public ParticipantFull ToParticipantFull(QueryParticipant query)
        {
            var participant = new ParticipantFull(ToGuid(query.Participant.ParticipantId))
            {
                Project = ToProject(query.Project),
                Removed = Convert.ToBoolean(query.Participant.Removed),
                Created = TenantUtil.DateTimeFromUtc(query.Participant.Created),
                Updated = TenantUtil.DateTimeFromUtc(query.Participant.Created)
            };
            return ParticipantHelper.InitParticipant(participant);
        }

        public Project ToProject(DbProject dbProject)
        {
            return new Project
            {
                ID = Convert.ToInt32(dbProject.Id),
                Title = dbProject.Title,
                Description = dbProject.Description,
                Status = (ProjectStatus)dbProject.Status,
                Responsible = ToGuid(dbProject.ResponsibleId),
                Private = Convert.ToBoolean(dbProject.Private),
                CreateBy = ToGuid(dbProject.CreateBy),
                CreateOn = TenantUtil.DateTimeFromUtc(dbProject.CreateOn),
                LastModifiedBy = ToGuid(dbProject.LastModifiedBy),
                LastModifiedOn = TenantUtil.DateTimeFromUtc(dbProject.LastModifiedOn)
            };
        }

        public DbProject ToDbProject(Project dbProject)
        {
            return new DbProject
            {
                Id = Convert.ToInt32(dbProject.ID),
                Title = dbProject.Title,
                Description = dbProject.Description,
                Status = (int)dbProject.Status,
                ResponsibleId = dbProject.Responsible.ToString(),
                Private = Convert.ToInt32(dbProject.Private),
                CreateBy = dbProject.CreateBy.ToString(),
                CreateOn = TenantUtil.DateTimeFromUtc(dbProject.CreateOn),
                LastModifiedBy = dbProject.LastModifiedBy.ToString(),
                LastModifiedOn = TenantUtil.DateTimeFromUtc(dbProject.LastModifiedOn)
            };
        }

        public Project ToProjectFull(QueryProject query)
        {
            var project = ToProject(query.Project);

            project.TaskCount = query.TaskCount;
            project.TaskCountTotal = query.TaskCountTotal;
            project.MilestoneCount = query.MilestoneCount;
            project.ParticipantCount = query.ParticipantCount;

            return project;
        }

        public Participant ToParticipant(QueryParticipant query)
        {
            var participant = new Participant(new Guid(query.Participant.ParticipantId))
            {
                ProjectTeamSecurity = (ProjectTeamSecurity)Convert.ToInt32(query.Participant.Security),
                ProjectID = Convert.ToInt32(query.Participant.ProjectId),
                IsManager = Convert.ToBoolean(query.IsManager),
                IsRemovedFromTeam = Convert.ToBoolean(query.Participant.Removed)
            };
            return ParticipantHelper.InitParticipant(participant);
        }
    }

    public class QueryProject
    {
        public DbProject Project { get; internal set; }
        public int TaskCount { get; internal set; }
        public int TaskCountTotal { get; internal set; }
        public int MilestoneCount { get; internal set; }
        public int ParticipantCount { get; internal set; }
    }

    public class QueryParticipant
    {
        public DbParticipant Participant { get; internal set; }
        public bool IsManager { get; internal set; }
        public DbProject Project { get; set; }
    }
}
