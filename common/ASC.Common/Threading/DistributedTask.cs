using JsonSerializer = System.Text.Json.JsonSerializer;

namespace ASC.Common.Threading;

public class DistributedTask
{
    public Action<DistributedTask> Publication { get; set; }
    public int InstanceId
    {
        get => DistributedTaskCache.InstanceId;
        set => DistributedTaskCache.InstanceId = value;
    }
    public string Id
    {
        get => DistributedTaskCache.Id;
        protected set => DistributedTaskCache.Id = value ?? "";
    }
    public DistributedTaskStatus Status
    {
        get => Enum.Parse<DistributedTaskStatus>(DistributedTaskCache.Status);
        set => DistributedTaskCache.Status = value.ToString();
    }
    public Exception Exception
    {
        get => new Exception(DistributedTaskCache.Exception);
        set => DistributedTaskCache.Exception = value?.ToString() ?? "";
    }
    protected internal DistributedTaskCache DistributedTaskCache { get; internal set; }

    public DistributedTask()
    {
        DistributedTaskCache = new DistributedTaskCache
        {
            Id = Guid.NewGuid().ToString()
        };
    }

    public DistributedTask(DistributedTaskCache distributedTaskCache)
    {
        DistributedTaskCache = distributedTaskCache;
    }

    public T GetProperty<T>(string name)
    {
        var prop = DistributedTaskCache.Props.FirstOrDefault(r => r.Key == name);
        if (prop == null)
        {
            return default;
        }

        return JsonSerializer.Deserialize<T>(prop.Value);
    }

    public void SetProperty(string name, object value)
    {
        var prop = new DistributedTaskCache.Types.DistributedTaskCacheProp()
        {
            Key = name,
            Value = JsonSerializer.Serialize(value)
        };

        var current = DistributedTaskCache.Props.SingleOrDefault(r => r.Key == name);
        if (current != null)
        {
            DistributedTaskCache.Props.Remove(current);
        }

        if (value != null)
        {
            DistributedTaskCache.Props.Add(prop);
        }
    }

    public void PublishChanges()
    {
        if (Publication == null)
        {
            throw new InvalidOperationException("Publication not found.");
        }

        Publication(this);
    }
}