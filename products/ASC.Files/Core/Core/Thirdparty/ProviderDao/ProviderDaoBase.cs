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

namespace ASC.Files.Thirdparty.ProviderDao;

internal class ProviderDaoBase : ThirdPartyProviderDao, IDisposable
{
    private List<IDaoSelector> _selectors;
    private List<IDaoSelector> Selectors
    {
        get => _selectors ??= new List<IDaoSelector>
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

    private int _tenantID;
    private int TenantID
    {
        get
        {
            if (_tenantID == 0)
            {
                _tenantID = TenantManager.GetCurrentTenant().Id;
            }

            return _tenantID;
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

    protected readonly IServiceProvider ServiceProvider;
    protected readonly TenantManager TenantManager;
    protected readonly SecurityDao<string> SecurityDao;
    protected readonly TagDao<string> TagDao;
    protected readonly CrossDao CrossDao;

    protected bool IsCrossDao(string id1, string id2)
    {
        if (id2 == null || id1 == null)
        {
            return false;
        }

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

        foreach (var id in ids)
        {
            var firstEntry = await entries.FirstOrDefaultAsync(y => y.Id.Equals(id));

            if (firstEntry != null)
            {
                firstEntry.Shared = true;
            }
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
        if (_selectors != null)
        {
            _selectors.ForEach(r => r.Dispose());
        }
    }
}
