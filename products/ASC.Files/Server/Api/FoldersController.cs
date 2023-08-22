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
    /// Creates a new folder with the title specified in the request. The parent folder ID can be also specified.
    /// </summary>
    /// <short>
    /// Create a folder
    /// </short>
    /// <category>Folders</category>
    /// <param type="System.Int32, System" method="url" name="folderId">Parent folder ID</param>
    /// <param type="ASC.Files.Core.ApiModels.RequestDto.CreateFolderRequestDto, ASC.Files.Core" name="inDto">Request parameters for creating a folder</param>
    /// <returns type="ASC.Files.Core.ApiModels.ResponseDto.FolderDto, ASC.Files.Core">New folder parameters</returns>
    /// <path>api/2.0/files/folder/{folderId}</path>
    /// <httpMethod>POST</httpMethod>
    [HttpPost("folder/{folderId}")]
    public async Task<FolderDto<T>> CreateFolderAsync(T folderId, CreateFolderRequestDto inDto)
    {
        return await _foldersControllerHelper.CreateFolderAsync(folderId, inDto.Title);
    }

    /// <summary>
    /// Deletes a folder with the ID specified in the request.
    /// </summary>
    /// <short>Delete a folder</short>
    /// <category>Folders</category>
    /// <param type="System.Int32, System" method="url" name="folderId">Folder ID</param>
    /// <param type="ASC.Files.Core.ApiModels.RequestDto.DeleteFolderDto, ASC.Files.Core" name="inDto">Request parameters for deleting a folder</param>
    /// <returns type="ASC.Files.Core.ApiModels.ResponseDto.FileOperationDto, ASC.Files.Core">List of file operations</returns>
    /// <path>api/2.0/files/folder/{folderId}</path>
    /// <httpMethod>DELETE</httpMethod>
    /// <collection>list</collection>
    [HttpDelete("folder/{folderId}")]
    public async IAsyncEnumerable<FileOperationDto> DeleteFolder(T folderId, DeleteFolderDto inDto)
    {
        foreach (var e in await _fileStorageService.DeleteFolderAsync("delete", folderId, false, inDto.DeleteAfter, inDto.Immediately))
        {
            yield return await _fileOperationDtoHelper.GetAsync(e);
        }
    }

    /// <summary>
    /// Returns the detailed list of files and folders located in the folder with the ID specified in the request.
    /// </summary>
    /// <short>
    /// Get a folder by ID
    /// </short>
    /// <category>Folders</category>
    /// <param type="System.Int32, System" method="url" name="folderId">Folder ID</param>
    /// <param type="System.Nullable{System.Guid}, System" name="userIdOrGroupId" optional="true">User or group ID</param>
    /// <param type="System.Nullable{ASC.Files.Core.FilterType}, System" name="filterType" optional="true" remark="Allowed values: None (0), FilesOnly (1), FoldersOnly (2), DocumentsOnly (3), PresentationsOnly (4), SpreadsheetsOnly (5), ImagesOnly (7), ByUser (8), ByDepartment (9), ArchiveOnly (10), ByExtension (11), MediaOnly (12), EditingRooms (14), CustomRooms (17), OFormTemplateOnly (18), OFormOnly (19)">Filter type</param>
    /// <param type="System.Int32, System" name="roomId">Room ID</param>
    /// <param type="System.Nullable{System.Boolean}, System" name="searchInContent">Specifies whether to search within the section contents or not</param>
    /// <param type="System.Nullable{System.Boolean}, System" name="withsubfolders">Specifies whether to return sections with or without subfolders</param>
    /// <param type="System.Nullable{System.Boolean}, System" name="excludeSubject">Specifies whether to exclude a subject or not</param>
    /// <param type="System.Nullable{ASC.Files.Core.Core.ApplyFilterOption}, System" name="applyFilterOption"></param>
    /// <returns type="ASC.Files.Core.ApiModels.ResponseDto.FolderContentDto, ASC.Files.Core">Folder contents</returns>
    /// <path>api/2.0/files/{folderId}</path>
    /// <httpMethod>GET</httpMethod>
    [AllowAnonymous]
    [HttpGet("{folderId}")]
    public async Task<FolderContentDto<T>> GetFolderAsync(T folderId, Guid? userIdOrGroupId, FilterType? filterType, T roomId, bool? searchInContent, bool? withsubfolders, bool? excludeSubject, 
        ApplyFilterOption? applyFilterOption)
    {
        var folder = await _foldersControllerHelper.GetFolderAsync(folderId, userIdOrGroupId, filterType, roomId, searchInContent, withsubfolders, excludeSubject, applyFilterOption);

        return folder.NotFoundIfNull();
    }

    /// <summary>
    /// Returns the detailed information about a folder with the ID specified in the request.
    /// </summary>
    /// <short>Get folder information</short>
    /// <param type="System.Int32, System" method="url" name="folderId">Folder ID</param>
    /// <category>Folders</category>
    /// <returns type="ASC.Files.Core.ApiModels.ResponseDto.FolderDto, ASC.Files.Core">Folder parameters</returns>
    /// <path>api/2.0/files/folder/{folderId}</path>
    /// <httpMethod>GET</httpMethod>
    [AllowAnonymous]
    [HttpGet("folder/{folderId}")]
    public async Task<FolderDto<T>> GetFolderInfoAsync(T folderId)
    {
        return await _foldersControllerHelper.GetFolderInfoAsync(folderId);
    }

    /// <summary>
    /// Returns a path to the folder with the ID specified in the request.
    /// </summary>
    /// <short>Get the folder path</short>
    /// <param type="System.Int32, System" method="url" name="folderId">Folder ID</param>
    /// <category>Folders</category>
    /// <returns type="ASC.Files.Core.ApiModels.ResponseDto.FileEntryDto, ASC.Files.Core">List of file entry information</returns>
    /// <path>api/2.0/files/folder/{folderId}/path</path>
    /// <httpMethod>GET</httpMethod>
    /// <collection>list</collection>
    [HttpGet("folder/{folderId}/path")]
    public async IAsyncEnumerable<FileEntryDto> GetFolderPathAsync(T folderId)
    {
        var breadCrumbs = await _entryManager.GetBreadCrumbsAsync(folderId);

        foreach (var e in breadCrumbs)
        {
            yield return await GetFileEntryWrapperAsync(e);
        }
    }

    /// <summary>
    /// Returns a list of all the subfolders from a folder with the ID specified in the request.
    /// </summary>
    /// <short>Get subfolders</short>
    /// <param type="System.Int32, System" method="url" name="folderId">Folder ID</param>
    /// <category>Folders</category>
    /// <returns type="ASC.Files.Core.ApiModels.ResponseDto.FileEntryDto, ASC.Files.Core">List of file entry information</returns>
    /// <path>api/2.0/files/{folderId}/subfolders</path>
    /// <httpMethod>GET</httpMethod>
    /// <collection>list</collection>
    [HttpGet("{folderId}/subfolders")]
    public async IAsyncEnumerable<FileEntryDto> GetFoldersAsync(T folderId)
    {
        var folders = await _fileStorageService.GetFoldersAsync(folderId);
        foreach (var folder in folders)
        {
            yield return await GetFileEntryWrapperAsync(folder);
        }
    }

    /// <summary>
    /// Returns a list of all the new items from a folder with the ID specified in the request.
    /// </summary>
    /// <short>Get new folder items</short>
    /// <param type="System.Int32, System" method="url" name="folderId">Folder ID</param>
    /// <category>Folders</category>
    /// <returns type="ASC.Files.Core.ApiModels.ResponseDto.FileEntryDto, ASC.Files.Core">List of file entry information</returns>
    /// <path>api/2.0/files/{folderId}/news</path>
    /// <httpMethod>GET</httpMethod>
    /// <collection>list</collection>
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
    /// Renames the selected folder with a new title specified in the request.
    /// </summary>
    /// <short>
    /// Rename a folder
    /// </short>
    /// <category>Folders</category>
    /// <param type="System.Int32, System" method="url" name="folderId">Folder ID</param>
    /// <param type="ASC.Files.Core.ApiModels.RequestDto.CreateFolderRequestDto, ASC.Files.Core" name="inDto">Request parameters for creating a folder: Title (string) - new folder title</param>
    /// <returns type="ASC.Files.Core.ApiModels.ResponseDto.FolderDto, ASC.Files.Core">Folder parameters</returns>
    /// <path>api/2.0/files/folder/{folderId}</path>
    /// <httpMethod>PUT</httpMethod>
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
    /// Returns the detailed list of files and folders located in the "Common" section.
    /// </summary>
    /// <short>Get the "Common" section</short>
    /// <category>Folders</category>
    /// <param type="System.Nullable{System.Guid}, System" name="userIdOrGroupId" optional="true">User or group ID</param>
    /// <param type="System.Nullable{ASC.Files.Core.FilterType}, System" name="filterType" optional="true" remark="Allowed values: None (0), FilesOnly (1), FoldersOnly (2), DocumentsOnly (3), PresentationsOnly (4), SpreadsheetsOnly (5), ImagesOnly (7), ByUser (8), ByDepartment (9), ArchiveOnly (10), ByExtension (11), MediaOnly (12), EditingRooms (14), CustomRooms (17), OFormTemplateOnly (18), OFormOnly (19)">Filter type</param>
    /// <param type="System.Nullable{System.Boolean}, System" name="searchInContent">Specifies whether to search within the section contents or not</param>
    /// <param type="System.Nullable{System.Boolean}, System" name="withsubfolders">Specifies whether to return sections with or without subfolders</param>
    /// <returns type="ASC.Files.Core.ApiModels.ResponseDto.FolderContentDto, ASC.Files.Core">The "Common" section contents</returns>
    /// <path>api/2.0/files/@common</path>
    /// <httpMethod>GET</httpMethod>
    [HttpGet("@common")]
    public async Task<FolderContentDto<int>> GetCommonFolderAsync(Guid? userIdOrGroupId, FilterType? filterType, bool? searchInContent, bool? withsubfolders)
    {
        return await _foldersControllerHelper.GetFolderAsync(await _globalFolderHelper.FolderCommonAsync, userIdOrGroupId, filterType, default, searchInContent, withsubfolders, 
            false, ApplyFilterOption.All);
    }

    /// <summary>
    /// Returns the detailed list of files and folders located in the "Favorites" section.
    /// </summary>
    /// <short>Get the "Favorites" section</short>
    /// <category>Folders</category>
    /// <param type="System.Nullable{System.Guid}, System" name="userIdOrGroupId" optional="true">User or group ID</param>
    /// <param type="System.Nullable{ASC.Files.Core.FilterType}, System" name="filterType" optional="true" remark="Allowed values: None (0), FilesOnly (1), FoldersOnly (2), DocumentsOnly (3), PresentationsOnly (4), SpreadsheetsOnly (5), ImagesOnly (7), ByUser (8), ByDepartment (9), ArchiveOnly (10), ByExtension (11), MediaOnly (12), EditingRooms (14), CustomRooms (17), OFormTemplateOnly (18), OFormOnly (19)">Filter type</param>
    /// <param type="System.Nullable{System.Boolean}, System" name="searchInContent">Specifies whether to search within the section contents or not</param>
    /// <param type="System.Nullable{System.Boolean}, System" name="withsubfolders">Specifies whether to return sections with or without subfolders</param>
    /// <returns type="ASC.Files.Core.ApiModels.ResponseDto.FolderContentDto, ASC.Files.Core">The "Favorites" section contents</returns>
    /// <path>api/2.0/files/@favorites</path>
    /// <httpMethod>GET</httpMethod>
    [HttpGet("@favorites")]
    public async Task<FolderContentDto<int>> GetFavoritesFolderAsync(Guid? userIdOrGroupId, FilterType? filterType, bool? searchInContent, bool? withsubfolders)
    {
        return await _foldersControllerHelper.GetFolderAsync(await _globalFolderHelper.FolderFavoritesAsync, userIdOrGroupId, filterType, default, searchInContent, withsubfolders, 
            false, ApplyFilterOption.All);
    }

    /// <summary>
    /// Returns the detailed list of files and folders located in the "My documents" section.
    /// </summary>
    /// <short>Get the "My documents" section</short>
    /// <category>Folders</category>
    /// <param type="System.Nullable{System.Guid}, System" name="userIdOrGroupId" optional="true">User or group ID</param>
    /// <param type="System.Nullable{ASC.Files.Core.FilterType}, System" name="filterType" optional="true" remark="Allowed values: None (0), FilesOnly (1), FoldersOnly (2), DocumentsOnly (3), PresentationsOnly (4), SpreadsheetsOnly (5), ImagesOnly (7), ByUser (8), ByDepartment (9), ArchiveOnly (10), ByExtension (11), MediaOnly (12), EditingRooms (14), CustomRooms (17), OFormTemplateOnly (18), OFormOnly (19)">Filter type</param>
    /// <param type="System.Nullable{System.Boolean}, System" name="searchInContent">Specifies whether to search within the section contents or not</param>
    /// <param type="System.Nullable{System.Boolean}, System" name="withsubfolders">Specifies whether to return sections with or without subfolders</param>
    /// <param type="System.Nullable{ASC.Files.Core.Core.ApplyFilterOption}, System" name="applyFilterOption"></param>
    /// <returns type="ASC.Files.Core.ApiModels.ResponseDto.FolderContentDto, ASC.Files.Core">The "My documents" section contents</returns>
    /// <path>api/2.0/files/@my</path>
    /// <httpMethod>GET</httpMethod>
    [HttpGet("@my")]
    public async Task<FolderContentDto<int>> GetMyFolderAsync(Guid? userIdOrGroupId, FilterType? filterType, bool? searchInContent, bool? withsubfolders, ApplyFilterOption? applyFilterOption)
    {
        return await _foldersControllerHelper.GetFolderAsync(await _globalFolderHelper.FolderMyAsync, userIdOrGroupId, filterType, default, searchInContent, withsubfolders, 
            false, applyFilterOption);
    }

    /// <summary>
    /// Returns the detailed list of files and folders located in the "Private Room" section.
    /// </summary>
    /// <short>Get the "Private Room" section</short>
    /// <category>Folders</category>
    /// <param type="System.Nullable{System.Guid}, System" name="userIdOrGroupId" optional="true">User or group ID</param>
    /// <param type="System.Nullable{ASC.Files.Core.FilterType}, System" name="filterType" optional="true" remark="Allowed values: None (0), FilesOnly (1), FoldersOnly (2), DocumentsOnly (3), PresentationsOnly (4), SpreadsheetsOnly (5), ImagesOnly (7), ByUser (8), ByDepartment (9), ArchiveOnly (10), ByExtension (11), MediaOnly (12), EditingRooms (14), CustomRooms (17), OFormTemplateOnly (18), OFormOnly (19)">Filter type</param>
    /// <param type="System.Nullable{System.Boolean}, System" name="searchInContent">Specifies whether to search within the section contents or not</param>
    /// <param type="System.Nullable{System.Boolean}, System" name="withsubfolders">Specifies whether to return sections with or without subfolders</param>
    /// <returns type="ASC.Files.Core.ApiModels.ResponseDto.FolderContentDto, ASC.Files.Core">The "Private Room" section contents</returns>
    /// <path>api/2.0/files/@privacy</path>
    /// <httpMethod>GET</httpMethod>
    [HttpGet("@privacy")]
    public async Task<FolderContentDto<int>> GetPrivacyFolderAsync(Guid? userIdOrGroupId, FilterType? filterType, bool? searchInContent, bool? withsubfolders)
    {
        if (PrivacyRoomSettings.IsAvailable())
        {
            throw new SecurityException();
        }

        return await _foldersControllerHelper.GetFolderAsync(await _globalFolderHelper.FolderPrivacyAsync, userIdOrGroupId, filterType, default, searchInContent, withsubfolders, 
            false, ApplyFilterOption.All);
    }

    /// <summary>
    /// Returns the detailed list of files and folders located in the "In projects" section.
    /// </summary>
    /// <short>Get the "In projects" section</short>
    /// <category>Folders</category>
    /// <param type="System.Nullable{System.Guid}, System" name="userIdOrGroupId" optional="true">User or group ID</param>
    /// <param type="System.Nullable{ASC.Files.Core.FilterType}, System" name="filterType" optional="true" remark="Allowed values: None (0), FilesOnly (1), FoldersOnly (2), DocumentsOnly (3), PresentationsOnly (4), SpreadsheetsOnly (5), ImagesOnly (7), ByUser (8), ByDepartment (9), ArchiveOnly (10), ByExtension (11), MediaOnly (12), EditingRooms (14), CustomRooms (17), OFormTemplateOnly (18), OFormOnly (19)">Filter type</param>
    /// <param type="System.Nullable{System.Boolean}, System" name="searchInContent">Specifies whether to search within the section contents or not</param>
    /// <param type="System.Nullable{System.Boolean}, System" name="withsubfolders">Specifies whether to return sections with or without subfolders</param>
    /// <returns type="ASC.Files.Core.ApiModels.ResponseDto.FolderContentDto, ASC.Files.Core">The "In projects" section contents</returns>
    /// <path>api/2.0/files/@projects</path>
    /// <httpMethod>GET</httpMethod>
    [HttpGet("@projects")]
    public async Task<FolderContentDto<string>> GetProjectsFolderAsync(Guid? userIdOrGroupId, FilterType? filterType, bool? searchInContent, bool? withsubfolders)
    {
        return await _foldersControllerHelper.GetFolderAsync(await _globalFolderHelper.GetFolderProjectsAsync<string>(), userIdOrGroupId, filterType, default, searchInContent, withsubfolders, 
            false, ApplyFilterOption.All);
    }

    /// <summary>
    /// Returns the detailed list of files located in the "Recent" section.
    /// </summary>
    /// <short>Get the "Recent" section</short>
    /// <category>Folders</category>
    /// <param type="System.Nullable{System.Guid}, System" name="userIdOrGroupId" optional="true">User or group ID</param>
    /// <param type="System.Nullable{ASC.Files.Core.FilterType}, System" name="filterType" optional="true" remark="Allowed values: None (0), FilesOnly (1), FoldersOnly (2), DocumentsOnly (3), PresentationsOnly (4), SpreadsheetsOnly (5), ImagesOnly (7), ByUser (8), ByDepartment (9), ArchiveOnly (10), ByExtension (11), MediaOnly (12), EditingRooms (14), CustomRooms (17), OFormTemplateOnly (18), OFormOnly (19)">Filter type</param>
    /// <param type="System.Nullable{System.Boolean}, System" name="searchInContent">Specifies whether to search within the section contents or not</param>
    /// <param type="System.Nullable{System.Boolean}, System" name="withsubfolders">Specifies whether to return sections with or without subfolders</param>
    /// <returns type="ASC.Files.Core.ApiModels.ResponseDto.FolderContentDto, ASC.Files.Core">The "Recent" section contents</returns>
    /// <path>api/2.0/files/@recent</path>
    /// <httpMethod>GET</httpMethod>
    [HttpGet("@recent")]
    public async Task<FolderContentDto<int>> GetRecentFolderAsync(Guid? userIdOrGroupId, FilterType? filterType, bool? searchInContent, bool? withsubfolders)
    {
        return await _foldersControllerHelper.GetFolderAsync(await _globalFolderHelper.FolderRecentAsync, userIdOrGroupId, filterType, default, searchInContent, withsubfolders, 
            false, ApplyFilterOption.All);
    }

    /// <summary>
    /// Returns all the sections matching the parameters specified in the request.
    /// </summary>
    /// <short>Get filtered sections</short>
    /// <param type="System.Nullable{System.Guid}, System" name="userIdOrGroupId" optional="true">User or group ID</param>
    /// <param type="System.Nullable{ASC.Files.Core.FilterType}, System" name="filterType" optional="true" remark="Allowed values: None (0), FilesOnly (1), FoldersOnly (2), DocumentsOnly (3), PresentationsOnly (4), SpreadsheetsOnly (5), ImagesOnly (7), ByUser (8), ByDepartment (9), ArchiveOnly (10), ByExtension (11), MediaOnly (12), EditingRooms (14), CustomRooms (17), OFormTemplateOnly (18), OFormOnly (19)">Filter type</param>
    /// <param type="System.Nullable{System.Boolean}, System" name="withsubfolders">Specifies whether to return sections with or without subfolders</param>
    /// <param type="System.Nullable{System.Boolean}, System" name="withoutTrash">Specifies whether to return the "Trash" section or not</param>
    /// <param type="System.Nullable{System.Boolean}, System" name="searchInContent">Specifies whether to search within the section contents or not</param>
    /// <param type="System.Nullable{System.Boolean}, System" name="withoutAdditionalFolder">Specifies whether to return sections with or without additional folders</param>
    /// <category>Folders</category>
    /// <returns type="ASC.Files.Core.ApiModels.ResponseDto.FolderContentDto, ASC.Files.Core">List of section contents with the following parameters</returns>
    /// <path>api/2.0/files/@root</path>
    /// <httpMethod>GET</httpMethod>
    /// <collection>list</collection>
    [HttpGet("@root")]
    public async IAsyncEnumerable<FolderContentDto<int>> GetRootFoldersAsync(Guid? userIdOrGroupId, FilterType? filterType, bool? withsubfolders, bool? withoutTrash, bool? searchInContent, bool? withoutAdditionalFolder)
    {
        var foldersIds = _foldersControllerHelper.GetRootFoldersIdsAsync(withoutTrash ?? false, withoutAdditionalFolder ?? false);

        await foreach (var folder in foldersIds)
        {
            yield return await _foldersControllerHelper.GetFolderAsync(folder, userIdOrGroupId, filterType, default, searchInContent, withsubfolders, false, 
                ApplyFilterOption.All);
        }
    }

    /// <summary>
    /// Returns the detailed list of files and folders located in the "Shared with me" section.
    /// </summary>
    /// <short>Get the "Shared with me" section</short>
    /// <category>Folders</category>
    /// <param type="System.Nullable{System.Guid}, System" name="userIdOrGroupId" optional="true">User or group ID</param>
    /// <param type="System.Nullable{ASC.Files.Core.FilterType}, System" name="filterType" optional="true" remark="Allowed values: None (0), FilesOnly (1), FoldersOnly (2), DocumentsOnly (3), PresentationsOnly (4), SpreadsheetsOnly (5), ImagesOnly (7), ByUser (8), ByDepartment (9), ArchiveOnly (10), ByExtension (11), MediaOnly (12), EditingRooms (14), CustomRooms (17), OFormTemplateOnly (18), OFormOnly (19)">Filter type</param>
    /// <param type="System.Nullable{System.Boolean}, System" name="searchInContent">Specifies whether to search within the section contents or not</param>
    /// <param type="System.Nullable{System.Boolean}, System" name="withsubfolders">Specifies whether to return sections with or without subfolders</param>
    /// <returns type="ASC.Files.Core.ApiModels.ResponseDto.FolderContentDto, ASC.Files.Core">The "Shared with me" section contents</returns>
    /// <path>api/2.0/files/@share</path>
    /// <httpMethod>GET</httpMethod>
    [HttpGet("@share")]
    public async Task<FolderContentDto<int>> GetShareFolderAsync(Guid? userIdOrGroupId, FilterType? filterType, bool? searchInContent, bool? withsubfolders)
    {
        return await _foldersControllerHelper.GetFolderAsync(await _globalFolderHelper.FolderShareAsync, userIdOrGroupId, filterType, default, searchInContent, withsubfolders, 
            false, ApplyFilterOption.All);
    }

    /// <summary>
    /// Returns the detailed list of files located in the "Templates" section.
    /// </summary>
    /// <short>Get the "Templates" section</short>
    /// <category>Folders</category>
    /// <param type="System.Nullable{System.Guid}, System" name="userIdOrGroupId" optional="true">User or group ID</param>
    /// <param type="System.Nullable{ASC.Files.Core.FilterType}, System" name="filterType" optional="true" remark="Allowed values: None (0), FilesOnly (1), FoldersOnly (2), DocumentsOnly (3), PresentationsOnly (4), SpreadsheetsOnly (5), ImagesOnly (7), ByUser (8), ByDepartment (9), ArchiveOnly (10), ByExtension (11), MediaOnly (12), EditingRooms (14), CustomRooms (17), OFormTemplateOnly (18), OFormOnly (19)">Filter type</param>
    /// <param type="System.Nullable{System.Boolean}, System" name="searchInContent">Specifies whether to search within the section contents or not</param>
    /// <param type="System.Nullable{System.Boolean}, System" name="withsubfolders">Specifies whether to return sections with or without subfolders</param>
    /// <returns type="ASC.Files.Core.ApiModels.ResponseDto.FolderContentDto, ASC.Files.Core">The "Templates" section contents</returns>
    /// <path>api/2.0/files/@templates</path>
    /// <httpMethod>GET</httpMethod>
    [HttpGet("@templates")]
    public async Task<FolderContentDto<int>> GetTemplatesFolderAsync(Guid? userIdOrGroupId, FilterType? filterType, bool? searchInContent, bool? withsubfolders)
    {
        return await _foldersControllerHelper.GetFolderAsync(await _globalFolderHelper.FolderTemplatesAsync, userIdOrGroupId, filterType, default, searchInContent, withsubfolders, 
            false, ApplyFilterOption.All);
    }

    /// <summary>
    /// Returns the detailed list of files and folders located in the "Trash" section.
    /// </summary>
    /// <short>Get the "Trash" section</short>
    /// <category>Folders</category>
    /// <param type="System.Nullable{System.Guid}, System" name="userIdOrGroupId" optional="true">User or group ID</param>
    /// <param type="System.Nullable{ASC.Files.Core.FilterType}, System" name="filterType" optional="true" remark="Allowed values: None (0), FilesOnly (1), FoldersOnly (2), DocumentsOnly (3), PresentationsOnly (4), SpreadsheetsOnly (5), ImagesOnly (7), ByUser (8), ByDepartment (9), ArchiveOnly (10), ByExtension (11), MediaOnly (12), EditingRooms (14), CustomRooms (17), OFormTemplateOnly (18), OFormOnly (19)">Filter type</param>
    /// <param type="System.Nullable{System.Boolean}, System" name="searchInContent">Specifies whether to search within the section contents or not</param>
    /// <param type="System.Nullable{System.Boolean}, System" name="withsubfolders">Specifies whether to return sections with or without subfolders</param>
    /// <param type="System.Nullable{ASC.Files.Core.Core.ApplyFilterOption}, System" name="applyFilterOption"></param>
    /// <returns type="ASC.Files.Core.ApiModels.ResponseDto.FolderContentDto, ASC.Files.Core">The "Trash" section contents</returns>
    /// <path>api/2.0/files/@trash</path>
    /// <httpMethod>GET</httpMethod>
    [HttpGet("@trash")]
    public async Task<FolderContentDto<int>> GetTrashFolderAsync(Guid? userIdOrGroupId, FilterType? filterType, bool? searchInContent, bool? withsubfolders, ApplyFilterOption? applyFilterOption)
    {
        return await _foldersControllerHelper.GetFolderAsync(Convert.ToInt32(await _globalFolderHelper.FolderTrashAsync), userIdOrGroupId, filterType, default, searchInContent, withsubfolders, 
            false, applyFilterOption);
    }
}