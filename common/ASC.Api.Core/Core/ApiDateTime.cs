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
using System.ComponentModel;
using System.Globalization;
using System.Runtime.Serialization;
using ASC.Common.Utils;
using ASC.Core;
using Newtonsoft.Json;

namespace ASC.Api.Core
{
    [DataContract(Name = "date", Namespace = "")]
    [JsonConverter(typeof(ApiDateTimeConverter))]
    [TypeConverter(typeof(ApiDateTimeTypeConverter))]
    public class ApiDateTime : IComparable<ApiDateTime>, IComparable
    {
        internal static readonly string[] Formats = new[]
                                                       {
                                                           "o",
                                                           "yyyy'-'MM'-'dd'T'HH'-'mm'-'ss'.'fffffffK",
                                                           "yyyy'-'MM'-'dd'T'HH'-'mm'-'ss'.'fffK",
                                                           "yyyy'-'MM'-'dd'T'HH':'mm':'ss'.'fffK",
                                                           "yyyy'-'MM'-'dd'T'HH'-'mm'-'ssK",
                                                           "yyyy'-'MM'-'dd'T'HH':'mm':'ssK",
                                                           "yyyy'-'MM'-'dd"
                                                       };

        public ApiDateTime()
            : this(null, null, null)
        {
        }

        public ApiDateTime(TenantManager tenantManager, TimeZoneConverter timeZoneConverter, DateTime? dateTime)
            : this(tenantManager, dateTime, null, timeZoneConverter)
        {
        }

        public ApiDateTime(TenantManager tenantManager, DateTime? dateTime, TimeZoneInfo timeZone, TimeZoneConverter timeZoneConverter)
        {
            if (dateTime.HasValue && dateTime.Value > DateTime.MinValue && dateTime.Value < DateTime.MaxValue)
            {
                TenantManager = tenantManager;
                TimeZoneConverter = timeZoneConverter;
                SetDate(dateTime.Value, timeZone);
            }
            else
            {
                UtcTime = DateTime.MinValue;
                TimeZoneOffset = TimeSpan.Zero;
            }
        }

        public ApiDateTime(DateTime utcTime, TimeSpan offset)
        {
            UtcTime = new DateTime(utcTime.Ticks, DateTimeKind.Utc);
            TimeZoneOffset = offset;
        }

        public static ApiDateTime Parse(string data, TenantManager tenantManager, TimeZoneConverter timeZoneConverter)
        {
            return Parse(data, null, tenantManager, timeZoneConverter);
        }

        public static ApiDateTime Parse(string data, TimeZoneInfo tz, TenantManager tenantManager, TimeZoneConverter timeZoneConverter)
        {
            if (string.IsNullOrEmpty(data)) throw new ArgumentNullException("data");

            if (data.Length < 7) throw new ArgumentException("invalid date time format");

            var offsetPart = data.Substring(data.Length - 6, 6);
            if (DateTime.TryParseExact(data, Formats, CultureInfo.InvariantCulture, DateTimeStyles.AdjustToUniversal, out var dateTime))
            {
                //Parse time   
                var tzOffset = TimeSpan.Zero;
                if (offsetPart.Contains(":") && TimeSpan.TryParse(offsetPart.TrimStart('+'), out tzOffset))
                {
                    return new ApiDateTime(dateTime, tzOffset);
                }
                if (!data.EndsWith("Z", true, CultureInfo.InvariantCulture))
                {
                    if (tz == null)
                    {
                        tz = GetTimeZoneInfo(tenantManager, timeZoneConverter);
                    }
                    tzOffset = tz.GetUtcOffset(dateTime);
                    dateTime = dateTime.Subtract(tzOffset);
                }
                return new ApiDateTime(dateTime, tzOffset);

            }
            throw new ArgumentException("invalid date time format: " + data);
        }


        private void SetDate(DateTime value, TimeZoneInfo timeZone)
        {
            TimeZoneOffset = TimeSpan.Zero;
            UtcTime = DateTime.MinValue;

            if (timeZone == null)
            {
                timeZone = GetTimeZoneInfo(TenantManager, TimeZoneConverter);
            }

            //Hack
            if (timeZone.IsInvalidTime(new DateTime(value.Ticks, DateTimeKind.Unspecified)))
            {
                value = value.AddHours(1);
            }

            if (value.Kind == DateTimeKind.Local)
            {
                value = TimeZoneInfo.ConvertTimeToUtc(new DateTime(value.Ticks, DateTimeKind.Unspecified), timeZone);
            }
            if (value.Kind == DateTimeKind.Unspecified)
            {
                //Assume it's utc
                value = new DateTime(value.Ticks, DateTimeKind.Utc);
            }

            if (value.Kind == DateTimeKind.Utc)
            {
                UtcTime = value; //Set UTC time
                TimeZoneOffset = timeZone.GetUtcOffset(value);
            }

        }

        private static TimeZoneInfo GetTimeZoneInfo(TenantManager tenantManager, TimeZoneConverter timeZoneConverter)
        {
            var timeZone = TimeZoneInfo.Local;
            try
            {
                timeZone = timeZoneConverter.GetTimeZone(tenantManager.GetCurrentTenant().TimeZone);
            }
            catch (Exception)
            {
                //Tenant failed
            }
            return timeZone;
        }

        private string ToRoundTripString(DateTime date, TimeSpan offset)
        {
            var dateString = date.ToString("yyyy'-'MM'-'dd'T'HH':'mm':'ss'.'fffffff", CultureInfo.InvariantCulture);
            var offsetString = offset.Ticks == 0 ? "Z" : ((offset < TimeSpan.Zero) ? "-" : "+") + offset.ToString("hh\\:mm", CultureInfo.InvariantCulture);
            return dateString + offsetString;
        }

        public static ApiDateTime FromDate(TenantManager tenantManager, TimeZoneConverter timeZoneConverter, DateTime d)
        {
            var date = new ApiDateTime(tenantManager, timeZoneConverter, d);
            return date;
        }

        public static ApiDateTime FromDate(TenantManager tenantManager, TimeZoneConverter timeZoneConverter, DateTime? d)
        {
            if (d.HasValue)
            {
                var date = new ApiDateTime(tenantManager, timeZoneConverter, d);
                return date;
            }
            return null;
        }


        public static bool operator >(ApiDateTime left, ApiDateTime right)
        {
            if (ReferenceEquals(left, right)) return false;
            if (left == null) return false;

            return left.CompareTo(right) > 0;
        }
        public static bool operator >=(ApiDateTime left, ApiDateTime right)
        {
            if (ReferenceEquals(left, right)) return false;
            if (left == null) return false;

            return left.CompareTo(right) >= 0;
        }

        public static bool operator <=(ApiDateTime left, ApiDateTime right)
        {
            return !(left >= right);
        }

        public static bool operator <(ApiDateTime left, ApiDateTime right)
        {
            return !(left > right);
        }

        public static bool operator ==(ApiDateTime left, ApiDateTime right)
        {
            return Equals(left, right);
        }

        public static bool operator !=(ApiDateTime left, ApiDateTime right)
        {
            return !(left == right);
        }

        public static implicit operator DateTime(ApiDateTime d)
        {
            if (d == null) return DateTime.MinValue;
            return d.UtcTime;
        }

        public static implicit operator DateTime?(ApiDateTime d)
        {
            if (d == null) return null;
            return d.UtcTime;
        }

        public int CompareTo(DateTime other)
        {
            return CompareTo(new ApiDateTime(TenantManager, TimeZoneConverter, other));
        }

        public int CompareTo(ApiDateTime other)
        {
            if (other == null) return 1;
            return UtcTime.CompareTo(other.UtcTime);
        }

        public override bool Equals(object obj)
        {
            if (obj is null) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != typeof(ApiDateTime)) return false;
            return Equals((ApiDateTime)obj);
        }

        public bool Equals(ApiDateTime other)
        {
            if (other is null) return false;
            if (ReferenceEquals(this, other)) return true;
            return UtcTime.Equals(other.UtcTime) && TimeZoneOffset.Equals(other.TimeZoneOffset);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return UtcTime.GetHashCode() * 397 + TimeZoneOffset.GetHashCode();
            }
        }

        public int CompareTo(object obj)
        {
            if (obj is DateTime)
                return CompareTo((DateTime)obj);
            return obj is ApiDateTime ? CompareTo((ApiDateTime)obj) : 0;
        }

        public override string ToString()
        {
            var localUtcTime = UtcTime;

            if (!UtcTime.Equals(DateTime.MinValue))
                localUtcTime = UtcTime.Add(TimeZoneOffset);

            return ToRoundTripString(localUtcTime, TimeZoneOffset);
        }

        public DateTime UtcTime { get; private set; }
        public TimeSpan TimeZoneOffset { get; private set; }
        public TenantManager TenantManager { get; }
        public TimeZoneConverter TimeZoneConverter { get; }

        public static ApiDateTime GetSample()
        {
            return new ApiDateTime(DateTime.UtcNow, TimeSpan.Zero);
        }
    }

    public class ApiDateTimeTypeConverter : DateTimeConverter
    {
        public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value, Type destinationType)
        {
            if (destinationType == typeof(string))
                return value.ToString();
            return base.ConvertTo(context, culture, value, destinationType);
        }

        public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object value)
        {
            if (value is string)
            {
                return ApiDateTime.Parse((string)value, null, null);
            }
            if (value is DateTime)
            {
                return new ApiDateTime(null, null, (DateTime)value);
            }
            return base.ConvertFrom(context, culture, value);
        }
    }

    public class ApiDateTimeConverter : JsonConverter
    {
        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            if (value is ApiDateTime)
            {
                writer.WriteValue(value.ToString());
            }
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            if (reader.ValueType != null)
            {
                if (reader.ValueType.Name == "String")
                {
                    if (DateTime.TryParseExact(reader.Value?.ToString(), ApiDateTime.Formats, CultureInfo.InvariantCulture, DateTimeStyles.RoundtripKind, out var result))
                    {
                        return new ApiDateTime(result, TimeSpan.Zero);
                    }
                    else
                    {
                        return new ApiDateTime();
                    }
                }
                else if (reader.ValueType.Name == "DateTime")
                {
                    return new ApiDateTime((DateTime)reader.Value, TimeSpan.Zero);
                }
            }

            return DateTime.MinValue;
        }

        public override bool CanConvert(Type objectType)
        {
            return typeof(ApiDateTime).IsAssignableFrom(objectType);
        }
    }
}