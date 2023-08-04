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

using System.Runtime.InteropServices;

namespace ASC.ActiveDirectory.Base.Settings;

/// <summary>
/// </summary>
[Scope]
[Serializable]
public class LdapSettings : ISettings<LdapSettings>, ICloneable
{
    [JsonIgnore]
    public Guid ID
    {
        get { return new Guid("{197149b3-fbc9-44c2-b42a-232f7e729c16}"); }
    }

    public LdapSettings()
    {
        LdapMapping = new Dictionary<MappingFields, string>();
        AccessRights = new Dictionary<AccessRight, string>();
    }

    /// <summary>LDAP settings mapping</summary>
    public enum MappingFields
    {
        FirstNameAttribute,
        SecondNameAttribute,
        BirthDayAttribute,
        GenderAttribute,
        MobilePhoneAttribute,
        MailAttribute,
        TitleAttribute,
        LocationAttribute,
        AvatarAttribute,

        AdditionalPhone,
        AdditionalMobilePhone,
        AdditionalMail,
        Skype,

        UserQuotaLimit
    }

    /// <summary>Accecss rights</summary>
    public enum AccessRight
    {
        FullAccess,
        Documents,
        Projects,
        CRM,
        Community,
        People,
        Mail
    }

    public static readonly Dictionary<AccessRight, Guid> AccessRightsGuids = new Dictionary<AccessRight, Guid>()
        {
            { AccessRight.FullAccess, Guid.Empty },
            { AccessRight.Documents, WebItemManager.DocumentsProductID },
            { AccessRight.Projects, WebItemManager.ProjectsProductID },
            { AccessRight.CRM, WebItemManager.CRMProductID },
            { AccessRight.Community, WebItemManager.CommunityProductID },
            { AccessRight.People, WebItemManager.PeopleProductID },
            { AccessRight.Mail, WebItemManager.MailProductID }
        };

    public LdapSettings GetDefault()
    {
        var isNotWindows = !RuntimeInformation.IsOSPlatform(OSPlatform.Windows);

        var settings = new LdapSettings()
        {
            Server = "",
            UserDN = "",
            PortNumber = LdapConstants.STANDART_LDAP_PORT,
            UserFilter = string.Format("({0}=*)",
                isNotWindows
                    ? LdapConstants.RfcLDAPAttributes.UID
                    : LdapConstants.ADSchemaAttributes.USER_PRINCIPAL_NAME),
            LoginAttribute = isNotWindows
                ? LdapConstants.RfcLDAPAttributes.UID
                : LdapConstants.ADSchemaAttributes.ACCOUNT_NAME,
            FirstNameAttribute = LdapConstants.ADSchemaAttributes.FIRST_NAME,
            SecondNameAttribute = LdapConstants.ADSchemaAttributes.SURNAME,
            MailAttribute = LdapConstants.ADSchemaAttributes.MAIL,
            TitleAttribute = LdapConstants.ADSchemaAttributes.TITLE,
            MobilePhoneAttribute = LdapConstants.ADSchemaAttributes.MOBILE,
            LocationAttribute = LdapConstants.ADSchemaAttributes.STREET,
            GroupDN = "",
            GroupFilter = string.Format("({0}={1})", LdapConstants.ADSchemaAttributes.OBJECT_CLASS,
                isNotWindows
                    ? LdapConstants.ObjectClassKnowedValues.POSIX_GROUP
                    : LdapConstants.ObjectClassKnowedValues.GROUP),
            UserAttribute =
                isNotWindows
                    ? LdapConstants.RfcLDAPAttributes.UID
                    : LdapConstants.ADSchemaAttributes.DISTINGUISHED_NAME,
            GroupAttribute = isNotWindows ? LdapConstants.RfcLDAPAttributes.MEMBER_UID : LdapConstants.ADSchemaAttributes.MEMBER,
            GroupNameAttribute = LdapConstants.ADSchemaAttributes.COMMON_NAME,
            Authentication = true,
            AcceptCertificate = false,
            AcceptCertificateHash = null,
            StartTls = false,
            Ssl = false,
            SendWelcomeEmail = false
        };

        return settings;
    }

    public override bool Equals(object obj)
    {
        var settings = obj as LdapSettings;

        return settings != null
               && EnableLdapAuthentication == settings.EnableLdapAuthentication
               && StartTls == settings.StartTls
               && Ssl == settings.Ssl
               && SendWelcomeEmail == settings.SendWelcomeEmail
               && (string.IsNullOrEmpty(Server)
                   && string.IsNullOrEmpty(settings.Server)
                   || Server == settings.Server)
               && (string.IsNullOrEmpty(UserDN)
                   && string.IsNullOrEmpty(settings.UserDN)
                   || UserDN == settings.UserDN)
               && PortNumber == settings.PortNumber
               && UserFilter == settings.UserFilter
               && LoginAttribute == settings.LoginAttribute
               && LdapMapping.Count == settings.LdapMapping.Count
               && LdapMapping.All(pair => settings.LdapMapping.ContainsKey(pair.Key)
                   && pair.Value == settings.LdapMapping[pair.Key])
               && AccessRights.Count == settings.AccessRights.Count
               && AccessRights.All(pair => settings.AccessRights.ContainsKey(pair.Key)
                   && pair.Value == settings.AccessRights[pair.Key])
               && GroupMembership == settings.GroupMembership
               && (string.IsNullOrEmpty(GroupDN)
                   && string.IsNullOrEmpty(settings.GroupDN)
                   || GroupDN == settings.GroupDN)
               && GroupFilter == settings.GroupFilter
               && UserAttribute == settings.UserAttribute
               && GroupAttribute == settings.GroupAttribute
               && (string.IsNullOrEmpty(Login)
                   && string.IsNullOrEmpty(settings.Login)
                   || Login == settings.Login)
               && Authentication == settings.Authentication;
    }

    public override int GetHashCode()
    {
        var hash = 3;
        hash = (hash * 2) + EnableLdapAuthentication.GetHashCode();
        hash = (hash * 2) + StartTls.GetHashCode();
        hash = (hash * 2) + Ssl.GetHashCode();
        hash = (hash * 2) + SendWelcomeEmail.GetHashCode();
        hash = (hash * 2) + Server.GetHashCode();
        hash = (hash * 2) + UserDN.GetHashCode();
        hash = (hash * 2) + PortNumber.GetHashCode();
        hash = (hash * 2) + UserFilter.GetHashCode();
        hash = (hash * 2) + LoginAttribute.GetHashCode();
        hash = (hash * 2) + GroupMembership.GetHashCode();
        hash = (hash * 2) + GroupDN.GetHashCode();
        hash = (hash * 2) + GroupNameAttribute.GetHashCode();
        hash = (hash * 2) + GroupFilter.GetHashCode();
        hash = (hash * 2) + UserAttribute.GetHashCode();
        hash = (hash * 2) + GroupAttribute.GetHashCode();
        hash = (hash * 2) + Authentication.GetHashCode();
        hash = (hash * 2) + Login.GetHashCode();

        foreach (var pair in LdapMapping)
        {
            hash = (hash * 2) + pair.Value.GetHashCode();
        }

        foreach (var pair in AccessRights)
        {
            hash = (hash * 2) + pair.Value.GetHashCode();
        }

        return hash;
    }

    public object Clone()
    {
        return MemberwiseClone();
    }

    /// <summary>Specifies if the LDAP authentication is enabled or not</summary>
    /// <type>System.Boolean, System</type>
    public bool EnableLdapAuthentication { get; set; }

    /// <summary>Specifies if the StartTLS is enabled or not</summary>
    /// <type>System.Boolean, System</type>
    public bool StartTls { get; set; }

    /// <summary>Specifies if the SSL is enabled or not</summary>
    /// <type>System.Boolean, System</type>
    public bool Ssl { get; set; }

    /// <summary>Specifies if the welcome email is sent or not</summary>
    /// <type>System.Boolean, System</type>
    public bool SendWelcomeEmail { get; set; }

    /// <summary>LDAP server URL address</summary>
    /// <type>System.String, System</type>
    public string Server { get; set; }

    /// <summary>Absolute path to the top level directory containing users for the import</summary>
    /// <type>System.String, System</type>
    // ReSharper disable once InconsistentNaming
    public string UserDN { get; set; }

    /// <summary>Port number</summary>
    /// <type>System.Int32, System</type>
    [JsonNumberHandling(JsonNumberHandling.AllowReadingFromString)]
    public int PortNumber { get; set; }

    /// <summary>User filter value to import the users who correspond to the specified search criteria. The default filter value (uid=*) allows importing all users</summary>
    /// <type>System.String, System</type>
    public string UserFilter { get; set; }

    /// <summary>Attribute in a user record that corresponds to the login that LDAP server users will use to log in to ONLYOFFICE</summary>
    /// <type>System.String, System</type>
    public string LoginAttribute { get; set; }

    /// <summary>Correspondence between the user data fields on the portal and the attributes in the LDAP server user record</summary>
    /// <type>System.Collections.Generic.Dictionary{ASC.ActiveDirectory.Base.Settings.MappingFields, System.String}, System.Collections.Generic</type>
    public Dictionary<MappingFields, string> LdapMapping { get; set; }

    /// <summary>Group access rights</summary>
    /// <type>System.Collections.Generic.Dictionary{ASC.ActiveDirectory.Base.Settings.AccessRight, System.String}, System.Collections.Generic</type>
    //ToDo: use SId instead of group name
    public Dictionary<AccessRight, string> AccessRights { get; set; }

    /// <summary>Attribute in a user record that corresponds to the user's first name</summary>
    /// <type>System.String, System</type>
    public string FirstNameAttribute
    {
        get
        {
            return GetOldSetting(MappingFields.FirstNameAttribute);
        }

        set
        {
            SetOldSetting(MappingFields.FirstNameAttribute, value);
        }
    }

    /// <summary>Attribute in a user record that corresponds to the user's second name</summary>
    /// <type>System.String, System</type>
    public string SecondNameAttribute
    {
        get
        {
            return GetOldSetting(MappingFields.SecondNameAttribute);
        }

        set
        {
            SetOldSetting(MappingFields.SecondNameAttribute, value);
        }
    }

    /// <summary>Attribute in a user record that corresponds to the user's email address</summary>
    /// <type>System.String, System</type>
    public string MailAttribute
    {
        get
        {
            return GetOldSetting(MappingFields.MailAttribute);
        }

        set
        {
            SetOldSetting(MappingFields.MailAttribute, value);
        }
    }

    /// <summary>Attribute in a user record that corresponds to the user's title</summary>
    /// <type>System.String, System</type>
    public string TitleAttribute
    {
        get
        {
            return GetOldSetting(MappingFields.TitleAttribute);
        }

        set
        {
            SetOldSetting(MappingFields.TitleAttribute, value);
        }
    }

    /// <summary>Attribute in a user record that corresponds to the user's mobile phone number</summary>
    /// <type>System.String, System</type>
    public string MobilePhoneAttribute
    {
        get
        {
            return GetOldSetting(MappingFields.MobilePhoneAttribute);
        }

        set
        {
            SetOldSetting(MappingFields.MobilePhoneAttribute, value);
        }
    }

    /// <summary>Attribute in a user record that corresponds to the user's location</summary>
    /// <type>System.String, System</type>
    public string LocationAttribute
    {
        get
        {
            return GetOldSetting(MappingFields.LocationAttribute);
        }

        set
        {
            SetOldSetting(MappingFields.LocationAttribute, value);
        }
    }

    /// <summary>Specifies if the groups from the LDAP server are added to the portal or not</summary>
    /// <type>System.Boolean, System</type>
    public bool GroupMembership { get; set; }

    /// <summary>The absolute path to the top level directory containing groups for the import</summary>
    /// <type>System.String, System</type>
    // ReSharper disable once InconsistentNaming
    public string GroupDN { get; set; }

    /// <summary>Attribute that corresponds to a name of the group where the user is included</summary>
    /// <type>System.String, System</type>
    public string GroupNameAttribute { get; set; }

    /// <summary>Group filter value to import the groups who correspond to the specified search criteria. The default filter value (objectClass=posixGroup) allows importing all users</summary>
    /// <type>System.String, System</type>
    public string GroupFilter { get; set; }

    /// <summary>Attribute that determines whether this user is a member of the groups</summary>
    /// <type>System.String, System</type>
    public string UserAttribute { get; set; }

    /// <summary>Attribute that specifies the users that the group includes</summary>
    /// <type>System.String, System</type>
    public string GroupAttribute { get; set; }

    /// <summary>Specifies if the user has rights to read data from LDAP server or not</summary>
    /// <type>System.Boolean, System</type>
    public bool Authentication { get; set; }

    /// <summary>Login</summary>
    /// <type>System.String, System</type>
    public string Login { get; set; }

    /// <summary>Password</summary>
    /// <type>System.String, System</type>
    public string Password { get; set; }

    /// <summary>Password bytes</summary>
    /// <type>System.Byte[], System</type>
    public byte[] PasswordBytes { get; set; }

    /// <summary>Specifies if the default LDAP settings are used or not</summary>
    /// <type>System.Boolean, System</type>
    public bool IsDefault { get; set; }

    /// <summary>Specifies if the certificate is accepted or not</summary>
    /// <type>System.Boolean, System</type>
    public bool AcceptCertificate { get; set; }

    /// <summary>Hash that is used to accept a certificate</summary>
    /// <type>System.String, System</type>
    public string AcceptCertificateHash { get; set; }

    private string GetOldSetting(MappingFields field)
    {
        if (LdapMapping == null)
        {
            LdapMapping = new Dictionary<MappingFields, string>();
        }

        if (LdapMapping.ContainsKey(field))
        {
            return LdapMapping[field];
        }
        else
        {
            return "";
        }
    }
    private void SetOldSetting(MappingFields field, string value)
    {
        if (LdapMapping == null)
        {
            LdapMapping = new Dictionary<MappingFields, string>();
        }

        if (string.IsNullOrEmpty(value))
        {
            if (LdapMapping.ContainsKey(field))
            {
                LdapMapping.Remove(field);
            }
            return;
        }

        if (LdapMapping.ContainsKey(field))
        {
            LdapMapping[field] = value;
        }
        else
        {
            LdapMapping.Add(field, value);
        }
    }
}

[Scope]
[Serializable]
public class LdapCronSettings : ISettings<LdapCronSettings>
{
    [JsonIgnore]
    public Guid ID
    {
        get { return new Guid("{58C42C54-56CD-4BEF-A3ED-C60ACCF6E975}"); }
    }

    public LdapCronSettings GetDefault()
    {
        return new LdapCronSettings()
        {
            Cron = null
        };
    }

    public string Cron { get; set; }
}

[Serializable]
public class LdapCurrentAcccessSettings : ISettings<LdapCurrentAcccessSettings>
{
    [JsonIgnore]
    public Guid ID
    {
        get { return new Guid("{134B5EAA-F612-4834-AEAB-34C90515EA4E}"); }
    }

    public LdapCurrentAcccessSettings GetDefault()
    {
        return new LdapCurrentAcccessSettings() { CurrentAccessRights = null };
    }

    public LdapCurrentAcccessSettings()
    {
        CurrentAccessRights = new Dictionary<LdapSettings.AccessRight, List<string>>();
    }

    public Dictionary<LdapSettings.AccessRight, List<string>> CurrentAccessRights { get; set; }
}

[Serializable]
public class LdapCurrentUserPhotos : ISettings<LdapCurrentUserPhotos>
{
    [JsonIgnore]
    public Guid ID
    {
        get { return new Guid("{50AE3C2B-0783-480F-AF30-679D0F0A2D3E}"); }
    }

    public LdapCurrentUserPhotos GetDefault()
    {
        return new LdapCurrentUserPhotos() { CurrentPhotos = null };
    }

    public LdapCurrentUserPhotos()
    {
        CurrentPhotos = new Dictionary<Guid, string>();
    }

    public Dictionary<Guid, string> CurrentPhotos { get; set; }
}

[Serializable]
public class LdapCurrentDomain : ISettings<LdapCurrentDomain>
{
    [JsonIgnore]
    public Guid ID
    {
        get { return new Guid("{75A5F745-F697-4418-B38D-0FE0D277E258}"); }
    }

    public LdapCurrentDomain GetDefault()
    {
        return new LdapCurrentDomain() { CurrentDomain = null };
    }

    public string CurrentDomain { get; set; }
}
