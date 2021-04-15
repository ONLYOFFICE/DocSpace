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
    public class CachedInvoiceTaxDao : InvoiceTaxDao
    {
        private readonly HttpRequestDictionary<InvoiceTax> _invoiceTaxCache;

        public CachedInvoiceTaxDao(DbContextManager<CRMDbContext> dbContextManager,
            TenantManager tenantManager,
            TenantUtil tenantUtil,
            SecurityContext securityContext,
            IHttpContextAccessor httpContextAccessor,
            IOptionsMonitor<ILog> logger,
            ICache ascCache,
            IMapper mapper
            )
            : base(dbContextManager,
                 tenantManager,
                 tenantUtil,
                 securityContext,
                 logger,
                 ascCache,
                 mapper)

        {
            _invoiceTaxCache = new HttpRequestDictionary<InvoiceTax>(httpContextAccessor?.HttpContext, "crm_invoice_tax");
        }

        public override InvoiceTax GetByID(int invoiceTaxID)
        {
            return _invoiceTaxCache.Get(invoiceTaxID.ToString(CultureInfo.InvariantCulture), () => GetByIDBase(invoiceTaxID));
        }

        private InvoiceTax GetByIDBase(int invoiceTaxID)
        {
            return base.GetByID(invoiceTaxID);
        }

        public override InvoiceTax SaveOrUpdateInvoiceTax(InvoiceTax invoiceTax)
        {
            if (invoiceTax != null && invoiceTax.ID > 0)
                ResetCache(invoiceTax.ID);

            return base.SaveOrUpdateInvoiceTax(invoiceTax);
        }

        public override InvoiceTax DeleteInvoiceTax(int invoiceTaxID)
        {
            ResetCache(invoiceTaxID);

            return base.DeleteInvoiceTax(invoiceTaxID);
        }

        private void ResetCache(int invoiceTaxID)
        {
            _invoiceTaxCache.Reset(invoiceTaxID.ToString(CultureInfo.InvariantCulture));
        }
    }

    [Scope]
    public class InvoiceTaxDao : AbstractDao
    {
        private readonly IMapper _mapper;

        public InvoiceTaxDao(
            DbContextManager<CRMDbContext> dbContextManager,
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
        public CRMSecurity CRMSecurity { get; }

        public Boolean IsExist(int invoiceTaxID)
        {
            return CRMDbContext.InvoiceTax.Where(x => x.Id == invoiceTaxID).Any();
        }

        public Boolean IsExist(String invoiceName)
        {
            return Query(CRMDbContext.InvoiceTax).Where(x => String.Compare(x.Name, invoiceName, true) == 0).Any();
        }

        public Boolean CanDelete(int invoiceTaxID)
        {
            return !Query(CRMDbContext.InvoiceItem)
                        .Where(x => x.InvoiceTax1Id == invoiceTaxID || x.InvoiceTax2Id == invoiceTaxID).Any() &&
                    !Query(CRMDbContext.InvoiceLine)
                        .Where(x => x.InvoiceTax1Id == invoiceTaxID || x.InvoiceTax2Id == invoiceTaxID).Any();
        }

        public virtual List<InvoiceTax> GetAll()
        {
            return _mapper.Map<List<DbInvoiceTax>, List<InvoiceTax>>(Query(CRMDbContext.InvoiceTax).ToList());
        }

        public DateTime GetMaxLastModified()
        {
            var result = Query(CRMDbContext.InvoiceTax).Max(x => x.LastModifedOn);

            if (result.HasValue) return result.Value;

            return DateTime.MinValue;
        }

        public virtual List<InvoiceTax> GetByID(int[] ids)
        {
            var result = Query(CRMDbContext.InvoiceTax)
                    .Where(x => ids.Contains(x.Id))
                    .ToList();

            return _mapper.Map<List<DbInvoiceTax>, List<InvoiceTax>>(result);
        }

        public virtual InvoiceTax GetByID(int id)
        {
            var invoiceTax = CRMDbContext.InvoiceTax.FirstOrDefault(x => x.Id == id);

            return _mapper.Map<InvoiceTax>(invoiceTax);
        }

        public virtual InvoiceTax SaveOrUpdateInvoiceTax(InvoiceTax invoiceTax)
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

            if (!Query(CRMDbContext.InvoiceTax).Where(x => x.Id == invoiceTax.ID).Any())
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

                CRMDbContext.InvoiceTax.Add(itemToInsert);

                CRMDbContext.SaveChanges();

                invoiceTax.ID = itemToInsert.Id;
            }
            else
            {
                var oldInvoiceTax = GetByID(invoiceTax.ID);

                CRMSecurity.DemandEdit(oldInvoiceTax);

                var itemToUpdate = Query(CRMDbContext.InvoiceTax)
                                    .FirstOrDefault(x => x.Id == invoiceTax.ID);

                itemToUpdate.Name = invoiceTax.Name;
                itemToUpdate.Description = invoiceTax.Description;
                itemToUpdate.Rate = invoiceTax.Rate;
                itemToUpdate.LastModifedOn = itemToUpdate.LastModifedOn;
                itemToUpdate.LastModifedBy = itemToUpdate.LastModifedBy;

                CRMDbContext.InvoiceTax.Update(itemToUpdate);

                CRMDbContext.SaveChanges();

            }

            return invoiceTax;
        }

        public virtual InvoiceTax DeleteInvoiceTax(int invoiceTaxID)
        {
            var invoiceTax = GetByID(invoiceTaxID);

            if (invoiceTax == null) return null;

            CRMSecurity.DemandDelete(invoiceTax);

            var itemToDelete = new DbInvoiceTax
            {
                Id = invoiceTaxID
            };

            CRMDbContext.Attach(invoiceTax);
            CRMDbContext.Remove(itemToDelete);

            CRMDbContext.SaveChanges();

            /* _cache.Remove(_invoiceItemCacheKey);
             _cache.Insert(_invoiceTaxCacheKey, String.Empty);*/
            return invoiceTax;
        }
    }

}