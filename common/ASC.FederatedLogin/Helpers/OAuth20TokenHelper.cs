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

namespace ASC.FederatedLogin.Helpers;

[Scope]
public class OAuth20TokenHelper
{
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly ConsumerFactory _consumerFactory;
    private readonly RequestHelper _requestHelper;

    public OAuth20TokenHelper(IHttpContextAccessor httpContextAccessor, ConsumerFactory consumerFactory, RequestHelper requestHelper)
    {
        _httpContextAccessor = httpContextAccessor;
        _consumerFactory = consumerFactory;
        _requestHelper = requestHelper;
    }

    public string RequestCode<T>(string scope = null, IDictionary<string, string> additionalArgs = null, IDictionary<string, string> additionalStateArgs = null)
        where T : Consumer, IOAuthProvider, new()
    {
        var loginProvider = _consumerFactory.Get<T>();
        var requestUrl = loginProvider.CodeUrl;
        var clientID = loginProvider.ClientID;
        var redirectUri = loginProvider.RedirectUri;

        var uriBuilder = new UriBuilder(requestUrl);

        var query = uriBuilder.Query;
        if (!string.IsNullOrEmpty(query))
        {
            query += "&";
        }

        query += "response_type=code";

        if (!string.IsNullOrEmpty(clientID))
        {
            query += $"&client_id={HttpUtility.UrlEncode(clientID)}";
        }

        if (!string.IsNullOrEmpty(redirectUri))
        {
            query += $"&redirect_uri={HttpUtility.UrlEncode(redirectUri)}";
        }

        if (!string.IsNullOrEmpty(scope))
        {
            query += $"&scope={HttpUtility.UrlEncode(scope)}";
        }

        var u = _httpContextAccessor.HttpContext.Request.Url();

        var stateUriBuilder = new UriBuilder(u.Scheme, u.Host, u.Port, $"thirdparty/{loginProvider.Name.ToLower()}/code");

        if (additionalStateArgs != null && additionalStateArgs.Count > 0)
        {
            var stateQuery = "";
            stateQuery = additionalStateArgs.Keys
                .Where(a => a != null)
                .Aggregate(stateQuery, (current, a) => a != null ? $"{current}&{a.Trim()}={additionalStateArgs[a] ?? "".Trim()}" : null);

            stateUriBuilder.Query = stateQuery.Substring(1);
        }

        var state = HttpUtility.UrlEncode(stateUriBuilder.Uri.AbsoluteUri);
        query += $"&state={state}";

        if (additionalArgs != null)
        {
            query = additionalArgs.Keys.Where(additionalArg => additionalArg != null)
                                  .Aggregate(query, (current, additionalArg) =>
                                                    additionalArg != null ? current
                                                                            + "&" + HttpUtility.UrlEncode(additionalArg.Trim())
                                                                               + "=" + HttpUtility.UrlEncode((additionalArgs[additionalArg] ?? "").Trim()) : null);
        }

        return uriBuilder.Uri + "?" + query;
    }

    public OAuth20Token GetAccessToken<T>(ConsumerFactory consumerFactory, string authCode) where T : Consumer, IOAuthProvider, new()
    {
        var loginProvider = consumerFactory.Get<T>();
        var requestUrl = loginProvider.AccessTokenUrl;
        var clientID = loginProvider.ClientID;
        var clientSecret = loginProvider.ClientSecret;
        var redirectUri = loginProvider.RedirectUri;

        ArgumentNullOrEmptyException.ThrowIfNullOrEmpty(authCode);
        ArgumentNullOrEmptyException.ThrowIfNullOrEmpty(clientID);
        ArgumentNullOrEmptyException.ThrowIfNullOrEmpty(clientSecret);

        var data = $"code={HttpUtility.UrlEncode(authCode)}&client_id={HttpUtility.UrlEncode(clientID)}&client_secret={HttpUtility.UrlEncode(clientSecret)}";

        if (!string.IsNullOrEmpty(redirectUri))
        {
            data += "&redirect_uri=" + HttpUtility.UrlEncode(redirectUri);
        }

        data += "&grant_type=authorization_code";

        var json = _requestHelper.PerformRequest(requestUrl, "application/x-www-form-urlencoded", "POST", data);
        if (json != null)
        {
            if (!json.StartsWith('{'))
            {
                json = "{\"" + json.Replace("=", "\":\"").Replace("&", "\",\"") + "\"}";
            }

            var token = OAuth20Token.FromJson(json);
            if (token == null)
            {
                return null;
            }

            token.ClientID = clientID;
            token.ClientSecret = clientSecret;
            token.RedirectUri = redirectUri;

            return token;
        }

        return null;
    }

    public OAuth20Token RefreshToken<T>(ConsumerFactory consumerFactory, OAuth20Token token) where T : Consumer, IOAuthProvider, new()
    {
        var loginProvider = consumerFactory.Get<T>();

        return RefreshToken(loginProvider.AccessTokenUrl, token);
    }

    public OAuth20Token RefreshToken(string requestUrl, OAuth20Token token)
    {
        if (token == null || !CanRefresh(token))
        {
            throw new ArgumentException("Can not refresh given token", nameof(token));
        }

        var data = $"client_id={HttpUtility.UrlEncode(token.ClientID)}&client_secret={HttpUtility.UrlEncode(token.ClientSecret)}&refresh_token={HttpUtility.UrlEncode(token.RefreshToken)}&grant_type=refresh_token";

        var json = _requestHelper.PerformRequest(requestUrl, "application/x-www-form-urlencoded", "POST", data);
        if (json != null)
        {
            var refreshed = OAuth20Token.FromJson(json);
            refreshed.ClientID = token.ClientID;
            refreshed.ClientSecret = token.ClientSecret;
            refreshed.RedirectUri = token.RedirectUri;
            refreshed.RefreshToken ??= token.RefreshToken;

            return refreshed;
        }

        return token;
    }

    private static bool CanRefresh(OAuth20Token token)
    {
        return !string.IsNullOrEmpty(token.ClientID) && !string.IsNullOrEmpty(token.ClientSecret);
    }
}
