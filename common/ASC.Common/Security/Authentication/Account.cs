namespace ASC.Common.Security.Authentication;

[Serializable]
public class Account : IAccount
{
    public Guid ID { get; private set; }
    public string Name { get; private set; }
    public virtual bool IsAuthenticated { get; private set; }
    public string AuthenticationType => "ASC";

    public Account(Guid id, string name, bool authenticated)
    {
        ID = id;
        Name = name;
        IsAuthenticated = authenticated;
    }

    public object Clone()
    {
        return MemberwiseClone();
    }

    public override bool Equals(object obj)
    {
        return obj is IAccount a && ID.Equals(a.ID);
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
