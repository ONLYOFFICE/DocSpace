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

namespace ASC.ActiveDirectory.ComplexOperations.Data;

[Scope]
public class LdapChangeCollection : List<LdapChange>
{
    private readonly UserFormatter _userFormatter;
    public LdapChangeCollection(UserFormatter userFormatter)
    {
        _userFormatter = userFormatter;
    }

    #region User

    public void SetSkipUserChange(UserInfo user)
    {
        var change = new LdapChange(user.Sid,
            _userFormatter.GetUserName(user, DisplayUserNameFormat.Default),
            user.Email,
            LdapChangeType.User, LdapChangeAction.Skip);

        Add(change);
    }

    public void SetSaveAsPortalUserChange(UserInfo user)
    {
        var fieldChanges = new List<LdapItemChange>
            {
                new LdapItemChange(LdapItemChangeKey.Sid, user.Sid, null)
            };

        var change = new LdapChange(user.Sid,
            _userFormatter.GetUserName(user, DisplayUserNameFormat.Default),
            user.Email, LdapChangeType.User, LdapChangeAction.SaveAsPortal, fieldChanges);

        Add(change);
    }

    public void SetNoneUserChange(UserInfo user)
    {
        var change = new LdapChange(user.Sid,
                    _userFormatter.GetUserName(user, DisplayUserNameFormat.Default), user.Email,
                    LdapChangeType.User, LdapChangeAction.None);

        Add(change);
    }

    public void SetUpdateUserChange(UserInfo beforeUserInfo, UserInfo afterUserInfo, ILogger log = null)
    {
        var fieldChanges =
                        LdapUserMapping.Fields.Select(field => GetPropChange(field, beforeUserInfo, afterUserInfo, log))
                            .Where(pch => pch != null)
                            .ToList();

        var change = new LdapChange(beforeUserInfo.Sid,
            _userFormatter.GetUserName(afterUserInfo, DisplayUserNameFormat.Default), afterUserInfo.Email,
            LdapChangeType.User, LdapChangeAction.Update, fieldChanges);

        Add(change);
    }

    public void SetAddUserChange(UserInfo user, ILogger log = null)
    {
        var fieldChanges =
                    LdapUserMapping.Fields.Select(field => GetPropChange(field, after: user, log: log))
                        .Where(pch => pch != null)
                        .ToList();

        var change = new LdapChange(user.Sid,
            _userFormatter.GetUserName(user, DisplayUserNameFormat.Default), user.Email,
            LdapChangeType.User, LdapChangeAction.Add, fieldChanges);

        Add(change);
    }

    public void SetRemoveUserChange(UserInfo user)
    {
        var change = new LdapChange(user.Sid,
                            _userFormatter.GetUserName(user, DisplayUserNameFormat.Default), user.Email,
                            LdapChangeType.User, LdapChangeAction.Remove);

        Add(change);
    }
    #endregion

    #region Group

    public void SetAddGroupChange(GroupInfo group, ILogger log = null)
    {
        var fieldChanges = new List<LdapItemChange>
                                    {
                                        new LdapItemChange(LdapItemChangeKey.Name, null, group.Name),
                                        new LdapItemChange(LdapItemChangeKey.Sid, null, group.Sid)
                                    };

        var change = new LdapChange(group.Sid, group.Name,
            LdapChangeType.Group, LdapChangeAction.Add, fieldChanges);

        Add(change);
    }

    public void SetAddGroupMembersChange(GroupInfo group,
        List<UserInfo> members)
    {
        var fieldChanges =
            members.Select(
                member =>
                    new LdapItemChange(LdapItemChangeKey.Member, null,
                        _userFormatter.GetUserName(member, DisplayUserNameFormat.Default))).ToList();

        var change = new LdapChange(group.Sid, group.Name,
            LdapChangeType.Group, LdapChangeAction.AddMember, fieldChanges);

        Add(change);
    }

    public void SetSkipGroupChange(GroupInfo group)
    {
        var change = new LdapChange(group.Sid, group.Name, LdapChangeType.Group,
            LdapChangeAction.Skip);

        Add(change);
    }

    public void SetUpdateGroupChange(GroupInfo group)
    {
        var fieldChanges = new List<LdapItemChange>
                                {
                                    new LdapItemChange(LdapItemChangeKey.Name, group.Name, group.Name)
                                };

        var change = new LdapChange(group.Sid, group.Name,
            LdapChangeType.Group, LdapChangeAction.Update, fieldChanges);

        Add(change);
    }

    public void SetRemoveGroupChange(GroupInfo group, ILogger log = null)
    {
        var change = new LdapChange(group.Sid, group.Name,
                        LdapChangeType.Group, LdapChangeAction.Remove);

        Add(change);
    }

    public void SetRemoveGroupMembersChange(GroupInfo group,
        List<UserInfo> members)
    {
        var fieldChanges =
            members.Select(
                member =>
                    new LdapItemChange(LdapItemChangeKey.Member, null,
                        _userFormatter.GetUserName(member, DisplayUserNameFormat.Default))).ToList();

        var change = new LdapChange(group.Sid, group.Name,
            LdapChangeType.Group, LdapChangeAction.RemoveMember, fieldChanges);

        Add(change);
    }

    #endregion

    private static LdapItemChange GetPropChange(string propName, UserInfo before = null, UserInfo after = null, ILogger log = null)
    {
        try
        {
            var valueSrc = before != null
                ? before.GetType().GetProperty(propName).GetValue(before, null) as string
                : "";
            var valueDst = after != null
                ? after.GetType().GetProperty(propName).GetValue(before, null) as string
                : "";

            LdapItemChangeKey key;
            if (!Enum.TryParse(propName, out key))
            {
                throw new InvalidEnumArgumentException(propName);
            }

            var change = new LdapItemChange(key, valueSrc, valueDst);

            return change;
        }
        catch (Exception ex)
        {
            if (log != null)
            {
                log.ErrorCanNotGetSidProperty(propName, ex);
            }
        }

        return null;
    }
}