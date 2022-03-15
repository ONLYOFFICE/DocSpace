namespace ASC.Data.Storage.Encryption;

[Singletone]
public class EncryptionFactory
{
    private readonly IServiceScopeFactory _serviceScopeFactory;

    public EncryptionFactory(IServiceScopeFactory serviceScopeFactory)
    {
        _serviceScopeFactory = serviceScopeFactory;
    }

    public ICrypt GetCrypt(string storageName, EncryptionSettings encryptionSettings)
    {
        ICrypt result = null;

        using var scope = _serviceScopeFactory.CreateScope();
        if (scope != null)
        {
            result = scope.ServiceProvider.GetService<ICrypt>();
        }

        result ??= new FakeCrypt();
        result.Init(storageName, encryptionSettings);

        return result;
    }
}
