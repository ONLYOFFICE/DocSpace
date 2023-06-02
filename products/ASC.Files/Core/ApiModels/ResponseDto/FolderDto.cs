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

namespace ASC.Files.Core.ApiModels.ResponseDto;

public class FolderDto<T> : FileEntryDto<T>
{
    public T ParentId { get; set; }
    public int FilesCount { get; set; }
    public int FoldersCount { get; set; }
    public bool? IsShareable { get; set; }
    public bool? IsFavorite { get; set; }
    public int New { get; set; }
    public bool Mute { get; set; }
    public IEnumerable<string> Tags { get; set; }
    public Logo Logo { get; set; }
    public bool Pinned { get; set; }
    public RoomType? RoomType { get; set; }
    public bool Private { get; set; }

    protected internal override FileEntryType EntryType { get => FileEntryType.Folder; }

    public FolderDto() { }

    public static FolderDto<int> GetSample()
    {
        return new FolderDto<int>
        {
            Access = FileShare.ReadWrite,
            //Updated = ApiDateTime.GetSample(),
            //Created = ApiDateTime.GetSample(),
            //CreatedBy = EmployeeWraper.GetSample(),
            Id = 10,
            RootFolderType = FolderType.BUNCH,
            Shared = false,
            Title = "Some titile",
            //UpdatedBy = EmployeeWraper.GetSample(),
            FilesCount = 5,
            FoldersCount = 7,
            ParentId = 10,
            IsShareable = null,
            IsFavorite = null
        };
    }
}

[Scope]
public class FolderDtoHelper : FileEntryDtoHelper
{
    private readonly AuthContext _authContext;
    private readonly IDaoFactory _daoFactory;
    private readonly GlobalFolderHelper _globalFolderHelper;
    private readonly RoomLogoManager _roomLogoManager;
    private readonly RoomsNotificationSettingsHelper _roomsNotificationSettingsHelper;
    private readonly BadgesSettingsHelper _badgesSettingsHelper;

    public FolderDtoHelper(
        ApiDateTimeHelper apiDateTimeHelper,
        EmployeeDtoHelper employeeWrapperHelper,
        AuthContext authContext,
        IDaoFactory daoFactory,
        FileSecurity fileSecurity,
        GlobalFolderHelper globalFolderHelper,
        FileSharingHelper fileSharingHelper,
        RoomLogoManager roomLogoManager,
        BadgesSettingsHelper badgesSettingsHelper,
        RoomsNotificationSettingsHelper roomsNotificationSettingsHelper)
        : base(apiDateTimeHelper, employeeWrapperHelper, fileSharingHelper, fileSecurity)
    {
        _authContext = authContext;
        _daoFactory = daoFactory;
        _globalFolderHelper = globalFolderHelper;
        _roomLogoManager = roomLogoManager;
        _roomsNotificationSettingsHelper = roomsNotificationSettingsHelper;
        _badgesSettingsHelper = badgesSettingsHelper;
    }

    public async Task<FolderDto<T>> GetAsync<T>(Folder<T> folder, List<Tuple<FileEntry<T>, bool>> folders = null)
    {
        var result = await GetFolderWrapperAsync(folder);

        result.ParentId = folder.ParentId;

        if (DocSpaceHelper.IsRoom(folder.FolderType))
        {
            if (folder.Tags == null)
            {
                var tagDao = _daoFactory.GetTagDao<T>();

                result.Tags = await tagDao.GetTagsAsync(TagType.Custom, new[] { folder }).Select(t => t.Name).ToListAsync();
            }
            else
            {
                result.Tags = folder.Tags.Select(t => t.Name);
            }

            result.Logo = await _roomLogoManager.GetLogoAsync(folder);
            result.RoomType = folder.FolderType switch
            {
                FolderType.FillingFormsRoom => RoomType.FillingFormsRoom,
                FolderType.EditingRoom => RoomType.EditingRoom,
                FolderType.ReviewRoom => RoomType.ReviewRoom,
                FolderType.ReadOnlyRoom => RoomType.ReadOnlyRoom,
                FolderType.CustomRoom => RoomType.CustomRoom,
                _ => null,
            };

            if (folder.ProviderEntry && folder.RootFolderType is FolderType.VirtualRooms)
            {
                result.ParentId = IdConverter.Convert<T>(await _globalFolderHelper.GetFolderVirtualRooms());

            var isMuted = _roomsNotificationSettingsHelper.CheckMuteForRoom(result.Id.ToString());
            result.Mute = isMuted;
            }
        }

        if (folder.RootFolderType == FolderType.USER
            && !Equals(folder.RootCreateBy, _authContext.CurrentAccount.ID))
        {
            result.RootFolderType = FolderType.SHARE;

            var folderDao = _daoFactory.GetFolderDao<T>();
            FileEntry<T> parentFolder;

            if (folders != null)
            {
                var folderWithRight = folders.FirstOrDefault(f => f.Item1.Id.Equals(folder.ParentId));
                if (folderWithRight == null || !folderWithRight.Item2)
                {
                    result.ParentId = await _globalFolderHelper.GetFolderShareAsync<T>();
                }
            }
            else
            {
                parentFolder = await folderDao.GetFolderAsync(folder.ParentId);
                var canRead = await _fileSecurity.CanReadAsync(parentFolder);
                if (!canRead)
                {
                    result.ParentId = await _globalFolderHelper.GetFolderShareAsync<T>();
                }
            }
        }

        return result;
    }

    private async Task<FolderDto<T>> GetFolderWrapperAsync<T>(Folder<T> folder)
    {
        var newBadges = folder.NewForMe;

        if (folder.RootFolderType == FolderType.VirtualRooms)
        {
            var isEnabledBadges = await _badgesSettingsHelper.GetEnabledForCurrentUserAsync();

            if (!isEnabledBadges)
            {
                newBadges = 0;
            }
        }

        var result = await GetAsync<FolderDto<T>, T>(folder);
        result.FilesCount = folder.FilesCount;
        result.FoldersCount = folder.FoldersCount;
        result.IsShareable = folder.Shareable.NullIfDefault();
        result.IsFavorite = folder.IsFavorite.NullIfDefault();
        result.New = newBadges;
        result.Pinned = folder.Pinned;
        result.Private = folder.Private;

        return result;
    }
}
