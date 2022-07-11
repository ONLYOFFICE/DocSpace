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
using System.Linq;

using ASC.Common;
using ASC.Common.Caching;
using ASC.Common.Logging;
using ASC.Core;
using ASC.Core.Common.EF;
using ASC.Core.Common.EF.Context;
using ASC.CRM.Core.EF;
using ASC.CRM.Core.Entities;

using AutoMapper;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

namespace ASC.CRM.Core.Dao
{
    [Scope]
    public class InvoiceTaxDao : AbstractDao
    {
        private CrmSecurity _crmSecurity { get; }

        public InvoiceTaxDao(
            DbContextManager<CrmDbContext> dbContextManager,
            TenantManager tenantManager,
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
            _mapper = mapper;
        }

        public Boolean IsExist(int id)
        {
            return CrmDbContext.InvoiceTax.AsQueryable().Where(x => x.Id == id).Any();
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
            var dbEntities = Query(CrmDbContext.InvoiceTax)
                                     .AsNoTracking()
                                     .ToList();

            return _mapper.Map<List<DbInvoiceTax>, List<InvoiceTax>>(dbEntities);
        }

        public DateTime GetMaxLastModified()
        {
            var result = Query(CrmDbContext.InvoiceTax).Max(x => x.LastModifedOn);

            if (result.HasValue) return result.Value;

            return DateTime.MinValue;
        }

        public List<InvoiceTax> GetByID(int[] ids)
        {
            var dbEntities = Query(CrmDbContext.InvoiceTax)
                    .AsNoTracking()
                    .Where(x => ids.Contains(x.Id))
                    .ToList();

            return _mapper.Map<List<DbInvoiceTax>, List<InvoiceTax>>(dbEntities);
        }

        public InvoiceTax GetByID(int id)
        {
            var dbEntity = CrmDbContext.InvoiceTax.Find(id);

            if (dbEntity.TenantId != TenantID) return null;

            return _mapper.Map<InvoiceTax>(dbEntity);
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

            var dbEntity = new DbInvoiceTax
            {
                Id = invoiceTax.ID,
                Name = invoiceTax.Name,
                Description = invoiceTax.Description,
                Rate = invoiceTax.Rate,
                CreateOn = invoiceTax.CreateOn == DateTime.MinValue ? DateTime.UtcNow : invoiceTax.CreateOn,
                CreateBy = invoiceTax.CreateBy == Guid.Empty ? _securityContext.CurrentAccount.ID : invoiceTax.CreateBy,
                LastModifedOn = DateTime.UtcNow,
                LastModifedBy = _securityContext.CurrentAccount.ID,
                TenantId = TenantID
            };


            CrmDbContext.Update(dbEntity);
            CrmDbContext.SaveChanges();

            return invoiceTax;
        }

        public InvoiceTax DeleteInvoiceTax(int id)
        {
            var dbEntity = CrmDbContext.InvoiceTax.Find(id);

            var entity = _mapper.Map<InvoiceTax>(dbEntity);

            if (entity == null) return null;

            _crmSecurity.DemandDelete(entity);

            CrmDbContext.InvoiceTax.Remove(dbEntity);

            CrmDbContext.SaveChanges();

            /* _cache.Remove(_invoiceItemCacheKey);
             _cache.Insert(_invoiceTaxCacheKey, String.Empty);*/
            return entity;
        }
    }

}