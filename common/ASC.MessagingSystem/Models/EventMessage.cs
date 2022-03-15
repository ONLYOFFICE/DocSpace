namespace ASC.MessagingSystem.Models;

public class EventMessage
{
    public int Id { get; set; }
    public string Ip { get; set; }
    public string Initiator { get; set; }
    public string Browser { get; set; }
    public string Platform { get; set; }
    public DateTime Date { get; set; }
    public int TenantId { get; set; }
    public Guid UserId { get; set; }
    public string Page { get; set; }
    public MessageAction Action { get; set; }
    public IList<string> Description { get; set; }
    public MessageTarget Target { get; set; }
    public string UAHeader { get; set; }
}
