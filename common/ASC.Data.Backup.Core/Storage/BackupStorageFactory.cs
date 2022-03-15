namespace ASC.Data.Backup.Storage;

[Scope]
public class BackupStorageFactory
{
    private readonly ConfigurationExtension _configuration;
    private readonly DocumentsBackupStorage _documentsBackupStorage;
    private readonly DataStoreBackupStorage _dataStoreBackupStorage;
    private readonly ILog _logger;
    private readonly LocalBackupStorage _localBackupStorage;
    private readonly ConsumerBackupStorage _consumerBackupStorage;
    private readonly TenantManager _tenantManager;

    public BackupStorageFactory(
        ConsumerBackupStorage consumerBackupStorage,
        LocalBackupStorage localBackupStorage,
        ConfigurationExtension configuration,
        DocumentsBackupStorage documentsBackupStorage,
        TenantManager tenantManager,
        DataStoreBackupStorage dataStoreBackupStorage,
        IOptionsMonitor<ILog> options)
    {
        _configuration = configuration;
        _documentsBackupStorage = documentsBackupStorage;
        _dataStoreBackupStorage = dataStoreBackupStorage;
        _logger = options.CurrentValue;
        _localBackupStorage = localBackupStorage;
        _consumerBackupStorage = consumerBackupStorage;
        _tenantManager = tenantManager;
    }

    public IBackupStorage GetBackupStorage(BackupRecord record)
    {
        try
        {
            return GetBackupStorage(record.StorageType, record.TenantId, JsonConvert.DeserializeObject<Dictionary<string, string>>(record.StorageParams));
        }
        catch (Exception error)
        {
            _logger.Error("can't get backup storage for record " + record.Id, error);

            return null;
        }
    }

    public IBackupStorage GetBackupStorage(BackupStorageType type, int tenantId, Dictionary<string, string> storageParams)
    {
        var settings = _configuration.GetSetting<BackupSettings>("backup");
        var webConfigPath = PathHelper.ToRootedConfigPath(settings.WebConfigs.CurrentPath);


        switch (type)
        {
            case BackupStorageType.Documents:
            case BackupStorageType.ThridpartyDocuments:
            {
                _documentsBackupStorage.Init(tenantId, webConfigPath);

                return _documentsBackupStorage;
            }
            case BackupStorageType.DataStore:
            {
                _dataStoreBackupStorage.Init(tenantId, webConfigPath);

                return _dataStoreBackupStorage;
            }
            case BackupStorageType.Local:
                return _localBackupStorage;
            case BackupStorageType.ThirdPartyConsumer:
            {
                if (storageParams == null)
                {
                    return null;
                }

                _tenantManager.SetCurrentTenant(tenantId);
                _consumerBackupStorage.Init(storageParams);

                return _consumerBackupStorage;
            }
            default:
                throw new InvalidOperationException("Unknown storage type.");
        }
    }
}
