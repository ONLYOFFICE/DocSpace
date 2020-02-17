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
using System.Reflection;

using ASC.Common;
using ASC.Common.Caching;
using ASC.Common.Logging;
using ASC.Core;
using ASC.Core.Common.Settings;
using ASC.Notify.Messages;
using ASC.Web.Core.WhiteLabel;

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace ASC.Notify
{
    public class NotifyService : INotifyService, IDisposable
    {
        private readonly ILog log;

        private readonly ICacheNotify<NotifyMessage> cacheNotify;

        private readonly DbWorker db;

        public IServiceProvider ServiceProvider { get; }

        public NotifyService(DbWorker db, IServiceProvider serviceProvider, ICacheNotify<NotifyMessage> cacheNotify, IOptionsMonitor<ILog> options)
        {
            this.db = db;
            ServiceProvider = serviceProvider;
            this.cacheNotify = cacheNotify;
            log = options.CurrentValue;
        }

        public void Start()
        {
            cacheNotify.Subscribe((n) => SendNotifyMessage(n), CacheNotifyAction.InsertOrUpdate);
        }

        public void Stop()
        {
            cacheNotify.Unsubscribe(CacheNotifyAction.InsertOrUpdate);
        }

        public void SendNotifyMessage(NotifyMessage notifyMessage)
        {
            try
            {
                db.SaveMessage(notifyMessage);
            }
            catch (Exception e)
            {
                log.Error(e);
            }
        }

        public void InvokeSendMethod(string service, string method, int tenant, params object[] parameters)
        {
            var serviceType = Type.GetType(service, true);
            var getinstance = serviceType.GetProperty("Instance", BindingFlags.Static | BindingFlags.Public);

            var instance = getinstance.GetValue(serviceType, null);
            if (instance == null)
            {
                throw new Exception("Service instance not found.");
            }

            var methodInfo = serviceType.GetMethod(method);
            if (methodInfo == null)
            {
                throw new Exception("Method not found.");
            }

            using var scope = ServiceProvider.CreateScope();
            var tenantManager = scope.ServiceProvider.GetService<TenantManager>();
            var tenantWhiteLabelSettingsHelper = scope.ServiceProvider.GetService<TenantWhiteLabelSettingsHelper>();
            var settingsManager = scope.ServiceProvider.GetService<SettingsManager>();
            tenantManager.SetCurrentTenant(tenant);
            tenantWhiteLabelSettingsHelper.Apply(settingsManager.Load<TenantWhiteLabelSettings>(), tenant);
            methodInfo.Invoke(instance, parameters);
        }

        public void Dispose()
        {
            cacheNotify.Unsubscribe(CacheNotifyAction.InsertOrUpdate);
        }
    }

    public static class NotifyServiceExtension
    {
        public static DIHelper AddNotifyService(this DIHelper services)
        {
            services.TryAddSingleton<NotifyService>();
            services.TryAddSingleton(typeof(ICacheNotify<>), typeof(KafkaCache<>));

            return services
                .AddDbWorker();
        }
    }
}
