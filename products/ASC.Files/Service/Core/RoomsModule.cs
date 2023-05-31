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

using Constants = ASC.Feed.Constants;
using FeedModule = ASC.Feed.Aggregator.Modules.FeedModule;

namespace ASC.Files.Service.Core;

public class RoomsModule : FeedModule
{
    public const string RoomItem = Constants.RoomItem;
    public const string SharedRoomItem = Constants.SharedRoomItem;

    private readonly IFolderDao<int> _folderDao;
    private readonly UserManager _userManager;
    private readonly FileSecurity _fileSecurity;
    private readonly TenantUtil _tenantUtil;

    public RoomsModule(
        TenantManager tenantManager,
        UserManager userManager,
        WebItemSecurity webItemSecurity,
        FileSecurity fileSecurity,
        IDaoFactory daoFactory,
        TenantUtil tenantUtil)
        : base(tenantManager, webItemSecurity)
    {
        _userManager = userManager;
        _fileSecurity = fileSecurity;
        _folderDao = daoFactory.GetFolderDao<int>();
        _tenantUtil = tenantUtil;
    }

    public override string Name => Constants.RoomsModule;
    public override string Product => Constants.Documents;
    public override Guid ProductID => WebItemManager.DocumentsProductID;
    protected override string DbId => Constants.FilesDbId;

    public override async Task<bool> VisibleForAsync(Feed.Aggregator.Feed feed, object data, Guid userId)
    {
        if (!await _webItemSecurity.IsAvailableForUserAsync(ProductID, userId))
        {
            return false;
        }

        var folderWithShare = (FolderWithShare)data;
        var folder = folderWithShare.Folder;
        var shareRecord = folderWithShare.ShareRecord;

        bool targetCond;
        if (feed.Target != null)
        {
            if (shareRecord != null && shareRecord.Owner == userId)
            {
                return false;
            }

            var owner = (Guid)feed.Target;
            var groupUsers = (await _userManager.GetUsersByGroupAsync(owner)).Select(x => x.Id).ToList();
            if (groupUsers.Count == 0)
            {
                groupUsers.Add(owner);
            }

            targetCond = groupUsers.Contains(userId);
        }
        else
        {
            targetCond = true;
        }

        return targetCond && await _fileSecurity.CanReadAsync(folder, userId);
    }

    public override async Task<IEnumerable<Tuple<Feed.Aggregator.Feed, object>>> GetFeeds(FeedFilter filter)
    {
        var rooms = await _folderDao.GetFeedsForRoomsAsync(filter.Tenant, filter.Time.From, filter.Time.To).ToListAsync();

        return rooms.Select(f => new Tuple<Feed.Aggregator.Feed, object>(ToFeed(f), f));
    }

    public override async Task<IEnumerable<int>> GetTenantsWithFeeds(DateTime fromTime)
    {
        return await _folderDao.GetTenantsWithRoomsFeedsAsync(fromTime).ToListAsync();
    }

    private Feed.Aggregator.Feed ToFeed(FolderWithShare folderWithSecurity)
    {
        var room = folderWithSecurity.Folder;
        var shareRecord = folderWithSecurity.ShareRecord;

        if (shareRecord == null)
        {
            var roomCreatedUtc = _tenantUtil.DateTimeToUtc(room.CreateOn);

            return new Feed.Aggregator.Feed(room.CreateBy, roomCreatedUtc)
            {
                Item = RoomItem,
                ItemId = room.Id.ToString(),
                Product = Product,
                Module = Name,
                Title = room.Title,
                ExtraLocationTitle = FilesUCResource.VirtualRooms,
                ExtraLocation = room.ParentId.ToString(),
                Keywords = room.Title,
                AdditionalInfo = ((int)room.FolderType).ToString(),
                AdditionalInfo4 = room.Private ? "private" : null,
                GroupId = GetGroupId(RoomItem, room.CreateBy, roomCreatedUtc, room.ParentId.ToString())
            };
        }

        var feed = new Feed.Aggregator.Feed(shareRecord.Owner, shareRecord.TimeStamp)
        {
            Item = SharedRoomItem,
            ItemId = $"{shareRecord.Subject}_{Guid.NewGuid()}",
            Product = Product,
            Module = Name,
            Title = room.Title,
            ExtraLocationTitle = FilesUCResource.VirtualRooms,
            ExtraLocation = room.ParentId.ToString(),
            Keywords = room.Title,
            AdditionalInfo = ((int)room.FolderType).ToString(),
            AdditionalInfo2 = ((int)shareRecord.Share).ToString(),
            AdditionalInfo3 = ((int)shareRecord.SubjectType).ToString(),
            AdditionalInfo4 = room.Private ? "private" : null,
            Target = shareRecord.Subject,
            GroupId = GetGroupId(SharedRoomItem, shareRecord.Owner, shareRecord.TimeStamp, room.ParentId.ToString()),
            ContextId = $"{RoomItem}_{room.Id}"
        };

        return feed;
    }
}