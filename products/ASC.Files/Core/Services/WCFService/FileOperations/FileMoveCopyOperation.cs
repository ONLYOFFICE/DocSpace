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

    }

    public override FileOperationType OperationType => ThirdPartyOperation.OperationType;
}

internal class FileMoveCopyOperationData<T> : FileOperationData<T>
{
    public string ThirdpartyFolderId { get; }
    public int DaoFolderId { get; }
    public bool Copy { get; }
    public FileConflictResolveType ResolveType { get; }
    public IDictionary<string, StringValues> Headers { get; }

    public FileMoveCopyOperationData(IEnumerable<T> folders, IEnumerable<T> files, Tenant tenant, JsonElement toFolderId, bool copy, FileConflictResolveType resolveType, bool holdResult = true, IDictionary<string, StringValues> headers = null)
        : base(folders, files, tenant, holdResult)
    {
        if (toFolderId.ValueKind == JsonValueKind.String)
        {
            if (!int.TryParse(toFolderId.GetString(), out var i))
            {
                ThirdpartyFolderId = toFolderId.GetString();
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
    private readonly int _daoFolderId;
    private readonly string _thirdpartyFolderId;
    private readonly bool _copy;
    private readonly FileConflictResolveType _resolveType;
    private readonly IDictionary<string, StringValues> _headers;
    private readonly ThumbnailSettings _thumbnailSettings;

    public override FileOperationType OperationType => _copy ? FileOperationType.Copy : FileOperationType.Move;

    public FileMoveCopyOperation(IServiceProvider serviceProvider, FileMoveCopyOperationData<T> data, ThumbnailSettings thumbnailSettings)
    : base(serviceProvider, data)
    {
        _daoFolderId = data.DaoFolderId;
        _thirdpartyFolderId = data.ThirdpartyFolderId;
        _copy = data.Copy;
        _resolveType = data.ResolveType;

        _headers = data.Headers;
        _thumbnailSettings = thumbnailSettings;
    }

    protected override async Task DoAsync(IServiceScope scope)
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

        Result += string.Format("folder_{0}{1}", _daoFolderId, SplitChar);

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
            Error = FilesCommonResource.ErrorMassage_FolderCopyError;

            return;
        }

        if (_copy)
        {
            Folder<T> rootFrom = null;
            if (0 < Folders.Count)
            {
                rootFrom = await FolderDao.GetRootFolderAsync(Folders[0]);
            }

            if (0 < Files.Count)
            {
                rootFrom = await FolderDao.GetRootFolderByFileAsync(Files[0]);
            }

            if (rootFrom != null)
            {
                if (rootFrom.FolderType == FolderType.TRASH)
                {
                    throw new InvalidOperationException("Can not copy from Trash.");
                }

                if (rootFrom.FolderType == FolderType.Archive)
                {
                    throw new InvalidOperationException("Can not copy from Archive.");
                }
            }

            if (toFolder.RootFolderType == FolderType.TRASH)
            {
                throw new InvalidOperationException("Can not copy to Trash.");
            }

            if (toFolder.RootFolderType == FolderType.Archive)
            {
                throw new InvalidOperationException("Can not copy to Archive");
            }
        }

        var needToMark = new List<FileEntry<TTo>>();

        var moveOrCopyFoldersTask = await MoveOrCopyFoldersAsync(scope, Folders, toFolder, _copy);
        var moveOrCopyFilesTask = await MoveOrCopyFilesAsync(scope, Files, toFolder, _copy);

        needToMark.AddRange(moveOrCopyFoldersTask);
        needToMark.AddRange(moveOrCopyFilesTask);

        var ntm = needToMark.Distinct();
        foreach (var n in ntm)
        {
            await fileMarker.MarkAsNewAsync(n);
        }
    }

    private async Task<List<FileEntry<TTo>>> MoveOrCopyFoldersAsync<TTo>(IServiceScope scope, List<T> folderIds, Folder<TTo> toFolder, bool copy)
    {
        var needToMark = new List<FileEntry<TTo>>();

        if (folderIds.Count == 0)
        {
            return needToMark;
        }

        var scopeClass = scope.ServiceProvider.GetService<FileMoveCopyOperationScope>();
        var (filesMessageService, fileMarker, _, _, _) = scopeClass;
        var folderDao = scope.ServiceProvider.GetService<IFolderDao<TTo>>();

        var toFolderId = toFolder.Id;
        var isToFolder = Equals(toFolderId, _daoFolderId);

        var sb = new StringBuilder();
        sb.Append(Result);
        foreach (var folderId in folderIds)
        {
            CancellationToken.ThrowIfCancellationRequested();

            var folder = await FolderDao.GetFolderAsync(folderId);
            var (isError, message) = await WithErrorAsync(scope, await FileDao.GetFilesAsync(folder.Id, new OrderBy(SortedByType.AZ, true), FilterType.FilesOnly, false, Guid.Empty, string.Empty, false, true).ToListAsync());

            var isRoom = DocSpaceHelper.IsRoom(folder.FolderType);

            var canEditRoom = await FilesSecurity.CanEditRoomAsync(folder);

            if (folder == null)
            {
                Error = FilesCommonResource.ErrorMassage_FolderNotFound;
            }
            else if (!await FilesSecurity.CanReadAsync(folder))
            {
                Error = FilesCommonResource.ErrorMassage_SecurityException_ReadFolder;
            }
            else if (isRoom && !canEditRoom)
            {
                Error = FilesCommonResource.ErrorMassage_SecurityException;
            }
            else if (!isRoom && (toFolder.FolderType == FolderType.VirtualRooms || toFolder.RootFolderType == FolderType.Archive))
            {
                Error = FilesCommonResource.ErrorMassage_SecurityException_MoveFolder;
            }
            else if (isRoom && toFolder.FolderType != FolderType.VirtualRooms && toFolder.FolderType != FolderType.Archive)
            {
                Error = FilesCommonResource.ErrorMessage_UnarchiveRoom;
            }
            else if (!await FilesSecurity.CanDownloadAsync(folder))
            {
                Error = FilesCommonResource.ErrorMassage_SecurityException;
            }
            else if (folder.RootFolderType == FolderType.Privacy
                && (copy || toFolder.RootFolderType != FolderType.Privacy))
            {
                Error = FilesCommonResource.ErrorMassage_SecurityException_MoveFolder;
            }
            else if (!Equals(folder.ParentId ?? default, toFolderId) || _resolveType == FileConflictResolveType.Duplicate)
            {
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
                            filesMessageService.Send(newFolder, toFolder, _headers, MessageAction.FolderCopied, newFolder.Title, toFolder.Title);

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
                            await MoveOrCopyFilesAsync(scope, await FileDao.GetFilesAsync(folder.Id).ToListAsync(), newFolder, copy);
                            await MoveOrCopyFoldersAsync(scope, await FolderDao.GetFoldersAsync(folder.Id).Select(f => f.Id).ToListAsync(), newFolder, copy);

                            if (!copy)
                            {
                                if (!await FilesSecurity.CanDeleteAsync(folder))
                                {
                                    Error = FilesCommonResource.ErrorMassage_SecurityException_MoveFolder;
                                }
                                else if (await FolderDao.IsEmptyAsync(folder.Id))
                                {
                                    await FolderDao.DeleteFolderAsync(folder.Id);

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
                                    filesMessageService.Send(newFolder, toFolder, _headers, MessageAction.FolderCopiedWithOverwriting, newFolder.Title, toFolder.Title);

                                    if (isToFolder)
                                    {
                                        needToMark.Add(newFolder);
                                    }

                                    if (ProcessedFolder(folderId))
                                    {
                                        sb.Append($"folder_{newFolderId}{SplitChar}");
                                    }
                                }
                                else if (!await FilesSecurity.CanDeleteAsync(folder))
                                {
                                    Error = FilesCommonResource.ErrorMassage_SecurityException_MoveFolder;
                                }
                                else if (isError)
                                {
                                    Error = message;
                                }
                                else
                                {
                                    await fileMarker.RemoveMarkAsNewForAllAsync(folder);

                                    newFolderId = await FolderDao.MoveFolderAsync(folder.Id, toFolderId, CancellationToken);
                                    newFolder = await folderDao.GetFolderAsync(newFolderId);
                                    filesMessageService.Send(folder, toFolder, _headers, MessageAction.FolderMovedWithOverwriting, folder.Title, toFolder.Title);

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
                        if (!isRoom && !await FilesSecurity.CanDeleteAsync(folder))
                        {
                            Error = FilesCommonResource.ErrorMassage_SecurityException_MoveFolder;
                        }
                        else if (isRoom && !await FilesSecurity.CanEditRoomAsync(folder))
                        {
                            Error = FilesCommonResource.ErrorMassage_SecurityException_MoveFolder;
                        }
                        else if (isError)
                        {
                            Error = message;
                        }
                        else
                        {
                            await fileMarker.RemoveMarkAsNewForAllAsync(folder);
                            var parentFolder = await FolderDao.GetFolderAsync(folder.RootId);

                            TTo newFolderId = default;

                            if (isRoom && folder.ProviderEntry)
                            {
                                await ProviderDao.UpdateProviderInfoAsync(folder.ProviderId, toFolder.FolderType);
                                newFolderId = (TTo)Convert.ChangeType(folder, typeof(TTo));
                            }
                            else
                            {
                                newFolderId = await FolderDao.MoveFolderAsync(folder.Id, toFolderId, CancellationToken);
                            }

                            newFolder = await folderDao.GetFolderAsync(newFolderId);

                            if (isRoom)
                            {
                                if (toFolder.FolderType == FolderType.Archive)
                                {
                                    filesMessageService.Send(folder, _headers, MessageAction.RoomArchived, folder.Title);
                                }
                                else
                                {
                                    filesMessageService.Send(folder, _headers, MessageAction.RoomUnarchived, folder.Title);
                                }
                            }
                            else
                            {
                                filesMessageService.Send(folder, toFolder, _headers, MessageAction.FolderMovedFrom, folder.Title, parentFolder.Title, toFolder.Title);
                            }


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
                    Result = sb.ToString();
                }
                catch (Exception ex)
                {
                    Error = ex.Message;

                    Logger.ErrorWithException(ex);
                }
            }

            ProgressStep(FolderDao.CanCalculateSubitems(folderId) ? default : folderId);
        }

        return needToMark;
    }

    private async Task<List<FileEntry<TTo>>> MoveOrCopyFilesAsync<TTo>(IServiceScope scope, List<T> fileIds, Folder<TTo> toFolder, bool copy)
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

        var toFolderId = toFolder.Id;
        var sb = new StringBuilder();
        foreach (var fileId in fileIds)
        {
            CancellationToken.ThrowIfCancellationRequested();

            var file = await FileDao.GetFileAsync(fileId);
            var (isError, message) = await WithErrorAsync(scope, new[] { file });

            if (file == null)
            {
                Error = FilesCommonResource.ErrorMassage_FileNotFound;
            }
            else if (toFolder.FolderType == FolderType.VirtualRooms || toFolder.RootFolderType == FolderType.Archive)
            {
                Error = FilesCommonResource.ErrorMassage_SecurityException_MoveFile;
            }
            else if (!await FilesSecurity.CanReadAsync(file))
            {
                Error = FilesCommonResource.ErrorMassage_SecurityException_ReadFile;
            }
            else if (!await FilesSecurity.CanDownloadAsync(file))
            {
                Error = FilesCommonResource.ErrorMassage_SecurityException;
            }
            else if (file.RootFolderType == FolderType.Privacy
                && (copy || toFolder.RootFolderType != FolderType.Privacy))
            {
                Error = FilesCommonResource.ErrorMassage_SecurityException_MoveFile;
            }
            else if (global.EnableUploadFilter
                     && !fileUtility.ExtsUploadable.Contains(FileUtility.GetFileExtension(file.Title)))
            {
                Error = FilesCommonResource.ErrorMassage_NotSupportedFormat;
            }
            else
            {
                var parentFolder = await FolderDao.GetFolderAsync(file.ParentId);
                try
                {
                    var conflict = _resolveType == FileConflictResolveType.Duplicate
                        || file.RootFolderType == FolderType.Privacy
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
                                filesMessageService.Send(newFile, toFolder, _headers, MessageAction.FileCopied, newFile.Title, parentFolder.Title, toFolder.Title);

                                if (Equals(newFile.ParentId.ToString(), _daoFolderId))
                                {
                                    needToMark.Add(newFile);
                                }

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
                                Error = message;
                            }
                            else
                            {
                                await fileMarker.RemoveMarkAsNewForAllAsync(file);

                                var newFileId = await FileDao.MoveFileAsync(file.Id, toFolderId);
                                newFile = await fileDao.GetFileAsync(newFileId);

                                filesMessageService.Send(file, toFolder, _headers, MessageAction.FileMoved, file.Title, parentFolder.Title, toFolder.Title);

                                if (file.RootFolderType == FolderType.TRASH && newFile.ThumbnailStatus == Thumbnail.NotRequired)
                                {
                                    newFile.ThumbnailStatus = Thumbnail.Waiting;
                                    foreach (var size in _thumbnailSettings.Sizes)
                                    {
                                        await fileDao.SaveThumbnailAsync(newFile, null, size.Width, size.Height);
                                    }
                                }

                                if (newFile.ProviderEntry)
                                {
                                    await LinkDao.DeleteAllLinkAsync(file.Id.ToString());
                                }

                                if (Equals(toFolderId.ToString(), _daoFolderId))
                                {
                                    needToMark.Add(newFile);
                                }

                                socketManager.DeleteFile(file);

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
                            if (!await FilesSecurity.CanEditAsync(conflict))
                            {
                                Error = FilesCommonResource.ErrorMassage_SecurityException;
                            }
                            else if (await entryManager.FileLockedForMeAsync(conflict.Id))
                            {
                                Error = FilesCommonResource.ErrorMassage_LockedFile;
                            }
                            else if (fileTracker.IsEditing(conflict.Id))
                            {
                                Error = FilesCommonResource.ErrorMassage_SecurityException_UpdateEditingFile;
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
                                newFile.ThumbnailStatus = Thumbnail.Waiting;

                                using (var stream = await FileDao.GetFileStreamAsync(file))
                                {
                                    newFile.ContentLength = stream.CanSeek ? stream.Length : file.ContentLength;

                                    newFile = await fileDao.SaveFileAsync(newFile, stream);
                                }

                                if (file.ThumbnailStatus == Thumbnail.Created)
                                {
                                    foreach (var size in _thumbnailSettings.Sizes)
                                    {
                                        using (var thumbnail = await FileDao.GetThumbnailAsync(file, size.Width, size.Height))
                                        {
                                            await fileDao.SaveThumbnailAsync(newFile, thumbnail, size.Width, size.Height);
                                        }
                                    }

                                    newFile.ThumbnailStatus = Thumbnail.Created;
                                }

                                await LinkDao.DeleteAllLinkAsync(newFile.Id.ToString());

                                needToMark.Add(newFile);

                                await socketManager.CreateFileAsync(newFile);

                                if (copy)
                                {
                                    filesMessageService.Send(newFile, toFolder, _headers, MessageAction.FileCopiedWithOverwriting, newFile.Title, parentFolder.Title, toFolder.Title);
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
                                            Error = message;
                                        }
                                        else
                                        {
                                            await FileDao.DeleteFileAsync(file.Id);

                                            await LinkDao.DeleteAllLinkAsync(file.Id.ToString());

                                            filesMessageService.Send(file, toFolder, _headers, MessageAction.FileMovedWithOverwriting, file.Title, parentFolder.Title, toFolder.Title);

                                            socketManager.DeleteFile(file);

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
                    Error = ex.Message;

                    Logger.ErrorWithException(ex);
                }
            }

            ProgressStep(fileId: FolderDao.CanCalculateSubitems(fileId) ? default : fileId);
        }

        Result = sb.ToString();

        return needToMark;
    }

    private async Task<(bool isError, string message)> WithErrorAsync(IServiceScope scope, IEnumerable<File<T>> files)
    {
        var entryManager = scope.ServiceProvider.GetService<EntryManager>();
        var fileTracker = scope.ServiceProvider.GetService<FileTrackerHelper>();
        string error = null;
        foreach (var file in files)
        {
            if (!await FilesSecurity.CanDeleteAsync(file))
            {
                error = FilesCommonResource.ErrorMassage_SecurityException_MoveFile;

                return (true, error);
            }
            if (await entryManager.FileLockedForMeAsync(file.Id))
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
