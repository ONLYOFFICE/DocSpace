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
using System.Runtime.Serialization;
using ASC.Common.Security;
using ASC.Common.Security.Authorizing;
using ASC.Calendar.Models;
using ASC.Web.Core.Calendars;
using ASC.Core.Tenants;
using ASC.Core;
using ASC.Common.Utils;
using ASC.Calendar.iCalParser;

namespace ASC.Calendar.BusinessObjects
{    
    
    [AllDayLongUTCAttribute]
    public class Event : BaseEvent, ISecurityObject
    {
        private AuthContext AuthContext { get; }
        private TimeZoneConverter TimeZoneConverter { get; }
        private iCalendar ICalendar { get; }
        private DataProvider DataProvider { get; }
        
        public Event(AuthContext context,
            TimeZoneConverter timeZoneConverter,
            iCalendar iCalendar,
            DataProvider dataProvider)
        {
            AuthContext = context;
            ICalendar = iCalendar;
            TimeZoneConverter = timeZoneConverter;
            DataProvider = dataProvider;
        }

        public int TenantId { get; set; }

        #region ISecurityObjectId Members

        public object SecurityId
        {
            get { return this.Id; }
        }

        /// <inheritdoc/>
        public Type ObjectType
        {
            get { return typeof(Event); }
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
            int calId;
            if (int.TryParse(this.CalendarId, out calId))
                return new Calendar(AuthContext, TimeZoneConverter, ICalendar, DataProvider) { Id = this.CalendarId };

            return null;
        }

        public bool InheritSupported
        {
            get { return true; }
        }

        public bool ObjectRolesSupported
        {
            get { return true; }
        }

        #endregion
       
    }
    
}
