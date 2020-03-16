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
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text.RegularExpressions;

namespace ASC.CRM.Core.Dao
{
    public class CachedTaskDao : TaskDao
    {

        private readonly HttpRequestDictionary<Task> _contactCache = new HttpRequestDictionary<Task>("crm_task");
            
        public CachedTaskDao(DbContextManager<CRMDbContext> dbContextManager,
                      TenantManager tenantManager,
                      SecurityContext securityContext,
                      CRMSecurity cRMSecurity,
                      TenantUtil tenantUtil,
                      FactoryIndexer<TasksWrapper> factoryIndexer
                      ):
           base(dbContextManager,
                tenantManager,
                securityContext,
                cRMSecurity,
                tenantUtil,
                factoryIndexer)
        {
            
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
                       FactoryIndexer<TasksWrapper> factoryIndexer
                       ) :
            base(dbContextManager,
                 tenantManager,
                 securityContext)
        {
            CRMSecurity = cRMSecurity;
            TenantUtil = tenantUtil;
            FactoryIndexer = factoryIndexer;
        }

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

            CRMDbContext.Tasks.UpdateRange(ids.Select(x => new DbTask { ExecAlert = true, Id = x })); ;

            CRMDbContext.SaveChanges();
        }

        public List<object[]> GetInfoForReminder(DateTime scheduleDate)
        {
            return CRMDbContext.Tasks.Where(x => 
                                            x.IsClosed == false &&
                                            x.AlertValue != 0 &&
                                            x.ExecAlert == false &&
                                            ( x.Deadline.AddMinutes(-x.AlertValue) >= scheduleDate.AddHours(-1) && x.Deadline.AddMinutes(-x.AlertValue) <= scheduleDate.AddHours(-1))
            ).Select(x => new object[]{ x.TenantId, x.Id, x.Deadline, x.AlertValue, x.ResponsibleId }).ToList();
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
            var taskTableAlias = "t";
            var sqlQuery = WhereConditional(GetTaskQuery(null, taskTableAlias), taskTableAlias, responsibleID,
                                        categoryID, isClosed, fromDate, toDate, entityType, entityID, from, count,
                                        orderBy);

            if (!String.IsNullOrEmpty(searchText))
            {
                searchText = searchText.Trim();


                var keywords = searchText.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries)
                   .ToArray();

                if (keywords.Length > 0)
                {
                    List<int> tasksIds;
                    if (!FactoryIndexer.TrySelectIds(s => s.MatchAll(searchText), out tasksIds))
                    {
                        sqlQuery.Where(BuildLike(new[] { taskTableAlias + ".title", taskTableAlias + ".description" }, keywords));
                    }
                    else
                    {
                        if (tasksIds.Any())
                            sqlQuery.Where(Exp.In(taskTableAlias + ".id", tasksIds));
                        else
                            return new List<Task>();
                    }

                }
            }

            return Db.ExecuteList(sqlQuery)
                .ConvertAll(row => ToTask(row));
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

            LogManager.GetLogger("ASC.CRM").DebugFormat("Starting GetTasksCount: {0}", DateTime.Now.ToString());

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
                LogManager.GetLogger("ASC.CRM").DebugFormat("End GetTasksCount: {0}. From cache", DateTime.Now.ToString());

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

                    var sqlQuery = Query("crm_task tbl_tsk").SelectCount();

                    sqlQuery = WhereConditional(sqlQuery, "tbl_tsk",
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

                    result = Db.ExecuteScalar<int>(sqlQuery);
                }
                else
                {
                    var taskIds = new List<int>();

                    var sqlQuery = Query("crm_task tbl_tsk")
                                  .Select("tbl_tsk.id")
                                  .LeftOuterJoin("crm_contact tbl_ctc", Exp.EqColumns("tbl_tsk.contact_id", "tbl_ctc.id"))
                                  .Where(Exp.Or(Exp.Eq("tbl_ctc.is_shared", Exp.Empty), Exp.Gt("tbl_ctc.is_shared", 0)));

                    sqlQuery = WhereConditional(sqlQuery, "tbl_tsk",
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

                    // count tasks without entityId and only open contacts

                    taskIds = Db.ExecuteList(sqlQuery).Select(item => Convert.ToInt32(item[0])).ToList();

                    LogManager.GetLogger("ASC.CRM").DebugFormat("End GetTasksCount: {0}. count tasks without entityId and only open contacts", DateTime.Now.ToString());


                    sqlQuery = Query("crm_task tbl_tsk")
                                .Select("tbl_tsk.id")
                                .InnerJoin("crm_contact tbl_ctc", Exp.EqColumns("tbl_tsk.contact_id", "tbl_ctc.id"))
                                .InnerJoin("core_acl tbl_cl", Exp.EqColumns("tbl_ctc.tenant_id", "tbl_cl.tenant") &
                                Exp.Eq("tbl_cl.subject", SecurityContext.CurrentAccount.ID.ToString()) &
                                Exp.Eq("tbl_cl.action", CRMSecurity._actionRead.ID.ToString()) &
                                Exp.EqColumns("tbl_cl.object", "CONCAT('ASC.CRM.Core.Entities.Company|', tbl_ctc.id)"))
                                .Where(Exp.Eq("tbl_ctc.is_shared", 0))
                                .Where(Exp.Eq("tbl_ctc.is_company", 1));

                    sqlQuery = WhereConditional(sqlQuery, "tbl_tsk",
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

                    // count tasks with entityId and only close contacts
                    taskIds.AddRange(Db.ExecuteList(sqlQuery).Select(item => Convert.ToInt32(item[0])).ToList());

                    LogManager.GetLogger("ASC.CRM").DebugFormat("End GetTasksCount: {0}. count tasks with entityId and only close contacts", DateTime.Now.ToString());

                    sqlQuery = Query("crm_task tbl_tsk")
                              .Select("tbl_tsk.id")
                              .InnerJoin("crm_contact tbl_ctc", Exp.EqColumns("tbl_tsk.contact_id", "tbl_ctc.id"))
                              .InnerJoin("core_acl tbl_cl", Exp.EqColumns("tbl_ctc.tenant_id", "tbl_cl.tenant") &
                              Exp.Eq("tbl_cl.subject", SecurityContext.CurrentAccount.ID.ToString()) &
                              Exp.Eq("tbl_cl.action", CRMSecurity._actionRead.ID.ToString()) &
                              Exp.EqColumns("tbl_cl.object", "CONCAT('ASC.CRM.Core.Entities.Person|', tbl_ctc.id)"))
                              .Where(Exp.Eq("tbl_ctc.is_shared", 0))
                              .Where(Exp.Eq("tbl_ctc.is_company", 0));

                    sqlQuery = WhereConditional(sqlQuery, "tbl_tsk",
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

                    // count tasks with entityId and only close contacts
                    taskIds.AddRange(Db.ExecuteList(sqlQuery).Select(item => Convert.ToInt32(item[0])).ToList());

                    LogManager.GetLogger("ASC.CRM").DebugFormat("End GetTasksCount: {0}. count tasks with entityId and only close contacts", DateTime.Now.ToString());


                    sqlQuery = Query("crm_task tbl_tsk")
                                .Select("tbl_tsk.id")
                                .InnerJoin("core_acl tbl_cl", Exp.EqColumns("tbl_tsk.tenant_id", "tbl_cl.tenant") &
                                Exp.Eq("tbl_cl.subject", SecurityContext.CurrentAccount.ID.ToString()) &
                                Exp.Eq("tbl_cl.action", CRMSecurity._actionRead.ID.ToString()) &
                                Exp.EqColumns("tbl_cl.object", "CONCAT('ASC.CRM.Core.Entities.Deal|', tbl_tsk.entity_id)"))
                                .Where(!Exp.Eq("tbl_tsk.entity_id", 0) & Exp.Eq("tbl_tsk.entity_type", (int)EntityType.Opportunity) & Exp.Eq("tbl_tsk.contact_id", 0));

                    sqlQuery = WhereConditional(sqlQuery, "tbl_tsk",
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

                    // count tasks with entityId and without contact
                    taskIds.AddRange(Db.ExecuteList(sqlQuery).Select(item => Convert.ToInt32(item[0])).ToList());

                    LogManager.GetLogger("ASC.CRM").DebugFormat("End GetTasksCount: {0}. count tasks with entityId and without contact", DateTime.Now.ToString());

                    sqlQuery = Query("crm_task tbl_tsk")
                                .Select("tbl_tsk.id")
                                .InnerJoin("core_acl tbl_cl", Exp.EqColumns("tbl_tsk.tenant_id", "tbl_cl.tenant") &
                                Exp.Eq("tbl_cl.subject", SecurityContext.CurrentAccount.ID.ToString()) &
                                Exp.Eq("tbl_cl.action", CRMSecurity._actionRead.ID.ToString()) &
                                Exp.EqColumns("tbl_cl.object", "CONCAT('ASC.CRM.Core.Entities.Cases|', tbl_tsk.entity_id)"))
                                .Where(!Exp.Eq("tbl_tsk.entity_id", 0) & Exp.Eq("tbl_tsk.entity_type", (int)EntityType.Case) & Exp.Eq("tbl_tsk.contact_id", 0));

                    sqlQuery = WhereConditional(sqlQuery, "tbl_tsk",
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

                    // count tasks with entityId and without contact
                    taskIds.AddRange(Db.ExecuteList(sqlQuery).Select(item => Convert.ToInt32(item[0])).ToList());

                    result = taskIds.Distinct().Count();

                    LogManager.GetLogger("ASC.CRM").DebugFormat("End GetTasksCount: {0}. count tasks with entityId and without contact", DateTime.Now.ToString());

                    LogManager.GetLogger("ASC.CRM").Debug("Finish");

                }
            }


            if (result > 0)
            {
                _cache.Insert(cacheKey, result, TimeSpan.FromMinutes(1));
            }

            return result;
        }


        private SqlQuery WhereConditional(
                                  SqlQuery sqlQuery,
                                  String alias,
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
            var aliasPrefix = !String.IsNullOrEmpty(alias) ? alias + "." : "";

            if (responsibleID != Guid.Empty)
                sqlQuery.Where(Exp.Eq("responsible_id", responsibleID));

            if (entityID > 0)
                switch (entityType)
                {
                    case EntityType.Contact:
                        var isCompany = true;
                        isCompany = Db.ExecuteScalar<bool>(Query("crm_contact").Select("is_company").Where(Exp.Eq("id", entityID)));

                        if (isCompany)
                            return WhereConditional(sqlQuery, alias, responsibleID, categoryID, isClosed, fromDate, toDate, EntityType.Company, entityID, from, count, orderBy);
                        else
                            return WhereConditional(sqlQuery, alias, responsibleID, categoryID, isClosed, fromDate, toDate, EntityType.Person, entityID, from, count, orderBy);

                    case EntityType.Person:
                        sqlQuery.Where(Exp.Eq(aliasPrefix + "contact_id", entityID));
                        break;
                    case EntityType.Company:

                        var personIDs = GetRelativeToEntity(entityID, EntityType.Person, null).ToList();

                        if (personIDs.Count == 0)
                            sqlQuery.Where(Exp.Eq(aliasPrefix + "contact_id", entityID));
                        else
                        {
                            personIDs.Add(entityID);
                            sqlQuery.Where(Exp.In(aliasPrefix + "contact_id", personIDs));
                        }

                        break;
                    case EntityType.Case:
                    case EntityType.Opportunity:
                        sqlQuery.Where(Exp.Eq(aliasPrefix + "entity_id", entityID) &
                                       Exp.Eq(aliasPrefix + "entity_type", (int)entityType));
                        break;
                }



            if (isClosed.HasValue)
                sqlQuery.Where(aliasPrefix + "is_closed", isClosed);

            if (categoryID > 0)
                sqlQuery.Where(Exp.Eq(aliasPrefix + "category_id", categoryID));

            if (fromDate != DateTime.MinValue && toDate != DateTime.MinValue)
                sqlQuery.Where(Exp.Between(aliasPrefix + "deadline", TenantUtil.DateTimeToUtc(fromDate), TenantUtil.DateTimeToUtc(toDate)));
            else if (fromDate != DateTime.MinValue)
                sqlQuery.Where(Exp.Ge(aliasPrefix + "deadline", TenantUtil.DateTimeToUtc(fromDate)));
            else if (toDate != DateTime.MinValue)
                sqlQuery.Where(Exp.Le(aliasPrefix + "deadline", TenantUtil.DateTimeToUtc(toDate)));

            if (0 < from && from < int.MaxValue)
                sqlQuery.SetFirstResult(from);

            if (0 < count && count < int.MaxValue)
                sqlQuery.SetMaxResults(count);

            sqlQuery.OrderBy(aliasPrefix + "is_closed", true);

            if (orderBy != null && Enum.IsDefined(typeof(TaskSortedByType), orderBy.SortedBy))
            {
                switch ((TaskSortedByType)orderBy.SortedBy)
                {
                    case TaskSortedByType.Title:
                        sqlQuery
                            .OrderBy(aliasPrefix + "title", orderBy.IsAsc)
                            .OrderBy(aliasPrefix + "deadline", true);
                        break;
                    case TaskSortedByType.DeadLine:
                        sqlQuery.OrderBy(aliasPrefix + "deadline", orderBy.IsAsc)
                                .OrderBy(aliasPrefix + "title", true);
                        break;
                    case TaskSortedByType.Category:
                        sqlQuery.OrderBy(aliasPrefix + "category_id", orderBy.IsAsc)
                                .OrderBy(aliasPrefix + "deadline", true)
                                .OrderBy(aliasPrefix + "title", true);
                        break;
                    case TaskSortedByType.ContactManager:
                        sqlQuery.LeftOuterJoin("core_user u", Exp.EqColumns(aliasPrefix + "responsible_id", "u.id"))
                                .OrderBy("case when u.lastname is null or u.lastname = '' then 1 else 0 end, u.lastname", orderBy.IsAsc)
                                .OrderBy("case when u.firstname is null or u.firstname = '' then 1 else 0 end, u.firstname", orderBy.IsAsc)
                                .OrderBy(aliasPrefix + "deadline", true)
                                .OrderBy(aliasPrefix + "title", true);
                        break;
                    case TaskSortedByType.Contact:
                        sqlQuery.LeftOuterJoin("crm_contact c_tbl", Exp.EqColumns(aliasPrefix + "contact_id", "c_tbl.id"))
                                .OrderBy("case when c_tbl.display_name is null then 1 else 0 end, c_tbl.display_name", orderBy.IsAsc)
                                .OrderBy(aliasPrefix + "deadline", true)
                                .OrderBy(aliasPrefix + "title", true);
                        break;
                }
            }
            else
            {
                sqlQuery
                    .OrderBy(aliasPrefix + "deadline", true)
                    .OrderBy(aliasPrefix + "title", true);
            }

            return sqlQuery;

        }

        public Dictionary<int, Task> GetNearestTask(int[] contactID)
        {

                                                                
            throw new Exception();

            //Query(CRMDbContext.Tasks)
            //  .Where(x => contactID.Contains(x.ContactId) && !x.IsClosed)
            //  .GroupBy(x => x.ContactId)
            //  .Select(x => x.);


            //  var sqlSubQuery =
            //            Query("crm_task")
            //            .SelectMin("id")
            //            .SelectMin("deadline")
            //            .Select("contact_id")
            //            .Where(Exp.In("contact_id", contactID) & Exp.Eq("is_closed", false))
            //            .GroupBy("contact_id");

            //var taskIDs = Db.ExecuteList(sqlSubQuery).ConvertAll(row => row[0]);

            //if (taskIDs.Count == 0) return new Dictionary<int, Task>();

            //var tasks = Db.ExecuteList(GetTaskQuery(Exp.In("id", taskIDs))).ConvertAll(row => ToTask(row)).Where(CRMSecurity.CanAccessTo);

            //var result = new Dictionary<int, Task>();

            //foreach (var task in tasks.Where(task => !result.ContainsKey(task.ContactID)))
            //{
            //    result.Add(task.ContactID, task);
            //}

            //return result;
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
            return CRMDbContext.Tasks
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
            if (String.IsNullOrEmpty(newTask.Title) || newTask.DeadLine == DateTime.MinValue ||
                newTask.CategoryID <= 0)
                throw new ArgumentException();
         
            if (newTask.ID == 0 || Db.ExecuteScalar<int>(Query("crm_task").SelectCount().Where(Exp.Eq("id", newTask.ID))) == 0)
            {
                newTask.CreateOn = DateTime.UtcNow;
                newTask.CreateBy = SecurityContext.CurrentAccount.ID;

                newTask.LastModifedOn = DateTime.UtcNow;
                newTask.LastModifedBy = SecurityContext.CurrentAccount.ID;

                new DbTask
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


                newTask.ID = Db.ExecuteScalar<int>(
                               Insert("crm_task")
                              .InColumnValue("id", 0)
                              .InColumnValue("title", newTask.Title)
                              .InColumnValue("description", newTask.Description)
                              .InColumnValue("deadline", TenantUtil.DateTimeToUtc(newTask.DeadLine))
                              .InColumnValue("responsible_id", newTask.ResponsibleID)
                              .InColumnValue("contact_id", newTask.ContactID)
                              .InColumnValue("entity_type", (int)newTask.EntityType)
                              .InColumnValue("entity_id", newTask.EntityID)
                              .InColumnValue("is_closed", newTask.IsClosed)
                              .InColumnValue("category_id", newTask.CategoryID)
                              .InColumnValue("create_on", newTask.CreateOn)
                              .InColumnValue("create_by", newTask.CreateBy)
                              .InColumnValue("last_modifed_on", newTask.LastModifedOn)
                              .InColumnValue("last_modifed_by", newTask.LastModifedBy)
                              .InColumnValue("alert_value", newTask.AlertValue)
                              .Identity(1, 0, true));
            }
            else
            {
                var oldTask = Db.ExecuteList(GetTaskQuery(Exp.Eq("id", newTask.ID)))
                    .ConvertAll(row => ToTask(row))
                    .FirstOrDefault();

                CRMSecurity.DemandEdit(oldTask);

                newTask.CreateOn = oldTask.CreateOn;
                newTask.CreateBy = oldTask.CreateBy;

                newTask.LastModifedOn = DateTime.UtcNow;
                newTask.LastModifedBy = SecurityContext.CurrentAccount.ID;

                newTask.IsClosed = oldTask.IsClosed;

                Db.ExecuteNonQuery(
                                Update("crm_task")
                                .Set("title", newTask.Title)
                                .Set("description", newTask.Description)
                                .Set("deadline", TenantUtil.DateTimeToUtc(newTask.DeadLine))
                                .Set("responsible_id", newTask.ResponsibleID)
                                .Set("contact_id", newTask.ContactID)
                                .Set("entity_type", (int)newTask.EntityType)
                                .Set("entity_id", newTask.EntityID)
                                .Set("category_id", newTask.CategoryID)
                                .Set("last_modifed_on", newTask.LastModifedOn)
                                .Set("last_modifed_by", newTask.LastModifedBy)
                                .Set("alert_value", (int)newTask.AlertValue)
                                .Set("exec_alert", 0)
                                .Where(Exp.Eq("id", newTask.ID)));
            }

            FactoryIndexer.IndexAsync(newTask);

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

            FactoryIndexer.IndexAsync(newTask);

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

            FactoryIndexer.DeleteAsync(task);
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

        private String[] GetTaskColumnsTable(String alias)
        {
            if (!String.IsNullOrEmpty(alias))
                alias = alias + ".";

            var result = new List<String>
                             {
                                "id",
                                "contact_id",
                                "title",
                                "description",
                                "deadline",
                                "responsible_id",
                                "is_closed",
                                "category_id",
                                "entity_id",
                                "entity_type",
                                "create_on",
                                "create_by",
                                "alert_value"
                             };

            if (String.IsNullOrEmpty(alias)) return result.ToArray();

            return result.ConvertAll(item => String.Concat(alias, item)).ToArray();
        }

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
                CreateOn = dbTask.CreateOn,
                DeadLine = dbTask.Deadline,
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
