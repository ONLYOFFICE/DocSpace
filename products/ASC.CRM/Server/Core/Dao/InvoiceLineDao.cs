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

using ASC.Collections;
using ASC.Common;
using ASC.Common.Caching;
using ASC.Common.Logging;
using ASC.Core;
using ASC.Core.Common.EF;
using ASC.CRM.Core.EF;
using ASC.CRM.Core.Entities;

using AutoMapper;

using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;

using Newtonsoft.Json;

namespace ASC.CRM.Core.Dao
{
    [Scope]
    public class InvoiceLineDao : AbstractDao
    {
        public InvoiceLineDao(DbContextManager<CrmDbContext> dbContextManager,
            TenantManager tenantManager,
            SecurityContext securityContext,
            IOptionsMonitor<ILog> logger,
            ICache ascCache,
            IMapper mapper)
              : base(dbContextManager,
                 tenantManager,
                 securityContext,
                 logger,
                 ascCache,
                 mapper)
        {

        }


        public static string GetJson(InvoiceItem invoiceItem)
        {
            return invoiceItem == null ?
                    string.Empty :
                    JsonConvert.SerializeObject(new
                    {
                        id = invoiceItem.ID,
                        title = invoiceItem.Title,
                        description = invoiceItem.Description
                    });
        }
        public static string GetJson(InvoiceTax invoiceTax)
        {
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

        public List<InvoiceLine> GetAll()
        {
            var dbInvoiceLines = Query(CrmDbContext.InvoiceLine)
                    .ToList();

            return _mapper.Map<List<DbInvoiceLine>, List<InvoiceLine>>(dbInvoiceLines);
        }

        public List<InvoiceLine> GetByID(int[] ids)
        {
            var dbInvoiceLines = Query(CrmDbContext.InvoiceLine).Where(x => ids.Contains(x.Id)).ToList();

            return _mapper.Map<List<DbInvoiceLine>, List<InvoiceLine>>(dbInvoiceLines);               
        }

        public InvoiceLine GetByID(int id)
        {
            return _mapper.Map<InvoiceLine>(Query(CrmDbContext.InvoiceLine).FirstOrDefault(x => x.Id == id));
        }

        public List<InvoiceLine> GetInvoiceLines(int invoiceID)
        {
            return GetInvoiceLinesInDb(invoiceID);
        }

        public List<InvoiceLine> GetInvoiceLinesInDb(int invoiceID)
        {
            var dbInvoiceLines = Query(CrmDbContext.InvoiceLine)
                .Where(x => x.InvoiceId == invoiceID)
                .OrderBy(x => x.SortOrder)
                .ToList();

            return _mapper.Map<List<DbInvoiceLine>, List<InvoiceLine>>(dbInvoiceLines);                
        }

        public int SaveOrUpdateInvoiceLine(InvoiceLine invoiceLine)
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

            if (Query(CrmDbContext.InvoiceLine).Where(x => x.Id == invoiceLine.ID).Any())
            {

                var itemToInsert = new DbInvoiceLine
                {
                    InvoiceId = invoiceLine.InvoiceItemID,
                    InvoiceItemId = invoiceLine.InvoiceItemID,
                    InvoiceTax1Id = invoiceLine.InvoiceTax1ID,
                    InvoiceTax2Id = invoiceLine.InvoiceTax2ID,
                    SortOrder = invoiceLine.SortOrder,
                    Description = invoiceLine.Description,
                    Quantity = invoiceLine.Quantity,
                    Price = invoiceLine.Price,
                    Discount = invoiceLine.Discount,
                    TenantId = TenantID
                };

                CrmDbContext.Add(itemToInsert);
                CrmDbContext.SaveChanges();

                invoiceLine.ID = itemToInsert.Id;
            }
            else
            {
                var itemToUpdate = Query(CrmDbContext.InvoiceLine).Single(x => x.Id == invoiceLine.ID);

                itemToUpdate.InvoiceId = invoiceLine.InvoiceID;
                itemToUpdate.InvoiceItemId = invoiceLine.InvoiceItemID;
                itemToUpdate.InvoiceTax1Id = invoiceLine.InvoiceTax1ID;
                itemToUpdate.InvoiceTax2Id = invoiceLine.InvoiceTax2ID;
                itemToUpdate.SortOrder = invoiceLine.SortOrder;
                itemToUpdate.Description = invoiceLine.Description;
                itemToUpdate.Quantity = invoiceLine.Quantity;
                itemToUpdate.Price = invoiceLine.Price;
                itemToUpdate.Discount = invoiceLine.Discount;

                CrmDbContext.Update(itemToUpdate);

                CrmDbContext.SaveChanges();


            }

            return invoiceLine.ID;
        }

        public void DeleteInvoiceLine(int invoiceLineID)
        {
            var invoiceLine = GetByID(invoiceLineID);

            if (invoiceLine == null) return;

            var itemToDelete = new DbInvoiceLine { Id = invoiceLineID };

            CrmDbContext.Attach(itemToDelete);
            CrmDbContext.Remove(itemToDelete);
            CrmDbContext.SaveChanges();

            /*_cache.Remove(_invoiceItemCacheKey);
            _cache.Insert(_invoiceLineCacheKey, String.Empty);*/
        }

        public void DeleteInvoiceLines(int invoiceID)
        {
            var itemToDelete = Query(CrmDbContext.InvoiceLine).Where(x => x.InvoiceId == invoiceID);

            CrmDbContext.RemoveRange(itemToDelete);
            CrmDbContext.SaveChanges();

            /*_cache.Remove(_invoiceItemCacheKey);
            _cache.Insert(_invoiceLineCacheKey, String.Empty);*/
        }

        public Boolean CanDelete(int invoiceLineID)
        {
            return CanDeleteInDb(invoiceLineID);
        }

        public Boolean CanDeleteInDb(int invoiceLineID)
        {
            var invoiceID = Query(CrmDbContext.InvoiceLine)
                .Where(x => x.Id == invoiceLineID)
                .Select(x => x.InvoiceId);

            if (!invoiceID.Any()) return false;

            return Query(CrmDbContext.InvoiceLine).Where(x => x.InvoiceId == invoiceLineID).Any();
        }
    }
}