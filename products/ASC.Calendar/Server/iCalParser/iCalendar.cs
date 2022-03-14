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
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text.RegularExpressions;

using ASC.Calendar.Resources;
using ASC.Common.Utils;
using ASC.Core;
using ASC.Web.Core.Calendars;

namespace ASC.Calendar.iCalParser
{
    public class iCalendar : BaseCalendar
    {
        private TenantManager TenantManager { get; }
        private IHttpClientFactory ClientFactory { get; }

        public iCalendar GetFromStream(TextReader reader)
        {
            var emitter = new iCalendarEmitter(AuthContext, TimeZoneConverter, TenantManager, ClientFactory);
            var parser = new Parser(reader, emitter);
            parser.Parse();
            return emitter.GetCalendar();
        }

        public iCalendar GetFromUrl(string url)
        {
            return GetFromUrl(url, null);
        }

        public iCalendar GetFromUrl(string url, string calendarId)
        {
            var cache = new iCalendarCache(AuthContext, TimeZoneConverter, TenantManager, ClientFactory);
            iCalendar calendar = null;
            if (calendarId != null)
                calendar = cache.GetCalendarFromCache(calendarId);

            if (calendar == null)
            {
                if (url.StartsWith("webcal"))
                {
                    url = new Regex("webcal").Replace(url, "http", 1);
                }

                var request = new HttpRequestMessage();
                request.RequestUri = new Uri(url);

                var httpClient = ClientFactory.CreateClient();
                using (var response = httpClient.Send(request))
                using (var stream = response.Content.ReadAsStream())
                {
                    var ms = new MemoryStream();
                    stream.CopyTo(ms);
                    ms.Seek(0, SeekOrigin.Begin);

                    using (var tempReader = new StreamReader(ms))
                    {
                        var reader = new StringReader(tempReader.ReadToEnd());
                        calendar = GetFromStream(reader);

                        if (calendar != null && calendarId != null)
                        {
                            tempReader.BaseStream.Seek(0, SeekOrigin.Begin);
                            cache.UpdateCalendarCache(calendarId, tempReader);
                        }
                    }
                }
            }

            if (calendar == null)
                throw new Exception(CalendarApiResource.WrongiCalFeedLink);

            return calendar;
        }


        public List<iCalEvent> Events { get; set; }


        public iCalendar(
            AuthContext authContext,
            TimeZoneConverter timeZoneConverter,
            TenantManager tenantManager,
            IHttpClientFactory clientFactory)
        : base(authContext, timeZoneConverter)
        {
            TenantManager = tenantManager;
            ClientFactory = clientFactory;
            this.Context.CanChangeAlertType = false;
            this.Context.CanChangeTimeZone = false;
            this.Context.GetGroupMethod = delegate () { return Resources.CalendarApiResource.iCalCalendarsGroup; };

            this.EventAlertType = EventAlertType.Never;
            this.Events = new List<iCalEvent>();
        }

        public bool isEmptyName
        {
            get { return String.IsNullOrEmpty(_name); }
        }

        private string _name;
        public override string Name
        {
            get
            {
                if (String.IsNullOrEmpty(_name))
                    return Resources.CalendarApiResource.NoNameCalendar;

                return _name;
            }
            set
            {
                _name = value;
            }
        }

        private TimeZoneInfo _timeZone;
        public override TimeZoneInfo TimeZone
        {
            get
            {
                if (_timeZone != null)
                    return _timeZone;

                if (!String.IsNullOrEmpty(xTimeZone))
                {
                    _timeZone = TimeZoneConverter.GetTimeZone(xTimeZone);
                    return _timeZone;
                }

                if (String.IsNullOrEmpty(TZID))
                {
                    _timeZone = TimeZoneConverter.GetTimeZone(TenantManager.GetCurrentTenant().TimeZone);
                    return _timeZone;
                }


                _timeZone = TimeZoneConverter.GetTimeZone(TZID);
                return _timeZone;
            }
            set
            {
                _timeZone = value;
            }
        }

        public string TZID { get; set; }

        public string xTimeZone { get; set; }

        public override List<IEvent> LoadEvents(Guid userId, DateTime utcStartDate, DateTime utcEndDate)
        {
            return Events.Cast<IEvent>().ToList();
        }
        public override List<ITodo> LoadTodos(Guid userId, DateTime utcStartDate, DateTime utcEndDate)
        {
            return new List<ITodo>();
        }
    }
}
