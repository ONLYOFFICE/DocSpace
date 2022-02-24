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

namespace ASC.Files.Core.Data;

public class AbstractDao
{
    protected readonly ICache Cache;

    private readonly Lazy<FilesDbContext> LazyFilesDbContext;
    public FilesDbContext FilesDbContext => LazyFilesDbContext.Value;
    private int _tenantID;
    protected internal int TenantID
    {
        get
        {
            if (_tenantID == 0)
            {
                _tenantID = TenantManager.GetCurrentTenant().TenantId;
            }

            return _tenantID;
        }
    }
    protected readonly UserManager UserManager;
    protected readonly TenantManager TenantManager;
    protected readonly TenantUtil TenantUtil;
    protected readonly SetupInfo SetupInfo;
    protected readonly TenantExtra TenantExtra;
    protected readonly TenantStatisticsProvider TenantStatisticProvider;
    protected readonly CoreBaseSettings CoreBaseSettings;
    protected readonly CoreConfiguration CoreConfiguration;
    protected readonly SettingsManager SettingsManager;
    protected readonly AuthContext AuthContext;
    protected readonly IServiceProvider ServiceProvider;

    protected AbstractDao(
        DbContextManager<FilesDbContext> dbContextManager,
        UserManager userManager,
        TenantManager tenantManager,
        TenantUtil tenantUtil,
        SetupInfo setupInfo,
        TenantExtra tenantExtra,
        TenantStatisticsProvider tenantStatisticProvider,
        CoreBaseSettings coreBaseSettings,
        CoreConfiguration coreConfiguration,
        SettingsManager settingsManager,
        AuthContext authContext,
        IServiceProvider serviceProvider,
        ICache cache)
    {
        Cache = cache;
        LazyFilesDbContext = new Lazy<FilesDbContext>(() => dbContextManager.Get(FileConstant.DatabaseId));
        UserManager = userManager;
        TenantManager = tenantManager;
        TenantUtil = tenantUtil;
        SetupInfo = setupInfo;
        TenantExtra = tenantExtra;
        TenantStatisticProvider = tenantStatisticProvider;
        CoreBaseSettings = coreBaseSettings;
        CoreConfiguration = coreConfiguration;
        SettingsManager = settingsManager;
        AuthContext = authContext;
        ServiceProvider = serviceProvider;
    }


    protected IQueryable<T> Query<T>(DbSet<T> set) where T : class, IDbFile
    {
        var tenantId = TenantID;

        return set.AsQueryable().Where(r => r.TenantId == tenantId);
    }

    protected internal IQueryable<DbFile> GetFileQuery(Expression<Func<DbFile, bool>> where)
    {
        return Query(FilesDbContext.Files)
            .Where(where);
    }

    protected async Task GetRecalculateFilesCountUpdateAsync(int folderId)
    {
        var folders = await FilesDbContext.Folders
            .AsQueryable()
            .Where(r => r.TenantId == TenantID)
            .Where(r => FilesDbContext.Tree.AsQueryable().Where(r => r.FolderId == folderId).Select(r => r.ParentId).Any(a => a == r.Id))
            .ToListAsync();

        foreach (var f in folders)
        {
            f.FilesCount = await
                FilesDbContext.Files
                .AsQueryable()
                .Join(FilesDbContext.Tree, a => a.FolderId, b => b.FolderId, (file, tree) => new { file, tree })
                .Where(r => r.file.TenantId == f.TenantId)
                .Where(r => r.tree.ParentId == f.Id)
                .Select(r => r.file.Id)
                .Distinct()
                .CountAsync();
        }

        await FilesDbContext.SaveChangesAsync();
    }

    protected ValueTask<object> MappingIDAsync(object id, bool saveIfNotExist = false)
    {
        if (id == null)
        {
            return ValueTask.FromResult<object>(null);
        }

        var isNumeric = int.TryParse(id.ToString(), out var n);

        if (isNumeric)
        {
            return ValueTask.FromResult<object>(n);
        }

        return InternalMappingIDAsync(id, saveIfNotExist);
    }

    private async ValueTask<object> InternalMappingIDAsync(object id, bool saveIfNotExist = false)
    {
        object result;

        if (id.ToString().StartsWith("sbox")
            || id.ToString().StartsWith("box")
            || id.ToString().StartsWith("dropbox")
            || id.ToString().StartsWith("spoint")
            || id.ToString().StartsWith("drive")
            || id.ToString().StartsWith("onedrive"))
        {
            result = Regex.Replace(BitConverter.ToString(Hasher.Hash(id.ToString(), HashAlg.MD5)), "-", "").ToLower();
        }
        else
        {
            result = await Query(FilesDbContext.ThirdpartyIdMapping)
                .Where(r => r.HashId == id.ToString())
                .Select(r => r.Id)
                .FirstOrDefaultAsync();
        }

        if (saveIfNotExist)
        {
            var newItem = new DbFilesThirdpartyIdMapping
            {
                Id = id.ToString(),
                HashId = result.ToString(),
                TenantId = TenantID
            };

            await FilesDbContext.AddOrUpdateAsync(r => r.ThirdpartyIdMapping, newItem);
        }

        return result;
    }

    protected ValueTask<object> MappingIDAsync(object id)
    {
        return MappingIDAsync(id, false);
    }

    internal static IQueryable<T> BuildSearch<T>(IQueryable<T> query, string text, SearhTypeEnum searhTypeEnum) where T : IDbSearch
    {
        var lowerText = GetSearchText(text);

        return searhTypeEnum switch
        {
            SearhTypeEnum.Start => query.Where(r => r.Title.ToLower().StartsWith(lowerText)),
            SearhTypeEnum.End => query.Where(r => r.Title.ToLower().EndsWith(lowerText)),
            SearhTypeEnum.Any => query.Where(r => r.Title.ToLower().Contains(lowerText)),
            _ => query,
        };
    }

    internal static string GetSearchText(string text) => (text ?? "").ToLower().Trim().Replace("%", "\\%").Replace("_", "\\_");

    internal enum SearhTypeEnum
    {
        Start,
        End,
        Any
    }
}
