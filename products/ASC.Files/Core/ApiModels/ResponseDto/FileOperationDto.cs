namespace ASC.Files.Core.ApiModels.ResponseDto;

public class FileOperationDto
{
    public string Id { get; set; }

    [JsonPropertyName("Operation")]
    public FileOperationType OperationType { get; set; }
    public int Progress { get; set; }
    public string Error { get; set; }
    public string Processed { get; set; }
    public bool Finished { get; set; }
    public string Url { get; set; }
    public List<FileEntryDto> Files { get; set; }
    public List<FileEntryDto> Folders { get; set; }

    public FileOperationDto() { }

    public static FileOperationDto GetSample()
    {
        return new FileOperationDto
        {
            Id = Guid.NewGuid().ToString(),
            OperationType = FileOperationType.Move,
            Progress = 100,
            //Source = "folder_1,file_1",
            //Result = "folder_1,file_1",
            Error = "",
            Processed = "1",
            Files = new List<FileEntryDto> { FileDto<int>.GetSample() },
            Folders = new List<FileEntryDto> { FolderDto<int>.GetSample() }
        };
    }
}

[Scope]
public class FileOperationDtoHelper
{
    private readonly FolderDtoHelper _folderWrapperHelper;
    private readonly FileDtoHelper _filesWrapperHelper;
    private readonly IDaoFactory _daoFactory;
    private readonly CommonLinkUtility _commonLinkUtility;

    public FileOperationDtoHelper(
        FolderDtoHelper folderWrapperHelper,
        FileDtoHelper filesWrapperHelper,
        IDaoFactory daoFactory,
        CommonLinkUtility commonLinkUtility)
    {
        _folderWrapperHelper = folderWrapperHelper;
        _filesWrapperHelper = filesWrapperHelper;
        _daoFactory = daoFactory;
        _commonLinkUtility = commonLinkUtility;
    }

    public async Task<FileOperationDto> GetAsync(FileOperationResult o)
    {
        var result = new FileOperationDto
        {
            Id = o.Id,
            OperationType = o.OperationType,
            Progress = o.Progress,
            Error = o.Error,
            Processed = o.Processed,
            Finished = o.Finished
        };

        if (!string.IsNullOrEmpty(o.Result) && result.OperationType != FileOperationType.Delete)
        {
            var arr = o.Result.Split(':');
            var folders = arr
                .Where(s => s.StartsWith("folder_"))
                .Select(s => s.Substring(7));

            if (folders.Any())
            {
                var fInt = new List<int>();
                var fString = new List<string>();

                foreach (var folder in folders)
                {
                    if (int.TryParse(folder, out var f))
                    {
                        fInt.Add(f);
                    }
                    else
                    {
                        fString.Add(folder);
                    }
                }

                result.Folders = await GetFoldersAsync(folders);
                result.Folders.AddRange(await GetFoldersAsync(fInt));
            }

            var files = arr
                .Where(s => s.StartsWith("file_"))
                .Select(s => s.Substring(5));

            if (files.Any())
            {
                var fInt = new List<int>();
                var fString = new List<string>();

                foreach (var file in files)
                {
                    if (int.TryParse(file, out var f))
                    {
                        fInt.Add(f);
                    }
                    else
                    {
                        fString.Add(file);
                    }
                }

                result.Files = await GetFilesAsync(fString);
                result.Files.AddRange(await GetFilesAsync(fInt));
            }

            if (result.OperationType == FileOperationType.Download)
            {
                result.Url = _commonLinkUtility.GetFullAbsolutePath(o.Result);
            }
        }

        return result;

        async Task<List<FileEntryDto>> GetFoldersAsync<T>(IEnumerable<T> folders)
        {
            var folderDao = _daoFactory.GetFolderDao<T>();
            var folderEnum = folderDao.GetFoldersAsync(folders).SelectAwait(async r => await _folderWrapperHelper.GetAsync(r)).Cast<FileEntryDto>();

            return await folderEnum.ToListAsync();
        }

        async Task<List<FileEntryDto>> GetFilesAsync<T>(IEnumerable<T> files)
        {
            var fileDao = _daoFactory.GetFileDao<T>();
            var filesEnum = fileDao.GetFilesAsync(files).SelectAwait(async r => await _filesWrapperHelper.GetAsync(r)).Cast<FileEntryDto>();

            return await filesEnum.ToListAsync(); ;
        }
    }
}
