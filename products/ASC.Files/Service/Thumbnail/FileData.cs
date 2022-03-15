namespace ASC.Files.ThumbnailBuilder;

public class FileData<T>
{
    public readonly int TenantId;
    public readonly T FileId;
    public readonly string BaseUri;

    public FileData(int tenantId, T fileId, string baseUri)
    {
        TenantId = tenantId;
        FileId = fileId;
        BaseUri = baseUri;
    }
}
