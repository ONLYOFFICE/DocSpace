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


using ASC.Core.Common.Settings;
using ASC.CRM.Core;
using ASC.CRM.Resources;
using ASC.MessagingSystem;
using ASC.Web.Api.Routing;
using ASC.Web.CRM.Classes;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ASC.Api.CRM
{
    public partial class CRMController
    {
        //TABLE `crm_currency_rate` column `rate` DECIMAL(10,2) NOT NULL
        public const decimal MaxRateValue = (decimal) 99999999.99;

        /// <summary>
        ///    Get the list of currency rates
        /// </summary>
        /// <short>Get currency rates list</short> 
        /// <category>Common</category>
        /// <returns>
        ///    List of currency rates
        /// </returns>
        [Read(@"currency/rates")]
        public IEnumerable<CurrencyRateWrapper> GetCurrencyRates()
        {
            return DaoFactory.GetCurrencyRateDao().GetAll().ConvertAll(x => CurrencyRateWrapperHelper.Get(x));
        }

        /// <summary>
        ///   Get currency rate by id
        /// </summary>
        /// <short>Get currency rate</short> 
        /// <category>Common</category>
        /// <returns>
        ///    Currency rate
        /// </returns>
        /// <exception cref="ArgumentException"></exception>
        [Read(@"currency/rates/{id:int}")]
        public CurrencyRateWrapper GetCurrencyRate(int id)
        {
            if (id <= 0) throw new ArgumentException();

            var currencyRate = DaoFactory.GetCurrencyRateDao().GetByID(id);

            return CurrencyRateWrapperHelper.Get(currencyRate);
        }

        /// <summary>
        ///   Get currency rate by currencies
        /// </summary>
        /// <short>Get currency rate</short> 
        /// <category>Common</category>
        /// <returns>
        ///    Currency rate
        /// </returns>
        /// <exception cref="ArgumentException"></exception>
        [Read(@"currency/rates/{fromCurrency}/{toCurrency}")]
        public CurrencyRateWrapper GetCurrencyRate(string fromCurrency, string toCurrency)
        {
            if (string.IsNullOrEmpty(fromCurrency) || string.IsNullOrEmpty(toCurrency))
                throw new ArgumentException();

            var currencyRate = DaoFactory.GetCurrencyRateDao().GetByCurrencies(fromCurrency, toCurrency);

            return CurrencyRateWrapperHelper.Get(currencyRate);
        }

        /// <summary>
        ///    Create new currency rate object
        /// </summary>
        /// <short></short>
        /// <category>Common</category>
        /// <returns></returns>
        [Create(@"currency/rates")]
        public CurrencyRateWrapper CreateCurrencyRate(string fromCurrency, string toCurrency, decimal rate)
        {
            ValidateRate(rate);

            ValidateCurrencies(new[] {fromCurrency, toCurrency});

            var currencyRate = new CurrencyRate
                {
                    FromCurrency = fromCurrency,
                    ToCurrency = toCurrency,
                    Rate = rate
                };

            currencyRate.ID = DaoFactory.GetCurrencyRateDao().SaveOrUpdate(currencyRate);
            MessageService.Send( MessageAction.CurrencyRateUpdated, fromCurrency, toCurrency);

            return CurrencyRateWrapperHelper.Get(currencyRate);
        }

        /// <summary>
        ///    Update currency rate object
        /// </summary>
        /// <short></short>
        /// <category>Common</category>
        /// <returns></returns>
        [Update(@"currency/rates/{id:int}")]
        public CurrencyRateWrapper UpdateCurrencyRate(int id, string fromCurrency, string toCurrency, decimal rate)
        {
            if (id <= 0)
                throw new ArgumentException();

            ValidateRate(rate);

            ValidateCurrencies(new[] {fromCurrency, toCurrency});

            var currencyRate = DaoFactory.GetCurrencyRateDao().GetByID(id);

            if (currencyRate == null)
                throw new ArgumentException();

            currencyRate.FromCurrency = fromCurrency;
            currencyRate.ToCurrency = toCurrency;
            currencyRate.Rate = rate;

            currencyRate.ID = DaoFactory.GetCurrencyRateDao().SaveOrUpdate(currencyRate);
            MessageService.Send( MessageAction.CurrencyRateUpdated, fromCurrency, toCurrency);

            return CurrencyRateWrapperHelper.Get(currencyRate);
        }

        /// <summary>
        ///    Set currency rates
        /// </summary>
        /// <short></short>
        /// <category>Common</category>
        /// <returns></returns>
        [Create(@"currency/setrates")]
        public List<CurrencyRateWrapper> SetCurrencyRates(String currency, List<CurrencyRate> rates)
        {
            if (!CRMSecurity.IsAdmin)
                throw CRMSecurity.CreateSecurityException();

            if (string.IsNullOrEmpty(currency))
                throw new ArgumentException();

            ValidateCurrencyRates(rates);

            currency = currency.ToUpper();
            
            if (SettingsManager.Load<CRMSettings>().DefaultCurrency.Abbreviation != currency)
            {
                var cur = CurrencyProvider.Get(currency);

                if (cur == null)
                    throw new ArgumentException();

                Global.SaveDefaultCurrencySettings(cur);

                MessageService.Send( MessageAction.CrmDefaultCurrencyUpdated);
            }

            rates = DaoFactory.GetCurrencyRateDao().SetCurrencyRates(rates);

            foreach (var rate in rates)
            {
                MessageService.Send( MessageAction.CurrencyRateUpdated, rate.FromCurrency, rate.ToCurrency);
            }

            return rates.Select(x => CurrencyRateWrapperHelper.Get(x)).ToList();
        }

        /// <summary>
        ///    Add currency rates
        /// </summary>
        /// <short></short>
        /// <category>Common</category>
        /// <returns></returns>
        [Create(@"currency/addrates")]
        public List<CurrencyRateWrapper> AddCurrencyRates(List<CurrencyRate> rates)
        {
            if (!CRMSecurity.IsAdmin)
                throw CRMSecurity.CreateSecurityException();

            ValidateCurrencyRates(rates);

            var existingRates = DaoFactory.GetCurrencyRateDao().GetAll();

            foreach (var rate in rates)
            {
                var exist = false;
                
                foreach (var existingRate in existingRates)
                {
                    if (rate.FromCurrency != existingRate.FromCurrency || rate.ToCurrency != existingRate.ToCurrency)
                        continue;

                    existingRate.Rate = rate.Rate;
                    DaoFactory.GetCurrencyRateDao().SaveOrUpdate(existingRate);
                    MessageService.Send( MessageAction.CurrencyRateUpdated, rate.FromCurrency, rate.ToCurrency);
                    exist = true;
                    break;
                }

                if (exist) continue;

                rate.ID = DaoFactory.GetCurrencyRateDao().SaveOrUpdate(rate);
                MessageService.Send( MessageAction.CurrencyRateUpdated, rate.FromCurrency, rate.ToCurrency);
                existingRates.Add(rate);
            }

            return existingRates.Select(x => CurrencyRateWrapperHelper.Get(x)).ToList();
        }

        /// <summary>
        ///    Delete currency rate object
        /// </summary>
        /// <short></short>
        /// <category>Common</category>
        /// <returns></returns>
        [Delete(@"currency/rates/{id:int}")]
        public CurrencyRateWrapper DeleteCurrencyRate(int id)
        {
            if (id <= 0)
                throw new ArgumentException();

            var currencyRate = DaoFactory.GetCurrencyRateDao().GetByID(id);
            
            if (currencyRate == null)
                throw new ArgumentException();

            DaoFactory.GetCurrencyRateDao().Delete(id);

            return CurrencyRateWrapperHelper.Get(currencyRate);
        }

        private void ValidateCurrencyRates(IEnumerable<CurrencyRate> rates)
        {
            var currencies = new List<string>();

            foreach (var rate in rates)
            {
                ValidateRate(rate.Rate);
                currencies.Add(rate.FromCurrency);
                currencies.Add(rate.ToCurrency);
            }

            ValidateCurrencies(currencies.ToArray());
        }
        
        private void ValidateCurrencies(string[] currencies)
        {
            if (currencies.Any(string.IsNullOrEmpty))
                throw new ArgumentException();

            var available = CurrencyProvider.GetAll().Select(x => x.Abbreviation);

            var unknown = currencies.Where(x => !available.Contains(x)).ToArray();

            if (!unknown.Any()) return;

            throw new ArgumentException(string.Format(CRMErrorsResource.UnknownCurrency, string.Join(",", unknown)));
        }

        private static void ValidateRate(decimal rate)
        {
            if (rate < 0 || rate > MaxRateValue)
                throw new ArgumentException(string.Format(CRMErrorsResource.InvalidCurrencyRate, rate));
        }
       
    }
}