﻿// (c) Copyright Ascensio System SIA 2010-2022
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

namespace ASC.ActiveDirectory.Novell.Extensions;

[Singletone]
public class NovellLdapEntryExtension
{
    private readonly ILogger<NovellLdapEntryExtension> _logger;
    public NovellLdapEntryExtension(ILogger<NovellLdapEntryExtension> logger)
    {
        _logger = logger;
    }
    public object GetAttributeValue(LdapEntry ldapEntry, string attributeName, bool getBytes = false)
    {
        try
        {
            var attribute = ldapEntry.GetAttribute(attributeName);

            if (attribute == null)
            {
                return null;
            }

            if (!(string.Equals(attributeName, LdapConstants.ADSchemaAttributes.OBJECT_SID,
                StringComparison.OrdinalIgnoreCase) || getBytes))
            {
                return attribute.StringValue;
            }

            if (attribute.ByteValue == null)
            {
                return null;
            }

            var value = new byte[attribute.ByteValue.Length];

            Buffer.BlockCopy(attribute.ByteValue, 0, value, 0, attribute.ByteValue.Length);

            if (getBytes)
            {
                return value;
            }

            return DecodeSid(value);
        }
        catch (Exception)
        {
            return null;
        }
    }

    public string[] GetAttributeArrayValue(LdapEntry ldapEntry, string attributeName)
    {
        var attribute = ldapEntry.GetAttribute(attributeName);
        return attribute == null ? null : attribute.StringValueArray;
    }

    private string DecodeSid(byte[] sid)
    {
        var strSid = new StringBuilder("S-");

        // get version
        int revision = sid[0];
        strSid.Append(revision.ToString(CultureInfo.InvariantCulture));

        //next byte is the count of sub-authorities
        var countSubAuths = sid[1] & 0xFF;

        //get the authority
        long authority = 0;

        //String rid = "";
        for (var i = 2; i <= 7; i++)
        {
            authority |= ((long)sid[i]) << (8 * (5 - (i - 2)));
        }

        strSid.Append("-");
        strSid.Append(authority);

        //iterate all the sub-auths
        var offset = 8;
        const int size = 4; //4 bytes for each sub auth

        for (var j = 0; j < countSubAuths; j++)
        {
            long subAuthority = 0;
            for (var k = 0; k < size; k++)
            {
                subAuthority |= (long)(sid[offset + k] & 0xFF) << (8 * k);
            }

            strSid.Append("-");
            strSid.Append(subAuthority);

            offset += size;
        }

        return strSid.ToString();
    }

    /// <summary>
    /// Create LDAPObject by LdapEntry
    /// </summary>
    /// <param name="ldapEntry">init ldapEntry</param>
    /// <param name="ldapUniqueIdAttribute"></param>
    /// <returns>LDAPObject</returns>
    public LdapObject ToLdapObject(LdapEntry ldapEntry, string ldapUniqueIdAttribute = null)
    {
        if (ldapEntry == null)
        {
            throw new ArgumentNullException("ldapEntry");
        }

        var novellLdapObject = new NovellLdapObject(_logger, this);
        novellLdapObject.Init(ldapEntry, ldapUniqueIdAttribute);

        return novellLdapObject;
    }

    /// <summary>
    /// Create lis of LDAPObject by LdapEntry list
    /// </summary>
    /// <param name="entries">list of LdapEntry</param>
    /// <param name="ldapUniqueIdAttribute"></param>
    /// <returns>list of LDAPObjects</returns>
    public List<LdapObject> ToLdapObjects(IEnumerable<LdapEntry> entries, string ldapUniqueIdAttribute = null)
    {
        return entries.Select(e => ToLdapObject(e, ldapUniqueIdAttribute)).ToList();
    }
}
