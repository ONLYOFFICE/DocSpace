//using System;
//using System.Collections.Generic;
//using System.Globalization;
//using System.Web;

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