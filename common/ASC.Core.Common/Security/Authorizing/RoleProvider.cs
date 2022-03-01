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

namespace ASC.Core.Security.Authorizing;

[Scope]
class RoleProvider : IRoleProvider
{
    //circ dep
    private readonly IServiceProvider _serviceProvider;
    public RoleProvider(IServiceProvider serviceProvider) => _serviceProvider = serviceProvider;

    public List<IRole> GetRoles(ISubject account)
    {
        var roles = new List<IRole>();
        if (!(account is ISystemAccount))
        {
            if (account is IRole)
            {
                roles = GetParentRoles(account.ID).ToList();
            }
            else if (account is IUserAccount)
            {
                roles = _serviceProvider.GetService<UserManager>()
                                   .GetUserGroups(account.ID, IncludeType.Distinct | IncludeType.InParent)
                                   .Select(g => (IRole)g)
                                   .ToList();
            }
        }

        return roles;
    }

    public bool IsSubjectInRole(ISubject account, IRole role)
    {
        return _serviceProvider.GetService<UserManager>().IsUserInGroup(account.ID, role.ID);
    }

    private List<IRole> GetParentRoles(Guid roleID)
    {
        var roles = new List<IRole>();
        var gi = _serviceProvider.GetService<UserManager>().GetGroupInfo(roleID);
        if (gi != null)
        {
            var parent = gi.Parent;
            while (parent != null)
            {
                roles.Add(parent);
                parent = parent.Parent;
            }
        }

        return roles;
    }
}
