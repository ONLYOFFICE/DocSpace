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


namespace ASC.ActiveDirectory.Base;
[Singletone]
public class LdapNotifyHelper
{
    private readonly Dictionary<int, Tuple<INotifyClient, LdapNotifySource>> _clients;
    private readonly DistributedTaskQueue _ldapTasks;
    private readonly IServiceProvider _serviceProvider;

    LdapNotifyHelper(
        IServiceProvider serviceProvider,
        DistributedTaskQueueOptionsManager distributedTaskQueueOptionsManager)
    {
        _clients = new Dictionary<int, Tuple<INotifyClient, LdapNotifySource>>();
        _ldapTasks = distributedTaskQueueOptionsManager.Get("ldapAutoSyncOperations");
        _serviceProvider = serviceProvider;
    }

    public void RegisterAll()
    {
        var task = new Task(() =>
        {
            using var scope = _serviceProvider.CreateScope();
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
        if (!_clients.ContainsKey(tenant.TenantId))
        {
            using var scope = _serviceProvider.CreateScope();
            var source = new LdapNotifySource(tenant, this);
            var client = WorkContext.NotifyContext.NotifyService.RegisterClient(source, scope);
            WorkContext.RegisterSendMethod(source.AutoSync, cron);
            _clients.Add(tenant.TenantId, new Tuple<INotifyClient, LdapNotifySource>(client, source)); //concurrent dict
        }
    }

    public void UnregisterAutoSync(Tenant tenant)
    {
        if (_clients.ContainsKey(tenant.TenantId))
        {
            var client = _clients[tenant.TenantId];
            WorkContext.UnregisterSendMethod(client.Item2.AutoSync);
            _clients.Remove(tenant.TenantId);
        }
    }

    public void AutoSync(Tenant tenant)
    {
        using var scope = _serviceProvider.CreateScope();
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
        _ldapTasks.QueueTask(op.RunJob, op.GetDistributedTask());
    }
}
