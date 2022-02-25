/*
 *
 * (c) Copyright Ascensio System Limited 2010-2021
 * 
 * Licensed under the Apache License, Version 2.0 (the "License");
 * you may not use this file except in compliance with the License.
 * You may obtain a copy of the License at
 * http://www.apache.org/licenses/LICENSE-2.0
 * Unless required by applicable law or agreed to in writing, software
 * distributed under the License is distributed on an "AS IS" BASIS,
 * WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 * See the License for the specific language governing permissions and
 * limitations under the License.
 *
*/

namespace ASC.Files.ThumbnailBuilder;

[Scope]
internal class FileDataProvider
{
    private FilesDbContext filesDbContext => _lazyFilesDbContext.Value;

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
            filesDbContext.Tariffs
            .Join(filesDbContext.Quotas.AsQueryable().DefaultIfEmpty(), a => a.Tariff, b => b.Tenant, (tariff, quota) => new { tariff, quota })
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
        var search = filesDbContext.Files
            .AsQueryable()
            .Where(r => r.CurrentVersion && r.Thumb == Thumbnail.Waiting && !r.Encrypted)
            .OrderByDescending(r => r.ModifiedOn)
            .Take(_thumbnailSettings.SqlMaxResults);

        if (where != null)
        {
            search = search.Where(where);
        }

        return search
            .Join(filesDbContext.Tenants, r => r.TenantId, r => r.Id, (f, t) => new FileTenant { DbFile = f, DbTenant = t })
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
