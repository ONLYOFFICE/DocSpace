namespace ASC.Files.Helpers;

public class UploadControllerHelper<T> : FilesHelperBase<T>
{
    private readonly FilesLinkUtility _filesLinkUtility;
    private readonly ChunkedUploadSessionHelper _chunkedUploadSessionHelper;
    private readonly TenantManager _tenantManager;
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly SecurityContext _securityContext;

    public UploadControllerHelper(
        FilesSettingsHelper filesSettingsHelper,
        FileUploader fileUploader,
        SocketManager socketManager,
        FileDtoHelper fileDtoHelper,
        ApiContext apiContext,
        FileStorageService<T> fileStorageService,
        FolderContentDtoHelper folderContentDtoHelper,
        IHttpContextAccessor httpContextAccessor,
        FolderDtoHelper folderDtoHelper,
        FilesLinkUtility filesLinkUtility,
        ChunkedUploadSessionHelper chunkedUploadSessionHelper,
        TenantManager tenantManager,
        IHttpClientFactory httpClientFactory,
        SecurityContext securityContext) 
        : base(
            filesSettingsHelper,
            fileUploader,
            socketManager,
            fileDtoHelper,
            apiContext,
            fileStorageService,
            folderContentDtoHelper,
            httpContextAccessor,
            folderDtoHelper)
    {
        _filesLinkUtility = filesLinkUtility;
        _chunkedUploadSessionHelper = chunkedUploadSessionHelper;
        _tenantManager = tenantManager;
        _httpClientFactory = httpClientFactory;
        _securityContext = securityContext;
    }

    public async Task<object> CreateUploadSessionAsync(T folderId, string fileName, long fileSize, string relativePath, ApiDateTime lastModified, bool encrypted)
    {
        var file = await _fileUploader.VerifyChunkedUploadAsync(folderId, fileName, fileSize, _filesSettingsHelper.UpdateIfExist, lastModified, relativePath);

        if (_filesLinkUtility.IsLocalFileUploader)
        {
            var session = await _fileUploader.InitiateUploadAsync(file.ParentId, file.Id ?? default, file.Title, file.ContentLength, encrypted);

            var responseObject = await _chunkedUploadSessionHelper.ToResponseObjectAsync(session, true);

            return new
            {
                success = true,
                data = responseObject
            };
        }

        var createSessionUrl = _filesLinkUtility.GetInitiateUploadSessionUrl(_tenantManager.GetCurrentTenant().Id, file.ParentId, file.Id, file.Title, file.ContentLength, encrypted, _securityContext);

        var httpClient = _httpClientFactory.CreateClient();

        var request = new HttpRequestMessage();
        request.RequestUri = new Uri(createSessionUrl);
        request.Method = HttpMethod.Post;

        // hack for uploader.onlyoffice.com in api requests
        var rewriterHeader = _apiContext.HttpContextAccessor.HttpContext.Request.Headers[HttpRequestExtensions.UrlRewriterHeader];
        if (!string.IsNullOrEmpty(rewriterHeader))
        {
            request.Headers.Add(HttpRequestExtensions.UrlRewriterHeader, rewriterHeader.ToString());
        }

        using var response = await httpClient.SendAsync(request);
        using var responseStream = await response.Content.ReadAsStreamAsync();
        using var streamReader = new StreamReader(responseStream);

        return JObject.Parse(await streamReader.ReadToEndAsync()); //result is json string
    }

    public async Task<object> UploadFileAsync(T folderId, UploadRequestDto uploadModel)
    {
        if (uploadModel.StoreOriginalFileFlag.HasValue)
        {
            _filesSettingsHelper.StoreOriginalFiles = uploadModel.StoreOriginalFileFlag.Value;
        }

        IEnumerable<IFormFile> files = _httpContextAccessor.HttpContext.Request.Form.Files;
        if (files == null || !files.Any())
        {
            files = uploadModel.Files;
        }

        if (files != null && files.Any())
        {
            if (files.Count() == 1)
            {
                //Only one file. return it
                var postedFile = files.First();

                return await InsertFileAsync(folderId, postedFile.OpenReadStream(), postedFile.FileName, uploadModel.CreateNewIfExist, uploadModel.KeepConvertStatus);
            }

            //For case with multiple files
            var result = new List<object>();

            foreach (var postedFile in uploadModel.Files)
            {
                result.Add(await InsertFileAsync(folderId, postedFile.OpenReadStream(), postedFile.FileName, uploadModel.CreateNewIfExist, uploadModel.KeepConvertStatus));
            }

            return result;
        }

        if (uploadModel.File != null)
        {
            var fileName = "file" + MimeMapping.GetExtention(uploadModel.ContentType.MediaType);
            if (uploadModel.ContentDisposition != null)
            {
                fileName = uploadModel.ContentDisposition.FileName;
            }

            return new List<FileDto<T>>
            {
                await InsertFileAsync(folderId, uploadModel.File.OpenReadStream(), fileName, uploadModel.CreateNewIfExist, uploadModel.KeepConvertStatus)
            };
        }

        throw new InvalidOperationException("No input files");
    }
}
