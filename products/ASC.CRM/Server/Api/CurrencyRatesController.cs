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

using ASC.Api.CRM;
using ASC.Core.Common.Settings;
using ASC.CRM.ApiModels;

using ASC.CRM.Core;
using ASC.CRM.Core.Dao;
using ASC.CRM.Resources;
using ASC.MessagingSystem.Core;
using ASC.Web.CRM.Classes;

using AutoMapper;

using Microsoft.AspNetCore.Mvc;

namespace ASC.CRM.Api
{
    public class CurrencyRatesController : BaseApiController
    {
        private readonly CurrencyProvider _currencyProvider;
        private readonly SettingsManager _settingsManager;
        private readonly Global _global;
        private readonly MessageService _messageService;

        public CurrencyRatesController(CrmSecurity crmSecurity,
                     DaoFactory daoFactory,
                     MessageService messageService,
                     SettingsManager settingsManager,
                     Global global,
                     CurrencyProvider currencyProvider,
                     IMapper mapper)
            : base(daoFactory, crmSecurity, mapper)
        {
            _messageService = messageService;
            _settingsManager = settingsManager;
            _global = global;
            _currencyProvider = currencyProvider;
        }


        //TABLE `crm_currency_rate` column `rate` DECIMAL(10,2) NOT NULL
        public const decimal MaxRateValue = (decimal)99999999.99;

        /// <summary>
        ///    Get the list of currency rates
        /// </summary>
        /// <short>Get currency rates list</short> 
        /// <category>Common</category>
        /// <returns>
        ///    List of currency rates
        /// </returns>
        [HttpGet(@"currency/rates")]
        public IEnumerable<CurrencyRateDto> GetCurrencyRates()
        {
            return _daoFactory.GetCurrencyRateDao().GetAll().ConvertAll(x => _mapper.Map<CurrencyRateDto>(x));
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
        [HttpGet(@"currency/rates/{id:int}")]
        public CurrencyRateDto GetCurrencyRate(int id)
        {
            if (id <= 0) throw new ArgumentException();

            var currencyRate = _daoFactory.GetCurrencyRateDao().GetByID(id);

            return _mapper.Map<CurrencyRateDto>(currencyRate);
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
        [HttpGet(@"currency/rates/{fromCurrency}/{toCurrency}")]
        public CurrencyRateDto GetCurrencyRate(string fromCurrency, string toCurrency)
        {
            if (string.IsNullOrEmpty(fromCurrency) || string.IsNullOrEmpty(toCurrency))
                throw new ArgumentException();

            var currencyRate = _daoFactory.GetCurrencyRateDao().GetByCurrencies(fromCurrency, toCurrency);

            return _mapper.Map<CurrencyRateDto>(currencyRate);
        }

        /// <summary>
        ///    Create new currency rate object
        /// </summary>
        /// <short></short>
        /// <category>Common</category>
        /// <returns></returns>
        [HttpPost(@"currency/rates")]
        public CurrencyRateDto CreateCurrencyRate(
            [FromBody] CreateCurrencyRateRequestDto inDto)
        {

            var rate = inDto.Rate;
            var fromCurrency = inDto.FromCurrency;
            var toCurrency = inDto.ToCurrency;

            ValidateRate(rate);

            ValidateCurrencies(new[] { fromCurrency, toCurrency });

            var currencyRate = new CurrencyRate
            {
                FromCurrency = fromCurrency,
                ToCurrency = toCurrency,
                Rate = rate
            };

            currencyRate.ID = _daoFactory.GetCurrencyRateDao().SaveOrUpdate(currencyRate);
            _messageService.Send(MessageAction.CurrencyRateUpdated, fromCurrency, toCurrency);

            return _mapper.Map<CurrencyRateDto>(currencyRate);
        }

        /// <summary>
        ///    Update currency rate object
        /// </summary>
        /// <short></short>
        /// <category>Common</category>
        /// <returns></returns>
        [HttpPut(@"currency/rates/{id:int}")]
        public CurrencyRateDto UpdateCurrencyRate(int id, string fromCurrency, string toCurrency, decimal rate)
        {
            if (id <= 0)
                throw new ArgumentException();

            ValidateRate(rate);

            ValidateCurrencies(new[] { fromCurrency, toCurrency });

            var currencyRate = _daoFactory.GetCurrencyRateDao().GetByID(id);

            if (currencyRate == null)
                throw new ArgumentException();

            currencyRate.FromCurrency = fromCurrency;
            currencyRate.ToCurrency = toCurrency;
            currencyRate.Rate = rate;

            currencyRate.ID = _daoFactory.GetCurrencyRateDao().SaveOrUpdate(currencyRate);
            _messageService.Send(MessageAction.CurrencyRateUpdated, fromCurrency, toCurrency);

            return _mapper.Map<CurrencyRateDto>(currencyRate);
        }

        /// <summary>
        ///    Set currency rates
        /// </summary>
        /// <short></short>
        /// <category>Common</category>
        /// <returns></returns>
        [HttpPost(@"currency/setrates")]
        public List<CurrencyRateDto> SetCurrencyRates(
             SetCurrencyRatesRequestDto inDto)
        {
            var currency = inDto.Currency;
            var rates = inDto.Rates;

            if (!_crmSecurity.IsAdmin)
                throw _crmSecurity.CreateSecurityException();

            if (string.IsNullOrEmpty(currency))
                throw new ArgumentException();

            ValidateCurrencyRates(rates);

            currency = currency.ToUpper();

            var crmSettings = _settingsManager.Load<CrmSettings>();
            var defaultCurrency = _currencyProvider.Get(_settingsManager.Load<CrmSettings>().DefaultCurrency);

            if (defaultCurrency.Abbreviation != currency)
            {
                var cur = _currencyProvider.Get(currency);

                if (cur == null)
                    throw new ArgumentException();

                _global.SaveDefaultCurrencySettings(cur);

                _messageService.Send(MessageAction.CrmDefaultCurrencyUpdated);
            }

            rates = _daoFactory.GetCurrencyRateDao().SetCurrencyRates(rates);

            foreach (var rate in rates)
            {
                _messageService.Send(MessageAction.CurrencyRateUpdated, rate.FromCurrency, rate.ToCurrency);
            }

            return rates.Select(x => _mapper.Map<CurrencyRateDto>(x)).ToList();
        }

        /// <summary>
        ///    Add currency rates
        /// </summary>
        /// <short></short>
        /// <category>Common</category>
        /// <returns></returns>
        [HttpPost(@"currency/addrates")]
        public List<CurrencyRateDto> AddCurrencyRates([FromBody] List<CurrencyRate> rates)
        {
            if (!_crmSecurity.IsAdmin)
                throw _crmSecurity.CreateSecurityException();

            ValidateCurrencyRates(rates);

            var existingRates = _daoFactory.GetCurrencyRateDao().GetAll();

            foreach (var rate in rates)
            {
                var exist = false;

                foreach (var existingRate in existingRates)
                {
                    if (rate.FromCurrency != existingRate.FromCurrency || rate.ToCurrency != existingRate.ToCurrency)
                        continue;

                    existingRate.Rate = rate.Rate;
                    _daoFactory.GetCurrencyRateDao().SaveOrUpdate(existingRate);
                    _messageService.Send(MessageAction.CurrencyRateUpdated, rate.FromCurrency, rate.ToCurrency);
                    exist = true;
                    break;
                }

                if (exist) continue;

                rate.ID = _daoFactory.GetCurrencyRateDao().SaveOrUpdate(rate);
                _messageService.Send(MessageAction.CurrencyRateUpdated, rate.FromCurrency, rate.ToCurrency);
                existingRates.Add(rate);
            }

            return existingRates.Select(x => _mapper.Map<CurrencyRateDto>(x)).ToList();
        }

        /// <summary>
        ///    Delete currency rate object
        /// </summary>
        /// <short></short>
        /// <category>Common</category>
        /// <returns></returns>
        [HttpDelete(@"currency/rates/{id:int}")]
        public CurrencyRateDto DeleteCurrencyRate(int id)
        {
            if (id <= 0)
                throw new ArgumentException();

            var currencyRate = _daoFactory.GetCurrencyRateDao().GetByID(id);

            if (currencyRate == null)
                throw new ArgumentException();

            _daoFactory.GetCurrencyRateDao().Delete(id);

            return _mapper.Map<CurrencyRateDto>(currencyRate);
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

            var available = _currencyProvider.GetAll().Select(x => x.Abbreviation);

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