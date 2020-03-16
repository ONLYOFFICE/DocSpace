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
using ASC.Core;
using ASC.Core.Common.EF;
using ASC.Core.Tenants;
using ASC.CRM.Core.EF;
using ASC.CRM.Core.Entities;
using ASC.CRM.Core.Enums;
using ASC.Files.Core;
using ASC.Web.CRM.Core.Search;
using ASC.Web.Files.Api;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text.RegularExpressions;
using OrderBy = ASC.CRM.Core.Entities.OrderBy;

namespace ASC.CRM.Core.Dao
{
    public class CachedCasesDao : CasesDao
    {
        private readonly HttpRequestDictionary<Cases> _casesCache = new HttpRequestDictionary<Cases>("crm_cases");

        public CachedCasesDao(DbContextManager<CRMDbContext> dbContextManager,
            TenantManager tenantManager,
            SecurityContext securityContext,
            CRMSecurity cRMSecurity,
            TenantUtil tenantUtil,
            FilesIntegration filesIntegration
            ) :
                 base(dbContextManager,
                 tenantManager,
                 securityContext,
                 cRMSecurity,
                 tenantUtil,
                 filesIntegration)

        {

            

        }

        public override Cases GetByID(int caseID)
        {
            return _casesCache.Get(caseID.ToString(CultureInfo.InvariantCulture), () => GetByIDBase(caseID));
        }

        private Cases GetByIDBase(int caseID)
        {
            return base.GetByID(caseID);
        }

        public override void UpdateCases(Cases cases)
        {
            if (cases != null && cases.ID > 0)
                ResetCache(cases.ID);

            base.UpdateCases(cases);
        }

        public override Cases DeleteCases(int casesID)
        {
            ResetCache(casesID);

            return base.DeleteCases(casesID);
        }

        private void ResetCache(int taskID)
        {
            _casesCache.Reset(taskID.ToString(CultureInfo.InvariantCulture));
        }
    }

    public class CasesDao : AbstractDao
    {
        public CasesDao(
            DbContextManager<CRMDbContext> dbContextManager,
            TenantManager tenantManager,
            SecurityContext securityContext,
            CRMSecurity cRMSecurity,
            TenantUtil tenantUtil,
            FilesIntegration filesIntegration,
            AuthorizationManager authorizationManager
            ) :
                 base(dbContextManager,
                 tenantManager,
                 securityContext)
        {
            CRMSecurity = cRMSecurity;
            TenantUtil = tenantUtil;
            FilesIntegration = filesIntegration;
            AuthorizationManager = authorizationManager;
        }

        public AuthorizationManager AuthorizationManager { get; }

        public FilesIntegration FilesIntegration { get; }

        public TenantUtil TenantUtil { get; }

        public CRMSecurity CRMSecurity { get; }

        public void AddMember(int caseID, int memberID)
        {
            SetRelative(memberID, EntityType.Case, caseID);
        }

        public Dictionary<int, int[]> GetMembers(int[] caseID)
        {
            return GetRelativeToEntity(null, EntityType.Case, caseID);
        }

        public int[] GetMembers(int caseID)
        {
            return GetRelativeToEntity(null, EntityType.Case, caseID);
        }

        public void SetMembers(int caseID, int[] memberID)
        {
            SetRelative(memberID, EntityType.Case, caseID);
        }

        public void RemoveMember(int caseID, int memberID)
        {
            RemoveRelative(memberID, EntityType.Case, caseID);
        }

        public virtual int[] SaveCasesList(List<Cases> items)
        {
            using var tx = CRMDbContext.Database.BeginTransaction();

            var result = items.Select(item => CreateCasesInDb(item.Title)).ToArray();

            tx.Commit();

            // Delete relative keys
            _cache.Remove(new Regex(TenantID.ToString(CultureInfo.InvariantCulture) + "cases.*"));

            return result;
        }

        public Cases CloseCases(int caseID)
        {
            if (caseID <= 0) throw new ArgumentException();

            var cases = GetByID(caseID);

            if (cases == null) return null;

            CRMSecurity.DemandAccessTo(cases);

            CRMDbContext.Cases.Update(new DbCase
            {
                Id = cases.ID,
                CreateBy = cases.CreateBy,
                CreateOn = cases.CreateOn,
                IsClosed = true,
                LastModifedBy = cases.LastModifedBy,
                LastModifedOn = cases.LastModifedOn,
                TenantId = TenantID,
                Title = cases.Title
            });

            CRMDbContext.SaveChanges();

            cases.IsClosed = true;

            return cases;
        }

        public Cases ReOpenCases(int caseID)
        {
            if (caseID <= 0) throw new ArgumentException();

            var cases = GetByID(caseID);

            if (cases == null) return null;

            CRMSecurity.DemandAccessTo(cases);

            CRMDbContext.Cases.Update(new DbCase
            {
                Id = cases.ID,
                CreateBy = cases.CreateBy,
                CreateOn = cases.CreateOn,
                IsClosed = false,
                LastModifedBy = cases.LastModifedBy,
                LastModifedOn = cases.LastModifedOn,
                TenantId = TenantID,
                Title = cases.Title
            });

            CRMDbContext.SaveChanges();

            cases.IsClosed = false;

            return cases;
        }

        public int CreateCases(String title)
        {
            var result = CreateCasesInDb(title);
            // Delete relative keys
            _cache.Remove(new Regex(TenantID.ToString(CultureInfo.InvariantCulture) + "invoice.*"));

            return result;
        }

        private int CreateCasesInDb(String title)
        {
            var dbCase = new DbCase
            {
                Title = title,
                IsClosed = false,
                CreateOn = TenantUtil.DateTimeToUtc(TenantUtil.DateTimeNow()),
                CreateBy = SecurityContext.CurrentAccount.ID,
                LastModifedOn = TenantUtil.DateTimeToUtc(TenantUtil.DateTimeNow()),
                LastModifedBy = SecurityContext.CurrentAccount.ID,
                TenantId = TenantID
            };

            CRMDbContext.Cases.Add(dbCase);

            CRMDbContext.SaveChanges();

            return dbCase.Id;

        }

        public virtual void UpdateCases(Cases cases)
        {
            CRMSecurity.DemandEdit(cases);

            // Delete relative keys
            _cache.Remove(new Regex(TenantID.ToString(CultureInfo.InvariantCulture) + "invoice.*"));

            CRMDbContext.Cases.Update(new DbCase
            {
                Id = cases.ID,
                Title = cases.Title,
                IsClosed = cases.IsClosed,
                LastModifedOn = TenantUtil.DateTimeToUtc(TenantUtil.DateTimeNow()),
                LastModifedBy = SecurityContext.CurrentAccount.ID,
                TenantId = TenantID,
                CreateBy = cases.CreateBy,
                CreateOn = cases.CreateOn
            });

            CRMDbContext.SaveChanges();
        }

        public virtual Cases DeleteCases(int casesID)
        {
            if (casesID <= 0) return null;

            var cases = GetByID(casesID);
            if (cases == null) return null;

            CRMSecurity.DemandDelete(cases);

            // Delete relative  keys
            _cache.Remove(new Regex(TenantID.ToString(CultureInfo.InvariantCulture) + "invoice.*"));

            DeleteBatchCases(new[] { casesID });
            return cases;
        }

        public virtual List<Cases> DeleteBatchCases(List<Cases> caseses)
        {
            caseses = caseses.FindAll(CRMSecurity.CanDelete).ToList();
            if (!caseses.Any()) return caseses;

            // Delete relative  keys
            _cache.Remove(new Regex(TenantID.ToString(CultureInfo.InvariantCulture) + "invoice.*"));

            DeleteBatchCasesExecute(caseses);

            return caseses;
        }

        public virtual List<Cases> DeleteBatchCases(int[] casesID)
        {
            if (casesID == null || !casesID.Any()) return null;

            var cases = GetCases(casesID).FindAll(CRMSecurity.CanDelete).ToList();
            if (!cases.Any()) return cases;

            // Delete relative  keys
            _cache.Remove(new Regex(TenantID.ToString(CultureInfo.InvariantCulture) + "invoice.*"));

            DeleteBatchCasesExecute(cases);

            return cases;
        }

        private void DeleteBatchCasesExecute(List<Cases> caseses)
        {
            var casesID = caseses.Select(x => x.ID).ToArray();

            var tagdao = FilesIntegration.GetTagDao();

            var tagNames = Db.ExecuteList(Query("crm_relationship_event").Select("id")
                .Where(Exp.Eq("have_files", true) & Exp.In("entity_id", casesID) & Exp.Eq("entity_type", (int)EntityType.Case)))
                .Select(row => String.Format("RelationshipEvent_{0}", row[0])).ToArray();

            var filesIDs = tagdao.GetTags(tagNames, TagType.System).Where(t => t.EntryType == FileEntryType.File).Select(t => t.EntryId).ToArray();

            using var tx = CRMDbContext.Database.BeginTransaction();

            Db.ExecuteNonQuery(Delete("crm_field_value").Where(Exp.In("entity_id", casesID) & Exp.Eq("entity_type", (int)EntityType.Case)));
            Db.ExecuteNonQuery(Delete("crm_relationship_event").Where(Exp.In("entity_id", casesID) & Exp.Eq("entity_type", (int)EntityType.Case)));
            Db.ExecuteNonQuery(Delete("crm_task").Where(Exp.In("entity_id", casesID) & Exp.Eq("entity_type", (int)EntityType.Case)));
            Db.ExecuteNonQuery(new SqlDelete("crm_entity_tag").Where(Exp.In("entity_id", casesID) & Exp.Eq("entity_type", (int)EntityType.Case)));
            Db.ExecuteNonQuery(Delete("crm_case").Where(Exp.In("id", casesID)));

            tx.Commit();

            caseses.ForEach(item => AuthorizationManager.RemoveAllAces(item));

            if (0 < tagNames.Length)
            {
                var filedao = FilesIntegration.GetFileDao();

                foreach (var filesID in filesIDs)
                {
                    filedao.DeleteFile(filesID);
                }
            }

            //todo: remove indexes
        }

        public List<Cases> GetAllCases()
        {
            return GetCases(String.Empty, 0, null, null, 0, 0, new OrderBy(Enums.SortedByType.Title, true));
        }

        public int GetCasesCount()
        {
            return GetCasesCount(String.Empty, 0, null, null);
        }

        public int GetCasesCount(
                                String searchText,
                                int contactID,
                                bool? isClosed,
                                IEnumerable<String> tags)
        {

            var cacheKey = TenantID.ToString(CultureInfo.InvariantCulture) +
                           "cases" +
                           SecurityContext.CurrentAccount.ID.ToString() +
                           searchText +
                           contactID;

            if (tags != null)
                cacheKey += String.Join("", tags.ToArray());

            if (isClosed.HasValue)
                cacheKey += isClosed.Value;

            var fromCache = _cache.Get<string>(cacheKey);

            if (fromCache != null) return Convert.ToInt32(fromCache);


            var withParams = !(String.IsNullOrEmpty(searchText) &&
                               contactID <= 0 &&
                               isClosed == null &&
                               (tags == null || !tags.Any()));


            var exceptIDs = CRMSecurity.GetPrivateItems(typeof(Cases)).ToList();

            int result;

            if (withParams)
            {
                var whereConditional = WhereConditional(exceptIDs, searchText, contactID, isClosed, tags);
                result = whereConditional != null ? Db.ExecuteScalar<int>(Query("crm_case").Where(whereConditional).SelectCount()) : 0;
            }
            else
            {
                var countWithoutPrivate = Db.ExecuteScalar<int>(Query("crm_case").SelectCount());
                var privateCount = exceptIDs.Count;

                if (privateCount > countWithoutPrivate)
                {
                    _log.ErrorFormat(@"Private cases count more than all cases. Tenant: {0}. CurrentAccount: {1}",
                                                            TenantID,
                                                            SecurityContext.CurrentAccount.ID);

                    privateCount = 0;
                }

                result = countWithoutPrivate - privateCount;

            }

            if (result > 0)
            {
                _cache.Insert(cacheKey, result, TimeSpan.FromSeconds(30));
            }
            return result;
        }


        private Exp WhereConditional(
                                ICollection<int> exceptIDs,
                                String searchText,
                                int contactID,
                                bool? isClosed,
                                IEnumerable<String> tags)
        {

            var conditions = new List<Exp>();

            var ids = new List<int>();

            if (!String.IsNullOrEmpty(searchText))
            {
                searchText = searchText.Trim();

                var keywords = searchText.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries)
                   .ToArray();

                if (keywords.Length > 0)
                {
                    if (!BundleSearch.TrySelectCase(searchText, out ids))
                    {
                        conditions.Add(BuildLike(new[] { "title" }, keywords));
                    }
                    else if (!ids.Any())
                    {
                        return null;

                    }
                }
            }

            if (contactID > 0)
            {

                var sqlQuery = new SqlQuery("crm_entity_contact")
                    .Select("entity_id")
                    .Where(Exp.Eq("contact_id", contactID) & Exp.Eq("entity_type", (int)EntityType.Case));

                if (ids.Count > 0)
                    sqlQuery.Where(Exp.In("entity_id", ids));

                ids = Db.ExecuteList(sqlQuery).Select(item => Convert.ToInt32(item[0])).ToList();
                if (ids.Count == 0) return null;
            }

            if (isClosed.HasValue)
                conditions.Add(Exp.Eq("is_closed", isClosed));

            if (tags != null && tags.Any())
            {
                ids = SearchByTags(EntityType.Case, ids.ToArray(), tags);

                if (ids.Count == 0) return null;
            }

            if (ids.Count > 0)
            {
                if (exceptIDs.Count > 0)
                {
                    ids = ids.Except(exceptIDs).ToList();

                    if (ids.Count == 0) return null;
                }

                conditions.Add(Exp.In("id", ids));

            }
            else if (exceptIDs.Count > 0)
            {
                conditions.Add(!Exp.In("id", exceptIDs.ToArray()));
            }

            if (conditions.Count == 0) return null;

            return conditions.Count == 1 ? conditions[0] : conditions.Aggregate((i, j) => i & j);
        }

        public List<Cases> GetCases(IEnumerable<int> casesID)
        {

            if (casesID == null || !casesID.Any()) return new List<Cases>();

            var sqlQuery = GetCasesSqlQuery(Exp.In("id", casesID.ToArray()));

            return Db.ExecuteList(sqlQuery).ConvertAll(ToCases).FindAll(CRMSecurity.CanAccessTo);
        }

        public List<Cases> GetCases(
                                 String searchText,
                                 int contactID,
                                 bool? isClosed,
                                 IEnumerable<String> tags,
                                 int from,
                                 int count,
                                 OrderBy orderBy)
        {
            var sqlQuery = GetCasesSqlQuery(null);

            var withParams = !(String.IsNullOrEmpty(searchText) &&
                          contactID <= 0 &&
                          isClosed == null &&
                          (tags == null || !tags.Any()));

            var whereConditional = WhereConditional(CRMSecurity.GetPrivateItems(typeof(Cases)).ToList(), searchText,
                                                    contactID, isClosed,
                                                    tags);

            if (withParams && whereConditional == null)
                return new List<Cases>();

            sqlQuery.Where(whereConditional);

            if (0 < from && from < int.MaxValue) sqlQuery.SetFirstResult(from);
            if (0 < count && count < int.MaxValue) sqlQuery.SetMaxResults(count);

            sqlQuery.OrderBy("is_closed", true);


            if (orderBy != null && Enum.IsDefined(typeof(SortedByType), orderBy.SortedBy))
                switch ((SortedByType)orderBy.SortedBy)
                {
                    case SortedByType.Title:
                        sqlQuery.OrderBy("title", orderBy.IsAsc);
                        break;
                    case SortedByType.CreateBy:
                        sqlQuery.OrderBy("create_by", orderBy.IsAsc);
                        break;
                    case SortedByType.DateAndTime:
                        sqlQuery.OrderBy("create_on", orderBy.IsAsc);
                        break;
                }


            return Db.ExecuteList(sqlQuery).ConvertAll(ToCases);
        }

        public List<Cases> GetCasesByPrefix(String prefix, int from, int count)
        {
            if (count == 0)
                throw new ArgumentException();

            prefix = prefix.Trim();

            var keywords = prefix.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries).ToArray();

            var q = GetCasesSqlQuery(null);

            if (keywords.Length == 1)
            {
                q.Where(Exp.Like("title", keywords[0]));
            }
            else
            {
                foreach (var k in keywords)
                {
                    q.Where(Exp.Like("title", k));
                }
            }

            if (0 < from && from < int.MaxValue) q.SetFirstResult(from);
            if (0 < count && count < int.MaxValue) q.SetMaxResults(count);

            var sqlResult = Db.ExecuteList(q).ConvertAll(row => ToCases(row)).FindAll(CRMSecurity.CanAccessTo);
            return sqlResult.OrderBy(cases => cases.Title).ToList();
        }

        public virtual List<Cases> GetByID(int[] ids)
        {
            return CRMDbContext.Cases
                               .Where(x => ids.Contains(x.Id))
                               .ToList()
                               .ConvertAll(ToCases);
        }

        public virtual Cases GetByID(int id)
        {
            if (id <= 0) return null;

            var cases = GetByID(new[] { id });

            return cases.Count == 0 ? null : cases[0];
        }

        private Cases ToCases(DbCase dbCase)
        {
            return new Cases
            {
                ID = dbCase.Id,
                Title = dbCase.Title,
                CreateBy = dbCase.CreateBy,
                CreateOn = dbCase.CreateOn,
                IsClosed = dbCase.IsClosed,
                LastModifedBy = dbCase.LastModifedBy,
                LastModifedOn = dbCase.LastModifedOn
            };
        }

        private SqlQuery GetCasesSqlQuery(Exp where)
        {

            var sqlQuery = Query("crm_case")
                .Select("id", "title", "create_by", "create_on", "is_closed");

            if (where != null)
            {
                sqlQuery.Where(where);
            }

            return sqlQuery;
        }

        public void ReassignCasesResponsible(Guid fromUserId, Guid toUserId)
        {
            var cases = GetAllCases();

            foreach (var item in cases)
            {
                var responsibles = CRMSecurity.GetAccessSubjectGuidsTo(item);

                if (!responsibles.Any()) continue;

                responsibles.Remove(fromUserId);
                responsibles.Add(toUserId);

                CRMSecurity.SetAccessTo(item, responsibles.Distinct().ToList());
            }
        }
    }
}