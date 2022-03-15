namespace ASC.AuditTrail.Models;

public class BaseEvent
{
    public int Id { get; set; }
    public int TenantId { get; set; }
    public Guid UserId { get; set; }
    public bool Mobile { get; set; }
    public IList<string> Description { get; set; }

    [Event("IpCol")]
    public string Ip { get; set; }

    [Event("BrowserCol")]
    public string Browser { get; set; }

    [Event("PlatformCol")]
    public string Platform { get; set; }

    [Event("DateCol")]
    public DateTime Date { get; set; }

    [Event("UserCol")]
    public string UserName { get; set; }

    [Event("PageCol")]
    public string Page { get; set; }

    [Event("ActionCol")]
    public string ActionText { get; set; }
}

internal class BaseEventMap<T> : ClassMap<T> where T : BaseEvent
{
    public BaseEventMap()
    {
        var eventType = typeof(T);
        var eventProps = eventType
            .GetProperties()
            .Where(r => r.GetCustomAttribute<EventAttribute>() != null)
            .OrderBy(r => r.GetCustomAttribute<EventAttribute>().Order);

        foreach (var prop in eventProps)
        {
            var attr = prop.GetCustomAttribute<EventAttribute>().Resource;
            Map(eventType, prop).Name(AuditReportResource.ResourceManager.GetString(attr));
        }
    }
}