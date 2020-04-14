/*
 *
 * (c) Copyright Ascensio System Limited 2010-2018
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
 * Pursuant to Section 7 § 3(b) of the GNU GPL you must retain the original ONLYOFFICE logo which contains 
 * relevant author attributions when distributing the software. If the display of the logo in its graphic 
 * form is not reasonably feasible for technical reasons, you must include the words "Powered by ONLYOFFICE" 
 * in every copy of the program you distribute. 
 * Pursuant to Section 7 § 3(e) we decline to grant you any rights under trademark law for use of our trademarks.
 *
*/


using ASC.Collections;
using ASC.Common.Logging;
using ASC.Core;
using ASC.Core.Common.EF;
using ASC.Core.Tenants;
using ASC.CRM.Core.EF;
using ASC.CRM.Core.Entities;
using ASC.CRM.Core.Enums;
using ASC.ElasticSearch;
using ASC.Web.CRM.Core.Search;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text.RegularExpressions;

namespace ASC.CRM.Core.Dao
{
    public class CachedTaskDao : TaskDao
    {

        private readonly HttpRequestDictionary<Task> _contactCache;

        public CachedTaskDao(DbContextManager<CRMDbContext> dbContextManager,
                      TenantManager tenantManager,
                      SecurityContext securityContext,
                      CRMSecurity cRMSecurity,
                      TenantUtil tenantUtil,
                      FactoryIndexer<TasksWrapper> factoryIndexer,
                      IOptionsMonitor<ILog> logger,
                      IHttpContextAccessor httpContextAccessor,
                      DbContextManager<UserDbContext> userDbContext,
                      DbContextManager<CoreDbContext> coreDbContext
                      ) :
           base(dbContextManager,
                tenantManager,
                securityContext,
                cRMSecurity,
                tenantUtil,
                factoryIndexer,
                logger,
                userDbContext,
                coreDbContext
                )
        {
            _contactCache = new HttpRequestDictionary<Task>(httpContextAccessor?.HttpContext, "crm_task");
        }


        public override Task GetByID(int taskID)
        {
            return _contactCache.Get(taskID.ToString(), () => GetByIDBase(taskID));

        }

        private Task GetByIDBase(int taskID)
        {
            return base.GetByID(taskID);
        }

        public override void DeleteTask(int taskID)
        {
            ResetCache(taskID);

            base.DeleteTask(taskID);
        }

        public override Task SaveOrUpdateTask(Task task)
        {

            if (task != null && task.ID > 0)
            {
                ResetCache(task.ID);
            }

            return base.SaveOrUpdateTask(task);
        }

        private void ResetCache(int taskID)
        {
            _contactCache.Reset(taskID.ToString());
        }

    }

    public class TaskDao : AbstractDao
    {
        public TaskDao(DbContextManager<CRMDbContext> dbContextManager,
                       TenantManager tenantManager,
                       SecurityContext securityContext,
                       CRMSecurity cRMSecurity,
                       TenantUtil tenantUtil,
                       FactoryIndexer<TasksWrapper> factoryIndexer,
                       IOptionsMonitor<ILog> logger,
                       DbContextManager<UserDbContext> userDbContext,
                       DbContextManager<CoreDbContext> coreDbContext
                       ) :
            base(dbContextManager,
                 tenantManager,
                 securityContext,
                 logger)
        {
            CRMSecurity = cRMSecurity;
            TenantUtil = tenantUtil;
            FactoryIndexer = factoryIndexer;
            UserDbContext = userDbContext.Value;
            CoreDbContext = coreDbContext.Value;
        }

        public CoreDbContext CoreDbContext { get; }

        public UserDbContext UserDbContext { get; }

        public FactoryIndexer<TasksWrapper> FactoryIndexer { get; }

        public TenantUtil TenantUtil { get; }

        public CRMSecurity CRMSecurity { get; }

        public void OpenTask(int taskId)
        {
            var task = GetByID(taskId);

            if (task == null)
                throw new ArgumentException();

            CRMSecurity.DemandEdit(task);

            DbTask entity = new DbTask()
            {
                Id = taskId,
                IsClosed = false
            };

            CRMDbContext.Tasks.Update(entity);

            CRMDbContext.SaveChanges();
        }

        public void CloseTask(int taskId)
        {
            var task = GetByID(taskId);

            if (task == null)
                throw new ArgumentException();

            CRMSecurity.DemandEdit(task);

            DbTask entity = new DbTask()
            {
                Id = taskId,
                IsClosed = true
            };

            CRMDbContext.Tasks.Update(entity);

            CRMDbContext.SaveChanges();
        }

        public virtual Task GetByID(int taskID)
        {
            var crmTask = CRMDbContext.Tasks.Where(x => x.Id == taskID).SingleOrDefault();

            if (crmTask == default(DbTask)) return null;

            return ToTask(crmTask);
        }

        public List<Task> GetTasks(EntityType entityType, int entityID, bool? onlyActiveTask)
        {
            return GetTasks(String.Empty, Guid.Empty, 0, onlyActiveTask, DateTime.MinValue, DateTime.MinValue,
                            entityType, entityID, 0, 0, null);
        }
        public int GetAllTasksCount()
        {
            return CRMDbContext.Tasks.Count();
        }

        public List<Task> GetAllTasks()
        {
            return Query(CRMDbContext.Tasks)
                 .OrderBy(x => x.Deadline)
                 .OrderBy(x => x.Title)
                 .ToList()
                 .ConvertAll(ToTask)
                 .FindAll(CRMSecurity.CanAccessTo);
        }

        public void ExecAlert(IEnumerable<int> ids)
        {
            if (!ids.Any()) return;

            CRMDbContext.Tasks.UpdateRange(ids.Select(x => new DbTask { ExecAlert = 1, Id = x })); ;

            CRMDbContext.SaveChanges();
        }

        public List<object[]> GetInfoForReminder(DateTime scheduleDate)
        {
            return CRMDbContext.Tasks.Where(x =>
                                            x.IsClosed == false &&
                                            x.AlertValue != 0 &&
                                            x.ExecAlert == 0 &&
                                            (x.Deadline.AddMinutes(-x.AlertValue) >= scheduleDate.AddHours(-1) && x.Deadline.AddMinutes(-x.AlertValue) <= scheduleDate.AddHours(-1))
            ).Select(x => new object[] { x.TenantId, x.Id, x.Deadline, x.AlertValue, x.ResponsibleId }).ToList();
        }

        public List<Task> GetTasks(
                                  String searchText,
                                  Guid responsibleID,
                                  int categoryID,
                                  bool? isClosed,
                                  DateTime fromDate,
                                  DateTime toDate,
                                  EntityType entityType,
                                  int entityID,
                                  int from,
                                  int count,
                                  OrderBy orderBy)
        {

            if (CRMSecurity.IsAdmin)
                return GetCrudeTasks(
                    searchText,
                    responsibleID,
                    categoryID,
                    isClosed,
                    fromDate,
                    toDate,
                    entityType,
                    entityID,
                    from,
                    count,
                    orderBy);


            var crudeTasks = GetCrudeTasks(
                    searchText,
                    responsibleID,
                    categoryID,
                    isClosed,
                    fromDate,
                    toDate,
                    entityType,
                    entityID,
                    0,
                    from + count,
                    orderBy);

            if (crudeTasks.Count == 0) return crudeTasks;

            if (crudeTasks.Count < from + count) return CRMSecurity.FilterRead(crudeTasks).Skip(from).ToList();

            var result = CRMSecurity.FilterRead(crudeTasks).ToList();

            if (result.Count == crudeTasks.Count) return result.Skip(from).ToList();

            var localCount = count;
            var localFrom = from + count;

            while (true)
            {
                crudeTasks = GetCrudeTasks(
                    searchText,
                    responsibleID,
                    categoryID,
                    isClosed,
                    fromDate,
                    toDate,
                    entityType,
                    entityID,
                    localFrom,
                    localCount,
                    orderBy);

                if (crudeTasks.Count == 0) break;

                result.AddRange(CRMSecurity.FilterRead(crudeTasks));

                if ((result.Count >= count + from) || (crudeTasks.Count < localCount)) break;

                localFrom += localCount;
                localCount = localCount * 2;
            }

            return result.Skip(from).Take(count).ToList();
        }


        private List<Task> GetCrudeTasks(
                                  String searchText,
                                  Guid responsibleID,
                                  int categoryID,
                                  bool? isClosed,
                                  DateTime fromDate,
                                  DateTime toDate,
                                  EntityType entityType,
                                  int entityID,
                                  int from,
                                  int count,
                                  OrderBy orderBy)
        {

            var sqlQuery = GetDbTaskByFilters(responsibleID, categoryID, isClosed, fromDate, toDate, entityType, entityID, from, count, orderBy);

            if (!string.IsNullOrEmpty(searchText))
            {
                searchText = searchText.Trim();

                var keywords = searchText.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries)
                   .ToArray();

                if (keywords.Length > 0)
                {
                    List<int> tasksIds;

                    if (!FactoryIndexer.TrySelectIds(s => s.MatchAll(searchText), out tasksIds))
                    {
                        foreach (var k in keywords)
                        {
                            sqlQuery = sqlQuery.Where(x => Microsoft.EntityFrameworkCore.EF.Functions.Like(x.Title, k + "%") ||
                                                             Microsoft.EntityFrameworkCore.EF.Functions.Like(x.Description, k + "%"));
                        }
                    }
                    else
                    {
                        if (tasksIds.Any())
                        {
                            sqlQuery = sqlQuery.Where(x => tasksIds.Contains(x.Id));
                        }
                        else
                        {
                            return new List<Task>();
                        }
                    }

                }
            }

            return sqlQuery.ToList().ConvertAll(ToTask);
        }

        public int GetTasksCount(
                                       String searchText,
                                       Guid responsibleId,
                                       int categoryId,
                                       bool? isClosed,
                                       DateTime fromDate,
                                       DateTime toDate,
                                       EntityType entityType,
                                       int entityId)
        {

            int result = 0;

            Logger.DebugFormat("Starting GetTasksCount: {0}", DateTime.Now.ToString());

            var cacheKey = TenantID.ToString(CultureInfo.InvariantCulture) +
                           "tasks" +
                           SecurityContext.CurrentAccount.ID.ToString() +
                           searchText +
                           responsibleId +
                           categoryId +
                           fromDate.ToString(CultureInfo.InvariantCulture) +
                           toDate.ToString(CultureInfo.InvariantCulture) +
                           (int)entityType +
                           entityId;

            if (!String.IsNullOrEmpty(_cache.Get<String>(cacheKey)))
            {
                Logger.DebugFormat("End GetTasksCount: {0}. From cache", DateTime.Now.ToString());

                return Convert.ToInt32(_cache.Get<String>(cacheKey));
            }


            if (!String.IsNullOrEmpty(searchText))
            {
                var tasks = GetCrudeTasks(searchText,
                                      responsibleId,
                                      categoryId,
                                      isClosed,
                                      fromDate,
                                      toDate,
                                      entityType,
                                      entityId,
                                      0,
                                      0,
                                      null);

                if (CRMSecurity.IsAdmin)
                    result = tasks.Count();
                else
                    result = CRMSecurity.FilterRead(tasks).Count();
            }
            else
            {
                if (CRMSecurity.IsAdmin)
                {
                    result = GetDbTaskByFilters(
                                              responsibleId,
                                              categoryId,
                                              isClosed,
                                              fromDate,
                                              toDate,
                                              entityType,
                                              entityId,
                                              0,
                                              0,
                                              null).Count();
                }
                else
                {
                    throw new NotImplementedException();
                    //var taskIds = new List<int>();

                    //// PrivateTask


                    //// count tasks without entityId and only open contacts
                    //taskIds = GetDbTaskByFilters(responsibleId,
                    //                            categoryId,
                    //                            isClosed,
                    //                            fromDate,
                    //                            toDate,
                    //                            entityType,
                    //                            entityId,
                    //                            0,
                    //                            0,
                    //                            null).GroupJoin(Query(CRMDbContext.Contacts),
                    //                    x => x.ContactId,
                    //                    y => y.Id,
                    //                    (x, y) => new { x, y })
                    //                    .SelectMany(x => x.y.DefaultIfEmpty(), (x, y) => new { x.x, y })
                    //                    .Where(x => 
                                                    
                    //                                x.y == null ||
                    //                                x.y.IsShared == null ||
                    //                                x.y.IsShared.Value == ShareType.Read ||
                    //                                x.y.IsShared.Value == ShareType.ReadWrite)
                    //                    .Select(x => x.x.Id)
                    //                    .ToList();

                    //Logger.DebugFormat("End GetTasksCount: {0}. count tasks without entityId and only open contacts", DateTime.Now.ToString());

                    //var idsFromAcl = CoreDbContext.Acl.Where(x => x.Tenant == TenantID &&
                    //                                                x.Action == CRMSecurity._actionRead.ID &&
                    //                                                x.Subject == SecurityContext.CurrentAccount.ID &&
                    //                                                (Microsoft.EntityFrameworkCore.EF.Functions.Like(x.Object, typeof(Company).FullName + "%") ||
                    //                                                Microsoft.EntityFrameworkCore.EF.Functions.Like(x.Object, typeof(Person).FullName + "%")))
                    //                                    .Select(x => Convert.ToInt32(x.Object.Split('|', StringSplitOptions.None)[1]))
                    //                                    .ToList();

                    //Query(CRMDbContext.Contacts).GroupBy(x => x.Id).Select(x => x.)
                    //taskIds.AddRange(GetDbTaskByFilters(responsibleId,
                    //                           categoryId,
                    //                           isClosed,
                    //                           fromDate,
                    //                           toDate,
                    //                           entityType,
                    //                           entityId,
                    //                           0,
                    //                           0,
                    //                           null).GroupJoin(CRMDbContext.Contacts,
                    //                   x => x.ContactId,
                    //                   y => y.Id,
                    //                   (x, y) => new { x, y })
                    //                   .SelectMany(x => x.y.DefaultIfEmpty(), (x, y) => new { x.x, y })
                    //                   .Where(x => x.y != null && 
                    //                               x.y.IsShared == 0 &&
                    //                               idsFromAcl.Contains(x.y.Id))
                    //                   .Select(x => x.x.Id)
                    //                   .ToList());
                                  
                    //Logger.DebugFormat("End GetTasksCount: {0}. count tasks with entityId and only close contacts", DateTime.Now.ToString());


                    //sqlQuery = Query("crm_task tbl_tsk")
                    //            .Select("tbl_tsk.id")
                    //            .InnerJoin("core_acl tbl_cl", Exp.EqColumns("tbl_tsk.tenant_id", "tbl_cl.tenant") &
                    //            Exp.Eq("tbl_cl.subject", SecurityContext.CurrentAccount.ID.ToString()) &
                    //            Exp.Eq("tbl_cl.action", CRMSecurity._actionRead.ID.ToString()) &
                    //            Exp.EqColumns("tbl_cl.object", "CONCAT('ASC.CRM.Core.Entities.Deal|', tbl_tsk.entity_id)"))
                    //            .Where(!Exp.Eq("tbl_tsk.entity_id", 0) & Exp.Eq("tbl_tsk.entity_type", (int)EntityType.Opportunity) & Exp.Eq("tbl_tsk.contact_id", 0));

                    //sqlQuery = GetDbTaskByFilters(responsibleId,
                    //                            categoryId,
                    //                            isClosed,
                    //                            fromDate,
                    //                            toDate,
                    //                            entityType,
                    //                            entityId,
                    //                            0,
                    //                            0,
                    //                            null);

                    //// count tasks with entityId and without contact
                    //taskIds.AddRange(Db.ExecuteList(sqlQuery).Select(item => Convert.ToInt32(item[0])).ToList());

                    //Logger.DebugFormat("End GetTasksCount: {0}. count tasks with entityId and without contact", DateTime.Now.ToString());

                    //sqlQuery = Query("crm_task tbl_tsk")
                    //            .Select("tbl_tsk.id")
                    //            .InnerJoin("core_acl tbl_cl", Exp.EqColumns("tbl_tsk.tenant_id", "tbl_cl.tenant") &
                    //            Exp.Eq("tbl_cl.subject", SecurityContext.CurrentAccount.ID.ToString()) &
                    //            Exp.Eq("tbl_cl.action", CRMSecurity._actionRead.ID.ToString()) &
                    //            Exp.EqColumns("tbl_cl.object", "CONCAT('ASC.CRM.Core.Entities.Cases|', tbl_tsk.entity_id)"))
                    //            .Where(!Exp.Eq("tbl_tsk.entity_id", 0) & Exp.Eq("tbl_tsk.entity_type", (int)EntityType.Case) & Exp.Eq("tbl_tsk.contact_id", 0));

                    //sqlQuery = GetDbTaskByFilters(responsibleId,
                    //                            categoryId,
                    //                            isClosed,
                    //                            fromDate,
                    //                            toDate,
                    //                            entityType,
                    //                            entityId,
                    //                            0,
                    //                            0,
                    //                            null);

                    //// count tasks with entityId and without contact
                    //taskIds.AddRange(Db.ExecuteList(sqlQuery).Select(item => Convert.ToInt32(item[0])).ToList());

                    //result = taskIds.Distinct().Count();

                    //Logger.DebugFormat("End GetTasksCount: {0}. count tasks with entityId and without contact", DateTime.Now.ToString());

                    //Logger.Debug("Finish");

                }
            }


            if (result > 0)
            {
                _cache.Insert(cacheKey, result, TimeSpan.FromMinutes(1));
            }

            return result;
        }

        private IQueryable<DbTask> GetDbTaskByFilters(
                                  Guid responsibleID,
                                  int categoryID,
                                  bool? isClosed,
                                  DateTime fromDate,
                                  DateTime toDate,
                                  EntityType entityType,
                                  int entityID,
                                  int from,
                                  int count,
                                  OrderBy orderBy)
        {
            var sqlQuery = Query(CRMDbContext.Tasks);

            if (responsibleID != Guid.Empty)
                sqlQuery = sqlQuery.Where(x => x.ResponsibleId == responsibleID);

            if (entityID > 0)
                switch (entityType)
                {
                    case EntityType.Contact:
                        var isCompany = Query(CRMDbContext.Contacts).Where(x => x.Id == entityID).Select(x => x.IsCompany).Single();

                        if (isCompany)
                            return GetDbTaskByFilters(responsibleID, categoryID, isClosed, fromDate, toDate, EntityType.Company, entityID, from, count, orderBy);
                        else
                            return GetDbTaskByFilters(responsibleID, categoryID, isClosed, fromDate, toDate, EntityType.Person, entityID, from, count, orderBy);

                    case EntityType.Person:
                        sqlQuery = sqlQuery.Where(x => x.ContactId == entityID);
                        break;
                    case EntityType.Company:

                        var personIDs = GetRelativeToEntity(entityID, EntityType.Person, null).ToList();

                        if (personIDs.Count == 0)
                        {
                            sqlQuery = sqlQuery.Where(x => x.ContactId == entityID);
                        }
                        else
                        {
                            personIDs.Add(entityID);
                            sqlQuery = sqlQuery.Where(x => personIDs.Contains(x.ContactId));
                        }

                        break;
                    case EntityType.Case:
                    case EntityType.Opportunity:
                        sqlQuery = sqlQuery.Where(x => x.EntityId == entityID && x.EntityType == entityType);
                        break;
                }



            if (isClosed.HasValue)
            {
                sqlQuery = sqlQuery.Where(x => x.IsClosed == isClosed);
            }

            if (categoryID > 0)
            {
                sqlQuery = sqlQuery.Where(x => x.CategoryId == categoryID);
            }

            if (fromDate != DateTime.MinValue && toDate != DateTime.MinValue)
            {
                sqlQuery = sqlQuery.Where(x => x.Deadline >= TenantUtil.DateTimeToUtc(fromDate) && x.Deadline <= TenantUtil.DateTimeToUtc(toDate));
            }
            else if (fromDate != DateTime.MinValue)
            {
                sqlQuery = sqlQuery.Where(x => x.Deadline > TenantUtil.DateTimeToUtc(fromDate));
            }
            else if (toDate != DateTime.MinValue)
            {
                sqlQuery = sqlQuery.Where(x => x.Deadline < TenantUtil.DateTimeToUtc(toDate));
            }

            if (0 < from && from < int.MaxValue)
                sqlQuery = sqlQuery.Skip(from);

            if (0 < count && count < int.MaxValue)
                sqlQuery = sqlQuery.Take(count);

            sqlQuery = sqlQuery.OrderBy(x => x.IsClosed);

            if (orderBy != null && Enum.IsDefined(typeof(TaskSortedByType), orderBy.SortedBy))
            {
                switch ((TaskSortedByType)orderBy.SortedBy)
                {
                    case TaskSortedByType.Title:
                        {
                            if (orderBy.IsAsc)
                                sqlQuery = sqlQuery.OrderBy(x => x.Title)
                                                   .ThenBy(x => x.Deadline);
                            else
                                sqlQuery = sqlQuery.OrderByDescending(x => x.Title)
                                                   .ThenBy(x => x.Deadline);
                        }

                        break;
                    case TaskSortedByType.DeadLine:
                        {
                            if (orderBy.IsAsc)
                                sqlQuery = sqlQuery.OrderBy(x => x.Deadline)
                                                   .ThenBy(x => x.Title);
                            else
                                sqlQuery = sqlQuery.OrderByDescending(x => x.Deadline)
                                                   .ThenBy(x => x.Title);
                        }
                        break;
                    case TaskSortedByType.Category:
                        {
                            if (orderBy.IsAsc)
                                sqlQuery = sqlQuery.OrderBy(x => x.CategoryId)
                                                   .ThenBy(x => x.Deadline)
                                                   .ThenBy(x => x.Title);
                            else
                                sqlQuery = sqlQuery.OrderByDescending(x => x.CategoryId)
                                                   .ThenBy(x => x.Deadline)
                                                   .ThenBy(x => x.Title);
                        }

                        break;
                    case TaskSortedByType.ContactManager:
                        {
                            sqlQuery =  sqlQuery.GroupJoin(UserDbContext.Users.Where(x => x.Tenant == TenantID),
                                  x => x.ResponsibleId,
                                  y => y.Id,
                                  (x, y) => new { x, y }
                                )
                             .SelectMany(x => x.y.DefaultIfEmpty(), (x,y) => new { x.x,y })
                             .OrderBy("x.y.LastName", orderBy.IsAsc)
                             .OrderBy("x.y.FirstName", orderBy.IsAsc)
                             .OrderBy(x => x.x.Deadline)
                             .OrderBy(x => x.x.Title)
                             .Select(x => x.x);
                        }

                        break;
                    case TaskSortedByType.Contact:
                        {
                            var subSqlQuery = sqlQuery.GroupJoin(CRMDbContext.Contacts,
                                                          x => x.ContactId,
                                                          y => y.Id,
                                                          (x, y) => new { x, y }
                                                        )
                                                .SelectMany(x => x.y.DefaultIfEmpty(), (x, y) => new { x.x, y });

                            if (orderBy.IsAsc)
                            {
                                subSqlQuery = subSqlQuery.OrderBy(x => x.y != null ? x.y.DisplayName : "")
                                    .ThenBy(x => x.x.Deadline)
                                    .ThenBy(x => x.x.Title);
                            }
                            else
                            {
                                subSqlQuery = subSqlQuery.OrderByDescending(x => x.y != null ? x.y.DisplayName : "")
                                      .ThenBy(x => x.x.Deadline)
                                      .ThenBy(x => x.x.Title);

                            }

                            sqlQuery = subSqlQuery.Select(x => x.x);

                            break;
                        }
                }
            }
            else
            {
                sqlQuery = sqlQuery.OrderBy(x => x.Deadline)
                                   .ThenBy(x => x.Title);
            }

            return sqlQuery;

        }

        public Dictionary<int, Task> GetNearestTask(int[] contactID)
        {
            return Query(CRMDbContext.Tasks)
                        .Where(x => contactID.Contains(x.ContactId) && !x.IsClosed)
                        .GroupBy(x => x.ContactId)
                        .ToDictionary(x => x.Key, y => ToTask(y.Single(x => x.Id == y.Min(x => x.Id))));
        }

        public IEnumerable<Guid> GetResponsibles(int categoryID)
        {
            var sqlQuery = Query(CRMDbContext.Tasks);

            if (0 < categoryID)
                sqlQuery = sqlQuery.Where(x => x.CategoryId == categoryID);

            return sqlQuery.GroupBy(x => x.ResponsibleId).Select(x => x.Key).ToList();
        }

        public Dictionary<int, int> GetTasksCount(int[] contactID)
        {
            return Query(CRMDbContext.Tasks)
                .Where(x => contactID.Contains(x.ContactId))
                .GroupBy(x => x.ContactId)
                .ToDictionary(x => x.Key, x => x.Count());
        }

        public int GetTasksCount(int contactID)
        {
            var result = GetTasksCount(new[] { contactID });

            if (result.Count == 0) return 0;

            return result[contactID];
        }

        public Dictionary<int, bool> HaveLateTask(int[] contactID)
        {
            return Query(CRMDbContext.Tasks)
                    .Where(x => contactID.Contains(x.ContactId) && !x.IsClosed && x.Deadline <= DateTime.UtcNow)
                    .GroupBy(x => x.ContactId)
                    .ToDictionary(x => x.Key, x => x.Any());
        }

        public bool HaveLateTask(int contactID)
        {
            var result = HaveLateTask(new[] { contactID });

            if (result.Count == 0) return false;

            return result[contactID];
        }

        public virtual Task SaveOrUpdateTask(Task newTask)
        {
            _cache.Remove(new Regex(TenantID.ToString(CultureInfo.InvariantCulture) + "tasks.*"));
            return SaveOrUpdateTaskInDb(newTask);
        }

        public virtual Task[] SaveOrUpdateTaskList(List<Task> newTasks)
        {
            _cache.Remove(new Regex(TenantID.ToString(CultureInfo.InvariantCulture) + "tasks.*"));
            var result = new List<Task>();
            foreach (var newTask in newTasks)
            {
                result.Add(SaveOrUpdateTaskInDb(newTask));
            }
            return result.ToArray();
        }

        private Task SaveOrUpdateTaskInDb(Task newTask)
        {
            if (string.IsNullOrEmpty(newTask.Title) || newTask.DeadLine == DateTime.MinValue ||
                newTask.CategoryID <= 0)
                throw new ArgumentException();

            if (newTask.ID == 0 || Query(CRMDbContext.Tasks).Where(x => x.Id == newTask.ID).Count() == 0)
            {
                newTask.CreateOn = DateTime.UtcNow;
                newTask.CreateBy = SecurityContext.CurrentAccount.ID;

                newTask.LastModifedOn = DateTime.UtcNow;
                newTask.LastModifedBy = SecurityContext.CurrentAccount.ID;

                var itemToInsert = new DbTask
                {
                    Title = newTask.Title,
                    Description = newTask.Description,
                    Deadline = TenantUtil.DateTimeToUtc(newTask.DeadLine),
                    ResponsibleId = newTask.ResponsibleID,
                    ContactId = newTask.ContactID,
                    EntityType = newTask.EntityType,
                    EntityId = newTask.EntityID,
                    IsClosed = newTask.IsClosed,
                    CategoryId = newTask.CategoryID,
                    CreateOn = newTask.CreateOn,
                    CreateBy = newTask.CreateBy,
                    LastModifedOn = newTask.LastModifedOn,
                    LastModifedBy = newTask.LastModifedBy,
                    AlertValue = newTask.AlertValue,
                    TenantId = TenantID
                };

                CRMDbContext.Add(itemToInsert);
                CRMDbContext.SaveChanges();

                newTask.ID = itemToInsert.Id;

            }
            else
            {

                var itemToUpdate = Query(CRMDbContext.Tasks).FirstOrDefault(x => x.Id == newTask.ID);

                var oldTask = ToTask(itemToUpdate);

                CRMSecurity.DemandEdit(oldTask);

                itemToUpdate.Title = newTask.Title;
                itemToUpdate.Description = newTask.Description;
                itemToUpdate.Deadline = TenantUtil.DateTimeToUtc(newTask.DeadLine);
                itemToUpdate.ResponsibleId = newTask.ResponsibleID;
                itemToUpdate.ContactId = newTask.ContactID;
                itemToUpdate.EntityType = newTask.EntityType;
                itemToUpdate.EntityId = newTask.EntityID;
                itemToUpdate.CategoryId = newTask.CategoryID;
                itemToUpdate.LastModifedOn = newTask.LastModifedOn;
                itemToUpdate.LastModifedBy = newTask.LastModifedBy;
                itemToUpdate.AlertValue = newTask.AlertValue;
                itemToUpdate.ExecAlert = 0;
                itemToUpdate.TenantId = TenantID;

                CRMDbContext.Update(itemToUpdate);
                CRMDbContext.SaveChanges();


                newTask.CreateOn = oldTask.CreateOn;
                newTask.CreateBy = oldTask.CreateBy;

                newTask.LastModifedOn = DateTime.UtcNow;
                newTask.LastModifedBy = SecurityContext.CurrentAccount.ID;

                newTask.IsClosed = oldTask.IsClosed;

            }

           

            FactoryIndexer.IndexAsync(TasksWrapper.FromTask(TenantID, newTask));

            return newTask;
        }

        public virtual int SaveTask(Task newTask)
        {
            _cache.Remove(new Regex(TenantID.ToString(CultureInfo.InvariantCulture) + "tasks.*"));
            return SaveTaskInDb(newTask);
        }

        private int SaveTaskInDb(Task newTask)
        {
            if (String.IsNullOrEmpty(newTask.Title) || newTask.DeadLine == DateTime.MinValue ||
                newTask.CategoryID == 0)
                throw new ArgumentException();

            var dbTask = new DbTask
            {
                Title = newTask.Title,
                Description = newTask.Description,
                Deadline = TenantUtil.DateTimeToUtc(newTask.DeadLine),
                ResponsibleId = newTask.ResponsibleID,
                ContactId = newTask.ContactID,
                EntityType = newTask.EntityType,
                EntityId = newTask.EntityID,
                IsClosed = newTask.IsClosed,
                CategoryId = newTask.CategoryID,
                CreateOn = newTask.CreateOn == DateTime.MinValue ? DateTime.UtcNow : newTask.CreateOn,
                CreateBy = SecurityContext.CurrentAccount.ID,
                LastModifedOn = newTask.CreateOn == DateTime.MinValue ? DateTime.UtcNow : newTask.CreateOn,
                LastModifedBy = SecurityContext.CurrentAccount.ID,
                AlertValue = newTask.AlertValue,
                TenantId = TenantID
            };

            CRMDbContext.Tasks.Add(dbTask);

            CRMDbContext.SaveChanges();

            newTask.ID = dbTask.Id;

            FactoryIndexer.IndexAsync(TasksWrapper.FromTask(TenantID, newTask));

            return newTask.ID;
        }

        public virtual int[] SaveTaskList(List<Task> items)
        {

            using var tx = CRMDbContext.Database.BeginTransaction();

            var result = new List<int>();

            foreach (var item in items)
            {
                result.Add(SaveTaskInDb(item));
            }

            CRMDbContext.SaveChanges();

            tx.Commit();

            return result.ToArray();
        }


        public virtual void DeleteTask(int taskID)
        {
            var task = GetByID(taskID);

            if (task == null) return;

            CRMSecurity.DemandEdit(task);

            CRMDbContext.Tasks.Remove(new DbTask { Id = taskID });

            _cache.Remove(new Regex(TenantID.ToString(CultureInfo.InvariantCulture) + "tasks.*"));

            FactoryIndexer.DeleteAsync(TasksWrapper.FromTask(TenantID, task));
        }

        public List<Task> CreateByTemplate(List<TaskTemplate> templateItems, EntityType entityType, int entityID)
        {
            if (templateItems == null || templateItems.Count == 0) return new List<Task>();

            var result = new List<Task>();

            using var tx = CRMDbContext.Database.BeginTransaction();

            foreach (var templateItem in templateItems)
            {
                var task = new Task
                {
                    ResponsibleID = templateItem.ResponsibleID,
                    Description = templateItem.Description,
                    DeadLine = TenantUtil.DateTimeNow().AddTicks(templateItem.Offset.Ticks),
                    CategoryID = templateItem.CategoryID,
                    Title = templateItem.Title,
                    CreateOn = TenantUtil.DateTimeNow(),
                    CreateBy = templateItem.CreateBy
                };

                switch (entityType)
                {
                    case EntityType.Contact:
                    case EntityType.Person:
                    case EntityType.Company:
                        task.ContactID = entityID;
                        break;
                    case EntityType.Opportunity:
                        task.EntityType = EntityType.Opportunity;
                        task.EntityID = entityID;
                        break;
                    case EntityType.Case:
                        task.EntityType = EntityType.Case;
                        task.EntityID = entityID;
                        break;
                    default:
                        throw new NotImplementedException();
                }

                task = SaveOrUpdateTask(task);

                result.Add(task);

                CRMDbContext.TaskTemplateTask.Add(new DbTaskTemplateTask
                {
                    TaskId = task.ID,
                    TaskTemplateId = templateItem.ID,
                    TenantId = TenantID
                });
            }

            tx.Commit();

            CRMDbContext.SaveChanges();

            return result;
        }

        #region Private Methods

        //private String[] GetTaskColumnsTable(String alias)
        //{
        //    if (!String.IsNullOrEmpty(alias))
        //        alias = alias + ".";

        //    var result = new List<String>
        //                     {
        //                        "id",
        //                        "contact_id",
        //                        "title",
        //                        "description",
        //                        "deadline",
        //                        "responsible_id",
        //                        "is_closed",
        //                        "category_id",
        //                        "entity_id",
        //                        "entity_type",
        //                        "create_on",
        //                        "create_by",
        //                        "alert_value"
        //                     };

        //    if (String.IsNullOrEmpty(alias)) return result.ToArray();

        //    return result.ConvertAll(item => String.Concat(alias, item)).ToArray();
        //}

        //private SqlQuery GetTaskQuery(Exp where, String alias)
        //{

        //    var sqlQuery = Query("crm_task");

        //    if (!String.IsNullOrEmpty(alias))
        //    {
        //        sqlQuery = new SqlQuery(String.Concat("crm_task ", alias))
        //                   .Where(Exp.Eq(alias + ".tenant_id", TenantID));
        //        sqlQuery.Select(GetTaskColumnsTable(alias));

        //    }
        //    else
        //        sqlQuery.Select(GetTaskColumnsTable(String.Empty));


        //    if (where != null)
        //        sqlQuery.Where(where);

        //    return sqlQuery;

        //}

        //private SqlQuery GetTaskQuery(Exp where)
        //{
        //    return GetTaskQuery(where, String.Empty);

        //}


        #endregion

        public void ReassignTasksResponsible(Guid fromUserId, Guid toUserId)
        {
            var tasks = GetTasks(String.Empty, fromUserId, 0, false, DateTime.MinValue, DateTime.MinValue,
                            EntityType.Any, 0, 0, 0, null);

            foreach (var task in tasks)
            {
                task.ResponsibleID = toUserId;

                SaveOrUpdateTask(task);
            }
        }

        /// <summary>
        /// Test method
        /// </summary>
        /// <param name="taskId"></param>
        /// <param name="creationDate"></param>
        public void SetTaskCreationDate(int taskId, DateTime creationDate)
        {
            DbTask entity = new DbTask()
            {
                Id = taskId,
                CreateOn = TenantUtil.DateTimeToUtc(creationDate)
            };

            CRMDbContext.Tasks.Update(entity);

            CRMDbContext.SaveChanges();

            // Delete relative keys
            _cache.Remove(new Regex(TenantID.ToString(CultureInfo.InvariantCulture) + "tasks.*"));
        }

        /// <summary>
        /// Test method
        /// </summary>
        /// <param name="taskId"></param>
        /// <param name="lastModifedDate"></param>
        public void SetTaskLastModifedDate(int taskId, DateTime lastModifedDate)
        {
            DbTask entity = new DbTask()
            {
                Id = taskId,
                LastModifedOn = TenantUtil.DateTimeToUtc(lastModifedDate)
            };

            CRMDbContext.Tasks.Update(entity);

            CRMDbContext.SaveChanges();

            // Delete relative keys
            _cache.Remove(new Regex(TenantID.ToString(CultureInfo.InvariantCulture) + "tasks.*"));
        }

        public Task ToTask(DbTask dbTask)
        {
            if (dbTask == null) return null;

            return new Task
            {
                ID = dbTask.Id,
                AlertValue = dbTask.AlertValue,
                CategoryID = dbTask.CategoryId,
                ContactID = dbTask.ContactId,
                CreateBy = dbTask.CreateBy,
                CreateOn = TenantUtil.DateTimeFromUtc(dbTask.CreateOn),
                DeadLine = TenantUtil.DateTimeFromUtc(dbTask.Deadline),
                Description = dbTask.Description,
                IsClosed = dbTask.IsClosed,
                EntityID = dbTask.EntityId,
                EntityType = dbTask.EntityType,
                LastModifedBy = dbTask.LastModifedBy,
                LastModifedOn = dbTask.LastModifedOn,
                ResponsibleID = dbTask.ResponsibleId,
                Title = dbTask.Title
            };

        }
    }
}
