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
using System.Globalization;
using System.Linq;
using System.Threading;
using ASC.Common.Caching;
using ASC.Common.Utils;
using ASC.Core;
using ASC.Notify;
using ASC.Notify.Model;
using ASC.Notify.Patterns;
using ASC.Notify.Recipients;
using ASC.Web.Studio.Utility;
using Microsoft.Extensions.DependencyInjection;

namespace ASC.Web.Studio.Core.Notify
{
    public class StudioNotifyServiceSender
    {
        private readonly INotifyClient client;
        private readonly ICacheNotify<NotifyItem> cache;

        private static string EMailSenderName { get { return ASC.Core.Configuration.Constants.NotifyEMailSenderSysName; } }

        public IServiceProvider ServiceProvider { get; }

        public StudioNotifyServiceSender(IServiceProvider serviceProvider)
        {
            cache = new KafkaCache<NotifyItem>();
            cache.Subscribe(OnMessage, CacheNotifyAction.Any);
            ServiceProvider = serviceProvider;
        }

        public void OnMessage(NotifyItem item)
        {
            using var scope = ServiceProvider.CreateScope();
            var tenantManager = scope.ServiceProvider.GetService<TenantManager>();
            var userManager = scope.ServiceProvider.GetService<UserManager>();
            var securityContext = scope.ServiceProvider.GetService<SecurityContext>();
            var authContext = scope.ServiceProvider.GetService<AuthContext>();
            var studioNotifyHelper = scope.ServiceProvider.GetService<StudioNotifyHelper>();

            tenantManager.SetCurrentTenant(item.TenantId);
            securityContext.AuthenticateMe(Guid.Parse(item.UserId));
            CultureInfo culture = null;

            var client = WorkContext.NotifyContext.NotifyService.RegisterClient(studioNotifyHelper.NotifySource, userManager, authContext);

            var tenant = tenantManager.GetCurrentTenant(false);

            if (tenant != null)
            {
                culture = tenant.GetCulture();
            }

            var user = userManager.GetUsers(securityContext.CurrentAccount.ID);
            if (!string.IsNullOrEmpty(user.CultureName))
            {
                culture = CultureInfo.GetCultureInfo(user.CultureName);
            }

            if (culture != null && !Equals(Thread.CurrentThread.CurrentCulture, culture))
            {
                Thread.CurrentThread.CurrentCulture = culture;
            }
            if (culture != null && !Equals(Thread.CurrentThread.CurrentUICulture, culture))
            {
                Thread.CurrentThread.CurrentUICulture = culture;
            }

            client.SendNoticeToAsync(
                (NotifyAction)item.Action,
                item.ObjectID,
                item.Recipients?.Select(r => r.IsGroup ? new RecipientsGroup(r.ID, r.Name) : (IRecipient)new DirectRecipient(r.ID, r.Name, r.Addresses.ToArray(), r.CheckActivation)).ToArray(),
                item.SenderNames.Any() ? item.SenderNames.ToArray() : null,
                item.CheckSubsciption,
                item.Tags.Select(r => new TagValue(r.Tag_, r.Value)).ToArray());
        }

        public void RegisterSendMethod()
        {
            var cron = ConfigurationManager.AppSettings["core:notify:cron"] ?? "0 0 5 ? * *"; // 5am every day

            using var scope = ServiceProvider.CreateScope();
            var tenantExtra = scope.ServiceProvider.GetService<TenantExtra>();
            var coreBaseSettings = scope.ServiceProvider.GetService<CoreBaseSettings>();

            if (ConfigurationManager.AppSettings["core:notify:tariff"] != "false")
            {
                if (tenantExtra.Enterprise)
                {
                    client.RegisterSendMethod(SendEnterpriseTariffLetters, cron);
                }
                else if (tenantExtra.Opensource)
                {
                    client.RegisterSendMethod(SendOpensourceTariffLetters, cron);
                }
                else if (tenantExtra.Saas)
                {
                    if (coreBaseSettings.Personal)
                    {
                        client.RegisterSendMethod(SendLettersPersonal, cron);
                    }
                    else
                    {
                        client.RegisterSendMethod(SendSaasTariffLetters, cron);
                    }
                }
            }

            if (!coreBaseSettings.Personal)
            {
                client.RegisterSendMethod(SendMsgWhatsNew, "0 0 * ? * *"); // every hour
            }
        }

        public void SendSaasTariffLetters(DateTime scheduleDate)
        {
            StudioPeriodicNotify.SendSaasLetters(client, EMailSenderName, scheduleDate, ServiceProvider);
        }

        public void SendEnterpriseTariffLetters(DateTime scheduleDate)
        {
            StudioPeriodicNotify.SendEnterpriseLetters(client, EMailSenderName, scheduleDate, ServiceProvider);
        }

        public void SendOpensourceTariffLetters(DateTime scheduleDate)
        {
            StudioPeriodicNotify.SendOpensourceLetters(client, EMailSenderName, scheduleDate, ServiceProvider);
        }

        public void SendLettersPersonal(DateTime scheduleDate)
        {
            StudioPeriodicNotify.SendPersonalLetters(client, EMailSenderName, scheduleDate, ServiceProvider);
        }

        public void SendMsgWhatsNew(DateTime scheduleDate)
        {
            using var scope = ServiceProvider.CreateScope();
            scope.ServiceProvider.GetService<StudioWhatsNewNotify>().SendMsgWhatsNew(scheduleDate, client);
        }
    }
}