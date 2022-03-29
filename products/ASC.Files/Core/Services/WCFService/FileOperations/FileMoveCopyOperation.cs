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
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

using ASC.Common;
using ASC.Core.Tenants;
using ASC.Files.Core;
using ASC.Files.Core.Resources;
using ASC.MessagingSystem;
using ASC.Web.Core.Files;
using ASC.Web.Files.Classes;
using ASC.Web.Files.Helpers;
using ASC.Web.Files.Utils;

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Primitives;

namespace ASC.Web.Files.Services.WCFService.FileOperations
{
    [Transient]
    class FileMoveCopyOperation : ComposeFileOperation<FileMoveCopyOperationData<string>, FileMoveCopyOperationData<int>>
    {
        public FileMoveCopyOperation(IServiceProvider serviceProvider,
            FileOperation<FileMoveCopyOperationData<string>, string> thirdPartyOperation,
            FileOperation<FileMoveCopyOperationData<int>, int> daoOperation)
            : base(serviceProvider, thirdPartyOperation, daoOperation)
        {

        }

        public override FileOperationType OperationType
        {
            get
            {
                return ThirdPartyOperation.OperationType;
            }
        }
    }

    internal class FileMoveCopyOperationData<T> : FileOperationData<T>
    {
        public string ThirdpartyFolderId { get; }
        public int DaoFolderId { get; }
        public bool Copy { get; }
        public FileConflictResolveType ResolveType { get; }
        public IDictionary<string, StringValues> Headers { get; }

        public FileMoveCopyOperationData(IEnumerable<object> folders, IEnumerable<object> files, Tenant tenant, JsonElement toFolderId, bool copy, FileConflictResolveType resolveType, bool holdResult = true, IDictionary<string, StringValues> headers = null)
            : this(folders.OfType<T>(), files.OfType<T>(), tenant, toFolderId, copy, resolveType, holdResult, headers)
        {
        }

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
        private readonly int DaoFolderId;
        private readonly string ThirdpartyFolderId;
        private readonly bool _copy;
        private readonly FileConflictResolveType _resolveType;

        private readonly IDictionary<string, StringValues> _headers;

        public override FileOperationType OperationType
        {
            get { return _copy ? FileOperationType.Copy : FileOperationType.Move; }
        }

        public FileMoveCopyOperation(IServiceProvider serviceProvider, FileMoveCopyOperationData<T> data)
            : base(serviceProvider, data)
        {
            DaoFolderId = data.DaoFolderId;
            ThirdpartyFolderId = data.ThirdpartyFolderId;
            _copy = data.Copy;
            _resolveType = data.ResolveType;

            _headers = data.Headers;
        }

        protected override async Task DoAsync(IServiceScope scope)
        {
            if (DaoFolderId != 0)
            {
                await DoAsync(scope, DaoFolderId);
            }

            if (!string.IsNullOrEmpty(ThirdpartyFolderId))
            {
                await DoAsync(scope, ThirdpartyFolderId);
            }
        }

        private async Task DoAsync<TTo>(IServiceScope scope, TTo tto)
        {
            var fileMarker = scope.ServiceProvider.GetService<FileMarker>();
            var folderDao = scope.ServiceProvider.GetService<IFolderDao<TTo>>();

            Result += string.Format("folder_{0}{1}", DaoFolderId, SPLIT_CHAR);

            //TODO: check on each iteration?
            var toFolder = await folderDao.GetFolderAsync(tto);
            if (toFolder == null) return;
            if (!await FilesSecurity.CanCreateAsync(toFolder)) throw new System.Security.SecurityException(FilesCommonResource.ErrorMassage_SecurityException_Create);

            var parentFolders = await folderDao.GetParentFoldersAsync(toFolder.ID);
            if (parentFolders.Any(parent => Folders.Any(r => r.ToString() == parent.ID.ToString())))
            {
                Error = FilesCommonResource.ErrorMassage_FolderCopyError;
                return;
            }

            if (_copy)
            {
                Folder<T> rootFrom = null;
                if (0 < Folders.Count) rootFrom = await FolderDao.GetRootFolderAsync(Folders[0]);
                if (0 < Files.Count) rootFrom = await FolderDao.GetRootFolderByFileAsync(Files[0]);
                if (rootFrom != null && rootFrom.FolderType == FolderType.TRASH) throw new InvalidOperationException("Can not copy from Trash.");
                if (toFolder.RootFolderType == FolderType.TRASH) throw new InvalidOperationException("Can not copy to Trash.");
            }

            var needToMark = new List<FileEntry<TTo>>();

            var moveOrCopyFoldersTask = MoveOrCopyFoldersAsync(scope, Folders, toFolder, _copy);
            var moveOrCopyFilesTask = MoveOrCopyFilesAsync(scope, Files, toFolder, _copy);

            needToMark.AddRange(await moveOrCopyFoldersTask);
            needToMark.AddRange(await moveOrCopyFilesTask);

            var ntm = needToMark.Distinct(); foreach (var n in ntm)
            {
                await fileMarker.MarkAsNewAsync(n);
            }
        }

        private async Task<List<FileEntry<TTo>>> MoveOrCopyFoldersAsync<TTo>(IServiceScope scope, List<T> folderIds, Folder<TTo> toFolder, bool copy)
        {
            var needToMark = new List<FileEntry<TTo>>();

            if (folderIds.Count == 0) return needToMark;

            var scopeClass = scope.ServiceProvider.GetService<FileMoveCopyOperationScope>();
            var (filesMessageService, fileMarker, _, _, _) = scopeClass;
            var folderDao = scope.ServiceProvider.GetService<IFolderDao<TTo>>();

            var toFolderId = toFolder.ID;
            var isToFolder = Equals(toFolderId, DaoFolderId);

            var sb = new StringBuilder();
            sb.Append(Result);
            foreach (var folderId in folderIds)
            {
                CancellationToken.ThrowIfCancellationRequested();

                var folder = await FolderDao.GetFolderAsync(folderId);
                var taskError = WithErrorAsync(scope, await FileDao.GetFilesAsync(folder.ID, new OrderBy(SortedByType.AZ, true), FilterType.FilesOnly, false, Guid.Empty, string.Empty, false, true).ToListAsync());

                if (folder == null)
                {
                    Error = FilesCommonResource.ErrorMassage_FolderNotFound;
                }
                else if (!await FilesSecurity.CanReadAsync(folder))
                {
                    Error = FilesCommonResource.ErrorMassage_SecurityException_ReadFolder;
                }
                else if (folder.RootFolderType == FolderType.Privacy
                    && (copy || toFolder.RootFolderType != FolderType.Privacy))
                {
                    Error = FilesCommonResource.ErrorMassage_SecurityException_MoveFolder;
                }
                else if (!Equals(folder.FolderID ?? default, toFolderId) || _resolveType == FileConflictResolveType.Duplicate)
                {
                    try
                    {
                        //if destination folder contains folder with same name then merge folders
                        var conflictFolder = folder.RootFolderType == FolderType.Privacy
                            ? null
                            : await folderDao.GetFolderAsync(folder.Title, toFolderId);
                        Folder<TTo> newFolder;

                        if (copy || conflictFolder != null)
                        {
                            if (conflictFolder != null)
                            {
                                newFolder = conflictFolder;

                                if (isToFolder)
                                    needToMark.Add(conflictFolder);
                            }
                            else
                            {
                                newFolder = await FolderDao.CopyFolderAsync(folder.ID, toFolderId, CancellationToken);
                                filesMessageService.Send(newFolder, toFolder, _headers, MessageAction.FolderCopied, newFolder.Title, toFolder.Title);

                                if (isToFolder)
                                    needToMark.Add(newFolder);

                                if (ProcessedFolder(folderId))
                                {
                                    sb.Append($"folder_{newFolder.ID}{SPLIT_CHAR}");
                                }
                            }

                            if (toFolder.ProviderId == folder.ProviderId // crossDao operation is always recursive
                                && FolderDao.UseRecursiveOperation(folder.ID, toFolderId))
                            {
                                await MoveOrCopyFilesAsync(scope, await FileDao.GetFilesAsync(folder.ID), newFolder, copy);
                                await MoveOrCopyFoldersAsync(scope, await FolderDao.GetFoldersAsync(folder.ID).Select(f => f.ID).ToListAsync(), newFolder, copy);

                                if (!copy)
                                {
                                    if (!await FilesSecurity.CanDeleteAsync(folder))
                                    {
                                        Error = FilesCommonResource.ErrorMassage_SecurityException_MoveFolder;
                                    }
                                    else if (await FolderDao.IsEmptyAsync(folder.ID))
                                    {
                                        await FolderDao.DeleteFolderAsync(folder.ID);

                                        if (ProcessedFolder(folderId))
                                        {
                                            sb.Append($"folder_{newFolder.ID}{SPLIT_CHAR}");
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
                                        newFolder = await FolderDao.CopyFolderAsync(folder.ID, toFolderId, CancellationToken);
                                        newFolderId = newFolder.ID;
                                        filesMessageService.Send(newFolder, toFolder, _headers, MessageAction.FolderCopiedWithOverwriting, newFolder.Title, toFolder.Title);

                                        if (isToFolder)
                                            needToMark.Add(newFolder);

                                        if (ProcessedFolder(folderId))
                                        {
                                            sb.Append($"folder_{newFolderId}{SPLIT_CHAR}");
                                        }
                                    }
                                    else if (!await FilesSecurity.CanDeleteAsync(folder))
                                    {
                                        Error = FilesCommonResource.ErrorMassage_SecurityException_MoveFolder;
                                    }
                                    else if ((await taskError).isError)
                                    {
                                        Error = (await taskError).message;
                                    }
                                    else
                                    {
                                        await fileMarker.RemoveMarkAsNewForAllAsync(folder);

                                        newFolderId = await FolderDao.MoveFolderAsync(folder.ID, toFolderId, CancellationToken);
                                        newFolder = await folderDao.GetFolderAsync(newFolderId);

                                        if (folder.RootFolderType != FolderType.USER)
                                        {
                                            filesMessageService.Send(folder, toFolder, _headers, MessageAction.FolderMovedWithOverwriting, folder.Title, toFolder.Title);
                                        }
                                        else
                                        {
                                            filesMessageService.Send(newFolder, toFolder, _headers, MessageAction.FolderMovedWithOverwriting, folder.Title, toFolder.Title);
                                        }

                                        if (isToFolder)
                                            needToMark.Add(newFolder);

                                        if (ProcessedFolder(folderId))
                                        {
                                            sb.Append($"folder_{newFolderId}{SPLIT_CHAR}");
                                        }
                                    }
                                }
                            }
                        }
                        else
                        {
                            if (!await FilesSecurity.CanDeleteAsync(folder))
                            {
                                Error = FilesCommonResource.ErrorMassage_SecurityException_MoveFolder;
                            }
                            else if ((await taskError).isError)
                            {
                                Error = (await taskError).message;
                            }
                            else
                            {
                                await fileMarker.RemoveMarkAsNewForAllAsync(folder);

                                var newFolderId = await FolderDao.MoveFolderAsync(folder.ID, toFolderId, CancellationToken);
                                newFolder = await folderDao.GetFolderAsync(newFolderId);

                                if (folder.RootFolderType != FolderType.USER)
                                {
                                    filesMessageService.Send(folder, toFolder, _headers, MessageAction.FolderMoved, folder.Title, toFolder.Title);
                                }
                                else
                                {
                                    filesMessageService.Send(newFolder, toFolder, _headers, MessageAction.FolderMoved, folder.Title, toFolder.Title);
                                }

                                if (isToFolder)
                                    needToMark.Add(newFolder);

                                if (ProcessedFolder(folderId))
                                {
                                    sb.Append($"folder_{newFolderId}{SPLIT_CHAR}");
                                }
                            }
                        }
                        Result = sb.ToString();
                    }
                    catch (Exception ex)
                    {
                        Error = ex.Message;

                        Logger.Error(Error, ex);
                    }
                }
                ProgressStep(FolderDao.CanCalculateSubitems(folderId) ? default : folderId);
            }

            return needToMark;
        }

        private async Task<List<FileEntry<TTo>>> MoveOrCopyFilesAsync<TTo>(IServiceScope scope, List<T> fileIds, Folder<TTo> toFolder, bool copy)
        {
            var needToMark = new List<FileEntry<TTo>>();

            if (fileIds.Count == 0) return needToMark;

            var scopeClass = scope.ServiceProvider.GetService<FileMoveCopyOperationScope>();
            var (filesMessageService, fileMarker, fileUtility, global, entryManager) = scopeClass;
            var fileDao = scope.ServiceProvider.GetService<IFileDao<TTo>>();
            var fileTracker = scope.ServiceProvider.GetService<FileTrackerHelper>();
            var socketManager = scope.ServiceProvider.GetService<SocketManager>();

            var toFolderId = toFolder.ID;
            var sb = new StringBuilder();
            foreach (var fileId in fileIds)
            {
                CancellationToken.ThrowIfCancellationRequested();

                var file = await FileDao.GetFileAsync(fileId);
                var taskError = WithErrorAsync(scope, new[] { file });

                if (file == null)
                {
                    Error = FilesCommonResource.ErrorMassage_FileNotFound;
                }
                else if (!await FilesSecurity.CanReadAsync(file))
                {
                    Error = FilesCommonResource.ErrorMassage_SecurityException_ReadFile;
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
                    var parentFolder = await FolderDao.GetFolderAsync(file.FolderID);
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
                                    newFile = await FileDao.CopyFileAsync(file.ID, toFolderId); //Stream copy will occur inside dao
                                    filesMessageService.Send(newFile, toFolder, _headers, MessageAction.FileCopied, newFile.Title, parentFolder.Title, toFolder.Title);

                                    if (Equals(newFile.FolderID.ToString(), DaoFolderId))
                                    {
                                        needToMark.Add(newFile);
                                    }

                                    await socketManager.CreateFileAsync(newFile);

                                    if (ProcessedFile(fileId))
                                    {
                                        sb.Append($"file_{newFile.ID}{SPLIT_CHAR}");
                                    }
                                }
                                catch
                                {
                                    if (newFile != null)
                                    {
                                        await fileDao.DeleteFileAsync(newFile.ID);
                                    }
                                    throw;
                                }
                            }
                            else
                            {
                                if ((await taskError).isError)
                                {
                                    Error = (await taskError).message;
                                }
                                else
                                {
                                    await fileMarker.RemoveMarkAsNewForAllAsync(file);

                                    var newFileId = await FileDao.MoveFileAsync(file.ID, toFolderId);
                                    newFile = await fileDao.GetFileAsync(newFileId);

                                    if (file.RootFolderType != FolderType.USER)
                                    {
                                        filesMessageService.Send(file, toFolder, _headers, MessageAction.FileMoved, file.Title, parentFolder.Title, toFolder.Title);
                                    }
                                    else
                                    {
                                        filesMessageService.Send(newFile, toFolder, _headers, MessageAction.FileMoved, file.Title, parentFolder.Title, toFolder.Title);
                                    }

                                    if (file.RootFolderType == FolderType.TRASH && newFile.ThumbnailStatus == Thumbnail.NotRequired)
                                    {
                                        newFile.ThumbnailStatus = Thumbnail.Waiting;
                                        await fileDao.SaveThumbnailAsync(newFile, null);
                                    }

                                    if (newFile.ProviderEntry)
                                    {
                                        await LinkDao.DeleteAllLinkAsync(file.ID.ToString());
                                    }

                                    if (Equals(toFolderId.ToString(), DaoFolderId))
                                    {
                                        needToMark.Add(newFile);
                                    }

                                    socketManager.DeleteFile(file);

                                    await socketManager.CreateFileAsync(newFile);

                                    if (ProcessedFile(fileId))
                                    {
                                        sb.Append($"file_{newFileId}{SPLIT_CHAR}");
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
                                else if (await entryManager.FileLockedForMeAsync(conflict.ID))
                                {
                                    Error = FilesCommonResource.ErrorMassage_LockedFile;
                                }
                                else if (fileTracker.IsEditing(conflict.ID))
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
                                        using (var thumbnail = await FileDao.GetThumbnailAsync(file))
                                        {
                                            await fileDao.SaveThumbnailAsync(newFile, thumbnail);
                                        }
                                        newFile.ThumbnailStatus = Thumbnail.Created;
                                    }

                                    await LinkDao.DeleteAllLinkAsync(newFile.ID.ToString());

                                    needToMark.Add(newFile);

                                    await socketManager.CreateFileAsync(newFile);

                                    if (copy)
                                    {
                                        filesMessageService.Send(newFile, toFolder, _headers, MessageAction.FileCopiedWithOverwriting, newFile.Title, parentFolder.Title, toFolder.Title);
                                        if (ProcessedFile(fileId))
                                        {
                                            sb.Append($"file_{newFile.ID}{SPLIT_CHAR}");
                                        }
                                    }
                                    else
                                    {
                                        if (Equals(file.FolderID.ToString(), toFolderId.ToString()))
                                        {
                                            if (ProcessedFile(fileId))
                                            {
                                                sb.Append($"file_{newFile.ID}{SPLIT_CHAR}");
                                            }
                                        }
                                        else
                                        {
                                            if ((await taskError).isError)
                                            {
                                                Error = (await taskError).message;
                                            }
                                            else
                                            {
                                                await FileDao.DeleteFileAsync(file.ID);

                                                await LinkDao.DeleteAllLinkAsync(file.ID.ToString());

                                                if (file.RootFolderType != FolderType.USER)
                                                {
                                                    filesMessageService.Send(file, toFolder, _headers, MessageAction.FileMovedWithOverwriting, file.Title, parentFolder.Title, toFolder.Title);
                                                }
                                                else
                                                {
                                                    filesMessageService.Send(newFile, toFolder, _headers, MessageAction.FileMovedWithOverwriting, file.Title, parentFolder.Title, toFolder.Title);
                                                }

                                                socketManager.DeleteFile(file);

                                                if (ProcessedFile(fileId))
                                                {
                                                    sb.Append($"file_{newFile.ID}{SPLIT_CHAR}");
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
                        Logger.Error(Error, ex);
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
                if (await entryManager.FileLockedForMeAsync(file.ID))
                {
                    error = FilesCommonResource.ErrorMassage_LockedFile;
                    return (true, error);
                }
                if (fileTracker.IsEditing(file.ID))
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
        private FilesMessageService FilesMessageService { get; }
        private FileMarker FileMarker { get; }
        private FileUtility FileUtility { get; }
        private Global Global { get; }
        private EntryManager EntryManager { get; }

        public FileMoveCopyOperationScope(FilesMessageService filesMessageService, FileMarker fileMarker, FileUtility fileUtility, Global global, EntryManager entryManager)
        {
            FilesMessageService = filesMessageService;
            FileMarker = fileMarker;
            FileUtility = fileUtility;
            Global = global;
            EntryManager = entryManager;
        }

        public void Deconstruct(out FilesMessageService filesMessageService, out FileMarker fileMarker, out FileUtility fileUtility, out Global global, out EntryManager entryManager)
        {
            filesMessageService = FilesMessageService;
            fileMarker = FileMarker;
            fileUtility = FileUtility;
            global = Global;
            entryManager = EntryManager;
        }
    }
}