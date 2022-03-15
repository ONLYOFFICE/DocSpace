namespace ASC.MessagingSystem.Models;

[Singletone]
public class MessageTarget
{
    private IEnumerable<string> _items;
    private readonly ILog _logger;
    private readonly IOptionsMonitor<ILog> _option;

    public MessageTarget(IOptionsMonitor<ILog> option)
    {
        _logger = option.Get("ASC.Messaging");
        _option = option;
    }

    public MessageTarget Create<T>(T value)
    {
        try
        {
            var res = new List<string>();

            if (value is System.Collections.IEnumerable ids)
            {
                res.AddRange(from object id in ids select id.ToString());
            }
            else
            {
                res.Add(value.ToString());
            }

            return new MessageTarget(_option)
            {
                _items = res.Distinct()
            };
        }
        catch (Exception e)
        {
            _logger.Error("EventMessageTarget exception", e);

            return null;
        }

    }

    public MessageTarget Create(IEnumerable<string> value)
    {
        try
        {
            return new MessageTarget(_option)
            {
                _items = value.Distinct()
            };
        }
        catch (Exception e)
        {
            _logger.Error("EventMessageTarget exception", e);

            return null;
        }
    }

    public MessageTarget Parse(string value)
    {
        if (string.IsNullOrEmpty(value))
        {
            return null;
        }

        var items = value.Split(',');

        if (items.Length == 0)
        {
            return null;
        }

        return new MessageTarget(_option)
        {
            _items = items
        };
    }

    public override string ToString()
    {
        return string.Join(",", _items);
    }
}
