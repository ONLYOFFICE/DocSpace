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

internal class ProviderDaoBase : ThirdPartyProviderDao
{
    private int TenantID
    {
        get
        {
            return _tenantManager.GetCurrentTenant().Id;
        }
    }

    public ProviderDaoBase(
        IServiceProvider serviceProvider,
        TenantManager tenantManager,
        CrossDao crossDao,
        SelectorFactory selectorFactory,
        ISecurityDao<string> securityDao)
    {
        _serviceProvider = serviceProvider;
        _tenantManager = tenantManager;
        _securityDao = securityDao;
        _selectorFactory = selectorFactory;
        _crossDao = crossDao;
    }

    protected readonly IServiceProvider _serviceProvider;
    protected readonly TenantManager _tenantManager;
    protected readonly ISecurityDao<string> _securityDao;
    protected readonly SelectorFactory _selectorFactory;
    protected readonly CrossDao _crossDao;

    protected bool IsCrossDao(string id1, string id2)
    {
        if (id2 == null || id1 == null)
        {
            return false;
        }

        return !Equals(_selectorFactory.GetSelector(id1).GetIdCode(id1), _selectorFactory.GetSelector(id2).GetIdCode(id2));
    }

    protected async Task SetSharedPropertyAsync(IEnumerable<FileEntry<string>> entries)
    {
        var pureShareRecords = await _securityDao.GetPureShareRecordsAsync(entries).ToListAsync();
        var ids = pureShareRecords
            //.Where(x => x.Owner == SecurityContext.CurrentAccount.ID)
            .Select(x => x.EntryId).Distinct();

        foreach (var id in ids)
        {
            var firstEntry = entries.FirstOrDefault(y => y.Id.Equals(id));

            if (firstEntry != null)
            {
                firstEntry.Shared = true;
            }
        }
    }

    protected internal Task<File<string>> PerformCrossDaoFileCopyAsync(string fromFileId, string toFolderId, bool deleteSourceFile)
    {
        var fromSelector = _selectorFactory.GetSelector(fromFileId);
        var toSelector = _selectorFactory.GetSelector(toFolderId);

        return _crossDao.PerformCrossDaoFileCopyAsync(
            fromFileId, fromSelector.GetFileDao(fromFileId), fromSelector.ConvertId,
            toFolderId, toSelector.GetFileDao(toFolderId), toSelector.ConvertId,
            deleteSourceFile);
    }

    protected async Task<File<int>> PerformCrossDaoFileCopyAsync(string fromFileId, int toFolderId, bool deleteSourceFile)
    {
        var fromSelector = _selectorFactory.GetSelector(fromFileId);
        await using var scope = _serviceProvider.CreateAsyncScope();
        var tenantManager = scope.ServiceProvider.GetService<TenantManager>();
        await tenantManager.SetCurrentTenantAsync(TenantID);

        return await _crossDao.PerformCrossDaoFileCopyAsync(
            fromFileId, fromSelector.GetFileDao(fromFileId), fromSelector.ConvertId,
            toFolderId, scope.ServiceProvider.GetService<IFileDao<int>>(), r => r,
            deleteSourceFile);
    }

    protected Task<Folder<string>> PerformCrossDaoFolderCopyAsync(string fromFolderId, string toRootFolderId, bool deleteSourceFolder, CancellationToken? cancellationToken)
    {
        var fromSelector = _selectorFactory.GetSelector(fromFolderId);
        var toSelector = _selectorFactory.GetSelector(toRootFolderId);

        return _crossDao.PerformCrossDaoFolderCopyAsync(
            fromFolderId, fromSelector.GetFolderDao(fromFolderId), fromSelector.GetFileDao(fromFolderId), fromSelector.ConvertId,
            toRootFolderId, toSelector.GetFolderDao(toRootFolderId), toSelector.GetFileDao(toRootFolderId), toSelector.ConvertId,
            deleteSourceFolder, cancellationToken);
    }

    protected Task<Folder<int>> PerformCrossDaoFolderCopyAsync(string fromFolderId, int toRootFolderId, bool deleteSourceFolder, CancellationToken? cancellationToken)
    {
        var fromSelector = _selectorFactory.GetSelector(fromFolderId);

        return _crossDao.PerformCrossDaoFolderCopyAsync(
            fromFolderId, fromSelector.GetFolderDao(fromFolderId), fromSelector.GetFileDao(fromFolderId), fromSelector.ConvertId,
            toRootFolderId, _serviceProvider.GetService<IFolderDao<int>>(), _serviceProvider.GetService<IFileDao<int>>(), r => r,
            deleteSourceFolder, cancellationToken);
    }
}
