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

namespace ASC.ActiveDirectory.Novell.Data;
/// <summary>
/// Novell LDAP object class
/// </summary>
public class NovellLdapObject : LdapObject
{
    private LdapEntry _ldapEntry;
    private readonly ILogger _logger;
    private string _sid;
    private string _sidAttribute;
    private readonly NovellLdapEntryExtension _novellLdapEntryExtension;

    /// <summary>
    /// Constructor
    /// </summary>
    /// <param name="logger">init ldap entry</param>
    /// <param name="novellLdapEntryExtension"></param>
    public NovellLdapObject(ILogger logger, NovellLdapEntryExtension novellLdapEntryExtension)
    {
        _novellLdapEntryExtension = novellLdapEntryExtension;
        _logger = logger;
    }

    public void Init(LdapEntry ldapEntry, string ldapUniqueIdAttribute = null)
    {
        if (ldapEntry == null)
        {
            throw new ArgumentNullException("ldapEntry");
        }

        _ldapEntry = ldapEntry;

        if (string.IsNullOrEmpty(ldapUniqueIdAttribute))
        {
            return;
        }

        try
        {
            _sid = GetValue(ldapUniqueIdAttribute) as string;
            _sidAttribute = ldapUniqueIdAttribute;
        }
        catch (Exception e)
        {
            _logger.ErrorCanNotGetSidProperty(e);
        }
    }

    #region .Public

    public override string DistinguishedName
    {
        get { return _ldapEntry.Dn; }
    }

    public override string Sid
    {
        get { return _sid; }
    }

    public override string SidAttribute
    {
        get { return _sidAttribute; }
    }

    public override bool IsDisabled
    {
        get
        {
            var userAccauntControl = LdapConstants.UserAccountControl.EMPTY;
            try
            {
                var uac = Convert.ToInt32(GetValue(LdapConstants.ADSchemaAttributes.USER_ACCOUNT_CONTROL));
                userAccauntControl = (LdapConstants.UserAccountControl)uac;
            }
            catch (Exception e)
            {
                _logger.ErrorCanNotGetUserAccountControlProperty(e);
            }

            return (userAccauntControl & LdapConstants.UserAccountControl.ADS_UF_ACCOUNTDISABLE) > 0;
        }
    }

    #endregion

    /// <summary>
    /// Get property object
    /// </summary>
    /// <param name="propertyName">property name</param>
    /// <param name="getBytes"></param>
    /// <returns>value object</returns>
    public sealed override object GetValue(string propertyName, bool getBytes = false)
    {
        return _novellLdapEntryExtension.GetAttributeValue(_ldapEntry, propertyName, getBytes);
    }

    /// <summary>
    /// Get property values
    /// </summary>
    /// <param name="propertyName">property name</param>
    /// <returns>list of values</returns>
    public override List<string> GetValues(string propertyName)
    {
        var propertyValueArray = _novellLdapEntryExtension.GetAttributeArrayValue(_ldapEntry, propertyName);
        if (propertyValueArray == null)
        {
            return new List<string>();
        }

        var properties = propertyValueArray.ToList();
        return properties;
    }
}
