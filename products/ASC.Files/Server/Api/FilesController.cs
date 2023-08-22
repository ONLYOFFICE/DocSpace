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
public class FilesControllerInternal : FilesController<int>
{
    public FilesControllerInternal(
        FilesControllerHelper filesControllerHelper,
        FileStorageService fileStorageService,
        IMapper mapper,
        FileOperationDtoHelper fileOperationDtoHelper,
        FolderDtoHelper folderDtoHelper,
        FileDtoHelper fileDtoHelper)
        : base(filesControllerHelper, fileStorageService, mapper, fileOperationDtoHelper, folderDtoHelper, fileDtoHelper)
    {
    }
}

public class FilesControllerThirdparty : FilesController<string>
{
    private readonly ThirdPartySelector _thirdPartySelector;
    private readonly DocumentServiceHelper _documentServiceHelper;

    public FilesControllerThirdparty(
        FilesControllerHelper filesControllerHelper,
        FileStorageService fileStorageService,
        ThirdPartySelector thirdPartySelector,
        DocumentServiceHelper documentServiceHelper,
        IMapper mapper,
        FileOperationDtoHelper fileOperationDtoHelper,
        FolderDtoHelper folderDtoHelper,
        FileDtoHelper fileDtoHelper)
        : base(filesControllerHelper, fileStorageService, mapper, fileOperationDtoHelper, folderDtoHelper, fileDtoHelper)
    {
        _thirdPartySelector = thirdPartySelector;
        _documentServiceHelper = documentServiceHelper;
    }

    /// <summary>
    /// Returns the detailed information about a third-party file with the ID specified in the request.
    /// </summary>
    /// <short>Get third-party file information</short>
    /// <param type="System.String, System" method="url" name="fileId">File ID</param>
    /// <category>Files</category>
    /// <returns type="ASC.Files.Core.ApiModels.ResponseDto.FileEntryDto, ASC.Files.Core">File entry information</returns>
    /// <path>api/2.0/files/file/app-{fileId}</path>
    /// <httpMethod>GET</httpMethod>
    [HttpGet("file/app-{fileId}", Order = 1)]
    public async Task<FileEntryDto> GetFileInfoThirdPartyAsync(string fileId)
    {
        fileId = "app-" + fileId;
        var app = _thirdPartySelector.GetAppByFileId(fileId?.ToString());
        (var file, var editable) = await app.GetFileAsync(fileId?.ToString());
        var docParams = await _documentServiceHelper.GetParamsAsync(file, true, editable ? FileShare.ReadWrite : FileShare.Read, false, editable, editable, editable, false);
        return await GetFileEntryWrapperAsync(docParams.File);
    }
}

public abstract class FilesController<T> : ApiControllerBase
{
    protected readonly FilesControllerHelper _filesControllerHelper;
    private readonly FileStorageService _fileStorageService;
    private readonly IMapper _mapper;
    private readonly FileOperationDtoHelper _fileOperationDtoHelper;

    public FilesController(
        FilesControllerHelper filesControllerHelper,
        FileStorageService fileStorageService,
        IMapper mapper,
        FileOperationDtoHelper fileOperationDtoHelper,
        FolderDtoHelper folderDtoHelper,
        FileDtoHelper fileDtoHelper) : base(folderDtoHelper, fileDtoHelper)
    {
        _filesControllerHelper = filesControllerHelper;
        _fileStorageService = fileStorageService;
        _mapper = mapper;
        _fileOperationDtoHelper = fileOperationDtoHelper;
    }

    /// <summary>
    /// Changes the version history of a file with the ID specified in the request.
    /// </summary>
    /// <short>Change version history</short>
    /// <param type="System.Int32, System" method="url" name="fileId">File ID</param>
    /// <param type="ASC.Files.Core.ApiModels.RequestDto.ChangeHistoryRequestDto, ASC.Files.Core" name="inDto">Request parameters for changing version history</param>
    /// <category>Files</category>
    /// <returns type="ASC.Files.Core.ApiModels.ResponseDto.FileDto, ASC.Files.Core">Updated information about file versions</returns>
    /// <path>api/2.0/files/file/{fileId}/history</path>
    /// <httpMethod>PUT</httpMethod>
    /// <collection>list</collection>
    [HttpPut("file/{fileId}/history")]
    public IAsyncEnumerable<FileDto<T>> ChangeHistoryAsync(T fileId, ChangeHistoryRequestDto inDto)
    {
        return _filesControllerHelper.ChangeHistoryAsync(fileId, inDto.Version, inDto.ContinueVersion);
    }

    /// <summary>
    /// Checks the conversion status of a file with the ID specified in the request.
    /// </summary>
    /// <short>Get conversion status</short>
    /// <category>Operations</category>
    /// <param type="System.Int32, System" method="url" name="fileId">File ID</param>
    /// <param type="System.Boolean, System" name="start">Specifies if a conversion operation is started or not</param>
    /// <returns type="ASC.Files.Core.ApiModels.ResponseDto.ConversationResultDto, ASC.Files.Core">Conversion result</returns>
    /// <path>api/2.0/files/file/{fileId}/checkconversion</path>
    /// <httpMethod>GET</httpMethod>
    /// <collection>list</collection>
    [HttpGet("file/{fileId}/checkconversion")]
    public async IAsyncEnumerable<ConversationResultDto<T>> CheckConversionAsync(T fileId, bool start)
    {
        await foreach (var r in _filesControllerHelper.CheckConversionAsync(new CheckConversionRequestDto<T>()
        {
            FileId = fileId,
            StartConvert = start
        }))
        {
            yield return r;
        }
    }

    /// <summary>
    /// Returns a link to download a file with the ID specified in the request.
    /// </summary>
    /// <short>Get file download link</short>
    /// <category>Files</category>
    /// <param type="System.Int32, System" method="url" name="fileId">File ID</param>
    /// <returns type="System.String, System">File download link</returns>
    /// <path>api/2.0/files/file/{fileId}/presigneduri</path>
    /// <httpMethod>GET</httpMethod>
    [HttpGet("file/{fileId}/presigneduri")]
    public async Task<string> GetPresignedUri(T fileId)
    {
        return await _filesControllerHelper.GetPresignedUri(fileId);
    }

    /// <summary>
    /// Copies (and converts if possible) an existing file to the specified folder.
    /// </summary>
    /// <short>Copy a file</short>
    /// <category>Files</category>
    /// <param type="System.Int32, System" method="url" name="fileId">File ID</param>
    /// <param type="ASC.Files.Core.ApiModels.RequestDto.CopyAsRequestDto{System.Text.Json.JsonElement}, ASC.Files.Core" name="inDto">Request parameters for copying a file</param>
    /// <returns type="ASC.Files.Core.ApiModels.ResponseDto.FileEntryDto, ASC.Files.Core">Copied file entry information</returns>
    /// <path>api/2.0/files/file/{fileId}/copyas</path>
    /// <httpMethod>POST</httpMethod>
    [HttpPost("file/{fileId}/copyas")]
    public async Task<FileEntryDto> CopyFileAs(T fileId, CopyAsRequestDto<JsonElement> inDto)
    {
        if (inDto.DestFolderId.ValueKind == JsonValueKind.Number)
        {
            return await _filesControllerHelper.CopyFileAsAsync(fileId, inDto.DestFolderId.GetInt32(), inDto.DestTitle, inDto.Password);
        }
        else if (inDto.DestFolderId.ValueKind == JsonValueKind.String)
        {
            return await _filesControllerHelper.CopyFileAsAsync(fileId, inDto.DestFolderId.GetString(), inDto.DestTitle, inDto.Password);
        }

        return null;
    }

    /// <summary>
    /// Creates a new file in the specified folder with the title specified in the request.
    /// </summary>
    /// <short>Create a file</short>
    /// <category>Files</category>
    /// <param type="System.Int32, System" method="url" name="folderId">Folder ID</param>
    /// <param type="ASC.Files.Core.ApiModels.RequestDto.CreateFileRequestDto{System.Text.Json.JsonElement}, ASC.Files.Core" name="inDto">Request parameters for creating a file</param>
    /// <remarks>If a file extension is different from DOCX/XLSX/PPTX and refers to one of the known text, spreadsheet, or presentation formats, it will be changed to DOCX/XLSX/PPTX accordingly. If the file extension is not specified or is unknown, the DOCX extension will be added to the file title.</remarks>
    /// <returns type="ASC.Files.Core.ApiModels.ResponseDto.FileDto, ASC.Files.Core">New file information</returns>
    /// <path>api/2.0/files/{folderId}/file</path>
    /// <httpMethod>POST</httpMethod>
    [HttpPost("{folderId}/file")]
    public async Task<FileDto<T>> CreateFileAsync(T folderId, CreateFileRequestDto<JsonElement> inDto)
    {
        return await _filesControllerHelper.CreateFileAsync(folderId, inDto.Title, inDto.TemplateId, inDto.FormId, inDto.EnableExternalExt);
    }

    /// <summary>
    /// Creates an HTML (.html) file in the selected folder with the title and contents specified in the request.
    /// </summary>
    /// <short>Create an HTML file</short>
    /// <category>Files</category>
    /// <param type="System.Int32, System" method="url" name="folderId">Folder ID</param>
    /// <param type="ASC.Files.Core.ApiModels.RequestDto.CreateTextOrHtmlFileRequestDto, ASC.Files.Core" name="inDto">Request parameters for creating an HTML file</param>
    /// <returns type="ASC.Files.Core.ApiModels.ResponseDto.FileDto, ASC.Files.Core">New file information</returns>
    /// <path>api/2.0/files/{folderId}/html</path>
    /// <httpMethod>POST</httpMethod>
    [HttpPost("{folderId}/html")]
    public async Task<FileDto<T>> CreateHtmlFileAsync(T folderId, CreateTextOrHtmlFileRequestDto inDto)
    {
        return await _filesControllerHelper.CreateHtmlFileAsync(folderId, inDto.Title, inDto.Content);
    }

    /// <summary>
    /// Creates a text (.txt) file in the selected folder with the title and contents specified in the request.
    /// </summary>
    /// <short>Create a txt file</short>
    /// <category>Files</category>
    /// <param type="System.Int32, System" method="url" name="folderId">Folder ID</param>
    /// <param type="ASC.Files.Core.ApiModels.RequestDto.CreateTextOrHtmlFileRequestDto, ASC.Files.Core" name="inDto">Request parameters for creating a text file</param>
    /// <returns type="ASC.Files.Core.ApiModels.ResponseDto.FileDto, ASC.Files.Core">New file information</returns>
    /// <path>api/2.0/files/{folderId}/text</path>
    /// <httpMethod>POST</httpMethod>
    [HttpPost("{folderId}/text")]
    public async Task<FileDto<T>> CreateTextFileAsync(T folderId, CreateTextOrHtmlFileRequestDto inDto)
    {
        return await _filesControllerHelper.CreateTextFileAsync(folderId, inDto.Title, inDto.Content);
    }

    /// <summary>
    /// Deletes a file with the ID specified in the request.
    /// </summary>
    /// <short>Delete a file</short>
    /// <category>Files</category>
    /// <param type="System.Int32, System" method="url" name="fileId">File ID</param>
    /// <param type="ASC.Files.Core.ApiModels.RequestDto.DeleteRequestDto, ASC.Files.Core" name="inDto">Request parameters for deleting a file</param>
    /// <returns type="ASC.Files.Core.ApiModels.ResponseDto.FileOperationDto, ASC.Files.Core">List of file operations</returns>
    /// <path>api/2.0/files/file/{fileId}</path>
    /// <httpMethod>DELETE</httpMethod>
    /// <collection>list</collection>
    [HttpDelete("file/{fileId}")]
    public async IAsyncEnumerable<FileOperationDto> DeleteFile(T fileId, [FromBody] DeleteRequestDto inDto)
    {
        foreach (var e in await _fileStorageService.DeleteFileAsync("delete", fileId, false, inDto.DeleteAfter, inDto.Immediately))
        {
            yield return await _fileOperationDtoHelper.GetAsync(e);
        }
    }

    /// <summary>
    /// Returns a URL to the changes of a file version specified in the request.
    /// </summary>
    /// <short>Get changes URL</short>
    /// <category>Files</category>
    /// <param type="System.Int32, System" method="url" name="fileId">File ID</param>
    /// <param type="System.Int32, System" name="version">File version</param>
    /// <param type="System.String, System" name="doc">Shared token</param>
    /// <returns type="ASC.Files.Core.EditHistoryDataDto, ASC.Files.Core">File version history data</returns>
    /// <path>api/2.0/files/file/{fileId}/edit/diff</path>
    /// <httpMethod>GET</httpMethod>
    /// <requiresAuthorization>false</requiresAuthorization>
    [AllowAnonymous]
    [HttpGet("file/{fileId}/edit/diff")]
    public async Task<EditHistoryDataDto> GetEditDiffUrlAsync(T fileId, int version = 0, string doc = null)
    {
        return await _filesControllerHelper.GetEditDiffUrlAsync(fileId, version, doc);
    }

    /// <summary>
    /// Returns the version history of a file with the ID specified in the request.
    /// </summary>
    /// <short>Get version history</short>
    /// <category>Files</category>
    /// <param type="System.Int32, System" method="url" name="fileId">File ID</param>
    /// <param type="System.String, System" name="doc">Shared token</param>
    /// <returns type="ASC.Files.Core.ApiModels.ResponseDto.EditHistoryDto, ASC.Files.Core">Version history data</returns>
    /// <path>api/2.0/files/file/{fileId}/edit/history</path>
    /// <httpMethod>GET</httpMethod>
    /// <requiresAuthorization>false</requiresAuthorization>
    /// <collection>list</collection>
    [AllowAnonymous]
    [HttpGet("file/{fileId}/edit/history")]
    public IAsyncEnumerable<EditHistoryDto> GetEditHistoryAsync(T fileId, string doc = null)
    {
        return _filesControllerHelper.GetEditHistoryAsync(fileId, doc);
    }

    /// <summary>
    /// Returns the detailed information about a file with the ID specified in the request.
    /// </summary>
    /// <short>Get file information</short>
    /// <param type="System.Int32, System" method="url" name="fileId">File ID</param>
    /// <param type="System.Int32, System" name="version">File version</param>
    /// <category>Files</category>
    /// <returns type="ASC.Files.Core.ApiModels.ResponseDto.FileDto, ASC.Files.Core">File information</returns>
    /// <path>api/2.0/files/file/{fileId}</path>
    /// <httpMethod>GET</httpMethod>
    [AllowAnonymous]
    [HttpGet("file/{fileId}")]
    public async Task<FileDto<T>> GetFileInfoAsync(T fileId, int version = -1)
    {
        return await _filesControllerHelper.GetFileInfoAsync(fileId, version);
    }


    /// <summary>
    /// Returns the detailed information about all the available file versions with the ID specified in the request.
    /// </summary>
    /// <short>Get file versions</short>
    /// <category>Files</category>
    /// <param type="System.Int32, System" method="url" name="fileId">File ID</param>
    /// <returns type="ASC.Files.Core.ApiModels.ResponseDto.FileDto, ASC.Files.Core">Information about file versions: folder ID, version, version group, content length, pure content length, file status, URL to view a file, web URL, file type, file extension, comment, encrypted or not, thumbnail URL, thumbnail status, locked or not, user ID who locked a file, denies file downloading or not, denies file sharing or not, file accessibility</returns>
    /// <path>api/2.0/files/file/{fileId}/history</path>
    /// <httpMethod>GET</httpMethod>
    /// <collection>list</collection>
    [HttpGet("file/{fileId}/history")]
    public IAsyncEnumerable<FileDto<T>> GetFileVersionInfoAsync(T fileId)
    {
        return _filesControllerHelper.GetFileVersionInfoAsync(fileId);
    }

    /// <summary>
    /// Locks a file with the ID specified in the request.
    /// </summary>
    /// <short>Lock a file</short>
    /// <category>Files</category>
    /// <param type="System.Int32, System" method="url" name="fileId">File ID</param>
    /// <param type="ASC.Files.Core.ApiModels.RequestDto.LockFileRequestDto, ASC.Files.Core" name="inDto">Request parameters for locking a file</param>
    /// <returns type="ASC.Files.Core.ApiModels.ResponseDto.FileDto, ASC.Files.Core">Locked file information</returns>
    /// <path>api/2.0/files/file/{fileId}/lock</path>
    /// <httpMethod>PUT</httpMethod>
    [HttpPut("file/{fileId}/lock")]
    public async Task<FileDto<T>> LockFileAsync(T fileId, LockFileRequestDto inDto)
    {
        return await _filesControllerHelper.LockFileAsync(fileId, inDto.LockFile);
    }

    /// <summary>
    /// Restores a file version specified in the request.
    /// </summary>
    /// <short>Restore a file version</short>
    /// <category>Files</category>
    /// <param type="System.Int32, System" method="url" name="fileId">File ID</param>
    /// <param type="System.Int32, System" name="version">File version</param>
    /// <param type="System.String, System" name="url">File version URL</param>
    /// <param type="System.String, System" name="doc">Shared token</param>
    /// <returns type="ASC.Files.Core.ApiModels.ResponseDto.EditHistoryDto, ASC.Files.Core">Version history data: file ID, key, file version, version group, a user who updated a file, creation time, history changes in the string format, list of history changes, server version</returns>
    /// <path>api/2.0/files/file/{fileId}/restoreversion</path>
    /// <httpMethod>GET</httpMethod>
    /// <requiresAuthorization>false</requiresAuthorization>
    /// <collection>list</collection>
    [AllowAnonymous]
    [HttpGet("file/{fileId}/restoreversion")]
    public IAsyncEnumerable<EditHistoryDto> RestoreVersionAsync(T fileId, int version = 0, string url = null, string doc = null)
    {
        return _filesControllerHelper.RestoreVersionAsync(fileId, version, url, doc);
    }

    /// <summary>
    /// Starts a conversion operation of a file with the ID specified in the request.
    /// </summary>
    /// <short>Start file conversion</short>
    /// <category>Operations</category>
    /// <param type="System.Int32, System" method="url" name="fileId">File ID</param>
    /// <param type="ASC.Files.Core.ApiModels.RequestDto.CheckConversionRequestDto, ASC.Files.Core" name="inDto">Request parameters for starting file conversion</param>
    /// <returns type="ASC.Files.Core.ApiModels.ResponseDto.ConversationResultDto, ASC.Files.Core">Conversion result</returns>
    /// <path>api/2.0/files/file/{fileId}/checkconversion</path>
    /// <httpMethod>PUT</httpMethod>
    /// <collection>list</collection>
    [HttpPut("file/{fileId}/checkconversion")]
    public IAsyncEnumerable<ConversationResultDto<T>> StartConversion(T fileId, [FromBody(EmptyBodyBehavior = Microsoft.AspNetCore.Mvc.ModelBinding.EmptyBodyBehavior.Allow)] CheckConversionRequestDto<T> inDto)
    {
        if (inDto == null)
        {
            inDto = new CheckConversionRequestDto<T>();
        }
        inDto.FileId = fileId;

        return _filesControllerHelper.StartConversionAsync(inDto);
    }

    /// <summary>
    /// Updates a comment in a file with the ID specified in the request.
    /// </summary>
    /// <short>Update a comment</short>
    /// <category>Operations</category>
    /// <param type="System.Int32, System" method="url" name="fileId">File ID</param>
    /// <param type="ASC.Files.Core.ApiModels.RequestDto.UpdateCommentRequestDto, ASC.Files.Core" name="inDto">Request parameters for updating a comment</param>
    /// <returns type="System.Object, System">Updated comment</returns>
    /// <path>api/2.0/files/file/{fileId}/comment</path>
    /// <httpMethod>PUT</httpMethod>
    [HttpPut("file/{fileId}/comment")]
    public async Task<object> UpdateCommentAsync(T fileId, UpdateCommentRequestDto inDto)
    {
        return await _filesControllerHelper.UpdateCommentAsync(fileId, inDto.Version, inDto.Comment);
    }

    /// <summary>
    /// Updates the information of the selected file with the parameters specified in the request.
    /// </summary>
    /// <short>Update a file</short>
    /// <category>Files</category>
    /// <param type="System.Int32, System" method="url" name="fileId">File ID</param>
    /// <param type="ASC.Files.Core.ApiModels.RequestDto.UpdateFileRequestDto, ASC.Files.Core" name="inDto">Request parameters for updating a file</param>
    /// <returns type="ASC.Files.Core.ApiModels.ResponseDto.FileDto, ASC.Files.Core">Updated file information</returns>
    /// <path>api/2.0/files/file/{fileId}</path>
    /// <httpMethod>PUT</httpMethod>
    [HttpPut("file/{fileId}")]
    public async Task<FileDto<T>> UpdateFileAsync(T fileId, UpdateFileRequestDto inDto)
    {
        return await _filesControllerHelper.UpdateFileAsync(fileId, inDto.Title, inDto.LastVersion);
    }

    /// <summary>
    /// Updates the contents of a file with the ID specified in the request.
    /// </summary>
    /// <short>Update file contents</short>
    /// <category>Files</category>
    /// <param type="System.Int32, System" method="url" name="fileId">File ID</param>
    /// <param type="ASC.Files.Core.ApiModels.RequestDto.FileStreamRequestDto, ASC.Files.Core" name="inDto">Request parameters for updating file contents</param>
    /// <path>api/2.0/files/{fileId}/update</path>
    /// <httpMethod>PUT</httpMethod>
    /// <returns type="ASC.Files.Core.ApiModels.ResponseDto.FileDto, ASC.Files.Core">Updated file information</returns>
    /// <visible>false</visible>
    [HttpPut("{fileId}/update")]
    public async Task<FileDto<T>> UpdateFileStreamFromFormAsync(T fileId, [FromForm] FileStreamRequestDto inDto)
    {
        return await _filesControllerHelper.UpdateFileStreamAsync(_filesControllerHelper.GetFileFromRequest(inDto).OpenReadStream(), fileId, inDto.FileExtension, inDto.Encrypted, inDto.Forcesave);
    }

    /// <summary>
    /// Returns file properties of the specified file.
    /// </summary>
    /// <short>Get file properties</short>
    /// <param type="System.Int32, System" method="url" name="fileId">File ID</param>
    /// <category>Files</category>
    /// <returns type="ASC.Files.Core.ApiModels.RequestDto.EntryPropertiesRequestDto, ASC.Files.Core">File properties</returns>
    /// <path>api/2.0/files/{fileId}/properties</path>
    /// <httpMethod>GET</httpMethod>
    [HttpGet("{fileId}/properties")]
    public async Task<EntryPropertiesRequestDto> GetProperties(T fileId)
    {
        return _mapper.Map<EntryProperties, EntryPropertiesRequestDto>(await _fileStorageService.GetFileProperties(fileId));
    }


    /// <summary>
    /// Saves file properties to the specified file.
    /// </summary>
    /// <short>Save file properties to a file</short>
    /// <param type="System.Int32, System" method="url" name="fileId">File ID</param>
    /// <param type="ASC.Files.Core.ApiModels.RequestDto.EntryPropertiesRequestDto, ASC.Files.Core" name="inDto">File properties request parameters</param>
    /// <category>Files</category>
    /// <returns type="ASC.Files.Core.EntryProperties, ASC.Files.Core">File properties</returns>
    /// <path>api/2.0/files/{fileId}/properties</path>
    /// <httpMethod>PUT</httpMethod>
    [HttpPut("{fileId}/properties")]
    public Task<EntryProperties> SetProperties(T fileId, EntryPropertiesRequestDto inDto)
    {
        return _fileStorageService.SetFileProperties(fileId, _mapper.Map<EntryPropertiesRequestDto, EntryProperties>(inDto));
    }
}

public class FilesControllerCommon : ApiControllerBase
{
    private readonly IMapper _mapper;
    private readonly IServiceScopeFactory _serviceScopeFactory;
    private readonly GlobalFolderHelper _globalFolderHelper;
    private readonly FileStorageService _fileStorageService;
    private readonly FilesControllerHelper _filesControllerHelperInternal;

    public FilesControllerCommon(
        IMapper mapper,
        IServiceScopeFactory serviceScopeFactory,
        GlobalFolderHelper globalFolderHelper,
        FileStorageService fileStorageService,
        FilesControllerHelper filesControllerHelperInternal,
        FolderDtoHelper folderDtoHelper,
        FileDtoHelper fileDtoHelper) : base(folderDtoHelper, fileDtoHelper)
    {
        _mapper = mapper;
        _serviceScopeFactory = serviceScopeFactory;
        _globalFolderHelper = globalFolderHelper;
        _fileStorageService = fileStorageService;
        _filesControllerHelperInternal = filesControllerHelperInternal;
    }

    /// <summary>
    /// Creates a new file in the "My documents" section with the title specified in the request.
    /// </summary>
    /// <short>Create a file in the "My documents" section</short>
    /// <category>Files</category>
    /// <param type="ASC.Files.Core.ApiModels.RequestDto.CreateFileRequestDto{System.Text.Json.JsonElement}, ASC.Files.Core" name="inDto">Request parameters for creating a file</param>
    /// <remarks>If a file extension is different from DOCX/XLSX/PPTX and refers to one of the known text, spreadsheet, or presentation formats, it will be changed to DOCX/XLSX/PPTX accordingly. If the file extension is not specified or is unknown, the DOCX extension will be added to the file title.</remarks>
    /// <returns type="ASC.Files.Core.ApiModels.ResponseDto.FileDto, ASC.Files.Core">New file information</returns>
    /// <path>api/2.0/files/@my/file</path>
    /// <httpMethod>POST</httpMethod>
    [HttpPost("@my/file")]
    public async Task<FileDto<int>> CreateFileAsync(CreateFileRequestDto<JsonElement> inDto)
    {
        return await _filesControllerHelperInternal.CreateFileAsync(await _globalFolderHelper.FolderMyAsync, inDto.Title, inDto.TemplateId, inDto.FormId, inDto.EnableExternalExt);
    }

    /// <summary>
    /// Creates an HTML (.html) file in the "Common" section with the title and contents specified in the request.
    /// </summary>
    /// <short>Create an HTML file in the "Common" section</short>
    /// <category>Files</category>
    /// <param type="ASC.Files.Core.ApiModels.RequestDto.CreateTextOrHtmlFileRequestDto, ASC.Files.Core" name="inDto">Request parameters for creating an HTML file</param>
    /// <returns type="ASC.Files.Core.ApiModels.ResponseDto.FileDto, ASC.Files.Core">New file information</returns>
    /// <path>api/2.0/files/@common/html</path>
    /// <httpMethod>POST</httpMethod>
    [HttpPost("@common/html")]
    public async Task<FileDto<int>> CreateHtmlFileInCommonAsync(CreateTextOrHtmlFileRequestDto inDto)
    {
        return await _filesControllerHelperInternal.CreateHtmlFileAsync(await _globalFolderHelper.FolderCommonAsync, inDto.Title, inDto.Content);
    }

    /// <summary>
    /// Creates an HTML (.html) file in the "My documents" section with the title and contents specified in the request.
    /// </summary>
    /// <short>Create an HTML file in the "My documents" section</short>
    /// <category>Files</category>
    /// <param type="ASC.Files.Core.ApiModels.RequestDto.CreateTextOrHtmlFileRequestDto, ASC.Files.Core" name="inDto">Request parameters for creating an HTML file</param>
    /// <returns type="ASC.Files.Core.ApiModels.ResponseDto.FileDto, ASC.Files.Core">New file information</returns>
    /// <path>api/2.0/files/@my/html</path>
    /// <httpMethod>POST</httpMethod>
    [HttpPost("@my/html")]
    public async Task<FileDto<int>> CreateHtmlFileInMyAsync(CreateTextOrHtmlFileRequestDto inDto)
    {
        return await _filesControllerHelperInternal.CreateHtmlFileAsync(await _globalFolderHelper.FolderMyAsync, inDto.Title, inDto.Content);
    }

    /// <summary>
    /// Creates a text (.txt) file in the "Common" section with the title and contents specified in the request.
    /// </summary>
    /// <short>Create a text file in the "Common" section</short>
    /// <category>Files</category>
    /// <param type="ASC.Files.Core.ApiModels.RequestDto.CreateTextOrHtmlFileRequestDto, ASC.Files.Core" name="inDto">Request parameters for creating a text file</param>
    /// <returns type="ASC.Files.Core.ApiModels.ResponseDto.FileDto, ASC.Files.Core">New file information</returns>
    /// <path>api/2.0/files/@common/text</path>
    /// <httpMethod>POST</httpMethod>
    [HttpPost("@common/text")]
    public async Task<FileDto<int>> CreateTextFileInCommonAsync(CreateTextOrHtmlFileRequestDto inDto)
    {
        return await _filesControllerHelperInternal.CreateTextFileAsync(await _globalFolderHelper.FolderCommonAsync, inDto.Title, inDto.Content);
    }

    /// <summary>
    /// Creates a text (.txt) file in the "My documents" section with the title and contents specified in the request.
    /// </summary>
    /// <short>Create a text file in the "My documents" section</short>
    /// <category>Files</category>
    /// <param type="ASC.Files.Core.ApiModels.RequestDto.CreateTextOrHtmlFileRequestDto, ASC.Files.Core" name="inDto">Request parameters for creating a text file</param>
    /// <returns type="ASC.Files.Core.ApiModels.ResponseDto.FileDto, ASC.Files.Core">New file information</returns>
    /// <path>api/2.0/files/@my/text</path>
    /// <httpMethod>POST</httpMethod>
    [HttpPost("@my/text")]
    public async Task<FileDto<int>> CreateTextFileInMyAsync(CreateTextOrHtmlFileRequestDto inDto)
    {
        return await _filesControllerHelperInternal.CreateTextFileAsync(await _globalFolderHelper.FolderMyAsync, inDto.Title, inDto.Content);
    }

    /// <summary>
    /// Creates thumbnails for the files with the IDs specified in the request.
    /// </summary>
    /// <short>Create thumbnails</short>
    /// <category>Files</category>
    /// <param type="ASC.Files.Core.ApiModels.RequestDto.BaseBatchRequestDto, ASC.Files.Core" name="inDto">Base batch request parameters</param>
    /// <returns type="System.Text.Json.JsonElement, System.Text.Json">List of file IDs</returns>
    /// <path>api/2.0/files/thumbnails</path>
    /// <httpMethod>POST</httpMethod>
    /// <collection>list</collection>
    [AllowAnonymous]
    [HttpPost("thumbnails")]
    public async Task<IEnumerable<JsonElement>> CreateThumbnailsAsync(BaseBatchRequestDto inDto)
    {
        return await _fileStorageService.CreateThumbnailsAsync(inDto.FileIds.ToList());
    }


    /// <summary>
    /// Saves file properties to the specified files.
    /// </summary>
    /// <short>Save file properties to files</short>
    /// <param type="ASC.Files.Core.ApiModels.RequestDto.BatchEntryPropertiesRequestDto, ASC.Files.Core" name="inDto">Batch entry properties request parameters</param>
    /// <category>Files</category>
    /// <returns type="ASC.Files.Core.EntryProperties, ASC.Files.Core">List of file properties: collects the data from the filled forms or not, folder ID where a file will be saved, folder path where a file will be saved, new folder title, file name mask</returns>
    /// <path>api/2.0/files/batch/properties</path>
    /// <httpMethod>PUT</httpMethod>
    /// <collection>list</collection>
    [HttpPut("batch/properties")]
    public async Task<List<EntryProperties>> SetProperties(BatchEntryPropertiesRequestDto inDto)
    {
        var result = new List<EntryProperties>();

        foreach (var fileId in inDto.FilesId)
        {
            if (fileId.ValueKind == JsonValueKind.String)
            {
                await AddProps(fileId.GetString());
            }
            else if (fileId.ValueKind == JsonValueKind.String)
            {
                await AddProps(fileId.GetInt32());
            }
        }

        return result;

        async Task AddProps<T>(T fileId)
        {
            await using var scope = _serviceScopeFactory.CreateAsyncScope();
            var fileStorageService = scope.ServiceProvider.GetRequiredService<FileStorageService>();
            var props = _mapper.Map<EntryPropertiesRequestDto, EntryProperties>(inDto.FileProperties);
            if (inDto.CreateSubfolder)
            {
                var file = await fileStorageService.GetFileAsync(fileId, -1).NotFoundIfNull("File not found");
                props.FormFilling.CreateFolderTitle = Path.GetFileNameWithoutExtension(file.Title);
            }

            result.Add(await fileStorageService.SetFileProperties(fileId, props));
        }
    }
}