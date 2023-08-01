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

namespace ASC.Core.Users;

/// <summary>
/// </summary>
[Serializable]
public sealed class UserInfo : IDirectRecipient, ICloneable, IMapFrom<User>
{
    public UserInfo()
    {
        Status = EmployeeStatus.Active;
        ActivationStatus = EmployeeActivationStatus.NotActivated;
        LastModified = DateTime.UtcNow;
    }

    /// <summary>ID</summary>
    /// <type>System.Guid, System</type>
    public Guid Id { get; set; }

    /// <summary>First name</summary>
    /// <type>System.String, System</type>
    public string FirstName { get; set; }

    /// <summary>Last name</summary>
    /// <type>System.String, System</type>
    public string LastName { get; set; }

    /// <summary>Username</summary>
    /// <type>System.String, System</type>
    public string UserName { get; set; }

    /// <summary>Birthday</summary>
    /// <type>System.Nullable{System.DateTime}, System</type>
    public DateTime? BirthDate { get; set; }

    /// <summary>Sex (male or female)</summary>
    /// <type>System.Nullable{System.Boolean}, System</type>
    public bool? Sex { get; set; }

    /// <summary>Status</summary>
    /// <type>ASC.Core.Users.EmployeeStatus, ASC.Core.Common</type>
    public EmployeeStatus Status { get; set; }

    /// <summary>Activation status</summary>
    /// <type>ASC.Core.Users.EmployeeActivationStatus, ASC.Core.Common</type>
    public EmployeeActivationStatus ActivationStatus { get; set; }

    /// <summary>The date and time when the user account was terminated</summary>
    /// <type>System.Nullable{System.DateTime}, System</type>
    public DateTime? TerminatedDate { get; set; }

    /// <summary>Title</summary>
    /// <type>System.String, System</type>
    public string Title { get; set; }

    /// <summary>Registration date</summary>
    /// <type>System.Nullable{System.DateTime}, System</type>
    public DateTime? WorkFromDate { get; set; }

    /// <summary>Email</summary>
    /// <type>System.String, System</type>
    public string Email { get; set; }

    private string _contacts;

    /// <summary>List of contacts in the string format</summary>
    /// <type>System.String, System</type>
    public string Contacts
    {
        get => _contacts;
        set
        {
            _contacts = value;
            ContactsFromString(_contacts);
        }
    }

    /// <summary>List of contacts</summary>
    /// <type>System.Collections.Generic.List{System.String}, System.Collections.Generic</type>
    public List<string> ContactsList { get; set; }

    /// <summary>Location</summary>
    /// <type>System.String, System</type>
    public string Location { get; set; }

    /// <summary>Notes</summary>
    /// <type>System.String, System</type>
    public string Notes { get; set; }

    /// <summary>Specifies if the user account was removed or not</summary>
    /// <type>System.Boolean, System</type>
    public bool Removed { get; set; }

    /// <summary>Last modified date</summary>
    /// <type>System.DateTime, System</type>
    public DateTime LastModified { get; set; }

    /// <summary>Tenant ID</summary>
    /// <type>System.Int32, System</type>
    public int TenantId { get; set; }

    /// <summary>Spceifies if the user is active or not</summary>
    /// <type>System.Boolean, System</type>
    public bool IsActive => ActivationStatus.HasFlag(EmployeeActivationStatus.Activated);

    /// <summary>Language</summary>
    /// <type>System.String, System</type>
    public string CultureName { get; set; }

    /// <summary>Mobile phone</summary>
    /// <type>System.String, System</type>
    public string MobilePhone { get; set; }

    /// <summary>Mobile phone activation status</summary>
    /// <type>ASC.Core.Users.MobilePhoneActivationStatus, ASC.Core.Common</type>
    public MobilePhoneActivationStatus MobilePhoneActivationStatus { get; set; }

    /// <summary>LDAP user identificator</summary>
    /// <type>System.String, System</type>
    public string Sid { get; set; } // LDAP user identificator

    /// <summary>LDAP user quota attribute</summary>
    /// <type>System.Int64, System</type>
    public long LdapQouta { get; set; } // LDAP user quota attribute

    /// <summary>SSO SAML user identificator</summary>
    /// <type>System.String, System</type>
    public string SsoNameId { get; set; } // SSO SAML user identificator

    /// <summary>SSO SAML user session identificator</summary>
    /// <type>System.String, System</type>
    public string SsoSessionId { get; set; } // SSO SAML user session identificator

    /// <summary>Creation date</summary>
    /// <type>System.DateTime, System</type>
    public DateTime CreateDate { get; set; }

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

    string[] IDirectRecipient.Addresses => !string.IsNullOrEmpty(Email) ? new[] { Email } : Array.Empty<string>();
    public bool CheckActivation => !IsActive; /*if user already active we don't need activation*/
    string IRecipient.ID => Id.ToString();
    string IRecipient.Name => ToString();

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
            sBuilder.Append($"{contact}|");
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
