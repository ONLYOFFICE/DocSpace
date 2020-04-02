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
using ASC.Web.CRM.Classes;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace ASC.CRM.Core.Dao
{
    public class CachedInvoiceItemDao : InvoiceItemDao
    {
        private readonly HttpRequestDictionary<InvoiceItem> _invoiceItemCache;

        public CachedInvoiceItemDao(DbContextManager<CRMDbContext> dbContextManager,
                TenantManager tenantManager,
                SecurityContext securityContext,
                TenantUtil tenantUtil,
                CRMSecurity cRMSecurity,
                IHttpContextAccessor httpContextAccessor,
                IOptionsMonitor<ILog> logger
            ) : base(dbContextManager,
                 tenantManager,
                 securityContext,
                 tenantUtil,
                 cRMSecurity,
                 logger)

        {
            _invoiceItemCache = new HttpRequestDictionary<InvoiceItem>(httpContextAccessor?.HttpContext, "crm_invoice_item");
        }

        public override InvoiceItem GetByID(int invoiceItemID)
        {
            return _invoiceItemCache.Get(invoiceItemID.ToString(CultureInfo.InvariantCulture), () => GetByIDBase(invoiceItemID));
        }

        private InvoiceItem GetByIDBase(int invoiceItemID)
        {
            return base.GetByID(invoiceItemID);
        }

        public override InvoiceItem SaveOrUpdateInvoiceItem(InvoiceItem invoiceItem)
        {
            if (invoiceItem != null && invoiceItem.ID > 0)
                ResetCache(invoiceItem.ID);

            return base.SaveOrUpdateInvoiceItem(invoiceItem);
        }

        public override InvoiceItem DeleteInvoiceItem(int invoiceItemID)
        {
            ResetCache(invoiceItemID);

            return base.DeleteInvoiceItem(invoiceItemID);
        }

        private void ResetCache(int invoiceItemID)
        {
            _invoiceItemCache.Reset(invoiceItemID.ToString(CultureInfo.InvariantCulture));
        }
    }

    public class InvoiceItemDao : AbstractDao
    {
        public InvoiceItemDao(
                DbContextManager<CRMDbContext> dbContextManager,
                TenantManager tenantManager,
                SecurityContext securityContext,
                TenantUtil tenantUtil,
                CRMSecurity cRMSecurity,
                IOptionsMonitor<ILog> logger
            ) : base(dbContextManager,
                 tenantManager,
                 securityContext,
                 logger)
        {
            TenantUtil = tenantUtil;
            CRMSecurity = cRMSecurity;
        }

        public CRMSecurity CRMSecurity { get; }

        public TenantUtil TenantUtil { get; }

        public Boolean IsExist(int invoiceItemID)
        {
            return IsExistInDb(invoiceItemID);
        }

        public Boolean IsExistInDb(int invoiceItemID)
        {
            return Query(CRMDbContext.InvoiceItem).Any(x => x.Id == invoiceItemID);
        }

        public Boolean CanDelete(int invoiceItemID)
        {
            return Query(CRMDbContext.InvoiceLine).Any(x => x.InvoiceItemId == invoiceItemID);
        }

        public virtual List<InvoiceItem> GetAll()
        {
            return GetAllInDb();
        }
        public virtual List<InvoiceItem> GetAllInDb()
        {
            return Db.ExecuteList(GetInvoiceItemSqlQuery(null)).ConvertAll(ToInvoiceItem);
        }

        public virtual List<InvoiceItem> GetByID(int[] ids)
        {
            return Db.ExecuteList(GetInvoiceItemSqlQuery(Exp.In("id", ids))).ConvertAll(ToInvoiceItem);
        }

        public virtual InvoiceItem GetByID(int id)
        {
            var invoiceItems = Db.ExecuteList(GetInvoiceItemSqlQuery(Exp.Eq("id", id))).ConvertAll(ToInvoiceItem);

            return invoiceItems.Count > 0 ? invoiceItems[0] : null;
        }

        public List<InvoiceItem> GetInvoiceItems(IEnumerable<int> ids)
        {
            if (ids == null || !ids.Any()) return new List<InvoiceItem>();

            var sqlQuery = GetInvoiceItemSqlQuery(Exp.In("id", ids.ToArray()));

            return Db.ExecuteList(sqlQuery).ConvertAll(ToInvoiceItem);
        }

        public List<InvoiceItem> GetInvoiceItems(
                                string searchText,
                                int status,
                                bool? inventoryStock,
                                int from,
                                int count,
                                OrderBy orderBy)
        {

            var sqlQuery = GetInvoiceItemSqlQuery(null);

            var withParams = !(String.IsNullOrEmpty(searchText) || status != 0 || inventoryStock.HasValue);


            var whereConditional = WhereConditional(new List<int>(), searchText, status, inventoryStock);
            // WhereConditional(CRMSecurity.GetPrivateItems(typeof(Invoice)).ToList(), searchText);

            if (withParams && whereConditional == null)
                return new List<InvoiceItem>();

            sqlQuery.Where(whereConditional);

            if (0 < from && from < int.MaxValue) sqlQuery.SetFirstResult(from);
            if (0 < count && count < int.MaxValue) sqlQuery.SetMaxResults(count);

            if (orderBy != null && Enum.IsDefined(typeof(InvoiceItemSortedByType), orderBy.SortedBy))
            {
                switch ((InvoiceItemSortedByType)orderBy.SortedBy)
                {
                    case InvoiceItemSortedByType.Name:
                        sqlQuery.OrderBy("title", orderBy.IsAsc);
                        break;
                    case InvoiceItemSortedByType.SKU:
                        sqlQuery.OrderBy("stock_keeping_unit", orderBy.IsAsc);
                        break;
                    case InvoiceItemSortedByType.Price:
                        sqlQuery.OrderBy("price", orderBy.IsAsc);
                        break;
                    case InvoiceItemSortedByType.Quantity:
                        sqlQuery.OrderBy("stock_quantity", orderBy.IsAsc).OrderBy("title", true);
                        break;
                    case InvoiceItemSortedByType.Created:
                        sqlQuery.OrderBy("create_on", orderBy.IsAsc);
                        break;
                    default:
                        sqlQuery.OrderBy("title", true);
                        break;
                }
            }
            else
            {
                sqlQuery.OrderBy("title", true);
            }

            return Db.ExecuteList(sqlQuery).ConvertAll(ToInvoiceItem);
        }


        public int GetInvoiceItemsCount()
        {
            return GetInvoiceItemsCount(String.Empty, 0, null);
        }

        public int GetInvoiceItemsCount(
                                    String searchText,
                                    int status,
                                    bool? inventoryStock)
        {
            var cacheKey = TenantID.ToString(CultureInfo.InvariantCulture) +
                           "invoiceItem" +
                           SecurityContext.CurrentAccount.ID.ToString() +
                           searchText;

            var fromCache = _cache.Get<string>(cacheKey);

            if (fromCache != null) return Convert.ToInt32(fromCache);

            var withParams = !(String.IsNullOrEmpty(searchText) || status != 0 || inventoryStock.HasValue);

            var exceptIDs = CRMSecurity.GetPrivateItems(typeof(InvoiceItem)).ToList();

            int result;

            if (withParams)
            {
                var whereConditional = WhereConditional(exceptIDs, searchText, status, inventoryStock);
                result = whereConditional != null ? Db.ExecuteScalar<int>(Query("crm_invoice_item").Where(whereConditional).SelectCount()) : 0;
            }
            else
            {
                var countWithoutPrivate = Db.ExecuteScalar<int>(Query("crm_invoice_item").SelectCount());
                var privateCount = exceptIDs.Count;

                if (privateCount > countWithoutPrivate)
                {
                    _log.ErrorFormat(@"Private invoice items count more than all cases. Tenant: {0}. CurrentAccount: {1}",
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

        public virtual InvoiceItem SaveOrUpdateInvoiceItem(InvoiceItem invoiceItem)
        {
            /*_cache.Remove(_invoiceItemCacheKey);
            _cache.Insert(_invoiceItemCacheKey, String.Empty);*/

            return SaveOrUpdateInvoiceItemInDb(invoiceItem);
        }

        private InvoiceItem SaveOrUpdateInvoiceItemInDb(InvoiceItem invoiceItem)
        {
            if (invoiceItem.Price <= 0 || String.IsNullOrEmpty(invoiceItem.Title))
                throw new ArgumentException();

            if (invoiceItem.Price > Global.MaxInvoiceItemPrice)
                throw new ArgumentException("Max Invoice Item Price: " + Global.MaxInvoiceItemPrice);

            if (!CRMSecurity.IsAdmin) CRMSecurity.CreateSecurityException();

            if (String.IsNullOrEmpty(invoiceItem.Description))
            {
                invoiceItem.Description = String.Empty;
            }
            if (String.IsNullOrEmpty(invoiceItem.StockKeepingUnit))
            {
                invoiceItem.StockKeepingUnit = String.Empty;
            }

            if (!IsExistInDb(invoiceItem.ID))
            {
                invoiceItem.ID = Db.ExecuteScalar<int>(
                               Insert("crm_invoice_item")
                              .InColumnValue("id", 0)
                              .InColumnValue("title", invoiceItem.Title)
                              .InColumnValue("description", invoiceItem.Description)
                              .InColumnValue("stock_keeping_unit", invoiceItem.StockKeepingUnit)
                              .InColumnValue("price", invoiceItem.Price)
                              .InColumnValue("stock_quantity", invoiceItem.StockQuantity)
                              .InColumnValue("track_inventory", invoiceItem.TrackInventory)
                              .InColumnValue("invoice_tax1_id", invoiceItem.InvoiceTax1ID)
                              .InColumnValue("invoice_tax2_id", invoiceItem.InvoiceTax2ID)
                              .InColumnValue("currency", String.Empty)
                              .InColumnValue("create_on", DateTime.UtcNow)
                              .InColumnValue("create_by", SecurityContext.CurrentAccount.ID)
                              .InColumnValue("last_modifed_on", DateTime.UtcNow)
                              .InColumnValue("last_modifed_by", SecurityContext.CurrentAccount.ID)
                              .Identity(1, 0, true));

                invoiceItem.CreateOn = DateTime.UtcNow;
                invoiceItem.LastModifedOn = invoiceItem.CreateOn;
                invoiceItem.CreateBy = SecurityContext.CurrentAccount.ID;
                invoiceItem.LastModifedBy = invoiceItem.CreateBy;
            }
            else
            {

                var oldInvoiceItem = Db.ExecuteList(GetInvoiceItemSqlQuery(Exp.Eq("id", invoiceItem.ID)))
                    .ConvertAll(ToInvoiceItem)
                    .FirstOrDefault();

                CRMSecurity.DemandEdit(oldInvoiceItem);

                Db.ExecuteNonQuery(
                    Update("crm_invoice_item")
                        .Set("title", invoiceItem.Title)
                        .Set("description", invoiceItem.Description)
                        .Set("stock_keeping_unit", invoiceItem.StockKeepingUnit)
                        .Set("price", invoiceItem.Price)
                        .Set("stock_quantity", invoiceItem.StockQuantity)
                        .Set("track_inventory", invoiceItem.TrackInventory)
                        .Set("invoice_tax1_id", invoiceItem.InvoiceTax1ID)
                        .Set("invoice_tax2_id", invoiceItem.InvoiceTax2ID)
                        .Set("currency", String.Empty)
                        .Set("last_modifed_on", DateTime.UtcNow)
                        .Set("last_modifed_by", SecurityContext.CurrentAccount.ID)
                        .Where(Exp.Eq("id", invoiceItem.ID)));

                invoiceItem.LastModifedOn = DateTime.UtcNow;
                invoiceItem.LastModifedBy = SecurityContext.CurrentAccount.ID;
            }

            return invoiceItem;
        }

        public virtual InvoiceItem DeleteInvoiceItem(int invoiceItemID)
        {
            var invoiceItem = GetByID(invoiceItemID);
            if (invoiceItem == null) return null;

            CRMSecurity.DemandDelete(invoiceItem);

            CRMDbContext.Remove(new DbInvoiceItem
            {
                Id = invoiceItemID,
                TenantId = TenantID
            });

            CRMDbContext.SaveChanges();

            /*_cache.Remove(_invoiceItemCacheKey);
            _cache.Insert(_invoiceItemCacheKey, String.Empty);*/

            return invoiceItem;
        }

        public virtual List<InvoiceItem> DeleteBatchInvoiceItems(int[] invoiceItemIDs)
        {
            if (invoiceItemIDs == null || !invoiceItemIDs.Any()) return null;

            var items = GetInvoiceItems(invoiceItemIDs).Where(CRMSecurity.CanDelete).ToList();

            if (!items.Any()) return items;

            // Delete relative  keys
            /*_cache.Remove(_invoiceItemCacheKey);
            _cache.Insert(_invoiceItemCacheKey, String.Empty);*/

            DeleteBatchItemsExecute(items);

            return items;
        }

        private void DeleteBatchItemsExecute(List<InvoiceItem> items)
        {
            CRMDbContext.RemoveRange(items.ConvertAll(x => new DbInvoiceItem
                                        {
                                            Id = x.ID,
                                            TenantId = TenantID
                                        }));

            CRMDbContext.SaveChanges();
        }

        private InvoiceItem ToInvoiceItem(DbInvoiceItem dbInvoiceItem)
        {
            var result = new InvoiceItem
            {
                ID = dbInvoiceItem.Id,
                Title = dbInvoiceItem.Title,
                Description = dbInvoiceItem.Description,
                StockKeepingUnit = dbInvoiceItem.StockKeepingUnit,
                Price = dbInvoiceItem.Price,
                StockQuantity = dbInvoiceItem.StockQuantity,
                TrackInventory = dbInvoiceItem.TrackInventory,
                InvoiceTax1ID = dbInvoiceItem.InvoiceTax1Id,
                InvoiceTax2ID = dbInvoiceItem.InvoiceTax2Id,
                Currency = dbInvoiceItem.Currency,
                CreateOn = TenantUtil.DateTimeFromUtc(dbInvoiceItem.CreateOn),
                CreateBy = dbInvoiceItem.CreateBy,
                LastModifedBy = dbInvoiceItem.LastModifedBy
            };

            if (result.LastModifedOn.HasValue)
                result.LastModifedOn = TenantUtil.DateTimeFromUtc(dbInvoiceItem.LastModifedOn.Value);

            return result;

        }

        //private SqlQuery GetInvoiceItemSqlQuery(Exp where)
        //{
        //    var sqlQuery = Query("crm_invoice_item")
        //        .Select(
        //            "id",
        //            "title",
        //            "description",
        //            "stock_keeping_unit",
        //            "price",
        //            "stock_quantity",
        //            "track_inventory",
        //            "invoice_tax1_id",
        //            "invoice_tax2_id",
        //            "currency",
        //            "create_on",
        //            "create_by",
        //            "last_modifed_on",
        //            "last_modifed_by");

        //    if (where != null)
        //    {
        //        sqlQuery.Where(where);
        //    }

        //    return sqlQuery;
        //}

        private Exp WhereConditional(
                                ICollection<int> exceptIDs,
                                String searchText,
                                int status,
                                bool? inventoryStock)
        {
            var conditions = new List<Exp>();

            //if (status != null)
            //{
            //    conditions.Add(Exp.Eq("status", (int)status.Value));
            //}


            if (!String.IsNullOrEmpty(searchText))
            {
                searchText = searchText.Trim();

                var keywords = searchText.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries)
                   .ToArray();

                if (keywords.Length > 0)
                    //if (FullTextSearch.SupportModule(FullTextSearch.CRMInvoiceItemModule))
                    //{
                    //    ids = FullTextSearch.Search(searchText, FullTextSearch.CRMInvoiceItemModule)
                    //        .GetIdentifiers()
                    //        .Select(item => Convert.ToInt32(item.Split('_')[1])).Distinct().ToList();

                    //    if (ids.Count == 0) return null;
                    //}
                    //else
                    conditions.Add(BuildLike(new[] { "title", "description", "stock_keeping_unit" }, keywords));
            }

            if (exceptIDs.Count > 0)
            {
                conditions.Add(!Exp.In("id", exceptIDs.ToArray()));
            }


            if (inventoryStock.HasValue)
            {
                conditions.Add(Exp.Eq("track_inventory", inventoryStock.Value));
            }

            if (conditions.Count == 0) return null;

            return conditions.Count == 1 ? conditions[0] : conditions.Aggregate((i, j) => i & j);
        }

    }
}