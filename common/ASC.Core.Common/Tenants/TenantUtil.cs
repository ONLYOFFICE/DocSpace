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

namespace ASC.Core.Tenants;

[Scope]
public class TenantUtil
{
    internal TenantManager _tenantManager;
    internal TimeZoneConverter _timeZoneConverter;

    public TenantUtil() { }

    public TenantUtil(TenantManager tenantManager, TimeZoneConverter timeZoneConverter)
    {
        _tenantManager = tenantManager;
        _timeZoneConverter = timeZoneConverter;
    }

    private TimeZoneInfo _timeZoneInfo;
    private TimeZoneInfo TimeZoneInfo => _timeZoneInfo ??= _timeZoneConverter.GetTimeZone(_tenantManager.GetCurrentTenant().TimeZone);

    public DateTime DateTimeFromUtc(DateTime utc)
    {
        return DateTimeFromUtc(TimeZoneInfo, utc);
    }

    public DateTime DateTimeFromUtc(string timeZone, DateTime utc)
    {
        return DateTimeFromUtc(_timeZoneConverter.GetTimeZone(timeZone), utc);
    }

    public static DateTime DateTimeFromUtc(TimeZoneInfo timeZone, DateTime utc)
    {
        if (utc.Kind != DateTimeKind.Utc)
        {
            utc = DateTime.SpecifyKind(utc, DateTimeKind.Utc);
        }

        if (utc == DateTime.MinValue || utc == DateTime.MaxValue)
        {
            return utc;
        }

        return DateTime.SpecifyKind(TimeZoneInfo.ConvertTime(utc, TimeZoneInfo.Utc, timeZone), DateTimeKind.Local);
    }


    public DateTime DateTimeToUtc(DateTime local)
    {
        return DateTimeToUtc(TimeZoneInfo, local);
    }

    public static DateTime DateTimeToUtc(TimeZoneInfo timeZone, DateTime local)
    {
        if (local.Kind == DateTimeKind.Utc || local == DateTime.MinValue || local == DateTime.MaxValue)
        {
            return local;
        }

        if (timeZone.IsInvalidTime(DateTime.SpecifyKind(local, DateTimeKind.Unspecified)))
        {
            // hack
            local = local.AddHours(1);
        }

        return TimeZoneInfo.ConvertTimeToUtc(DateTime.SpecifyKind(local, DateTimeKind.Unspecified), timeZone);

    }

    public DateTime DateTimeNow()
    {
        return DateTimeNow(TimeZoneInfo);
    }

    public static DateTime DateTimeNow(TimeZoneInfo timeZone)
    {
        return DateTime.SpecifyKind(TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, timeZone), DateTimeKind.Local);
    }
    public DateTime DateTimeNow(string timeZone)
    {
        return DateTimeNow(_timeZoneConverter.GetTimeZone(timeZone));
    }
}
