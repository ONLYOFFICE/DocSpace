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
using ASC.ElasticSearch;
using ASC.Files.Core;
using ASC.Web.CRM.Core.Search;
using ASC.Web.Files.Api;

using AutoMapper;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

using OrderBy = ASC.CRM.Core.Entities.OrderBy;

namespace ASC.CRM.Core.Dao
{
    [Scope]
    public class DealDao : AbstractDao
    {
        private readonly BundleSearch _bundleSearch;
        private readonly FilesIntegration _filesIntegration;
        private readonly FactoryIndexerDeal _factoryIndexer;
        private readonly TenantUtil _tenantUtil;
        private readonly CrmSecurity _crmSecurity;
        private readonly AuthorizationManager _authorizationManager;

        public DealDao(DbContextManager<CrmDbContext> dbContextManager,
                       TenantManager tenantManager,
                       SecurityContext securityContext,
                       CrmSecurity crmSecurity,
                       FactoryIndexerDeal factoryIndexer,
                       FilesIntegration filesIntegration,
                       TenantUtil tenantUtil,
                       AuthorizationManager authorizationManager,
                       IOptionsMonitor<ILog> logger,
                       ICache ascCache,
                       IMapper mapper,
                       BundleSearch bundleSearch) :
            base(dbContextManager,
                 tenantManager,
                 securityContext,
                 logger,
                 ascCache,
                 mapper)
        {
            _crmSecurity = crmSecurity;
            _factoryIndexer = factoryIndexer;
            _filesIntegration = filesIntegration;
            _bundleSearch = bundleSearch;
            _mapper = mapper;
            _tenantUtil = tenantUtil;
            _authorizationManager = authorizationManager;
        }


        public void AddMember(int dealID, int memberID)
        {
            SetRelative(memberID, EntityType.Opportunity, dealID);
        }

        public Dictionary<int, int[]> GetMembers(int[] dealID)
        {
            return GetRelativeToEntity(null, EntityType.Opportunity, dealID);
        }

        public int[] GetMembers(int dealID)
        {
            return GetRelativeToEntity(null, EntityType.Opportunity, dealID);
        }

        public void SetMembers(int dealID, int[] memberID)
        {
            SetRelative(memberID, EntityType.Opportunity, dealID);
        }

        public void RemoveMember(int dealID, int memberID)
        {
            RemoveRelative(memberID, EntityType.Opportunity, dealID);
        }

        public List<Deal> GetDeals(int[] id)
        {
            if (id == null || !id.Any()) return new List<Deal>();

            var dbDeals = Query(CrmDbContext.Deals).Where(x => id.Contains(x.Id)).ToList();

            return _mapper.Map<List<DbDeal>, List<Deal>>(dbDeals);
        }

        public Deal GetByID(int dealID)
        {
            var deals = GetDeals(new[] { dealID });

            return deals.Count == 0 ? null : deals[0];
        }

        public int CreateNewDeal(Deal deal)
        {
            var result = CreateNewDealInDb(deal);

            deal.ID = result;


            _factoryIndexer.Index(Query(CrmDbContext.Deals).Where(x => x.Id == deal.ID).FirstOrDefault());

            return result;
        }

        private int CreateNewDealInDb(Deal deal)
        {
            if (String.IsNullOrEmpty(deal.Title) || deal.ResponsibleID == Guid.Empty || deal.DealMilestoneID <= 0)
                throw new ArgumentException();

            // Delete relative  keys
            _cache.Remove(new Regex(TenantID.ToString(CultureInfo.InvariantCulture) + "deals.*"));

            var itemToInsert = new DbDeal
            {
                Title = deal.Title,
                Description = deal.Description,
                ResponsibleId = deal.ResponsibleID,
                ContactId = deal.ContactID,
                BidCurrency = deal.BidCurrency,
                BidValue = deal.BidValue,
                BidType = deal.BidType,
                DealMilestoneId = deal.DealMilestoneID,
                DealMilestoneProbability = deal.DealMilestoneProbability,
                ExpectedCloseDate = deal.ExpectedCloseDate,
                ActualCloseDate = deal.ActualCloseDate,
                PerPeriodValue = deal.PerPeriodValue,
                CreateOn = _tenantUtil.DateTimeToUtc(deal.CreateOn == DateTime.MinValue ? _tenantUtil.DateTimeNow() : deal.CreateOn),
                CreateBy = _securityContext.CurrentAccount.ID,
                LastModifedOn = _tenantUtil.DateTimeToUtc(deal.CreateOn == DateTime.MinValue ? _tenantUtil.DateTimeNow() : deal.CreateOn),
                LastModifedBy = _securityContext.CurrentAccount.ID,
                TenantId = TenantID
            };

            CrmDbContext.Deals.Add(itemToInsert);
            CrmDbContext.SaveChanges();

            var dealID = itemToInsert.Id;

            //    if (deal.ContactID > 0)
            //      AddMember(dealID, deal.ContactID);

            return dealID;
        }

        public int[] SaveDealList(List<Deal> items)
        {
            var tx = CrmDbContext.Database.BeginTransaction();

            var result = items.Select(item => CreateNewDealInDb(item)).ToArray();

            tx.Commit();

            var dbDeals = Query(CrmDbContext.Deals).Where(x => result.Contains(x.Id));

            foreach (var deal in dbDeals)
            {
                _factoryIndexer.Index(deal);
            }

            return result;
        }

        public void EditDeal(Deal deal)
        {
            _crmSecurity.DemandEdit(deal);

            //   var oldDeal = GetByID(deal.ID);

            //   if (oldDeal.ContactID > 0)
            //      RemoveMember(oldDeal.ID, oldDeal.ContactID);

            //    AddMember(deal.ID, deal.ContactID);

            var itemToUpdate = CrmDbContext.Deals.Find(deal.ID);

            if (itemToUpdate.TenantId != TenantID)
                throw new ArgumentException();

            itemToUpdate.Title = deal.Title;
            itemToUpdate.Description = deal.Description;
            itemToUpdate.ResponsibleId = deal.ResponsibleID;
            itemToUpdate.ContactId = deal.ContactID;
            itemToUpdate.BidCurrency = deal.BidCurrency;
            itemToUpdate.BidValue = deal.BidValue;
            itemToUpdate.BidType = deal.BidType;
            itemToUpdate.DealMilestoneId = deal.DealMilestoneID;
            itemToUpdate.DealMilestoneProbability = deal.DealMilestoneProbability;
            itemToUpdate.ExpectedCloseDate = deal.ExpectedCloseDate;
            itemToUpdate.PerPeriodValue = deal.PerPeriodValue;
            itemToUpdate.ActualCloseDate = _tenantUtil.DateTimeToUtc(deal.ActualCloseDate);
            itemToUpdate.LastModifedOn = _tenantUtil.DateTimeToUtc(_tenantUtil.DateTimeNow());
            itemToUpdate.LastModifedBy = _securityContext.CurrentAccount.ID;

            CrmDbContext.SaveChanges();

            _factoryIndexer.Index(itemToUpdate);
        }

        public int GetDealsCount()
        {
            return GetDealsCount(String.Empty, Guid.Empty, 0, null, 0, null, null, DateTime.MinValue, DateTime.MinValue);
        }

        public List<Deal> GetAllDeals()
        {
            return GetDeals(String.Empty,
                            Guid.Empty,
                            0,
                            null,
                            0,
                            null,
                            null,
                            DateTime.MinValue,
                            DateTime.MinValue,
                            0,
                            0,
                            new OrderBy(DealSortedByType.Stage, true));
        }

        private IQueryable<DbDeal> GetDbDealByFilters(
                                  ICollection<int> exceptIDs,
                                  String searchText,
                                  Guid responsibleID,
                                  int milestoneID,
                                  IEnumerable<String> tags,
                                  int contactID,
                                  DealMilestoneStatus? stageType,
                                  bool? contactAlsoIsParticipant,
                                  DateTime fromDate,
                                  DateTime toDate,
                                  OrderBy orderBy)
        {

            var sqlQuery = Query(CrmDbContext.Deals).Join(CrmDbContext.DealMilestones,
                                            x => x.DealMilestoneId,
                                            y => y.Id,
                                            (x, y) => new { x, y }
                                        );

            var ids = new List<int>();

            if (!String.IsNullOrEmpty(searchText))
            {
                searchText = searchText.Trim();

                var keywords = searchText.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries).ToArray();

                if (keywords.Length > 0)
                {
                    if (!_bundleSearch.TrySelectOpportunity(searchText, out ids))
                    {
                        foreach (var k in keywords)
                        {
                            sqlQuery = sqlQuery.Where(x => Microsoft.EntityFrameworkCore.EF.Functions.Like(x.x.Title, k + "%") || Microsoft.EntityFrameworkCore.EF.Functions.Like(x.x.Description, k + "%"));
                        }
                    }
                    else if (ids.Count == 0) return null;
                }
            }

            if (tags != null && tags.Any())
            {
                ids = SearchByTags(EntityType.Opportunity, ids.ToArray(), tags);

                if (ids.Count == 0) return null;
            }

            if (contactID > 0)
            {
                if (contactAlsoIsParticipant.HasValue && contactAlsoIsParticipant.Value)
                {
                    var relativeContactsID = GetRelativeToEntity(contactID, EntityType.Opportunity, null).ToList();

                    if (relativeContactsID.Count == 0)
                    {
                        sqlQuery = sqlQuery.Where(x => x.x.ContactId == contactID);
                    }
                    else
                    {
                        if (ids.Count > 0)
                        {
                            ids = relativeContactsID.Intersect(ids).ToList();

                            if (ids.Count == 0) return null;
                        }
                        else
                        {
                            ids = relativeContactsID;
                        }
                    }
                }
                else
                {
                    sqlQuery = sqlQuery.Where(x => x.x.ContactId == contactID);
                }
            }

            if (0 < milestoneID && milestoneID < int.MaxValue)
            {
                sqlQuery = sqlQuery.Where(x => x.x.DealMilestoneId == milestoneID);
            }

            if (responsibleID != Guid.Empty)
            {
                sqlQuery = sqlQuery.Where(x => x.x.ResponsibleId == responsibleID);
            }


            if (ids.Count > 0)
            {
                if (exceptIDs.Count > 0)
                {
                    ids = ids.Except(exceptIDs).ToList();
                    if (ids.Count == 0) return null;
                }

                sqlQuery = sqlQuery.Where(x => ids.Contains(x.x.Id));
            }
            else if (exceptIDs.Count > 0)
            {
                sqlQuery = sqlQuery.Where(x => !exceptIDs.Contains(x.x.Id));
            }

            if ((stageType != null) || (fromDate != DateTime.MinValue && toDate != DateTime.MinValue))
            {
                if (stageType != null)
                    sqlQuery = sqlQuery.Where(x => x.y.Status == stageType.Value);

                if (fromDate != DateTime.MinValue && toDate != DateTime.MinValue)
                    sqlQuery = sqlQuery.Where(x => x.y.Status == 0 ? x.x.ExpectedCloseDate >= _tenantUtil.DateTimeToUtc(fromDate) && x.x.ExpectedCloseDate <= _tenantUtil.DateTimeToUtc(toDate)
                                                                         : x.x.ActualCloseDate >= _tenantUtil.DateTimeToUtc(fromDate) && x.x.ActualCloseDate <= _tenantUtil.DateTimeToUtc(toDate));

            }


            if (orderBy != null && Enum.IsDefined(typeof(DealSortedByType), orderBy.SortedBy))
                switch ((DealSortedByType)orderBy.SortedBy)
                {
                    case DealSortedByType.Title:
                    {
                        sqlQuery = orderBy.IsAsc ? sqlQuery.OrderBy(x => x.x.Title) 
                                                 : sqlQuery.OrderByDescending(x => x.x.Title);

                        break;
                    }
                    case DealSortedByType.BidValue:
                    {
                        sqlQuery = orderBy.IsAsc ? sqlQuery.OrderBy(x => x.x.BidValue)
                                                 : sqlQuery.OrderByDescending(x => x.x.BidValue);

                        break;
                    }
                    case DealSortedByType.Responsible:
                    {
                        if (orderBy.IsAsc)
                        {
                            sqlQuery = sqlQuery.OrderBy(x => x.x.ResponsibleId)
                                                     .ThenBy(x => x.y.SortOrder)
                                                     .ThenBy(x => x.x.ContactId)
                                                     .ThenByDescending(x => x.x.ActualCloseDate)
                                                     .ThenBy(x => x.x.ExpectedCloseDate)
                                                     .ThenBy(x => x.x.Title);
                        }
                        else
                        {
                            sqlQuery = sqlQuery.OrderByDescending(x => x.x.ResponsibleId)
                                                .OrderByDescending(x => x.y.SortOrder)
                                                .ThenBy(x => x.x.ContactId)
                                                .ThenByDescending(x => x.x.ActualCloseDate)
                                                .ThenBy(x => x.x.ExpectedCloseDate)
                                                .ThenBy(x => x.x.Title);

                        }

                        break;
                    }
                    case DealSortedByType.Stage:
                    {
                        if (orderBy.IsAsc)
                        {
                            sqlQuery = sqlQuery.OrderBy(x => x.y.SortOrder)
                                               .ThenBy(x => x.x.ContactId)
                                               .ThenByDescending(x => x.x.ActualCloseDate)
                                               .ThenBy(x => x.x.ExpectedCloseDate)
                                               .ThenBy(x => x.x.Title);
                        }
                        else
                        {
                            sqlQuery = sqlQuery.OrderByDescending(x => x.y.SortOrder)
                                               .ThenBy(x => x.x.ContactId)
                                               .ThenByDescending(x => x.x.ActualCloseDate)
                                               .ThenBy(x => x.x.ExpectedCloseDate)
                                               .ThenBy(x => x.x.Title);

                        }

                        break;

                    }
                    case DealSortedByType.DateAndTime:
                    {
                        sqlQuery.OrderBy("x.x.close_date", orderBy.IsAsc);

                        break;

                    }
                    default:
                        throw new ArgumentException();
                }
            else
            {
                sqlQuery = sqlQuery.OrderBy(x => x.y.SortOrder)
                                   .ThenBy(x => x.x.ContactId)
                                   .ThenBy(x => x.x.Title);
            }

            return sqlQuery.Select(x => x.x);
        }

        public int GetDealsCount(String searchText,
                                  Guid responsibleID,
                                  int milestoneID,
                                  IEnumerable<String> tags,
                                  int contactID,
                                  DealMilestoneStatus? stageType,
                                  bool? contactAlsoIsParticipant,
                                  DateTime fromDate,
                                  DateTime toDate)
        {
            var cacheKey = TenantID.ToString(CultureInfo.InvariantCulture) +
                        "deals" +
                        _securityContext.CurrentAccount.ID.ToString() +
                        searchText +
                        responsibleID +
                        milestoneID +
                        contactID;

            if (tags != null)
                cacheKey += String.Join("", tags.ToArray());

            if (stageType.HasValue)
                cacheKey += stageType.Value;

            if (contactAlsoIsParticipant.HasValue)
                cacheKey += contactAlsoIsParticipant.Value;

            if (fromDate != DateTime.MinValue)
                cacheKey += fromDate.ToString();

            if (toDate != DateTime.MinValue)
                cacheKey += toDate.ToString();

            var fromCache = _cache.Get<string>(cacheKey);

            if (fromCache != null) return Convert.ToInt32(fromCache);

            var withParams = !(String.IsNullOrEmpty(searchText) &&
                               responsibleID == Guid.Empty &&
                               milestoneID <= 0 &&
                               (tags == null || !tags.Any()) &&
                               contactID <= 0 &&
                               stageType == null &&
                               contactAlsoIsParticipant == null &&
                               fromDate == DateTime.MinValue &&
                               toDate == DateTime.MinValue);

            ICollection<int> exceptIDs = _crmSecurity.GetPrivateItems(typeof(Deal)).ToList();

            int result;

            if (withParams)
            {
                var sqlQuery = GetDbDealByFilters(exceptIDs, searchText, responsibleID, milestoneID, tags,
                                                        contactID, stageType, contactAlsoIsParticipant,
                                                        fromDate,
                                                        toDate, null);

                if (sqlQuery == null)
                {
                    result = 0;
                }
                else
                {
                    result = sqlQuery.Count();
                }
            }
            else
            {

                var countWithoutPrivate = Query(CrmDbContext.Deals).Count();
                var privateCount = exceptIDs.Count;

                if (privateCount > countWithoutPrivate)
                {
                    _logger.Error("Private deals count more than all deals");

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

        public List<Deal> GetDeals(
                                  String searchText,
                                  Guid responsibleID,
                                  int milestoneID,
                                  IEnumerable<String> tags,
                                  int contactID,
                                  DealMilestoneStatus? stageType,
                                  bool? contactAlsoIsParticipant,
                                  DateTime fromDate,
                                  DateTime toDate,
                                  int from,
                                  int count,
                                  OrderBy orderBy)
        {

            if (_crmSecurity.IsAdmin)
                return GetCrudeDeals(searchText,
                                     responsibleID,
                                     milestoneID,
                                     tags,
                                     contactID,
                                     stageType,
                                     contactAlsoIsParticipant,
                                     fromDate,
                                     toDate,
                                     from,
                                     count,
                                     orderBy);

            var crudeDeals = GetCrudeDeals(searchText,
                                     responsibleID,
                                     milestoneID,
                                     tags,
                                     contactID,
                                     stageType,
                                     contactAlsoIsParticipant,
                                      fromDate,
                                      toDate,
                                      0,
                                      from + count,
                                      orderBy);

            if (crudeDeals.Count == 0) return crudeDeals;

            if (crudeDeals.Count < from + count) return crudeDeals.FindAll(_crmSecurity.CanAccessTo).Skip(from).ToList();

            var result = crudeDeals.FindAll(_crmSecurity.CanAccessTo);

            if (result.Count == crudeDeals.Count) return result.Skip(from).ToList();

            var localCount = count;
            var localFrom = from + count;

            while (true)
            {
                crudeDeals = GetCrudeDeals(searchText,
                                              responsibleID,
                                              milestoneID,
                                              tags,
                                              contactID,
                                              stageType,
                                              contactAlsoIsParticipant,
                                              fromDate,
                                              toDate,
                                              localFrom,
                                              localCount,
                                              orderBy);

                if (crudeDeals.Count == 0) break;

                result.AddRange(crudeDeals.Where(_crmSecurity.CanAccessTo));

                if ((result.Count >= count + from) || (crudeDeals.Count < localCount)) break;

                localFrom += localCount;
                localCount = localCount * 2;
            }

            return result.Skip(from).Take(count).ToList();
        }


        private List<Deal> GetCrudeDeals(
                                   String searchText,
                                   Guid responsibleID,
                                   int milestoneID,
                                   IEnumerable<String> tags,
                                   int contactID,
                                   DealMilestoneStatus? stageType,
                                   bool? contactAlsoIsParticipant,
                                   DateTime fromDate,
                                   DateTime toDate,
                                   int from,
                                   int count,
                                   OrderBy orderBy)
        {
            var withParams = !(String.IsNullOrEmpty(searchText) &&
                           responsibleID == Guid.Empty &&
                           milestoneID <= 0 &&
                           (tags == null || !tags.Any()) &&
                           contactID <= 0 &&
                           stageType == null &&
                           contactAlsoIsParticipant == null &&
                           fromDate == DateTime.MinValue &&
                           toDate == DateTime.MinValue);

            var sqlQuery = GetDbDealByFilters(new List<int>(),
                                                    searchText,
                                                    responsibleID,
                                                    milestoneID,
                                                    tags,
                                                    contactID,
                                                    stageType,
                                                    contactAlsoIsParticipant,
                                                    fromDate,
                                                    toDate,
                                                    orderBy);

            if (withParams && sqlQuery == null)
            {
                return new List<Deal>();
            }

            if (0 < from && from < int.MaxValue)
            {
                sqlQuery = sqlQuery.Skip(from);
            }

            if (0 < count && count < int.MaxValue)
            {
                sqlQuery = sqlQuery.Take(count);
            }

            return _mapper.Map<List<DbDeal>, List<Deal>>(sqlQuery.ToList());
        }

        public List<Deal> GetDealsByContactID(int contactID)
        {

            return GetDeals(String.Empty, Guid.Empty, 0, null, contactID, null, true, DateTime.MinValue,
                            DateTime.MinValue, 0, 0, new OrderBy(DealSortedByType.Title, true));

        }

        public List<Deal> GetDealsByPrefix(String prefix, int from, int count, int contactId = 0, bool internalSearch = true)
        {
            if (count == 0)
                throw new ArgumentException();

            prefix = prefix.Trim();

            var keywords = prefix.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries).ToArray();

            var sqlQuery = Query(CrmDbContext.Deals);

            if (keywords.Length == 1)
            {
                sqlQuery = sqlQuery.Where(x => Microsoft.EntityFrameworkCore.EF.Functions.Like(x.Title, keywords[0]));
            }
            else
            {
                foreach (var k in keywords)
                {
                    sqlQuery = sqlQuery.Where(x => Microsoft.EntityFrameworkCore.EF.Functions.Like(x.Title, k));
                }
            }

            if (0 < from && from < int.MaxValue) sqlQuery = sqlQuery.Skip(from);
            if (0 < count && count < int.MaxValue) sqlQuery = sqlQuery.Take(count);

            if (contactId > 0)
            {
                var ids = GetRelativeToEntity(contactId, EntityType.Opportunity, null);

                if (internalSearch)
                    sqlQuery = sqlQuery.Where(x => x.ContactId == contactId || ids.Contains(x.Id));
                else
                    sqlQuery = sqlQuery.Where(x => x.ContactId != contactId && !ids.Contains(x.Id));
            }

            sqlQuery = sqlQuery.OrderBy(x => x.Title);

            var dbDeals = sqlQuery.ToList();

            return _mapper.Map<List<DbDeal>, List<Deal>>(dbDeals).FindAll(_crmSecurity.CanAccessTo);
        }

        public Task<Deal> DeleteDealAsync(int id)
        {
            if (id <= 0) return null;

            var deal = GetByID(id);

            if (deal == null) return null;

            return internalDeleteDealAsync(id, deal);
        }

        private async Task<Deal> internalDeleteDealAsync(int id, Deal deal)
        {
            _crmSecurity.DemandDelete(deal);

            var dbEntity = CrmDbContext.Deals.Find(id);

            _factoryIndexer.Delete(dbEntity);

            // Delete relative  keys
            _cache.Remove(new Regex(TenantID.ToString(CultureInfo.InvariantCulture) + "deals.*"));

            await DeleteBatchDealsExecuteAsync(new List<Deal>() { deal });

            return deal;
        }

        public Task<List<Deal>> DeleteBatchDealsAsync(int[] dealID)
        {
            var deals = GetDeals(dealID).FindAll(_crmSecurity.CanDelete).ToList();

            if (deals.Count == 0) return System.Threading.Tasks.Task.FromResult(deals);

            return internalDeleteBatchDealsAsync(deals);
        }

        public Task<List<Deal>> DeleteBatchDealsAsync(List<Deal> deals)
        {
            deals = deals.FindAll(_crmSecurity.CanDelete).ToList();

            if (deals.Count == 0) return System.Threading.Tasks.Task.FromResult(deals);

            return internalDeleteBatchDealsAsync(deals);
        }

        private async Task<List<Deal>> internalDeleteBatchDealsAsync(List<Deal> deals)
        {
            // Delete relative  keys
            _cache.Remove(new Regex(TenantID.ToString(CultureInfo.InvariantCulture) + "deals.*"));

            await DeleteBatchDealsExecuteAsync(deals);

            return deals;
        }

        private System.Threading.Tasks.Task DeleteBatchDealsExecuteAsync(List<Deal> deals)
        {
            if (deals == null || deals.Count == 0) return System.Threading.Tasks.Task.CompletedTask;

            return InternalDeleteBatchDealsExecuteAsync(deals);
        }

        private async System.Threading.Tasks.Task InternalDeleteBatchDealsExecuteAsync(List<Deal> deals)
        {
            var dealID = deals.Select(x => x.ID).ToArray();


            var tagdao = _filesIntegration.DaoFactory.GetTagDao<int>();

            var tagNames = await Query(CrmDbContext.RelationshipEvent)
                                .Where(x => x.HaveFiles && dealID.Contains(x.EntityId) && x.EntityType == EntityType.Opportunity)
                                .Select(x => string.Format("RelationshipEvent_{0}", x.Id))
                                .ToArrayAsync();

            var filesIDs = tagdao.GetTagsAsync(tagNames, TagType.System).Where(t => t.EntryType == FileEntryType.File).Select(t => t.EntryId);

            var tx = await CrmDbContext.Database.BeginTransactionAsync();


            CrmDbContext.RemoveRange(Query(CrmDbContext.FieldValue)
                                          .AsNoTracking()
                                          .Where(x => dealID.Contains(x.EntityId) && x.EntityType == EntityType.Opportunity)
                                    );

            CrmDbContext.RemoveRange(CrmDbContext.EntityContact
                                                  .AsNoTracking()
                                                 .Where(x => dealID.Contains(x.EntityId) && x.EntityType == EntityType.Opportunity));

            CrmDbContext.RemoveRange(Query(CrmDbContext.RelationshipEvent)
                                        .AsNoTracking()
                                        .Where(x => dealID.Contains(x.EntityId) && x.EntityType == EntityType.Opportunity));

            CrmDbContext.RemoveRange(Query(CrmDbContext.Tasks)
                                        .AsNoTracking()
                                        .Where(x => dealID.Contains(x.EntityId) && x.EntityType == EntityType.Opportunity));

            CrmDbContext.RemoveRange(CrmDbContext.EntityTags
                                        .AsNoTracking()
                                        .Where(x => dealID.Contains(x.EntityId) && x.EntityType == EntityType.Opportunity));

            CrmDbContext.RemoveRange(Query(CrmDbContext.Deals)
                                        .AsNoTracking()
                                        .Where(x => dealID.Contains(x.Id)));

            await tx.CommitAsync();

            deals.ForEach(deal => _authorizationManager.RemoveAllAces(deal));

            var filedao = _filesIntegration.DaoFactory.GetFileDao<int>();

            await foreach (var filesID in filesIDs)
            {
                await filedao.DeleteFileAsync(Convert.ToInt32(filesID));
            }

        }

        public void ReassignDealsResponsible(Guid fromUserId, Guid toUserId)
        {
            var deals = GetDeals(String.Empty,
                            fromUserId,
                            0,
                            null,
                            0,
                            DealMilestoneStatus.Open,
                            null,
                            DateTime.MinValue,
                            DateTime.MinValue,
                            0,
                            0,
                            null);

            foreach (var deal in deals)
            {
                deal.ResponsibleID = toUserId;

                EditDeal(deal);

                var responsibles = _crmSecurity.GetAccessSubjectGuidsTo(deal);

                if (!responsibles.Any()) continue;

                responsibles.Remove(fromUserId);
                responsibles.Add(toUserId);

                _crmSecurity.SetAccessTo(deal, responsibles.Distinct().ToList());
            }
        }

        /// <summary>
        /// Test method
        /// </summary>
        /// <param name="id"></param>
        /// <param name="creationDate"></param>
        public void SetDealCreationDate(int id, DateTime creationDate)
        {
            var dbEntity = CrmDbContext.Deals.Find(id);

            if (dbEntity.TenantId != TenantID)
                throw new ArgumentException();

            dbEntity.CreateOn = _tenantUtil.DateTimeToUtc(creationDate);

            CrmDbContext.SaveChanges();

            // Delete relative keys
            _cache.Remove(new Regex(TenantID.ToString(CultureInfo.InvariantCulture) + "deals.*"));
        }

        /// <summary>
        /// Test method
        /// </summary>
        /// <param name="id"></param>
        /// <param name="lastModifedDate"></param>
        public void SetDealLastModifedDate(int id, DateTime lastModifedDate)
        {
            var dbEntity = CrmDbContext.Deals.Find(id);

            if (dbEntity.TenantId != TenantID)
                throw new ArgumentException();

            dbEntity.LastModifedOn = _tenantUtil.DateTimeToUtc(lastModifedDate);

            CrmDbContext.SaveChanges();

            // Delete relative keys
            _cache.Remove(new Regex(TenantID.ToString(CultureInfo.InvariantCulture) + "deals.*"));

        }
    }
}