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
public class FacebookLoginProvider : BaseLoginProvider<FacebookLoginProvider>
{
    public override string AccessTokenUrl => "https://graph.facebook.com/v2.7/oauth/access_token";
    public override string RedirectUri => this["facebookRedirectUrl"];
    public override string ClientID => this["facebookClientId"];
    public override string ClientSecret => this["facebookClientSecret"];
    public override string CodeUrl => "https://www.facebook.com/v2.7/dialog/oauth/";
    public override string Scopes => "email,public_profile";

    private const string FacebookProfileUrl = "https://graph.facebook.com/v2.7/me?fields=email,id,birthday,link,first_name,last_name,gender,timezone,locale";

    public FacebookLoginProvider() { }

    private readonly RequestHelper _requestHelper;

    public FacebookLoginProvider(
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

    public override LoginProfile GetLoginProfile(string accessToken)
    {
        if (string.IsNullOrEmpty(accessToken))
        {
            throw new Exception("Login failed");
        }

        return RequestProfile(accessToken);
    }

    internal LoginProfile ProfileFromFacebook(string facebookProfile)
    {
        var jProfile = JObject.Parse(facebookProfile);
        if (jProfile == null)
        {
            throw new Exception("Failed to correctly process the response");
        }

        var profile = new LoginProfile(Signature, InstanceCrypto)
        {
            BirthDay = jProfile.Value<string>("birthday"),
            Link = jProfile.Value<string>("link"),
            FirstName = jProfile.Value<string>("first_name"),
            LastName = jProfile.Value<string>("last_name"),
            Gender = jProfile.Value<string>("gender"),
            EMail = jProfile.Value<string>("email"),
            Id = jProfile.Value<string>("id"),
            TimeZone = jProfile.Value<string>("timezone"),
            Locale = jProfile.Value<string>("locale"),
            Provider = ProviderConstants.Facebook,
            Avatar = "http://graph.facebook.com/" + jProfile.Value<string>("id") + "/picture?type=large"
        };

        return profile;
    }

    private LoginProfile RequestProfile(string accessToken)
    {
        var facebookProfile = _requestHelper.PerformRequest(FacebookProfileUrl + "&access_token=" + accessToken);
        var loginProfile = ProfileFromFacebook(facebookProfile);

        return loginProfile;
    }
}
