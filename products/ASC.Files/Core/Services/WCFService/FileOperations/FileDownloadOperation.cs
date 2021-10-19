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
using System.IO;
using System.Linq;
using System.Threading;

using ASC.Common;
using ASC.Common.Security.Authentication;
using ASC.Common.Threading;
using ASC.Common.Web;
using ASC.Core.Tenants;
using ASC.Files.Core;
using ASC.Files.Core.EF;
using ASC.Files.Core.Resources;
using ASC.MessagingSystem;
using ASC.Web.Core.Files;
using ASC.Web.Files.Classes;
using ASC.Web.Files.Core.Compress;
using ASC.Web.Files.Helpers;
using ASC.Web.Files.Utils;
using ASC.Web.Studio.Core;

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Primitives;

namespace ASC.Web.Files.Services.WCFService.FileOperations
{
    internal class FileDownloadOperationData<T> : FileOperationData<T>
    {
        public Dictionary<T, string> FilesDownload { get; }
        public IDictionary<string, StringValues> Headers { get; }

        public FileDownloadOperationData(Dictionary<T, string> folders, Dictionary<T, string> files, Tenant tenant, IDictionary<string, StringValues> headers, bool holdResult = true)
            : base(folders.Select(f => f.Key).ToList(), files.Select(f => f.Key).ToList(), tenant, holdResult)
        {
            FilesDownload = files;
            Headers = headers;
        }
    }

    [Transient]
    class FileDownloadOperation : ComposeFileOperation<FileDownloadOperationData<string>, FileDownloadOperationData<int>>
    {
        public FileDownloadOperation(IServiceProvider serviceProvider, TempStream tempStream, FileOperation<FileDownloadOperationData<string>, string> f1, FileOperation<FileDownloadOperationData<int>, int> f2)
            : base(serviceProvider, f1, f2)
        {
            TempStream = tempStream;
        }

        public override FileOperationType OperationType
        {
            get { return FileOperationType.Download; }
        }

        private TempStream TempStream { get; }

        public override void RunJob(DistributedTask distributedTask, CancellationToken cancellationToken)
        {
            base.RunJob(distributedTask, cancellationToken);

            using var scope = ThirdPartyOperation.CreateScope();
            var scopeClass = scope.ServiceProvider.GetService<FileDownloadOperationScope>();
            var (globalStore, filesLinkUtility, _, _, _) = scopeClass;
            var stream = TempStream.Create();

            (ThirdPartyOperation as FileDownloadOperation<string>).CompressToZip(stream, scope);
            (DaoOperation as FileDownloadOperation<int>).CompressToZip(stream, scope);

            if (stream != null)
            {
                var archiveExtension = "";

                using(var zip = scope.ServiceProvider.GetService<CompressToArchive>())
                {
                    archiveExtension = zip.ArchiveExtension;
                }

                stream.Position = 0;
                string fileName = FileConstant.DownloadTitle + archiveExtension;
                var store = globalStore.GetStore();
                var path = string.Format(@"{0}\{1}", ((IAccount)Thread.CurrentPrincipal.Identity).ID, fileName);

                if (store.IsFile(FileConstant.StorageDomainTmp, path))
                {
                    store.Delete(FileConstant.StorageDomainTmp, path);
                }

                store.Save(
                    FileConstant.StorageDomainTmp,
                    path,
                    stream,
                    MimeMapping.GetMimeMapping(path),
                    "attachment; filename=\"" + fileName + "\"");
                Result = string.Format("{0}?{1}=bulk&ext={2}", filesLinkUtility.FileHandlerPath, FilesLinkUtility.Action, archiveExtension);

                TaskInfo.SetProperty(PROGRESS, 100);
                TaskInfo.SetProperty(RESULT, Result);
                TaskInfo.SetProperty(FINISHED, true);

            }

            TaskInfo.PublishChanges();
        }

        public override void PublishChanges(DistributedTask task)
        {
            var thirdpartyTask = ThirdPartyOperation.GetDistributedTask();
            var daoTask = DaoOperation.GetDistributedTask();

            var error1 = thirdpartyTask.GetProperty<string>(ERROR);
            var error2 = daoTask.GetProperty<string>(ERROR);

            if (!string.IsNullOrEmpty(error1))
            {
                Error = error1;
            }
            else if (!string.IsNullOrEmpty(error2))
            {
                Error = error2;
            }

            successProcessed = thirdpartyTask.GetProperty<int>(PROCESSED) + daoTask.GetProperty<int>(PROCESSED);

            var progressSteps = ThirdPartyOperation.Total + DaoOperation.Total + 1;

            var progress = (int)(successProcessed / (double)progressSteps * 100);

            base.FillDistributedTask();

            TaskInfo.SetProperty(PROGRESS, progress < 100 ? progress : progress);
            TaskInfo.PublishChanges();
        }
    }

    class FileDownloadOperation<T> : FileOperation<FileDownloadOperationData<T>, T>
    {
        private readonly Dictionary<T, string> files;
        private readonly IDictionary<string, StringValues> headers;
        ItemNameValueCollection<T> entriesPathId;
        public override FileOperationType OperationType
        {
            get { return FileOperationType.Download; }
        }

        public FileDownloadOperation(IServiceProvider serviceProvider, FileDownloadOperationData<T> fileDownloadOperationData)
            : base(serviceProvider, fileDownloadOperationData)
        {
            files = fileDownloadOperationData.FilesDownload;
            headers = fileDownloadOperationData.Headers;
        }

        protected override void Do(IServiceScope scope)
        {
            if (!Files.Any() && !Folders.Any()) return;

            entriesPathId = GetEntriesPathId(scope);

            if (entriesPathId == null || entriesPathId.Count == 0)
            {
                if (Files.Count > 0)
                {
                    throw new FileNotFoundException(FilesCommonResource.ErrorMassage_FileNotFound);
                }

                throw new DirectoryNotFoundException(FilesCommonResource.ErrorMassage_FolderNotFound);
            }

            ReplaceLongPath(entriesPathId);

            Total = entriesPathId.Count;

            TaskInfo.PublishChanges();
        }

        private ItemNameValueCollection<T> ExecPathFromFile(IServiceScope scope, File<T> file, string path)
        {
            var fileMarker = scope.ServiceProvider.GetService<FileMarker>();
            fileMarker.RemoveMarkAsNew(file);

            var title = file.Title;

            if (files.ContainsKey(file.ID))
            {
                var convertToExt = files[file.ID];

                if (!string.IsNullOrEmpty(convertToExt))
                {
                    title = FileUtility.ReplaceFileExtension(title, convertToExt);
                }
            }

            var entriesPathId = new ItemNameValueCollection<T>();
            entriesPathId.Add(path + title, file.ID);

            return entriesPathId;
        }

        private ItemNameValueCollection<T> GetEntriesPathId(IServiceScope scope)
        {
            var fileMarker = scope.ServiceProvider.GetService<FileMarker>();
            var entriesPathId = new ItemNameValueCollection<T>();
            if (0 < Files.Count)
            {
                var files = FileDao.GetFiles(Files);
                files = FilesSecurity.FilterRead(files).ToList();
                files.ForEach(file => entriesPathId.Add(ExecPathFromFile(scope, file, string.Empty)));
            }
            if (0 < Folders.Count)
            {
                FilesSecurity.FilterRead(FolderDao.GetFolders(Files)).Cast<FileEntry<T>>().ToList()
                             .ForEach(folder => fileMarker.RemoveMarkAsNew(folder));

                var filesInFolder = GetFilesInFolders(scope, Folders, string.Empty);
                entriesPathId.Add(filesInFolder);
            }
            return entriesPathId;
        }

        private ItemNameValueCollection<T> GetFilesInFolders(IServiceScope scope, IEnumerable<T> folderIds, string path)
        {
            var fileMarker = scope.ServiceProvider.GetService<FileMarker>();

            CancellationToken.ThrowIfCancellationRequested();

            var entriesPathId = new ItemNameValueCollection<T>();
            foreach (var folderId in folderIds)
            {
                CancellationToken.ThrowIfCancellationRequested();

                var folder = FolderDao.GetFolder(folderId);
                if (folder == null || !FilesSecurity.CanRead(folder)) continue;
                var folderPath = path + folder.Title + "/";

                var files = FileDao.GetFiles(folder.ID, null, FilterType.None, false, Guid.Empty, string.Empty, true);
                files = FilesSecurity.FilterRead(files).ToList();
                files.ForEach(file => entriesPathId.Add(ExecPathFromFile(scope, file, folderPath)));

                fileMarker.RemoveMarkAsNew(folder);

                var nestedFolders = FolderDao.GetFolders(folder.ID);
                nestedFolders = FilesSecurity.FilterRead(nestedFolders).ToList();
                if (files.Count == 0 && nestedFolders.Count == 0)
                {
                    entriesPathId.Add(folderPath, default(T));
                }

                var filesInFolder = GetFilesInFolders(scope, nestedFolders.ConvertAll(f => f.ID), folderPath);
                entriesPathId.Add(filesInFolder);
            }
            return entriesPathId;
        }
            
        internal void CompressToZip(Stream stream, IServiceScope scope)
        {
            if (entriesPathId == null) return;
            var scopeClass = scope.ServiceProvider.GetService<FileDownloadOperationScope>();
            var (_, _, _, fileConverter, filesMessageService) = scopeClass;
            var FileDao = scope.ServiceProvider.GetService<IFileDao<T>>();

            using (var compressTo = scope.ServiceProvider.GetService<CompressToArchive>())
            {
                compressTo.SetStream(stream);

                foreach (var path in entriesPathId.AllKeys)
                {
                    var counter = 0;
                    foreach (var entryId in entriesPathId[path])
                    {
                        if (CancellationToken.IsCancellationRequested)
                        {
                            compressTo.Dispose();
                            stream.Dispose();
                            CancellationToken.ThrowIfCancellationRequested();
                        }

                        var newtitle = path;

                        File<T> file = null;
                        var convertToExt = string.Empty;

                        if (!Equals(entryId, default(T)))
                        {
                            FileDao.InvalidateCache(entryId);
                            file = FileDao.GetFile(entryId);

                            if (file == null)
                            {
                                Error = FilesCommonResource.ErrorMassage_FileNotFound;
                                continue;
                            }

                            if (files.ContainsKey(file.ID))
                            {
                                convertToExt = files[file.ID];
                                if (!string.IsNullOrEmpty(convertToExt))
                                {
                                    newtitle = FileUtility.ReplaceFileExtension(path, convertToExt);
                                }
                            }
                        }

                        if (0 < counter)
                        {
                            var suffix = " (" + counter + ")";

                            if (!Equals(entryId, default(T)))
                            {
                                newtitle = 0 < newtitle.IndexOf('.') ? newtitle.Insert(newtitle.LastIndexOf('.'), suffix) : newtitle + suffix;
                            }
                            else
                            {
                                break;
                            }
                        }

                        compressTo.CreateEntry(newtitle);

                        if (!Equals(entryId, default(T)) && file != null)
                        {
                            try
                            {
                                if (fileConverter.EnableConvert(file, convertToExt))
                                {
                                    //Take from converter
                                    using (var readStream = fileConverter.Exec(file, convertToExt))
                                    {
                                        compressTo.PutStream(readStream);

                                        if (!string.IsNullOrEmpty(convertToExt))
                                        {
                                            filesMessageService.Send(file, headers, MessageAction.FileDownloadedAs, file.Title, convertToExt);
                                        }
                                        else
                                        {
                                            filesMessageService.Send(file, headers, MessageAction.FileDownloaded, file.Title);
                                        }
                                    }
                                }
                                else
                                {
                                    using (var readStream = FileDao.GetFileStream(file))
                                    {
                                        compressTo.PutStream(readStream);

                                        filesMessageService.Send(file, headers, MessageAction.FileDownloaded, file.Title);
                                    }
                                }
                            }
                            catch (Exception ex)
                            {
                                Error = ex.Message;
                                Logger.Error(Error, ex);
                            }
                        }
                        else
                        {
                            compressTo.PutNextEntry();
                        }
                        compressTo.CloseEntry();
                        counter++;

                        if (!Equals(entryId, default(T)) && file != null)
                        {
                            ProcessedFile(entryId);
                        }
                        else
                        {
                            ProcessedFolder(default(T));
                        }
                    }

                    ProgressStep();
                }
            }


        }

        private void ReplaceLongPath(ItemNameValueCollection<T> entriesPathId)
        {
            foreach (var path in new List<string>(entriesPathId.AllKeys))
            {
                CancellationToken.ThrowIfCancellationRequested();

                if (200 >= path.Length || 0 >= path.IndexOf('/')) continue;

                var ids = entriesPathId[path];
                entriesPathId.Remove(path);

                var newtitle = "LONG_FOLDER_NAME" + path.Substring(path.LastIndexOf('/'));
                entriesPathId.Add(newtitle, ids);
            }
        }
    }

    internal class ItemNameValueCollection<T>
    {
        private readonly Dictionary<string, List<T>> dic = new Dictionary<string, List<T>>();


        public IEnumerable<string> AllKeys
        {
            get { return dic.Keys; }
        }

        public IEnumerable<T> this[string name]
        {
            get { return dic[name].ToArray(); }
        }

        public int Count
        {
            get { return dic.Count; }
        }

        public void Add(string name, T value)
        {
            if (!dic.ContainsKey(name))
            {
                dic.Add(name, new List<T>());
            }
            dic[name].Add(value);
        }

        public void Add(ItemNameValueCollection<T> collection)
        {
            foreach (var key in collection.AllKeys)
            {
                foreach (var value in collection[key])
                {
                    Add(key, value);
                }
            }
        }

        public void Add(string name, IEnumerable<T> values)
        {
            if (!dic.ContainsKey(name))
            {
                dic.Add(name, new List<T>());
            }
            dic[name].AddRange(values);
        }

        public void Remove(string name)
        {
            dic.Remove(name);
        }
    }

    [Scope]
    public class FileDownloadOperationScope
    {
        private GlobalStore GlobalStore { get; }
        private FilesLinkUtility FilesLinkUtility { get; }
        private SetupInfo SetupInfo { get; }
        private FileConverter FileConverter { get; }
        private FilesMessageService FilesMessageService { get; }

        public FileDownloadOperationScope(
            GlobalStore globalStore,
            FilesLinkUtility filesLinkUtility,
            SetupInfo setupInfo,
            FileConverter fileConverter,
            FilesMessageService filesMessageService)
        {
            GlobalStore = globalStore;
            FilesLinkUtility = filesLinkUtility;
            SetupInfo = setupInfo;
            FileConverter = fileConverter;
            FilesMessageService = filesMessageService;
        }

        public void Deconstruct(out GlobalStore globalStore, out FilesLinkUtility filesLinkUtility, out SetupInfo setupInfo, out FileConverter fileConverter, out FilesMessageService filesMessageService)
        {
            globalStore = GlobalStore;
            filesLinkUtility = FilesLinkUtility;
            setupInfo = SetupInfo;
            fileConverter = FileConverter;
            filesMessageService = FilesMessageService;
        }
    }

}