namespace ASC.Files.Api;

public class UploadController : ApiControllerBase
{
    private readonly GlobalFolderHelper _globalFolderHelper;
    private readonly UploadControllerHelper<string> _filesControllerHelperString;
    private readonly UploadControllerHelper<int> _filesControllerHelperInt;

    public UploadController(
        GlobalFolderHelper globalFolderHelper,
        UploadControllerHelper<string> filesControllerHelperString,
        UploadControllerHelper<int> filesControllerHelperInt)
    {
        _globalFolderHelper = globalFolderHelper;
        _filesControllerHelperString = filesControllerHelperString;
        _filesControllerHelperInt = filesControllerHelperInt;
    }

    /// <summary>
    /// Creates session to upload large files in multiple chunks.
    /// </summary>
    /// <short>Chunked upload</short>
    /// <category>Uploads</category>
    /// <param name="folderId">Id of the folder in which file will be uploaded</param>
    /// <param name="fileName">Name of file which has to be uploaded</param>
    /// <param name="fileSize">Length in bytes of file which has to be uploaded</param>
    /// <param name="relativePath">Relative folder from folderId</param>
    /// <param name="encrypted" visible="false"></param>
    /// <remarks>
    /// <![CDATA[
    /// Each chunk can have different length but its important what length is multiple of <b>512</b> and greater or equal than <b>10 mb</b>. Last chunk can have any size.
    /// After initial request respond with status 200 OK you must obtain value of 'location' field from the response. Send all your chunks to that location.
    /// Each chunk must be sent in strict order in which chunks appears in file.
    /// After receiving each chunk if no errors occured server will respond with current information about upload session.
    /// When number of uploaded bytes equal to the number of bytes you send in initial request server will respond with 201 Created and will send you info about uploaded file.
    /// ]]>
    /// </remarks>
    /// <returns>
    /// <![CDATA[
    /// Information about created session. Which includes:
    /// <ul>
    /// <li><b>id:</b> unique id of this upload session</li>
    /// <li><b>created:</b> UTC time when session was created</li>
    /// <li><b>expired:</b> UTC time when session will be expired if no chunks will be sent until that time</li>
    /// <li><b>location:</b> URL to which you must send your next chunk</li>
    /// <li><b>bytes_uploaded:</b> If exists contains number of bytes uploaded for specific upload id</li>
    /// <li><b>bytes_total:</b> Number of bytes which has to be uploaded</li>
    /// </ul>
    /// ]]>
    /// </returns>
    [Create("{folderId}/upload/create_session")]
    public Task<object> CreateUploadSessionFromBodyAsync(string folderId, [FromBody] SessionRequestDto inDto)
    {
        return _filesControllerHelperString.CreateUploadSessionAsync(folderId, inDto.FileName, inDto.FileSize, inDto.RelativePath, inDto.LastModified, inDto.Encrypted);
    }

    [Create("{folderId:int}/upload/create_session")]
    public Task<object> CreateUploadSessionFromBodyAsync(int folderId, [FromBody] SessionRequestDto inDto)
    {
        return _filesControllerHelperInt.CreateUploadSessionAsync(folderId, inDto.FileName, inDto.FileSize, inDto.RelativePath, inDto.LastModified, inDto.Encrypted);
    }

    [Create("{folderId}/upload/create_session")]
    [Consumes("application/x-www-form-urlencoded")]
    public Task<object> CreateUploadSessionFromFormAsync(string folderId, [FromForm] SessionRequestDto inDto)
    {
        return _filesControllerHelperString.CreateUploadSessionAsync(folderId, inDto.FileName, inDto.FileSize, inDto.RelativePath, inDto.LastModified, inDto.Encrypted);
    }

    [Create("{folderId:int}/upload/create_session")]
    [Consumes("application/x-www-form-urlencoded")]
    public Task<object> CreateUploadSessionFromFormAsync(int folderId, [FromForm] SessionRequestDto inDto)
    {
        return _filesControllerHelperInt.CreateUploadSessionAsync(folderId, inDto.FileName, inDto.FileSize, inDto.RelativePath, inDto.LastModified, inDto.Encrypted);
    }

    /// <summary>
    /// Uploads the file specified with single file upload
    /// </summary>
    /// <param name="folderId">Folder ID to upload to</param>
    /// <param name="file" visible="false">Request Input stream</param>
    /// <param name="title">Name of file which has to be uploaded</param>
    /// <param name="createNewIfExist" visible="false">Create New If Exist</param>
    /// <param name="keepConvertStatus" visible="false">Keep status conversation after finishing</param>
    /// <category>Uploads</category>
    /// <returns></returns>
    [Create("{folderId}/insert", order: int.MaxValue)]
    public Task<FileDto<string>> InsertFileAsync(string folderId, [FromForm][ModelBinder(BinderType = typeof(InsertFileModelBinder))] InsertFileRequestDto inDto)
    {
        return _filesControllerHelperString.InsertFileAsync(folderId, inDto.Stream, inDto.Title, inDto.CreateNewIfExist, inDto.KeepConvertStatus);
    }

    [Create("{folderId:int}/insert", order: int.MaxValue - 1)]
    public Task<FileDto<int>> InsertFileFromFormAsync(int folderId, [FromForm][ModelBinder(BinderType = typeof(InsertFileModelBinder))] InsertFileRequestDto inDto)
    {
        return _filesControllerHelperInt.InsertFileAsync(folderId, inDto.Stream, inDto.Title, inDto.CreateNewIfExist, inDto.KeepConvertStatus);
    }

    /// <summary>
    /// Uploads the file specified with single file upload to 'Common Documents' section
    /// </summary>
    /// <param name="file" visible="false">Request Input stream</param>
    /// <param name="title">Name of file which has to be uploaded</param>
    /// <param name="createNewIfExist" visible="false">Create New If Exist</param>
    /// <param name="keepConvertStatus" visible="false">Keep status conversation after finishing</param>
    /// <category>Uploads</category>
    /// <returns></returns>
    [Create("@common/insert")]
    public async Task<FileDto<int>> InsertFileToCommonFromBodyAsync([FromForm][ModelBinder(BinderType = typeof(InsertFileModelBinder))] InsertFileRequestDto inDto)
    {
        return await _filesControllerHelperInt.InsertFileAsync(await _globalFolderHelper.FolderCommonAsync, inDto.Stream, inDto.Title, inDto.CreateNewIfExist, inDto.KeepConvertStatus);
    }

    /// <summary>
    /// Uploads the file specified with single file upload to 'Common Documents' section
    /// </summary>
    /// <param name="file" visible="false">Request Input stream</param>
    /// <param name="title">Name of file which has to be uploaded</param>
    /// <param name="createNewIfExist" visible="false">Create New If Exist</param>
    /// <param name="keepConvertStatus" visible="false">Keep status conversation after finishing</param>
    /// <category>Uploads</category>
    /// <returns></returns>
    [Create("@my/insert")]
    public Task<FileDto<int>> InsertFileToMyFromBodyAsync([FromForm][ModelBinder(BinderType = typeof(InsertFileModelBinder))] InsertFileRequestDto inDto)
    {
        return _filesControllerHelperInt.InsertFileAsync(_globalFolderHelper.FolderMy, inDto.Stream, inDto.Title, inDto.CreateNewIfExist, inDto.KeepConvertStatus);
    }

    /// <summary>
    /// Uploads the file specified with single file upload or standart multipart/form-data method to the selected folder
    /// </summary>
    /// <short>Upload to folder</short>
    /// <category>Uploads</category>
    /// <remarks>
    /// <![CDATA[
    ///  Upload can be done in 2 different ways:
    ///  <ol>
    /// <li>Single file upload. You should set Content-Type &amp; Content-Disposition header to specify filename and content type, and send file in request body</li>
    /// <li>Using standart multipart/form-data method</li>
    /// </ol>]]>
    /// </remarks>
    /// <param name="folderId">Folder ID to upload to</param>
    /// <param name="file" visible="false">Request Input stream</param>
    /// <param name="contentType" visible="false">Content-Type Header</param>
    /// <param name="contentDisposition" visible="false">Content-Disposition Header</param>
    /// <param name="files" visible="false">List of files when posted as multipart/form-data</param>
    /// <param name="createNewIfExist" visible="false">Create New If Exist</param>
    /// <param name="storeOriginalFileFlag" visible="false">If True, upload documents in original formats as well</param>
    /// <param name="keepConvertStatus" visible="false">Keep status conversation after finishing</param>
    /// <returns>Uploaded file</returns>
    [Create("{folderId}/upload", order: int.MaxValue)]
    public Task<object> UploadFileAsync(string folderId, [ModelBinder(BinderType = typeof(UploadModelBinder))] UploadRequestDto inDto)
    {
        return _filesControllerHelperString.UploadFileAsync(folderId, inDto);
    }

    [Create("{folderId:int}/upload", order: int.MaxValue - 1)]
    public Task<object> UploadFileAsync(int folderId, [ModelBinder(BinderType = typeof(UploadModelBinder))] UploadRequestDto inDto)
    {
        return _filesControllerHelperInt.UploadFileAsync(folderId, inDto);
    }

    /// <summary>
    /// Uploads the file specified with single file upload or standart multipart/form-data method to 'Common Documents' section
    /// </summary>
    /// <short>Upload to Common</short>
    /// <category>Uploads</category>
    /// <remarks>
    /// <![CDATA[
    ///  Upload can be done in 2 different ways:
    ///  <ol>
    /// <li>Single file upload. You should set Content-Type &amp; Content-Disposition header to specify filename and content type, and send file in request body</li>
    /// <li>Using standart multipart/form-data method</li>
    /// </ol>]]>
    /// </remarks>
    /// <param name="file" visible="false">Request Input stream</param>
    /// <param name="contentType" visible="false">Content-Type Header</param>
    /// <param name="contentDisposition" visible="false">Content-Disposition Header</param>
    /// <param name="files" visible="false">List of files when posted as multipart/form-data</param>
    /// <returns>Uploaded file</returns>
    [Create("@common/upload")]
    public async Task<object> UploadFileToCommonAsync([ModelBinder(BinderType = typeof(UploadModelBinder))] UploadRequestDto inDto)
    {
        inDto.CreateNewIfExist = false;

        return await _filesControllerHelperInt.UploadFileAsync(await _globalFolderHelper.FolderCommonAsync, inDto);
    }

    /// <summary>
    /// Uploads the file specified with single file upload or standart multipart/form-data method to 'My Documents' section
    /// </summary>
    /// <short>Upload to My</short>
    /// <category>Uploads</category>
    /// <remarks>
    /// <![CDATA[
    ///  Upload can be done in 2 different ways:
    ///  <ol>
    /// <li>Single file upload. You should set Content-Type &amp; Content-Disposition header to specify filename and content type, and send file in request body</li>
    /// <li>Using standart multipart/form-data method</li>
    /// </ol>]]>
    /// </remarks>
    /// <param name="file" visible="false">Request Input stream</param>
    /// <param name="contentType" visible="false">Content-Type Header</param>
    /// <param name="contentDisposition" visible="false">Content-Disposition Header</param>
    /// <param name="files" visible="false">List of files when posted as multipart/form-data</param>
    /// <returns>Uploaded file</returns>
    [Create("@my/upload")]
    public Task<object> UploadFileToMyAsync([ModelBinder(BinderType = typeof(UploadModelBinder))] UploadRequestDto inDto)
    {
        inDto.CreateNewIfExist = false;

        return _filesControllerHelperInt.UploadFileAsync(_globalFolderHelper.FolderMy, inDto);
    }
}
