namespace ASC.Files.Core.ApiModels.ResponseDto;

public class FileDto<T> : FileEntryWrapper<T>
{
    public T FolderId { get; set; }
    public int Version { get; set; }
    public int VersionGroup { get; set; }
    public string ContentLength { get; set; }
    public long? PureContentLength { get; set; }
    public FileStatus FileStatus { get; set; }
    public string ViewUrl { get; set; }
    public string WebUrl { get; set; }
    public FileType FileType { get; set; }
    public string FileExst { get; set; }
    public string Comment { get; set; }
    public bool? Encrypted { get; set; }
    public string ThumbnailUrl { get; set; }
    public Thumbnail ThumbnailStatus { get; set; }
    public bool? Locked { get; set; }
    public string LockedBy { get; set; }
    public bool CanWebRestrictedEditing { get; set; }
    public bool CanFillForms { get; set; }

    public FileDto() { }

    public static FileDto<int> GetSample()
    {
        return new FileDto<int>
        {
            Access = FileShare.ReadWrite,
            //Updated = ApiDateTime.GetSample(),
            //Created = ApiDateTime.GetSample(),
            //CreatedBy = EmployeeWraper.GetSample(),
            Id = 10,
            RootFolderType = FolderType.BUNCH,
            Shared = false,
            Title = "Some titile.txt",
            FileExst = ".txt",
            FileType = FileType.Document,
            //UpdatedBy = EmployeeWraper.GetSample(),
            ContentLength = 12345.ToString(CultureInfo.InvariantCulture),
            FileStatus = FileStatus.IsNew,
            FolderId = 12334,
            Version = 3,
            VersionGroup = 1,
            ViewUrl = "http://www.onlyoffice.com/viewfile?fileid=2221"
        };
    }
}

[Scope]
public class FileDtoHelper : FileEntryDtoHelper
{
    private readonly AuthContext _authContext;
    private readonly IDaoFactory _daoFactory;
    private readonly GlobalFolderHelper _globalFolderHelper;
    private readonly CommonLinkUtility _commonLinkUtility;
    private readonly FilesLinkUtility _filesLinkUtility;
    private readonly FileUtility _fileUtility;

    public FileDtoHelper(
        ApiDateTimeHelper apiDateTimeHelper,
        EmployeeDtoHelper employeeWrapperHelper,
        AuthContext authContext,
        IDaoFactory daoFactory,
        FileSecurity fileSecurity,
        GlobalFolderHelper globalFolderHelper,
        CommonLinkUtility commonLinkUtility,
        FilesLinkUtility filesLinkUtility,
        FileUtility fileUtility,
        FileSharingHelper fileSharingHelper)
        : base(apiDateTimeHelper, employeeWrapperHelper, fileSharingHelper, fileSecurity)
    {
        _authContext = authContext;
        _daoFactory = daoFactory;
        _globalFolderHelper = globalFolderHelper;
        _commonLinkUtility = commonLinkUtility;
        _filesLinkUtility = filesLinkUtility;
        _fileUtility = fileUtility;
    }

    public async Task<FileDto<T>> GetAsync<T>(File<T> file, List<Tuple<FileEntry<T>, bool>> folders = null)
    {
        var result = await GetFileWrapperAsync(file);

        result.FolderId = file.FolderID;
        if (file.RootFolderType == FolderType.USER
            && !Equals(file.RootFolderCreator, _authContext.CurrentAccount.ID))
        {
            result.RootFolderType = FolderType.SHARE;
            var folderDao = _daoFactory.GetFolderDao<T>();
            FileEntry<T> parentFolder;

            if (folders != null)
            {
                var folderWithRight = folders.FirstOrDefault(f => f.Item1.ID.Equals(file.FolderID));
                if (folderWithRight == null || !folderWithRight.Item2)
                {
                    result.FolderId = await _globalFolderHelper.GetFolderShareAsync<T>();
                }
            }
            else
            {
                parentFolder = await folderDao.GetFolderAsync(file.FolderID);
                if (!await _fileSecurity.CanReadAsync(parentFolder))
                {
                    result.FolderId = await _globalFolderHelper.GetFolderShareAsync<T>();
                }
            }
        }

        return result;
    }

    private async Task<FileDto<T>> GetFileWrapperAsync<T>(File<T> file)
    {
        var result = await GetAsync<FileDto<T>, T>(file);

        result.FileExst = FileUtility.GetFileExtension(file.Title);
        result.FileType = FileUtility.GetFileTypeByExtention(result.FileExst);
        result.Version = file.Version;
        result.VersionGroup = file.VersionGroup;
        result.ContentLength = file.ContentLengthString;
        result.FileStatus = file.FileStatus;
        result.PureContentLength = file.ContentLength.NullIfDefault();
        result.Comment = file.Comment;
        result.Encrypted = file.Encrypted.NullIfDefault();
        result.Locked = file.Locked.NullIfDefault();
        result.LockedBy = file.LockedBy;
        result.CanWebRestrictedEditing = _fileUtility.CanWebRestrictedEditing(file.Title);
        result.CanFillForms = await _fileSecurity.CanFillFormsAsync(file);

        try
        {
            result.ViewUrl = _commonLinkUtility.GetFullAbsolutePath(file.DownloadUrl);

            result.WebUrl = _commonLinkUtility.GetFullAbsolutePath(_filesLinkUtility.GetFileWebPreviewUrl(_fileUtility, file.Title, file.ID, file.Version));

            result.ThumbnailStatus = file.ThumbnailStatus;

            if (file.ThumbnailStatus == Thumbnail.Created)
            {
                result.ThumbnailUrl = _commonLinkUtility.GetFullAbsolutePath(_filesLinkUtility.GetFileThumbnailUrl(file.ID, file.Version));
            }
        }
        catch (Exception)
        {
            //Don't catch anything here because of httpcontext
        }

        return result;
    }
}
