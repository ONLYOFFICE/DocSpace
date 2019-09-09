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
using System.Security;
using System.Security.Authentication;
using System.Security.Principal;
using System.Threading;
using System.Web;
using ASC.Common.Logging;
using ASC.Common.Security;
using ASC.Common.Security.Authentication;
using ASC.Common.Security.Authorizing;
using ASC.Core.Billing;
using ASC.Core.Common.Security;
using ASC.Core.Security.Authentication;
using ASC.Core.Tenants;
using ASC.Core.Users;
using ASC.Security.Cryptography;
using Microsoft.AspNetCore.Http;

namespace ASC.Core
{
    public class SecurityContext
    {
        private static readonly ILog log = LogManager.GetLogger("ASC.Core");


        public IAccount CurrentAccount
        {
            get => AuthContext.CurrentAccount;
        }

        public bool IsAuthenticated
        {
            get => AuthContext.IsAuthenticated;
        }

        public UserManager UserManager { get; }
        public AuthManager Authentication { get; }
        public AuthContext AuthContext { get; }
        public IHttpContextAccessor HttpContextAccessor { get; }

        public SecurityContext(
            IHttpContextAccessor httpContextAccessor,
            UserManager userManager,
            AuthManager authentication,
            AuthContext authContext
            )
        {
            UserManager = userManager;
            Authentication = authentication;
            AuthContext = authContext;
            HttpContextAccessor = httpContextAccessor;
        }


        public string AuthenticateMe(string login, string password)
        {
            if (login == null) throw new ArgumentNullException("login");
            if (password == null) throw new ArgumentNullException("password");

            var tenantid = CoreContext.TenantManager.GetCurrentTenant().TenantId;
            var u = UserManager.GetUsers(tenantid, login, Hasher.Base64Hash(password, HashAlg.SHA256));

            return AuthenticateMe(new UserAccount(u, tenantid));
        }

        public bool AuthenticateMe(string cookie)
        {
            if (!string.IsNullOrEmpty(cookie))
            {

                if (cookie.Equals("Bearer", StringComparison.InvariantCulture))
                {
                    var ipFrom = string.Empty;
                    var address = string.Empty;
                    if (HttpContextAccessor?.HttpContext != null)
                    {
                        var request = HttpContextAccessor?.HttpContext.Request;
                        ipFrom = "from " + (request.Headers["X-Forwarded-For"].ToString() ?? request.GetUserHostAddress());
                        address = "for " + request.GetUrlRewriter();
                    }
                    log.InfoFormat("Empty Bearer cookie: {0} {1}", ipFrom, address);
                }
                else if (CookieStorage.DecryptCookie(cookie, out var tenant, out var userid, out var login, out var password, out var indexTenant, out var expire, out var indexUser))
                {
                    if (tenant != CoreContext.TenantManager.GetCurrentTenant().TenantId)
                    {
                        return false;
                    }

                    var settingsTenant = TenantCookieSettings.GetForTenant(tenant);
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
                        if (userid != Guid.Empty)
                        {
                            var settingsUser = TenantCookieSettings.GetForUser(userid);
                            if (indexUser != settingsUser.Index)
                            {
                                return false;
                            }

                            AuthenticateMe(new UserAccount(new UserInfo { ID = userid }, tenant));
                        }
                        else
                        {
                            AuthenticateMe(login, password);
                        }
                        return true;
                    }
                    catch (InvalidCredentialException ice)
                    {
                        log.DebugFormat("{0}: cookie {1}, tenant {2}, userid {3}, login {4}, pass {5}",
                                        ice.Message, cookie, tenant, userid, login, password);
                    }
                    catch (SecurityException se)
                    {
                        log.DebugFormat("{0}: cookie {1}, tenant {2}, userid {3}, login {4}, pass {5}",
                                        se.Message, cookie, tenant, userid, login, password);
                    }
                    catch (Exception err)
                    {
                        log.ErrorFormat("Authenticate error: cookie {0}, tenant {1}, userid {2}, login {3}, pass {4}: {5}",
                                        cookie, tenant, userid, login, password, err);
                    }
                }
                else
                {
                    var ipFrom = string.Empty;
                    var address = string.Empty;
                    if (HttpContextAccessor?.HttpContext != null)
                    {
                        var request = HttpContextAccessor?.HttpContext.Request;
                        address = "for " + request.GetUrlRewriter();
                        ipFrom = "from " + (request.Headers["X-Forwarded-For"].ToString() ?? request.GetUserHostAddress());
                    }
                    log.WarnFormat("Can not decrypt cookie: {0} {1} {2}", cookie, ipFrom, address);
                }
            }
            return false;
        }

        public string AuthenticateMe(IAccount account)
        {
            if (account == null || account.Equals(Configuration.Constants.Guest)) throw new InvalidCredentialException("account");

            var roles = new List<string> { Role.Everyone };
            string cookie = null;


            if (account is ISystemAccount && account.ID == Configuration.Constants.CoreSystem.ID)
            {
                roles.Add(Role.System);
            }

            if (account is IUserAccount)
            {
                var tenant = CoreContext.TenantManager.GetCurrentTenant();

                var u = UserManager.GetUsers(tenant.TenantId, account.ID);

                if (u.ID == Users.Constants.LostUser.ID)
                {
                    throw new InvalidCredentialException("Invalid username or password.");
                }
                if (u.Status != EmployeeStatus.Active)
                {
                    throw new SecurityException("Account disabled.");
                }

                // for LDAP users only
                if (u.Sid != null)
                {
                    if (!CoreContext.TenantManager.GetTenantQuota(tenant.TenantId).Ldap)
                    {
                        throw new BillingException("Your tariff plan does not support this option.", "Ldap");
                    }
                }
                if (UserManager.IsUserInGroup(tenant, u.ID, Users.Constants.GroupAdmin.ID))
                {
                    roles.Add(Role.Administrators);
                }
                roles.Add(Role.Users);

                account = new UserAccount(u, CoreContext.TenantManager.GetCurrentTenant().TenantId);
                cookie = CookieStorage.EncryptCookie(CoreContext.TenantManager.GetCurrentTenant().TenantId, account.ID);
            }

            AuthContext.Principal = new GenericPrincipal(account, roles.ToArray());

            return cookie;
        }

        public string AuthenticateMe(int tenantId, Guid userId)
        {
            return AuthenticateMe(Authentication.GetAccountByID(tenantId, userId));
        }

        public void Logout()
        {
            AuthContext.Principal = null;
        }

        public void SetUserPassword(int tenantId, Guid userID, string password)
        {
            Authentication.SetUserPassword(tenantId, userID, password);
        }
    }

    public class PermissionContext
    {
        public IPermissionResolver PermissionResolver { get; private set; }
        public AuthContext AuthContext { get; }

        public PermissionContext(IPermissionResolver permissionResolver, AuthContext authContext)
        {
            PermissionResolver = permissionResolver;
            AuthContext = authContext;
        }

        public bool CheckPermissions(Tenant tenant, params IAction[] actions)
        {
            return PermissionResolver.Check(tenant, AuthContext.CurrentAccount, actions);
        }

        public bool CheckPermissions(Tenant tenant, ISecurityObject securityObject, params IAction[] actions)
        {
            return CheckPermissions(tenant, securityObject, null, actions);
        }

        public bool CheckPermissions(Tenant tenant, ISecurityObjectId objectId, ISecurityObjectProvider securityObjProvider, params IAction[] actions)
        {
            return PermissionResolver.Check(tenant, AuthContext.CurrentAccount, objectId, securityObjProvider, actions);
        }

        public void DemandPermissions(Tenant tenant, params IAction[] actions)
        {
            PermissionResolver.Demand(tenant, AuthContext.CurrentAccount, actions);
        }

        public void DemandPermissions(Tenant tenant, ISecurityObject securityObject, params IAction[] actions)
        {
            DemandPermissions(tenant, securityObject, null, actions);
        }

        public void DemandPermissions(Tenant tenant, ISecurityObjectId objectId, ISecurityObjectProvider securityObjProvider, params IAction[] actions)
        {
            PermissionResolver.Demand(tenant, AuthContext.CurrentAccount, objectId, securityObjProvider, actions);
        }
    }

    public class AuthContext
    {
        public AuthContext(IHttpContextAccessor httpContextAccessor)
        {
            HttpContextAccessor = httpContextAccessor;
        }

        public IAccount CurrentAccount
        {
            get { return Principal?.Identity is IAccount ? (IAccount)Principal.Identity : Configuration.Constants.Guest; }
        }

        public bool IsAuthenticated
        {
            get { return CurrentAccount.IsAuthenticated; }
        }

        public IHttpContextAccessor HttpContextAccessor { get; }

        internal IPrincipal Principal
        {
            get => Thread.CurrentPrincipal ?? HttpContextAccessor?.HttpContext?.User;
            set
            {
                var principal = new CustomClaimsPrincipal(value);
                Thread.CurrentPrincipal = principal;
                if (HttpContextAccessor?.HttpContext != null) HttpContextAccessor.HttpContext.User = principal;
            }
        }
    }
}