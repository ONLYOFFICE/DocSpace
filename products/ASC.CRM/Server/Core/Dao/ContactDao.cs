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
using ASC.Files.Core;
using ASC.Web.CRM.Core.Search;
using ASC.Web.Files.Api;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text.RegularExpressions;
using OrderBy = ASC.CRM.Core.Entities.OrderBy;

namespace ASC.CRM.Core.Dao
{
    public class CachedContactDao : ContactDao
    {
        private readonly HttpRequestDictionary<Contact> _contactCache;

        public CachedContactDao(
            DbContextManager<CRMDbContext> dbContextManager,
            TenantManager tenantManager,
            SecurityContext securityContext,
            CRMSecurity cRMSecurity,
            IHttpContextAccessor httpContextAccessor,
            TenantUtil tenantUtil,
            AuthorizationManager authorizationManager,
            FilesIntegration filesIntegration,
            FactoryIndexer<ContactsWrapper> factoryIndexerContactsWrapper,
            FactoryIndexer<EmailWrapper> factoryIndexerEmailWrapper,
            IOptionsMonitor<ILog> logger,
            DbContextManager<CoreDbContext> coreDbContext,
            BundleSearch bundleSearch) :
                 base(dbContextManager,
                 tenantManager,
                 securityContext,
                 cRMSecurity,
                 tenantUtil,
                 authorizationManager,
                 filesIntegration,
                 factoryIndexerContactsWrapper,
                 factoryIndexerEmailWrapper,
                 logger,
                 coreDbContext,
                 bundleSearch)
        {
            _contactCache = new HttpRequestDictionary<Contact>(httpContextAccessor?.HttpContext, "crm_contact");
        }

        public override Contact GetByID(int contactID)
        {
            return _contactCache.Get(contactID.ToString(CultureInfo.InvariantCulture), () => GetByIDBase(contactID));
        }

        private Contact GetByIDBase(int contactID)
        {
            return base.GetByID(contactID);
        }

        public override Contact DeleteContact(int contactID)
        {
            ResetCache(contactID);

            return base.DeleteContact(contactID);
        }

        public override void UpdateContact(Contact contact)
        {
            if (contact != null && contact.ID > 0)
                ResetCache(contact.ID);

            base.UpdateContact(contact);
        }


        public override int SaveContact(Contact contact)
        {
            if (contact != null)
            {
                ResetCache(contact.ID);
            }

            return base.SaveContact(contact);

        }

        private void ResetCache(int contactID)
        {
            _contactCache.Reset(contactID.ToString());
        }
    }

    public class ContactDao : AbstractDao
    {
        public ContactDao(
            DbContextManager<CRMDbContext> dbContextManager,
            TenantManager tenantManager,
            SecurityContext securityContext,
            CRMSecurity cRMSecurity,
            TenantUtil tenantUtil,
            AuthorizationManager authorizationManager,
            FilesIntegration filesIntegration,
            FactoryIndexer<ContactsWrapper> factoryIndexerContactsWrapper,
            FactoryIndexer<EmailWrapper> factoryIndexerEmailWrapper,
            IOptionsMonitor<ILog> logger,
            DbContextManager<CoreDbContext> coreDbContext,
            BundleSearch bundleSearch
            ) :
                 base(dbContextManager,
                 tenantManager,
                 securityContext,
                 logger)
        {
            CRMSecurity = cRMSecurity;
            TenantUtil = tenantUtil;
            AuthorizationManager = authorizationManager;
            FilesIntegration = filesIntegration;
            FactoryIndexerContactsWrapper = factoryIndexerContactsWrapper;
            FactoryIndexerEmailWrapper = factoryIndexerEmailWrapper;
            CoreDbContext = coreDbContext.Value;
            BundleSearch = bundleSearch;
        }

        public BundleSearch BundleSearch { get; }
        public CoreDbContext CoreDbContext { get; }
        public FactoryIndexer<EmailWrapper> FactoryIndexerEmailWrapper { get; }
        public FactoryIndexer<ContactsWrapper> FactoryIndexerContactsWrapper { get; }
        public FilesIntegration FilesIntegration { get; }
        public AuthorizationManager AuthorizationManager { get; }
        public TenantUtil TenantUtil { get; }
        public CRMSecurity CRMSecurity { get; }

        private readonly String _displayNameSeparator = "!=!";

        public List<Contact> GetContactsByPrefix(String prefix, int searchType, int from, int count)
        {
            if (count == 0)
                throw new ArgumentException();

            var keywords = prefix.Trim().Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);

            var sqlQuery = Query(CRMDbContext.Contacts);

            switch (searchType)
            {
                case 0: // Company
                    sqlQuery = sqlQuery.Where(x => x.IsCompany);
                    break;
                case 1: // Persons
                    sqlQuery = sqlQuery.Where(x => !x.IsCompany);
                    break;
                case 2: // PersonsWithoutCompany
                    sqlQuery = sqlQuery.Where(x => x.CompanyId == 0 && !x.IsCompany);
                    break;
                case 3: // CompaniesAndPersonsWithoutCompany 
                    sqlQuery = sqlQuery.Where(x => x.CompanyId == 0);
                    break;
            }

            foreach (var k in keywords)
            {
                sqlQuery = sqlQuery.Where(x => Microsoft.EntityFrameworkCore.EF.Functions.Like(x.DisplayName, k));
            }

            sqlQuery = sqlQuery.OrderBy(x => x.DisplayName);

            if (!CRMSecurity.IsAdmin)
            {
                var idsFromAcl = CoreDbContext.Acl.Where(x => x.Tenant == TenantID &&
                                                     x.Action == CRMSecurity._actionRead.ID &&
                                                     x.Subject == SecurityContext.CurrentAccount.ID &&
                                                     (Microsoft.EntityFrameworkCore.EF.Functions.Like(x.Object, typeof(Company).FullName + "%") ||
                                                     Microsoft.EntityFrameworkCore.EF.Functions.Like(x.Object, typeof(Person).FullName + "%")))
                                       .Select(x => Convert.ToInt32(x.Object.Split('|', StringSplitOptions.None)[1]))
                                       .ToList();

                // oldContacts || publicContact || contact is private, but user is ManagerContact
                sqlQuery = sqlQuery.Where(x => x.IsShared == null || x.IsShared > 0 || idsFromAcl.Contains(x.Id));
            }


            if (0 < from && from < int.MaxValue) sqlQuery = sqlQuery.Skip(from);
            if (0 < count && count < int.MaxValue) sqlQuery = sqlQuery.Take(count);

            return sqlQuery.ToList().ConvertAll(ToContact);
        }

        public int GetAllContactsCount()
        {
            return GetContactsCount(String.Empty, null, -1, -1, ContactListViewType.All, DateTime.MinValue, DateTime.MinValue);
        }

        public List<Contact> GetAllContacts()
        {
            return GetContacts(String.Empty, new List<string>(), -1, -1, ContactListViewType.All, DateTime.MinValue, DateTime.MinValue, 0, 0,
                new OrderBy(ContactSortedByType.DisplayName, true));
        }

        public int GetContactsCount(String searchText,
                                   IEnumerable<String> tags,
                                   int contactStage,
                                   int contactType,
                                   ContactListViewType contactListView,
                                   DateTime fromDate,
                                   DateTime toDate,
                                   Guid? responsibleid = null,
                                   bool? isShared = null)
        {

            var cacheKey = TenantID.ToString(CultureInfo.InvariantCulture) +
                           "contacts" +
                           SecurityContext.CurrentAccount.ID +
                           searchText +
                           contactStage +
                           contactType +
                           (int)contactListView +
                           responsibleid +
                           isShared;

            if (tags != null)
                cacheKey += String.Join("", tags.ToArray());

            if (fromDate != DateTime.MinValue)
                cacheKey += fromDate.ToString();

            if (toDate != DateTime.MinValue)
                cacheKey += toDate.ToString();

            var fromCache = _cache.Get<string>(cacheKey);

            if (fromCache != null) return Convert.ToInt32(fromCache);

            var withParams = HasSearchParams(searchText,
                                            tags,
                                            contactStage,
                                            contactType,
                                            contactListView,
                                            fromDate,
                                            toDate,
                                            responsibleid,
                                            isShared);
            int result;

            if (withParams)
            {
                ICollection<int> excludedContactIDs;

                var sharedTypes = new[] { ShareType.Read, ShareType.ReadWrite }.ToList();

                switch (contactListView)
                {
                    case ContactListViewType.Person:
                        {

                            excludedContactIDs = CRMSecurity.GetPrivateItems(typeof(Person))
                                                .Except(Query(CRMDbContext.Contacts).Where(x => x.IsShared.HasValue ?
                                                                    sharedTypes.Contains(x.IsShared.Value) :
                                                                    true && !x.IsCompany).Select(x => x.Id))
                                                .ToList();

                            break;

                        }
                    case ContactListViewType.Company:
                        {
                            excludedContactIDs = CRMSecurity.GetPrivateItems(typeof(Person))
                                                .Except(Query(CRMDbContext.Contacts).Where(x => x.IsShared.HasValue ?
                                                                    sharedTypes.Contains(x.IsShared.Value) :
                                                                    true && x.IsCompany).Select(x => x.Id))
                                                .ToList();

                            break;
                        }
                    default:
                        {
                            excludedContactIDs = CRMSecurity.GetPrivateItems(typeof(Company))
                                                .Union(CRMSecurity.GetPrivateItems(typeof(Person)))
                                                .Except(Query(CRMDbContext.Contacts).Where(x => x.IsShared.HasValue ?
                                                                    sharedTypes.Contains(x.IsShared.Value) :
                                                                    true && x.IsCompany).Select(x => x.Id))
                                                .ToList();

                            break;
                        }
                }

                var dbContactsByFilters = GetDbContactsByFilters(excludedContactIDs,
                                                        searchText,
                                                        tags,
                                                        contactStage,
                                                        contactType,
                                                        contactListView,
                                                        fromDate,
                                                        toDate,
                                                        responsibleid,
                                                        isShared);

                if (dbContactsByFilters != null)
                {
                    if (!isShared.HasValue)
                    {
                        result = dbContactsByFilters.Count();
                    }
                    else
                    {
                        var sqlResultRows = dbContactsByFilters.Select(x => new { x.Id, x.IsCompany, x.IsShared }).ToList();

                        var resultContactsNewScheme_Count = sqlResultRows.Where(x => x.IsShared != null).ToList().Count; //new scheme

                        var fakeContactsOldScheme = sqlResultRows
                            .Where(x => x.IsShared == null).ToList() // old scheme
                            .ConvertAll(x => x.IsCompany ? new Company() { ID = x.Id } as Contact : new Person() { ID = x.Id } as Contact);

                        var resultFakeContactsOldScheme_Count = fakeContactsOldScheme.Where(fc =>
                        {
                            var accessSubjectToContact = CRMSecurity.GetAccessSubjectTo(fc);
                            if (isShared.Value == true)
                            {
                                return !accessSubjectToContact.Any();
                            }
                            else
                            {
                                return accessSubjectToContact.Any();
                            }

                        }).ToList().Count;

                        return resultContactsNewScheme_Count + resultFakeContactsOldScheme_Count;
                    }
                }
                else
                {
                    result = 0;
                }
            }
            else
            {
                var countWithoutPrivate = Query(CRMDbContext.Contacts).Count();

                var sharedTypes = new[] { ShareType.Read, ShareType.ReadWrite }.ToList();

                var privateCount = CRMSecurity.GetPrivateItemsCount(typeof(Person)) +
                                    CRMSecurity.GetPrivateItemsCount(typeof(Company)) -
                                    Query(CRMDbContext.Contacts).Where(x => x.IsShared.HasValue ?
                                                                    sharedTypes.Contains(x.IsShared.Value) :
                                                                    true).Count();
                if (privateCount < 0)
                    privateCount = 0;

                if (privateCount > countWithoutPrivate)
                {
                    Logger.Error("Private contacts count more than all contacts");

                    privateCount = 0;
                }

                result = countWithoutPrivate - privateCount;
            }
            if (result > 0)
            {
                _cache.Insert(cacheKey, result, TimeSpan.FromMinutes(1));
            }

            return result;
        }

        public List<Contact> SearchContactsByEmail(string searchText, int maxCount)
        {
            var contacts = new List<Contact>();

            if (string.IsNullOrEmpty(searchText) || maxCount <= 0)
                return contacts;

            var ids = new List<int>();

            List<int> contactsIds;

            if (FactoryIndexerEmailWrapper.TrySelectIds(s => s.MatchAll(searchText), out contactsIds))
            {
                if (!contactsIds.Any())
                    return contacts;

                ids = contactsIds.Select(r => Convert.ToInt32(r)).ToList();
            }

            var isAdmin = CRMSecurity.IsAdmin;

            const int count_per_query = 100;

            var f = 0;

            do
            {
                var query = Query(CRMDbContext.Contacts).Join(CRMDbContext.ContactsInfo,
                                                                     x => new { Column1 = x.TenantId, Column2 = x.Id },
                                                                     y => new { Column1 = y.TenantId, Column2 = y.ContactId },
                                                                (x, y) => new { x, y })
                                                        .Where(x => x.y.Type == ContactInfoType.Email);

                if (ids.Any())
                {
                    var partIds = ids.Skip(f).Take(count_per_query).ToList();

                    if (!partIds.Any())

                        break;

                    query = query.Where(x => partIds.Contains(x.x.Id));

                }
                else
                {

                    query = query.Where(x => Microsoft.EntityFrameworkCore.EF.Functions.Like(String.Concat(x.x.DisplayName, " ", x.y.Data), "%" + searchText + "%"));

                    query = query.Skip(f).Take(count_per_query);
                }

                var partContacts = query.Select(x => x.x).ToList().ConvertAll(ToContact);

                foreach (var partContact in partContacts)
                {
                    if (maxCount - contacts.Count == 0)
                        return contacts;

                    if (isAdmin || CRMSecurity.CanAccessTo(partContact))
                        contacts.Add(partContact);
                }

                if (maxCount - contacts.Count == 0 || !partContacts.Any() || partContacts.Count < count_per_query)
                    break;

                f += count_per_query;

            } while (true);

            return contacts;
        }

        public List<Contact> GetContacts(String searchText,
                                        IEnumerable<string> tags,
                                        int contactStage,
                                        int contactType,
                                        ContactListViewType contactListView,
                                        DateTime fromDate,
                                        DateTime toDate,
                                        int from,
                                        int count,
                                        OrderBy orderBy,
                                        Guid? responsibleId = null,
                                        bool? isShared = null)
        {
            if (CRMSecurity.IsAdmin)
            {
                if (!isShared.HasValue)
                {
                    return GetCrudeContacts(
                                            searchText,
                                            tags,
                                            contactStage,
                                            contactType,
                                            contactListView,
                                            fromDate,
                                            toDate,
                                            from,
                                            count,
                                            orderBy,
                                            responsibleId,
                                            isShared,
                                            false);
                }
                else
                {
                    var crudeContacts = GetCrudeContacts(
                                            searchText,
                                            tags,
                                            contactStage,
                                            contactType,
                                            contactListView,
                                            fromDate,
                                            toDate,
                                            0,
                                            from + count,
                                            orderBy,
                                            responsibleId,
                                            isShared,
                                            false);

                    if (crudeContacts.Count == 0) return crudeContacts;

                    var result = crudeContacts.Where(c => (isShared.Value == true ? c.ShareType != ShareType.None : c.ShareType == ShareType.None)).ToList();

                    if (result.Count == crudeContacts.Count) return result.Skip(from).ToList();

                    var localCount = count;
                    var localFrom = from + count;

                    while (true)
                    {
                        crudeContacts = GetCrudeContacts(
                                                searchText,
                                                tags,
                                                contactStage,
                                                contactType,
                                                contactListView,
                                                fromDate,
                                                toDate,
                                                localFrom,
                                                localCount,
                                                orderBy,
                                                responsibleId,
                                                isShared,
                                                false);

                        if (crudeContacts.Count == 0) break;

                        result.AddRange(crudeContacts.Where(c => (isShared.Value == true ? c.ShareType != ShareType.None : c.ShareType == ShareType.None)));

                        if ((result.Count >= count + from) || (crudeContacts.Count < localCount)) break;

                        localFrom += localCount;
                        localCount = localCount * 2;
                    }

                    return result.Skip(from).Take(count).ToList();
                }
            }
            else
            {
                var crudeContacts = GetCrudeContacts(
                                            searchText,
                                            tags,
                                            contactStage,
                                            contactType,
                                            contactListView,
                                            fromDate,
                                            toDate,
                                            0,
                                            from + count,
                                            orderBy,
                                            responsibleId,
                                            isShared,
                                            false);

                if (crudeContacts.Count == 0) return crudeContacts;

                var tmp = isShared.HasValue ? crudeContacts.Where(c => (isShared.Value == true ? c.ShareType != ShareType.None : c.ShareType == ShareType.None)).ToList() : crudeContacts;

                if (crudeContacts.Count < from + count)
                {
                    return tmp.FindAll(CRMSecurity.CanAccessTo).Skip(from).ToList();
                }

                var result = tmp.FindAll(CRMSecurity.CanAccessTo);

                if (result.Count == crudeContacts.Count) return result.Skip(from).ToList();

                var localCount = count;
                var localFrom = from + count;

                while (true)
                {
                    crudeContacts = GetCrudeContacts(
                                            searchText,
                                            tags,
                                            contactStage,
                                            contactType,
                                            contactListView,
                                            fromDate,
                                            toDate,
                                            localFrom,
                                            localCount,
                                            orderBy,
                                            responsibleId,
                                            isShared,
                                            false);

                    if (crudeContacts.Count == 0) break;

                    tmp = isShared.HasValue ? crudeContacts.Where(c => (isShared.Value == true ? c.ShareType != ShareType.None : c.ShareType == ShareType.None)).ToList() : crudeContacts;

                    result.AddRange(tmp.Where(CRMSecurity.CanAccessTo));

                    if ((result.Count >= count + from) || (crudeContacts.Count < localCount)) break;

                    localFrom += localCount;
                    localCount = localCount * 2;
                }

                return result.Skip(from).Take(count).ToList();
            }
        }

        private bool HasSearchParams(String searchText,
                                    IEnumerable<String> tags,
                                    int contactStage,
                                    int contactType,
                                    ContactListViewType contactListView,
                                    DateTime fromDate,
                                    DateTime toDate,
                                    Guid? responsibleid = null,
                                    bool? isShared = null)
        {
            var hasNoParams = String.IsNullOrEmpty(searchText) &&
                                    (tags == null || !tags.Any()) &&
                                    contactStage < 0 &&
                                    contactType < 0 &&
                                    contactListView == ContactListViewType.All &&
                                    !isShared.HasValue &&
                                    fromDate == DateTime.MinValue &&
                                    toDate == DateTime.MinValue &&
                                    !responsibleid.HasValue;

            return !hasNoParams;
        }

        private IQueryable<DbContact> GetDbContactsByFilters(
            ICollection<int> exceptIDs,
            String searchText,
            IEnumerable<String> tags,
            int contactStage,
            int contactType,
            ContactListViewType contactListView,
            DateTime fromDate,
            DateTime toDate,
            Guid? responsibleid = null,
            bool? isShared = null)
        {

            var sqlQuery = Query(CRMDbContext.Contacts);

            var ids = new List<int>();

            if (responsibleid.HasValue)
            {

                if (responsibleid != default(Guid))
                {
                    ids = CRMSecurity.GetContactsIdByManager(responsibleid.Value).ToList();
                    if (ids.Count == 0) return null;
                }
                else
                {
                    if (exceptIDs == null)
                        exceptIDs = new List<int>();

                    exceptIDs = exceptIDs.Union(CRMSecurity.GetContactsIdByManager(Guid.Empty)).ToList();

                    if (!exceptIDs.Any()) // HACK
                        exceptIDs.Add(0);
                }

            }

            if (!String.IsNullOrEmpty(searchText))
            {
                searchText = searchText.Trim();

                var keywords = searchText.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries)
                   .ToArray();

                List<int> contactsIds;
                if (!BundleSearch.TrySelectContact(searchText, out contactsIds))
                {
                    Logger.Debug("FullTextSearch.SupportModule('CRM.Contacts') = false");

                    foreach (var k in keywords)
                    {
                        sqlQuery = sqlQuery.Where(x => Microsoft.EntityFrameworkCore.EF.Functions.Like(x.DisplayName, k + "%"));
                    }
                }
                else
                {
                    Logger.Debug("FullTextSearch.SupportModule('CRM.Contacts') = true");
                    Logger.DebugFormat("FullTextSearch.Search: searchText = {0}", searchText);

                    var full_text_ids = contactsIds;

                    if (full_text_ids.Count == 0) return null;

                    if (ids.Count != 0)
                    {
                        ids = ids.Where(full_text_ids.Contains).ToList();
                        if (ids.Count == 0) return null;
                    }
                    else
                    {
                        ids = full_text_ids;
                    }

                    if (contactListView == ContactListViewType.All || contactListView == ContactListViewType.Person)
                    {
                        ids.AddRange(GetContactIDsByCompanyIds(ids));
                    }
                }
            }

            if (tags != null && tags.Any())
            {
                ids = SearchByTags(EntityType.Contact, ids.ToArray(), tags);

                if (ids.Count == 0) return null;
            }


            switch (contactListView)
            {
                case ContactListViewType.Company:
                    {
                        sqlQuery = sqlQuery.Where(x => x.IsCompany);

                        break;
                    }
                case ContactListViewType.Person:
                    {
                        sqlQuery = sqlQuery.Where(x => !x.IsCompany);

                        break;
                    }
                case ContactListViewType.WithOpportunity:
                    if (ids.Count > 0)
                    {
                        ids = CRMDbContext.EntityContact.Where(x => ids.Contains(x.ContactId) && x.EntityType == EntityType.Opportunity)
                                                        .Select(x => x.ContactId)
                                                        .Distinct()
                                                        .ToList();
                    }
                    else
                    {
                        ids = CRMDbContext.EntityContact.Where(x => x.EntityType == EntityType.Opportunity)
                                                        .Select(x => x.ContactId)
                                                        .Distinct()
                                                        .ToList();
                    }

                    if (ids.Count == 0) return null;

                    break;
            }

            if (contactStage >= 0)
                sqlQuery = sqlQuery.Where(x => x.StatusId == contactStage);

            if (contactType >= 0)
                sqlQuery = sqlQuery.Where(x => x.ContactTypeId == contactType);

            if (fromDate != DateTime.MinValue && toDate != DateTime.MinValue)
            {
                sqlQuery = sqlQuery.Where(x => x.CreateOn >= TenantUtil.DateTimeToUtc(fromDate) && x.CreateOn <= TenantUtil.DateTimeToUtc(toDate.AddDays(1).AddMinutes(-1)));
            }
            else if (fromDate != DateTime.MinValue)
            {
                sqlQuery = sqlQuery.Where(x => x.CreateOn >= TenantUtil.DateTimeToUtc(fromDate));
            }
            else if (toDate != DateTime.MinValue)
            {
                sqlQuery = sqlQuery.Where(x => x.CreateOn <= TenantUtil.DateTimeToUtc(toDate.AddDays(1).AddMinutes(-1)));
            }

            if (isShared.HasValue)
            {
                var sharedTypes = new[] { ShareType.Read, ShareType.ReadWrite }.ToList();

                if (isShared.Value)
                {
                    sqlQuery = sqlQuery.Where(x => x.IsShared.HasValue ? sharedTypes.Contains(x.IsShared.Value) : x.IsShared == null);
                }
                else
                {
                    sqlQuery = sqlQuery.Where(x => x.IsShared.HasValue ? x.IsShared == ShareType.None : x.IsShared == null);
                }
            }

            if (ids.Count > 0)
            {
                if (exceptIDs.Count > 0)
                {
                    ids = ids.Except(exceptIDs).ToList();
                    if (ids.Count == 0) return null;
                }

                sqlQuery = sqlQuery.Where(x => ids.Contains(x.Id));
            }
            else if (exceptIDs.Count > 0)
            {
                var _MaxCountForSQLNotIn = 500;

                if (exceptIDs.Count > _MaxCountForSQLNotIn)
                {
                    ids = Query(CRMDbContext.Contacts).Select(x => x.Id).ToList();

                    ids = ids.Except(exceptIDs).ToList();
                    if (ids.Count == 0) return null;

                    sqlQuery = sqlQuery.Where(x => ids.Contains(x.Id));

                }
                else
                {
                    sqlQuery = sqlQuery.Where(x => !exceptIDs.Contains(x.Id));
                }
            }

            return sqlQuery;
        }

        private List<Contact> GetCrudeContacts(
            String searchText,
            IEnumerable<string> tags,
            int contactStage,
            int contactType,
            ContactListViewType contactListView,
            DateTime fromDate,
            DateTime toDate,
            int from,
            int count,
            OrderBy orderBy,
            Guid? responsibleid = null,
            bool? isShared = null,
            bool selectIsSharedInNewScheme = true)
        {
            var withParams = HasSearchParams(searchText,
                                            tags,
                                            contactStage,
                                            contactType,
                                            contactListView,
                                            fromDate,
                                            toDate,
                                            responsibleid,
                                            isShared);

            var dbContactsQuery = GetDbContactsByFilters(new List<int>(),
                                                    searchText,
                                                    tags,
                                                    contactStage,
                                                    contactType,
                                                    contactListView,
                                                    fromDate,
                                                    toDate,
                                                    responsibleid,
                                                    isShared);

            if (withParams && dbContactsQuery == null) return new List<Contact>();

            if (0 < from && from < int.MaxValue) dbContactsQuery = dbContactsQuery.Skip(from);
            if (0 < count && count < int.MaxValue) dbContactsQuery = dbContactsQuery.Take(count);

            if (orderBy != null)
            {

                if (!Enum.IsDefined(typeof(ContactSortedByType), orderBy.SortedBy.ToString()))
                {
                    orderBy.SortedBy = ContactSortedByType.Created;
                }

                switch ((ContactSortedByType)orderBy.SortedBy)
                {
                    case ContactSortedByType.DisplayName:
                        dbContactsQuery = dbContactsQuery.OrderBy("DisplayName", orderBy.IsAsc);
                        break;
                    case ContactSortedByType.Created:
                        dbContactsQuery = dbContactsQuery.OrderBy("CreateOn", orderBy.IsAsc);
                        break;
                    case ContactSortedByType.ContactType:

                        dbContactsQuery = dbContactsQuery.OrderBy("StatusId", orderBy.IsAsc);

                        break;
                    case ContactSortedByType.FirstName:
                        {
                            dbContactsQuery = dbContactsQuery.OrderBy("FirstName", orderBy.IsAsc);
                            dbContactsQuery = dbContactsQuery.OrderBy("LastName", orderBy.IsAsc);
                        }

                        break;
                    case ContactSortedByType.LastName:
                        {
                            dbContactsQuery = dbContactsQuery.OrderBy("LastName", orderBy.IsAsc);
                            dbContactsQuery = dbContactsQuery.OrderBy("FirstName", orderBy.IsAsc);
                        }
                        break;
                    case ContactSortedByType.History:
                        {
                            dbContactsQuery = dbContactsQuery.GroupJoin(Query(CRMDbContext.RelationshipEvent),
                                                                          x => x.Id,
                                                                          y => y.ContactId,
                                                                          (x, y) => new { x, y })
                                                              .OrderBy(x => x.y.Max(x => x.LastModifedOn))
                                                              .ThenBy(x => x.x.CreateOn)
                                                              .Select(x => x.x);


                        }

                        break;
                    default:
                        {
                            dbContactsQuery = dbContactsQuery.OrderBy("DisplayName", orderBy.IsAsc);
                        }

                        break;
                }
            }


            var contacts = dbContactsQuery.ToList().ConvertAll(ToContact);

            return selectIsSharedInNewScheme && isShared.HasValue ?
                contacts.Where(c => (isShared.Value ? c.ShareType != ShareType.None : c.ShareType == ShareType.None)).ToList() :
                contacts;
        }

        public List<Contact> GetContactsByName(String title, bool isCompany)
        {
            if (String.IsNullOrEmpty(title)) return new List<Contact>();

            title = title.Trim();

            if (isCompany)
            {
                return CRMDbContext.Contacts.Where(x => String.Compare(x.DisplayName, title) == 0 && x.IsCompany)
                                    .ToList()
                                    .ConvertAll(ToContact)
                                    .FindAll(CRMSecurity.CanAccessTo);
            }
            else
            {
                var titleParts = title.Split(new[] { " " }, StringSplitOptions.RemoveEmptyEntries);

                if (titleParts.Length == 1 || titleParts.Length == 2)
                {
                    if (titleParts.Length == 1)
                    {
                        return Query(CRMDbContext.Contacts).Where(x => String.Compare(x.DisplayName, title) == 0 && !x.IsCompany)
                               .ToList()
                               .ConvertAll(ToContact)
                               .FindAll(CRMSecurity.CanAccessTo);
                    }
                    else
                    {
                        return Query(CRMDbContext.Contacts).Where(x => String.Compare(x.DisplayName, String.Concat(titleParts[0], _displayNameSeparator, titleParts[1])) == 0 && !x.IsCompany)
                            .ToList()
                            .ConvertAll(ToContact)
                            .FindAll(CRMSecurity.CanAccessTo);
                    }
                }
            }

            return GetContacts(title, null, -1, -1, isCompany ? ContactListViewType.Company : ContactListViewType.Person, DateTime.MinValue, DateTime.MinValue, 0, 0, null);
        }

        public void RemoveMember(int[] peopleID)
        {
            if ((peopleID == null) || (peopleID.Length == 0)) return;

            foreach (var id in peopleID)
            {
                var itemToUpdate = new DbContact
                {
                    Id = id,
                    CompanyId = 0,
                    TenantId = TenantID
                };

                CRMDbContext.Attach(itemToUpdate);
                CRMDbContext.Entry(itemToUpdate).Property(x => x.CompanyId).IsModified = true;
            }

            CRMDbContext.SaveChanges();

            RemoveRelativeInDb(null, EntityType.Person, peopleID);

        }

        public void RemoveMember(int peopleID)
        {
            var itemToUpdate = new DbContact
            {
                Id = peopleID,
                CompanyId = 0,
                TenantId = TenantID
            };

            CRMDbContext.Attach(itemToUpdate);
            CRMDbContext.Entry(itemToUpdate).Property(x => x.CompanyId).IsModified = true;

            CRMDbContext.SaveChanges();

            RemoveRelative(0, EntityType.Person, peopleID);
        }

        public void AddMemberInDb(int peopleID, int companyID)
        {

            var itemToUpdate = new DbContact
            {
                Id = peopleID,
                CompanyId = companyID,
                TenantId = TenantID
            };

            CRMDbContext.Attach(itemToUpdate);
            CRMDbContext.Entry(itemToUpdate).Property(x => x.CompanyId).IsModified = true;

            CRMDbContext.SaveChanges();

            SetRelative(companyID, EntityType.Person, peopleID);
        }

        public void AddMember(int peopleID, int companyID)
        {
            AddMemberInDb(peopleID, companyID);
        }

        public void SetMembers(int companyID, params int[] peopleIDs)
        {
            if (companyID == 0)
                throw new ArgumentException();

            var tx = CRMDbContext.Database.BeginTransaction();

            CRMDbContext.EntityContact
                    .RemoveRange(CRMDbContext.EntityContact
                    .Where(x => x.EntityType == EntityType.Person && x.ContactId == companyID));

            var itemsToUpdate = Query(CRMDbContext.Contacts).Where(x => x.CompanyId == companyID).ToList();

            itemsToUpdate.ForEach(x => x.CompanyId = 0);

            CRMDbContext.UpdateRange(itemsToUpdate);

            CRMDbContext.SaveChanges();

            if (!(peopleIDs == null || peopleIDs.Length == 0))
            {
                foreach (var id in peopleIDs)
                {
                    var contactItemToUpdate = new DbContact
                    {
                        Id = id,
                        CompanyId = companyID,
                        TenantId = TenantID
                    };

                    CRMDbContext.Attach(contactItemToUpdate);
                    CRMDbContext.Entry(contactItemToUpdate).Property(x => x.CompanyId).IsModified = true;
                    CRMDbContext.Update(contactItemToUpdate);
                }

                CRMDbContext.SaveChanges();

                foreach (var peopleID in peopleIDs)
                {
                    SetRelative(companyID, EntityType.Person, peopleID);
                }
            }

            tx.Commit();

        }

        public void SetRelativeContactProject(IEnumerable<int> contactid, int projectid)
        {
            var tx = CRMDbContext.Database.BeginTransaction();

            foreach (var id in contactid)
            {
                var itemToInsert = new DbProjects
                {
                    ContactId = id,
                    ProjectId = projectid,
                    TenantId = TenantID
                };

                CRMDbContext.Projects.Add(itemToInsert);
            }

            tx.Commit();

        }

        public void RemoveRelativeContactProject(int contactid, int projectid)
        {
            CRMDbContext.RemoveRange(Query(CRMDbContext.Projects)
                                        .Where(x => x.ContactId == contactid && x.ProjectId == projectid));
        }


        public IEnumerable<Contact> GetContactsByProjectID(int projectid)
        {
            var contactIds = Query(CRMDbContext.Projects)
                            .Where(x => x.ProjectId == projectid)
                            .Select(x => x.ContactId);

            if (!contactIds.Any()) return new List<Contact>();

            return GetContacts(contactIds.ToArray());
        }

        public List<Contact> GetMembers(int companyID)
        {
            return GetContacts(GetRelativeToEntity(companyID, EntityType.Person, null));
        }

        public List<Contact> GetRestrictedMembers(int companyID)
        {
            return GetRestrictedContacts(GetRelativeToEntity(companyID, EntityType.Person, null));
        }

        public Dictionary<int, int> GetMembersCount(int[] companyID)
        {
            return CRMDbContext.EntityContact
                .Where(x => companyID.Contains(x.ContactId) && x.EntityType == EntityType.Person)
                .GroupBy(x => x.ContactId)
                .Select(x => new { GroupId = x.Key, Count = x.Count() })
                .ToDictionary(x => x.GroupId, x => x.Count);
        }


        public int GetMembersCount(int companyID)
        {
            return CRMDbContext.EntityContact
                        .Where(x => x.ContactId == companyID && x.EntityType == EntityType.Person)
                        .Count();
        }

        public List<int> GetMembersIDs(int companyID)
        {
            return CRMDbContext.EntityContact
                    .Where(x => x.ContactId == companyID && x.EntityType == EntityType.Person)
                    .Select(x => x.EntityId)
                    .ToList();
        }

        public Dictionary<int, ShareType?> GetMembersIDsAndShareType(int companyID)
        {
            return CRMDbContext.EntityContact
                                .Where(x => x.ContactId == companyID && x.EntityType == EntityType.Person)
                                .GroupJoin(CRMDbContext.Contacts,
                                            x => x.EntityId,
                                            y => y.Id,
                                            (x, y) => new { x, y })
                                .SelectMany(x => x.y.DefaultIfEmpty(), (x, y) => new
                                {
                                    EntityId = x.x.EntityId,
                                    IsShared = y.IsShared
                                }).ToDictionary(x => x.EntityId, y => y.IsShared);
        }


        public virtual void UpdateContact(Contact contact)
        {
            UpdateContactFromDb(contact);
        }

        private void UpdateContactFromDb(Contact contact)
        {
            var originalContact = GetByID(contact.ID);
            if (originalContact == null) throw new ArgumentException();
            CRMSecurity.DemandEdit(originalContact);

            String firstName;
            String lastName;
            String companyName;
            String title;
            int companyID;

            var displayName = String.Empty;

            if (contact is Company)
            {
                firstName = String.Empty;
                lastName = String.Empty;
                title = String.Empty;
                companyName = ((Company)contact).CompanyName.Trim();
                companyID = 0;
                displayName = companyName;

                if (String.IsNullOrEmpty(companyName))
                    throw new ArgumentException();

            }
            else
            {
                var people = (Person)contact;

                firstName = people.FirstName.Trim();
                lastName = people.LastName.Trim();
                title = people.JobTitle;
                companyName = String.Empty;
                companyID = people.CompanyID;

                displayName = String.Concat(firstName, _displayNameSeparator, lastName);

                RemoveMember(people.ID);

                if (companyID > 0)
                {
                    AddMemberInDb(people.ID, companyID);
                }

                if (String.IsNullOrEmpty(firstName))// || String.IsNullOrEmpty(lastName)) lastname is not required field now
                    throw new ArgumentException();

            }

            if (!String.IsNullOrEmpty(title))
                title = title.Trim();

            if (!String.IsNullOrEmpty(contact.About))
                contact.About = contact.About.Trim();

            if (!String.IsNullOrEmpty(contact.Industry))
                contact.Industry = contact.Industry.Trim();


            var itemToUpdate = Query(CRMDbContext.Contacts).Single(x => x.Id == contact.ID);

            itemToUpdate.FirstName = firstName;
            itemToUpdate.LastName = lastName;
            itemToUpdate.CompanyName = companyName;
            itemToUpdate.Title = title;
            itemToUpdate.Notes = contact.About;
            itemToUpdate.Industry = contact.Industry;
            itemToUpdate.StatusId = contact.StatusID;
            itemToUpdate.CompanyId = companyID;
            itemToUpdate.LastModifedOn = TenantUtil.DateTimeToUtc(TenantUtil.DateTimeNow());
            itemToUpdate.LastModifedBy = SecurityContext.CurrentAccount.ID;
            itemToUpdate.DisplayName = displayName;
            itemToUpdate.IsShared = contact.ShareType;
            itemToUpdate.ContactTypeId = contact.ContactTypeID;
            itemToUpdate.Currency = contact.Currency;
            itemToUpdate.TenantId = TenantID;

            CRMDbContext.Update(itemToUpdate);
            CRMDbContext.SaveChanges();


            // Delete relative  keys
            _cache.Remove(new Regex(TenantID.ToString(CultureInfo.InvariantCulture) + "contacts.*"));

            FactoryIndexerContactsWrapper.UpdateAsync(contact);

        }

        public void UpdateContactStatus(IEnumerable<int> contactid, int statusid)
        {
            foreach (var item in contactid)
            {
                var itemToUpdate = new DbContact
                {
                    Id = item,
                    StatusId = statusid,
                    TenantId = TenantID
                };

                CRMDbContext.Attach(itemToUpdate);
                CRMDbContext.Entry(itemToUpdate).Property(x => x.StatusId).IsModified = true;

            }

            CRMDbContext.SaveChanges();

            // Delete relative  keys
            _cache.Remove(new Regex(TenantID.ToString(CultureInfo.InvariantCulture) + "contacts.*"));
        }

        public List<int> FindDuplicateByEmail(List<ContactInfo> items, bool resultReal)
        {
            //resultReal - true => real, false => fake
            if (items.Count == 0) return new List<int>();

            var result = new List<int>();
            var emails = items.ConvertAll(i => i.Data).ToList();
            var sqlQuery = Query(CRMDbContext.ContactsInfo)
                                .Where(x => emails.Contains(x.Data) && x.Type == ContactInfoType.Email);

            if (resultReal)
            {
                result = sqlQuery.Select(x => x.ContactId).ToList();
            }
            else
            {
                List<string> emailsAlreadyExists;

                emailsAlreadyExists = sqlQuery.Select(x => x.Data).ToList();

                if (emailsAlreadyExists.Count != 0)
                {
                    result = items.Where(i => emailsAlreadyExists.Contains(i.Data)).Select(i => i.ContactID).ToList();
                }
            }

            return result;
        }

        public virtual Dictionary<int, int> SaveContactList(List<Contact> items)
        {
            var tx = CRMDbContext.Database.BeginTransaction();

            var result = new Dictionary<int, int>();

            for (int index = 0; index < items.Count; index++)
            {
                var item = items[index];

                if (item.ID == 0)
                    item.ID = index;

                result.Add(item.ID, SaveContactToDb(item));
            }

            tx.Commit();

            // Delete relative  keys
            _cache.Remove(new Regex(TenantID.ToString(CultureInfo.InvariantCulture) + "contacts.*"));

            return result;
        }

        public virtual void UpdateContactList(List<Contact> items)
        {
            var tx = CRMDbContext.Database.BeginTransaction();

            for (int index = 0; index < items.Count; index++)
            {
                var item = items[index];

                if (item.ID == 0)
                    item.ID = index;

                UpdateContactFromDb(item);
            }

            tx.Commit();

            // Delete relative  keys
            _cache.Remove(new Regex(TenantID.ToString(CultureInfo.InvariantCulture) + "contacts.*"));
        }

        public void MakePublic(int contactId, bool isShared)
        {
            if (contactId <= 0) throw new ArgumentException();

            var itemToUpdate = new DbContact
            {
                Id = contactId,
                IsShared = isShared ? ShareType.ReadWrite : ShareType.None,
                TenantId = TenantID
            };

            CRMDbContext.Attach(itemToUpdate);
            CRMDbContext.Entry(itemToUpdate).Property(x => x.IsShared).IsModified = true;
            CRMDbContext.SaveChanges();

        }

        public virtual int SaveContact(Contact contact)
        {
            var result = SaveContactToDb(contact);
            FactoryIndexerContactsWrapper.IndexAsync(contact);

            // Delete relative  keys
            _cache.Remove(new Regex(TenantID.ToString(CultureInfo.InvariantCulture) + "contacts.*"));

            return result;

        }

        private int SaveContactToDb(Contact contact)
        {
            String firstName;
            String lastName;
            bool isCompany;
            String companyName;
            String title;
            int companyID;

            var displayName = String.Empty;

            if (contact is Company)
            {
                firstName = String.Empty;
                lastName = String.Empty;
                title = String.Empty;
                companyName = (((Company)contact).CompanyName ?? "").Trim();
                isCompany = true;
                companyID = 0;
                displayName = companyName;

                if (String.IsNullOrEmpty(companyName))
                    throw new ArgumentException();

            }
            else
            {
                var people = (Person)contact;

                firstName = people.FirstName.Trim();
                lastName = people.LastName.Trim();
                title = people.JobTitle;
                companyName = String.Empty;
                isCompany = false;

                if (IsExist(people.CompanyID))
                    companyID = people.CompanyID;
                else
                    companyID = 0;

                displayName = String.Concat(firstName, _displayNameSeparator, lastName);

                if (String.IsNullOrEmpty(firstName))// || String.IsNullOrEmpty(lastName)) lastname is not required field now
                    throw new ArgumentException();

            }

            if (!String.IsNullOrEmpty(title))
                title = title.Trim();

            if (!String.IsNullOrEmpty(contact.About))
                contact.About = contact.About.Trim();

            if (!String.IsNullOrEmpty(contact.Industry))
                contact.Industry = contact.Industry.Trim();

            var itemToInsert = new DbContact
            {
                Id = 0,
                FirstName = firstName,
                LastName = lastName,
                CompanyName = companyName,
                Title = title,
                Notes = contact.About,
                IsCompany = isCompany,
                Industry = contact.Industry,
                StatusId = contact.StatusID,
                CompanyId = companyID,
                CreateBy = SecurityContext.CurrentAccount.ID,
                CreateOn = TenantUtil.DateTimeToUtc(contact.CreateOn == DateTime.MinValue ? TenantUtil.DateTimeNow() : contact.CreateOn),
                LastModifedOn = TenantUtil.DateTimeToUtc(contact.CreateOn == DateTime.MinValue ? TenantUtil.DateTimeNow() : contact.CreateOn),
                LastModifedBy = SecurityContext.CurrentAccount.ID,
                DisplayName = displayName,
                IsShared = contact.ShareType,
                ContactTypeId = contact.ContactTypeID,
                Currency = contact.Currency
            };

            CRMDbContext.Contacts.Add(itemToInsert);
            CRMDbContext.SaveChanges();

            contact.ID = itemToInsert.Id;

            if (companyID > 0)
                AddMemberInDb(contact.ID, companyID);

            return contact.ID;
        }

        public Boolean IsExist(int contactID)
        {
            return Query(CRMDbContext.Contacts).Where(x => x.Id == contactID).Any();
        }

        public Boolean CanDelete(int contactID)
        {
            return !Query(CRMDbContext.Invoices).Where(x => x.ContactId == contactID || x.ConsigneeId == contactID).Any();
        }

        public Dictionary<int, bool> CanDelete(int[] contactID)
        {
            var result = new Dictionary<int, bool>();

            if (contactID.Length == 0) return result;

            var contactIDs = contactID.Distinct().ToList();

            List<int> hasInvoiceIDs = Query(CRMDbContext.Invoices).Where(x => contactID.Contains(x.ContactId))
                                        .Distinct()
                                        .Select(x => x.ContactId).Union(
            Query(CRMDbContext.Invoices).Where(x => contactID.Contains(x.ConsigneeId))
                                        .Distinct()
                                        .Select(x => x.ConsigneeId)).ToList();


            foreach (var cid in contactIDs)
            {
                result.Add(cid, !hasInvoiceIDs.Contains(cid));
            }

            return result;
        }

        public virtual Contact GetByID(int contactID)
        {
            return GetByIDFromDb(contactID);
        }

        public Contact GetByIDFromDb(int contactID)
        {
            return ToContact(Query(CRMDbContext.Contacts)
                      .FirstOrDefault(x => x.Id == contactID));
        }

        public List<Contact> GetContacts(int[] contactID)
        {
            if (contactID == null || contactID.Length == 0) return new List<Contact>();

            return Query(CRMDbContext.Contacts)
              .Where(x => contactID.Contains(x.Id))
              .ToList()
              .ConvertAll(ToContact)
              .FindAll(CRMSecurity.CanAccessTo);
        }

        public List<Contact> GetRestrictedContacts(int[] contactID)
        {
            if (contactID == null || contactID.Length == 0) return new List<Contact>();

            return Query(CRMDbContext.Contacts)
                        .Where(x => contactID.Contains(x.Id))
                        .ToList()
                        .ConvertAll(ToContact)
                        .FindAll(cont => !CRMSecurity.CanAccessTo(cont));
        }

        public List<Contact> GetRestrictedAndAccessedContacts(int[] contactID)
        {
            if (contactID == null || contactID.Length == 0) return new List<Contact>();

            return Query(CRMDbContext.Contacts)
                    .Where(x => contactID.Contains(x.Id))
                    .ToList()
                    .ConvertAll(ToContact);
        }

        public virtual List<Contact> DeleteBatchContact(int[] contactID)
        {
            if (contactID == null || contactID.Length == 0) return null;

            var contacts = GetContacts(contactID).Where(CRMSecurity.CanDelete).ToList();
            if (!contacts.Any()) return contacts;

            // Delete relative  keys
            _cache.Remove(new Regex(TenantID.ToString(CultureInfo.InvariantCulture) + "contacts.*"));

            DeleteBatchContactsExecute(contacts);

            return contacts;
        }

        public virtual List<Contact> DeleteBatchContact(List<Contact> contacts)
        {
            contacts = contacts.FindAll(CRMSecurity.CanDelete).ToList();
            if (!contacts.Any()) return contacts;

            // Delete relative  keys
            _cache.Remove(new Regex(TenantID.ToString(CultureInfo.InvariantCulture) + "contacts.*"));

            DeleteBatchContactsExecute(contacts);

            return contacts;
        }

        public virtual Contact DeleteContact(int contactID)
        {
            if (contactID <= 0) return null;

            var contact = GetByID(contactID);
            if (contact == null) return null;

            CRMSecurity.DemandDelete(contact);

            DeleteBatchContactsExecute(new List<Contact>() { contact });

            // Delete relative  keys
            _cache.Remove(new Regex(TenantID.ToString(CultureInfo.InvariantCulture) + "contacts.*"));

            var person = contact as Person;
            if (person != null)
            {
                FactoryIndexerContactsWrapper.DeleteAsync(person);
            }
            else
            {
                FactoryIndexerContactsWrapper.DeleteAsync(contact as Company);
            }

            return contact;
        }

        private void DeleteBatchContactsExecute(List<Contact> contacts)
        {
            var personsID = new List<int>();
            var companyID = new List<int>();
            var newContactID = new List<int>();

            foreach (var contact in contacts)
            {
                newContactID.Add(contact.ID);

                if (contact is Company)
                    companyID.Add(contact.ID);
                else
                    personsID.Add(contact.ID);
            }

            var contactID = newContactID.ToArray();
            int[] filesIDs = new int[0];

            var tx = CRMDbContext.Database.BeginTransaction();

            var tagdao = FilesIntegration.DaoFactory.GetTagDao<int>();

            var tagNames = Query(CRMDbContext.RelationshipEvent).Where(x => contactID.Contains(x.ContactId) && x.HaveFiles)
                            .Select(x => String.Format("RelationshipEvent_{0}", x.Id)).ToArray();

            if (0 < tagNames.Length)
            {
                filesIDs = tagdao.GetTags(tagNames, TagType.System)
                                 .Where(t => t.EntryType == FileEntryType.File)
                                 .Select(t => Convert.ToInt32(t.EntryId))
                                 .ToArray();
            }

            CRMDbContext.RemoveRange(Query(CRMDbContext.FieldValue)
                                          .Where(x => contactID.Contains(x.EntityId))
                                          .Where(x => x.EntityType == EntityType.Contact || x.EntityType == EntityType.Person || x.EntityType == EntityType.Company)
                                    );

            CRMDbContext.RemoveRange(Query(CRMDbContext.Tasks)
                                        .Where(x => contactID.Contains(x.ContactId)));

            CRMDbContext.RemoveRange(CRMDbContext.EntityTags
                                                .Where(x => contactID.Contains(x.EntityId) && x.EntityType == EntityType.Contact));

            CRMDbContext.RemoveRange(CRMDbContext.RelationshipEvent.Where(x => contactID.Contains(x.ContactId)));

            var dealToUpdate = CRMDbContext.Deals.Where(x => contactID.Contains(x.ContactId)).ToList();

            dealToUpdate.ForEach(x => x.ContactId = 0);

            CRMDbContext.SaveChanges();

            if (companyID.Count > 0)
            {
                var itemToUpdate = Query(CRMDbContext.Contacts).Where(x => companyID.Contains(x.CompanyId)).ToList();

                itemToUpdate.ForEach(x => x.CompanyId = 0);

                CRMDbContext.SaveChanges();
            }

            if (personsID.Count > 0)
            {
                RemoveRelativeInDb(null, EntityType.Person, personsID.ToArray());
            }

            RemoveRelativeInDb(contactID, EntityType.Any, null);

            CRMDbContext.RemoveRange(Query(CRMDbContext.ContactsInfo).Where(x => contactID.Contains(x.ContactId)));

            CRMDbContext.RemoveRange(contactID.ToList().ConvertAll(x => new DbContact
            {
                Id = x,
                TenantId = TenantID
            }));

            CRMDbContext.SaveChanges();

            tx.Commit();

            contacts.ForEach(contact => AuthorizationManager.RemoveAllAces(contact));

            var filedao = FilesIntegration.DaoFactory.GetFileDao<int>();

            foreach (var filesID in filesIDs)
            {
                filedao.DeleteFile(filesID);
            }

            //todo: remove indexes
        }

        private void MergeContactInfo(Contact fromContact, Contact toContact)
        {
            if ((toContact is Person) && (fromContact is Person))
            {
                var fromPeople = (Person)fromContact;
                var toPeople = (Person)toContact;

                if (toPeople.CompanyID == 0)
                    toPeople.CompanyID = fromPeople.CompanyID;

                if (String.IsNullOrEmpty(toPeople.JobTitle))
                    toPeople.JobTitle = fromPeople.JobTitle;
            }

            if (String.IsNullOrEmpty(toContact.Industry))
                toContact.Industry = fromContact.Industry;

            if (toContact.StatusID == 0)
                toContact.StatusID = fromContact.StatusID;
            if (toContact.ContactTypeID == 0)
                toContact.ContactTypeID = fromContact.ContactTypeID;

            if (String.IsNullOrEmpty(toContact.About))
                toContact.About = fromContact.About;

            UpdateContactFromDb(toContact);
        }

        public void MergeDublicate(int fromContactID, int toContactID)
        {
            if (fromContactID == toContactID)
            {
                if (GetByID(fromContactID) == null)
                    throw new ArgumentException();
                return;
            }

            var fromContact = GetByID(fromContactID);
            var toContact = GetByID(toContactID);

            if (fromContact == null || toContact == null)
                throw new ArgumentException();

            using (var tx = CRMDbContext.Database.BeginTransaction())
            {
                var taskToUpdate = Query(CRMDbContext.Tasks)
                                        .Where(x => x.ContactId == fromContactID)
                                        .ToList();

                taskToUpdate.ForEach(x => x.ContactId = toContactID);

                CRMDbContext.UpdateRange(taskToUpdate);
                CRMDbContext.SaveChanges();

                // crm_entity_contact
                CRMDbContext.EntityContact.RemoveRange(
                                                CRMDbContext.EntityContact
                                                    .Join(CRMDbContext.EntityContact,
                                                          x => new { x.EntityId, x.EntityType },
                                                          y => new { y.EntityId, y.EntityType },
                                                          (x, y) => new { x, y })
                                                    .Where(x => x.x.ContactId == fromContactID && x.y.ContactId == toContactID)
                                                    .Select(x => x.x));

                var entityContactToUpdate = CRMDbContext.EntityContact.Where(x => x.ContactId == fromContactID).ToList();
                entityContactToUpdate.ForEach(x => x.ContactId = toContactID);

                CRMDbContext.UpdateRange(entityContactToUpdate);
                CRMDbContext.SaveChanges();

                // crm_deal
                var dealsToUpdate = Query(CRMDbContext.Deals).Where(x => x.ContactId == fromContactID).ToList();
                dealsToUpdate.ForEach(x => x.ContactId = toContactID);

                CRMDbContext.UpdateRange(dealsToUpdate);
                CRMDbContext.SaveChanges();

                // crm_invoice
                var invoicesToUpdate = Query(CRMDbContext.Invoices).Where(x => x.ContactId == fromContactID).ToList();
                invoicesToUpdate.ForEach(x => x.ContactId = toContactID);

                CRMDbContext.UpdateRange(invoicesToUpdate);
                CRMDbContext.SaveChanges();

                // crm_projects
                var dublicateProjectID = Query(CRMDbContext.Projects)
                    .Join(CRMDbContext.Projects,
                           x => x.ProjectId,
                           y => y.ProjectId,
                           (x, y) => new { x, y })
                    .Where(x => x.x.ContactId == fromContactID && x.y.ContactId == toContactID)
                    .Select(x => x.x.ProjectId);

                CRMDbContext.RemoveRange(Query(CRMDbContext.Projects)
                                            .Where(x => x.ContactId == fromContactID && dublicateProjectID.Contains(x.ProjectId)));

                var projectsToUpdate = Query(CRMDbContext.Projects).Where(x => x.ContactId == fromContactID).ToList();
                projectsToUpdate.ForEach(x => x.ContactId = toContactID);

                CRMDbContext.UpdateRange(projectsToUpdate);
                CRMDbContext.SaveChanges();

                // crm_relationship_event
                var relationshipEventToUpdate = Query(CRMDbContext.RelationshipEvent).Where(x => x.ContactId == fromContactID).ToList();
                relationshipEventToUpdate.ForEach(x => x.ContactId = toContactID);

                CRMDbContext.UpdateRange(relationshipEventToUpdate);
                CRMDbContext.SaveChanges();

                // crm_entity_tag
                var dublicateTagsID = CRMDbContext.EntityTags.Join(CRMDbContext.EntityTags,
                                                                   x => new { x.TagId, x.EntityType },
                                                                   y => new { y.TagId, y.EntityType },
                                                                   (x, y) => new { x, y }
                                                                )
                                                    .Where(x => x.x.EntityId == fromContactID && x.y.EntityId == toContactID)
                                                    .Select(x => x.x.TagId).ToList();

                CRMDbContext.EntityTags.Where(x => x.EntityId == fromContactID &&
                                                   x.EntityType == EntityType.Contact &&
                                                   dublicateTagsID.Contains(x.TagId));


                var entityTagToUpdate = CRMDbContext.EntityTags.Where(x => x.EntityId == fromContactID && x.EntityType == EntityType.Contact).ToList();

                entityTagToUpdate.ForEach(x => x.EntityId = toContactID);

                CRMDbContext.UpdateRange(entityTagToUpdate);
                CRMDbContext.SaveChanges();

                // crm_field_value
                var dublicateCustomFieldValueID = Query(CRMDbContext.FieldValue)
                                                        .Join(CRMDbContext.FieldValue,
                                                              x => new { x.TenantId, x.FieldId, x.EntityType },
                                                              y => new { y.TenantId, y.FieldId, y.EntityType },
                                                              (x, y) => new { x, y })
                                                        .Where(x => x.x.EntityId == fromContactID && x.y.EntityId == toContactID)
                                                        .Select(x => x.x.FieldId);

                CRMDbContext.RemoveRange(CRMDbContext.FieldValue.Where(x => x.EntityId == fromContactID &&
                                                                            (new[] { EntityType.Contact, EntityType.Person, EntityType.Company }).Contains(x.EntityType) &&
                                                                              dublicateCustomFieldValueID.Contains(x.FieldId)));

                var fieldValueToUpdate = Query(CRMDbContext.FieldValue)
                                            .Where(x => x.EntityId == fromContactID && x.EntityType == EntityType.Contact).ToList();
                fieldValueToUpdate.ForEach(x => x.EntityId = toContactID);

                CRMDbContext.UpdateRange(fieldValueToUpdate);
                CRMDbContext.SaveChanges();

                // crm_contact_info
                var dublicatePrimaryContactInfoID = Query(CRMDbContext.ContactsInfo)
                        .Join(CRMDbContext.ContactsInfo,
                                x => new { x.TenantId, x.Type, x.Category, x.IsPrimary },
                                y => new { y.TenantId, y.Type, y.Category, y.IsPrimary },
                                (x, y) => new { x, y })
                        .Where(x => x.x.IsPrimary &&
                                    x.x.Data != x.y.Data &&
                                    x.x.ContactId == fromContactID &&
                                    x.y.ContactId == toContactID)
                        .Select(x => x.x.Id);

                var contactInfosToUpdate = Query(CRMDbContext.ContactsInfo)
                                            .Where(x => x.ContactId == fromContactID && dublicatePrimaryContactInfoID.Contains(x.Id))
                                            .ToList();

                contactInfosToUpdate.ForEach(x => x.IsPrimary = false);

                CRMDbContext.UpdateRange(fieldValueToUpdate);
                CRMDbContext.SaveChanges();

                var dublicateContactInfoID = Query(CRMDbContext.ContactsInfo)
                                                .Join(CRMDbContext.ContactsInfo,
                                                          x => new { x.TenantId, x.Type, x.IsPrimary, x.Category, x.Data },
                                                          y => new { y.TenantId, y.Type, y.IsPrimary, y.Category, y.Data },
                                                          (x, y) => new { x, y })
                                                .Where(x => x.x.ContactId == fromContactID && x.y.ContactId == toContactID)
                                                .Select(x => x.x.Id)
                                                .ToList();

                CRMDbContext.ContactsInfo.RemoveRange(dublicateContactInfoID.ConvertAll(x => new DbContactInfo
                {
                    Id = x,
                    ContactId = fromContactID,
                    TenantId = TenantID
                }));

                CRMDbContext.SaveChanges();

                var contactInfoToUpdate = Query(CRMDbContext.ContactsInfo).Where(x => x.ContactId == fromContactID).ToList();
                contactInfoToUpdate.ForEach(x => x.ContactId = toContactID);

                CRMDbContext.UpdateRange(contactInfoToUpdate);
                CRMDbContext.SaveChanges();

                MergeContactInfo(fromContact, toContact);

                // crm_contact
                if ((fromContact is Company) && (toContact is Company))
                {
                    var contactsToUpdate = Query(CRMDbContext.Contacts).Where(x => x.CompanyId == fromContactID).ToList();

                    contactsToUpdate.ForEach(x => x.CompanyId = toContactID);

                    CRMDbContext.UpdateRange(contactsToUpdate);
                    CRMDbContext.SaveChanges();

                }

                CRMDbContext.Contacts.Remove(new DbContact
                {
                    Id = fromContactID
                });

                CRMDbContext.SaveChanges();

                tx.Commit();
            }

            AuthorizationManager.RemoveAllAces(fromContact);
        }

        public List<int> GetContactIDsByCompanyIds(List<int> companyIDs)
        {
            return Query(CRMDbContext.Contacts)
                .Where(x => companyIDs.Contains(x.CompanyId) && !x.IsCompany)
                .Select(x => x.Id)
                .Distinct()
                .ToList();
        }

        public List<int> GetContactIDsByContactInfo(ContactInfoType infoType, String data, int? category, bool? isPrimary)
        {
            var q = Query(CRMDbContext.ContactsInfo)
                    .Where(x => x.Type == infoType);

            if (!string.IsNullOrWhiteSpace(data))
            {
                q = q.Where(x => Microsoft.EntityFrameworkCore.EF.Functions.Like(x.Data, "%" + data + "%"));
            }

            if (category.HasValue)
            {
                q = q.Where(x => x.Category == category.Value);
            }

            if (isPrimary.HasValue)
            {
                q = q.Where(x => x.IsPrimary == isPrimary.Value);
            }

            return q.Select(x => x.ContactId).ToList();
        }

        protected Contact ToContact(DbContact dbContact)
        {
            if (dbContact == null) return null;

            Contact contact;

            var isCompany = dbContact.IsCompany;

            if (isCompany)
                contact = new Company
                {
                    CompanyName = dbContact.CompanyName
                };
            else
                contact = new Person
                {
                    FirstName = dbContact.FirstName,
                    LastName = dbContact.LastName,
                    JobTitle = dbContact.Title,
                    CompanyID = dbContact.CompanyId
                };

            contact.ID = dbContact.Id;
            contact.About = dbContact.Notes;
            contact.Industry = dbContact.Industry;
            contact.StatusID = dbContact.StatusId;
            contact.CreateOn = TenantUtil.DateTimeFromUtc(dbContact.CreateOn);
            contact.CreateBy = dbContact.CreateBy;
            contact.ContactTypeID = dbContact.ContactTypeId;
            contact.Currency = dbContact.Currency;

            if (dbContact.IsShared == null)
            {
                var accessSubjectToContact = CRMSecurity.GetAccessSubjectTo(contact);

                contact.ShareType = !accessSubjectToContact.Any() ? ShareType.ReadWrite : ShareType.None;
            }
            else
            {
                contact.ShareType = (ShareType)(Convert.ToInt32(dbContact.IsShared));
            }

            return contact;
        }

        public void ReassignContactsResponsible(Guid fromUserId, Guid toUserId)
        {
            var ids = CRMSecurity.GetContactsIdByManager(fromUserId).ToList();

            foreach (var id in ids)
            {
                var contact = GetByID(id);

                if (contact == null) continue;

                var responsibles = CRMSecurity.GetAccessSubjectGuidsTo(contact);

                if (!responsibles.Any()) continue;

                responsibles.Remove(fromUserId);
                responsibles.Add(toUserId);

                CRMSecurity.SetAccessTo(contact, responsibles.Distinct().ToList());
            }
        }


        /// <summary>
        /// Test method
        /// </summary>
        /// <param name="contactId"></param>
        /// <param name="creationDate"></param>
        public void SetContactCreationDate(int contactId, DateTime creationDate)
        {
            var itemToUpdate = new DbContact
            {
                Id = contactId,
                TenantId = TenantID,
                CreateOn = TenantUtil.DateTimeToUtc(creationDate)
            };

            CRMDbContext.Attach(itemToUpdate);
            CRMDbContext.Entry(itemToUpdate).Property(x => x.CreateOn).IsModified = true;
            CRMDbContext.SaveChanges();

            // Delete relative keys
            _cache.Remove(new Regex(TenantID.ToString(CultureInfo.InvariantCulture) + "contacts.*"));
        }

        /// <summary>
        /// Test method
        /// </summary>
        /// <param name="contactId"></param>
        /// <param name="lastModifedDate"></param>
        public void SetContactLastModifedDate(int contactId, DateTime lastModifedDate)
        {

            var itemToUpdate = new DbContact
            {
                Id = contactId,
                TenantId = TenantID,
                LastModifedOn = TenantUtil.DateTimeToUtc(lastModifedDate)
            };

            CRMDbContext.Attach(itemToUpdate);
            CRMDbContext.Entry(itemToUpdate).Property(x => x.LastModifedOn).IsModified = true;
            CRMDbContext.SaveChanges();

            // Delete relative keys
            _cache.Remove(new Regex(TenantID.ToString(CultureInfo.InvariantCulture) + "contacts.*"));
        }
    }
}