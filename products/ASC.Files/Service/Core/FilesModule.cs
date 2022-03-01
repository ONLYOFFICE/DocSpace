using FeedModule = ASC.Feed.Aggregator.Modules.FeedModule;

namespace ASC.Files.Service.Core;

public class FilesModule : FeedModule
{
    public override Guid ProductID => WebItemManager.DocumentsProductID;
    public override string Name => Constants.FilesModule;
    public override string Product => "documents";
    protected override string DbId => "files";

    private const string FileItem = "file";
    private const string SharedFileItem = "sharedFile";

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
        if (!WebItemSecurity.IsAvailableForUser(ProductID, userId))
        {
            return false;
        }

        var tuple = ((File<int>, SmallShareRecord))data;
        var file = tuple.Item1;
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

        return targetCond && _fileSecurity.CanReadAsync(file, userId).Result;
    }

    public override void VisibleFor(List<Tuple<FeedRow, object>> feed, Guid userId)
    {
        if (!WebItemSecurity.IsAvailableForUser(ProductID, userId))
        {
            return;
        }

        var feed1 = feed.Select(r =>
        {
            var tuple = ((File<int>, SmallShareRecord))r.Item2;

            return new Tuple<FeedRow, File<int>, SmallShareRecord>(r.Item1, tuple.Item1, tuple.Item2);
        })
        .ToList();

        var files = feed1.Where(r => r.Item1.Feed.Target == null).Select(r => r.Item2).ToList();

        foreach (var f in feed1.Where(r => r.Item1.Feed.Target != null && !(r.Item3 != null && r.Item3.ShareBy == userId)))
        {
            var file = f.Item2;
            if (IsTarget(f.Item1.Feed.Target, userId) && !files.Any(r => r.UniqID.Equals(file.UniqID)))
            {
                files.Add(file);
            }
        }

        var canRead = _fileSecurity.CanReadAsync(files, userId).Result.Where(r => r.Item2).ToList();

        foreach (var f in feed1)
        {
            if (IsTarget(f.Item1.Feed.Target, userId) && canRead.Any(r => r.Item1.ID.Equals(f.Item2.ID)))
            {
                f.Item1.Users.Add(userId);
            }
        }
    }

    public override IEnumerable<Tuple<Feed.Aggregator.Feed, object>> GetFeeds(FeedFilter filter)
    {
        var files = _fileDao.GetFeedsAsync(filter.Tenant, filter.Time.From, filter.Time.To).Result
            .Where(f => f.Item1.RootFolderType != FolderType.TRASH && f.Item1.RootFolderType != FolderType.BUNCH)
            .ToList();

        var folderIDs = files.Select(r => r.Item1.FolderID).ToList();
        var folders = _folderDao.GetFoldersAsync(folderIDs, checkShare: false).ToListAsync().Result;

        return files.Select(f => new Tuple<Feed.Aggregator.Feed, object>(ToFeed(f, folders.FirstOrDefault(r => r.ID.Equals(f.Item1.FolderID))), f));
    }

    public override IEnumerable<int> GetTenantsWithFeeds(DateTime fromTime)
    {
        return _fileDao.GetTenantsWithFeedsAsync(fromTime).Result;
    }

    private Feed.Aggregator.Feed ToFeed((File<int>, SmallShareRecord) tuple, Folder<int> rootFolder)
    {
        var file = tuple.Item1;
        var shareRecord = tuple.Item2;

        if (shareRecord != null)
        {
            var feed = new Feed.Aggregator.Feed(shareRecord.ShareBy, shareRecord.ShareOn, true)
            {
                Item = SharedFileItem,
                ItemId = string.Format("{0}_{1}", file.ID, shareRecord.ShareTo),
                ItemUrl = _filesLinkUtility.GetFileRedirectPreviewUrl(file.ID, true),
                Product = Product,
                Module = Name,
                Title = file.Title,
                ExtraLocation = rootFolder.FolderType == FolderType.DEFAULT ? rootFolder.Title : string.Empty,
                ExtraLocationUrl = rootFolder.FolderType == FolderType.DEFAULT ? _filesLinkUtility.GetFileRedirectPreviewUrl(file.FolderID, false) : string.Empty,
                AdditionalInfo = file.ContentLengthString,
                Keywords = file.Title,
                HasPreview = false,
                CanComment = false,
                Target = shareRecord.ShareTo,
                GroupId = GetGroupId(SharedFileItem, shareRecord.ShareBy, file.FolderID.ToString())
            };

            return feed;
        }

        var updated = file.Version != 1;

        return new Feed.Aggregator.Feed(file.ModifiedBy, file.ModifiedOn, true)
        {
            Item = FileItem,
            ItemId = string.Format("{0}_{1}", file.ID, file.Version > 1 ? 1 : 0),
            ItemUrl = _filesLinkUtility.GetFileRedirectPreviewUrl(file.ID, true),
            Product = Product,
            Module = Name,
            Action = updated ? FeedAction.Updated : FeedAction.Created,
            Title = file.Title,
            ExtraLocation = rootFolder.FolderType == FolderType.DEFAULT ? rootFolder.Title : string.Empty,
            ExtraLocationUrl = rootFolder.FolderType == FolderType.DEFAULT ? _filesLinkUtility.GetFileRedirectPreviewUrl(file.FolderID, false) : string.Empty,
            AdditionalInfo = file.ContentLengthString,
            Keywords = file.Title,
            HasPreview = false,
            CanComment = false,
            Target = null,
            GroupId = GetGroupId(FileItem, file.ModifiedBy, file.FolderID.ToString(), updated ? 1 : 0)
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
