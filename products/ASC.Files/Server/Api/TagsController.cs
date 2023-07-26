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

    [HttpPost("file/{fileId}/recent")]
    public async Task<FileDto<T>> AddToRecentAsync(T fileId)
    {
        var file = await _fileStorageService.GetFileAsync(fileId, -1).NotFoundIfNull("File not found");

        await _entryManager.MarkAsRecent(file);

        return await _fileDtoHelper.GetAsync(file);
    }

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
    /// Adding files to favorite list
    /// </summary>
    /// <short>Favorite add</short>
    /// <category>Files</category>
    /// <param name="folderIds" visible="false"></param>
    /// <param name="fileIds">File IDs</param>
    /// <returns></returns>
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
    /// Adding files to template list
    /// </summary>
    /// <short>Template add</short>
    /// <category>Files</category>
    /// <param name="fileIds">File IDs</param>
    /// <returns></returns>
    [HttpPost("templates")]
    public async Task<bool> AddTemplatesAsync(TemplatesRequestDto inDto)
    {
        await _fileStorageService.AddToTemplatesAsync(inDto.FileIds);

        return true;
    }

    /// <summary>
    /// Removing files from favorite list
    /// </summary>
    /// <short>Favorite delete</short>
    /// <category>Files</category>
    /// <param name="folderIds" visible="false"></param>
    /// <param name="fileIds">File IDs</param>
    /// <returns></returns>
    [HttpDelete("favorites")]
    [Consumes("application/json")]
    public async Task<bool> DeleteFavoritesFromBodyAsync([FromBody] BaseBatchRequestDto inDto)
    {
        return await DeleteFavoritesAsync(inDto);
    }

    [HttpDelete("favorites")]
    public async Task<bool> DeleteFavoritesFromQueryAsync([FromQuery][ModelBinder(BinderType = typeof(BaseBatchModelBinder))] BaseBatchRequestDto inDto)
    {
        return await DeleteFavoritesAsync(inDto);
    }

    /// <summary>
    /// Removing files from template list
    /// </summary>
    /// <short>Template delete</short>
    /// <category>Files</category>
    /// <param name="fileIds">File IDs</param>
    /// <returns></returns>
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