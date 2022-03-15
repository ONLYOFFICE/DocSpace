namespace ASC.Core.Common.Notify.Jabber;

public class MessageClass : IComparable<MessageClass>
{
    [JsonProperty("i")]
    public int Id { get; set; }

    [JsonProperty("u")]
    public string UserName { get; set; }

    [JsonProperty("t")]
    public string Text { get; set; }

    [JsonProperty("d")]
    public DateTime DateTime { get; set; }

    public int CompareTo(MessageClass other)
    {
        return Id.CompareTo(other.Id);
    }
}
