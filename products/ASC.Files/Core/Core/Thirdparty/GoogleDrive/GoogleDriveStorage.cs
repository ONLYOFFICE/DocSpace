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

using DriveFile = Google.Apis.Drive.v3.Data.File;
using MimeMapping = ASC.Common.Web.MimeMapping;

namespace ASC.Files.Thirdparty.GoogleDrive;

[Transient]
internal class GoogleDriveStorage : IThirdPartyStorage<DriveFile, DriveFile, DriveFile>, IGoogleDriveItemStorage<DriveFile>, IDisposable
{
    private OAuth20Token _token;

    private string AccessToken
    {
        get
        {
            if (_token == null)
            {
                throw new Exception("Cannot create GoogleDrive session with given token");
            }

            if (_token.IsExpired)
            {
                _token = _oAuth20TokenHelper.RefreshToken<GoogleLoginProvider>(_consumerFactory, _token);
            }

            return _token.AccessToken;
        }
    }

    private readonly ILogger _logger;
    public bool IsOpened { get; private set; }

    private DriveService _driveService;
    private readonly ConsumerFactory _consumerFactory;
    private readonly FileUtility _fileUtility;
    private readonly TempStream _tempStream;
    private readonly IHttpClientFactory _clientFactory;
    private readonly OAuth20TokenHelper _oAuth20TokenHelper;

    public const long MaxChunkedUploadFileSize = 2L * 1024L * 1024L * 1024L;

    public GoogleDriveStorage(
        ConsumerFactory consumerFactory,
        FileUtility fileUtility,
        ILoggerProvider monitor,
        TempStream tempStream,
        OAuth20TokenHelper oAuth20TokenHelper,
        IHttpClientFactory clientFactory)
    {
        _consumerFactory = consumerFactory;
        _fileUtility = fileUtility;
        _logger = monitor.CreateLogger("ASC.Files");
        _tempStream = tempStream;
        _clientFactory = clientFactory;
        _oAuth20TokenHelper = oAuth20TokenHelper;
    }

    public void Open(OAuth20Token token)
    {
        if (IsOpened)
        {
            return;
        }

        _token = token ?? throw new UnauthorizedAccessException("Cannot create GoogleDrive session with given token");

        var tokenResponse = new TokenResponse
        {
            AccessToken = _token.AccessToken,
            RefreshToken = _token.RefreshToken,
            IssuedUtc = _token.Timestamp,
            ExpiresInSeconds = _token.ExpiresIn,
            TokenType = "Bearer"
        };

        var apiCodeFlow = new GoogleAuthorizationCodeFlow(new GoogleAuthorizationCodeFlow.Initializer
        {
            ClientSecrets = new ClientSecrets
            {
                ClientId = _token.ClientID,
                ClientSecret = _token.ClientSecret
            },
            Scopes = new[] { DriveService.Scope.Drive }
        });

        _driveService = new DriveService(new BaseClientService.Initializer
        {
            HttpClientInitializer = new UserCredential(apiCodeFlow, string.Empty, tokenResponse)
        });

        IsOpened = true;
    }

    public void Close()
    {
        _driveService.Dispose();

        IsOpened = false;
    }

    public string GetRootFolderId()
    {
        var rootFolder = _driveService.Files.Get("root").Execute();

        return rootFolder.Id;
    }

    public async Task<DriveFile> GetItemAsync(string itemId)
    {
        try
        {
            var request = _driveService.Files.Get(itemId);
            request.Fields = GoogleLoginProvider.FilesFields;

            return await request.ExecuteAsync();
        }
        catch (GoogleApiException ex)
        {
            if (ex.HttpStatusCode == HttpStatusCode.NotFound)
            {
                return null;
            }
            throw;
        }
    }

    public async Task<Stream> GetThumbnailAsync(string fileId, int width, int height)
    {
        try
        {
            var url = $"https://lh3.google.com/u/0/d/{fileId}=w{width}-h{height}-p-k-nu-iv1";
            var httpClient = _driveService.HttpClient;
            var response = await httpClient.GetAsync(url);
            return await response.Content.ReadAsStreamAsync();
        }
        catch (Exception)
        {
            return null;
        }
    }

    public async Task<List<DriveFile>> GetItemsAsync(string folderId)
    {
        return await GetItemsInternalAsync(folderId);
    }

    public async Task<List<DriveFile>> GetItemsAsync(string folderId, bool? folders)
    {
        return await GetItemsInternalAsync(folderId, folders);
    }

    private async Task<List<DriveFile>> GetItemsInternalAsync(string folderId, bool? folders = null)
    {
        var request = _driveService.Files.List();

        var query = "'" + folderId + "' in parents and trashed=false";

        if (folders.HasValue)
        {
            query += " and mimeType " + (folders.Value ? "" : "!") + "= '" + GoogleLoginProvider.GoogleDriveMimeTypeFolder + "'";
        }

        request.Q = query;

        request.Fields = "nextPageToken, files(" + GoogleLoginProvider.FilesFields + ")";

        var files = new List<DriveFile>();
        do
        {
            try
            {
                var fileList = await request.ExecuteAsync();

                files.AddRange(fileList.Files);

                request.PageToken = fileList.NextPageToken;
            }
            catch (Exception)
            {
                request.PageToken = null;
            }
        } while (!string.IsNullOrEmpty(request.PageToken));

        return files;
    }

    public async Task<Stream> DownloadStreamAsync(DriveFile file, int offset = 0)
    {
        ArgumentNullException.ThrowIfNull(file);

        var downloadArg = $"{file.Id}?alt=media";

        var ext = MimeMapping.GetExtention(file.MimeType);
        if (GoogleLoginProvider.GoogleDriveExt.Contains(ext))
        {
            var internalExt = _fileUtility.GetGoogleDownloadableExtension(ext);
            var requiredMimeType = MimeMapping.GetMimeMapping(internalExt);

            downloadArg = $"{file.Id}/export?mimeType={HttpUtility.UrlEncode(requiredMimeType)}";
        }

        var request = new HttpRequestMessage
        {
            RequestUri = new Uri(GoogleLoginProvider.GoogleUrlFile + downloadArg),
            Method = HttpMethod.Get
        };
        request.Headers.Add("Authorization", "Bearer " + AccessToken);

        var httpClient = _clientFactory.CreateClient();
        var response = await httpClient.SendAsync(request);

        if (offset == 0 && file.Size.HasValue && file.Size > 0)
        {
            return new ResponseStream(await response.Content.ReadAsStreamAsync(), file.Size.Value);
        }

        var tempBuffer = _tempStream.Create();
        await using (var str = await response.Content.ReadAsStreamAsync())
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

    public async Task<DriveFile> InsertEntryAsync(Stream fileStream, string title, string parentId, bool folder = false)
    {
        var mimeType = folder ? GoogleLoginProvider.GoogleDriveMimeTypeFolder : MimeMapping.GetMimeMapping(title);

        var body = FileConstructor(title, mimeType, parentId);

        if (folder)
        {
            var requestFolder = _driveService.Files.Create(body);
            requestFolder.Fields = GoogleLoginProvider.FilesFields;

            return await requestFolder.ExecuteAsync();
        }

        var request = _driveService.Files.Create(body, fileStream, mimeType);
        request.Fields = GoogleLoginProvider.FilesFields;

        var result = await request.UploadAsync();
        if (result.Exception != null)
        {
            if (request.ResponseBody == null)
            {
                throw result.Exception;
            }

            _logger.ErrorWhileTryingToInsertEntity(result.Exception);
        }

        return request.ResponseBody;
    }
    public Task DeleteItemAsync(DriveFile entry)
    {
        return _driveService.Files.Delete(entry.Id).ExecuteAsync();
    }

    public async Task<DriveFile> CopyEntryAsync(string toFolderId, string originEntryId, string newTitle)
    {
        var body = FileConstructor(folderId: toFolderId, title: newTitle);
        try
        {
            var request = _driveService.Files.Copy(body, originEntryId);
            request.Fields = GoogleLoginProvider.FilesFields;

            return await request.ExecuteAsync();
        }
        catch (GoogleApiException ex)
        {
            if (ex.HttpStatusCode == HttpStatusCode.Forbidden)
            {
                throw new SecurityException(ex.Error.Message);
            }
            throw;
        }
    }

    public async Task<DriveFile> RenameEntryAsync(string fileId, string newTitle)
    {
        var request = _driveService.Files.Update(FileConstructor(newTitle), fileId);
        request.Fields = GoogleLoginProvider.FilesFields;

        return await request.ExecuteAsync();
    }

    public async Task<DriveFile> SaveStreamAsync(string fileId, Stream fileStream, string fileTitle)
    {
        var mimeType = MimeMapping.GetMimeMapping(fileTitle);
        var file = FileConstructor(fileTitle, mimeType);

        var request = _driveService.Files.Update(file, fileId, fileStream, mimeType);
        request.Fields = GoogleLoginProvider.FilesFields;
        var result = await request.UploadAsync();
        if (result.Exception != null)
        {
            if (request.ResponseBody == null)
            {
                throw result.Exception;
            }

            _logger.ErrorWhileTryingToInsertEntity(result.Exception);
        }

        return request.ResponseBody;
    }

    public DriveFile FileConstructor(string title = null, string mimeType = null, string folderId = null)
    {
        var file = new DriveFile();

        if (!string.IsNullOrEmpty(title))
        {
            file.Name = title;
        }

        if (!string.IsNullOrEmpty(mimeType))
        {
            file.MimeType = mimeType;
        }

        if (!string.IsNullOrEmpty(folderId))
        {
            file.Parents = new List<string> { folderId };
        }

        return file;
    }

    public async ValueTask<ResumableUploadSession> CreateResumableSessionAsync(DriveFile driveFile, long contentLength)
    {
        ArgumentNullException.ThrowIfNull(driveFile);

        var fileId = string.Empty;
        var method = "POST";
        var body = string.Empty;
        var folderId = driveFile.Parents.FirstOrDefault();

        if (driveFile.Id != null)
        {
            fileId = "/" + driveFile.Id;
            method = "PATCH";
        }
        else
        {
            var titleData = !string.IsNullOrEmpty(driveFile.Name) ? $"\"name\":\"{driveFile.Name}\"" : "";
            var parentData = !string.IsNullOrEmpty(folderId) ? $",\"parents\":[\"{folderId}\"]" : "";

            body = !string.IsNullOrEmpty(titleData + parentData) ? "{" + titleData + parentData + "}" : "";
        }

        var request = new HttpRequestMessage
        {
            RequestUri = new Uri(GoogleLoginProvider.GoogleUrlFileUpload + fileId + "?uploadType=resumable"),
            Method = new HttpMethod(method)
        };
        request.Headers.Add("X-Upload-Content-Type", MimeMapping.GetMimeMapping(driveFile.Name));
        request.Headers.Add("X-Upload-Content-Length", contentLength.ToString(CultureInfo.InvariantCulture));
        request.Headers.Add("Authorization", "Bearer " + AccessToken);
        request.Content = new StringContent(body, Encoding.UTF8, "application/json");

        var httpClient = _clientFactory.CreateClient();
        using var response = await httpClient.SendAsync(request);

        var uploadSession = new ResumableUploadSession(driveFile.Id, folderId, contentLength)
        {
            Location = response.Headers.Location.ToString(),
            Status = ResumableUploadSessionStatus.Started
        };

        return uploadSession;
    }

    public async ValueTask TransferAsync(ResumableUploadSession googleDriveSession, Stream stream, long chunkLength, bool lastChunk)
    {
        ArgumentNullException.ThrowIfNull(stream);

        if (googleDriveSession.Status != ResumableUploadSessionStatus.Started)
        {
            throw new InvalidOperationException("Can't upload chunk for given upload session.");
        }

        var request = new HttpRequestMessage
        {
            RequestUri = new Uri(googleDriveSession.Location),
            Method = HttpMethod.Put
        };
        request.Headers.Add("Authorization", "Bearer " + AccessToken);
        request.Content = new StreamContent(stream);
        if (googleDriveSession.BytesToTransfer > 0)
        {
            request.Content.Headers.ContentRange = new ContentRangeHeaderValue(
                                                       googleDriveSession.BytesTransfered,
                                                       googleDriveSession.BytesTransfered + chunkLength - 1,
                                                       googleDriveSession.BytesToTransfer);
        }
        else
        {
            var bytesToTransfer = lastChunk ? (googleDriveSession.BytesTransfered + chunkLength).ToString() : "*";

            request.Content.Headers.ContentRange = new ContentRangeHeaderValue(
                                           googleDriveSession.BytesTransfered,
                                           googleDriveSession.BytesTransfered + chunkLength - 1,
                                           Convert.ToInt64(bytesToTransfer));
        }
        var httpClient = _clientFactory.CreateClient();
        HttpResponseMessage response;

        try
        {
            response = await httpClient.SendAsync(request);
        }
        catch (Exception exception) // todo create catch
        {
            _logger.ErrorWithException(exception);
            throw;
        }

        if (response == null || response.StatusCode != HttpStatusCode.Created && response.StatusCode != HttpStatusCode.OK)
        {
            var uplSession = googleDriveSession;
            uplSession.BytesTransfered += chunkLength;

            if (response != null)
            {
                var locationHeader = response.Headers.Location;

                if (locationHeader != null)
                {
                    uplSession.Location = locationHeader.ToString();
                }
            }
        }
        else
        {
            googleDriveSession.Status = ResumableUploadSessionStatus.Completed;

            await using var responseStream = await response.Content.ReadAsStreamAsync();
            if (responseStream == null)
            {
                return;
            }

            string responseString;
            using (var readStream = new StreamReader(responseStream))
            {
                responseString = await readStream.ReadToEndAsync();
            }
            var responseJson = JObject.Parse(responseString);

            googleDriveSession.FileId = responseJson.Value<string>("id");
        }
    }

    public long GetMaxUploadSize()
    {
        var request = _driveService.About.Get();
        request.Fields = "maxUploadSize";
        var about = request.Execute();

        return about.MaxUploadSize ?? MaxChunkedUploadFileSize;
    }

    public async Task<long> GetMaxUploadSizeAsync()
    {
        var request = _driveService.About.Get();
        request.Fields = "maxUploadSize";
        var about = await request.ExecuteAsync();

        return about.MaxUploadSize ?? MaxChunkedUploadFileSize;
    }

    public void Dispose()
    {
        if (_driveService != null)
        {
            _driveService.Dispose();
        }
    }

    public Task<DriveFile> GetFolderAsync(string folderId)
    {
        return GetItemAsync(folderId);
    }

    public Task<DriveFile> GetFileAsync(string fileId)
    {
        return GetItemAsync(fileId);
    }

    public Task<DriveFile> CreateFolderAsync(string title, string parentId)
    {
        return InsertEntryAsync(null, title, parentId, true);
    }

    public Task<DriveFile> CreateFileAsync(Stream fileStream, string title, string parentId)
    {
        return InsertEntryAsync(fileStream, title, parentId);
    }

    public async Task<DriveFile> MoveFolderAsync(string folderId, string newFolderName, string toFolderId)
    {
        var folder = await GetFileAsync(folderId);
        var newFolder = await CopyEntryAsync(toFolderId, folderId, newFolderName);

        await DeleteItemAsync(folder);
        return newFolder;
    }

    public async Task<DriveFile> MoveFileAsync(string fileId, string newFileName, string toFolderId)
    {
        var file = await GetFileAsync(fileId);
        var newFile = await CopyEntryAsync(toFolderId, fileId, newFileName);

        await DeleteItemAsync(file);
        return newFile;
    }

    public Task<DriveFile> CopyFolderAsync(string folderId, string newFolderName, string toFolderId)
    {
        return CopyEntryAsync(toFolderId, folderId, newFolderName);
    }

    public Task<DriveFile> CopyFileAsync(string fileId, string newFileName, string toFolderId)
    {
        return CopyEntryAsync(toFolderId, fileId, newFileName);
    }

    public Task<DriveFile> RenameFolderAsync(string folderId, string newName)
    {
        return RenameEntryAsync(folderId, newName);
    }

    public Task<DriveFile> RenameFileAsync(string fileId, string newName)
    {
        return RenameEntryAsync(fileId, newName);
    }

    public async Task<DriveFile> SaveStreamAsync(string fileId, Stream fileStream)
    {
        var file = await GetFileAsync(fileId);
        return await SaveStreamAsync(fileId, fileStream, file.Name);
    }

    public async Task<bool> CheckAccessAsync()
    {
        try
        {
            var rootFolder = await GetFolderAsync("root");
            return !string.IsNullOrEmpty(rootFolder.Id);
        }
        catch (UnauthorizedAccessException)
        {
            return false;
        }
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
