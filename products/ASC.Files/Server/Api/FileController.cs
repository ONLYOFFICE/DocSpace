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
    public Task<IEnumerable<FileWrapper<string>>> ChangeHistoryFromBodyAsync(string fileId, [FromBody] ChangeHistoryModel model)
    {
        return _filesControllerHelperString.ChangeHistoryAsync(fileId, model.Version, model.ContinueVersion);
    }

    [Update("file/{fileId:int}/history")]
    public Task<IEnumerable<FileWrapper<int>>> ChangeHistoryFromBodyAsync(int fileId, [FromBody] ChangeHistoryModel model)
    {
        return _filesControllerHelperInt.ChangeHistoryAsync(fileId, model.Version, model.ContinueVersion);
    }

    [Update("file/{fileId}/history")]
    [Consumes("application/x-www-form-urlencoded")]
    public Task<IEnumerable<FileWrapper<string>>> ChangeHistoryFromFormAsync(string fileId, [FromForm] ChangeHistoryModel model)
    {
        return _filesControllerHelperString.ChangeHistoryAsync(fileId, model.Version, model.ContinueVersion);
    }

    [Update("file/{fileId:int}/history")]
    [Consumes("application/x-www-form-urlencoded")]
    public Task<IEnumerable<FileWrapper<int>>> ChangeHistoryFromFormAsync(int fileId, [FromForm] ChangeHistoryModel model)
    {
        return _filesControllerHelperInt.ChangeHistoryAsync(fileId, model.Version, model.ContinueVersion);
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
    public IAsyncEnumerable<ConversationResult<string>> CheckConversionAsync(string fileId, bool start)
    {
        return _filesControllerHelperString.CheckConversionAsync(new CheckConversionModel<string>()
        {
            FileId = fileId,
            StartConvert = start
        });
    }

    [Read("file/{fileId:int}/checkconversion")]
    public IAsyncEnumerable<ConversationResult<int>> CheckConversionAsync(int fileId, bool start)
    {
        return _filesControllerHelperInt.CheckConversionAsync(new CheckConversionModel<int>()
        {
            FileId = fileId,
            StartConvert = start
        });
    }

    [Create("file/{fileId:int}/copyas", order: int.MaxValue - 1)]
    public object CopyFileAsFromBody(int fileId, [FromBody] CopyAsModel<JsonElement> model)
    {
        return CopyFile(fileId, model);
    }

    [Create("file/{fileId}/copyas", order: int.MaxValue)]
    public object CopyFileAsFromBody(string fileId, [FromBody] CopyAsModel<JsonElement> model)
    {
        return CopyFile(fileId, model);
    }

    [Create("file/{fileId:int}/copyas", order: int.MaxValue - 1)]
    [Consumes("application/x-www-form-urlencoded")]
    public object CopyFileAsFromForm(int fileId, [FromForm] CopyAsModel<JsonElement> model)
    {
        return CopyFile(fileId, model);
    }

    [Create("file/{fileId}/copyas", order: int.MaxValue)]
    [Consumes("application/x-www-form-urlencoded")]
    public object CopyFileAsFromForm(string fileId, [FromForm] CopyAsModel<JsonElement> model)
    {
        return CopyFile(fileId, model);
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
    public Task<FileWrapper<int>> CreateFileFromBodyAsync([FromBody] CreateFileModel<JsonElement> model)
    {
        return _filesControllerHelperInt.CreateFileAsync(_globalFolderHelper.FolderMy, model.Title, model.TemplateId, model.EnableExternalExt);
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
    public Task<FileWrapper<string>> CreateFileFromBodyAsync(string folderId, [FromBody] CreateFileModel<JsonElement> model)
    {
        return _filesControllerHelperString.CreateFileAsync(folderId, model.Title, model.TemplateId, model.EnableExternalExt);
    }

    [Create("{folderId:int}/file")]
    public Task<FileWrapper<int>> CreateFileFromBodyAsync(int folderId, [FromBody] CreateFileModel<JsonElement> model)
    {
        return _filesControllerHelperInt.CreateFileAsync(folderId, model.Title, model.TemplateId, model.EnableExternalExt);
    }

    [Create("@my/file")]
    [Consumes("application/x-www-form-urlencoded")]
    public Task<FileWrapper<int>> CreateFileFromFormAsync([FromForm] CreateFileModel<JsonElement> model)
    {
        return _filesControllerHelperInt.CreateFileAsync(_globalFolderHelper.FolderMy, model.Title, model.TemplateId, model.EnableExternalExt);
    }

    [Create("{folderId}/file")]
    [Consumes("application/x-www-form-urlencoded")]
    public Task<FileWrapper<string>> CreateFileFromFormAsync(string folderId, [FromForm] CreateFileModel<JsonElement> model)
    {
        return _filesControllerHelperString.CreateFileAsync(folderId, model.Title, model.TemplateId, model.EnableExternalExt);
    }

    [Create("{folderId:int}/file")]
    [Consumes("application/x-www-form-urlencoded")]
    public Task<FileWrapper<int>> CreateFileFromFormAsync(int folderId, [FromForm] CreateFileModel<JsonElement> model)
    {
        return _filesControllerHelperInt.CreateFileAsync(folderId, model.Title, model.TemplateId, model.EnableExternalExt);
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
    public Task<FileWrapper<string>> CreateHtmlFileFromBodyAsync(string folderId, [FromBody] CreateTextOrHtmlFileModel model)
    {
        return _filesControllerHelperString.CreateHtmlFileAsync(folderId, model.Title, model.Content);
    }

    [Create("{folderId:int}/html")]
    public Task<FileWrapper<int>> CreateHtmlFileFromBodyAsync(int folderId, [FromBody] CreateTextOrHtmlFileModel model)
    {
        return _filesControllerHelperInt.CreateHtmlFileAsync(folderId, model.Title, model.Content);
    }

    [Create("{folderId}/html")]
    [Consumes("application/x-www-form-urlencoded")]
    public Task<FileWrapper<string>> CreateHtmlFileFromFormAsync(string folderId, [FromForm] CreateTextOrHtmlFileModel model)
    {
        return _filesControllerHelperString.CreateHtmlFileAsync(folderId, model.Title, model.Content);
    }

    [Create("{folderId:int}/html")]
    [Consumes("application/x-www-form-urlencoded")]
    public Task<FileWrapper<int>> CreateHtmlFileFromFormAsync(int folderId, [FromForm] CreateTextOrHtmlFileModel model)
    {
        return _filesControllerHelperInt.CreateHtmlFileAsync(folderId, model.Title, model.Content);
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
    public async Task<FileWrapper<int>> CreateHtmlFileInCommonFromBodyAsync([FromBody] CreateTextOrHtmlFileModel model)
    {
        return await _filesControllerHelperInt.CreateHtmlFileAsync(await _globalFolderHelper.FolderCommonAsync, model.Title, model.Content);
    }

    [Create("@common/html")]
    [Consumes("application/x-www-form-urlencoded")]
    public async Task<FileWrapper<int>> CreateHtmlFileInCommonFromFormAsync([FromForm] CreateTextOrHtmlFileModel model)
    {
        return await _filesControllerHelperInt.CreateHtmlFileAsync(await _globalFolderHelper.FolderCommonAsync, model.Title, model.Content);
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
    public Task<FileWrapper<int>> CreateHtmlFileInMyFromBodyAsync([FromBody] CreateTextOrHtmlFileModel model)
    {
        return _filesControllerHelperInt.CreateHtmlFileAsync(_globalFolderHelper.FolderMy, model.Title, model.Content);
    }

    [Create("@my/html")]
    [Consumes("application/x-www-form-urlencoded")]
    public Task<FileWrapper<int>> CreateHtmlFileInMyFromFormAsync([FromForm] CreateTextOrHtmlFileModel model)
    {
        return _filesControllerHelperInt.CreateHtmlFileAsync(_globalFolderHelper.FolderMy, model.Title, model.Content);
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
    public Task<FileWrapper<string>> CreateTextFileFromBodyAsync(string folderId, [FromBody] CreateTextOrHtmlFileModel model)
    {
        return _filesControllerHelperString.CreateTextFileAsync(folderId, model.Title, model.Content); 
    }

    [Create("{folderId:int}/text")]
    public Task<FileWrapper<int>> CreateTextFileFromBodyAsync(int folderId, [FromBody] CreateTextOrHtmlFileModel model)
    {
        return _filesControllerHelperInt.CreateTextFileAsync(folderId, model.Title, model.Content);
    }

    [Create("{folderId}/text")]
    [Consumes("application/x-www-form-urlencoded")]
    public Task<FileWrapper<string>> CreateTextFileFromFormAsync(string folderId, [FromForm] CreateTextOrHtmlFileModel model)
    {
        return _filesControllerHelperString.CreateTextFileAsync(folderId, model.Title, model.Content);
    }

    [Create("{folderId:int}/text")]
    [Consumes("application/x-www-form-urlencoded")]
    public Task<FileWrapper<int>> CreateTextFileFromFormAsync(int folderId, [FromForm] CreateTextOrHtmlFileModel model)
    {
        return _filesControllerHelperInt.CreateTextFileAsync(folderId, model.Title, model.Content);
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
    public async Task<FileWrapper<int>> CreateTextFileInCommonFromBodyAsync([FromBody] CreateTextOrHtmlFileModel model)
    {
        return await _filesControllerHelperInt.CreateTextFileAsync(await _globalFolderHelper.FolderCommonAsync, model.Title, model.Content);
    }

    [Create("@common/text")]
    [Consumes("application/x-www-form-urlencoded")]
    public async Task<FileWrapper<int>> CreateTextFileInCommonFromFormAsync([FromForm] CreateTextOrHtmlFileModel model)
    {
        return await _filesControllerHelperInt.CreateTextFileAsync(await _globalFolderHelper.FolderCommonAsync, model.Title, model.Content);
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
    public Task<FileWrapper<int>> CreateTextFileInMyFromBodyAsync([FromBody] CreateTextOrHtmlFileModel model)
    {
        return _filesControllerHelperInt.CreateTextFileAsync(_globalFolderHelper.FolderMy, model.Title, model.Content);
    }

    [Create("@my/text")]
    [Consumes("application/x-www-form-urlencoded")]
    public Task<FileWrapper<int>> CreateTextFileInMyFromFormAsync([FromForm] CreateTextOrHtmlFileModel model)
    {
        return _filesControllerHelperInt.CreateTextFileAsync(_globalFolderHelper.FolderMy, model.Title, model.Content);
    }

    [Create("thumbnails")]
    public Task<IEnumerable<JsonElement>> CreateThumbnailsFromBodyAsync([FromBody] BaseBatchModel model)
    {
        return _fileStorageServiceString.CreateThumbnailsAsync(model.FileIds.ToList());
    }

    [Create("thumbnails")]
    [Consumes("application/x-www-form-urlencoded")]
    public async Task<IEnumerable<JsonElement>> CreateThumbnailsFromFormAsync([FromForm][ModelBinder(BinderType = typeof(BaseBatchModelBinder))] BaseBatchModel model)
    {
        return await _fileStorageServiceString.CreateThumbnailsAsync(model.FileIds.ToList());
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
    public Task<IEnumerable<FileOperationWraper>> DeleteFile(string fileId, [FromBody] DeleteModel model)
    {
        return _filesControllerHelperString.DeleteFileAsync(fileId, model.DeleteAfter, model.Immediately);
    }

    [Delete("file/{fileId:int}", order: int.MaxValue - 1, DisableFormat = true)]
    public Task<IEnumerable<FileOperationWraper>> DeleteFile(int fileId, [FromBody] DeleteModel model)
    {
        return _filesControllerHelperInt.DeleteFileAsync(fileId, model.DeleteAfter, model.Immediately);
    }

    [AllowAnonymous]
    [Read("file/{fileId}/edit/diff")]
    public Task<EditHistoryData> GetEditDiffUrlAsync(string fileId, int version = 0, string doc = null)
    {
        return _filesControllerHelperString.GetEditDiffUrlAsync(fileId, version, doc);
    }

    [AllowAnonymous]
    [Read("file/{fileId:int}/edit/diff")]
    public Task<EditHistoryData> GetEditDiffUrlAsync(int fileId, int version = 0, string doc = null)
    {
        return _filesControllerHelperInt.GetEditDiffUrlAsync(fileId, version, doc);
    }

    [AllowAnonymous]
    [Read("file/{fileId}/edit/history")]
    public Task<List<EditHistoryWrapper>> GetEditHistoryAsync(string fileId, string doc = null)
    {
        return _filesControllerHelperString.GetEditHistoryAsync(fileId, doc);
    }

    [AllowAnonymous]
    [Read("file/{fileId:int}/edit/history")]
    public Task<List<EditHistoryWrapper>> GetEditHistoryAsync(int fileId, string doc = null)
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
    public Task<FileWrapper<string>> GetFileInfoAsync(string fileId, int version = -1)
    {
        return _filesControllerHelperString.GetFileInfoAsync(fileId, version);
    }

    [Read("file/{fileId:int}")]
    public Task<FileWrapper<int>> GetFileInfoAsync(int fileId, int version = -1)
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
    public Task<IEnumerable<FileWrapper<string>>> GetFileVersionInfoAsync(string fileId)
    {
        return _filesControllerHelperString.GetFileVersionInfoAsync(fileId);
    }

    [Read("file/{fileId:int}/history")]
    public Task<IEnumerable<FileWrapper<int>>> GetFileVersionInfoAsync(int fileId)
    {
        return _filesControllerHelperInt.GetFileVersionInfoAsync(fileId);
    }

    [Update("file/{fileId}/lock")]
    public Task<FileWrapper<string>> LockFileFromBodyAsync(string fileId, [FromBody] LockFileModel model)
    {
        return _filesControllerHelperString.LockFileAsync(fileId, model.LockFile);
    }

    [Update("file/{fileId:int}/lock")]
    public Task<FileWrapper<int>> LockFileFromBodyAsync(int fileId, [FromBody] LockFileModel model)
    {
        return _filesControllerHelperInt.LockFileAsync(fileId, model.LockFile);
    }

    [Update("file/{fileId}/lock")]
    [Consumes("application/x-www-form-urlencoded")]
    public Task<FileWrapper<string>> LockFileFromFormAsync(string fileId, [FromForm] LockFileModel model)
    {
        return _filesControllerHelperString.LockFileAsync(fileId, model.LockFile);
    }

    [Update("file/{fileId:int}/lock")]
    [Consumes("application/x-www-form-urlencoded")]
    public Task<FileWrapper<int>> LockFileFromFormAsync(int fileId, [FromForm] LockFileModel model)
    {
        return _filesControllerHelperInt.LockFileAsync(fileId, model.LockFile);
    }

    [AllowAnonymous]
    [Read("file/{fileId}/restoreversion")]
    public Task<List<EditHistoryWrapper>> RestoreVersionAsync(string fileId, int version = 0, string url = null, string doc = null)
    {
        return _filesControllerHelperString.RestoreVersionAsync(fileId, version, url, doc);
    }

    [AllowAnonymous]
    [Read("file/{fileId:int}/restoreversion")]
    public Task<List<EditHistoryWrapper>> RestoreVersionAsync(int fileId, int version = 0, string url = null, string doc = null)
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
    public IAsyncEnumerable<ConversationResult<string>> StartConversion(string fileId, [FromBody(EmptyBodyBehavior = Microsoft.AspNetCore.Mvc.ModelBinding.EmptyBodyBehavior.Allow)] CheckConversionModel<string> model)
    {
        if (model == null)
        {
            model = new CheckConversionModel<string>();
        }
        model.FileId = fileId;
        return _filesControllerHelperString.StartConversionAsync(model);
    }

    [Update("file/{fileId:int}/checkconversion")]
    public IAsyncEnumerable<ConversationResult<int>> StartConversion(int fileId, [FromBody(EmptyBodyBehavior = Microsoft.AspNetCore.Mvc.ModelBinding.EmptyBodyBehavior.Allow)] CheckConversionModel<int> model)
    {
        if (model == null)
        {
            model = new CheckConversionModel<int>();
        }
        model.FileId = fileId;
        return _filesControllerHelperInt.StartConversionAsync(model);
    }

    [Update("file/{fileId}/comment")]
    public async Task<object> UpdateCommentFromBodyAsync(string fileId, [FromBody] UpdateCommentModel model)
    {
        return await _filesControllerHelperString.UpdateCommentAsync(fileId, model.Version, model.Comment);
    }

    [Update("file/{fileId:int}/comment")]
    public async Task<object> UpdateCommentFromBodyAsync(int fileId, [FromBody] UpdateCommentModel model)
    {
        return await _filesControllerHelperInt.UpdateCommentAsync(fileId, model.Version, model.Comment);
    }

    [Update("file/{fileId}/comment")]
    [Consumes("application/x-www-form-urlencoded")]
    public async Task<object> UpdateCommentFromFormAsync(string fileId, [FromForm] UpdateCommentModel model)
    {
        return await _filesControllerHelperString.UpdateCommentAsync(fileId, model.Version, model.Comment);
    }

    [Update("file/{fileId:int}/comment")]
    [Consumes("application/x-www-form-urlencoded")]
    public async Task<object> UpdateCommentFromFormAsync(int fileId, [FromForm] UpdateCommentModel model)
    {
        return await _filesControllerHelperInt.UpdateCommentAsync(fileId, model.Version, model.Comment);
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
    public Task<FileWrapper<string>> UpdateFileFromBodyAsync(string fileId, [FromBody] UpdateFileModel model)
    {
        return _filesControllerHelperString.UpdateFileAsync(fileId, model.Title, model.LastVersion);
    }

    [Update("file/{fileId:int}", order: int.MaxValue - 1, DisableFormat = true)]
    public Task<FileWrapper<int>> UpdateFileFromBodyAsync(int fileId, [FromBody] UpdateFileModel model)
    {
        return _filesControllerHelperInt.UpdateFileAsync(fileId, model.Title, model.LastVersion);
    }

    [Update("file/{fileId}", order: int.MaxValue, DisableFormat = true)]
    [Consumes("application/x-www-form-urlencoded")]
    public Task<FileWrapper<string>> UpdateFileFromFormAsync(string fileId, [FromForm] UpdateFileModel model)
    {
        return _filesControllerHelperString.UpdateFileAsync(fileId, model.Title, model.LastVersion);
    }

    [Update("file/{fileId:int}", order: int.MaxValue - 1, DisableFormat = true)]
    [Consumes("application/x-www-form-urlencoded")]
    public Task<FileWrapper<int>> UpdateFileFromFormAsync(int fileId, [FromForm] UpdateFileModel model)
    {
        return _filesControllerHelperInt.UpdateFileAsync(fileId, model.Title, model.LastVersion);
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
    public Task<FileWrapper<string>> UpdateFileStreamFromFormAsync(string fileId, [FromForm] FileStreamModel model)
    {
        return _filesControllerHelperString.UpdateFileStreamAsync(_filesControllerHelperInt.GetFileFromRequest(model).OpenReadStream(), fileId, model.FileExtension, model.Encrypted, model.Forcesave);
    }

    [Update("{fileId:int}/update")]
    public Task<FileWrapper<int>> UpdateFileStreamFromFormAsync(int fileId, [FromForm] FileStreamModel model)
    {
        return _filesControllerHelperInt.UpdateFileStreamAsync(_filesControllerHelperInt.GetFileFromRequest(model).OpenReadStream(), fileId, model.FileExtension, model.Encrypted, model.Forcesave);
    }

    private object CopyFile<T>(T fileId, CopyAsModel<JsonElement> model)
    {
        var helper = _serviceProvider.GetService<FilesControllerHelper<T>>();
        if (model.DestFolderId.ValueKind == JsonValueKind.Number)
        {
            return helper.CopyFileAsAsync(fileId, model.DestFolderId.GetInt32(), model.DestTitle, model.Password);
        }
        else if (model.DestFolderId.ValueKind == JsonValueKind.String)
        {
            return helper.CopyFileAsAsync(fileId, model.DestFolderId.GetString(), model.DestTitle, model.Password);
        }

        return null;
    }
}