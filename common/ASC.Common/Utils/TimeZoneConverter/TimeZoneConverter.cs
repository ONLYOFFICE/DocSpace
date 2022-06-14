// (c) Copyright Ascensio System SIA 2010-2022
//
// This program is a free software product.
// You can redistribute it and/or modify it under the terms
// of the GNU Affero General Public License (AGPL) version 3 as published by the Free Software
// Foundation. In accordance with Section 7(a) of the GNU AGPL its Section 15 shall be amended
// to the effect that Ascensio System SIA expressly excludes the warranty of non-infringement of
// any third-party rights.
//
// This program is distributed WITHOUT ANY WARRANTY, without even the implied warranty
// of MERCHANTABILITY or FITNESS FOR A PARTICULAR  PURPOSE. For details, see
// the GNU AGPL at: http://www.gnu.org/licenses/agpl-3.0.html
//
// You can contact Ascensio System SIA at Lubanas st. 125a-25, Riga, Latvia, EU, LV-1021.
//
// The  interactive user interfaces in modified source and object code versions of the Program must
// display Appropriate Legal Notices, as required under Section 5 of the GNU AGPL version 3.
//
// Pursuant to Section 7(b) of the License you must retain the original Product logo when
// distributing the program. Pursuant to Section 7(e) we decline to grant you any rights under
// trademark law for use of our trademarks.
//
// All the Product's GUI elements, including illustrations and icon sets, as well as technical writing
// content are licensed under the terms of the Creative Commons Attribution-ShareAlike 4.0
// International. See the License terms at http://creativecommons.org/licenses/by-sa/4.0/legalcode

namespace ASC.Common.Utils;

[Singletone]
public class TimeZoneConverter
{
    private TimeZoneInfo _defaultTimeZone;
    private IEnumerable<MapZone> _mapZones;
    private bool _customMode;
    private bool _isMono;
    private Dictionary<string, string> _translations;
    private readonly IConfiguration _configuration;
    private readonly ILogger<TimeZoneConverter> _logger;

    public TimeZoneConverter(IConfiguration configuration, ILogger<TimeZoneConverter> logger)
    {
        _logger = logger;
        _configuration = configuration;

        InitMapZones();
        InitTranslations();
    }

    public string GetTimeZoneDisplayName(TimeZoneInfo tz)
    {
        var displayName = GetTimeZoneName(tz);
        if (!displayName.StartsWith("(UTC") && !displayName.StartsWith("UTC"))
        {
            if (tz.BaseUtcOffset != TimeSpan.Zero)
            {
                var offSet = tz.BaseUtcOffset < TimeSpan.Zero ? "-" : "+";
                var name = tz.BaseUtcOffset.ToString(@"hh\:mm");
                displayName = $"(UTC{offSet}{name}) {displayName}";
            }
            else
            {
                displayName = "(UTC) " + displayName;
            }
        }

        return displayName;
    }

    public string OlsonTzId2WindowsTzId(string olsonTimeZoneId, bool defaultIfNoMatch = true)
    {
        var mapZone = GetMapZoneByWindowsTzId(olsonTimeZoneId);

        if (mapZone != null)
        {
            return olsonTimeZoneId; //already Windows
        }

        mapZone = GetMapZoneByOlsonTzId(olsonTimeZoneId);

        if (mapZone != null)
        {
            return mapZone.WindowsTimeZoneId;
        }

        _logger.ErrorOlsonTimeZoneNotFound(olsonTimeZoneId);

        return defaultIfNoMatch ? "UTC" : null;
    }

    public string WindowsTzId2OlsonTzId(string windowsTimeZoneId, bool defaultIfNoMatch = true)
    {
        var mapZone = GetMapZoneByOlsonTzId(windowsTimeZoneId);

        if (mapZone != null)
        {
            return windowsTimeZoneId; //already Olson
        }

        mapZone = GetMapZoneByWindowsTzId(windowsTimeZoneId);

        if (mapZone != null)
        {
            return mapZone.OlsonTimeZoneId;
        }

        _logger.ErrorWindowsTimeZoneNotFound(windowsTimeZoneId);

        return defaultIfNoMatch ? "Etc/GMT" : null;
    }

    public TimeZoneInfo GetTimeZone(string timeZoneId, bool defaultIfNoMatch = true)
    {
        var defaultTimezone = GetTimeZoneDefault();

        if (string.IsNullOrEmpty(timeZoneId))
        {
            return defaultIfNoMatch ? defaultTimezone : null;
        }

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

                _logger.InformationTimeZoneNotFound(timeZoneId);

                return defaultIfNoMatch ? GetTimeZoneByOffset(timeZoneId) ?? defaultTimezone : null;
            }
            catch (Exception error)
            {
                _logger.ErrorGetTimeZone(error);

                return defaultIfNoMatch ? defaultTimezone : null;
            }
        }
        catch (Exception error)
        {
            _logger.ErrorGetTimeZone(error);

            return defaultIfNoMatch ? defaultTimezone : null;
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

        if (timeZone != null)
        {
            return timeZone;
        }

        var regex = new Regex(@"[+-][0-9]{2}:[0-9]{2}\b");

        var offsetStr = regex.Match(timeZoneId).Value.TrimStart('+');
        if (string.IsNullOrEmpty(offsetStr))
        {
            return null;
        }

        if (!TimeSpan.TryParse(offsetStr, out var offset))
        {
            return null;
        }

        return systemTimeZones.FirstOrDefault(tz => tz.BaseUtcOffset == offset);
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
            _logger.ErrorInitMapZones(error);
        }
    }

    public string GetTimeZoneName(TimeZoneInfo timeZone)
    {
        if (!_customMode)
        {
            return _isMono ? timeZone.Id : timeZone.DisplayName;
        }

        return _translations.ContainsKey(timeZone.Id) ? _translations[timeZone.Id] : timeZone.DisplayName;
    }

    private TimeZoneInfo GetTimeZoneDefault()
    {
        if (_defaultTimeZone == null)
        {
            try
            {
                var tz = TimeZoneInfo.Local;
                if (Path.DirectorySeparatorChar == '/')
                {
                    if (tz.StandardName == "UTC" || tz.StandardName == "UCT")
                    {
                        tz = TimeZoneInfo.Utc;
                    }
                    else
                    {
                        var id = string.Empty;

                        if (File.Exists("/etc/timezone"))
                        {
                            _isMono = true;
                            id = File.ReadAllText("/etc/timezone").Trim();
                        }

                        if (string.IsNullOrEmpty(id))
                        {
                            var psi = new ProcessStartInfo
                            {
                                FileName = "/bin/bash",
                                Arguments = "date +%Z",
                                RedirectStandardOutput = true,
                                UseShellExecute = false,
                            };

                            using var p = Process.Start(psi);

                            if (p.WaitForExit(1000))
                            {
                                id = p.StandardOutput.ReadToEnd();
                            }

                            p.Close();
                        }

                        if (!string.IsNullOrEmpty(id))
                        {
                            tz = TimeZoneInfo.GetSystemTimeZones().FirstOrDefault(z => z.Id == id) ?? tz;
                        }
                    }
                }

                _defaultTimeZone = tz;
            }
            catch (Exception)
            {
                // ignore
                _defaultTimeZone = TimeZoneInfo.Utc;
            }
        }

        return _defaultTimeZone;
    }

    private void InitTranslations()
    {
        try
        {
            _customMode = _configuration["core:custom-mode"] == "true";

            if (!_customMode)
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
            _logger.ErrorInitTranslations(error);
        }
    }

    private class MapZone
    {
        public string OlsonTimeZoneId { get; set; }
        public string WindowsTimeZoneId { get; set; }
        public string Territory { get; set; }
    }
}