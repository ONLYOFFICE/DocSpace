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

using FeedModule = ASC.Feed.Aggregator.Modules.FeedModule;

namespace ASC.Files.Service.Core;

public class FilesModule : FeedModule
{
    public override Guid ProductID => WebItemManager.DocumentsProductID;
    public override string Name => Constants.FilesModule;
    public override string Product => Constants.Documents;
    protected override string DbId => Constants.FilesDbId;

    private const string FileItem = Constants.FileItem;
    private const string SharedFileItem = Constants.SharedFileItem;

    private readonly FileSecurity _fileSecurity;
    private readonly FilesLinkUtility _filesLinkUtility;
    private readonly IFileDao<int> _fileDao;
    private readonly IFolderDao<int> _folderDao;
    private readonly UserManager _userManager;

    public FilesModule(
        TenantManager tenantManager,
        UserManager userManager,
        WebItemSecurity webItemSecurity,
        FilesLinkUtility filesLinkUtility,
        FileSecurity fileSecurity,
        IDaoFactory daoFactory)
        : base(tenantManager, webItemSecurity)
    {
        _fileDao = daoFactory.GetFileDao<int>();
        _folderDao = daoFactory.GetFolderDao<int>();
        _userManager = userManager;
        _filesLinkUtility = filesLinkUtility;
        _fileSecurity = fileSecurity;
    }

    public override bool VisibleFor(Feed.Aggregator.Feed feed, object data, Guid userId)
    {
        if (!_webItemSecurity.IsAvailableForUser(ProductID, userId))
        {
            return false;
        }

        var fileWithShare = (FileWithShare)data;
        var file = fileWithShare.File;
        var shareRecord = fileWithShare.ShareRecord;

        bool targetCond;
        if (feed.Target != null)
        {
            if (shareRecord != null && shareRecord.Owner == userId)
            {
                return false;
            }

            var owner = (Guid)feed.Target;
            var groupUsers = _userManager.GetUsersByGroup(owner).Select(x => x.Id).ToList();
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

        return targetCond && _fileSecurity.CanReadAsync(file, userId).Result;
    }

    public override void VisibleFor(List<Tuple<FeedRow, object>> feed, Guid userId)
    {
        if (!_webItemSecurity.IsAvailableForUser(ProductID, userId))
        {
            return;
        }

        var feed1 = feed.Select(r =>
        {
            var fileWithShare = (FileWithShare)r.Item2;

            return new Tuple<FeedRow, File<int>, SmallShareRecord>(r.Item1, fileWithShare.File, fileWithShare.ShareRecord);
        })
        .ToList();

        var files = feed1.Where(r => r.Item1.Feed.Target == null).Select(r => r.Item2).ToList();

        foreach (var f in feed1.Where(r => r.Item1.Feed.Target != null && !(r.Item3 != null && r.Item3.Owner == userId)))
        {
            var file = f.Item2;
            if (IsTarget(f.Item1.Feed.Target, userId) && !files.Any(r => r.UniqID.Equals(file.UniqID)))
            {
                files.Add(file);
            }
        }

        var canRead = _fileSecurity.CanReadAsync(files.ToAsyncEnumerable(), userId).ToListAsync().Result.Where(r => r.Item2).ToList();

        foreach (var f in feed1)
        {
            if (IsTarget(f.Item1.Feed.Target, userId) && canRead.Any(r => r.Item1.Id.Equals(f.Item2.Id)))
            {
                f.Item1.Users.Add(userId);
            }
        }
    }

    public override IEnumerable<Tuple<Feed.Aggregator.Feed, object>> GetFeeds(FeedFilter filter)
    {
        var files = _fileDao.GetFeedsAsync(filter.Tenant, filter.Time.From, filter.Time.To)
            .Where(f => f.File.RootFolderType != FolderType.TRASH && f.File.RootFolderType != FolderType.BUNCH)
            .Where(f => f.ShareRecord == null)
            .ToListAsync().Result;

        var folderIDs = files.Select(r => r.File.ParentId).ToList();
        var folders = _folderDao.GetFoldersAsync(folderIDs, checkShare: false).ToListAsync().Result;
        var roomsIds = _folderDao.GetParentRoomsAsync(folderIDs).ToDictionaryAsync(k => k.FolderId, v => v.ParentRoomId).Result;

        return files.Select(f => new Tuple<Feed.Aggregator.Feed, object>(ToFeed(f, folders.FirstOrDefault(r => r.Id.Equals(f.File.ParentId)), 
            roomsIds.GetValueOrDefault(f.File.ParentId)), f));
    }

    public override IEnumerable<int> GetTenantsWithFeeds(DateTime fromTime)
    {
        return _fileDao.GetTenantsWithFeedsAsync(fromTime).ToListAsync().Result;
    }

    private Feed.Aggregator.Feed ToFeed(FileWithShare tuple, Folder<int> parentFolder, int roomId)
    {
        var file = tuple.File;
        var shareRecord = tuple.ShareRecord;
        var contextId = roomId != default ? $"{RoomsModule.RoomItem}_{roomId}" : null;

        if (shareRecord != null)
        {
            var feed = new Feed.Aggregator.Feed(shareRecord.Owner, shareRecord.TimeStamp, true)
            {
                Item = SharedFileItem,
                ItemId = string.Format("{0}_{1}", file.Id, shareRecord.Subject),
                Product = Product,
                Module = Name,
                Title = file.Title,
                ExtraLocationTitle = parentFolder.Title,
                ExtraLocation = parentFolder.Id.ToString(),
                AdditionalInfo = file.ContentLengthString,
                AdditionalInfo2 = file.Encrypted ? "Encrypted" : string.Empty,
                Keywords = file.Title,
                Target = shareRecord.Subject,
                GroupId = GetGroupId(SharedFileItem, shareRecord.Owner, file.ParentId.ToString()),
                ContextId = contextId
            };

            return feed;
        }

        var updated = file.Version != 1;

        return new Feed.Aggregator.Feed(file.ModifiedBy, file.ModifiedOn, true)
        {
            Item = FileItem,
            ItemId = string.Format("{0}_{1}", file.Id, file.Version > 1 ? 1 : 0),
            Product = Product,
            Module = Name,
            Action = updated ? FeedAction.Updated : FeedAction.Created,
            Title = file.Title,
            ExtraLocationTitle = parentFolder.Title,
            ExtraLocation = parentFolder.Id.ToString(),
            AdditionalInfo = file.ContentLengthString,
            AdditionalInfo2 = file.Encrypted ? "Encrypted" : string.Empty,
            Keywords = file.Title,
            GroupId = GetGroupId(FileItem, file.ModifiedBy, file.ParentId.ToString(), updated ? 1 : 0),
            ContextId = contextId
        };
    }

    private bool IsTarget(object target, Guid userId)
    {
        if (target == null)
        {
            return true;
        }

        var owner = (Guid)target;
        var groupUsers = _userManager.GetUsersByGroup(owner).Select(x => x.Id).ToList();
        if (groupUsers.Count == 0)
        {
            groupUsers.Add(owner);
        }

        return groupUsers.Contains(userId);
    }
}
