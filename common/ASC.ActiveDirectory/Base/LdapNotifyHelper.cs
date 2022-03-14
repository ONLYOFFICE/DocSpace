/*
 *
 * (c) Copyright Ascensio System Limited 2010-2021
 * 
 * Licensed under the Apache License, Version 2.0 (the "License");
 * you may not use this file except in compliance with the License.
 * You may obtain a copy of the License at
 * http://www.apache.org/licenses/LICENSE-2.0
 * Unless required by applicable law or agreed to in writing, software
 * distributed under the License is distributed on an "AS IS" BASIS,
 * WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 * See the License for the specific language governing permissions and
 * limitations under the License.
 *
*/


using ASC.ActiveDirectory.Base.Settings;
using ASC.ActiveDirectory.ComplexOperations;
using ASC.Common;
using ASC.Common.Threading;
using ASC.Core;
using ASC.Core.Common.Settings;
using ASC.Core.Tenants;
using ASC.Notify;

using Microsoft.Extensions.DependencyInjection;

namespace ASC.ActiveDirectory.Base
{
    [Singletone]
    public class LdapNotifyHelper
    {
        private readonly Dictionary<int, Tuple<INotifyClient, LdapNotifySource>> clients;
        private readonly DistributedTaskQueue ldapTasks;
        private IServiceProvider ServiceProvider { get; set; }

        LdapNotifyHelper(
            IServiceProvider serviceProvider,
            DistributedTaskQueueOptionsManager distributedTaskQueueOptionsManager)
        {
            clients = new Dictionary<int, Tuple<INotifyClient, LdapNotifySource>>();
            ldapTasks = distributedTaskQueueOptionsManager.Get("ldapAutoSyncOperations");
            ServiceProvider = serviceProvider;
        }

        public void RegisterAll()
        {
            var task = new Task(() =>
            {
                using var scope = ServiceProvider.CreateScope();
                var tenantManager = scope.ServiceProvider.GetService<TenantManager>();
                var settingsManager = scope.ServiceProvider.GetService<SettingsManager>();

                var tenants = tenantManager.GetTenants(settingsManager.Load<LdapSettings>().GetTenants());
                foreach (var t in tenants)
                {
                    var tId = t.TenantId;

                    var ldapSettings = settingsManager.LoadForTenant<LdapSettings>(tId);
                    if (!ldapSettings.EnableLdapAuthentication) continue;

                    var cronSettings = settingsManager.LoadForTenant<LdapCronSettings>(tId);
                    if (string.IsNullOrEmpty(cronSettings.Cron)) continue;

                    RegisterAutoSync(t, cronSettings.Cron);
                }
            }, TaskCreationOptions.LongRunning);

            task.Start();
        }

        public void RegisterAutoSync(Tenant tenant, string cron)
        {
            if (!clients.ContainsKey(tenant.TenantId))
            {
                using var scope = ServiceProvider.CreateScope();
                var source = new LdapNotifySource(tenant, this);
                var client = WorkContext.NotifyContext.NotifyService.RegisterClient(source, scope);
                WorkContext.RegisterSendMethod(source.AutoSync, cron);
                clients.Add(tenant.TenantId, new Tuple<INotifyClient, LdapNotifySource>(client, source)); //concurrent dict
            }
        }

        public void UnregisterAutoSync(Tenant tenant)
        {
            if (clients.ContainsKey(tenant.TenantId))
            {
                var client = clients[tenant.TenantId];
                WorkContext.UnregisterSendMethod(client.Item2.AutoSync);
                clients.Remove(tenant.TenantId);
            }
        }

        public void AutoSync(Tenant tenant)
        {
            using var scope = ServiceProvider.CreateScope();
            var settingsManager = scope.ServiceProvider.GetService<SettingsManager>();
            var ldapSettings = settingsManager.LoadForTenant<LdapSettings>(tenant.TenantId);

            if (!ldapSettings.EnableLdapAuthentication)
            {
                var cronSettings = settingsManager.LoadForTenant<LdapCronSettings>(tenant.TenantId);
                cronSettings.Cron = "";
                settingsManager.SaveForTenant(cronSettings, tenant.TenantId);
                UnregisterAutoSync(tenant);
                return;
            }

            var op = scope.ServiceProvider.GetService<LdapSaveSyncOperation>();

            op.Init(ldapSettings, tenant, LdapOperationType.Sync);
            ldapTasks.QueueTask(op.RunJob, op.GetDistributedTask());
        }
    }
}
