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
public class YandexLoginProvider : BaseLoginProvider<YandexLoginProvider>
{
    public override string CodeUrl => "https://oauth.yandex.ru/authorize";
    public override string AccessTokenUrl => "https://oauth.yandex.ru/token";
    public override string ClientID => this["yandexClientId"];
    public override string ClientSecret => this["yandexClientSecret"];
    public override string RedirectUri => this["yandexRedirectUrl"];

    private readonly RequestHelper _requestHelper;
    private const string YandexProfileUrl = "https://login.yandex.ru/info";


    public YandexLoginProvider() { }

    public YandexLoginProvider(
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
                        { "force_confirm", "true" }
                }
                : null, additionalStateArgs);

            if (redirect)
            {
                return null;
            }

            return GetLoginProfile(token?.AccessToken);
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
        var yandexProfile = _requestHelper.PerformRequest(YandexProfileUrl + "?format=json&oauth_token=" + accessToken);
        var loginProfile = ProfileFromYandex(yandexProfile);

        return loginProfile;
    }

    private LoginProfile ProfileFromYandex(string strProfile)
    {
        var jProfile = JObject.Parse(strProfile);
        if (jProfile == null)
        {
            throw new Exception("Failed to correctly process the response");
        }

        var profile = new LoginProfile(Signature, InstanceCrypto)
        {
            EMail = jProfile.Value<string>("default_email"),
            Id = jProfile.Value<string>("id"),
            FirstName = jProfile.Value<string>("first_name"),
            LastName = jProfile.Value<string>("last_name"),
            DisplayName = jProfile.Value<string>("display_name"),
            Gender = jProfile.Value<string>("sex"),

            Provider = ProviderConstants.Yandex,
        };

        return profile;
    }
}
