﻿// (c) Copyright Ascensio System SIA 2010-2022
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

namespace ASC.Files.Helpers;

public class FoldersControllerHelper<T> : FilesHelperBase<T>
{
    private readonly EntryManager _entryManager;
    private readonly UserManager _userManager;
    private readonly SecurityContext _securityContext;
    private readonly GlobalFolderHelper _globalFolderHelper;
    private readonly CoreBaseSettings _coreBaseSettings;
    private readonly FileUtility _fileUtility;

    public FoldersControllerHelper(
        FilesSettingsHelper filesSettingsHelper,
        FileUploader fileUploader,
        SocketManager socketManager,
        FileDtoHelper fileDtoHelper,
        ApiContext apiContext,
        FileStorageService<T> fileStorageService,
        FolderContentDtoHelper folderContentDtoHelper,
        IHttpContextAccessor httpContextAccessor,
        FolderDtoHelper folderDtoHelper,
        EntryManager entryManager,
        UserManager userManager,
        SecurityContext securityContext,
        GlobalFolderHelper globalFolderHelper,
        CoreBaseSettings coreBaseSettings,
        FileUtility fileUtility)
        : base(
            filesSettingsHelper,
            fileUploader,
            socketManager,
            fileDtoHelper,
            apiContext,
            fileStorageService,
            folderContentDtoHelper,
            httpContextAccessor,
            folderDtoHelper)
    {
        _globalFolderHelper = globalFolderHelper;
        _coreBaseSettings = coreBaseSettings;
        _fileUtility = fileUtility;
        _securityContext = securityContext;
        _entryManager = entryManager;
        _userManager = userManager;
    }

    public async Task<FolderDto<T>> CreateFolderAsync(T folderId, string title)
    {
        var folder = await _fileStorageService.CreateNewFolderAsync(folderId, title);

        return await _folderDtoHelper.GetAsync(folder);
    }

    public async Task<FolderContentDto<T>> GetFolderAsync(T folderId, Guid? userIdOrGroupId, FilterType? filterType, bool? searchInContent, bool? withSubFolders, bool? excludeSubject = false)
    {
        var folderContentWrapper = await ToFolderContentWrapperAsync(folderId, userIdOrGroupId ?? Guid.Empty, filterType ?? FilterType.None, searchInContent ?? false, withSubFolders ?? false, excludeSubject ?? false);

        return folderContentWrapper.NotFoundIfNull();
    }

    public async Task<FolderDto<T>> GetFolderInfoAsync(T folderId)
    {
        var folder = await _fileStorageService.GetFolderAsync(folderId).NotFoundIfNull("Folder not found");

        return await _folderDtoHelper.GetAsync(folder);
    }

    public async IAsyncEnumerable<int> GetRootFoldersIdsAsync(bool withoutTrash, bool withoutAdditionalFolder)
    {
        var user = _userManager.GetUsers(_securityContext.CurrentAccount.ID);
        var IsUser = _userManager.IsUser(user);
        var IsOutsider = _userManager.IsOutsider(user);

        if (IsOutsider)
        {
            withoutTrash = true;
            withoutAdditionalFolder = true;
        }

        if (!IsUser)
        {
            yield return _globalFolderHelper.FolderMy;
        }

        if (!_coreBaseSettings.Personal && _coreBaseSettings.DisableDocSpace
            && !_userManager.IsOutsider(user))
        {
            yield return await _globalFolderHelper.FolderShareAsync;
        }

        if (!withoutAdditionalFolder)
        {
            if (_filesSettingsHelper.FavoritesSection)
            {
                yield return await _globalFolderHelper.FolderFavoritesAsync;
            }

            if (_filesSettingsHelper.RecentSection)
            {
                yield return await _globalFolderHelper.FolderRecentAsync;
            }

            if (!IsUser &&
                !_coreBaseSettings.Personal &&
                _coreBaseSettings.DisableDocSpace &&
                PrivacyRoomSettings.IsAvailable())
            {
                yield return await _globalFolderHelper.FolderPrivacyAsync;
            }
        }

        if (!_coreBaseSettings.Personal && _coreBaseSettings.DisableDocSpace)
        {
            yield return await _globalFolderHelper.FolderCommonAsync;
        }

        if (!IsUser
           && _coreBaseSettings.DisableDocSpace
           && !withoutAdditionalFolder
           && _fileUtility.ExtsWebTemplate.Count > 0
           && _filesSettingsHelper.TemplatesSection)
        {
            yield return await _globalFolderHelper.FolderTemplatesAsync;
        }

        if (!withoutTrash && !IsUser)
        {
            yield return (int)_globalFolderHelper.FolderTrash;
        }

        if (!_coreBaseSettings.DisableDocSpace)
        {
            yield return await _globalFolderHelper.FolderVirtualRoomsAsync;
            yield return await _globalFolderHelper.FolderArchiveAsync;
        }
    }

    public async Task<FolderDto<T>> RenameFolderAsync(T folderId, string title)
    {
        var folder = await _fileStorageService.FolderRenameAsync(folderId, title);

        return await _folderDtoHelper.GetAsync(folder);
    }

    private async Task<FolderContentDto<T>> ToFolderContentWrapperAsync(T folderId, Guid userIdOrGroupId, FilterType filterType, bool searchInContent, bool withSubFolders, bool excludeSubject)
    {
        OrderBy orderBy = null;
        if (SortedByTypeExtensions.TryParse(_apiContext.SortBy, true, out var sortBy))
        {
            orderBy = new OrderBy(sortBy, !_apiContext.SortDescending);
        }

        var startIndex = Convert.ToInt32(_apiContext.StartIndex);
        var items = await _fileStorageService.GetFolderItemsAsync(folderId, startIndex, Convert.ToInt32(_apiContext.Count), filterType, filterType == FilterType.ByUser, userIdOrGroupId.ToString(), _apiContext.FilterValue, searchInContent, withSubFolders, orderBy, excludeSubject: excludeSubject);

        return await _folderContentDtoHelper.GetAsync(items, startIndex);
    }
}