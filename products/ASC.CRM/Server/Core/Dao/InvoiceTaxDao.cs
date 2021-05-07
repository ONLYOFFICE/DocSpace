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

using ASC.Collections;
using ASC.Common;
using ASC.Common.Caching;
using ASC.Common.Logging;
using ASC.Core;
using ASC.Core.Common.EF;
using ASC.Core.Tenants;
using ASC.CRM.Core.EF;
using ASC.CRM.Core.Entities;

using AutoMapper;

using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;

namespace ASC.CRM.Core.Dao
{
    [Scope]
    public class InvoiceTaxDao : AbstractDao
    {
        public InvoiceTaxDao(
            DbContextManager<CrmDbContext> dbContextManager,
            TenantManager tenantManager,
            TenantUtil tenantUtil,
            SecurityContext securityContext,
            IOptionsMonitor<ILog> logger,
            ICache ascCache,
            IMapper mapper
            )
            : base(dbContextManager,
                 tenantManager,
                 securityContext,
                 logger,
                 ascCache,
                 mapper)
        {
            TenantUtil = tenantUtil;
            _mapper = mapper;
        }

        public TenantUtil TenantUtil { get; }
        public CrmSecurity CRMSecurity { get; }

        public Boolean IsExist(int invoiceTaxID)
        {
            return CrmDbContext.InvoiceTax.Where(x => x.Id == invoiceTaxID).Any();
        }

        public Boolean IsExist(String invoiceName)
        {
            return Query(CrmDbContext.InvoiceTax).Where(x => String.Compare(x.Name, invoiceName, true) == 0).Any();
        }

        public Boolean CanDelete(int invoiceTaxID)
        {
            return !Query(CrmDbContext.InvoiceItem)
                        .Where(x => x.InvoiceTax1Id == invoiceTaxID || x.InvoiceTax2Id == invoiceTaxID).Any() &&
                    !Query(CrmDbContext.InvoiceLine)
                        .Where(x => x.InvoiceTax1Id == invoiceTaxID || x.InvoiceTax2Id == invoiceTaxID).Any();
        }

        public List<InvoiceTax> GetAll()
        {
            return _mapper.Map<List<DbInvoiceTax>, List<InvoiceTax>>(Query(CrmDbContext.InvoiceTax).ToList());
        }

        public DateTime GetMaxLastModified()
        {
            var result = Query(CrmDbContext.InvoiceTax).Max(x => x.LastModifedOn);

            if (result.HasValue) return result.Value;

            return DateTime.MinValue;
        }

        public List<InvoiceTax> GetByID(int[] ids)
        {
            var result = Query(CrmDbContext.InvoiceTax)
                    .Where(x => ids.Contains(x.Id))
                    .ToList();

            return _mapper.Map<List<DbInvoiceTax>, List<InvoiceTax>>(result);
        }

        public InvoiceTax GetByID(int id)
        {
            var invoiceTax = CrmDbContext.InvoiceTax.FirstOrDefault(x => x.Id == id);

            return _mapper.Map<InvoiceTax>(invoiceTax);
        }

        public InvoiceTax SaveOrUpdateInvoiceTax(InvoiceTax invoiceTax)
        {
            /*_cache.Remove(_invoiceItemCacheKey);
            _cache.Insert(_invoiceTaxCacheKey, String.Empty);*/

            return SaveOrUpdateInvoiceTaxInDb(invoiceTax);
        }

        private InvoiceTax SaveOrUpdateInvoiceTaxInDb(InvoiceTax invoiceTax)
        {
            if (String.IsNullOrEmpty(invoiceTax.Name))
                throw new ArgumentException();

            invoiceTax.LastModifedBy = _securityContext.CurrentAccount.ID;
            invoiceTax.LastModifedOn = DateTime.UtcNow;

            if (!Query(CrmDbContext.InvoiceTax).Where(x => x.Id == invoiceTax.ID).Any())
            {
                invoiceTax.CreateOn = DateTime.UtcNow;
                invoiceTax.CreateBy = _securityContext.CurrentAccount.ID;

                var itemToInsert = new DbInvoiceTax
                {
                    Name = invoiceTax.Name,
                    Description = invoiceTax.Description,
                    Rate = invoiceTax.Rate,
                    CreateOn = invoiceTax.CreateOn,
                    CreateBy = _securityContext.CurrentAccount.ID,
                    LastModifedBy = invoiceTax.LastModifedBy,
                    LastModifedOn = invoiceTax.LastModifedOn,
                    TenantId = TenantID
                };

                CrmDbContext.InvoiceTax.Add(itemToInsert);

                CrmDbContext.SaveChanges();

                invoiceTax.ID = itemToInsert.Id;
            }
            else
            {
                var oldInvoiceTax = GetByID(invoiceTax.ID);

                CRMSecurity.DemandEdit(oldInvoiceTax);

                var itemToUpdate = Query(CrmDbContext.InvoiceTax)
                                    .FirstOrDefault(x => x.Id == invoiceTax.ID);

                itemToUpdate.Name = invoiceTax.Name;
                itemToUpdate.Description = invoiceTax.Description;
                itemToUpdate.Rate = invoiceTax.Rate;
                itemToUpdate.LastModifedOn = itemToUpdate.LastModifedOn;
                itemToUpdate.LastModifedBy = itemToUpdate.LastModifedBy;

                CrmDbContext.InvoiceTax.Update(itemToUpdate);

                CrmDbContext.SaveChanges();

            }

            return invoiceTax;
        }

        public InvoiceTax DeleteInvoiceTax(int invoiceTaxID)
        {
            var invoiceTax = GetByID(invoiceTaxID);

            if (invoiceTax == null) return null;

            CRMSecurity.DemandDelete(invoiceTax);

            var itemToDelete = new DbInvoiceTax
            {
                Id = invoiceTaxID
            };

            CrmDbContext.Attach(invoiceTax);
            CrmDbContext.Remove(itemToDelete);

            CrmDbContext.SaveChanges();

            /* _cache.Remove(_invoiceItemCacheKey);
             _cache.Insert(_invoiceTaxCacheKey, String.Empty);*/
            return invoiceTax;
        }
    }

}