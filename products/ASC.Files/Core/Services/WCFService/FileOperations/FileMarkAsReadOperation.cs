namespace ASC.Web.Files.Services.WCFService.FileOperations;

class FileMarkAsReadOperationData<T> : FileOperationData<T>
{
    public FileMarkAsReadOperationData(IEnumerable<object> folders, IEnumerable<object> files, Tenant tenant, bool holdResult = true)
        : this(folders.OfType<T>(), files.OfType<T>(), tenant, holdResult)
    {
    }

    public FileMarkAsReadOperationData(IEnumerable<T> folders, IEnumerable<T> files, Tenant tenant, bool holdResult = true) : base(folders, files, tenant, holdResult)
    {
    }
}

[Transient]
class FileMarkAsReadOperation : ComposeFileOperation<FileMarkAsReadOperationData<string>, FileMarkAsReadOperationData<int>>
{
    public FileMarkAsReadOperation(IServiceProvider serviceProvider, FileOperation<FileMarkAsReadOperationData<string>, string> f1, FileOperation<FileMarkAsReadOperationData<int>, int> f2)
        : base(serviceProvider, f1, f2)
    {
    }

    public override FileOperationType OperationType => FileOperationType.MarkAsRead;
}

class FileMarkAsReadOperation<T> : FileOperation<FileMarkAsReadOperationData<T>, T>
{
    public override FileOperationType OperationType => FileOperationType.MarkAsRead;


    public FileMarkAsReadOperation(IServiceProvider serviceProvider, FileMarkAsReadOperationData<T> fileOperationData)
        : base(serviceProvider, fileOperationData)
    {
    }

    protected override int InitTotalProgressSteps()
    {
        return Files.Count + Folders.Count;
    }

    protected override async Task DoAsync(IServiceScope scope)
    {
        var scopeClass = scope.ServiceProvider.GetService<FileMarkAsReadOperationScope>();
        var (fileMarker, globalFolder, daoFactory, settingsManager) = scopeClass;
        var entries = AsyncEnumerable.Empty<FileEntry<T>>();
        if (Folders.Count > 0)
        {
            entries.Concat(FolderDao.GetFoldersAsync(Folders));
        }
        if (Files.Count > 0)
        {
            entries.Concat(FileDao.GetFilesAsync(Files));
        }

        await entries.ForEachAwaitAsync(async x =>
        {
            CancellationToken.ThrowIfCancellationRequested();

            await fileMarker.RemoveMarkAsNewAsync(x, ((IAccount)Thread.CurrentPrincipal.Identity).ID);

            if (x.FileEntryType == FileEntryType.File)
            {
                ProcessedFile(((File<T>)x).ID);
            }
            else
            {
                ProcessedFolder(((Folder<T>)x).ID);
            }

            ProgressStep();
        });

        var rootIds = new List<int>
            {
                globalFolder.GetFolderMy(fileMarker, daoFactory),
                await globalFolder.GetFolderCommonAsync(fileMarker, daoFactory),
                await globalFolder.GetFolderShareAsync(daoFactory),
                await globalFolder.GetFolderProjectsAsync(daoFactory),
            };

        if (PrivacyRoomSettings.GetEnabled(settingsManager))
        {
            rootIds.Add(await globalFolder.GetFolderPrivacyAsync(daoFactory));
        }

        var newrootfolder = new List<string>();

        foreach (var r in rootIds)
        {
            var item = new KeyValuePair<int, int>(r, await fileMarker.GetRootFoldersIdMarkedAsNewAsync(r));
            newrootfolder.Add($"new_{{\"key\"? \"{item.Key}\", \"value\"? \"{item.Value}\"}}");
        }

        Result += string.Join(SplitChar, newrootfolder.ToArray());
    }
}

[Scope]
public class FileMarkAsReadOperationScope
{
    private readonly FileMarker _fileMarker;
    private readonly GlobalFolder _globalFolder;
    private readonly IDaoFactory _daoFactory;
    private readonly SettingsManager _settingsManager;

    public FileMarkAsReadOperationScope(
        FileMarker fileMarker,
        GlobalFolder globalFolder,
        IDaoFactory daoFactory,
        SettingsManager settingsManager)
    {
        _fileMarker = fileMarker;
        _globalFolder = globalFolder;
        _daoFactory = daoFactory;
        _settingsManager = settingsManager;
    }

    public void Deconstruct(
        out FileMarker fileMarker,
        out GlobalFolder globalFolder,
        out IDaoFactory daoFactory,
        out SettingsManager settingsManager)
    {
        fileMarker = _fileMarker;
        globalFolder = _globalFolder;
        daoFactory = _daoFactory;
        settingsManager = _settingsManager;
    }
}
