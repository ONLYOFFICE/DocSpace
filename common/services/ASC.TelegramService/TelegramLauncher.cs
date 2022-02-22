/*
 *
 * (c) Copyright Ascensio System Limited 2010-2020
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
using System.Threading;
using System.Threading.Tasks;

using ASC.Common;
using ASC.Core;
using ASC.Core.Common.Configuration;
using ASC.FederatedLogin.LoginProviders;

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace ASC.TelegramService
{
    [Singletone(Additional = typeof(TelegramLauncherExtension))]
    public class TelegramLauncher : IHostedService
    {
        private TelegramListener TelegramListener { get; set; }
        private IServiceProvider ServiceProvider { get; set; }

        public TelegramLauncher(TelegramListener telegramListener, IServiceProvider serviceProvider)
        {
            TelegramListener = telegramListener;
            ServiceProvider = serviceProvider;
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            Task.Run(CreateClients);
            TelegramListener.Start();
            return Task.CompletedTask;
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            TelegramListener.Stop();
            return Task.CompletedTask;
        }

        private void CreateClients()
        {
            var scopeClass = ServiceProvider.CreateScope().ServiceProvider.GetService<ScopeTelegramLauncher>();
            var (tenantManager, handler, telegramLoginProvider) = scopeClass;
            var tenants = tenantManager.GetTenants();
            foreach (var tenant in tenants)
            {
                tenantManager.SetCurrentTenant(tenant);
                if (telegramLoginProvider.IsEnabled())
                {
                    handler.CreateOrUpdateClientForTenant(tenant.TenantId, telegramLoginProvider.TelegramBotToken, telegramLoginProvider.TelegramAuthTokenLifespan, telegramLoginProvider.TelegramProxy, true, true);
                }
            }
        }
    }

    [Scope]
    public class ScopeTelegramLauncher
    {
        private TelegramHandler Handler { get; set; }
        private TenantManager TenantManager { get; set; }
        private TelegramLoginProvider TelegramLoginProvider { get; set; }

        public ScopeTelegramLauncher(TenantManager tenantManager, TelegramHandler telegramHandler, ConsumerFactory consumerFactory)
        {
            TelegramLoginProvider = consumerFactory.Get<TelegramLoginProvider>();
            TenantManager = tenantManager;
            Handler = telegramHandler;
        }

        public void Deconstruct(out TenantManager tenantManager, out TelegramHandler handler, out TelegramLoginProvider telegramLoginProvider)
        {
            tenantManager = TenantManager;
            handler = Handler;
            telegramLoginProvider = TelegramLoginProvider;
        }
    }

    public static class TelegramLauncherExtension
    {
        public static void Register(DIHelper services)
        {
            services.TryAdd<ScopeTelegramLauncher>();
        }
    }
}