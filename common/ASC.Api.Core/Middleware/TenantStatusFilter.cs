namespace ASC.Api.Core.Middleware;

[Scope]
public class TenantStatusFilter : IResourceFilter
{
    private readonly TenantManager _tenantManager;
    private readonly ILog _logger;

    public TenantStatusFilter(IOptionsMonitor<ILog> options, TenantManager tenantManager)
    {
        _logger = options.CurrentValue;
        _tenantManager = tenantManager;
    }

    public void OnResourceExecuted(ResourceExecutedContext context) { }

    public void OnResourceExecuting(ResourceExecutingContext context)
    {
        var tenant = _tenantManager.GetCurrentTenant(false);
        if (tenant == null)
        {
            context.Result = new StatusCodeResult((int)HttpStatusCode.NotFound);
            _logger.Warn("Current tenant not found");

            return;
        }

        if (tenant.Status == TenantStatus.RemovePending || tenant.Status == TenantStatus.Suspended)
        {
            context.Result = new StatusCodeResult((int)HttpStatusCode.NotFound);
            _logger.WarnFormat("Tenant {0} is not removed or suspended", tenant.Id);

            return;
        }
    }
}