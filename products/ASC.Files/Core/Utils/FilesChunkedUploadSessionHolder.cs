namespace ASC.Web.Files.Utils;

public class FilesChunkedUploadSessionHolder : CommonChunkedUploadSessionHolder
{
    private readonly IDaoFactory _daoFactory;
    public FilesChunkedUploadSessionHolder(IDaoFactory daoFactory, TempPath tempPath, ILogger logger, IDataStore dataStore, string domain, long maxChunkUploadSize = 10485760)
        : base(tempPath, logger, dataStore, domain, maxChunkUploadSize)
    {
        _daoFactory = daoFactory;
    }
    public override async Task<string> UploadChunkAsync(CommonChunkedUploadSession uploadSession, Stream stream, long length)
    {
        if (uploadSession is ChunkedUploadSession<int>)
        {
            return (await InternalUploadChunkAsync<int>(uploadSession, stream, length)).ToString();
        }
        else
        {
            return await InternalUploadChunkAsync<string>(uploadSession, stream, length);
        }
    }

    private async Task<T> InternalUploadChunkAsync<T>(CommonChunkedUploadSession uploadSession, Stream stream, long length)
    {
        var chunkedUploadSession = uploadSession as ChunkedUploadSession<T>;
        chunkedUploadSession.File.ContentLength += stream.Length;
        var fileDao = GetFileDao<T>();
        var file = await fileDao.UploadChunkAsync(chunkedUploadSession, stream, length);
        return file.Id;
    }

    public override async Task<string> FinalizeAsync(CommonChunkedUploadSession uploadSession)
    {
        if (uploadSession is ChunkedUploadSession<int>)
        {
            return (await InternalFinalizeAsync<int>(uploadSession)).ToString();
        }
        else
        {
            return await InternalFinalizeAsync<string>(uploadSession);
        }
    }

    private async Task<T> InternalFinalizeAsync<T>(CommonChunkedUploadSession commonChunkedUploadSession)
    {
        var chunkedUploadSession = commonChunkedUploadSession as ChunkedUploadSession<T>;
        chunkedUploadSession.BytesTotal = chunkedUploadSession.BytesUploaded;
        var fileDao = GetFileDao<T>();
        var file = await fileDao.FinalizeUploadSessionAsync(chunkedUploadSession);
        return file.Id;
    }

    private IFileDao<T> GetFileDao<T>()
    {
        return _daoFactory.GetFileDao<T>();
    }
}
