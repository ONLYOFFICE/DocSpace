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

using Mapping = ASC.ActiveDirectory.Base.Settings.LdapSettings.MappingFields;

namespace ASC.ActiveDirectory.Base.Data;
/// <summary>
/// LDAP object extensions class
/// </summary>
[Scope]
public class LdapObjectExtension
{
    private readonly TenantUtil _tenantUtil;
    private readonly SettingsManager _settingsManager;
    private readonly ILogger<LdapObjectExtension> _logger;

    public LdapObjectExtension(TenantUtil tenantUtil, SettingsManager settingsManager, ILogger<LdapObjectExtension> logger)
    {
        _tenantUtil = tenantUtil;
        _settingsManager = settingsManager;
        _logger = logger;
    }
    public string GetAttribute(LdapObject ldapObject, string attribute)
    {
        if (string.IsNullOrEmpty(attribute))
        {
            return string.Empty;
        }

        try
        {
            return ldapObject.GetValue(attribute) as string;
        }
        catch (Exception e)
        {
            _logger.ErrorCanNotGetAttribute(attribute, ldapObject.DistinguishedName, e);

            return string.Empty;
        }
    }

    public List<string> GetAttributes(LdapObject ldapObject, string attribute)
    {
        var list = new List<string>();

        if (string.IsNullOrEmpty(attribute))
        {
            return list;
        }

        try
        {
            return ldapObject.GetValues(attribute);
        }
        catch (Exception e)
        {

            _logger.ErrorCanNotGetAttributes(attribute, ldapObject.DistinguishedName, e);

            return list;
        }
    }

    private const int MAX_NUMBER_OF_SYMBOLS = 64;
    private const string EXT_MOB_PHONE = "extmobphone";
    private const string EXT_MAIL = "extmail";
    private const string EXT_PHONE = "extphone";
    private const string EXT_SKYPE = "extskype";

    private List<string> GetContacts(LdapObject ldapUser, Mapping key, LdapSettings settings)
    {
        if (!settings.LdapMapping.ContainsKey(key))
        {
            return null;
        }

        var bindings = settings.LdapMapping[key].Split(',').Select(x => x.Trim()).ToArray();
        if (bindings.Length > 1)
        {
            var list = new List<string>();
            foreach (var bind in bindings)
            {
                list.AddRange(GetAttributes(ldapUser, bind));
            }
            return list;
        }
        else
        {
            return GetAttributes(ldapUser, bindings[0]);
        }
    }

    private void PopulateContacts(List<string> Contacts, string type, List<string> values)
    {
        if (values == null || !values.Any())
        {
            return;
        }

        foreach (var val in values)
        {
            Contacts.Add(type);
            Contacts.Add(val);
        }
    }

    public async Task<UserInfo> ToUserInfoAsync(LdapObject ldapUser, LdapUserImporter ldapUserImporter)
    {
        var settings = ldapUserImporter.Settings;
        var resource = ldapUserImporter.Resource;

        var userName = GetAttribute(ldapUser, settings.LoginAttribute);

        var firstName = settings.LdapMapping.ContainsKey(Mapping.FirstNameAttribute) ? GetAttribute(ldapUser, settings.LdapMapping[Mapping.FirstNameAttribute]) : string.Empty;
        var secondName = settings.LdapMapping.ContainsKey(Mapping.SecondNameAttribute) ? GetAttribute(ldapUser, settings.LdapMapping[Mapping.SecondNameAttribute]) : string.Empty;
        var birthDay = settings.LdapMapping.ContainsKey(Mapping.BirthDayAttribute) ? GetAttribute(ldapUser, settings.LdapMapping[Mapping.BirthDayAttribute]) : string.Empty;
        var gender = settings.LdapMapping.ContainsKey(Mapping.GenderAttribute) ? GetAttribute(ldapUser, settings.LdapMapping[Mapping.GenderAttribute]) : string.Empty;
        var primaryPhone = settings.LdapMapping.ContainsKey(Mapping.MobilePhoneAttribute) ? GetAttribute(ldapUser, settings.LdapMapping[Mapping.MobilePhoneAttribute]) : string.Empty;
        var mail = settings.LdapMapping.ContainsKey(Mapping.MailAttribute) ? GetAttribute(ldapUser, settings.LdapMapping[Mapping.MailAttribute]) : string.Empty;
        var title = settings.LdapMapping.ContainsKey(Mapping.TitleAttribute) ? GetAttribute(ldapUser, settings.LdapMapping[Mapping.TitleAttribute]) : string.Empty;
        var location = settings.LdapMapping.ContainsKey(Mapping.LocationAttribute) ? GetAttribute(ldapUser, settings.LdapMapping[Mapping.LocationAttribute]) : string.Empty;

        var phones = GetContacts(ldapUser, Mapping.AdditionalPhone, settings);
        var mobilePhones = GetContacts(ldapUser, Mapping.AdditionalMobilePhone, settings);
        var emails = GetContacts(ldapUser, Mapping.AdditionalMail, settings);
        var skype = GetContacts(ldapUser, Mapping.Skype, settings);

        var quotaSettings = await _settingsManager.LoadAsync<TenantUserQuotaSettings>();
        var quota = settings.LdapMapping.ContainsKey(Mapping.UserQuotaLimit) ? ByteConverter.ConvertSizeToBytes(GetAttribute(ldapUser, settings.LdapMapping[Mapping.UserQuotaLimit])) : quotaSettings.DefaultUserQuota;

        if (string.IsNullOrEmpty(userName))
        {
            throw new Exception("LDAP LoginAttribute is empty");
        }

        var contacts = new List<string>();

        PopulateContacts(contacts, EXT_PHONE, phones);
        PopulateContacts(contacts, EXT_MOB_PHONE, mobilePhones);
        PopulateContacts(contacts, EXT_MAIL, emails);
        PopulateContacts(contacts, EXT_SKYPE, skype);

        var user = new UserInfo
        {
            Id = Guid.Empty,
            UserName = userName,
            Sid = ldapUser.Sid,
            ActivationStatus = settings.SendWelcomeEmail && !string.IsNullOrEmpty(mail) ? EmployeeActivationStatus.Pending : EmployeeActivationStatus.NotActivated,
            Status = ldapUser.IsDisabled ? EmployeeStatus.Terminated : EmployeeStatus.Active,
            Title = !string.IsNullOrEmpty(title) ? title : string.Empty,
            Location = !string.IsNullOrEmpty(location) ? location : string.Empty,
            WorkFromDate = _tenantUtil.DateTimeNow(),
            ContactsList = contacts,
            LdapQouta = quota
        };

        if (!string.IsNullOrEmpty(firstName))
        {
            user.FirstName = firstName.Length > MAX_NUMBER_OF_SYMBOLS
                ? firstName.Substring(0, MAX_NUMBER_OF_SYMBOLS)
                : firstName;
        }
        else
        {
            user.FirstName = resource.FirstName;
        }

        if (!string.IsNullOrEmpty(secondName))
        {
            user.LastName = secondName.Length > MAX_NUMBER_OF_SYMBOLS
                ? secondName.Substring(0, MAX_NUMBER_OF_SYMBOLS)
                : secondName;
        }
        else
        {
            user.LastName = resource.LastName;
        }

        if (!string.IsNullOrEmpty(birthDay))
        {
            DateTime date;
            if (DateTime.TryParse(birthDay, out date))
            {
                user.BirthDate = date;
            }
        }

        if (!string.IsNullOrEmpty(gender))
        {
            bool b;
            if (bool.TryParse(gender, out b))
            {
                user.Sex = b;
            }
            else
            {
                switch (gender.ToLowerInvariant())
                {
                    case "male":
                    case "m":
                        user.Sex = true;
                        break;
                    case "female":
                    case "f":
                        user.Sex = false;
                        break;
                }
            }
        }

        if (string.IsNullOrEmpty(mail))
        {
            user.Email = userName.Contains("@") ? userName : string.Format("{0}@{1}", userName, ldapUserImporter.LDAPDomain);
            user.ActivationStatus = EmployeeActivationStatus.AutoGenerated;
        }
        else
        {
            user.Email = mail;
        }

        user.MobilePhone = string.IsNullOrEmpty(primaryPhone)
            ? null : primaryPhone;

        return user;
    }

    public GroupInfo ToGroupInfo(LdapObject ldapGroup, LdapSettings settings)
    {
        var name = GetAttribute(ldapGroup, settings.GroupNameAttribute);

        if (string.IsNullOrEmpty(name))
        {
            throw new Exception("LDAP GroupNameAttribute is empty");
        }

        var group = new GroupInfo
        {
            Name = name,
            Sid = ldapGroup.Sid
        };

        return group;
    }

    public string GetDomainFromDn(LdapObject ldapObject)
    {
        if (ldapObject == null || string.IsNullOrEmpty(ldapObject.DistinguishedName))
        {
            return null;
        }

        return LdapUtils.DistinguishedNameToDomain(ldapObject.DistinguishedName);
    }
}
