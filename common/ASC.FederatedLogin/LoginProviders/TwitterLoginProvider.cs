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

using System.Globalization;

using Tweetinvi;
using Tweetinvi.Auth;
using Tweetinvi.Parameters;

namespace ASC.FederatedLogin.LoginProviders;

public class TwitterLoginProvider : BaseLoginProvider<TwitterLoginProvider>
{
    public override string AccessTokenUrl { get { return "https://api.twitter.com/oauth/access_token"; } }
    public override string RedirectUri { get { return this["twitterRedirectUrl"]; } }
    public override string ClientID { get { return this["twitterKey"]; } }
    public override string ClientSecret { get { return this["twitterSecret"]; } }
    public override string CodeUrl { get { return "https://api.twitter.com/oauth/request_token"; } }

    private static readonly IAuthenticationRequestStore _myAuthRequestStore = new LocalAuthenticationRequestStore();

    public override bool IsEnabled
    {
        get
        {
            return !string.IsNullOrEmpty(ClientID) &&
                   !string.IsNullOrEmpty(ClientSecret);
        }
    }

    public TwitterLoginProvider() { }
    public TwitterLoginProvider(
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


    public override LoginProfile ProcessAuthoriztion(HttpContext context, IDictionary<string, string> @params, IDictionary<string, string> additionalStateArgs)
    {
        if (!string.IsNullOrEmpty(context.Request.Query["denied"]))
        {
            return LoginProfile.FromError(Signature, InstanceCrypto, new Exception("Canceled at provider"));
        }

        var appClient = new TwitterClient(ClientID, ClientSecret);

        if (string.IsNullOrEmpty(context.Request.Query["oauth_token"]))
        {
            var callbackAddress = new UriBuilder(RedirectUri)
            {
                Query = "state=" + HttpUtility.UrlEncode(context.Request.Url().AbsoluteUri)
            };

            var authenticationRequestId = Guid.NewGuid().ToString();

            // Add the user identifier as a query parameters that will be received by `ValidateTwitterAuth`
            var redirectURL = _myAuthRequestStore.AppendAuthenticationRequestIdToCallbackUrl(callbackAddress.ToString(), authenticationRequestId);

            // Initialize the authentication process
            var authenticationRequestToken = appClient.Auth.RequestAuthenticationUrlAsync(redirectURL)
                                   .ConfigureAwait(false)
                                   .GetAwaiter()
                                   .GetResult();

            // Store the token information in the store
            _myAuthRequestStore.AddAuthenticationTokenAsync(authenticationRequestId, authenticationRequestToken)
                                   .ConfigureAwait(false)
                                   .GetAwaiter()
                                   .GetResult();

            context.Response.Redirect(authenticationRequestToken.AuthorizationURL, true);

            return null;
        }

        // Extract the information from the redirection url
        var requestParameters = RequestCredentialsParameters.FromCallbackUrlAsync(context.Request.GetDisplayUrl(), _myAuthRequestStore).GetAwaiter().GetResult();
        // Request Twitter to generate the credentials.
        var userCreds = appClient.Auth.RequestCredentialsAsync(requestParameters)
                                   .ConfigureAwait(false)
                                   .GetAwaiter()
                                   .GetResult();

        // Congratulations the user is now authenticated!
        var userClient = new TwitterClient(userCreds);

        var user = userClient.Users.GetAuthenticatedUserAsync()
                                   .ConfigureAwait(false)
                                   .GetAwaiter()
                                   .GetResult();

        var userSettings = userClient.AccountSettings.GetAccountSettingsAsync()
                                   .ConfigureAwait(false)
                                   .GetAwaiter()
                                   .GetResult();

        return user == null
                   ? null
                   : new LoginProfile(Signature, InstanceCrypto)
                   {
                       Name = user.Name,
                       DisplayName = user.ScreenName,
                       Avatar = user.ProfileImageUrl,
                       Locale = userSettings.Language.ToString(),
                       Id = user.Id.ToString(CultureInfo.InvariantCulture),
                       Provider = ProviderConstants.Twitter
                   };

    }

    protected override OAuth20Token Auth(HttpContext context, string scopes, out bool redirect, IDictionary<string, string> additionalArgs = null, IDictionary<string, string> additionalStateArgs = null)
    {
        throw new NotImplementedException();
    }

    public override LoginProfile GetLoginProfile(string accessToken)
    {
        throw new NotImplementedException();
    }

    internal LoginProfile ProfileFromTwitter(string twitterProfile)
    {
        var jProfile = JObject.Parse(twitterProfile);
        if (jProfile == null) throw new Exception("Failed to correctly process the response");

        return new LoginProfile(Signature, InstanceCrypto)
        {
            DisplayName = jProfile.Value<string>("name"),
            Locale = jProfile.Value<string>("lang"),
            Id = jProfile.Value<string>("id"),
            Provider = ProviderConstants.Twitter
        };
    }
}