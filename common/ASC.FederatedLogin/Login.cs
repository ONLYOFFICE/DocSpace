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
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Web;

using ASC.Common.Utils;
using ASC.Core.Common.Configuration;
using ASC.FederatedLogin.Helpers;
using ASC.FederatedLogin.LoginProviders;
using ASC.FederatedLogin.Profile;
using ASC.Security.Cryptography;

using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Caching.Memory;

using Newtonsoft.Json;

namespace ASC.FederatedLogin
{
    public class Login
    {
        private Dictionary<string, string> _params;

        private readonly RequestDelegate _next;
        private IWebHostEnvironment WebHostEnvironment { get; }
        private IMemoryCache MemoryCache { get; }
        public Signature Signature { get; }
        public InstanceCrypto InstanceCrypto { get; }
        public ConsumerFactory ConsumerFactory { get; }

        public Login(
            RequestDelegate next,
            IWebHostEnvironment webHostEnvironment,
            IMemoryCache memoryCache,
            Signature signature,
            InstanceCrypto instanceCrypto,
            ConsumerFactory consumerFactory)
        {
            _next = next;
            WebHostEnvironment = webHostEnvironment;
            MemoryCache = memoryCache;
            Signature = signature;
            InstanceCrypto = instanceCrypto;
            ConsumerFactory = consumerFactory;
        }


        public async Task Invoke(HttpContext context)
        {
            context.PushRewritenUri();

            if (string.IsNullOrEmpty(context.Request.Query["p"]))
            {
                _params = new Dictionary<string, string>(context.Request.Query.Count);
                //Form params and redirect
                foreach (var key in context.Request.Query.Keys)
                {
                    _params.Add(key, context.Request.Query[key]);
                }

                //Pack and redirect
                var uriBuilder = new UriBuilder(context.Request.GetUrlRewriter());
                var token = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(_params)));
                uriBuilder.Query = "p=" + token;
                context.Response.Redirect(uriBuilder.Uri.ToString(), true);
            }
            else
            {
                _params = ((Dictionary<string, object>)JsonConvert.DeserializeObject(
                    Encoding.UTF8.GetString(WebEncoders.Base64UrlDecode(context.Request.Query["p"])))).ToDictionary(x => x.Key, y => (string)y.Value);
            }

            if (!string.IsNullOrEmpty(Auth))
            {
                try
                {
                    var profile = ProviderManager.Process(Auth, Signature, InstanceCrypto, ConsumerFactory, context, _params);
                    if (profile != null)
                    {
                        await SendClientData(context, profile);
                    }
                }
                catch (ThreadAbortException)
                {
                    //Thats is responce ending
                }
                catch (Exception ex)
                {
                    await SendClientData(context, LoginProfile.FromError(Signature, InstanceCrypto, ex));
                }
            }
            else
            {
                //Render xrds
                await RenderXrds(context);
            }
            context.PopRewritenUri();
        }

        protected bool Minimal
        {
            get
            {
                if (_params.ContainsKey("min"))
                {
                    bool.TryParse(_params.Get("min"), out var result);
                    return result;
                }
                return false;
            }
        }

        protected string Callback
        {
            get { return _params.Get("callback") ?? "loginCallback"; }
        }

        private async Task RenderXrds(HttpContext context)
        {
            var xrdsloginuri = new Uri(context.Request.GetUrlRewriter(), new Uri(context.Request.GetUrlRewriter().AbsolutePath, UriKind.Relative)) + "?auth=openid&returnurl=" + ReturnUrl;
            var xrdsimageuri = new Uri(context.Request.GetUrlRewriter(), new Uri(WebHostEnvironment.WebRootPath, UriKind.Relative)) + "openid.gif";
            await XrdsHelper.RenderXrds(context.Response, xrdsloginuri, xrdsimageuri);
        }

        protected LoginMode Mode
        {
            get
            {
                if (!string.IsNullOrEmpty(_params.Get("mode")))
                {
                    return (LoginMode)Enum.Parse(typeof(LoginMode), _params.Get("mode"), true);
                }
                return LoginMode.Popup;
            }
        }

        protected string ReturnUrl
        {
            get { return _params.Get("returnurl"); } //TODO?? FormsAuthentication.LoginUrl; }
        }

        protected string Auth
        {
            get { return _params.Get("auth"); }
        }

        public bool IsReusable
        {
            get { return false; }
        }

        private async Task SendClientData(HttpContext context, LoginProfile profile)
        {
            switch (Mode)
            {
                case LoginMode.Redirect:
                    RedirectToReturnUrl(context, profile);
                    break;
                case LoginMode.Popup:
                    await SendJsCallback(context, profile);
                    break;
            }
        }

        private async Task SendJsCallback(HttpContext context, LoginProfile profile)
        {
            //Render a page
            context.Response.ContentType = "text/html";
            await context.Response.WriteAsync(JsCallbackHelper.GetCallbackPage().Replace("%PROFILE%", profile.ToJson()).Replace("%CALLBACK%", Callback));
        }

        private void RedirectToReturnUrl(HttpContext context, LoginProfile profile)
        {
            var useMinimalProfile = Minimal;
            if (useMinimalProfile)
                profile = profile.GetMinimalProfile(); //Only id and provider

            if (context.Session != null && !useMinimalProfile)
            {
                //Store in session
                context.Response.Redirect(new Uri(ReturnUrl, UriKind.Absolute).AddProfileSession(profile, context).ToString(), true);
            }
            else if (MemoryCache != null && !useMinimalProfile)
            {
                context.Response.Redirect(new Uri(ReturnUrl, UriKind.Absolute).AddProfileCache(profile, MemoryCache).ToString(), true);
            }
            else
            {
                context.Response.Redirect(new Uri(ReturnUrl, UriKind.Absolute).AddProfile(profile).ToString(), true);
            }
        }
    }
}