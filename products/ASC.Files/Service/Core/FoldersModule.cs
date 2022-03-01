
using FeedModule = ASC.Feed.Aggregator.Modules.FeedModule;

namespace ASC.Files.Service.Core;

public class FoldersModule : FeedModule
{
    public override Guid ProductID => WebItemManager.DocumentsProductID;
    public override string Name => Constants.FoldersModule;
    public override string Product => "documents";
    protected override string DbId => Constants.FilesDbId;

    private const string FolderItem = "folder";
    private const string SharedFolderItem = "sharedFolder";

    private readonly FileSecurity _fileSecurity;
    private readonly FilesLinkUtility _filesLinkUtility;
    private readonly IFolderDao<int> _folderDao;
    private readonly UserManager _userManager;

    public FoldersModule(
        TenantManager tenantManager,
        UserManager userManager,
        WebItemSecurity webItemSecurity,
        FilesLinkUtility filesLinkUtility,
        FileSecurity fileSecurity,
        IDaoFactory daoFactory)
        : base(tenantManager, webItemSecurity)
    {
        _userManager = userManager;
        _filesLinkUtility = filesLinkUtility;
        _fileSecurity = fileSecurity;
        _folderDao = daoFactory.GetFolderDao<int>();
    }

    public override bool VisibleFor(Feed.Aggregator.Feed feed, object data, Guid userId)
    {
        if (!WebItemSecurity.IsAvailableForUser(ProductID, userId))
        {
            return false;
        }

        var tuple = (Tuple<Folder<int>, SmallShareRecord>)data;
        var folder = tuple.Item1;
        var shareRecord = tuple.Item2;

        bool targetCond;
        if (feed.Target != null)
        {
            if (shareRecord != null && shareRecord.ShareBy == userId)
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

        return targetCond && _fileSecurity.CanReadAsync(folder, userId).Result;
    }

    public override IEnumerable<int> GetTenantsWithFeeds(DateTime fromTime)
    {
        return _folderDao.GetTenantsWithFeedsForFoldersAsync(fromTime).Result;
    }

    public override IEnumerable<Tuple<Feed.Aggregator.Feed, object>> GetFeeds(FeedFilter filter)
    {
        var folders = _folderDao.GetFeedsForFoldersAsync(filter.Tenant, filter.Time.From, filter.Time.To).Result
                    .Where(f => f.Item1.RootFolderType != FolderType.TRASH && f.Item1.RootFolderType != FolderType.BUNCH)
                    .ToList();

        var parentFolderIDs = folders.Select(r => r.Item1.FolderID).ToList();
        var parentFolders = _folderDao.GetFoldersAsync(parentFolderIDs, checkShare: false).ToListAsync().Result;

        return folders.Select(f => new Tuple<Feed.Aggregator.Feed, object>(ToFeed(f, parentFolders.FirstOrDefault(r => r.ID.Equals(f.Item1.FolderID))), f));
    }

    private Feed.Aggregator.Feed ToFeed((Folder<int>, SmallShareRecord) tuple, Folder<int> rootFolder)
    {
        var folder = tuple.Item1;
        var shareRecord = tuple.Item2;

        if (shareRecord != null)
        {
            var feed = new Feed.Aggregator.Feed(shareRecord.ShareBy, shareRecord.ShareOn, true)
            {
                Item = SharedFolderItem,
                ItemId = string.Format("{0}_{1}", folder.ID, shareRecord.ShareTo),
                ItemUrl = _filesLinkUtility.GetFileRedirectPreviewUrl(folder.ID, false),
                Product = Product,
                Module = Name,
                Title = folder.Title,
                ExtraLocation = rootFolder.FolderType == FolderType.DEFAULT ? rootFolder.Title : string.Empty,
                ExtraLocationUrl = rootFolder.FolderType == FolderType.DEFAULT ? _filesLinkUtility.GetFileRedirectPreviewUrl(folder.FolderID, false) : string.Empty,
                Keywords = folder.Title,
                HasPreview = false,
                CanComment = false,
                Target = shareRecord.ShareTo,
                GroupId = GetGroupId(SharedFolderItem, shareRecord.ShareBy, folder.FolderID.ToString())
            };

            return feed;
        }

        return new Feed.Aggregator.Feed(folder.CreateBy, folder.CreateOn)
        {
            Item = FolderItem,
            ItemId = folder.ID.ToString(),
            ItemUrl = _filesLinkUtility.GetFileRedirectPreviewUrl(folder.ID, false),
            Product = Product,
            Module = Name,
            Title = folder.Title,
            ExtraLocation = rootFolder.FolderType == FolderType.DEFAULT ? rootFolder.Title : string.Empty,
            ExtraLocationUrl = rootFolder.FolderType == FolderType.DEFAULT ? _filesLinkUtility.GetFileRedirectPreviewUrl(folder.FolderID, false) : string.Empty,
            Keywords = folder.Title,
            HasPreview = false,
            CanComment = false,
            Target = null,
            GroupId = GetGroupId(FolderItem, folder.CreateBy, folder.FolderID.ToString())
        };
    }
}
