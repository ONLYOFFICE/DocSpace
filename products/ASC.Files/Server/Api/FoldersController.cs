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
public class FoldersControllerInternal : FoldersController<int>
{
    public FoldersControllerInternal(
        EntryManager entryManager,
        FoldersControllerHelper foldersControllerHelper,
        FileStorageService fileStorageService,
        FileOperationDtoHelper fileOperationDtoHelper,
        FolderDtoHelper folderDtoHelper,
        FileDtoHelper fileDtoHelper)
        : base(entryManager, foldersControllerHelper, fileStorageService, fileOperationDtoHelper, folderDtoHelper, fileDtoHelper)
    {
    }
}

public class FoldersControllerThirdparty : FoldersController<string>
{
    public FoldersControllerThirdparty(
        EntryManager entryManager,
        FoldersControllerHelper foldersControllerHelper,
        FileStorageService fileStorageService,
        FileOperationDtoHelper fileOperationDtoHelper,
        FolderDtoHelper folderDtoHelper,
        FileDtoHelper fileDtoHelper)
        : base(entryManager, foldersControllerHelper, fileStorageService, fileOperationDtoHelper, folderDtoHelper, fileDtoHelper)
    {
    }
}

public abstract class FoldersController<T> : ApiControllerBase
{
    private readonly EntryManager _entryManager;
    private readonly FoldersControllerHelper _foldersControllerHelper;
    private readonly FileStorageService _fileStorageService;
    private readonly FileOperationDtoHelper _fileOperationDtoHelper;

    public FoldersController(
        EntryManager entryManager,
        FoldersControllerHelper foldersControllerHelper,
        FileStorageService fileStorageService,
        FileOperationDtoHelper fileOperationDtoHelper,
        FolderDtoHelper folderDtoHelper,
        FileDtoHelper fileDtoHelper) : base(folderDtoHelper, fileDtoHelper)
    {
        _entryManager = entryManager;
        _foldersControllerHelper = foldersControllerHelper;
        _fileStorageService = fileStorageService;
        _fileOperationDtoHelper = fileOperationDtoHelper;
    }

    /// <summary>
    /// Creates a new folder with the title sent in the request. The ID of a parent folder can be also specified.
    /// </summary>
    /// <short>
    /// New folder
    /// </short>
    /// <category>Folders</category>
    /// <param name="folderId">Parent folder ID</param>
    /// <param name="title">Title of new folder</param>
    /// <returns>New folder contents</returns>
    [HttpPost("folder/{folderId}")]
    public async Task<FolderDto<T>> CreateFolderAsync(T folderId, CreateFolderRequestDto inDto)
    {
        return await _foldersControllerHelper.CreateFolderAsync(folderId, inDto.Title);
    }

    /// <summary>
    /// Deletes the folder with the ID specified in the request
    /// </summary>
    /// <short>Delete folder</short>
    /// <category>Folders</category>
    /// <param name="folderId">Folder ID</param>
    /// <param name="deleteAfter">Delete after finished</param>
    /// <param name="immediately">Don't move to the Recycle Bin</param>
    /// <returns>Operation result</returns>
    [HttpDelete("folder/{folderId}")]
    public async IAsyncEnumerable<FileOperationDto> DeleteFolder(T folderId, DeleteFolderDto model)
    {
        foreach (var e in await _fileStorageService.DeleteFolderAsync("delete", folderId, false, model.DeleteAfter, model.Immediately))
        {
            yield return await _fileOperationDtoHelper.GetAsync(e);
        }
    }

    /// <summary>
    /// Returns the detailed list of files and folders located in the folder with the ID specified in the request
    /// </summary>
    /// <short>
    /// Folder by ID
    /// </short>
    /// <category>Folders</category>
    /// <param name="folderId">Folder ID</param>
    /// <param name="userIdOrGroupId" optional="true">User or group ID</param>
    /// <param name="filterType" optional="true" remark="Allowed values: None (0), FilesOnly (1), FoldersOnly (2), DocumentsOnly (3), PresentationsOnly (4), SpreadsheetsOnly (5) or ImagesOnly (7)">Filter type</param>
    /// <returns>Folder contents</returns>
    [HttpGet("{folderId}", Order = 1)]
    public async Task<FolderContentDto<T>> GetFolderAsync(T folderId, Guid? userIdOrGroupId, FilterType? filterType, T roomId, bool? searchInContent, bool? withsubfolders, bool? excludeSubject)
    {
        var folder = await _foldersControllerHelper.GetFolderAsync(folderId, userIdOrGroupId, filterType, roomId, searchInContent, withsubfolders, excludeSubject);

        return folder.NotFoundIfNull();
    }

    /// <summary>
    /// Returns a detailed information about the folder with the ID specified in the request
    /// </summary>
    /// <short>Folder information</short>
    /// <category>Folders</category>
    /// <returns>Folder info</returns>
    [HttpGet("folder/{folderId}")]
    public async Task<FolderDto<T>> GetFolderInfoAsync(T folderId)
    {
        return await _foldersControllerHelper.GetFolderInfoAsync(folderId);
    }

    /// <summary>
    /// Returns parent folders
    /// </summary>
    /// <param name="folderId"></param>
    /// <category>Folders</category>
    /// <returns>Parent folders</returns>
    [HttpGet("folder/{folderId}/path")]
    public async IAsyncEnumerable<FileEntryDto> GetFolderPathAsync(T folderId)
    {
        var breadCrumbs = await _entryManager.GetBreadCrumbsAsync(folderId);

        foreach (var e in breadCrumbs)
        {
            yield return await GetFileEntryWrapperAsync(e);
        }
    }

    [HttpGet("{folderId}/subfolders")]
    public async IAsyncEnumerable<FileEntryDto> GetFoldersAsync(T folderId)
    {
        var folders = await _fileStorageService.GetFoldersAsync(folderId);
        foreach (var folder in folders)
        {
            yield return await GetFileEntryWrapperAsync(folder);
        }
    }

    [HttpGet("{folderId}/news")]
    public async IAsyncEnumerable<FileEntryDto> GetNewItemsAsync(T folderId)
    {
        var newItems = await _fileStorageService.GetNewItemsAsync(folderId);

        foreach (var e in newItems)
        {
            yield return await GetFileEntryWrapperAsync(e);
        }
    }

    /// <summary>
    /// Renames the selected folder to the new title specified in the request
    /// </summary>
    /// <short>
    /// Rename folder
    /// </short>
    /// <category>Folders</category>
    /// <param name="folderId">Folder ID</param>
    /// <param name="title">New title</param>
    /// <returns>Folder contents</returns>
    [HttpPut("folder/{folderId}")]
    public async Task<FolderDto<T>> RenameFolderAsync(T folderId, CreateFolderRequestDto inDto)
    {
        return await _foldersControllerHelper.RenameFolderAsync(folderId, inDto.Title);
    }
}

public class FoldersControllerCommon : ApiControllerBase
{
    private readonly GlobalFolderHelper _globalFolderHelper;
    private readonly FoldersControllerHelper _foldersControllerHelper;

    public FoldersControllerCommon(
        GlobalFolderHelper globalFolderHelper,
        FoldersControllerHelper foldersControllerHelper,
        FolderDtoHelper folderDtoHelper,
        FileDtoHelper fileDtoHelper) : base(folderDtoHelper, fileDtoHelper)
    {
        _globalFolderHelper = globalFolderHelper;
        _foldersControllerHelper = foldersControllerHelper;
    }

    /// <summary>
    /// Returns the detailed list of files and folders located in the 'Common Documents' section
    /// </summary>
    /// <short>
    /// Common folder
    /// </short>
    /// <category>Folders</category>
    /// <returns>Common folder contents</returns>
    [HttpGet("@common")]
    public async Task<FolderContentDto<int>> GetCommonFolderAsync(Guid? userIdOrGroupId, FilterType? filterType, bool? searchInContent, bool? withsubfolders)
    {
        return await _foldersControllerHelper.GetFolderAsync(await _globalFolderHelper.FolderCommonAsync, userIdOrGroupId, filterType, default, searchInContent, withsubfolders, false);
    }

    /// <summary>
    /// Returns the detailed list of favorites files
    /// </summary>
    /// <short>Section Favorite</short>
    /// <category>Folders</category>
    /// <returns>Favorites contents</returns>
    [HttpGet("@favorites")]
    public async Task<FolderContentDto<int>> GetFavoritesFolderAsync(Guid? userIdOrGroupId, FilterType? filterType, bool? searchInContent, bool? withsubfolders)
    {
        return await _foldersControllerHelper.GetFolderAsync(await _globalFolderHelper.FolderFavoritesAsync, userIdOrGroupId, filterType, default, searchInContent, withsubfolders, false);
    }

    /// <summary>
    /// Returns the detailed list of files and folders located in the current user 'My Documents' section
    /// </summary>
    /// <short>
    /// My folder
    /// </short>
    /// <category>Folders</category>
    /// <returns>My folder contents</returns>
    [HttpGet("@my")]
    public async Task<FolderContentDto<int>> GetMyFolderAsync(Guid? userIdOrGroupId, FilterType? filterType, bool? searchInContent, bool? withsubfolders)
    {
        return await _foldersControllerHelper.GetFolderAsync(await _globalFolderHelper.FolderMyAsync, userIdOrGroupId, filterType, default, searchInContent, withsubfolders, false);
    }

    [HttpGet("@privacy")]
    public async Task<FolderContentDto<int>> GetPrivacyFolderAsync(Guid? userIdOrGroupId, FilterType? filterType, bool? searchInContent, bool? withsubfolders)
    {
        if (PrivacyRoomSettings.IsAvailable())
        {
            throw new SecurityException();
        }

        return await _foldersControllerHelper.GetFolderAsync(await _globalFolderHelper.FolderPrivacyAsync, userIdOrGroupId, filterType, default, searchInContent, withsubfolders, false);
    }

    /// <summary>
    /// Returns the detailed list of files and folders located in the current user 'Projects Documents' section
    /// </summary>
    /// <short>
    /// Projects folder
    /// </short>
    /// <category>Folders</category>
    /// <returns>Projects folder contents</returns>
    [HttpGet("@projects")]
    public async Task<FolderContentDto<string>> GetProjectsFolderAsync(Guid? userIdOrGroupId, FilterType? filterType, bool? searchInContent, bool? withsubfolders)
    {
        return await _foldersControllerHelper.GetFolderAsync(await _globalFolderHelper.GetFolderProjectsAsync<string>(), userIdOrGroupId, filterType, default, searchInContent, withsubfolders, false);
    }

    /// <summary>
    /// Returns the detailed list of recent files
    /// </summary>
    /// <short>Section Recent</short>
    /// <category>Folders</category>
    /// <returns>Recent contents</returns>
    [HttpGet("@recent")]
    public async Task<FolderContentDto<int>> GetRecentFolderAsync(Guid? userIdOrGroupId, FilterType? filterType, bool? searchInContent, bool? withsubfolders)
    {
        return await _foldersControllerHelper.GetFolderAsync(await _globalFolderHelper.FolderRecentAsync, userIdOrGroupId, filterType, default, searchInContent, withsubfolders, false);
    }

    [HttpGet("@root")]
    public async IAsyncEnumerable<FolderContentDto<int>> GetRootFoldersAsync(Guid? userIdOrGroupId, FilterType? filterType, bool? withsubfolders, bool? withoutTrash, bool? searchInContent, bool? withoutAdditionalFolder)
    {
        var foldersIds = _foldersControllerHelper.GetRootFoldersIdsAsync(withoutTrash ?? false, withoutAdditionalFolder ?? false);

        await foreach (var folder in foldersIds)
        {
            yield return await _foldersControllerHelper.GetFolderAsync(folder, userIdOrGroupId, filterType, default, searchInContent, withsubfolders, false);
        }
    }

    /// <summary>
    /// Returns the detailed list of files and folders located in the 'Shared with Me' section
    /// </summary>
    /// <short>
    /// Shared folder
    /// </short>
    /// <category>Folders</category>
    /// <returns>Shared folder contents</returns>
    [HttpGet("@share")]
    public async Task<FolderContentDto<int>> GetShareFolderAsync(Guid? userIdOrGroupId, FilterType? filterType, bool? searchInContent, bool? withsubfolders)
    {
        return await _foldersControllerHelper.GetFolderAsync(await _globalFolderHelper.FolderShareAsync, userIdOrGroupId, filterType, default, searchInContent, withsubfolders, false);
    }

    /// <summary>
    /// Returns the detailed list of templates files
    /// </summary>
    /// <short>Section Template</short>
    /// <category>Folders</category>
    /// <returns>Templates contents</returns>
    [HttpGet("@templates")]
    public async Task<FolderContentDto<int>> GetTemplatesFolderAsync(Guid? userIdOrGroupId, FilterType? filterType, bool? searchInContent, bool? withsubfolders)
    {
        return await _foldersControllerHelper.GetFolderAsync(await _globalFolderHelper.FolderTemplatesAsync, userIdOrGroupId, filterType, default, searchInContent, withsubfolders, false);
    }

    /// <summary>
    /// Returns the detailed list of files and folders located in the 'Recycle Bin' section
    /// </summary>
    /// <short>
    /// Trash folder
    /// </short>
    /// <category>Folders</category>
    /// <returns>Trash folder contents</returns>
    [HttpGet("@trash")]
    public async Task<FolderContentDto<int>> GetTrashFolderAsync(Guid? userIdOrGroupId, FilterType? filterType, bool? searchInContent, bool? withsubfolders)
    {
        return await _foldersControllerHelper.GetFolderAsync(Convert.ToInt32(await _globalFolderHelper.FolderTrashAsync), userIdOrGroupId, filterType, default, searchInContent, withsubfolders, false);
    }
}