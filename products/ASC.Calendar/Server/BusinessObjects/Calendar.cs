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


using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;

using ASC.Api.Core;
using ASC.Calendar.iCalParser;
using ASC.Calendar.Models;
using ASC.Common.Security;
using ASC.Common.Security.Authorizing;
using ASC.Common.Utils;
using ASC.Core;
using ASC.Web.Core.Calendars;

namespace ASC.Calendar.BusinessObjects
{

    public static class CalendarExtention
    {
        public static bool IsiCalStream(this BaseCalendar calendar)
        {
            return (calendar is BusinessObjects.Calendar && !String.IsNullOrEmpty((calendar as BusinessObjects.Calendar).iCalUrl));
        }
        public static bool IsExistTodo(this BaseCalendar calendar)
        {
            return (calendar is BusinessObjects.Calendar && (calendar as BusinessObjects.Calendar).IsTodo != 0);
        }

        public static BaseCalendar GetUserCalendar(this BaseCalendar calendar, UserViewSettings userViewSettings)
        {
            var cal = (BaseCalendar)calendar.Clone();

            if (userViewSettings == null)
                return cal;

            //name             
            if (!String.IsNullOrEmpty(userViewSettings.Name))
                cal.Name = userViewSettings.Name;

            //backgroundColor
            if (!String.IsNullOrEmpty(userViewSettings.BackgroundColor))
                cal.Context.HtmlBackgroundColor = userViewSettings.BackgroundColor;

            //textColor
            if (!String.IsNullOrEmpty(userViewSettings.TextColor))
                cal.Context.HtmlTextColor = userViewSettings.TextColor;

            //TimeZoneInfo      
            if (userViewSettings.TimeZone != null)
                cal.TimeZone = userViewSettings.TimeZone;

            //alert type            
            cal.EventAlertType = userViewSettings.EventAlertType;

            return cal;
        }

        public static List<EventWrapper> GetEventWrappers(this BaseCalendar calendar, Guid userId, ApiDateTime startDate, ApiDateTime endDate, EventWrapperHelper eventWrapperHelper)
        {
            var result = new List<EventWrapper>();
            if (calendar != null)
            {
                var events = calendar.LoadEvents(userId, startDate.UtcTime, endDate.UtcTime);
                foreach (var e in events)
                {
                    var wrapper = eventWrapperHelper.Get(e, userId, calendar.TimeZone);
                    var listWrapper = eventWrapperHelper.GetList(startDate.UtcTime, endDate.UtcTime, userId, e);
                    result.AddRange(listWrapper);
                }
            }

            return result;
        }
        public static List<TodoWrapper> GetTodoWrappers(this BaseCalendar calendar, Guid userId, ApiDateTime startDate, ApiDateTime endDate, TodoWrapperHelper todoWrapperHelper)
        {
            var result = new List<TodoWrapper>();
            if (calendar != null)
            {
                var events = calendar.LoadTodos(userId, startDate.UtcTime, endDate.UtcTime);
                foreach (var e in events)
                {
                    var wrapper = todoWrapperHelper.Get(e, userId, calendar.TimeZone);
                    result.Add(wrapper);
                }
            }
            return result;

        }
    }


    [DataContract(Name = "calendar", Namespace = "")]
    public class Calendar : BaseCalendar, ISecurityObject
    {
        public static string DefaultTextColor { get { return "#000000"; } }
        public static string DefaultBackgroundColor { get { return "#9bb845"; } }
        public static string DefaultTodoBackgroundColor { get { return "#ffb45e"; } }
        public string FullId => AzObjectIdHelper.GetFullObjectId(this);
        public iCalendar ICalendar { get; }
        public DataProvider DataProvider { get; }


        public Calendar(
            AuthContext context,
            TimeZoneConverter timeZoneConverter,
            iCalendar iCalendar,
            DataProvider dataProvider)
        : base(context, timeZoneConverter)
        {
            ICalendar = iCalendar;
            DataProvider = dataProvider;
            this.ViewSettings = new List<UserViewSettings>();
            this.Context.CanChangeAlertType = true;
            this.Context.CanChangeTimeZone = true;
        }

        public int TenantId { get; set; }

        public List<UserViewSettings> ViewSettings { get; set; }

        public string iCalUrl { get; set; }

        public string calDavGuid { get; set; }

        public int IsTodo { get; set; }

        #region ISecurityObjectId Members

        /// <inheritdoc/>
        public object SecurityId
        {
            get { return this.Id; }
        }

        /// <inheritdoc/>
        public Type ObjectType
        {
            get { return typeof(Calendar); }
        }

        #endregion

        #region ISecurityObjectProvider Members

        public IEnumerable<ASC.Common.Security.Authorizing.IRole> GetObjectRoles(ASC.Common.Security.Authorizing.ISubject account, ISecurityObjectId objectId, SecurityCallContext callContext)
        {
            List<IRole> roles = new List<IRole>();
            if (account.ID.Equals(this.OwnerId))
                roles.Add(ASC.Common.Security.Authorizing.Constants.Owner);

            return roles;
        }

        public ISecurityObjectId InheritFrom(ISecurityObjectId objectId)
        {
            throw new NotImplementedException();
        }

        public bool InheritSupported
        {
            get { return false; }
        }

        public bool ObjectRolesSupported
        {
            get { return true; }
        }

        #endregion

        public override List<IEvent> LoadEvents(Guid userId, DateTime utcStartDate, DateTime utcEndDate)
        {
            if (!String.IsNullOrEmpty(iCalUrl))
            {
                try
                {
                    var cal = ICalendar.GetFromUrl(iCalUrl, this.Id);
                    return cal.LoadEvents(userId, utcStartDate, utcEndDate);
                }
                catch
                {
                    return new List<IEvent>();
                }
            }

            return DataProvider.LoadEvents(Convert.ToInt32(this.Id), userId, TenantId, utcStartDate, utcEndDate)
                        .Cast<IEvent>()
                        .ToList();

        }

        public override List<ITodo> LoadTodos(Guid userId, DateTime utcStartDate, DateTime utcEndDate)
        {
            if (!String.IsNullOrEmpty(iCalUrl))
            {
                try
                {
                    var cal = ICalendar.GetFromUrl(iCalUrl, this.Id);
                    return cal.LoadTodos(userId, utcStartDate, utcEndDate);
                }
                catch
                {
                    return new List<ITodo>();
                }
            }

            return DataProvider.LoadTodos(Convert.ToInt32(this.Id), userId, TenantId, utcStartDate, utcEndDate)
                        .Cast<ITodo>()
                        .ToList();

        }
    }

}
