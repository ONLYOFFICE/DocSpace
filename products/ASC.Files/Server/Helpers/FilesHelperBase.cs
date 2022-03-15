namespace ASC.Files.Helpers;

[Scope]
public abstract class FilesHelperBase<T>
{
    protected readonly FilesSettingsHelper _filesSettingsHelper;
    protected readonly FileUploader _fileUploader;
    protected readonly SocketManager _socketManager;
    protected readonly FileDtoHelper _fileDtoHelper;
    protected readonly ApiContext _apiContext;
    protected readonly FileStorageService<T> _fileStorageService;
    protected readonly FolderContentDtoHelper _folderContentDtoHelper;
    protected readonly IHttpContextAccessor _httpContextAccessor;
    protected readonly FolderDtoHelper _folderDtoHelper;

    protected FilesHelperBase(
        FilesSettingsHelper filesSettingsHelper,
        FileUploader fileUploader,
        SocketManager socketManager,
        FileDtoHelper fileDtoHelper,
        ApiContext apiContext,
        FileStorageService<T> fileStorageService,
        FolderContentDtoHelper folderContentDtoHelper,
        IHttpContextAccessor httpContextAccessor,
        FolderDtoHelper folderDtoHelper)
    {
        _filesSettingsHelper = filesSettingsHelper;
        _fileUploader = fileUploader;
        _socketManager = socketManager;
        _fileDtoHelper = fileDtoHelper;
        _apiContext = apiContext;
        _fileStorageService = fileStorageService;
        _folderContentDtoHelper = folderContentDtoHelper;
        _httpContextAccessor = httpContextAccessor;
        _folderDtoHelper = folderDtoHelper;
    }

    public async Task<FileDto<T>> InsertFileAsync(T folderId, Stream file, string title, bool? createNewIfExist, bool keepConvertStatus = false)
    {
        try
        {
            var resultFile = await _fileUploader.ExecAsync(folderId, title, file.Length, file, createNewIfExist ?? !_filesSettingsHelper.UpdateIfExist, !keepConvertStatus);

            await _socketManager.CreateFileAsync(resultFile);

            return await _fileDtoHelper.GetAsync(resultFile);
        }
        catch (FileNotFoundException e)
        {
            throw new ItemNotFoundException("File not found", e);
        }
        catch (DirectoryNotFoundException e)
        {
            throw new ItemNotFoundException("Folder not found", e);
        }
    }

    public IFormFile GetFileFromRequest(IModelWithFile model)
    {
        IEnumerable<IFormFile> files = _httpContextAccessor.HttpContext.Request.Form.Files;
        if (files != null && files.Any())
        {
            return files.First();
        }

        return model.File;
    }

    public async Task<FileDto<T>> GetFileInfoAsync(T fileId, int version = -1)
    {
        var file = await _fileStorageService.GetFileAsync(fileId, version);
        file = file.NotFoundIfNull("File not found");

        return await _fileDtoHelper.GetAsync(file);
    }

    public async Task<FileEntryDto> GetFileEntryWrapperAsync(FileEntry r)
    {
        FileEntryDto wrapper = null;
        if (r is Folder<int> fol1)
        {
            wrapper = await _folderDtoHelper.GetAsync(fol1);
        }
        else if (r is Folder<string> fol2)
        {
            wrapper = await _folderDtoHelper.GetAsync(fol2);
        }
        else if (r is File<int> file1)
        {
            wrapper = await _fileDtoHelper.GetAsync(file1);
        }
        else if (r is File<string> file2)
        {
            wrapper = await _fileDtoHelper.GetAsync(file2);
        }

        return wrapper;
    }
}
