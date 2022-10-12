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

namespace ASC.Core.Users;

public static class UserExtensions
{
    public static bool IsOwner(this UserInfo ui, Tenant tenant)
    {
        if (ui == null)
        {
            return false;
        }

        return tenant != null && tenant.OwnerId.Equals(ui.Id);
    }

    public static bool IsMe(this UserInfo ui, AuthContext authContext)
    {
        return ui != null && ui.Id == authContext.CurrentAccount.ID;
    }

    public static bool IsAdmin(this UserManager userManager, Guid id)
    {
        var ui = userManager.GetUsers(id);
        return userManager.IsAdmin(ui);
    }

    public static bool IsAdmin(this UserManager userManager, UserInfo ui)
    {
        return ui != null && userManager.IsUserInGroup(ui.Id, Constants.GroupAdmin.ID);
    }

    public static bool IsVisitor(this UserManager userManager, Guid id)
    {
        var ui = userManager.GetUsers(id);
        return userManager.IsVisitor(ui);
    }

    public static bool IsVisitor(this UserManager userManager, UserInfo ui)
    {
        return ui != null && userManager.IsUserInGroup(ui.Id, Constants.GroupUser.ID);
    }

    public static bool IsOutsider(this UserManager userManager, Guid id)
    {
        return userManager.IsVisitor(id) && id == Constants.OutsideUser.Id;
    }

    public static bool IsOutsider(this UserManager userManager, UserInfo ui)
    {
        return userManager.IsVisitor(ui) && ui.Id == Constants.OutsideUser.Id;
    }

    public static bool IsLDAP(this UserInfo ui)
    {
        if (ui == null)
        {
            return false;
        }

        return !string.IsNullOrEmpty(ui.Sid);
    }

    // ReSharper disable once InconsistentNaming
    public static bool IsSSO(this UserInfo ui)
    {
        if (ui == null)
        {
            return false;
        }

        return !string.IsNullOrEmpty(ui.SsoNameId);
    }

    private const string _extMobPhone = "extmobphone";
    private const string _mobPhone = "mobphone";
    private const string _extMail = "extmail";
    private const string _mail = "mail";

    public static void ConvertExternalContactsToOrdinary(this UserInfo ui)
    {
        var ldapUserContacts = ui.ContactsList;

        if (ui.ContactsList == null)
        {
            return;
        }

        var newContacts = new List<string>();

        for (int i = 0, m = ldapUserContacts.Count; i < m; i += 2)
        {
            if (i + 1 >= ldapUserContacts.Count)
            {
                continue;
            }

            var type = ldapUserContacts[i];
            var value = ldapUserContacts[i + 1];

            switch (type)
            {
                case _extMobPhone:
                    newContacts.Add(_mobPhone);
                    newContacts.Add(value);
                    break;
                case _extMail:
                    newContacts.Add(_mail);
                    newContacts.Add(value);
                    break;
                default:
                    newContacts.Add(type);
                    newContacts.Add(value);
                    break;
            }
        }

        ui.ContactsList = newContacts;
    }
}
