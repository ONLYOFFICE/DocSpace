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
using ASC.Common.Logging;
using ASC.Core;
using ASC.Core.Users;
using ASC.Notify.Model;
using ASC.Notify.Recipients;
using ASC.Web.Core.WhiteLabel;
using ASC.Web.Studio.Utility;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Options;

namespace ASC.Web.Studio.Core.Notify
{
    public class StudioNotifyHelper
    {
        public readonly string Helplink;

        public readonly StudioNotifySource NotifySource;

        public readonly ISubscriptionProvider SubscriptionProvider;

        public readonly IRecipientProvider RecipientsProvider;


        private UserManager UserManager { get; }
        private SetupInfo SetupInfo { get; }
        private TenantManager TenantManager { get; }
        private ILog Log { get; }

        public StudioNotifyHelper(
            StudioNotifySource studioNotifySource,
            UserManager userManager,
            AdditionalWhiteLabelSettings additionalWhiteLabelSettings,
            CommonLinkUtility commonLinkUtility,
            SetupInfo setupInfo,
            TenantManager tenantManager,
            IOptionsMonitor<LogNLog> option)
        {
            Helplink = commonLinkUtility.GetHelpLink(additionalWhiteLabelSettings, false);
            NotifySource = studioNotifySource;
            UserManager = userManager;
            SetupInfo = setupInfo;
            TenantManager = tenantManager;
            SubscriptionProvider = NotifySource.GetSubscriptionProvider();
            RecipientsProvider = NotifySource.GetRecipientsProvider();
            Log = option.Get("ASC");
        }


        public IEnumerable<UserInfo> GetRecipients(bool toadmins, bool tousers, bool toguests)
        {
            if (toadmins)
            {
                if (tousers)
                {
                    if (toguests)
                        return UserManager.GetUsers();

                    return UserManager.GetUsers(EmployeeStatus.Default, EmployeeType.User);
                }

                if (toguests)
                    return
                        UserManager.GetUsersByGroup(Constants.GroupAdmin.ID)
                                   .Concat(UserManager.GetUsers(EmployeeStatus.Default, EmployeeType.Visitor));

                return UserManager.GetUsersByGroup(Constants.GroupAdmin.ID);
            }

            if (tousers)
            {
                if (toguests)
                    return UserManager.GetUsers()
                                      .Where(u => !UserManager.IsUserInGroup(u.ID, Constants.GroupAdmin.ID));

                return UserManager.GetUsers(EmployeeStatus.Default, EmployeeType.User)
                                  .Where(u => !UserManager.IsUserInGroup(u.ID, Constants.GroupAdmin.ID));
            }

            if (toguests)
                return UserManager.GetUsers(EmployeeStatus.Default, EmployeeType.Visitor);

            return new List<UserInfo>();
        }

        public IRecipient ToRecipient(Guid userId)
        {
            return RecipientsProvider.GetRecipient(userId.ToString());
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


        public string GetNotifyAnalytics(INotifyAction action, bool toowner, bool toadmins,
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
                                               TenantManager.GetCurrentTenant().TenantId,
                                               target,
                                               action.ID));
        }


        public bool IsSubscribedToNotify(Guid userId, INotifyAction notifyAction)
        {
            return IsSubscribedToNotify(ToRecipient(userId), notifyAction);
        }

        public bool IsSubscribedToNotify(IRecipient recipient, INotifyAction notifyAction)
        {
            return recipient != null && SubscriptionProvider.IsSubscribed(Log, notifyAction, recipient, null);
        }

        public void SubscribeToNotify(Guid userId, INotifyAction notifyAction, bool subscribe)
        {
            SubscribeToNotify(ToRecipient(userId), notifyAction, subscribe);
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

    public static class StudioNotifyHelperFactory
    {
        public static IServiceCollection AddStudioNotifyHelperService(this IServiceCollection services)
        {
            services.TryAddScoped<StudioNotifyHelper>();

            return services
                .AddStudioNotifySourceService()
                .AddUserManagerService()
                .AddAdditionalWhiteLabelSettingsService()
                .AddCommonLinkUtilityService()
                .AddTenantManagerService()
                .AddSetupInfo();
        }
    }
}