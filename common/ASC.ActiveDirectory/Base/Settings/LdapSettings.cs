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

namespace ASC.ActiveDirectory.Base.Settings;

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
        Skype
    }

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
        var isMono = WorkContext.IsMono;

        var settings = new LdapSettings()
        {
            Server = "",
            UserDN = "",
            PortNumber = LdapConstants.STANDART_LDAP_PORT,
            UserFilter = string.Format("({0}=*)",
                isMono
                    ? LdapConstants.RfcLDAPAttributes.UID
                    : LdapConstants.ADSchemaAttributes.USER_PRINCIPAL_NAME),
            LoginAttribute = isMono
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
                isMono
                    ? LdapConstants.ObjectClassKnowedValues.POSIX_GROUP
                    : LdapConstants.ObjectClassKnowedValues.GROUP),
            UserAttribute =
                isMono
                    ? LdapConstants.RfcLDAPAttributes.UID
                    : LdapConstants.ADSchemaAttributes.DISTINGUISHED_NAME,
            GroupAttribute = isMono ? LdapConstants.RfcLDAPAttributes.MEMBER_UID : LdapConstants.ADSchemaAttributes.MEMBER,
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

    public bool EnableLdapAuthentication { get; set; }

    public bool StartTls { get; set; }

    public bool Ssl { get; set; }

    public bool SendWelcomeEmail { get; set; }

    public string Server { get; set; }

    // ReSharper disable once InconsistentNaming
    public string UserDN { get; set; }

    [JsonNumberHandling(JsonNumberHandling.AllowReadingFromString)]
    public int PortNumber { get; set; }

    public string UserFilter { get; set; }

    public string LoginAttribute { get; set; }

    public Dictionary<MappingFields, string> LdapMapping { get; set; }

    //ToDo: use SId instead of group name
    public Dictionary<AccessRight, string> AccessRights { get; set; }

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

    public bool GroupMembership { get; set; }

    // ReSharper disable once InconsistentNaming
    public string GroupDN { get; set; }

    public string GroupNameAttribute { get; set; }

    public string GroupFilter { get; set; }

    public string UserAttribute { get; set; }

    public string GroupAttribute { get; set; }

    public bool Authentication { get; set; }

    public string Login { get; set; }

    public string Password { get; set; }

    public byte[] PasswordBytes { get; set; }

    public bool IsDefault { get; set; }

    public bool AcceptCertificate { get; set; }

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
