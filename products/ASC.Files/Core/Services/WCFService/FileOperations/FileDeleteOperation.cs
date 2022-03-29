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

    public FileDeleteOperationData(IEnumerable<object> folders, IEnumerable<object> files, Tenant tenant,
        bool holdResult = true, bool ignoreException = false, bool immediately = false, IDictionary<string, StringValues> headers = null)
        : this(folders.OfType<T>(), files.OfType<T>(), tenant, holdResult, ignoreException, immediately, headers)
    {
    }

    public FileDeleteOperationData(IEnumerable<T> folders, IEnumerable<T> files, Tenant tenant,
        bool holdResult = true, bool ignoreException = false, bool immediately = false, IDictionary<string, StringValues> headers = null)
        : base(folders, files, tenant, holdResult)
    {
        IgnoreException = ignoreException;
        Immediately = immediately;
        Headers = headers;
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
    private readonly IDictionary<string, StringValues> _headers;

    public override FileOperationType OperationType => FileOperationType.Delete;


    public FileDeleteOperation(IServiceProvider serviceProvider, FileDeleteOperationData<T> fileOperationData)
        : base(serviceProvider, fileOperationData)
    {
        _ignoreException = fileOperationData.IgnoreException;
        _immediately = fileOperationData.Immediately;
        _headers = fileOperationData.Headers;
    }

    protected override async Task DoAsync(IServiceScope scope)
    {
        var folderDao = scope.ServiceProvider.GetService<IFolderDao<int>>();
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

        await DeleteFilesAsync(Files, scope);
        await DeleteFoldersAsync(Folders, scope);
    }

    private async Task DeleteFoldersAsync(IEnumerable<T> folderIds, IServiceScope scope)
    {
        var scopeClass = scope.ServiceProvider.GetService<FileDeleteOperationScope>();
        var (fileMarker, filesMessageService) = scopeClass;
        foreach (var folderId in folderIds)
        {
            CancellationToken.ThrowIfCancellationRequested();

            var folder = await FolderDao.GetFolderAsync(folderId);
            T canCalculate = default;
            if (folder == null)
            {
                Error = FilesCommonResource.ErrorMassage_FolderNotFound;
            }
            else if (folder.FolderType != FolderType.DEFAULT && folder.FolderType != FolderType.BUNCH)
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
                        await ProviderDao.RemoveProviderInfoAsync(folder.ProviderId);
                        filesMessageService.Send(folder, _headers, MessageAction.ThirdPartyDeleted, folder.Id.ToString(), folder.ProviderKey);
                    }

                    ProcessedFolder(folderId);
                }
                else
                {
                    var immediately = _immediately || !FolderDao.UseTrashForRemove(folder);
                    if (immediately && FolderDao.UseRecursiveOperation(folder.Id, default(T)))
                    {
                        var files = await FileDao.GetFilesAsync(folder.Id);
                        await DeleteFilesAsync(files, scope);

                        var folders = await FolderDao.GetFoldersAsync(folder.Id).ToListAsync();
                        await DeleteFoldersAsync(folders.Select(f => f.Id).ToList(), scope);

                        if (await FolderDao.IsEmptyAsync(folder.Id))
                        {
                            await FolderDao.DeleteFolderAsync(folder.Id);
                            filesMessageService.Send(folder, _headers, MessageAction.FolderDeleted, folder.Title);

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
                                await FolderDao.DeleteFolderAsync(folder.Id);
                                filesMessageService.Send(folder, _headers, MessageAction.FolderDeleted, folder.Title);
                            }
                            else
                            {
                                await FolderDao.MoveFolderAsync(folder.Id, _trashId, CancellationToken);
                                filesMessageService.Send(folder, _headers, MessageAction.FolderMovedToTrash, folder.Title);
                            }

                            ProcessedFolder(folderId);
                        }
                    }
                }
            }
            ProgressStep(canCalculate);
        }
    }

    private async Task DeleteFilesAsync(IEnumerable<T> fileIds, IServiceScope scope)
    {
        var scopeClass = scope.ServiceProvider.GetService<FileDeleteOperationScope>();
        var socketManager = scope.ServiceProvider.GetService<SocketManager>();

        var (fileMarker, filesMessageService) = scopeClass;
        foreach (var fileId in fileIds)
        {
            CancellationToken.ThrowIfCancellationRequested();

            var file = await FileDao.GetFileAsync(fileId);
            var tmp = WithErrorAsync(scope, new[] { file }, false);
            if (file == null)
            {
                Error = FilesCommonResource.ErrorMassage_FileNotFound;
            }
            else if (!_ignoreException && (await tmp).isError)
            {
                Error = (await tmp).message;
            }
            else
            {
                await fileMarker.RemoveMarkAsNewForAllAsync(file);
                if (!_immediately && FileDao.UseTrashForRemove(file))
                {
                    await FileDao.MoveFileAsync(file.Id, _trashId);
                    filesMessageService.Send(file, _headers, MessageAction.FileMovedToTrash, file.Title);

                    if (file.ThumbnailStatus == Thumbnail.Waiting)
                    {
                        file.ThumbnailStatus = Thumbnail.NotRequired;
                        await FileDao.SaveThumbnailAsync(file, null);
                    }

                    socketManager.DeleteFile(file);
                }
                else
                {
                    try
                    {
                        await FileDao.DeleteFileAsync(file.Id);
                        filesMessageService.Send(file, _headers, MessageAction.FileDeleted, file.Title);

                        socketManager.DeleteFile(file);
                    }
                    catch (Exception ex)
                    {
                        Error = ex.Message;
                        Logger.Error(Error, ex);
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

    public FileDeleteOperationScope(FileMarker fileMarker, FilesMessageService filesMessageService)
    {
        _fileMarker = fileMarker;
        _filesMessageService = filesMessageService;
    }

    public void Deconstruct(out FileMarker fileMarker, out FilesMessageService filesMessageService)
    {
        fileMarker = _fileMarker;
        filesMessageService = _filesMessageService;
    }
}
