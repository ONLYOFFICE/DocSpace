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

namespace ASC.Web.Files.Core.Search;

[Scope(Additional = typeof(FactoryIndexerFolderExtension))]
public class FactoryIndexerFolder : FactoryIndexer<DbFolder>
{
    private readonly IDbContextFactory<FilesDbContext> _dbContextFactory;
    private readonly Settings _settings;

    public FactoryIndexerFolder(
        ILoggerProvider options,
        TenantManager tenantManager,
        SearchSettingsHelper searchSettingsHelper,
        FactoryIndexer factoryIndexer,
        BaseIndexer<DbFolder> baseIndexer,
        IServiceProvider serviceProvider,
        IDbContextFactory<FilesDbContext> dbContextFactory,
        ICache cache,
        Settings settings)
        : base(options, tenantManager, searchSettingsHelper, factoryIndexer, baseIndexer, serviceProvider, cache)
    {
        _dbContextFactory = dbContextFactory;
        _settings = settings;
    }

    public override async Task IndexAllAsync()
    {
        (int, int, int) getCount(DateTime lastIndexed)
        {
            using var filesDbContext = _dbContextFactory.CreateDbContext();

            var minid = Queries.FolderMinId(filesDbContext, lastIndexed);

            var maxid = Queries.FolderMaxId(filesDbContext, lastIndexed);

            var count = Queries.FoldersCount(filesDbContext, lastIndexed);

            return new(count, maxid, minid);
        }

        List<DbFolder> getData(long start, long stop, DateTime lastIndexed)
        {
            using var filesDbContext = _dbContextFactory.CreateDbContext();
            return Queries.FolderData(filesDbContext, lastIndexed, start, stop).ToList();
        }

        List<int> getIds(DateTime lastIndexed)
        {
            var start = 0;
            var result = new List<int>();

            using var filesDbContext = _dbContextFactory.CreateDbContext();

            while (true)
            {
                var id = Queries.FolderId(filesDbContext, lastIndexed, start);
                if (id != 0)
                {
                    start = id;
                    result.Add(id);
                }
                else
                {
                    break;
                }
            }

            return result;
        }

        try
        {
            var j = 0;
            var tasks = new List<Task>();

            foreach (var data in await _indexer.IndexAllAsync(getCount, getIds, getData))
            {
                if (_settings.Threads == 1)
                {
                    await Index(data);
                }
                else
                {
                    tasks.Add(IndexAsync(data));
                    j++;
                    if (j >= _settings.Threads)
                    {
                        Task.WaitAll(tasks.ToArray());
                        tasks = new List<Task>();
                        j = 0;
                    }
                }
            }

            if (tasks.Count > 0)
            {
                Task.WaitAll(tasks.ToArray());
            }
        }
        catch (Exception e)
        {
            Logger.ErrorFactoryIndexerFolder(e);
            throw;
        }
    }
}

class FolderTenant
{
    public DbTenant DbTenant { get; set; }
    public DbFolder DbFolder { get; set; }
}

public static class FactoryIndexerFolderExtension
{
    public static void Register(DIHelper services)
    {
        services.TryAdd<DbFolder>();
    }
}

static file class Queries
{
    public static readonly Func<FilesDbContext, DateTime, int> FolderMinId =
        EF.CompileQuery(
            (FilesDbContext ctx, DateTime lastIndexed) =>
                ctx.Folders
                    .Where(r => r.ModifiedOn >= lastIndexed)
                    .Join(ctx.Tenants, r => r.TenantId, r => r.Id,
                        (f, t) => new FolderTenant { DbFolder = f, DbTenant = t })
                    .Where(r => r.DbTenant.Status == TenantStatus.Active)
                    .OrderBy(r => r.DbFolder.Id)
                    .Select(r => r.DbFolder.Id)
                    .FirstOrDefault());

    public static readonly Func<FilesDbContext, DateTime, int> FolderMaxId =
        EF.CompileQuery(
            (FilesDbContext ctx, DateTime lastIndexed) =>
                ctx.Folders
                    .Where(r => r.ModifiedOn >= lastIndexed)
                    .Join(ctx.Tenants, r => r.TenantId, r => r.Id,
                        (f, t) => new FolderTenant { DbFolder = f, DbTenant = t })
                    .Where(r => r.DbTenant.Status == TenantStatus.Active)
                    .OrderByDescending(r => r.DbFolder.Id)
                    .Select(r => r.DbFolder.Id)
                    .FirstOrDefault());

    public static readonly Func<FilesDbContext, DateTime, int> FoldersCount =
        EF.CompileQuery(
            (FilesDbContext ctx, DateTime lastIndexed) =>
                ctx.Folders
                    .Where(r => r.ModifiedOn >= lastIndexed)
                    .Join(ctx.Tenants, r => r.TenantId, r => r.Id,
                        (f, t) => new FolderTenant { DbFolder = f, DbTenant = t })
                    .Where(r => r.DbTenant.Status == TenantStatus.Active)
                    .Count());

    public static readonly Func<FilesDbContext, DateTime, long, int> FolderId =
        EF.CompileQuery(
            (FilesDbContext ctx, DateTime lastIndexed, long start) =>
                ctx.Folders
                    .Where(r => r.ModifiedOn >= lastIndexed)
                    .Join(ctx.Tenants, r => r.TenantId, r => r.Id,
                        (f, t) => new FolderTenant { DbFolder = f, DbTenant = t })
                    .Where(r => r.DbTenant.Status == TenantStatus.Active)
                    .Where(r => r.DbFolder.Id >= start)
                    .OrderBy(r => r.DbFolder.Id)
                    .Select(r => r.DbFolder.Id)
                    .Skip(BaseIndexer<DbFolder>.QueryLimit)
                    .FirstOrDefault());

    public static readonly Func<FilesDbContext, DateTime, long, long, IEnumerable<DbFolder>> FolderData =
        EF.CompileQuery(
            (FilesDbContext ctx, DateTime lastIndexed, long start, long stop) =>
                ctx.Folders
                    .Where(r => r.ModifiedOn >= lastIndexed)
                    .Join(ctx.Tenants, r => r.TenantId, r => r.Id,
                        (f, t) => new FolderTenant { DbFolder = f, DbTenant = t })
                    .Where(r => r.DbTenant.Status == TenantStatus.Active)
                    .Where(r => r.DbFolder.Id >= start && r.DbFolder.Id <= stop)
                    .Select(r => r.DbFolder));
}