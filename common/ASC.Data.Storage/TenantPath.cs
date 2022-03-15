namespace ASC.Data.Storage;

public static class TenantPath
{
    public static string CreatePath(string tenant)
    {
        ArgumentNullException.ThrowIfNull(tenant);

        if (long.TryParse(tenant, NumberStyles.Integer, CultureInfo.InvariantCulture, out var tenantId))
        {
            var culture = CultureInfo.InvariantCulture;

            return tenantId == 0 ? tenantId.ToString(culture) : tenantId.ToString("00/00/00", culture);
        }

        return tenant;
    }

    public static bool TryGetTenant(string tenantPath, out int tenant)
    {
        tenantPath = tenantPath.Replace("/", "");

        return int.TryParse(tenantPath, out tenant);
    }
}
