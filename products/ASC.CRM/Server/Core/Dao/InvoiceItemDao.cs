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

using ASC.Common;
using ASC.Common.Caching;
using ASC.Core;
using ASC.Core.Common.EF;
using ASC.CRM.Core.EF;
using ASC.CRM.Core.Entities;
using ASC.CRM.Core.Enums;
using ASC.Web.CRM.Classes;

using AutoMapper;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace ASC.CRM.Core.Dao
{
    [Scope]
    public class InvoiceItemDao : AbstractDao
    {
        private CrmSecurity _crmSecurity { get; }

        public InvoiceItemDao(
                DbContextManager<CrmDbContext> dbContextManager,
                TenantManager tenantManager,
                SecurityContext securityContext,
                CrmSecurity crmSecurity,
                ILogger logger,
                ICache ascCache,
                IMapper mapper
            ) : base(dbContextManager,
                 tenantManager,
                 securityContext,
                 logger,
                 ascCache,
                 mapper)
        {
            _crmSecurity = crmSecurity;
        }

        public Boolean IsExist(int id)
        {
            return IsExistInDb(id);
        }

        public Boolean IsExistInDb(int id)
        {
            return Query(CrmDbContext.InvoiceItem).Any(x => x.Id == id);
        }

        public Boolean CanDelete(int id)
        {
            return Query(CrmDbContext.InvoiceLine).Any(x => x.InvoiceItemId == id);
        }

        public List<InvoiceItem> GetAll()
        {
            return GetAllInDb();
        }
        public List<InvoiceItem> GetAllInDb()
        {
            var dbInvoiceItems = Query(CrmDbContext.InvoiceItem)
                                .AsNoTracking()
                                .ToList();

            return _mapper.Map<List<DbInvoiceItem>, List<InvoiceItem>>(dbInvoiceItems);
        }

        public List<InvoiceItem> GetByID(int[] ids)
        {
            var dbInvoiceItems = Query(CrmDbContext.InvoiceItem)
                                .AsNoTracking()
                                .Where(x => ids.Contains(x.Id))
                                .ToList();

            return _mapper.Map<List<DbInvoiceItem>, List<InvoiceItem>>(dbInvoiceItems);
        }

        public InvoiceItem GetByID(int id)
        {
            var dbEntity = CrmDbContext.InvoiceItem.Find(id);

            return _mapper.Map<InvoiceItem>(dbEntity);
        }

        public List<InvoiceItem> GetInvoiceItems(IEnumerable<int> ids)
        {
            if (ids == null || !ids.Any()) return new List<InvoiceItem>();

            var dbInvoiceItems = Query(CrmDbContext.InvoiceItem)
                                .AsNoTracking()
                                .Where(x => ids.Contains(x.Id))
                                .ToList();

            return _mapper.Map<List<DbInvoiceItem>, List<InvoiceItem>>(dbInvoiceItems);
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

            var dbInvoiceItems = sqlQuery.ToList();

            return _mapper.Map<List<DbInvoiceItem>, List<InvoiceItem>>(dbInvoiceItems);
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
                           _securityContext.CurrentAccount.ID.ToString() +
                           searchText;

            var fromCache = _cache.Get<string>(cacheKey);

            if (fromCache != null) return Convert.ToInt32(fromCache);

            var exceptIDs = _crmSecurity.GetPrivateItems(typeof(InvoiceItem)).ToList();

            int result;

            var withParams = !(String.IsNullOrEmpty(searchText) || status != 0 || inventoryStock.HasValue);

            if (withParams)
            {
                result = GetDbInvoiceItemByFilters(exceptIDs, searchText, status, inventoryStock).Count();
            }
            else
            {
                var countWithoutPrivate = Query(CrmDbContext.InvoiceItem).Count();

                var privateCount = exceptIDs.Count;

                if (privateCount > countWithoutPrivate)
                {
                    _logger.LogError(@"Private invoice items count more than all cases. Tenant: {0}. CurrentAccount: {1}",
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

        public InvoiceItem SaveOrUpdateInvoiceItem(InvoiceItem invoiceItem)
        {
            /*_cache.Remove(_invoiceItemCacheKey);
            _cache.Insert(_invoiceItemCacheKey, String.Empty);*/

            return SaveOrUpdateInvoiceItemInDb(invoiceItem);
        }

        private InvoiceItem SaveOrUpdateInvoiceItemInDb(InvoiceItem invoiceItem)
        {
            if (invoiceItem.Price <= 0 || string.IsNullOrEmpty(invoiceItem.Title))
                throw new ArgumentException();

            if (invoiceItem.Price > Global.MaxInvoiceItemPrice)
                throw new ArgumentException("Max Invoice Item Price: " + Global.MaxInvoiceItemPrice);

            if (!_crmSecurity.IsAdmin) _crmSecurity.CreateSecurityException();


            var dbEntity = new DbInvoiceItem
            {
                Id = invoiceItem.ID,
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
                CreateBy = _securityContext.CurrentAccount.ID,
                LastModifedOn = DateTime.Now,
                LastModifedBy = _securityContext.CurrentAccount.ID,
                TenantId = TenantID
            };

            CrmDbContext.Update(dbEntity);
            CrmDbContext.SaveChanges();

            return invoiceItem;
        }

        public InvoiceItem DeleteInvoiceItem(int invoiceItemID)
        {
            var invoiceItem = GetByID(invoiceItemID);

            if (invoiceItem == null) return null;

            _crmSecurity.DemandDelete(invoiceItem);

            CrmDbContext.Remove(new DbInvoiceItem
            {
                Id = invoiceItemID,
                TenantId = TenantID
            });

            CrmDbContext.SaveChanges();

            /*_cache.Remove(_invoiceItemCacheKey);
            _cache.Insert(_invoiceItemCacheKey, String.Empty);*/

            return invoiceItem;
        }

        public List<InvoiceItem> DeleteBatchInvoiceItems(int[] ids)
        {
            var result = new List<InvoiceItem>();

            foreach (var id in ids)
            {
                var dbEntity = CrmDbContext.InvoiceItem.Find(id);

                var entity = _mapper.Map<InvoiceItem>(dbEntity);

                result.Add(entity);

                _crmSecurity.DemandDelete(entity);

                CrmDbContext.Remove(dbEntity);
            }

            CrmDbContext.SaveChanges();

            // Delete relative  keys
            /*_cache.Remove(_invoiceItemCacheKey);
            _cache.Insert(_invoiceItemCacheKey, String.Empty);*/

            return result;
        }

        private IQueryable<DbInvoiceItem> GetDbInvoiceItemByFilters(
                                ICollection<int> exceptIDs,
                                string searchText,
                                int status,
                                bool? inventoryStock)
        {

            var sqlQuery = Query(CrmDbContext.InvoiceItem);

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
}