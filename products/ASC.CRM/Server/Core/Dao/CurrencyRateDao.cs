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
using ASC.Core;
using ASC.Core.Common.EF;
using ASC.CRM.Core.EF;

namespace ASC.CRM.Core.Dao
{
    public class CurrencyRateDao : AbstractDao
    {
        public CurrencyRateDao(
            DbContextManager<CRMDbContext> dbContextManager,
            TenantManager tenantManager,
            SecurityContext securityContext) :
              base(dbContextManager,
                 tenantManager,
                 securityContext)
        {

        }

        public virtual List<CurrencyRate> GetAll()
        {
            return CRMDbContext.CurrencyRate.Where(x => x.TenantId == TenantID).ToList().ConvertAll(ToCurrencyRate);
        }

        public virtual CurrencyRate GetByID(int id)
        {
            return ToCurrencyRate(CRMDbContext.CurrencyRate.FirstOrDefault(x => x.Id == id));
        }

        public CurrencyRate GetByCurrencies(string fromCurrency, string toCurrency)
        {
           return ToCurrencyRate(CRMDbContext.CurrencyRate.FirstOrDefault(x => x.TenantId == TenantID && String.Compare(x.FromCurrency, fromCurrency, true) == 0 &&
                                                String.Compare(x.ToCurrency, toCurrency, true) == 0));
        }

        public int SaveOrUpdate(CurrencyRate currencyRate)
        {
            if (String.IsNullOrEmpty(currencyRate.FromCurrency) || String.IsNullOrEmpty(currencyRate.ToCurrency) || currencyRate.Rate < 0)
                throw new ArgumentException();

            if (currencyRate.ID > 0 && currencyRate.Rate == 0)
                return Delete(currencyRate.ID);
            
            if (CRMDbContext.CurrencyRate.Where(x => x.Id == currencyRate.ID).Any())
            {
                var itemToInsert = new DbCurrencyRate
                {
                    FromCurrency = currencyRate.FromCurrency.ToUpper(),
                    ToCurrency = currencyRate.ToCurrency.ToUpper(),
                    Rate = currencyRate.Rate,
                    CreateBy = SecurityContext.CurrentAccount.ID,
                    CreateOn = DateTime.UtcNow,
                    LastModifedBy = SecurityContext.CurrentAccount.ID,
                    LastModifedOn = DateTime.UtcNow,
                    TenantId = TenantID
                };

                CRMDbContext.CurrencyRate.Add(itemToInsert);
                CRMDbContext.SaveChanges();

                currencyRate.ID = itemToInsert.Id;
            }
            else
            {
                var itemToUpdate = CRMDbContext.CurrencyRate.FirstOrDefault(x => x.Id == currencyRate.ID);

                itemToUpdate.FromCurrency = currencyRate.FromCurrency.ToUpper();
                itemToUpdate.ToCurrency = currencyRate.ToCurrency.ToUpper();
                itemToUpdate.Rate = currencyRate.Rate;
                itemToUpdate.LastModifedBy = SecurityContext.CurrentAccount.ID;
                itemToUpdate.LastModifedOn = DateTime.UtcNow;
                itemToUpdate.TenantId = TenantID;
                
                CRMDbContext.Update(itemToUpdate);
                CRMDbContext.SaveChanges();                
            }

            return currencyRate.ID;
        }

        public int Delete(int id)
        {
            if (id <= 0) throw new ArgumentException();

            var itemToDelete = new DbCurrencyRate { Id = id };

            CRMDbContext.CurrencyRate.Attach(itemToDelete);
            CRMDbContext.CurrencyRate.Remove(itemToDelete);
            CRMDbContext.SaveChanges();

            return id;
        }

        public List<CurrencyRate> SetCurrencyRates(List<CurrencyRate> rates)
        {
            using var tx = CRMDbContext.Database.BeginTransaction();

            var items = CRMDbContext.CurrencyRate.Where(x => x.TenantId == TenantID);

            CRMDbContext.RemoveRange(items);
            
            foreach (var rate in rates)
            {
                var itemToInsert = new DbCurrencyRate {
                    FromCurrency = rate.FromCurrency.ToUpper(),
                    ToCurrency = rate.ToCurrency.ToUpper(),
                    Rate = rate.Rate,
                    CreateBy = SecurityContext.CurrentAccount.ID,
                    CreateOn = DateTime.UtcNow,
                    LastModifedBy = SecurityContext.CurrentAccount.ID,
                    LastModifedOn = DateTime.UtcNow,
                    TenantId = TenantID
                };

                CRMDbContext.CurrencyRate.Add(itemToInsert);

                CRMDbContext.SaveChanges();

                rate.ID = itemToInsert.Id;
            }

            tx.Commit();

            return rates;
        }
                
        private static CurrencyRate ToCurrencyRate(DbCurrencyRate dbCurrencyRate)
        {
            if (dbCurrencyRate == null) return null;

            return new CurrencyRate
            {
                ID = dbCurrencyRate.Id,
                FromCurrency = dbCurrencyRate.FromCurrency,
                ToCurrency = dbCurrencyRate.ToCurrency,
                Rate = dbCurrencyRate.Rate,
                CreateBy = dbCurrencyRate.CreateBy,
                CreateOn = dbCurrencyRate.CreateOn,
                LastModifedBy = dbCurrencyRate.LastModifedBy,
                LastModifedOn = dbCurrencyRate.LastModifedOn
            };
        }
    }
}