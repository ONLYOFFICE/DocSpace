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
using ASC.CRM.Core.EF;
using ASC.CRM.Core.Entities;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text.RegularExpressions;

namespace ASC.CRM.Core.Dao
{
    public class CachedInvoiceLineDao : InvoiceLineDao
    {
        private readonly HttpRequestDictionary<InvoiceLine> _invoiceLineCache;

        public CachedInvoiceLineDao(DbContextManager<CRMDbContext> dbContextManager,
            TenantManager tenantManager,
            SecurityContext securityContext,
            IHttpContextAccessor httpContextAccessor)
              : base(dbContextManager,
                 tenantManager,
                 securityContext)
        {
            _invoiceLineCache = new HttpRequestDictionary<InvoiceLine>(httpContextAccessor?.HttpContext, "crm_invoice_line");
        }

        public override InvoiceLine GetByID(int invoiceLineID)
        {
            return _invoiceLineCache.Get(invoiceLineID.ToString(CultureInfo.InvariantCulture), () => GetByIDBase(invoiceLineID));
        }

        private InvoiceLine GetByIDBase(int invoiceLineID)
        {
            return base.GetByID(invoiceLineID);
        }

        public override int SaveOrUpdateInvoiceLine(InvoiceLine invoiceLine)
        {
            if (invoiceLine != null && invoiceLine.ID > 0)
                ResetCache(invoiceLine.ID);

            return base.SaveOrUpdateInvoiceLine(invoiceLine);
        }

        public override void DeleteInvoiceLine(int invoiceLineID)
        {
            ResetCache(invoiceLineID);

            base.DeleteInvoiceLine(invoiceLineID);
        }

        private void ResetCache(int invoiceLineID)
        {
            _invoiceLineCache.Reset(invoiceLineID.ToString(CultureInfo.InvariantCulture));
        }
    }
    
    public class InvoiceLineDao : AbstractDao
    {
        public InvoiceLineDao(DbContextManager<CRMDbContext> dbContextManager,
            TenantManager tenantManager,
            SecurityContext securityContext,
            IOptionsMonitor<ILog> logger)
              : base(dbContextManager,
                 tenantManager,
                 securityContext,
                 logger)
        {

        }


        public static string GetJson(InvoiceItem invoiceItem) {
            return invoiceItem == null ?
                    string.Empty :
                    JsonConvert.SerializeObject(new
                    {
                        id = invoiceItem.ID,
                        title = invoiceItem.Title,
                        description = invoiceItem.Description
                    });
        }
        public static string GetJson(InvoiceTax invoiceTax) {
            return invoiceTax == null ?
                    string.Empty :
                    JsonConvert.SerializeObject(new
                    {
                        id = invoiceTax.ID,
                        name = invoiceTax.Name,
                        rate = invoiceTax.Rate,
                        description = invoiceTax.Description
                    });
        }
            
        public virtual List<InvoiceLine> GetAll()
        {
            return Query(CRMDbContext.InvoiceLine)
                    .ToList()
                    .ConvertAll(ToInvoiceLine);
        }

        public virtual List<InvoiceLine> GetByID(int[] ids)
        {
            return Query(CRMDbContext.InvoiceLine).Where(x => ids.Contains(x.Id)).ToList()
                    .ConvertAll(ToInvoiceLine);
        }

        public virtual InvoiceLine GetByID(int id)
        {
            return ToInvoiceLine(Query(CRMDbContext.InvoiceLine).FirstOrDefault(x => x.Id == id));
        }
        
        public List<InvoiceLine> GetInvoiceLines(int invoiceID)
        {
            return GetInvoiceLinesInDb(invoiceID);
        }

        public List<InvoiceLine> GetInvoiceLinesInDb(int invoiceID)
        {
            return Query(CRMDbContext.InvoiceLine)
                .Where(x => x.InvoiceId == invoiceID)
                .OrderBy(x => x.SortOrder)
                .ToList()
                .ConvertAll(ToInvoiceLine);
        }
                
        public virtual int SaveOrUpdateInvoiceLine(InvoiceLine invoiceLine)
        {
            _cache.Remove(new Regex(TenantID.ToString(CultureInfo.InvariantCulture) + "invoice.*"));

            return SaveOrUpdateInvoiceLineInDb(invoiceLine);
        }

        private int SaveOrUpdateInvoiceLineInDb(InvoiceLine invoiceLine)
        {
            if (invoiceLine.InvoiceID <= 0 || invoiceLine.InvoiceItemID <= 0)
                throw new ArgumentException();

            if (String.IsNullOrEmpty(invoiceLine.Description))
            {
                invoiceLine.Description = String.Empty;
            }
                        

            if (Db.ExecuteScalar<int>(Query("crm_invoice_line").SelectCount().Where(Exp.Eq("id", invoiceLine.ID))) == 0)
            {
                invoiceLine.ID = Db.ExecuteScalar<int>(
                               Insert("crm_invoice_line")
                              .InColumnValue("id", 0)
                              .InColumnValue("invoice_id", invoiceLine.InvoiceID)
                              .InColumnValue("invoice_item_id", invoiceLine.InvoiceItemID)
                              .InColumnValue("invoice_tax1_id", invoiceLine.InvoiceTax1ID)
                              .InColumnValue("invoice_tax2_id", invoiceLine.InvoiceTax2ID)
                              .InColumnValue("sort_order", invoiceLine.SortOrder)
                              .InColumnValue("description", invoiceLine.Description)
                              .InColumnValue("quantity", invoiceLine.Quantity)
                              .InColumnValue("price", invoiceLine.Price)
                              .InColumnValue("discount", invoiceLine.Discount)
                              .Identity(1, 0, true));
            }
            else
            {

                Db.ExecuteNonQuery(
                    Update("crm_invoice_line")
                        .Set("invoice_id", invoiceLine.InvoiceID)
                        .Set("invoice_item_id", invoiceLine.InvoiceItemID)
                        .Set("invoice_tax1_id", invoiceLine.InvoiceTax1ID)
                        .Set("invoice_tax2_id", invoiceLine.InvoiceTax2ID)
                        .Set("sort_order", invoiceLine.SortOrder)
                        .Set("description", invoiceLine.Description)
                        .Set("quantity", invoiceLine.Quantity)
                        .Set("price", invoiceLine.Price)
                        .Set("discount", invoiceLine.Discount)
                        .Where(Exp.Eq("id", invoiceLine.ID)));
            }
            return invoiceLine.ID;
        }
                
        public virtual void DeleteInvoiceLine(int invoiceLineID)
        {
            var invoiceLine = GetByID(invoiceLineID);

            if (invoiceLine == null) return;

            var itemToDelete = new DbInvoiceLine { Id = invoiceLineID };

            CRMDbContext.Attach(itemToDelete);
            CRMDbContext.Remove(itemToDelete);
            CRMDbContext.SaveChanges();
                       
            /*_cache.Remove(_invoiceItemCacheKey);
            _cache.Insert(_invoiceLineCacheKey, String.Empty);*/
        }

        public void DeleteInvoiceLines(int invoiceID)
        {
            var itemToDelete = Query(CRMDbContext.InvoiceLine).Where(x => x.InvoiceId == invoiceID);

            CRMDbContext.RemoveRange(itemToDelete);
            CRMDbContext.SaveChanges();
            
            /*_cache.Remove(_invoiceItemCacheKey);
            _cache.Insert(_invoiceLineCacheKey, String.Empty);*/
        }

        public Boolean CanDelete(int invoiceLineID)
        {
            return CanDeleteInDb(invoiceLineID);
        }

        public Boolean CanDeleteInDb(int invoiceLineID)
        {
            var invoiceID = Query(CRMDbContext.InvoiceLine)
                .Where(x => x.Id == invoiceLineID)
                .Select(x => x.InvoiceId);

            if (!invoiceID.Any()) return false;

            return Query(CRMDbContext.InvoiceLine).Where(x => x.InvoiceId == invoiceLineID).Any();            
        }

        private InvoiceLine ToInvoiceLine(DbInvoiceLine dbInvoiceLine)
        {
            if (dbInvoiceLine == null) return null;

            return new InvoiceLine
            {
                ID = dbInvoiceLine.Id,
                InvoiceID = dbInvoiceLine.InvoiceId,
                InvoiceItemID = dbInvoiceLine.InvoiceItemId,
                InvoiceTax1ID = dbInvoiceLine.InvoiceTax1Id,
                InvoiceTax2ID = dbInvoiceLine.InvoiceTax2Id,
                SortOrder = dbInvoiceLine.SortOrder,
                Description = dbInvoiceLine.Description,
                Quantity = dbInvoiceLine.Quantity,
                Price = dbInvoiceLine.Price,
                Discount = dbInvoiceLine.Discount
            };
        }
    }
}