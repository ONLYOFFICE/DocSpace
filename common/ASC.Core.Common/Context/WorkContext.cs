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


using Constants = ASC.Core.Configuration.Constants;
using NotifyContext = ASC.Notify.Context;

namespace ASC.Core;

public static class WorkContext
{
    private static readonly object _syncRoot = new object();
    private static bool _notifyStarted;
    private static bool? _isMono;
    private static string _monoVersion;


    public static NotifyContext NotifyContext { get; private set; }

    public static string[] DefaultClientSenders => new[] { Constants.NotifyEMailSenderSysName, };

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


    public static void NotifyStartUp(IServiceProvider serviceProvider)
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

            var configuration = serviceProvider.GetService<IConfiguration>();
            var cacheNotify = serviceProvider.GetService<ICacheNotify<NotifyMessage>>();
            var cacheInvoke = serviceProvider.GetService<ICacheNotify<NotifyInvoke>>();
            var options = serviceProvider.GetService<IOptionsMonitor<ILog>>();

            NotifyContext = new NotifyContext(serviceProvider);

            INotifySender jabberSender = new NotifyServiceSender(cacheNotify, cacheInvoke);
            INotifySender emailSender = new NotifyServiceSender(cacheNotify, cacheInvoke);
            INotifySender telegramSender = new TelegramSender(options, serviceProvider);

            var postman = configuration["core:notify:postman"];

            if ("ases".Equals(postman, StringComparison.InvariantCultureIgnoreCase) || "smtp".Equals(postman, StringComparison.InvariantCultureIgnoreCase))
            {
                jabberSender = new JabberSender(serviceProvider);

                var properties = new Dictionary<string, string>
                {
                    ["useCoreSettings"] = "true"
                };
                if ("ases".Equals(postman, StringComparison.InvariantCultureIgnoreCase))
                {
                    emailSender = new AWSSender(serviceProvider, options);
                    properties["accessKey"] = configuration["ses:accessKey"];
                    properties["secretKey"] = configuration["ses:secretKey"];
                    properties["refreshTimeout"] = configuration["ses:refreshTimeout"];
                }
                else
                {
                    emailSender = new SmtpSender(serviceProvider, options);
                }

                emailSender.Init(properties);
            }

            NotifyContext.NotifyService.RegisterSender(Constants.NotifyEMailSenderSysName, new EmailSenderSink(emailSender, serviceProvider, options));
            NotifyContext.NotifyService.RegisterSender(Constants.NotifyMessengerSenderSysName, new JabberSenderSink(jabberSender, serviceProvider));
            NotifyContext.NotifyService.RegisterSender(Constants.NotifyTelegramSenderSysName, new TelegramSenderSink(telegramSender, serviceProvider));

            NotifyContext.NotifyEngine.BeforeTransferRequest += NotifyEngine_BeforeTransferRequest;
            NotifyContext.NotifyEngine.AfterTransferRequest += NotifyEngine_AfterTransferRequest;
            _notifyStarted = true;
        }
    }

    public static void RegisterSendMethod(Action<DateTime> method, string cron)
    {
        NotifyContext.NotifyEngine.RegisterSendMethod(method, cron);
    }

    public static void UnregisterSendMethod(Action<DateTime> method)
    {
        NotifyContext.NotifyEngine.UnregisterSendMethod(method);

    }
    private static void NotifyEngine_BeforeTransferRequest(NotifyEngine sender, NotifyRequest request, IServiceScope serviceScope)
    {
        request.Properties.Add("Tenant", serviceScope.ServiceProvider.GetService<TenantManager>().GetCurrentTenant(false));
    }

    private static void NotifyEngine_AfterTransferRequest(NotifyEngine sender, NotifyRequest request, IServiceScope scope)
    {
        if ((request.Properties.Contains("Tenant") ? request.Properties["Tenant"] : null) is Tenant tenant)
        {
            var tenantManager = scope.ServiceProvider.GetService<TenantManager>();
            tenantManager.SetCurrentTenant(tenant);
        }
    }
}

public static class WorkContextExtension
{
    public static void Register(DIHelper dIHelper)
    {
        dIHelper.TryAdd<TelegramHelper>();
        dIHelper.TryAdd<EmailSenderSinkScope>();
        dIHelper.TryAdd<JabberSenderSinkScope>();
    }
}
