namespace ASC.Data.Storage;

public class CrossModuleTransferUtility
{
    private readonly ILog _logger;
    private readonly IDataStore _source;
    private readonly IDataStore _destination;
    private readonly long _maxChunkUploadSize;
    private readonly int _chunkSize;
    private readonly IOptionsMonitor<ILog> _option;
    private readonly TempStream _tempStream;
    private readonly TempPath _tempPath;

    public CrossModuleTransferUtility(
        IOptionsMonitor<ILog> option,
        TempStream tempStream,
        TempPath tempPath,
        IDataStore source,
        IDataStore destination)
    {
        _logger = option.Get("ASC.CrossModuleTransferUtility");
        _option = option;
        _tempStream = tempStream;
        _tempPath = tempPath;
        _source = source ?? throw new ArgumentNullException(nameof(source));
        _destination = destination ?? throw new ArgumentNullException(nameof(destination));
        _maxChunkUploadSize = 10 * 1024 * 1024;
        _chunkSize = 5 * 1024 * 1024;
    }

    public Task CopyFileAsync(string srcDomain, string srcPath, string destDomain, string destPath)
    {
        ArgumentNullException.ThrowIfNull(srcDomain);
        ArgumentNullException.ThrowIfNull(srcPath);
        ArgumentNullException.ThrowIfNull(destDomain);
        ArgumentNullException.ThrowIfNull(destPath);

        return InternalCopyFileAsync(srcDomain, srcPath, destDomain, destPath);
    }

    private async Task InternalCopyFileAsync(string srcDomain, string srcPath, string destDomain, string destPath)
    {
        using var stream = await _source.GetReadStreamAsync(srcDomain, srcPath);
        if (stream.Length < _maxChunkUploadSize)
        {
            await _destination.SaveAsync(destDomain, destPath, stream);
        }
        else
        {
            var session = new CommonChunkedUploadSession(stream.Length);
            var holder = new CommonChunkedUploadSessionHolder(_tempPath, _option, _destination, destDomain);
            await holder.InitAsync(session);
            try
            {
                Stream memstream = null;
                try
                {
                    while (GetStream(stream, out memstream))
                    {
                        memstream.Seek(0, SeekOrigin.Begin);
                        await holder.UploadChunkAsync(session, memstream, _chunkSize);
                        await memstream.DisposeAsync();
                    }
                }
                finally
                {
                    if (memstream != null)
                    {
                        await memstream.DisposeAsync();
                    }
                }

                await holder.FinalizeAsync(session);
                await _destination.MoveAsync(destDomain, session.TempPath, destDomain, destPath);
            }
            catch (Exception ex)
            {
                _logger.Error("Copy File", ex);
                await holder.AbortAsync(session);
            }
        }
    }

    private bool GetStream(Stream stream, out Stream memstream)
    {
        memstream = _tempStream.Create();
        var total = 0;
        int readed;
        const int portion = 2048;
        var buffer = new byte[portion];

        while ((readed = stream.Read(buffer, 0, portion)) > 0)
        {
            memstream.Write(buffer, 0, readed);
            total += readed;
            if (total >= _chunkSize)
            {
                break;
            }
        }

        return total > 0;
    }
}
