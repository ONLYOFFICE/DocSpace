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
using System.Threading.Tasks;

using ASC.Common;
using ASC.FederatedLogin;

using Dropbox.Api;
using Dropbox.Api.Files;

namespace ASC.Files.Thirdparty.Dropbox
{
    internal class DropboxStorage : IDisposable
    {
        private DropboxClient dropboxClient;

        public bool IsOpened { get; private set; }
        private TempStream TempStream { get; }

        public long MaxChunkedUploadFileSize = 20L * 1024L * 1024L * 1024L;

        public DropboxStorage(TempStream tempStream)
        {
            TempStream = tempStream;
        }

        public void Open(OAuth20Token token)
        {
            if (IsOpened)
                return;

            dropboxClient = new DropboxClient(token.AccessToken);

            IsOpened = true;
        }

        public void Close()
        {
            dropboxClient.Dispose();

            IsOpened = false;
        }



        public string MakeDropboxPath(string parentPath, string name)
        {
            return (parentPath ?? "") + "/" + (name ?? "");
        }

        public async Task<long> GetUsedSpaceAsync()
        {
            var spaceUsage = await dropboxClient.Users.GetSpaceUsageAsync();
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
                var metadata = await dropboxClient.Files.GetMetadataAsync(folderPath);
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
                var data = await dropboxClient.Files.GetMetadataAsync(filePath);
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
            var data = await dropboxClient.Files.ListFolderAsync(folderPath);
            return new List<Metadata>(data.Entries);
        }

        public Task<Stream> DownloadStreamAsync(string filePath, int offset = 0)
        {
            if (string.IsNullOrEmpty(filePath)) throw new ArgumentNullException("file");

            return InternalDownloadStreamAsync(filePath, offset);
        }

        public async Task<Stream> InternalDownloadStreamAsync(string filePath, int offset = 0)
        {
            using var response = await dropboxClient.Files.DownloadAsync(filePath);
            var tempBuffer = TempStream.Create();
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
            var result = await dropboxClient.Files.CreateFolderV2Async(path, true);
            return result.Metadata;
        }

        public Task<FileMetadata> CreateFileAsync(Stream fileStream, string title, string parentPath)
        {
            var path = MakeDropboxPath(parentPath, title);
            return dropboxClient.Files.UploadAsync(path, WriteMode.Add.Instance, true, body: fileStream);
        }

        public Task DeleteItemAsync(Metadata dropboxItem)
        {
            return dropboxClient.Files.DeleteV2Async(dropboxItem.PathDisplay);
        }

        public async Task<FolderMetadata> MoveFolderAsync(string dropboxFolderPath, string dropboxFolderPathTo, string folderName)
        {
            var pathTo = MakeDropboxPath(dropboxFolderPathTo, folderName);
            var result = await dropboxClient.Files.MoveV2Async(dropboxFolderPath, pathTo, autorename: true);
            return (FolderMetadata)result.Metadata;
        }

        public async Task<FileMetadata> MoveFileAsync(string dropboxFilePath, string dropboxFolderPathTo, string fileName)
        {
            var pathTo = MakeDropboxPath(dropboxFolderPathTo, fileName);
            var result = await dropboxClient.Files.MoveV2Async(dropboxFilePath, pathTo, autorename: true);
            return (FileMetadata)result.Metadata;
        }

        public async Task<FolderMetadata> CopyFolderAsync(string dropboxFolderPath, string dropboxFolderPathTo, string folderName)
        {
            var pathTo = MakeDropboxPath(dropboxFolderPathTo, folderName);
            var result = await dropboxClient.Files.CopyV2Async(dropboxFolderPath, pathTo, autorename: true);
            return (FolderMetadata)result.Metadata;
        }

        public async Task<FileMetadata> CopyFileAsync(string dropboxFilePath, string dropboxFolderPathTo, string fileName)
        {
            var pathTo = MakeDropboxPath(dropboxFolderPathTo, fileName);
            var result = await dropboxClient.Files.CopyV2Async(dropboxFilePath, pathTo, autorename: true);
            return (FileMetadata)result.Metadata;
        }

        public async Task<FileMetadata> SaveStreamAsync(string filePath, Stream fileStream)
        {
            var metadata = await dropboxClient.Files.UploadAsync(filePath, WriteMode.Overwrite.Instance, body: fileStream);
            return metadata.AsFile;
        }

        public async Task<string> CreateResumableSessionAsync()
        {
            var session = await dropboxClient.Files.UploadSessionStartAsync(body: new MemoryStream());
            return session.SessionId;
        }

        public Task TransferAsync(string dropboxSession, long offset, Stream stream)
        {
            return dropboxClient.Files.UploadSessionAppendV2Async(new UploadSessionCursor(dropboxSession, (ulong)offset), body: stream);
        }

        public Task<Metadata> FinishResumableSessionAsync(string dropboxSession, string dropboxFolderPath, string fileName, long offset)
        {
            var dropboxFilePath = MakeDropboxPath(dropboxFolderPath, fileName);
            return FinishResumableSessionAsync(dropboxSession, dropboxFilePath, offset);
        }

        public async Task<Metadata> FinishResumableSessionAsync(string dropboxSession, string dropboxFilePath, long offset)
        {
            return await dropboxClient.Files.UploadSessionFinishAsync(
                new UploadSessionCursor(dropboxSession, (ulong)offset),
                new CommitInfo(dropboxFilePath, WriteMode.Overwrite.Instance),
                new MemoryStream());
        }

        public void Dispose()
        {
            if (dropboxClient != null)
            {
                dropboxClient.Dispose();
            }
        }
    }
}