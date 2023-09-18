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

namespace ASC.Web.Files.Services.WCFService.FileOperations;

[Transient]
class FileMoveCopyOperation : ComposeFileOperation<FileMoveCopyOperationData<string>, FileMoveCopyOperationData<int>>
{
    public FileMoveCopyOperation(IServiceProvider serviceProvider,
        FileOperation<FileMoveCopyOperationData<string>, string> thirdPartyOperation,
        FileOperation<FileMoveCopyOperationData<int>, int> daoOperation)
        : base(serviceProvider, thirdPartyOperation, daoOperation)
    {
        this[OpType] = (int)ThirdPartyOperation[OpType];
    }
}

internal class FileMoveCopyOperationData<T> : FileOperationData<T>
{
    public string ThirdPartyFolderId { get; }
    public int DaoFolderId { get; }
    public bool Copy { get; }
    public FileConflictResolveType ResolveType { get; }
    public IDictionary<string, StringValues> Headers { get; }

    public FileMoveCopyOperationData(IEnumerable<T> folders, IEnumerable<T> files, Tenant tenant, JsonElement toFolderId, bool copy, FileConflictResolveType resolveType,
        ExternalShareData externalShareData, bool holdResult = true, IDictionary<string, StringValues> headers = null)
        : base(folders, files, tenant, externalShareData, holdResult)
    {
        if (toFolderId.ValueKind == JsonValueKind.String)
        {
            if (!int.TryParse(toFolderId.GetString(), out var i))
            {
                ThirdPartyFolderId = toFolderId.GetString();
            }
            else
            {
                DaoFolderId = i;
            }
        }
        else if (toFolderId.ValueKind == JsonValueKind.Number)
        {
            DaoFolderId = toFolderId.GetInt32();
        }

        Copy = copy;
        ResolveType = resolveType;
        Headers = headers;
    }
}

class FileMoveCopyOperation<T> : FileOperation<FileMoveCopyOperationData<T>, T>
{
    private static readonly SemaphoreSlim _semaphore = new SemaphoreSlim(1);

    private readonly int _daoFolderId;
    private readonly string _thirdpartyFolderId;
    private readonly bool _copy;
    private readonly FileConflictResolveType _resolveType;
    private readonly IDictionary<string, StringValues> _headers;
    private readonly ThumbnailSettings _thumbnailSettings;
    private readonly Dictionary<T, Folder<T>> _parentRooms = new Dictionary<T, Folder<T>>();

    public FileMoveCopyOperation(IServiceProvider serviceProvider, FileMoveCopyOperationData<T> data, ThumbnailSettings thumbnailSettings)
        : base(serviceProvider, data)
    {
        _daoFolderId = data.DaoFolderId;
        _thirdpartyFolderId = data.ThirdPartyFolderId;
        _copy = data.Copy;
        _resolveType = data.ResolveType;

        _headers = data.Headers;
        _thumbnailSettings = thumbnailSettings;
        this[OpType] = (int)(_copy ? FileOperationType.Copy : FileOperationType.Move);
    }

    protected override async Task DoJob(IServiceScope scope)
    {
        if (_daoFolderId != 0)
        {
            await DoAsync(scope, _daoFolderId);
        }

        if (!string.IsNullOrEmpty(_thirdpartyFolderId))
        {
            await DoAsync(scope, _thirdpartyFolderId);
        }
    }

    private async Task DoAsync<TTo>(IServiceScope scope, TTo tto)
    {
        var fileMarker = scope.ServiceProvider.GetService<FileMarker>();
        var folderDao = scope.ServiceProvider.GetService<IFolderDao<TTo>>();
        var fileSecurity = scope.ServiceProvider.GetService<FileSecurity>();
        var socketManager = scope.ServiceProvider.GetService<SocketManager>();

        this[Res] += string.Format("folder_{0}{1}", _daoFolderId, SplitChar);

        //TODO: check on each iteration?
        var toFolder = await folderDao.GetFolderAsync(tto);
        if (toFolder == null)
        {
            return;
        }

        if (toFolder.FolderType != FolderType.VirtualRooms && toFolder.FolderType != FolderType.Archive)
        {
            if (!await FilesSecurity.CanCreateAsync(toFolder))
            {
                throw new SecurityException(FilesCommonResource.ErrorMassage_SecurityException_Create);
            }
        }

        var parentFolders = await folderDao.GetParentFoldersAsync(toFolder.Id).ToListAsync();
        if (parentFolders.Any(parent => Folders.Any(r => r.ToString() == parent.Id.ToString())))
        {
            this[Err] = FilesCommonResource.ErrorMassage_FolderCopyError;

            return;
        }

        if (0 < Folders.Count)
        {
            var firstFolder = await FolderDao.GetFolderAsync(Folders[0]);
            var isRoom = DocSpaceHelper.IsRoom(firstFolder.FolderType);

            if (_copy && !await FilesSecurity.CanCopyAsync(firstFolder))
            {
                this[Err] = FilesCommonResource.ErrorMassage_SecurityException_CopyFolder;

                return;
            }
            if (!_copy && !await FilesSecurity.CanMoveAsync(firstFolder))
            {
                if (isRoom)
                {
                    this[Err] = toFolder.FolderType == FolderType.Archive
                        ? FilesCommonResource.ErrorMessage_SecurityException_ArchiveRoom
                        : FilesCommonResource.ErrorMessage_SecurityException_UnarchiveRoom;

                }
                else
                {
                    this[Err] = FilesCommonResource.ErrorMassage_SecurityException_MoveFolder;
                }

                return;
            }
        }

        if (0 < Files.Count)
        {
            var firstFile = await FileDao.GetFileAsync(Files[0]);

            if (_copy && !await FilesSecurity.CanCopyAsync(firstFile))
            {
                this[Err] = FilesCommonResource.ErrorMassage_SecurityException_CopyFile;

                return;
            }
            if (!_copy && !await FilesSecurity.CanMoveAsync(firstFile))
            {
                this[Err] = FilesCommonResource.ErrorMassage_SecurityException_MoveFile;

                return;
            }
        }

        if (_copy && !await fileSecurity.CanCopyToAsync(toFolder))
        {
            this[Err] = FilesCommonResource.ErrorMessage_SecurityException_CopyToFolder;

            return;
        }
        if (!_copy && !await fileSecurity.CanMoveToAsync(toFolder))
        {
            this[Err] = toFolder.FolderType == FolderType.VirtualRooms ? FilesCommonResource.ErrorMessage_SecurityException_UnarchiveRoom :
                toFolder.FolderType == FolderType.Archive ? FilesCommonResource.ErrorMessage_SecurityException_UnarchiveRoom :
                FilesCommonResource.ErrorMessage_SecurityException_MoveToFolder;

            return;
        }

        var needToMark = new List<FileEntry>();

        var moveOrCopyFoldersTask = await MoveOrCopyFoldersAsync(scope, Folders, toFolder, _copy, parentFolders);
        var moveOrCopyFilesTask = await MoveOrCopyFilesAsync(scope, Files, toFolder, _copy, parentFolders);

        needToMark.AddRange(moveOrCopyFilesTask);

        foreach (var folder in moveOrCopyFoldersTask)
        {
            needToMark.AddRange(await GetFilesAsync(scope, folder));
            await socketManager.CreateFolderAsync(folder);
        }

        var ntm = needToMark.Distinct();
        foreach (var n in ntm)
        {
            if (n is FileEntry<T> entry1)
            {
                await fileMarker.MarkAsNewAsync(entry1);
            }
            else if (n is FileEntry<TTo> entry2)
            {
                await fileMarker.MarkAsNewAsync(entry2);
            }
        }
    }

    private async Task<List<File<TTo>>> GetFilesAsync<TTo>(IServiceScope scope, Folder<TTo> folder)
    {
        var fileDao = scope.ServiceProvider.GetService<IFileDao<TTo>>();

        var files = await fileDao.GetFilesAsync(folder.Id, new OrderBy(SortedByType.AZ, true), FilterType.FilesOnly, false, Guid.Empty, string.Empty, false, withSubfolders: true).ToListAsync();

        return files;
    }

    private async Task<List<Folder<TTo>>> MoveOrCopyFoldersAsync<TTo>(IServiceScope scope, List<T> folderIds, Folder<TTo> toFolder, bool copy, IEnumerable<Folder<TTo>> toFolderParents, bool checkPermissions = true)
    {
        var needToMark = new List<Folder<TTo>>();

        if (folderIds.Count == 0)
        {
            return needToMark;
        }

        var scopeClass = scope.ServiceProvider.GetService<FileMoveCopyOperationScope>();
        var (filesMessageService, fileMarker, _, _, _) = scopeClass;
        var folderDao = scope.ServiceProvider.GetService<IFolderDao<TTo>>();
        var countRoomChecker = scope.ServiceProvider.GetRequiredService<CountRoomChecker>();
        var socketManager = scope.ServiceProvider.GetService<SocketManager>();
        var tenantQuotaFeatureStatHelper = scope.ServiceProvider.GetService<TenantQuotaFeatureStatHelper>();
        var quotaSocketManager = scope.ServiceProvider.GetService<QuotaSocketManager>();

        var toFolderId = toFolder.Id;
        var isToFolder = Equals(toFolderId, _daoFolderId);

        var sb = new StringBuilder();
        sb.Append(this[Res]);
        foreach (var folderId in folderIds)
        {
            CancellationToken.ThrowIfCancellationRequested();

            var folder = await FolderDao.GetFolderAsync(folderId);

            var isRoom = DocSpaceHelper.IsRoom(folder.FolderType);
            var isThirdPartyRoom = isRoom && folder.ProviderEntry;

            var canMoveOrCopy = (copy && await FilesSecurity.CanCopyAsync(folder)) || (!copy && await FilesSecurity.CanMoveAsync(folder));
            checkPermissions = isRoom ? !canMoveOrCopy : checkPermissions;

            if (folder == null)
            {
                this[Err] = FilesCommonResource.ErrorMassage_FolderNotFound;
            }
            else if (copy && checkPermissions && !canMoveOrCopy)
            {
                this[Err] = FilesCommonResource.ErrorMassage_SecurityException_CopyFolder;
            }
            else if (!copy && checkPermissions && !canMoveOrCopy)
            {
                if (isRoom)
                {
                    this[Err] = toFolder.FolderType == FolderType.Archive
                        ? FilesCommonResource.ErrorMessage_SecurityException_ArchiveRoom
                        : FilesCommonResource.ErrorMessage_SecurityException_UnarchiveRoom;
                }
                else
                {
                    this[Err] = FilesCommonResource.ErrorMassage_SecurityException_MoveFolder;
                }
            }
            else if (!isRoom && (toFolder.FolderType == FolderType.VirtualRooms || toFolder.RootFolderType == FolderType.Archive))
            {
                this[Err] = FilesCommonResource.ErrorMassage_SecurityException_MoveFolder;
            }
            else if (isRoom && toFolder.FolderType != FolderType.VirtualRooms && toFolder.FolderType != FolderType.Archive)
            {
                this[Err] = FilesCommonResource.ErrorMessage_SecurityException_UnarchiveRoom;
            }
            else if (!isRoom && folder.Private && !await CompliesPrivateRoomRulesAsync(folder, toFolderParents))
            {
                this[Err] = FilesCommonResource.ErrorMassage_SecurityException_MoveFolder;
            }
            else if (checkPermissions && folder.RootFolderType != FolderType.TRASH && !await FilesSecurity.CanDownloadAsync(folder))
            {
                this[Err] = FilesCommonResource.ErrorMassage_SecurityException;
            }
            else if (folder.RootFolderType == FolderType.Privacy
                && (copy || toFolder.RootFolderType != FolderType.Privacy))
            {
                this[Err] = FilesCommonResource.ErrorMassage_SecurityException_MoveFolder;
            }
            else if (!Equals(folder.ParentId ?? default, toFolderId) || _resolveType == FileConflictResolveType.Duplicate)
            {
                var files = await FileDao.GetFilesAsync(folder.Id, new OrderBy(SortedByType.AZ, true), FilterType.FilesOnly, false, Guid.Empty, string.Empty, false, withSubfolders: true).ToListAsync();
                var (isError, message) = await WithErrorAsync(scope, files, checkPermissions);

                try
                {
                    //if destination folder contains folder with same name then merge folders
                    var conflictFolder = (folder.RootFolderType == FolderType.Privacy || isRoom)
                        ? null
                        : await folderDao.GetFolderAsync(folder.Title, toFolderId);
                    Folder<TTo> newFolder;

                    if (copy || conflictFolder != null)
                    {
                        if (conflictFolder != null)
                        {
                            newFolder = conflictFolder;

                            if (isToFolder)
                            {
                                needToMark.Add(conflictFolder);
                            }
                        }
                        else
                        {
                            newFolder = await FolderDao.CopyFolderAsync(folder.Id, toFolderId, CancellationToken);
                            _ = filesMessageService.SendAsync(newFolder, toFolder, _headers, MessageAction.FolderCopied, newFolder.Title, toFolder.Title);

                            if (isToFolder)
                            {
                                needToMark.Add(newFolder);
                            }

                            if (ProcessedFolder(folderId))
                            {
                                sb.Append($"folder_{newFolder.Id}{SplitChar}");
                            }
                        }

                        if (toFolder.ProviderId == folder.ProviderId // crossDao operation is always recursive
                            && FolderDao.UseRecursiveOperation(folder.Id, toFolderId))
                        {
                            await MoveOrCopyFilesAsync(scope, await FileDao.GetFilesAsync(folder.Id).ToListAsync(), newFolder, copy, toFolderParents, checkPermissions);
                            await MoveOrCopyFoldersAsync(scope, await FolderDao.GetFoldersAsync(folder.Id).Select(f => f.Id).ToListAsync(), newFolder, copy, toFolderParents, checkPermissions);

                            if (!copy)
                            {
                                if (checkPermissions && !await FilesSecurity.CanMoveAsync(folder))
                                {
                                    this[Err] = FilesCommonResource.ErrorMassage_SecurityException_MoveFolder;
                                }
                                else if (await FolderDao.IsEmptyAsync(folder.Id))
                                {
                                    await FolderDao.DeleteFolderAsync(folder.Id);

                                    await socketManager.DeleteFolder(folder);

                                    if (ProcessedFolder(folderId))
                                    {
                                        sb.Append($"folder_{newFolder.Id}{SplitChar}");
                                    }
                                }
                            }
                        }
                        else
                        {
                            if (conflictFolder != null)
                            {
                                TTo newFolderId;
                                if (copy)
                                {
                                    newFolder = await FolderDao.CopyFolderAsync(folder.Id, toFolderId, CancellationToken);
                                    newFolderId = newFolder.Id;
                                    _ = filesMessageService.SendAsync(newFolder, toFolder, _headers, MessageAction.FolderCopiedWithOverwriting, newFolder.Title, toFolder.Title);

                                    if (isToFolder)
                                    {
                                        needToMark.Add(newFolder);
                                    }

                                    if (ProcessedFolder(folderId))
                                    {
                                        sb.Append($"folder_{newFolderId}{SplitChar}");
                                    }
                                }
                                else if (checkPermissions && !await FilesSecurity.CanMoveAsync(folder))
                                {
                                    this[Err] = FilesCommonResource.ErrorMassage_SecurityException_MoveFolder;
                                }
                                else if (isError)
                                {
                                    this[Err] = message;
                                }
                                else
                                {
                                    await fileMarker.RemoveMarkAsNewForAllAsync(folder);

                                    newFolderId = await FolderDao.MoveFolderAsync(folder.Id, toFolderId, CancellationToken);
                                    newFolder = await folderDao.GetFolderAsync(newFolderId);
                                    _ = filesMessageService.SendAsync(folder, toFolder, _headers, MessageAction.FolderMovedWithOverwriting, folder.Title, toFolder.Title);

                                    if (isToFolder)
                                    {
                                        needToMark.Add(newFolder);
                                    }

                                    if (ProcessedFolder(folderId))
                                    {
                                        sb.Append($"folder_{newFolderId}{SplitChar}");
                                    }
                                }
                            }
                        }
                    }
                    else
                    {
                        if (checkPermissions && !await FilesSecurity.CanMoveAsync(folder))
                        {
                            this[Err] = FilesCommonResource.ErrorMassage_SecurityException_MoveFolder;
                        }
                        else if (isError)
                        {
                            this[Err] = message;
                        }
                        else
                        {
                            await fileMarker.RemoveMarkAsNewForAllAsync(folder);
                            var parentFolder = await FolderDao.GetFolderAsync(folder.RootId);

                            TTo newFolderId = default;

                            if (isThirdPartyRoom)
                            {
                                await ProviderDao.UpdateProviderInfoAsync(folder.ProviderId, toFolder.FolderType);
                            }
                            else
                            {
                                try
                                {
                                    if (isRoom && toFolder.FolderType == FolderType.VirtualRooms)
                                    {
                                        await _semaphore.WaitAsync();
                                        await countRoomChecker.CheckAppend();
                                        newFolderId = await FolderDao.MoveFolderAsync(folder.Id, toFolderId, CancellationToken);
                                        await socketManager.DeleteFolder(folder);

                                        var (name, value) = await tenantQuotaFeatureStatHelper.GetStatAsync<CountRoomFeature, int>();
                                        _ = quotaSocketManager.ChangeQuotaUsedValueAsync(name, value);
                                    }
                                    else if (isRoom && toFolder.FolderType == FolderType.Archive)
                                    {
                                        await _semaphore.WaitAsync();
                                        newFolderId = await FolderDao.MoveFolderAsync(folder.Id, toFolderId, CancellationToken);

                                        var (name, value) = await tenantQuotaFeatureStatHelper.GetStatAsync<CountRoomFeature, int>();
                                        _ = quotaSocketManager.ChangeQuotaUsedValueAsync(name, value);
                                    }
                                    else
                                    {
                                        newFolderId = await FolderDao.MoveFolderAsync(folder.Id, toFolderId, CancellationToken);
                                    }
                                }
                                catch (Exception)
                                {
                                    throw;
                                }
                                finally
                                {
                                    _semaphore.Release();
                                }
                            }

                            if (isRoom)
                            {
                                if (toFolder.FolderType == FolderType.Archive)
                                {
                                    var pins = await TagDao.GetTagsAsync(Guid.Empty, TagType.Pin, new List<FileEntry<T>> { folder }).ToListAsync();
                                    if (pins.Count > 0)
                                    {
                                        await TagDao.RemoveTags(pins);
                                    }

                                    _ = filesMessageService.SendAsync(folder, _headers, MessageAction.RoomArchived, folder.Title);
                                }
                                else
                                {
                                    _ = filesMessageService.SendAsync(folder, _headers, MessageAction.RoomUnarchived, folder.Title);
                                }
                            }
                            else
                            {
                                _ = filesMessageService.SendAsync(folder, toFolder, _headers, MessageAction.FolderMoved, folder.Title, parentFolder.Title, toFolder.Title);
                            }


                            if (isToFolder)
                            {
                                newFolder = await folderDao.GetFolderAsync(newFolderId);
                                needToMark.Add(newFolder);
                            }

                            if (ProcessedFolder(folderId))
                            {
                                var id = isThirdPartyRoom ? folder.Id.ToString() : newFolderId.ToString();
                                sb.Append($"folder_{id}{SplitChar}");
                            }
                        }
                    }
                    this[Res] = sb.ToString();
                }
                catch (Exception ex)
                {
                    this[Err] = ex.Message;

                    Logger.ErrorWithException(ex);
                }
            }

            ProgressStep(FolderDao.CanCalculateSubitems(folderId) ? default : folderId);
        }

        return needToMark;
    }

    private async Task<List<FileEntry<TTo>>> MoveOrCopyFilesAsync<TTo>(IServiceScope scope, List<T> fileIds, Folder<TTo> toFolder, bool copy, IEnumerable<Folder<TTo>> toParentFolders, bool checkPermissions = true)
    {
        var needToMark = new List<FileEntry<TTo>>();

        if (fileIds.Count == 0)
        {
            return needToMark;
        }

        var scopeClass = scope.ServiceProvider.GetService<FileMoveCopyOperationScope>();
        var (filesMessageService, fileMarker, fileUtility, global, entryManager) = scopeClass;
        var fileDao = scope.ServiceProvider.GetService<IFileDao<TTo>>();
        var fileTracker = scope.ServiceProvider.GetService<FileTrackerHelper>();
        var socketManager = scope.ServiceProvider.GetService<SocketManager>();
        var globalStorage = scope.ServiceProvider.GetService<GlobalStore>();

        var toFolderId = toFolder.Id;
        var sb = new StringBuilder();
        foreach (var fileId in fileIds)
        {
            CancellationToken.ThrowIfCancellationRequested();

            var file = await FileDao.GetFileAsync(fileId);
            var (isError, message) = await WithErrorAsync(scope, new[] { file }, checkPermissions);

            if (file == null)
            {
                this[Err] = FilesCommonResource.ErrorMassage_FileNotFound;
            }
            else if (toFolder.FolderType == FolderType.VirtualRooms || toFolder.RootFolderType == FolderType.Archive)
            {
                this[Err] = FilesCommonResource.ErrorMassage_SecurityException_MoveFile;
            }
            else if (copy && !await FilesSecurity.CanCopyAsync(file))
            {
                this[Err] = FilesCommonResource.ErrorMassage_SecurityException_CopyFile;
            }
            else if (!copy && checkPermissions && !await FilesSecurity.CanMoveAsync(file))
            {
                this[Err] = FilesCommonResource.ErrorMassage_SecurityException_MoveFile;
            }
            else if (checkPermissions && file.RootFolderType != FolderType.TRASH && !await FilesSecurity.CanDownloadAsync(file))
            {
                this[Err] = FilesCommonResource.ErrorMassage_SecurityException;
            }
            else if (!await CompliesPrivateRoomRulesAsync(file, toParentFolders))
            {
                this[Err] = FilesCommonResource.ErrorMassage_SecurityException_MoveFile;
            }
            else if (file.RootFolderType == FolderType.Privacy
                && (copy || toFolder.RootFolderType != FolderType.Privacy))
            {
                this[Err] = FilesCommonResource.ErrorMassage_SecurityException_MoveFile;
            }
            else if (global.EnableUploadFilter
                     && !fileUtility.ExtsUploadable.Contains(FileUtility.GetFileExtension(file.Title)))
            {
                this[Err] = FilesCommonResource.ErrorMassage_NotSupportedFormat;
            }
            else
            {
                var parentFolder = await FolderDao.GetFolderAsync(file.ParentId);
                try
                {
                    var conflict = _resolveType == FileConflictResolveType.Duplicate
                        || file.RootFolderType == FolderType.Privacy || file.Encrypted
                                       ? null
                                       : await fileDao.GetFileAsync(toFolderId, file.Title);
                    if (conflict == null)
                    {
                        File<TTo> newFile = null;
                        if (copy)
                        {
                            try
                            {
                                newFile = await FileDao.CopyFileAsync(file.Id, toFolderId); //Stream copy will occur inside dao
                                _ = filesMessageService.SendAsync(newFile, toFolder, _headers, MessageAction.FileCopied, newFile.Title, parentFolder.Title, toFolder.Title);

                                needToMark.Add(newFile);

                                await socketManager.CreateFileAsync(newFile);

                                if (ProcessedFile(fileId))
                                {
                                    sb.Append($"file_{newFile.Id}{SplitChar}");
                                }
                            }
                            catch
                            {
                                if (newFile != null)
                                {
                                    await fileDao.DeleteFileAsync(newFile.Id);
                                }

                                throw;
                            }
                        }
                        else
                        {
                            if (isError)
                            {
                                this[Err] = message;
                            }
                            else
                            {
                                await fileMarker.RemoveMarkAsNewForAllAsync(file);

                                var newFileId = await FileDao.MoveFileAsync(file.Id, toFolderId);
                                newFile = await fileDao.GetFileAsync(newFileId);

                                _ = filesMessageService.SendAsync(file, toFolder, _headers, MessageAction.FileMoved, file.Title, parentFolder.Title, toFolder.Title);

                                if (file.RootFolderType == FolderType.TRASH && newFile.ThumbnailStatus == Thumbnail.NotRequired)
                                {
                                    newFile.ThumbnailStatus = Thumbnail.Waiting;

                                    await fileDao.SetThumbnailStatusAsync(newFile, Thumbnail.Waiting);
                                }

                                if (newFile.ProviderEntry)
                                {
                                    await LinkDao.DeleteAllLinkAsync(file.Id.ToString());
                                }

                                if (Equals(toFolderId, _daoFolderId))
                                {
                                    needToMark.Add(newFile);
                                }

                                await socketManager.DeleteFileAsync(file);

                                await socketManager.CreateFileAsync(newFile);

                                if (ProcessedFile(fileId))
                                {
                                    sb.Append($"file_{newFileId}{SplitChar}");
                                }
                            }
                        }
                    }
                    else
                    {
                        if (_resolveType == FileConflictResolveType.Overwrite)
                        {
                            if (checkPermissions && !await FilesSecurity.CanEditAsync(conflict) && !await FilesSecurity.CanFillFormsAsync(conflict))
                            {
                                this[Err] = FilesCommonResource.ErrorMassage_SecurityException;
                            }
                            else if (await entryManager.FileLockedForMeAsync(conflict.Id))
                            {
                                this[Err] = FilesCommonResource.ErrorMassage_LockedFile;
                            }
                            else if (fileTracker.IsEditing(conflict.Id))
                            {
                                this[Err] = FilesCommonResource.ErrorMassage_SecurityException_UpdateEditingFile;
                            }
                            else
                            {
                                var newFile = conflict;
                                newFile.Version++;
                                newFile.VersionGroup++;
                                newFile.PureTitle = file.PureTitle;
                                newFile.ConvertedType = file.ConvertedType;
                                newFile.Comment = FilesCommonResource.CommentOverwrite;
                                newFile.Encrypted = file.Encrypted;
                                newFile.ThumbnailStatus = file.ThumbnailStatus == Thumbnail.Created ? Thumbnail.Creating : Thumbnail.Waiting;


                                await using (var stream = await FileDao.GetFileStreamAsync(file))
                                {
                                    newFile.ContentLength = stream.CanSeek ? stream.Length : file.ContentLength;

                                    newFile = await fileDao.SaveFileAsync(newFile, stream);
                                }

                                if (file.ThumbnailStatus == Thumbnail.Created)
                                {
                                    foreach (var size in _thumbnailSettings.Sizes)
                                    {
                                        await (await globalStorage.GetStoreAsync()).CopyAsync(String.Empty,
                                                                                FileDao.GetUniqThumbnailPath(file, size.Width, size.Height),
                                                                                String.Empty,
                                                                                fileDao.GetUniqThumbnailPath(newFile, size.Width, size.Height));
                                    }

                                    await fileDao.SetThumbnailStatusAsync(newFile, Thumbnail.Created);

                                    newFile.ThumbnailStatus = Thumbnail.Created;
                                }

                                await LinkDao.DeleteAllLinkAsync(newFile.Id.ToString());

                                needToMark.Add(newFile);

                                await socketManager.CreateFileAsync(newFile);

                                if (copy)
                                {
                                    _ = filesMessageService.SendAsync(newFile, toFolder, _headers, MessageAction.FileCopiedWithOverwriting, newFile.Title, parentFolder.Title, toFolder.Title);
                                    if (ProcessedFile(fileId))
                                    {
                                        sb.Append($"file_{newFile.Id}{SplitChar}");
                                    }
                                }
                                else
                                {
                                    if (Equals(file.ParentId.ToString(), toFolderId.ToString()))
                                    {
                                        if (ProcessedFile(fileId))
                                        {
                                            sb.Append($"file_{newFile.Id}{SplitChar}");
                                        }
                                    }
                                    else
                                    {
                                        if (isError)
                                        {
                                            this[Err] = message;
                                        }
                                        else
                                        {
                                            await FileDao.DeleteFileAsync(file.Id);

                                            await LinkDao.DeleteAllLinkAsync(file.Id.ToString());

                                            _ = filesMessageService.SendAsync(file, toFolder, _headers, MessageAction.FileMovedWithOverwriting, file.Title, parentFolder.Title, toFolder.Title);

                                            await socketManager.DeleteFileAsync(file);

                                            if (ProcessedFile(fileId))
                                            {
                                                sb.Append($"file_{newFile.Id}{SplitChar}");
                                            }
                                        }
                                    }
                                }
                            }
                        }
                        else if (_resolveType == FileConflictResolveType.Skip)
                        {
                            //nothing
                        }
                    }
                }
                catch (Exception ex)
                {
                    this[Err] = ex.Message;

                    Logger.ErrorWithException(ex);
                }
            }

            ProgressStep(fileId: FolderDao.CanCalculateSubitems(fileId) ? default : fileId);
        }

        this[Res] = sb.ToString();

        return needToMark;
    }

    private async Task<(bool isError, string message)> WithErrorAsync(IServiceScope scope, IEnumerable<File<T>> files, bool checkPermissions = true)
    {
        var entryManager = scope.ServiceProvider.GetService<EntryManager>();
        var fileTracker = scope.ServiceProvider.GetService<FileTrackerHelper>();
        string error = null;
        foreach (var file in files)
        {
            if (checkPermissions && !await FilesSecurity.CanMoveAsync(file))
            {
                error = FilesCommonResource.ErrorMassage_SecurityException_MoveFile;

                return (true, error);
            }
            if (checkPermissions && await entryManager.FileLockedForMeAsync(file.Id))
            {
                error = FilesCommonResource.ErrorMassage_LockedFile;

                return (true, error);
            }
            if (fileTracker.IsEditing(file.Id))
            {
                error = FilesCommonResource.ErrorMassage_SecurityException_UpdateEditingFile;

                return (true, error);
            }
        }
        return (false, error);
    }

    private async Task<bool> CompliesPrivateRoomRulesAsync<TTo>(FileEntry<T> entry, IEnumerable<Folder<TTo>> toFolderParents)
    {
        Folder<T> entryParentRoom;

        if (_parentRooms.ContainsKey(entry.ParentId))
        {
            entryParentRoom = _parentRooms.Get(entry.ParentId);
        }
        else
        {
            entryParentRoom = await FolderDao.GetParentFoldersAsync(entry.ParentId).FirstOrDefaultAsync(f => f.Private && DocSpaceHelper.IsRoom(f.FolderType));
            _parentRooms.Add(entry.ParentId, entryParentRoom);
        }

        var toFolderParentRoom = toFolderParents.FirstOrDefault(f => f.Private && DocSpaceHelper.IsRoom(f.FolderType));

        if (entryParentRoom == null && toFolderParentRoom == null)
        {
            return true;
        }

        if (entryParentRoom != null && toFolderParentRoom == null
            || entryParentRoom == null && toFolderParentRoom != null)
        {
            return false;
        }

        return entryParentRoom.Id.Equals(toFolderParentRoom.Id) && !_copy;
    }
}

[Scope]
public class FileMoveCopyOperationScope
{
    private readonly FilesMessageService _filesMessageService;
    private readonly FileMarker _fileMarker;
    private readonly FileUtility _fileUtility;
    private readonly Global _global;
    private readonly EntryManager _entryManager;

    public FileMoveCopyOperationScope(FilesMessageService filesMessageService, FileMarker fileMarker, FileUtility fileUtility, Global global, EntryManager entryManager)
    {
        _filesMessageService = filesMessageService;
        _fileMarker = fileMarker;
        _fileUtility = fileUtility;
        _global = global;
        _entryManager = entryManager;
    }

    public void Deconstruct(out FilesMessageService filesMessageService, out FileMarker fileMarker, out FileUtility fileUtility, out Global global, out EntryManager entryManager)
    {
        filesMessageService = _filesMessageService;
        fileMarker = _fileMarker;
        fileUtility = _fileUtility;
        global = _global;
        entryManager = _entryManager;
    }
}
