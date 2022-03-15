namespace ASC.Core.Users;

[Serializable]
public sealed class UserInfo : IDirectRecipient, ICloneable, IMapFrom<User>
{
    public UserInfo()
    {
        Status = EmployeeStatus.Active;
        ActivationStatus = EmployeeActivationStatus.NotActivated;
        LastModified = DateTime.UtcNow;
    }

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

    private string _contacts;
    public string Contacts
    {
        get => _contacts;
        set
        {
            _contacts = value;
            ContactsFromString(_contacts);
        }
    }

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
