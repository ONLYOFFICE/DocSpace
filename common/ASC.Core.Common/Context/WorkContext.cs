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
using NotifyContext = ASC.Notify.Context;

namespace ASC.Core;

[Singletone]
public class WorkContext
{
    private static readonly object _syncRoot = new object();
    private readonly IServiceProvider _serviceProvider;
    private readonly IConfiguration _configuration;
    private readonly DispatchEngine _dispatchEngine;
    private readonly JabberSender _jabberSender;
    private readonly AWSSender _awsSender;
    private readonly SmtpSender _smtpSender;
    private readonly NotifyServiceSender _notifyServiceSender;
    private readonly TelegramSender _telegramSender;
    private readonly PushSender _pushSender;
    private static bool _notifyStarted;
    private static bool? _isMono;
    private static string _monoVersion;


    public NotifyContext NotifyContext { get; private set; }
    public NotifyEngine NotifyEngine { get; private set; }

    public static string[] DefaultClientSenders => new[] { Constants.NotifyEMailSenderSysName };

    public static bool IsMono
    {
        get
        {
            if (_isMono.HasValue)
            {
                return _isMono.Value;
            }

            var monoRuntime = Type.GetType("Mono.Runtime");
            _isMono = monoRuntime != null;
            if (monoRuntime != null)
            {
                var dispalayName = monoRuntime.GetMethod("GetDisplayName", BindingFlags.NonPublic | BindingFlags.Static);
                if (dispalayName != null)
                {
                    _monoVersion = dispalayName.Invoke(null, null) as string;
                }
            }

            return _isMono.Value;
        }
    }

    public static string MonoVersion => IsMono ? _monoVersion : null;

    public WorkContext(
        IServiceProvider serviceProvider,
        IConfiguration configuration,
        DispatchEngine dispatchEngine,
        NotifyEngine notifyEngine,
        NotifyContext notifyContext,
        JabberSender jabberSender,
        AWSSender awsSender,
        SmtpSender smtpSender,
        NotifyServiceSender notifyServiceSender,
        TelegramSender telegramSender,
        PushSender pushSender
        )
    {
        _serviceProvider = serviceProvider;
        _configuration = configuration;
        _dispatchEngine = dispatchEngine;
        NotifyEngine = notifyEngine;
        NotifyContext = notifyContext;
        _jabberSender = jabberSender;
        _awsSender = awsSender;
        _smtpSender = smtpSender;
        _notifyServiceSender = notifyServiceSender;
        _telegramSender = telegramSender;
        _pushSender = pushSender;
    }

    public void NotifyStartUp()
    {
        if (_notifyStarted)
        {
            return;
        }

        lock (_syncRoot)
        {
            if (_notifyStarted)
            {
                return;
            }

            INotifySender jabberSender = _notifyServiceSender;
            INotifySender emailSender = _notifyServiceSender;
            INotifySender telegramSender = _telegramSender;
            INotifySender pushSender = _pushSender;


            var postman = _configuration["core:notify:postman"];

            if ("ases".Equals(postman, StringComparison.InvariantCultureIgnoreCase) || "smtp".Equals(postman, StringComparison.InvariantCultureIgnoreCase))
            {
                jabberSender = _jabberSender;

                var properties = new Dictionary<string, string>
                {
                    ["useCoreSettings"] = "true"
                };
                if ("ases".Equals(postman, StringComparison.InvariantCultureIgnoreCase))
                {
                    emailSender = _awsSender;
                    properties["accessKey"] = _configuration["ses:accessKey"];
                    properties["secretKey"] = _configuration["ses:secretKey"];
                    properties["refreshTimeout"] = _configuration["ses:refreshTimeout"];
                }
                else
                {
                    emailSender = _smtpSender;
                }

                emailSender.Init(properties);
            }

            NotifyContext.RegisterSender(_dispatchEngine, Constants.NotifyEMailSenderSysName, new EmailSenderSink(emailSender, _serviceProvider));
            NotifyContext.RegisterSender(_dispatchEngine, Constants.NotifyMessengerSenderSysName, new JabberSenderSink(jabberSender, _serviceProvider));
            NotifyContext.RegisterSender(_dispatchEngine, Constants.NotifyTelegramSenderSysName, new TelegramSenderSink(telegramSender, _serviceProvider));
            NotifyContext.RegisterSender(_dispatchEngine, Constants.NotifyPushSenderSysName, new PushSenderSink(pushSender, _serviceProvider));

            NotifyEngine.AddAction<NotifyTransferRequest>();

            _notifyStarted = true;
        }
    }

    public void RegisterSendMethod(Func<DateTime, Task> method, string cron)
    {
        NotifyEngine.RegisterSendMethod(method, cron);
    }

    public void UnregisterSendMethod(Func<DateTime, Task> method)
    {
        NotifyEngine.UnregisterSendMethod(method);
    }
}

[Scope]
public class NotifyTransferRequest : INotifyEngineAction
{
    private readonly TenantManager _tenantManager;

    public NotifyTransferRequest(TenantManager tenantManager)
    {
        _tenantManager = tenantManager;
    }

    public void AfterTransferRequest(NotifyRequest request)
    {
        if ((request.Properties.Contains("Tenant") ? request.Properties["Tenant"] : null) is Tenant tenant)
        {
            _tenantManager.SetCurrentTenant(tenant);
        }
    }

    public async Task BeforeTransferRequestAsync(NotifyRequest request)
    {
        request.Properties.Add("Tenant", await _tenantManager.GetCurrentTenantAsync(false));
    }
}

public static class WorkContextExtension
{
    public static void Register(DIHelper dIHelper)
    {
        dIHelper.TryAdd<NotifyTransferRequest>();
        dIHelper.TryAdd<TelegramHelper>();
        dIHelper.TryAdd<TelegramSenderSinkMessageCreator>();
        dIHelper.TryAdd<JabberSenderSinkMessageCreator>();
        dIHelper.TryAdd<PushSenderSinkMessageCreator>();
        dIHelper.TryAdd<EmailSenderSinkMessageCreator>();
    }
}
