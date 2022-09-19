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

namespace ASC.Files.Core.Data;

public class AbstractDao
{
    protected readonly ICache _cache;
    private int _tenantID;
    protected internal int TenantID
    {
        get
        {
            if (_tenantID == 0)
            {
                _tenantID = _tenantManager.GetCurrentTenant().Id;
            }

            return _tenantID;
        }
    }

    protected readonly IDbContextFactory<FilesDbContext> _dbContextFactory;
    protected readonly UserManager _userManager;
    protected readonly TenantManager _tenantManager;
    protected readonly TenantUtil _tenantUtil;
    protected readonly SetupInfo _setupInfo;
    protected readonly MaxTotalSizeStatistic _maxTotalSizeStatistic;
    protected readonly CoreBaseSettings _coreBaseSettings;
    protected readonly CoreConfiguration _coreConfiguration;
    protected readonly SettingsManager _settingsManager;
    protected readonly AuthContext _authContext;
    protected readonly IServiceProvider _serviceProvider;

    protected AbstractDao(
        IDbContextFactory<FilesDbContext> dbContextFactory,
        UserManager userManager,
        TenantManager tenantManager,
        TenantUtil tenantUtil,
        SetupInfo setupInfo,
        MaxTotalSizeStatistic maxTotalSizeStatistic,
        CoreBaseSettings coreBaseSettings,
        CoreConfiguration coreConfiguration,
        SettingsManager settingsManager,
        AuthContext authContext,
        IServiceProvider serviceProvider,
        ICache cache)
    {
        _cache = cache;
        _dbContextFactory = dbContextFactory;
        _userManager = userManager;
        _tenantManager = tenantManager;
        _tenantUtil = tenantUtil;
        _setupInfo = setupInfo;
        _maxTotalSizeStatistic = maxTotalSizeStatistic;
        _coreBaseSettings = coreBaseSettings;
        _coreConfiguration = coreConfiguration;
        _settingsManager = settingsManager;
        _authContext = authContext;
        _serviceProvider = serviceProvider;
    }


    protected IQueryable<T> Query<T>(DbSet<T> set) where T : class, IDbFile
    {
        var tenantId = TenantID;

        return set.Where(r => r.TenantId == tenantId);
    }

    protected internal IQueryable<DbFile> GetFileQuery(FilesDbContext filesDbContext, Expression<Func<DbFile, bool>> where)
    {
        return Query(filesDbContext.Files)
            .Where(where);
    }

    protected async Task GetRecalculateFilesCountUpdateAsync(int folderId)
    {
        using var filesDbContext = _dbContextFactory.CreateDbContext();
        var folders = await filesDbContext.Folders
            .Where(r => r.TenantId == TenantID)
            .Where(r => filesDbContext.Tree.Where(r => r.FolderId == folderId).Any(a => a.ParentId == r.Id))
            .ToListAsync();

        foreach (var f in folders)
        {
            f.FilesCount = await
                filesDbContext.Files
                .Join(filesDbContext.Tree, a => a.ParentId, b => b.FolderId, (file, tree) => new { file, tree })
                .Where(r => r.file.TenantId == f.TenantId)
                .Where(r => r.tree.ParentId == f.Id)
                .Select(r => r.file.Id)
                .Distinct()
                .CountAsync();
        }

        await filesDbContext.SaveChangesAsync();
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
            using var filesDbContext = _dbContextFactory.CreateDbContext();
            result = await Query(filesDbContext.ThirdpartyIdMapping)
                .AsNoTracking()
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

            using var filesDbContext = _dbContextFactory.CreateDbContext();
            await filesDbContext.AddOrUpdateAsync(r => r.ThirdpartyIdMapping, newItem);
            await filesDbContext.SaveChangesAsync();
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
