namespace ASC.Files.Core.Thirdparty;

[Scope(Additional = typeof(CrossDaoExtension))]
internal class CrossDao //Additional SharpBox
{
    private readonly IServiceProvider _serviceProvider;
    private readonly SetupInfo _setupInfo;
    private readonly FileConverter _fileConverter;

    public CrossDao(
        IServiceProvider serviceProvider,
        SetupInfo setupInfo,
        FileConverter fileConverter)
    {
        _serviceProvider = serviceProvider;
        _setupInfo = setupInfo;
        _fileConverter = fileConverter;
    }

    public async Task<File<TTo>> PerformCrossDaoFileCopyAsync<TFrom, TTo>(
        TFrom fromFileId, IFileDao<TFrom> fromFileDao, Func<TFrom, TFrom> fromConverter,
        TTo toFolderId, IFileDao<TTo> toFileDao, Func<TTo, TTo> toConverter,
        bool deleteSourceFile)
    {
        //Get File from first dao
        var fromFile = await fromFileDao.GetFileAsync(fromConverter(fromFileId));

        if (fromFile.ContentLength > _setupInfo.AvailableFileSize)
        {
            throw new Exception(string.Format(deleteSourceFile ? FilesCommonResource.ErrorMassage_FileSizeMove : FilesCommonResource.ErrorMassage_FileSizeCopy,
                                              FileSizeComment.FilesSizeToString(_setupInfo.AvailableFileSize)));
        }

        var securityDao = _serviceProvider.GetService<ISecurityDao<TFrom>>();
        var tagDao = _serviceProvider.GetService<ITagDao<TFrom>>();

        var fromFileShareRecords = (await securityDao.GetPureShareRecordsAsync(fromFile)).Where(x => x.EntryType == FileEntryType.File);
        var fromFileNewTags = await tagDao.GetNewTagsAsync(Guid.Empty, fromFile).ToListAsync();
        var fromFileLockTag = (await tagDao.GetTagsAsync(fromFile.Id, FileEntryType.File, TagType.Locked).ToListAsync()).FirstOrDefault();
        var fromFileFavoriteTag = await tagDao.GetTagsAsync(fromFile.Id, FileEntryType.File, TagType.Favorite).ToListAsync();
        var fromFileTemplateTag = await tagDao.GetTagsAsync(fromFile.Id, FileEntryType.File, TagType.Template).ToListAsync();

        var toFile = _serviceProvider.GetService<File<TTo>>();

        toFile.Title = fromFile.Title;
        toFile.Encrypted = fromFile.Encrypted;
        toFile.ParentId = toConverter(toFolderId);

        fromFile.Id = fromConverter(fromFile.Id);

        var mustConvert = !string.IsNullOrEmpty(fromFile.ConvertedType);
        using (var fromFileStream = mustConvert
                                        ? await _fileConverter.ExecAsync(fromFile)
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
            {
                foreach (var record in fromFileShareRecords)
                {
                    record.EntryId = toFile.Id;
                    await securityDao.SetShareAsync(record);
                }
            }

            var fromFileTags = fromFileNewTags;
            if (fromFileLockTag != null)
            {
                fromFileTags.Add(fromFileLockTag);
            }

            if (fromFileFavoriteTag != null)
            {
                fromFileTags.AddRange(fromFileFavoriteTag);
            }

            if (fromFileTemplateTag != null)
            {
                fromFileTags.AddRange(fromFileTemplateTag);
            }

            if (fromFileTags.Count > 0)
            {
                fromFileTags.ForEach(x => x.EntryId = toFile.Id);

                tagDao.SaveTags(fromFileTags);
            }

            //Delete source file if needed
            await fromFileDao.DeleteFileAsync(fromConverter(fromFileId));
        }

        return toFile;
    }

    public async Task<Folder<TTo>> PerformCrossDaoFolderCopyAsync<TFrom, TTo>
        (TFrom fromFolderId, IFolderDao<TFrom> fromFolderDao, IFileDao<TFrom> fromFileDao, Func<TFrom, TFrom> fromConverter,
        TTo toRootFolderId, IFolderDao<TTo> toFolderDao, IFileDao<TTo> toFileDao, Func<TTo, TTo> toConverter,
        bool deleteSourceFolder, CancellationToken? cancellationToken)
    {
        var fromFolder = await fromFolderDao.GetFolderAsync(fromConverter(fromFolderId));

        var toFolder1 = _serviceProvider.GetService<Folder<TTo>>();
        toFolder1.Title = fromFolder.Title;
        toFolder1.ParentId = toConverter(toRootFolderId);

        var toFolder = await toFolderDao.GetFolderAsync(fromFolder.Title, toConverter(toRootFolderId));
        var toFolderId = toFolder != null
                             ? toFolder.Id
                             : await toFolderDao.SaveFolderAsync(toFolder1);

        var foldersToCopy = await fromFolderDao.GetFoldersAsync(fromConverter(fromFolderId)).ToListAsync();
        var fileIdsToCopy = await fromFileDao.GetFilesAsync(fromConverter(fromFolderId));
        Exception copyException = null;
        //Copy files first
        foreach (var fileId in fileIdsToCopy)
        {
            if (cancellationToken.HasValue)
            {
                cancellationToken.Value.ThrowIfCancellationRequested();
            }

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
            if (cancellationToken.HasValue)
            {
                cancellationToken.Value.ThrowIfCancellationRequested();
            }

            try
            {
                await PerformCrossDaoFolderCopyAsync(folder.Id, fromFolderDao, fromFileDao, fromConverter,
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
            var securityDao = _serviceProvider.GetService<ISecurityDao<TFrom>>();
            var fromFileShareRecords = (await securityDao.GetPureShareRecordsAsync(fromFolder))
                .Where(x => x.EntryType == FileEntryType.Folder);

            if (fromFileShareRecords.Any())
            {
                foreach (var record in fromFileShareRecords)
                {
                    record.EntryId = toFolderId;
                    await securityDao.SetShareAsync(record);
                }
            }

            var tagDao = _serviceProvider.GetService<ITagDao<TFrom>>();
            var fromFileNewTags = await tagDao.GetNewTagsAsync(Guid.Empty, fromFolder).ToListAsync();

            if (fromFileNewTags.Count > 0)
            {
                fromFileNewTags.ForEach(x => x.EntryId = toFolderId);

                tagDao.SaveTags(fromFileNewTags);
            }

            if (copyException == null)
            {
                await fromFolderDao.DeleteFolderAsync(fromConverter(fromFolderId));
            }
        }

        if (copyException != null)
        {
            throw copyException;
        }

        return await toFolderDao.GetFolderAsync(toConverter(toFolderId));
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
