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


#region usings

using System;
using System.Collections;
using System.Globalization;
using System.Runtime.Serialization;
using System.Text;

#endregion

namespace ASC.Notify.Cron
{
    [Serializable]
    public class CronExpression : ICloneable, IDeserializationCallback
    {
        protected const int Second = 0;

        protected const int Minute = 1;

        protected const int Hour = 2;

        protected const int DayOfMonth = 3;

        protected const int Month = 4;

        protected const int DayOfWeek = 5;

        protected const int Year = 6;

        protected const int AllSpecInt = 99;

        protected const int NoSpecInt = 98;

        protected const int AllSpec = AllSpecInt;

        protected const int NoSpec = NoSpecInt;
        private static readonly Hashtable monthMap = new Hashtable(20);
        private static readonly Hashtable dayMap = new Hashtable(60);
        [NonSerialized] protected bool calendardayOfMonth;
        [NonSerialized] protected bool calendardayOfWeek;

        [NonSerialized] protected TreeSet daysOfMonth;

        [NonSerialized] protected TreeSet daysOfWeek;
        [NonSerialized] protected bool expressionParsed;
        [NonSerialized] protected TreeSet hours;

        [NonSerialized] protected bool lastdayOfMonth;
        [NonSerialized] protected bool lastdayOfWeek;
        [NonSerialized] protected TreeSet minutes;
        [NonSerialized] protected TreeSet months;

        [NonSerialized] protected bool nearestWeekday;
        [NonSerialized] protected int nthdayOfWeek;
        [NonSerialized] protected TreeSet seconds;
        private TimeZoneInfo timeZone;
        [NonSerialized] protected TreeSet years;

        static CronExpression()
        {
            monthMap.Add("JAN", 0);
            monthMap.Add("FEB", 1);
            monthMap.Add("MAR", 2);
            monthMap.Add("APR", 3);
            monthMap.Add("MAY", 4);
            monthMap.Add("JUN", 5);
            monthMap.Add("JUL", 6);
            monthMap.Add("AUG", 7);
            monthMap.Add("SEP", 8);
            monthMap.Add("OCT", 9);
            monthMap.Add("NOV", 10);
            monthMap.Add("DEC", 11);
            dayMap.Add("SUN", 1);
            dayMap.Add("MON", 2);
            dayMap.Add("TUE", 3);
            dayMap.Add("WED", 4);
            dayMap.Add("THU", 5);
            dayMap.Add("FRI", 6);
            dayMap.Add("SAT", 7);
        }

        public CronExpression(string cronExpression)
        {
            if (cronExpression == null)
            {
                throw new ArgumentException("cronExpression cannot be null");
            }
            CronExpressionString = cronExpression.ToUpper(CultureInfo.InvariantCulture);
            BuildExpression(cronExpression);
        }

        public virtual TimeZoneInfo TimeZone
        {
            set { timeZone = value; }
            get
            {
                if (timeZone == null)
                {
                    timeZone = TimeZoneInfo.Utc;
                }
                return timeZone;
            }
        }

        public string CronExpressionString { get; }

        public TimeSpan? Period()
        {
            var date = new DateTime(2014, 1, 1);
            DateTime.SpecifyKind(date, DateTimeKind.Utc);
            var after = GetTimeAfter(date);
            if (!after.HasValue) return null;
            return after.Value.Subtract(date);
        }

        #region ICloneable Members

        public object Clone()
        {
            CronExpression copy;
            try
            {
                copy = new CronExpression(CronExpressionString)
                {
                    TimeZone = TimeZone
                };
            }
            catch (FormatException)
            {
                throw new Exception("Not Cloneable.");
            }
            return copy;
        }

        #endregion

        #region IDeserializationCallback Members

        public void OnDeserialization(object sender)
        {
            BuildExpression(CronExpressionString);
        }

        #endregion

        public virtual bool IsSatisfiedBy(DateTime dateUtc)
        {
            var test =
                new DateTime(dateUtc.Year, dateUtc.Month, dateUtc.Day, dateUtc.Hour, dateUtc.Minute, dateUtc.Second).
                    AddSeconds(-1);
            var timeAfter = GetTimeAfter(test);
            if (timeAfter.HasValue && timeAfter.Value.Equals(dateUtc))
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public virtual DateTime? GetNextValidTimeAfter(DateTime date)
        {
            return GetTimeAfter(date);
        }

        public virtual DateTime? GetNextInvalidTimeAfter(DateTime date)
        {
            long difference = 1000;

            var lastDate =
                new DateTime(date.Year, date.Month, date.Day, date.Hour, date.Minute, date.Second).AddSeconds(-1);

            while (difference == 1000)
            {
                var newDate = GetTimeAfter(lastDate).Value;
                difference = (long)(newDate - lastDate).TotalMilliseconds;
                if (difference == 1000)
                {
                    lastDate = newDate;
                }
            }
            return lastDate.AddSeconds(1);
        }

        public override string ToString()
        {
            return CronExpressionString;
        }

        public static bool IsValidExpression(string cronExpression)
        {
            try
            {
                new CronExpression(cronExpression);
            }
            catch (FormatException)
            {
                return false;
            }
            return true;
        }

        protected void BuildExpression(string expression)
        {
            expressionParsed = true;
            try
            {
                if (seconds == null)
                {
                    seconds = new TreeSet();
                }
                if (minutes == null)
                {
                    minutes = new TreeSet();
                }
                if (hours == null)
                {
                    hours = new TreeSet();
                }
                if (daysOfMonth == null)
                {
                    daysOfMonth = new TreeSet();
                }
                if (months == null)
                {
                    months = new TreeSet();
                }
                if (daysOfWeek == null)
                {
                    daysOfWeek = new TreeSet();
                }
                if (years == null)
                {
                    years = new TreeSet();
                }
                var exprOn = Second;
#if NET_20                string[] exprsTok = expression.Trim().Split(new char[] { ' ', '\t', '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries);
#else
                var exprsTok = expression.Trim().Split(new[] { ' ', '\t', '\r', '\n' });
#endif
                foreach (var exprTok in exprsTok)
                {
                    var expr = exprTok.Trim();
                    if (expr.Length == 0)
                    {
                        continue;
                    }
                    if (exprOn > Year)
                    {
                        break;
                    }

                    if (exprOn == DayOfMonth && expr.IndexOf('L') != -1 && expr.Length > 1 && expr.IndexOf(',') >= 0)
                    {
                        throw new FormatException(
                            "Support for specifying 'L' and 'LW' with other days of the month is not implemented");
                    }

                    if (exprOn == DayOfWeek && expr.IndexOf('L') != -1 && expr.Length > 1 && expr.IndexOf(',') >= 0)
                    {
                        throw new FormatException(
                            "Support for specifying 'L' with other days of the week is not implemented");
                    }
                    var vTok = expr.Split(',');
                    foreach (var v in vTok)
                    {
                        StoreExpressionVals(0, v, exprOn);
                    }
                    exprOn++;
                }
                if (exprOn <= DayOfWeek)
                {
                    throw new FormatException("Unexpected end of expression.");
                }
                if (exprOn <= Year)
                {
                    StoreExpressionVals(0, "*", Year);
                }
                var dow = GetSet(DayOfWeek);
                var dom = GetSet(DayOfMonth);

                var dayOfMSpec = !dom.Contains(NoSpec);
                var dayOfWSpec = !dow.Contains(NoSpec);
                if (dayOfMSpec && !dayOfWSpec)
                {
                }
                else if (dayOfWSpec && !dayOfMSpec)
                {
                }
                else
                {
                    throw new FormatException(
                        "Support for specifying both a day-of-week AND a day-of-month parameter is not implemented.");
                }
            }
            catch (FormatException)
            {
                throw;
            }
            catch (Exception e)
            {
                throw new FormatException(string.Format(CultureInfo.InvariantCulture,
                                                        "Illegal cron expression format ({0})", e));
            }
        }

        protected virtual int StoreExpressionVals(int pos, string s, int type)
        {
            var incr = 0;
            var i = SkipWhiteSpace(pos, s);
            if (i >= s.Length)
            {
                return i;
            }
            var c = s[i];
            if ((c >= 'A') && (c <= 'Z') && (!s.Equals("L")) && (!s.Equals("LW")))
            {
                var sub = s.Substring(i, 3);
                int sval;
                var eval = -1;
                if (type == Month)
                {
                    sval = GetMonthNumber(sub) + 1;
                    if (sval <= 0)
                    {
                        throw new FormatException($"Invalid Month value: '{sub}'");
                    }
                    if (s.Length > i + 3)
                    {
                        c = s[i + 3];
                        if (c == '-')
                        {
                            i += 4;
                            sub = s.Substring(i, 3);
                            eval = GetMonthNumber(sub) + 1;
                            if (eval <= 0)
                            {
                                throw new FormatException($"Invalid Month value: '{sub}'");
                            }
                        }
                    }
                }
                else if (type == DayOfWeek)
                {
                    sval = GetDayOfWeekNumber(sub);
                    if (sval < 0)
                    {
                        throw new FormatException($"Invalid Day-of-Week value: '{sub}'");
                    }
                    if (s.Length > i + 3)
                    {
                        c = s[i + 3];
                        if (c == '-')
                        {
                            i += 4;
                            sub = s.Substring(i, 3);
                            eval = GetDayOfWeekNumber(sub);
                            if (eval < 0)
                            {
                                throw new FormatException($"Invalid Day-of-Week value: '{sub}'");
                            }
                        }
                        else if (c == '#')
                        {
                            try
                            {
                                i += 4;
                                nthdayOfWeek = Convert.ToInt32(s.Substring(i), CultureInfo.InvariantCulture);
                                if (nthdayOfWeek < 1 || nthdayOfWeek > 5)
                                {
                                    throw new Exception();
                                }
                            }
                            catch (Exception)
                            {
                                throw new FormatException(
                                    "A numeric value between 1 and 5 must follow the '#' option");
                            }
                        }
                        else if (c == 'L')
                        {
                            lastdayOfWeek = true;
                            i++;
                        }
                    }
                }
                else
                {
                    throw new FormatException($"Illegal characters for this position: '{sub}'");
                }
                if (eval != -1)
                {
                    incr = 1;
                }
                AddToSet(sval, eval, incr, type);
                return i + 3;
            }
            if (c == '?')
            {
                i++;
                if ((i + 1) < s.Length
                    && s[i] != ' ' && s[i + 1] != '\t')
                {
                    throw new FormatException("Illegal character after '?': "
                                              + s[i]);
                }
                if (type != DayOfWeek && type != DayOfMonth)
                {
                    throw new FormatException(
                        "'?' can only be specified for Day-of-Month or Day-of-Week.");
                }
                if (type == DayOfWeek && !lastdayOfMonth)
                {
                    var val = (int)daysOfMonth[^1];
                    if (val == NoSpecInt)
                    {
                        throw new FormatException(
                            "'?' can only be specified for Day-of-Month -OR- Day-of-Week.");
                    }
                }
                AddToSet(NoSpecInt, -1, 0, type);
                return i;
            }
            if (c == '*' || c == '/')
            {
                if (c == '*' && (i + 1) >= s.Length)
                {
                    AddToSet(AllSpecInt, -1, incr, type);
                    return i + 1;
                }
                else if (c == '/'
                         && ((i + 1) >= s.Length || s[i + 1] == ' ' || s[i + 1] == '\t'))
                {
                    throw new FormatException("'/' must be followed by an integer.");
                }
                else if (c == '*')
                {
                    i++;
                }
                c = s[i];
                if (c == '/')
                {
                    i++;
                    if (i >= s.Length)
                    {
                        throw new FormatException("Unexpected end of string.");
                    }
                    incr = GetNumericValue(s, i);
                    i++;
                    if (incr > 10)
                    {
                        i++;
                    }
                    if (incr > 59 && (type == Second || type == Minute))
                    {
                        throw new FormatException(
                            string.Format(CultureInfo.InvariantCulture, "Increment > 60 : {0}", incr));
                    }
                    else if (incr > 23 && (type == Hour))
                    {
                        throw new FormatException(
                            string.Format(CultureInfo.InvariantCulture, "Increment > 24 : {0}", incr));
                    }
                    else if (incr > 31 && (type == DayOfMonth))
                    {
                        throw new FormatException(
                            string.Format(CultureInfo.InvariantCulture, "Increment > 31 : {0}", incr));
                    }
                    else if (incr > 7 && (type == DayOfWeek))
                    {
                        throw new FormatException(
                            string.Format(CultureInfo.InvariantCulture, "Increment > 7 : {0}", incr));
                    }
                    else if (incr > 12 && (type == Month))
                    {
                        throw new FormatException(string.Format(CultureInfo.InvariantCulture, "Increment > 12 : {0}",
                                                                incr));
                    }
                }
                else
                {
                    incr = 1;
                }
                AddToSet(AllSpecInt, -1, incr, type);
                return i;
            }
            else if (c == 'L')
            {
                i++;
                if (type == DayOfMonth)
                {
                    lastdayOfMonth = true;
                }
                if (type == DayOfWeek)
                {
                    AddToSet(7, 7, 0, type);
                }
                if (type == DayOfMonth && s.Length > i)
                {
                    c = s[i];
                    if (c == 'W')
                    {
                        nearestWeekday = true;
                        i++;
                    }
                }
                return i;
            }
            else if (c >= '0' && c <= '9')
            {
                var val = Convert.ToInt32(c.ToString(), CultureInfo.InvariantCulture);
                i++;
                if (i >= s.Length)
                {
                    AddToSet(val, -1, -1, type);
                }
                else
                {
                    c = s[i];
                    if (c >= '0' && c <= '9')
                    {
                        var vs = GetValue(val, s, i);
                        val = vs.TheValue;
                        i = vs.Pos;
                    }
                    i = CheckNext(i, s, val, type);
                    return i;
                }
            }
            else
            {
                throw new FormatException(string.Format(CultureInfo.InvariantCulture, "Unexpected character: {0}", c));
            }
            return i;
        }

        protected virtual int CheckNext(int pos, string s, int val, int type)
        {
            var end = -1;
            var i = pos;
            if (i >= s.Length)
            {
                AddToSet(val, end, -1, type);
                return i;
            }
            var c = s[pos];
            if (c == 'L')
            {
                if (type == DayOfWeek)
                {
                    lastdayOfWeek = true;
                }
                else
                {
                    throw new FormatException(string.Format(CultureInfo.InvariantCulture,
                                                            "'L' option is not valid here. (pos={0})", i));
                }
                var data = GetSet(type);
                data.Add(val);
                i++;
                return i;
            }
            if (c == 'W')
            {
                if (type == DayOfMonth)
                {
                    nearestWeekday = true;
                }
                else
                {
                    throw new FormatException(string.Format(CultureInfo.InvariantCulture,
                                                            "'W' option is not valid here. (pos={0})", i));
                }
                var data = GetSet(type);
                data.Add(val);
                i++;
                return i;
            }
            if (c == '#')
            {
                if (type != DayOfWeek)
                {
                    throw new FormatException(
                        string.Format(CultureInfo.InvariantCulture, "'#' option is not valid here. (pos={0})", i));
                }
                i++;
                try
                {
                    nthdayOfWeek = Convert.ToInt32(s.Substring(i), CultureInfo.InvariantCulture);
                    if (nthdayOfWeek < 1 || nthdayOfWeek > 5)
                    {
                        throw new Exception();
                    }
                }
                catch (Exception)
                {
                    throw new FormatException(
                        "A numeric value between 1 and 5 must follow the '#' option");
                }
                var data = GetSet(type);
                data.Add(val);
                i++;
                return i;
            }
            if (c == 'C')
            {
                if (type == DayOfWeek)
                {
                    calendardayOfWeek = true;
                }
                else if (type == DayOfMonth)
                {
                    calendardayOfMonth = true;
                }
                else
                {
                    throw new FormatException(string.Format(CultureInfo.InvariantCulture,
                                                            "'C' option is not valid here. (pos={0})", i));
                }
                var data = GetSet(type);
                data.Add(val);
                i++;
                return i;
            }
            if (c == '-')
            {
                i++;
                c = s[i];
                var v = Convert.ToInt32(c.ToString(), CultureInfo.InvariantCulture);
                end = v;
                i++;
                if (i >= s.Length)
                {
                    AddToSet(val, end, 1, type);
                    return i;
                }
                c = s[i];
                if (c >= '0' && c <= '9')
                {
                    var vs = GetValue(v, s, i);
                    var v1 = vs.TheValue;
                    end = v1;
                    i = vs.Pos;
                }
                if (i < s.Length && (s[i] == '/'))
                {
                    i++;
                    c = s[i];
                    var v2 = Convert.ToInt32(c.ToString(), CultureInfo.InvariantCulture);
                    i++;
                    if (i >= s.Length)
                    {
                        AddToSet(val, end, v2, type);
                        return i;
                    }
                    c = s[i];
                    if (c >= '0' && c <= '9')
                    {
                        var vs = GetValue(v2, s, i);
                        var v3 = vs.TheValue;
                        AddToSet(val, end, v3, type);
                        i = vs.Pos;
                        return i;
                    }
                    else
                    {
                        AddToSet(val, end, v2, type);
                        return i;
                    }
                }
                else
                {
                    AddToSet(val, end, 1, type);
                    return i;
                }
            }
            if (c == '/')
            {
                i++;
                c = s[i];
                var v2 = Convert.ToInt32(c.ToString(), CultureInfo.InvariantCulture);
                i++;
                if (i >= s.Length)
                {
                    AddToSet(val, end, v2, type);
                    return i;
                }
                c = s[i];
                if (c >= '0' && c <= '9')
                {
                    var vs = GetValue(v2, s, i);
                    var v3 = vs.TheValue;
                    AddToSet(val, end, v3, type);
                    i = vs.Pos;
                    return i;
                }
                else
                {
                    throw new FormatException(string.Format(CultureInfo.InvariantCulture,
                                                            "Unexpected character '{0}' after '/'", c));
                }
            }
            AddToSet(val, end, 0, type);
            i++;
            return i;
        }

        public virtual string GetExpressionSummary()
        {
            var buf = new StringBuilder();
            buf.Append("seconds: ");
            buf.Append(GetExpressionSetSummary(seconds));
            buf.Append('\n');
            buf.Append("minutes: ");
            buf.Append(GetExpressionSetSummary(minutes));
            buf.Append('\n');
            buf.Append("hours: ");
            buf.Append(GetExpressionSetSummary(hours));
            buf.Append('\n');
            buf.Append("daysOfMonth: ");
            buf.Append(GetExpressionSetSummary(daysOfMonth));
            buf.Append('\n');
            buf.Append("months: ");
            buf.Append(GetExpressionSetSummary(months));
            buf.Append('\n');
            buf.Append("daysOfWeek: ");
            buf.Append(GetExpressionSetSummary(daysOfWeek));
            buf.Append('\n');
            buf.Append("lastdayOfWeek: ");
            buf.Append(lastdayOfWeek);
            buf.Append('\n');
            buf.Append("nearestWeekday: ");
            buf.Append(nearestWeekday);
            buf.Append('\n');
            buf.Append("NthDayOfWeek: ");
            buf.Append(nthdayOfWeek);
            buf.Append('\n');
            buf.Append("lastdayOfMonth: ");
            buf.Append(lastdayOfMonth);
            buf.Append('\n');
            buf.Append("calendardayOfWeek: ");
            buf.Append(calendardayOfWeek);
            buf.Append('\n');
            buf.Append("calendardayOfMonth: ");
            buf.Append(calendardayOfMonth);
            buf.Append('\n');
            buf.Append("years: ");
            buf.Append(GetExpressionSetSummary(years));
            buf.Append('\n');
            return buf.ToString();
        }

        protected virtual string GetExpressionSetSummary(ISet data)
        {
            if (data.Contains(NoSpec))
            {
                return "?";
            }
            if (data.Contains(AllSpec))
            {
                return "*";
            }
            var buf = new StringBuilder();
            var first = true;
            foreach (int iVal in data)
            {
                var val = iVal.ToString(CultureInfo.InvariantCulture);
                if (!first)
                {
                    buf.Append(',');
                }
                buf.Append(val);
                first = false;
            }
            return buf.ToString();
        }

        protected virtual int SkipWhiteSpace(int i, string s)
        {
            for (; i < s.Length && (s[i] == ' ' || s[i] == '\t'); i++)
            {
                
            }
            return i;
        }

        protected virtual int FindNextWhiteSpace(int i, string s)
        {
            for (; i < s.Length && (s[i] != ' ' || s[i] != '\t'); i++)
            {
                
            }
            return i;
        }

        protected virtual void AddToSet(int val, int end, int incr, int type)
        {
            var data = GetSet(type);
            if (type == Second || type == Minute)
            {
                if ((val < 0 || val > 59 || end > 59) && (val != AllSpecInt))
                {
                    throw new FormatException(
                        "Minute and Second values must be between 0 and 59");
                }
            }
            else if (type == Hour)
            {
                if ((val < 0 || val > 23 || end > 23) && (val != AllSpecInt))
                {
                    throw new FormatException(
                        "Hour values must be between 0 and 23");
                }
            }
            else if (type == DayOfMonth)
            {
                if ((val < 1 || val > 31 || end > 31) && (val != AllSpecInt)
                    && (val != NoSpecInt))
                {
                    throw new FormatException(
                        "Day of month values must be between 1 and 31");
                }
            }
            else if (type == Month)
            {
                if ((val < 1 || val > 12 || end > 12) && (val != AllSpecInt))
                {
                    throw new FormatException(
                        "Month values must be between 1 and 12");
                }
            }
            else if (type == DayOfWeek)
            {
                if ((val == 0 || val > 7 || end > 7) && (val != AllSpecInt)
                    && (val != NoSpecInt))
                {
                    throw new FormatException(
                        "Day-of-Week values must be between 1 and 7");
                }
            }
            if ((incr == 0 || incr == -1) && val != AllSpecInt)
            {
                if (val != -1)
                {
                    data.Add(val);
                }
                else
                {
                    data.Add(NoSpec);
                }
                return;
            }
            var startAt = val;
            var stopAt = end;
            if (val == AllSpecInt && incr <= 0)
            {
                incr = 1;
                data.Add(AllSpec);
            }
            if (type == Second || type == Minute)
            {
                if (stopAt == -1)
                {
                    stopAt = 59;
                }
                if (startAt == -1 || startAt == AllSpecInt)
                {
                    startAt = 0;
                }
            }
            else if (type == Hour)
            {
                if (stopAt == -1)
                {
                    stopAt = 23;
                }
                if (startAt == -1 || startAt == AllSpecInt)
                {
                    startAt = 0;
                }
            }
            else if (type == DayOfMonth)
            {
                if (stopAt == -1)
                {
                    stopAt = 31;
                }
                if (startAt == -1 || startAt == AllSpecInt)
                {
                    startAt = 1;
                }
            }
            else if (type == Month)
            {
                if (stopAt == -1)
                {
                    stopAt = 12;
                }
                if (startAt == -1 || startAt == AllSpecInt)
                {
                    startAt = 1;
                }
            }
            else if (type == DayOfWeek)
            {
                if (stopAt == -1)
                {
                    stopAt = 7;
                }
                if (startAt == -1 || startAt == AllSpecInt)
                {
                    startAt = 1;
                }
            }
            else if (type == Year)
            {
                if (stopAt == -1)
                {
                    stopAt = 2099;
                }
                if (startAt == -1 || startAt == AllSpecInt)
                {
                    startAt = 1970;
                }
            }

            var max = -1;
            if (stopAt < startAt)
            {
                max = type switch
                {
                    Second => 60,
                    Minute => 60,
                    Hour => 24,
                    Month => 12,
                    DayOfWeek => 7,
                    DayOfMonth => 31,
                    Year => throw new ArgumentException("Start year must be less than stop year"),
                    _ => throw new ArgumentException("Unexpected type encountered"),
                };
                stopAt += max;
            }
            for (var i = startAt; i <= stopAt; i += incr)
            {
                if (max == -1)
                {
                    data.Add(i);
                }
                else
                {
                    var i2 = i % max;

                    if (i2 == 0 && (type == Month || type == DayOfWeek || type == DayOfMonth))
                    {
                        i2 = max;
                    }
                    data.Add(i2);
                }
            }
        }

        protected virtual TreeSet GetSet(int type)
        {
            return type switch
            {
                Second => seconds,
                Minute => minutes,
                Hour => hours,
                DayOfMonth => daysOfMonth,
                Month => months,
                DayOfWeek => daysOfWeek,
                Year => years,
                _ => null,
            };
        }

        protected virtual ValueSet GetValue(int v, string s, int i)
        {
            var c = s[i];
            var sb = new StringBuilder();
            sb.Append(v.ToString(CultureInfo.InvariantCulture));
            while (c >= '0' && c <= '9')
            {
                sb.Append(c);
                i++;
                if (i >= s.Length)
                {
                    break;
                }
                c = s[i];
            }
            var val = new ValueSet();
            if (i < s.Length)
            {
                val.Pos = i;
            }
            else
            {
                val.Pos = i + 1;
            }
            val.TheValue = Convert.ToInt32(sb.ToString(), CultureInfo.InvariantCulture);
            return val;
        }

        protected virtual int GetNumericValue(string s, int i)
        {
            var endOfVal = FindNextWhiteSpace(i, s);
            var val = s[i..endOfVal];
            return Convert.ToInt32(val, CultureInfo.InvariantCulture);
        }

        protected virtual int GetMonthNumber(string s)
        {
            if (monthMap.ContainsKey(s))
            {
                return (int)monthMap[s];
            }
            else
            {
                return -1;
            }
        }

        protected virtual int GetDayOfWeekNumber(string s)
        {
            if (dayMap.ContainsKey(s))
            {
                return (int)dayMap[s];
            }
            else
            {
                return -1;
            }
        }

        protected virtual DateTime? GetTime(int sc, int mn, int hr, int dayofmn, int mon)
        {
            try
            {
                if (sc == -1)
                {
                    sc = 0;
                }
                if (mn == -1)
                {
                    mn = 0;
                }
                if (hr == -1)
                {
                    hr = 0;
                }
                if (dayofmn == -1)
                {
                    dayofmn = 0;
                }
                if (mon == -1)
                {
                    mon = 0;
                }
                return new DateTime(DateTime.UtcNow.Year, mon, dayofmn, hr, mn, sc);
            }
            catch (Exception)
            {
                return null;
            }
        }

        public virtual DateTime? GetTimeAfter(DateTime afterTimeUtc)
        {
            if (afterTimeUtc == DateTime.MaxValue)
                return null;

            afterTimeUtc = afterTimeUtc.AddSeconds(1);

            var d = CreateDateTimeWithoutMillis(afterTimeUtc);

            d = TimeZoneInfo.ConvertTimeFromUtc(d, TimeZone);
            var gotOne = false;

            while (!gotOne)
            {
                ISortedSet st;
                int t;
                var sec = d.Second;

                st = seconds.TailSet(sec);
                if (st != null && st.Count != 0)
                {
                    sec = (int)st.First();
                }
                else
                {
                    sec = (int)seconds.First();
                    d = d.AddMinutes(1);
                }
                d = new DateTime(d.Year, d.Month, d.Day, d.Hour, d.Minute, sec, d.Millisecond);
                var min = d.Minute;
                var hr = d.Hour;
                t = -1;

                st = minutes.TailSet(min);
                if (st != null && st.Count != 0)
                {
                    t = min;
                    min = (int)st.First();
                }
                else
                {
                    min = (int)minutes.First();
                    hr++;
                }
                if (min != t)
                {
                    d = new DateTime(d.Year, d.Month, d.Day, d.Hour, min, 0, d.Millisecond);
                    d = SetCalendarHour(d, hr);
                    continue;
                }
                d = new DateTime(d.Year, d.Month, d.Day, d.Hour, min, d.Second, d.Millisecond);
                hr = d.Hour;
                var day = d.Day;
                t = -1;

                st = hours.TailSet(hr);
                if (st != null && st.Count != 0)
                {
                    t = hr;
                    hr = (int)st.First();
                }
                else
                {
                    hr = (int)hours.First();
                    day++;
                }
                if (hr != t)
                {
                    var daysInMonth = DateTime.DaysInMonth(d.Year, d.Month);
                    if (day > daysInMonth)
                    {
                        d =
                            new DateTime(d.Year, d.Month, daysInMonth, d.Hour, 0, 0, d.Millisecond).AddDays(day -
                                                                                                            daysInMonth);
                    }
                    else
                    {
                        d = new DateTime(d.Year, d.Month, day, d.Hour, 0, 0, d.Millisecond);
                    }
                    d = SetCalendarHour(d, hr);
                    continue;
                }
                d = new DateTime(d.Year, d.Month, d.Day, hr, d.Minute, d.Second, d.Millisecond);
                day = d.Day;
                var mon = d.Month;
                t = -1;
                var tmon = mon;

                var dayOfMSpec = !daysOfMonth.Contains(NoSpec);
                var dayOfWSpec = !daysOfWeek.Contains(NoSpec);
                if (dayOfMSpec && !dayOfWSpec)
                {
                    st = daysOfMonth.TailSet(day);
                    if (lastdayOfMonth)
                    {
                        if (!nearestWeekday)
                        {
                            t = day;
                            day = GetLastDayOfMonth(mon, d.Year);
                        }
                        else
                        {
                            t = day;
                            day = GetLastDayOfMonth(mon, d.Year);
                            var tcal = new DateTime(d.Year, mon, day, 0, 0, 0);
                            var ldom = GetLastDayOfMonth(mon, d.Year);
                            var dow = tcal.DayOfWeek;
                            if (dow == System.DayOfWeek.Saturday && day == 1)
                            {
                                day += 2;
                            }
                            else if (dow == System.DayOfWeek.Saturday)
                            {
                                day -= 1;
                            }
                            else if (dow == System.DayOfWeek.Sunday && day == ldom)
                            {
                                day -= 2;
                            }
                            else if (dow == System.DayOfWeek.Sunday)
                            {
                                day += 1;
                            }
                            var nTime = new DateTime(tcal.Year, mon, day, hr, min, sec, d.Millisecond);
                            if (nTime.ToUniversalTime() < afterTimeUtc)
                            {
                                day = 1;
                                mon++;
                            }
                        }
                    }
                    else if (nearestWeekday)
                    {
                        t = day;
                        day = (int)daysOfMonth.First();
                        var tcal = new DateTime(d.Year, mon, day, 0, 0, 0);
                        var ldom = GetLastDayOfMonth(mon, d.Year);
                        var dow = tcal.DayOfWeek;
                        if (dow == System.DayOfWeek.Saturday && day == 1)
                        {
                            day += 2;
                        }
                        else if (dow == System.DayOfWeek.Saturday)
                        {
                            day -= 1;
                        }
                        else if (dow == System.DayOfWeek.Sunday && day == ldom)
                        {
                            day -= 2;
                        }
                        else if (dow == System.DayOfWeek.Sunday)
                        {
                            day += 1;
                        }
                        tcal = new DateTime(tcal.Year, mon, day, hr, min, sec);
                        if (tcal.ToUniversalTime() < afterTimeUtc)
                        {
                            day = (int)daysOfMonth.First();
                            mon++;
                        }
                    }
                    else if (st != null && st.Count != 0)
                    {
                        t = day;
                        day = (int)st.First();

                        var lastDay = GetLastDayOfMonth(mon, d.Year);
                        if (day > lastDay)
                        {
                            day = (int)daysOfMonth.First();
                            mon++;
                        }
                    }
                    else
                    {
                        day = (int)daysOfMonth.First();
                        mon++;
                    }
                    if (day != t || mon != tmon)
                    {
                        if (mon > 12)
                        {
                            d = new DateTime(d.Year, 12, day, 0, 0, 0).AddMonths(mon - 12);
                        }
                        else
                        {
                            var lDay = DateTime.DaysInMonth(d.Year, mon);
                            if (day <= lDay)
                            {
                                d = new DateTime(d.Year, mon, day, 0, 0, 0);
                            }
                            else
                            {
                                d = new DateTime(d.Year, mon, lDay, 0, 0, 0).AddDays(day - lDay);
                            }
                        }
                        continue;
                    }
                }
                else if (dayOfWSpec && !dayOfMSpec)
                {
                    if (lastdayOfWeek)
                    {
                        var dow = (int)daysOfWeek.First();

                        var cDow = (int)d.DayOfWeek;
                        var daysToAdd = 0;
                        if (cDow < dow)
                        {
                            daysToAdd = dow - cDow;
                        }
                        if (cDow > dow)
                        {
                            daysToAdd = dow + (7 - cDow);
                        }
                        var lDay = GetLastDayOfMonth(mon, d.Year);
                        if (day + daysToAdd > lDay)
                        {
                            if (mon == 12)
                            {
                                if (d.Year == DateTime.MaxValue.Year)
                                    return null;
                                d = new DateTime(d.Year, mon - 11, 1, 0, 0, 0).AddYears(1);
                            }
                            else
                            {
                                d = new DateTime(d.Year, mon + 1, 1, 0, 0, 0);
                            }

                            continue;
                        }

                        while ((day + daysToAdd + 7) <= lDay)
                        {
                            daysToAdd += 7;
                        }
                        day += daysToAdd;
                        if (daysToAdd > 0)
                        {
                            d = new DateTime(d.Year, mon, day, 0, 0, 0);

                            continue;
                        }
                    }
                    else if (nthdayOfWeek != 0)
                    {
                        var dow = (int)daysOfWeek.First();

                        var cDow = (int)d.DayOfWeek;
                        var daysToAdd = 0;
                        if (cDow < dow)
                        {
                            daysToAdd = dow - cDow;
                        }
                        else if (cDow > dow)
                        {
                            daysToAdd = dow + (7 - cDow);
                        }
                        var dayShifted = false;
                        if (daysToAdd > 0)
                        {
                            dayShifted = true;
                        }
                        day += daysToAdd;
                        var weekOfMonth = day / 7;
                        if (day % 7 > 0)
                        {
                            weekOfMonth++;
                        }
                        daysToAdd = (nthdayOfWeek - weekOfMonth) * 7;
                        day += daysToAdd;
                        if (daysToAdd < 0 || day > GetLastDayOfMonth(mon, d.Year))
                        {
                            if (mon == 12)
                            {
                                if (d.Year == DateTime.MaxValue.Year)
                                    return null;

                                d = new DateTime(d.Year, mon - 11, 1, 0, 0, 0).AddYears(1);
                            }
                            else
                            {
                                d = new DateTime(d.Year, mon + 1, 1, 0, 0, 0);
                            }

                            continue;
                        }
                        else if (daysToAdd > 0 || dayShifted)
                        {
                            d = new DateTime(d.Year, mon, day, 0, 0, 0);

                            continue;
                        }
                    }
                    else
                    {
                        var cDow = ((int)d.DayOfWeek) + 1;
                        var dow = (int)daysOfWeek.First();

                        st = daysOfWeek.TailSet(cDow);
                        if (st != null && st.Count > 0)
                        {
                            dow = (int)st.First();
                        }
                        var daysToAdd = 0;
                        if (cDow < dow)
                        {
                            daysToAdd = dow - cDow;
                        }
                        if (cDow > dow)
                        {
                            daysToAdd = dow + (7 - cDow);
                        }
                        var lDay = GetLastDayOfMonth(mon, d.Year);
                        if (day + daysToAdd > lDay)
                        {
                            if (mon == 12)
                            {
                                if (d.Year == DateTime.MaxValue.Year)
                                    return null;

                                d = new DateTime(d.Year, mon - 11, 1, 0, 0, 0).AddYears(1);
                            }
                            else
                            {
                                d = new DateTime(d.Year, mon + 1, 1, 0, 0, 0);
                            }

                            continue;
                        }
                        else if (daysToAdd > 0)
                        {
                            d = new DateTime(d.Year, mon, day + daysToAdd, 0, 0, 0);
                            continue;
                        }
                    }
                }
                else
                {
                    throw new Exception(
                        "Support for specifying both a day-of-week AND a day-of-month parameter is not implemented.");
                }
                d = new DateTime(d.Year, d.Month, day, d.Hour, d.Minute, d.Second);
                mon = d.Month;
                var year = d.Year;
                t = -1;

                if (year > 2099)
                {
                    return null;
                }

                st = months.TailSet(mon);
                if (st != null && st.Count != 0)
                {
                    t = mon;
                    mon = (int)st.First();
                }
                else
                {
                    mon = (int)months.First();
                    year++;
                }
                if (mon != t)
                {
                    d = new DateTime(year, mon, 1, 0, 0, 0);
                    continue;
                }
                d = new DateTime(d.Year, mon, d.Day, d.Hour, d.Minute, d.Second);
                year = d.Year;
                st = years.TailSet(year);
                if (st != null && st.Count != 0)
                {
                    t = year;
                    year = (int)st.First();
                }
                else
                {
                    return null;
                }
                if (year != t)
                {
                    d = new DateTime(year, 1, 1, 0, 0, 0);
                    continue;
                }
                d = new DateTime(year, d.Month, d.Day, d.Hour, d.Minute, d.Second);
                gotOne = true;
            }
            return TimeZoneInfo.ConvertTimeToUtc(d, TimeZone);
        }

        protected static DateTime CreateDateTimeWithoutMillis(DateTime time)
        {
            return new DateTime(time.Year, time.Month, time.Day, time.Hour, time.Minute, time.Second);
        }

        protected static DateTime SetCalendarHour(DateTime date, int hour)
        {
            var hourToSet = hour;
            if (hourToSet == 24)
            {
                hourToSet = 0;
            }
            var d =
                new DateTime(date.Year, date.Month, date.Day, hourToSet, date.Minute, date.Second, date.Millisecond);
            if (hour == 24)
            {
                d = d.AddDays(1);
            }
            return d;
        }

        public virtual DateTime? GetTimeBefore(DateTime? endTime)
        {
            return null;
        }

        public virtual DateTime? GetFinalFireTime()
        {
            return null;
        }

        protected virtual bool IsLeapYear(int year)
        {
            return DateTime.IsLeapYear(year);
        }

        protected virtual int GetLastDayOfMonth(int monthNum, int year)
        {
            return DateTime.DaysInMonth(year, monthNum);
        }
    }

    public class ValueSet
    {
        public int Pos { get; set; }
        public int TheValue { get; set; }
    }
}