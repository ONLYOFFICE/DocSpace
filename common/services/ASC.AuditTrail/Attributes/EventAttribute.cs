using System;

namespace ASC.AuditTrail.Attributes
{
    [AttributeUsage(AttributeTargets.Property)]
    public class EventAttribute : Attribute
    {
        public string Resource { get; private set; }
        public int Order { get; private set; }

        public EventAttribute(string resource, int order = 0)
        {
            Resource = resource;
            Order = order;
        }
    }
}
