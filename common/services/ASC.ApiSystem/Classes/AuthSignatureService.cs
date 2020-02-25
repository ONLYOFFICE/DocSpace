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
using System.Globalization;
using System.Net;
using System.Net.Http;
using System.Security.Cryptography;
using System.Text;
using System.Web.Http.Controllers;
using ASC.Web.Core.Helpers;
using ASC.Common.Logging;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace ASC.ApiSystem.Classes
{
    public class AuthSignatureService
    {
        private ILog Log { get; }

        private IConfiguration Configuration { get; }

        private ApiSystemHelper ApiSystemHelper { get; }

        public AuthSignatureService(IConfiguration configuration, IOptionsMonitor<ILog> option, ApiSystemHelper apiSystemHelper)
        {
            Configuration = configuration;

            Log = option.Get("ASC.ApiSystem");

            ApiSystemHelper = apiSystemHelper;
        }


        public void OnAuthorization(HttpActionContext context, string allowskipCfgName)
        {
            allowskipCfgName = !string.IsNullOrEmpty(allowskipCfgName) ? allowskipCfgName : "auth.allowskip";

            if (Convert.ToBoolean(Configuration[allowskipCfgName] ?? "false"))
            {
                Log.DebugFormat("Auth for {0} skipped", allowskipCfgName);
                return; // skip auth
            }

            try
            {
                var header = context.ControllerContext.Request.Headers.Authorization;

                if (header == null)
                {
                    Log.Debug("Auth header is NULL");

                    context.Response = context.Request.CreateResponse(HttpStatusCode.Forbidden, new
                    {
                        error = "authorization",
                        message = "Authorization is requered"
                    });

                    return;
                }

                if (!string.IsNullOrWhiteSpace(header.Scheme) && header.Scheme.StartsWith("ASC", StringComparison.InvariantCultureIgnoreCase))
                {
                    var splitted = header.Parameter.Split(':');

                    if (splitted.Length < 3)
                    {
                        Log.DebugFormat("Auth failed: invalid token {0}.", header.Parameter);

                        context.Response = context.Request.CreateResponse(HttpStatusCode.Forbidden, new
                        {
                            error = "authorization",
                            message = "Authorization invalid"
                        });

                        return;
                    }

                    var pkey = splitted[0];
                    var date = splitted[1];
                    var orighash = splitted[2];

                    Log.Debug("Variant of correct auth:" + ApiSystemHelper.CreateAuthToken(pkey));

                    if (!string.IsNullOrWhiteSpace(date))
                    {
                        var timestamp = DateTime.ParseExact(date, "yyyyMMddHHmmss", CultureInfo.InvariantCulture);

                        var trustInterval = TimeSpan.FromMinutes(Convert.ToDouble(Configuration["auth.trust-interval"] ?? "5"));

                        if (header.Scheme == "ASC" && DateTime.UtcNow > timestamp.Add(trustInterval))
                        {
                            Log.DebugFormat("Auth failed: invalid timesatmp {0}, now {1}.", timestamp, DateTime.UtcNow);

                            context.Response = context.Request.CreateResponse(HttpStatusCode.Forbidden, new
                            {
                                error = "authorization",
                                message = "Authorization is expired"
                            });

                            return;
                        }
                    }

                    var skey = Configuration["core.machinekey"];
                    using var hasher = new HMACSHA1(Encoding.UTF8.GetBytes(skey));
                    var data = string.Join("\n", date, pkey);
                    var hash = hasher.ComputeHash(Encoding.UTF8.GetBytes(data));

                    if (WebEncoders.Base64UrlEncode(hash) != orighash && Convert.ToBase64String(hash) != orighash)
                    {
                        Log.DebugFormat("Auth failed: invalid token {0}, expect {1} or {2}.", orighash, WebEncoders.Base64UrlEncode(hash), Convert.ToBase64String(hash));

                        context.Response = context.Request.CreateResponse(HttpStatusCode.Forbidden, new
                        {
                            error = "authorization",
                            message = "Authorization invalid"
                        });

                        return;
                    }
                }
                else
                {
                    Log.DebugFormat("Auth failed: invalid auth header. Sheme: {0}, parameter: {1}.", header.Scheme, header.Parameter);

                    context.Response = context.Request.CreateResponse(HttpStatusCode.Forbidden, new
                    {
                        error = "authorization",
                        message = "Authorization scheme unknown"
                    });

                    return;
                }
            }
            catch (Exception ex)
            {
                Log.Error(ex);

                context.Response = context.Request.CreateResponse(HttpStatusCode.InternalServerError, new
                {
                    error = "authorization",
                    message = ex.Message,
                    stacktrace = ex.StackTrace
                });

                return;
            }

            Log.InfoFormat("Auth success {0}", allowskipCfgName);
        }
    }

    public static class AuthSignatureServiceExtention
    {
        public static IServiceCollection AddAuthSignatureService(this IServiceCollection services)
        {
            services
                .AddApiSystemHelper()
                .TryAddScoped<AuthSignatureService>();

            return services;
        }
    }
}