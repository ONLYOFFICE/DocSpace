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

namespace ASC.Core.Tenants
{
    [Scope]
    class ConfigureTenantUtil : IConfigureNamedOptions<TenantUtil>
    {
        private readonly IOptionsSnapshot<TenantManager> _tenantManager;
        private readonly TimeZoneConverter _timeZoneConverter;

        public ConfigureTenantUtil(
            IOptionsSnapshot<TenantManager> tenantManager,
            TimeZoneConverter timeZoneConverter
            )
        {
            _tenantManager = tenantManager;
            _timeZoneConverter = timeZoneConverter;
        }

        public void Configure(string name, TenantUtil options)
        {
            Configure(options);
            options.TenantManager = _tenantManager.Get(name);
        }

        public void Configure(TenantUtil options)
        {
            options.TimeZoneConverter = _timeZoneConverter;
            options.TenantManager = _tenantManager.Value;
        }
    }

    [Scope(typeof(ConfigureTenantUtil))]
    public class TenantUtil
    {
        internal TenantManager TenantManager { get; set; }
        internal TimeZoneConverter TimeZoneConverter { get; set; }
        private TimeZoneInfo TimeZoneInfo =>
            _timeZoneInfo ??= TimeZoneConverter.GetTimeZone(TenantManager.GetCurrentTenant().TimeZone);

        private TimeZoneInfo _timeZoneInfo;

        public TenantUtil() { }

        public TenantUtil(TenantManager tenantManager, TimeZoneConverter timeZoneConverter)
        {
            TenantManager = tenantManager;
            TimeZoneConverter = timeZoneConverter;
        }

        public DateTime DateTimeFromUtc(DateTime utc) => DateTimeFromUtc(TimeZoneInfo, utc);

        public DateTime DateTimeFromUtc(string timeZone, DateTime utc) => 
            DateTimeFromUtc(TimeZoneConverter.GetTimeZone(timeZone), utc);

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
                local = local.AddHours(1); // hack
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
            return DateTimeNow(TimeZoneConverter.GetTimeZone(timeZone));
        }
    }
}