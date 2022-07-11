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


using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

using ASC.Common;
using ASC.Common.Caching;
using ASC.Common.Logging;
using ASC.Core;
using ASC.Core.Common.EF;
using ASC.Core.Common.EF.Context;
using ASC.Core.Tenants;
using ASC.CRM.Core.EF;
using ASC.CRM.Core.Entities;
using ASC.CRM.Core.Enums;
using ASC.Files.Core;
using ASC.Web.CRM.Core.Search;
using ASC.Web.Files.Api;

using AutoMapper;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

using OrderBy = ASC.CRM.Core.Entities.OrderBy;
using SortedByType = ASC.CRM.Core.Enums.SortedByType;

namespace ASC.CRM.Core.Dao
{
    [Scope]
    public class CasesDao : AbstractDao
    {
        private readonly BundleSearch _bundleSearch;
        private readonly AuthorizationManager _authorizationManager;
        private readonly FilesIntegration _filesIntegration;
        private readonly TenantUtil _tenantUtil;
        private readonly CrmSecurity _crmSecurity;

        public CasesDao(
            DbContextManager<CrmDbContext> dbContextManager,
            TenantManager tenantManager,
            SecurityContext securityContext,
            CrmSecurity crmSecurity,
            TenantUtil tenantUtil,
            FilesIntegration filesIntegration,
            AuthorizationManager authorizationManager,
            IOptionsMonitor<ILog> logger,
            ICache ascCache,
            BundleSearch bundleSearch,
            IMapper mapper
            ) :
                 base(dbContextManager,                    
                 tenantManager,
                 securityContext,
                 logger,
                 ascCache,
                 mapper)
        {
            _crmSecurity = crmSecurity;
            _tenantUtil = tenantUtil;
            _filesIntegration = filesIntegration;
            _authorizationManager = authorizationManager;
            _bundleSearch = bundleSearch;
        }

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

        public int[] SaveCasesList(List<Cases> items)
        {
            using var tx = CrmDbContext.Database.BeginTransaction();

            var result = items.Select(item => CreateCasesInDb(item.Title)).ToArray();

            tx.Commit();

            // Delete relative keys
            _cache.Remove(new Regex(TenantID.ToString(CultureInfo.InvariantCulture) + "cases.*"));

            return result;
        }

        public Cases CloseCases(int id)
        {
            var dbEntity = CrmDbContext.Cases.Find(id);

            if (dbEntity == null)
                throw new ArgumentException();

            var entity = _mapper.Map<Cases>(dbEntity);

            if (dbEntity.IsClosed) return entity;
            if (dbEntity.TenantId != TenantID) return null;

            _crmSecurity.DemandEdit(entity);

            dbEntity.IsClosed = true;

            CrmDbContext.SaveChanges();

            return entity;
        }

        public Cases ReOpenCases(int id)
        {
            var dbEntity = CrmDbContext.Cases.Find(id);

            if (dbEntity == null)
                throw new ArgumentException();

            var entity = _mapper.Map<Cases>(dbEntity);

            if (!dbEntity.IsClosed) return entity;
            if (dbEntity.TenantId != TenantID) return null;

            _crmSecurity.DemandEdit(entity);

            dbEntity.IsClosed = false;

            CrmDbContext.SaveChanges();

            return entity;
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
                CreateOn = _tenantUtil.DateTimeToUtc(_tenantUtil.DateTimeNow()),
                CreateBy = _securityContext.CurrentAccount.ID,
                LastModifedOn = _tenantUtil.DateTimeToUtc(_tenantUtil.DateTimeNow()),
                LastModifedBy = _securityContext.CurrentAccount.ID,
                TenantId = TenantID
            };

            CrmDbContext.Cases.Add(dbCase);

            CrmDbContext.SaveChanges();

            return dbCase.Id;

        }

        public void UpdateCases(Cases cases)
        {
            _crmSecurity.DemandEdit(cases);

            // Delete relative keys
            _cache.Remove(new Regex(TenantID.ToString(CultureInfo.InvariantCulture) + "invoice.*"));

            CrmDbContext.Cases.Update(new DbCase
            {
                Id = cases.ID,
                Title = cases.Title,
                IsClosed = cases.IsClosed,
                LastModifedOn = _tenantUtil.DateTimeToUtc(_tenantUtil.DateTimeNow()),
                LastModifedBy = _securityContext.CurrentAccount.ID,
                TenantId = TenantID,
                CreateBy = cases.CreateBy,
                CreateOn = cases.CreateOn
            });

            CrmDbContext.SaveChanges();
        }

        public Task<Cases> DeleteCasesAsync(int casesID)
        {
            if (casesID <= 0) return System.Threading.Tasks.Task.FromResult<Cases>(null);

            var cases = GetByID(casesID);

            if (cases == null) return System.Threading.Tasks.Task.FromResult<Cases>(null);

            return InternalDeleteCasesAsync(casesID, cases);
        }

        private async Task<Cases> InternalDeleteCasesAsync(int casesID, Cases cases)
        {
            _crmSecurity.DemandDelete(cases);

            // Delete relative  keys
            _cache.Remove(new Regex(TenantID.ToString(CultureInfo.InvariantCulture) + "invoice.*"));

            await DeleteBatchCasesAsync(new[] { casesID });
            return cases;
        }

        public Task<List<Cases>> DeleteBatchCasesAsync(List<Cases> caseses)
        {
            caseses = caseses.FindAll(_crmSecurity.CanDelete).ToList();

            if (caseses.Count == 0) return System.Threading.Tasks.Task.FromResult(caseses);

            return InternalDeleteBatchCasesAsync(caseses);
        }

        public Task<List<Cases>> DeleteBatchCasesAsync(int[] casesID)
        {
            if (casesID == null || casesID.Length == 0) return null;

            var cases = GetCases(casesID).FindAll(_crmSecurity.CanDelete).ToList();

            if (cases.Count == 0) return System.Threading.Tasks.Task.FromResult(cases);

            return InternalDeleteBatchCasesAsync(cases);
        }

        private async Task<List<Cases>> InternalDeleteBatchCasesAsync(List<Cases> cases)
        {
            // Delete relative  keys
            _cache.Remove(new Regex(TenantID.ToString(CultureInfo.InvariantCulture) + "invoice.*"));

            await DeleteBatchCasesExecuteAsync(cases);

            return cases;
        }

        private async System.Threading.Tasks.Task DeleteBatchCasesExecuteAsync(List<Cases> caseses)
        {
            var casesID = caseses.Select(x => x.ID).ToArray();

            var tagdao = _filesIntegration.DaoFactory.GetTagDao<int>();

            var tagNames = Query(CrmDbContext.RelationshipEvent)
                            .Where(x => x.HaveFiles && casesID.Contains(x.EntityId) && x.EntityType == EntityType.Case)
                            .Select(x => String.Format("RelationshipEvent_{0}", x.Id)).ToArray();

            var filesIDs = tagdao.GetTagsAsync(tagNames, TagType.System).Where(t => t.EntryType == FileEntryType.File).Select(t => Convert.ToInt32(t.EntryId));

            using var tx = await CrmDbContext.Database.BeginTransactionAsync();

            CrmDbContext.RemoveRange(Query(CrmDbContext.FieldValue)
                                     .Where(x => casesID.Contains(x.EntityId) && x.EntityType == EntityType.Case));

            CrmDbContext.RemoveRange(Query(CrmDbContext.RelationshipEvent)
                                     .Where(x => casesID.Contains(x.EntityId) && x.EntityType == EntityType.Case));

            CrmDbContext.RemoveRange(Query(CrmDbContext.Tasks)
                                    .Where(x => casesID.Contains(x.EntityId) && x.EntityType == EntityType.Case));

            CrmDbContext.RemoveRange(CrmDbContext.EntityTags.AsQueryable().Where(x => casesID.Contains(x.EntityId) && x.EntityType == EntityType.Case));

            CrmDbContext.Cases.RemoveRange(caseses.ConvertAll(x => new DbCase
            {
                Id = x.ID,
                TenantId = TenantID
            }));

            await CrmDbContext.SaveChangesAsync();

            await tx.CommitAsync();

            caseses.ForEach(item => _authorizationManager.RemoveAllAces(item));

            if (0 < tagNames.Length)
            {
                var filedao = _filesIntegration.DaoFactory.GetFileDao<int>();

                await foreach (var filesID in filesIDs)
                {
                    await filedao.DeleteFileAsync(filesID);
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
                           _securityContext.CurrentAccount.ID.ToString() +
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


            var exceptIDs = _crmSecurity.GetPrivateItems(typeof(Cases)).ToList();

            int result;

            if (withParams)
            {
                var dbCasesQuery = GetDbCasesByFilters(exceptIDs, searchText, contactID, isClosed, tags);

                result = dbCasesQuery != null ? dbCasesQuery.Count() : 0;

            }
            else
            {

                var countWithoutPrivate = Query(CrmDbContext.Cases).Count();

                var privateCount = exceptIDs.Count;

                if (privateCount > countWithoutPrivate)
                {
                    _logger.ErrorFormat(@"Private cases count more than all cases. Tenant: {0}. CurrentAccount: {1}",
                                                            TenantID,
                                                            _securityContext.CurrentAccount.ID);
                    privateCount = 0;
                }

                result = countWithoutPrivate - privateCount;

            }

            if (result > 0)
            {
                _cache.Insert(cacheKey, result.ToString(), TimeSpan.FromSeconds(30));
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

            var result = Query(CrmDbContext.Cases).AsNoTracking();

            var ids = new List<int>();

            if (!String.IsNullOrEmpty(searchText))
            {
                searchText = searchText.Trim();

                var keywords = searchText.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries)
                   .ToArray();

                if (keywords.Length > 0)
                {
                    if (!_bundleSearch.TrySelectCase(searchText, out ids))
                    {
                        foreach (var k in keywords)
                        {
                            result = result.Where(x => Microsoft.EntityFrameworkCore.EF.Functions.Like(x.Title, k + "%"));
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
                var sqlQuery = CrmDbContext.EntityContact
                    .AsQueryable()
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

            var result = Query(CrmDbContext.Cases)
                        .Where(x => casesID.Contains(x.Id))
                        .AsNoTracking()
                        .ToList();

            return _mapper.Map<List<DbCase>, List<Cases>>(result)
                                        .FindAll(_crmSecurity.CanAccessTo);
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
            var dbCasesQuery = GetDbCasesByFilters(_crmSecurity.GetPrivateItems(typeof(Cases)).ToList(), searchText,
                                                    contactID, isClosed,
                                                    tags);

            if (dbCasesQuery == null) return new List<Cases>();

            if (0 < from && from < int.MaxValue) dbCasesQuery = dbCasesQuery.Skip(from);
            if (0 < count && count < int.MaxValue) dbCasesQuery = dbCasesQuery.Take(count);

            dbCasesQuery = dbCasesQuery.OrderBy("IsClosed", orderBy.IsAsc);

            if (orderBy != null && Enum.IsDefined(typeof(Enums.SortedByType), orderBy.SortedBy))
            {
                switch ((SortedByType)orderBy.SortedBy)
                {
                    case SortedByType.Title:
                        dbCasesQuery = dbCasesQuery.OrderBy("Title", orderBy.IsAsc);
                        break;
                    case SortedByType.CreateBy:
                        dbCasesQuery = dbCasesQuery.OrderBy("CreateBy", orderBy.IsAsc);
                        break;
                    case SortedByType.DateAndTime:
                        dbCasesQuery = dbCasesQuery.OrderBy("CreateOn", orderBy.IsAsc);
                        break;
                }
            }

            return _mapper.Map<List<DbCase>, List<Cases>>(dbCasesQuery.ToList());
        }

        public List<Cases> GetCasesByPrefix(String prefix, int from, int count)
        {
            if (count == 0)
                throw new ArgumentException();

            prefix = prefix.Trim();

            var keywords = prefix.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries).ToArray();

            var q = Query(CrmDbContext.Cases).AsNoTracking();

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

            return _mapper.Map<List<DbCase>, List<Cases>>(q.ToList())
                    .FindAll(_crmSecurity.CanAccessTo);
        }


        public List<Cases> GetByID(int[] ids)
        {
            var result = Query(CrmDbContext.Cases)
                               .Where(x => ids.Contains(x.Id))
                               .AsNoTracking()
                               .ToList();

            return _mapper.Map<List<DbCase>, List<Cases>>(result);
        }

        public Cases GetByID(int id)
        {
            var dbEntity = CrmDbContext.Cases.Find(id);

            if (dbEntity.TenantId != TenantID) return null;

            var entity = _mapper.Map<Cases>(dbEntity);

            _crmSecurity.DemandAccessTo(entity);

            return _mapper.Map<Cases>(dbEntity);
        }

        public void ReassignCasesResponsible(Guid fromUserId, Guid toUserId)
        {
            var cases = GetAllCases();

            foreach (var item in cases)
            {
                var responsibles = _crmSecurity.GetAccessSubjectGuidsTo(item);

                if (!responsibles.Any()) continue;

                responsibles.Remove(fromUserId);
                responsibles.Add(toUserId);

                _crmSecurity.SetAccessTo(item, responsibles.Distinct().ToList());
            }
        }
    }
}