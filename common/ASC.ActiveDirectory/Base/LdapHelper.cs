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

namespace ASC.ActiveDirectory.Base;

[Scope]
public abstract class LdapHelper : IDisposable
{
    public LdapSettings Settings { get; private set; }
    public abstract bool IsConnected { get; }

    protected readonly ILogger<LdapHelper> _logger;
    protected readonly InstanceCrypto _instanceCrypto;

    protected LdapHelper(
        ILogger<LdapHelper> logger,
        InstanceCrypto instanceCrypto)
    {
        _logger = logger;
        _instanceCrypto = instanceCrypto;
    }

    public void Init(LdapSettings settings)
    {
        Settings = settings;
    }

    public abstract void Connect();

    public abstract Dictionary<string, string[]> GetCapabilities();

    public abstract string SearchDomain();

    public abstract void CheckCredentials(string login, string password, string server, int portNumber,
        bool startTls, bool ssl, bool acceptCertificate, string acceptCertificateHash);

    public abstract bool CheckUserDn(string userDn);

    public abstract List<LdapObject> GetUsers(string filter = null, int limit = -1);

    public abstract LdapObject GetUserBySid(string sid);

    public abstract bool CheckGroupDn(string groupDn);

    public abstract List<LdapObject> GetGroups(Criteria criteria = null);

    public bool UserExistsInGroup(LdapObject domainGroup, LdapObject domainUser, LdapSettings settings) // string memberString, string groupAttribute, string primaryGroupId)
    {
        try
        {
            if (domainGroup == null || domainUser == null)
            {
                return false;
            }

            var memberString = domainUser.GetValue(Settings.UserAttribute) as string;
            if (string.IsNullOrEmpty(memberString))
            {
                return false;
            }

            var groupAttribute = settings.GroupAttribute;
            if (string.IsNullOrEmpty(groupAttribute))
            {
                return false;
            }

            var userPrimaryGroupId = domainUser.GetValue(LdapConstants.ADSchemaAttributes.PRIMARY_GROUP_ID) as string;

            if (!string.IsNullOrEmpty(userPrimaryGroupId) && domainGroup.Sid.EndsWith("-" + userPrimaryGroupId))
            {
                // Domain Users found
                return true;
            }
            else
            {
                var members = domainGroup.GetValues(groupAttribute);

                if (members.Count == 0)
                {
                    return false;
                }

                if (members.Any(member => memberString.Equals(member, StringComparison.InvariantCultureIgnoreCase)
                    || member.Equals(domainUser.DistinguishedName, StringComparison.InvariantCultureIgnoreCase)))
                {
                    return true;
                }
            }
        }
        catch (Exception e)
        {
            _logger.ErrorUserExistsInGroupFailed(e);
        }

        return false;
    }

    public string GetPassword(byte[] passwordBytes)
    {
        if (passwordBytes == null || passwordBytes.Length == 0)
        {
            return string.Empty;
        }

        string password;
        try
        {
            password = _instanceCrypto.Decrypt(passwordBytes, new UnicodeEncoding());
        }
        catch (Exception)
        {
            password = string.Empty;
        }
        return password;
    }

    public byte[] GetPasswordBytes(string password)
    {
        byte[] passwordBytes;

        try
        {
            passwordBytes = _instanceCrypto.Encrypt(new UnicodeEncoding().GetBytes(password));
        }
        catch (Exception)
        {
            passwordBytes = Array.Empty<byte>();
        }

        return passwordBytes;
    }

    public abstract void Dispose();
}
