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

internal class FileDeleteOperationData<T> : FileOperationData<T>
{
    public bool IgnoreException { get; }
    public bool Immediately { get; }
    public IDictionary<string, StringValues> Headers { get; }
    public bool IsEmptyTrash { get; }

    public FileDeleteOperationData(IEnumerable<T> folders, IEnumerable<T> files, Tenant tenant,
        bool holdResult = true, bool ignoreException = false, bool immediately = false, IDictionary<string, StringValues> headers = null, bool isEmptyTrash = false)
        : base(folders, files, tenant, holdResult)
    {
        IgnoreException = ignoreException;
        Immediately = immediately;
        Headers = headers;
        IsEmptyTrash = isEmptyTrash;
    }
}

[Transient]
class FileDeleteOperation : ComposeFileOperation<FileDeleteOperationData<string>, FileDeleteOperationData<int>>
{
    public FileDeleteOperation(IServiceProvider serviceProvider, FileOperation<FileDeleteOperationData<string>, string> f1, FileOperation<FileDeleteOperationData<int>, int> f2)
        : base(serviceProvider, f1, f2)
    {
    }

    public override FileOperationType OperationType => FileOperationType.Delete;
}

class FileDeleteOperation<T> : FileOperation<FileDeleteOperationData<T>, T>
{
    private int _trashId;
    private readonly bool _ignoreException;
    private readonly bool _immediately;
    private readonly bool _isEmptyTrash;
    private readonly IDictionary<string, StringValues> _headers;
    private readonly ThumbnailSettings _thumbnailSettings;

    public override FileOperationType OperationType => FileOperationType.Delete;


    public FileDeleteOperation(IServiceProvider serviceProvider, FileDeleteOperationData<T> fileOperationData, ThumbnailSettings thumbnailSettings)
    : base(serviceProvider, fileOperationData)
    {
        _ignoreException = fileOperationData.IgnoreException;
        _immediately = fileOperationData.Immediately;
        _headers = fileOperationData.Headers;
        _isEmptyTrash = fileOperationData.IsEmptyTrash;
        _thumbnailSettings = thumbnailSettings;
    }

    protected override async Task DoAsync(IServiceScope scope)
    {
        var folderDao = scope.ServiceProvider.GetService<IFolderDao<int>>();
        var messageService = scope.ServiceProvider.GetService<MessageService>();
        _trashId = await folderDao.GetFolderIDTrashAsync(true);

        Folder<T> root = null;
        if (0 < Folders.Count)
        {
            root = await FolderDao.GetRootFolderAsync(Folders[0]);
        }
        else if (0 < Files.Count)
        {
            root = await FolderDao.GetRootFolderByFileAsync(Files[0]);
        }
        if (root != null)
        {
            Result += string.Format("folder_{0}{1}", root.Id, SplitChar);
        }
        if (_isEmptyTrash)
        {
            await DeleteFilesAsync(Files, scope);
            await DeleteFoldersAsync(Folders, scope);
            messageService.Send(_headers, MessageAction.TrashEmptied);
        }
        else
        {
            await DeleteFilesAsync(Files, scope, true);
            await DeleteFoldersAsync(Folders, scope, true);
        }

    }

    private async Task DeleteFoldersAsync(IEnumerable<T> folderIds, IServiceScope scope, bool isNeedSendActions = false)
    {
        var scopeClass = scope.ServiceProvider.GetService<FileDeleteOperationScope>();
        var (fileMarker, filesMessageService, roomLogoManager) = scopeClass;
        foreach (var folderId in folderIds)
        {
            CancellationToken.ThrowIfCancellationRequested();

            var folder = await FolderDao.GetFolderAsync(folderId);
            var isRoom = DocSpaceHelper.IsRoom(folder.FolderType);

            T canCalculate = default;
            if (folder == null)
            {
                Error = FilesCommonResource.ErrorMassage_FolderNotFound;
            }
            else if (folder.FolderType != FolderType.DEFAULT && folder.FolderType != FolderType.BUNCH
                && !DocSpaceHelper.IsRoom(folder.FolderType))
            {
                Error = FilesCommonResource.ErrorMassage_SecurityException_DeleteFolder;
            }
            else if (!_ignoreException && !await FilesSecurity.CanDeleteAsync(folder))
            {
                canCalculate = FolderDao.CanCalculateSubitems(folderId) ? default : folderId;

                Error = FilesCommonResource.ErrorMassage_SecurityException_DeleteFolder;
            }
            else
            {
                canCalculate = FolderDao.CanCalculateSubitems(folderId) ? default : folderId;

                await fileMarker.RemoveMarkAsNewForAllAsync(folder);
                if (folder.ProviderEntry && folder.Id.Equals(folder.RootId))
                {
                    if (ProviderDao != null)
                    {
                        if (folder.RootFolderType == FolderType.VirtualRooms || folder.RootFolderType == FolderType.Archive)
                        {
                            var providerInfo = await ProviderDao.GetProviderInfoAsync(folder.ProviderId);

                            if (providerInfo.FolderId != null)
                            {
                                await roomLogoManager.DeleteAsync(providerInfo.FolderId);
                            }
                        }

                        await ProviderDao.RemoveProviderInfoAsync(folder.ProviderId);
                        if (isNeedSendActions)
                        {
                            filesMessageService.Send(folder, _headers, MessageAction.ThirdPartyDeleted, folder.Id.ToString(), folder.ProviderKey);
                        }
                    }

                    ProcessedFolder(folderId);
                }
                else
                {
                    var immediately = _immediately || FolderDao.UseTrashForRemoveAsync(folder);
                    if (immediately && FolderDao.UseRecursiveOperation(folder.Id, default(T)))
                    {
                        var files = await FileDao.GetFilesAsync(folder.Id).ToListAsync();
                        await DeleteFilesAsync(files, scope);

                        var folders = await FolderDao.GetFoldersAsync(folder.Id).ToListAsync();
                        await DeleteFoldersAsync(folders.Select(f => f.Id).ToList(), scope);

                        if (await FolderDao.IsEmptyAsync(folder.Id))
                        {
                            if (isRoom)
                            {
                                await roomLogoManager.DeleteAsync(folder.Id);
                            }

                            await FolderDao.DeleteFolderAsync(folder.Id);

                            if (isRoom && folder.ProviderEntry)
                            {
                                await ProviderDao.RemoveProviderInfoAsync(folder.ProviderId);
                            }

                            filesMessageService.Send(folder, _headers, isRoom ? MessageAction.RoomDeleted : MessageAction.FolderDeleted, folder.Title);

                            ProcessedFolder(folderId);
                        }
                    }
                    else
                    {
                        var files = await FileDao.GetFilesAsync(folder.Id, new OrderBy(SortedByType.AZ, true), FilterType.FilesOnly, false, Guid.Empty, string.Empty, false, true).ToListAsync();
                        var (isError, message) = await WithErrorAsync(scope, files, true);
                        if (!_ignoreException && isError)
                        {
                            Error = message;
                        }
                        else
                        {
                            if (immediately)
                            {
                                if (isRoom)
                                {
                                    await roomLogoManager.DeleteAsync(folder.Id);
                                }

                                await FolderDao.DeleteFolderAsync(folder.Id);

                                if (isRoom && folder.ProviderEntry)
                                {
                                    await ProviderDao.RemoveProviderInfoAsync(folder.ProviderId);
                                }

                                if (isNeedSendActions)
                                {
                                    filesMessageService.Send(folder, _headers, isRoom ? MessageAction.RoomDeleted : MessageAction.FolderDeleted, folder.Title);
                                }
                            }
                            else
                            {
                                await FolderDao.MoveFolderAsync(folder.Id, _trashId, CancellationToken);
                                if (isNeedSendActions)
                                {
                                    filesMessageService.Send(folder, _headers, MessageAction.FolderMovedToTrash, folder.Title);
                                }
                            }

                            ProcessedFolder(folderId);
                        }
                    }
                }
            }
            ProgressStep(canCalculate);
        }
    }

    private async Task DeleteFilesAsync(IEnumerable<T> fileIds, IServiceScope scope, bool isNeedSendActions = false)
    {
        var scopeClass = scope.ServiceProvider.GetService<FileDeleteOperationScope>();
        var socketManager = scope.ServiceProvider.GetService<SocketManager>();

        var (fileMarker, filesMessageService, _) = scopeClass;
        foreach (var fileId in fileIds)
        {
            CancellationToken.ThrowIfCancellationRequested();

            var file = await FileDao.GetFileAsync(fileId);
            var (isError, message) = await WithErrorAsync(scope, new[] { file }, false);
            if (file == null)
            {
                Error = FilesCommonResource.ErrorMassage_FileNotFound;
            }
            else if (!_ignoreException && isError)
            {
                Error = message;
            }
            else
            {
                await fileMarker.RemoveMarkAsNewForAllAsync(file);
                if (!_immediately && FileDao.UseTrashForRemove(file))
                {
                    await FileDao.MoveFileAsync(file.Id, _trashId);
                    if (isNeedSendActions)
                    {
                        filesMessageService.Send(file, _headers, MessageAction.FileMovedToTrash, file.Title);
                    }

                    if (file.ThumbnailStatus == Thumbnail.Waiting)
                    {
                        file.ThumbnailStatus = Thumbnail.NotRequired;
                        foreach (var size in _thumbnailSettings.Sizes)
                        {
                            await FileDao.SaveThumbnailAsync(file, null, size.Width, size.Height);
                        }
                    }

                    socketManager.DeleteFile(file);
                }
                else
                {
                    try
                    {
                        await FileDao.DeleteFileAsync(file.Id);

                        if (_headers != null)
                        {
                            if (isNeedSendActions)
                            {
                                filesMessageService.Send(file, _headers, MessageAction.FileDeleted, file.Title);
                            }
                        }
                        else
                        {
                            filesMessageService.Send(file, MessageInitiator.AutoCleanUp, MessageAction.FileDeleted, file.Title);
                        }

                        socketManager.DeleteFile(file);
                    }
                    catch (Exception ex)
                    {
                        Error = ex.Message;
                        Logger.ErrorWithException(ex);
                    }

                    await LinkDao.DeleteAllLinkAsync(file.Id.ToString());
                }

                ProcessedFile(fileId);
            }

            ProgressStep(fileId: FolderDao.CanCalculateSubitems(fileId) ? default : fileId);
        }
    }

    private async Task<(bool isError, string message)> WithErrorAsync(IServiceScope scope, IEnumerable<File<T>> files, bool folder)
    {
        var entryManager = scope.ServiceProvider.GetService<EntryManager>();
        var fileTracker = scope.ServiceProvider.GetService<FileTrackerHelper>();

        string error = null;
        foreach (var file in files)
        {
            if (!await FilesSecurity.CanDeleteAsync(file))
            {
                error = FilesCommonResource.ErrorMassage_SecurityException_DeleteFile;

                return (true, error);
            }
            if (await entryManager.FileLockedForMeAsync(file.Id))
            {
                error = FilesCommonResource.ErrorMassage_LockedFile;

                return (true, error);
            }
            if (fileTracker.IsEditing(file.Id))
            {
                error = folder ? FilesCommonResource.ErrorMassage_SecurityException_DeleteEditingFolder : FilesCommonResource.ErrorMassage_SecurityException_DeleteEditingFile;

                return (true, error);
            }
        }

        return (false, error);
    }
}

[Scope]
public class FileDeleteOperationScope
{
    private readonly FileMarker _fileMarker;
    private readonly FilesMessageService _filesMessageService;
    private readonly RoomLogoManager _roomLogoManager;

    public FileDeleteOperationScope(FileMarker fileMarker, FilesMessageService filesMessageService, RoomLogoManager roomLogoManager)
    {
        _fileMarker = fileMarker;
        _filesMessageService = filesMessageService;
        _roomLogoManager = roomLogoManager;
        _roomLogoManager.EnableAudit = false;
    }

    public void Deconstruct(out FileMarker fileMarker, out FilesMessageService filesMessageService, out RoomLogoManager roomLogoManager)
    {
        fileMarker = _fileMarker;
        filesMessageService = _filesMessageService;
        roomLogoManager = _roomLogoManager;
    }
}
