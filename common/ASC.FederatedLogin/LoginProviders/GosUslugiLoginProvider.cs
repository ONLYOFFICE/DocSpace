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
using System.Linq;
using System.Security.Cryptography.Pkcs;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading;
using System.Web;

using ASC.Common;
using ASC.Common.Caching;
using ASC.Common.Utils;
using ASC.Core;
using ASC.Core.Common.Configuration;
using ASC.FederatedLogin.Helpers;
using ASC.FederatedLogin.Profile;
using ASC.Security.Cryptography;
using ASC.Web.Core.Files;

using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Configuration;

using Newtonsoft.Json.Linq;

namespace ASC.FederatedLogin.LoginProviders
{
    [Scope]
    public class GosUslugiLoginProvider : BaseLoginProvider<GosUslugiLoginProvider>
    {
        public string BaseDomain
        {
            get { return this["gosUslugiDomain"]; }
        }

        public override string CodeUrl
        {
            get { return BaseDomain + "/aas/oauth2/ac"; }
        }

        public override string AccessTokenUrl
        {
            get { return BaseDomain + "/aas/oauth2/te"; }
        }

        public override string ClientID
        {
            get { return this["gosUslugiClientId"]; }
        }

        public override string ClientSecret
        {
            get { return this["gosUslugiCert"]; }
        }

        public override string RedirectUri
        {
            get { return this["gosUslugiRedirectUrl"]; }
        }

        public override string Scopes
        {
            get { return "fullname birthdate gender email"; }
        }

        private string GosUslugiProfileUrl
        {
            get { return BaseDomain + "/rs/prns/"; }
        }

        private readonly RequestHelper _requestHelper;

        public GosUslugiLoginProvider()
        {
        }

        public GosUslugiLoginProvider(
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
                var token = Auth(context, Scopes, out var redirect);

                if (redirect)
                {
                    return null;
                }

                if (token == null)
                {
                    throw new Exception("Login failed");
                }

                return GetLoginProfile(token.AccessToken);
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

        protected override OAuth20Token Auth(HttpContext context, string scopes, out bool redirect, IDictionary<string, string> additionalArgs = null, IDictionary<string, string> additionalStateArgs = null)
        {
            var error = context.Request.Query["error"];
            if (!string.IsNullOrEmpty(error))
            {
                if (error == "access_denied")
                {
                    error = "Canceled at provider";
                }
                throw new Exception(error);
            }

            var code = context.Request.Query["code"];
            if (string.IsNullOrEmpty(code))
            {
                RequestCode(context, scopes);
                redirect = true;
                return null;
            }

            redirect = false;
            var state = context.Request.Query["state"];
            return GetAccessToken(state, code);
        }

        private void RequestCode(HttpContext context, string scope = null)
        {
            var timestamp = DateTime.UtcNow.ToString("yyyy.MM.dd HH:mm:ss +0000");
            var state = Guid.NewGuid().ToString();//HttpContext.Current.Request.GetUrlRewriter().AbsoluteUri;

            var msg = scope + timestamp + ClientID + state;
            var encodedSignature = SignMsg(msg);
            var clientSecret = WebEncoders.Base64UrlEncode(encodedSignature);

            var requestParams = new Dictionary<string, string>
                {
                    { "client_id", ClientID },
                    { "client_secret", clientSecret },
                    { "redirect_uri", RedirectUri },
                    { "scope", scope },
                    { "response_type", "code" },
                    { "state", state },
                    { "timestamp", timestamp },
                    { "access_type", "online" },
                    { "display", "popup" }
                };
            var requestQuery = string.Join("&", requestParams.Select(pair => pair.Key + "=" + HttpUtility.UrlEncode(pair.Value)));//.Replace("+", "%2b");

            var redURL = CodeUrl + "?" + requestQuery;
            context.Response.Redirect(redURL, true);
        }

        private OAuth20Token GetAccessToken(string state, string code)
        {
            var timestamp = DateTime.UtcNow.ToString("yyyy.MM.dd HH:mm:ss +0000");

            var msg = Scopes + timestamp + ClientID + state;
            var encodedSignature = SignMsg(msg);
            var clientSecret = WebEncoders.Base64UrlEncode(encodedSignature);

            var requestParams = new Dictionary<string, string>
                {
                    { "client_id", ClientID },
                    { "code", code },
                    { "grant_type", "authorization_code" },
                    { "client_secret", clientSecret },
                    { "state", state },
                    { "redirect_uri", RedirectUri },
                    { "scope", Scopes },
                    { "timestamp", timestamp },
                    { "token_type", "Bearer" }
                };
            var requestQuery = string.Join("&", requestParams.Select(pair => pair.Key + "=" + HttpUtility.UrlEncode(pair.Value)));

            var result = _requestHelper.PerformRequest(AccessTokenUrl, "application/x-www-form-urlencoded", "POST", requestQuery);

            return OAuth20Token.FromJson(result);
        }

        public override LoginProfile GetLoginProfile(string accessToken)
        {
            var tokenPayloadString = JsonWebToken.Decode(accessToken, string.Empty, false, true);
            var tokenPayload = JObject.Parse(tokenPayloadString);
            if (tokenPayload == null)
            {
                throw new Exception("Payload is incorrect");
            }
            var oid = tokenPayload.Value<string>("urn:esia:sbj_id");

            var userInfoString = _requestHelper.PerformRequest(GosUslugiProfileUrl + oid, "application/x-www-form-urlencoded", headers: new Dictionary<string, string> { { "Authorization", "Bearer " + accessToken } });
            var userInfo = JObject.Parse(userInfoString);
            if (userInfo == null)
            {
                throw new Exception("userinfo is incorrect");
            }

            var profile = new LoginProfile(Signature, InstanceCrypto)
            {
                Id = oid,
                FirstName = userInfo.Value<string>("firstName"),
                LastName = userInfo.Value<string>("lastName"),

                Provider = ProviderConstants.GosUslugi,
            };

            var userContactsString = _requestHelper.PerformRequest(GosUslugiProfileUrl + oid + "/ctts", "application/x-www-form-urlencoded", headers: new Dictionary<string, string> { { "Authorization", "Bearer " + accessToken } });
            var userContacts = JObject.Parse(userContactsString);
            if (userContacts == null)
            {
                throw new Exception("usercontacts is incorrect");
            }

            var contactElements = userContacts.Value<JArray>("elements");
            if (contactElements == null)
            {
                throw new Exception("usercontacts elements is incorrect");
            }

            foreach (var contactElement in contactElements.ToObject<List<string>>())
            {
                var userContactString = _requestHelper.PerformRequest(contactElement, "application/x-www-form-urlencoded", headers: new Dictionary<string, string> { { "Authorization", "Bearer " + accessToken } });

                var userContact = JObject.Parse(userContactString);
                if (userContact == null)
                {
                    throw new Exception("usercontacts is incorrect");
                }

                var type = userContact.Value<string>("type");
                if (type != "EML") continue;

                profile.EMail = userContact.Value<string>("value");
                break;
            }

            return profile;
        }

        private X509Certificate2 GetSignerCert()
        {
            var storeMy = new X509Store(StoreName.Root, StoreLocation.LocalMachine);
            storeMy.Open(OpenFlags.ReadOnly);
            var certColl = storeMy.Certificates.Find(X509FindType.FindBySubjectKeyIdentifier, ClientSecret, false);
            storeMy.Close();
            if (certColl.Count == 0)
            {
                throw new Exception("Certificate not found");
            }
            return certColl[0];
        }

        private byte[] SignMsg(string msg)
        {
            var signerCert = GetSignerCert();

            var msgBytes = Encoding.UTF8.GetBytes(msg);
            var contentInfo = new ContentInfo(msgBytes);
            var signedCms = new SignedCms(contentInfo, true);
            var cmsSigner = new CmsSigner(signerCert);
            signedCms.ComputeSignature(cmsSigner);
            return signedCms.Encode();
        }

        //private static bool VerifyMsg(Byte[] msg, byte[] encodedSignature)
        //{
        //    ContentInfo contentInfo = new ContentInfo(msg);
        //    SignedCms signedCms = new SignedCms(contentInfo, true);
        //    signedCms.Decode(encodedSignature);

        //    try
        //    {
        //        signedCms.CheckSignature(true);
        //    }
        //    catch (System.Security.Cryptography.CryptographicException e)
        //    {
        //        return false;
        //    }

        //    return true;
        //}
    }
}