namespace ASC.Files.Api;

public class FileController : ApiControllerBase
{
    private readonly IServiceProvider _serviceProvider;
    private readonly GlobalFolderHelper _globalFolderHelper;
    private readonly FileStorageService<string> _fileStorageServiceString;

    public FileController(
        FilesControllerHelper<int> filesControllerHelperInt,
        FilesControllerHelper<string> filesControllerHelperString,
        IServiceProvider serviceProvider,
        GlobalFolderHelper globalFolderHelper,
        FileStorageService<string> fileStorageServiceString) 
        : base(filesControllerHelperInt, filesControllerHelperString)
    {
        _serviceProvider = serviceProvider;
        _globalFolderHelper = globalFolderHelper;
        _fileStorageServiceString = fileStorageServiceString;
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
    public Task<IEnumerable<FileDto<string>>> ChangeHistoryFromBodyAsync(string fileId, [FromBody] ChangeHistoryRequestDto requestDto)
    {
        return _filesControllerHelperString.ChangeHistoryAsync(fileId, requestDto.Version, requestDto.ContinueVersion);
    }

    [Update("file/{fileId:int}/history")]
    public Task<IEnumerable<FileDto<int>>> ChangeHistoryFromBodyAsync(int fileId, [FromBody] ChangeHistoryRequestDto requestDto)
    {
        return _filesControllerHelperInt.ChangeHistoryAsync(fileId, requestDto.Version, requestDto.ContinueVersion);
    }

    [Update("file/{fileId}/history")]
    [Consumes("application/x-www-form-urlencoded")]
    public Task<IEnumerable<FileDto<string>>> ChangeHistoryFromFormAsync(string fileId, [FromForm] ChangeHistoryRequestDto requestDto)
    {
        return _filesControllerHelperString.ChangeHistoryAsync(fileId, requestDto.Version, requestDto.ContinueVersion);
    }

    [Update("file/{fileId:int}/history")]
    [Consumes("application/x-www-form-urlencoded")]
    public Task<IEnumerable<FileDto<int>>> ChangeHistoryFromFormAsync(int fileId, [FromForm] ChangeHistoryRequestDto requestDto)
    {
        return _filesControllerHelperInt.ChangeHistoryAsync(fileId, requestDto.Version, requestDto.ContinueVersion);
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
    public IAsyncEnumerable<ConversationResultDto<string>> CheckConversionAsync(string fileId, bool start)
    {
        return _filesControllerHelperString.CheckConversionAsync(new CheckConversionRequestDto<string>()
        {
            FileId = fileId,
            StartConvert = start
        });
    }

    [Read("file/{fileId:int}/checkconversion")]
    public IAsyncEnumerable<ConversationResultDto<int>> CheckConversionAsync(int fileId, bool start)
    {
        return _filesControllerHelperInt.CheckConversionAsync(new CheckConversionRequestDto<int>()
        {
            FileId = fileId,
            StartConvert = start
        });
    }

    [Create("file/{fileId:int}/copyas", order: int.MaxValue - 1)]
    public object CopyFileAsFromBody(int fileId, [FromBody] CopyAsRequestDto<JsonElement> requestDto)
    {
        return CopyFile(fileId, requestDto);
    }

    [Create("file/{fileId}/copyas", order: int.MaxValue)]
    public object CopyFileAsFromBody(string fileId, [FromBody] CopyAsRequestDto<JsonElement> requestDto)
    {
        return CopyFile(fileId, requestDto);
    }

    [Create("file/{fileId:int}/copyas", order: int.MaxValue - 1)]
    [Consumes("application/x-www-form-urlencoded")]
    public object CopyFileAsFromForm(int fileId, [FromForm] CopyAsRequestDto<JsonElement> requestDto)
    {
        return CopyFile(fileId, requestDto);
    }

    [Create("file/{fileId}/copyas", order: int.MaxValue)]
    [Consumes("application/x-www-form-urlencoded")]
    public object CopyFileAsFromForm(string fileId, [FromForm] CopyAsRequestDto<JsonElement> requestDto)
    {
        return CopyFile(fileId, requestDto);
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
    public Task<FileDto<int>> CreateFileFromBodyAsync([FromBody] CreateFileRequestDto<JsonElement> requestDto)
    {
        return _filesControllerHelperInt.CreateFileAsync(_globalFolderHelper.FolderMy, requestDto.Title, requestDto.TemplateId, requestDto.EnableExternalExt);
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
    public Task<FileDto<string>> CreateFileFromBodyAsync(string folderId, [FromBody] CreateFileRequestDto<JsonElement> requestDto)
    {
        return _filesControllerHelperString.CreateFileAsync(folderId, requestDto.Title, requestDto.TemplateId, requestDto.EnableExternalExt);
    }

    [Create("{folderId:int}/file")]
    public Task<FileDto<int>> CreateFileFromBodyAsync(int folderId, [FromBody] CreateFileRequestDto<JsonElement> requestDto)
    {
        return _filesControllerHelperInt.CreateFileAsync(folderId, requestDto.Title, requestDto.TemplateId, requestDto.EnableExternalExt);
    }

    [Create("@my/file")]
    [Consumes("application/x-www-form-urlencoded")]
    public Task<FileDto<int>> CreateFileFromFormAsync([FromForm] CreateFileRequestDto<JsonElement> requestDto)
    {
        return _filesControllerHelperInt.CreateFileAsync(_globalFolderHelper.FolderMy, requestDto.Title, requestDto.TemplateId, requestDto.EnableExternalExt);
    }

    [Create("{folderId}/file")]
    [Consumes("application/x-www-form-urlencoded")]
    public Task<FileDto<string>> CreateFileFromFormAsync(string folderId, [FromForm] CreateFileRequestDto<JsonElement> requestDto)
    {
        return _filesControllerHelperString.CreateFileAsync(folderId, requestDto.Title, requestDto.TemplateId, requestDto.EnableExternalExt);
    }

    [Create("{folderId:int}/file")]
    [Consumes("application/x-www-form-urlencoded")]
    public Task<FileDto<int>> CreateFileFromFormAsync(int folderId, [FromForm] CreateFileRequestDto<JsonElement> requestDto)
    {
        return _filesControllerHelperInt.CreateFileAsync(folderId, requestDto.Title, requestDto.TemplateId, requestDto.EnableExternalExt);
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
    public Task<FileDto<string>> CreateHtmlFileFromBodyAsync(string folderId, [FromBody] CreateTextOrHtmlFileRequestDto requestDto)
    {
        return _filesControllerHelperString.CreateHtmlFileAsync(folderId, requestDto.Title, requestDto.Content);
    }

    [Create("{folderId:int}/html")]
    public Task<FileDto<int>> CreateHtmlFileFromBodyAsync(int folderId, [FromBody] CreateTextOrHtmlFileRequestDto requestDto)
    {
        return _filesControllerHelperInt.CreateHtmlFileAsync(folderId, requestDto.Title, requestDto.Content);
    }

    [Create("{folderId}/html")]
    [Consumes("application/x-www-form-urlencoded")]
    public Task<FileDto<string>> CreateHtmlFileFromFormAsync(string folderId, [FromForm] CreateTextOrHtmlFileRequestDto requestDto)
    {
        return _filesControllerHelperString.CreateHtmlFileAsync(folderId, requestDto.Title, requestDto.Content);
    }

    [Create("{folderId:int}/html")]
    [Consumes("application/x-www-form-urlencoded")]
    public Task<FileDto<int>> CreateHtmlFileFromFormAsync(int folderId, [FromForm] CreateTextOrHtmlFileRequestDto requestDto)
    {
        return _filesControllerHelperInt.CreateHtmlFileAsync(folderId, requestDto.Title, requestDto.Content);
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
    public async Task<FileDto<int>> CreateHtmlFileInCommonFromBodyAsync([FromBody] CreateTextOrHtmlFileRequestDto requestDto)
    {
        return await _filesControllerHelperInt.CreateHtmlFileAsync(await _globalFolderHelper.FolderCommonAsync, requestDto.Title, requestDto.Content);
    }

    [Create("@common/html")]
    [Consumes("application/x-www-form-urlencoded")]
    public async Task<FileDto<int>> CreateHtmlFileInCommonFromFormAsync([FromForm] CreateTextOrHtmlFileRequestDto requestDto)
    {
        return await _filesControllerHelperInt.CreateHtmlFileAsync(await _globalFolderHelper.FolderCommonAsync, requestDto.Title, requestDto.Content);
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
    public Task<FileDto<int>> CreateHtmlFileInMyFromBodyAsync([FromBody] CreateTextOrHtmlFileRequestDto requestDto)
    {
        return _filesControllerHelperInt.CreateHtmlFileAsync(_globalFolderHelper.FolderMy, requestDto.Title, requestDto.Content);
    }

    [Create("@my/html")]
    [Consumes("application/x-www-form-urlencoded")]
    public Task<FileDto<int>> CreateHtmlFileInMyFromFormAsync([FromForm] CreateTextOrHtmlFileRequestDto requestDto)
    {
        return _filesControllerHelperInt.CreateHtmlFileAsync(_globalFolderHelper.FolderMy, requestDto.Title, requestDto.Content);
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
    public Task<FileDto<string>> CreateTextFileFromBodyAsync(string folderId, [FromBody] CreateTextOrHtmlFileRequestDto requestDto)
    {
        return _filesControllerHelperString.CreateTextFileAsync(folderId, requestDto.Title, requestDto.Content); 
    }

    [Create("{folderId:int}/text")]
    public Task<FileDto<int>> CreateTextFileFromBodyAsync(int folderId, [FromBody] CreateTextOrHtmlFileRequestDto requestDto)
    {
        return _filesControllerHelperInt.CreateTextFileAsync(folderId, requestDto.Title, requestDto.Content);
    }

    [Create("{folderId}/text")]
    [Consumes("application/x-www-form-urlencoded")]
    public Task<FileDto<string>> CreateTextFileFromFormAsync(string folderId, [FromForm] CreateTextOrHtmlFileRequestDto requestDto)
    {
        return _filesControllerHelperString.CreateTextFileAsync(folderId, requestDto.Title, requestDto.Content);
    }

    [Create("{folderId:int}/text")]
    [Consumes("application/x-www-form-urlencoded")]
    public Task<FileDto<int>> CreateTextFileFromFormAsync(int folderId, [FromForm] CreateTextOrHtmlFileRequestDto requestDto)
    {
        return _filesControllerHelperInt.CreateTextFileAsync(folderId, requestDto.Title, requestDto.Content);
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
    public async Task<FileDto<int>> CreateTextFileInCommonFromBodyAsync([FromBody] CreateTextOrHtmlFileRequestDto requestDto)
    {
        return await _filesControllerHelperInt.CreateTextFileAsync(await _globalFolderHelper.FolderCommonAsync, requestDto.Title, requestDto.Content);
    }

    [Create("@common/text")]
    [Consumes("application/x-www-form-urlencoded")]
    public async Task<FileDto<int>> CreateTextFileInCommonFromFormAsync([FromForm] CreateTextOrHtmlFileRequestDto requestDto)
    {
        return await _filesControllerHelperInt.CreateTextFileAsync(await _globalFolderHelper.FolderCommonAsync, requestDto.Title, requestDto.Content);
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
    public Task<FileDto<int>> CreateTextFileInMyFromBodyAsync([FromBody] CreateTextOrHtmlFileRequestDto requestDto)
    {
        return _filesControllerHelperInt.CreateTextFileAsync(_globalFolderHelper.FolderMy, requestDto.Title, requestDto.Content);
    }

    [Create("@my/text")]
    [Consumes("application/x-www-form-urlencoded")]
    public Task<FileDto<int>> CreateTextFileInMyFromFormAsync([FromForm] CreateTextOrHtmlFileRequestDto requestDto)
    {
        return _filesControllerHelperInt.CreateTextFileAsync(_globalFolderHelper.FolderMy, requestDto.Title, requestDto.Content);
    }

    [Create("thumbnails")]
    public Task<IEnumerable<JsonElement>> CreateThumbnailsFromBodyAsync([FromBody] BaseBatchRequestDto requestDto)
    {
        return _fileStorageServiceString.CreateThumbnailsAsync(requestDto.FileIds.ToList());
    }

    [Create("thumbnails")]
    [Consumes("application/x-www-form-urlencoded")]
    public async Task<IEnumerable<JsonElement>> CreateThumbnailsFromFormAsync([FromForm][ModelBinder(BinderType = typeof(BaseBatchModelBinder))] BaseBatchRequestDto requestDto)
    {
        return await _fileStorageServiceString.CreateThumbnailsAsync(requestDto.FileIds.ToList());
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
    public Task<IEnumerable<FileOperationDto>> DeleteFile(string fileId, [FromBody] DeleteRequestDto requestDto)
    {
        return _filesControllerHelperString.DeleteFileAsync(fileId, requestDto.DeleteAfter, requestDto.Immediately);
    }

    [Delete("file/{fileId:int}", order: int.MaxValue - 1, DisableFormat = true)]
    public Task<IEnumerable<FileOperationDto>> DeleteFile(int fileId, [FromBody] DeleteRequestDto requestDto)
    {
        return _filesControllerHelperInt.DeleteFileAsync(fileId, requestDto.DeleteAfter, requestDto.Immediately);
    }

    [AllowAnonymous]
    [Read("file/{fileId}/edit/diff")]
    public Task<EditHistoryDataDto> GetEditDiffUrlAsync(string fileId, int version = 0, string doc = null)
    {
        return _filesControllerHelperString.GetEditDiffUrlAsync(fileId, version, doc);
    }

    [AllowAnonymous]
    [Read("file/{fileId:int}/edit/diff")]
    public Task<EditHistoryDataDto> GetEditDiffUrlAsync(int fileId, int version = 0, string doc = null)
    {
        return _filesControllerHelperInt.GetEditDiffUrlAsync(fileId, version, doc);
    }

    [AllowAnonymous]
    [Read("file/{fileId}/edit/history")]
    public Task<List<EditHistoryDto>> GetEditHistoryAsync(string fileId, string doc = null)
    {
        return _filesControllerHelperString.GetEditHistoryAsync(fileId, doc);
    }

    [AllowAnonymous]
    [Read("file/{fileId:int}/edit/history")]
    public Task<List<EditHistoryDto>> GetEditHistoryAsync(int fileId, string doc = null)
    {
        return _filesControllerHelperInt.GetEditHistoryAsync(fileId, doc);
    }

    /// <summary>
    /// Returns a detailed information about the file with the ID specified in the request
    /// </summary>
    /// <short>File information</short>
    /// <category>Files</category>
    /// <returns>File info</returns>
    [Read("file/{fileId}", order: int.MaxValue, DisableFormat = true)]
    public Task<FileDto<string>> GetFileInfoAsync(string fileId, int version = -1)
    {
        return _filesControllerHelperString.GetFileInfoAsync(fileId, version);
    }

    [Read("file/{fileId:int}")]
    public Task<FileDto<int>> GetFileInfoAsync(int fileId, int version = -1)
    {
        return _filesControllerHelperInt.GetFileInfoAsync(fileId, version);
    }

    /// <summary>
    /// Returns the detailed information about all the available file versions with the ID specified in the request
    /// </summary>
    /// <short>File versions</short>
    /// <category>Files</category>
    /// <param name="fileId">File ID</param>
    /// <returns>File information</returns>
    [Read("file/{fileId}/history")]
    public Task<IEnumerable<FileDto<string>>> GetFileVersionInfoAsync(string fileId)
    {
        return _filesControllerHelperString.GetFileVersionInfoAsync(fileId);
    }

    [Read("file/{fileId:int}/history")]
    public Task<IEnumerable<FileDto<int>>> GetFileVersionInfoAsync(int fileId)
    {
        return _filesControllerHelperInt.GetFileVersionInfoAsync(fileId);
    }

    [Update("file/{fileId}/lock")]
    public Task<FileDto<string>> LockFileFromBodyAsync(string fileId, [FromBody] LockFileRequestDto requestDto)
    {
        return _filesControllerHelperString.LockFileAsync(fileId, requestDto.LockFile);
    }

    [Update("file/{fileId:int}/lock")]
    public Task<FileDto<int>> LockFileFromBodyAsync(int fileId, [FromBody] LockFileRequestDto requestDto)
    {
        return _filesControllerHelperInt.LockFileAsync(fileId, requestDto.LockFile);
    }

    [Update("file/{fileId}/lock")]
    [Consumes("application/x-www-form-urlencoded")]
    public Task<FileDto<string>> LockFileFromFormAsync(string fileId, [FromForm] LockFileRequestDto requestDto)
    {
        return _filesControllerHelperString.LockFileAsync(fileId, requestDto.LockFile);
    }

    [Update("file/{fileId:int}/lock")]
    [Consumes("application/x-www-form-urlencoded")]
    public Task<FileDto<int>> LockFileFromFormAsync(int fileId, [FromForm] LockFileRequestDto requestDto)
    {
        return _filesControllerHelperInt.LockFileAsync(fileId, requestDto.LockFile);
    }

    [AllowAnonymous]
    [Read("file/{fileId}/restoreversion")]
    public Task<List<EditHistoryDto>> RestoreVersionAsync(string fileId, int version = 0, string url = null, string doc = null)
    {
        return _filesControllerHelperString.RestoreVersionAsync(fileId, version, url, doc);
    }

    [AllowAnonymous]
    [Read("file/{fileId:int}/restoreversion")]
    public Task<List<EditHistoryDto>> RestoreVersionAsync(int fileId, int version = 0, string url = null, string doc = null)
    {
        return _filesControllerHelperInt.RestoreVersionAsync(fileId, version, url, doc);
    }

    /// <summary>
    ///  Start conversion
    /// </summary>
    /// <short>Convert</short>
    /// <category>File operations</category>
    /// <param name="fileId"></param>
    /// <returns>Operation result</returns>
    [Update("file/{fileId}/checkconversion")]
    public IAsyncEnumerable<ConversationResultDto<string>> StartConversion(string fileId, [FromBody(EmptyBodyBehavior = Microsoft.AspNetCore.Mvc.ModelBinding.EmptyBodyBehavior.Allow)] CheckConversionRequestDto<string> requestDto)
    {
        if (requestDto == null)
        {
            requestDto = new CheckConversionRequestDto<string>();
        }
        requestDto.FileId = fileId;
        return _filesControllerHelperString.StartConversionAsync(requestDto);
    }

    [Update("file/{fileId:int}/checkconversion")]
    public IAsyncEnumerable<ConversationResultDto<int>> StartConversion(int fileId, [FromBody(EmptyBodyBehavior = Microsoft.AspNetCore.Mvc.ModelBinding.EmptyBodyBehavior.Allow)] CheckConversionRequestDto<int> requestDto)
    {
        if (requestDto == null)
        {
            requestDto = new CheckConversionRequestDto<int>();
        }
        requestDto.FileId = fileId;
        return _filesControllerHelperInt.StartConversionAsync(requestDto);
    }

    [Update("file/{fileId}/comment")]
    public async Task<object> UpdateCommentFromBodyAsync(string fileId, [FromBody] UpdateCommentRequestDto requestDto)
    {
        return await _filesControllerHelperString.UpdateCommentAsync(fileId, requestDto.Version, requestDto.Comment);
    }

    [Update("file/{fileId:int}/comment")]
    public async Task<object> UpdateCommentFromBodyAsync(int fileId, [FromBody] UpdateCommentRequestDto requestDto)
    {
        return await _filesControllerHelperInt.UpdateCommentAsync(fileId, requestDto.Version, requestDto.Comment);
    }

    [Update("file/{fileId}/comment")]
    [Consumes("application/x-www-form-urlencoded")]
    public async Task<object> UpdateCommentFromFormAsync(string fileId, [FromForm] UpdateCommentRequestDto requestDto)
    {
        return await _filesControllerHelperString.UpdateCommentAsync(fileId, requestDto.Version, requestDto.Comment);
    }

    [Update("file/{fileId:int}/comment")]
    [Consumes("application/x-www-form-urlencoded")]
    public async Task<object> UpdateCommentFromFormAsync(int fileId, [FromForm] UpdateCommentRequestDto requestDto)
    {
        return await _filesControllerHelperInt.UpdateCommentAsync(fileId, requestDto.Version, requestDto.Comment);
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
    public Task<FileDto<string>> UpdateFileFromBodyAsync(string fileId, [FromBody] UpdateFileRequestDto requestDto)
    {
        return _filesControllerHelperString.UpdateFileAsync(fileId, requestDto.Title, requestDto.LastVersion);
    }

    [Update("file/{fileId:int}", order: int.MaxValue - 1, DisableFormat = true)]
    public Task<FileDto<int>> UpdateFileFromBodyAsync(int fileId, [FromBody] UpdateFileRequestDto requestDto)
    {
        return _filesControllerHelperInt.UpdateFileAsync(fileId, requestDto.Title, requestDto.LastVersion);
    }

    [Update("file/{fileId}", order: int.MaxValue, DisableFormat = true)]
    [Consumes("application/x-www-form-urlencoded")]
    public Task<FileDto<string>> UpdateFileFromFormAsync(string fileId, [FromForm] UpdateFileRequestDto requestDto)
    {
        return _filesControllerHelperString.UpdateFileAsync(fileId, requestDto.Title, requestDto.LastVersion);
    }

    [Update("file/{fileId:int}", order: int.MaxValue - 1, DisableFormat = true)]
    [Consumes("application/x-www-form-urlencoded")]
    public Task<FileDto<int>> UpdateFileFromFormAsync(int fileId, [FromForm] UpdateFileRequestDto requestDto)
    {
        return _filesControllerHelperInt.UpdateFileAsync(fileId, requestDto.Title, requestDto.LastVersion);
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
    public Task<FileDto<string>> UpdateFileStreamFromFormAsync(string fileId, [FromForm] FileStreamRequestDto requestDto)
    {
        return _filesControllerHelperString.UpdateFileStreamAsync(_filesControllerHelperInt.GetFileFromRequest(requestDto).OpenReadStream(), fileId, requestDto.FileExtension, requestDto.Encrypted, requestDto.Forcesave);
    }

    [Update("{fileId:int}/update")]
    public Task<FileDto<int>> UpdateFileStreamFromFormAsync(int fileId, [FromForm] FileStreamRequestDto requestDto)
    {
        return _filesControllerHelperInt.UpdateFileStreamAsync(_filesControllerHelperInt.GetFileFromRequest(requestDto).OpenReadStream(), fileId, requestDto.FileExtension, requestDto.Encrypted, requestDto.Forcesave);
    }

    private object CopyFile<T>(T fileId, CopyAsRequestDto<JsonElement> requestDto)
    {
        var helper = _serviceProvider.GetService<FilesControllerHelper<T>>();
        if (requestDto.DestFolderId.ValueKind == JsonValueKind.Number)
        {
            return helper.CopyFileAsAsync(fileId, requestDto.DestFolderId.GetInt32(), requestDto.DestTitle, requestDto.Password);
        }
        else if (requestDto.DestFolderId.ValueKind == JsonValueKind.String)
        {
            return helper.CopyFileAsAsync(fileId, requestDto.DestFolderId.GetString(), requestDto.DestTitle, requestDto.Password);
        }

        return null;
    }
}