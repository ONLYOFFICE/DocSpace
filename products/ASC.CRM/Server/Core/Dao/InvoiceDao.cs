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
using ASC.Core.Common.Settings;
using ASC.Core.Tenants;
using ASC.CRM.Core.EF;
using ASC.CRM.Core.Entities;
using ASC.CRM.Core.Enums;
using ASC.ElasticSearch;
using ASC.Web.CRM.Classes;
using ASC.Web.CRM.Core.Search;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Linq.Expressions;
using System.Text.RegularExpressions;
using SecurityContext = ASC.Core.SecurityContext;

namespace ASC.CRM.Core.Dao
{
    public class CachedInvoiceDao : InvoiceDao
    {
        private readonly HttpRequestDictionary<Invoice> _invoiceCache;

        public CachedInvoiceDao(DbContextManager<CRMDbContext> dbContextManager,
            TenantManager tenantManager,
            SecurityContext securityContext,
            FactoryIndexer<InvoicesWrapper> factoryIndexer,
            IHttpContextAccessor httpContextAccessor,
            IOptionsMonitor<ILog> logger,
            SettingsManager settingsManager,
            InvoiceSetting invoiceSetting)
              : base(dbContextManager,
                 tenantManager,
                 securityContext,
                 factoryIndexer,
                 logger,
                 settingsManager,
                 invoiceSetting
                 )
        {
            _invoiceCache = new HttpRequestDictionary<Invoice>(httpContextAccessor?.HttpContext, "crm_invoice");
        }

        public override Invoice GetByID(int invoiceID)
        {
            return _invoiceCache.Get(invoiceID.ToString(CultureInfo.InvariantCulture), () => GetByIDBase(invoiceID));
        }

        private Invoice GetByIDBase(int invoiceID)
        {
            return base.GetByID(invoiceID);
        }

        public override int SaveOrUpdateInvoice(Invoice invoice)
        {
            if (invoice != null && invoice.ID > 0)
                ResetCache(invoice.ID);

            return base.SaveOrUpdateInvoice(invoice);
        }

        public override Invoice DeleteInvoice(int invoiceID)
        {
            ResetCache(invoiceID);

            return base.DeleteInvoice(invoiceID);
        }

        private void ResetCache(int invoiceID)
        {
            _invoiceCache.Reset(invoiceID.ToString(CultureInfo.InvariantCulture));
        }
    }

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

        public InvoiceDao(
            DbContextManager<CRMDbContext> dbContextManager,
            TenantManager tenantManager,
            SecurityContext securityContext,
            FactoryIndexer<InvoicesWrapper> factoryIndexer,
            IOptionsMonitor<ILog> logger,
            SettingsManager settingsManager,
            InvoiceSetting invoiceSetting)
              : base(dbContextManager,
                 tenantManager,
                 securityContext,
                 logger)
        {
            FactoryIndexer = factoryIndexer;
            SettingsManager = settingsManager;
        }

        public InvoiceSetting InvoiceSetting { get; }

        public  SettingsManager SettingsManager { get; }

        public FactoryIndexer<InvoicesWrapper> FactoryIndexer { get; }

        public TenantUtil TenantUtil { get; }
        public CRMSecurity CRMSecurity { get; }

        public Boolean IsExist(int invoiceID)
        {
            return IsExistFromDb(invoiceID);
        }

        public Boolean IsExistFromDb(int invoiceID)
        {
            return Query(CRMDbContext.Invoices).Where(x => x.Id == invoiceID).Any();
        }

        public Boolean IsExist(string number)
        {
            return IsExistFromDb(number);
        }

        public Boolean IsExistFromDb(string number)
        {
            return Query(CRMDbContext.Invoices)
                .Where(x => x.Number == number)
                .Any();
        }

        public virtual List<Invoice> GetAll()
        {
            return Query(CRMDbContext.Invoices)
                .ToList()
                .ConvertAll(ToInvoice);
        }

        public virtual List<Invoice> GetByID(int[] ids)
        {
            return Query(CRMDbContext.Invoices)
                        .Where(x => ids.Contains(x.Id))
                        .ToList()
                        .ConvertAll(ToInvoice);
        }

        public virtual Invoice GetByID(int id)
        {
            return GetByIDFromDb(id);
        }

        public virtual Invoice GetByIDFromDb(int id)
        {
            return ToInvoice(Query(CRMDbContext.Invoices).FirstOrDefault(x => x.Id == id));
        }

        public Invoice GetByNumber(string number)
        {
            return ToInvoice(Query(CRMDbContext.Invoices).FirstOrDefault(x => x.Number == number));
        }

        public Invoice GetByFileId(Int32 fileID)
        {
            return ToInvoice(Query(CRMDbContext.Invoices)
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

            if (CRMSecurity.IsAdmin)
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

            if (crudeInvoices.Count < from + count) return CRMSecurity.FilterRead(crudeInvoices).Skip(from).ToList();

            var result = CRMSecurity.FilterRead(crudeInvoices).ToList();

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

                result.AddRange(CRMSecurity.FilterRead(crudeInvoices));

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

                        var subSqlQuery = sqlQuery.GroupJoin(CRMDbContext.Contacts,
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

            return sqlQuery.ToList().ConvertAll(ToInvoice);
        }

        public int GetAllInvoicesCount()
        {
            return Query(CRMDbContext.Invoices).Count();
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
                           SecurityContext.CurrentAccount.ID.ToString() +
                           searchText;

            var fromCache = _cache.Get<string>(cacheKey);

            if (fromCache != null) return Convert.ToInt32(fromCache);

            var withParams = hasParams(searchText, status, issueDateFrom, issueDateTo, dueDateFrom, dueDateTo, entityType, entityID, currency);

            var exceptIDs = CRMSecurity.GetPrivateItems(typeof(Invoice)).ToList();

            int result;

            if (withParams)
            {
                var sqlQuery = GetDbInvoceByFilters(exceptIDs, searchText, status, issueDateFrom, issueDateTo, dueDateFrom, dueDateTo, entityType, entityID, currency);
                
                result = sqlQuery != null ? sqlQuery.Count() : 0;

            }
            else
            {
                var countWithoutPrivate = Query(CRMDbContext.Invoices).Count();

                var privateCount = exceptIDs.Count;

                if (privateCount > countWithoutPrivate)
                {
                    Logger.ErrorFormat(@"Private invoice count more than all cases. Tenant: {0}. CurrentAccount: {1}",
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

        public List<Invoice> GetInvoices(int[] ids)
        {
            if (ids == null || !ids.Any()) return new List<Invoice>();

            return Query(CRMDbContext.Invoices)
                .Where(x => ids.Contains(x.Id))
                .ToList()
                .ConvertAll(ToInvoice)
                .FindAll(CRMSecurity.CanAccessTo);
        }

        public List<Invoice> GetEntityInvoices(EntityType entityType, int entityID)
        {
            var result = new List<Invoice>();
            if (entityID <= 0)
                return result;

            if (entityType == EntityType.Opportunity)
            {
                return Query(CRMDbContext.Invoices).Where(x => x.EntityId == entityID && x.EntityType == entityType)
                    .ToList()
                    .ConvertAll(ToInvoice)
                    .FindAll(CRMSecurity.CanAccessTo);
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

            return Query(CRMDbContext.Invoices).Where(x => x.ContactId == contactID)
                    .ToList()
                    .ConvertAll(ToInvoice)
                    .FindAll(CRMSecurity.CanAccessTo);
        }

        public string GetNewInvoicesNumber()
        {
            var settings = GetSettings();

            if (!settings.Autogenerated)
                return string.Empty;

            var stringNumber = Query(CRMDbContext.Invoices).OrderByDescending(x => x.Id).Select(x => x.Number).ToString();

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
            var tenantSettings = SettingsManager.Load<CRMSettings>();

            return tenantSettings.InvoiceSetting ?? InvoiceSetting.DefaultSettings;
        }

        public virtual int SaveOrUpdateInvoice(Invoice invoice)
        {
            _cache.Remove(new Regex(TenantID.ToString(CultureInfo.InvariantCulture) + "invoice.*"));

            var result = SaveOrUpdateInvoiceInDb(invoice);

            FactoryIndexer.IndexAsync(invoice);

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


            invoice.PurchaseOrderNumber = !String.IsNullOrEmpty(invoice.PurchaseOrderNumber) ? invoice.PurchaseOrderNumber : String.Empty;

            if (!IsExistFromDb(invoice.ID))
            {
                if (IsExistFromDb(invoice.Number))
                    throw new ArgumentException();

                var itemToInsert = new DbInvoice
                {
                    Status = invoice.Status,
                    Number = invoice.Number,
                    IssueDate = TenantUtil.DateTimeToUtc(invoice.IssueDate),
                    TemplateType = invoice.TemplateType,
                    ContactId = invoice.ContactID,
                    ConsigneeId = invoice.ConsigneeID,
                    EntityType = invoice.EntityType,
                    EntityId = invoice.EntityID,
                    DueDate = TenantUtil.DateTimeToUtc(invoice.DueDate),
                    Language = invoice.Language,
                    Currency = invoice.Currency,
                    ExchangeRate = invoice.ExchangeRate,
                    PurchaseOrderNumber = invoice.PurchaseOrderNumber,
                    Terms = invoice.Terms,
                    Description = invoice.Description,
                    JsonData = invoice.JsonData,
                    FileId = invoice.FileID,
                    CreateOn = invoice.CreateOn == DateTime.MinValue ? DateTime.UtcNow : invoice.CreateOn,
                    CreateBy = SecurityContext.CurrentAccount.ID,
                    LastModifedOn = invoice.CreateOn == DateTime.MinValue ? DateTime.UtcNow : invoice.CreateOn,
                    LastModifedBy = SecurityContext.CurrentAccount.ID,
                    TenantId = TenantID
                };

                CRMDbContext.Invoices.Add(itemToInsert);

                CRMDbContext.SaveChanges();

                invoice.ID = itemToInsert.Id;

            }
            else
            {
                var itemToUpdate = Query(CRMDbContext.Invoices).FirstOrDefault(x => x.Id == invoice.ID);

                var oldInvoice = ToInvoice(itemToUpdate);

                CRMSecurity.DemandEdit(oldInvoice);

                itemToUpdate.Status = invoice.Status;
                itemToUpdate.IssueDate = TenantUtil.DateTimeToUtc(invoice.IssueDate);
                itemToUpdate.TemplateType = invoice.TemplateType;
                itemToUpdate.ContactId = invoice.ContactID;
                itemToUpdate.ConsigneeId = invoice.ConsigneeID;
                itemToUpdate.EntityType = invoice.EntityType;
                itemToUpdate.EntityId = invoice.EntityID;
                itemToUpdate.DueDate = TenantUtil.DateTimeToUtc(invoice.DueDate);
                itemToUpdate.Language = invoice.Language;
                itemToUpdate.Currency = invoice.Currency;
                itemToUpdate.ExchangeRate = invoice.ExchangeRate;
                itemToUpdate.PurchaseOrderNumber = invoice.PurchaseOrderNumber;
                itemToUpdate.Description = invoice.Description;
                itemToUpdate.JsonData = null;
                itemToUpdate.FileId = 0;
                itemToUpdate.LastModifedOn = DateTime.UtcNow;
                itemToUpdate.LastModifedBy = SecurityContext.CurrentAccount.ID;
                itemToUpdate.TenantId = TenantID;

                CRMDbContext.SaveChanges();


            }

            return invoice.ID;
        }

        public virtual Invoice UpdateInvoiceStatus(int invoiceid, InvoiceStatus status)
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
                Logger.Error("Invoice not found");

                return null;
            }
            CRMSecurity.DemandAccessTo(invoice);

            if (!invoiceStatusMap.Contains(new KeyValuePair<InvoiceStatus, InvoiceStatus>(invoice.Status, status)))
            {
                Logger.ErrorFormat("Status for invoice with ID={0} can't be changed. Return without changes", invoiceid);
                
                return invoice;
            }

            var itemToUpdate = Query(CRMDbContext.Invoices).FirstOrDefault(x => x.Id == invoiceid);

            itemToUpdate.Status = status;
            itemToUpdate.LastModifedOn = DateTime.UtcNow;
            itemToUpdate.LastModifedBy = SecurityContext.CurrentAccount.ID;

            CRMDbContext.Update(itemToUpdate);

            CRMDbContext.SaveChanges();

            invoice.Status = status;

            return invoice;

        }

        public virtual int UpdateInvoiceJsonData(int invoiceId, string jsonData)
        {
            return UpdateInvoiceJsonDataInDb(invoiceId, jsonData);
        }

        private int UpdateInvoiceJsonDataInDb(int invoiceId, string jsonData)
        {
            var itemToUpdate = Query(CRMDbContext.Invoices).FirstOrDefault(x => x.Id == invoiceId);

            itemToUpdate.JsonData = jsonData;
            itemToUpdate.LastModifedOn = DateTime.UtcNow;
            itemToUpdate.LastModifedBy = SecurityContext.CurrentAccount.ID;

            CRMDbContext.Update(itemToUpdate);
            CRMDbContext.SaveChanges();

            return invoiceId;
        }

        public void UpdateInvoiceJsonData(Invoice invoice, int billingAddressID, int deliveryAddressID)
        {
            var jsonData = InvoiceFormattedData.GetData(invoice, billingAddressID, deliveryAddressID);
            if (jsonData.LogoBase64Id != 0)
            {
                jsonData.LogoBase64 = null;
            }
            invoice.JsonData = JsonConvert.SerializeObject(jsonData);
            UpdateInvoiceJsonData(invoice.ID, invoice.JsonData);
        }

        public void UpdateInvoiceJsonDataAfterLinesUpdated(Invoice invoice)
        {
            var jsonData = InvoiceFormattedData.GetDataAfterLinesUpdated(invoice);
            if (jsonData.LogoBase64Id != 0)
            {
                jsonData.LogoBase64 = null;
            }
            UpdateInvoiceJsonData(invoice.ID, invoice.JsonData);
        }

        public virtual int UpdateInvoiceFileID(int invoiceId, int fileId)
        {
            return UpdateInvoiceFileIDInDb(invoiceId, fileId);
        }

        private int UpdateInvoiceFileIDInDb(int invoiceId, int fileId)
        {

            var sqlToUpdate = Query(CRMDbContext.Invoices).FirstOrDefault(x => x.Id == invoiceId);

            sqlToUpdate.FileId = fileId;
            sqlToUpdate.LastModifedOn = DateTime.UtcNow;
            sqlToUpdate.LastModifedBy = SecurityContext.CurrentAccount.ID;

            CRMDbContext.Update(sqlToUpdate);
            CRMDbContext.SaveChanges();

            return invoiceId;
        }

        public InvoiceSetting SaveInvoiceSettings(InvoiceSetting invoiceSetting)
        {
            var tenantSettings = SettingsManager.Load<CRMSettings>(); 
            tenantSettings.InvoiceSetting = invoiceSetting;
       
            SettingsManager.Save<CRMSettings>(tenantSettings);

            return tenantSettings.InvoiceSetting;
        }

        public virtual Invoice DeleteInvoice(int invoiceID)
        {
            if (invoiceID <= 0) return null;

            var invoice = GetByID(invoiceID);
            if (invoice == null) return null;

            CRMSecurity.DemandDelete(invoice);

            // Delete relative  keys
            _cache.Remove(new Regex(TenantID.ToString(CultureInfo.InvariantCulture) + "invoice.*"));

            DeleteBatchInvoicesExecute(new List<Invoice> { invoice });

            return invoice;
        }

        public List<Invoice> DeleteBatchInvoices(int[] invoiceID)
        {
            var invoices = GetInvoices(invoiceID).Where(CRMSecurity.CanDelete).ToList();
            if (!invoices.Any()) return invoices;

            // Delete relative  keys
            _cache.Remove(new Regex(TenantID.ToString(CultureInfo.InvariantCulture) + "invoice.*"));

            DeleteBatchInvoicesExecute(invoices);

            return invoices;
        }

        private void DeleteBatchInvoicesExecute(List<Invoice> invoices)
        {
            var invoiceID = invoices.Select(x => x.ID).ToArray();

            using var tx = CRMDbContext.Database.BeginTransaction();

            CRMDbContext.InvoiceLine.RemoveRange(Query(CRMDbContext.InvoiceLine).Where(x => invoiceID.Contains(x.InvoiceId)));
            CRMDbContext.Invoices.RemoveRange(Query(CRMDbContext.Invoices).Where(x => invoiceID.Contains(x.Id)));

            CRMDbContext.SaveChanges();

            tx.Commit();

            invoices.ForEach(invoice => FactoryIndexer.DeleteAsync(invoice));

        }

        private Invoice ToInvoice(DbInvoice dbInvoice)
        {
            if (dbInvoice == null) return null;

            var result = new Invoice
            {
                ID = dbInvoice.Id,
                Status = dbInvoice.Status,
                Number = dbInvoice.Number,
                IssueDate = TenantUtil.DateTimeFromUtc(dbInvoice.IssueDate),
                TemplateType = dbInvoice.TemplateType,
                ContactID = dbInvoice.ContactId,
                ConsigneeID = dbInvoice.ConsigneeId,
                EntityType = dbInvoice.EntityType,
                EntityID = dbInvoice.EntityId,
                DueDate = TenantUtil.DateTimeFromUtc(dbInvoice.DueDate),
                Language = dbInvoice.Language,
                Currency = dbInvoice.Currency,
                ExchangeRate = dbInvoice.ExchangeRate,
                PurchaseOrderNumber = dbInvoice.PurchaseOrderNumber,
                Terms = dbInvoice.Terms,
                Description = dbInvoice.Description,
                JsonData = dbInvoice.JsonData,
                FileID = dbInvoice.FileId,
                CreateOn = TenantUtil.DateTimeFromUtc(dbInvoice.CreateOn),
                CreateBy = dbInvoice.CreateBy,
                LastModifedBy = dbInvoice.LastModifedBy
            };

            if (dbInvoice.LastModifedOn.HasValue)
            {
                result.LastModifedOn = TenantUtil.DateTimeFromUtc(dbInvoice.LastModifedOn.Value);
            }

            return result;
        }

        private IQueryable<DbInvoice> GetDbInvoceByFilters(
                                ICollection<int> exceptIDs,
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
            var sqlQuery = Query(CRMDbContext.Invoices);

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
                    if (!FactoryIndexer.TrySelectIds(s => s.MatchAll(searchText), out invoicesIds))
                    {
                        foreach (var k in keywords)
                        {
                            sqlQuery = sqlQuery.Where(x => Microsoft.EntityFrameworkCore.EF.Functions.Like(x.Number, k) ||
                                                             Microsoft.EntityFrameworkCore.EF.Functions.Like(x.Description, k));
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
                sqlQuery = sqlQuery.Where(x => x.IssueDate >= TenantUtil.DateTimeToUtc(issueDateFrom) && x.DueDate <= TenantUtil.DateTimeToUtc(issueDateTo.AddDays(1).AddMinutes(-1)));
            }
            else if (issueDateFrom != DateTime.MinValue)
            {
                sqlQuery = sqlQuery.Where(x => x.IssueDate > TenantUtil.DateTimeToUtc(issueDateFrom));
            }
            else if (issueDateTo != DateTime.MinValue)
            {
                sqlQuery = sqlQuery.Where(x => x.IssueDate < TenantUtil.DateTimeToUtc(issueDateTo.AddDays(1).AddMinutes(-1)));
            }

            if (dueDateFrom != DateTime.MinValue && dueDateTo != DateTime.MinValue)
            {
                sqlQuery = sqlQuery.Where(x => x.DueDate >= TenantUtil.DateTimeToUtc(dueDateFrom) && x.DueDate <= TenantUtil.DateTimeToUtc(dueDateTo.AddDays(1).AddMinutes(-1)));
            }
            else if (dueDateFrom != DateTime.MinValue)
            {
                sqlQuery = sqlQuery.Where(x => x.DueDate > TenantUtil.DateTimeToUtc(dueDateFrom));
            }
            else if (dueDateTo != DateTime.MinValue)
            {
                sqlQuery = sqlQuery.Where(x => x.DueDate < TenantUtil.DateTimeToUtc(dueDateTo.AddDays(1).AddMinutes(-1)));
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
        /// <param name="invoiceId"></param>
        /// <param name="creationDate"></param>
        public void SetInvoiceCreationDate(int invoiceId, DateTime creationDate)
        {
            var itemToUpdate = Query(CRMDbContext.Invoices).FirstOrDefault(x => x.Id == invoiceId);

            itemToUpdate.CreateOn = TenantUtil.DateTimeToUtc(creationDate);

            CRMDbContext.Invoices.Update(itemToUpdate);
            CRMDbContext.SaveChanges();

            // Delete relative  keys
            _cache.Remove(new Regex(TenantID.ToString(CultureInfo.InvariantCulture) + "invoice.*"));
        }

        /// <summary>
        /// Test method
        /// </summary>
        /// <param name="invoiceId"></param>
        /// <param name="lastModifedDate"></param>
        public void SetInvoiceLastModifedDate(int invoiceId, DateTime lastModifedDate)
        {
            var itemToUpdate = Query(CRMDbContext.Invoices).FirstOrDefault(x => x.Id == invoiceId);

            itemToUpdate.LastModifedOn = TenantUtil.DateTimeToUtc(lastModifedDate);

            CRMDbContext.Invoices.Update(itemToUpdate);

            CRMDbContext.SaveChanges();

            // Delete relative  keys
            _cache.Remove(new Regex(TenantID.ToString(CultureInfo.InvariantCulture) + "invoice.*"));
        }
    }
}