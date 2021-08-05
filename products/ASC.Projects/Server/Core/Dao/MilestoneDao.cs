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
    /*
    class CachedMilestoneDao : MilestoneDao
    {
        private readonly HttpRequestDictionary<DbMilestone> projectCache = new HttpRequestDictionary<DbMilestone>("milestone");


        public CachedMilestoneDao(int tenant)
            : base(tenant)
        {
        }

        public override DbMilestone GetById(int id)
        {
            return projectCache.Get(id.ToString(CultureInfo.InvariantCulture), () => GetBaseById(id));
        }

        private DbMilestone GetBaseById(int id)
        {
            return base.GetById(id);
        }

        public override DbMilestone Save(DbMilestone milestone)
        {
            if (milestone != null)
            {
                ResetCache(milestone.ID);
            }
            return base.Save(milestone);
        }

        public override void Delete(int id)
        {
            ResetCache(id);
            base.Delete(id);
        }

        private void ResetCache(int milestoneId)
        {
            projectCache.Reset(milestoneId.ToString(CultureInfo.InvariantCulture));
        }
    }
    */
    [Scope]
    public class MilestoneDao : BaseDao, IMilestoneDao
    {
        private TenantUtil TenantUtil { get; set; }
        private FactoryIndexer<DbMilestone> FactoryIndexer { get; set; }
        private SettingsManager SettingsManager { get; set; }
        private FilterHelper FilterHelper { get; set; }
        private IDaoFactory DaoFactory { get; set; }
        public MilestoneDao(SecurityContext securityContext, DbContextManager<WebProjectsContext> dbContextManager, TenantUtil tenantUtil, FactoryIndexer<DbMilestone> factoryIndexer, IDaoFactory daoFactory, SettingsManager settingsManager, FilterHelper filterHelper, TenantManager tenantManager)
            : base(securityContext, dbContextManager, tenantManager)
        {
            TenantUtil = tenantUtil;
            FactoryIndexer = factoryIndexer;
            SettingsManager = settingsManager;
            FilterHelper = filterHelper;
            DaoFactory = daoFactory;
        }

        public List<Milestone> GetAll()
        {
            return CreateQuery()
                .AsEnumerable()
                .GroupBy(q => new QueryMilestone{ Milestone = q.Milestone, Project = q.Project, ActiveTaskCount = q.ActiveTaskCount, ClosedTaskCount = q.ClosedTaskCount })
                .ToList()
                .ConvertAll(q => ToMilestone(q.Key));
        }

        public List<Milestone> GetByProject(int projectId)
        {
            return CreateQuery()
                .Where(q => q.Milestone.ProjectId == projectId)
                .AsEnumerable()
                .GroupBy(q => new QueryMilestone { Milestone = q.Milestone, Project = q.Project, ActiveTaskCount = q.ActiveTaskCount, ClosedTaskCount = q.ClosedTaskCount })
                .ToList()
                .ConvertAll(q => ToMilestone(q.Key));
        }

        public List<Milestone> GetByFilter(TaskFilter filter, bool isAdmin, bool checkAccess)
        {
            var query = CreateQuery();

            if (filter.Max > 0 && filter.Max < 150000)
            {
                query = query.Skip((int)filter.Offset);
                query = query.Take((int)filter.Max * 2);
            }

            query = CreateQueryFilter(query, filter, isAdmin, checkAccess);

            var groupQuery = query.AsEnumerable()
                .GroupBy(q => new QueryMilestone { Milestone = q.Milestone, Project = q.Project, ActiveTaskCount = q.ActiveTaskCount, ClosedTaskCount = q.ClosedTaskCount });

            var filtredQuery = groupQuery.OrderBy(q => q.Key.Milestone.Status);

            if (!string.IsNullOrEmpty(filter.SortBy))
            {
                var sortColumns = filter.SortColumns["Milestone"];
                sortColumns.Remove(filter.SortBy);

                filtredQuery = filtredQuery.ThenBy(q => filter.SortBy == "create_on" ? q.Key.Milestone.Id : q.Key.Milestone.Id);

                foreach (var sort in sortColumns.Keys)
                {
                    filtredQuery = filtredQuery.ThenBy(q => sort == "create_on" ? q.Key.Milestone.Id : q.Key.Milestone.Id);
                }
            }

            return filtredQuery
                .ToList()
                .ConvertAll(q => ToMilestone(q.Key));
        }

        public int GetByFilterCount(TaskFilter filter, bool isAdmin, bool checkAccess)
        {
            var query = WebProjectsContext.Milestone.Join(WebProjectsContext.Project,
                m => m.ProjectId,
                p => p.Id,
                (m, p) => new QueryMilestone
                {
                    Milestone = m,
                    Project = p
                }).Where(q => q.Project.TenantId == Tenant && q.Milestone.TenantId == Tenant);

            query = CreateQueryFilter(query, filter, isAdmin, checkAccess);

            return query
                .GroupBy(q=> q.Milestone.Id)
                .Count();
        }

        public List<Tuple<Guid, int, int>> GetByFilterCountForReport(TaskFilter filter, bool isAdmin, bool checkAccess)
        {
            filter = (TaskFilter)filter.Clone();

            var query = WebProjectsContext.Milestone.Join(WebProjectsContext.Project,
                m => m.ProjectId,
                p => p.Id,
                (m, p) => new QueryMilestone
                {
                    Milestone = m,
                    Project = p
                }).Where(q => q.Milestone.TenantId == Tenant && q.Project.TenantId == Tenant && q.Milestone.CreateOn >= FilterHelper.GetFromDate(filter) && q.Milestone.CreateOn <= FilterHelper.GetToDate(filter));

            if (filter.HasUserId)
            {
                query = query.Where(q=> FilterHelper.GetUserIds(filter).Contains(q.Milestone.CreateBy));
                filter.UserId = Guid.Empty;
                filter.DepartmentId = Guid.Empty;
            }

            query = CreateQueryFilter(query, filter, isAdmin, checkAccess);

            var queryCount = query.GroupBy(q => new { q.Milestone.CreateBy, q.Milestone.ProjectId })
                .Select(q => new Tuple<Guid, int, int>(ToGuid(q.Key.CreateBy), q.Key.ProjectId, q.Count())).ToList();
            return queryCount.ToList();
        }

        public List<Milestone> GetByStatus(int projectId, MilestoneStatus milestoneStatus)
        {
            return CreateQuery()
                .Where(q=> q.Milestone.ProjectId == projectId && (MilestoneStatus)q.Milestone.Status == milestoneStatus)
                .AsEnumerable()
                .GroupBy(q => new QueryMilestone { Milestone = q.Milestone, Project = q.Project, ActiveTaskCount = q.ActiveTaskCount, ClosedTaskCount = q.ClosedTaskCount })
                .ToList()
                .ConvertAll(q => ToMilestone(q.Key));
        }

        public List<Milestone> GetUpcomingMilestones(int offset, int max, params int[] projects)
        {
            var query = CreateQuery()
                .Where(q => (ProjectStatus)q.Project.Status == ProjectStatus.Open && q.Milestone.Deadline >= TenantUtil.DateTimeNow().Date && (MilestoneStatus)q.Milestone.Status == MilestoneStatus.Open)
                .Skip(offset)
                .Take(max);
            if (projects != null && 0 < projects.Length)
            {
                query.Where(q=> projects.Take(0 < max ? max : projects.Length).ToArray().Contains(q.Project.Id));
            }

            return query
                .OrderBy(q => q.Milestone.Deadline)
                .ToList()
                .ConvertAll(q => ToMilestone(q));
        }

        public List<Milestone> GetLateMilestones(int offset, int max)
        {

            var now = TenantUtil.DateTimeNow();
            var yesterday = now.Date.AddDays(-1);
            return CreateQuery()
                .Where(q => (ProjectStatus)q.Project.Status == ProjectStatus.Open && (MilestoneStatus)q.Milestone.Status != MilestoneStatus.Closed && q.Milestone.Deadline <= yesterday)
                .Skip(offset)
                .Take(max)
                .AsEnumerable()
                .GroupBy(q => new QueryMilestone { Milestone = q.Milestone, Project = q.Project, ActiveTaskCount = q.ActiveTaskCount, ClosedTaskCount = q.ClosedTaskCount })
                .OrderBy(q => q.Key.Milestone.Deadline)
                .ToList()
                .ConvertAll(q => ToMilestone(q.Key));
        }

        public List<Milestone> GetByDeadLine(DateTime deadline)
        {
            return CreateQuery()
                .Where(q => q.Milestone.Deadline == deadline)
                .OrderBy(q => q.Milestone.Deadline)
                .AsEnumerable()
                .GroupBy(q => new QueryMilestone { Milestone = q.Milestone, Project = q.Project, ActiveTaskCount = q.ActiveTaskCount, ClosedTaskCount = q.ClosedTaskCount })
                .ToList()
                .ConvertAll(q => ToMilestone(q.Key));
        }

        public virtual Milestone GetById(int id)
        {
            var query = CreateQuery()
                .Where(q => q.Milestone.Id == id)
                .SingleOrDefault();
            return ToMilestone(query);
        }

        public List<Milestone> GetById(int[] id)
        {
            return CreateQuery()
                .Where(q => id.Contains(q.Milestone.Id))
                .AsEnumerable()
                .GroupBy(q => new QueryMilestone { Milestone = q.Milestone, Project = q.Project, ActiveTaskCount = q.ActiveTaskCount, ClosedTaskCount = q.ClosedTaskCount })
                .ToList()
                .ConvertAll(q => ToMilestone(q.Key)); ;
        }

        public bool IsExists(int id)
        {
            var count = WebProjectsContext.Milestone
                .Where(m => m.Id == id)
                .Count();
            return 0 < count;
        }

        public List<object[]> GetInfoForReminder(DateTime deadline)
        {
            var deadlineDate = deadline.Date;
            return WebProjectsContext.Milestone.Join(WebProjectsContext.Project,
                m => m.ProjectId,
                p => p.Id,
                (m, p) => new
                {
                    Milestone = m,
                    Project = p
                }).Where(q => q.Milestone.TenantId == Tenant && q.Project.TenantId == Tenant && q.Milestone.Deadline >= deadline.AddDays(-1) && q.Milestone.Deadline <= deadline.AddDays(1) && (MilestoneStatus)q.Milestone.Status == MilestoneStatus.Open && (ProjectStatus)q.Project.Status == ProjectStatus.Open && q.Milestone.IsNotify == 1)
                .ToList()
                .ConvertAll(q => new object[] { q.Milestone.TenantId, q.Milestone.Id, q.Milestone.Deadline });
        }

        public virtual Milestone SaveOrUpdate(Milestone milestone)
        {
            if (milestone.DeadLine.Kind != DateTimeKind.Local)
                milestone.DeadLine = TenantUtil.DateTimeFromUtc(milestone.DeadLine);

            milestone.CreateOn = TenantUtil.DateTimeToUtc(milestone.CreateOn);
            milestone.LastModifiedOn = TenantUtil.DateTimeToUtc(milestone.LastModifiedOn);
            if(WebProjectsContext.Milestone.Where(m=> m.Id == milestone.ID).Any())
            {
                var dbMilestone = WebProjectsContext.Milestone.Where(m => m.Id == milestone.ID).SingleOrDefault();
                dbMilestone.Id = milestone.ID;
                dbMilestone.Title = milestone.Title;
                dbMilestone.CreateBy = milestone.CreateBy.ToString();
                dbMilestone.CreateOn = TenantUtil.DateTimeFromUtc(milestone.CreateOn);
                dbMilestone.LastModifiedBy = milestone.LastModifiedBy.ToString();
                dbMilestone.LastModifiedOn = TenantUtil.DateTimeFromUtc(milestone.LastModifiedOn);
                dbMilestone.Deadline = DateTime.SpecifyKind(milestone.DeadLine, DateTimeKind.Local);
                dbMilestone.Status = (int)milestone.Status;
                dbMilestone.IsNotify = Convert.ToInt32(milestone.IsNotify);
                dbMilestone.IsKey = Convert.ToInt32(milestone.IsKey);
                dbMilestone.Description = milestone.Description;
                dbMilestone.ResponsibleId = milestone.Responsible.ToString();
                dbMilestone.TenantId = Tenant;
                dbMilestone.ProjectId = milestone.Project.ID;
                dbMilestone.StatusChanged = milestone.StatusChangedOn;
                WebProjectsContext.Milestone.Update(dbMilestone);
                WebProjectsContext.SaveChanges();
                milestone.ID = dbMilestone.Id;
                return milestone;
            }
            else
            {
                var dbMilestone = ToDbMilestone(milestone);
                WebProjectsContext.Milestone.Add(dbMilestone);
                WebProjectsContext.SaveChanges();
                milestone.ID = dbMilestone.Id;
                return milestone;
            }
        }

        public virtual void Delete(int id)
        {
            var milestone = WebProjectsContext.Milestone.Where(m => m.Id == id).SingleOrDefault();
            WebProjectsContext.Remove(milestone);
            WebProjectsContext.SaveChanges();
        }

        public string GetLastModified()
        {
            var max = WebProjectsContext.Milestone.Max(m => m.LastModifiedOn);
            var count = WebProjectsContext.Milestone.Where(m => m.LastModifiedOn == max).ToList().Count();
            if (count == 0)
            {
                return "";
            }
            var lastModified = "";
            if (max != null)
            {
                lastModified += TenantUtil.DateTimeFromUtc(max.GetValueOrDefault()).ToString(CultureInfo.InvariantCulture);
            }
            if (count != 0)
            {
                lastModified += count;
            }

            return lastModified;
        }

        private IQueryable<QueryMilestone> CreateQuery()
        {
            return WebProjectsContext.Milestone.Join(WebProjectsContext.Project,
                m => m.ProjectId,
                p => p.Id,
                (m, p) => new
                {
                    Milestone = m,
                    Project = p
                })
                .Where(q => q.Milestone.TenantId == Tenant && q.Project.TenantId == Tenant)
                .Select(q => new QueryMilestone
                {
                    Milestone = q.Milestone,
                    Project = q.Project,
                    ActiveTaskCount = WebProjectsContext.Task.Where(t=> t.MilestoneId == q.Milestone.Id && t.TenantId == q.Milestone.TenantId)
                    .Select(t=> t.Status == 1 || t.Status == 4 ? 1 : 0).Count(),
                    ClosedTaskCount = WebProjectsContext.Task.Where(t => t.MilestoneId == q.Milestone.Id && t.TenantId == q.Milestone.TenantId)
                    .Select(t => t.Status == 2 ? 1 : 0).Count()
                });
        }

        private IQueryable<QueryMilestone> CreateQueryFilter(IQueryable<QueryMilestone> query, TaskFilter filter, bool isAdmin, bool checkAccess)
        {
            if (filter.MilestoneStatuses.Count != 0)
            {
                query = query.Where(q=> filter.MilestoneStatuses.Contains((MilestoneStatus)q.Milestone.Status));
            }

            if (filter.ProjectIds.Count != 0)
            {
                query = query.Where(q=> filter.ProjectIds.Contains(q.Milestone.ProjectId));
            }
            else
            {
                if (SettingsManager.Load<ProjectsCommonSettings>().HideEntitiesInPausedProjects)
                {
                    query = query.Where(q=> (ProjectStatus)q.Project.Status != ProjectStatus.Paused);
                }

                if (filter.MyProjects)
                {
                    query = query.Join(WebProjectsContext.Participant,
                        q => q.Project.Id,
                        ppp => ppp.ProjectId,
                        (q, ppp) => new
                        {
                            Milestone = q.Milestone,
                            Project = q.Project,
                            ActiveTaskCount = q.ActiveTaskCount,
                            ClosedTaskCount = q.ClosedTaskCount,
                            Participant = ppp
                        }).Where(q => q.Participant.Removed == 0 && q.Participant.Tenant == q.Milestone.TenantId && ToGuid(q.Participant.ParticipantId) == CurrentUserID)
                        .Select(q=> new QueryMilestone
                        {
                            Milestone = q.Milestone,
                            Project = q.Project,
                            ActiveTaskCount = q.ActiveTaskCount,
                            ClosedTaskCount = q.ClosedTaskCount,
                        });
                }
            }

            if (filter.UserId != Guid.Empty)
            {
                query = query.Where(q=> ToGuid(q.Milestone.ResponsibleId) == filter.UserId);
            }

            if (filter.TagId != 0)
            {
                query = query.Join(WebProjectsContext.TagToProject,
                    q => q.Milestone.ProjectId,
                    tp => tp.ProjectId,
                    (q, tp) => new
                    {
                        Milestone = q.Milestone,
                        Project = q.Project,
                        ActiveTaskCount = q.ActiveTaskCount,
                        ClosedTaskCount = q.ClosedTaskCount,
                        TagToProject = tp
                    }).Where(q => q.TagToProject.TagId == (filter.TagId == -1 ? null : filter.TagId))//todo
                    .Select(q => new QueryMilestone
                    {
                        Milestone = q.Milestone,
                        Project = q.Project,
                        ActiveTaskCount = q.ActiveTaskCount,
                        ClosedTaskCount = q.ClosedTaskCount,
                    });
            }

            if (filter.ParticipantId.HasValue)
            {
                query = query.Join(WebProjectsContext.Task,
                    q => q.Milestone.Id,
                    t => t.MilestoneId,
                    (q, t) => new
                    {
                        Query = q,
                        Task = t
                    }).Where(q => q.Task.TenantId == q.Query.Milestone.TenantId &&
                    WebProjectsContext.Subtask.Where(s => s.TenantId == q.Query.Milestone.TenantId && s.TaskId == q.Task.Id && (TaskStatus)s.Status == TaskStatus.Open && s.ResponsibleId == filter.ParticipantId.ToString()).Any()
                    || WebProjectsContext.TasksResponsible.Where(tr => tr.TenantId == q.Query.Milestone.TenantId && q.Task.Id == tr.TaskId && tr.ResponsibleId == filter.ParticipantId.ToString()).Any())
                    .Select(q => q.Query);
            }

            if (!filter.FromDate.Equals(DateTime.MinValue) && !filter.FromDate.Equals(DateTime.MaxValue))
            {
                query = query.Where(q => q.Milestone.Deadline >= TenantUtil.DateTimeFromUtc(filter.FromDate));
            }

            if (!filter.ToDate.Equals(DateTime.MinValue) && !filter.ToDate.Equals(DateTime.MaxValue))
            {
                query = query.Where(q => q.Milestone.Deadline <= TenantUtil.DateTimeFromUtc(filter.ToDate));
            }

            if (!string.IsNullOrEmpty(filter.SearchText))
            {
                List<int> mIds;
                if (FactoryIndexer.TrySelectIds(s => s.MatchAll(filter.SearchText), out mIds))
                {
                    query = query.Where(q=> mIds.Contains(q.Milestone.Id));
                }
                else
                {
                    query = query.Where(q => q.Milestone.Title.Contains(filter.SearchText));
                }
            }

            query = CheckSecurity(query, filter, isAdmin, checkAccess);

            return query;
        }

        private IQueryable<QueryMilestone> CheckSecurity(IQueryable<QueryMilestone> query, TaskFilter filter, bool isAdmin, bool checkAccess)
        {
            if (checkAccess)
            {
                query = query.Where(q=> q.Project.Private == 0);
                return query;
            }

            if (isAdmin) return query;

            if (!filter.MyProjects && !filter.MyMilestones)
            {
                query = query.Join(WebProjectsContext.Participant,
                    q => q.Milestone.ProjectId,
                    ppp => ppp.ProjectId,
                    (q, ppp) => new
                    {
                        Milestone = q.Milestone,
                        Project = q.Project,
                        ActiveTaskCount = q.ActiveTaskCount,
                        ClosedTaskCount = q.ClosedTaskCount,
                        Participant = ppp
                    }).Where(q => ToGuid(q.Participant.ParticipantId) == CurrentUserID && q.Participant.Tenant == q.Milestone.TenantId)
                    .Select(q => new QueryMilestone
                    {
                        Milestone = q.Milestone,
                        Project = q.Project,
                        ActiveTaskCount = q.ActiveTaskCount,
                        ClosedTaskCount = q.ClosedTaskCount,
                    });
            }

            query = query.Join(WebProjectsContext.Participant,
                    q => q.Milestone.ProjectId,
                    ppp => ppp.ProjectId,
                    (q, ppp) => new
                    {
                        Milestone = q.Milestone,
                        Project = q.Project,
                        ActiveTaskCount = q.ActiveTaskCount,
                        ClosedTaskCount = q.ClosedTaskCount,
                        Participant = ppp
                    }).Where(q => q.Project.Private == 0 || (q.Participant.Security == null && q.Participant.Removed == 0) && (q.Participant.Security == 1|| ToGuid(q.Milestone.ResponsibleId) == CurrentUserID))
                    .Select(q => new QueryMilestone
                    {
                        Milestone = q.Milestone,
                        Project = q.Project,
                        ActiveTaskCount = q.ActiveTaskCount,
                        ClosedTaskCount = q.ClosedTaskCount,
                    });
            return query;
        }

        private Milestone ToMilestone(QueryMilestone query)
        {
            var milestone = query.Milestone;
            return new Milestone
            {
                Project = query.Project != null ? DaoFactory.GetProjectDao().ToProject(query.Project) : null,
                ID = milestone.Id,
                Title = milestone.Title,
                CreateBy = ToGuid(milestone.CreateBy),
                CreateOn = TenantUtil.DateTimeFromUtc(milestone.CreateOn),
                LastModifiedBy = ToGuid(milestone.LastModifiedBy),
                LastModifiedOn = TenantUtil.DateTimeFromUtc(milestone.LastModifiedOn.GetValueOrDefault()),
                DeadLine = DateTime.SpecifyKind(milestone.Deadline, DateTimeKind.Local),
                Status = (MilestoneStatus)milestone.Status,
                IsNotify = Convert.ToBoolean(milestone.IsNotify),
                IsKey = Convert.ToBoolean(milestone.IsKey),
                Description = milestone.Description,
                Responsible = ToGuid(milestone.ResponsibleId),
                ActiveTaskCount = query.ActiveTaskCount,
                ClosedTaskCount = query.ClosedTaskCount
            };
        }

        public DbMilestone ToDbMilestone(Milestone milestone)
        {
            return new DbMilestone
            {
                Id = milestone.ID,
                Title = milestone.Title,
                CreateBy = milestone.CreateBy.ToString(),
                CreateOn = TenantUtil.DateTimeFromUtc(milestone.CreateOn),
                LastModifiedBy = milestone.LastModifiedBy.ToString(),
                LastModifiedOn = TenantUtil.DateTimeFromUtc(milestone.LastModifiedOn),
                Deadline = DateTime.SpecifyKind(milestone.DeadLine, DateTimeKind.Local),
                Status = (int)milestone.Status,
                IsNotify = Convert.ToInt32(milestone.IsNotify),
                IsKey = Convert.ToInt32(milestone.IsKey),
                Description = milestone.Description,
                ResponsibleId = milestone.Responsible.ToString(),
                TenantId = Tenant,
                ProjectId = milestone.Project.ID,
                StatusChanged = milestone.StatusChangedOn
            };
        }
    }

    internal class QueryMilestone
    {
        public DbMilestone Milestone { get; set; }
        public DbProject Project { get; set; }
        public int ActiveTaskCount { get; set; }
        public int ClosedTaskCount { get; set; }
    }
}
