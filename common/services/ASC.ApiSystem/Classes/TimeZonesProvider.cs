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

namespace ASC.ApiSystem.Classes;

[Singletone]
public class TimeZonesProvider
{
    public ILogger<TimeZonesProvider> Log { get; }

    private CommonConstants CommonConstants { get; }

    public TimeZonesProvider(ILogger<TimeZonesProvider> logger, CommonConstants commonConstants)
    {
        Log = logger;

        CommonConstants = commonConstants;
    }

    #region Private

    private static readonly Dictionary<string, KeyValuePair<string, string>> TimeZones = new Dictionary<string, KeyValuePair<string, string>>
    {
        { "", new KeyValuePair<string, string>("Europe/London", "GMT Standard Time") },
        { "fr", new KeyValuePair<string, string>("Europe/Paris", "Romance Standard Time") },
        { "es", new KeyValuePair<string, string>("Europe/Madrid", "Romance Standard Time")},
        { "de", new KeyValuePair<string, string>("Europe/Berlin", "W. Europe Standard Time") },
        { "ru", new KeyValuePair<string, string>("Europe/Moscow", "Russian Standard Time") },
        { "lv", new KeyValuePair<string, string>("Europe/Riga", "FLE Standard Time") },
        { "pt", new KeyValuePair<string, string>("America/Cuiaba", "Central Brazilian Standard Time") },
        { "it", new KeyValuePair<string, string>("Europe/Rome", "Central European Standard Time") },
        { "tr", new KeyValuePair<string, string>("Europe/Istanbul", "GTB Standard Time") },

        { "id", new KeyValuePair<string, string>("Europe/London", "GMT Standard Time") },
        { "zh", new KeyValuePair<string, string>("Asia/Shanghai", "China Standard Time") },
        { "ja", new KeyValuePair<string, string>("Asia/Tokyo", "Tokyo Standard Time") },
        { "ko", new KeyValuePair<string, string>("Asia/Seoul", "Korea Standard Time") },
        { "az", new KeyValuePair<string, string>("Asia/Baku", "Azerbaijan Standard Time") },
        { "cs", new KeyValuePair<string, string>("Europe/Warsaw", "Central European Standard Time") },
        { "el", new KeyValuePair<string, string>("Europe/Warsaw", "Central European Standard Time") },
        { "fi", new KeyValuePair<string, string>("Europe/Warsaw", "Central European Standard Time") },
        { "pl", new KeyValuePair<string, string>("Europe/Warsaw", "Central European Standard Time") },
        { "uk", new KeyValuePair<string, string>("Europe/Kiev", "FLE Standard Time") },
        { "vi", new KeyValuePair<string, string>("Asia/Shanghai", "China Standard Time") }
    };

    private static readonly Dictionary<string, CultureInfo> CultureUiMap = new Dictionary<string, CultureInfo>
    {
        { "", CultureInfo.GetCultureInfo("en-US") },
        { "fr", CultureInfo.GetCultureInfo("fr-FR") },
        { "es", CultureInfo.GetCultureInfo("es-ES") },
        { "de", CultureInfo.GetCultureInfo("de-DE") },
        { "ru", CultureInfo.GetCultureInfo("ru-RU") },
        { "lv", CultureInfo.GetCultureInfo("lv-LV") },
        { "pt", CultureInfo.GetCultureInfo("pt-BR") },
        { "it", CultureInfo.GetCultureInfo("it-IT") },
        { "tr", CultureInfo.GetCultureInfo("tr-TR") },

        { "id", CultureInfo.GetCultureInfo("id-ID") },
        { "zh", CultureInfo.GetCultureInfo("zh-CN") },
        { "ja", CultureInfo.GetCultureInfo("ja-JP") },
        { "ko", CultureInfo.GetCultureInfo("ko-KR") },
        { "az", CultureInfo.GetCultureInfo("az-Latn-AZ") },
        { "cs", CultureInfo.GetCultureInfo("cs-CZ") },
        { "el", CultureInfo.GetCultureInfo("el-GR") },
        { "fi", CultureInfo.GetCultureInfo("fi-FI") },
        { "pl", CultureInfo.GetCultureInfo("pl-PL") },
        { "uk", CultureInfo.GetCultureInfo("uk-UA") },
        { "vi", CultureInfo.GetCultureInfo("vi-VN") }
    };

    #endregion


    #region Public

    public TimeZoneInfo GetCurrentTimeZoneInfo(string languageKey)
    {
        var time = TimeZones.ContainsKey(languageKey) ? TimeZones[languageKey] : TimeZones[""];
        try
        {
            try
            {
                return TimeZoneInfo.FindSystemTimeZoneById(time.Value);
            }
            catch (TimeZoneNotFoundException)
            {
                return TimeZoneInfo.FindSystemTimeZoneById(time.Key);
            }
        }
        catch (Exception e)
        {
            Log.LogError(e, "GetCurrentTimeZoneInfo");

            return TimeZoneInfo.Utc;
        }
    }

    public CultureInfo GetCurrentCulture(string languageKey)
    {
        if (string.IsNullOrEmpty(languageKey))
        {
            return CommonConstants.DefaultCulture;
        }

        var culture = CultureUiMap.ContainsKey(languageKey) ? CultureUiMap[languageKey] : null;

        return culture ?? CommonConstants.DefaultCulture;
    }

    #endregion
}
