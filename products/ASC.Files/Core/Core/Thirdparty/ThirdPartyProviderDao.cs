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

namespace ASC.Files.Thirdparty;

internal abstract class ThirdPartyProviderDao<T> : IDisposable where T : class, IProviderInfo
{
    public int TenantID { get; private set; }
    protected readonly IServiceProvider _serviceProvider;
    protected readonly UserManager _userManager;
    protected readonly TenantUtil _tenantUtil;
    protected readonly IDbContextFactory<FilesDbContext> _dbContextFactory;
    protected readonly SetupInfo _setupInfo;
    protected readonly ILogger _logger;
    protected readonly FileUtility _fileUtility;
    protected readonly TempPath _tempPath;
    protected readonly AuthContext _authContext;
    protected RegexDaoSelectorBase<T> DaoSelector { get; set; }
    protected T ProviderInfo { get; set; }
    protected string PathPrefix { get; private set; }

    protected abstract string Id { get; }

    protected ThirdPartyProviderDao(
        IServiceProvider serviceProvider,
        UserManager userManager,
        TenantManager tenantManager,
        TenantUtil tenantUtil,
        IDbContextFactory<FilesDbContext> dbContextFactory,
        SetupInfo setupInfo,
        ILogger logger,
        FileUtility fileUtility,
        TempPath tempPath,
        AuthContext authContext)
    {
        _serviceProvider = serviceProvider;
        _userManager = userManager;
        _tenantUtil = tenantUtil;
        _dbContextFactory = dbContextFactory;
        _setupInfo = setupInfo;
        _logger = logger;
        _fileUtility = fileUtility;
        _tempPath = tempPath;
        TenantID = tenantManager.GetCurrentTenant().Id;
        _authContext = authContext;
    }

    public void Init(BaseProviderInfo<T> providerInfo, RegexDaoSelectorBase<T> selectorBase)
    {
        ProviderInfo = providerInfo.ProviderInfo;
        PathPrefix = providerInfo.PathPrefix;
        DaoSelector = selectorBase;
    }

    public IQueryable<TSet> Query<TSet>(DbSet<TSet> set) where TSet : class, IDbFile
    {
        return set.Where(r => r.TenantId == TenantID);
    }

    protected Task<string> MappingIDAsync(string id, bool saveIfNotExist = false)
    {
        if (id == null)
        {
            return null;
        }

        return InternalMappingIDAsync(id, saveIfNotExist);
    }

    private async Task<string> InternalMappingIDAsync(string id, bool saveIfNotExist = false)
    {
        using var filesDbContext = _dbContextFactory.CreateDbContext();

        string result;
        if (id.StartsWith(Id))
        {
            result = Regex.Replace(BitConverter.ToString(Hasher.Hash(id, HashAlg.MD5)), "-", "").ToLower();
        }
        else
        {
            result = await filesDbContext.ThirdpartyIdMapping
                    .Where(r => r.HashId == id)
                    .Select(r => r.Id)
                    .FirstOrDefaultAsync();
        }
        if (saveIfNotExist)
        {
            var newMapping = new DbFilesThirdpartyIdMapping
            {
                Id = id,
                HashId = result,
                TenantId = TenantID
            };

            await filesDbContext.ThirdpartyIdMapping.AddAsync(newMapping);
            await filesDbContext.SaveChangesAsync();
        }

        return result;
    }

    protected Folder<string> GetFolder()
    {
        var folder = _serviceProvider.GetService<Folder<string>>();

        InitFileEntry(folder);

        folder.FolderType = FolderType.DEFAULT;
        folder.Shareable = false;
        folder.FilesCount = 0;
        folder.FoldersCount = 0;

        return folder;
    }

    protected Folder<string> GetErrorFolder(ErrorEntry entry)
    {
        var folder = GetFolder();

        InitFileEntryError(folder, entry);

        folder.ParentId = null;

        return folder;
    }

    protected File<string> GetFile()
    {
        var file = _serviceProvider.GetService<File<string>>();

        InitFileEntry(file);

        file.Access = FileShare.None;
        file.Shared = false;
        file.Version = 1;

        return file;
    }

    protected File<string> GetErrorFile(ErrorEntry entry)
    {
        var file = GetFile();
        InitFileEntryError(file, entry);

        return file;
    }

    protected void InitFileEntry(FileEntry<string> fileEntry)
    {
        fileEntry.CreateBy = ProviderInfo.Owner;
        fileEntry.ModifiedBy = ProviderInfo.Owner;
        fileEntry.ProviderId = ProviderInfo.ProviderId;
        fileEntry.ProviderKey = ProviderInfo.ProviderKey;
        fileEntry.RootCreateBy = ProviderInfo.Owner;
        fileEntry.RootFolderType = ProviderInfo.RootFolderType;
        fileEntry.RootId = MakeId();
    }

    protected void InitFileEntryError(FileEntry<string> fileEntry, ErrorEntry entry)
    {
        fileEntry.Id = MakeId(entry.ErrorId);
        fileEntry.CreateOn = _tenantUtil.DateTimeNow();
        fileEntry.ModifiedOn = _tenantUtil.DateTimeNow();
        fileEntry.Error = entry.Error;
    }

    protected void SetFolderType(Folder<string> folder, bool isRoot)
    {
        if (isRoot && (ProviderInfo.RootFolderType == FolderType.VirtualRooms ||
            ProviderInfo.RootFolderType == FolderType.Archive))
        {
            folder.FolderType = ProviderInfo.RootFolderType;
        }
        else if (ProviderInfo.FolderId == folder.Id)
        {
            folder.FolderType = ProviderInfo.FolderType;
        }
    }

    public bool CheckInvalidFilter(FilterType filterType)
    {
        return filterType is
            FilterType.FilesOnly or
            FilterType.ByExtension or
            FilterType.DocumentsOnly or
            FilterType.OFormOnly or
            FilterType.OFormTemplateOnly or
            FilterType.ImagesOnly or
            FilterType.PresentationsOnly or
            FilterType.SpreadsheetsOnly or
            FilterType.ArchiveOnly or
            FilterType.MediaOnly;
    }

    public abstract string MakeId(string path = null);

    public async IAsyncEnumerable<Tag> GetNewTagsAsync(Guid subject, Folder<string> parentFolder, bool deepSearch)
    {
        var folderId = DaoSelector.ConvertId(parentFolder.Id);

        var filesDbContext = _dbContextFactory.CreateDbContext();
        var entryIDs = await filesDbContext.ThirdpartyIdMapping
                   .Where(r => r.Id.StartsWith(PathPrefix))
                   .Select(r => r.HashId)
                   .ToListAsync();

        if (!entryIDs.Any())
        {
            yield break;
        }

        var q = from r in filesDbContext.Tag
                from l in filesDbContext.TagLink.Where(a => a.TenantId == r.TenantId && a.TagId == r.Id).DefaultIfEmpty()
                where r.TenantId == TenantID && l.TenantId == TenantID && r.Type == TagType.New && entryIDs.Contains(l.EntryId)
                select new { tag = r, tagLink = l };

        if (subject != Guid.Empty)
        {
            q = q.Where(r => r.tag.Owner == subject);
        }

        var qList = await q
            .Distinct()
            .AsAsyncEnumerable()
            .ToListAsync();

        var tags = new List<Tag>();

        foreach (var r in qList)
        {
            tags.Add(new Tag
            {
                Name = r.tag.Name,
                Type = r.tag.Type,
                Owner = r.tag.Owner,
                EntryId = await MappingIDAsync(r.tagLink.EntryId),
                EntryType = r.tagLink.EntryType,
                Count = r.tagLink.Count,
                Id = r.tag.Id
            });
        }


        if (deepSearch)
        {
            foreach (var e in tags)
            {
                yield return e;
            }
            yield break;
        }

        var folderFileIds = new[] { parentFolder.Id }
            .Concat(await GetChildrenAsync(folderId));

        foreach (var e in tags.Where(tag => folderFileIds.Contains(tag.EntryId.ToString())))
        {
            yield return e;
        }
    }

    public abstract Task<IEnumerable<string>> GetChildrenAsync(string folderId);

    public void Dispose()
    {
        if (ProviderInfo != null)
        {
            ProviderInfo.Dispose();
            ProviderInfo = null;
        }
    }
}

internal class ErrorEntry
{
    public string Error { get; set; }
    public string ErrorId { get; set; }

    public ErrorEntry(string error, string errorId)
    {
        Error = error;
        ErrorId = errorId;
    }
}

public class TagLink
{
    public int TenantId { get; set; }
    public int Id { get; set; }
}

public class TagLinkComparer : IEqualityComparer<TagLink>
{
    public bool Equals([AllowNull] TagLink x, [AllowNull] TagLink y)
    {
        return x.Id == y.Id && x.TenantId == y.TenantId;
    }

    public int GetHashCode([DisallowNull] TagLink obj)
    {
        return obj.Id.GetHashCode() + obj.TenantId.GetHashCode();
    }
}