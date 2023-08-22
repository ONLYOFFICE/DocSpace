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

using Role = ASC.Common.Security.Authorizing.Role;

namespace ASC.Api.Core.Auth;

[Scope]
public class AuthHandler : AuthenticationHandler<AuthenticationSchemeOptions>
{
    private readonly ILogger<AuthHandler> _log;
    private readonly IConfiguration _configuration;
    private readonly ApiSystemHelper _apiSystemHelper;
    private readonly MachinePseudoKeys _machinePseudoKeys;
    private readonly IHttpContextAccessor _httpContextAccessor;

    public AuthHandler(
        IOptionsMonitor<AuthenticationSchemeOptions> options,
        ILoggerFactory logger,
        UrlEncoder encoder,
        ISystemClock clock,
        IConfiguration configuration,
        ILogger<AuthHandler> log,
        ApiSystemHelper apiSystemHelper,
        MachinePseudoKeys machinePseudoKeys,
        IHttpContextAccessor httpContextAccessor) :
        base(options, logger, encoder, clock)
    {
        _configuration = configuration;
        _log = log;
        _apiSystemHelper = apiSystemHelper;
        _machinePseudoKeys = machinePseudoKeys;
        _httpContextAccessor = httpContextAccessor;
    }

    protected override Task<AuthenticateResult> HandleAuthenticateAsync()
    {
        if (Convert.ToBoolean(_configuration[Scheme.Name] ?? "false"))
        {
            _log.LogDebug("Auth for {0} skipped", Scheme.Name);
            Authenticate();
            return Task.FromResult(AuthenticateResult.Success(new AuthenticationTicket(Context.User, new AuthenticationProperties(), Scheme.Name)));
        }

        try
        {
            Context.Request.Headers.TryGetValue("Authorization", out var headers);

            var header = headers.FirstOrDefault();

            if (string.IsNullOrEmpty(header))
            {
                _log.LogDebug("Auth header is NULL");

                return Task.FromResult(AuthenticateResult.Fail(new AuthenticationException(nameof(HttpStatusCode.Unauthorized))));
            }

            var substring = "ASC";

            if (header.StartsWith(substring, StringComparison.InvariantCultureIgnoreCase))
            {
                var splitted = header.Substring(substring.Length).Trim().Split(':', StringSplitOptions.RemoveEmptyEntries);

                if (splitted.Length < 3)
                {
                    _log.LogDebug("Auth failed: invalid token {0}.", header);

                    return Task.FromResult(AuthenticateResult.Fail(new AuthenticationException(nameof(HttpStatusCode.Unauthorized))));
                }

                var pkey = splitted[0];
                var date = splitted[1];
                var orighash = splitted[2];

                _log.LogDebug("Variant of correct auth:" + _apiSystemHelper.CreateAuthToken(pkey));

                if (!string.IsNullOrWhiteSpace(date))
                {
                    var timestamp = DateTime.ParseExact(date, "yyyyMMddHHmmss", CultureInfo.InvariantCulture);

                    var trustInterval = TimeSpan.FromMinutes(Convert.ToDouble(_configuration["auth:trust-interval"] ?? "5"));

                    if (DateTime.UtcNow > timestamp.Add(trustInterval))
                    {
                        _log.LogDebug("Auth failed: invalid timesatmp {0}, now {1}.", timestamp, DateTime.UtcNow);

                        return Task.FromResult(AuthenticateResult.Fail(new AuthenticationException(nameof(HttpStatusCode.Forbidden))));
                    }
                }

                var skey = _machinePseudoKeys.GetMachineConstant();
                using var hasher = new HMACSHA1(skey);
                var data = string.Join("\n", date, pkey);
                var hash = hasher.ComputeHash(Encoding.UTF8.GetBytes(data));

                if (WebEncoders.Base64UrlEncode(hash) != orighash && Convert.ToBase64String(hash) != orighash)
                {
                    _log.LogDebug("Auth failed: invalid token {0}, expect {1} or {2}.", orighash, WebEncoders.Base64UrlEncode(hash), Convert.ToBase64String(hash));

                    return Task.FromResult(AuthenticateResult.Fail(new AuthenticationException(nameof(HttpStatusCode.Forbidden))));
                }
            }
            else
            {
                _log.LogDebug("Auth failed: invalid auth header. Sheme: {0}, parameter: {1}.", Scheme.Name, header);

                return Task.FromResult(AuthenticateResult.Fail(new AuthenticationException(nameof(HttpStatusCode.Forbidden))));
            }
        }
        catch (Exception ex)
        {
            _log.LogError(ex, "auth error");

            return Task.FromResult(AuthenticateResult.Fail(new AuthenticationException(nameof(HttpStatusCode.InternalServerError))));
        }
        var identity = new ClaimsIdentity(Scheme.Name);

        _log.LogInformation("Auth success {0}", Scheme.Name);
        if (_httpContextAccessor?.HttpContext != null) _httpContextAccessor.HttpContext.User = new CustomClaimsPrincipal(new ClaimsIdentity(Scheme.Name), identity);

        Authenticate();
        return Task.FromResult(AuthenticateResult.Success(new AuthenticationTicket(Context.User, new AuthenticationProperties(), Scheme.Name)));
    }

    private void Authenticate()
    {
        var account = ASC.Core.Configuration.Constants.SystemAccounts.FirstOrDefault(a => a.ID == ASC.Core.Configuration.Constants.CoreSystem.ID);

        var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Sid, account.ID.ToString()),
                new Claim(ClaimTypes.Name, account.Name),
                new Claim(ClaimTypes.Role, Role.System)
            };

        _httpContextAccessor.HttpContext.User = new CustomClaimsPrincipal(new ClaimsIdentity(account, claims), account);
    }
}
