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
public class VKLoginProvider : BaseLoginProvider<VKLoginProvider>
{
    public override string CodeUrl => "https://oauth.vk.com/authorize";
    public override string AccessTokenUrl => "https://oauth.vk.com/access_token";
    public override string ClientID => this["vkClientId"];
    public override string ClientSecret => this["vkClientSecret"];
    public override string RedirectUri => this["vkRedirectUrl"];
    public override string Scopes => (new[] { 4194304 }).Sum().ToString();

    private readonly RequestHelper _requestHelper;
    private const string VKProfileUrl = "https://api.vk.com/method/users.get?v=5.103";

    public VKLoginProvider() { }

    public VKLoginProvider(
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


    public override LoginProfile ProcessAuthoriztion(HttpContext context, IDictionary<string, string> @params, IDictionary<string, string> additionalStateArgs)
    {
        try
        {
            var token = Auth(context, Scopes, out var redirect, context.Request.Query["access_type"] == "offline"
                ? new Dictionary<string, string>
                {
                        { "revoke", "1" }
                }
                : null, additionalStateArgs);

            if (redirect)
            {
                return null;
            }

            if (token == null)
            {
                throw new Exception("Login failed");
            }

            var loginProfile = GetLoginProfile(token.AccessToken);

            loginProfile.EMail = GetMail(token);

            return loginProfile;
        }
        catch (ThreadAbortException)
        {
            throw;
        }
        catch (Exception ex)
        {
            return LoginProfile.FromError(Signature, InstanceCrypto, ex);
        }
    }

    public override LoginProfile GetLoginProfile(string accessToken)
    {
        if (string.IsNullOrEmpty(accessToken))
        {
            throw new Exception("Login failed");
        }

        return RequestProfile(accessToken);
    }

    private LoginProfile RequestProfile(string accessToken)
    {
        var fields = new[] { "sex" };
        var vkProfile = _requestHelper.PerformRequest(VKProfileUrl + "&fields=" + HttpUtility.UrlEncode(string.Join(",", fields)) + "&access_token=" + accessToken);
        var loginProfile = ProfileFromVK(vkProfile);

        return loginProfile;
    }

    private LoginProfile ProfileFromVK(string strProfile)
    {
        var jProfile = JObject.Parse(strProfile);
        if (jProfile == null)
        {
            throw new Exception("Failed to correctly process the response");
        }

        var error = jProfile.Value<JObject>("error");
        if (error != null)
        {
            throw new Exception(error.Value<string>("error_msg"));
        }

        var profileJson = jProfile.Value<JArray>("response");
        if (profileJson == null)
        {
            throw new Exception("Failed to correctly process the response");
        }

        var vkProfiles = profileJson.ToObject<List<VKProfile>>();
        if (vkProfiles.Count == 0)
        {
            throw new Exception("Failed to correctly process the response");
        }

        var profile = new LoginProfile(Signature, InstanceCrypto)
        {
            Id = vkProfiles[0].Id,
            FirstName = vkProfiles[0].FirstName,
            LastName = vkProfiles[0].LastName,

            Provider = ProviderConstants.VK,
        };

        return profile;
    }

    private class VKProfile
    {
        public string Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
    }

    private static string GetMail(OAuth20Token token)
    {
        if (string.IsNullOrEmpty(token.OriginJson))
        {
            return null;
        }

        var parser = JObject.Parse(token.OriginJson);

        return parser?.Value<string>("email");
    }
}
