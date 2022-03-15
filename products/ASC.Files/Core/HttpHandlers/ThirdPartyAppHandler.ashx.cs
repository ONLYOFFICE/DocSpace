namespace ASC.Web.Files.HttpHandlers
{
    public class ThirdPartyAppHandler
    {
        private RequestDelegate Next { get; }
        private IServiceProvider ServiceProvider { get; }

        public ThirdPartyAppHandler(RequestDelegate next, IServiceProvider serviceProvider)
        {
            Next = next;
            ServiceProvider = serviceProvider;
        }

        public async Task Invoke(HttpContext context)
        {
            using var scope = ServiceProvider.CreateScope();
            var thirdPartyAppHandlerService = scope.ServiceProvider.GetService<ThirdPartyAppHandlerService>();
            await thirdPartyAppHandlerService.InvokeAsync(context);
            await Next.Invoke(context);
        }
    }

    [Scope]
    public class ThirdPartyAppHandlerService
    {
        private AuthContext AuthContext { get; }
        private CommonLinkUtility CommonLinkUtility { get; }
        private ILog Log { get; set; }

        public string HandlerPath { get; set; }

        public ThirdPartyAppHandlerService(
            IOptionsMonitor<ILog> optionsMonitor,
            AuthContext authContext,
            BaseCommonLinkUtility baseCommonLinkUtility,
            CommonLinkUtility commonLinkUtility)
        {
            AuthContext = authContext;
            CommonLinkUtility = commonLinkUtility;
            Log = optionsMonitor.CurrentValue;
            HandlerPath = baseCommonLinkUtility.ToAbsolute("~/thirdpartyapp");
        }

        public async Task InvokeAsync(HttpContext context)
        {
            Log.Debug("ThirdPartyApp: handler request - " + context.Request.Url());

            var message = string.Empty;

            try
            {
                var app = ThirdPartySelector.GetApp(context.Request.Query[ThirdPartySelector.AppAttr]);
                Log.Debug("ThirdPartyApp: app - " + app);

                if (await app.RequestAsync(context))
                {
                    return;
                }
            }
            catch (ThreadAbortException)
            {
                //Thats is responce ending
                return;
            }
            catch (Exception e)
            {
                Log.Error("ThirdPartyApp", e);
                message = e.Message;
            }

            if (string.IsNullOrEmpty(message))
            {
                if ((context.Request.Query["error"].FirstOrDefault() ?? "").Equals("access_denied", StringComparison.InvariantCultureIgnoreCase))
                {
                    message = context.Request.Query["error_description"].FirstOrDefault() ?? FilesCommonResource.AppAccessDenied;
                }
            }

            var redirectUrl = CommonLinkUtility.GetDefault();
            if (!string.IsNullOrEmpty(message))
            {
                redirectUrl += AuthContext.IsAuthenticated ? "#error/" : "?m=";
                redirectUrl += HttpUtility.UrlEncode(message);
            }
            context.Response.Redirect(redirectUrl, true);
        }
    }

    public static class ThirdPartyAppHandlerExtention
    {
        public static IApplicationBuilder UseThirdPartyAppHandler(this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<ThirdPartyAppHandler>();
        }
    }
}