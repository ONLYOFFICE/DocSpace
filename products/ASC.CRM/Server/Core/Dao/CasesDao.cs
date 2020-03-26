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
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text.RegularExpressions;
using OrderBy = ASC.CRM.Core.Entities.OrderBy;
using Microsoft.EntityFrameworkCore;
using SortedByType = ASC.CRM.Core.Enums.SortedByType;

namespace ASC.CRM.Core.Dao
{
    public class CachedCasesDao : CasesDao
    {
        private readonly HttpRequestDictionary<Cases> _casesCache;

        public CachedCasesDao(DbContextManager<CRMDbContext> dbContextManager,
            TenantManager tenantManager,
            SecurityContext securityContext,
            CRMSecurity cRMSecurity,
            TenantUtil tenantUtil,
            FilesIntegration filesIntegration,
            AuthorizationManager authorizationManager,
            IHttpContextAccessor httpContextAccessor)
            :
                 base(dbContextManager,
                 tenantManager,
                 securityContext,
                 cRMSecurity,
                 tenantUtil,
                 filesIntegration,
                 authorizationManager)

        {
            _casesCache = new HttpRequestDictionary<Cases>(httpContextAccessor?.HttpContext, "crm_cases");
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

            var itemToUpdate = new DbCase
            {
                Id = cases.ID,
                CreateBy = cases.CreateBy,
                CreateOn = cases.CreateOn,
                IsClosed = true,
                LastModifedBy = cases.LastModifedBy,
                LastModifedOn = cases.LastModifedOn,
                TenantId = TenantID,
                Title = cases.Title
            };

            CRMDbContext.Cases.Update(itemToUpdate);

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

            var tagdao = FilesIntegration.TagDao();

            var tagNames = Query(CRMDbContext.RelationshipEvent)
                            .Where(x => x.HaveFiles && casesID.Contains(x.EntityId) && x.EntityType == EntityType.Case)
                            .Select(x => String.Format("RelationshipEvent_{0}", x.Id)).ToArray();
            
            var filesIDs = tagdao.GetTags(tagNames, TagType.System).Where(t => t.EntryType == FileEntryType.File).Select(t => t.EntryId).ToArray();

            using var tx = CRMDbContext.Database.BeginTransaction();

            CRMDbContext.RemoveRange(Query(CRMDbContext.FieldValue)
                                     .Where(x => casesID.Contains(x.EntityId) && x.EntityType == EntityType.Case));

            CRMDbContext.RemoveRange(Query(CRMDbContext.RelationshipEvent)
                                     .Where(x => casesID.Contains(x.EntityId) && x.EntityType == EntityType.Case));

            CRMDbContext.RemoveRange(Query(CRMDbContext.Tasks)
                                    .Where(x => casesID.Contains(x.EntityId) && x.EntityType == EntityType.Case));

            CRMDbContext.RemoveRange(CRMDbContext.EntityTags.Where(x => casesID.Contains(x.EntityId) && x.EntityType == EntityType.Case));

            CRMDbContext.Cases.RemoveRange(caseses.ConvertAll(x => new DbCase
            {
                 Id = x.ID
            }));

            CRMDbContext.SaveChanges();

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
                var dbCasesQuery = GetDbCasesByFilters(exceptIDs, searchText, contactID, isClosed, tags);
                                               
                result = dbCasesQuery != null ? dbCasesQuery.Count() : 0;
            
            }
            else
            {

                var countWithoutPrivate = Query(CRMDbContext.Cases).Count();
                
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


        private IQueryable<DbCase> GetDbCasesByFilters(
                                ICollection<int> exceptIDs,
                                String searchText,
                                int contactID,
                                bool? isClosed,
                                IEnumerable<String> tags)
        {

            var result = Query(CRMDbContext.Cases);

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
                        foreach(var k in keywords)
                        { 
                            result = result.Where(x => Microsoft.EntityFrameworkCore.EF.Functions.Like(x.Title, k));
                        }
                    }
                    else if (!ids.Any())
                    {
                        return null;
                    }
                }
            }

            if (contactID > 0)
            {
                var sqlQuery = CRMDbContext.EntityContact
                    .Where(x => x.ContactId == contactID && x.EntityType == EntityType.Case);
                
                if (ids.Count > 0)
                    sqlQuery = sqlQuery.Where(x => ids.Contains(x.EntityId));

                ids = sqlQuery.Select(x => x.EntityId).ToList();
                
                if (ids.Count == 0) return null;
            }

            if (isClosed.HasValue)
            {
                result = result.Where(x => x.IsClosed == isClosed);
            }

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

                result = result.Where(x => ids.Contains(x.Id));

            }
            else if (exceptIDs.Count > 0)
            {
                result = result.Where(x => !exceptIDs.Contains(x.Id));
            }

            return result;
        }

        public List<Cases> GetCases(IEnumerable<int> casesID)
        {
            if (casesID == null || !casesID.Any()) return new List<Cases>();

            return Query(CRMDbContext.Cases)
                        .Where(x => casesID.Contains(x.Id))
                        .ToList()
                        .ConvertAll(ToCases)
                        .FindAll(CRMSecurity.CanAccessTo);
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
            var dbCasesQuery = GetDbCasesByFilters(CRMSecurity.GetPrivateItems(typeof(Cases)).ToList(), searchText,
                                                    contactID, isClosed,
                                                    tags);

            if (dbCasesQuery == null) return new List<Cases>();

            if (0 < from && from < int.MaxValue) dbCasesQuery.Skip(from);
            if (0 < count && count < int.MaxValue) dbCasesQuery.Take(count);

            dbCasesQuery = dbCasesQuery.OrderBy(x => x.IsClosed);

            if (orderBy != null && Enum.IsDefined(typeof(Enums.SortedByType), orderBy.SortedBy))
                switch ((SortedByType)orderBy.SortedBy)
                {
                    case SortedByType.Title:
                        dbCasesQuery = dbCasesQuery.OrderBy(x => x.Title);
                        break;
                    case SortedByType.CreateBy:
                        dbCasesQuery = dbCasesQuery.OrderBy(x => x.CreateBy);
                        break;
                    case SortedByType.DateAndTime:
                        dbCasesQuery = dbCasesQuery.OrderBy(x => x.CreateOn);
                        break;
                }


            return dbCasesQuery.ToList().ConvertAll(ToCases);
        }

        public List<Cases> GetCasesByPrefix(String prefix, int from, int count)
        {
            if (count == 0)
                throw new ArgumentException();

            prefix = prefix.Trim();

            var keywords = prefix.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries).ToArray();

            var q = Query(CRMDbContext.Cases); 
          
            if (keywords.Length == 1)
            {
                q = q.Where(x => Microsoft.EntityFrameworkCore.EF.Functions.Like(x.Title, keywords[0]));
            }
            else
            {
                foreach (var k in keywords)
                {
                    q = q.Where(x => Microsoft.EntityFrameworkCore.EF.Functions.Like(x.Title, k));
                }
            }
            
            if (0 < from && from < int.MaxValue) q = q.Skip(from);
            if (0 < count && count < int.MaxValue) q = q.Take(count);

            q = q.OrderBy(x => x.Title);
                       
            return q.ToList().ConvertAll(ToCases).FindAll(CRMSecurity.CanAccessTo);
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
            if (dbCase == null) return null;

            return new Cases
            {
                ID = dbCase.Id,
                Title = dbCase.Title,
                CreateBy = dbCase.CreateBy,
                CreateOn = TenantUtil.DateTimeFromUtc(dbCase.CreateOn),
                IsClosed = dbCase.IsClosed,
                LastModifedBy = dbCase.LastModifedBy,
                LastModifedOn = dbCase.LastModifedOn
            };
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