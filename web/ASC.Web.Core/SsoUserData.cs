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


using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Text.Json;

using ASC.Core;
using ASC.Core.Tenants;
using ASC.Core.Users;

namespace ASC.Web.Studio.UserControls.Management.SingleSignOnSettings;

[Serializable]
public class SsoUserData
{
    private readonly UserManager _userManager;
    private readonly TenantUtil _tenantUtil;

    public SsoUserData(
        UserManager userManager,
        TenantUtil tenantUtil)
    {
        _userManager = userManager;
        _tenantUtil = tenantUtil;
    }

    private const int MAX_NUMBER_OF_SYMBOLS = 64;

    [DataMember(Name = "nameID")]
    public string NameId { get; set; }

    [DataMember(Name = "sessionID")]
    public string SessionId { get; set; }

    [DataMember(Name = "email")]
    public string Email { get; set; }

    [DataMember(Name = "firstName")]
    public string FirstName { get; set; }

    [DataMember(Name = "lastName")]
    public string LastName { get; set; }

    [DataMember(Name = "location")]
    public string Location { get; set; }

    [DataMember(Name = "phone")]
    public string Phone { get; set; }

    [DataMember(Name = "title")]
    public string Title { get; set; }

    public override string ToString()
    {
        return JsonSerializer.Serialize(this);
    }

    private const string MOB_PHONE = "mobphone";
    private const string EXT_MOB_PHONE = "extmobphone";

    public UserInfo ToUserInfo(bool checkExistance = false)
    {
        var firstName = TrimToLimit(FirstName);
        var lastName = TrimToLimit(LastName);

        if (string.IsNullOrEmpty(Email) || string.IsNullOrEmpty(firstName) || string.IsNullOrEmpty(lastName))
        {
            return Constants.LostUser;
        }

        var userInfo = Constants.LostUser;

        if (checkExistance)
        {
            userInfo = _userManager.GetSsoUserByNameId(NameId);

            if (Equals(userInfo, Constants.LostUser))
            {
                userInfo = _userManager.GetUserByEmail(Email);
            }
        }

        if (Equals(userInfo, Constants.LostUser))
        {
            userInfo = new UserInfo
            {
                Email = Email,
                FirstName = firstName,
                LastName = lastName,
                SsoNameId = NameId,
                SsoSessionId = SessionId,
                Location = Location,
                Title = Title,
                ActivationStatus = EmployeeActivationStatus.NotActivated,
                WorkFromDate = _tenantUtil.DateTimeNow()
            };

            if (string.IsNullOrEmpty(Phone))
                return userInfo;

            var contacts = new List<string> { EXT_MOB_PHONE, Phone };
            userInfo.ContactsList = contacts;
        }
        else
        {
            userInfo.Email = Email;
            userInfo.FirstName = firstName;
            userInfo.LastName = lastName;
            userInfo.SsoNameId = NameId;
            userInfo.SsoSessionId = SessionId;
            userInfo.Location = Location;
            userInfo.Title = Title;

            var portalUserContacts = userInfo.ContactsList;

            var newContacts = new List<string>();
            var phones = new List<string>();
            var otherContacts = new List<string>();

            for (int i = 0, n = portalUserContacts.Count; i < n; i += 2)
            {
                if (i + 1 >= portalUserContacts.Count)
                    continue;

                var type = portalUserContacts[i];
                var value = portalUserContacts[i + 1];

                switch (type)
                {
                    case EXT_MOB_PHONE:
                        break;
                    case MOB_PHONE:
                        phones.Add(value);
                        break;
                    default:
                        otherContacts.Add(type);
                        otherContacts.Add(value);
                        break;
                }
            }

            if (!string.IsNullOrEmpty(Phone))
            {
                if (phones.Exists(p => p.Equals(Phone)))
                {
                    phones.Remove(Phone);
                }

                newContacts.Add(EXT_MOB_PHONE);
                newContacts.Add(Phone);
            }

            phones.ForEach(p =>
            {
                newContacts.Add(MOB_PHONE);
                newContacts.Add(p);
            });

            newContacts.AddRange(otherContacts);

            userInfo.ContactsList = newContacts;
        }

        return userInfo;
    }

    private static string TrimToLimit(string str, int limit = MAX_NUMBER_OF_SYMBOLS)
    {
        if (string.IsNullOrEmpty(str))
            return "";

        var newStr = str.Trim();

        return newStr.Length > limit
                ? newStr.Substring(0, MAX_NUMBER_OF_SYMBOLS)
                : newStr;
    }
}

[Serializable]
public class LogoutSsoUserData
{
    [DataMember(Name = "nameID")]
    public string NameId { get; set; }

    [DataMember(Name = "sessionID")]
    public string SessionId { get; set; }
}
