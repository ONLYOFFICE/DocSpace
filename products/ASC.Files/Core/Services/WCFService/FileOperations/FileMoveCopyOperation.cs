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
using System.Text.Json;

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

        protected override void Do(IServiceScope scope)
        {
            if (DaoFolderId != 0)
            {
                Do(scope, DaoFolderId);
            }

            if (!string.IsNullOrEmpty(ThirdpartyFolderId))
            {
                Do(scope, ThirdpartyFolderId);
            }
        }

        private void Do<TTo>(IServiceScope scope, TTo tto)
        {
            if (Folders.Count == 0 && Files.Count == 0) return;

            var fileMarker = scope.ServiceProvider.GetService<FileMarker>();
            var folderDao = scope.ServiceProvider.GetService<IFolderDao<TTo>>();

            Result += string.Format("folder_{0}{1}", DaoFolderId, SPLIT_CHAR);

            //TODO: check on each iteration?
            var toFolder = folderDao.GetFolder(tto);
            if (toFolder == null) return;
            if (!FilesSecurity.CanCreate(toFolder)) throw new System.Security.SecurityException(FilesCommonResource.ErrorMassage_SecurityException_Create);

            if (folderDao.GetParentFolders(toFolder.ID).Any(parent => Folders.Any(r => r.ToString() == parent.ID.ToString())))
            {
                Error = FilesCommonResource.ErrorMassage_FolderCopyError;
                return;
            }

            if (_copy)
            {
                Folder<T> rootFrom = null;
                if (0 < Folders.Count) rootFrom = FolderDao.GetRootFolder(Folders[0]);
                if (0 < Files.Count) rootFrom = FolderDao.GetRootFolderByFile(Files[0]);
                if (rootFrom != null && rootFrom.FolderType == FolderType.TRASH) throw new InvalidOperationException("Can not copy from Trash.");
                if (toFolder.RootFolderType == FolderType.TRASH) throw new InvalidOperationException("Can not copy to Trash.");
            }

            var needToMark = new List<FileEntry<TTo>>();

            needToMark.AddRange(MoveOrCopyFolders(scope, Folders, toFolder, _copy));
            needToMark.AddRange(MoveOrCopyFiles(scope, Files, toFolder, _copy));

            needToMark.Distinct().ToList().ForEach(x => fileMarker.MarkAsNew(x));
        }

        private List<FileEntry<TTo>> MoveOrCopyFolders<TTo>(IServiceScope scope, List<T> folderIds, Folder<TTo> toFolder, bool copy)
        {
            var needToMark = new List<FileEntry<TTo>>();

            if (folderIds.Count == 0) return needToMark;

            var scopeClass = scope.ServiceProvider.GetService<FileMoveCopyOperationScope>();
            var (filesMessageService, fileMarker, _, _, _) = scopeClass;
            var folderDao = scope.ServiceProvider.GetService<IFolderDao<TTo>>();

            var toFolderId = toFolder.ID;
            var isToFolder = Equals(toFolderId, DaoFolderId);


            foreach (var folderId in folderIds)
            {
                CancellationToken.ThrowIfCancellationRequested();

                var folder = FolderDao.GetFolder(folderId);
                if (folder == null)
                {
                    Error = FilesCommonResource.ErrorMassage_FolderNotFound;
                }
                else if (!FilesSecurity.CanRead(folder))
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
                            : folderDao.GetFolder(folder.Title, toFolderId);
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
                                newFolder = FolderDao.CopyFolder(folder.ID, toFolderId, CancellationToken);
                                filesMessageService.Send(newFolder, toFolder, _headers, MessageAction.FolderCopied, newFolder.Title, toFolder.Title);

                                if (isToFolder)
                                    needToMark.Add(newFolder);

                                if (ProcessedFolder(folderId))
                                {
                                    Result += string.Format("folder_{0}{1}", newFolder.ID, SPLIT_CHAR);
                                }
                            }

                            if (FolderDao.UseRecursiveOperation(folder.ID, toFolderId))
                            {
                                MoveOrCopyFiles(scope, FileDao.GetFiles(folder.ID), newFolder, copy);
                                MoveOrCopyFolders(scope, FolderDao.GetFolders(folder.ID).Select(f => f.ID).ToList(), newFolder, copy);

                                if (!copy)
                                {
                                    if (!FilesSecurity.CanDelete(folder))
                                    {
                                        Error = FilesCommonResource.ErrorMassage_SecurityException_MoveFolder;
                                    }
                                    else if (FolderDao.IsEmpty(folder.ID))
                                    {
                                        FolderDao.DeleteFolder(folder.ID);
                                        if (ProcessedFolder(folderId))
                                        {
                                            Result += string.Format("folder_{0}{1}", newFolder.ID, SPLIT_CHAR);
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
                                        newFolder = FolderDao.CopyFolder(folder.ID, toFolderId, CancellationToken);
                                        newFolderId = newFolder.ID;
                                        filesMessageService.Send(newFolder, toFolder, _headers, MessageAction.FolderCopiedWithOverwriting, newFolder.Title, toFolder.Title);

                                        if (isToFolder)
                                            needToMark.Add(newFolder);

                                        if (ProcessedFolder(folderId))
                                        {
                                            Result += string.Format("folder_{0}{1}", newFolderId, SPLIT_CHAR);
                                        }
                                    }
                                    else if (!FilesSecurity.CanDelete(folder))
                                    {
                                        Error = FilesCommonResource.ErrorMassage_SecurityException_MoveFolder;
                                    }
                                    else if (WithError(scope, FileDao.GetFiles(folder.ID, new OrderBy(SortedByType.AZ, true), FilterType.FilesOnly, false, Guid.Empty, string.Empty, false, true), out var tmpError))
                                    {
                                        Error = tmpError;
                                    }
                                    else
                                    {
                                        fileMarker.RemoveMarkAsNewForAll(folder);

                                        newFolderId = FolderDao.MoveFolder(folder.ID, toFolderId, CancellationToken);
                                        newFolder = folderDao.GetFolder(newFolderId);

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
                                            Result += string.Format("folder_{0}{1}", newFolderId, SPLIT_CHAR);
                                        }
                                    }
                                }
                            }
                        }
                        else
                        {
                            if (!FilesSecurity.CanDelete(folder))
                            {
                                Error = FilesCommonResource.ErrorMassage_SecurityException_MoveFolder;
                            }
                            else if (WithError(scope, FileDao.GetFiles(folder.ID, new OrderBy(SortedByType.AZ, true), FilterType.FilesOnly, false, Guid.Empty, string.Empty, false, true), out var tmpError))
                            {
                                Error = tmpError;
                            }
                            else
                            {
                                fileMarker.RemoveMarkAsNewForAll(folder);

                                var newFolderId = FolderDao.MoveFolder(folder.ID, toFolderId, CancellationToken);
                                newFolder = folderDao.GetFolder(newFolderId);

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
                                    Result += string.Format("folder_{0}{1}", newFolderId, SPLIT_CHAR);
                                }
                            }
                        }
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

        private List<FileEntry<TTo>> MoveOrCopyFiles<TTo>(IServiceScope scope, List<T> fileIds, Folder<TTo> toFolder, bool copy)
        {
            var needToMark = new List<FileEntry<TTo>>();

            if (fileIds.Count == 0) return needToMark;

            var scopeClass = scope.ServiceProvider.GetService<FileMoveCopyOperationScope>();
            var (filesMessageService, fileMarker, fileUtility, global, entryManager) = scopeClass;
            var fileDao = scope.ServiceProvider.GetService<IFileDao<TTo>>();
            var fileTracker = scope.ServiceProvider.GetService<FileTrackerHelper>();

            var toFolderId = toFolder.ID;
            foreach (var fileId in fileIds)
            {
                CancellationToken.ThrowIfCancellationRequested();

                var file = FileDao.GetFile(fileId);
                if (file == null)
                {
                    Error = FilesCommonResource.ErrorMassage_FileNotFound;
                }
                else if (!FilesSecurity.CanRead(file))
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
                    var parentFolder = FolderDao.GetFolder(file.FolderID);
                    try
                    {
                        var conflict = _resolveType == FileConflictResolveType.Duplicate
                            || file.RootFolderType == FolderType.Privacy
                                           ? null
                                           : fileDao.GetFile(toFolderId, file.Title);
                        if (conflict == null)
                        {
                            File<TTo> newFile = null;
                            if (copy)
                            {
                                try
                                {
                                    newFile = FileDao.CopyFile(file.ID, toFolderId); //Stream copy will occur inside dao
                                    filesMessageService.Send(newFile, toFolder, _headers, MessageAction.FileCopied, newFile.Title, parentFolder.Title, toFolder.Title);

                                    if (Equals(newFile.FolderID.ToString(), DaoFolderId))
                                    {
                                        needToMark.Add(newFile);
                                    }

                                    if (ProcessedFile(fileId))
                                    {
                                        Result += string.Format("file_{0}{1}", newFile.ID, SPLIT_CHAR);
                                    }
                                }
                                catch
                                {
                                    if (newFile != null)
                                    {
                                        fileDao.DeleteFile(newFile.ID);
                                    }
                                    throw;
                                }
                            }
                            else
                            {
                                if (WithError(scope, new[] { file }, out var tmpError))
                                {
                                    Error = tmpError;
                                }
                                else
                                {
                                    fileMarker.RemoveMarkAsNewForAll(file);

                                    var newFileId = FileDao.MoveFile(file.ID, toFolderId);
                                    newFile = fileDao.GetFile(newFileId);

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
                                        fileDao.SaveThumbnail(newFile, null);
                                    }


                                    if (Equals(toFolderId.ToString(), DaoFolderId))
                                    {
                                        needToMark.Add(newFile);
                                    }

                                    if (ProcessedFile(fileId))
                                    {
                                        Result += string.Format("file_{0}{1}", newFileId, SPLIT_CHAR);
                                    }
                                }
                            }
                        }
                        else
                        {
                            if (_resolveType == FileConflictResolveType.Overwrite)
                            {
                                if (!FilesSecurity.CanEdit(conflict))
                                {
                                    Error = FilesCommonResource.ErrorMassage_SecurityException;
                                }
                                else if (entryManager.FileLockedForMe(conflict.ID))
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

                                    using (var stream = FileDao.GetFileStream(file))
                                    {
                                        newFile.ContentLength = stream.CanSeek ? stream.Length : file.ContentLength;

                                        newFile = fileDao.SaveFile(newFile, stream);
                                    }

                                    if (file.ThumbnailStatus == Thumbnail.Created)
                                    {
                                        using (var thumbnail = FileDao.GetThumbnail(file))
                                        {
                                            fileDao.SaveThumbnail(newFile, thumbnail);
                                        }
                                        newFile.ThumbnailStatus = Thumbnail.Created;
                                    }

                                    needToMark.Add(newFile);

                                    if (copy)
                                    {
                                        filesMessageService.Send(newFile, toFolder, _headers, MessageAction.FileCopiedWithOverwriting, newFile.Title, parentFolder.Title, toFolder.Title);
                                        if (ProcessedFile(fileId))
                                        {
                                            Result += string.Format("file_{0}{1}", newFile.ID, SPLIT_CHAR);
                                        }
                                    }
                                    else
                                    {
                                        if (Equals(file.FolderID.ToString(), toFolderId.ToString()))
                                        {
                                            if (ProcessedFile(fileId))
                                            {
                                                Result += string.Format("file_{0}{1}", newFile.ID, SPLIT_CHAR);
                                            }
                                        }
                                        else
                                        {
                                            if (WithError(scope, new[] { file }, out var tmpError))
                                            {
                                                Error = tmpError;
                                            }
                                            else
                                            {
                                                FileDao.DeleteFile(file.ID);

                                                if (file.RootFolderType != FolderType.USER)
                                                {
                                                    filesMessageService.Send(file, toFolder, _headers, MessageAction.FileMovedWithOverwriting, file.Title, parentFolder.Title, toFolder.Title);
                                                }
                                                else
                                                {
                                                    filesMessageService.Send(newFile, toFolder, _headers, MessageAction.FileMovedWithOverwriting, file.Title, parentFolder.Title, toFolder.Title);
                                                }

                                                if (ProcessedFile(fileId))
                                                {
                                                    Result += string.Format("file_{0}{1}", newFile.ID, SPLIT_CHAR);
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

            return needToMark;
        }

        private bool WithError(IServiceScope scope, IEnumerable<File<T>> files, out string error)
        {
            var entryManager = scope.ServiceProvider.GetService<EntryManager>();
            var fileTracker = scope.ServiceProvider.GetService<FileTrackerHelper>();
            error = null;
            foreach (var file in files)
            {
                if (!FilesSecurity.CanDelete(file))
                {
                    error = FilesCommonResource.ErrorMassage_SecurityException_MoveFile;
                    return true;
                }
                if (entryManager.FileLockedForMe(file.ID))
                {
                    error = FilesCommonResource.ErrorMassage_LockedFile;
                    return true;
                }
                if (fileTracker.IsEditing(file.ID))
                {
                    error = FilesCommonResource.ErrorMassage_SecurityException_UpdateEditingFile;
                    return true;
                }
            }
            return false;
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