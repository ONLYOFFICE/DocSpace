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
using System.Linq;
using System.Security;
using System.Threading;
using System.Threading.Tasks;

using ASC.Api.Core;
using ASC.Common;
using ASC.Core;
using ASC.Core.Users;
using ASC.Files.Core;
using ASC.Files.Core.Resources;
using ASC.Files.Core.Security;
using ASC.MessagingSystem;
using ASC.Web.Core.Files;
using ASC.Web.Files.Classes;
using ASC.Web.Files.Helpers;
using ASC.Web.Studio.Core;
using ASC.Web.Studio.UserControls.Statistics;
using ASC.Web.Studio.Utility;

using Microsoft.Extensions.DependencyInjection;

namespace ASC.Web.Files.Utils
{
    [Scope]
    public class FileUploader
    {
        private FilesSettingsHelper FilesSettingsHelper { get; }
        private FileUtility FileUtility { get; }
        private UserManager UserManager { get; }
        private TenantManager TenantManager { get; }
        private AuthContext AuthContext { get; }
        private SetupInfo SetupInfo { get; }
        private TenantExtra TenantExtra { get; }
        private TenantStatisticsProvider TenantStatisticsProvider { get; }
        private FileMarker FileMarker { get; }
        private FileConverter FileConverter { get; }
        private IDaoFactory DaoFactory { get; }
        private Global Global { get; }
        private FilesLinkUtility FilesLinkUtility { get; }
        private FilesMessageService FilesMessageService { get; }
        private FileSecurity FileSecurity { get; }
        private EntryManager EntryManager { get; }
        private IServiceProvider ServiceProvider { get; }
        private ChunkedUploadSessionHolder ChunkedUploadSessionHolder { get; }
        private FileTrackerHelper FileTracker { get; }

        public FileUploader(
            FilesSettingsHelper filesSettingsHelper,
            FileUtility fileUtility,
            UserManager userManager,
            TenantManager tenantManager,
            AuthContext authContext,
            SetupInfo setupInfo,
            TenantExtra tenantExtra,
            TenantStatisticsProvider tenantStatisticsProvider,
            FileMarker fileMarker,
            FileConverter fileConverter,
            IDaoFactory daoFactory,
            Global global,
            FilesLinkUtility filesLinkUtility,
            FilesMessageService filesMessageService,
            FileSecurity fileSecurity,
            EntryManager entryManager,
            IServiceProvider serviceProvider,
            ChunkedUploadSessionHolder chunkedUploadSessionHolder,
            FileTrackerHelper fileTracker)
        {
            FilesSettingsHelper = filesSettingsHelper;
            FileUtility = fileUtility;
            UserManager = userManager;
            TenantManager = tenantManager;
            AuthContext = authContext;
            SetupInfo = setupInfo;
            TenantExtra = tenantExtra;
            TenantStatisticsProvider = tenantStatisticsProvider;
            FileMarker = fileMarker;
            FileConverter = fileConverter;
            DaoFactory = daoFactory;
            Global = global;
            FilesLinkUtility = filesLinkUtility;
            FilesMessageService = filesMessageService;
            FileSecurity = fileSecurity;
            EntryManager = entryManager;
            ServiceProvider = serviceProvider;
            ChunkedUploadSessionHolder = chunkedUploadSessionHolder;
            FileTracker = fileTracker;
        }

        public Task<File<T>> ExecAsync<T>(T folderId, string title, long contentLength, Stream data)
        {
            return ExecAsync(folderId, title, contentLength, data, !FilesSettingsHelper.UpdateIfExist);
        }

        public async Task<File<T>> ExecAsync<T>(T folderId, string title, long contentLength, Stream data, bool createNewIfExist, bool deleteConvertStatus = true)
        {
            if (contentLength <= 0)
                throw new Exception(FilesCommonResource.ErrorMassage_EmptyFile);

            var file = await VerifyFileUploadAsync(folderId, title, contentLength, !createNewIfExist);

            var dao = DaoFactory.GetFileDao<T>();
            file = await dao.SaveFileAsync(file, data);

            var linkDao = DaoFactory.GetLinkDao();
            await linkDao.DeleteAllLinkAsync(file.ID.ToString());

            await FileMarker.MarkAsNewAsync(file);

            if (FileConverter.EnableAsUploaded && FileConverter.MustConvert(file))
                await FileConverter.ExecAsynchronouslyAsync(file, deleteConvertStatus);

            return file;
        }

        public async Task<File<T>> VerifyFileUploadAsync<T>(T folderId, string fileName, bool updateIfExists, string relativePath = null)
        {
            fileName = Global.ReplaceInvalidCharsAndTruncate(fileName);

            if (Global.EnableUploadFilter && !FileUtility.ExtsUploadable.Contains(FileUtility.GetFileExtension(fileName)))
                throw new NotSupportedException(FilesCommonResource.ErrorMassage_NotSupportedFormat);

            folderId = await GetFolderIdAsync(folderId, string.IsNullOrEmpty(relativePath) ? null : relativePath.Split('/').ToList());

            var fileDao = DaoFactory.GetFileDao<T>();
            var file = await fileDao.GetFileAsync(folderId, fileName);

            if (updateIfExists && await CanEditAsync(file))
            {
                file.Title = fileName;
                file.ConvertedType = null;
                file.Comment = FilesCommonResource.CommentUpload;
                file.Version++;
                file.VersionGroup++;
                file.Encrypted = false;
                file.ThumbnailStatus = Thumbnail.Waiting;

                return file;
            }

            var newFile = ServiceProvider.GetService<File<T>>();
            newFile.FolderID = folderId;
            newFile.Title = fileName;
            return newFile;
        }

        public async Task<File<T>> VerifyFileUploadAsync<T>(T folderId, string fileName, long fileSize, bool updateIfExists)
        {
            if (fileSize <= 0)
                throw new Exception(FilesCommonResource.ErrorMassage_EmptyFile);

            var maxUploadSize = await GetMaxFileSizeAsync(folderId);

            if (fileSize > maxUploadSize)
                throw FileSizeComment.GetFileSizeException(maxUploadSize);

            var file = await VerifyFileUploadAsync(folderId, fileName, updateIfExists);
            file.ContentLength = fileSize;
            return file;
        }

        private async Task<bool> CanEditAsync<T>(File<T> file)
        {
            return file != null
                   && await FileSecurity.CanEditAsync(file)
                   && !UserManager.GetUsers(AuthContext.CurrentAccount.ID).IsVisitor(UserManager)
                   && !await EntryManager.FileLockedForMeAsync(file.ID)
                   && !FileTracker.IsEditing(file.ID)
                   && file.RootFolderType != FolderType.TRASH
                   && !file.Encrypted;
        }      

        private async Task<T> GetFolderIdAsync<T>(T folderId, IList<string> relativePath)
        {
            var folderDao = DaoFactory.GetFolderDao<T>();
            var folder = await folderDao.GetFolderAsync(folderId);

            if (folder == null)
                throw new DirectoryNotFoundException(FilesCommonResource.ErrorMassage_FolderNotFound);

            if (!await FileSecurity.CanCreateAsync(folder))
                throw new SecurityException(FilesCommonResource.ErrorMassage_SecurityException_Create);

            if (relativePath != null && relativePath.Count > 0)
            {
                var subFolderTitle = Global.ReplaceInvalidCharsAndTruncate(relativePath.FirstOrDefault());

                if (!string.IsNullOrEmpty(subFolderTitle))
                {
                    folder = await folderDao.GetFolderAsync(subFolderTitle, folder.ID);

                    if (folder == null)
                    {
                        var newFolder = ServiceProvider.GetService<Folder<T>>();
                        newFolder.Title = subFolderTitle;
                        newFolder.FolderID = folderId;

                        folderId = await folderDao.SaveFolderAsync(newFolder);

                        folder = await folderDao .GetFolderAsync(folderId);
                        FilesMessageService.Send(folder, MessageAction.FolderCreated, folder.Title);
                    }

                    folderId = folder.ID;

                    relativePath.RemoveAt(0);
                    folderId = await GetFolderIdAsync(folderId, relativePath);
                }
            }

            return folderId;
        }

        #region chunked upload

        public async Task<File<T>> VerifyChunkedUploadAsync<T>(T folderId, string fileName, long fileSize, bool updateIfExists, ApiDateTime lastModified, string relativePath = null)
        {
            var maxUploadSize = await GetMaxFileSizeAsync(folderId, true);

            if (fileSize > maxUploadSize)
                throw FileSizeComment.GetFileSizeException(maxUploadSize);

            var file = await VerifyFileUploadAsync(folderId, fileName, updateIfExists, relativePath);
            file.ContentLength = fileSize;

            if(lastModified != null)
            {
                file.ModifiedOn = lastModified;
            }

            return file;
        }

        public async Task<ChunkedUploadSession<T>> InitiateUploadAsync<T>(T folderId, T fileId, string fileName, long contentLength, bool encrypted)
        {
            var file = ServiceProvider.GetService<File<T>>();
            file.ID = fileId;
            file.FolderID = folderId;
            file.Title = fileName;
            file.ContentLength = contentLength;

            var dao = DaoFactory.GetFileDao<T>();
            var uploadSession = await dao.CreateUploadSessionAsync(file, contentLength);

            uploadSession.Expired = uploadSession.Created + ChunkedUploadSessionHolder.SlidingExpiration;
            uploadSession.Location = FilesLinkUtility.GetUploadChunkLocationUrl(uploadSession.Id);
            uploadSession.TenantId = TenantManager.GetCurrentTenant().TenantId;
            uploadSession.UserId = AuthContext.CurrentAccount.ID;
            uploadSession.FolderId = folderId;
            uploadSession.CultureName = Thread.CurrentThread.CurrentUICulture.Name;
            uploadSession.Encrypted = encrypted;

            await ChunkedUploadSessionHolder.StoreSessionAsync(uploadSession);

            return uploadSession;
        }

        public async Task<ChunkedUploadSession<T>> UploadChunkAsync<T>(string uploadId, Stream stream, long chunkLength)
        {
            var uploadSession = await ChunkedUploadSessionHolder.GetSessionAsync<T>(uploadId);
            uploadSession.Expired = DateTime.UtcNow + ChunkedUploadSessionHolder.SlidingExpiration;

            if (chunkLength <= 0)
            {
                throw new Exception(FilesCommonResource.ErrorMassage_EmptyFile);
            }

            if (chunkLength > SetupInfo.ChunkUploadSize)
            {
                throw FileSizeComment.GetFileSizeException(SetupInfo.MaxUploadSize(TenantExtra, TenantStatisticsProvider));
            }

            var maxUploadSize = await GetMaxFileSizeAsync(uploadSession.FolderId, uploadSession.BytesTotal > 0);

            if (uploadSession.BytesUploaded + chunkLength > maxUploadSize)
            {
                await AbortUploadAsync(uploadSession);
                throw FileSizeComment.GetFileSizeException(maxUploadSize);
            }

            var dao = DaoFactory.GetFileDao<T>();
            await dao.UploadChunkAsync(uploadSession, stream, chunkLength);

            if (uploadSession.BytesUploaded == uploadSession.BytesTotal)
            {
                var linkDao = DaoFactory.GetLinkDao();
                await linkDao.DeleteAllLinkAsync(uploadSession.File.ID.ToString());

                await FileMarker.MarkAsNewAsync(uploadSession.File);
                await ChunkedUploadSessionHolder.RemoveSessionAsync(uploadSession);
            }
            else
            {
                await ChunkedUploadSessionHolder.StoreSessionAsync(uploadSession);
            }

            return uploadSession;
        }

        public async Task AbortUploadAsync<T>(string uploadId)
        {
            await AbortUploadAsync(await ChunkedUploadSessionHolder.GetSessionAsync<T>(uploadId));
        }

        private async Task  AbortUploadAsync<T>(ChunkedUploadSession<T> uploadSession)
        {
            await DaoFactory.GetFileDao<T>().AbortUploadSessionAsync(uploadSession);

            await ChunkedUploadSessionHolder.RemoveSessionAsync(uploadSession);
        }

        private Task<long> GetMaxFileSizeAsync<T>(T folderId, bool chunkedUpload = false)
        {
            var folderDao = DaoFactory.GetFolderDao<T>();
            return folderDao.GetMaxUploadSizeAsync(folderId, chunkedUpload);
        }

        #endregion
    }
}