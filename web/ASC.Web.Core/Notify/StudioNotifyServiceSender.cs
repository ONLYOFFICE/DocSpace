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

using Constants = ASC.Core.Configuration.Constants;

namespace ASC.Web.Studio.Core.Notify;

[Singletone(Additional = typeof(ServiceLauncherExtension))]
public class StudioNotifyServiceSender
{
    private static string EMailSenderName { get { return Constants.NotifyEMailSenderSysName; } }

    private readonly IServiceScopeFactory _serviceProvider;
    private readonly IConfiguration _configuration;
    private readonly WorkContext _workContext;
    private readonly TenantExtraConfig _tenantExtraConfig;
    private readonly CoreBaseSettings _coreBaseSettings;

    public StudioNotifyServiceSender(
        IServiceScopeFactory serviceProvider,
        IConfiguration configuration,
        ICacheNotify<NotifyItem> cache,
        WorkContext workContext,
        TenantExtraConfig tenantExtraConfig,
        CoreBaseSettings coreBaseSettings)
    {
        cache.Subscribe(OnMessage, CacheNotifyAction.Any);
        _serviceProvider = serviceProvider;
        _configuration = configuration;
        _workContext = workContext;
        _tenantExtraConfig = tenantExtraConfig;
        _coreBaseSettings = coreBaseSettings;
    }

    public void OnMessage(NotifyItem item)
    {
        using var scope = _serviceProvider.CreateScope();
        var scopeClass = scope.ServiceProvider.GetRequiredService<StudioNotifyWorker>();
        scopeClass.OnMessageAsync(item).Wait();
    }

    public void RegisterSendMethod()
    {
        var cron = _configuration["core:notify:cron"] ?? "0 0 5 ? * *"; // 5am every day

        if (_configuration["core:notify:tariff"] != "false")
        {
            if (_tenantExtraConfig.Enterprise)
            {
                _workContext.RegisterSendMethod(SendEnterpriseTariffLettersAsync, cron);
            }
            else if (_tenantExtraConfig.Opensource)
            {
                _workContext.RegisterSendMethod(SendOpensourceTariffLettersAsync, cron);
            }
            else if (_tenantExtraConfig.Saas)
            {
                if (_coreBaseSettings.Personal)
                {
                    if (!_coreBaseSettings.CustomMode)
                    {
                        _workContext.RegisterSendMethod(SendLettersPersonalAsync, cron);
                    }
                }
                else
                {
                    _workContext.RegisterSendMethod(SendSaasTariffLettersAsync, cron);
                }
            }
        }

        if (!_coreBaseSettings.Personal)
        {
            _workContext.RegisterSendMethod(SendMsgWhatsNewAsync, "0 0 * ? * *"); // every hour
            _workContext.RegisterSendMethod(SendRoomsActivityAsync, "0 0 * ? * *"); //every hour
        }
    }

    public async Task SendSaasTariffLettersAsync(DateTime scheduleDate)
    {
        using var scope = _serviceProvider.CreateScope();
        await scope.ServiceProvider.GetService<StudioPeriodicNotify>().SendSaasLettersAsync(EMailSenderName, scheduleDate);
    }

    public async Task SendEnterpriseTariffLettersAsync(DateTime scheduleDate)
    {
        using var scope = _serviceProvider.CreateScope();
        await scope.ServiceProvider.GetService<StudioPeriodicNotify>().SendEnterpriseLettersAsync(EMailSenderName, scheduleDate);
    }

    public async Task SendOpensourceTariffLettersAsync(DateTime scheduleDate)
    {
        using var scope = _serviceProvider.CreateScope();
        await scope.ServiceProvider.GetService<StudioPeriodicNotify>().SendOpensourceLettersAsync(EMailSenderName, scheduleDate);
    }

    public async Task SendLettersPersonalAsync(DateTime scheduleDate)
    {
        using var scope = _serviceProvider.CreateScope();
        await scope.ServiceProvider.GetService<StudioPeriodicNotify>().SendPersonalLettersAsync(EMailSenderName, scheduleDate);
    }

    public async Task SendMsgWhatsNewAsync(DateTime scheduleDate)
    {
        using var scope = _serviceProvider.CreateScope();
        await scope.ServiceProvider.GetRequiredService<StudioWhatsNewNotify>().SendMsgWhatsNewAsync(scheduleDate, WhatsNewType.DailyFeed);
    }

    public async Task SendRoomsActivityAsync(DateTime scheduleDate)
    {
        using var scope = _serviceProvider.CreateScope();
        await scope.ServiceProvider.GetRequiredService<StudioWhatsNewNotify>().SendMsgWhatsNewAsync(scheduleDate, WhatsNewType.RoomsActivity);
    }
}

[Scope]
public class StudioNotifyWorker
{
    private readonly NotifyEngineQueue _notifyEngineQueue;
    private readonly WorkContext _workContext;
    private readonly TenantManager _tenantManager;
    private readonly UserManager _userManager;
    private readonly SecurityContext _securityContext;
    private readonly StudioNotifyHelper _studioNotifyHelper;
    private readonly CommonLinkUtility _commonLinkUtility;

    public StudioNotifyWorker(
        TenantManager tenantManager,
        UserManager userManager,
        SecurityContext securityContext,
        StudioNotifyHelper studioNotifyHelper,
        CommonLinkUtility baseCommonLinkUtility,
        NotifyEngineQueue notifyEngineQueue,
        WorkContext workContext)
    {
        _tenantManager = tenantManager;
        _userManager = userManager;
        _securityContext = securityContext;
        _studioNotifyHelper = studioNotifyHelper;
        _commonLinkUtility = baseCommonLinkUtility;
        _notifyEngineQueue = notifyEngineQueue;
        _workContext = workContext;
    }

    public async Task OnMessageAsync(NotifyItem item)
    {
        _commonLinkUtility.ServerUri = item.BaseUrl;
        await _tenantManager.SetCurrentTenantAsync(item.TenantId);

        CultureInfo culture = null;

        var client = _workContext.NotifyContext.RegisterClient(_notifyEngineQueue, _studioNotifyHelper.NotifySource);

        var tenant = await _tenantManager.GetCurrentTenantAsync(false);

        if (tenant != null)
        {
            culture = tenant.GetCulture();
        }

        if (Guid.TryParse(item.UserId, out var userId) && !userId.Equals(Constants.Guest.ID) && !userId.Equals(Guid.Empty))
        {
            await _securityContext.AuthenticateMeWithoutCookieAsync(Guid.Parse(item.UserId));
            var user = await _userManager.GetUsersAsync(userId);
            if (!string.IsNullOrEmpty(user.CultureName))
            {
                culture = CultureInfo.GetCultureInfo(user.CultureName);
            }
        }

        if (culture != null && !Equals(CultureInfo.CurrentCulture, culture))
        {
            CultureInfo.CurrentCulture = culture;
        }
        if (culture != null && !Equals(CultureInfo.CurrentUICulture, culture))
        {
            CultureInfo.CurrentUICulture = culture;
        }

        await client.SendNoticeToAsync(
            (NotifyAction)item.Action,
            item.ObjectId,
            item.Recipients?.Select(r => r.IsGroup ? new RecipientsGroup(r.Id, r.Name) : (IRecipient)new DirectRecipient(r.Id, r.Name, r.Addresses.ToArray(), r.CheckActivation)).ToArray(),
            item.SenderNames.Count > 0 ? item.SenderNames.ToArray() : null,
            item.CheckSubsciption,
            item.Tags.Select(r => new TagValue(r.Tag_, r.Value)).ToArray());
    }
}

public static class ServiceLauncherExtension
{
    public static void Register(DIHelper services)
    {
        services.TryAdd<StudioNotifyWorker>();
        services.TryAdd<StudioPeriodicNotify>();
        services.TryAdd<StudioWhatsNewNotify>();
    }
}
