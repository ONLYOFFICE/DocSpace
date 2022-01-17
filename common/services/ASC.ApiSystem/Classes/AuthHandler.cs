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
using System.Linq;
using System.Net;
using System.Security.Authentication;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Text.Encodings.Web;
using System.Threading.Tasks;

using ASC.Common;
using ASC.Common.Logging;
using ASC.Core.Common.Security;
using ASC.Security.Cryptography;
using ASC.Web.Core.Helpers;

using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace ASC.ApiSystem.Classes
{
    [Scope]
    public class AuthHandler : AuthenticationHandler<AuthenticationSchemeOptions>
    {
        public AuthHandler(IOptionsMonitor<AuthenticationSchemeOptions> options, ILoggerFactory logger, UrlEncoder encoder, ISystemClock clock) :
            base(options, logger, encoder, clock)
        {
        }
        public AuthHandler(
            IOptionsMonitor<AuthenticationSchemeOptions> options,
            ILoggerFactory logger,
            UrlEncoder encoder,
            ISystemClock clock,
            IConfiguration configuration,
            IOptionsMonitor<ILog> option,
            ApiSystemHelper apiSystemHelper,
            MachinePseudoKeys machinePseudoKeys,
            IHttpContextAccessor httpContextAccessor) :
            base(options, logger, encoder, clock)
        {
            Configuration = configuration;

            Log = option.Get("ASC.ApiSystem");

            ApiSystemHelper = apiSystemHelper;
            MachinePseudoKeys = machinePseudoKeys;
            HttpContextAccessor = httpContextAccessor;
        }

        private ILog Log { get; }

        private IConfiguration Configuration { get; }

        private ApiSystemHelper ApiSystemHelper { get; }
        private MachinePseudoKeys MachinePseudoKeys { get; }
        private IHttpContextAccessor HttpContextAccessor { get; }

        protected override Task<AuthenticateResult> HandleAuthenticateAsync()
        {
            if (Convert.ToBoolean(Configuration[Scheme.Name] ?? "false"))
            {
                Log.DebugFormat("Auth for {0} skipped", Scheme.Name);

                return Task.FromResult(AuthenticateResult.Success(new AuthenticationTicket(Context.User, new AuthenticationProperties(), Scheme.Name)));
            }

            try
            {
                Context.Request.Headers.TryGetValue("Authorization", out var headers);

                var header = headers.FirstOrDefault();

                if (string.IsNullOrEmpty(header))
                {
                    Log.Debug("Auth header is NULL");

                    return Task.FromResult(AuthenticateResult.Fail(new AuthenticationException(nameof(HttpStatusCode.Unauthorized))));
                }

                var substring = "ASC";

                if (header.StartsWith(substring, StringComparison.InvariantCultureIgnoreCase))
                {
                    var splitted = header.Substring(substring.Length).Trim().Split(':', StringSplitOptions.RemoveEmptyEntries);

                    if (splitted.Length < 3)
                    {
                        Log.DebugFormat("Auth failed: invalid token {0}.", header);

                        return Task.FromResult(AuthenticateResult.Fail(new AuthenticationException(nameof(HttpStatusCode.Unauthorized))));
                    }

                    var pkey = splitted[0];
                    var date = splitted[1];
                    var orighash = splitted[2];

                    Log.Debug("Variant of correct auth:" + ApiSystemHelper.CreateAuthToken(pkey));

                    if (!string.IsNullOrWhiteSpace(date))
                    {
                        var timestamp = DateTime.ParseExact(date, "yyyyMMddHHmmss", CultureInfo.InvariantCulture);

                        var trustInterval = TimeSpan.FromMinutes(Convert.ToDouble(Configuration["auth:trust-interval"] ?? "5"));

                        if (DateTime.UtcNow > timestamp.Add(trustInterval))
                        {
                            Log.DebugFormat("Auth failed: invalid timesatmp {0}, now {1}.", timestamp, DateTime.UtcNow);

                            return Task.FromResult(AuthenticateResult.Fail(new AuthenticationException(nameof(HttpStatusCode.Forbidden))));
                        }
                    }

                    var skey = MachinePseudoKeys.GetMachineConstant();
                    using var hasher = new HMACSHA1(skey);
                    var data = string.Join("\n", date, pkey);
                    var hash = hasher.ComputeHash(Encoding.UTF8.GetBytes(data));

                    if (WebEncoders.Base64UrlEncode(hash) != orighash && Convert.ToBase64String(hash) != orighash)
                    {
                        Log.DebugFormat("Auth failed: invalid token {0}, expect {1} or {2}.", orighash, WebEncoders.Base64UrlEncode(hash), Convert.ToBase64String(hash));

                        return Task.FromResult(AuthenticateResult.Fail(new AuthenticationException(nameof(HttpStatusCode.Forbidden))));
                    }
                }
                else
                {
                    Log.DebugFormat("Auth failed: invalid auth header. Sheme: {0}, parameter: {1}.", Scheme.Name, header);

                    return Task.FromResult(AuthenticateResult.Fail(new AuthenticationException(nameof(HttpStatusCode.Forbidden))));
                }
            }
            catch (Exception ex)
            {
                Log.Error(ex);

                return Task.FromResult(AuthenticateResult.Fail(new AuthenticationException(nameof(HttpStatusCode.InternalServerError))));
            }
            var identity = new ClaimsIdentity( Scheme.Name);

            Log.InfoFormat("Auth success {0}", Scheme.Name);
            if (HttpContextAccessor?.HttpContext != null) HttpContextAccessor.HttpContext.User = new CustomClaimsPrincipal(new ClaimsIdentity(Scheme.Name), identity);
            return Task.FromResult(AuthenticateResult.Success(new AuthenticationTicket(Context.User, new AuthenticationProperties(), Scheme.Name)));
        }
    }
}