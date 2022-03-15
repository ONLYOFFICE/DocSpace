namespace ASC.Files.Thirdparty.Sharpbox;

[Transient]
internal class SharpBoxProviderInfo : IProviderInfo
{
    public int ID { get; set; }
    public Guid Owner { get; set; }
    public ILog Logger { get; private set; }

    private nSupportedCloudConfigurations _providerKey;
    public AuthData AuthData { get; set; }

    public SharpBoxProviderInfo(SharpBoxStorageDisposableWrapper storageDisposableWrapper, IOptionsMonitor<ILog> monitor)
    {
        _wrapper = storageDisposableWrapper;
        Logger = monitor.CurrentValue;
    }

    public void Dispose()
    {
        if (StorageOpened)
        {
            Storage.Close();
        }
    }

    internal CloudStorage Storage
    {
        get
        {
            if (_wrapper.Storage == null || !_wrapper.Storage.IsOpened)
            {
                return _wrapper.CreateStorage(AuthData, _providerKey);
            }

            return _wrapper.Storage;
        }
    }

    internal bool StorageOpened => _wrapper.Storage != null && _wrapper.Storage.IsOpened;

    public string CustomerTitle { get; set; }
    public DateTime CreateOn { get; set; }
    public string RootFolderId => "sbox-" + ID;

    public void UpdateTitle(string newtitle)
    {
        CustomerTitle = newtitle;
    }

    public Task<bool> CheckAccessAsync()
    {
        try
        {
            return Task.FromResult(Storage.GetRoot() != null);
        }
        catch (UnauthorizedAccessException)
        {
            return Task.FromResult(false);
        }
        catch (SharpBoxException ex)
        {
            Logger.Error("Sharpbox CheckAccess error", ex);

            return Task.FromResult(false);
        }
    }

    public Task InvalidateStorageAsync()
    {
        if (_wrapper != null)
        {
            _wrapper.Dispose();
        }

        return Task.CompletedTask;
    }

    public string ProviderKey
    {
        get => _providerKey.ToString();
        set => _providerKey = (nSupportedCloudConfigurations)Enum.Parse(typeof(nSupportedCloudConfigurations), value, true);
    }

    public FolderType RootFolderType { get; set; }
    private SharpBoxStorageDisposableWrapper _wrapper;
}

[Scope]
class SharpBoxStorageDisposableWrapper : IDisposable
{
    public CloudStorage Storage { get; private set; }

    public SharpBoxStorageDisposableWrapper() { }

    internal CloudStorage CreateStorage(AuthData _authData, nSupportedCloudConfigurations _providerKey)
    {
        var prms = Array.Empty<object>();
        if (!string.IsNullOrEmpty(_authData.Url))
        {
            var uri = _authData.Url;
            if (Uri.IsWellFormedUriString(uri, UriKind.Relative))
            {
                uri = Uri.UriSchemeHttp + Uri.SchemeDelimiter + uri;
            }

            prms = new object[] { new Uri(uri) };
        }

        var storage = new CloudStorage();
        var config = CloudStorage.GetCloudConfigurationEasy(_providerKey, prms);
        if (!string.IsNullOrEmpty(_authData.Token))
        {
            if (_providerKey != nSupportedCloudConfigurations.BoxNet)
            {
                var token = storage.DeserializeSecurityTokenFromBase64(_authData.Token);
                storage.Open(config, token);
            }
        }
        else
        {
            storage.Open(config, new GenericNetworkCredentials { Password = _authData.Password, UserName = _authData.Login });
        }

        return Storage = storage;
    }

    public void Dispose()
    {
        if (Storage != null && Storage.IsOpened)
        {
            Storage.Close();
            Storage = null;
        }
    }
}
