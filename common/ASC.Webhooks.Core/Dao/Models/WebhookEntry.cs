namespace ASC.Webhooks.Core.Dao.Models;

public class WebhookEntry
{
    public int Id { get; set; }
    public string Payload { get; set; }
    public string SecretKey { get; set; }
    public string Uri { get; set; }
    public override bool Equals(object other)
    {
        var toCompareWith = other as WebhookEntry;
        if (toCompareWith == null)
            return false;
        return Id == toCompareWith.Id &&
            Payload == toCompareWith.Payload &&
            Uri == toCompareWith.Uri &&
            SecretKey == toCompareWith.SecretKey;
    }

    public override int GetHashCode()
    {
        return Id.GetHashCode();
    }
}
