/*
 *
 * (c) Copyright Ascensio System Limited 2010-2018
 *
 * This program is freeware. You can redistribute it and/or modify it under the terms of the GNU 
 * General Public License (GPL) version 3 as published by the Free Software Foundation (https://www.gnu.org/copyleft/gpl.html). 
 * In accordance with Section 7(a) of the GNU GPL its Section 15 shall be amended to the effect that 
 * Ascensio System SIA expressly excludes the warranty of non-infringement of any third-party rights.
 *
 * THIS PROGRAM IS DISTRIBUTED WITHOUT ANY WARRANTY; WITHOUT EVEN THE IMPLIED WARRANTY OF MERCHANTABILITY OR
 * FITNESS FOR A PARTICULAR PURPOSE. For more details, see GNU GPL at https://www.gnu.org/copyleft/gpl.html
 *
 * You can contact Ascensio System SIA by email at sales@onlyoffice.com
 *
 * The interactive user interfaces in modified source and object code versions of ONLYOFFICE must display 
 * Appropriate Legal Notices, as required under Section 5 of the GNU GPL version 3.
 *
 * Pursuant to Section 7 § 3(b) of the GNU GPL you must retain the original ONLYOFFICE logo which contains 
 * relevant author attributions when distributing the software. If the display of the logo in its graphic 
 * form is not reasonably feasible for technical reasons, you must include the words "Powered by ONLYOFFICE" 
 * in every copy of the program you distribute. 
 * Pursuant to Section 7 § 3(e) we decline to grant you any rights under trademark law for use of our trademarks.
 *
*/


using System;
using System.Collections.Generic;
using System.Text;

using ASC.Common;
using ASC.Common.Caching;
using ASC.Core;
using ASC.Core.Common.Configuration;
using ASC.FederatedLogin.Helpers;

using Microsoft.Extensions.Configuration;

namespace ASC.FederatedLogin.LoginProviders
{
    [Scope]
    public class DocuSignLoginProvider : Consumer, IOAuthProvider
    {
        public static string DocuSignLoginProviderScopes { get { return "signature"; } }
        public string Scopes { get { return DocuSignLoginProviderScopes; } }
        public string CodeUrl { get { return DocuSignHost + "/oauth/auth"; } }
        public string AccessTokenUrl { get { return DocuSignHost + "/oauth/token"; } }
        public string RedirectUri { get { return this["docuSignRedirectUrl"]; } }
        public string ClientID { get { return this["docuSignClientId"]; } }
        public string ClientSecret { get { return this["docuSignClientSecret"]; } }
        public string DocuSignHost { get { return "https://" + this["docuSignHost"]; } }

        public bool IsEnabled
        {
            get
            {
                return !string.IsNullOrEmpty(ClientID) &&
                       !string.IsNullOrEmpty(ClientSecret) &&
                       !string.IsNullOrEmpty(RedirectUri);
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

        private readonly RequestHelper _requestHelper;

        public OAuth20Token GetAccessToken(string authCode)
        {
            if (string.IsNullOrEmpty(authCode)) throw new ArgumentNullException(nameof(authCode));
            if (string.IsNullOrEmpty(ClientID)) throw new ArgumentException("clientID");
            if (string.IsNullOrEmpty(ClientSecret)) throw new ArgumentException("clientSecret");

            var data = $"grant_type=authorization_code&code={authCode}";
            var headers = new Dictionary<string, string> { { "Authorization", AuthHeader } };

            var json = _requestHelper.PerformRequest(AccessTokenUrl, "application/x-www-form-urlencoded", "POST", data, headers);
            if (json == null) throw new Exception("Can not get token");

            if (!json.StartsWith('{'))
            {
                json = "{\"" + json.Replace("=", "\":\"").Replace("&", "\",\"") + "\"}";
            }

            var token = OAuth20Token.FromJson(json);
            if (token == null) return null;

            token.ClientID = ClientID;
            token.ClientSecret = ClientSecret;
            token.RedirectUri = RedirectUri;
            return token;
        }

        public OAuth20Token RefreshToken(string refreshToken)
        {
            if (string.IsNullOrEmpty(refreshToken) || string.IsNullOrEmpty(ClientID) || string.IsNullOrEmpty(ClientSecret))
                throw new ArgumentException("Can not refresh given token");

            var data = $"grant_type=refresh_token&refresh_token={refreshToken}";
            var headers = new Dictionary<string, string> { { "Authorization", AuthHeader } };

            var json = _requestHelper.PerformRequest(AccessTokenUrl, "application/x-www-form-urlencoded", "POST", data, headers);
            if (json == null) throw new Exception("Can not get token");

            var refreshed = OAuth20Token.FromJson(json);
            refreshed.ClientID = ClientID;
            refreshed.ClientSecret = ClientSecret;
            refreshed.RedirectUri = RedirectUri;
            refreshed.RefreshToken ??= refreshToken;
            return refreshed;
        }
    }
}