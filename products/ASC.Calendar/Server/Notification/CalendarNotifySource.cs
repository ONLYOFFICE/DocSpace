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

using ASC.Common;
using ASC.Core;
using ASC.Core.Notify;
using ASC.Core.Users;
using ASC.Notify;
using ASC.Notify.Engine;
using ASC.Notify.Model;
using ASC.Notify.Patterns;
using ASC.Notify.Recipients;
using ASC.Web.Core.Users;
using ASC.Web.Studio.Utility;

using NotifyContext = ASC.Notify.Context;

namespace ASC.Calendar.Notification
{

    [Scope]
    public class CalendarNotifyClient
    {
        private static INotifyClient _notifyClient;

        private static readonly string _syncName = "calendarNotifySyncName";

        public AuthContext AuthContext { get; }
        public IServiceProvider ServiceProvider { get; }
        public UserManager UserManager { get; }
        private CommonLinkUtility CommonLinkUtility { get; }
        private CalendarNotifySource CalendarNotifySource { get; }
        public DisplayUserSettingsHelper DisplayUserSettingsHelper { get; }

        public static NotifyContext NotifyContext { get; private set; }
        public CalendarNotifyClient(
            AuthContext authContext,
            IServiceProvider serviceProvider,
            UserManager userManager,
            CalendarNotifySource calendarNotifySource,
            CommonLinkUtility commonLinkUtility,
            DisplayUserSettingsHelper displayUserSettingsHelper,
            WorkContext workContext,
            NotifyEngineQueue notifyEngineQueue)
        {
            AuthContext = authContext;
            ServiceProvider = serviceProvider;
            CalendarNotifySource = calendarNotifySource;
            UserManager = userManager;
            CommonLinkUtility = commonLinkUtility;
            DisplayUserSettingsHelper = displayUserSettingsHelper;

            _notifyClient = workContext.NotifyContext.RegisterClient(notifyEngineQueue, CalendarNotifySource);
        }

        public void NotifyAboutSharingCalendar(ASC.Calendar.BusinessObjects.Calendar calendar)
        {
            NotifyAboutSharingCalendar(calendar, null);
        }
        public void NotifyAboutSharingCalendar(ASC.Calendar.BusinessObjects.Calendar calendar, ASC.Calendar.BusinessObjects.Calendar oldCalendar)
        {
            var initatorInterceptor = new InitiatorInterceptor(new DirectRecipient(AuthContext.CurrentAccount.ID.ToString(), AuthContext.CurrentAccount.Name));
            try
            {
                var usr = UserManager.GetUsers(AuthContext.CurrentAccount.ID);
                var userLink = PerformUrl(CommonLinkUtility.GetUserProfile(usr.Id.ToString(), false));

                foreach (var item in calendar.SharingOptions.PublicItems)
                {
                    if (oldCalendar != null && oldCalendar.SharingOptions.PublicItems.Exists(i => i.Id.Equals(item.Id)))
                        continue;

                    var r = CalendarNotifySource.GetRecipientsProvider().GetRecipient(item.Id.ToString());
                    if (r != null)
                    {
                        _notifyClient.SendNoticeAsync(CalendarNotifySource.CalendarSharing, null, r, true,
                            new TagValue("SharingType", "calendar"),
                            new TagValue("UserName", usr.DisplayUserName(DisplayUserSettingsHelper)),
                            new TagValue("UserLink", userLink),
                            new TagValue("CalendarName", calendar.Name));
                    }
                }
                _notifyClient.EndSingleRecipientEvent(_syncName);
            }
            finally
            {
                _notifyClient.RemoveInterceptor(initatorInterceptor.Name);
            }
        }
        public void NotifyAboutSharingEvent(ASC.Calendar.BusinessObjects.Event calendarEvent)
        {
            NotifyAboutSharingEvent(calendarEvent, null);
        }
        public void NotifyAboutSharingEvent(ASC.Calendar.BusinessObjects.Event calendarEvent, ASC.Calendar.BusinessObjects.Event oldCalendarEvent)
        {
            var initatorInterceptor = new InitiatorInterceptor(new DirectRecipient(AuthContext.CurrentAccount.ID.ToString(), AuthContext.CurrentAccount.Name));
            try
            {
                var usr = UserManager.GetUsers(AuthContext.CurrentAccount.ID);
                var userLink = PerformUrl(CommonLinkUtility.GetUserProfile(usr.Id.ToString(), false));

                foreach (var item in calendarEvent.SharingOptions.PublicItems)
                {
                    if (oldCalendarEvent != null && oldCalendarEvent.SharingOptions.PublicItems.Exists(i => i.Id.Equals(item.Id)))
                        continue;

                    var r = CalendarNotifySource.GetRecipientsProvider().GetRecipient(item.Id.ToString());
                    if (r != null)
                    {
                        _notifyClient.SendNoticeAsync(CalendarNotifySource.CalendarSharing, null, r, true,
                            new TagValue("SharingType", "event"),
                            new TagValue("UserName", usr.DisplayUserName(DisplayUserSettingsHelper)),
                            new TagValue("UserLink", userLink),
                            new TagValue("EventName", calendarEvent.Name));
                    }
                }
                _notifyClient.EndSingleRecipientEvent(_syncName);
            }
            finally
            {
                _notifyClient.RemoveInterceptor(initatorInterceptor.Name);
            }
        }

        private string PerformUrl(string url)
        {
            return CommonLinkUtility.GetFullAbsolutePath(url);
        }
    }

    [Scope]
    public class CalendarNotifySource : NotifySource
    {
        public static INotifyAction CalendarSharing = new NotifyAction("CalendarSharingPattern");
        public static INotifyAction EventAlert = new NotifyAction("EventAlertPattern");

        public CalendarNotifySource(UserManager userManager, IRecipientProvider recipientsProvider, SubscriptionManager subscriptionManager)
            : base(new Guid("{40650DA3-F7C1-424c-8C89-B9C115472E08}"), userManager, recipientsProvider, subscriptionManager)
        {
        }


        protected override IActionProvider CreateActionProvider()
        {
            return new ConstActionProvider(
                CalendarSharing,
                EventAlert);
        }

        protected override IPatternProvider CreatePatternsProvider()
        {
            return new XmlPatternProvider2(CalendarPatterns.calendar_patterns);
        }
    }
}
