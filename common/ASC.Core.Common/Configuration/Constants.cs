namespace ASC.Core.Configuration;

public static class Constants
{
    public static readonly string NotifyEMailSenderSysName = "email.sender";
    public static readonly string NotifyMessengerSenderSysName = "messanger.sender";
    public static readonly string NotifyPushSenderSysName = "push.sender";
    public static readonly string NotifyTelegramSenderSysName = "telegram.sender";
    public static readonly ISystemAccount CoreSystem = new SystemAccount(new Guid("A37EE56E-3302-4a7b-B67E-DDBEA64CD032"), "asc system", true);
    public static readonly ISystemAccount Guest = new SystemAccount(new Guid("712D9EC3-5D2B-4b13-824F-71F00191DCCA"), "guest", false);
    public static readonly IPrincipal Anonymous = new GenericPrincipal(Guest, new[] { Role.Everyone });
    public static readonly ISystemAccount[] SystemAccounts = new[] { CoreSystem, Guest };
    public static readonly int DefaultTrialPeriod = 30;
}
