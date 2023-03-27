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

namespace ASC.Notify.IntegrationEvents.EventHandling;

[Scope]
public class NotifyInvokeSendMethodRequestedIntegrationEventHandler : IIntegrationEventHandler<NotifyInvokeSendMethodRequestedIntegrationEvent>
{
    private readonly ILogger _logger;
    private readonly IServiceScopeFactory _serviceScopeFactory;

    public NotifyInvokeSendMethodRequestedIntegrationEventHandler(
        ILoggerProvider options,
        IServiceScopeFactory serviceScopeFactory)
    {
        _logger = options.CreateLogger("ASC.NotifyService");
        _serviceScopeFactory = serviceScopeFactory;
    }

    private async Task InvokeSendMethodAsync(NotifyInvoke notifyInvoke)
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

        await tenantManager.SetCurrentTenantAsync(tenant);
        await tenantWhiteLabelSettingsHelper.ApplyAsync(await settingsManager.LoadAsync<TenantWhiteLabelSettings>(), tenant);
        methodInfo.Invoke(instance, parameters.ToArray());
    }


    public async Task Handle(NotifyInvokeSendMethodRequestedIntegrationEvent @event)
    {
        CustomSynchronizationContext.CreateContext();
        using (_logger.BeginScope(new[] { new KeyValuePair<string, object>("integrationEventContext", $"{@event.Id}-{Program.AppName}") }))
        {
            _logger.InformationHandlingIntegrationEvent(@event.Id, Program.AppName, @event);

            await InvokeSendMethodAsync(@event.NotifyInvoke);

            await Task.CompletedTask;
        }
    }
}