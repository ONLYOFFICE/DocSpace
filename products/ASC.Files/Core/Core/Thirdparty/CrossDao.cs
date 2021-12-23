using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

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
            var fromFile = fromFileDao.GetFileAsync(fromConverter(fromFileId)).Result;

            if (fromFile.ContentLength > SetupInfo.AvailableFileSize)
            {
                throw new Exception(string.Format(deleteSourceFile ? FilesCommonResource.ErrorMassage_FileSizeMove : FilesCommonResource.ErrorMassage_FileSizeCopy,
                                                  FileSizeComment.FilesSizeToString(SetupInfo.AvailableFileSize)));
            }

            var securityDao = ServiceProvider.GetService<ISecurityDao<TFrom>>();
            var tagDao = ServiceProvider.GetService<ITagDao<TFrom>>();

            var fromFileShareRecords = securityDao.GetPureShareRecords(fromFile).Where(x => x.EntryType == FileEntryType.File);
            var fromFileNewTags = tagDao.GetNewTagsAsync(Guid.Empty, fromFile).ToListAsync().Result;
            var fromFileLockTag = tagDao.GetTagsAsync(fromFile.ID, FileEntryType.File, TagType.Locked).ToListAsync().Result.FirstOrDefault();
            var fromFileFavoriteTag = tagDao.GetTagsAsync(fromFile.ID, FileEntryType.File, TagType.Favorite).ToListAsync().Result;
            var fromFileTemplateTag = tagDao.GetTagsAsync(fromFile.ID, FileEntryType.File, TagType.Template).ToListAsync().Result;

            var toFile = ServiceProvider.GetService<File<TTo>>();

            toFile.Title = fromFile.Title;
            toFile.Encrypted = fromFile.Encrypted;
            toFile.FolderID = toConverter(toFolderId);

            fromFile.ID = fromConverter(fromFile.ID);

            var mustConvert = !string.IsNullOrEmpty(fromFile.ConvertedType);
            using (var fromFileStream = mustConvert
                                            ? FileConverter.Exec(fromFile)
                                            : fromFileDao.GetFileStreamAsync(fromFile).Result)
            {
                toFile.ContentLength = fromFileStream.CanSeek ? fromFileStream.Length : fromFile.ContentLength;
                toFile = toFileDao.SaveFileAsync(toFile, fromFileStream).Result;
            }

            if (fromFile.ThumbnailStatus == Thumbnail.Created)
            {
                using (var thumbnail = fromFileDao.GetThumbnailAsync(fromFile).Result)
                {
                    toFileDao.SaveThumbnailAsync(toFile, thumbnail).Wait();
                }
                toFile.ThumbnailStatus = Thumbnail.Created;
            }

            if (deleteSourceFile)
            {
                if (fromFileShareRecords.Any())
                    fromFileShareRecords.ToList().ForEach(x =>
                    {
                        x.EntryId = toFile.ID;
                        securityDao.SetShare(x);
                    });

                var fromFileTags = fromFileNewTags;
                if (fromFileLockTag != null) fromFileTags.Add(fromFileLockTag);
                if (fromFileFavoriteTag != null) fromFileTags.AddRange(fromFileFavoriteTag);
                if (fromFileTemplateTag != null) fromFileTags.AddRange(fromFileTemplateTag);

                if (fromFileTags.Any())
                {
                    fromFileTags.ForEach(x => x.EntryId = toFile.ID);

                    tagDao.SaveTags(fromFileTags);
                }

                //Delete source file if needed
                fromFileDao.DeleteFileAsync(fromConverter(fromFileId)).Wait();
            }
            return toFile;
        }

        public async Task<File<TTo>> PerformCrossDaoFileCopyAsync<TFrom, TTo>(
           TFrom fromFileId, IFileDao<TFrom> fromFileDao, Func<TFrom, TFrom> fromConverter,
           TTo toFolderId, IFileDao<TTo> toFileDao, Func<TTo, TTo> toConverter,
           bool deleteSourceFile)
        {
            //Get File from first dao
            var fromFile = await fromFileDao.GetFileAsync(fromConverter(fromFileId));

            if (fromFile.ContentLength > SetupInfo.AvailableFileSize)
            {
                throw new Exception(string.Format(deleteSourceFile ? FilesCommonResource.ErrorMassage_FileSizeMove : FilesCommonResource.ErrorMassage_FileSizeCopy,
                                                  FileSizeComment.FilesSizeToString(SetupInfo.AvailableFileSize)));
            }

            var securityDao = ServiceProvider.GetService<ISecurityDao<TFrom>>();
            var tagDao = ServiceProvider.GetService<ITagDao<TFrom>>();

            var fromFileShareRecords = securityDao.GetPureShareRecords(fromFile).Where(x => x.EntryType == FileEntryType.File);
            var fromFileNewTags = tagDao.GetNewTagsAsync(Guid.Empty, fromFile).ToListAsync().Result;
            var fromFileLockTag = tagDao.GetTagsAsync(fromFile.ID, FileEntryType.File, TagType.Locked).ToListAsync().Result.FirstOrDefault();
            var fromFileFavoriteTag = tagDao.GetTagsAsync(fromFile.ID, FileEntryType.File, TagType.Favorite).ToListAsync().Result;
            var fromFileTemplateTag = tagDao.GetTagsAsync(fromFile.ID, FileEntryType.File, TagType.Template).ToListAsync().Result;

            var toFile = ServiceProvider.GetService<File<TTo>>();

            toFile.Title = fromFile.Title;
            toFile.Encrypted = fromFile.Encrypted;
            toFile.FolderID = toConverter(toFolderId);

            fromFile.ID = fromConverter(fromFile.ID);

            var mustConvert = !string.IsNullOrEmpty(fromFile.ConvertedType);
            using (var fromFileStream = mustConvert
                                            ? await FileConverter.ExecAsync(fromFile)
                                            : await fromFileDao.GetFileStreamAsync(fromFile))
            {
                toFile.ContentLength = fromFileStream.CanSeek ? fromFileStream.Length : fromFile.ContentLength;
                toFile = await toFileDao.SaveFileAsync(toFile, fromFileStream);
            }

            if (fromFile.ThumbnailStatus == Thumbnail.Created)
            {
                using (var thumbnail = await fromFileDao.GetThumbnailAsync(fromFile))
                {
                    await toFileDao.SaveThumbnailAsync(toFile, thumbnail);
                }
                toFile.ThumbnailStatus = Thumbnail.Created;
            }

            if (deleteSourceFile)
            {
                if (fromFileShareRecords.Any())
                    fromFileShareRecords.ToList().ForEach(x =>
                    {
                        x.EntryId = toFile.ID;
                        securityDao.SetShare(x);
                    });

                var fromFileTags = fromFileNewTags;
                if (fromFileLockTag != null) fromFileTags.Add(fromFileLockTag);
                if (fromFileFavoriteTag != null) fromFileTags.AddRange(fromFileFavoriteTag);
                if (fromFileTemplateTag != null) fromFileTags.AddRange(fromFileTemplateTag);

                if (fromFileTags.Any())
                {
                    fromFileTags.ForEach(x => x.EntryId = toFile.ID);

                    tagDao.SaveTags(fromFileTags);
                }

                //Delete source file if needed
                await fromFileDao.DeleteFileAsync(fromConverter(fromFileId));
            }
            return toFile;
        }

        public Folder<TTo> PerformCrossDaoFolderCopy<TFrom, TTo>
            (TFrom fromFolderId, IFolderDao<TFrom> fromFolderDao, IFileDao<TFrom> fromFileDao, Func<TFrom, TFrom> fromConverter,
            TTo toRootFolderId, IFolderDao<TTo> toFolderDao, IFileDao<TTo> toFileDao, Func<TTo, TTo> toConverter,
            bool deleteSourceFolder, CancellationToken? cancellationToken)
        {
            var fromFolder = fromFolderDao.GetFolderAsync(fromConverter(fromFolderId)).Result;

            var toFolder1 = ServiceProvider.GetService<Folder<TTo>>();
            toFolder1.Title = fromFolder.Title;
            toFolder1.FolderID = toConverter(toRootFolderId);

            var toFolder = toFolderDao.GetFolderAsync(fromFolder.Title, toConverter(toRootFolderId)).Result;
            var toFolderId = toFolder != null
                                 ? toFolder.ID
                                 : toFolderDao.SaveFolderAsync(toFolder1).Result;

            var foldersToCopy = fromFolderDao.GetFoldersAsync(fromConverter(fromFolderId)).ToListAsync().Result;
            var fileIdsToCopy = fromFileDao.GetFilesAsync(fromConverter(fromFolderId)).Result;
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
                    fromFileShareRecords.ToList().ForEach(x =>
                    {
                        x.EntryId = toFolderId;
                        securityDao.SetShare(x);
                    });
                }

                var tagDao = ServiceProvider.GetService<ITagDao<TFrom>>();
                var fromFileNewTags = tagDao.GetNewTagsAsync(Guid.Empty, fromFolder).ToListAsync().Result;

                if (fromFileNewTags.Any())
                {
                    fromFileNewTags.ForEach(x => x.EntryId = toFolderId);

                    tagDao.SaveTags(fromFileNewTags);
                }

                if (copyException == null)
                    fromFolderDao.DeleteFolderAsync(fromConverter(fromFolderId)).Wait();
            }

            if (copyException != null) throw copyException;

            return toFolderDao.GetFolderAsync(toConverter(toFolderId)).Result;
        }

        public async Task<Folder<TTo>> PerformCrossDaoFolderCopyAsync<TFrom, TTo>
            (TFrom fromFolderId, IFolderDao<TFrom> fromFolderDao, IFileDao<TFrom> fromFileDao, Func<TFrom, TFrom> fromConverter,
            TTo toRootFolderId, IFolderDao<TTo> toFolderDao, IFileDao<TTo> toFileDao, Func<TTo, TTo> toConverter,
            bool deleteSourceFolder, CancellationToken? cancellationToken)
        {
            var fromFolder = await fromFolderDao.GetFolderAsync(fromConverter(fromFolderId));

            var toFolder1 = ServiceProvider.GetService<Folder<TTo>>();
            toFolder1.Title = fromFolder.Title;
            toFolder1.FolderID = toConverter(toRootFolderId);

            var toFolder = await toFolderDao.GetFolderAsync(fromFolder.Title, toConverter(toRootFolderId));
            var toFolderId = toFolder != null
                                 ? toFolder.ID
                                 : await toFolderDao.SaveFolderAsync(toFolder1);

            var foldersToCopy = await fromFolderDao.GetFoldersAsync(fromConverter(fromFolderId)).ToListAsync();
            var fileIdsToCopy = await fromFileDao.GetFilesAsync(fromConverter(fromFolderId));
            Exception copyException = null;
            //Copy files first
            foreach (var fileId in fileIdsToCopy)
            {
                if (cancellationToken.HasValue) cancellationToken.Value.ThrowIfCancellationRequested();
                try
                {
                    await PerformCrossDaoFileCopyAsync(fileId, fromFileDao, fromConverter,
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
                    await PerformCrossDaoFolderCopyAsync(folder.ID, fromFolderDao, fromFileDao, fromConverter,
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
                    fromFileShareRecords.ToList().ForEach(x =>
                    {
                        x.EntryId = toFolderId;
                        securityDao.SetShare(x);
                    });
                }

                var tagDao = ServiceProvider.GetService<ITagDao<TFrom>>();
                var fromFileNewTags = tagDao.GetNewTagsAsync(Guid.Empty, fromFolder).ToListAsync().Result;

                if (fromFileNewTags.Any())
                {
                    fromFileNewTags.ForEach(x => x.EntryId = toFolderId);

                    tagDao.SaveTags(fromFileNewTags);
                }

                if (copyException == null)
                    await fromFolderDao.DeleteFolderAsync(fromConverter(fromFolderId));
            }

            if (copyException != null) throw copyException;

            return await toFolderDao.GetFolderAsync(toConverter(toFolderId));
        }
    }

    public class CrossDaoExtension
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
