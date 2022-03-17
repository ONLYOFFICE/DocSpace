/*
 *
 * (c) Copyright Ascensio System Limited 2010-2021
 * 
 * Licensed under the Apache License, Version 2.0 (the "License");
 * you may not use this file except in compliance with the License.
 * You may obtain a copy of the License at
 * http://www.apache.org/licenses/LICENSE-2.0
 * Unless required by applicable law or agreed to in writing, software
 * distributed under the License is distributed on an "AS IS" BASIS,
 * WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 * See the License for the specific language governing permissions and
 * limitations under the License.
 *
*/


namespace ASC.ActiveDirectory.Novell.Data;
/// <summary>
/// Novell LDAP object class
/// </summary>
public class NovellLdapObject : LdapObject
{
    private LdapEntry _ldapEntry;
    private readonly ILog _log;
    private string _sid;
    private string _sidAttribute;
    private readonly NovellLdapEntryExtension _novellLdapEntryExtension;

    /// <summary>
    /// Constructor
    /// </summary>
    /// <param name="ldapEntry">init ldap entry</param>
    /// <param name="ldapUniqueIdAttribute"></param>
    public NovellLdapObject(IOptionsMonitor<ILog> option, NovellLdapEntryExtension novellLdapEntryExtension)
    {
        _novellLdapEntryExtension = novellLdapEntryExtension;
        _log = option.Get("ASC");
    }

    public void Init(LdapEntry ldapEntry, string ldapUniqueIdAttribute = null)
    {
        if (ldapEntry == null)
            throw new ArgumentNullException("ldapEntry");

        _ldapEntry = ldapEntry;

        if (string.IsNullOrEmpty(ldapUniqueIdAttribute))
            return;

        try
        {
            _sid = GetValue(ldapUniqueIdAttribute) as string;
            _sidAttribute = ldapUniqueIdAttribute;
        }
        catch (Exception e)
        {
            _log.ErrorFormat("Can't get LDAPObject Sid property. {0}", e);
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
                _log.ErrorFormat("Can't get LDAPUser UserAccauntControl property. {0}", e);
            }

            return (userAccauntControl & LdapConstants.UserAccountControl.ADS_UF_ACCOUNTDISABLE) > 0;
        }
    }

    #endregion

    /// <summary>
    /// Get property object
    /// </summary>
    /// <param name="propertyName">property name</param>
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
