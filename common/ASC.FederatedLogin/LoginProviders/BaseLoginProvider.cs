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
using System.Threading;

using ASC.Common;
using ASC.Common.Caching;
using ASC.Common.Utils;
using ASC.Core;
using ASC.Core.Common.Configuration;
using ASC.FederatedLogin.Helpers;
using ASC.FederatedLogin.Profile;
using ASC.Security.Cryptography;

using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;

namespace ASC.FederatedLogin.LoginProviders
{
    public enum LoginProviderEnum
    {
        Facebook,
        Google,
        Dropbox,
        Docusign,
        Box,
        OneDrive,
        GosUslugi,
        LinkedIn,
        MailRu,
        VK,
        Wordpress,
        Yahoo,
        Yandex
    }

    public abstract class BaseLoginProvider<T> : Consumer, ILoginProvider where T : Consumer, ILoginProvider, new()
    {
        public T Instance
        {
            get
            {
                return ConsumerFactory.Get<T>();
            }
        }

        public abstract string CodeUrl { get; }
        public abstract string AccessTokenUrl { get; }
        public abstract string RedirectUri { get; }
        public abstract string ClientID { get; }
        public abstract string ClientSecret { get; }
        public virtual string Scopes { get { return ""; } }

        public virtual bool IsEnabled
        {
            get
            {
                return !string.IsNullOrEmpty(ClientID) &&
                       !string.IsNullOrEmpty(ClientSecret) &&
                       !string.IsNullOrEmpty(RedirectUri);
            }
        }

        private OAuth20TokenHelper OAuth20TokenHelper { get; }
        internal Signature Signature { get; }
        internal InstanceCrypto InstanceCrypto { get; }

        protected BaseLoginProvider()
        {

        }

        protected BaseLoginProvider(
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
            : base(tenantManager, coreBaseSettings, coreSettings, configuration, cache, consumerFactory, name, order, props, additional)
        {
            OAuth20TokenHelper = oAuth20TokenHelper;
            Signature = signature;
            InstanceCrypto = instanceCrypto;
        }

        public virtual LoginProfile ProcessAuthoriztion(HttpContext context, IDictionary<string, string> @params, IDictionary<string, string> additionalStateArgs = null)
        {
            try
            {
                var token = Auth(context, Scopes, out var redirect, @params, additionalStateArgs);

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

        protected virtual OAuth20Token Auth(HttpContext context, string scopes, out bool redirect, IDictionary<string, string> additionalArgs = null, IDictionary<string, string> additionalStateArgs = null)
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
                context.Response.Redirect(OAuth20TokenHelper.RequestCode<T>(scopes, additionalArgs, additionalStateArgs));
                redirect = true;
                return null;
            }

            redirect = false;
            return OAuth20TokenHelper.GetAccessToken<T>(ConsumerFactory, code);
        }

        public abstract LoginProfile GetLoginProfile(string accessToken);
    }

    public class BaseLoginProviderExtension
    {
        public static void Register(DIHelper services)
        {
            services.TryAdd<BoxLoginProvider>();
            services.TryAdd<DropboxLoginProvider>();
            services.TryAdd<OneDriveLoginProvider>();
            services.TryAdd<DocuSignLoginProvider>();
            services.TryAdd<GoogleLoginProvider>();
            services.TryAdd<WordpressLoginProvider>();
        }
    }
}
