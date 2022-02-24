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
using System.Threading;
using System.Threading.Tasks;

using ASC.Core;
using ASC.Files.Core;
using ASC.Files.Core.Data;
using ASC.Files.Core.Thirdparty;
using ASC.Files.Thirdparty.Box;
using ASC.Files.Thirdparty.Dropbox;
using ASC.Files.Thirdparty.GoogleDrive;
using ASC.Files.Thirdparty.OneDrive;
using ASC.Files.Thirdparty.SharePoint;
using ASC.Files.Thirdparty.Sharpbox;

using Microsoft.Extensions.DependencyInjection;

namespace ASC.Files.Thirdparty.ProviderDao
{
    internal class ProviderDaoBase : ThirdPartyProviderDao, IDisposable
    {
        private List<IDaoSelector> selectors;
        private List<IDaoSelector> Selectors
        {
            get => selectors ??= new List<IDaoSelector>
            {
                //Fill in selectors
                ServiceProvider.GetService<SharpBoxDaoSelector>(),
                ServiceProvider.GetService<SharePointDaoSelector>(),
                ServiceProvider.GetService<GoogleDriveDaoSelector>(),
                ServiceProvider.GetService<BoxDaoSelector>(),
                ServiceProvider.GetService<DropboxDaoSelector>(),
                ServiceProvider.GetService<OneDriveDaoSelector>()
            };
        }

        private int tenantID;
        private int TenantID
        {
            get
            {
                if (tenantID == 0) tenantID = TenantManager.GetCurrentTenant().TenantId;
                return tenantID;
            }
        }

        public ProviderDaoBase(
            IServiceProvider serviceProvider,
            TenantManager tenantManager,
            SecurityDao<string> securityDao,
            TagDao<string> tagDao,
            CrossDao crossDao)
        {
            ServiceProvider = serviceProvider;
            TenantManager = tenantManager;
            SecurityDao = securityDao;
            TagDao = tagDao;
            CrossDao = crossDao;
        }

        protected IServiceProvider ServiceProvider { get; }
        protected TenantManager TenantManager { get; }
        protected SecurityDao<string> SecurityDao { get; }
        protected TagDao<string> TagDao { get; }
        protected CrossDao CrossDao { get; }

        protected bool IsCrossDao(string id1, string id2)
        {
            if (id2 == null || id1 == null)
                return false;
            return !Equals(GetSelector(id1).GetIdCode(id1), GetSelector(id2).GetIdCode(id2));
        }

        public IDaoSelector GetSelector(string id)
        {
            return Selectors.FirstOrDefault(selector => selector.IsMatch(id));
        }

        protected async Task SetSharedPropertyAsync(IAsyncEnumerable<FileEntry<string>> entries)
        {
            var pureShareRecords = await SecurityDao.GetPureShareRecordsAsync(entries);
            var ids = pureShareRecords
                //.Where(x => x.Owner == SecurityContext.CurrentAccount.ID)
                .Select(x => x.EntryId).Distinct();

            foreach(var id in ids)
            {
                var firstEntry = await entries.FirstOrDefaultAsync(y => y.ID.Equals(id));

                if (firstEntry != null)
                    firstEntry.Shared = true;
            }
        }

        protected IEnumerable<IDaoSelector> GetSelectors()
        {
            return Selectors;
        }

        protected internal Task<File<string>> PerformCrossDaoFileCopyAsync(string fromFileId, string toFolderId, bool deleteSourceFile)
        {
            var fromSelector = GetSelector(fromFileId);
            var toSelector = GetSelector(toFolderId);

            return CrossDao.PerformCrossDaoFileCopyAsync(
                fromFileId, fromSelector.GetFileDao(fromFileId), fromSelector.ConvertId,
                toFolderId, toSelector.GetFileDao(toFolderId), toSelector.ConvertId,
                deleteSourceFile);
        }

        protected async Task<File<int>> PerformCrossDaoFileCopyAsync(string fromFileId, int toFolderId, bool deleteSourceFile)
        {
            var fromSelector = GetSelector(fromFileId);
            using var scope = ServiceProvider.CreateScope();
            var tenantManager = scope.ServiceProvider.GetService<TenantManager>();
            tenantManager.SetCurrentTenant(TenantID);

            return await CrossDao.PerformCrossDaoFileCopyAsync(
                fromFileId, fromSelector.GetFileDao(fromFileId), fromSelector.ConvertId,
                toFolderId, scope.ServiceProvider.GetService<IFileDao<int>>(), r => r,
                deleteSourceFile);
        }

        protected Task<Folder<string>> PerformCrossDaoFolderCopyAsync(string fromFolderId, string toRootFolderId, bool deleteSourceFolder, CancellationToken? cancellationToken)
        {
            var fromSelector = GetSelector(fromFolderId);
            var toSelector = GetSelector(toRootFolderId);

            return CrossDao.PerformCrossDaoFolderCopyAsync(
                fromFolderId, fromSelector.GetFolderDao(fromFolderId), fromSelector.GetFileDao(fromFolderId), fromSelector.ConvertId,
                toRootFolderId, toSelector.GetFolderDao(toRootFolderId), toSelector.GetFileDao(toRootFolderId), toSelector.ConvertId,
                deleteSourceFolder, cancellationToken);
        }

        protected Task<Folder<int>> PerformCrossDaoFolderCopyAsync(string fromFolderId, int toRootFolderId, bool deleteSourceFolder, CancellationToken? cancellationToken)
        {
            var fromSelector = GetSelector(fromFolderId);

            return CrossDao.PerformCrossDaoFolderCopyAsync(
                fromFolderId, fromSelector.GetFolderDao(fromFolderId), fromSelector.GetFileDao(fromFolderId), fromSelector.ConvertId,
                toRootFolderId, ServiceProvider.GetService<IFolderDao<int>>(), ServiceProvider.GetService<IFileDao<int>>(), r => r,
                deleteSourceFolder, cancellationToken);
        }

        public void Dispose()
        {
            if (selectors != null)
            {
                selectors.ForEach(r => r.Dispose());
            }
        }
    }
}