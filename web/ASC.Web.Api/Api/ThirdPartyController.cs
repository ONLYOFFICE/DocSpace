namespace ASC.Web.Api.Controllers;

[Scope(Additional = typeof(BaseLoginProviderExtension))]
[DefaultRoute]
[ApiController]
public class ThirdPartyController : ControllerBase
{
    private readonly OAuth20TokenHelper _oAuth20TokenHelper;

    public ThirdPartyController(OAuth20TokenHelper oAuth20TokenHelper)
    {
        _oAuth20TokenHelper = oAuth20TokenHelper;
    }

    [Read("{provider}")]
    public object Get(LoginProviderEnum provider)
    {
        var desktop = HttpContext.Request.Query["desktop"] == "true";
        var additionals = new Dictionary<string, string>();

        if (desktop)
        {
            additionals = HttpContext.Request.Query.ToDictionary(r => r.Key, r => r.Value.FirstOrDefault());
        }

        switch (provider)
        {
            case LoginProviderEnum.Google:
                return _oAuth20TokenHelper.RequestCode<GoogleLoginProvider>(
                                                                    GoogleLoginProvider.GoogleScopeDrive,
                                                                    new Dictionary<string, string>
                                                                        {
                                                                    { "access_type", "offline" },
                                                                    { "prompt", "consent" }
                                                                        }, additionalStateArgs: additionals);

            case LoginProviderEnum.Dropbox:
                return _oAuth20TokenHelper.RequestCode<DropboxLoginProvider>(
                                                    additionalArgs: new Dictionary<string, string>
                                                        {
                                                                        { "force_reauthentication", "true" }
                                                        }, additionalStateArgs: additionals);

            case LoginProviderEnum.Docusign:
                return _oAuth20TokenHelper.RequestCode<DocuSignLoginProvider>(
                                                                        DocuSignLoginProvider.DocuSignLoginProviderScopes,
                                                                        new Dictionary<string, string>
                                                                            {
                                                                        { "prompt", "login" }
                                                                            }, additionalStateArgs: additionals);
            case LoginProviderEnum.Box:
                return _oAuth20TokenHelper.RequestCode<BoxLoginProvider>(additionalStateArgs: additionals);

            case LoginProviderEnum.OneDrive:
                return _oAuth20TokenHelper.RequestCode<OneDriveLoginProvider>(OneDriveLoginProvider.OneDriveLoginProviderScopes, additionalStateArgs: additionals);

            case LoginProviderEnum.Wordpress:
                return _oAuth20TokenHelper.RequestCode<WordpressLoginProvider>(additionalStateArgs: additionals);

        }

        return null;
    }

    [Read("{provider}/code")]
    public object GetCode(string redirect, string code, string error)
    {
        try
        {
            if (!string.IsNullOrEmpty(error))
            {
                if (error == "access_denied")
                {
                    error = "Canceled at provider";
                }
                throw new Exception(error);
            }

            if (!string.IsNullOrEmpty(redirect))
            {
                return AppendCode(redirect, code);
            }

            return code;
        }
        catch (ThreadAbortException)
        {
        }
        catch (Exception ex)
        {
            if (!string.IsNullOrEmpty(redirect))
            {
                return AppendCode(redirect, error: ex.Message);
            }

            return ex.Message;
        }

        return null;
    }


    private static string AppendCode(string url, string code = null, string error = null)
    {
        url += (url.Contains('#') ? "&" : "#")
                + (string.IsNullOrEmpty(error)
                        ? (string.IsNullOrEmpty(code)
                                ? string.Empty
                                : "code=" + HttpUtility.UrlEncode(code))
                        : ("error/" + HttpUtility.UrlEncode(error)));

        return url;
    }
}
