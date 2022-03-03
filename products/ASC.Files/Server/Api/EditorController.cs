namespace ASC.Files.Api;

public class EditorController : ApiControllerBase
{
    private readonly FilesLinkUtility _filesLinkUtility;
    private readonly MessageService _messageService;
    private readonly DocumentServiceConnector _documentServiceConnector;
    private readonly CommonLinkUtility _commonLinkUtility;

    public EditorController(
        FilesControllerHelper<int> filesControllerHelperInt,
        FilesControllerHelper<string> filesControllerHelperString,
        FilesLinkUtility filesLinkUtility,
        MessageService messageService,
        DocumentServiceConnector documentServiceConnector,
        CommonLinkUtility commonLinkUtility) 
        : base(
            filesControllerHelperInt, 
            filesControllerHelperString)
    {
        _filesLinkUtility = filesLinkUtility;
        _messageService = messageService;
        _documentServiceConnector = documentServiceConnector;
        _commonLinkUtility = commonLinkUtility;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="fileId">File ID</param>
    /// <param name="fileExtension"></param>
    /// <param name="downloadUri"></param>
    /// <param name="stream"></param>
    /// <param name="doc"></param>
    /// <param name="forcesave"></param>
    /// <category>Files</category>
    /// <returns></returns>
    [Update("file/{fileId}/saveediting")]
    public Task<FileWrapper<string>> SaveEditingFromFormAsync(string fileId, [FromForm] SaveEditingModel model)
    {
        using var stream = _filesControllerHelperInt.GetFileFromRequest(model).OpenReadStream();

        return _filesControllerHelperString.SaveEditingAsync(fileId, model.FileExtension, model.DownloadUri, stream, model.Doc, model.Forcesave);
    }

    [Update("file/{fileId:int}/saveediting")]
    public Task<FileWrapper<int>> SaveEditingFromFormAsync(int fileId, [FromForm] SaveEditingModel model)
    {
        using var stream = _filesControllerHelperInt.GetFileFromRequest(model).OpenReadStream();

        return _filesControllerHelperInt.SaveEditingAsync(fileId, model.FileExtension, model.DownloadUri, stream, model.Doc, model.Forcesave);
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="fileId">File ID</param>
    /// <param name="editingAlone"></param>
    /// <param name="doc"></param>
    /// <category>Files</category>
    /// <returns></returns>
    [Create("file/{fileId}/startedit")]
    [Consumes("application/json")]
    public async Task<object> StartEditFromBodyAsync(string fileId, [FromBody] StartEditModel model)
    {
        return await _filesControllerHelperString.StartEditAsync(fileId, model.EditingAlone, model.Doc);
    }

    [Create("file/{fileId}/startedit")]
    [Consumes("application/x-www-form-urlencoded")]
    public async Task<object> StartEditFromFormAsync(string fileId, [FromForm] StartEditModel model)
    {
        return await _filesControllerHelperString.StartEditAsync(fileId, model.EditingAlone, model.Doc);
    }

    [Create("file/{fileId:int}/startedit")]
    [Consumes("application/json")]
    public async Task<object> StartEditFromBodyAsync(int fileId, [FromBody] StartEditModel model)
    {
        return await _filesControllerHelperInt.StartEditAsync(fileId, model.EditingAlone, model.Doc);
    }

    [Create("file/{fileId:int}/startedit")]
    public async Task<object> StartEditAsync(int fileId)
    {
        return await _filesControllerHelperInt.StartEditAsync(fileId, false, null);
    }

    [Create("file/{fileId:int}/startedit")]
    [Consumes("application/x-www-form-urlencoded")]
    public async Task<object> StartEditFromFormAsync(int fileId, [FromForm] StartEditModel model)
    {
        return await _filesControllerHelperInt.StartEditAsync(fileId, model.EditingAlone, model.Doc);
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="fileId">File ID</param>
    /// <param name="tabId"></param>
    /// <param name="docKeyForTrack"></param>
    /// <param name="doc"></param>
    /// <param name="isFinish"></param>
    /// <category>Files</category>
    /// <returns></returns>
    [Read("file/{fileId}/trackeditfile")]
    public Task<KeyValuePair<bool, string>> TrackEditFileAsync(string fileId, Guid tabId, string docKeyForTrack, string doc, bool isFinish)
    {
        return _filesControllerHelperString.TrackEditFileAsync(fileId, tabId, docKeyForTrack, doc, isFinish);
    }

    [Read("file/{fileId:int}/trackeditfile")]
    public Task<KeyValuePair<bool, string>> TrackEditFileAsync(int fileId, Guid tabId, string docKeyForTrack, string doc, bool isFinish)
    {
        return _filesControllerHelperInt.TrackEditFileAsync(fileId, tabId, docKeyForTrack, doc, isFinish);
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="fileId">File ID</param>
    /// <param name="version"></param>
    /// <param name="doc"></param>
    /// <category>Files</category>
    /// <returns></returns>
    [AllowAnonymous]
    [Read("file/{fileId}/openedit", Check = false)]
    public Task<Configuration<string>> OpenEditAsync(string fileId, int version, string doc, bool view)
    {
        return _filesControllerHelperString.OpenEditAsync(fileId, version, doc, view);
    }

    [AllowAnonymous]
    [Read("file/{fileId:int}/openedit", Check = false)]
    public Task<Configuration<int>> OpenEditAsync(int fileId, int version, string doc, bool view)
    {
        return _filesControllerHelperInt.OpenEditAsync(fileId, version, doc, view);
    }

    /// <summary>
    ///  Checking document service location
    /// </summary>
    /// <param name="docServiceUrl">Document editing service Domain</param>
    /// <param name="docServiceUrlInternal">Document command service Domain</param>
    /// <param name="docServiceUrlPortal">Community Server Address</param>
    /// <returns></returns>
    [Update("docservice")]
    public Task<IEnumerable<string>> CheckDocServiceUrlFromBodyAsync([FromBody] CheckDocServiceUrlModel model)
    {
        return CheckDocServiceUrlAsync(model);
    }

    [Update("docservice")]
    [Consumes("application/x-www-form-urlencoded")]
    public Task<IEnumerable<string>> CheckDocServiceUrlFromFormAsync([FromForm] CheckDocServiceUrlModel model)
    {
        return CheckDocServiceUrlAsync(model);
    }

    /// <visible>false</visible>
    [AllowAnonymous]
    [Read("docservice")]
    public Task<object> GetDocServiceUrlAsync(bool version)
    {
        var url = _commonLinkUtility.GetFullAbsolutePath(_filesLinkUtility.DocServiceApiUrl);
        if (!version)
        {
            return Task.FromResult<object>(url);
        }

        return InternalGetDocServiceUrlAsync(url);
    }

    [Read("file/{fileId}/presigned")]
    public Task<DocumentService.FileLink> GetPresignedUriAsync(string fileId)
    {
        return _filesControllerHelperString.GetPresignedUriAsync(fileId);
    }

    [Read("file/{fileId:int}/presigned")]
    public Task<DocumentService.FileLink> GetPresignedUriAsync(int fileId)
    {
        return _filesControllerHelperInt.GetPresignedUriAsync(fileId);
    }

    private async Task<object> InternalGetDocServiceUrlAsync(string url)
    {
        var dsVersion = await _documentServiceConnector.GetVersionAsync();

        return new
        {
            version = dsVersion,
            docServiceUrlApi = url,
        };
    }

    private Task<IEnumerable<string>> CheckDocServiceUrlAsync(CheckDocServiceUrlModel model)
    {
        _filesLinkUtility.DocServiceUrl = model.DocServiceUrl;
        _filesLinkUtility.DocServiceUrlInternal = model.DocServiceUrlInternal;
        _filesLinkUtility.DocServicePortalUrl = model.DocServiceUrlPortal;

        _messageService.Send(MessageAction.DocumentServiceLocationSetting);

        var https = new Regex(@"^https://", RegexOptions.IgnoreCase);
        var http = new Regex(@"^http://", RegexOptions.IgnoreCase);
        if (https.IsMatch(_commonLinkUtility.GetFullAbsolutePath("")) && http.IsMatch(_filesLinkUtility.DocServiceUrl))
        {
            throw new Exception("Mixed Active Content is not allowed. HTTPS address for Document Server is required.");
        }

        return InternalCheckDocServiceUrlAsync();
    }

    private async Task<IEnumerable<string>> InternalCheckDocServiceUrlAsync()
    {
        await _documentServiceConnector.CheckDocServiceUrlAsync();

        return new[]
        {
                    _filesLinkUtility.DocServiceUrl,
                    _filesLinkUtility.DocServiceUrlInternal,
                    _filesLinkUtility.DocServicePortalUrl
        };
    }
}