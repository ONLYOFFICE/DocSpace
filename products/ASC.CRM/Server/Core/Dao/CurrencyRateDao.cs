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
using ASC.Core;
using ASC.Core.Common.EF;
using ASC.CRM.Core.EF;

using AutoMapper;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace ASC.CRM.Core.Dao
{
    [Scope]
    public class CurrencyRateDao : AbstractDao
    {
        public CurrencyRateDao(
            DbContextManager<CrmDbContext> dbContextManager,
            TenantManager tenantManager,
            SecurityContext securityContext,
            ILogger logger,
            ICache ascCache,
            IMapper mapper) :
              base(dbContextManager,
                 tenantManager,
                 securityContext,
                 logger,
                 ascCache,
                 mapper)
        {

        }

        public List<CurrencyRate> GetAll()
        {
            var rates = Query(CrmDbContext.CurrencyRate).ToList();

            return _mapper.Map<List<DbCurrencyRate>, List<CurrencyRate>>(rates);
        }

        public CurrencyRate GetByID(int id)
        {
            var entity = CrmDbContext.CurrencyRate.Find(id);

            if (entity.TenantId != TenantID) return null;

            return _mapper.Map<CurrencyRate>(entity);
        }

        public CurrencyRate GetByCurrencies(string fromCurrency, string toCurrency)
        {
            var dbEntity = Query(CrmDbContext.CurrencyRate).FirstOrDefault(x =>
                                string.Compare(x.FromCurrency, fromCurrency, true) == 0 &&
                                string.Compare(x.ToCurrency, toCurrency, true) == 0);

            return _mapper.Map<CurrencyRate>(dbEntity);

        }

        public int SaveOrUpdate(CurrencyRate entity)
        {
            if (String.IsNullOrEmpty(entity.FromCurrency) || String.IsNullOrEmpty(entity.ToCurrency) || entity.Rate < 0)
                throw new ArgumentException();

            if (entity.ID > 0 && entity.Rate == 0)
                return Delete(entity.ID);

            var dbEntity = new DbCurrencyRate
            {
                Id = entity.ID,
                FromCurrency = entity.FromCurrency.ToUpper(),
                ToCurrency = entity.ToCurrency.ToUpper(),
                Rate = entity.Rate,
                CreateOn = entity.CreateOn == DateTime.MinValue ? DateTime.UtcNow : entity.CreateOn,
                CreateBy = entity.CreateBy == Guid.Empty ? _securityContext.CurrentAccount.ID : entity.CreateBy,
                LastModifedOn = DateTime.UtcNow,
                LastModifedBy = _securityContext.CurrentAccount.ID,
                TenantId = TenantID
            };

            CrmDbContext.Update(dbEntity);
            CrmDbContext.SaveChanges();

            return dbEntity.Id;
        }

        public int Delete(int id)
        {
            var dbEntity = CrmDbContext.CurrencyRate.Find(id);

            CrmDbContext.Remove(dbEntity);

            CrmDbContext.SaveChanges();

            return id;
        }

        public List<CurrencyRate> SetCurrencyRates(List<CurrencyRate> rates)
        {
            using var tx = CrmDbContext.Database.BeginTransaction();

            var items = Query(CrmDbContext.CurrencyRate).AsNoTracking();

            CrmDbContext.RemoveRange(items);

            foreach (var rate in rates)
            {
                var itemToInsert = new DbCurrencyRate
                {
                    FromCurrency = rate.FromCurrency.ToUpper(),
                    ToCurrency = rate.ToCurrency.ToUpper(),
                    Rate = rate.Rate,
                    CreateBy = _securityContext.CurrentAccount.ID,
                    CreateOn = DateTime.UtcNow,
                    LastModifedBy = _securityContext.CurrentAccount.ID,
                    LastModifedOn = DateTime.UtcNow,
                    TenantId = TenantID
                };

                CrmDbContext.CurrencyRate.Add(itemToInsert);

                CrmDbContext.SaveChanges();

                rate.ID = itemToInsert.Id;
            }

            tx.Commit();

            return rates;
        }
    }
}