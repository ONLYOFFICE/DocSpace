namespace ASC.Common.Security.Authorizing;

[Serializable]
public sealed class Role : IRole
{
    public const string Everyone = "Everyone";
    public const string Visitors = "Visitors";
    public const string Users = "Users";
    public const string Administrators = "Administrators";
    public const string System = "System";

    public Guid ID { get; internal set; }
    public string Name { get; internal set; }
    public string AuthenticationType => "ASC";
    public bool IsAuthenticated => false;


    public Role(Guid id, string name)
    {
        if (id == Guid.Empty)
        {
            throw new ArgumentException(nameof(id));
        }

        ArgumentNullOrEmptyException.ThrowIfNullOrEmpty(name);

        ID = id;
        Name = name;
    }

    public override int GetHashCode()
    {
        return ID.GetHashCode();
    }

    public override bool Equals(object obj)
    {
        return obj is Role r && r.ID == ID;
    }

    public override string ToString()
    {
        return $"Role: {Name}";
    }
}
