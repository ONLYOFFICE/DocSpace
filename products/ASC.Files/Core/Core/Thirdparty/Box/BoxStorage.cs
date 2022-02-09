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


using BoxSDK = Box.V2;

namespace ASC.Files.Thirdparty.Box
{
    internal class BoxStorage
    {
        private OAuth20Token _token;

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

            _token = token;

            var config = new BoxConfig(_token.ClientID, _token.ClientSecret, new Uri(_token.RedirectUri));
            var session = new OAuthSession(_token.AccessToken, _token.RefreshToken, (int)_token.ExpiresIn, "bearer");
            _boxClient = new BoxClient(config, session);

            IsOpened = true;
        }

        public void Close()
        {
            IsOpened = false;
        }


        public string GetRootFolderId()
        {
            var root = GetFolder("0");

            return root.Id;
        }

        public BoxFolder GetFolder(string folderId)
        {
            try
            {
                return _boxClient.FoldersManager.GetInformationAsync(folderId, _boxFields).Result;
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

        public BoxFile GetFile(string fileId)
        {
            try
            {
                return _boxClient.FilesManager.GetInformationAsync(fileId, _boxFields).Result;
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

        public List<BoxItem> GetItems(string folderId, int limit = 500)
        {
            return _boxClient.FoldersManager.GetFolderItemsAsync(folderId, limit, 0, _boxFields).Result.Entries;
        }

        public Stream DownloadStream(BoxFile file, int offset = 0)
        {
            if (file == null) throw new ArgumentNullException("file");

            if (offset > 0 && file.Size.HasValue)
            {
                return _boxClient.FilesManager.DownloadAsync(file.Id, startOffsetInBytes: offset, endOffsetInBytes: (int)file.Size - 1).Result;
            }

            var str = _boxClient.FilesManager.DownloadAsync(file.Id).Result;
            if (offset == 0)
            {
                return str;
            }

            var tempBuffer = TempStream.Create();
            if (str != null)
            {
                str.CopyTo(tempBuffer);
                tempBuffer.Flush();
                tempBuffer.Seek(offset, SeekOrigin.Begin);

                str.Dispose();
            }

            return tempBuffer;
        }

        public BoxFolder CreateFolder(string title, string parentId)
        {
            var boxFolderRequest = new BoxFolderRequest
            {
                Name = title,
                Parent = new BoxRequestEntity
                {
                    Id = parentId
                }
            };
            return _boxClient.FoldersManager.CreateAsync(boxFolderRequest, _boxFields).Result;
        }

        public BoxFile CreateFile(Stream fileStream, string title, string parentId)
        {
            var boxFileRequest = new BoxFileRequest
            {
                Name = title,
                Parent = new BoxRequestEntity
                {
                    Id = parentId
                }
            };
            return _boxClient.FilesManager.UploadAsync(boxFileRequest, fileStream, _boxFields, setStreamPositionToZero: false).Result;
        }

        public void DeleteItem(BoxItem boxItem)
        {
            if (boxItem is BoxFolder)
            {
                _boxClient.FoldersManager.DeleteAsync(boxItem.Id, true).Wait();
            }
            else
            {
                _boxClient.FilesManager.DeleteAsync(boxItem.Id).Wait();
            }
        }

        public BoxFolder MoveFolder(string boxFolderId, string newFolderName, string toFolderId)
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
            return _boxClient.FoldersManager.UpdateInformationAsync(boxFolderRequest, _boxFields).Result;
        }

        public BoxFile MoveFile(string boxFileId, string newFileName, string toFolderId)
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
            return _boxClient.FilesManager.UpdateInformationAsync(boxFileRequest, null, _boxFields).Result;
        }

        public BoxFolder CopyFolder(string boxFolderId, string newFolderName, string toFolderId)
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
            return _boxClient.FoldersManager.CopyAsync(boxFolderRequest, _boxFields).Result;
        }

        public BoxFile CopyFile(string boxFileId, string newFileName, string toFolderId)
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
            return _boxClient.FilesManager.CopyAsync(boxFileRequest, _boxFields).Result;
        }

        public BoxFolder RenameFolder(string boxFolderId, string newName)
        {
            var boxFolderRequest = new BoxFolderRequest { Id = boxFolderId, Name = newName };
            return _boxClient.FoldersManager.UpdateInformationAsync(boxFolderRequest, _boxFields).Result;
        }

        public BoxFile RenameFile(string boxFileId, string newName)
        {
            var boxFileRequest = new BoxFileRequest { Id = boxFileId, Name = newName };
            return _boxClient.FilesManager.UpdateInformationAsync(boxFileRequest, null, _boxFields).Result;
        }

        public BoxFile SaveStream(string fileId, Stream fileStream)
        {
            return _boxClient.FilesManager.UploadNewVersionAsync(null, fileId, fileStream, fields: _boxFields, setStreamPositionToZero: false).Result;
        }

        public long GetMaxUploadSize()
        {
            var boxUser = _boxClient.UsersManager.GetCurrentUserInformationAsync(new List<string>() { "max_upload_size" }).Result;
            var max = boxUser.MaxUploadSize ?? MaxChunkedUploadFileSize;

            //todo: without chunked uploader:
            return Math.Min(max, MaxChunkedUploadFileSize);
        }
    }
}