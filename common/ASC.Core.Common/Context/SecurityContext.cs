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
using System.Security;
using System.Security.Authentication;
using System.Security.Claims;
using System.Threading;
using System.Web;

using ASC.Common;
using ASC.Common.Logging;
using ASC.Common.Security;
using ASC.Common.Security.Authentication;
using ASC.Common.Security.Authorizing;
using ASC.Core.Billing;
using ASC.Core.Common.Security;
using ASC.Core.Security.Authentication;
using ASC.Core.Tenants;
using ASC.Core.Users;

using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;

namespace ASC.Core
{
    [Scope]
    public class SecurityContext
    {
        public IAccount CurrentAccount => _authContext.CurrentAccount;
        public bool IsAuthenticated => _authContext.IsAuthenticated;

        private readonly UserManager _userManager;
        private readonly AuthManager _authentication;
        private readonly AuthContext _authContext;
        private readonly TenantManager _tenantManager;
        private readonly UserFormatter _userFormatter;
        private readonly CookieStorage _cookieStorage;
        private readonly TenantCookieSettingsHelper _tenantCookieSettingsHelper;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly ILog _logger;

        public SecurityContext(
            UserManager userManager,
            AuthManager authentication,
            AuthContext authContext,
            TenantManager tenantManager,
            UserFormatter userFormatter,
            CookieStorage cookieStorage,
            TenantCookieSettingsHelper tenantCookieSettingsHelper,
            IOptionsMonitor<ILog> options
            )
        {
            _logger = options.CurrentValue;
            _userManager = userManager;
            _authentication = authentication;
            _authContext = authContext;
            _tenantManager = tenantManager;
            _userFormatter = userFormatter;
            _cookieStorage = cookieStorage;
            _tenantCookieSettingsHelper = tenantCookieSettingsHelper;
        }

        public SecurityContext(
            IHttpContextAccessor httpContextAccessor,
            UserManager userManager,
            AuthManager authentication,
            AuthContext authContext,
            TenantManager tenantManager,
            UserFormatter userFormatter,
            CookieStorage cookieStorage,
            TenantCookieSettingsHelper tenantCookieSettingsHelper,
            IOptionsMonitor<ILog> options
            ) : this(userManager, authentication, authContext, tenantManager, userFormatter, cookieStorage, tenantCookieSettingsHelper, options)
        {
            _httpContextAccessor = httpContextAccessor;
        }

        public string AuthenticateMe(string login, string passwordHash)
        {
            if (login == null)
            {
                throw new ArgumentNullException(nameof(login));
            }

            if (passwordHash == null)
            {
                throw new ArgumentNullException(nameof(passwordHash));
            }

            var tenantid = _tenantManager.GetCurrentTenant().Id;
            var u = _userManager.GetUsersByPasswordHash(tenantid, login, passwordHash);

            return AuthenticateMe(new UserAccount(u, tenantid, _userFormatter));
        }

        public bool AuthenticateMe(string cookie)
        {
            if (!string.IsNullOrEmpty(cookie))
            {
                if (cookie.Equals("Bearer", StringComparison.InvariantCulture))
                {
                    var ipFrom = string.Empty;
                    var address = string.Empty;

                    if (_httpContextAccessor?.HttpContext != null)
                    {
                        var request = _httpContextAccessor?.HttpContext.Request;
                        ipFrom = "from " + (request.Headers["X-Forwarded-For"].ToString() ?? request.GetUserHostAddress());
                        address = "for " + request.GetUrlRewriter();
                    }

                    _logger.InfoFormat("Empty Bearer cookie: {0} {1}", ipFrom, address);
                }
                else if (_cookieStorage.DecryptCookie(cookie, out var tenant, out var userid, out var indexTenant, out var expire, out var indexUser))
                {
                    if (tenant != _tenantManager.GetCurrentTenant().Id)
                    {
                        return false;
                    }

                    var settingsTenant = _tenantCookieSettingsHelper.GetForTenant(tenant);
                    if (indexTenant != settingsTenant.Index)
                    {
                        return false;
                    }

                    if (expire != DateTime.MaxValue && expire < DateTime.UtcNow)
                    {
                        return false;
                    }

                    try
                    {
                        var settingsUser = _tenantCookieSettingsHelper.GetForUser(userid);
                        if (indexUser != settingsUser.Index)
                        {
                            return false;
                        }

                        AuthenticateMeWithoutCookie(new UserAccount(new UserInfo { Id = userid }, tenant, _userFormatter));

                        return true;
                    }
                    catch (InvalidCredentialException ice)
                    {
                        _logger.DebugFormat("{0}: cookie {1}, tenant {2}, userid {3}", ice.Message, cookie, tenant, userid);
                    }
                    catch (SecurityException se)
                    {
                        _logger.DebugFormat("{0}: cookie {1}, tenant {2}, userid {3}", se.Message, cookie, tenant, userid);
                    }
                    catch (Exception err)
                    {
                        _logger.ErrorFormat("Authenticate error: cookie {0}, tenant {1}, userid {2}, : {3}", cookie, tenant, userid, err);
                    }
                }
                else
                {
                    var ipFrom = string.Empty;
                    var address = string.Empty;
                    if (_httpContextAccessor?.HttpContext != null)
                    {
                        var request = _httpContextAccessor?.HttpContext.Request;
                        address = "for " + request.GetUrlRewriter();
                        ipFrom = "from " + (request.Headers["X-Forwarded-For"].ToString() ?? request.GetUserHostAddress());
                    }

                    _logger.WarnFormat("Can not decrypt cookie: {0} {1} {2}", cookie, ipFrom, address);
                }
            }

            return false;
        }

        public string AuthenticateMe(IAccount account, List<Claim> additionalClaims = null)
        {
            AuthenticateMeWithoutCookie(account, additionalClaims);
            
            string cookie = null;

            if (account is IUserAccount)
            {
                cookie = _cookieStorage.EncryptCookie(_tenantManager.GetCurrentTenant().Id, account.ID);
            }

            return cookie;
        }

        public void AuthenticateMeWithoutCookie(IAccount account, List<Claim> additionalClaims = null)
        {
            if (account == null || account.Equals(Configuration.Constants.Guest))
            {
                throw new InvalidCredentialException("account");
            }

            var roles = new List<string> { Role.Everyone };

            if (account is ISystemAccount && account.ID == Configuration.Constants.CoreSystem.ID)
            {
                roles.Add(Role.System);
            }

            if (account is IUserAccount)
            {
                var tenant = _tenantManager.GetCurrentTenant();

                var u = _userManager.GetUsers(account.ID);

                if (u.Id == Users.Constants.LostUser.Id)
                {
                    throw new InvalidCredentialException("Invalid username or password.");
                }

                if (u.Status != EmployeeStatus.Active)
                {
                    throw new SecurityException("Account disabled.");
                }

                // for LDAP users only
                if (u.Sid != null && !_tenantManager.GetTenantQuota(tenant.Id).Ldap)
                {
                    throw new BillingException("Your tariff plan does not support this option.", "Ldap");
                }

                if (_userManager.IsUserInGroup(u.Id, Users.Constants.GroupAdmin.ID))
                {
                    roles.Add(Role.Administrators);
                }

                roles.Add(Role.Users);

                account = new UserAccount(u, _tenantManager.GetCurrentTenant().Id, _userFormatter);
            }

            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Sid, account.ID.ToString()),
                new Claim(ClaimTypes.Name, account.Name)
            };

            claims.AddRange(roles.Select(r => new Claim(ClaimTypes.Role, r)));

            if (additionalClaims != null)
            {
                claims.AddRange(additionalClaims);
            }

            _authContext.Principal = new CustomClaimsPrincipal(new ClaimsIdentity(account, claims), account);
        }

        public string AuthenticateMe(Guid userId, List<Claim> additionalClaims = null)
        {
            var account = _authentication.GetAccountByID(_tenantManager.GetCurrentTenant().Id, userId);

            return AuthenticateMe(account, additionalClaims);
        }

        public void AuthenticateMeWithoutCookie(Guid userId, List<Claim> additionalClaims = null)
        {
            var account = _authentication.GetAccountByID(_tenantManager.GetCurrentTenant().Id, userId);

            AuthenticateMeWithoutCookie(account, additionalClaims);
        }

        public void Logout() => _authContext.Logout();

        public void SetUserPasswordHash(Guid userID, string passwordHash)
        {
            var tenantid = _tenantManager.GetCurrentTenant().Id;
            var u = _userManager.GetUsersByPasswordHash(tenantid, userID.ToString(), passwordHash);

            if (!Equals(u, Users.Constants.LostUser))
            {
                throw new PasswordException("A new password must be used");
            }

            _authentication.SetUserPasswordHash(userID, passwordHash);
        }

        public class PasswordException : Exception
        {
            public PasswordException(string message) : base(message) { }
        }
    }

    [Scope]
    public class PermissionContext
    {
        public IPermissionResolver PermissionResolver { get; set; }

        private readonly AuthContext _authContext;

        public PermissionContext(IPermissionResolver permissionResolver, AuthContext authContext)
        {
            PermissionResolver = permissionResolver;
            _authContext = authContext;
        }

        public bool CheckPermissions(params IAction[] actions)
        {
            return PermissionResolver.Check(_authContext.CurrentAccount, actions);
        }

        public bool CheckPermissions(ISecurityObject securityObject, params IAction[] actions)
        {
            return CheckPermissions(securityObject, null, actions);
        }

        public bool CheckPermissions(ISecurityObjectId objectId, ISecurityObjectProvider securityObjProvider, params IAction[] actions)
        {
            return PermissionResolver.Check(_authContext.CurrentAccount, objectId, securityObjProvider, actions);
        }

        public void DemandPermissions(params IAction[] actions)
        {
            PermissionResolver.Demand(_authContext.CurrentAccount, actions);
        }

        public void DemandPermissions(ISecurityObject securityObject, params IAction[] actions)
        {
            DemandPermissions(securityObject, null, actions);
        }

        public void DemandPermissions(ISecurityObjectId objectId, ISecurityObjectProvider securityObjProvider, params IAction[] actions)
        {
            PermissionResolver.Demand(_authContext.CurrentAccount, objectId, securityObjProvider, actions);
        }
    }

    [Scope]
    public class AuthContext
    {
        public IAccount CurrentAccount =>
           Principal?.Identity is IAccount ? (IAccount)Principal.Identity : Configuration.Constants.Guest;
        public bool IsAuthenticated => CurrentAccount.IsAuthenticated;
        internal ClaimsPrincipal Principal
        {
            get => Thread.CurrentPrincipal as ClaimsPrincipal ?? _httpContextAccessor?.HttpContext?.User;
            set
            {
                Thread.CurrentPrincipal = value;
                if (_httpContextAccessor?.HttpContext != null) _httpContextAccessor.HttpContext.User = value;
            }
        }

        private readonly IHttpContextAccessor _httpContextAccessor;

        public AuthContext() { }

        public AuthContext(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }

        public void Logout() => Principal = null;
    }
}