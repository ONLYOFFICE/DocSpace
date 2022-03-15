namespace ASC.Data.Backup.Storage;

[Scope]
public class DataStoreBackupStorage : IBackupStorage
{
    private string _webConfigPath;
    private int _tenant;
    private readonly StorageFactory _storageFactory;

    public DataStoreBackupStorage(StorageFactory storageFactory)
    {
        _storageFactory = storageFactory;
    }

    public void Init(int tenant, string webConfigPath)
    {
        _webConfigPath = webConfigPath;
        _tenant = tenant;
    }

    public string Upload(string storageBasePath, string localPath, Guid userId)
    {
        using var stream = File.OpenRead(localPath);
        var storagePath = Path.GetFileName(localPath);
            GetDataStore().SaveAsync("", storagePath, stream).Wait();

        return storagePath;
    }

    public void Download(string storagePath, string targetLocalPath)
    {
            using var source = GetDataStore().GetReadStreamAsync("", storagePath).Result;
        using var destination = File.OpenWrite(targetLocalPath);
        source.CopyTo(destination);
    }

    public void Delete(string storagePath)
    {
        var dataStore = GetDataStore();
            if (dataStore.IsFileAsync("", storagePath).Result)
        {
                dataStore.DeleteAsync("", storagePath).Wait();
        }
    }

    public bool IsExists(string storagePath)
    {
            return GetDataStore().IsFileAsync("", storagePath).Result;
    }

    public string GetPublicLink(string storagePath)
    {
            return GetDataStore().GetPreSignedUriAsync("", storagePath, TimeSpan.FromDays(1), null).Result.ToString();
    }

    protected virtual IDataStore GetDataStore()
    {
        return _storageFactory.GetStorage(_webConfigPath, _tenant.ToString(), "backup", null);
    }
}
