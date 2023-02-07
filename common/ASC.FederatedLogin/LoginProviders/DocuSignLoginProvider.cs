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

namespace ASC.FederatedLogin.LoginProviders;

[Scope]
public class DocuSignLoginProvider : Consumer, IOAuthProvider
{
    public static string DocuSignLoginProviderScopes => "signature";
    public string Scopes => DocuSignLoginProviderScopes;
    public string CodeUrl => DocuSignHost + "/oauth/auth";
    public string AccessTokenUrl => DocuSignHost + "/oauth/token";
    public string RedirectUri => this["docuSignRedirectUrl"];
    public string ClientID => this["docuSignClientId"];
    public string ClientSecret => this["docuSignClientSecret"];
    public string DocuSignHost => "https://" + this["docuSignHost"];
    public bool IsEnabled
    {
        get
        {
            return !string.IsNullOrEmpty(ClientID) &&
                   !string.IsNullOrEmpty(ClientSecret) &&
                   !string.IsNullOrEmpty(RedirectUri);
        }
    }
    private string AuthHeader
    {
        get
        {
            var codeAuth = $"{ClientID}:{ClientSecret}";
            var codeAuthBytes = Encoding.UTF8.GetBytes(codeAuth);
            var codeAuthBase64 = Convert.ToBase64String(codeAuthBytes);

            return "Basic " + codeAuthBase64;
        }
    }

    public DocuSignLoginProvider() { }

    public DocuSignLoginProvider(
        TenantManager tenantManager,
        CoreBaseSettings coreBaseSettings,
        CoreSettings coreSettings,
        IConfiguration configuration,
        ICacheNotify<ConsumerCacheItem> cache,
        ConsumerFactory consumerFactory,
            RequestHelper requestHelper,
        string name, int order, Dictionary<string, string> props, Dictionary<string, string> additional = null)
        : base(tenantManager, coreBaseSettings, coreSettings, configuration, cache, consumerFactory, name, order, props, additional)
    {
        _requestHelper = requestHelper;
    }

    private readonly RequestHelper _requestHelper;

    public OAuth20Token GetAccessToken(string authCode)
    {
        ArgumentNullOrEmptyException.ThrowIfNullOrEmpty(authCode);
        ArgumentNullOrEmptyException.ThrowIfNullOrEmpty(ClientID);
        ArgumentNullOrEmptyException.ThrowIfNullOrEmpty(ClientSecret);

        var data = $"grant_type=authorization_code&code={authCode}";
        var headers = new Dictionary<string, string> { { "Authorization", AuthHeader } };

        var json = _requestHelper.PerformRequest(AccessTokenUrl, "application/x-www-form-urlencoded", "POST", data, headers);
        if (json == null)
        {
            throw new Exception("Can not get token");
        }

        if (!json.StartsWith('{'))
        {
            json = "{\"" + json.Replace("=", "\":\"").Replace("&", "\",\"") + "\"}";
        }

        var token = OAuth20Token.FromJson(json);
        if (token == null)
        {
            return null;
        }

        token.ClientID = ClientID;
        token.ClientSecret = ClientSecret;
        token.RedirectUri = RedirectUri;

        return token;
    }

    public OAuth20Token RefreshToken(string refreshToken)
    {
        if (string.IsNullOrEmpty(refreshToken) || string.IsNullOrEmpty(ClientID) || string.IsNullOrEmpty(ClientSecret))
        {
            throw new ArgumentException("Can not refresh given token");
        }

        var data = $"grant_type=refresh_token&refresh_token={refreshToken}";
        var headers = new Dictionary<string, string> { { "Authorization", AuthHeader } };

        var json = _requestHelper.PerformRequest(AccessTokenUrl, "application/x-www-form-urlencoded", "POST", data, headers);
        if (json == null)
        {
            throw new Exception("Can not get token");
        }

        var refreshed = OAuth20Token.FromJson(json);
        refreshed.ClientID = ClientID;
        refreshed.ClientSecret = ClientSecret;
        refreshed.RedirectUri = RedirectUri;
        refreshed.RefreshToken ??= refreshToken;

        return refreshed;
    }
}
