namespace ASC.Core.Security.Authentication;

[Serializable]
class UserAccount : MarshalByRefObject, IUserAccount
{
    public Guid ID { get; private set; }
    public string Name { get; private set; }
    public string AuthenticationType => "ASC";
    public bool IsAuthenticated => true;
    public string FirstName { get; private set; }
    public string LastName { get; private set; }
    public string Title { get; private set; }
    public int Tenant { get; private set; }
    public string Email { get; private set; }

    public UserAccount(UserInfo info, int tenant, UserFormatter userFormatter)
    {
        ID = info.Id;
        Name = userFormatter.GetUserName(info);
        FirstName = info.FirstName;
        LastName = info.LastName;
        Title = info.Title;
        Tenant = tenant;
        Email = info.Email;
    }

    public object Clone()
    {
        return MemberwiseClone();
    }

    public override bool Equals(object obj)
    {
        return obj is IUserAccount a && ID.Equals(a.ID);
    }

    public override int GetHashCode()
    {
        return ID.GetHashCode();
    }

    public override string ToString()
    {
        return Name;
    }
}
