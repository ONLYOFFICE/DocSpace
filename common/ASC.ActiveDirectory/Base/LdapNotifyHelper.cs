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

namespace ASC.ActiveDirectory.Base;

[Singletone(Additional = typeof(LdapNotifyHelperExtension))]
public class LdapNotifyService : BackgroundService
{
    private readonly ConcurrentDictionary<int, Tuple<INotifyClient, LdapNotifySource>> _clients;
    private readonly IServiceScopeFactory _serviceScopeFactory;
    private readonly WorkContext _workContext;
    private readonly LdapSaveSyncOperation _ldapSaveSyncOperation;

    public LdapNotifyService(
        IServiceScopeFactory serviceScopeFactory,
        WorkContext workContext,
        LdapSaveSyncOperation ldapSaveSyncOperation)
    {
        _clients = new ConcurrentDictionary<int, Tuple<INotifyClient, LdapNotifySource>>();
        _serviceScopeFactory = serviceScopeFactory;
        _workContext = workContext;
        _ldapSaveSyncOperation = ldapSaveSyncOperation;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        using var scope = _serviceScopeFactory.CreateScope();
        var tenantManager = scope.ServiceProvider.GetRequiredService<TenantManager>();
        var settingsManager = scope.ServiceProvider.GetRequiredService<SettingsManager>();
        var dbHelper = scope.ServiceProvider.GetRequiredService<DbHelper>();

        var tenants = await tenantManager.GetTenantsAsync(await dbHelper.TenantsAsync());
        foreach (var t in tenants)
        {
            var tId = t.Id;

            var ldapSettings = await settingsManager.LoadAsync<LdapSettings>(tId);
            if (!ldapSettings.EnableLdapAuthentication)
            {
                continue;
            }

            var cronSettings = await settingsManager.LoadAsync<LdapCronSettings>(tId);
            if (string.IsNullOrEmpty(cronSettings.Cron))
            {
                continue;
            }

            RegisterAutoSync(t, cronSettings.Cron);
        }
    }

    public void RegisterAutoSync(Tenant tenant, string cron)
    {
        if (!_clients.ContainsKey(tenant.Id))
        {
            var scope = _serviceScopeFactory.CreateScope();
            var source = scope.ServiceProvider.GetRequiredService<LdapNotifySource>();
            var notifyEngineQueue = scope.ServiceProvider.GetRequiredService<NotifyEngineQueue>();
            source.Init(tenant);
            var client = _workContext.NotifyContext.RegisterClient(notifyEngineQueue, source);
            _workContext.RegisterSendMethod(source.AutoSyncAsync, cron);
            _clients.TryAdd(tenant.Id, new Tuple<INotifyClient, LdapNotifySource>(client, source));
        }
    }

    public void UnregisterAutoSync(Tenant tenant)
    {
        if (_clients.ContainsKey(tenant.Id))
        {
            var client = _clients[tenant.Id];
            _workContext.UnregisterSendMethod(client.Item2.AutoSyncAsync);
            _clients.TryRemove(tenant.Id, out _);
        }
    }

    public async Task AutoSyncAsync(Tenant tenant)
    {
        using var scope = _serviceScopeFactory.CreateScope();
        var settingsManager = scope.ServiceProvider.GetRequiredService<SettingsManager>();
        var ldapSettings = await settingsManager.LoadAsync<LdapSettings>(tenant.Id);

        if (!ldapSettings.EnableLdapAuthentication)
        {
            var cronSettings = await settingsManager.LoadAsync<LdapCronSettings>(tenant.Id);
            cronSettings.Cron = "";
            await settingsManager.SaveAsync(cronSettings, tenant.Id);
            UnregisterAutoSync(tenant);
            return;
        }

        await _ldapSaveSyncOperation.RunJobAsync(ldapSettings, tenant, LdapOperationType.Sync);
    }
}

public static class LdapNotifyHelperExtension
{
    public static void Register(DIHelper services)
    {
        services.TryAdd<DbHelper>();
        services.TryAdd<LdapNotifySource>();
        services.TryAdd<NotifyEngineQueue>();
    }
}
