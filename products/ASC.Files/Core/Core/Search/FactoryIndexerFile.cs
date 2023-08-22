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

[Scope]
public class BaseIndexerFile : BaseIndexer<DbFile>
{
    private readonly IDaoFactory _daoFactory;

    public BaseIndexerFile(
        Client client,
        ILogger<BaseIndexerFile> log,
        IDbContextFactory<WebstudioDbContext> dbContextManager,
        TenantManager tenantManager,
        BaseIndexerHelper baseIndexerHelper,
        Settings settings,
        IServiceProvider serviceProvider,
        IDaoFactory daoFactory)
        : base(client, log, dbContextManager, tenantManager, baseIndexerHelper, settings, serviceProvider)
    {
        _daoFactory = daoFactory;
    }

    protected override async Task<bool> BeforeIndex(DbFile data)
    {
        if (!(await base.BeforeIndex(data)))
        {
            return false;
        }

        var fileDao = _daoFactory.GetFileDao<int>() as FileDao;
        await _tenantManager.SetCurrentTenantAsync(data.TenantId);
        await fileDao.InitDocumentAsync(data);

        return true;
    }

    protected override async Task<bool> BeforeIndexAsync(DbFile data)
    {
        if (!(await base.BeforeIndex(data)))
        {
            return false;
        }

        var fileDao = _daoFactory.GetFileDao<int>() as FileDao;
        await _tenantManager.SetCurrentTenantAsync(data.TenantId);
        await fileDao.InitDocumentAsync(data);

        return true;
    }
}


[Scope(Additional = typeof(FactoryIndexerFileExtension))]
public class FactoryIndexerFile : FactoryIndexer<DbFile>
{
    private readonly IDbContextFactory<FilesDbContext> _dbContextFactory;
    private readonly Settings _settings;

    public FactoryIndexerFile(
        ILoggerProvider options,
        TenantManager tenantManager,
        SearchSettingsHelper searchSettingsHelper,
        FactoryIndexer factoryIndexer,
        BaseIndexerFile baseIndexer,
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

            var minid = Queries.FileMinId(filesDbContext, lastIndexed);

            var maxid = Queries.FileMaxId(filesDbContext, lastIndexed);

            var count = Queries.FilesCount(filesDbContext, lastIndexed);

            return new(count, maxid, minid);
        }

        List<DbFile> getData(long start, long stop, DateTime lastIndexed)
        {
            using var filesDbContext = _dbContextFactory.CreateDbContext();
            return Queries.FilesFoldersPair(filesDbContext, lastIndexed, start, stop)
                .Select(r =>
                {
                    var result = r.File;
                    result.Folders = r.Folders;
                    return result;
                })
                .ToList();

        }

        List<int> getIds(DateTime lastIndexed)
        {
            var start = 0;
            var result = new List<int>();

            using var filesDbContext = _dbContextFactory.CreateDbContext();

            while (true)
            {
                var id = Queries.FileId(filesDbContext, lastIndexed, start);
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
            Logger.ErrorFactoryIndexerFile(e);
            throw;
        }
    }

    public override string SettingsTitle => FilesCommonResource.IndexTitle;
}

public class FileTenant
{
    public DbTenant DbTenant { get; set; }
    public DbFile DbFile { get; set; }
}

public static class FactoryIndexerFileExtension
{
    public static void Register(DIHelper services)
    {
        services.TryAdd<DbFile>();
    }
}

file class FilesFoldersPair
{
    public DbFile File { get; set; }
    public List<DbFolderTree> Folders { get; set; }
}

static file class Queries
{
    public static readonly Func<FilesDbContext, DateTime, int> FileMinId = EF.CompileQuery(
        (FilesDbContext ctx, DateTime lastIndexed) =>
            ctx.Files
                .Where(r => r.ModifiedOn >= lastIndexed)
                .Join(ctx.Tenants, r => r.TenantId, r => r.Id, (f, t) => new FileTenant { DbFile = f, DbTenant = t })
                .Where(r => r.DbTenant.Status == TenantStatus.Active)
                .Select(r => r.DbFile)
                .Where(r => r.Version == 1)
                .OrderBy(r => r.Id)
                .Select(r => r.Id)
                .FirstOrDefault());

    public static readonly Func<FilesDbContext, DateTime, int> FileMaxId = EF.CompileQuery(
        (FilesDbContext ctx, DateTime lastIndexed) =>
            ctx.Files
                .Where(r => r.ModifiedOn >= lastIndexed)
                .Join(ctx.Tenants, r => r.TenantId, r => r.Id, (f, t) => new FileTenant { DbFile = f, DbTenant = t })
                .Where(r => r.DbTenant.Status == TenantStatus.Active)
                .Select(r => r.DbFile)
                .Where(r => r.Version == 1)
                .OrderByDescending(r => r.Id)
                .Select(r => r.Id)
                .FirstOrDefault());

    public static readonly Func<FilesDbContext, DateTime, int> FilesCount = EF.CompileQuery(
        (FilesDbContext ctx, DateTime lastIndexed) =>
            ctx.Files
                .Where(r => r.ModifiedOn >= lastIndexed)
                .Join(ctx.Tenants, r => r.TenantId, r => r.Id, (f, t) => new FileTenant { DbFile = f, DbTenant = t })
                .Where(r => r.DbTenant.Status == TenantStatus.Active)
                .Select(r => r.DbFile)
                .Where(r => r.Version == 1)
                .Count());

    public static readonly Func<FilesDbContext, DateTime, long, long, IEnumerable<FilesFoldersPair>> FilesFoldersPair =
        EF.CompileQuery(
            (FilesDbContext ctx, DateTime lastIndexed, long start, long stop) =>
                ctx.Files
                    .Where(r => r.ModifiedOn >= lastIndexed)
                    .Join(ctx.Tenants, r => r.TenantId, r => r.Id,
                        (f, t) => new FileTenant { DbFile = f, DbTenant = t })
                    .Where(r => r.DbTenant.Status == TenantStatus.Active)
                    .Select(r => r.DbFile)
                    .Where(r => r.Id >= start && r.Id <= stop && r.CurrentVersion)
                    .Select(file => new FilesFoldersPair
                    {
                        File = file, Folders = ctx.Tree.Where(b => b.FolderId == file.ParentId).ToList()
                    }));
    
    public static readonly Func<FilesDbContext, DateTime, long, int> FileId = EF.CompileQuery(
    (FilesDbContext ctx, DateTime lastIndexed, long start) =>
        ctx.Files
            .Where(r => r.ModifiedOn >= lastIndexed)
            .Join(ctx.Tenants, r => r.TenantId, r => r.Id, (f, t) => new FileTenant { DbFile = f, DbTenant = t })
            .Where(r => r.DbTenant.Status == TenantStatus.Active)
            .Select(r => r.DbFile)
            .Where(r => r.Id >= start)
            .Where(r => r.Version == 1)
            .OrderBy(r => r.Id)
            .Select(r => r.Id)
            .Skip(BaseIndexer<DbFile>.QueryLimit)
            .FirstOrDefault());
}