namespace ASC.Data.Backup.Storage;

[Scope]
public class ConsumerBackupStorage : IBackupStorage
{
    private const string Domain = "backup";

    private IDataStore _store;
    private readonly StorageSettingsHelper _storageSettingsHelper;

    public ConsumerBackupStorage(StorageSettingsHelper storageSettingsHelper)
    {
        _storageSettingsHelper = storageSettingsHelper;
    }

    public void Init(IReadOnlyDictionary<string, string> storageParams)
    {
        var settings = new StorageSettings { Module = storageParams["module"], Props = storageParams.Where(r => r.Key != "module").ToDictionary(r => r.Key, r => r.Value) };
        _store = _storageSettingsHelper.DataStore(settings);
    }

    public string Upload(string storageBasePath, string localPath, Guid userId)
    {
        using var stream = File.OpenRead(localPath);
        var storagePath = Path.GetFileName(localPath);
            _store.SaveAsync(Domain, storagePath, stream, ACL.Private).Wait();
        return storagePath;
    }

    public void Download(string storagePath, string targetLocalPath)
    {
        using var source = _store.GetReadStreamAsync(Domain, storagePath).Result;
        using var destination = File.OpenWrite(targetLocalPath);
        source.CopyTo(destination);
    }

    public void Delete(string storagePath)
    {
            if (_store.IsFileAsync(Domain, storagePath).Result)
        {
                _store.DeleteAsync(Domain, storagePath).Wait();
        }
    }

    public bool IsExists(string storagePath)
    {
            return _store.IsFileAsync(Domain, storagePath).Result;
    }

    public string GetPublicLink(string storagePath)
    {
            return _store.GetInternalUriAsync(Domain, storagePath, TimeSpan.FromDays(1), null).Result.AbsoluteUri;
    }
}
