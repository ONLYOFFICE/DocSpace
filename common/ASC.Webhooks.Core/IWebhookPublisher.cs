using ASC.Common;

namespace ASC.Webhooks.Core
{
    [Scope]
    public interface IWebhookPublisher
    {
        public void Publish(string eventName, object data);
    }
}
