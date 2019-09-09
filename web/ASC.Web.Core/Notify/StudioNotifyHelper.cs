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
using ASC.Core;
using ASC.Core.Tenants;
using ASC.Core.Users;
using ASC.Notify.Model;
using ASC.Notify.Recipients;
using ASC.Web.Studio.Utility;

namespace ASC.Web.Studio.Core.Notify
{
    public class StudioNotifyHelper
    {
        public readonly string Helplink;

        public readonly StudioNotifySource NotifySource;

        public readonly ISubscriptionProvider SubscriptionProvider;

        public readonly IRecipientProvider RecipientsProvider;

        public UserManager UserManager { get; }

        public StudioNotifyHelper(StudioNotifySource studioNotifySource, UserManager userManager)
        {
            Helplink = CommonLinkUtility.GetHelpLink(false);
            NotifySource = studioNotifySource;
            UserManager = userManager;
            SubscriptionProvider = NotifySource.GetSubscriptionProvider();
            RecipientsProvider = NotifySource.GetRecipientsProvider();
        }


        public IEnumerable<UserInfo> GetRecipients(Tenant tenant, bool toadmins, bool tousers, bool toguests)
        {
            if (toadmins)
            {
                if (tousers)
                {
                    if (toguests)
                        return UserManager.GetUsers(tenant);

                    return UserManager.GetUsers(tenant, EmployeeStatus.Default, EmployeeType.User);
                }

                if (toguests)
                    return
                        UserManager.GetUsersByGroup(tenant, Constants.GroupAdmin.ID)
                                   .Concat(UserManager.GetUsers(tenant, EmployeeStatus.Default, EmployeeType.Visitor));

                return UserManager.GetUsersByGroup(tenant, Constants.GroupAdmin.ID);
            }

            if (tousers)
            {
                if (toguests)
                    return UserManager.GetUsers(tenant)
                                      .Where(u => !UserManager.IsUserInGroup(tenant, u.ID, Constants.GroupAdmin.ID));

                return UserManager.GetUsers(tenant, EmployeeStatus.Default, EmployeeType.User)
                                  .Where(u => !UserManager.IsUserInGroup(tenant, u.ID, Constants.GroupAdmin.ID));
            }

            if (toguests)
                return UserManager.GetUsers(tenant, EmployeeStatus.Default, EmployeeType.Visitor);

            return new List<UserInfo>();
        }

        public IRecipient ToRecipient(int tenantId, Guid userId)
        {
            return RecipientsProvider.GetRecipient(tenantId, userId.ToString());
        }

        public IRecipient[] RecipientFromEmail(string email, bool checkActivation)
        {
            return RecipientFromEmail(new[] { email }, checkActivation);
        }

        public IRecipient[] RecipientFromEmail(string[] emails, bool checkActivation)
        {
            var res = new List<IRecipient>();

            if (emails == null) return res.ToArray();

            res.AddRange(emails.
                             Select(email => email.ToLower()).
                             Select(e => new DirectRecipient(e, null, new[] { e }, checkActivation)));

            return res.ToArray();
        }


        public static string GetNotifyAnalytics(int tenantId, INotifyAction action, bool toowner, bool toadmins,
                                                bool tousers, bool toguests)
        {
            if (string.IsNullOrEmpty(SetupInfo.NotifyAnalyticsUrl))
                return string.Empty;

            var target = "";

            if (toowner) target = "owner";
            if (toadmins) target += string.IsNullOrEmpty(target) ? "admin" : "-admin";
            if (tousers) target += string.IsNullOrEmpty(target) ? "user" : "-user";
            if (toguests) target += string.IsNullOrEmpty(target) ? "guest" : "-guest";

            return string.Format("<img src=\"{0}\" width=\"1\" height=\"1\"/>",
                                 string.Format(SetupInfo.NotifyAnalyticsUrl,
                                               tenantId,
                                               target,
                                               action.ID));
        }


        public bool IsSubscribedToNotify(Tenant tenant, Guid userId, INotifyAction notifyAction)
        {
            return IsSubscribedToNotify(tenant, ToRecipient(tenant.TenantId, userId), notifyAction);
        }

        public bool IsSubscribedToNotify(Tenant tenant, IRecipient recipient, INotifyAction notifyAction)
        {
            return recipient != null && SubscriptionProvider.IsSubscribed(tenant, notifyAction, recipient, null);
        }

        public void SubscribeToNotify(int tenantId, Guid userId, INotifyAction notifyAction, bool subscribe)
        {
            SubscribeToNotify(ToRecipient(tenantId, userId), notifyAction, subscribe);
        }

        public void SubscribeToNotify(IRecipient recipient, INotifyAction notifyAction, bool subscribe)
        {
            if (recipient == null) return;

            if (subscribe)
            {
                SubscriptionProvider.Subscribe(notifyAction, null, recipient);
            }
            else
            {
                SubscriptionProvider.UnSubscribe(notifyAction, null, recipient);
            }
        }
    }
}