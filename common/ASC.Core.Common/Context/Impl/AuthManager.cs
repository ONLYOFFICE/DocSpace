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


    public async Task<IUserAccount[]> GetUserAccountsAsync(Tenant tenant)
    {
        return (await _userManager.GetUsersAsync(EmployeeStatus.Active)).Select(u => ToAccount(tenant.Id, u)).ToArray();
    }

    public async Task SetUserPasswordHashAsync(Guid userID, string passwordHash)
    {
        await _userService.SetUserPasswordHashAsync(await _tenantManager.GetCurrentTenantIdAsync(), userID, passwordHash);
    }

    public async Task<DateTime> GetUserPasswordStampAsync(Guid userID)
    {
        return await _userService.GetUserPasswordStampAsync(await _tenantManager.GetCurrentTenantIdAsync(), userID);
    }

    public async Task<IAccount> GetAccountByIDAsync(int tenantId, Guid id)
    {
        var s = Configuration.Constants.SystemAccounts.FirstOrDefault(a => a.ID == id);
        if (s != null)
        {
            return s;
        }

        var u = await _userManager.GetUsersAsync(id);

        return !Users.Constants.LostUser.Equals(u) && u.Status == EmployeeStatus.Active ? ToAccount(tenantId, u) : Configuration.Constants.Guest;
    }

    private IUserAccount ToAccount(int tenantId, UserInfo u)
    {
        return new UserAccount(u, tenantId, _userFormatter);
    }
}
