using BoxSDK = Box.V2;

namespace ASC.Files.Thirdparty.Box;

internal class BoxStorage
{
    private BoxClient _boxClient;

    private readonly List<string> _boxFields = new List<string> { "created_at", "modified_at", "name", "parent", "size" };

    public bool IsOpened { get; private set; }
    private readonly TempStream _tempStream;

    public long MaxChunkedUploadFileSize = 250L * 1024L * 1024L;

    public BoxStorage(TempStream tempStream)
    {
        _tempStream = tempStream;
    }

    public void Open(OAuth20Token token)
    {
        if (IsOpened)
        {
            return;
        }

        var config = new BoxConfig(token.ClientID, token.ClientSecret, new Uri(token.RedirectUri));
        var session = new OAuthSession(token.AccessToken, token.RefreshToken, (int)token.ExpiresIn, "bearer");
        _boxClient = new BoxClient(config, session);

        IsOpened = true;
    }

    public void Close()
    {
        IsOpened = false;
    }

    public async Task<string> GetRootFolderIdAsync()
    {
        var root = await GetFolderAsync("0");

        return root.Id;
    }

    public async Task<BoxFolder> GetFolderAsync(string folderId)
    {
        try
        {
            return await _boxClient.FoldersManager.GetInformationAsync(folderId, _boxFields);
        }
        catch (Exception ex)
        {
            if (ex.InnerException is BoxSDK.Exceptions.BoxException boxException && boxException.Error.Status == ((int)HttpStatusCode.NotFound).ToString())
            {
                return null;
            }
            throw;
        }
    }

    public ValueTask<BoxFile> GetFileAsync(string fileId)
    {
        try
        {
            return new ValueTask<BoxFile>(_boxClient.FilesManager.GetInformationAsync(fileId, _boxFields));
        }
        catch (Exception ex)
        {
            if (ex.InnerException is BoxSDK.Exceptions.BoxException boxException && boxException.Error.Status == ((int)HttpStatusCode.NotFound).ToString())
            {
                return ValueTask.FromResult<BoxFile>(null);
            }
            throw;
        }
    }

    public async Task<List<BoxItem>> GetItemsAsync(string folderId, int limit = 500)
    {
        var folderItems = await _boxClient.FoldersManager.GetFolderItemsAsync(folderId, limit, 0, _boxFields);

        return folderItems.Entries;
    }

    public Task<Stream> DownloadStreamAsync(BoxFile file, int offset = 0)
    {
        ArgumentNullException.ThrowIfNull(file);

        return InternalDownloadStreamAsync(file, offset);
    }

    public async Task<Stream> InternalDownloadStreamAsync(BoxFile file, int offset = 0)
    {
        if (offset > 0 && file.Size.HasValue)
        {
            return await _boxClient.FilesManager.DownloadAsync(file.Id, startOffsetInBytes: offset, endOffsetInBytes: (int)file.Size - 1);
        }

        var str = await _boxClient.FilesManager.DownloadAsync(file.Id);
        if (offset == 0)
        {
            return str;
        }

        var tempBuffer = _tempStream.Create();
        if (str != null)
        {
            await str.CopyToAsync(tempBuffer);
            await tempBuffer.FlushAsync();
            tempBuffer.Seek(offset, SeekOrigin.Begin);

            str.Dispose();
        }

        return tempBuffer;
    }

    public Task<BoxFolder> CreateFolderAsync(string title, string parentId)
    {
        var boxFolderRequest = new BoxFolderRequest
        {
            Name = title,
            Parent = new BoxRequestEntity
            {
                Id = parentId
            }
        };

        return _boxClient.FoldersManager.CreateAsync(boxFolderRequest, _boxFields);
    }

    public Task<BoxFile> CreateFileAsync(Stream fileStream, string title, string parentId)
    {
        var boxFileRequest = new BoxFileRequest
        {
            Name = title,
            Parent = new BoxRequestEntity
            {
                Id = parentId
            }
        };

        return _boxClient.FilesManager.UploadAsync(boxFileRequest, fileStream, _boxFields, setStreamPositionToZero: false);
    }

    public async Task DeleteItemAsync(BoxItem boxItem)
    {
        if (boxItem is BoxFolder)
        {
            await _boxClient.FoldersManager.DeleteAsync(boxItem.Id, true);
        }
        else
        {
            await _boxClient.FilesManager.DeleteAsync(boxItem.Id);
        }
    }

    public Task<BoxFolder> MoveFolderAsync(string boxFolderId, string newFolderName, string toFolderId)
    {
        var boxFolderRequest = new BoxFolderRequest
        {
            Id = boxFolderId,
            Name = newFolderName,
            Parent = new BoxRequestEntity
            {
                Id = toFolderId
            }
        };

        return _boxClient.FoldersManager.UpdateInformationAsync(boxFolderRequest, _boxFields);
    }

    public Task<BoxFile> MoveFileAsync(string boxFileId, string newFileName, string toFolderId)
    {
        var boxFileRequest = new BoxFileRequest
        {
            Id = boxFileId,
            Name = newFileName,
            Parent = new BoxRequestEntity
            {
                Id = toFolderId
            }
        };

        return _boxClient.FilesManager.UpdateInformationAsync(boxFileRequest, null, _boxFields);
    }

    public Task<BoxFolder> CopyFolderAsync(string boxFolderId, string newFolderName, string toFolderId)
    {
        var boxFolderRequest = new BoxFolderRequest
        {
            Id = boxFolderId,
            Name = newFolderName,
            Parent = new BoxRequestEntity
            {
                Id = toFolderId
            }
        };

        return _boxClient.FoldersManager.CopyAsync(boxFolderRequest, _boxFields);
    }

    public Task<BoxFile> CopyFileAsync(string boxFileId, string newFileName, string toFolderId)
    {
        var boxFileRequest = new BoxFileRequest
        {
            Id = boxFileId,
            Name = newFileName,
            Parent = new BoxRequestEntity
            {
                Id = toFolderId
            }
        };

        return _boxClient.FilesManager.CopyAsync(boxFileRequest, _boxFields);
    }

    public Task<BoxFolder> RenameFolderAsync(string boxFolderId, string newName)
    {
        var boxFolderRequest = new BoxFolderRequest { Id = boxFolderId, Name = newName };

        return _boxClient.FoldersManager.UpdateInformationAsync(boxFolderRequest, _boxFields);
    }

    public Task<BoxFile> RenameFileAsync(string boxFileId, string newName)
    {
        var boxFileRequest = new BoxFileRequest { Id = boxFileId, Name = newName };

        return _boxClient.FilesManager.UpdateInformationAsync(boxFileRequest, null, _boxFields);
    }

    public Task<BoxFile> SaveStreamAsync(string fileId, Stream fileStream)
    {
        return _boxClient.FilesManager.UploadNewVersionAsync(null, fileId, fileStream, fields: _boxFields, setStreamPositionToZero: false);
    }

    public async Task<long> GetMaxUploadSizeAsync()
    {
        var boxUser = await _boxClient.UsersManager.GetCurrentUserInformationAsync(new List<string>() { "max_upload_size" });
        var max = boxUser.MaxUploadSize ?? MaxChunkedUploadFileSize;

        //todo: without chunked uploader:
        return Math.Min(max, MaxChunkedUploadFileSize);
    }
}
