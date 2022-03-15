namespace ASC.Web.Files.Utils;

[Scope]
public class ChunkedUploadSessionHolder
{
    public static readonly TimeSpan SlidingExpiration = TimeSpan.FromHours(12);

    private readonly IOptionsMonitor<ILog> _options;
    private readonly GlobalStore _globalStore;
    private readonly SetupInfo _setupInfo;
    private readonly TempPath _tempPath;
    private readonly FileHelper _fileHelper;

    public ChunkedUploadSessionHolder(
        IOptionsMonitor<ILog> options,
        GlobalStore globalStore,
        SetupInfo setupInfo,
        TempPath tempPath,
        FileHelper fileHelper)
    {
        _options = options;
        _globalStore = globalStore;
        _setupInfo = setupInfo;
        _tempPath = tempPath;
        _fileHelper = fileHelper;

        // clear old sessions
        //TODO
        //try
        //{
        //    CommonSessionHolder(false).DeleteExpired();
        //}
        //catch (Exception err)
        //{
        //    options.CurrentValue.Error(err);
        //}
    }

    public async Task StoreSessionAsync<T>(ChunkedUploadSession<T> s)
    {
        await CommonSessionHolder(false).StoreAsync(s);
    }

    public async Task RemoveSessionAsync<T>(ChunkedUploadSession<T> s)
    {
        await CommonSessionHolder(false).RemoveAsync(s);
    }

    public async Task<ChunkedUploadSession<T>> GetSessionAsync<T>(string sessionId)
    {
        using var stream = await CommonSessionHolder(false).GetStreamAsync(sessionId);
        var chunkedUploadSession = ChunkedUploadSession<T>.Deserialize(stream, _fileHelper);

        return chunkedUploadSession;
    }

    public async Task<ChunkedUploadSession<T>> CreateUploadSessionAsync<T>(File<T> file, long contentLength)
    {
        var result = new ChunkedUploadSession<T>(file, contentLength);
        await CommonSessionHolder().InitAsync(result);

        return result;
    }

    public async Task UploadChunkAsync<T>(ChunkedUploadSession<T> uploadSession, Stream stream, long length)
    {
        await CommonSessionHolder().UploadChunkAsync(uploadSession, stream, length);
    }

    public async Task FinalizeUploadSessionAsync<T>(ChunkedUploadSession<T> uploadSession)
    {
        await CommonSessionHolder().FinalizeAsync(uploadSession);
    }

    public async Task MoveAsync<T>(ChunkedUploadSession<T> chunkedUploadSession, string newPath)
    {
        await CommonSessionHolder().MoveAsync(chunkedUploadSession, newPath, chunkedUploadSession.CheckQuota);
    }

    public async Task AbortUploadSessionAsync<T>(ChunkedUploadSession<T> uploadSession)
    {
        await CommonSessionHolder().AbortAsync(uploadSession);
    }

    public Stream UploadSingleChunk<T>(ChunkedUploadSession<T> uploadSession, Stream stream, long chunkLength)
    {
        return CommonSessionHolder().UploadSingleChunk(uploadSession, stream, chunkLength);
    }

    public Task<Stream> UploadSingleChunkAsync<T>(ChunkedUploadSession<T> uploadSession, Stream stream, long chunkLength)
    {
        return CommonSessionHolder().UploadSingleChunkAsync(uploadSession, stream, chunkLength);
    }

    private CommonChunkedUploadSessionHolder CommonSessionHolder(bool currentTenant = true)
    {
        return new CommonChunkedUploadSessionHolder(_tempPath, _options, _globalStore.GetStore(currentTenant), FileConstant.StorageDomainTmp, _setupInfo.ChunkUploadSize);
    }
}
