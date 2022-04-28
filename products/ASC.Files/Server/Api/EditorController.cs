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

[ConstraintRoute("int")]
public class EditorControllerInternal : EditorController<int>
{
    public EditorControllerInternal(FileStorageService<int> fileStorageService, FileDtoHelper fileDtoHelper, DocumentServiceHelper documentServiceHelper, DocumentServiceTrackerHelper documentServiceTrackerHelper, EncryptionKeyPairDtoHelper encryptionKeyPairDtoHelper, SettingsManager settingsManager, EntryManager entryManager, IHttpContextAccessor httpContextAccessor) : base(fileStorageService, fileDtoHelper, documentServiceHelper, documentServiceTrackerHelper, encryptionKeyPairDtoHelper, settingsManager, entryManager, httpContextAccessor)
    {
    }
}

public class EditorControllerThirdparty : EditorController<string>
{
    public EditorControllerThirdparty(FileStorageService<string> fileStorageService, FileDtoHelper fileDtoHelper, DocumentServiceHelper documentServiceHelper, DocumentServiceTrackerHelper documentServiceTrackerHelper, EncryptionKeyPairDtoHelper encryptionKeyPairDtoHelper, SettingsManager settingsManager, EntryManager entryManager, IHttpContextAccessor httpContextAccessor) : base(fileStorageService, fileDtoHelper, documentServiceHelper, documentServiceTrackerHelper, encryptionKeyPairDtoHelper, settingsManager, entryManager, httpContextAccessor)
    {
    }
}

public abstract class EditorController<T> : ApiControllerBase
{
    private readonly FileStorageService<T> _fileStorageService;
    private readonly FileDtoHelper _fileDtoHelper;
    private readonly DocumentServiceHelper _documentServiceHelper;
    private readonly DocumentServiceTrackerHelper _documentServiceTrackerHelper;
    private readonly EncryptionKeyPairDtoHelper _encryptionKeyPairDtoHelper;
    private readonly SettingsManager _settingsManager;
    private readonly EntryManager _entryManager;
    private readonly IHttpContextAccessor _httpContextAccessor;

    public EditorController(
        FileStorageService<T> fileStorageService,
        FileDtoHelper fileDtoHelper,
        DocumentServiceHelper documentServiceHelper,
        DocumentServiceTrackerHelper documentServiceTrackerHelper,
        EncryptionKeyPairDtoHelper encryptionKeyPairDtoHelper,
        SettingsManager settingsManager,
        EntryManager entryManager,
        IHttpContextAccessor httpContextAccessor)
    {
        _fileStorageService = fileStorageService;
        _fileDtoHelper = fileDtoHelper;
        _documentServiceHelper = documentServiceHelper;
        _documentServiceTrackerHelper = documentServiceTrackerHelper;
        _encryptionKeyPairDtoHelper = encryptionKeyPairDtoHelper;
        _settingsManager = settingsManager;
        _entryManager = entryManager;
        _httpContextAccessor = httpContextAccessor;
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
    public async Task<FileDto<T>> SaveEditingFromFormAsync(T fileId, [FromForm] SaveEditingRequestDto inDto)
    {
        var file = inDto.File;
        IEnumerable<IFormFile> files = _httpContextAccessor.HttpContext.Request.Form.Files;
        if (files != null && files.Any())
        {
            file = files.First();
        }

        using var stream = file.OpenReadStream();

        return await _fileDtoHelper.GetAsync(await _fileStorageService.SaveEditingAsync(fileId, inDto.FileExtension, inDto.DownloadUri, stream, inDto.Doc, inDto.Forcesave));
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
    public async Task<object> StartEditFromBodyAsync(T fileId, [FromBody] StartEditRequestDto inDto)
    {
        return await _fileStorageService.StartEditAsync(fileId, inDto.EditingAlone, inDto.Doc);
    }

    [Create("file/{fileId}/startedit")]
    [Consumes("application/x-www-form-urlencoded")]
    public async Task<object> StartEditFromFormAsync(T fileId, [FromForm] StartEditRequestDto inDto)
    {
        return await _fileStorageService.StartEditAsync(fileId, inDto.EditingAlone, inDto.Doc);
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
    public Task<KeyValuePair<bool, string>> TrackEditFileAsync(T fileId, Guid tabId, string docKeyForTrack, string doc, bool isFinish)
    {
        return _fileStorageService.TrackEditFileAsync(fileId, tabId, docKeyForTrack, doc, isFinish);
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
    public async Task<Configuration<T>> OpenEditAsync(T fileId, int version, string doc, bool view)
    {
        var docParams = await _documentServiceHelper.GetParamsAsync(fileId, version, doc, true, !view, true);
        var configuration = docParams.Configuration;

        configuration.EditorType = EditorType.External;
        if (configuration.EditorConfig.ModeWrite)
        {
            configuration.EditorConfig.CallbackUrl = _documentServiceTrackerHelper.GetCallbackUrl(configuration.Document.Info.GetFile().Id.ToString());
        }

        if (configuration.Document.Info.GetFile().RootFolderType == FolderType.Privacy && PrivacyRoomSettings.GetEnabled(_settingsManager))
        {
            var keyPair = _encryptionKeyPairDtoHelper.GetKeyPair();
            if (keyPair != null)
            {
                configuration.EditorConfig.EncryptionKeys = new EncryptionKeysConfig
                {
                    PrivateKeyEnc = keyPair.PrivateKeyEnc,
                    PublicKey = keyPair.PublicKey,
                };
            }
        }

        if (!configuration.Document.Info.GetFile().Encrypted && !configuration.Document.Info.GetFile().ProviderEntry)
        {
            _entryManager.MarkAsRecent(configuration.Document.Info.GetFile());
        }

        configuration.Token = _documentServiceHelper.GetSignature(configuration);

        return configuration;
    }

    [Read("file/{fileId}/presigned")]
    public Task<DocumentService.FileLink> GetPresignedUriAsync(T fileId)
    {
        return _fileStorageService.GetPresignedUriAsync(fileId);
    }
}

public class EditorController : ApiControllerBase
{
    private readonly FilesLinkUtility _filesLinkUtility;
    private readonly MessageService _messageService;
    private readonly DocumentServiceConnector _documentServiceConnector;
    private readonly CommonLinkUtility _commonLinkUtility;

    public EditorController(
        FilesLinkUtility filesLinkUtility,
        MessageService messageService,
        DocumentServiceConnector documentServiceConnector,
        CommonLinkUtility commonLinkUtility)
    {
        _filesLinkUtility = filesLinkUtility;
        _messageService = messageService;
        _documentServiceConnector = documentServiceConnector;
        _commonLinkUtility = commonLinkUtility;
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