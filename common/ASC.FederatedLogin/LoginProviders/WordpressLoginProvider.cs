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
public class WordpressLoginProvider : BaseLoginProvider<WordpressLoginProvider>
{
    public const string WordpressMeInfoUrl = "https://public-api.wordpress.com/rest/v1/me";
    public const string WordpressSites = "https://public-api.wordpress.com/rest/v1.2/sites/";

    public override string CodeUrl => "https://public-api.wordpress.com/oauth2/authorize";
    public override string AccessTokenUrl => "https://public-api.wordpress.com/oauth2/token";
    public override string RedirectUri => this["wpRedirectUrl"];
    public override string ClientID => this["wpClientId"];
    public override string ClientSecret => this["wpClientSecret"];

    public WordpressLoginProvider() { }

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
        string name, int order, Dictionary<string, string> props, Dictionary<string, string> additional = null)
        : base(oAuth20TokenHelper, tenantManager, coreBaseSettings, coreSettings, configuration, cache, consumerFactory, signature, instanceCrypto, name, order, props, additional)
    {
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

    public override LoginProfile GetLoginProfile(string accessToken)
    {
        throw new NotImplementedException();
    }
}
