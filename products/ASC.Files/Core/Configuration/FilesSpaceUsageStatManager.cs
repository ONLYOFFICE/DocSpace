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

namespace ASC.Web.Files;

[Scope]
public class FilesSpaceUsageStatManager : SpaceUsageStatManager
{
    private readonly IDbContextFactory<FilesDbContext> _dbContextFactory;
    private readonly TenantManager _tenantManager;
    private readonly UserManager _userManager;
    private readonly UserPhotoManager _userPhotoManager;
    private readonly DisplayUserSettingsHelper _displayUserSettingsHelper;
    private readonly CommonLinkUtility _commonLinkUtility;
    private readonly GlobalFolderHelper _globalFolderHelper;
    private readonly PathProvider _pathProvider;

    public FilesSpaceUsageStatManager(
        IDbContextFactory<FilesDbContext> dbContextFactory,
        TenantManager tenantManager,
        UserManager userManager,
        UserPhotoManager userPhotoManager,
        DisplayUserSettingsHelper displayUserSettingsHelper,
        CommonLinkUtility commonLinkUtility,
        GlobalFolderHelper globalFolderHelper,
        PathProvider pathProvider)
    {
        _dbContextFactory = dbContextFactory;
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
        using var filesDbContext = _dbContextFactory.CreateDbContext();
        var myFiles = filesDbContext.Files
            .Join(filesDbContext.Tree, a => a.ParentId, b => b.FolderId, (file, tree) => new { file, tree })
            .Join(filesDbContext.BunchObjects, a => a.tree.ParentId.ToString(), b => b.LeftNode, (fileTree, bunch) => new { fileTree.file, fileTree.tree, bunch })
            .Where(r => r.file.TenantId == r.bunch.TenantId)
            .Where(r => r.file.TenantId == _tenantManager.GetCurrentTenant().Id)
            .Where(r => r.bunch.RightNode.StartsWith("files/my/") || r.bunch.RightNode.StartsWith("files/trash/"))
            .GroupBy(r => r.file.CreateBy)
            .Select(r => new { CreateBy = r.Key, Size = r.Sum(a => a.file.ContentLength) });

        var commonFiles = filesDbContext.Files
            .Join(filesDbContext.Tree, a => a.ParentId, b => b.FolderId, (file, tree) => new { file, tree })
            .Join(filesDbContext.BunchObjects, a => a.tree.ParentId.ToString(), b => b.LeftNode, (fileTree, bunch) => new { fileTree.file, fileTree.tree, bunch })
            .Where(r => r.file.TenantId == r.bunch.TenantId)
            .Where(r => r.file.TenantId == _tenantManager.GetCurrentTenant().Id)
            .Where(r => r.bunch.RightNode.StartsWith("files/common/"))
            .GroupBy(r => Constants.LostUser.Id)
            .Select(r => new { CreateBy = Constants.LostUser.Id, Size = r.Sum(a => a.file.ContentLength) });

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
                    item.ImgUrl = await user.GetSmallPhotoURL(_userPhotoManager);
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
    private readonly IDbContextFactory<FilesDbContext> _dbContextFactory;
    private readonly TenantManager _tenantManager;
    private readonly GlobalFolder _globalFolder;
    private readonly FileMarker _fileMarker;
    private readonly IDaoFactory _daoFactory;

    public FilesUserSpaceUsage(
        IDbContextFactory<FilesDbContext> dbContextFactory,
        TenantManager tenantManager,
        GlobalFolder globalFolder,
        FileMarker fileMarker,
        IDaoFactory daoFactory)
    {
        _dbContextFactory = dbContextFactory;
        _tenantManager = tenantManager;
        _globalFolder = globalFolder;
        _fileMarker = fileMarker;
        _daoFactory = daoFactory;
    }

    public async Task<long> GetUserSpaceUsageAsync(Guid userId)
    {
        var tenantId = _tenantManager.GetCurrentTenant().Id;
        var my = _globalFolder.GetFolderMy(_fileMarker, _daoFactory);
        var trash = await _globalFolder.GetFolderTrashAsync<int>(_daoFactory);

        using var filesDbContext = _dbContextFactory.CreateDbContext();
        return await filesDbContext.Files
            .Where(r => r.TenantId == tenantId && r.CreateBy == userId && (r.ParentId == my || r.ParentId == trash))
            .SumAsync(r => r.ContentLength);
    }
}
