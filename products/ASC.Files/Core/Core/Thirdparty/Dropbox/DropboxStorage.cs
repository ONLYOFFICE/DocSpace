namespace ASC.Files.Thirdparty.Dropbox;

internal class DropboxStorage : IDisposable
{
    public bool IsOpened { get; private set; }
    public long MaxChunkedUploadFileSize = 20L * 1024L * 1024L * 1024L;

    private DropboxClient _dropboxClient;
    private readonly TempStream _tempStream;

    public DropboxStorage(TempStream tempStream)
    {
        _tempStream = tempStream;
    }

    public void Open(OAuth20Token token)
    {
        if (IsOpened)
        {
            return;
        }

        _dropboxClient = new DropboxClient(token.AccessToken);

        IsOpened = true;
    }

    public void Close()
    {
        _dropboxClient.Dispose();

        IsOpened = false;
    }



    public string MakeDropboxPath(string parentPath, string name)
    {
        return (parentPath ?? "") + "/" + (name ?? "");
    }

    public async Task<long> GetUsedSpaceAsync()
    {
        var spaceUsage = await _dropboxClient.Users.GetSpaceUsageAsync();

        return (long)spaceUsage.Used;
    }


    public Task<FolderMetadata> GetFolderAsync(string folderPath)
    {
        if (string.IsNullOrEmpty(folderPath) || folderPath == "/")
        {
            return Task.FromResult(new FolderMetadata(string.Empty, "/"));
        }

        return InternalGetFolderAsync(folderPath);
    }

    public async Task<FolderMetadata> InternalGetFolderAsync(string folderPath)
    {
        try
        {
            var metadata = await _dropboxClient.Files.GetMetadataAsync(folderPath);

            return metadata.AsFolder;
        }
        catch (AggregateException ex)
        {
            if (ex.InnerException is ApiException<GetMetadataError>
                && ex.InnerException.Message.StartsWith("path/not_found/"))
            {
                return null;
            }
            throw;
        }
    }

    public ValueTask<FileMetadata> GetFileAsync(string filePath)
    {
        if (string.IsNullOrEmpty(filePath) || filePath == "/")
        {
            return ValueTask.FromResult<FileMetadata>(null);
        }

        return InternalGetFileAsync(filePath);
    }

    private async ValueTask<FileMetadata> InternalGetFileAsync(string filePath)
    {
        try
        {
            var data = await _dropboxClient.Files.GetMetadataAsync(filePath);

            return data.AsFile;
        }
        catch (AggregateException ex)
        {
            if (ex.InnerException is ApiException<GetMetadataError>
                && ex.InnerException.Message.StartsWith("path/not_found/"))
            {
                return null;
            }
            throw;
        }
    }


    public async Task<List<Metadata>> GetItemsAsync(string folderPath)
    {
        var data = await _dropboxClient.Files.ListFolderAsync(folderPath);

        return new List<Metadata>(data.Entries);
    }

    public Task<Stream> DownloadStreamAsync(string filePath, int offset = 0)
    {
        ArgumentNullOrEmptyException.ThrowIfNullOrEmpty(filePath);

        return InternalDownloadStreamAsync(filePath, offset);
    }

    public async Task<Stream> InternalDownloadStreamAsync(string filePath, int offset = 0)
    {
        using var response = await _dropboxClient.Files.DownloadAsync(filePath);
        var tempBuffer = _tempStream.Create();
        using (var str = await response.GetContentAsStreamAsync())
        {
            if (str != null)
            {
                await str.CopyToAsync(tempBuffer);
                await tempBuffer.FlushAsync();
                tempBuffer.Seek(offset, SeekOrigin.Begin);
            }
        }

        return tempBuffer;
    }

    public async Task<FolderMetadata> CreateFolderAsync(string title, string parentPath)
    {
        var path = MakeDropboxPath(parentPath, title);
        var result = await _dropboxClient.Files.CreateFolderV2Async(path, true);

        return result.Metadata;
    }

    public Task<FileMetadata> CreateFileAsync(Stream fileStream, string title, string parentPath)
    {
        var path = MakeDropboxPath(parentPath, title);

        return _dropboxClient.Files.UploadAsync(path, WriteMode.Add.Instance, true, body: fileStream);
    }

    public Task DeleteItemAsync(Metadata dropboxItem)
    {
        return _dropboxClient.Files.DeleteV2Async(dropboxItem.PathDisplay);
    }

    public async Task<FolderMetadata> MoveFolderAsync(string dropboxFolderPath, string dropboxFolderPathTo, string folderName)
    {
        var pathTo = MakeDropboxPath(dropboxFolderPathTo, folderName);
        var result = await _dropboxClient.Files.MoveV2Async(dropboxFolderPath, pathTo, autorename: true);

        return (FolderMetadata)result.Metadata;
    }

    public async Task<FileMetadata> MoveFileAsync(string dropboxFilePath, string dropboxFolderPathTo, string fileName)
    {
        var pathTo = MakeDropboxPath(dropboxFolderPathTo, fileName);
        var result = await _dropboxClient.Files.MoveV2Async(dropboxFilePath, pathTo, autorename: true);

        return (FileMetadata)result.Metadata;
    }

    public async Task<FolderMetadata> CopyFolderAsync(string dropboxFolderPath, string dropboxFolderPathTo, string folderName)
    {
        var pathTo = MakeDropboxPath(dropboxFolderPathTo, folderName);
        var result = await _dropboxClient.Files.CopyV2Async(dropboxFolderPath, pathTo, autorename: true);

        return (FolderMetadata)result.Metadata;
    }

    public async Task<FileMetadata> CopyFileAsync(string dropboxFilePath, string dropboxFolderPathTo, string fileName)
    {
        var pathTo = MakeDropboxPath(dropboxFolderPathTo, fileName);
        var result = await _dropboxClient.Files.CopyV2Async(dropboxFilePath, pathTo, autorename: true);

        return (FileMetadata)result.Metadata;
    }

    public async Task<FileMetadata> SaveStreamAsync(string filePath, Stream fileStream)
    {
        var metadata = await _dropboxClient.Files.UploadAsync(filePath, WriteMode.Overwrite.Instance, body: fileStream);

        return metadata.AsFile;
    }

    public async Task<string> CreateResumableSessionAsync()
    {
        var session = await _dropboxClient.Files.UploadSessionStartAsync(body: new MemoryStream());

        return session.SessionId;
    }

    public Task TransferAsync(string dropboxSession, long offset, Stream stream)
    {
        return _dropboxClient.Files.UploadSessionAppendV2Async(new UploadSessionCursor(dropboxSession, (ulong)offset), body: stream);
    }

    public Task<Metadata> FinishResumableSessionAsync(string dropboxSession, string dropboxFolderPath, string fileName, long offset)
    {
        var dropboxFilePath = MakeDropboxPath(dropboxFolderPath, fileName);
        return FinishResumableSessionAsync(dropboxSession, dropboxFilePath, offset);
    }

    public async Task<Metadata> FinishResumableSessionAsync(string dropboxSession, string dropboxFilePath, long offset)
    {
        return await _dropboxClient.Files.UploadSessionFinishAsync(
            new UploadSessionCursor(dropboxSession, (ulong)offset),
            new CommitInfo(dropboxFilePath, WriteMode.Overwrite.Instance),
            new MemoryStream());
    }

    public void Dispose()
    {
        if (_dropboxClient != null)
        {
            _dropboxClient.Dispose();
        }
    }
}
