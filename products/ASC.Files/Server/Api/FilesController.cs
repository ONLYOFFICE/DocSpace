namespace ASC.Files.Api;

[ConstraintRoute("int")]
public class FilesControllerInternal : FilesController<int>
{
    public FilesControllerInternal(IServiceProvider serviceProvider, FilesControllerHelper<int> filesControllerHelper) : base(serviceProvider, filesControllerHelper)
    {
    }
}

public class FilesControllerThirdparty : FilesController<string>
{
    public FilesControllerThirdparty(IServiceProvider serviceProvider, FilesControllerHelper<string> filesControllerHelper) : base(serviceProvider, filesControllerHelper)
    {
    }
}

public abstract class FilesController<T> : ApiControllerBase
{
    private readonly IServiceProvider _serviceProvider;
    private readonly FilesControllerHelper<T> _filesControllerHelper;

    public FilesController(
        IServiceProvider serviceProvider,
        FilesControllerHelper<T> filesControllerHelper)
    {
        _serviceProvider = serviceProvider;
        _filesControllerHelper = filesControllerHelper;
    }

    /// <summary>
    /// Change version history
    /// </summary>
    /// <param name="fileId">File ID</param>
    /// <param name="version">Version of history</param>
    /// <param name="continueVersion">Mark as version or revision</param>
    /// <category>Files</category>
    /// <returns></returns>
    [Update("file/{fileId}/history")]
    public Task<IEnumerable<FileDto<T>>> ChangeHistoryFromBodyAsync(T fileId, [FromBody] ChangeHistoryRequestDto inDto)
    {
        return _filesControllerHelper.ChangeHistoryAsync(fileId, inDto.Version, inDto.ContinueVersion);
    }

    [Update("file/{fileId}/history")]
    [Consumes("application/x-www-form-urlencoded")]
    public Task<IEnumerable<FileDto<T>>> ChangeHistoryFromFormAsync(T fileId, [FromForm] ChangeHistoryRequestDto inDto)
    {
        return _filesControllerHelper.ChangeHistoryAsync(fileId, inDto.Version, inDto.ContinueVersion);
    }

    /// <summary>
    ///  Check conversion status
    /// </summary>
    /// <short>Convert</short>
    /// <category>File operations</category>
    /// <param name="fileId"></param>
    /// <param name="start"></param>
    /// <returns>Operation result</returns>
    [Read("file/{fileId}/checkconversion")]
    public IAsyncEnumerable<ConversationResultDto<T>> CheckConversionAsync(T fileId, bool start)
    {
        return _filesControllerHelper.CheckConversionAsync(new CheckConversionRequestDto<T>()
        {
            FileId = fileId,
            StartConvert = start
        });
    }

    [Create("file/{fileId}/copyas", order: int.MaxValue)]
    public object CopyFileAsFromBody(T fileId, [FromBody] CopyAsRequestDto<JsonElement> inDto)
    {
        return CopyFile(fileId, inDto);
    }

    [Create("file/{fileId}/copyas", order: int.MaxValue)]
    [Consumes("application/x-www-form-urlencoded")]
    public object CopyFileAsFromForm(T fileId, [FromForm] CopyAsRequestDto<JsonElement> inDto)
    {
        return CopyFile(fileId, inDto);
    }

    /// <summary>
    /// Creates a new file in the specified folder with the title sent in the request
    /// </summary>
    /// <short>Create file</short>
    /// <category>File Creation</category>
    /// <param name="folderId">Folder ID</param>
    /// <param name="title" remark="Allowed values: the file must have one of the following extensions: DOCX, XLSX, PPTX">File title</param>
    /// <remarks>In case the extension for the file title differs from DOCX/XLSX/PPTX and belongs to one of the known text, spreadsheet or presentation formats, it will be changed to DOCX/XLSX/PPTX accordingly. If the file extension is not set or is unknown, the DOCX extension will be added to the file title.</remarks>
    /// <returns>New file info</returns>
    [Create("{folderId}/file")]
    public Task<FileDto<T>> CreateFileFromBodyAsync(T folderId, [FromBody] CreateFileRequestDto<JsonElement> inDto)
    {
        return _filesControllerHelper.CreateFileAsync(folderId, inDto.Title, inDto.TemplateId, inDto.EnableExternalExt);
    }

    [Create("{folderId}/file")]
    [Consumes("application/x-www-form-urlencoded")]
    public Task<FileDto<T>> CreateFileFromFormAsync(T folderId, [FromForm] CreateFileRequestDto<JsonElement> inDto)
    {
        return _filesControllerHelper.CreateFileAsync(folderId, inDto.Title, inDto.TemplateId, inDto.EnableExternalExt);
    }

    /// <summary>
    /// Creates an html (.html) file in the selected folder with the title and contents sent in the request
    /// </summary>
    /// <short>Create html</short>
    /// <category>File Creation</category>
    /// <param name="folderId">Folder ID</param>
    /// <param name="title">File title</param>
    /// <param name="content">File contents</param>
    /// <returns>Folder contents</returns>
    [Create("{folderId}/html")]
    public Task<FileDto<T>> CreateHtmlFileFromBodyAsync(T folderId, [FromBody] CreateTextOrHtmlFileRequestDto inDto)
    {
        return _filesControllerHelper.CreateHtmlFileAsync(folderId, inDto.Title, inDto.Content);
    }

    [Create("{folderId}/html")]
    [Consumes("application/x-www-form-urlencoded")]
    public Task<FileDto<T>> CreateHtmlFileFromFormAsync(T folderId, [FromForm] CreateTextOrHtmlFileRequestDto inDto)
    {
        return _filesControllerHelper.CreateHtmlFileAsync(folderId, inDto.Title, inDto.Content);
    }

    /// <summary>
    /// Creates a text (.txt) file in the selected folder with the title and contents sent in the request
    /// </summary>
    /// <short>Create txt</short>
    /// <category>File Creation</category>
    /// <param name="folderId">Folder ID</param>
    /// <param name="title">File title</param>
    /// <param name="content">File contents</param>
    /// <returns>Folder contents</returns>
    [Create("{folderId}/text")]
    public Task<FileDto<T>> CreateTextFileFromBodyAsync(T folderId, [FromBody] CreateTextOrHtmlFileRequestDto inDto)
    {
        return _filesControllerHelper.CreateTextFileAsync(folderId, inDto.Title, inDto.Content);
    }

    [Create("{folderId}/text")]
    [Consumes("application/x-www-form-urlencoded")]
    public Task<FileDto<T>> CreateTextFileFromFormAsync(T folderId, [FromForm] CreateTextOrHtmlFileRequestDto inDto)
    {
        return _filesControllerHelper.CreateTextFileAsync(folderId, inDto.Title, inDto.Content);
    }

    /// <summary>
    /// Deletes the file with the ID specified in the request
    /// </summary>
    /// <short>Delete file</short>
    /// <category>Files</category>
    /// <param name="fileId">File ID</param>
    /// <param name="deleteAfter">Delete after finished</param>
    /// <param name="immediately">Don't move to the Recycle Bin</param>
    /// <returns>Operation result</returns>
    [Delete("file/{fileId}", order: int.MaxValue, DisableFormat = true)]
    public Task<IEnumerable<FileOperationDto>> DeleteFile(T fileId, [FromBody] DeleteRequestDto inDto)
    {
        return _filesControllerHelper.DeleteFileAsync(fileId, inDto.DeleteAfter, inDto.Immediately);
    }

    [AllowAnonymous]
    [Read("file/{fileId}/edit/diff")]
    public Task<EditHistoryDataDto> GetEditDiffUrlAsync(T fileId, int version = 0, string doc = null)
    {
        return _filesControllerHelper.GetEditDiffUrlAsync(fileId, version, doc);
    }

    [AllowAnonymous]
    [Read("file/{fileId}/edit/history")]
    public Task<List<EditHistoryDto>> GetEditHistoryAsync(T fileId, string doc = null)
    {
        return _filesControllerHelper.GetEditHistoryAsync(fileId, doc);
    }

    /// <summary>
    /// Returns a detailed information about the file with the ID specified in the request
    /// </summary>
    /// <short>File information</short>
    /// <category>Files</category>
    /// <returns>File info</returns>
    [Read("file/{fileId}", order: int.MaxValue, DisableFormat = true)]
    public Task<FileDto<T>> GetFileInfoAsync(T fileId, int version = -1)
    {
        return _filesControllerHelper.GetFileInfoAsync(fileId, version);
    }

    /// <summary>
    /// Returns the detailed information about all the available file versions with the ID specified in the request
    /// </summary>
    /// <short>File versions</short>
    /// <category>Files</category>
    /// <param name="fileId">File ID</param>
    /// <returns>File information</returns>
    [Read("file/{fileId}/history")]
    public Task<IEnumerable<FileDto<T>>> GetFileVersionInfoAsync(T fileId)
    {
        return _filesControllerHelper.GetFileVersionInfoAsync(fileId);
    }

    [Update("file/{fileId}/lock")]
    public Task<FileDto<T>> LockFileFromBodyAsync(T fileId, [FromBody] LockFileRequestDto inDto)
    {
        return _filesControllerHelper.LockFileAsync(fileId, inDto.LockFile);
    }

    [Update("file/{fileId}/lock")]
    [Consumes("application/x-www-form-urlencoded")]
    public Task<FileDto<T>> LockFileFromFormAsync(T fileId, [FromForm] LockFileRequestDto inDto)
    {
        return _filesControllerHelper.LockFileAsync(fileId, inDto.LockFile);
    }

    [AllowAnonymous]
    [Read("file/{fileId}/restoreversion")]
    public Task<List<EditHistoryDto>> RestoreVersionAsync(T fileId, int version = 0, string url = null, string doc = null)
    {
        return _filesControllerHelper.RestoreVersionAsync(fileId, version, url, doc);
    }

    /// <summary>
    ///  Start conversion
    /// </summary>
    /// <short>Convert</short>
    /// <category>File operations</category>
    /// <param name="fileId"></param>
    /// <returns>Operation result</returns>
    [Update("file/{fileId}/checkconversion")]
    public IAsyncEnumerable<ConversationResultDto<T>> StartConversion(T fileId, [FromBody(EmptyBodyBehavior = Microsoft.AspNetCore.Mvc.ModelBinding.EmptyBodyBehavior.Allow)] CheckConversionRequestDto<T> inDto)
    {
        if (inDto == null)
        {
            inDto = new CheckConversionRequestDto<T>();
        }
        inDto.FileId = fileId;

        return _filesControllerHelper.StartConversionAsync(inDto);
    }

    [Update("file/{fileId}/comment")]
    public async Task<object> UpdateCommentFromBodyAsync(T fileId, [FromBody] UpdateCommentRequestDto inDto)
    {
        return await _filesControllerHelper.UpdateCommentAsync(fileId, inDto.Version, inDto.Comment);
    }

    [Update("file/{fileId}/comment")]
    [Consumes("application/x-www-form-urlencoded")]
    public async Task<object> UpdateCommentFromFormAsync(T fileId, [FromForm] UpdateCommentRequestDto inDto)
    {
        return await _filesControllerHelper.UpdateCommentAsync(fileId, inDto.Version, inDto.Comment);
    }

    /// <summary>
    ///     Updates the information of the selected file with the parameters specified in the request
    /// </summary>
    /// <short>Update file info</short>
    /// <category>Files</category>
    /// <param name="fileId">File ID</param>
    /// <param name="title">New title</param>
    /// <param name="lastVersion">File last version number</param>
    /// <returns>File info</returns>
    [Update("file/{fileId}", order: int.MaxValue, DisableFormat = true)]
    public Task<FileDto<T>> UpdateFileFromBodyAsync(T fileId, [FromBody] UpdateFileRequestDto inDto)
    {
        return _filesControllerHelper.UpdateFileAsync(fileId, inDto.Title, inDto.LastVersion);
    }

    [Update("file/{fileId}", order: int.MaxValue, DisableFormat = true)]
    [Consumes("application/x-www-form-urlencoded")]
    public Task<FileDto<T>> UpdateFileFromFormAsync(T fileId, [FromForm] UpdateFileRequestDto inDto)
    {
        return _filesControllerHelper.UpdateFileAsync(fileId, inDto.Title, inDto.LastVersion);
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="file"></param>
    /// <param name="fileId"></param>
    /// <param name="encrypted"></param>
    /// <returns></returns>
    /// <visible>false</visible>
    [Update("{fileId}/update")]
    public Task<FileDto<T>> UpdateFileStreamFromFormAsync(T fileId, [FromForm] FileStreamRequestDto inDto)
    {
        return _filesControllerHelper.UpdateFileStreamAsync(_filesControllerHelper.GetFileFromRequest(inDto).OpenReadStream(), fileId, inDto.FileExtension, inDto.Encrypted, inDto.Forcesave);
    }


    private object CopyFile(T fileId, CopyAsRequestDto<JsonElement> inDto)
    {
        var helper = _serviceProvider.GetService<FilesControllerHelper<T>>();
        if (inDto.DestFolderId.ValueKind == JsonValueKind.Number)
        {
            return helper.CopyFileAsAsync(fileId, inDto.DestFolderId.GetInt32(), inDto.DestTitle, inDto.Password);
        }
        else if (inDto.DestFolderId.ValueKind == JsonValueKind.String)
        {
            return helper.CopyFileAsAsync(fileId, inDto.DestFolderId.GetString(), inDto.DestTitle, inDto.Password);
        }

        return null;
    }
}

public class FilesControllerCommon : ApiControllerBase
{
    private readonly GlobalFolderHelper _globalFolderHelper;
    private readonly FileStorageService<string> _fileStorageServiceThirdparty;
    private readonly FilesControllerHelper<int> _filesControllerHelperInternal;

    public FilesControllerCommon(
        GlobalFolderHelper globalFolderHelper,
        FileStorageService<string> fileStorageServiceThirdparty,
        FilesControllerHelper<int> filesControllerHelperInternal)
    {
        _globalFolderHelper = globalFolderHelper;
        _fileStorageServiceThirdparty = fileStorageServiceThirdparty;
        _filesControllerHelperInternal = filesControllerHelperInternal;
    }

    /// <summary>
    /// Creates a new file in the 'My Documents' section with the title sent in the request
    /// </summary>
    /// <short>Create file</short>
    /// <category>File Creation</category>
    /// <param name="title" remark="Allowed values: the file must have one of the following extensions: DOCX, XLSX, PPTX">File title</param>
    /// <remarks>In case the extension for the file title differs from DOCX/XLSX/PPTX and belongs to one of the known text, spreadsheet or presentation formats, it will be changed to DOCX/XLSX/PPTX accordingly. If the file extension is not set or is unknown, the DOCX extension will be added to the file title.</remarks>
    /// <returns>New file info</returns>
    [Create("@my/file")]
    public Task<FileDto<int>> CreateFileFromBodyAsync([FromBody] CreateFileRequestDto<JsonElement> inDto)
    {
        return _filesControllerHelperInternal.CreateFileAsync(_globalFolderHelper.FolderMy, inDto.Title, inDto.TemplateId, inDto.EnableExternalExt);
    }

    [Create("@my/file")]
    [Consumes("application/x-www-form-urlencoded")]
    public Task<FileDto<int>> CreateFileFromFormAsync([FromForm] CreateFileRequestDto<JsonElement> inDto)
    {
        return _filesControllerHelperInternal.CreateFileAsync(_globalFolderHelper.FolderMy, inDto.Title, inDto.TemplateId, inDto.EnableExternalExt);
    }

    /// <summary>
    /// Creates an html (.html) file in 'Common Documents' section with the title and contents sent in the request
    /// </summary>
    /// <short>Create html in 'Common'</short>
    /// <category>File Creation</category>
    /// <param name="title">File title</param>
    /// <param name="content">File contents</param>
    /// <returns>Folder contents</returns>        
    [Create("@common/html")]
    public async Task<FileDto<int>> CreateHtmlFileInCommonFromBodyAsync([FromBody] CreateTextOrHtmlFileRequestDto inDto)
    {
        return await _filesControllerHelperInternal.CreateHtmlFileAsync(await _globalFolderHelper.FolderCommonAsync, inDto.Title, inDto.Content);
    }

    [Create("@common/html")]
    [Consumes("application/x-www-form-urlencoded")]
    public async Task<FileDto<int>> CreateHtmlFileInCommonFromFormAsync([FromForm] CreateTextOrHtmlFileRequestDto inDto)
    {
        return await _filesControllerHelperInternal.CreateHtmlFileAsync(await _globalFolderHelper.FolderCommonAsync, inDto.Title, inDto.Content);
    }

    /// <summary>
    /// Creates an html (.html) file in 'My Documents' section with the title and contents sent in the request
    /// </summary>
    /// <short>Create html in 'My'</short>
    /// <category>File Creation</category>
    /// <param name="title">File title</param>
    /// <param name="content">File contents</param>
    /// <returns>Folder contents</returns>
    [Create("@my/html")]
    public Task<FileDto<int>> CreateHtmlFileInMyFromBodyAsync([FromBody] CreateTextOrHtmlFileRequestDto inDto)
    {
        return _filesControllerHelperInternal.CreateHtmlFileAsync(_globalFolderHelper.FolderMy, inDto.Title, inDto.Content);
    }

    [Create("@my/html")]
    [Consumes("application/x-www-form-urlencoded")]
    public Task<FileDto<int>> CreateHtmlFileInMyFromFormAsync([FromForm] CreateTextOrHtmlFileRequestDto inDto)
    {
        return _filesControllerHelperInternal.CreateHtmlFileAsync(_globalFolderHelper.FolderMy, inDto.Title, inDto.Content);
    }

    /// <summary>
    /// Creates a text (.txt) file in 'Common Documents' section with the title and contents sent in the request
    /// </summary>
    /// <short>Create txt in 'Common'</short>
    /// <category>File Creation</category>
    /// <param name="title">File title</param>
    /// <param name="content">File contents</param>
    /// <returns>Folder contents</returns>
    [Create("@common/text")]
    public async Task<FileDto<int>> CreateTextFileInCommonFromBodyAsync([FromBody] CreateTextOrHtmlFileRequestDto inDto)
    {
        return await _filesControllerHelperInternal.CreateTextFileAsync(await _globalFolderHelper.FolderCommonAsync, inDto.Title, inDto.Content);
    }

    [Create("@common/text")]
    [Consumes("application/x-www-form-urlencoded")]
    public async Task<FileDto<int>> CreateTextFileInCommonFromFormAsync([FromForm] CreateTextOrHtmlFileRequestDto inDto)
    {
        return await _filesControllerHelperInternal.CreateTextFileAsync(await _globalFolderHelper.FolderCommonAsync, inDto.Title, inDto.Content);
    }

    /// <summary>
    /// Creates a text (.txt) file in 'My Documents' section with the title and contents sent in the request
    /// </summary>
    /// <short>Create txt in 'My'</short>
    /// <category>File Creation</category>
    /// <param name="title">File title</param>
    /// <param name="content">File contents</param>
    /// <returns>Folder contents</returns>
    [Create("@my/text")]
    public Task<FileDto<int>> CreateTextFileInMyFromBodyAsync([FromBody] CreateTextOrHtmlFileRequestDto inDto)
    {
        return _filesControllerHelperInternal.CreateTextFileAsync(_globalFolderHelper.FolderMy, inDto.Title, inDto.Content);
    }

    [Create("@my/text")]
    [Consumes("application/x-www-form-urlencoded")]
    public Task<FileDto<int>> CreateTextFileInMyFromFormAsync([FromForm] CreateTextOrHtmlFileRequestDto inDto)
    {
        return _filesControllerHelperInternal.CreateTextFileAsync(_globalFolderHelper.FolderMy, inDto.Title, inDto.Content);
    }

    [Create("thumbnails")]
    public Task<IEnumerable<JsonElement>> CreateThumbnailsFromBodyAsync([FromBody] BaseBatchRequestDto inDto)
    {
        return _fileStorageServiceThirdparty.CreateThumbnailsAsync(inDto.FileIds.ToList());
    }

    [Create("thumbnails")]
    [Consumes("application/x-www-form-urlencoded")]
    public async Task<IEnumerable<JsonElement>> CreateThumbnailsFromFormAsync([FromForm][ModelBinder(BinderType = typeof(BaseBatchModelBinder))] BaseBatchRequestDto inDto)
    {
        return await _fileStorageServiceThirdparty.CreateThumbnailsAsync(inDto.FileIds.ToList());
    }
}