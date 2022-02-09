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

namespace ASC.Files.Thirdparty.Dropbox
{
    internal class DropboxStorage : IDisposable
    {
        private OAuth20Token _token;

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

            _token = token;

            dropboxClient = new DropboxClient(_token.AccessToken);

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

        public long GetUsedSpace()
        {
            return (long)dropboxClient.Users.GetSpaceUsageAsync().Result.Used;
        }

        public FolderMetadata GetFolder(string folderPath)
        {
            if (string.IsNullOrEmpty(folderPath) || folderPath == "/")
            {
                return new FolderMetadata(string.Empty, "/");
            }
            try
            {
                return dropboxClient.Files.GetMetadataAsync(folderPath).Result.AsFolder;
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

        public FileMetadata GetFile(string filePath)
        {
            if (string.IsNullOrEmpty(filePath) || filePath == "/")
            {
                return null;
            }
            try
            {
                return dropboxClient.Files.GetMetadataAsync(filePath).Result.AsFile;
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

        public List<Metadata> GetItems(string folderPath)
        {
            return new List<Metadata>(dropboxClient.Files.ListFolderAsync(folderPath).Result.Entries);
        }

        public Stream DownloadStream(string filePath, int offset = 0)
        {
            if (string.IsNullOrEmpty(filePath)) throw new ArgumentNullException("file");

            using var response = dropboxClient.Files.DownloadAsync(filePath).Result;
            var tempBuffer = TempStream.Create();
            using (var str = response.GetContentAsStreamAsync().Result)
            {
                if (str != null)
                {
                    str.CopyTo(tempBuffer);
                    tempBuffer.Flush();
                    tempBuffer.Seek(offset, SeekOrigin.Begin);
                }
            }
            return tempBuffer;
        }

        public FolderMetadata CreateFolder(string title, string parentPath)
        {
            var path = MakeDropboxPath(parentPath, title);
            var result = dropboxClient.Files.CreateFolderV2Async(path, true).Result;
            return result.Metadata;
        }

        public FileMetadata CreateFile(Stream fileStream, string title, string parentPath)
        {
            var path = MakeDropboxPath(parentPath, title);
            return dropboxClient.Files.UploadAsync(path, WriteMode.Add.Instance, true, body: fileStream).Result;
        }

        public void DeleteItem(Metadata dropboxItem)
        {
            dropboxClient.Files.DeleteV2Async(dropboxItem.PathDisplay).Wait();
        }

        public FolderMetadata MoveFolder(string dropboxFolderPath, string dropboxFolderPathTo, string folderName)
        {
            var pathTo = MakeDropboxPath(dropboxFolderPathTo, folderName);
            var result = dropboxClient.Files.MoveV2Async(dropboxFolderPath, pathTo, autorename: true).Result;
            return (FolderMetadata)result.Metadata;
        }

        public FileMetadata MoveFile(string dropboxFilePath, string dropboxFolderPathTo, string fileName)
        {
            var pathTo = MakeDropboxPath(dropboxFolderPathTo, fileName);
            var result = dropboxClient.Files.MoveV2Async(dropboxFilePath, pathTo, autorename: true).Result;
            return (FileMetadata)result.Metadata;
        }

        public FolderMetadata CopyFolder(string dropboxFolderPath, string dropboxFolderPathTo, string folderName)
        {
            var pathTo = MakeDropboxPath(dropboxFolderPathTo, folderName);
            var result = dropboxClient.Files.CopyV2Async(dropboxFolderPath, pathTo, autorename: true).Result;
            return (FolderMetadata)result.Metadata;
        }

        public FileMetadata CopyFile(string dropboxFilePath, string dropboxFolderPathTo, string fileName)
        {
            var pathTo = MakeDropboxPath(dropboxFolderPathTo, fileName);
            var result = dropboxClient.Files.CopyV2Async(dropboxFilePath, pathTo, autorename: true).Result;
            return (FileMetadata)result.Metadata;
        }

        public FileMetadata SaveStream(string filePath, Stream fileStream)
        {
            return dropboxClient.Files.UploadAsync(filePath, WriteMode.Overwrite.Instance, body: fileStream).Result.AsFile;
        }

        public string CreateResumableSession()
        {
            return dropboxClient.Files.UploadSessionStartAsync(body: new MemoryStream()).Result.SessionId;
        }

        public void Transfer(string dropboxSession, long offset, Stream stream)
        {
            dropboxClient.Files.UploadSessionAppendV2Async(new UploadSessionCursor(dropboxSession, (ulong)offset), body: stream).Wait();
        }

        public Metadata FinishResumableSession(string dropboxSession, string dropboxFolderPath, string fileName, long offset)
        {
            var dropboxFilePath = MakeDropboxPath(dropboxFolderPath, fileName);
            return FinishResumableSession(dropboxSession, dropboxFilePath, offset);
        }

        public Metadata FinishResumableSession(string dropboxSession, string dropboxFilePath, long offset)
        {
            return dropboxClient.Files.UploadSessionFinishAsync(
                new UploadSessionCursor(dropboxSession, (ulong)offset),
                new CommitInfo(dropboxFilePath, WriteMode.Overwrite.Instance),
                new MemoryStream()).Result;
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