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
public class TagsControllerInternal : TagsController<int>
{
    public TagsControllerInternal(
        FileStorageService fileStorageService,
        EntryManager entryManager,
        FolderDtoHelper folderDtoHelper,
        FileDtoHelper fileDtoHelper)
        : base(fileStorageService, entryManager, folderDtoHelper, fileDtoHelper)
    {
    }
}

public class TagsControllerThirdparty : TagsController<string>
{
    public TagsControllerThirdparty(
        FileStorageService fileStorageService,
        EntryManager entryManager,
        FolderDtoHelper folderDtoHelper,
        FileDtoHelper fileDtoHelper)
        : base(fileStorageService, entryManager, folderDtoHelper, fileDtoHelper)
    {
    }
}

public abstract class TagsController<T> : ApiControllerBase
{
    private readonly FileStorageService _fileStorageService;
    private readonly EntryManager _entryManager;

    public TagsController(
        FileStorageService fileStorageService,
        EntryManager entryManager,
        FolderDtoHelper folderDtoHelper,
        FileDtoHelper fileDtoHelper) : base(folderDtoHelper, fileDtoHelper)
    {
        _fileStorageService = fileStorageService;
        _entryManager = entryManager;
        _fileDtoHelper = fileDtoHelper;
    }

    /// <summary>
    /// Adds a file with the ID specified in the request to the "Recent" section.
    /// </summary>
    /// <param type="System.Int32, System" method="url" name="fileId">File ID</param>
    /// <short>Add a file to the "Recent" section</short>
    /// <category>Files</category>
    /// <returns type="ASC.Files.Core.ApiModels.ResponseDto.FileDto, ASC.Files.Core">New file information</returns>
    /// <path>api/2.0/files/file/{fileId}/recent</path>
    /// <httpMethod>POST</httpMethod>
    [HttpPost("file/{fileId}/recent")]
    public async Task<FileDto<T>> AddToRecentAsync(T fileId)
    {
        var file = await _fileStorageService.GetFileAsync(fileId, -1).NotFoundIfNull("File not found");

        await _entryManager.MarkAsRecent(file);

        return await _fileDtoHelper.GetAsync(file);
    }

    /// <summary>
    /// Changes the favorite status of the file with the ID specified in the request.
    /// </summary>
    /// <param type="System.Int32, System" method="url" name="fileId">File ID</param>
    /// <param type="System.Boolean, System" name="favorite">Specifies if this file is marked as favorite or not</param>
    /// <short>Change the file favorite status</short>
    /// <category>Files</category>
    /// <returns type="System.Boolean, System">Boolean value: true - the file is favorite, false - the file is not favorite</returns>
    /// <path>api/2.0/files/favorites/{fileId}</path>
    /// <httpMethod>GET</httpMethod>
    [HttpGet("favorites/{fileId}")]
    public async Task<bool> ToggleFileFavoriteAsync(T fileId, bool favorite)
    {
        return await _fileStorageService.ToggleFileFavoriteAsync(fileId, favorite);
    }
}

public class TagsControllerCommon : ApiControllerBase
{
    private readonly FileStorageService _fileStorageService;

    public TagsControllerCommon(
        FileStorageService fileStorageService,
        FolderDtoHelper folderDtoHelper,
        FileDtoHelper fileDtoHelper) : base(folderDtoHelper, fileDtoHelper)
    {
        _fileStorageService = fileStorageService;
    }

    /// <summary>
    /// Adds files and folders with the IDs specified in the request to the favorite list.
    /// </summary>
    /// <short>Add favorite files and folders</short>
    /// <category>Operations</category>
    /// <param type="ASC.Files.Core.ApiModels.RequestDto.BaseBatchRequestDto, ASC.Files.Core" name="inDto">Base batch request parameters</param>
    /// <returns type="System.Boolean, System">Boolean value: true if the operation is successful</returns>
    /// <path>api/2.0/files/favorites</path>
    /// <httpMethod>POST</httpMethod>
    [HttpPost("favorites")]
    public async Task<bool> AddFavoritesAsync(BaseBatchRequestDto inDto)
    {
        var (folderIntIds, folderStringIds) = FileOperationsManager.GetIds(inDto.FolderIds);
        var (fileIntIds, fileStringIds) = FileOperationsManager.GetIds(inDto.FileIds);

        await _fileStorageService.AddToFavoritesAsync(folderIntIds, fileIntIds);
        await _fileStorageService.AddToFavoritesAsync(folderStringIds, fileStringIds);

        return true;
    }

    /// <summary>
    /// Adds files with the IDs specified in the request to the template list.
    /// </summary>
    /// <short>Add template files</short>
    /// <category>Files</category>
    /// <param type="ASC.Files.Core.ApiModels.RequestDto.TemplatesRequestDto, ASC.Files.Core" name="inDto">Request parameters for adding files to the template list</param>
    /// <returns type="System.Boolean, System">Boolean value: true if the operation is successful</returns>
    /// <path>api/2.0/files/templates</path>
    /// <httpMethod>POST</httpMethod>
    [HttpPost("templates")]
    public async Task<bool> AddTemplatesAsync(TemplatesRequestDto inDto)
    {
        await _fileStorageService.AddToTemplatesAsync(inDto.FileIds);

        return true;
    }

    /// <summary>
    /// Removes files and folders with the IDs specified in the request from the favorite list. This method uses the body parameters.
    /// </summary>
    /// <short>Delete favorite files and folders (using body parameters)</short>
    /// <category>Operations</category>
    /// <param type="ASC.Files.Core.ApiModels.RequestDto.BaseBatchRequestDto, ASC.Files.Core" name="inDto">Base batch request parameters</param>
    /// <returns type="System.Boolean, System">Boolean value: true if the operation is successful</returns>
    /// <path>api/2.0/files/favorites</path>
    /// <httpMethod>DELETE</httpMethod>
    [HttpDelete("favorites")]
    [Consumes("application/json")]
    public async Task<bool> DeleteFavoritesFromBodyAsync([FromBody] BaseBatchRequestDto inDto)
    {
        return await DeleteFavoritesAsync(inDto);
    }

    /// <summary>
    /// Removes files and folders with the IDs specified in the request from the favorite list. This method uses the query parameters.
    /// </summary>
    /// <short>Delete favorite files and folders (using query parameters)</short>
    /// <category>Operations</category>
    /// <param type="ASC.Files.Core.ApiModels.RequestDto.BaseBatchRequestDto, ASC.Files.Core" name="inDto">Base batch request parameters</param>
    /// <returns type="System.Boolean, System">Boolean value: true if the operation is successful</returns>
    /// <path>api/2.0/files/favorites</path>
    /// <httpMethod>DELETE</httpMethod>
    /// <visible>false</visible>
    [HttpDelete("favorites")]
    public async Task<bool> DeleteFavoritesFromQueryAsync([FromQuery][ModelBinder(BinderType = typeof(BaseBatchModelBinder))] BaseBatchRequestDto inDto)
    {
        return await DeleteFavoritesAsync(inDto);
    }

    /// <summary>
    /// Removes files with the IDs specified in the request from the template list.
    /// </summary>
    /// <short>Delete template files</short>
    /// <category>Files</category>
    /// <param type="System.Collections.Generic.IEnumerable{System.Int32}, System.Collections.Generic" name="fileIds">List of file IDs</param>
    /// <returns type="System.Boolean, System">Boolean value: true if the operation is successful</returns>
    /// <path>api/2.0/files/templates</path>
    /// <httpMethod>DELETE</httpMethod>
    [HttpDelete("templates")]
    public async Task<bool> DeleteTemplatesAsync(IEnumerable<int> fileIds)
    {
        await _fileStorageService.DeleteTemplatesAsync(fileIds);

        return true;
    }

    private async Task<bool> DeleteFavoritesAsync(BaseBatchRequestDto inDto)
    {
        var (folderIntIds, folderStringIds) = FileOperationsManager.GetIds(inDto.FolderIds);
        var (fileIntIds, fileStringIds) = FileOperationsManager.GetIds(inDto.FileIds);

        await _fileStorageService.DeleteFavoritesAsync(folderIntIds, fileIntIds);
        await _fileStorageService.DeleteFavoritesAsync(folderStringIds, fileStringIds);

        return true;
    }
}