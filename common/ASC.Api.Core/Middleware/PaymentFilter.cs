namespace ASC.Api.Core.Middleware;

[Scope]
public class PaymentFilter : IResourceFilter
{
    private readonly TenantExtra _tenantExtra;
    private readonly ILog _logger;

    public PaymentFilter(IOptionsMonitor<ILog> options, TenantExtra tenantExtra)
    {
        _logger = options.CurrentValue;
        _tenantExtra = tenantExtra;
    }

    public void OnResourceExecuted(ResourceExecutedContext context) { }

    public void OnResourceExecuting(ResourceExecutingContext context)
    {
        if (context.ActionDescriptor is ControllerActionDescriptor controllerActionDescriptor
            && !controllerActionDescriptor.EndpointMetadata.OfType<CustomHttpMethodAttribute>().FirstOrDefault().Check)
        {
            _logger.Debug("Payment is not required");

            return;
        }

        var header = context.HttpContext.Request.Headers["Payment-Info"];
        if (string.IsNullOrEmpty(header) || (bool.TryParse(header, out var flag) && flag))
        {
            if (_tenantExtra.IsNotPaid())
            {
                context.Result = new StatusCodeResult((int)HttpStatusCode.PaymentRequired);
                _logger.WarnFormat("Payment Required {0}.", context.HttpContext.Request.Url());
            }
        }
    }
}