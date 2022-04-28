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

namespace ASC.Notify.Services;

[Singletone]
public class NotifyService : IHostedService
{
    private readonly DbWorker _db;
    private readonly ICacheNotify<NotifyInvoke> _cacheInvoke;
    private readonly ICacheNotify<NotifyMessage> _cacheNotify;
    private readonly ILogger _logger;
    private readonly IServiceScopeFactory _serviceScopeFactory;
    private readonly NotifyConfiguration _notifyConfiguration;
    private readonly NotifyServiceCfg _notifyServiceCfg;

    public NotifyService(
        IOptions<NotifyServiceCfg> notifyServiceCfg,
        DbWorker db,
        ICacheNotify<NotifyInvoke> cacheInvoke,
        ICacheNotify<NotifyMessage> cacheNotify,
        ILoggerProvider options,
        IServiceScopeFactory serviceScopeFactory,
        NotifyConfiguration notifyConfiguration)
    {
        _cacheInvoke = cacheInvoke;
        _cacheNotify = cacheNotify;
        _db = db;
        _logger = options.CreateLogger("ASC.NotifyService");
        _notifyConfiguration = notifyConfiguration;
        _notifyServiceCfg = notifyServiceCfg.Value;
        _serviceScopeFactory = serviceScopeFactory;
    }

    public Task StartAsync(CancellationToken cancellationToken)
    {
        _logger.LogInformation("Notify Service running.");

        _cacheNotify.Subscribe((n) => SendNotifyMessage(n), CacheNotifyAction.InsertOrUpdate);
        _cacheInvoke.Subscribe((n) => InvokeSendMethod(n), CacheNotifyAction.InsertOrUpdate);

        if (0 < _notifyServiceCfg.Schedulers.Count)
        {
            InitializeNotifySchedulers();
        }

        return Task.CompletedTask;
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        _logger.LogInformation("Notify Service is stopping.");

        _cacheNotify.Unsubscribe(CacheNotifyAction.InsertOrUpdate);
        _cacheInvoke.Unsubscribe(CacheNotifyAction.InsertOrUpdate);

        return Task.CompletedTask;
    }

    private void SendNotifyMessage(NotifyMessage notifyMessage)
    {
        try
        {
            _db.SaveMessage(notifyMessage);
        }
        catch (Exception e)
        {
            _logger.LogError(e, "SendNotifyMessage");
        }
    }

    private void InvokeSendMethod(NotifyInvoke notifyInvoke)
    {
        var service = notifyInvoke.Service;
        var method = notifyInvoke.Method;
        var tenant = notifyInvoke.Tenant;
        var parameters = notifyInvoke.Parameters;

        var serviceType = Type.GetType(service, true);

        using var scope = _serviceScopeFactory.CreateScope();

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

        var tenantManager = scope.ServiceProvider.GetService<TenantManager>();
        var tenantWhiteLabelSettingsHelper = scope.ServiceProvider.GetService<TenantWhiteLabelSettingsHelper>();
        var settingsManager = scope.ServiceProvider.GetService<SettingsManager>();

        tenantManager.SetCurrentTenant(tenant);
        tenantWhiteLabelSettingsHelper.Apply(settingsManager.Load<TenantWhiteLabelSettings>(), tenant);
        methodInfo.Invoke(instance, parameters.ToArray());
    }

    private void InitializeNotifySchedulers()
    {
        _notifyConfiguration.Configure();
        foreach (var pair in _notifyServiceCfg.Schedulers.Where(r => r.MethodInfo != null))
        {
            _logger.LogDebug("Start scheduler {notifyInfo} ({notifyMethodIndo})", pair.Name, pair.MethodInfo);
            pair.MethodInfo.Invoke(null, null);
        }
    }
}
