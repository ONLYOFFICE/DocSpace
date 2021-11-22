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
using System.Threading;
using System.Web;

using ASC.Common;
using ASC.FederatedLogin.Helpers;
using ASC.FederatedLogin.LoginProviders;
using ASC.Web.Api.Routing;

using Microsoft.AspNetCore.Mvc;

namespace ASC.Web.Api.Controllers
{
    [Scope(Additional = typeof(BaseLoginProviderExtension))]
    [DefaultRoute]
    [ApiController]
    public class ThirdPartyController : ControllerBase
    {
        private OAuth20TokenHelper OAuth20TokenHelper { get; }

        public ThirdPartyController(OAuth20TokenHelper oAuth20TokenHelper)
        {
            OAuth20TokenHelper = oAuth20TokenHelper;
        }

        [Read("{provider}")]
        public object Get(LoginProviderEnum provider)
        {
            var desktop = HttpContext.Request.Query["desktop"] == "true";
            var additionals = new Dictionary<string, string>();

            if (desktop)
            {
                additionals = HttpContext.Request.Query.ToDictionary(r => r.Key, r => r.Value.FirstOrDefault());
            }

            switch (provider)
            {
                case LoginProviderEnum.Google:
                    return OAuth20TokenHelper.RequestCode<GoogleLoginProvider>(
                                                                        GoogleLoginProvider.GoogleScopeDrive,
                                                                        new Dictionary<string, string>
                                                                            {
                                                                        { "access_type", "offline" },
                                                                        { "prompt", "consent" }
                                                                            }, additionalStateArgs: additionals);

                case LoginProviderEnum.Dropbox:
                    return OAuth20TokenHelper.RequestCode<DropboxLoginProvider>(
                                                        additionalArgs: new Dictionary<string, string>
                                                            {
                                                                            { "force_reauthentication", "true" }
                                                            }, additionalStateArgs: additionals);

                case LoginProviderEnum.Docusign:
                    return OAuth20TokenHelper.RequestCode<DocuSignLoginProvider>(
                                                                            DocuSignLoginProvider.DocuSignLoginProviderScopes,
                                                                            new Dictionary<string, string>
                                                                                {
                                                                            { "prompt", "login" }
                                                                                }, additionalStateArgs: additionals);
                case LoginProviderEnum.Box:
                    return OAuth20TokenHelper.RequestCode<BoxLoginProvider>(additionalStateArgs: additionals);

                case LoginProviderEnum.OneDrive:
                    return OAuth20TokenHelper.RequestCode<OneDriveLoginProvider>(OneDriveLoginProvider.OneDriveLoginProviderScopes, additionalStateArgs: additionals);

                case LoginProviderEnum.Wordpress:
                    return OAuth20TokenHelper.RequestCode<WordpressLoginProvider>(additionalStateArgs: additionals);

            }

            return null;
        }

        [Read("{provider}/code")]
        public object GetCode(string redirect, string code, string error)
        {
            try
            {
                if (!string.IsNullOrEmpty(error))
                {
                    if (error == "access_denied")
                    {
                        error = "Canceled at provider";
                    }
                    throw new Exception(error);
                }

                if (!string.IsNullOrEmpty(redirect))
                {
                    return AppendCode(redirect, code);
                }

                return code;
            }
            catch (ThreadAbortException)
            {
            }
            catch (Exception ex)
            {
                if (!string.IsNullOrEmpty(redirect))
                {
                    return AppendCode(redirect, error: ex.Message);
                }

                return ex.Message;
            }

            return null;
        }


        private static string AppendCode(string url, string code = null, string error = null)
        {
            url += (url.Contains("#") ? "&" : "#")
                   + (string.IsNullOrEmpty(error)
                          ? (string.IsNullOrEmpty(code)
                                 ? string.Empty
                                 : "code=" + HttpUtility.UrlEncode(code))
                          : ("error/" + HttpUtility.UrlEncode(error)));

            return url;
        }
    }
}
