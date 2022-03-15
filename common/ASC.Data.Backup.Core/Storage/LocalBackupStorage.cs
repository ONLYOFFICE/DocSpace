namespace ASC.Data.Backup.Storage;

[Scope]
public class LocalBackupStorage : IBackupStorage
{
    public string Upload(string storageBasePath, string localPath, Guid userId)
    {
        if (!Directory.Exists(storageBasePath))
        {
            throw new FileNotFoundException("Directory not found.");
        }

        var storagePath = CrossPlatform.PathCombine(storageBasePath, Path.GetFileName(localPath));
        if (localPath != storagePath)
        {
            File.Copy(localPath, storagePath, true);
        }

        return storagePath;
    }

    public void Download(string storagePath, string targetLocalPath)
    {
        File.Copy(storagePath, targetLocalPath, true);
    }

    public void Delete(string storagePath)
    {
        File.Delete(storagePath);
    }

    public bool IsExists(string storagePath)
    {
        return File.Exists(storagePath);
    }

    public string GetPublicLink(string storagePath)
    {
        return string.Empty;
    }
}
