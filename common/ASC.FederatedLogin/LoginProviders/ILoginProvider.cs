namespace ASC.FederatedLogin.LoginProviders;

public interface ILoginProvider : IOAuthProvider
{
    LoginProfile ProcessAuthoriztion(HttpContext context, IDictionary<string, string> @params, IDictionary<string, string> additionalStateArgs);

    LoginProfile GetLoginProfile(string accessToken);
}

public interface IOAuthProvider
{
    string Scopes { get; }
    string CodeUrl { get; }
    string AccessTokenUrl { get; }
    string RedirectUri { get; }
    string ClientID { get; }
    string ClientSecret { get; }
    bool IsEnabled { get; }
}
