namespace ASC.Data.Storage.Migration;

[ServiceContract]
public interface IService
{
    [OperationContract]
    void Migrate(int tenant, StorageSettings storageSettings);

    [OperationContract]
    double GetProgress(int tenant);

    [OperationContract]
    void StopMigrate();

    [OperationContract]
    void UploadCdn(int tenant, string relativePath, string mappedPath, CdnStorageSettings cdnStorageSettings = null);
}
