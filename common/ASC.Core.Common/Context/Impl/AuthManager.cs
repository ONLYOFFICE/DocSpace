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
