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
using System.Linq;

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
    [Singletone(Additional = typeof(NotifyServiceExtension))]
    public class NotifyService : INotifyService, IDisposable
    {
        private ILog Log { get; }

        private ICacheNotify<NotifyMessage> CacheNotify { get; }
        private ICacheNotify<NotifyInvoke> CacheInvoke { get; }
        private DbWorker Db { get; }
        private IServiceProvider ServiceProvider { get; }

        public NotifyService(DbWorker db, IServiceProvider serviceProvider, ICacheNotify<NotifyMessage> cacheNotify, ICacheNotify<NotifyInvoke> cacheInvoke, IOptionsMonitor<ILog> options)
        {
            Db = db;
            ServiceProvider = serviceProvider;
            CacheNotify = cacheNotify;
            CacheInvoke = cacheInvoke;
            Log = options.CurrentValue;
        }

        public void Start()
        {
            CacheNotify.Subscribe((n) => SendNotifyMessage(n), CacheNotifyAction.InsertOrUpdate);
            CacheInvoke.Subscribe((n) => InvokeSendMethod(n), CacheNotifyAction.InsertOrUpdate);
        }

        public void Stop()
        {
            CacheNotify.Unsubscribe(CacheNotifyAction.InsertOrUpdate);
        }

        public void SendNotifyMessage(NotifyMessage notifyMessage)
        {
            try
            {
                Db.SaveMessage(notifyMessage);
            }
            catch (Exception e)
            {
                Log.Error(e);
            }
        }


        public void InvokeSendMethod(NotifyInvoke notifyInvoke)
        {
            var service = notifyInvoke.Service;
            var method = notifyInvoke.Method;
            var tenant = notifyInvoke.Tenant;
            var parameters = notifyInvoke.Parameters;

            var serviceType = Type.GetType(service, true);

            using var scope = ServiceProvider.CreateScope();

            var instance = scope.ServiceProvider.GetService(serviceType);
            if (instance == null)
            {
                throw new Exception("Service instance not found.");
            }

            var methodInfo = serviceType.GetMethod(method);
            if (methodInfo == null)
            {
                throw new Exception("Method not found.");
            }

            var scopeClass = scope.ServiceProvider.GetService<NotifyServiceScope>();
            var (tenantManager, tenantWhiteLabelSettingsHelper, settingsManager) = scopeClass;
            tenantManager.SetCurrentTenant(tenant);
            tenantWhiteLabelSettingsHelper.Apply(settingsManager.Load<TenantWhiteLabelSettings>(), tenant);
            methodInfo.Invoke(instance, parameters.ToArray());
        }

        public void Dispose()
        {
            CacheNotify.Unsubscribe(CacheNotifyAction.InsertOrUpdate);
            CacheInvoke.Unsubscribe(CacheNotifyAction.InsertOrUpdate);
        }
    }

    [Scope]
    public class NotifyServiceScope
    {
        private TenantManager TenantManager { get; }
        private TenantWhiteLabelSettingsHelper TenantWhiteLabelSettingsHelper { get; }
        private SettingsManager SettingsManager { get; }

        public NotifyServiceScope(TenantManager tenantManager, TenantWhiteLabelSettingsHelper tenantWhiteLabelSettingsHelper, SettingsManager settingsManager)
        {
            TenantManager = tenantManager;
            TenantWhiteLabelSettingsHelper = tenantWhiteLabelSettingsHelper;
            SettingsManager = settingsManager;
        }

        public void Deconstruct(out TenantManager tenantManager, out TenantWhiteLabelSettingsHelper tenantWhiteLabelSettingsHelper, out SettingsManager settingsManager)
        {
            tenantManager = TenantManager;
            tenantWhiteLabelSettingsHelper = TenantWhiteLabelSettingsHelper;
            settingsManager = SettingsManager;
        }
    }

    public static class NotifyServiceExtension
    {
        public static void Register(DIHelper services)
        {
            services.TryAdd<NotifyServiceScope>();
        }
    }
}
