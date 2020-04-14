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
using ASC.Common;
using ASC.Common.Logging;
using ASC.Core;
using ASC.Core.Common.EF;
using ASC.Core.Tenants;
using ASC.CRM.Core.EF;
using ASC.CRM.Core.Entities;
using ASC.CRM.Core.Enums;
using ASC.Web.CRM.Classes;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
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
            return Query(CRMDbContext.InvoiceItem).ToList().ConvertAll(ToInvoiceItem);
        }

        public virtual List<InvoiceItem> GetByID(int[] ids)
        {
            return Query(CRMDbContext.InvoiceItem)
                        .Where(x => ids.Contains(x.Id))
                        .ToList()
                        .ConvertAll(ToInvoiceItem);
        }

        public virtual InvoiceItem GetByID(int id)
        {
            return ToInvoiceItem(Query(CRMDbContext.InvoiceItem).FirstOrDefault(x => x.Id == id));
        }

        public List<InvoiceItem> GetInvoiceItems(IEnumerable<int> ids)
        {
            if (ids == null || !ids.Any()) return new List<InvoiceItem>();

            return Query(CRMDbContext.InvoiceItem)
                            .Where(x => ids.Contains(x.Id))
                            .ToList()
                            .ConvertAll(ToInvoiceItem);
        }

        public List<InvoiceItem> GetInvoiceItems(
                                string searchText,
                                int status,
                                bool? inventoryStock,
                                int from,
                                int count,
                                OrderBy orderBy)
        {

            var sqlQuery = GetDbInvoiceItemByFilters(new List<int>(), searchText, status, inventoryStock);

            var withParams = !(String.IsNullOrEmpty(searchText) || status != 0 || inventoryStock.HasValue);

            if (withParams && sqlQuery == null)
                return new List<InvoiceItem>();

            if (0 < from && from < int.MaxValue) sqlQuery = sqlQuery.Skip(from);
            if (0 < count && count < int.MaxValue) sqlQuery = sqlQuery.Take(count);

            if (orderBy != null && Enum.IsDefined(typeof(InvoiceItemSortedByType), orderBy.SortedBy))
            {
                switch ((InvoiceItemSortedByType)orderBy.SortedBy)
                {
                    case InvoiceItemSortedByType.Name:
                        sqlQuery = sqlQuery.OrderBy("Title", orderBy.IsAsc);
                        break;
                    case InvoiceItemSortedByType.SKU:
                        sqlQuery = sqlQuery.OrderBy("StockKeepingUnit", orderBy.IsAsc);
                        break;
                    case InvoiceItemSortedByType.Price:
                        sqlQuery = sqlQuery.OrderBy("Price", orderBy.IsAsc);
                        break;
                    case InvoiceItemSortedByType.Quantity:
                        {
                            sqlQuery = sqlQuery.OrderBy("StockQuantity", orderBy.IsAsc)
                                               .OrderBy("Title", true);
                            break;
                        }
                    case InvoiceItemSortedByType.Created:
                        sqlQuery = sqlQuery.OrderBy("CreateOn", orderBy.IsAsc);
                        break;
                    default:
                        sqlQuery = sqlQuery.OrderBy("Title", true);
                        break;
                }
            }
            else
            {
                sqlQuery = sqlQuery.OrderBy("Title", true);
            }

            return sqlQuery.ToList().ConvertAll(ToInvoiceItem);
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

            var exceptIDs = CRMSecurity.GetPrivateItems(typeof(InvoiceItem)).ToList();
            
            int result;

            var withParams = !(String.IsNullOrEmpty(searchText) || status != 0 || inventoryStock.HasValue);

            if (withParams)
            {
                result = GetDbInvoiceItemByFilters(exceptIDs, searchText, status, inventoryStock).Count();
            }
            else
            {
                var countWithoutPrivate = Query(CRMDbContext.InvoiceItem).Count();

                var privateCount = exceptIDs.Count;

                if (privateCount > countWithoutPrivate)
                {
                    Logger.ErrorFormat(@"Private invoice items count more than all cases. Tenant: {0}. CurrentAccount: {1}",
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
                var itemToInsert = new DbInvoiceItem
                {
                    Title = invoiceItem.Title,
                    Description = invoiceItem.Description,
                    StockKeepingUnit = invoiceItem.StockKeepingUnit,
                    Price = invoiceItem.Price,
                    StockQuantity = invoiceItem.StockQuantity,
                    TrackInventory = invoiceItem.TrackInventory,
                    InvoiceTax1Id = invoiceItem.InvoiceTax1ID,
                    InvoiceTax2Id = invoiceItem.InvoiceTax2ID,
                    Currency = String.Empty,
                    CreateOn = DateTime.UtcNow,
                    CreateBy = SecurityContext.CurrentAccount.ID,
                    LastModifedOn = DateTime.Now,
                    LastModifedBy = SecurityContext.CurrentAccount.ID,
                    TenantId = TenantID
                };

                CRMDbContext.Add(itemToInsert);
                CRMDbContext.SaveChanges();

                invoiceItem.ID = itemToInsert.Id;



            }
            else
            {

                var itemToUpdate = Query(CRMDbContext.InvoiceItem).Single(x => x.Id == invoiceItem.ID);
                var oldInvoiceItem = ToInvoiceItem(itemToUpdate);

                CRMSecurity.DemandEdit(oldInvoiceItem);

                itemToUpdate.Title = invoiceItem.Title;
                itemToUpdate.Description = invoiceItem.Description;
                itemToUpdate.StockKeepingUnit = invoiceItem.StockKeepingUnit;
                itemToUpdate.Price = invoiceItem.Price;
                itemToUpdate.StockQuantity = invoiceItem.StockQuantity;
                itemToUpdate.TrackInventory = invoiceItem.TrackInventory;
                itemToUpdate.InvoiceTax1Id = invoiceItem.InvoiceTax1ID;
                itemToUpdate.InvoiceTax2Id = invoiceItem.InvoiceTax2ID;

                itemToUpdate.Currency = invoiceItem.Currency;
                itemToUpdate.LastModifedOn = invoiceItem.LastModifedOn;
                itemToUpdate.LastModifedBy = SecurityContext.CurrentAccount.ID;

                CRMDbContext.Add(itemToUpdate);
                CRMDbContext.SaveChanges();


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
            if (dbInvoiceItem == null) return null;

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

        private IQueryable<DbInvoiceItem> GetDbInvoiceItemByFilters(
                                ICollection<int> exceptIDs,
                                String searchText,
                                int status,
                                bool? inventoryStock)
        {

            var sqlQuery = Query(CRMDbContext.InvoiceItem);

            //if (status > 0)
            //{
            //    sqlQuery = sqlQuery.Where(x => x.);
            //    conditions.Add(Exp.Eq("status", (int)status.Value));
            //}

            if (!String.IsNullOrEmpty(searchText))
            {
                searchText = searchText.Trim();

                var keywords = searchText.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries)
                   .ToArray();

                if (keywords.Length > 0)
                {
                    foreach (var k in keywords)
                    {
                        sqlQuery = sqlQuery.Where(x => Microsoft.EntityFrameworkCore.EF.Functions.Like(x.Title, k + "%") ||
                                                       Microsoft.EntityFrameworkCore.EF.Functions.Like(x.Description, k + "%") ||
                                                       Microsoft.EntityFrameworkCore.EF.Functions.Like(x.StockKeepingUnit, k + "%")
                                                       );
                    }
                }
            }

            if (exceptIDs.Count > 0)
            {
                sqlQuery = sqlQuery.Where(x => !exceptIDs.Contains(x.Id));
            }


            if (inventoryStock.HasValue)
            {
                sqlQuery = sqlQuery.Where(x => x.TrackInventory == inventoryStock.Value);
            }

            return sqlQuery;
        }
    }

    public static class InvoiceItemDaoExtention
    {
        public static DIHelper AddInvoiceItemDaoService(this DIHelper services)
        {
            services.TryAddScoped<InvoiceItemDao>();

            return services.AddCRMDbContextService()
                           .AddTenantManagerService()
                           .AddSecurityContextService()
                           .AddTenantUtilService()
                           .AddCRMSecurityService();
        }
    }
}        