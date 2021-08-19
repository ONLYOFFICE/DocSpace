using ASC.Common;

namespace ASC.Webhooks
{
    [Scope]
    public interface IWebhookPublisher
    {
        public void Publish(string eventName, object data);
    }
}
