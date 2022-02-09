/*
 *
 * (c) Copyright Ascensio System Limited 2010-2018
 *
 * This program is freeware. You can redistribute it and/or modify it under the terms of the GNU 
 * General Public License (GPL) version 3 as published by the Free Software Foundation (https://www.gnu.org/copyleft/gpl.html). 
 * In accordance with Section 7(a) of the GNU GPL its Section 15 shall be amended to the effect that 
 * Ascensio System SIA expressly excludes the warranty of non-infringement of any third-party rights.
 *
 * THIS PROGRAM IS DISTRIBUTED WITHOUT ANY WARRANTY; WITHOUT EVEN THE IMPLIED WARRANTY OF MERCHANTABILITY OR
 * FITNESS FOR A PARTICULAR PURPOSE. For more details, see GNU GPL at https://www.gnu.org/copyleft/gpl.html
 *
 * You can contact Ascensio System SIA by email at sales@onlyoffice.com
 *
 * The interactive user interfaces in modified source and object code versions of ONLYOFFICE must display 
 * Appropriate Legal Notices, as required under Section 5 of the GNU GPL version 3.
 *
 * Pursuant to Section 7 § 3(b) of the GNU GPL you must retain the original ONLYOFFICE logo which contains 
 * relevant author attributions when distributing the software. If the display of the logo in its graphic 
 * form is not reasonably feasible for technical reasons, you must include the words "Powered by ONLYOFFICE" 
 * in every copy of the program you distribute. 
 * Pursuant to Section 7 § 3(e) we decline to grant you any rights under trademark law for use of our trademarks.
 *
*/

namespace ASC.Core.Users;

[Serializable]
public sealed class UserInfo : IDirectRecipient, ICloneable, IMapFrom<User>
{
    public Guid Id { get; set; }
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public string UserName { get; set; }
    public DateTime? BirthDate { get; set; }
    public bool? Sex { get; set; }
    public EmployeeStatus Status { get; set; }
    public EmployeeActivationStatus ActivationStatus { get; set; }
    public DateTime? TerminatedDate { get; set; }
    public string Title { get; set; }
    public DateTime? WorkFromDate { get; set; }
    public string Email { get; set; }
    public List<string> ContactsList { get; set; }
    public string Location { get; set; }
    public string Notes { get; set; }
    public bool Removed { get; set; }
    public DateTime LastModified { get; set; }
    public int Tenant { get; set; }
    public bool IsActive => ActivationStatus.HasFlag(EmployeeActivationStatus.Activated);
    public string CultureName { get; set; }
    public string MobilePhone { get; set; }
    public MobilePhoneActivationStatus MobilePhoneActivationStatus { get; set; }
    public string Sid { get; set; } // LDAP user identificator
    public string SsoNameId { get; set; } // SSO SAML user identificator
    public string SsoSessionId { get; set; } // SSO SAML user session identificator
    public DateTime CreateDate { get; set; }
    public string Contacts
    {
        get => _contacts;
        set
        {
            _contacts = value;
            ContactsFromString(_contacts);
        }
    }
    string[] IDirectRecipient.Addresses => !string.IsNullOrEmpty(Email) ? new[] { Email } : Array.Empty<string>();
    public bool CheckActivation => IsActive; /*if user already active we don't need activation*/
    string IRecipient.ID => Id.ToString();
    string IRecipient.Name => ToString();

    private string _contacts;

    public UserInfo()
    {
        Status = EmployeeStatus.Active;
        ActivationStatus = EmployeeActivationStatus.NotActivated;
        LastModified = DateTime.UtcNow;
    }

    public override string ToString()
    {
        return $"{FirstName} {LastName}".Trim();
    }

    public override int GetHashCode()
    {
        return Id.GetHashCode();
    }

    public override bool Equals(object obj)
    {
        return obj is UserInfo ui && Id.Equals(ui.Id);
    }

    public bool Equals(UserInfo obj)
    {
        return obj != null && Id.Equals(obj.Id);
    }

    public CultureInfo GetCulture()
    {
        return string.IsNullOrEmpty(CultureName) ? CultureInfo.CurrentCulture : CultureInfo.GetCultureInfo(CultureName);
    }

    public object Clone()
    {
        return MemberwiseClone();
    }

    internal string ContactsToString()
    {
        if (ContactsList == null || ContactsList.Count == 0)
        {
            return null;
        }

        var sBuilder = new StringBuilder();

        foreach (var contact in ContactsList)
        {
            sBuilder.AppendFormat("{0}|", contact);
        }

        return sBuilder.ToString();
    }

    internal UserInfo ContactsFromString(string contacts)
    {
        if (string.IsNullOrEmpty(contacts))
        {
            return this;
        }

        if (ContactsList == null)
        {
            ContactsList = new List<string>();
        }
        else
        {
            ContactsList.Clear();
        }

        ContactsList.AddRange(contacts.Split(new[] { '|' }, StringSplitOptions.RemoveEmptyEntries));

        return this;
    }
}
