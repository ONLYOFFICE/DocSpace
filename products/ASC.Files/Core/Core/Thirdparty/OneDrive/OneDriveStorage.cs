/*
 *
 * (c) Copyright Ascensio System Limited 2010-2018
 *
 * This program is freeware. You can redistribute it and/or modify it under the terms of the GNU 
 * General Public License (GPL) version 3 as published by the Free Software Foundation (https://www.gnu.org/copyleft/gpl.html). 
 * In accordance with Section 7(a) of the GNU GPL its Section 15 shall be amended to the effect that 
 * Ascensio System SIA expressly excludes the warranty of non-infringement of any third-party rights.
 *
 * THIS PROGRAM IS DISTRIBUTED WITHOUT ANY WARRANTY; WITHOUT EVEN THE IMPLIED WARRANTY OF MERCHANTABILITY OR
 * FITNESS FOR A PARTICULAR PURPOSE. For more details, see GNU GPL at https://www.gnu.org/copyleft/gpl.html
 *
 * You can contact Ascensio System SIA by email at sales@onlyoffice.com
 *
 * The interactive user interfaces in modified source and object code versions of ONLYOFFICE must display 
 * Appropriate Legal Notices, as required under Section 5 of the GNU GPL version 3.
 *
 * Pursuant to Section 7 § 3(b) of the GNU GPL you must retain the original ONLYOFFICE logo which contains 
 * relevant author attributions when distributing the software. If the display of the logo in its graphic 
 * form is not reasonably feasible for technical reasons, you must include the words "Powered by ONLYOFFICE" 
 * in every copy of the program you distribute. 
 * Pursuant to Section 7 § 3(e) we decline to grant you any rights under trademark law for use of our trademarks.
 *
*/


using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

using ASC.Common;
using ASC.Core.Common.Configuration;
using ASC.FederatedLogin;
using ASC.FederatedLogin.Helpers;
using ASC.FederatedLogin.LoginProviders;

using Microsoft.Graph;
using Microsoft.OneDrive.Sdk;

using Newtonsoft.Json.Linq;

namespace ASC.Files.Thirdparty.OneDrive
{
    [Scope]
    internal class OneDriveStorage
    {
        private OAuth20Token _token;

        private string AccessToken
        {
            get
            {
                if (_token == null) throw new Exception("Cannot create OneDrive session with given token");
                if (_token.IsExpired)
                {
                    _token = OAuth20TokenHelper.RefreshToken<OneDriveLoginProvider>(ConsumerFactory, _token);
                    _onedriveClientCache = null;
                }
                return _token.AccessToken;
            }
        }

        private OneDriveClient _onedriveClientCache;

        private OneDriveClient OnedriveClient
        {
            get { return _onedriveClientCache ??= new OneDriveClient(new OneDriveAuthProvider(AccessToken)); }
        }

        public bool IsOpened { get; private set; }
        private ConsumerFactory ConsumerFactory { get; }
        private IHttpClientFactory ClientFactory { get; }

        public long MaxChunkedUploadFileSize = 10L * 1024L * 1024L * 1024L;

        public OneDriveStorage(ConsumerFactory consumerFactory, IHttpClientFactory clientFactory)
        {
            ConsumerFactory = consumerFactory;
            ClientFactory = clientFactory;
        }

        public void Open(OAuth20Token token)
        {
            if (IsOpened)
                return;

            _token = token;

            IsOpened = true;
        }

        public void Close()
        {
            IsOpened = false;
        }

        public bool CheckAccess()
        {
            return CheckAccessAsync().Result;
        }

        public async Task<bool> CheckAccessAsync()
        {
            var request = await OnedriveClient
                       .Drive
                       .Request()
                       .GetAsync();

            return request != null;
        }


        public static readonly string RootPath = "/drive/root:";
        public static readonly string ApiVersion = "v1.0";

        public static string MakeOneDrivePath(string parentPath, string name)
        {
            return (parentPath ?? "") + "/" + (name ?? "");
        }

        public Task<Item> GetItemAsync(string itemId)
        {
            try
            {
                return GetItemRequest(itemId).Request().GetAsync();
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

        public async Task<List<Item>> GetItemsAsync(string folderId, int limit = 500)
        {
            return new List<Item>(await GetItemRequest(folderId).Children.Request().GetAsync());
        }

        public Task<Stream> DownloadStreamAsync(Item file, int offset = 0)
        {
            if (file == null || file.File == null) throw new ArgumentNullException(nameof(file));

            return InternalDownloadStreamAsync(file, offset);
        }

        private async Task<Stream> InternalDownloadStreamAsync(Item file, int offset = 0)
        {
            var fileStream = await OnedriveClient
                .Drive
                .Items[file.Id]
                .Content
                .Request()
                .GetAsync();

            if (fileStream != null && offset > 0)
                fileStream.Seek(offset, SeekOrigin.Begin);

            return fileStream;
        }

        public Task<Item> CreateFolderAsync(string title, string parentId)
        {
            var newFolderItem = new Item
            {
                Folder = new Folder(),
                Name = title
            };

            return GetItemRequest(parentId)
                .Children
                .Request()
                .AddAsync(newFolderItem);
        }


        public Task<Item> CreateFileAsync(Stream fileStream, string title, string parentPath)
        {
            return OnedriveClient
                .Drive
                .Root
                .ItemWithPath(MakeOneDrivePath(parentPath, title))
                .Content
                .Request()
                .PutAsync<Item>(fileStream);
        }

        public Task DeleteItemAsync(Item item)
        {
            return OnedriveClient
                .Drive
                .Items[item.Id]
                .Request()
                .DeleteAsync();
        }

        public Task<Item> MoveItemAsync(string itemId, string newItemName, string toFolderId)
        {
            var updateItem = new Item { ParentReference = new ItemReference { Id = toFolderId }, Name = newItemName };

            return OnedriveClient
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

        public Task<Item> RenameItemAsync(string itemId, string newName)
        {
            var updateItem = new Item { Name = newName };
            return OnedriveClient
                .Drive
                .Items[itemId]
                .Request()
                .UpdateAsync(updateItem);
        }

        public Task<Item> SaveStreamAsync(string fileId, Stream fileStream)
        {
            return OnedriveClient
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

        public Task<ResumableUploadSession> CreateResumableSessionAsync(Item onedriveFile, long contentLength)
        {
            if (onedriveFile == null) throw new ArgumentNullException(nameof(onedriveFile));

            return InternalCreateResumableSessionAsync(onedriveFile, contentLength);
        }

        private async Task<ResumableUploadSession> InternalCreateResumableSessionAsync(Item onedriveFile, long contentLength)
        {
            var folderId = onedriveFile.ParentReference.Id;
            var fileName = onedriveFile.Name;

            var uploadUriBuilder = new UriBuilder(OneDriveLoginProvider.OneDriveApiUrl)
            {
                Path = "/" + ApiVersion + "/drive/items/" + folderId + ":/" + fileName + ":/oneDrive.createUploadSession"
            };

            var request = new HttpRequestMessage();
            request.RequestUri = uploadUriBuilder.Uri;
            request.Method = HttpMethod.Post;
            request.Headers.Add("Authorization", "Bearer " + AccessToken);
            request.Content.Headers.ContentType = new MediaTypeHeaderValue("application/json")
            {
                CharSet = Encoding.UTF8.WebName
            };

            var uploadSession = new ResumableUploadSession(onedriveFile.Id, folderId, contentLength);

            var httpClient = ClientFactory.CreateClient();
            using (var response = await httpClient.SendAsync(request))
            using (var responseStream = await response.Content.ReadAsStreamAsync())
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

        public Task TransferAsync(ResumableUploadSession oneDriveSession, Stream stream, long chunkLength)
        {
            if (stream == null)
                throw new ArgumentNullException(nameof(stream));

            if (oneDriveSession.Status != ResumableUploadSessionStatus.Started)
                throw new InvalidOperationException("Can't upload chunk for given upload session.");

            return InternalTransferAsync(oneDriveSession, stream, chunkLength);
        }

        private async Task InternalTransferAsync(ResumableUploadSession oneDriveSession, Stream stream, long chunkLength)
        {
            var request = new HttpRequestMessage();
            request.RequestUri = new Uri(oneDriveSession.Location);
            request.Method = HttpMethod.Put;
            request.Headers.Add("Authorization", "Bearer " + AccessToken);
            request.Headers.Add("Content-Range", string.Format("bytes {0}-{1}/{2}",
                                                               oneDriveSession.BytesTransfered,
                                                               oneDriveSession.BytesTransfered + chunkLength - 1,
                                                               oneDriveSession.BytesToTransfer));
            request.Content = new StreamContent(stream);

            var httpClient = ClientFactory.CreateClient();
            using var response = await httpClient.SendAsync(request);

            if (response.StatusCode != HttpStatusCode.Created && response.StatusCode != HttpStatusCode.OK)
            {
                oneDriveSession.BytesTransfered += chunkLength;
            }
            else
            {
                oneDriveSession.Status = ResumableUploadSessionStatus.Completed;

                using var responseStream = await response.Content.ReadAsStreamAsync();
                if (responseStream == null) return;
                using var readStream = new StreamReader(responseStream);
                var responseString = await readStream.ReadToEndAsync();
                var responseJson = JObject.Parse(responseString);

                oneDriveSession.FileId = responseJson.Value<string>("id");
            }
        }

        public async Task CancelTransferAsync(ResumableUploadSession oneDriveSession)
        {
            var request = new HttpRequestMessage();
            request.RequestUri = new Uri(oneDriveSession.Location);
            request.Method = HttpMethod.Delete;

            var httpClient = ClientFactory.CreateClient();
            using var response = await httpClient.SendAsync(request);
        }
    }



    public class OneDriveAuthProvider : IAuthenticationProvider
    {
        private readonly string _accessToken;

        public OneDriveAuthProvider(string accessToken)
        {
            _accessToken = accessToken;
        }

        public Task AuthenticateRequestAsync(HttpRequestMessage request)
        {
            request.Headers.Authorization = new AuthenticationHeaderValue("bearer", _accessToken);
            return Task.WhenAll();
        }
    }

    public enum ResumableUploadSessionStatus
    {
        None,
        Started,
        Completed,
        Aborted
    }

    [Serializable]
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
}