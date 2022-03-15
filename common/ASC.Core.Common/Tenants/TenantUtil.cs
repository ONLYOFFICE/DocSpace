namespace ASC.Core.Tenants;

[Scope]
class ConfigureTenantUtil : IConfigureNamedOptions<TenantUtil>
{
    private readonly IOptionsSnapshot<TenantManager> _tenantManager;
    private readonly TimeZoneConverter _timeZoneConverter;

    public ConfigureTenantUtil(
        IOptionsSnapshot<TenantManager> tenantManager,
        TimeZoneConverter timeZoneConverter
        )
    {
        _tenantManager = tenantManager;
        _timeZoneConverter = timeZoneConverter;
    }

    public void Configure(string name, TenantUtil options)
    {
        Configure(options);
        options.TenantManager = _tenantManager.Get(name);
    }

    public void Configure(TenantUtil options)
    {
        options.TimeZoneConverter = _timeZoneConverter;
        options.TenantManager = _tenantManager.Value;
    }
}

[Scope(typeof(ConfigureTenantUtil))]
public class TenantUtil
{
    internal TenantManager TenantManager;
    internal TimeZoneConverter TimeZoneConverter;

    public TenantUtil() { }

    public TenantUtil(TenantManager tenantManager, TimeZoneConverter timeZoneConverter)
    {
        TenantManager = tenantManager;
        TimeZoneConverter = timeZoneConverter;
    }

    private TimeZoneInfo _timeZoneInfo;
    private TimeZoneInfo TimeZoneInfo => _timeZoneInfo ??= TimeZoneConverter.GetTimeZone(TenantManager.GetCurrentTenant().TimeZone);

    public DateTime DateTimeFromUtc(DateTime utc)
    {
        return DateTimeFromUtc(TimeZoneInfo, utc);
    }

    public DateTime DateTimeFromUtc(string timeZone, DateTime utc)
    {
        return DateTimeFromUtc(TimeZoneConverter.GetTimeZone(timeZone), utc);
    }

    public static DateTime DateTimeFromUtc(TimeZoneInfo timeZone, DateTime utc)
    {
        if (utc.Kind != DateTimeKind.Utc)
        {
            utc = DateTime.SpecifyKind(utc, DateTimeKind.Utc);
        }

        if (utc == DateTime.MinValue || utc == DateTime.MaxValue)
        {
            return utc;
        }

        return DateTime.SpecifyKind(TimeZoneInfo.ConvertTime(utc, TimeZoneInfo.Utc, timeZone), DateTimeKind.Local);
    }


    public DateTime DateTimeToUtc(DateTime local)
    {
        return DateTimeToUtc(TimeZoneInfo, local);
    }

    public static DateTime DateTimeToUtc(TimeZoneInfo timeZone, DateTime local)
    {
        if (local.Kind == DateTimeKind.Utc || local == DateTime.MinValue || local == DateTime.MaxValue)
        {
            return local;
        }

        if (timeZone.IsInvalidTime(DateTime.SpecifyKind(local, DateTimeKind.Unspecified)))
        {
            // hack
            local = local.AddHours(1);
        }

        return TimeZoneInfo.ConvertTimeToUtc(DateTime.SpecifyKind(local, DateTimeKind.Unspecified), timeZone);

    }

    public DateTime DateTimeNow()
    {
        return DateTimeNow(TimeZoneInfo);
    }

    public static DateTime DateTimeNow(TimeZoneInfo timeZone)
    {
        return DateTime.SpecifyKind(TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, timeZone), DateTimeKind.Local);
    }
    public DateTime DateTimeNow(string timeZone)
    {
        return DateTimeNow(TimeZoneConverter.GetTimeZone(timeZone));
    }
}
