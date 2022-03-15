namespace ASC.Web.Studio.UserControls.Management
{
    public class AuthService
    {
        public Consumer Consumer { get; set; }

        public string Name { get { return Consumer.Name; } }

        public string Title { get; private set; }

        public string Description { get; private set; }

        public string Instruction { get; private set; }

        public bool CanSet { get { return Consumer.CanSet; } }

        public int? Order { get { return Consumer.Order; } }

        public List<AuthKey> Props { get; private set; }

        public AuthService(Consumer consumer)
        {
            Consumer = consumer;
            Title = ConsumerExtension.GetResourceString(consumer.Name) ?? consumer.Name;
            Description = ConsumerExtension.GetResourceString(consumer.Name + "Description");
            Instruction = ConsumerExtension.GetResourceString(consumer.Name + "InstructionV11");
            Props = new List<AuthKey>();

            foreach (var item in consumer.ManagedKeys)
            {
                Props.Add(new AuthKey { Name = item, Value = Consumer[item], Title = ConsumerExtension.GetResourceString(item) ?? item });
            }
        }
    }

    public static class ConsumerExtension
    {
        public static string GetResourceString(string resourceKey)
        {
            try
            {
                Resource.ResourceManager.IgnoreCase = true;
                return Resource.ResourceManager.GetString("Consumers" + resourceKey);
            }
            catch
            {
                return null;
            }
        }
    }

    [DebuggerDisplay("({Name},{Value})")]
    public class AuthKey
    {
        public string Name { get; set; }

        public string Value { get; set; }

        public string Title { get; set; }
    }
}
