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
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text.RegularExpressions;

using ASC.Common;
using ASC.Common.Logging;
using ASC.Core.Common.Settings;
using ASC.CRM.Core;
using ASC.CRM.Core.Dao;

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;

namespace ASC.Web.CRM.Classes
{
    [Scope]
    public class CurrencyProvider
    {
        private readonly ILog _log;
        private readonly object _syncRoot = new object();
        private readonly Dictionary<String, CurrencyInfo> _currencies;
        private Dictionary<string, decimal> _exchangeRates;
        private DateTime _publisherDate;
        private const String _formatDate = "yyyy-MM-ddTHH:mm:ss.fffffffK";

        public CurrencyProvider(IOptionsMonitor<ILog> logger,
                                IConfiguration configuration,
                                SettingsManager settingsManager,
                                DaoFactory daoFactory)
        {
            _log = logger.Get("ASC");
            Configuration = configuration;
            SettingsManager = settingsManager;

            var daocur = daoFactory.GetCurrencyInfoDao();
            var currencies = daocur.GetAll();

            if (currencies == null || currencies.Count == 0)
            {
                currencies = new List<CurrencyInfo>
                    {
                        new CurrencyInfo("Currency_UnitedStatesDollar", "USD", "$", "US", true, true)
                    };
            }

            _currencies = currencies.ToDictionary(c => c.Abbreviation);

        }

        public IConfiguration Configuration { get; }
        public SettingsManager SettingsManager { get; }

        public DateTime GetPublisherDate
        {
            get
            {
                TryToReadPublisherDate(GetExchangesTempPath());
                return _publisherDate;
            }
        }

        public CurrencyInfo Get(string currencyAbbreviation)
        {
            if (!_currencies.ContainsKey(currencyAbbreviation))
                return null;

            return _currencies[currencyAbbreviation];
        }

        public List<CurrencyInfo> GetAll()
        {
            return _currencies.Values.OrderBy(v => v.Abbreviation).ToList();
        }

        public List<CurrencyInfo> GetBasic()
        {
            return _currencies.Values.Where(c => c.IsBasic).OrderBy(v => v.Abbreviation).ToList();
        }

        public List<CurrencyInfo> GetOther()
        {
            return _currencies.Values.Where(c => !c.IsBasic).OrderBy(v => v.Abbreviation).ToList();
        }

        public Dictionary<CurrencyInfo, Decimal> MoneyConvert(CurrencyInfo baseCurrency)
        {
            if (baseCurrency == null) throw new ArgumentNullException("baseCurrency");
            if (!_currencies.ContainsKey(baseCurrency.Abbreviation)) throw new ArgumentOutOfRangeException("baseCurrency", "Not found.");

            var result = new Dictionary<CurrencyInfo, Decimal>();
            var rates = GetExchangeRates();
            foreach (var ci in GetAll())
            {

                if (baseCurrency.Title == ci.Title)
                {
                    result.Add(ci, 1);

                    continue;
                }

                var key = String.Format("{1}/{0}", baseCurrency.Abbreviation, ci.Abbreviation);

                if (!rates.ContainsKey(key))
                    continue;

                result.Add(ci, rates[key]);
            }
            return result;
        }


        public bool IsConvertable(String abbreviation)
        {
            var findedItem = _currencies.Keys.ToList().Find(item => String.Compare(abbreviation, item) == 0);

            if (findedItem == null)
                throw new ArgumentException(abbreviation);

            return _currencies[findedItem].IsConvertable;
        }

        public Decimal MoneyConvert(decimal amount, string from, string to)
        {
            if (string.IsNullOrEmpty(from) || string.IsNullOrEmpty(to) || string.Compare(from, to, true) == 0) return amount;

            var rates = GetExchangeRates();

            if (from.Contains('-')) from = new RegionInfo(from).ISOCurrencySymbol;
            if (to.Contains('-')) to = new RegionInfo(to).ISOCurrencySymbol;
            var key = string.Format("{0}/{1}", to, from);

            return Math.Round(rates[key] * amount, 4, MidpointRounding.AwayFromZero);
        }

        public decimal MoneyConvertToDefaultCurrency(decimal amount, string from)
        {

            var crmSettings = SettingsManager.Load<CrmSettings>();
            var defaultCurrency = Get(crmSettings.DefaultCurrency);

            return MoneyConvert(amount, from, defaultCurrency.Abbreviation);
        }

        private bool ObsoleteData()
        {
            return _exchangeRates == null || (DateTime.UtcNow.Date.Subtract(_publisherDate.Date).Days > 0);
        }

        private string GetExchangesTempPath()
        {
            return Path.Combine(Path.GetTempPath(), Path.Combine("onlyoffice", "exchanges"));
        }

        private readonly Regex CurRateRegex = new Regex("<td id=\"(?<Currency>[a-zA-Z]{3})\">(?<Rate>[\\d\\.]+)</td>");

        private Dictionary<String, Decimal> GetExchangeRates()
        {
            if (ObsoleteData())
            {
                lock (_syncRoot)
                {
                    if (ObsoleteData())
                    {
                        try
                        {
                            _exchangeRates = new Dictionary<string, decimal>();

                            var tmppath = GetExchangesTempPath();

                            TryToReadPublisherDate(tmppath);



                            var updateEnable = Configuration["crm:update:currency:info:enable"] != "false";
                            var ratesUpdatedFlag = false;

                            foreach (var ci in _currencies.Values.Where(c => c.IsConvertable))
                            {
                                var filepath = Path.Combine(tmppath, ci.Abbreviation + ".html");

                                if (updateEnable && 0 < (DateTime.UtcNow.Date - _publisherDate.Date).TotalDays || !File.Exists(filepath))
                                {
                                    var filepath_temp = Path.Combine(tmppath, ci.Abbreviation + "_temp.html");

                                    DownloadCurrencyPage(ci.Abbreviation, filepath_temp);

                                    if (File.Exists(filepath_temp))
                                    {
                                        if (TryGetRatesFromFile(filepath_temp, ci))
                                        {
                                            ratesUpdatedFlag = true;
                                            File.Copy(filepath_temp, filepath, true);
                                        }
                                        File.Delete(filepath_temp);
                                        continue;
                                    }
                                }

                                if (File.Exists(filepath) && TryGetRatesFromFile(filepath, ci))
                                {
                                    ratesUpdatedFlag = true;
                                }
                            }

                            if (ratesUpdatedFlag)
                            {
                                _publisherDate = DateTime.UtcNow;
                                WritePublisherDate(tmppath);
                            }

                        }
                        catch (Exception error)
                        {
                            _log.Error(error);
                            _publisherDate = DateTime.UtcNow;
                        }
                    }
                }
            }

            return _exchangeRates;
        }

        private bool TryGetRatesFromFile(string filepath, CurrencyInfo curCI)
        {
            var success = false;
            var currencyLines = File.ReadAllLines(filepath);
            for (var i = 0; i < currencyLines.Length; i++)
            {
                var line = currencyLines[i];

                if (line.Contains("id=\"major-currency-table\"") || line.Contains("id=\"minor-currency-table\"") || line.Contains("id=\"exotic-currency-table\""))
                {
                    var currencyInfos = CurRateRegex.Matches(line);

                    if (currencyInfos.Count > 0)
                    {
                        foreach (var curInfo in currencyInfos)
                        {
                            _exchangeRates.Add(
                                String.Format("{0}/{1}", (curInfo as Match).Groups["Currency"].Value.Trim(), curCI.Abbreviation),
                                Convert.ToDecimal((curInfo as Match).Groups["Rate"].Value.Trim(), CultureInfo.InvariantCulture.NumberFormat));

                            success = true;
                        }
                    }
                }
            }

            return success;
        }


        private void TryToReadPublisherDate(string tmppath)
        {
            if (_publisherDate == default(DateTime))
            {
                try
                {
                    var timefile = Path.Combine(tmppath, "last.time");
                    if (File.Exists(timefile))
                    {
                        var dateFromFile = File.ReadAllText(timefile);
                        _publisherDate = DateTime.ParseExact(dateFromFile, _formatDate, null);
                    }
                }
                catch (Exception err)
                {
                    _log.Error(err);
                }
            }
        }

        private void WritePublisherDate(string tmppath)
        {
            try
            {
                var timefile = Path.Combine(tmppath, "last.time");
                File.WriteAllText(timefile, _publisherDate.ToString(_formatDate));
            }
            catch (Exception err)
            {
                _log.Error(err);
            }
        }

        private void DownloadCurrencyPage(string currency, string filepath)
        {

            try
            {
                var dir = Path.GetDirectoryName(filepath);

                if (!Directory.Exists(dir))
                {
                    Directory.CreateDirectory(dir);
                }

                var destinationURI = new Uri(String.Format("https://themoneyconverter.com/{0}/{0}.aspx", currency));

                var request = new HttpRequestMessage();
                request.RequestUri = destinationURI;
                request.Method = HttpMethod.Get;
                request.Headers.Add("User-Agent", "Mozilla/5.0 (Windows NT 6.1; rv:8.0) Gecko/20100101 Firefox/8.0");

                var handler = new HttpClientHandler();
                handler.AllowAutoRedirect = true;
                handler.MaxAutomaticRedirections = 2;
                handler.UseDefaultCredentials = true;

                var httpClient = new HttpClient(handler);
                using var response = httpClient.Send(request);
                using (var responseStream = new StreamReader(response.Content.ReadAsStream()))
                {
                    var data = responseStream.ReadToEnd();

                    File.WriteAllText(filepath, data);
                }
            }
            catch (Exception error)
            {
                _log.Error(error);
            }
        }

    }
}
