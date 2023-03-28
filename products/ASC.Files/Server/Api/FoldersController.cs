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
        FoldersControllerHelper<int> foldersControllerHelper,
        FileStorageService<int> fileStorageService,
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
        FoldersControllerHelper<string> foldersControllerHelper,
        FileStorageService<string> fileStorageService,
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
    private readonly FoldersControllerHelper<T> _foldersControllerHelper;
    private readonly FileStorageService<T> _fileStorageService;
    private readonly FileOperationDtoHelper _fileOperationDtoHelper;

    public FoldersController(
        EntryManager entryManager,
        FoldersControllerHelper<T> foldersControllerHelper,
        FileStorageService<T> fileStorageService,
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
    /// <param type="ASC.Files.Core.ApiModels.RequestDto.CreateFolderRequestDto, ASC.Files.Core.ApiModels.RequestDto" name="inDto">Request parameters for creating a folder: Title (string) - folder title</param>
    /// <returns>New folder parameters: parent folder ID, number of files, number of folders, shareable or not, favorite or not, number for a new folder, list of tags, logo, pinned or not, room type, private or not</returns>
    /// <path>api/2.0/files/folder/{folderId}</path>
    /// <httpMethod>POST</httpMethod>
    [HttpPost("folder/{folderId}")]
    public Task<FolderDto<T>> CreateFolderAsync(T folderId, CreateFolderRequestDto inDto)
    {
        return _foldersControllerHelper.CreateFolderAsync(folderId, inDto.Title);
    }

    /// <summary>
    /// Deletes a folder with the ID specified in the request.
    /// </summary>
    /// <short>Delete a folder</short>
    /// <category>Folders</category>
    /// <param type="System.Int32, System" name="folderId">Folder ID</param>
    /// <param type="ASC.Files.Core.ApiModels.RequestDto.DeleteFolderDto, ASC.Files.Core.ApiModels.RequestDto" name="model">Request parameters for deleting a folder: <![CDATA[
    /// <ul>
    ///     <li><b>DeleteAfter</b> (bool) - specifies whether to delete a folder after the editing session is finished or not,</li>
    ///     <li><b>Immediately</b> (bool) - specifies whether to move a folder to the "Trash" folder or delete it immediately.</li>
    /// </ul>
    /// ]]></param>
    /// <returns>List of file operations: operation ID, operation type, operation progress, error, processing status, finished or not, URL, list of files, list of folders</returns>
    /// <path>api/2.0/files/folder/{folderId}</path>
    /// <httpMethod>DELETE</httpMethod>
    [HttpDelete("folder/{folderId}")]
    public async IAsyncEnumerable<FileOperationDto> DeleteFolder(T folderId, DeleteFolderDto model)
    {
        foreach (var e in _fileStorageService.DeleteFolder("delete", folderId, false, model.DeleteAfter, model.Immediately))
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
    /// <param type="System.Int32, System" name="folderId">Folder ID</param>
    /// <param type="System.Nullable{System.Guid}, System" name="userIdOrGroupId" optional="true">User or group ID</param>
    /// <param type="System.Nullable{ASC.Files.Core.FilterType}, System" name="filterType" optional="true" remark="Allowed values: None (0), FilesOnly (1), FoldersOnly (2), DocumentsOnly (3), PresentationsOnly (4), SpreadsheetsOnly (5), ImagesOnly (7), ByUser (8), ByDepartment (9), ArchiveOnly (10), ByExtension (11), MediaOnly (12), FillingFormsRooms (13), EditingRooms (14), ReviewRooms (15), ReadOnlyRooms (16), CustomRooms (17), OFormTemplateOnly (18), OFormOnly (19)">Filter type</param>
    /// <param type="System.Nullable{System.Boolean}, System" name="searchInContent">Specifies whether to search within the section contents or not</param>
    /// <param type="System.Nullable{System.Boolean}, System" name="withsubfolders">Specifies whether to return sections with or without subfolders</param>
    /// <param type="System.Nullable{System.Boolean}, System" name="excludeSubject">Specifies whether to exclude a subject or not</param>
    /// <returns>Folder contents: list of files, list of folders, current folder information, folder path, folder start index, number of folder elements, total number of elements in the folder, new element index</returns>
    /// <path>api/2.0/files/{folderId}</path>
    /// <httpMethod>GET</httpMethod>
    [HttpGet("{folderId}", Order = 1)]
    public async Task<FolderContentDto<T>> GetFolderAsync(T folderId, Guid? userIdOrGroupId, FilterType? filterType, bool? searchInContent, bool? withsubfolders, bool? excludeSubject)
    {
        var folder = await _foldersControllerHelper.GetFolderAsync(folderId, userIdOrGroupId, filterType, searchInContent, withsubfolders, excludeSubject);

        return folder.NotFoundIfNull();
    }

    /// <summary>
    /// Returns the detailed information about a folder with the ID specified in the request.
    /// </summary>
    /// <short>Get the folder information</short>
    /// <param type="System.Int32, System" name="folderId">Folder ID</param>
    /// <category>Folders</category>
    /// <returns>Folder parameters: parent folder ID, number of files, number of folders, shareable or not, favorite or not, number for a new folder, list of tags, logo, pinned or not, room type, private or not</returns>
    /// <path>api/2.0/files/folder/{folderId}</path>
    /// <httpMethod>GET</httpMethod>
    [HttpGet("folder/{folderId}")]
    public Task<FolderDto<T>> GetFolderInfoAsync(T folderId)
    {
        return _foldersControllerHelper.GetFolderInfoAsync(folderId);
    }

    /// <summary>
    /// Returns a path to the folder with the ID specified in the request.
    /// </summary>
    /// <short>Get the folder path</short>
    /// <param type="System.Int32, System" name="folderId">Folder ID</param>
    /// <category>Folders</category>
    /// <returns>List of file entry information: title, access rights, shared or not, creation time, author, time of the last file update, root folder type, a user who updated a file, provider is specified or not, provider key, provider ID</returns>
    /// <path>api/2.0/files/folder/{folderId}/path</path>
    /// <httpMethod>GET</httpMethod>
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
    /// <param type="System.Int32, System" name="folderId">Folder ID</param>
    /// <category>Folders</category>
    /// <returns>List of file entry information: title, access rights, shared or not, creation time, author, time of the last file update, root folder type, a user who updated a file, provider is specified or not, provider key, provider ID</returns>
    /// <path>api/2.0/files/{folderId}/subfolders</path>
    /// <httpMethod>GET</httpMethod>
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
    /// <param type="System.Int32, System" name="folderId">Folder ID</param>
    /// <category>Folders</category>
    /// <returns>List of file entry information: title, access rights, shared or not, creation time, author, time of the last file update, root folder type, a user who updated a file, provider is specified or not, provider key, provider ID</returns>
    /// <path>api/2.0/files/{folderId}/news</path>
    /// <httpMethod>GET</httpMethod>
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
    /// <param type="System.Int32, System" name="folderId">Folder ID</param>
    /// <param type="ASC.Files.Core.ApiModels.RequestDto.CreateFolderRequestDto, ASC.Files.Core.ApiModels.RequestDto" name="inDto">Request parameters for creating a folder: Title (string) - new folder title</param>
    /// <returns>Folder parameters: parent folder ID, number of files, number of folders, shareable or not, favorite or not, number for a new folder, list of tags, logo, pinned or not, room type, private or not</returns>
    /// <path>api/2.0/files/folder/{folderId}</path>
    /// <httpMethod>PUT</httpMethod>
    [HttpPut("folder/{folderId}")]
    public Task<FolderDto<T>> RenameFolderAsync(T folderId, CreateFolderRequestDto inDto)
    {
        return _foldersControllerHelper.RenameFolderAsync(folderId, inDto.Title);
    }
}

public class FoldersControllerCommon : ApiControllerBase
{
    private readonly GlobalFolderHelper _globalFolderHelper;
    private readonly FoldersControllerHelper<int> _foldersControllerHelperInt;
    private readonly FoldersControllerHelper<string> _foldersControllerHelperString;

    public FoldersControllerCommon(
        GlobalFolderHelper globalFolderHelper,
        FoldersControllerHelper<int> foldersControllerHelperInt,
        FoldersControllerHelper<string> foldersControllerHelperString,
        FolderDtoHelper folderDtoHelper,
        FileDtoHelper fileDtoHelper) : base(folderDtoHelper, fileDtoHelper)
    {
        _globalFolderHelper = globalFolderHelper;
        _foldersControllerHelperInt = foldersControllerHelperInt;
        _foldersControllerHelperString = foldersControllerHelperString;
    }

    /// <summary>
    /// Returns the detailed list of files and folders located in the "Common" section.
    /// </summary>
    /// <short>Get the "Common" section</short>
    /// <category>Folders</category>
    /// <param type="System.Nullable{System.Guid}, System" name="userIdOrGroupId" optional="true">User or group ID</param>
    /// <param type="System.Nullable{ASC.Files.Core.FilterType}, System" name="filterType" optional="true" remark="Allowed values: None (0), FilesOnly (1), FoldersOnly (2), DocumentsOnly (3), PresentationsOnly (4), SpreadsheetsOnly (5), ImagesOnly (7), ByUser (8), ByDepartment (9), ArchiveOnly (10), ByExtension (11), MediaOnly (12), FillingFormsRooms (13), EditingRooms (14), ReviewRooms (15), ReadOnlyRooms (16), CustomRooms (17), OFormTemplateOnly (18), OFormOnly (19)">Filter type</param>
    /// <param type="System.Nullable{System.Boolean}, System" name="searchInContent">Specifies whether to search within the section contents or not</param>
    /// <param type="System.Nullable{System.Boolean}, System" name="withsubfolders">Specifies whether to return sections with or without subfolders</param>
    /// <returns>The "Common" section contents: list of files, list of folders, current folder information, folder path, folder start index, number of folder elements, total number of elements in the folder, new element index</returns>
    /// <path>api/2.0/files/@common</path>
    /// <httpMethod>GET</httpMethod>
    [HttpGet("@common")]
    public async Task<FolderContentDto<int>> GetCommonFolderAsync(Guid? userIdOrGroupId, FilterType? filterType, bool? searchInContent, bool? withsubfolders)
    {
        return await _foldersControllerHelperInt.GetFolderAsync(await _globalFolderHelper.FolderCommonAsync, userIdOrGroupId, filterType, searchInContent, withsubfolders);
    }

    /// <summary>
    /// Returns the detailed list of files and folders located in the "Favorites" section.
    /// </summary>
    /// <short>Get the "Favorites" section</short>
    /// <category>Folders</category>
    /// <param type="System.Nullable{System.Guid}, System" name="userIdOrGroupId" optional="true">User or group ID</param>
    /// <param type="System.Nullable{ASC.Files.Core.FilterType}, System" name="filterType" optional="true" remark="Allowed values: None (0), FilesOnly (1), FoldersOnly (2), DocumentsOnly (3), PresentationsOnly (4), SpreadsheetsOnly (5), ImagesOnly (7), ByUser (8), ByDepartment (9), ArchiveOnly (10), ByExtension (11), MediaOnly (12), FillingFormsRooms (13), EditingRooms (14), ReviewRooms (15), ReadOnlyRooms (16), CustomRooms (17), OFormTemplateOnly (18), OFormOnly (19)">Filter type</param>
    /// <param type="System.Nullable{System.Boolean}, System" name="searchInContent">Specifies whether to search within the section contents or not</param>
    /// <param type="System.Nullable{System.Boolean}, System" name="withsubfolders">Specifies whether to return sections with or without subfolders</param>
    /// <returns>The "Favorites" section contents: list of files, list of folders, current folder information, folder path, folder start index, number of folder elements, total number of elements in the folder, new element index</returns>
    /// <path>api/2.0/files/@favorites</path>
    /// <httpMethod>GET</httpMethod>
    [HttpGet("@favorites")]
    public async Task<FolderContentDto<int>> GetFavoritesFolderAsync(Guid? userIdOrGroupId, FilterType? filterType, bool? searchInContent, bool? withsubfolders)
    {
        return await _foldersControllerHelperInt.GetFolderAsync(await _globalFolderHelper.FolderFavoritesAsync, userIdOrGroupId, filterType, searchInContent, withsubfolders);
    }

    /// <summary>
    /// Returns the detailed list of files and folders located in the "My documents" section.
    /// </summary>
    /// <short>Get the "My documents" section</short>
    /// <category>Folders</category>
    /// <param type="System.Nullable{System.Guid}, System" name="userIdOrGroupId" optional="true">User or group ID</param>
    /// <param type="System.Nullable{ASC.Files.Core.FilterType}, System" name="filterType" optional="true" remark="Allowed values: None (0), FilesOnly (1), FoldersOnly (2), DocumentsOnly (3), PresentationsOnly (4), SpreadsheetsOnly (5), ImagesOnly (7), ByUser (8), ByDepartment (9), ArchiveOnly (10), ByExtension (11), MediaOnly (12), FillingFormsRooms (13), EditingRooms (14), ReviewRooms (15), ReadOnlyRooms (16), CustomRooms (17), OFormTemplateOnly (18), OFormOnly (19)">Filter type</param>
    /// <param type="System.Nullable{System.Boolean}, System" name="searchInContent">Specifies whether to search within the section contents or not</param>
    /// <param type="System.Nullable{System.Boolean}, System" name="withsubfolders">Specifies whether to return sections with or without subfolders</param>
    /// <returns>The "My documents" section contents: list of files, list of folders, current folder information, folder path, folder start index, number of folder elements, total number of elements in the folder, new element index</returns>
    /// <path>api/2.0/files/@my</path>
    /// <httpMethod>GET</httpMethod>
    [HttpGet("@my")]
    public Task<FolderContentDto<int>> GetMyFolderAsync(Guid? userIdOrGroupId, FilterType? filterType, bool? searchInContent, bool? withsubfolders)
    {
        return _foldersControllerHelperInt.GetFolderAsync(_globalFolderHelper.FolderMy, userIdOrGroupId, filterType, searchInContent, withsubfolders);
    }

    /// <summary>
    /// Returns the detailed list of files and folders located in the "Private Room" section.
    /// </summary>
    /// <short>Get the "Private Room" section</short>
    /// <category>Folders</category>
    /// <param type="System.Nullable{System.Guid}, System" name="userIdOrGroupId" optional="true">User or group ID</param>
    /// <param type="System.Nullable{ASC.Files.Core.FilterType}, System" name="filterType" optional="true" remark="Allowed values: None (0), FilesOnly (1), FoldersOnly (2), DocumentsOnly (3), PresentationsOnly (4), SpreadsheetsOnly (5), ImagesOnly (7), ByUser (8), ByDepartment (9), ArchiveOnly (10), ByExtension (11), MediaOnly (12), FillingFormsRooms (13), EditingRooms (14), ReviewRooms (15), ReadOnlyRooms (16), CustomRooms (17), OFormTemplateOnly (18), OFormOnly (19)">Filter type</param>
    /// <param type="System.Nullable{System.Boolean}, System" name="searchInContent">Specifies whether to search within the section contents or not</param>
    /// <param type="System.Nullable{System.Boolean}, System" name="withsubfolders">Specifies whether to return sections with or without subfolders</param>
    /// <returns>The "Private Room" section contents: list of files, list of folders, current folder information, folder path, folder start index, number of folder elements, total number of elements in the folder, new element index</returns>
    /// <path>api/2.0/files/@privacy</path>
    /// <httpMethod>GET</httpMethod>
    [HttpGet("@privacy")]
    public async Task<FolderContentDto<int>> GetPrivacyFolderAsync(Guid? userIdOrGroupId, FilterType? filterType, bool? searchInContent, bool? withsubfolders)
    {
        if (PrivacyRoomSettings.IsAvailable())
        {
            throw new SecurityException();
        }

        return await _foldersControllerHelperInt.GetFolderAsync(await _globalFolderHelper.FolderPrivacyAsync, userIdOrGroupId, filterType, searchInContent, withsubfolders);
    }

    /// <summary>
    /// Returns the detailed list of files and folders located in the "In projects" section.
    /// </summary>
    /// <short>Get the "In projects" section</short>
    /// <category>Folders</category>
    /// <param type="System.Nullable{System.Guid}, System" name="userIdOrGroupId" optional="true">User or group ID</param>
    /// <param type="System.Nullable{ASC.Files.Core.FilterType}, System" name="filterType" optional="true" remark="Allowed values: None (0), FilesOnly (1), FoldersOnly (2), DocumentsOnly (3), PresentationsOnly (4), SpreadsheetsOnly (5), ImagesOnly (7), ByUser (8), ByDepartment (9), ArchiveOnly (10), ByExtension (11), MediaOnly (12), FillingFormsRooms (13), EditingRooms (14), ReviewRooms (15), ReadOnlyRooms (16), CustomRooms (17), OFormTemplateOnly (18), OFormOnly (19)">Filter type</param>
    /// <param type="System.Nullable{System.Boolean}, System" name="searchInContent">Specifies whether to search within the section contents or not</param>
    /// <param type="System.Nullable{System.Boolean}, System" name="withsubfolders">Specifies whether to return sections with or without subfolders</param>
    /// <returns>The "In projects" section contents: list of files, list of folders, current folder information, folder path, folder start index, number of folder elements, total number of elements in the folder, new element index</returns>
    /// <path>api/2.0/files/@projects</path>
    /// <httpMethod>GET</httpMethod>
    [HttpGet("@projects")]
    public async Task<FolderContentDto<string>> GetProjectsFolderAsync(Guid? userIdOrGroupId, FilterType? filterType, bool? searchInContent, bool? withsubfolders)
    {
        return await _foldersControllerHelperString.GetFolderAsync(await _globalFolderHelper.GetFolderProjectsAsync<string>(), userIdOrGroupId, filterType, searchInContent, withsubfolders);
    }

    /// <summary>
    /// Returns the detailed list of files located in the "Recent" section.
    /// </summary>
    /// <short>Get the "Recent" section</short>
    /// <category>Folders</category>
    /// <param type="System.Nullable{System.Guid}, System" name="userIdOrGroupId" optional="true">User or group ID</param>
    /// <param type="System.Nullable{ASC.Files.Core.FilterType}, System" name="filterType" optional="true" remark="Allowed values: None (0), FilesOnly (1), FoldersOnly (2), DocumentsOnly (3), PresentationsOnly (4), SpreadsheetsOnly (5), ImagesOnly (7), ByUser (8), ByDepartment (9), ArchiveOnly (10), ByExtension (11), MediaOnly (12), FillingFormsRooms (13), EditingRooms (14), ReviewRooms (15), ReadOnlyRooms (16), CustomRooms (17), OFormTemplateOnly (18), OFormOnly (19)">Filter type</param>
    /// <param type="System.Nullable{System.Boolean}, System" name="searchInContent">Specifies whether to search within the section contents or not</param>
    /// <param type="System.Nullable{System.Boolean}, System" name="withsubfolders">Specifies whether to return sections with or without subfolders</param>
    /// <returns>The "Recent" section contents: list of files, list of folders, current folder information, folder path, folder start index, number of folder elements, total number of elements in the folder, new element index</returns>
    /// <path>api/2.0/files/@recent</path>
    /// <httpMethod>GET</httpMethod>
    [HttpGet("@recent")]
    public async Task<FolderContentDto<int>> GetRecentFolderAsync(Guid? userIdOrGroupId, FilterType? filterType, bool? searchInContent, bool? withsubfolders)
    {
        return await _foldersControllerHelperInt.GetFolderAsync(await _globalFolderHelper.FolderRecentAsync, userIdOrGroupId, filterType, searchInContent, withsubfolders);
    }

    /// <summary>
    /// Returns all the sections matching the parameters specified in the request.
    /// </summary>
    /// <short>Get filtered sections</short>
    /// <param type="System.Nullable{System.Guid}, System" name="userIdOrGroupId" optional="true">User or group ID</param>
    /// <param type="System.Nullable{ASC.Files.Core.FilterType}, System" name="filterType" optional="true" remark="Allowed values: None (0), FilesOnly (1), FoldersOnly (2), DocumentsOnly (3), PresentationsOnly (4), SpreadsheetsOnly (5), ImagesOnly (7), ByUser (8), ByDepartment (9), ArchiveOnly (10), ByExtension (11), MediaOnly (12), FillingFormsRooms (13), EditingRooms (14), ReviewRooms (15), ReadOnlyRooms (16), CustomRooms (17), OFormTemplateOnly (18), OFormOnly (19)">Filter type</param>
    /// <param type="System.Nullable{System.Boolean}, System" name="withsubfolders">Specifies whether to return sections with or without subfolders</param>
    /// <param type="System.Nullable{System.Boolean}, System" name="withoutTrash">Specifies whether to return the "Trash" section or not</param>
    /// <param type="System.Nullable{System.Boolean}, System" name="searchInContent">Specifies whether to search within the section contents or not</param>
    /// <param type="System.Nullable{System.Boolean}, System" name="withoutAdditionalFolder">Specifies whether to return sections with or without additional folders</param>
    /// <category>Folders</category>
    /// <returns>List of section contents with the following parameters: list of files, list of folders, current folder information, folder path, folder start index, number of folder elements, total number of elements in the folder, new element index</returns>
    /// <path>api/2.0/files/@root</path>
    /// <httpMethod>GET</httpMethod>
    [HttpGet("@root")]
    public async IAsyncEnumerable<FolderContentDto<int>> GetRootFoldersAsync(Guid? userIdOrGroupId, FilterType? filterType, bool? withsubfolders, bool? withoutTrash, bool? searchInContent, bool? withoutAdditionalFolder)
    {
        var foldersIds = _foldersControllerHelperInt.GetRootFoldersIdsAsync(withoutTrash ?? false, withoutAdditionalFolder ?? false);

        await foreach (var folder in foldersIds)
        {
            yield return await _foldersControllerHelperInt.GetFolderAsync(folder, userIdOrGroupId, filterType, searchInContent, withsubfolders);
        }
    }

    /// <summary>
    /// Returns the detailed list of files and folders located in the "Shared with me" section.
    /// </summary>
    /// <short>Get the "Shared with me" section</short>
    /// <category>Folders</category>
    /// <param type="System.Nullable{System.Guid}, System" name="userIdOrGroupId" optional="true">User or group ID</param>
    /// <param type="System.Nullable{ASC.Files.Core.FilterType}, System" name="filterType" optional="true" remark="Allowed values: None (0), FilesOnly (1), FoldersOnly (2), DocumentsOnly (3), PresentationsOnly (4), SpreadsheetsOnly (5), ImagesOnly (7), ByUser (8), ByDepartment (9), ArchiveOnly (10), ByExtension (11), MediaOnly (12), FillingFormsRooms (13), EditingRooms (14), ReviewRooms (15), ReadOnlyRooms (16), CustomRooms (17), OFormTemplateOnly (18), OFormOnly (19)">Filter type</param>
    /// <param type="System.Nullable{System.Boolean}, System" name="searchInContent">Specifies whether to search within the section contents or not</param>
    /// <param type="System.Nullable{System.Boolean}, System" name="withsubfolders">Specifies whether to return sections with or without subfolders</param>
    /// <returns>The "Shared with me" section contents: list of files, list of folders, current folder information, folder path, folder start index, number of folder elements, total number of elements in the folder, new element index</returns>
    /// <path>api/2.0/files/@share</path>
    /// <httpMethod>GET</httpMethod>
    [HttpGet("@share")]
    public async Task<FolderContentDto<int>> GetShareFolderAsync(Guid? userIdOrGroupId, FilterType? filterType, bool? searchInContent, bool? withsubfolders)
    {
        return await _foldersControllerHelperInt.GetFolderAsync(await _globalFolderHelper.FolderShareAsync, userIdOrGroupId, filterType, searchInContent, withsubfolders);
    }

    /// <summary>
    /// Returns the detailed list of files located in the "Templates" section.
    /// </summary>
    /// <short>Get the "Templates" section</short>
    /// <category>Folders</category>
    /// <param type="System.Nullable{System.Guid}, System" name="userIdOrGroupId" optional="true">User or group ID</param>
    /// <param type="System.Nullable{ASC.Files.Core.FilterType}, System" name="filterType" optional="true" remark="Allowed values: None (0), FilesOnly (1), FoldersOnly (2), DocumentsOnly (3), PresentationsOnly (4), SpreadsheetsOnly (5), ImagesOnly (7), ByUser (8), ByDepartment (9), ArchiveOnly (10), ByExtension (11), MediaOnly (12), FillingFormsRooms (13), EditingRooms (14), ReviewRooms (15), ReadOnlyRooms (16), CustomRooms (17), OFormTemplateOnly (18), OFormOnly (19)">Filter type</param>
    /// <param type="System.Nullable{System.Boolean}, System" name="searchInContent">Specifies whether to search within the section contents or not</param>
    /// <param type="System.Nullable{System.Boolean}, System" name="withsubfolders">Specifies whether to return sections with or without subfolders</param>
    /// <returns>The "Templates" section contents: list of files, list of folders, current folder information, folder path, folder start index, number of folder elements, total number of elements in the folder, new element index</returns>
    /// <path>api/2.0/files/@templates</path>
    /// <httpMethod>GET</httpMethod>
    [HttpGet("@templates")]
    public async Task<FolderContentDto<int>> GetTemplatesFolderAsync(Guid? userIdOrGroupId, FilterType? filterType, bool? searchInContent, bool? withsubfolders)
    {
        return await _foldersControllerHelperInt.GetFolderAsync(await _globalFolderHelper.FolderTemplatesAsync, userIdOrGroupId, filterType, searchInContent, withsubfolders);
    }

    /// <summary>
    /// Returns the detailed list of files and folders located in the "Trash" section.
    /// </summary>
    /// <short>Get the "Trash" section</short>
    /// <category>Folders</category>
    /// <param type="System.Nullable{System.Guid}, System" name="userIdOrGroupId" optional="true">User or group ID</param>
    /// <param type="System.Nullable{ASC.Files.Core.FilterType}, System" name="filterType" optional="true" remark="Allowed values: None (0), FilesOnly (1), FoldersOnly (2), DocumentsOnly (3), PresentationsOnly (4), SpreadsheetsOnly (5), ImagesOnly (7), ByUser (8), ByDepartment (9), ArchiveOnly (10), ByExtension (11), MediaOnly (12), FillingFormsRooms (13), EditingRooms (14), ReviewRooms (15), ReadOnlyRooms (16), CustomRooms (17), OFormTemplateOnly (18), OFormOnly (19)">Filter type</param>
    /// <param type="System.Nullable{System.Boolean}, System" name="searchInContent">Specifies whether to search within the section contents or not</param>
    /// <param type="System.Nullable{System.Boolean}, System" name="withsubfolders">Specifies whether to return sections with or without subfolders</param>
    /// <returns>The "Trash" section contents: list of files, list of folders, current folder information, folder path, folder start index, number of folder elements, total number of elements in the folder, new element index</returns>
    /// <path>api/2.0/files/@trash</path>
    /// <httpMethod>GET</httpMethod>
    [HttpGet("@trash")]
    public Task<FolderContentDto<int>> GetTrashFolderAsync(Guid? userIdOrGroupId, FilterType? filterType, bool? searchInContent, bool? withsubfolders)
    {
        return _foldersControllerHelperInt.GetFolderAsync(Convert.ToInt32(_globalFolderHelper.FolderTrash), userIdOrGroupId, filterType, searchInContent, withsubfolders);
    }
}
