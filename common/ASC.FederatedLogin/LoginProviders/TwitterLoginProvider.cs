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

//using ASC.FederatedLogin.Profile;

//using Microsoft.AspNetCore.Http;

//namespace ASC.FederatedLogin.LoginProviders
//{
//    public class TwitterLoginProvider : BaseLoginProvider<TwitterLoginProvider>
//    {
//        public string TwitterKey { get { return Instance.ClientID; } }
//        public string TwitterSecret { get { return Instance.ClientSecret; } }
//        public string TwitterDefaultAccessToken { get { return Instance["twitterAccessToken_Default"]; } }
//        public string TwitterAccessTokenSecret { get { return Instance["twitterAccessTokenSecret_Default"]; } }

//        public override string AccessTokenUrl { get { return "https://api.twitter.com/oauth/access_token"; } }
//        public override string RedirectUri { get { return this["twitterRedirectUrl"]; } }
//        public override string ClientID { get { return this["twitterKey"]; } }
//        public override string ClientSecret { get { return this["twitterSecret"]; } }
//        public override string CodeUrl { get { return "https://api.twitter.com/oauth/request_token"; } }

//        public override bool IsEnabled
//        {
//            get
//            {
//                return !string.IsNullOrEmpty(ClientID) &&
//                       !string.IsNullOrEmpty(ClientSecret);
//            }
//        }

//        public TwitterLoginProvider() { }
//        public TwitterLoginProvider(string name, int order, Dictionary<string, string> props, Dictionary<string, string> additional = null) : base(name, order, props, additional) { }

//        public override LoginProfile ProcessAuthoriztion(HttpContext context, IDictionary<string, string> @params)
//        {
//            if (!string.IsNullOrEmpty(context.Request.Query["denied"]))
//            {
//                return LoginProfile.FromError(new Exception("Canceled at provider"));
//            }

//            if (string.IsNullOrEmpty(context.Request.Query["oauth_token"]))
//            {
//                var callbackAddress = new UriBuilder(RedirectUri)
//                {
//                    Query = "state=" + HttpUtility.UrlEncode(context.Request.GetUrlRewriter().AbsoluteUri)
//                };

//                var reqToken = OAuthUtility.GetRequestToken(TwitterKey, TwitterSecret, callbackAddress.ToString());
//                var url = OAuthUtility.BuildAuthorizationUri(reqToken.Token).ToString();
//                context.Response.Redirect(url, true);
//                return null;
//            }

//            var requestToken = context.Request.Query["oauth_token"];
//            var pin = context.Request.Query["oauth_verifier"];

//            var tokens = OAuthUtility.GetAccessToken(TwitterKey, TwitterSecret, requestToken, pin);

//            var accesstoken = new OAuthTokens
//            {
//                AccessToken = tokens.Token,
//                AccessTokenSecret = tokens.TokenSecret,
//                ConsumerKey = TwitterKey,
//                ConsumerSecret = TwitterSecret
//            };

//            var account = TwitterAccount.VerifyCredentials(accesstoken).ResponseObject;
//            return ProfileFromTwitter(account);
//        }

//        protected override OAuth20Token Auth(HttpContext context, string scopes, Dictionary<string, string> additional = null)
//        {
//            throw new NotImplementedException();
//        }

//        public override LoginProfile GetLoginProfile(string accessToken)
//        {
//            throw new NotImplementedException();
//        }

//        internal static LoginProfile ProfileFromTwitter(TwitterUser twitterUser)
//        {
//            return twitterUser == null
//                       ? null
//                       : new LoginProfile
//                       {
//                           Name = twitterUser.Name,
//                           DisplayName = twitterUser.ScreenName,
//                           Avatar = twitterUser.ProfileImageSecureLocation,
//                           TimeZone = twitterUser.TimeZone,
//                           Locale = twitterUser.Language,
//                           Id = twitterUser.Id.ToString(CultureInfo.InvariantCulture),
//                           Provider = ProviderConstants.Twitter
//                       };
//        }
//    }
//}