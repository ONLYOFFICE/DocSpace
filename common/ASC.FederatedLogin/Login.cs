// (c) Copyright Ascensio System SIA 2010-2022
//
// This program is a free software product.
// You can redistribute it and/or modify it under the terms
// of the GNU Affero General Public License (AGPL) version 3 as published by the Free Software
// Foundation. In accordance with Section 7(a) of the GNU AGPL its Section 15 shall be amended
// to the effect that Ascensio System SIA expressly excludes the warranty of non-infringement of
// any third-party rights.
//
// This program is distributed WITHOUT ANY WARRANTY, without even the implied warranty
// of MERCHANTABILITY or FITNESS FOR A PARTICULAR  PURPOSE. For details, see
// the GNU AGPL at: http://www.gnu.org/licenses/agpl-3.0.html
//
// You can contact Ascensio System SIA at Lubanas st. 125a-25, Riga, Latvia, EU, LV-1021.
//
// The  interactive user interfaces in modified source and object code versions of the Program must
// display Appropriate Legal Notices, as required under Section 5 of the GNU AGPL version 3.
//
// Pursuant to Section 7(b) of the License you must retain the original Product logo when
// distributing the program. Pursuant to Section 7(e) we decline to grant you any rights under
// trademark law for use of our trademarks.
//
// All the Product's GUI elements, including illustrations and icon sets, as well as technical writing
// content are licensed under the terms of the Creative Commons Attribution-ShareAlike 4.0
// International. See the License terms at http://creativecommons.org/licenses/by-sa/4.0/legalcode

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
    private readonly Signature _signature;
    private readonly InstanceCrypto _instanceCrypto;
    private readonly ProviderManager _providerManager;

    public Login(
        IWebHostEnvironment webHostEnvironment,
        Signature signature,
        InstanceCrypto instanceCrypto,
        ProviderManager providerManager)
    {
        _webHostEnvironment = webHostEnvironment;
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
            var uriBuilder = new UriBuilder(context.Request.Url());
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
        var xrdsloginuri = new Uri(context.Request.Url(), new Uri(context.Request.Url().AbsolutePath, UriKind.Relative)) + "?auth=openid&returnurl=" + ReturnUrl;
        var xrdsimageuri = new Uri(context.Request.Url(), new Uri(_webHostEnvironment.WebRootPath, UriKind.Relative)) + "openid.gif";
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
    public LoginHandler(RequestDelegate next)
    {
    }

    public async Task InvokeAsync(HttpContext context, Login login)
    {
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
