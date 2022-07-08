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
using System.Text.Json;
using System.Text.RegularExpressions;

using ASC.Common;
using ASC.Common.Caching;
using ASC.Core;
using ASC.Core.Common.EF;
using ASC.Core.Common.Settings;
using ASC.Core.Tenants;
using ASC.CRM.Core.EF;
using ASC.CRM.Core.Entities;
using ASC.CRM.Core.Enums;
using ASC.ElasticSearch;
using ASC.Web.CRM.Classes;
using ASC.Web.CRM.Core.Search;

using AutoMapper;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

using SecurityContext = ASC.Core.SecurityContext;

namespace ASC.CRM.Core.Dao
{
    [Scope]
    public class InvoiceDao : AbstractDao
    {
        public List<KeyValuePair<InvoiceStatus, InvoiceStatus>> invoiceStatusMap = new List<KeyValuePair<InvoiceStatus, InvoiceStatus>>()
        {
            new KeyValuePair<InvoiceStatus, InvoiceStatus>(InvoiceStatus.Draft, InvoiceStatus.Sent),
            new KeyValuePair<InvoiceStatus, InvoiceStatus>(InvoiceStatus.Sent, InvoiceStatus.Paid),
            new KeyValuePair<InvoiceStatus, InvoiceStatus>(InvoiceStatus.Sent, InvoiceStatus.Rejected),
            new KeyValuePair<InvoiceStatus, InvoiceStatus>(InvoiceStatus.Rejected, InvoiceStatus.Draft),
            new KeyValuePair<InvoiceStatus, InvoiceStatus>(InvoiceStatus.Paid, InvoiceStatus.Sent)//Bug 23450
        };

        private readonly InvoiceSetting _invoiceSetting;
        private readonly InvoiceFormattedData _invoiceFormattedData;
        private readonly SettingsManager _settingsManager;
        private readonly FactoryIndexerInvoice _factoryIndexer;
        private readonly TenantUtil _tenantUtil;
        private readonly CrmSecurity _crmSecurity;

        public InvoiceDao(
            DbContextManager<CrmDbContext> dbContextManager,
            TenantManager tenantManager,
            SecurityContext securityContext,
            FactoryIndexerInvoice factoryIndexer,
            ILogger logger,
            ICache ascCache,
            SettingsManager settingsManager,
            InvoiceSetting invoiceSetting,
            InvoiceFormattedData invoiceFormattedData,
            CrmSecurity crmSecurity,
            TenantUtil tenantUtil,
            IMapper mapper)
              : base(dbContextManager,
                 tenantManager,
                 securityContext,
                 logger,
                 ascCache,
                 mapper)
        {
            _factoryIndexer = factoryIndexer;
            _settingsManager = settingsManager;
            _invoiceSetting = invoiceSetting;
            _invoiceFormattedData = invoiceFormattedData;
            _crmSecurity = crmSecurity;
            _tenantUtil = tenantUtil;
        }


        public Boolean IsExist(int invoiceID)
        {
            return IsExistFromDb(invoiceID);
        }

        public Boolean IsExistFromDb(int id)
        {
            return Query(CrmDbContext.Invoices).Where(x => x.Id == id).Any();
        }

        public Boolean IsExist(string number)
        {
            return IsExistFromDb(number);
        }

        public Boolean IsExistFromDb(string number)
        {
            return Query(CrmDbContext.Invoices)
                .Where(x => x.Number == number)
                .Any();
        }

        public List<Invoice> GetAll()
        {
            var dbInvoices = Query(CrmDbContext.Invoices)
                .ToList();

            return _mapper.Map<List<DbInvoice>, List<Invoice>>(dbInvoices);
        }

        public List<Invoice> GetByID(int[] ids)
        {
            var dbInvoices = Query(CrmDbContext.Invoices)
                                    .AsNoTracking()
                                    .Where(x => ids.Contains(x.Id))
                                    .ToList();

            return _mapper.Map<List<DbInvoice>, List<Invoice>>(dbInvoices);
        }

        public Invoice GetByID(int id)
        {
            return GetByIDFromDb(id);
        }

        public Invoice GetByIDFromDb(int id)
        {
            var dbEntity = CrmDbContext.Invoices.Find(id);

            if (dbEntity.TenantId != TenantID)
                throw new ArgumentException();

            return _mapper.Map<Invoice>(dbEntity);
        }

        public Invoice GetByNumber(string number)
        {
            var dbEntity = Query(CrmDbContext.Invoices).FirstOrDefault(x => x.Number == number);

            return _mapper.Map<Invoice>(dbEntity);
        }

        public Invoice GetByFileId(Int32 fileID)
        {
            return _mapper.Map<Invoice>(Query(CrmDbContext.Invoices)
                    .FirstOrDefault(x => x.FileId == fileID));
        }

        public List<Invoice> GetInvoices(
                                    String searchText,
                                    InvoiceStatus? status,
                                    DateTime issueDateFrom,
                                    DateTime issueDateTo,
                                    DateTime dueDateFrom,
                                    DateTime dueDateTo,
                                    EntityType entityType,
                                    int entityID,
                                    String currency,
                                    int from,
                                    int count,
                                    OrderBy orderBy)
        {

            if (_crmSecurity.IsAdmin)
                return GetCrudeInvoices(
                    searchText,
                    status,
                    issueDateFrom,
                    issueDateTo,
                    dueDateFrom,
                    dueDateTo,
                    entityType,
                    entityID,
                    currency,
                    from,
                    count,
                    orderBy);


            var crudeInvoices = GetCrudeInvoices(
                    searchText,
                    status,
                    issueDateFrom,
                    issueDateTo,
                    dueDateFrom,
                    dueDateTo,
                    entityType,
                    entityID,
                    currency,
                    0,
                    from + count,
                    orderBy);

            if (crudeInvoices.Count == 0) return crudeInvoices;

            if (crudeInvoices.Count < from + count) return _crmSecurity.FilterRead(crudeInvoices).Skip(from).ToList();

            var result = _crmSecurity.FilterRead(crudeInvoices).ToList();

            if (result.Count == crudeInvoices.Count) return result.Skip(from).ToList();

            var localCount = count;
            var localFrom = from + count;

            while (true)
            {
                crudeInvoices = GetCrudeInvoices(
                    searchText,
                    status,
                    issueDateFrom,
                    issueDateTo,
                    dueDateFrom,
                    dueDateTo,
                    entityType,
                    entityID,
                    currency,
                    localFrom,
                    localCount,
                    orderBy);

                if (crudeInvoices.Count == 0) break;

                result.AddRange(_crmSecurity.FilterRead(crudeInvoices));

                if ((result.Count >= count + from) || (crudeInvoices.Count < localCount)) break;

                localFrom += localCount;
                localCount = localCount * 2;
            }

            return result.Skip(from).Take(count).ToList();
        }


        public List<Invoice> GetCrudeInvoices(
                                String searchText,
                                InvoiceStatus? status,
                                DateTime issueDateFrom,
                                DateTime issueDateTo,
                                DateTime dueDateFrom,
                                DateTime dueDateTo,
                                EntityType entityType,
                                int entityID,
                                String currency,
                                int from,
                                int count,
                                OrderBy orderBy)
        {
            var withParams = hasParams(searchText, status, issueDateFrom, issueDateTo, dueDateFrom, dueDateTo, entityType, entityID, currency);

            var sqlQuery = GetDbInvoceByFilters(new List<int>(), searchText, status, issueDateFrom, issueDateTo, dueDateFrom, dueDateTo, entityType, entityID, currency);

            if (withParams && sqlQuery == null)
                return new List<Invoice>();

            if (0 < from && from < int.MaxValue) sqlQuery = sqlQuery.Skip(from);
            if (0 < count && count < int.MaxValue) sqlQuery = sqlQuery.Take(count);

            if (orderBy != null && Enum.IsDefined(typeof(InvoiceSortedByType), orderBy.SortedBy))
            {
                switch ((InvoiceSortedByType)orderBy.SortedBy)
                {
                    case InvoiceSortedByType.Number:
                        sqlQuery = sqlQuery.OrderBy("Number", orderBy.IsAsc);
                        break;
                    case InvoiceSortedByType.Status:
                        sqlQuery = sqlQuery.OrderBy("Status", orderBy.IsAsc);
                        break;
                    case InvoiceSortedByType.DueDate:
                        sqlQuery = sqlQuery.OrderBy("DueDate", orderBy.IsAsc);
                        break;
                    case InvoiceSortedByType.IssueDate:
                        sqlQuery = sqlQuery.OrderBy("IssueDate", orderBy.IsAsc);
                        break;
                    case InvoiceSortedByType.Contact:

                        var subSqlQuery = sqlQuery.GroupJoin(CrmDbContext.Contacts,
                                        x => x.ContactId,
                                        y => y.Id,
                                        (x, y) => new { x, y })
                                        .SelectMany(x => x.y.DefaultIfEmpty(), (x, y) => new { x.x, y });

                        if (orderBy.IsAsc)
                        {
                            subSqlQuery = subSqlQuery.OrderBy(x => x.y != null ? x.y.DisplayName : "")
                                .ThenBy(x => x.x.Number);
                        }
                        else
                        {
                            subSqlQuery = subSqlQuery.OrderByDescending(x => x.y != null ? x.y.DisplayName : "")
                                .ThenBy(x => x.x.Number);

                        }

                        sqlQuery = subSqlQuery.Select(x => x.x);

                        break;
                    default:
                        sqlQuery = sqlQuery.OrderBy("Number", orderBy.IsAsc);

                        break;
                }
            }
            else
            {
                sqlQuery = sqlQuery.OrderBy("Number", orderBy.IsAsc);
            }

            return _mapper.Map<List<DbInvoice>, List<Invoice>>(sqlQuery.ToList());
        }

        public int GetAllInvoicesCount()
        {
            return Query(CrmDbContext.Invoices).Count();
        }


        public int GetInvoicesCount()
        {
            return GetInvoicesCount(String.Empty, null, DateTime.MinValue, DateTime.MinValue, DateTime.MinValue, DateTime.MinValue, EntityType.Any, 0, null);
        }

        public int GetInvoicesCount(
                                    String searchText,
                                    InvoiceStatus? status,
                                    DateTime issueDateFrom,
                                    DateTime issueDateTo,
                                    DateTime dueDateFrom,
                                    DateTime dueDateTo,
                                    EntityType entityType,
                                    int entityID,
                                    String currency)
        {
            var cacheKey = TenantID.ToString(CultureInfo.InvariantCulture) +
                           "invoice" +
                           _securityContext.CurrentAccount.ID.ToString() +
                           searchText;

            var fromCache = _cache.Get<string>(cacheKey);

            if (fromCache != null) return Convert.ToInt32(fromCache);

            var withParams = hasParams(searchText, status, issueDateFrom, issueDateTo, dueDateFrom, dueDateTo, entityType, entityID, currency);

            var exceptIDs = _crmSecurity.GetPrivateItems(typeof(Invoice)).ToList();

            int result;

            if (withParams)
            {
                var sqlQuery = GetDbInvoceByFilters(exceptIDs, searchText, status, issueDateFrom, issueDateTo, dueDateFrom, dueDateTo, entityType, entityID, currency);

                result = sqlQuery != null ? sqlQuery.Count() : 0;

            }
            else
            {
                var countWithoutPrivate = Query(CrmDbContext.Invoices).Count();

                var privateCount = exceptIDs.Count;

                if (privateCount > countWithoutPrivate)
                {
                    _logger.LogError(@"Private invoice count more than all cases. Tenant: {0}. CurrentAccount: {1}",
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

        public List<Invoice> GetInvoices(int[] ids)
        {
            if (ids == null || !ids.Any()) return new List<Invoice>();

            var dbInvoices = Query(CrmDbContext.Invoices)
                .AsNoTracking()
                .Where(x => ids.Contains(x.Id))
                .ToList();

            return _mapper.Map<List<DbInvoice>, List<Invoice>>(dbInvoices)
                .FindAll(_crmSecurity.CanAccessTo);
        }

        public List<Invoice> GetEntityInvoices(EntityType entityType, int entityID)
        {
            var result = new List<Invoice>();

            if (entityID <= 0)
                return result;

            if (entityType == EntityType.Opportunity)
            {

                var sqlQuery = Query(CrmDbContext.Invoices)
                                .AsNoTracking()
                                .Where(x => x.EntityId == entityID && x.EntityType == entityType)
                                .ToList();

                return _mapper.Map<List<DbInvoice>, List<Invoice>>(sqlQuery)
                    .FindAll(_crmSecurity.CanAccessTo);

            }

            if (entityType == EntityType.Contact || entityType == EntityType.Person || entityType == EntityType.Company)
                return GetContactInvoices(entityID);

            return result;
        }

        public List<Invoice> GetContactInvoices(int contactID)
        {
            var result = new List<Invoice>();

            if (contactID <= 0)
                return result;

            var dbInvoices = Query(CrmDbContext.Invoices)
                            .AsNoTracking()
                            .Where(x => x.ContactId == contactID)
                            .ToList();

            return _mapper.Map<List<DbInvoice>, List<Invoice>>(dbInvoices)
                    .FindAll(_crmSecurity.CanAccessTo);
        }

        public string GetNewInvoicesNumber()
        {
            var settings = GetSettings();

            if (!settings.Autogenerated)
                return string.Empty;

            var stringNumber = Query(CrmDbContext.Invoices).OrderByDescending(x => x.Id).Select(x => x.Number).ToString();

            if (string.IsNullOrEmpty(stringNumber) || !stringNumber.StartsWith(settings.Prefix))
                return string.Concat(settings.Prefix, settings.Number);

            if (!string.IsNullOrEmpty(settings.Prefix))
                stringNumber = stringNumber.Replace(settings.Prefix, string.Empty);

            int intNumber;
            if (!Int32.TryParse(stringNumber, out intNumber))
                intNumber = 0;

            var format = string.Empty;
            for (var i = 0; i < settings.Number.Length; i++)
            {
                format += "0";
            }

            var nextNumber = intNumber + 1;

            return settings.Prefix + (string.IsNullOrEmpty(format) ? nextNumber.ToString(CultureInfo.InvariantCulture) : nextNumber.ToString(format));
        }

        public InvoiceSetting GetSettings()
        {
            var tenantSettings = _settingsManager.Load<CrmSettings>();

            return tenantSettings.InvoiceSetting ?? _invoiceSetting.DefaultSettings;
        }

        public int SaveOrUpdateInvoice(Invoice invoice)
        {
            _cache.Remove(new Regex(TenantID.ToString(CultureInfo.InvariantCulture) + "invoice.*"));

            var result = SaveOrUpdateInvoiceInDb(invoice);



            _factoryIndexer.Index(Query(CrmDbContext.Invoices).Where(x => x.Id == invoice.ID).Single());

            return result;
        }

        private int SaveOrUpdateInvoiceInDb(Invoice invoice)
        {
            if (String.IsNullOrEmpty(invoice.Number) ||
                invoice.IssueDate == DateTime.MinValue ||
                invoice.ContactID <= 0 ||
                invoice.DueDate == DateTime.MinValue ||
                String.IsNullOrEmpty(invoice.Currency) ||
                invoice.ExchangeRate <= 0 ||
                String.IsNullOrEmpty(invoice.Terms))
                throw new ArgumentException();

            var dbEntity = new DbInvoice
            {
                Id = invoice.ID,
                Status = invoice.Status,
                Number = invoice.Number,
                IssueDate = _tenantUtil.DateTimeToUtc(invoice.IssueDate),
                TemplateType = invoice.TemplateType,
                ContactId = invoice.ContactID,
                ConsigneeId = invoice.ConsigneeID,
                EntityType = invoice.EntityType,
                EntityId = invoice.EntityID,
                DueDate = _tenantUtil.DateTimeToUtc(invoice.DueDate),
                Language = invoice.Language,
                Currency = invoice.Currency,
                ExchangeRate = invoice.ExchangeRate,
                PurchaseOrderNumber = invoice.PurchaseOrderNumber,
                Terms = invoice.Terms,
                Description = invoice.Description,
                JsonData = invoice.JsonData,
                FileId = invoice.FileID,
                CreateOn = invoice.CreateOn == DateTime.MinValue ? DateTime.UtcNow : invoice.CreateOn,
                CreateBy = _securityContext.CurrentAccount.ID,
                LastModifedOn = DateTime.UtcNow,
                LastModifedBy = _securityContext.CurrentAccount.ID,
                TenantId = TenantID
            };

            dbEntity.PurchaseOrderNumber = !String.IsNullOrEmpty(invoice.PurchaseOrderNumber) ? invoice.PurchaseOrderNumber : String.Empty;

            CrmDbContext.Update(dbEntity);
            CrmDbContext.SaveChanges();

            return dbEntity.Id;
        }

        public Invoice UpdateInvoiceStatus(int invoiceid, InvoiceStatus status)
        {
            return UpdateInvoiceStatusInDb(invoiceid, status);
        }

        public List<Invoice> UpdateInvoiceBatchStatus(int[] invoiceids, InvoiceStatus status)
        {
            if (invoiceids == null || !invoiceids.Any())
                throw new ArgumentException();

            var invoices = new List<Invoice>();

            foreach (var id in invoiceids)
            {
                var inv = UpdateInvoiceStatusInDb(id, status);
                if (inv != null)
                {
                    invoices.Add(inv);
                }
            }
            return invoices;
        }

        private Invoice UpdateInvoiceStatusInDb(int invoiceid, InvoiceStatus status)
        {
            var invoice = GetByIDFromDb(invoiceid);
            if (invoice == null)
            {
                _logger.LogError("Invoice not found");

                return null;
            }
            _crmSecurity.DemandAccessTo(invoice);

            if (!invoiceStatusMap.Contains(new KeyValuePair<InvoiceStatus, InvoiceStatus>(invoice.Status, status)))
            {
                _logger.LogError("Status for invoice with ID={0} can't be changed. Return without changes", invoiceid);

                return invoice;
            }

            var itemToUpdate = Query(CrmDbContext.Invoices).FirstOrDefault(x => x.Id == invoiceid);

            itemToUpdate.Status = status;
            itemToUpdate.LastModifedOn = DateTime.UtcNow;
            itemToUpdate.LastModifedBy = _securityContext.CurrentAccount.ID;

            CrmDbContext.Update(itemToUpdate);

            CrmDbContext.SaveChanges();

            invoice.Status = status;

            return invoice;

        }

        public int UpdateInvoiceJsonData(int invoiceId, string jsonData)
        {
            return UpdateInvoiceJsonDataInDb(invoiceId, jsonData);
        }

        private int UpdateInvoiceJsonDataInDb(int invoiceId, string jsonData)
        {
            var itemToUpdate = Query(CrmDbContext.Invoices).FirstOrDefault(x => x.Id == invoiceId);

            itemToUpdate.JsonData = jsonData;
            itemToUpdate.LastModifedOn = DateTime.UtcNow;
            itemToUpdate.LastModifedBy = _securityContext.CurrentAccount.ID;

            CrmDbContext.Update(itemToUpdate);
            CrmDbContext.SaveChanges();

            return invoiceId;
        }

        public void UpdateInvoiceJsonData(Invoice invoice, int billingAddressID, int deliveryAddressID)
        {
            var jsonData = _invoiceFormattedData.GetData(invoice, billingAddressID, deliveryAddressID);
            if (jsonData.LogoBase64Id != 0)
            {
                jsonData.LogoBase64 = null;
            }

            invoice.JsonData = JsonSerializer.Serialize(jsonData);

            UpdateInvoiceJsonData(invoice.ID, invoice.JsonData);
        }

        public void UpdateInvoiceJsonDataAfterLinesUpdated(Invoice invoice)
        {
            var jsonData = _invoiceFormattedData.GetDataAfterLinesUpdated(invoice);
            if (jsonData.LogoBase64Id != 0)
            {
                jsonData.LogoBase64 = null;
            }
            UpdateInvoiceJsonData(invoice.ID, invoice.JsonData);
        }

        public int UpdateInvoiceFileID(int invoiceId, int fileId)
        {
            return UpdateInvoiceFileIDInDb(invoiceId, fileId);
        }

        private int UpdateInvoiceFileIDInDb(int invoiceId, int fileId)
        {

            var sqlToUpdate = Query(CrmDbContext.Invoices).FirstOrDefault(x => x.Id == invoiceId);

            sqlToUpdate.FileId = fileId;
            sqlToUpdate.LastModifedOn = DateTime.UtcNow;
            sqlToUpdate.LastModifedBy = _securityContext.CurrentAccount.ID;

            CrmDbContext.Update(sqlToUpdate);
            CrmDbContext.SaveChanges();

            return invoiceId;
        }

        public InvoiceSetting SaveInvoiceSettings(InvoiceSetting invoiceSetting)
        {
            var tenantSettings = _settingsManager.Load<CrmSettings>();
            tenantSettings.InvoiceSetting = invoiceSetting;

            _settingsManager.Save<CrmSettings>(tenantSettings);

            return tenantSettings.InvoiceSetting;
        }

        public Invoice DeleteInvoice(int invoiceID)
        {
            if (invoiceID <= 0) return null;

            var invoice = GetByID(invoiceID);
            if (invoice == null) return null;

            _crmSecurity.DemandDelete(invoice);

            // Delete relative  keys
            _cache.Remove(new Regex(TenantID.ToString(CultureInfo.InvariantCulture) + "invoice.*"));

            DeleteBatchInvoicesExecute(new List<Invoice> { invoice });

            return invoice;
        }

        public List<Invoice> DeleteBatchInvoices(int[] invoiceID)
        {
            var invoices = GetInvoices(invoiceID).Where(_crmSecurity.CanDelete).ToList();
            if (!invoices.Any()) return invoices;

            // Delete relative  keys
            _cache.Remove(new Regex(TenantID.ToString(CultureInfo.InvariantCulture) + "invoice.*"));

            DeleteBatchInvoicesExecute(invoices);

            return invoices;
        }

        private void DeleteBatchInvoicesExecute(List<Invoice> invoices)
        {
            var invoiceID = invoices.Select(x => x.ID).ToArray();

            using var tx = CrmDbContext.Database.BeginTransaction();

            var dbInvoicesQuery = Query(CrmDbContext.Invoices).Where(x => invoiceID.Contains(x.Id));

            CrmDbContext.InvoiceLine.RemoveRange(Query(CrmDbContext.InvoiceLine).Where(x => invoiceID.Contains(x.InvoiceId)));
            CrmDbContext.Invoices.RemoveRange(dbInvoicesQuery);

            CrmDbContext.SaveChanges();

            tx.Commit();

            dbInvoicesQuery.ToList().ForEach(invoice => _factoryIndexer.Delete(invoice));
        }

        private IQueryable<DbInvoice> GetDbInvoceByFilters(
                                ICollection<int> exceptIDs,
                                string searchText,
                                InvoiceStatus? status,
                                DateTime issueDateFrom,
                                DateTime issueDateTo,
                                DateTime dueDateFrom,
                                DateTime dueDateTo,
                                EntityType entityType,
                                int entityID,
                                String currency)
        {
            var sqlQuery = Query(CrmDbContext.Invoices)
                            .AsNoTracking();

            if (entityID > 0)
            {

                switch (entityType)
                {
                    case EntityType.Contact:
                    case EntityType.Person:
                    case EntityType.Company:
                        sqlQuery = sqlQuery.Where(x => x.ContactId == entityID);
                        break;
                    case EntityType.Case:
                    case EntityType.Opportunity:
                        sqlQuery = sqlQuery.Where(x => x.EntityId == entityID && x.EntityType == entityType);
                        break;
                }

            }

            if (status != null)
            {
                sqlQuery = sqlQuery.Where(x => x.Status == status.Value);
            }

            if (!String.IsNullOrEmpty(searchText))
            {
                searchText = searchText.Trim();

                var keywords = searchText.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries)
                   .ToArray();

                if (keywords.Length > 0)
                {
                    List<int> invoicesIds;
                    if (!_factoryIndexer.TrySelectIds(s => s.MatchAll(searchText), out invoicesIds))
                    {
                        foreach (var k in keywords)
                        {
                            sqlQuery = sqlQuery.Where(x => Microsoft.EntityFrameworkCore.EF.Functions.Like(x.Number, k + "%") ||
                                                             Microsoft.EntityFrameworkCore.EF.Functions.Like(x.Description, k + "%"));
                        }
                    }
                    else
                    {
                        sqlQuery = sqlQuery.Where(x => invoicesIds.Contains(x.Id));
                    }
                }
            }

            if (exceptIDs.Count > 0)
            {
                sqlQuery = sqlQuery.Where(x => !exceptIDs.Contains(x.Id));
            }

            if (issueDateFrom != DateTime.MinValue && issueDateTo != DateTime.MinValue)
            {
                sqlQuery = sqlQuery.Where(x => x.IssueDate >= _tenantUtil.DateTimeToUtc(issueDateFrom) && x.DueDate <= _tenantUtil.DateTimeToUtc(issueDateTo.AddDays(1).AddMinutes(-1)));
            }
            else if (issueDateFrom != DateTime.MinValue)
            {
                sqlQuery = sqlQuery.Where(x => x.IssueDate > _tenantUtil.DateTimeToUtc(issueDateFrom));
            }
            else if (issueDateTo != DateTime.MinValue)
            {
                sqlQuery = sqlQuery.Where(x => x.IssueDate < _tenantUtil.DateTimeToUtc(issueDateTo.AddDays(1).AddMinutes(-1)));
            }

            if (dueDateFrom != DateTime.MinValue && dueDateTo != DateTime.MinValue)
            {
                sqlQuery = sqlQuery.Where(x => x.DueDate >= _tenantUtil.DateTimeToUtc(dueDateFrom) && x.DueDate <= _tenantUtil.DateTimeToUtc(dueDateTo.AddDays(1).AddMinutes(-1)));
            }
            else if (dueDateFrom != DateTime.MinValue)
            {
                sqlQuery = sqlQuery.Where(x => x.DueDate > _tenantUtil.DateTimeToUtc(dueDateFrom));
            }
            else if (dueDateTo != DateTime.MinValue)
            {
                sqlQuery = sqlQuery.Where(x => x.DueDate < _tenantUtil.DateTimeToUtc(dueDateTo.AddDays(1).AddMinutes(-1)));
            }

            if (!String.IsNullOrEmpty(currency))
            {
                sqlQuery = sqlQuery.Where(x => x.Currency == currency);
            }

            return sqlQuery;
        }

        private bool hasParams(
                                String searchText,
                                InvoiceStatus? status,
                                DateTime issueDateFrom,
                                DateTime issueDateTo,
                                DateTime dueDateFrom,
                                DateTime dueDateTo,
                                EntityType entityType,
                                int entityID,
                                String currency)
        {
            return !(String.IsNullOrEmpty(searchText) && !status.HasValue &&
                issueDateFrom == DateTime.MinValue && issueDateTo == DateTime.MinValue &&
                dueDateFrom == DateTime.MinValue && dueDateTo == DateTime.MinValue &&
                entityID == 0 && String.IsNullOrEmpty(currency));
        }

        /// <summary>
        /// Test method
        /// </summary>
        /// <param name="id"></param>
        /// <param name="creationDate"></param>
        public void SetInvoiceCreationDate(int id, DateTime creationDate)
        {
            var dbEntity = CrmDbContext.Invoices.Find(id);

            var entity = _mapper.Map<Invoice>(dbEntity);

            dbEntity.CreateOn = _tenantUtil.DateTimeToUtc(creationDate);

            CrmDbContext.SaveChanges();

            // Delete relative  keys
            _cache.Remove(new Regex(TenantID.ToString(CultureInfo.InvariantCulture) + "invoice.*"));
        }

        /// <summary>
        /// Test method
        /// </summary>
        /// <param name="id"></param>
        /// <param name="lastModifedDate"></param>
        public void SetInvoiceLastModifedDate(int id, DateTime lastModifedDate)
        {
            var dbEntity = CrmDbContext.Invoices.Find(id);

            var entity = _mapper.Map<Invoice>(dbEntity);

            _crmSecurity.DemandAccessTo(entity);

            dbEntity.LastModifedOn = _tenantUtil.DateTimeToUtc(lastModifedDate);

            CrmDbContext.SaveChanges();

            // Delete relative  keys
            _cache.Remove(new Regex(TenantID.ToString(CultureInfo.InvariantCulture) + "invoice.*"));
        }
    }

}