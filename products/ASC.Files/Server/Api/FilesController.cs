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
    public FilesControllerInternal(FilesControllerHelper<int> filesControllerHelper) : base(filesControllerHelper)
    {
    }
}

public class FilesControllerThirdparty : FilesController<string>
{
    public FilesControllerThirdparty(FilesControllerHelper<string> filesControllerHelper) : base(filesControllerHelper)
    {
    }
}

public abstract class FilesController<T> : ApiControllerBase
{
    private readonly FilesControllerHelper<T> _filesControllerHelper;

    public FilesController(FilesControllerHelper<T> filesControllerHelper)
    {
        _filesControllerHelper = filesControllerHelper;
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
    public Task<IEnumerable<FileDto<T>>> ChangeHistoryAsync(T fileId, ChangeHistoryRequestDto inDto)
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
    [Read("file/{fileId}/checkconversion")]
    public IAsyncEnumerable<ConversationResultDto<T>> CheckConversionAsync(T fileId, bool start)
    {
        return _filesControllerHelper.CheckConversionAsync(new CheckConversionRequestDto<T>()
        {
            FileId = fileId,
            StartConvert = start
        });
    }

    [Create("file/{fileId}/copyas", order: int.MaxValue)]
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
    [Create("{folderId}/file")]
    public Task<FileDto<T>> CreateFileAsync(T folderId, CreateFileRequestDto<JsonElement> inDto)
    {
        return _filesControllerHelper.CreateFileAsync(folderId, inDto.Title, inDto.TemplateId, inDto.FormId, inDto.EnableExternalExt);
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
    public Task<FileDto<T>> CreateHtmlFileAsync(T folderId, CreateTextOrHtmlFileRequestDto inDto)
    {
        return _filesControllerHelper.CreateHtmlFileAsync(folderId, inDto.Title, inDto.Content);
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
    public Task<FileDto<T>> CreateTextFileAsync(T folderId, CreateTextOrHtmlFileRequestDto inDto)
    {
        return _filesControllerHelper.CreateTextFileAsync(folderId, inDto.Title, inDto.Content);
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
    public Task<IEnumerable<FileOperationDto>> DeleteFile(T fileId, [FromBody] DeleteRequestDto inDto)
    {
        return _filesControllerHelper.DeleteFileAsync(fileId, inDto.DeleteAfter, inDto.Immediately);
    }

    [AllowAnonymous]
    [Read("file/{fileId}/edit/diff")]
    public Task<EditHistoryDataDto> GetEditDiffUrlAsync(T fileId, int version = 0, string doc = null)
    {
        return _filesControllerHelper.GetEditDiffUrlAsync(fileId, version, doc);
    }

    [AllowAnonymous]
    [Read("file/{fileId}/edit/history")]
    public Task<List<EditHistoryDto>> GetEditHistoryAsync(T fileId, string doc = null)
    {
        return _filesControllerHelper.GetEditHistoryAsync(fileId, doc);
    }

    /// <summary>
    /// Returns a detailed information about the file with the ID specified in the request
    /// </summary>
    /// <short>File information</short>
    /// <category>Files</category>
    /// <returns>File info</returns>
    [Read("file/{fileId}", order: int.MaxValue, DisableFormat = true)]
    public Task<FileDto<T>> GetFileInfoAsync(T fileId, int version = -1)
    {
        return _filesControllerHelper.GetFileInfoAsync(fileId, version);
    }

    /// <summary>
    /// Returns the detailed information about all the available file versions with the ID specified in the request
    /// </summary>
    /// <short>File versions</short>
    /// <category>Files</category>
    /// <param name="fileId">File ID</param>
    /// <returns>File information</returns>
    [Read("file/{fileId}/history")]
    public Task<IEnumerable<FileDto<T>>> GetFileVersionInfoAsync(T fileId)
    {
        return _filesControllerHelper.GetFileVersionInfoAsync(fileId);
    }

    [Update("file/{fileId}/lock")]
    public Task<FileDto<T>> LockFileAsync(T fileId, LockFileRequestDto inDto)
    {
        return _filesControllerHelper.LockFileAsync(fileId, inDto.LockFile);
    }

    [AllowAnonymous]
    [Read("file/{fileId}/restoreversion")]
    public Task<List<EditHistoryDto>> RestoreVersionAsync(T fileId, int version = 0, string url = null, string doc = null)
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
    [Update("file/{fileId}/checkconversion")]
    public IAsyncEnumerable<ConversationResultDto<T>> StartConversion(T fileId, [FromBody(EmptyBodyBehavior = Microsoft.AspNetCore.Mvc.ModelBinding.EmptyBodyBehavior.Allow)] CheckConversionRequestDto<T> inDto)
    {
        if (inDto == null)
        {
            inDto = new CheckConversionRequestDto<T>();
        }
        inDto.FileId = fileId;

        return _filesControllerHelper.StartConversionAsync(inDto);
    }

    [Update("file/{fileId}/comment")]
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
    [Update("file/{fileId}", order: int.MaxValue, DisableFormat = true)]
    public Task<FileDto<T>> UpdateFileAsync(T fileId, UpdateFileRequestDto inDto)
    {
        return _filesControllerHelper.UpdateFileAsync(fileId, inDto.Title, inDto.LastVersion);
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
    public Task<FileDto<T>> UpdateFileStreamFromFormAsync(T fileId, [FromForm] FileStreamRequestDto inDto)
    {
        return _filesControllerHelper.UpdateFileStreamAsync(_filesControllerHelper.GetFileFromRequest(inDto).OpenReadStream(), fileId, inDto.FileExtension, inDto.Encrypted, inDto.Forcesave);
    }
}

public class FilesControllerCommon : ApiControllerBase
{
    private readonly GlobalFolderHelper _globalFolderHelper;
    private readonly FileStorageService<string> _fileStorageServiceThirdparty;
    private readonly FilesControllerHelper<int> _filesControllerHelperInternal;

    public FilesControllerCommon(
        GlobalFolderHelper globalFolderHelper,
        FileStorageService<string> fileStorageServiceThirdparty,
        FilesControllerHelper<int> filesControllerHelperInternal)
    {
        _globalFolderHelper = globalFolderHelper;
        _fileStorageServiceThirdparty = fileStorageServiceThirdparty;
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
    [Create("@my/file")]
    public Task<FileDto<int>> CreateFileAsync(CreateFileRequestDto<JsonElement> inDto)
    {
        return _filesControllerHelperInternal.CreateFileAsync(_globalFolderHelper.FolderMy, inDto.Title, inDto.TemplateId, inDto.FormId, inDto.EnableExternalExt);
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
    [Create("@my/html")]
    public Task<FileDto<int>> CreateHtmlFileInMyAsync(CreateTextOrHtmlFileRequestDto inDto)
    {
        return _filesControllerHelperInternal.CreateHtmlFileAsync(_globalFolderHelper.FolderMy, inDto.Title, inDto.Content);
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
    [Create("@my/text")]
    public Task<FileDto<int>> CreateTextFileInMyAsync(CreateTextOrHtmlFileRequestDto inDto)
    {
        return _filesControllerHelperInternal.CreateTextFileAsync(_globalFolderHelper.FolderMy, inDto.Title, inDto.Content);
    }

    [Create("thumbnails")]
    public Task<IEnumerable<JsonElement>> CreateThumbnailsAsync(BaseBatchRequestDto inDto)
    {
        return _fileStorageServiceThirdparty.CreateThumbnailsAsync(inDto.FileIds.ToList());
    }
}