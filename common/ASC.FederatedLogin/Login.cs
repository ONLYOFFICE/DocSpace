using JsonSerializer = System.Text.Json.JsonSerializer;

namespace ASC.FederatedLogin;

[Scope]
public class Login
{
    public bool IsReusable => false;
    protected string Callback => _params.Get("callback") ?? "loginCallback";
    protected string Auth => _params.Get("auth");
    protected string ReturnUrl => _params.Get("returnurl"); //TODO?? FormsAuthentication.LoginUrl;

    protected LoginMode Mode
    {
        get
        {
            if (!string.IsNullOrEmpty(_params.Get("mode")))
            {
                return (LoginMode)Enum.Parse(typeof(LoginMode), _params.Get("mode"), true);
            }

            return LoginMode.Popup;
        }
    }

    protected bool Minimal
    {
        get
        {
            if (_params.ContainsKey("min"))
            {
                bool.TryParse(_params.Get("min"), out var result);

                return result;
            }

            return false;
        }
    }

    private Dictionary<string, string> _params;
    private readonly IWebHostEnvironment _webHostEnvironment;
    private readonly IMemoryCache _memoryCache;
    private readonly Signature _signature;
    private readonly InstanceCrypto _instanceCrypto;
    private readonly ProviderManager _providerManager;

    public Login(
        IWebHostEnvironment webHostEnvironment,
        IMemoryCache memoryCache,
        Signature signature,
        InstanceCrypto instanceCrypto,
        ProviderManager providerManager)
    {
        _webHostEnvironment = webHostEnvironment;
        _memoryCache = memoryCache;
        _signature = signature;
        _instanceCrypto = instanceCrypto;
        _providerManager = providerManager;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        _ = context.PushRewritenUri();

        if (string.IsNullOrEmpty(context.Request.Query["p"]))
        {
            _params = new Dictionary<string, string>(context.Request.Query.Count);
            //Form params and redirect
            foreach (var key in context.Request.Query.Keys)
            {
                _params.Add(key, context.Request.Query[key]);
            }

            //Pack and redirect
            var uriBuilder = new UriBuilder(context.Request.GetUrlRewriter());
            var token = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(JsonSerializer.Serialize(_params)));
            uriBuilder.Query = "p=" + token;
            context.Response.Redirect(uriBuilder.Uri.ToString(), true);
            await context.Response.CompleteAsync();

            return;
        }
        else
        {
            _params = JsonSerializer.Deserialize<Dictionary<string, string>>(Encoding.UTF8.GetString(WebEncoders.Base64UrlDecode(context.Request.Query["p"])));
        }

        if (!string.IsNullOrEmpty(Auth))
        {
            try
            {
                var desktop = _params.ContainsKey("desktop") && _params["desktop"] == "true";
                IDictionary<string, string> additionalStateArgs = null;

                if (desktop)
                {
                    additionalStateArgs = context.Request.Query.ToDictionary(r => r.Key, r => r.Value.FirstOrDefault());
                    if (!additionalStateArgs.ContainsKey("desktop"))
                    {
                        additionalStateArgs.Add("desktop", "true");
                    }
                }

                var profile = _providerManager.Process(Auth, context, null, additionalStateArgs);
                if (profile != null)
                {
                    await SendJsCallbackAsync(context, profile);
                }
            }
            catch (ThreadAbortException)
            {
                //Thats is responce ending
            }
            catch (Exception ex)
            {
                await SendJsCallbackAsync(context, LoginProfile.FromError(_signature, _instanceCrypto, ex));
            }
        }
        else
        {
            //Render xrds
            await RenderXrdsAsync(context);
        }
        context.PopRewritenUri();
    }

    private async Task RenderXrdsAsync(HttpContext context)
    {
        var xrdsloginuri = new Uri(context.Request.GetUrlRewriter(), new Uri(context.Request.GetUrlRewriter().AbsolutePath, UriKind.Relative)) + "?auth=openid&returnurl=" + ReturnUrl;
        var xrdsimageuri = new Uri(context.Request.GetUrlRewriter(), new Uri(_webHostEnvironment.WebRootPath, UriKind.Relative)) + "openid.gif";
        await XrdsHelper.RenderXrdsAsync(context.Response, xrdsloginuri, xrdsimageuri);
    }

    private async Task SendJsCallbackAsync(HttpContext context, LoginProfile profile)
    {
        //Render a page
        context.Response.ContentType = "text/html";
        await context.Response.WriteAsync(
            JsCallbackHelper.GetCallbackPage()
            .Replace("%PROFILE%", $"\"{profile.Serialized}\"")
            .Replace("%CALLBACK%", Callback)
            .Replace("%DESKTOP%", (Mode == LoginMode.Redirect).ToString().ToLowerInvariant())
            );
    }
}

public class LoginHandler
{
    private readonly RequestDelegate _next;
    private readonly IServiceScopeFactory _serviceScopeFactory;

    public LoginHandler(RequestDelegate next, IServiceScopeFactory serviceScopeFactory)
    {
        _next = next;
        _serviceScopeFactory = serviceScopeFactory;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        using var scope = _serviceScopeFactory.CreateScope();
        var login = scope.ServiceProvider.GetService<Login>();

        await login.InvokeAsync(context);
    }
}

public static class LoginHandlerExtensions
{
    public static IApplicationBuilder UseLoginHandler(this IApplicationBuilder builder)
    {
        return builder.UseMiddleware<LoginHandler>();
    }
}
