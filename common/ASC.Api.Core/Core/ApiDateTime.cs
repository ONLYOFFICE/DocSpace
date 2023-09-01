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

using System.ComponentModel;

namespace ASC.Api.Core;

[TypeConverter(typeof(ApiDateTimeTypeConverter))]
[JsonConverter(typeof(ApiDateTimeConverter))]
public sealed class ApiDateTime : IComparable<ApiDateTime>, IComparable
{
    public DateTime UtcTime { get; private set; }
    public TimeSpan TimeZoneOffset { get; private set; }

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

    private readonly TenantManager _tenantManager;
    private readonly TimeZoneConverter _timeZoneConverter;

    public ApiDateTime() : this(null, null, null) { }

    public ApiDateTime(
        TenantManager tenantManager,
        TimeZoneConverter timeZoneConverter,
        DateTime? dateTime)
        : this(tenantManager, dateTime, null, timeZoneConverter) { }

    public ApiDateTime(
        TenantManager tenantManager,
        DateTime? dateTime,
        TimeZoneInfo timeZone,
        TimeZoneConverter timeZoneConverter)
    {
        if (dateTime.HasValue && dateTime.Value > DateTime.MinValue && dateTime.Value < DateTime.MaxValue)
        {
            _tenantManager = tenantManager;
            _timeZoneConverter = timeZoneConverter;
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
        ArgumentNullOrEmptyException.ThrowIfNullOrEmpty(data);

        var offsetPart = data.Substring(data.Length - 6, 6);
        if (DateTime.TryParseExact(data, Formats, CultureInfo.InvariantCulture, DateTimeStyles.AdjustToUniversal, out var dateTime))
        {
            //Parse time   
            var tzOffset = TimeSpan.Zero;
            if (offsetPart.Contains(':') && TimeSpan.TryParse(offsetPart.TrimStart('+'), out tzOffset))
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
            timeZone = GetTimeZoneInfo(_tenantManager, _timeZoneConverter);
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
            value = new DateTime(value.Ticks, DateTimeKind.Utc); //Assume it's utc
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
        var offsetString = offset.Ticks == 0
            ? "Z" : ((offset < TimeSpan.Zero)
            ? "-" : "+") + offset.ToString("hh\\:mm", CultureInfo.InvariantCulture);

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
        if (ReferenceEquals(left, right))
        {
            return false;
        }
        if (left == null)
        {
            return false;
        }

        return left.CompareTo(right) > 0;
    }

    public static bool operator >=(ApiDateTime left, ApiDateTime right)
    {
        if (ReferenceEquals(left, right))
        {
            return false;
        }
        if (left == null)
        {
            return false;
        }

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
        if (d == null)
        {
            return DateTime.MinValue;
        }

        return d.UtcTime;
    }

    public static implicit operator DateTime?(ApiDateTime d)
    {
        if (d == null)
        {
            return null;
        }

        return d.UtcTime;
    }

    public int CompareTo(DateTime other)
    {
        return CompareTo(new ApiDateTime(_tenantManager, _timeZoneConverter, other));
    }

    public int CompareTo(ApiDateTime other)
    {
        if (other == null)
        {
            return 1;
        }

        return UtcTime.CompareTo(other.UtcTime);
    }

    public override bool Equals(object obj)
    {
        if (obj is null)
        {
            return false;
        }

        if (ReferenceEquals(this, obj))
        {
            return true;
        }

        if (obj is not ApiDateTime)
        {
            return false;
        }

        return Equals((ApiDateTime)obj);
    }

    public bool Equals(ApiDateTime other)
    {
        if (other is null)
        {
            return false;
        }
        if (ReferenceEquals(this, other))
        {
            return true;
        }

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
        if (obj is DateTime dateTime)
        {
            return CompareTo(dateTime);
        }

        return obj is ApiDateTime apiDateTime ? CompareTo(apiDateTime) : 0;
    }

    public override string ToString()
    {
        var localUtcTime = UtcTime;
        if (!UtcTime.Equals(DateTime.MinValue))
        {
            localUtcTime = UtcTime.Add(TimeZoneOffset);
        }

        return ToRoundTripString(localUtcTime, TimeZoneOffset);
    }

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
        {
            return value.ToString();
        }

        return base.ConvertTo(context, culture, value, destinationType);
    }

    public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object value)
    {
        if (value is string @string)
        {
            return ApiDateTime.Parse(@string, null, null);
        }
        if (value is DateTime time)
        {
            return new ApiDateTime(null, null, time);
        }

        return base.ConvertFrom(context, culture, value);
    }
}

public class ApiDateTimeConverter : JsonConverter<ApiDateTime>
{
    public override ApiDateTime Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        if (reader.TryGetDateTime(out var result))
        {
            return new ApiDateTime(result, TimeSpan.Zero);
        }
        else
        {
            if (DateTime.TryParseExact(reader.GetString(), ApiDateTime.Formats,
                CultureInfo.InvariantCulture, DateTimeStyles.RoundtripKind, out var dateTime))
            {
                return new ApiDateTime(dateTime, TimeSpan.Zero);
            }
            else
            {
                return new ApiDateTime();
            }
        }
    }

    public override void Write(Utf8JsonWriter writer, ApiDateTime value, JsonSerializerOptions options)
    {
        writer.WriteStringValue(value.ToString());
    }
}

[Scope]
public class ApiDateTimeHelper
{
    private readonly TenantManager _tenantManager;
    private readonly TimeZoneConverter _timeZoneConverter;

    public ApiDateTimeHelper(TenantManager tenantManager, TimeZoneConverter timeZoneConverter)
    {
        _tenantManager = tenantManager;
        _timeZoneConverter = timeZoneConverter;
    }

    public ApiDateTime Get(DateTime? from)
    {
        return ApiDateTime.FromDate(_tenantManager, _timeZoneConverter, from);
    }
}