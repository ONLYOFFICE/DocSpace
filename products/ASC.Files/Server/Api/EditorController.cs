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

    /// <summary>
    /// Opens a third-party file with the ID specified in the request for editing.
    /// </summary>
    /// <short>
    /// Open a third-party file
    /// </short>
    /// <category>Third-party integration</category>
    /// <param type="System.String, System" method="url" name="fileId">File ID</param>
    /// <returns type="ASC.Web.Files.Services.DocumentService.Configuration, ASC.Files.Core">Configuration parameters</returns>
    /// <path>api/2.0/files/file/app-{fileId}/openedit</path>
    /// <httpMethod>GET</httpMethod>
    /// <requiresAuthorization>false</requiresAuthorization>
    [AllowAnonymous]
    [AllowNotPayment]
    [HttpGet("file/app-{fileId}/openedit")]
    public async Task<Configuration<string>> OpenEditThirdPartyAsync(string fileId)
    {
        fileId = "app-" + fileId;
        var app = _thirdPartySelector.GetAppByFileId(fileId?.ToString());
        (var file, var editable) = await app.GetFileAsync(fileId?.ToString());
        var docParams = await _documentServiceHelper.GetParamsAsync(file, true, editable ? FileShare.ReadWrite : FileShare.Read, false, editable, editable, editable, false);
        var configuration = docParams.Configuration;
        configuration.Document.Url = app.GetFileStreamUrl(file);
        configuration.Document.Info.Favorite = null;
        configuration.EditorConfig.Customization.GobackUrl = string.Empty;
        configuration.EditorType = EditorType.External;

        if (file.RootFolderType == FolderType.Privacy && await PrivacyRoomSettings.GetEnabledAsync(_settingsManager) || docParams.LocatedInPrivateRoom)
        {
            var keyPair = await _encryptionKeyPairDtoHelper.GetKeyPairAsync();
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
    /// Saves edits to a file with the ID specified in the request.
    /// </summary>
    /// <short>Save file edits</short>
    /// <param type="System.Int32, System" method="url" name="fileId">File ID</param>
    /// <param type="ASC.Files.Core.ApiModels.RequestDto.SaveEditingRequestDto, ASC.Files.Core" name="inDto">Request parameters for saving file edits</param>
    /// <category>Files</category>
    /// <returns type="ASC.Files.Core.ApiModels.ResponseDto.FileDto, ASC.Files.Core">Saved file parameters</returns>
    /// <path>api/2.0/files/file/{fileId}/saveediting</path>
    /// <httpMethod>PUT</httpMethod>
    [HttpPut("file/{fileId}/saveediting")]
    public async Task<FileDto<T>> SaveEditingFromFormAsync(T fileId, [FromForm] SaveEditingRequestDto inDto)
    {
        await using var stream = _httpContextAccessor.HttpContext.Request.Body;

        return await _fileDtoHelper.GetAsync(await _fileStorageService.SaveEditingAsync(fileId, inDto.FileExtension, inDto.DownloadUri, stream, inDto.Doc, inDto.Forcesave));
    }

    /// <summary>
    /// Informs about opening a file with the ID specified in the request for editing, locking it from being deleted or moved (this method is called by the mobile editors).
    /// </summary>
    /// <short>Start file editing</short>
    /// <param type="System.Int32, System" method="url" name="fileId">File ID</param>
    /// <param type="ASC.Files.Core.ApiModels.RequestDto.StartEditRequestDto, ASC.Files.Core" name="inDto">Request parameters for starting file editing</param>
    /// <category>Files</category>
    /// <returns type="System.Object, System">File key for Document Service</returns>
    /// <path>api/2.0/files/file/{fileId}/startedit</path>
    /// <httpMethod>POST</httpMethod>
    [HttpPost("file/{fileId}/startedit")]
    public async Task<object> StartEditAsync(T fileId, StartEditRequestDto inDto)
    {
        return await _fileStorageService.StartEditAsync(fileId, inDto.EditingAlone, inDto.Doc);
    }

    /// <summary>
    /// Tracks file changes when editing.
    /// </summary>
    /// <short>Track file editing</short>
    /// <param type="System.Int32, System" method="url" name="fileId">File ID</param>
    /// <param type="System.Guid, System" name="tabId">Tab ID</param>
    /// <param type="System.String, System" name="docKeyForTrack">Document key for tracking</param>
    /// <param type="System.String, System" name="doc">Shared token</param>
    /// <param type="System.Boolean, System" name="isFinish">Specifies whether to finish file tracking or not</param>
    /// <category>Files</category>
    /// <returns type="System.Collections.Generic.KeyValuePair{System.Boolean, System.String}, System.Collections.Generic">File changes</returns>
    /// <path>api/2.0/files/file/{fileId}/trackeditfile</path>
    /// <httpMethod>GET</httpMethod>
    [HttpGet("file/{fileId}/trackeditfile")]
    public async Task<KeyValuePair<bool, string>> TrackEditFileAsync(T fileId, Guid tabId, string docKeyForTrack, string doc, bool isFinish)
    {
        return await _fileStorageService.TrackEditFileAsync(fileId, tabId, docKeyForTrack, doc, isFinish);
    }

    /// <summary>
    /// Returns the initialization configuration of a file to open it in the editor.
    /// </summary>
    /// <short>Open a file</short>
    /// <param type="System.Int32, System" method="url" name="fileId">File ID</param>
    /// <param type="System.Int32, System" name="version">File version</param>
    /// <param type="System.String, System" name="doc">Shared token</param>
    /// <param type="System.Boolean, System" name="view">Specifies if a document will be opened for viewing only or not</param>
    /// <category>Files</category>
    /// <returns type="ASC.Files.Core.ApiModels.ResponseDto.ConfigurationDto, ASC.Files.Core">Configuration parameters</returns>
    /// <path>api/2.0/files/file/{fileId}/openedit</path>
    /// <requiresAuthorization>false</requiresAuthorization>
    /// <httpMethod>GET</httpMethod>
    [AllowAnonymous]
    [AllowNotPayment]
    [HttpGet("file/{fileId}/openedit")]
    public async Task<ConfigurationDto<T>> OpenEditAsync(T fileId, int version, string doc, bool view)
    {
        var docParams = await _documentServiceHelper.GetParamsAsync(fileId, version, doc, true, !view, true);
        var configuration = docParams.Configuration;
        var file = docParams.File;
        configuration.EditorType = EditorType.External;

        if (file.RootFolderType == FolderType.Privacy && await PrivacyRoomSettings.GetEnabledAsync(_settingsManager) || docParams.LocatedInPrivateRoom)
        {
            var keyPair = await _encryptionKeyPairDtoHelper.GetKeyPairAsync();
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

    /// <summary>
    /// Returns a link to download a file with the ID specified in the request asynchronously.
    /// </summary>
    /// <short>Get file download link asynchronously</short>
    /// <category>Files</category>
    /// <param type="System.Int32, System" method="url" name="fileId">File ID</param>
    /// <returns type="ASC.Files.Core.Helpers.DocumentService.FileLink, ASC.Files.Core">File download link</returns>
    /// <path>api/2.0/files/file/{fileId}/presigned</path>
    /// <httpMethod>GET</httpMethod>
    [HttpGet("file/{fileId}/presigned")]
    public async Task<DocumentService.FileLink> GetPresignedUriAsync(T fileId)
    {
        return await _fileStorageService.GetPresignedUriAsync(fileId);
    }

    /// <summary>
    /// Returns a list of users with their access rights to the file with the ID specified in the request.
    /// </summary>
    /// <short>Get shared users</short>
    /// <category>Sharing</category>
    /// <param type="System.Int32, System" method="url" name="fileId">File ID</param>
    /// <returns type="ASC.Web.Files.Services.WCFService.MentionWrapper, ASC.Files.Core">List of users with their access rights to the file</returns>
    /// <path>api/2.0/files/file/{fileId}/sharedusers</path>
    /// <httpMethod>GET</httpMethod>
    /// <collection>list</collection>
    [HttpGet("file/{fileId}/sharedusers")]
    public async Task<List<MentionWrapper>> SharedUsers(T fileId)
    {
        return await _fileStorageService.SharedUsersAsync(fileId);
    }

    /// <summary>
    /// Returns the reference data to uniquely identify a file in its system and check the availability of insering data into the destination spreadsheet by the external link.
    /// </summary>
    /// <short>Get reference data</short>
    /// <category>Files</category>
    /// <param type="ASC.Files.Core.ApiModels.RequestDto.GetReferenceDataDto, ASC.Files.Core" name="inDto">Request parameters for getting reference data</param>
    /// <returns type="ASC.Web.Files.Services.DocumentService.FileReference, ASC.Files.Core">File reference data</returns>
    /// <path>api/2.0/files/file/referencedata</path>
    /// <httpMethod>POST</httpMethod>
    [HttpPost("file/referencedata")]
    public async Task<FileReference<T>> GetReferenceDataAsync(GetReferenceDataDto<T> inDto)
    {
        return await _fileStorageService.GetReferenceDataAsync(inDto.FileKey, inDto.InstanceId, inDto.SourceFileId, inDto.Path);
    }

    /// <summary>
    /// 
    /// </summary>
    /// <short>Get protect users</short>
    /// <category>Files</category>
    /// <param type="System.Int32, System" name="fileId"></param>
    /// <returns type="ASC.Web.Files.Services.WCFService.MentionWrapper, ASC.Files.Core">List of users</returns>
    /// <path>api/2.0/files/file/{fileId}/protectusers</path>
    /// <httpMethod>GET</httpMethod>
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
    /// Checks the document service location.
    /// </summary>
    /// <short>Check the document service URL</short>
    /// <param type="ASC.Files.Core.ApiModels.RequestDto.CheckDocServiceUrlRequestDto, ASC.Files.Core" name="inDto">Request parameters for checking the document service location</param>
    /// <category>Settings</category>
    /// <returns type="System.String, System">Document service information: the Document Server address, the Document Server address in the local private network, the Community Server address</returns>
    /// <path>api/2.0/files/docservice</path>
    /// <httpMethod>PUT</httpMethod>
    /// <collection>list</collection>
    [HttpPut("docservice")]
    public async Task<IEnumerable<string>> CheckDocServiceUrl(CheckDocServiceUrlRequestDto inDto)
    {
        _filesLinkUtility.DocServiceUrl = inDto.DocServiceUrl;
        _filesLinkUtility.DocServiceUrlInternal = inDto.DocServiceUrlInternal;
        _filesLinkUtility.DocServicePortalUrl = inDto.DocServiceUrlPortal;

        await _messageService.SendAsync(MessageAction.DocumentServiceLocationSetting);

        var https = new Regex(@"^https://", RegexOptions.IgnoreCase);
        var http = new Regex(@"^http://", RegexOptions.IgnoreCase);
        if (https.IsMatch(_commonLinkUtility.GetFullAbsolutePath("")) && http.IsMatch(_filesLinkUtility.DocServiceUrl))
        {
            throw new Exception("Mixed Active Content is not allowed. HTTPS address for Document Server is required.");
        }

        await _documentServiceConnector.CheckDocServiceUrlAsync();

        return new[]
        {
            _filesLinkUtility.DocServiceUrl,
            _filesLinkUtility.DocServiceUrlInternal,
            _filesLinkUtility.DocServicePortalUrl
        };
    }

    /// <summary>
    /// Returns the address of the connected editors.
    /// </summary>
    /// <short>Get the document service URL</short>
    /// <category>Settings</category>
    /// <param type="System.Boolean, System" name="version">Specifies the editor version or not</param>
    /// <returns type="System.Object, System">The document service URL with the editor version specified</returns>
    /// <path>api/2.0/files/docservice</path>
    /// <httpMethod>GET</httpMethod>
    /// <requiresAuthorization>false</requiresAuthorization>
    /// <visible>false</visible>
    [AllowAnonymous]
    [HttpGet("docservice")]
    public async Task<object> GetDocServiceUrlAsync(bool version)
    {
        var url = _commonLinkUtility.GetFullAbsolutePath(_filesLinkUtility.DocServiceApiUrl);

        var dsVersion = "";

        if (version)
        {
            dsVersion = await _documentServiceConnector.GetVersionAsync();
        }

        return new
        {
            version = dsVersion,
            docServiceUrlApi = url,
            _filesLinkUtility.DocServiceUrl,
            _filesLinkUtility.DocServiceUrlInternal,
            _filesLinkUtility.DocServicePortalUrl,
            _filesLinkUtility.IsDefault
        };
    }
}