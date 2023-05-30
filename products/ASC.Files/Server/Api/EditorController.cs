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
    public EditorControllerInternal(
        FileStorageService fileStorageService,
        DocumentServiceHelper documentServiceHelper,
        EncryptionKeyPairDtoHelper encryptionKeyPairDtoHelper,
        SettingsManager settingsManager,
        EntryManager entryManager,
        IHttpContextAccessor httpContextAccessor,
        IMapper mapper,
        CommonLinkUtility commonLinkUtility,
        FilesLinkUtility filesLinkUtility,
        FolderDtoHelper folderDtoHelper,
        FileDtoHelper fileDtoHelper)
        : base(fileStorageService, documentServiceHelper, encryptionKeyPairDtoHelper, settingsManager, entryManager, httpContextAccessor, mapper, commonLinkUtility, filesLinkUtility, folderDtoHelper, fileDtoHelper)
    {
    }
}

public class EditorControllerThirdparty : EditorController<string>
{
    private readonly ThirdPartySelector _thirdPartySelector;

    public EditorControllerThirdparty(
        FileStorageService fileStorageService,
        DocumentServiceHelper documentServiceHelper,
        EncryptionKeyPairDtoHelper encryptionKeyPairDtoHelper,
        SettingsManager settingsManager,
        EntryManager entryManager,
        IHttpContextAccessor httpContextAccessor,
        ThirdPartySelector thirdPartySelector,
        IMapper mapper,
        CommonLinkUtility commonLinkUtility,
        FilesLinkUtility filesLinkUtility,
        FolderDtoHelper folderDtoHelper,
        FileDtoHelper fileDtoHelper)
        : base(fileStorageService, documentServiceHelper, encryptionKeyPairDtoHelper, settingsManager, entryManager, httpContextAccessor, mapper, commonLinkUtility, filesLinkUtility, folderDtoHelper, fileDtoHelper)
    {
        _thirdPartySelector = thirdPartySelector;
    }

    [AllowAnonymous]
    [AllowNotPayment]
    [HttpGet("file/app-{fileId}/openedit")]
    public async Task<Configuration<string>> OpenEditThirdPartyAsync(string fileId)
    {
        fileId = "app-" + fileId;
        var app = _thirdPartySelector.GetAppByFileId(fileId?.ToString());
        bool editable;
        var file = app.GetFile(fileId?.ToString(), out editable);
        var docParams = await _documentServiceHelper.GetParamsAsync(file, true, editable ? FileShare.ReadWrite : FileShare.Read, false, editable, editable, editable, false);
        var configuration = docParams.Configuration;
        configuration.Document.Url = app.GetFileStreamUrl(file);
        configuration.Document.Info.Favorite = null;
        configuration.EditorConfig.Customization.GobackUrl = string.Empty;
        configuration.EditorType = EditorType.External;

        if (file.RootFolderType == FolderType.Privacy && PrivacyRoomSettings.GetEnabled(_settingsManager) || docParams.LocatedInPrivateRoom)
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

        if (!file.Encrypted && !file.ProviderEntry)
        {
            await _entryManager.MarkAsRecent(file);
        }

        configuration.Token = _documentServiceHelper.GetSignature(configuration);

        return configuration;
    }
}

public abstract class EditorController<T> : ApiControllerBase
{
    protected readonly FileStorageService _fileStorageService;
    protected readonly DocumentServiceHelper _documentServiceHelper;
    protected readonly EncryptionKeyPairDtoHelper _encryptionKeyPairDtoHelper;
    protected readonly SettingsManager _settingsManager;
    protected readonly EntryManager _entryManager;
    protected readonly IHttpContextAccessor _httpContextAccessor;
    private readonly IMapper _mapper;
    private readonly CommonLinkUtility _commonLinkUtility;
    private readonly FilesLinkUtility _filesLinkUtility;

    public EditorController(
        FileStorageService fileStorageService,
        DocumentServiceHelper documentServiceHelper,
        EncryptionKeyPairDtoHelper encryptionKeyPairDtoHelper,
        SettingsManager settingsManager,
        EntryManager entryManager,
        IHttpContextAccessor httpContextAccessor,
        IMapper mapper,
        CommonLinkUtility commonLinkUtility,
        FilesLinkUtility filesLinkUtility,
        FolderDtoHelper folderDtoHelper,
        FileDtoHelper fileDtoHelper) : base(folderDtoHelper, fileDtoHelper)
    {
        _fileStorageService = fileStorageService;
        _documentServiceHelper = documentServiceHelper;
        _encryptionKeyPairDtoHelper = encryptionKeyPairDtoHelper;
        _settingsManager = settingsManager;
        _entryManager = entryManager;
        _httpContextAccessor = httpContextAccessor;
        _mapper = mapper;
        _commonLinkUtility = commonLinkUtility;
        _filesLinkUtility = filesLinkUtility;
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
    [HttpPut("file/{fileId}/saveediting")]
    public async Task<FileDto<T>> SaveEditingFromFormAsync(T fileId, [FromForm] SaveEditingRequestDto inDto)
    {
        using var stream = _httpContextAccessor.HttpContext.Request.Body;

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
    [HttpPost("file/{fileId}/startedit")]
    public async Task<object> StartEditAsync(T fileId, StartEditRequestDto inDto)
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
    [HttpGet("file/{fileId}/trackeditfile")]
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
    [AllowNotPayment]
    [HttpGet("file/{fileId}/openedit")]
    public async Task<ConfigurationDto<T>> OpenEditAsync(T fileId, int version, string doc, bool view)
    {
        var docParams = await _documentServiceHelper.GetParamsAsync(fileId, version, doc, true, !view, true);
        var configuration = docParams.Configuration;
        var file = docParams.File;
        configuration.EditorType = EditorType.External;

        if (file.RootFolderType == FolderType.Privacy && PrivacyRoomSettings.GetEnabled(_settingsManager) || docParams.LocatedInPrivateRoom)
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

        if (!file.Encrypted && !file.ProviderEntry)
        {
            await _entryManager.MarkAsRecent(file);
        }

        configuration.Token = _documentServiceHelper.GetSignature(configuration);

        var result = _mapper.Map<Configuration<T>, ConfigurationDto<T>>(configuration);
        result.EditorUrl = _commonLinkUtility.GetFullAbsolutePath(_filesLinkUtility.DocServiceApiUrl);
        result.File = await _fileDtoHelper.GetAsync(file);
        return result;
    }

    [HttpGet("file/{fileId}/presigned")]
    public Task<DocumentService.FileLink> GetPresignedUriAsync(T fileId)
    {
        return _fileStorageService.GetPresignedUriAsync(fileId);
    }

    [HttpGet("file/{fileId}/sharedusers")]
    public Task<List<MentionWrapper>> SharedUsers(T fileId)
    {
        return _fileStorageService.SharedUsersAsync(fileId);
    }

    [HttpPost("file/referencedata")]
    public Task<FileReference<T>> GetReferenceDataAsync(GetReferenceDataDto<T> inDto)
    {

        return _fileStorageService.GetReferenceDataAsync(inDto.FileKey, inDto.InstanceId, inDto.SourceFileId, inDto.Path);
    }

    [HttpGet("file/{fileId}/protectusers")]
    public async Task<List<MentionWrapper>> ProtectUsers(T fileId)
    {
        return await _fileStorageService.ProtectUsersAsync(fileId);
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
        CommonLinkUtility commonLinkUtility,
        FolderDtoHelper folderDtoHelper,
        FileDtoHelper fileDtoHelper) : base(folderDtoHelper, fileDtoHelper)
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
    [HttpPut("docservice")]
    public Task<IEnumerable<string>> CheckDocServiceUrl(CheckDocServiceUrlRequestDto inDto)
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

    /// <visible>false</visible>
    [AllowAnonymous]
    [HttpGet("docservice")]
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