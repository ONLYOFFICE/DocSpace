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

using Folder = Microsoft.OneDrive.Sdk.Folder;

namespace ASC.Files.Thirdparty.OneDrive;

[Transient]
internal class OneDriveStorage : IThirdPartyStorage<Item, Item, Item>
{
    private OAuth20Token _token;

    private string AccessToken
    {
        get
        {
            if (_token == null)
            {
                throw new Exception("Cannot create OneDrive session with given token");
            }

            if (_token.IsExpired)
            {
                _token = _oAuth20TokenHelper.RefreshToken<OneDriveLoginProvider>(_consumerFactory, _token);
                _onedriveClientCache = null;
            }

            return _token.AccessToken;
        }
    }

    private OneDriveClient _onedriveClientCache;

    private OneDriveClient OnedriveClient => _onedriveClientCache ??= new OneDriveClient(new OneDriveAuthProvider(AccessToken));

    public bool IsOpened { get; private set; }
    private readonly ConsumerFactory _consumerFactory;
    private readonly IHttpClientFactory _clientFactory;
    private readonly OAuth20TokenHelper _oAuth20TokenHelper;

    public long MaxChunkedUploadFileSize = 10L * 1024L * 1024L * 1024L;

    public OneDriveStorage(ConsumerFactory consumerFactory, IHttpClientFactory clientFactory, OAuth20TokenHelper oAuth20TokenHelper)
    {
        _consumerFactory = consumerFactory;
        _clientFactory = clientFactory;
        _oAuth20TokenHelper = oAuth20TokenHelper;
    }

    public void Open(OAuth20Token token)
    {
        if (IsOpened)
        {
            return;
        }

        _token = token;

        IsOpened = true;
    }

    public void Close()
    {
        IsOpened = false;
    }

    public async Task<bool> CheckAccessAsync()
    {
        try
        {
            var request = await OnedriveClient
                       .Drive
                       .Request()
                       .GetAsync();
            return request != null;
        }
        catch
        {
            return false;
        }
    }


    public static readonly string RootPath = "/drive/root:";
    public static readonly string ApiVersion = "v1.0";

    public static string MakeOneDrivePath(string parentPath, string name)
    {
        return (parentPath ?? "") + "/" + (name ?? "");
    }

    public async Task<Item> GetItemAsync(string itemId)
    {
        try
        {
            return await GetItemRequest(itemId).Request().GetAsync();
        }
        catch (Exception ex)
        {
            var serviceException = (ServiceException)ex.InnerException;
            if (serviceException != null && serviceException.StatusCode == HttpStatusCode.NotFound)
            {
                return null;
            }
            throw;
        }
    }

    public async Task<List<Item>> GetItemsAsync(string folderId)
    {
        return new List<Item>(await GetItemRequest(folderId).Children.Request().GetAsync());
    }

    public async Task<Stream> DownloadStreamAsync(Item file, int offset = 0)
    {
        ArgumentNullException.ThrowIfNull(file);
        ArgumentNullException.ThrowIfNull(file.File);

        var fileStream = await OnedriveClient
            .Drive
            .Items[file.Id]
            .Content
            .Request()
            .GetAsync();

        if (fileStream != null && offset > 0)
        {
            fileStream.Seek(offset, SeekOrigin.Begin);
        }

        return fileStream;
    }

    public async Task<Item> CreateFolderAsync(string title, string parentId)
    {
        var newFolderItem = new Item
        {
            Folder = new Folder(),
            Name = title
        };

        return await GetItemRequest(parentId)
            .Children
            .Request()
            .AddAsync(newFolderItem);
    }


    public async Task<Item> CreateFileAsync(Stream fileStream, string title, string parentPath)
    {
        return await OnedriveClient
            .Drive
            .Root
            .ItemWithPath(MakeOneDrivePath(parentPath, title))
            .Content
            .Request()
            .PutAsync<Item>(fileStream);
    }

    public async Task DeleteItemAsync(Item item)
    {
        await OnedriveClient
            .Drive
            .Items[item.Id]
            .Request()
            .DeleteAsync();
    }

    public async Task<Item> MoveItemAsync(string itemId, string newItemName, string toFolderId)
    {
        var updateItem = new Item { ParentReference = new ItemReference { Id = toFolderId }, Name = newItemName };

        return await OnedriveClient
            .Drive
            .Items[itemId]
            .Request()
            .UpdateAsync(updateItem);
    }

    public async Task<Item> CopyItemAsync(string itemId, string newItemName, string toFolderId)
    {
        var copyMonitor = await OnedriveClient
            .Drive
            .Items[itemId]
            .Copy(newItemName, new ItemReference { Id = toFolderId })
            .Request()
            .PostAsync();

        return await copyMonitor.PollForOperationCompletionAsync(null, CancellationToken.None);
    }

    public async Task<Item> RenameItemAsync(string itemId, string newName)
    {
        var updateItem = new Item { Name = newName };

        return await OnedriveClient
            .Drive
            .Items[itemId]
            .Request()
            .UpdateAsync(updateItem);
    }

    public async Task<Item> SaveStreamAsync(string fileId, Stream fileStream)
    {
        return await OnedriveClient
            .Drive
            .Items[fileId]
            .Content
            .Request()
            .PutAsync<Item>(fileStream);
    }

    private IItemRequestBuilder GetItemRequest(string itemId)
    {
        return string.IsNullOrEmpty(itemId)
                   ? OnedriveClient.Drive.Root
                   : OnedriveClient.Drive.Items[itemId];
    }

    public async Task<ResumableUploadSession> CreateResumableSessionAsync(Item onedriveFile, long contentLength)
    {
        ArgumentNullException.ThrowIfNull(onedriveFile);

        var folderId = onedriveFile.ParentReference.Id;
        var fileName = onedriveFile.Name;

        var uploadUriBuilder = new UriBuilder(OneDriveLoginProvider.OneDriveApiUrl)
        {
            Path = "/" + ApiVersion + "/drive/items/" + folderId + ":/" + fileName + ":/oneDrive.createUploadSession"
        };

        var request = new HttpRequestMessage
        {
            RequestUri = uploadUriBuilder.Uri,
            Method = HttpMethod.Post
        };
        request.Headers.Add("Authorization", "Bearer " + AccessToken);
        //request.Content.Headers.ContentType = new MediaTypeHeaderValue("application/json")
        //{
        //    CharSet = Encoding.UTF8.WebName
        //};

        var uploadSession = new ResumableUploadSession(onedriveFile.Id, folderId, contentLength);

        var httpClient = _clientFactory.CreateClient();
        using (var response = await httpClient.SendAsync(request))
        await using (var responseStream = await response.Content.ReadAsStreamAsync())
        {
            if (responseStream != null)
            {
                using var readStream = new StreamReader(responseStream);
                var responseString = await readStream.ReadToEndAsync();
                var responseJson = JObject.Parse(responseString);
                uploadSession.Location = responseJson.Value<string>("uploadUrl");
            }
        }

        uploadSession.Status = ResumableUploadSessionStatus.Started;

        return uploadSession;
    }

    public async ValueTask TransferAsync(ResumableUploadSession oneDriveSession, Stream stream, long chunkLength)
    {
        ArgumentNullException.ThrowIfNull(stream);

        if (oneDriveSession.Status != ResumableUploadSessionStatus.Started)
        {
            throw new InvalidOperationException("Can't upload chunk for given upload session.");
        }

        var request = new HttpRequestMessage
        {
            RequestUri = new Uri(oneDriveSession.Location),
            Method = HttpMethod.Put
        };
        request.Headers.Add("Authorization", "Bearer " + AccessToken);
        request.Content = new StreamContent(stream);

        request.Content.Headers.ContentRange = new ContentRangeHeaderValue(oneDriveSession.BytesTransfered,
                                                               oneDriveSession.BytesTransfered + chunkLength - 1,
                                                               oneDriveSession.BytesToTransfer);

        var httpClient = _clientFactory.CreateClient();
        using var response = await httpClient.SendAsync(request);

        if (response.StatusCode != HttpStatusCode.Created && response.StatusCode != HttpStatusCode.OK)
        {
            oneDriveSession.BytesTransfered += chunkLength;
        }
        else
        {
            oneDriveSession.Status = ResumableUploadSessionStatus.Completed;

            await using var responseStream = await response.Content.ReadAsStreamAsync();
            if (responseStream == null)
            {
                return;
            }

            using var readStream = new StreamReader(responseStream);
            var responseString = await readStream.ReadToEndAsync();
            var responseJson = JObject.Parse(responseString);

            oneDriveSession.FileId = responseJson.Value<string>("id");
        }
    }

    public async Task CancelTransferAsync(ResumableUploadSession oneDriveSession)
    {
        var request = new HttpRequestMessage
        {
            RequestUri = new Uri(oneDriveSession.Location),
            Method = HttpMethod.Delete
        };

        var httpClient = _clientFactory.CreateClient();
        using var response = await httpClient.SendAsync(request);
    }

    public async Task<Stream> GetThumbnailAsync(string fileId, int width, int height)
    {
        var thumbnails = await OnedriveClient.Drive.Items[fileId].Thumbnails.Request().GetAsync();
        if (thumbnails.Count > 0)
        {
            var url = thumbnails[0].Medium.Url;
            url = url.Substring(0, url.IndexOf("?width"));
            url = url + $"?width={width}&height={height}&cropmode=none";
            var request = new HttpRequestMessage
            {
                RequestUri = new Uri(url),
                Method = HttpMethod.Get
            };
            var httpClient = _clientFactory.CreateClient();
            using var response = await httpClient.SendAsync(request);
            var bytes = await response.Content.ReadAsByteArrayAsync();
            return new MemoryStream(bytes);
        }
        else
        {
            return null;
        }
    }

    public Task<Item> GetFolderAsync(string folderId)
    {
        return GetItemAsync(folderId);
    }

    public Task<Item> GetFileAsync(string fileId)
    {
        return GetItemAsync(fileId);
    }

    public Task<Item> MoveFolderAsync(string folderId, string newFolderName, string toFolderId)
    {
        return MoveItemAsync(folderId, newFolderName, toFolderId);
    }

    public Task<Item> MoveFileAsync(string fileId, string newFileName, string toFolderId)
    {
        return MoveItemAsync(fileId, newFileName, toFolderId);
    }

    public Task<Item> CopyFolderAsync(string folderId, string newFolderName, string toFolderId)
    {
        return CopyItemAsync(folderId, newFolderName, toFolderId);
    }

    public Task<Item> CopyFileAsync(string fileId, string newFileName, string toFolderId)
    {
        return CopyItemAsync(fileId, newFileName, toFolderId);
    }

    public Task<Item> RenameFolderAsync(string folderId, string newName)
    {
        return RenameItemAsync(folderId, newName);
    }

    public Task<Item> RenameFileAsync(string fileId, string newName)
    {
        return RenameItemAsync(fileId, newName);
    }

    public Task<long> GetMaxUploadSizeAsync()
    {
        return Task.FromResult(MaxChunkedUploadFileSize);
    }
}



public class OneDriveAuthProvider : IAuthenticationProvider
{
    private readonly string _accessToken;

    public OneDriveAuthProvider(string accessToken)
    {
        _accessToken = accessToken;
    }

    public async Task AuthenticateRequestAsync(HttpRequestMessage request)
    {
        request.Headers.Authorization = new AuthenticationHeaderValue("bearer", _accessToken);

        await Task.WhenAll();
    }
}

public enum ResumableUploadSessionStatus
{
    None,
    Started,
    Completed,
    Aborted
}

internal class ResumableUploadSession
{
    public long BytesToTransfer { get; set; }
    public long BytesTransfered { get; set; }
    public string FileId { get; set; }
    public string FolderId { get; set; }
    public ResumableUploadSessionStatus Status { get; set; }
    public string Location { get; set; }

    public ResumableUploadSession(string fileId, string folderId, long bytesToTransfer)
    {
        FileId = fileId;
        FolderId = folderId;
        BytesToTransfer = bytesToTransfer;
        Status = ResumableUploadSessionStatus.None;
    }
}
