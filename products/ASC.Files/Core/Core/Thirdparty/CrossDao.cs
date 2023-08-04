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

namespace ASC.Files.Core.Thirdparty;

[Scope]
internal class CrossDao //Additional SharpBox
{
    private readonly IServiceProvider _serviceProvider;
    private readonly SetupInfo _setupInfo;
    private readonly FileConverter _fileConverter;
    private readonly SocketManager _socketManager;
    private readonly ThumbnailSettings _thumbnailSettings;
    public CrossDao(
        IServiceProvider serviceProvider,
        SetupInfo setupInfo,
        FileConverter fileConverter,
        ThumbnailSettings thumbnailSettings,
        SocketManager socketManager)
    {
        _serviceProvider = serviceProvider;
        _setupInfo = setupInfo;
        _fileConverter = fileConverter;
        _socketManager = socketManager;
        _thumbnailSettings = thumbnailSettings;
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
        var globalStore = _serviceProvider.GetService<GlobalStore>();

        var fromFileShareRecords = securityDao.GetPureShareRecordsAsync(fromFile);
        var fromFileNewTags = tagDao.GetNewTagsAsync(Guid.Empty, fromFile);
        var fromFileLockTag = await tagDao.GetTagsAsync(fromFile.Id, FileEntryType.File, TagType.Locked).FirstOrDefaultAsync();
        var fromFileFavoriteTag = await tagDao.GetTagsAsync(fromFile.Id, FileEntryType.File, TagType.Favorite).ToListAsync();
        var fromFileTemplateTag = await tagDao.GetTagsAsync(fromFile.Id, FileEntryType.File, TagType.Template).ToListAsync();

        var toFile = _serviceProvider.GetService<File<TTo>>();

        toFile.Title = fromFile.Title;
        toFile.Encrypted = fromFile.Encrypted;
        toFile.ParentId = toConverter(toFolderId);
        toFile.ThumbnailStatus = fromFile.ThumbnailStatus == Thumbnail.Created ? Thumbnail.Creating : Thumbnail.Waiting;

        fromFile.Id = fromConverter(fromFile.Id);

        var mustConvert = !string.IsNullOrEmpty(fromFile.ConvertedType);
        await using (var fromFileStream = mustConvert
                         ? await _fileConverter.ExecAsync(fromFile)
                         : await fromFileDao.GetFileStreamAsync(fromFile))
        {
            toFile.ContentLength = fromFileStream.CanSeek ? fromFileStream.Length : fromFile.ContentLength;
            toFile = await toFileDao.SaveFileAsync(toFile, fromFileStream);
        }

        if (fromFile.ThumbnailStatus == Thumbnail.Created)
        {
            foreach (var size in _thumbnailSettings.Sizes)
            {
                await (await globalStore.GetStoreAsync()).CopyAsync(String.Empty,
                                      fromFileDao.GetUniqThumbnailPath(fromFile, size.Width, size.Height),
                                      String.Empty,
                                      toFileDao.GetUniqThumbnailPath(toFile, size.Width, size.Height));
            }

            await toFileDao.SetThumbnailStatusAsync(toFile, Thumbnail.Created);

            toFile.ThumbnailStatus = Thumbnail.Created;
        }

        if (deleteSourceFile)
        {
            await foreach (var record in fromFileShareRecords.Where(x => x.EntryType == FileEntryType.File))
            {
                record.EntryId = toFile.Id;
                await securityDao.SetShareAsync(record);
            }

            var fromFileTags = await fromFileNewTags.ToListAsync();
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

                await tagDao.SaveTags(fromFileTags);
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

        if (toFolder == null)
        {
            await _socketManager.CreateFolderAsync(toFolder1);
        }

        var foldersToCopy = await fromFolderDao.GetFoldersAsync(fromConverter(fromFolderId)).ToListAsync();
        var fileIdsToCopy = await fromFileDao.GetFilesAsync(fromConverter(fromFolderId)).ToListAsync();
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
            var fromFileShareRecords = securityDao.GetPureShareRecordsAsync(fromFolder);

            await foreach (var record in fromFileShareRecords.Where(x => x.EntryType == FileEntryType.Folder))
            {
                record.EntryId = toFolderId;
                await securityDao.SetShareAsync(record);
            }

            var tagDao = _serviceProvider.GetService<ITagDao<TFrom>>();
            var fromFileNewTags = await tagDao.GetNewTagsAsync(Guid.Empty, fromFolder).ToListAsync();

            if (fromFileNewTags.Count > 0)
            {
                fromFileNewTags.ForEach(x => x.EntryId = toFolderId);

                await tagDao.SaveTags(fromFileNewTags);
            }

            if (copyException == null)
            {
                var id = fromConverter(fromFolderId);
                var folder = await fromFolderDao.GetFolderAsync(id);
                await fromFolderDao.DeleteFolderAsync(id);
                await _socketManager.DeleteFolder(folder);
            }
        }

        if (copyException != null)
        {
            throw copyException;
        }

        return await toFolderDao.GetFolderAsync(toConverter(toFolderId));
    }
}