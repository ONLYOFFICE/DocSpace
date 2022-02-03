namespace ASC.Api.Core.Middleware
{
    [Scope]
    public class IpSecurityFilter : IResourceFilter
    {
        private readonly AuthContext _authContext;
        private readonly IPSecurity.IPSecurity _iPSecurity;
        private readonly ILog _logger;

        public IpSecurityFilter(
            IOptionsMonitor<ILog> options,
            AuthContext authContext,
            IPSecurity.IPSecurity IPSecurity)
        {
            _logger = options.CurrentValue;
            _authContext = authContext;
            _iPSecurity = IPSecurity;
        }

        public void OnResourceExecuted(ResourceExecutedContext context) { }

        public void OnResourceExecuting(ResourceExecutingContext context)
        {
            if (_authContext.IsAuthenticated && !_iPSecurity.Verify())
            {
                context.Result = new StatusCodeResult((int)HttpStatusCode.Forbidden);
                _logger.WarnFormat("IPSecurity: user {0}", _authContext.CurrentAccount.ID);

                return;
            }
        }
    }
}