namespace ASC.Data.Backup.Storage;

public interface IBackupStorage
{
    bool IsExists(string storagePath);
    string GetPublicLink(string storagePath);
    string Upload(string storageBasePath, string localPath, Guid userId);
    void Delete(string storagePath);
    void Download(string storagePath, string targetLocalPath);
}
