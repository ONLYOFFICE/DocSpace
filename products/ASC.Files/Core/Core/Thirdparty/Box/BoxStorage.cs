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
using System.Threading.Tasks;

using ASC.Common;
using ASC.FederatedLogin;

using Box.V2;
using Box.V2.Auth;
using Box.V2.Config;
using Box.V2.Models;

using BoxSDK = Box.V2;

namespace ASC.Files.Thirdparty.Box
{
    internal class BoxStorage
    {
        private BoxClient _boxClient;

        private readonly List<string> _boxFields = new List<string> { "created_at", "modified_at", "name", "parent", "size" };

        public bool IsOpened { get; private set; }
        private TempStream TempStream { get; }

        public long MaxChunkedUploadFileSize = 250L * 1024L * 1024L;

        public BoxStorage(TempStream tempStream)
        {
            TempStream = tempStream;
        }

        public void Open(OAuth20Token token)
        {
            if (IsOpened)
                return;

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
                return  new ValueTask<BoxFile>(_boxClient.FilesManager.GetInformationAsync(fileId, _boxFields));
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
            if (file == null) throw new ArgumentNullException(nameof(file));

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

            var tempBuffer = TempStream.Create();
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
}