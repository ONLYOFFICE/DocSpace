namespace ASC.Core.Common.Notify.Push;

public class PushNotification
{
    public int Id { get; set; }
    public string Message { get; set; }
    public string ShortMessage { get; set; }
    public int? Badge { get; set; }
    public PushModule Module { get; set; }
    public PushAction Action { get; set; }
    public PushItem Item { get; set; }
    public PushItem ParentItem { get; set; }
    public DateTime QueuedOn { get; set; }

    public static PushNotification ApiNotification(string message, int? badge)
    {
        return new PushNotification { Message = message, Badge = badge };
    }
}

public class PushItem
{
    public PushItemType Type { get; set; }
    public string ID { get; set; }
    public string Description { get; set; }

    public PushItem() { }

    public PushItem(PushItemType type, string id, string description)
    {
        Type = type;
        ID = id;
        Description = description;
    }
}
