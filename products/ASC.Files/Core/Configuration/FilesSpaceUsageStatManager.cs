/*
 *
 * (c) Copyright Ascensio System Limited 2010-2018
 *
 * This program is freeware. You can redistribute it and/or modify it under the terms of the GNU 
 * General Public License (GPL) version 3 as published by the Free Software Foundation (https://www.gnu.org/copyleft/gpl.html). 
 * In accordance with Section 7(a) of the GNU GPL its Section 15 shall be amended to the effect that 
 * Ascensio System SIA expressly excludes the warranty of non-infringement of any third-party rights.
 *
 * THIS PROGRAM IS DISTRIBUTED WITHOUT ANY WARRANTY; WITHOUT EVEN THE IMPLIED WARRANTY OF MERCHANTABILITY OR
 * FITNESS FOR A PARTICULAR PURPOSE. For more details, see GNU GPL at https://www.gnu.org/copyleft/gpl.html
 *
 * You can contact Ascensio System SIA by email at sales@onlyoffice.com
 *
 * The interactive user interfaces in modified source and object code versions of ONLYOFFICE must display 
 * Appropriate Legal Notices, as required under Section 5 of the GNU GPL version 3.
 *
 * Pursuant to Section 7 § 3(b) of the GNU GPL you must retain the original ONLYOFFICE logo which contains 
 * relevant author attributions when distributing the software. If the display of the logo in its graphic 
 * form is not reasonably feasible for technical reasons, you must include the words "Powered by ONLYOFFICE" 
 * in every copy of the program you distribute. 
 * Pursuant to Section 7 § 3(e) we decline to grant you any rights under trademark law for use of our trademarks.
 *
*/

namespace ASC.Web.Files
{
    [Scope]
    public class FilesSpaceUsageStatManager : SpaceUsageStatManager
    {
        private FilesDbContext FilesDbContext => LazyFilesDbContext.Value;

        private readonly Lazy<FilesDbContext> LazyFilesDbContext;
        private readonly TenantManager _tenantManager;
        private readonly UserManager _userManager;
        private readonly UserPhotoManager _userPhotoManager;
        private readonly DisplayUserSettingsHelper _displayUserSettingsHelper;
        private readonly CommonLinkUtility _commonLinkUtility;
        private readonly GlobalFolderHelper _globalFolderHelper;
        private readonly PathProvider _pathProvider;

        public FilesSpaceUsageStatManager(
            DbContextManager<FilesDbContext> dbContextManager,
            TenantManager tenantManager,
            UserManager userManager,
            UserPhotoManager userPhotoManager,
            DisplayUserSettingsHelper displayUserSettingsHelper,
            CommonLinkUtility commonLinkUtility,
            GlobalFolderHelper globalFolderHelper,
            PathProvider pathProvider)
        {
            LazyFilesDbContext = new Lazy<FilesDbContext>(() => dbContextManager.Get(FileConstant.DatabaseId));
            _tenantManager = tenantManager;
            _userManager = userManager;
            _userPhotoManager = userPhotoManager;
            _displayUserSettingsHelper = displayUserSettingsHelper;
            _commonLinkUtility = commonLinkUtility;
            _globalFolderHelper = globalFolderHelper;
            _pathProvider = pathProvider;
        }

        public override ValueTask<List<UsageSpaceStatItem>> GetStatDataAsync()
        {
            var myFiles = FilesDbContext.Files
                .AsQueryable()
                .Join(FilesDbContext.Tree, a => a.FolderId, b => b.FolderId, (file, tree) => new { file, tree })
                .Join(FilesDbContext.BunchObjects, a => a.tree.ParentId.ToString(), b => b.LeftNode, (fileTree, bunch) => new { fileTree.file, fileTree.tree, bunch })
                .Where(r => r.file.TenantId == r.bunch.TenantId)
                .Where(r => r.file.TenantId == _tenantManager.GetCurrentTenant().TenantId)
                .Where(r => r.bunch.RightNode.StartsWith("files/my/") || r.bunch.RightNode.StartsWith("files/trash/"))
                .GroupBy(r => r.file.CreateBy)
                .Select(r => new { CreateBy = r.Key, Size = r.Sum(a => a.file.ContentLength) });

            var commonFiles = FilesDbContext.Files
                .AsQueryable()
                .Join(FilesDbContext.Tree, a => a.FolderId, b => b.FolderId, (file, tree) => new { file, tree })
                .Join(FilesDbContext.BunchObjects, a => a.tree.ParentId.ToString(), b => b.LeftNode, (fileTree, bunch) => new { fileTree.file, fileTree.tree, bunch })
                .Where(r => r.file.TenantId == r.bunch.TenantId)
                .Where(r => r.file.TenantId == _tenantManager.GetCurrentTenant().TenantId)
                .Where(r => r.bunch.RightNode.StartsWith("files/common/"))
                .GroupBy(r => Constants.LostUser.ID)
                .Select(r => new { CreateBy = Constants.LostUser.ID, Size = r.Sum(a => a.file.ContentLength) });

            return myFiles.Union(commonFiles)
                .AsAsyncEnumerable()
                .GroupByAwait(
                async r => await Task.FromResult(r.CreateBy),
                async r => await Task.FromResult(r.Size),
                async (userId, items) =>
                {
                    var user = _userManager.GetUsers(userId);
                    var item = new UsageSpaceStatItem { SpaceUsage = await items.SumAsync() };
                    if (user.Equals(Constants.LostUser))
                    {
                        item.Name = FilesUCResource.CorporateFiles;
                        item.ImgUrl = _pathProvider.GetImagePath("corporatefiles_big.png");
                        item.Url = await _pathProvider.GetFolderUrlByIdAsync(await _globalFolderHelper.FolderCommonAsync);
                    }
                    else
                    {
                        item.Name = user.DisplayUserName(false, _displayUserSettingsHelper);
                        item.ImgUrl = user.GetSmallPhotoURL(_userPhotoManager);
                        item.Url = user.GetUserProfilePageURL(_commonLinkUtility);
                        item.Disabled = user.Status == EmployeeStatus.Terminated;
                    }
                    return item;
                })
                .OrderByDescending(i => i.SpaceUsage)
                .ToListAsync();

        }
    }

    [Scope]
    public class FilesUserSpaceUsage : IUserSpaceUsage
    {
        private FilesDbContext FilesDbContext => _lazyFilesDbContext.Value;

        private readonly Lazy<FilesDbContext> _lazyFilesDbContext;
        private readonly TenantManager TenantManager;
        private readonly GlobalFolder GlobalFolder;
        private readonly FileMarker _fileMarker;
        private readonly IDaoFactory _daoFactory;

        public FilesUserSpaceUsage(
            DbContextManager<FilesDbContext> dbContextManager,
            TenantManager tenantManager,
            GlobalFolder globalFolder,
            FileMarker fileMarker,
            IDaoFactory daoFactory)
        {
            _lazyFilesDbContext = new Lazy<FilesDbContext>(() => dbContextManager.Get(FileConstant.DatabaseId));
            TenantManager = tenantManager;
            GlobalFolder = globalFolder;
            _fileMarker = fileMarker;
            _daoFactory = daoFactory;
        }

        public async Task<long> GetUserSpaceUsageAsync(Guid userId)
        {
            var tenantId = TenantManager.GetCurrentTenant().TenantId;
            var my = GlobalFolder.GetFolderMy(_fileMarker, _daoFactory);
            var trash = await GlobalFolder.GetFolderTrashAsync<int>(_daoFactory);

            return await FilesDbContext.Files
                .AsQueryable()
                .Where(r => r.TenantId == tenantId && (r.FolderId == my || r.FolderId == trash))
                .SumAsync(r => r.ContentLength);
        }
    }
}