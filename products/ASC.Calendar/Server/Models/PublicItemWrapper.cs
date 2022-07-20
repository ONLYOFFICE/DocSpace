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
using System.Net.Http;
using System.Runtime.Serialization;
using System.Text.Json.Serialization;

using ASC.Calendar.BusinessObjects;
using ASC.Calendar.iCalParser;
using ASC.Common;
using ASC.Common.Security.Authorizing;
using ASC.Common.Utils;
using ASC.Core;
using ASC.Core.Users;
using ASC.Web.Core.Users;

namespace ASC.Calendar.Models
{
    [DataContract(Name = "publicItem")]
    [Scope]
    public class PublicItemWrapper : ASC.Web.Core.Calendars.SharingOptions.PublicItem
    {
        [DataMember(Name = "id", Order = 10)]
        public string ItemId { get; set; }

        [DataMember(Name = "name", Order = 20)]
        public string ItemName { get; set; }

        [DataMember(Name = "isGroup", Order = 30)]
        public new bool IsGroup { get; set; }

        [DataMember(Name = "canEdit", Order = 40)]
        public bool CanEdit { get; set; }

        [JsonPropertyName("selectedAction")]
        public AccessOption SharingOption { get; set; }

        public static PublicItemWrapper GetSample()
        {
            return new PublicItemWrapper
            {
                SharingOption = AccessOption.GetSample(),
                CanEdit = true,
                IsGroup = true,
                ItemName = "Everyone",
                ItemId = "2fdfe577-3c26-4736-9df9-b5a683bb8520"
            };
        }
    }

    [Scope]
    public class PublicItemWrapperHelper
    {
        private Guid _owner;
        private string _calendarId;
        private string _eventId;
        private bool _isCalendar;

        public UserManager UserManager { get; }
        private AuthManager Authentication { get; }
        public AuthContext AuthContext { get; }
        private TenantManager TenantManager { get; }
        private TimeZoneConverter TimeZoneConverter { get; }
        private PermissionContext PermissionContext { get; }
        private IHttpClientFactory ClientFactory { get; }
        public DisplayUserSettingsHelper DisplayUserSettingsHelper { get; }
        public DataProvider DataProvider { get; }


        public PublicItemWrapperHelper(
            UserManager userManager,
            AuthManager authentication,
            AuthContext authContext,
            TenantManager tenantManager,
            TimeZoneConverter timeZoneConverter,
            PermissionContext permissionContext,
            DisplayUserSettingsHelper displayUserSettingsHelper,
            DataProvider dataProvider,
            IHttpClientFactory clientFactory)
        {
            UserManager = userManager;
            Authentication = authentication;
            TenantManager = tenantManager;
            AuthContext = authContext;
            TimeZoneConverter = timeZoneConverter;
            PermissionContext = permissionContext;
            DisplayUserSettingsHelper = displayUserSettingsHelper;
            DataProvider = dataProvider;
            ClientFactory = clientFactory;
        }

        public PublicItemWrapper Get(ASC.Web.Core.Calendars.SharingOptions.PublicItem publicItem, string calendartId, Guid owner)
        {
            var result = new PublicItemWrapper();

            result.Id = publicItem.Id;
            result.IsGroup = publicItem.IsGroup;

            _owner = owner;
            _calendarId = calendartId;
            _isCalendar = true;

            Init(publicItem, ref result);

            return result;
        }
        public PublicItemWrapper Get(ASC.Web.Core.Calendars.SharingOptions.PublicItem publicItem, string calendarId, string eventId, Guid owner)
        {
            var result = new PublicItemWrapper();

            result.Id = publicItem.Id;
            result.IsGroup = publicItem.IsGroup;

            _owner = owner;
            _calendarId = calendarId;
            _eventId = eventId;
            _isCalendar = false;

            Init(publicItem, ref result);

            return result;
        }

        protected void Init(ASC.Web.Core.Calendars.SharingOptions.PublicItem publicItem, ref PublicItemWrapper result)
        {
            result.ItemId = publicItem.Id.ToString();

            //---ItemName
            if (result.IsGroup)
                result.ItemName = UserManager.GetGroupInfo(publicItem.Id).Name;
            else
                result.ItemName = UserManager.GetUsers(publicItem.Id).DisplayUserName(DisplayUserSettingsHelper);

            //---CanEdit
            result.CanEdit = !publicItem.Id.Equals(_owner);

            //---SharingOption
            if (publicItem.Id.Equals(_owner))
            {
                result.SharingOption = AccessOption.OwnerOption;
            }
            else
            {
                var subject = publicItem.IsGroup ? (ISubject)UserManager.GetGroupInfo(publicItem.Id) : (ISubject)Authentication.GetAccountByID(TenantManager.GetCurrentTenant().Id, publicItem.Id);
                int calId;
                if (_isCalendar && int.TryParse(_calendarId, out calId))
                {
                    var icalendar = new iCalendar(AuthContext, TimeZoneConverter, TenantManager, ClientFactory);
                    var obj = new BusinessObjects.Calendar(AuthContext, TimeZoneConverter, icalendar, DataProvider) { Id = _calendarId };
                    if (PermissionContext.PermissionResolver.Check(subject, obj, null, CalendarAccessRights.FullAccessAction))
                        result.SharingOption = AccessOption.FullAccessOption;
                    else
                        result.SharingOption = AccessOption.ReadOption;
                }
                else if (!_isCalendar)
                {
                    var icalendar = new iCalendar(AuthContext, TimeZoneConverter, TenantManager, ClientFactory);
                    var obj = new BusinessObjects.Event(AuthContext, TimeZoneConverter, icalendar, DataProvider) { Id = _eventId, CalendarId = _calendarId };
                    if (PermissionContext.PermissionResolver.Check(subject, obj, null, CalendarAccessRights.FullAccessAction))
                        result.SharingOption = AccessOption.FullAccessOption;
                    else
                        result.SharingOption = AccessOption.ReadOption;
                }
                else
                {
                    result.SharingOption = AccessOption.ReadOption;
                }

            }

        }
    }
}
