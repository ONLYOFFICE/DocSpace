namespace ASC.Data.Storage.Encryption;

[Singletone]
public class EncryptionWorker
{
    private readonly object _locker;
    private readonly FactoryOperation _factoryOperation;
    private readonly DistributedTaskQueue _queue;

    public EncryptionWorker(FactoryOperation factoryOperation, DistributedTaskQueueOptionsManager options)
    {
        _locker = new object();
        _factoryOperation = factoryOperation;
        _queue = options.Get<EncryptionOperation>();
    }

    public void Start(EncryptionSettingsProto encryptionSettings)
    {
        EncryptionOperation encryptionOperation;
        lock (_locker)
        {
            if (_queue.GetTask<EncryptionOperation>(GetCacheId()) != null)
            {
                return;
            }

            encryptionOperation = _factoryOperation.CreateOperation(encryptionSettings, GetCacheId());
            _queue.QueueTask(encryptionOperation);
        }
    }

    public void Stop()
    {
        _queue.CancelTask(GetCacheId());
    }

    public string GetCacheId()
    {
        return typeof(EncryptionOperation).FullName;
    }

    public double? GetEncryptionProgress()
    {
        var progress = _queue.GetTasks<EncryptionOperation>().FirstOrDefault();

        return progress.Percentage;
    }
}

[Singletone(Additional = typeof(FactoryOperationExtension))]
public class FactoryOperation
{
    private readonly IServiceProvider _serviceProvider;

    public FactoryOperation(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }

    public EncryptionOperation CreateOperation(EncryptionSettingsProto encryptionSettings, string id)
    {
        var item = _serviceProvider.GetService<EncryptionOperation>();
        item.Init(encryptionSettings, id);

        return item;
    }
}

public static class FactoryOperationExtension
{
    public static void Register(DIHelper dIHelper)
    {
        dIHelper.TryAdd<EncryptionOperation>();
        dIHelper.AddDistributedTaskQueueService<EncryptionOperation>(1);
    }
}
