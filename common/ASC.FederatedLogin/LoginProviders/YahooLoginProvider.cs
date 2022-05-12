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
public class YahooLoginProvider : BaseLoginProvider<YahooLoginProvider>
{
    public const string YahooUrlUserGuid = "https://social.yahooapis.com/v1/me/guid";
    public const string YahooUrlContactsFormat = "https://social.yahooapis.com/v1/user/{0}/contacts";

    public override string CodeUrl => "https://api.login.yahoo.com/oauth2/request_auth";
    public override string AccessTokenUrl => "https://api.login.yahoo.com/oauth2/get_token";
    public override string RedirectUri => this["yahooRedirectUrl"];
    public override string ClientID => this["yahooClientId"];
    public override string ClientSecret => this["yahooClientSecret"];
    public override string Scopes => "sdct-r";

    public YahooLoginProvider() { }

    public YahooLoginProvider(
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
        : base(oAuth20TokenHelper, tenantManager, coreBaseSettings, coreSettings, configuration, cache, consumerFactory, signature, instanceCrypto, name, order, props, additional) { }

    public OAuth20Token Auth(HttpContext context)
    {
        return Auth(context, Scopes, out var _);
    }

    public override LoginProfile GetLoginProfile(string accessToken)
    {
        throw new NotImplementedException();
    }
}
