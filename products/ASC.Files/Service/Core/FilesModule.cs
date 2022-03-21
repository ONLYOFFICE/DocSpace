
using System;
using System.Collections.Generic;
using System.Linq;

using ASC.Core;
using ASC.Feed;
using ASC.Feed.Data;
using ASC.Files.Core;
using ASC.Files.Core.Security;
using ASC.Web.Core;
using ASC.Web.Core.Files;

using FeedModule = ASC.Feed.Aggregator.Modules.FeedModule;

namespace ASC.Files.Service.Core
{
    public class FilesModule : FeedModule
    {
        private const string fileItem = "file";
        private const string sharedFileItem = "sharedFile";

        public override string Name => Constants.FilesModule;

        public override string Product => "documents";

        public override Guid ProductID => WebItemManager.DocumentsProductID;

        protected override string DbId => "files";

        private IFileDao<int> FileDao { get; }
        private IFolderDao<int> FolderDao { get; }
        private UserManager UserManager { get; }
        private FilesLinkUtility FilesLinkUtility { get; }
        private FileSecurity FileSecurity { get; }

        public FilesModule(
            TenantManager tenantManager,
            UserManager userManager,
            WebItemSecurity webItemSecurity,
            FilesLinkUtility filesLinkUtility,
            FileSecurity fileSecurity,
            IDaoFactory daoFactory)
            : base(tenantManager, webItemSecurity)
        {
            FileDao = daoFactory.GetFileDao<int>();
            FolderDao = daoFactory.GetFolderDao<int>();
            UserManager = userManager;
            FilesLinkUtility = filesLinkUtility;
            FileSecurity = fileSecurity;
        }

        public override bool VisibleFor(Feed.Aggregator.Feed feed, object data, Guid userId)
        {
            if (!WebItemSecurity.IsAvailableForUser(ProductID, userId)) return false;

            var tuple = ((File<int>, SmallShareRecord))data;
            var file = tuple.Item1;
            var shareRecord = tuple.Item2;

            bool targetCond;
            if (feed.Target != null)
            {
                if (shareRecord != null && shareRecord.ShareBy == userId) return false;

                var owner = (Guid)feed.Target;
                var groupUsers = UserManager.GetUsersByGroup(owner).Select(x => x.ID).ToList();
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

            return targetCond && FileSecurity.CanReadAsync(file, userId).Result;
        }

        public override void VisibleFor(List<Tuple<FeedRow, object>> feed, Guid userId)
        {
            if (!WebItemSecurity.IsAvailableForUser(ProductID, userId)) return;

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

            var canRead = FileSecurity.CanReadAsync(files, userId).Result.Where(r => r.Item2).ToList();

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
            var files = FileDao.GetFeedsAsync(filter.Tenant, filter.Time.From, filter.Time.To).Result
                .Where(f => f.Item1.RootFolderType != FolderType.TRASH && f.Item1.RootFolderType != FolderType.BUNCH)
                .ToList();

            var folderIDs = files.Select(r => r.Item1.FolderID).ToList();
            var folders = FolderDao.GetFoldersAsync(folderIDs, checkShare: false).ToListAsync().Result;

            return files.Select(f => new Tuple<Feed.Aggregator.Feed, object>(ToFeed(f, folders.FirstOrDefault(r => r.ID.Equals(f.Item1.FolderID))), f));
        }

        public override IEnumerable<int> GetTenantsWithFeeds(DateTime fromTime)
        {
            return FileDao.GetTenantsWithFeedsAsync(fromTime).Result;
        }

        private Feed.Aggregator.Feed ToFeed((File<int>, SmallShareRecord) tuple, Folder<int> rootFolder)
        {
            var file = tuple.Item1;
            var shareRecord = tuple.Item2;

            if (shareRecord != null)
            {
                var feed = new Feed.Aggregator.Feed(shareRecord.ShareBy, shareRecord.ShareOn, true)
                {
                    Item = sharedFileItem,
                    ItemId = string.Format("{0}_{1}", file.ID, shareRecord.ShareTo),
                    ItemUrl = FilesLinkUtility.GetFileRedirectPreviewUrl(file.ID, true),
                    Product = Product,
                    Module = Name,
                    Title = file.Title,
                    ExtraLocation = rootFolder.FolderType == FolderType.DEFAULT ? rootFolder.Title : string.Empty,
                    ExtraLocationUrl = rootFolder.FolderType == FolderType.DEFAULT ? FilesLinkUtility.GetFileRedirectPreviewUrl(file.FolderID, false) : string.Empty,
                    AdditionalInfo = file.ContentLengthString,
                    Keywords = file.Title,
                    HasPreview = false,
                    CanComment = false,
                    Target = shareRecord.ShareTo,
                    GroupId = GetGroupId(sharedFileItem, shareRecord.ShareBy, file.FolderID.ToString())
                };

                return feed;
            }

            var updated = file.Version != 1;
            return new Feed.Aggregator.Feed(file.ModifiedBy, file.ModifiedOn, true)
            {
                Item = fileItem,
                ItemId = string.Format("{0}_{1}", file.ID, file.Version > 1 ? 1 : 0),
                ItemUrl = FilesLinkUtility.GetFileRedirectPreviewUrl(file.ID, true),
                Product = Product,
                Module = Name,
                Action = updated ? FeedAction.Updated : FeedAction.Created,
                Title = file.Title,
                ExtraLocation = rootFolder.FolderType == FolderType.DEFAULT ? rootFolder.Title : string.Empty,
                ExtraLocationUrl = rootFolder.FolderType == FolderType.DEFAULT ? FilesLinkUtility.GetFileRedirectPreviewUrl(file.FolderID, false) : string.Empty,
                AdditionalInfo = file.ContentLengthString,
                Keywords = file.Title,
                HasPreview = false,
                CanComment = false,
                Target = null,
                GroupId = GetGroupId(fileItem, file.ModifiedBy, file.FolderID.ToString(), updated ? 1 : 0)
            };
        }

        private bool IsTarget(object target, Guid userId)
        {
            if (target == null) return true;
            var owner = (Guid)target;
            var groupUsers = UserManager.GetUsersByGroup(owner).Select(x => x.ID).ToList();
            if (groupUsers.Count == 0)
            {
                groupUsers.Add(owner);
            }

            return groupUsers.Contains(userId);
        }
    }
}
