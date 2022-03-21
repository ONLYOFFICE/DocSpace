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
public class LinkedInLoginProvider : BaseLoginProvider<LinkedInLoginProvider>
{
    public override string AccessTokenUrl => "https://www.linkedin.com/oauth/v2/accessToken";
    public override string RedirectUri => this["linkedInRedirectUrl"];
    public override string ClientID => this["linkedInKey"];
    public override string ClientSecret => this["linkedInSecret"];
    public override string CodeUrl => "https://www.linkedin.com/oauth/v2/authorization";
    public override string Scopes => "r_liteprofile r_emailaddress";

    private const string LinkedInProfileUrl = "https://api.linkedin.com/v2/me";
    private const string LinkedInEmailUrl = "https://api.linkedin.com/v2/emailAddress?q=members&projection=(elements*(handle~))";
    private readonly RequestHelper _requestHelper;

    public LinkedInLoginProvider() { }

    public LinkedInLoginProvider(
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

    internal LoginProfile ProfileFromLinkedIn(string linkedInProfile)
    {
        var jProfile = JObject.Parse(linkedInProfile);
        if (jProfile == null)
        {
            throw new Exception("Failed to correctly process the response");
        }

        var profile = new LoginProfile(Signature, InstanceCrypto)
        {
            Id = jProfile.Value<string>("id"),
            FirstName = jProfile.Value<string>("localizedFirstName"),
            LastName = jProfile.Value<string>("localizedLastName"),
            EMail = jProfile.Value<string>("emailAddress"),
            Provider = ProviderConstants.LinkedIn,
        };

        return profile;
    }

    internal static string EmailFromLinkedIn(string linkedInEmail)
    {
        var jEmail = JObject.Parse(linkedInEmail);
        if (jEmail == null)
        {
            throw new Exception("Failed to correctly process the response");
        }

        return jEmail.SelectToken("elements[0].handle~.emailAddress").ToString();
    }

    private LoginProfile RequestProfile(string accessToken)
    {
        var linkedInProfile = _requestHelper.PerformRequest(LinkedInProfileUrl,
            headers: new Dictionary<string, string> { { "Authorization", "Bearer " + accessToken } });
        var loginProfile = ProfileFromLinkedIn(linkedInProfile);

        var linkedInEmail = _requestHelper.PerformRequest(LinkedInEmailUrl, headers: new Dictionary<string, string> { { "Authorization", "Bearer " + accessToken } });
        loginProfile.EMail = EmailFromLinkedIn(linkedInEmail);

        return loginProfile;
    }
}
