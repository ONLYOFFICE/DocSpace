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
using System.Web;

using ASC.Common;
using ASC.Common.Caching;
using ASC.Common.Utils;
using ASC.Core;
using ASC.Core.Common.Configuration;
using ASC.FederatedLogin.Helpers;
using ASC.FederatedLogin.Profile;
using ASC.Security.Cryptography;

using Microsoft.Extensions.Configuration;

namespace ASC.FederatedLogin.LoginProviders
{
    [Scope]
    public class WordpressLoginProvider : BaseLoginProvider<WordpressLoginProvider>
    {
        public const string WordpressMeInfoUrl = "https://public-api.wordpress.com/rest/v1/me";
        public const string WordpressSites = "https://public-api.wordpress.com/rest/v1.2/sites/";

        public WordpressLoginProvider()
        {
        }

        public WordpressLoginProvider(
            OAuth20TokenHelper oAuth20TokenHelper,
            TenantManager tenantManager,
            CoreBaseSettings coreBaseSettings,
            CoreSettings coreSettings,
            IConfiguration configuration,
            ICacheNotify<ConsumerCacheItem> cache,
            ConsumerFactory consumerFactory,
            Signature signature,
            InstanceCrypto instanceCrypto,
            RequestHelper requestHelper,
            string name, int order, Dictionary<string, string> props, Dictionary<string, string> additional = null)
            : base(oAuth20TokenHelper, tenantManager, coreBaseSettings, coreSettings, configuration, cache, consumerFactory, signature, instanceCrypto, name, order, props, additional)
        {
            _requestHelper = requestHelper;
        }

        public static string GetWordpressMeInfo(RequestHelper requestHelper, string token)
        {
            var headers = new Dictionary<string, string>
                {
                    { "Authorization", "bearer " + token }
                };
            return requestHelper.PerformRequest(WordpressMeInfoUrl, "", "GET", "", headers);
        }

        public static bool CreateWordpressPost(RequestHelper requestHelper, string title, string content, string status, string blogId, OAuth20Token token)
        {
            try
            {
                var uri = WordpressSites + blogId + "/posts/new";
                const string contentType = "application/x-www-form-urlencoded";
                const string method = "POST";
                var body = "title=" + HttpUtility.UrlEncode(title) + "&content=" + HttpUtility.UrlEncode(content) + "&status=" + status + "&format=standard";
                var headers = new Dictionary<string, string>
                    {
                        { "Authorization", "bearer " + token.AccessToken }
                    };

                requestHelper.PerformRequest(uri, contentType, method, body, headers);
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public override string CodeUrl
        {
            get { return "https://public-api.wordpress.com/oauth2/authorize"; }
        }

        public override string AccessTokenUrl
        {
            get { return "https://public-api.wordpress.com/oauth2/token"; }
        }

        public override string RedirectUri
        {
            get { return this["wpRedirectUrl"]; }
        }

        public override string ClientID
        {
            get { return this["wpClientId"]; }
        }

        public override string ClientSecret
        {
            get { return this["wpClientSecret"]; }
        }

        private readonly RequestHelper _requestHelper;

        public override LoginProfile GetLoginProfile(string accessToken)
        {
            throw new NotImplementedException();
        }
    }
}