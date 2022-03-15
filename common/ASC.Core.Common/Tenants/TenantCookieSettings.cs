namespace ASC.Core.Tenants;

[Serializable]
public class TenantCookieSettings : ISettings
{
    public int Index { get; set; }
    public int LifeTime { get; set; }

    public ISettings GetDefault(IServiceProvider serviceProvider)
    {
        return GetInstance();
    }

    public bool IsDefault()
    {
        var defaultSettings = GetInstance();

        return LifeTime == defaultSettings.LifeTime;
    }

    public static TenantCookieSettings GetInstance()
    {
        return new TenantCookieSettings();
    }

    public Guid ID => new Guid("{16FB8E67-E96D-4B22-B217-C80F25C5DE1B}");
}

[Scope]
public class TenantCookieSettingsHelper
{
    public bool IsVisibleSettings { get; internal set; }
    private readonly SettingsManager _settingsManager;

    public TenantCookieSettingsHelper(IConfiguration configuration, SettingsManager settingsManager)
    {
        IsVisibleSettings = !(configuration["web:hide-settings"] ?? string.Empty)
        .Split(new[] { ',', ';', ' ' }, StringSplitOptions.RemoveEmptyEntries)
        .Contains("CookieSettings", StringComparer.CurrentCultureIgnoreCase);

        _settingsManager = settingsManager;
    }


    public TenantCookieSettings GetForTenant(int tenantId)
    {
        return IsVisibleSettings
                   ? _settingsManager.LoadForTenant<TenantCookieSettings>(tenantId)
                   : TenantCookieSettings.GetInstance();
    }

    public void SetForTenant(int tenantId, TenantCookieSettings settings = null)
    {
        if (!IsVisibleSettings)
        {
            return;
        }

        _settingsManager.SaveForTenant(settings ?? TenantCookieSettings.GetInstance(), tenantId);
    }

    public TenantCookieSettings GetForUser(Guid userId)
    {
        return IsVisibleSettings
                   ? _settingsManager.LoadForUser<TenantCookieSettings>(userId)
                   : TenantCookieSettings.GetInstance();
    }

    public TenantCookieSettings GetForUser(int tenantId, Guid userId)
    {
        return IsVisibleSettings
                   ? _settingsManager.LoadSettingsFor<TenantCookieSettings>(tenantId, userId)
                   : TenantCookieSettings.GetInstance();
    }

    public void SetForUser(Guid userId, TenantCookieSettings settings = null)
    {
        if (!IsVisibleSettings)
        {
            return;
        }

        _settingsManager.SaveForUser(settings ?? TenantCookieSettings.GetInstance(), userId);
    }

    public DateTime GetExpiresTime(int tenantId)
    {
        var settingsTenant = GetForTenant(tenantId);
        var expires = settingsTenant.IsDefault() ? DateTime.UtcNow.AddYears(1) : DateTime.UtcNow.AddMinutes(settingsTenant.LifeTime);

        return expires;
    }
}
