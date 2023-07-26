// (c) Copyright Ascensio System SIA 2010-2022
//
// This program is a free software product.
// You can redistribute it and/or modify it under the terms
// of the GNU Affero General Public License (AGPL) version 3 as published by the Free Software
// Foundation. In accordance with Section 7(a) of the GNU AGPL its Section 15 shall be amended
// to the effect that Ascensio System SIA expressly excludes the warranty of non-infringement of
// any third-party rights.
//
// This program is distributed WITHOUT ANY WARRANTY, without even the implied warranty
// of MERCHANTABILITY or FITNESS FOR A PARTICULAR  PURPOSE. For details, see
// the GNU AGPL at: http://www.gnu.org/licenses/agpl-3.0.html
//
// You can contact Ascensio System SIA at Lubanas st. 125a-25, Riga, Latvia, EU, LV-1021.
//
// The  interactive user interfaces in modified source and object code versions of the Program must
// display Appropriate Legal Notices, as required under Section 5 of the GNU AGPL version 3.
//
// Pursuant to Section 7(b) of the License you must retain the original Product logo when
// distributing the program. Pursuant to Section 7(e) we decline to grant you any rights under
// trademark law for use of our trademarks.
//
// All the Product's GUI elements, including illustrations and icon sets, as well as technical writing
// content are licensed under the terms of the Creative Commons Attribution-ShareAlike 4.0
// International. See the License terms at http://creativecommons.org/licenses/by-sa/4.0/legalcode

using BoxSDK = Box.V2;

namespace ASC.Files.Thirdparty.Box;

[Transient]
internal class BoxStorage : IThirdPartyStorage<BoxFile, BoxFolder, BoxItem>
{
    private BoxClient _boxClient;

    private readonly List<string> _boxFields = new List<string> { "created_at", "modified_at", "name", "parent", "size" };

    public bool IsOpened { get; private set; }
    private readonly TempStream _tempStream;

    private readonly long _maxChunkedUploadFileSize = 250L * 1024L * 1024L;

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

    public async Task<BoxFolder> GetFolderAsync(string folderId)
    {
        try
        {
            return await _boxClient.FoldersManager.GetInformationAsync(folderId, _boxFields);
        }
        catch (Exception ex)
        {
            if (ex.InnerException is BoxSDK.Exceptions.BoxAPIException boxException && boxException.Error.Status == ((int)HttpStatusCode.NotFound).ToString())
            {
                return null;
            }

            throw;
        }
    }

    public async Task<bool> CheckAccessAsync()
    {
        try
        {
            var rootFolder = await GetFolderAsync("0");
            return !string.IsNullOrEmpty(rootFolder.Id);
        }
        catch (UnauthorizedAccessException)
        {
            return false;
        }
    }

    public Task<BoxFile> GetFileAsync(string fileId)
    {
        try
        {
            return _boxClient.FilesManager.GetInformationAsync(fileId, _boxFields);
        }
        catch (Exception ex)
        {
            if (ex.InnerException is BoxSDK.Exceptions.BoxAPIException boxException && boxException.Error.Status == ((int)HttpStatusCode.NotFound).ToString())
            {
                return Task.FromResult<BoxFile>(null);
            }
            throw;
        }
    }

    public async Task<List<BoxItem>> GetItemsAsync(string folderId)
    {
        var folderItems = await _boxClient.FoldersManager.GetFolderItemsAsync(folderId, 500, 0, _boxFields);

        return folderItems.Entries;
    }

    public async Task<Stream> DownloadStreamAsync(BoxFile file, int offset = 0)
    {
        ArgumentNullException.ThrowIfNull(file);

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

    public async Task<BoxFolder> CreateFolderAsync(string title, string parentId)
    {
        var boxFolderRequest = new BoxFolderRequest
        {
            Name = title,
            Parent = new BoxRequestEntity
            {
                Id = parentId
            }
        };

        return await _boxClient.FoldersManager.CreateAsync(boxFolderRequest, _boxFields);
    }

    public async Task<BoxFile> CreateFileAsync(Stream fileStream, string title, string parentId)
    {
        var boxFileRequest = new BoxFileRequest
        {
            Name = title,
            Parent = new BoxRequestEntity
            {
                Id = parentId
            }
        };

        return await _boxClient.FilesManager.UploadAsync(boxFileRequest, fileStream, _boxFields, setStreamPositionToZero: false);
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

    public async Task<BoxFolder> MoveFolderAsync(string boxFolderId, string newFolderName, string toFolderId)
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

        return await _boxClient.FoldersManager.UpdateInformationAsync(boxFolderRequest, _boxFields);
    }

    public async Task<BoxFile> MoveFileAsync(string boxFileId, string newFileName, string toFolderId)
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

        return await _boxClient.FilesManager.UpdateInformationAsync(boxFileRequest, null, _boxFields);
    }

    public async Task<BoxFolder> CopyFolderAsync(string boxFolderId, string newFolderName, string toFolderId)
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

        return await _boxClient.FoldersManager.CopyAsync(boxFolderRequest, _boxFields);
    }

    public async Task<BoxFile> CopyFileAsync(string boxFileId, string newFileName, string toFolderId)
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

        return await _boxClient.FilesManager.CopyAsync(boxFileRequest, _boxFields);
    }

    public async Task<BoxFolder> RenameFolderAsync(string boxFolderId, string newName)
    {
        var boxFolderRequest = new BoxFolderRequest { Id = boxFolderId, Name = newName };

        return await _boxClient.FoldersManager.UpdateInformationAsync(boxFolderRequest, _boxFields);
    }

    public async Task<BoxFile> RenameFileAsync(string boxFileId, string newName)
    {
        var boxFileRequest = new BoxFileRequest { Id = boxFileId, Name = newName };

        return await _boxClient.FilesManager.UpdateInformationAsync(boxFileRequest, null, _boxFields);
    }

    public async Task<BoxFile> SaveStreamAsync(string fileId, Stream fileStream)
    {
        return await _boxClient.FilesManager.UploadNewVersionAsync(null, fileId, fileStream, fields: _boxFields, setStreamPositionToZero: false);
    }

    public async Task<long> GetMaxUploadSizeAsync()
    {
        var boxUser = await _boxClient.UsersManager.GetCurrentUserInformationAsync(new List<string>() { "max_upload_size" });
        var max = boxUser.MaxUploadSize ?? _maxChunkedUploadFileSize;

        //todo: without chunked uploader:
        return Math.Min(max, _maxChunkedUploadFileSize);
    }

    public async Task<Stream> GetThumbnailAsync(string fileId, int width, int height)
    {
        var boxRepresentation = new BoxRepresentationRequest();
        boxRepresentation.FileId = fileId;
        boxRepresentation.XRepHints = $"[jpg?dimensions=320x320]";
        return await _boxClient.FilesManager.GetRepresentationContentAsync(boxRepresentation);
    }
}
