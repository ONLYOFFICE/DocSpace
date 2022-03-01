namespace ASC.MessagingSystem.Mapping;

public class EventTypeConverter : ITypeConverter<EventMessage, LoginEvent>, ITypeConverter<EventMessage, AuditEvent>
{
    public LoginEvent Convert(EventMessage source, LoginEvent destination, ResolutionContext context)
    {
        var messageEvent = context.Mapper.Map<EventMessage, MessageEvent>(source);
        var loginEvent = context.Mapper.Map<MessageEvent, LoginEvent>(messageEvent);

        loginEvent.Login = source.Initiator;

        if (source.Description != null && source.Description.Count > 0)
        {
            loginEvent.DescriptionRaw =
                JsonConvert.SerializeObject(source.Description, new JsonSerializerSettings
                {
                    DateTimeZoneHandling = DateTimeZoneHandling.Utc
                });
        }

        return loginEvent;
    }

    public AuditEvent Convert(EventMessage source, AuditEvent destination, ResolutionContext context)
    {
        var messageEvent = context.Mapper.Map<EventMessage, MessageEvent>(source);
        var auditEvent = context.Mapper.Map<MessageEvent, AuditEvent>(messageEvent);

        auditEvent.Initiator = source.Initiator;
        auditEvent.Target = source.Target?.ToString();

        if (source.Description != null && source.Description.Count > 0)
        {
            auditEvent.DescriptionRaw =
                JsonConvert.SerializeObject(GetSafeDescription(source.Description), new JsonSerializerSettings
                {
                    DateTimeZoneHandling = DateTimeZoneHandling.Utc
                });
        }

        return auditEvent;
    }

    private static IList<string> GetSafeDescription(IEnumerable<string> description)
    {
        const int maxLength = 15000;

        var currentLength = 0;
        var safe = new List<string>();

        foreach (var d in description.Where(r => r != null))
        {
            if (currentLength + d.Length <= maxLength)
            {
                currentLength += d.Length;
                safe.Add(d);
            }
            else
            {
                safe.Add(d.Substring(0, maxLength - currentLength - 3) + "...");
                break;
            }
        }

        return safe;
    }
}
