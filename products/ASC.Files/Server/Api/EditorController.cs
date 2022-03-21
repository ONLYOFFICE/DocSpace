// (c) Copyright Ascensio System SIA 2010-2022
//
// This program is a free software product.
// You can redistribute it and/or modify it under the terms
// of the GNU Affero General Public License (AGPL) version 3 as published by the Free Software
// Foundation. In accordance with Section 7(a) of the GNU AGPL its Section 15 shall be amended
// to the effect that Ascensio System SIA expressly excludes the warranty of non-infringement of
// any third-party rights.
//
// This program is distributed WITHOUT ANY WARRANTY, without even the implied warranty
// of MERCHANTABILITY or FITNESS FOR A PARTICULAR  PURPOSE. For details, see
// the GNU AGPL at: http://www.gnu.org/licenses/agpl-3.0.html
//
// You can contact Ascensio System SIA at Lubanas st. 125a-25, Riga, Latvia, EU, LV-1021.
//
// The  interactive user interfaces in modified source and object code versions of the Program must
// display Appropriate Legal Notices, as required under Section 5 of the GNU AGPL version 3.
//
// Pursuant to Section 7(b) of the License you must retain the original Product logo when
// distributing the program. Pursuant to Section 7(e) we decline to grant you any rights under
// trademark law for use of our trademarks.
//
// All the Product's GUI elements, including illustrations and icon sets, as well as technical writing
// content are licensed under the terms of the Creative Commons Attribution-ShareAlike 4.0
// International. See the License terms at http://creativecommons.org/licenses/by-sa/4.0/legalcode

namespace ASC.Files.Api;

public class EditorController : ApiControllerBase
{
    private readonly FilesLinkUtility _filesLinkUtility;
    private readonly MessageService _messageService;
    private readonly DocumentServiceConnector _documentServiceConnector;
    private readonly CommonLinkUtility _commonLinkUtility;
    private readonly EditorControllerHelper<int> _editorControllerHelperInt;
    private readonly EditorControllerHelper<string> _editorControllerHelperString;

    public EditorController(
        FilesLinkUtility filesLinkUtility,
        MessageService messageService,
        DocumentServiceConnector documentServiceConnector,
        CommonLinkUtility commonLinkUtility,
        EditorControllerHelper<int> editorControllerHelperInt,
        EditorControllerHelper<string> editorControllerHelperString)
    {
        _filesLinkUtility = filesLinkUtility;
        _messageService = messageService;
        _documentServiceConnector = documentServiceConnector;
        _commonLinkUtility = commonLinkUtility;
        _editorControllerHelperInt = editorControllerHelperInt;
        _editorControllerHelperString = editorControllerHelperString;
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
    public Task<FileDto<string>> SaveEditingFromFormAsync(string fileId, [FromForm] SaveEditingRequestDto inDto)
    {
        using var stream = _editorControllerHelperInt.GetFileFromRequest(inDto).OpenReadStream();

        return _editorControllerHelperString.SaveEditingAsync(fileId, inDto.FileExtension, inDto.DownloadUri, stream, inDto.Doc, inDto.Forcesave);
    }

    [Update("file/{fileId:int}/saveediting")]
    public Task<FileDto<int>> SaveEditingFromFormAsync(int fileId, [FromForm] SaveEditingRequestDto inDto)
    {
        using var stream = _editorControllerHelperInt.GetFileFromRequest(inDto).OpenReadStream();

        return _editorControllerHelperInt.SaveEditingAsync(fileId, inDto.FileExtension, inDto.DownloadUri, stream, inDto.Doc, inDto.Forcesave);
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
    public async Task<object> StartEditFromBodyAsync(string fileId, [FromBody] StartEditRequestDto inDto)
    {
        return await _editorControllerHelperString.StartEditAsync(fileId, inDto.EditingAlone, inDto.Doc);
    }

    [Create("file/{fileId}/startedit")]
    [Consumes("application/x-www-form-urlencoded")]
    public async Task<object> StartEditFromFormAsync(string fileId, [FromForm] StartEditRequestDto inDto)
    {
        return await _editorControllerHelperString.StartEditAsync(fileId, inDto.EditingAlone, inDto.Doc);
    }

    [Create("file/{fileId:int}/startedit")]
    [Consumes("application/json")]
    public async Task<object> StartEditFromBodyAsync(int fileId, [FromBody] StartEditRequestDto inDto)
    {
        return await _editorControllerHelperInt.StartEditAsync(fileId, inDto.EditingAlone, inDto.Doc);
    }

    [Create("file/{fileId:int}/startedit")]
    public async Task<object> StartEditAsync(int fileId)
    {
        return await _editorControllerHelperInt.StartEditAsync(fileId, false, null);
    }

    [Create("file/{fileId:int}/startedit")]
    [Consumes("application/x-www-form-urlencoded")]
    public async Task<object> StartEditFromFormAsync(int fileId, [FromForm] StartEditRequestDto inDto)
    {
        return await _editorControllerHelperInt.StartEditAsync(fileId, inDto.EditingAlone, inDto.Doc);
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
        return _editorControllerHelperString.TrackEditFileAsync(fileId, tabId, docKeyForTrack, doc, isFinish);
    }

    [Read("file/{fileId:int}/trackeditfile")]
    public Task<KeyValuePair<bool, string>> TrackEditFileAsync(int fileId, Guid tabId, string docKeyForTrack, string doc, bool isFinish)
    {
        return _editorControllerHelperInt.TrackEditFileAsync(fileId, tabId, docKeyForTrack, doc, isFinish);
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
        return _editorControllerHelperString.OpenEditAsync(fileId, version, doc, view);
    }

    [AllowAnonymous]
    [Read("file/{fileId:int}/openedit", Check = false)]
    public Task<Configuration<int>> OpenEditAsync(int fileId, int version, string doc, bool view)
    {
        return _editorControllerHelperInt.OpenEditAsync(fileId, version, doc, view);
    }

    /// <summary>
    ///  Checking document service location
    /// </summary>
    /// <param name="docServiceUrl">Document editing service Domain</param>
    /// <param name="docServiceUrlInternal">Document command service Domain</param>
    /// <param name="docServiceUrlPortal">Community Server Address</param>
    /// <returns></returns>
    [Update("docservice")]
    public Task<IEnumerable<string>> CheckDocServiceUrlFromBodyAsync([FromBody] CheckDocServiceUrlRequestDto inDto)
    {
        return CheckDocServiceUrlAsync(inDto);
    }

    [Update("docservice")]
    [Consumes("application/x-www-form-urlencoded")]
    public Task<IEnumerable<string>> CheckDocServiceUrlFromFormAsync([FromForm] CheckDocServiceUrlRequestDto inDto)
    {
        return CheckDocServiceUrlAsync(inDto);
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
        return _editorControllerHelperString.GetPresignedUriAsync(fileId);
    }

    [Read("file/{fileId:int}/presigned")]
    public Task<DocumentService.FileLink> GetPresignedUriAsync(int fileId)
    {
        return _editorControllerHelperInt.GetPresignedUriAsync(fileId);
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

    private Task<IEnumerable<string>> CheckDocServiceUrlAsync(CheckDocServiceUrlRequestDto inDto)
    {
        _filesLinkUtility.DocServiceUrl = inDto.DocServiceUrl;
        _filesLinkUtility.DocServiceUrlInternal = inDto.DocServiceUrlInternal;
        _filesLinkUtility.DocServicePortalUrl = inDto.DocServiceUrlPortal;

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