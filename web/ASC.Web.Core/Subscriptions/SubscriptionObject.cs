namespace ASC.Web.Core.Subscriptions
{
    public class SubscriptionObject
    {
        public string ID { get; set; }

        public string URL { get; set; }

        public string Name { get; set; }

        public SubscriptionType SubscriptionType { get; set; }

        public SubscriptionGroup SubscriptionGroup { get; set; }
    }
}
