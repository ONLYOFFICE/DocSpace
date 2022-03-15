namespace ASC.Core;

[Scope]
public class AuthManager
{
    private readonly IUserService _userService;
    private readonly UserManager _userManager;
    private readonly UserFormatter _userFormatter;
    private readonly TenantManager _tenantManager;

    public AuthManager(IUserService service, UserManager userManager, UserFormatter userFormatter, TenantManager tenantManager)
    {
        _userService = service;
        _userManager = userManager;
        _userFormatter = userFormatter;
        _tenantManager = tenantManager;
    }


    public IUserAccount[] GetUserAccounts(Tenant tenant)
    {
        return _userManager.GetUsers(EmployeeStatus.Active).Select(u => ToAccount(tenant.Id, u)).ToArray();
    }

    public void SetUserPasswordHash(Guid userID, string passwordHash)
    {
        _userService.SetUserPasswordHash(_tenantManager.GetCurrentTenant().Id, userID, passwordHash);
    }

    public DateTime GetUserPasswordStamp(Guid userID)
    {
        return _userService.GetUserPasswordStamp(_tenantManager.GetCurrentTenant().Id, userID);
    }

    public IAccount GetAccountByID(int tenantId, Guid id)
    {
        var s = ASC.Core.Configuration.Constants.SystemAccounts.FirstOrDefault(a => a.ID == id);
        if (s != null)
        {
            return s;
        }

        var u = _userManager.GetUsers(id);

        return !Users.Constants.LostUser.Equals(u) && u.Status == EmployeeStatus.Active ? (IAccount)ToAccount(tenantId, u) : ASC.Core.Configuration.Constants.Guest;
    }

    private IUserAccount ToAccount(int tenantId, UserInfo u)
    {
        return new UserAccount(u, tenantId, _userFormatter);
    }
}
