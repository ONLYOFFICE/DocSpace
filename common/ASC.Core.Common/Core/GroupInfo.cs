namespace ASC.Core.Users;

[Serializable]
public class GroupInfo : IRole, IRecipientsGroup
{
    public Guid ID { get; internal set; }
    public string Name { get; set; }
    public Guid CategoryID { get; set; }
    public GroupInfo Parent { get; internal set; }
    public string Sid { get; set; }

    public GroupInfo() { }

    public GroupInfo(Guid categoryID)
    {
        CategoryID = categoryID;
    }

    public override string ToString()
    {
        return Name;
    }

    public override int GetHashCode()
    {
        return ID != Guid.Empty ? ID.GetHashCode() : base.GetHashCode();
    }

    public override bool Equals(object obj)
    {
        if (!(obj is GroupInfo g))
        {
            return false;
        }

        if (ID == Guid.Empty && g.ID == Guid.Empty)
        {
            return ReferenceEquals(this, g);
        }

        return g.ID == ID;
    }

    string IRecipient.ID => ID.ToString();
    string IRecipient.Name => Name;
    public string AuthenticationType => "ASC";
    public bool IsAuthenticated => false;
}
