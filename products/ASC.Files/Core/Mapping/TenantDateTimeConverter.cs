namespace ASC.Files.Core.Mapping;

[Scope]
public class TenantDateTimeConverter : IValueConverter<DateTime, DateTime>
{
    private readonly TenantUtil _tenantUtil;

    public TenantDateTimeConverter(TenantUtil tenantUtil)
    {
        _tenantUtil = tenantUtil;
    }

    public DateTime Convert(DateTime sourceMember, ResolutionContext context)
    {
        return _tenantUtil.DateTimeFromUtc(sourceMember);
    }
}
