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

using Action = System.Action;
using Constants = ASC.Core.Users.Constants;

namespace ASC.ActiveDirectory;

public static class LdapUtils
{
    private static readonly Regex _dcRegex = new Regex("dc=([^,]+)", RegexOptions.Compiled | RegexOptions.CultureInvariant | RegexOptions.IgnoreCase);

    public static string DistinguishedNameToDomain(string distinguishedName)
    {
        if (string.IsNullOrEmpty(distinguishedName))
        {
            return null;
        }

        var matchList = _dcRegex.Matches(distinguishedName);

        var dcList = matchList.Cast<Match>().Select(match => match.Groups[1].Value).ToList();

        return !dcList.Any() ? null : string.Join(".", dcList);
    }

    public static bool IsLoginAccepted(LdapLogin ldapLogin, UserInfo ldapUser, string ldapDomain)
    {
        if (ldapLogin == null
            || string.IsNullOrEmpty(ldapLogin.ToString())
            || string.IsNullOrEmpty(ldapDomain)
            || ldapUser == null
            || ldapUser.Equals(Constants.LostUser)
            || string.IsNullOrEmpty(ldapUser.Email)
            || string.IsNullOrEmpty(ldapUser.UserName))
        {
            return false;
        }

        var hasDomain = !string.IsNullOrEmpty(ldapLogin.Domain);

        if (!hasDomain)
        {
            return ldapLogin.Username.Equals(ldapUser.UserName, StringComparison.InvariantCultureIgnoreCase);
        }

        var fullLogin = ldapLogin.ToString();

        if (fullLogin.Equals(ldapUser.Email, StringComparison.InvariantCultureIgnoreCase))
        {
            return true;
        }

        if (!ldapDomain.StartsWith(ldapLogin.Domain))
        {
            return false;
        }

        var alterEmail = ldapUser.UserName.Contains("@")
            ? ldapUser.UserName
            : string.Format("{0}@{1}", ldapUser.UserName, ldapDomain);

        return IsLoginAndEmailSuitable(fullLogin, alterEmail);
    }

    private static string GetLdapAccessableEmail(string email)
    {
        try
        {
            if (string.IsNullOrEmpty(email))
            {
                return null;
            }

            var login = LdapLogin.ParseLogin(email);

            if (string.IsNullOrEmpty(login.Domain))
            {
                return email;
            }

            var dotIndex = login.Domain.LastIndexOf(".", StringComparison.Ordinal);

            var accessableEmail = dotIndex > -1 ? string.Format("{0}@{1}", login.Username, login.Domain.Remove(dotIndex)) : email;

            return accessableEmail;
        }
        catch (Exception)
        {
            return null;
        }
    }

    private static bool IsLoginAndEmailSuitable(string login, string email)
    {
        try
        {
            if (string.IsNullOrEmpty(login) || string.IsNullOrEmpty(email))
            {
                return false;
            }

            var accessableLogin = GetLdapAccessableEmail(login);

            if (string.IsNullOrEmpty(accessableLogin))
            {
                return false;
            }

            var accessableEmail = GetLdapAccessableEmail(email);

            if (string.IsNullOrEmpty(accessableEmail))
            {
                return false;
            }

            return accessableLogin.Equals(accessableEmail, StringComparison.InvariantCultureIgnoreCase);
        }
        catch (Exception)
        {
            return false;
        }
    }

    public static string GeneratePassword()
    {
        return Guid.NewGuid().ToString();
    }

    public static void SkipErrors(Action method, ILogger log = null)
    {
        try
        {
            method();
        }
        catch (Exception ex)
        {
            if (log != null)
            {
                log.ErrorSkipErrors(ex);
            }
        }
    }

    public static string GetContactsString(this UserInfo userInfo)
    {
        if (userInfo.ContactsList == null || userInfo.ContactsList.Count == 0)
        {
            return null;
        }

        var sBuilder = new StringBuilder();
        foreach (var contact in userInfo.Contacts)
        {
            sBuilder.AppendFormat("{0}|", contact);
        }
        return sBuilder.ToString();
    }

    public static string GetUserInfoString(this UserInfo userInfo)
    {
        return string.Format(
            "{{ ID: '{0}' SID: '{1}' Email '{2}' UserName: '{3}' FirstName: '{4}' LastName: '{5}' Title: '{6}' Location: '{7}' Contacts: '{8}' Status: '{9}' }}",
            userInfo.Id,
            userInfo.Sid,
            userInfo.Email,
            userInfo.UserName,
            userInfo.FirstName,
            userInfo.LastName,
            userInfo.Title,
            userInfo.Location,
            userInfo.GetContactsString(),
            Enum.GetName(typeof(EmployeeStatus), userInfo.Status));
    }

    public static string UnescapeLdapString(string ldapString)
    {
        var sb = new StringBuilder();
        for (var i = 0; i < ldapString.Length; i++)
        {
            var ch = ldapString[i];
            if (ch == '\\')
            {
                if (i + 1 < ldapString.Length && ldapString[i + 1] == ch)
                {
                    sb.Append(ch);
                    i++;
                }
            }
            else
            {
                sb.Append(ch);
            }
        }
        return sb.ToString();
    }
}
