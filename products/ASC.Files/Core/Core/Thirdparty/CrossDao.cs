using System;
using System.Linq;
using System.Threading;

using ASC.Common;
using ASC.Files.Core.Resources;
using ASC.Files.Core.Security;
using ASC.Files.Thirdparty.Box;
using ASC.Files.Thirdparty.Dropbox;
using ASC.Files.Thirdparty.GoogleDrive;
using ASC.Files.Thirdparty.OneDrive;
using ASC.Files.Thirdparty.SharePoint;
using ASC.Files.Thirdparty.Sharpbox;
using ASC.Web.Files.Utils;
using ASC.Web.Studio.Core;

using Microsoft.Extensions.DependencyInjection;

namespace ASC.Files.Core.Thirdparty
{
    [Scope(Additional = typeof(CrossDaoExtension))]
    internal class CrossDao //Additional SharpBox
    {
        private IServiceProvider ServiceProvider { get; }
        private SetupInfo SetupInfo { get; }
        private FileConverter FileConverter { get; }

        public CrossDao(
            IServiceProvider serviceProvider,
            SetupInfo setupInfo,
            FileConverter fileConverter)
        {
            ServiceProvider = serviceProvider;
            SetupInfo = setupInfo;
            FileConverter = fileConverter;
        }

        public File<TTo> PerformCrossDaoFileCopy<TFrom, TTo>(
            TFrom fromFileId, IFileDao<TFrom> fromFileDao, Func<TFrom, TFrom> fromConverter,
            TTo toFolderId, IFileDao<TTo> toFileDao, Func<TTo, TTo> toConverter,
            bool deleteSourceFile)
        {
            //Get File from first dao
            var fromFile = fromFileDao.GetFile(fromConverter(fromFileId));

            if (fromFile.ContentLength > SetupInfo.AvailableFileSize)
            {
                throw new Exception(string.Format(deleteSourceFile ? FilesCommonResource.ErrorMassage_FileSizeMove : FilesCommonResource.ErrorMassage_FileSizeCopy,
                                                  FileSizeComment.FilesSizeToString(SetupInfo.AvailableFileSize)));
            }

            var securityDao = ServiceProvider.GetService<ISecurityDao<TFrom>>();
            var tagDao = ServiceProvider.GetService<ITagDao<TFrom>>();

            var fromFileShareRecords = securityDao.GetPureShareRecords(fromFile).Where(x => x.EntryType == FileEntryType.File);
            var fromFileNewTags = tagDao.GetNewTags(Guid.Empty, fromFile).ToList();
            var fromFileLockTag = tagDao.GetTags(fromFile.ID, FileEntryType.File, TagType.Locked).FirstOrDefault();
            var fromFileFavoriteTag = tagDao.GetTags(fromFile.ID, FileEntryType.File, TagType.Favorite);
            var fromFileTemplateTag = tagDao.GetTags(fromFile.ID, FileEntryType.File, TagType.Template);

            var toFile = ServiceProvider.GetService<File<TTo>>();

            toFile.Title = fromFile.Title;
            toFile.Encrypted = fromFile.Encrypted;
            toFile.FolderID = toConverter(toFolderId);

            fromFile.ID = fromConverter(fromFile.ID);

            var mustConvert = !string.IsNullOrEmpty(fromFile.ConvertedType);
            using (var fromFileStream = mustConvert
                                            ? FileConverter.Exec(fromFile)
                                            : fromFileDao.GetFileStream(fromFile))
            {
                toFile.ContentLength = fromFileStream.CanSeek ? fromFileStream.Length : fromFile.ContentLength;
                toFile = toFileDao.SaveFile(toFile, fromFileStream);
            }

            if (fromFile.ThumbnailStatus == Thumbnail.Created)
            {
                using (var thumbnail = fromFileDao.GetThumbnail(fromFile))
                {
                    toFileDao.SaveThumbnail(toFile, thumbnail);
                }
                toFile.ThumbnailStatus = Thumbnail.Created;
            }

            if (deleteSourceFile)
            {
                if (fromFileShareRecords.Any())
                {
                    foreach (var record in fromFileShareRecords)
                    {
                        record.EntryId = toFile.ID;
                        securityDao.SetShare(record);
                    }
                }

                var fromFileTags = fromFileNewTags;
                if (fromFileLockTag != null) fromFileTags.Add(fromFileLockTag);
                if (fromFileFavoriteTag != null) fromFileTags.AddRange(fromFileFavoriteTag);
                if (fromFileTemplateTag != null) fromFileTags.AddRange(fromFileTemplateTag);

                if (fromFileTags.Count > 0)
                {
                    fromFileTags.ForEach(x => x.EntryId = toFile.ID);

                    tagDao.SaveTags(fromFileTags);
                }

                //Delete source file if needed
                fromFileDao.DeleteFile(fromConverter(fromFileId));
            }
            return toFile;
        }

        public Folder<TTo> PerformCrossDaoFolderCopy<TFrom, TTo>
            (TFrom fromFolderId, IFolderDao<TFrom> fromFolderDao, IFileDao<TFrom> fromFileDao, Func<TFrom, TFrom> fromConverter,
            TTo toRootFolderId, IFolderDao<TTo> toFolderDao, IFileDao<TTo> toFileDao, Func<TTo, TTo> toConverter,
            bool deleteSourceFolder, CancellationToken? cancellationToken)
        {
            var fromFolder = fromFolderDao.GetFolder(fromConverter(fromFolderId));

            var toFolder1 = ServiceProvider.GetService<Folder<TTo>>();
            toFolder1.Title = fromFolder.Title;
            toFolder1.FolderID = toConverter(toRootFolderId);

            var toFolder = toFolderDao.GetFolder(fromFolder.Title, toConverter(toRootFolderId));
            var toFolderId = toFolder != null
                                 ? toFolder.ID
                                 : toFolderDao.SaveFolder(toFolder1);

            var foldersToCopy = fromFolderDao.GetFolders(fromConverter(fromFolderId));
            var fileIdsToCopy = fromFileDao.GetFiles(fromConverter(fromFolderId));
            Exception copyException = null;
            //Copy files first
            foreach (var fileId in fileIdsToCopy)
            {
                if (cancellationToken.HasValue) cancellationToken.Value.ThrowIfCancellationRequested();
                try
                {
                    PerformCrossDaoFileCopy(fileId, fromFileDao, fromConverter,
                        toFolderId, toFileDao, toConverter,
                        deleteSourceFolder);
                }
                catch (Exception ex)
                {
                    copyException = ex;
                }
            }
            foreach (var folder in foldersToCopy)
            {
                if (cancellationToken.HasValue) cancellationToken.Value.ThrowIfCancellationRequested();
                try
                {
                    PerformCrossDaoFolderCopy(folder.ID, fromFolderDao, fromFileDao, fromConverter,
                        toFolderId, toFolderDao, toFileDao, toConverter,
                        deleteSourceFolder, cancellationToken);
                }
                catch (Exception ex)
                {
                    copyException = ex;
                }
            }

            if (deleteSourceFolder)
            {
                var securityDao = ServiceProvider.GetService<ISecurityDao<TFrom>>();
                var fromFileShareRecords = securityDao.GetPureShareRecords(fromFolder)
                    .Where(x => x.EntryType == FileEntryType.Folder);

                if (fromFileShareRecords.Any())
                {
                    foreach(var record in fromFileShareRecords){
                        record.EntryId = toFolderId;
                        securityDao.SetShare(record);
                    }
                }

                var tagDao = ServiceProvider.GetService<ITagDao<TFrom>>();
                var fromFileNewTags = tagDao.GetNewTags(Guid.Empty, fromFolder).ToList();

                if (fromFileNewTags.Count > 0)
                {
                    fromFileNewTags.ForEach(x => x.EntryId = toFolderId);

                    tagDao.SaveTags(fromFileNewTags);
                }

                if (copyException == null)
                    fromFolderDao.DeleteFolder(fromConverter(fromFolderId));
            }

            if (copyException != null) throw copyException;

            return toFolderDao.GetFolder(toConverter(toFolderId));
        }
    }

    public static class CrossDaoExtension
    {
        public static void Register(DIHelper services)
        {
            services.TryAdd<SharpBoxDaoSelector>();
            services.TryAdd<SharePointDaoSelector>();
            services.TryAdd<OneDriveDaoSelector>();
            services.TryAdd<GoogleDriveDaoSelector>();
            services.TryAdd<DropboxDaoSelector>();
            services.TryAdd<BoxDaoSelector>();
        }
    }
}
