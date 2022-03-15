using CacheNotifyAction = ASC.Common.Caching.CacheNotifyAction;

namespace ASC.Data.Storage.Encryption;

[Scope]
public class EncryptionServiceClient : IEncryptionService
{
    private readonly ICacheNotify<EncryptionSettingsProto> _notifySetting;
    private readonly ICacheNotify<EncryptionStop> _notifyStop;

    public EncryptionServiceClient(
        ICacheNotify<EncryptionSettingsProto> notifySetting, ICacheNotify<EncryptionStop> notifyStop)
    {
        _notifySetting = notifySetting;
        _notifyStop = notifyStop;
    }

    public void Start(EncryptionSettingsProto encryptionSettingsProto)
    {
        _notifySetting.Publish(encryptionSettingsProto, CacheNotifyAction.Insert);
    }

    public void Stop()
    {
        _notifyStop.Publish(new EncryptionStop(), CacheNotifyAction.Insert);
    }
}
