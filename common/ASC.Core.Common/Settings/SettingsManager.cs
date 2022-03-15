namespace ASC.Core.Common.Settings;

[Scope]
public class SettingsManager : DbSettingsManager
{
    public SettingsManager(
        IServiceProvider serviceProvider,
        DbSettingsManagerCache dbSettingsManagerCache,
        IOptionsMonitor<ILog> option,
        AuthContext authContext,
        TenantManager tenantManager,
        DbContextManager<WebstudioDbContext> dbContextManager)
        : base(serviceProvider, dbSettingsManagerCache, option, authContext, tenantManager, dbContextManager)
    {

    }
}
