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


using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using ASC.Common;
using ASC.Core;
using ASC.Core.Common.EF;
using ASC.Core.Common.EF.Context;
using ASC.Core.Users;
using ASC.Files.Core;
using ASC.Files.Core.Resources;
using ASC.Web.Core;
using ASC.Web.Core.Users;
using ASC.Web.Files.Classes;
using ASC.Web.Files.Utils;
using ASC.Web.Studio.Utility;

using Microsoft.EntityFrameworkCore;

namespace ASC.Web.Files
{
    [Scope]
    public class FilesSpaceUsageStatManager : SpaceUsageStatManager
    {
        private Lazy<ASC.Files.Core.EF.FilesDbContext> LazyFilesDbContext { get; }
        private ASC.Files.Core.EF.FilesDbContext FilesDbContext { get => LazyFilesDbContext.Value; }
        private TenantManager TenantManager { get; }
        private UserManager UserManager { get; }
        private UserPhotoManager UserPhotoManager { get; }
        private DisplayUserSettingsHelper DisplayUserSettingsHelper { get; }
        private CommonLinkUtility CommonLinkUtility { get; }
        private GlobalFolderHelper GlobalFolderHelper { get; }
        private PathProvider PathProvider { get; }

        public FilesSpaceUsageStatManager(
            DbContextManager<ASC.Files.Core.EF.FilesDbContext> dbContextManager,
            TenantManager tenantManager,
            UserManager userManager,
            UserPhotoManager userPhotoManager,
            DisplayUserSettingsHelper displayUserSettingsHelper,
            CommonLinkUtility commonLinkUtility,
            GlobalFolderHelper globalFolderHelper,
            PathProvider pathProvider)
        {
            LazyFilesDbContext = new Lazy<ASC.Files.Core.EF.FilesDbContext>(() => dbContextManager.Get(FileConstant.DatabaseId));
            TenantManager = tenantManager;
            UserManager = userManager;
            UserPhotoManager = userPhotoManager;
            DisplayUserSettingsHelper = displayUserSettingsHelper;
            CommonLinkUtility = commonLinkUtility;
            GlobalFolderHelper = globalFolderHelper;
            PathProvider = pathProvider;
        }

        public override ValueTask<List<UsageSpaceStatItem>> GetStatDataAsync()
        {
            var myFiles = FilesDbContext.Files
                .AsQueryable()
                .Join(FilesDbContext.Tree, a => a.FolderId, b => b.FolderId, (file, tree) => new { file, tree })
                .Join(FilesDbContext.BunchObjects, a => a.tree.ParentId.ToString(), b => b.LeftNode, (fileTree, bunch) => new { fileTree.file, fileTree.tree, bunch })
                .Where(r => r.file.TenantId == r.bunch.TenantId)
                .Where(r => r.file.TenantId == TenantManager.GetCurrentTenant().TenantId)
                .Where(r => r.bunch.RightNode.StartsWith("files/my/") || r.bunch.RightNode.StartsWith("files/trash/"))
                .GroupBy(r => r.file.CreateBy)
                .Select(r => new { CreateBy = r.Key, Size = r.Sum(a => a.file.ContentLength) });

            var commonFiles = FilesDbContext.Files
                .AsQueryable()
                .Join(FilesDbContext.Tree, a => a.FolderId, b => b.FolderId, (file, tree) => new { file, tree })
                .Join(FilesDbContext.BunchObjects, a => a.tree.ParentId.ToString(), b => b.LeftNode, (fileTree, bunch) => new { fileTree.file, fileTree.tree, bunch })
                .Where(r => r.file.TenantId == r.bunch.TenantId)
                .Where(r => r.file.TenantId == TenantManager.GetCurrentTenant().TenantId)
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
                    var user = UserManager.GetUsers(userId);
                    var item = new UsageSpaceStatItem { SpaceUsage = await items.SumAsync() };
                    if (user.Equals(Constants.LostUser))
                    {
                        item.Name = FilesUCResource.CorporateFiles;
                        item.ImgUrl = PathProvider.GetImagePath("corporatefiles_big.png");
                        item.Url = await PathProvider.GetFolderUrlByIdAsync(await GlobalFolderHelper.FolderCommonAsync);
                    }
                    else
                    {
                        item.Name = user.DisplayUserName(false, DisplayUserSettingsHelper);
                        item.ImgUrl = user.GetSmallPhotoURL(UserPhotoManager);
                        item.Url = user.GetUserProfilePageURL(CommonLinkUtility);
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
        private Lazy<ASC.Files.Core.EF.FilesDbContext> LazyFilesDbContext { get; }
        private ASC.Files.Core.EF.FilesDbContext FilesDbContext { get => LazyFilesDbContext.Value; }
        private TenantManager TenantManager { get; }
        private GlobalFolder GlobalFolder { get; }
        public FileMarker FileMarker { get; }
        public IDaoFactory DaoFactory { get; }

        public FilesUserSpaceUsage(
            DbContextManager<ASC.Files.Core.EF.FilesDbContext> dbContextManager,
            TenantManager tenantManager,
            GlobalFolder globalFolder,
            FileMarker fileMarker,
            IDaoFactory daoFactory)
        {
            LazyFilesDbContext = new Lazy<ASC.Files.Core.EF.FilesDbContext>(() => dbContextManager.Get(FileConstant.DatabaseId));
            TenantManager = tenantManager;
            GlobalFolder = globalFolder;
            FileMarker = fileMarker;
            DaoFactory = daoFactory;
        }

        public async Task<long> GetUserSpaceUsageAsync(Guid userId)
        {
            var tenantId = TenantManager.GetCurrentTenant().TenantId;
            var my = GlobalFolder.GetFolderMy(FileMarker, DaoFactory);
            var trash = await GlobalFolder.GetFolderTrashAsync<int>(DaoFactory);

            return await FilesDbContext.Files
                .AsQueryable()
                .Where(r => r.TenantId == tenantId && (r.FolderId == my || r.FolderId == trash))
                .SumAsync(r => r.ContentLength);
        }
    }
}