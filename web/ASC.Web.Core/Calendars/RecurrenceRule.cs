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

namespace ASC.Web.Core.Calendars;

internal static class DateTimeExtension
{
    public static int GetWeekOfYear(this DateTime date, DayOfWeek firstDayOfWeek)
    {
        return Thread.CurrentThread.CurrentCulture.Calendar.GetWeekOfYear(date,
                            CalendarWeekRule.FirstFourDayWeek, firstDayOfWeek);
    }

    public static int GetWeekOfYearCount(this DateTime date, DayOfWeek firstDayOfWeek)
    {
        return new DateTime(date.Year, 12, 31).GetWeekOfYear(firstDayOfWeek);
    }

    public static int GetDaysInMonth(this DateTime date)
    {
        return Thread.CurrentThread.CurrentCulture.Calendar.GetDaysInMonth(date.Year, date.Month);
    }

    public static int GetDaysInYear(this DateTime date)
    {
        return Thread.CurrentThread.CurrentCulture.Calendar.GetDaysInYear(date.Year);
    }

    public static int GetDayOfWeekInMonth(this DateTime date)
    {
        var count = 0;
        var d = date;
        while (date.Month == d.Month)
        {
            count++;
            d = d.AddDays(-7);
        }
        d = date.AddDays(7);
        while (date.Month == d.Month)
        {
            count++;
            d = d.AddDays(7);
        }
        return count;
    }

    public static string ToShortString(this DateTime targetDateTime)
    {
        return $"{targetDateTime.ToShortDateString()} {targetDateTime.ToShortTimeString()}";
    }

}

public enum Frequency
{
    Never = 0,
    Daily = 3,
    Weekly = 4,
    Monthly = 5,
    Yearly = 6,
    Secondly = 7,
    Minutely = 8,
    Hourly = 9
}

public class RecurrenceRule : IICalFormatView, ICloneable
{
    public static RecurrenceRule Parse(EventRepeatType repeatType)
    {
        return repeatType switch
        {
            EventRepeatType.EveryDay => new RecurrenceRule() { Freq = Frequency.Daily },

            EventRepeatType.EveryMonth => new RecurrenceRule() { Freq = Frequency.Monthly },

            EventRepeatType.EveryWeek => new RecurrenceRule() { Freq = Frequency.Weekly },

            EventRepeatType.EveryYear => new RecurrenceRule() { Freq = Frequency.Yearly },

            EventRepeatType.Never => new RecurrenceRule() { Freq = Frequency.Never },

            _ => new RecurrenceRule() { Freq = Frequency.Never },
        };
    }
    public class WeekDay
    {
        public string Id { get; set; }
        public int? Number { get; set; }
        public DayOfWeek DayOfWeek
        {
            get
            {
                return (Id ?? "").ToLower() switch
                {
                    "su" => DayOfWeek.Sunday,
                    "mo" => DayOfWeek.Monday,
                    "tu" => DayOfWeek.Tuesday,
                    "we" => DayOfWeek.Wednesday,
                    "th" => DayOfWeek.Thursday,
                    "fr" => DayOfWeek.Friday,
                    "sa" => DayOfWeek.Saturday,
                    _ => DayOfWeek.Monday,
                };
            }
        }

        private WeekDay() { }

        public List<DateTime> GetDates(DateTime startDate, bool monthly)
        {
            var dates = new List<DateTime>();
            var date = startDate;
            var count = 0;
            while ((monthly && date.Month == startDate.Month) || (!monthly && date.Year == startDate.Year))
            {
                if (date.DayOfWeek == this.DayOfWeek)
                {
                    count++;
                    if (!monthly && (!this.Number.HasValue || count == this.Number || this.Number == (count - (date.GetWeekOfYearCount(DayOfWeek.Monday) + 1))))
                    {
                        dates.Add(date);
                    }
                    else if (monthly && (!this.Number.HasValue || count == this.Number || this.Number == (count - (date.GetDayOfWeekInMonth() + 1))))
                    {
                        dates.Add(date);
                    }

                    date = date.AddDays(7);
                }
                else
                {
                    date = date.AddDays(1);
                }
            }
            return dates;
        }

        public static WeekDay ParseWeekDay(string iCalStrValue)
        {
            var d = new WeekDay();

            if (iCalStrValue.Length > 2)
            {
                d.Id = iCalStrValue.Substring(iCalStrValue.Length - 2).ToLower();
                d.Number = Convert.ToInt32(iCalStrValue[0..^2]);
            }
            else
            {
                d.Id = iCalStrValue;
                d.Number = null;
            }
            return d;
        }

        public override string ToString()
        {
            return (Number.HasValue ? Number.ToString() : "") + this.Id;
        }
    }

    public Frequency Freq { get; set; }
    public DateTime Until { get; set; }
    public int Count { get; set; }
    public int Interval { get; set; }

    public int[] BySecond { get; set; } //0 - 59
    public int[] ByMinute { get; set; } //0 - 59
    public int[] ByHour { get; set; } //0 - 23

    public WeekDay[] ByDay { get; set; } //
    public int[] ByMonthDay { get; set; } //1 -31 +-
    public int[] ByYearDay { get; set; } //1 -366 +-
    public int[] ByWeekNo { get; set; } // 1 - 53 +- 
    public int[] ByMonth { get; set; } // 1 - 12     

    public int[] BySetPos { get; set; } //- 366 +-
    public WeekDay WKST { get; set; }

    public struct ExDate
    {
        public DateTime Date { get; set; }
        public bool IsDateTime { get; set; }
    }

    public List<ExDate> ExDates { get; set; }

    public RecurrenceRule()
    {
        this.Freq = Frequency.Never;
        this.Until = DateTime.MinValue;
        this.Count = -1;
        this.Interval = 1;
        this.WKST = WeekDay.ParseWeekDay("mo");
        this.ExDates = new List<ExDate>();
    }

    private bool CheckDate(DateTime d)
    {
        if (ByMonth != null && !ByMonth.Contains(d.Month))
        {
            return false;
        }

        //only for YEARLY
        if (ByWeekNo != null)
        {
            var weekOfYear = d.GetWeekOfYear(this.WKST.DayOfWeek);
            if (!ByWeekNo.Contains(weekOfYear) && !ByWeekNo.Contains(weekOfYear - (d.GetWeekOfYearCount(this.WKST.DayOfWeek) + 1)))
            {
                return false;
            }
        }

        if (ByYearDay != null && !ByYearDay.Contains(d.DayOfYear) && !ByYearDay.Contains(d.DayOfYear - (d.GetDaysInYear() + 1)))
        {
            return false;
        }

        if (ByMonthDay != null && !ByMonthDay.Contains(d.Day) && !ByMonthDay.Contains(d.Day - d.GetDaysInMonth() + 1))
        {
            return false;
        }

        if (ByDay != null && !ByDay.Any(item => item.DayOfWeek == d.DayOfWeek))
        {
            return false;
        }

        return true;
    }

    public List<DateTime> GetDates(DateTime utcStartDate, TimeZoneInfo eventTimeZone, bool isAllDayLong, DateTime fromDate, int maxCount)
    {
        return GetDates(utcStartDate, eventTimeZone, isAllDayLong, fromDate, DateTime.MaxValue, maxCount);
    }
    public List<DateTime> GetDates(DateTime utcStartDate, TimeZoneInfo eventTimeZone, bool isAllDayLong, DateTime fromDate, DateTime toDate)
    {
        return GetDates(utcStartDate, eventTimeZone, isAllDayLong, fromDate, toDate, int.MaxValue);
    }
    public List<DateTime> GetDates(DateTime utcStartDate, TimeZoneInfo eventTimeZone, bool isAllDayLong, DateTime fromDate, DateTime toDate, int maxCount, bool removeExDates = true)
    {
        var dates = new List<DateTime>();
        var utcStartDateOffset = isAllDayLong ? TimeSpan.Zero : eventTimeZone.GetUtcOffset(utcStartDate);
        var endDate = this.Until == DateTime.MinValue ? toDate : (toDate > this.Until ? this.Until : toDate);

        //push start date           
        dates.Add(utcStartDate);

        DateTime d;
        switch (Freq)
        {
            case Frequency.Secondly:

                #region Secondly
                d = utcStartDate.AddSeconds(this.Interval);
                while (d <= endDate && CheckCount(dates, fromDate, maxCount))
                {
                    if (CheckDate(d) && (ByHour == null || (ByHour != null && ByHour.Contains(d.Hour)))
                        && (ByMinute == null || (ByMinute != null && ByMinute.Contains(d.Minute)))
                        && (BySecond == null || (BySecond != null && BySecond.Contains(d.Second))))
                    {

                        if (d >= utcStartDate && d <= endDate && !dates.Contains(d))
                        {
                            dates.Add(d);
                        }
                    }

                    d = d.AddMinutes(this.Interval);
                }
                break;
            #endregion

            case Frequency.Minutely:

                #region Minutely
                d = utcStartDate.AddMinutes(this.Interval);
                while (d <= endDate && CheckCount(dates, fromDate, maxCount))
                {
                    if (CheckDate(d) && (ByHour == null || (ByHour != null && ByHour.Contains(d.Hour)))
                        && (ByMinute == null || (ByMinute != null && ByMinute.Contains(d.Minute))))
                    {
                        //seconds
                        var seconds = new List<int>();
                        if (BySecond != null)
                        {
                            seconds.AddRange(BySecond);
                        }
                        else
                        {
                            seconds.Add(d.Second);
                        }

                        foreach (var s in seconds)
                        {
                            var newDate = new DateTime(d.Year, d.Month, d.Day, d.Hour, d.Minute, s);
                            if (newDate >= utcStartDate && newDate <= endDate && !dates.Contains(newDate))
                            {
                                dates.Add(newDate);
                            }
                        }

                    }

                    d = d.AddMinutes(this.Interval);
                }
                break;
            #endregion

            case Frequency.Hourly:

                #region Hourly
                d = utcStartDate.AddHours(this.Interval);
                while (d <= endDate && CheckCount(dates, fromDate, maxCount))
                {
                    if (CheckDate(d) && (ByHour == null || (ByHour != null && ByHour.Contains(d.Hour))))
                    {
                        //minutes
                        var minutes = new List<int>();
                        if (ByMinute != null)
                        {
                            minutes.AddRange(ByMinute);
                        }
                        else
                        {
                            minutes.Add(d.Minute);
                        }

                        //seconds
                        var seconds = new List<int>();
                        if (BySecond != null)
                        {
                            seconds.AddRange(BySecond);
                        }
                        else
                        {
                            seconds.Add(d.Second);
                        }

                        foreach (var m in minutes)
                        {
                            foreach (var s in seconds)
                            {
                                var newDate = new DateTime(d.Year, d.Month, d.Day, d.Hour, m, s);
                                if (newDate >= utcStartDate && newDate <= endDate && !dates.Contains(newDate))
                                {
                                    dates.Add(newDate);
                                }
                            }
                        }
                    }

                    d = d.AddHours(this.Interval);
                }
                break;
            #endregion

            case Frequency.Daily:

                #region Daily
                d = utcStartDate.AddDays(this.Interval);
                while (d <= endDate && CheckCount(dates, fromDate, maxCount))
                {
                    if (CheckDate(d))
                    {
                        GetDatesWithTime(ref dates, utcStartDate, endDate, d, new List<DateTime>() { d });
                    }

                    d = d.AddDays(this.Interval);
                }
                break;
            #endregion

            case Frequency.Weekly:

                #region Weekly

                d = utcStartDate + utcStartDateOffset;
                while (d <= (endDate == DateTime.MaxValue ? endDate : (endDate + utcStartDateOffset)) && CheckCount(dates, fromDate, maxCount))
                {
                    var dateRange = new List<DateTime>();
                    for (var i = 0; i < 7; i++)
                    {
                        dateRange.Add(d.AddDays(i));
                    }

                    if (ByMonth != null)
                    {
                        dateRange.RemoveAll(date => !ByMonth.Contains(date.Month));
                    }

                    if (ByYearDay != null)
                    {
                        dateRange.RemoveAll(date => !ByYearDay.Contains(date.DayOfYear) && !ByYearDay.Contains(date.DayOfYear - (date.GetDaysInYear() + 1)));
                    }

                    if (ByMonthDay != null)
                    {
                        dateRange.RemoveAll(date => !ByMonthDay.Contains(date.Day) && !ByMonthDay.Contains(date.Day - (date.GetDaysInMonth() + 1)));
                    }

                    if (ByDay != null)
                    {
                        dateRange.RemoveAll(date => !ByDay.Any(wd => wd.DayOfWeek == date.DayOfWeek));
                    }

                    if (ByDay == null && ByMonthDay == null && ByYearDay == null)
                    {
                        dateRange.RemoveAll(date => date.Day != d.Day);
                    }

                    GetDatesWithTime(ref dates, utcStartDate, endDate, d - utcStartDateOffset, dateRange.Select(item => item - utcStartDateOffset).ToList());

                    d = d.AddDays(7 * this.Interval);

                }
                break;
            #endregion

            case Frequency.Monthly:

                #region Monthly

                d = utcStartDate + utcStartDateOffset;
                while (d <= (endDate == DateTime.MaxValue ? endDate : (endDate + utcStartDateOffset)) && CheckCount(dates, fromDate, maxCount))
                {
                    var dateRange = new List<DateTime>();
                    if (ByMonth != null && !ByMonth.Contains(d.Month))
                    {
                        d = d.AddMonths(this.Interval);
                        continue;
                    }

                    var day = new DateTime(d.Year, d.Month, 1);
                    while (day.Month == d.Month)
                    {
                        dateRange.Add(day);
                        day = day.AddDays(1);
                    }

                    if (ByYearDay != null)
                    {
                        dateRange.RemoveAll(date => !ByYearDay.Contains(date.DayOfYear) && !ByYearDay.Contains(date.DayOfYear - (date.GetDaysInYear() + 1)));
                    }

                    if (ByMonthDay != null)
                    {
                        dateRange.RemoveAll(date => !ByMonthDay.Contains(date.Day) && !ByMonthDay.Contains(date.Day - (date.GetDaysInMonth() + 1)));
                    }

                    //only for MONTHLY or YEARLY
                    if (ByDay != null)
                    {
                        var listDates = new List<DateTime>();
                        foreach (var date in ByDay)
                        {
                            listDates.AddRange(date.GetDates(new DateTime(d.Year, d.Month, 1), true));
                        }

                        dateRange.RemoveAll(date => !listDates.Contains(date));
                    }

                    if (ByDay == null && ByMonthDay == null && ByYearDay == null)
                    {
                        dateRange.RemoveAll(date => date.Day != d.Day);
                    }

                    GetDatesWithTime(ref dates, utcStartDate, endDate, d - utcStartDateOffset, dateRange);

                    var nextd = d.AddMonths(this.Interval);

                    if (ByDay == null && ByMonthDay == null && ByYearDay == null)
                    {
                        var i = 1;
                        while (nextd.Day != d.Day)
                        {
                            nextd = d.AddMonths(this.Interval * (++i));
                        }
                    }

                    d = nextd;

                }
                break;
            #endregion

            case Frequency.Yearly:

                #region Yearly

                d = utcStartDate + utcStartDateOffset;

                if (d.Month == 2 && d.Day == 29)
                {
                    if (Interval == 1 && ByMonth == null && ByWeekNo == null && ByYearDay == null && ByMonthDay == null && ByDay == null)
                    {
                        Interval = 4;
                    }
                }

                while (d.Year <= (endDate == DateTime.MaxValue ? endDate : (endDate + utcStartDateOffset)).Year && CheckCount(dates, fromDate, maxCount))
                {
                    var dateRange = new List<DateTime>();
                    var isFirst = true;

                    if (ByMonth != null)
                    {
                        foreach (var m in ByMonth)
                        {
                            var date = new DateTime(d.Year, m, 1);
                            while (date.Month == m)
                            {
                                dateRange.Add(date);
                                date = date.AddDays(1);
                            }
                        }
                        isFirst = false;
                    }

                    //only for YEARLY
                    if (ByWeekNo != null)
                    {
                        if (isFirst)
                        {

                            var date = new DateTime(d.Year, 1, 1);
                            while (date.Year == d.Year)
                            {
                                var weekOfYear = date.GetWeekOfYear(this.WKST.DayOfWeek);
                                if (ByWeekNo.Contains(weekOfYear) || ByWeekNo.Contains(weekOfYear - (date.GetWeekOfYearCount(this.WKST.DayOfWeek) + 1)))
                                {
                                    dateRange.Add(date);
                                }

                                date = date.AddDays(1);
                            }
                        }
                        else
                        {
                            dateRange.RemoveAll(date =>
                            {
                                var weekOfYear = date.GetWeekOfYear(this.WKST.DayOfWeek);
                                return !ByWeekNo.Contains(weekOfYear) && !ByWeekNo.Contains(weekOfYear - (date.GetWeekOfYearCount(this.WKST.DayOfWeek) + 1));
                            });
                        }
                        isFirst = false;
                    }

                    if (ByYearDay != null)
                    {
                        if (isFirst)
                        {
                            foreach (var yearDay in ByYearDay)
                            {
                                dateRange.Add(new DateTime(d.Year, 1, 1).AddDays((yearDay > 0 ? yearDay : (d.GetDaysInYear() + yearDay)) - 1));
                            }
                        }
                        else
                        {
                            dateRange.RemoveAll(date => !ByYearDay.Contains(date.DayOfYear) && !ByYearDay.Contains(date.DayOfYear - (date.GetDaysInYear() + 1)));
                        }

                        isFirst = false;
                    }


                    if (ByMonthDay != null)
                    {
                        if (isFirst)
                        {
                            for (var m = 1; m <= 12; m++)
                            {
                                foreach (var day in ByMonthDay)
                                {
                                    var dd = new DateTime(d.Year, m, 1);
                                    dateRange.Add(dd.AddDays((day > 0 ? day : (dd.GetDaysInMonth() + day)) - 1));
                                }
                            }
                        }
                        else
                        {
                            dateRange.RemoveAll(date => !ByMonthDay.Contains(date.Day) && !ByMonthDay.Contains(date.Day - (date.GetDaysInMonth() + 1)));
                        }

                        isFirst = false;
                    }

                    //only for MONTHLY or YEARLY
                    if (ByDay != null)
                    {
                        var listDates = new List<DateTime>();
                        foreach (var day in ByDay)
                        {
                            listDates.AddRange(day.GetDates(new DateTime(d.Year, 1, 1), false));
                        }

                        listDates.Sort();

                        if (isFirst)
                        {
                            dateRange.AddRange(listDates);
                        }
                        else
                        {
                            dateRange.RemoveAll(date => !listDates.Contains(date));
                        }

                        isFirst = false;
                    }

                    if (ByDay == null && ByMonthDay == null && ByYearDay == null && ByWeekNo == null)
                    {
                        dateRange.RemoveAll(date => date.Day != d.Day);
                    }

                    //add yearly same date
                    if (isFirst)
                    {
                        dateRange.Add(d);
                    }

                    GetDatesWithTime(ref dates, utcStartDate, endDate, d - utcStartDateOffset, dateRange);

                    d = d.AddYears(this.Interval);

                }
                break;
                #endregion
        }

        if (Count >= 0)
        {
            var count = this.Count;
            dates = dates.FindAll(date => (--count) >= 0);
        }

        if (!isAllDayLong)
        {
            dates = OffsetCorrection(utcStartDate, eventTimeZone, dates);
        }

        if (removeExDates && ExDates != null)
        {
            foreach (var exDate in ExDates)
            {
                dates.RemoveAll(dt => (exDate.IsDateTime && dt == exDate.Date) || (!exDate.IsDateTime && dt.Date == exDate.Date));
            }
        }

        dates.RemoveAll(dt => dt < fromDate || dt > endDate);

        return dates;
    }

    private List<DateTime> OffsetCorrection(DateTime origUtcStartDate, TimeZoneInfo eventTimeZone, List<DateTime> dates)
    {
        var result = new List<DateTime>();

        var origOffset = eventTimeZone.GetUtcOffset(origUtcStartDate);

        foreach (var date in dates)
        {
            result.Add(date + (origOffset - eventTimeZone.GetUtcOffset(date))); //todo: check negative timespan
        }

        return result;
    }


    private bool CheckCount(List<DateTime> dates, DateTime fromDate, int maxCount)
    {
        if (Count >= 0)
        {
            return dates.Count <= this.Count;
        }

        if (maxCount != int.MaxValue)
        {
            if (ExDates != null && ExDates.Count > 0)
            {
                return dates.FindAll(dt => dt >= fromDate && !ExDates.Exists(exDate => (exDate.IsDateTime && dt == exDate.Date) || (!exDate.IsDateTime && dt.Date == exDate.Date)))
                    .Count < maxCount;
            }
            else
            {
                return dates.FindAll(d => d >= fromDate).Count < maxCount;
            }
        }

        return true;
    }

    private void GetDatesWithTime(ref List<DateTime> dates, DateTime startDate, DateTime endDate, DateTime d, List<DateTime> dateRange)
    {
        //hours
        var hours = new List<int>();
        if (ByHour != null)
        {
            hours.AddRange(ByHour);
        }
        else
        {
            hours.Add(d.Hour);
        }

        //minutes
        var minutes = new List<int>();
        if (ByMinute != null)
        {
            minutes.AddRange(ByMinute);
        }
        else
        {
            minutes.Add(d.Minute);
        }

        //seconds
        var seconds = new List<int>();
        if (BySecond != null)
        {
            seconds.AddRange(BySecond);
        }
        else
        {
            seconds.Add(d.Second);
        }

        if (BySetPos != null && (ByDay != null || ByMonth != null || ByMonthDay != null || ByWeekNo != null || ByYearDay != null))
        {
            var newDateRange = new List<DateTime>();
            foreach (var pos in BySetPos)
            {
                if (pos >= 1 && pos <= dateRange.Count)
                {
                    newDateRange.Add(dateRange[pos - 1]);
                }
                else if (pos < 0 && (pos * (-1)) <= dateRange.Count)
                {
                    newDateRange.Add(dateRange[dateRange.Count + pos]);
                }
            }
            newDateRange.Sort();
            dateRange = newDateRange;
        }


        foreach (var date in dateRange)
        {
            foreach (var h in hours)
            {
                foreach (var m in minutes)
                {
                    foreach (var s in seconds)
                    {
                        var newDate = new DateTime(date.Year, date.Month, date.Day, h, m, s);
                        if (newDate >= startDate && newDate <= endDate && !dates.Contains(newDate))
                        {
                            dates.Add(newDate);
                        }
                    }
                }
            }
        }

    }

    public override string ToString()
    {
        return ToString(false);
    }
    public string ToString(bool iCal)
    {
        var sb = new StringBuilder();
        switch (Freq)
        {
            case Frequency.Secondly:
                sb.Append("secondly");
                break;

            case Frequency.Minutely:
                sb.Append("minutely");
                break;

            case Frequency.Hourly:
                sb.Append("hourly");
                break;

            case Frequency.Daily:
                sb.Append("daily");
                break;

            case Frequency.Weekly:
                sb.Append("weekly");
                break;

            case Frequency.Monthly:
                sb.Append("monthly");
                break;

            case Frequency.Yearly:
                sb.Append("yearly");
                break;
        }

        if (Until != DateTime.MinValue)
        {
            sb.Append($";until={Until.ToString("yyyyMMdd'T'HHmmss'Z'")}");
        }
        else if (Count >= 0)
        {
            sb.Append($";count={Count}");
        }

        if (Interval > 1)
        {
            sb.Append($";interval={Interval}");
        }

        if (BySecond != null && BySecond.Length > 0)
        {
            sb.Append(";bysecond=");
            foreach (var s in BySecond)
            {
                sb.Append($"{s},");
            }

            sb.Remove(sb.Length - 1, 1);
        }

        if (ByMinute != null && ByMinute.Length > 0)
        {
            sb.Append(";byminute=");
            foreach (var m in ByMinute)
            {
                sb.AppendFormat("{0},", m);
            }

            sb.Remove(sb.Length - 1, 1);
        }

        if (ByHour != null && ByHour.Length > 0)
        {
            sb.Append(";byhour=");
            foreach (var h in ByHour)
            {
                sb.AppendFormat("{0},", h);
            }

            sb.Remove(sb.Length - 1, 1);
        }

        if (ByDay != null && ByDay.Length > 0)
        {
            sb.Append(";byday=");
            foreach (var d in ByDay)
            {
                sb.AppendFormat("{0},", d);
            }

            sb.Remove(sb.Length - 1, 1);
        }


        if (ByMonthDay != null && ByMonthDay.Length > 0)
        {
            sb.Append(";bymonthday=");
            foreach (var d in ByMonthDay)
            {
                sb.AppendFormat("{0},", d);
            }

            sb.Remove(sb.Length - 1, 1);
        }

        if (ByYearDay != null && ByYearDay.Length > 0)
        {
            sb.Append(";byyearday=");
            foreach (var d in ByYearDay)
            {
                sb.AppendFormat("{0},", d);
            }

            sb.Remove(sb.Length - 1, 1);
        }

        if (ByWeekNo != null && ByWeekNo.Length > 0)
        {
            sb.Append(";byweekno=");
            foreach (var w in ByWeekNo)
            {
                sb.AppendFormat("{0},", w);
            }

            sb.Remove(sb.Length - 1, 1);
        }

        if (ByMonth != null && ByMonth.Length > 0)
        {
            sb.Append(";bymonth=");
            foreach (var m in ByMonth)
            {
                sb.AppendFormat("{0},", m);
            }

            sb.Remove(sb.Length - 1, 1);
        }

        if (BySetPos != null && BySetPos.Length > 0)
        {
            sb.Append(";bysetpos=");
            foreach (var p in BySetPos)
            {
                sb.AppendFormat("{0},", p);
            }

            sb.Remove(sb.Length - 1, 1);
        }

        if (WKST.DayOfWeek != DayOfWeek.Monday)
        {
            sb.Append($";wkst={WKST.Id}");
        }

        if (!iCal && ExDates != null && ExDates.Count > 0)
        {
            sb.Append(";exdates=");
            foreach (var d in this.ExDates)
            {
                if (d.IsDateTime)
                {
                    sb.Append(d.Date.ToString("yyyyMMdd'T'HHmmssK") + ",");
                }
                else
                {
                    sb.Append(d.Date.ToString("yyyyMMdd") + ",");
                }
            }
            sb.Remove(sb.Length - 1, 1);
        }

        if (sb.Length > 0)
        {
            sb.Insert(0, "freq=");
        }

        return sb.ToString();
    }

    private static readonly string[] _dateTimeFormats = new[]
                                                   {
                                                           "o",
                                                           "yyyyMMdd'T'HHmmssK",
                                                           "yyyyMMdd",
                                                           "yyyy'-'MM'-'dd'T'HH'-'mm'-'ss'.'fffffffK",
                                                           "yyyy'-'MM'-'dd'T'HH'-'mm'-'ss'.'fffK",
                                                           "yyyy'-'MM'-'dd'T'HH':'mm':'ss'.'fffK",
                                                           "yyyy'-'MM'-'dd'T'HH'-'mm'-'ssK",
                                                           "yyyy'-'MM'-'dd'T'HH':'mm':'ssK",
                                                           "yyyy'-'MM'-'dd"
                                                       };

    public static Frequency ParseFrequency(string frequency)
    {
        return frequency.ToLower() switch
        {
            "monthly" => Frequency.Monthly,

            "secondly" => Frequency.Secondly,

            "daily" => Frequency.Daily,

            "hourly" => Frequency.Hourly,

            "minutely" => Frequency.Minutely,

            "weekly" => Frequency.Weekly,

            "yearly" => Frequency.Yearly,

            _ => Frequency.Never,
        };
    }

    public static RecurrenceRule Parse(string serializedString)
    {
        var rr = new RecurrenceRule();
        var parts = serializedString.Split(new char[] { ';' }, StringSplitOptions.RemoveEmptyEntries);
        foreach (var pair in parts)
        {
            var name = pair.Split('=')[0].Trim().ToLower();
            var val = pair.Split('=')[1].Trim().ToLower();
            switch (name)
            {
                case "freq":
                    rr.Freq = ParseFrequency(val);

                    break;
                case "until":
                    DateTime d;
                    if (DateTime.TryParseExact(val.ToUpper(), _dateTimeFormats, CultureInfo.InvariantCulture, DateTimeStyles.AdjustToUniversal, out d))
                    {
                        rr.Until = d;
                    }

                    break;

                case "count":
                    rr.Count = Convert.ToInt32(val);
                    break;

                case "interval":
                    rr.Interval = Convert.ToInt32(val);
                    break;

                case "bysecond":
                    rr.BySecond = val.Split(',').Select(v => Convert.ToInt32(v)).ToArray();
                    break;

                case "byminute":
                    rr.ByMinute = val.Split(',').Select(v => Convert.ToInt32(v)).ToArray();
                    break;

                case "byhour":
                    rr.ByHour = val.Split(',').Select(v => Convert.ToInt32(v)).ToArray();
                    break;

                case "byday":
                    rr.ByDay = val.Split(',').Select(v => WeekDay.ParseWeekDay(v)).ToArray();
                    break;

                case "bymonthday":
                    rr.ByMonthDay = val.Split(',').Select(v => Convert.ToInt32(v)).ToArray();
                    break;

                case "byyearday":
                    rr.ByYearDay = val.Split(',').Select(v => Convert.ToInt32(v)).ToArray();
                    break;

                case "byweekno":
                    rr.ByWeekNo = val.Split(',').Select(v => Convert.ToInt32(v)).ToArray();
                    break;

                case "bymonth":
                    rr.ByMonth = val.Split(',').Select(v => Convert.ToInt32(v)).ToArray();
                    break;

                case "bysetpos":
                    rr.BySetPos = val.Split(',').Select(v => Convert.ToInt32(v)).ToArray();
                    break;

                case "wkst":
                    rr.WKST = WeekDay.ParseWeekDay(val);
                    break;

                case "exdates":

                    foreach (var date in val.Split(','))
                    {
                        if (DateTime.TryParseExact(date.ToUpper(), _dateTimeFormats, CultureInfo.InvariantCulture, DateTimeStyles.AdjustToUniversal, out var dt))
                        {
                            rr.ExDates.Add(new ExDate() { Date = dt, IsDateTime = date.IndexOf('t', StringComparison.InvariantCultureIgnoreCase) >= 0 });
                        }
                    }
                    break;
            }
        }

        return rr;
    }

    #region IiCalFormatView Members

    public string ToiCalFormat()
    {
        if (this.Freq == Frequency.Never)
        {
            return string.Empty;
        }

        var sb = new StringBuilder();
        sb.Append("RRULE:" + this.ToString(true));

        if (this.ExDates.Count > 0)
        {
            sb.Append("\r\nEXDATE");
            if (!this.ExDates[0].IsDateTime)
            {
                sb.Append(";VALUE=DATE");
            }

            sb.Append(':');
            foreach (var d in this.ExDates)
            {
                if (d.IsDateTime)
                {
                    sb.Append(d.Date.ToString("yyyyMMdd'T'HHmmssK"));
                }
                else
                {
                    sb.Append(d.Date.ToString("yyyyMMdd"));
                }

                sb.Append(',');
            }
            sb.Remove(sb.Length - 1, 1);
        }

        return sb.ToString().ToUpper();
    }

    #endregion

    #region ICloneable Members

    public object Clone()
    {
        var o = (RecurrenceRule)this.MemberwiseClone();

        o.ExDates = new List<ExDate>();
        this.ExDates.ForEach(d => o.ExDates.Add(d));

        if (ByDay != null)
        {
            var days = new List<WeekDay>();
            foreach (var d in ByDay)
            {
                days.Add(WeekDay.ParseWeekDay(d.ToString()));
            }

            o.ByDay = days.ToArray();
        }
        o.WKST = WeekDay.ParseWeekDay(this.WKST.ToString());
        return o;
    }

    #endregion
}
