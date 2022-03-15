namespace ASC.Notify.Model;

[Serializable]
public class NotifyAction : INotifyAction
{
    public string ID { get; private set; }
    public string Name { get; private set; }

    public NotifyAction(string id)
        : this(id, null) { }

    public NotifyAction(string id, string name)
    {
        ID = id ?? throw new ArgumentNullException(nameof(id));
        Name = name;
    }

    public static implicit operator NotifyActionItem(NotifyAction cache)
    {
        return new NotifyActionItem() { Id = cache.ID };
    }

    public static explicit operator NotifyAction(NotifyActionItem cache)
    {
        return new NotifyAction(cache.Id);
    }

    public override bool Equals(object obj)
    {
        return obj is INotifyAction a && a.ID == ID;
    }

    public override int GetHashCode()
    {
        return ID.GetHashCode();
    }

    public override string ToString()
    {
        return $"action: {ID}";
    }
}
