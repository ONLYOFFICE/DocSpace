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

namespace ASC.Core;

[Scope]
public class SecurityContext
{
    private readonly ILogger<SecurityContext> _logger;
    private readonly DbLoginEventsManager _dbLoginEventsManager;

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

    public SecurityContext(
        UserManager userManager,
        AuthManager authentication,
        AuthContext authContext,
        TenantManager tenantManager,
        UserFormatter userFormatter,
        CookieStorage cookieStorage,
        TenantCookieSettingsHelper tenantCookieSettingsHelper,
        ILogger<SecurityContext> logger,
        DbLoginEventsManager dbLoginEventsManager
        )
    {
        _logger = logger;
        _dbLoginEventsManager = dbLoginEventsManager;
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
        ILogger<SecurityContext> logger,
        DbLoginEventsManager dbLoginEventsManager
        ) : this(userManager, authentication, authContext, tenantManager, userFormatter, cookieStorage, tenantCookieSettingsHelper, logger, dbLoginEventsManager)
    {
        _httpContextAccessor = httpContextAccessor;
    }


    public async Task<string> AuthenticateMeAsync(string login, string passwordHash, Func<Task<int>> funcLoginEvent = null)
    {
        ArgumentNullException.ThrowIfNull(login);
        ArgumentNullException.ThrowIfNull(passwordHash);

        var tenantid = await _tenantManager.GetCurrentTenantIdAsync();
        var u = await _userManager.GetUsersByPasswordHashAsync(tenantid, login, passwordHash);

        return await AuthenticateMeAsync(new UserAccount(u, tenantid, _userFormatter), funcLoginEvent);
    }

    public async Task<bool> AuthenticateMe(string cookie)
    {
        if (string.IsNullOrEmpty(cookie)) return false;

        if (!_cookieStorage.DecryptCookie(cookie, out var tenant, out var userid, out var indexTenant, out var expire, out var indexUser, out var loginEventId))
        {
            if (cookie.Equals("Bearer", StringComparison.InvariantCulture))
            {
                var ipFrom = string.Empty;
                var address = string.Empty;
                if (_httpContextAccessor?.HttpContext != null)
                {
                    var request = _httpContextAccessor?.HttpContext.Request;

                    ArgumentNullException.ThrowIfNull(request);

                    ipFrom = "from " + _httpContextAccessor?.HttpContext.Connection.RemoteIpAddress;
                    address = "for " + request.Url();
                }
                _logger.InformationEmptyBearer(ipFrom, address);
            }
            else
            {
                var ipFrom = string.Empty;
                var address = string.Empty;
                if (_httpContextAccessor?.HttpContext != null)
                {
                    var request = _httpContextAccessor?.HttpContext.Request;

                    ArgumentNullException.ThrowIfNull(request);

                    address = "for " + request.Url();
                    ipFrom = "from " + _httpContextAccessor?.HttpContext.Connection.RemoteIpAddress;
                }

                _logger.WarningCanNotDecrypt(cookie, ipFrom, address);
            }

            return false;
        }

        if (tenant != await _tenantManager.GetCurrentTenantIdAsync())
        {
            return false;
        }

        var settingsTenant = await _tenantCookieSettingsHelper.GetForTenantAsync(tenant);

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
            var settingsUser = await _tenantCookieSettingsHelper.GetForUserAsync(userid);
            if (indexUser != settingsUser.Index)
            {
                return false;
            }

            if (loginEventId != 0)
            {
                var loginEventById = await _dbLoginEventsManager.GetByIdAsync(loginEventId);
                if (loginEventById == null || !loginEventById.Active)
                {
                    return false;
                }
            }

            await AuthenticateMeWithoutCookieAsync(new UserAccount(new UserInfo { Id = userid }, tenant, _userFormatter));
            return true;
        }
        catch (InvalidCredentialException ice)
        {
            _logger.AuthenticateDebug(cookie, tenant, userid, ice);
        }
        catch (SecurityException se)
        {
            _logger.AuthenticateDebug(cookie, tenant, userid, se);
        }
        catch (Exception err)
        {
            _logger.AuthenticateError(cookie, tenant, userid, err);
        }


        return false;
    }

    public async Task<string> AuthenticateMeAsync(Guid userId, Func<Task<int>> funcLoginEvent = null, List<Claim> additionalClaims = null)
    {
        var account = await _authentication.GetAccountByIDAsync(await _tenantManager.GetCurrentTenantIdAsync(), userId);
        return await AuthenticateMeAsync(account, funcLoginEvent, additionalClaims);
    }

    public async Task<string> AuthenticateMeAsync(IAccount account, Func<Task<int>> funcLoginEvent = null, List<Claim> additionalClaims = null)
    {
        await AuthenticateMeWithoutCookieAsync(account, additionalClaims);

        string cookie = null;

        if (account is IUserAccount)
        {
            var loginEventId = 0;
            if (funcLoginEvent != null)
            {
                loginEventId = await funcLoginEvent();
            }

            cookie = await _cookieStorage.EncryptCookieAsync(await _tenantManager.GetCurrentTenantIdAsync(), account.ID, loginEventId);
        }

        return cookie;
    }

    public async Task AuthenticateMeWithoutCookieAsync(IAccount account, List<Claim> additionalClaims = null)
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
            var tenant = await _tenantManager.GetCurrentTenantAsync();

            var u = await _userManager.GetUsersAsync(account.ID);

            if (u.Id == Users.Constants.LostUser.Id)
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
                if (!(await _tenantManager.GetTenantQuotaAsync(tenant.Id)).Ldap)
                {
                    throw new BillingException("Your tariff plan does not support this option.", "Ldap");
                }
            }

            if (await _userManager.IsUserInGroupAsync(u.Id, Users.Constants.GroupAdmin.ID))
            {
                roles.Add(Role.DocSpaceAdministrators);
            }

            roles.Add(Role.RoomAdministrators);

            account = new UserAccount(u, await _tenantManager.GetCurrentTenantIdAsync(), _userFormatter);
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

    public async Task AuthenticateMeWithoutCookieAsync(Guid userId, List<Claim> additionalClaims = null)
    {
        var account = await _authentication.GetAccountByIDAsync(await _tenantManager.GetCurrentTenantIdAsync(), userId);

        await AuthenticateMeWithoutCookieAsync(account, additionalClaims);
    }

    public void Logout()
    {
        _authContext.Logout();
    }

    public async Task SetUserPasswordHashAsync(Guid userID, string passwordHash)
    {
        var tenantid = await _tenantManager.GetCurrentTenantIdAsync();
        var u = await _userManager.GetUsersByPasswordHashAsync(tenantid, userID.ToString(), passwordHash);
        if (!Equals(u, Users.Constants.LostUser))
        {
            throw new PasswordException("A new password must be used");
        }

        await _authentication.SetUserPasswordHashAsync(userID, passwordHash);
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
    private AuthContext AuthContext { get; }

    public PermissionContext(IPermissionResolver permissionResolver, AuthContext authContext)
    {
        PermissionResolver = permissionResolver;
        AuthContext = authContext;
    }

    public async Task<bool> CheckPermissionsAsync(params IAction[] actions)
    {
        return await PermissionResolver.CheckAsync(AuthContext.CurrentAccount, actions);
    }

    public async Task<bool> CheckPermissionsAsync(ISecurityObject securityObject, params IAction[] actions)
    {
        return await CheckPermissionsAsync(securityObject, null, actions);
    }

    public async Task<bool> CheckPermissionsAsync(IAccount account, ISecurityObject securityObject, params IAction[] actions)
    {
        return await PermissionResolver.CheckAsync(account, securityObject, null, actions);
    }

    public async Task<bool> CheckPermissionsAsync(ISecurityObjectId objectId, ISecurityObjectProvider securityObjProvider, params IAction[] actions)
    {
        return await PermissionResolver.CheckAsync(AuthContext.CurrentAccount, objectId, securityObjProvider, actions);
    }

    public async Task DemandPermissionsAsync(params IAction[] actions)
    {
        await PermissionResolver.DemandAsync(AuthContext.CurrentAccount, actions);
    }

    public async Task DemandPermissionsAsync(ISecurityObject securityObject, params IAction[] actions)
    {
        await DemandPermissionsAsync(securityObject, null, actions);
    }

    public async Task DemandPermissionsAsync(ISecurityObjectId objectId, ISecurityObjectProvider securityObjProvider, params IAction[] actions)
    {
        await PermissionResolver.DemandAsync(AuthContext.CurrentAccount, objectId, securityObjProvider, actions);
    }
}

[Scope]
public class AuthContext
{
    private IHttpContextAccessor HttpContextAccessor { get; }

    public AuthContext()
    {

    }

    public AuthContext(IHttpContextAccessor httpContextAccessor)
    {
        HttpContextAccessor = httpContextAccessor;
    }

    public IAccount CurrentAccount => Principal?.Identity is IAccount ? (IAccount)Principal.Identity : Configuration.Constants.Guest;

    public bool IsAuthenticated => CurrentAccount.IsAuthenticated;

    public void Logout()
    {
        Principal = null;
    }

    internal ClaimsPrincipal Principal
    {
        get => CustomSynchronizationContext.CurrentContext.CurrentPrincipal as ClaimsPrincipal ?? HttpContextAccessor?.HttpContext?.User;
        set
        {
            CustomSynchronizationContext.CurrentContext.CurrentPrincipal = value;

            if (HttpContextAccessor?.HttpContext != null)
            {
                HttpContextAccessor.HttpContext.User = value;
            }
        }
    }
}
