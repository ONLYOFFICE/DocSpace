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
