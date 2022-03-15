namespace ASC.Core.Configuration;

public class AmiPublicDnsSyncService : IServiceController
{
    public static IServiceProvider ServiceProvider { get; set; }

    public void Start()
    {
        Synchronize();
    }

    public void Stop() { }

    public static void Synchronize()
    {
        using var scope = ServiceProvider.CreateScope();
        var scopeClass = scope.ServiceProvider.GetService<AmiPublicDnsSyncServiceScope>();
        var (tenantManager, coreBaseSettings, clientFactory) = scopeClass;
        if (coreBaseSettings.Standalone)
        {
            var tenants = tenantManager.GetTenants(false).Where(t => MappedDomainNotSettedByUser(t.MappedDomain));
            if (tenants.Any())
            {
                var dnsname = GetAmiPublicDnsName(clientFactory);
                foreach (var tenant in tenants.Where(t => !string.IsNullOrEmpty(dnsname) && t.MappedDomain != dnsname))
                {
                    tenant.MappedDomain = dnsname;
                    tenantManager.SaveTenant(tenant);
                }
            }
        }
    }

    private static bool MappedDomainNotSettedByUser(string domain)
    {
        return string.IsNullOrEmpty(domain) || Regex.IsMatch(domain, "^ec2.+\\.compute\\.amazonaws\\.com$", RegexOptions.CultureInvariant | RegexOptions.IgnoreCase);
    }

    private static string GetAmiPublicDnsName(IHttpClientFactory clientFactory)
    {
        try
        {
            var request = new HttpRequestMessage();
            request.RequestUri = new Uri("http://169.254.169.254/latest/meta-data/public-hostname");
            request.Method = HttpMethod.Get;

            var httpClient = clientFactory.CreateClient();
            httpClient.Timeout = TimeSpan.FromMilliseconds(5000);

            using var responce = httpClient.Send(request);
            using var stream = responce.Content.ReadAsStream();
            using var reader = new StreamReader(stream);

            return reader.ReadToEnd();
        }
        catch (HttpRequestException ex)
        {
            if (ex.StatusCode == HttpStatusCode.NotFound || ex.StatusCode == HttpStatusCode.Conflict)
            {
                throw;
            }
        }

        return null;
    }
}

public class AmiPublicDnsSyncServiceScope
{
    private TenantManager _tenantManager;
    private CoreBaseSettings _coreBaseSettings;
    private IHttpClientFactory _clientFactory;

    public AmiPublicDnsSyncServiceScope(TenantManager tenantManager, CoreBaseSettings coreBaseSettings, IHttpClientFactory clientFactory)
    {
        _tenantManager = tenantManager;
        _coreBaseSettings = coreBaseSettings;
        _clientFactory = clientFactory;
    }

    public void Deconstruct(out TenantManager tenantManager, out CoreBaseSettings coreBaseSettings, out IHttpClientFactory clientFactory)
    {
        (tenantManager, coreBaseSettings, clientFactory) = (_tenantManager, _coreBaseSettings, _clientFactory);
    }
}
