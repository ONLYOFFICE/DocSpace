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

using AuthConstants = ASC.Common.Security.Authorizing.Constants;

namespace ASC.Core.Common.Security;

public class UserGroupObject : SecurityObject
{
    private readonly Guid _groupId;

    public UserGroupObject(ISubject user, Guid groupId)
    {
        SecurityId = user.ID;
        _groupId = groupId;
        ObjectType = typeof(UserGroupObject);
        FullId = $"{ObjectType.FullName}|{user.ID}|{_groupId}";
    }

    protected override IEnumerable<IRole> GetTargetRoles(IRoleProvider roleProvider)
    {
        if (_groupId == Users.Constants.GroupAdmin.ID)
        { 
            return new[] { AuthConstants.DocSpaceAdmin };
        }

        if (_groupId == Users.Constants.GroupUser.ID)
        {
            return new[] { AuthConstants.User };
        }

        if (_groupId == Users.Constants.GroupCollaborator.ID)
        {
            return new[] { AuthConstants.Collaborator };
        }

        return Array.Empty<IRole>();
    }

    protected override IRuleData GetRuleData()
    {
        return new Role(_groupId, "ruleData");
    }
}