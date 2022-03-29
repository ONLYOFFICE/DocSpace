using System;
using System.Collections.Generic;
using System.Linq;

using ASC.Core;
using ASC.Feed;
using ASC.Files.Core;
using ASC.Files.Core.Security;
using ASC.Web.Core;
using ASC.Web.Core.Files;

using FeedModule = ASC.Feed.Aggregator.Modules.FeedModule;

namespace ASC.Files.Service.Core
{
    public class FoldersModule : FeedModule
    {
        private const string folderItem = "folder";
        private const string sharedFolderItem = "sharedFolder";

        protected override string DbId => Constants.FilesDbId;

        public override string Name => Constants.FoldersModule;

        public override string Product => "documents";

        public override Guid ProductID => WebItemManager.DocumentsProductID;

        private UserManager UserManager { get; }
        private FilesLinkUtility FilesLinkUtility { get; }
        private FileSecurity FileSecurity { get; }

        private IFolderDao<int> FolderDao { get; }

        public FoldersModule(
            TenantManager tenantManager,
            UserManager userManager,
            WebItemSecurity webItemSecurity,
            FilesLinkUtility filesLinkUtility,
            FileSecurity fileSecurity,
            IDaoFactory daoFactory)
            : base(tenantManager, webItemSecurity)
        {
            UserManager = userManager;
            FilesLinkUtility = filesLinkUtility;
            FileSecurity = fileSecurity;
            FolderDao = daoFactory.GetFolderDao<int>();
        }

        public override bool VisibleFor(Feed.Aggregator.Feed feed, object data, Guid userId)
        {
            if (!WebItemSecurity.IsAvailableForUser(ProductID, userId)) return false;

            var tuple = (Tuple<Folder<int>, SmallShareRecord>)data;
            var folder = tuple.Item1;
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

            return targetCond && FileSecurity.CanReadAsync(folder, userId).Result;
        }

        public override IEnumerable<int> GetTenantsWithFeeds(DateTime fromTime)
        {
            return FolderDao.GetTenantsWithFeedsForFoldersAsync(fromTime).Result;
        }

        public override IEnumerable<Tuple<Feed.Aggregator.Feed, object>> GetFeeds(FeedFilter filter)
        {
            var folders = FolderDao.GetFeedsForFoldersAsync(filter.Tenant, filter.Time.From, filter.Time.To).Result
                        .Where(f => f.Item1.RootFolderType != FolderType.TRASH && f.Item1.RootFolderType != FolderType.BUNCH)
                        .ToList();

            var parentFolderIDs = folders.Select(r => r.Item1.FolderID).ToList();
            var parentFolders = FolderDao.GetFoldersAsync(parentFolderIDs, checkShare: false).ToListAsync().Result;

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
                    Item = sharedFolderItem,
                    ItemId = string.Format("{0}_{1}", folder.ID, shareRecord.ShareTo),
                    ItemUrl = FilesLinkUtility.GetFileRedirectPreviewUrl(folder.ID, false),
                    Product = Product,
                    Module = Name,
                    Title = folder.Title,
                    ExtraLocation = rootFolder.FolderType == FolderType.DEFAULT ? rootFolder.Title : string.Empty,
                    ExtraLocationUrl = rootFolder.FolderType == FolderType.DEFAULT ? FilesLinkUtility.GetFileRedirectPreviewUrl(folder.FolderID, false) : string.Empty,
                    Keywords = folder.Title,
                    HasPreview = false,
                    CanComment = false,
                    Target = shareRecord.ShareTo,
                    GroupId = GetGroupId(sharedFolderItem, shareRecord.ShareBy, folder.FolderID.ToString())
                };

                return feed;
            }

            return new Feed.Aggregator.Feed(folder.CreateBy, folder.CreateOn)
            {
                Item = folderItem,
                ItemId = folder.ID.ToString(),
                ItemUrl = FilesLinkUtility.GetFileRedirectPreviewUrl(folder.ID, false),
                Product = Product,
                Module = Name,
                Title = folder.Title,
                ExtraLocation = rootFolder.FolderType == FolderType.DEFAULT ? rootFolder.Title : string.Empty,
                ExtraLocationUrl = rootFolder.FolderType == FolderType.DEFAULT ? FilesLinkUtility.GetFileRedirectPreviewUrl(folder.FolderID, false) : string.Empty,
                Keywords = folder.Title,
                HasPreview = false,
                CanComment = false,
                Target = null,
                GroupId = GetGroupId(folderItem, folder.CreateBy, folder.FolderID.ToString())
            };
        }
    }
}
