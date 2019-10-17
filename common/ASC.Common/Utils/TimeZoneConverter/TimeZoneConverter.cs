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
using System.Reflection;
using System.Text.RegularExpressions;
using System.Xml.Linq;
using System.Xml.XPath;
using ASC.Common.Logging;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;

namespace ASC.Common.Utils
{
    public class TimeZoneConverter
    {
        private IEnumerable<MapZone> _mapZones;

        private bool _isR7;

        private Dictionary<string, string> _translations;

        public IConfiguration Configuration { get; }
        public ILog Log { get; }

        public TimeZoneConverter(IConfiguration configuration, IOptionsMonitor<LogNLog> option)
        {
            Log = option.CurrentValue;
            InitMapZones();
            InitTranslations();
            Configuration = configuration;
        }

        private void InitMapZones()
        {
            try
            {
                var assembly = Assembly.GetExecutingAssembly();

                using var stream = assembly.GetManifestResourceStream("ASC.Common.Utils.TimeZoneConverter.windowsZones.xml");
                var xml = XElement.Load(stream);
                _mapZones = from row in xml.XPathSelectElements("*/mapTimezones/mapZone")
                            let olsonTimeZones = row.Attribute("type").Value.Split(' ')
                            from olsonTimeZone in olsonTimeZones
                            select new MapZone
                            {
                                OlsonTimeZoneId = olsonTimeZone,
                                WindowsTimeZoneId = row.Attribute("other").Value,
                                Territory = row.Attribute("territory").Value
                            };
                _mapZones = _mapZones.ToList();
            }
            catch (Exception error)
            {
                _mapZones = new MapZone[0];
                Log.Error(error);
            }
        }

        public string OlsonTzId2WindowsTzId(string olsonTimeZoneId, bool defaultIfNoMatch = true)
        {
            var mapZone = GetMapZoneByWindowsTzId(olsonTimeZoneId);

            if (mapZone != null)
                return olsonTimeZoneId; //already Windows

            mapZone = GetMapZoneByOlsonTzId(olsonTimeZoneId);

            if (mapZone != null)
                return mapZone.WindowsTimeZoneId;

            Log.Error(string.Format("OlsonTimeZone {0} not found", olsonTimeZoneId));

            return defaultIfNoMatch ? "UTC" : null;
        }

        public string WindowsTzId2OlsonTzId(string windowsTimeZoneId, bool defaultIfNoMatch = true)
        {
            var mapZone = GetMapZoneByOlsonTzId(windowsTimeZoneId);

            if (mapZone != null)
                return windowsTimeZoneId; //already Olson

            mapZone = GetMapZoneByWindowsTzId(windowsTimeZoneId);

            if (mapZone != null)
                return mapZone.OlsonTimeZoneId;

            Log.Error(string.Format("WindowsTimeZone {0} not found", windowsTimeZoneId));

            return defaultIfNoMatch ? "Etc/GMT" : null;
        }

        public TimeZoneInfo GetTimeZone(string timeZoneId, bool defaultIfNoMatch = true)
        {
            try
            {
                return TimeZoneInfo.FindSystemTimeZoneById(timeZoneId);
            }
            catch (TimeZoneNotFoundException)
            {
                try
                {
                    var mapZone = GetMapZoneByOlsonTzId(timeZoneId);

                    if (mapZone != null)
                    {
                        return TimeZoneInfo.FindSystemTimeZoneById(mapZone.WindowsTimeZoneId);
                    }

                    mapZone = GetMapZoneByWindowsTzId(timeZoneId);

                    if (mapZone != null)
                    {
                        return TimeZoneInfo.FindSystemTimeZoneById(mapZone.OlsonTimeZoneId);
                    }

                    Log.InfoFormat("TimeZone {0} not found", timeZoneId);
                    return defaultIfNoMatch ? GetTimeZoneByOffset(timeZoneId) ?? TimeZoneInfo.Utc : null;
                }
                catch (Exception error)
                {
                    Log.Error(error);
                    return defaultIfNoMatch ? TimeZoneInfo.Utc : null;
                }
            }
            catch (Exception error)
            {
                Log.Error(error);
                return defaultIfNoMatch ? TimeZoneInfo.Utc : null;
            }
        }

        private MapZone GetMapZoneByOlsonTzId(string olsonTimeZoneId)
        {
            return _mapZones.FirstOrDefault(x =>
                                            x.OlsonTimeZoneId.Equals(olsonTimeZoneId, StringComparison.CurrentCultureIgnoreCase));
        }

        private MapZone GetMapZoneByWindowsTzId(string windowsTimeZoneId)
        {
            return _mapZones.FirstOrDefault(x =>
                                            x.WindowsTimeZoneId.Equals(windowsTimeZoneId, StringComparison.CurrentCultureIgnoreCase) &&
                                            x.Territory.Equals("001", StringComparison.CurrentCultureIgnoreCase));
        }

        private TimeZoneInfo GetTimeZoneByOffset(string timeZoneId)
        {
            var systemTimeZones = TimeZoneInfo.GetSystemTimeZones();

            var timeZone = systemTimeZones.FirstOrDefault(tz =>
                                                          tz.DisplayName == timeZoneId ||
                                                          tz.StandardName == timeZoneId ||
                                                          tz.DaylightName == timeZoneId);

            if (timeZone != null) return timeZone;

            var regex = new Regex(@"[+-][0-9]{2}:[0-9]{2}\b");

            var offsetStr = regex.Match(timeZoneId).Value.TrimStart('+');

            if (string.IsNullOrEmpty(offsetStr)) return null;


            if (!TimeSpan.TryParse(offsetStr, out var offset))
                return null;

            return systemTimeZones.FirstOrDefault(tz => tz.BaseUtcOffset == offset);
        }

        private void InitTranslations()
        {
            try
            {
                _isR7 = Configuration["core.r7office"] == "true";

                if (!_isR7)
                {
                    _translations = new Dictionary<string, string>();
                    return;
                }

                var assembly = Assembly.GetExecutingAssembly();

                using var stream = assembly.GetManifestResourceStream("ASC.Common.Utils.TimeZoneConverter.timeZoneNames.xml");
                var xml = XElement.Load(stream);
                _translations = (from row in xml.XPathSelectElements("*/zone")
                                 select new KeyValuePair<string, string>(row.Attribute("type").Value, row.Value)
                                ).ToDictionary(item => item.Key, item => item.Value);
            }
            catch (Exception error)
            {
                _translations = new Dictionary<string, string>();
                Log.Error(error);
            }
        }

        public string GetTimeZoneName(TimeZoneInfo timeZone)
        {
            if (!_isR7)
                return timeZone.DisplayName;

            return _translations.ContainsKey(timeZone.Id) ? _translations[timeZone.Id] : timeZone.DisplayName;
        }

        private class MapZone
        {
            public string OlsonTimeZoneId { get; set; }
            public string WindowsTimeZoneId { get; set; }
            public string Territory { get; set; }
        }
    }
}