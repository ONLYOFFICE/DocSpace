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

using SecurityContext = ASC.Core.SecurityContext;

namespace ASC.Api.Core.Auth;

[Scope]
public class CookieAuthHandler : AuthenticationHandler<AuthenticationSchemeOptions>
{
    private readonly SecurityContext _securityContext;
    private readonly CookiesManager _cookiesManager;
    private readonly IHttpContextAccessor _httpContextAccessor;

    public CookieAuthHandler(
        IOptionsMonitor<AuthenticationSchemeOptions> options,
        ILoggerFactory logger,
        UrlEncoder encoder,
        ISystemClock clock)
        : base(options, logger, encoder, clock) { }

    public CookieAuthHandler(
        IOptionsMonitor<AuthenticationSchemeOptions> options,
        ILoggerFactory logger,
        UrlEncoder encoder,
        ISystemClock clock,
        SecurityContext securityContext,
        CookiesManager cookiesManager,
        IHttpContextAccessor httpContextAccessor)
        : this(options, logger, encoder, clock)
    {
        _securityContext = securityContext;
        _cookiesManager = cookiesManager;
        _httpContextAccessor = httpContextAccessor;
    }

    protected override async Task<AuthenticateResult> HandleAuthenticateAsync()
    {
        try
        {
            var authorization = _httpContextAccessor.HttpContext.Request.Cookies[_cookiesManager.GetAscCookiesName()] ?? _httpContextAccessor.HttpContext.Request.Headers["Authorization"].ToString();

            if (string.IsNullOrEmpty(authorization))
            {
                throw new AuthenticationException(nameof(HttpStatusCode.Unauthorized));
            }

            authorization = authorization.Trim();

            if (0 <= authorization.IndexOf("Bearer", 0))
            {
                authorization = authorization.Substring("Bearer ".Length);
            }

            if (!(await _securityContext.AuthenticateMe(authorization)))
            {
                throw new AuthenticationException(nameof(HttpStatusCode.Unauthorized));
            }

        }
        catch (Exception)
        {
            return AuthenticateResult.Fail(new AuthenticationException(nameof(HttpStatusCode.Unauthorized)));
        }
        finally
        {
            if (!_securityContext.IsAuthenticated)
            {
                _securityContext.Logout();
                _cookiesManager.ClearCookies(CookiesType.AuthKey);
                _cookiesManager.ClearCookies(CookiesType.SocketIO);
            }
        }

        return AuthenticateResult.Success(new AuthenticationTicket(Context.User, Scheme.Name));
    }
}