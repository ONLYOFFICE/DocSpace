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
    /// Change version history
    /// </summary>
    /// <param name="fileId">File ID</param>
    /// <param name="version">Version of history</param>
    /// <param name="continueVersion">Mark as version or revision</param>
    /// <category>Files</category>
    /// <returns></returns>
    [HttpPut("file/{fileId}/history")]
    public IAsyncEnumerable<FileDto<T>> ChangeHistoryAsync(T fileId, ChangeHistoryRequestDto inDto)
    {
        return _filesControllerHelper.ChangeHistoryAsync(fileId, inDto.Version, inDto.ContinueVersion);
    }

    /// <summary>
    ///  Check conversion status
    /// </summary>
    /// <short>Convert</short>
    /// <category>File operations</category>
    /// <param name="fileId"></param>
    /// <param name="start"></param>
    /// <returns>Operation result</returns>
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

    [HttpGet("file/{fileId}/presigneduri")]
    public async Task<string> GetPresignedUri(T fileId)
    {
        return await _filesControllerHelper.GetPresignedUri(fileId);
    }

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
    /// Creates a new file in the specified folder with the title sent in the request
    /// </summary>
    /// <short>Create file</short>
    /// <category>File Creation</category>
    /// <param name="folderId">Folder ID</param>
    /// <param name="title" remark="Allowed values: the file must have one of the following extensions: DOCX, XLSX, PPTX">File title</param>
    /// <remarks>In case the extension for the file title differs from DOCX/XLSX/PPTX and belongs to one of the known text, spreadsheet or presentation formats, it will be changed to DOCX/XLSX/PPTX accordingly. If the file extension is not set or is unknown, the DOCX extension will be added to the file title.</remarks>
    /// <returns>New file info</returns>
    [HttpPost("{folderId}/file")]
    public async Task<FileDto<T>> CreateFileAsync(T folderId, CreateFileRequestDto<JsonElement> inDto)
    {
        return await _filesControllerHelper.CreateFileAsync(folderId, inDto.Title, inDto.TemplateId, inDto.FormId, inDto.EnableExternalExt);
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
    [HttpPost("{folderId}/html")]
    public async Task<FileDto<T>> CreateHtmlFileAsync(T folderId, CreateTextOrHtmlFileRequestDto inDto)
    {
        return await _filesControllerHelper.CreateHtmlFileAsync(folderId, inDto.Title, inDto.Content);
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
    [HttpPost("{folderId}/text")]
    public async Task<FileDto<T>> CreateTextFileAsync(T folderId, CreateTextOrHtmlFileRequestDto inDto)
    {
        return await _filesControllerHelper.CreateTextFileAsync(folderId, inDto.Title, inDto.Content);
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
    [HttpDelete("file/{fileId}")]
    public async IAsyncEnumerable<FileOperationDto> DeleteFile(T fileId, [FromBody] DeleteRequestDto inDto)
    {
        foreach (var e in await _fileStorageService.DeleteFileAsync("delete", fileId, false, inDto.DeleteAfter, inDto.Immediately))
        {
            yield return await _fileOperationDtoHelper.GetAsync(e);
        }
    }

    [AllowAnonymous]
    [HttpGet("file/{fileId}/edit/diff")]
    public async Task<EditHistoryDataDto> GetEditDiffUrlAsync(T fileId, int version = 0, string doc = null)
    {
        return await _filesControllerHelper.GetEditDiffUrlAsync(fileId, version, doc);
    }

    [AllowAnonymous]
    [HttpGet("file/{fileId}/edit/history")]
    public IAsyncEnumerable<EditHistoryDto> GetEditHistoryAsync(T fileId, string doc = null)
    {
        return _filesControllerHelper.GetEditHistoryAsync(fileId, doc);
    }

    /// <summary>
    /// Returns a detailed information about the file with the ID specified in the request
    /// </summary>
    /// <short>File information</short>
    /// <category>Files</category>
    /// <returns>File info</returns>
    [HttpGet("file/{fileId}")]
    public async Task<FileDto<T>> GetFileInfoAsync(T fileId, int version = -1)
    {
        return await _filesControllerHelper.GetFileInfoAsync(fileId, version);
    }


    /// <summary>
    /// Returns the detailed information about all the available file versions with the ID specified in the request
    /// </summary>
    /// <short>File versions</short>
    /// <category>Files</category>
    /// <param name="fileId">File ID</param>
    /// <returns>File information</returns>
    [HttpGet("file/{fileId}/history")]
    public IAsyncEnumerable<FileDto<T>> GetFileVersionInfoAsync(T fileId)
    {
        return _filesControllerHelper.GetFileVersionInfoAsync(fileId);
    }

    [HttpPut("file/{fileId}/lock")]
    public async Task<FileDto<T>> LockFileAsync(T fileId, LockFileRequestDto inDto)
    {
        return await _filesControllerHelper.LockFileAsync(fileId, inDto.LockFile);
    }

    [AllowAnonymous]
    [HttpGet("file/{fileId}/restoreversion")]
    public IAsyncEnumerable<EditHistoryDto> RestoreVersionAsync(T fileId, int version = 0, string url = null, string doc = null)
    {
        return _filesControllerHelper.RestoreVersionAsync(fileId, version, url, doc);
    }

    /// <summary>
    ///  Start conversion
    /// </summary>
    /// <short>Convert</short>
    /// <category>File operations</category>
    /// <param name="fileId"></param>
    /// <returns>Operation result</returns>
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

    [HttpPut("file/{fileId}/comment")]
    public async Task<object> UpdateCommentAsync(T fileId, UpdateCommentRequestDto inDto)
    {
        return await _filesControllerHelper.UpdateCommentAsync(fileId, inDto.Version, inDto.Comment);
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
    [HttpPut("file/{fileId}")]
    public async Task<FileDto<T>> UpdateFileAsync(T fileId, UpdateFileRequestDto inDto)
    {
        return await _filesControllerHelper.UpdateFileAsync(fileId, inDto.Title, inDto.LastVersion);
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="file"></param>
    /// <param name="fileId"></param>
    /// <param name="encrypted"></param>
    /// <returns></returns>
    /// <visible>false</visible>
    [HttpPut("{fileId}/update")]
    public async Task<FileDto<T>> UpdateFileStreamFromFormAsync(T fileId, [FromForm] FileStreamRequestDto inDto)
    {
        return await _filesControllerHelper.UpdateFileStreamAsync(_filesControllerHelper.GetFileFromRequest(inDto).OpenReadStream(), fileId, inDto.FileExtension, inDto.Encrypted, inDto.Forcesave);
    }

    [HttpGet("{fileId}/properties")]
    public async Task<EntryPropertiesRequestDto> GetProperties(T fileId)
    {
        return _mapper.Map<EntryProperties, EntryPropertiesRequestDto>(await _fileStorageService.GetFileProperties(fileId));
    }


    [HttpPut("{fileId}/properties")]
    public Task<EntryProperties> SetProperties(T fileId, EntryPropertiesRequestDto fileProperties)
    {
        return _fileStorageService.SetFileProperties(fileId, _mapper.Map<EntryPropertiesRequestDto, EntryProperties>(fileProperties));
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
    /// Creates a new file in the 'My Documents' section with the title sent in the request
    /// </summary>
    /// <short>Create file</short>
    /// <category>File Creation</category>
    /// <param name="title" remark="Allowed values: the file must have one of the following extensions: DOCX, XLSX, PPTX">File title</param>
    /// <remarks>In case the extension for the file title differs from DOCX/XLSX/PPTX and belongs to one of the known text, spreadsheet or presentation formats, it will be changed to DOCX/XLSX/PPTX accordingly. If the file extension is not set or is unknown, the DOCX extension will be added to the file title.</remarks>
    /// <returns>New file info</returns>
    [HttpPost("@my/file")]
    public async Task<FileDto<int>> CreateFileAsync(CreateFileRequestDto<JsonElement> inDto)
    {
        return await _filesControllerHelperInternal.CreateFileAsync(await _globalFolderHelper.FolderMyAsync, inDto.Title, inDto.TemplateId, inDto.FormId, inDto.EnableExternalExt);
    }

    /// <summary>
    /// Creates an html (.html) file in 'Common Documents' section with the title and contents sent in the request
    /// </summary>
    /// <short>Create html in 'Common'</short>
    /// <category>File Creation</category>
    /// <param name="title">File title</param>
    /// <param name="content">File contents</param>
    /// <returns>Folder contents</returns>        
    [HttpPost("@common/html")]
    public async Task<FileDto<int>> CreateHtmlFileInCommonAsync(CreateTextOrHtmlFileRequestDto inDto)
    {
        return await _filesControllerHelperInternal.CreateHtmlFileAsync(await _globalFolderHelper.FolderCommonAsync, inDto.Title, inDto.Content);
    }

    /// <summary>
    /// Creates an html (.html) file in 'My Documents' section with the title and contents sent in the request
    /// </summary>
    /// <short>Create html in 'My'</short>
    /// <category>File Creation</category>
    /// <param name="title">File title</param>
    /// <param name="content">File contents</param>
    /// <returns>Folder contents</returns>
    [HttpPost("@my/html")]
    public async Task<FileDto<int>> CreateHtmlFileInMyAsync(CreateTextOrHtmlFileRequestDto inDto)
    {
        return await _filesControllerHelperInternal.CreateHtmlFileAsync(await _globalFolderHelper.FolderMyAsync, inDto.Title, inDto.Content);
    }

    /// <summary>
    /// Creates a text (.txt) file in 'Common Documents' section with the title and contents sent in the request
    /// </summary>
    /// <short>Create txt in 'Common'</short>
    /// <category>File Creation</category>
    /// <param name="title">File title</param>
    /// <param name="content">File contents</param>
    /// <returns>Folder contents</returns>
    [HttpPost("@common/text")]
    public async Task<FileDto<int>> CreateTextFileInCommonAsync(CreateTextOrHtmlFileRequestDto inDto)
    {
        return await _filesControllerHelperInternal.CreateTextFileAsync(await _globalFolderHelper.FolderCommonAsync, inDto.Title, inDto.Content);
    }

    /// <summary>
    /// Creates a text (.txt) file in 'My Documents' section with the title and contents sent in the request
    /// </summary>
    /// <short>Create txt in 'My'</short>
    /// <category>File Creation</category>
    /// <param name="title">File title</param>
    /// <param name="content">File contents</param>
    /// <returns>Folder contents</returns>
    [HttpPost("@my/text")]
    public async Task<FileDto<int>> CreateTextFileInMyAsync(CreateTextOrHtmlFileRequestDto inDto)
    {
        return await _filesControllerHelperInternal.CreateTextFileAsync(await _globalFolderHelper.FolderMyAsync, inDto.Title, inDto.Content);
    }

    [HttpPost("thumbnails")]
    public async Task<IEnumerable<JsonElement>> CreateThumbnailsAsync(BaseBatchRequestDto inDto)
    {
        return await _fileStorageService.CreateThumbnailsAsync(inDto.FileIds.ToList());
    }


    [HttpPut("batch/properties")]
    public async Task<List<EntryProperties>> SetProperties(BatchEntryPropertiesRequestDto batchEntryPropertiesRequestDto)
    {
        var result = new List<EntryProperties>();

        foreach (var fileId in batchEntryPropertiesRequestDto.FilesId)
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
            var props = _mapper.Map<EntryPropertiesRequestDto, EntryProperties>(batchEntryPropertiesRequestDto.FileProperties);
            if (batchEntryPropertiesRequestDto.CreateSubfolder)
            {
                var file = await fileStorageService.GetFileAsync(fileId, -1).NotFoundIfNull("File not found");
                props.FormFilling.CreateFolderTitle = Path.GetFileNameWithoutExtension(file.Title);
            }

            result.Add(await fileStorageService.SetFileProperties(fileId, props));
        }
    }
}