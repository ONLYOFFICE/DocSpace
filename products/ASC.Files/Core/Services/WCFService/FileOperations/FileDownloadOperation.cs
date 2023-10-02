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

internal class FileDownloadOperationData<T> : FileOperationData<T>
{
    public Dictionary<T, string> FilesDownload { get; }
    public IDictionary<string, StringValues> Headers { get; }

    public FileDownloadOperationData(Dictionary<T, string> folders, Dictionary<T, string> files, Tenant tenant, IDictionary<string, StringValues> headers,
        ExternalShareData externalShareData, bool holdResult = true)
        : base(folders.Select(f => f.Key).ToList(), files.Select(f => f.Key).ToList(), tenant, externalShareData, holdResult)
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
        _tempStream = tempStream;
        this[OpType] = (int)FileOperationType.Download;
    }

    private readonly TempStream _tempStream;

    public override async Task RunJob(DistributedTask distributedTask, CancellationToken cancellationToken)
    {
        await base.RunJob(distributedTask, cancellationToken);

        await using var scope = ThirdPartyOperation.CreateScopeAsync();
        var tenantManager = scope.ServiceProvider.GetRequiredService<TenantManager>();
        var instanceCrypto = scope.ServiceProvider.GetRequiredService<InstanceCrypto>();
        var daoFactory = scope.ServiceProvider.GetRequiredService<IDaoFactory>();
        var externalShare = scope.ServiceProvider.GetRequiredService<ExternalShare>();
        var scopeClass = scope.ServiceProvider.GetService<FileDownloadOperationScope>();
        var (globalStore, filesLinkUtility, _, _, _, log) = scopeClass;
        var stream = _tempStream.Create();

        var thirdPartyOperation = ThirdPartyOperation as FileDownloadOperation<string>;
        var daoOperation = DaoOperation as FileDownloadOperation<int>;
        await thirdPartyOperation.CompressToZipAsync(stream, scope);
        await daoOperation.CompressToZipAsync(stream, scope);

        if (stream != null)
        {
            var archiveExtension = "";

            using (var zip = scope.ServiceProvider.GetService<CompressToArchive>())
            {
                archiveExtension = zip.ArchiveExtension;
            }

            stream.Position = 0;
            var fileName = FileConstant.DownloadTitle + archiveExtension;

            var thidpartyFolderOnly = thirdPartyOperation.Folders.Count == 1 && thirdPartyOperation.Files.Count == 0;
            var daoFolderOnly = daoOperation.Folders.Count == 1 && daoOperation.Files.Count == 0;
            if ((thidpartyFolderOnly || daoFolderOnly) && (thidpartyFolderOnly != daoFolderOnly))
            {
                if (thidpartyFolderOnly)
                {
                    fileName = string.Format(@"{0}{1}", (await daoFactory.GetFolderDao<string>().GetFolderAsync(thirdPartyOperation.Folders[0])).Title, archiveExtension);
                }
                else
                {
                    fileName = string.Format(@"{0}{1}", (await daoFactory.GetFolderDao<int>().GetFolderAsync(daoOperation.Folders[0])).Title, archiveExtension);
                }
            }
            else
            {
                fileName = string.Format(@"{0}-{1}-{2}{3}", (await tenantManager.GetCurrentTenantAsync()).Alias.ToLower(), FileConstant.DownloadTitle, DateTime.UtcNow.ToString("yyyy-MM-dd"), archiveExtension);
            }

            var store = await globalStore.GetStoreAsync();
            string path;
            string sessionKey = null;

            var isAuthenticated = _principal.Identity is IAccount;

            if (isAuthenticated)
            {
                path = string.Format(@"{0}\{1}", ((IAccount)_principal.Identity).ID, fileName);
            }
            else
            {
                var sessionId = await externalShare.GetSessionIdAsync();
                var linkId = await externalShare.GetLinkIdAsync();

                if (sessionId == default || linkId == default)
                {
                    throw new SecurityException(FilesCommonResource.ErrorMassage_SecurityException);
                }

                path = string.Format(@"{0}\{1}\{2}", linkId, sessionId, fileName);
                sessionKey = await externalShare.CreateDownloadSessionKeyAsync();
            }

            if (await store.IsFileAsync(FileConstant.StorageDomainTmp, path))
            {
                await store.DeleteAsync(FileConstant.StorageDomainTmp, path);
            }

            await store.SaveAsync(
                FileConstant.StorageDomainTmp,
                path,
                stream,
                MimeMapping.GetMimeMapping(path),
                "attachment; filename=\"" + Uri.EscapeDataString(fileName) + "\"");

            this[Res] = $"{filesLinkUtility.FileHandlerPath}?{FilesLinkUtility.Action}=bulk&filename={Uri.EscapeDataString(instanceCrypto.Encrypt(fileName))}";

            if (!isAuthenticated)
            {
                this[Res] += $"&session={HttpUtility.UrlEncode(sessionKey)}";
            }
        }

        this[Finish] = true;
        PublishChanges();
    }

    public override void PublishChanges(DistributedTask task)
    {
        var thirdpartyTask = ThirdPartyOperation;
        var daoTask = DaoOperation;

        var error1 = thirdpartyTask[Err];
        var error2 = daoTask[Err];

        if (!string.IsNullOrEmpty(error1))
        {
            this[Err] = error1;
        }
        else if (!string.IsNullOrEmpty(error2))
        {
            this[Err] = error2;
        }

        this[Process] = thirdpartyTask[Process] + daoTask[Process];

        var progressSteps = ThirdPartyOperation.Total + DaoOperation.Total + 1;

        var progress = (int)(this[Process] / (double)progressSteps * 100);

        this[Progress] = progress;
        PublishChanges();
    }
}

class FileDownloadOperation<T> : FileOperation<FileDownloadOperationData<T>, T>
{
    private readonly Dictionary<T, string> _files;
    private readonly IDictionary<string, StringValues> _headers;
    private ItemNameValueCollection<T> _entriesPathId;

    public FileDownloadOperation(IServiceProvider serviceProvider, FileDownloadOperationData<T> fileDownloadOperationData)
        : base(serviceProvider, fileDownloadOperationData)
    {
        _files = fileDownloadOperationData.FilesDownload;
        _headers = fileDownloadOperationData.Headers;
        this[OpType] = (int)FileOperationType.Download;
    }

    protected override async Task DoJob(IServiceScope scope)
    {
        if (Files.Count == 0 && Folders.Count == 0)
        {
            return;
        }

        (_entriesPathId, var filesForSend, var folderForSend) = await GetEntriesPathIdAsync(scope);

        if (_entriesPathId == null || _entriesPathId.Count == 0)
        {
            if (Files.Count > 0)
            {
                throw new FileNotFoundException(FilesCommonResource.ErrorMassage_FileNotFound);
            }

            throw new DirectoryNotFoundException(FilesCommonResource.ErrorMassage_FolderNotFound);
        }

        Total = _entriesPathId.Count + 1;

        ReplaceLongPath(_entriesPathId);

        Total = _entriesPathId.Count;

        PublishChanges();

        var filesMessageService = scope.ServiceProvider.GetRequiredService<FilesMessageService>();
        foreach (var file in filesForSend)
        {
            var key = file.Id;
            if (_files.ContainsKey(key) && !string.IsNullOrEmpty(_files[key]))
            {
                _ = filesMessageService.SendAsync(MessageAction.FileDownloadedAs, file, _headers, file.Title, _files[key]);
            }
            else
            {
                _ = filesMessageService.SendAsync(MessageAction.FileDownloaded, file, _headers, file.Title);
            }
        }

        foreach (var folder in folderForSend)
        {
            _ = filesMessageService.SendAsync(MessageAction.FolderDownloaded, folder, _headers, folder.Title);
        }
    }

    private async Task<ItemNameValueCollection<T>> ExecPathFromFileAsync(IServiceScope scope, FileEntry<T> file, string path)
    {
        var fileMarker = scope.ServiceProvider.GetService<FileMarker>();
        await fileMarker.RemoveMarkAsNewAsync(file);

        var title = file.Title;

        if (_files.TryGetValue(file.Id, out var convertToExt))
        {
            if (!string.IsNullOrEmpty(convertToExt))
            {
                title = FileUtility.ReplaceFileExtension(title, convertToExt);
            }
        }

        var entriesPathId = new ItemNameValueCollection<T>();
        entriesPathId.Add(path + title, file.Id);

        return entriesPathId;
    }

    private async Task<(ItemNameValueCollection<T>, IEnumerable<FileEntry<T>>, IEnumerable<FileEntry<T>>)> GetEntriesPathIdAsync(IServiceScope scope)
    {
        var fileMarker = scope.ServiceProvider.GetService<FileMarker>();
        var entriesPathId = new ItemNameValueCollection<T>();
        IEnumerable<FileEntry<T>> filesForSend = new List<File<T>>();
        IEnumerable<FileEntry<T>> folderForSend = new List<Folder<T>>();

        if (0 < Files.Count)
        {
            filesForSend = await FilesSecurity.FilterDownloadAsync(FileDao.GetFilesAsync(Files)).ToListAsync();

            foreach (var file in filesForSend)
            {
                entriesPathId.Add(await ExecPathFromFileAsync(scope, file, string.Empty));
            }
        }
        if (0 < Folders.Count)
        {
            folderForSend = await FilesSecurity.FilterDownloadAsync(FolderDao.GetFoldersAsync(Folders)).ToListAsync();

            foreach (var folder in folderForSend)
            {
                await fileMarker.RemoveMarkAsNewAsync(folder);
            }

            var filesInFolder = await GetFilesInFoldersAsync(scope, folderForSend.Select(x => x.Id), string.Empty);
            entriesPathId.Add(filesInFolder);
        }

        if (Folders.Count == 1 && Files.Count == 0)
        {
            var entriesPathIdWithoutRoot = new ItemNameValueCollection<T>();

            foreach (var path in entriesPathId.AllKeys)
            {
                entriesPathIdWithoutRoot.Add(path.Remove(0, path.IndexOf('/') + 1), entriesPathId[path]);
            }

            return (entriesPathIdWithoutRoot, filesForSend, folderForSend);
        }

        return (entriesPathId, filesForSend, folderForSend);
    }
    private async Task<ItemNameValueCollection<T>> GetFilesInFoldersAsync(IServiceScope scope, IEnumerable<T> folderIds, string path)
    {
        var fileMarker = scope.ServiceProvider.GetService<FileMarker>();

        CancellationToken.ThrowIfCancellationRequested();

        var entriesPathId = new ItemNameValueCollection<T>();

        foreach (var folderId in folderIds)
        {
            CancellationToken.ThrowIfCancellationRequested();

            var folder = await FolderDao.GetFolderAsync(folderId);
            if (folder == null || !await FilesSecurity.CanDownloadAsync(folder))
            {
                continue;
            }


            var folderPath = path + folder.Title + "/";
            entriesPathId.Add(folderPath, default(T));

            var files = FilesSecurity.FilterDownloadAsync(FileDao.GetFilesAsync(folder.Id, null, FilterType.None, false, Guid.Empty, string.Empty, true));

            await foreach (var file in files)
            {
                entriesPathId.Add(await ExecPathFromFileAsync(scope, file, folderPath));
            }

            await fileMarker.RemoveMarkAsNewAsync(folder);

            var nestedFolders = await FilesSecurity.FilterDownloadAsync(FolderDao.GetFoldersAsync(folder.Id)).ToListAsync();

            var filesInFolder = await GetFilesInFoldersAsync(scope, nestedFolders.Select(f => f.Id), folderPath);
            entriesPathId.Add(filesInFolder);
        }

        return entriesPathId;
    }

    internal async Task CompressToZipAsync(Stream stream, IServiceScope scope)
    {
        if (_entriesPathId == null)
        {
            return;
        }

        var scopeClass = scope.ServiceProvider.GetService<FileDownloadOperationScope>();
        var (_, _, _, fileConverter, filesMessageService, _) = scopeClass;
        var FileDao = scope.ServiceProvider.GetService<IFileDao<T>>();

        using (var compressTo = scope.ServiceProvider.GetService<CompressToArchive>())
        {
            compressTo.SetStream(stream);

            foreach (var path in _entriesPathId.AllKeys)
            {
                if (string.IsNullOrEmpty(path))
                {
                    ProgressStep();
                    continue;
                }

                var counter = 0;
                foreach (var entryId in _entriesPathId[path])
                {
                    if (CancellationToken.IsCancellationRequested)
                    {
                        CancellationToken.ThrowIfCancellationRequested();
                    }

                    var newtitle = path;

                    File<T> file = null;
                    var convertToExt = string.Empty;

                    if (!Equals(entryId, default(T)))
                    {
                        await FileDao.InvalidateCacheAsync(entryId);
                        file = await FileDao.GetFileAsync(entryId);

                        if (file == null)
                        {
                            this[Err] = FilesCommonResource.ErrorMassage_FileNotFound;
                            continue;
                        }

                        if (_files.TryGetValue(file.Id, out convertToExt))
                        {
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
                            newtitle = newtitle.IndexOf('.') > 0 ? newtitle.Insert(newtitle.LastIndexOf('.'), suffix) : newtitle + suffix;
                        }
                        else
                        {
                            break;
                        }
                    }

                    if (!Equals(entryId, default(T)) && file != null)
                    {
                        compressTo.CreateEntry(newtitle, file.ModifiedOn);
                        try
                        {
                            if (await fileConverter.EnableConvertAsync(file, convertToExt))
                            {
                                //Take from converter
                                await using (var readStream = await fileConverter.ExecAsync(file, convertToExt))
                                {
                                    await compressTo.PutStream(readStream);
                                }
                            }
                            else
                            {
                                await using (var readStream = await FileDao.GetFileStreamAsync(file))
                                {
                                    await compressTo.PutStream(readStream);
                                }
                            }
                            compressTo.CloseEntry();
                        }
                        catch (Exception ex)
                        {
                            this[Err] = ex.Message;

                            Logger.ErrorWithException(ex);
                        }
                    }
                    else
                    {
                        compressTo.CreateEntry(newtitle);
                        compressTo.PutNextEntry();
                        compressTo.CloseEntry();
                    }

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

            if (200 >= path.Length || 0 >= path.IndexOf('/'))
            {
                continue;
            }

            var ids = entriesPathId[path];
            entriesPathId.Remove(path);

            var newtitle = "LONG_FOLDER_NAME" + path.Substring(path.LastIndexOf('/'));
            entriesPathId.Add(newtitle, ids);
        }
    }
}

internal class ItemNameValueCollection<T>
{
    private readonly Dictionary<string, List<T>> _dic = new Dictionary<string, List<T>>();


    public IEnumerable<string> AllKeys => _dic.Keys;

    public IEnumerable<T> this[string name] => _dic[name].ToArray();

    public int Count => _dic.Count;

    public void Add(string name, T value)
    {
        if (!_dic.ContainsKey(name))
        {
            _dic.Add(name, new List<T>());
        }

        _dic[name].Add(value);
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
        if (!_dic.ContainsKey(name))
        {
            _dic.Add(name, new List<T>());
        }

        _dic[name].AddRange(values);
    }

    public void Remove(string name)
    {
        _dic.Remove(name);
    }
}

[Scope]
class FileDownloadOperationScope
{
    private readonly GlobalStore _globalStore;
    private readonly FilesLinkUtility _filesLinkUtility;
    private readonly SetupInfo _setupInfo;
    private readonly FileConverter _fileConverter;
    private readonly FilesMessageService _filesMessageService;
    private readonly ILogger _log;

    public FileDownloadOperationScope(
        GlobalStore globalStore,
        FilesLinkUtility filesLinkUtility,
        SetupInfo setupInfo,
        FileConverter fileConverter,
        FilesMessageService filesMessageService,
        ILogger<FileDownloadOperation> log)
    {
        _globalStore = globalStore;
        _filesLinkUtility = filesLinkUtility;
        _setupInfo = setupInfo;
        _fileConverter = fileConverter;
        _filesMessageService = filesMessageService;
        _log = log;
    }

    public void Deconstruct(out GlobalStore globalStore, out FilesLinkUtility filesLinkUtility, out SetupInfo setupInfo, out FileConverter fileConverter, out FilesMessageService filesMessageService, out ILogger log)
    {
        globalStore = _globalStore;
        filesLinkUtility = _filesLinkUtility;
        setupInfo = _setupInfo;
        fileConverter = _fileConverter;
        filesMessageService = _filesMessageService;
        log = _log;
    }
}
