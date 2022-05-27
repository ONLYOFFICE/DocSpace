﻿// (c) Copyright Ascensio System SIA 2010-2022
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

namespace ASC.Files.ThumbnailBuilder;

[Scope]
internal class FileDataProvider
{
    private FilesDbContext FilesDbContext => _lazyFilesDbContext.Value;

    private readonly ThumbnailSettings _thumbnailSettings;
    private readonly ICache _cache;
    private readonly Lazy<FilesDbContext> _lazyFilesDbContext;
    private readonly string _cacheKey;

    public FileDataProvider(
        ThumbnailSettings settings,
        ICache ascCache,
        DbContextManager<FilesDbContext> dbContextManager)
    {
        _thumbnailSettings = settings;
        _cache = ascCache;
        _lazyFilesDbContext = new Lazy<FilesDbContext>(() => dbContextManager.Get(_thumbnailSettings.ConnectionStringName));
        _cacheKey = "PremiumTenants";
    }

    public int[] GetPremiumTenants()
    {
        var result = _cache.Get<int[]>(_cacheKey);

        if (result != null)
        {
            return result;
        }

        /*
        // v_premium_tenants view:
        select
            t.tenant,
            (now() < max(t.stamp)) AS premium

        from tenants_tariff t
        left join tenants_quota q ON (t.tariff = q.tenant)

        where
        (
            (
                isnull(t.comment) or
                (
                    (not((t.comment like '%non-profit%'))) and
                    (not((t.comment like '%test%'))) and
                    (not((t.comment like '%translate%'))) and
                    (not((t.comment like '%trial%')))
                )
            ) and
            (not((q.features like '%free%'))) and
            (not((q.features like '%non-profit%'))) and
            (not((q.features like '%trial%')))
        )
        group by t.tenant
        */

        var search =
            FilesDbContext.Tariffs
            .Join(FilesDbContext.Quotas.AsQueryable().DefaultIfEmpty(), a => a.Tariff, b => b.Tenant, (tariff, quota) => new { tariff, quota })
            .Where(r =>
                    (
                        r.tariff.Comment == null ||
                        (
                         !r.tariff.Comment.Contains("non-profit") &&
                         !r.tariff.Comment.Contains("test") &&
                         !r.tariff.Comment.Contains("translate") &&
                         !r.tariff.Comment.Contains("trial")
                        )
                    ) &&

                        !r.quota.Features.Contains("free") &&
                        !r.quota.Features.Contains("non-profit") &&
                        !r.quota.Features.Contains("trial")

                    )
            .GroupBy(r => r.tariff.Tenant)
            .Select(r => new { tenant = r.Key, stamp = r.Max(b => b.tariff.Stamp) })
            .Where(r => r.stamp > DateTime.UtcNow);

        result = search.Select(r => r.tenant).ToArray();

        _cache.Insert(_cacheKey, result, DateTime.UtcNow.AddHours(1));

        return result;
    }

    private IEnumerable<FileData<int>> GetFileData(Expression<Func<DbFile, bool>> where)
    {
        var search = FilesDbContext.Files
            .AsQueryable()
            .Where(r => r.CurrentVersion && r.ThumbnailStatus == ASC.Files.Core.Thumbnail.Waiting && !r.Encrypted)
            .OrderByDescending(r => r.ModifiedOn)
            .Take(_thumbnailSettings.SqlMaxResults);

        if (where != null)
        {
            search = search.Where(where);
        }

        return search
            .Join(FilesDbContext.Tenants, r => r.TenantId, r => r.Id, (f, t) => new FileTenant { DbFile = f, DbTenant = t })
            .Where(r => r.DbTenant.Status == TenantStatus.Active)
            .Select(r => new FileData<int>(r.DbFile.TenantId, r.DbFile.Id, ""))
            .ToList();
    }

    public IEnumerable<FileData<int>> GetFilesWithoutThumbnails()
    {
        IEnumerable<FileData<int>> result;

        var premiumTenants = GetPremiumTenants();

        if (premiumTenants.Length > 0)
        {
            result = GetFileData(r => premiumTenants.Contains(r.TenantId));

            if (result.Any())
            {
                return result;
            }
        }

        result = GetFileData(null);

        return result;
    }
}
