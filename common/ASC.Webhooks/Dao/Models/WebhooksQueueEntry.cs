namespace ASC.Webhooks.Dao.Models
{
    public class WebhooksQueueEntry
    {
        public int Id { get; set; }
        public string Data { get; set; }
        public string Uri { get; set; }
        public string SecretKey { get; set; }
    }
}
