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

namespace ASC.Files.Thirdparty.Sharpbox;

internal abstract class SharpBoxDaoBase : ThirdPartyProviderDao<ICloudFileSystemEntry, ICloudDirectoryEntry, ICloudFileSystemEntry>
{
    internal SharpBoxProviderInfo SharpBoxProviderInfo { get; private set; }
    protected SharpBoxDaoBase(
        IServiceProvider serviceProvider,
        UserManager userManager,
        TenantManager tenantManager,
        TenantUtil tenantUtil,
        IDbContextFactory<FilesDbContext> dbContextManager,
        SetupInfo setupInfo,
        ILogger<SharpBoxDaoBase> monitor,
        FileUtility fileUtility,
        TempPath tempPath,
        AuthContext authContext,
        RegexDaoSelectorBase<ICloudFileSystemEntry, ICloudDirectoryEntry, ICloudFileSystemEntry> regexDaoSelectorBase)
        : base(serviceProvider, userManager, tenantManager, tenantUtil, dbContextManager, setupInfo, fileUtility, tempPath, authContext, regexDaoSelectorBase)
    {
        _logger = monitor;
    }

    public void Init(string pathPrefix, IProviderInfo<ICloudFileSystemEntry, ICloudDirectoryEntry, ICloudFileSystemEntry> providerInfo)
    {
        PathPrefix = pathPrefix;
        ProviderInfo = providerInfo;
        SharpBoxProviderInfo = providerInfo as SharpBoxProviderInfo;
    }

    protected class ErrorEntry : ICloudDirectoryEntry
    {
        public ErrorEntry(Exception e, object id)
        {
            if (e != null)
            {
                Error = e.Message;
            }

            Id = string.IsNullOrEmpty((id ?? "").ToString()) ? "/" : (id ?? "").ToString();
        }

        public string Error { get; set; }
        public string Name => "/";
        public string Id { get; private set; }
        public long Length => 0;

        public DateTime Modified => DateTime.UtcNow;

        public string ParentID
        {
            get => string.Empty;
            set { }
        }

        public ICloudDirectoryEntry Parent
        {
            get => null;
            set { }
        }

        public ICloudFileDataTransfer GetDataTransferAccessor()
        {
            return null;
        }

        public string GetPropertyValue(string key)
        {
            return null;
        }

        private readonly List<ICloudFileSystemEntry> _entries = new List<ICloudFileSystemEntry>(0);

        public IEnumerator<ICloudFileSystemEntry> GetEnumerator()
        {
            return _entries.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public ICloudFileSystemEntry GetChild(string name)
        {
            return null;
        }

        public ICloudFileSystemEntry GetChild(string name, bool bThrowException)
        {
            if (bThrowException)
            {
                throw new ArgumentNullException(name);
            }

            return null;
        }

        public ICloudFileSystemEntry GetChild(string idOrName, bool bThrowException, bool firstByNameIfNotFound)
        {
            if (bThrowException)
            {
                throw new ArgumentNullException(idOrName);
            }

            return null;
        }

        public ICloudFileSystemEntry GetChild(int idx)
        {
            return null;
        }

        public int Count => 0;

        public nChildState HasChildrens => nChildState.HasNoChilds;
    }

    protected async Task UpdatePathInDBAsync(string oldValue, string newValue)
    {
        if (oldValue.Equals(newValue))
        {
            return;
        }

        await using var filesDbContext = _dbContextFactory.CreateDbContext();
        var strategy = filesDbContext.Database.CreateExecutionStrategy();

        await strategy.ExecuteAsync(async () =>
        {
            await using var filesDbContext = _dbContextFactory.CreateDbContext();
            await using var tx = await filesDbContext.Database.BeginTransactionAsync();
            var oldIds = Queries.IdsAsync(filesDbContext, _tenantId, oldValue);

            await foreach (var oldId in oldIds)
            {
                var oldHashId = await MappingIDAsync(oldId);
                var newId = oldId.Replace(oldValue, newValue);
                var newHashId = await MappingIDAsync(newId);

                var mappingForDelete = await Queries.ThirdpartyIdMappingsAsync(filesDbContext, _tenantId, oldHashId).ToListAsync();

                var mappingForInsert = mappingForDelete.Select(m => new DbFilesThirdpartyIdMapping
                {
                    TenantId = m.TenantId,
                    Id = newId,
                    HashId = newHashId
                });

                filesDbContext.RemoveRange(mappingForDelete);
                await filesDbContext.AddRangeAsync(mappingForInsert);

                var securityForDelete = await Queries.DbFilesSecuritiesAsync(filesDbContext, _tenantId, oldHashId).ToListAsync();

                var securityForInsert = securityForDelete.Select(s => new DbFilesSecurity
                {
                    TenantId = s.TenantId,
                    TimeStamp = DateTime.Now,
                    EntryId = newHashId,
                    Share = s.Share,
                    Subject = s.Subject,
                    EntryType = s.EntryType,
                    Owner = s.Owner
                });

                filesDbContext.RemoveRange(securityForDelete);
                await filesDbContext.AddRangeAsync(securityForInsert);

                var linkForDelete = await Queries.DbFilesTagLinksAsync(filesDbContext, _tenantId, oldHashId).ToListAsync();

                var linkForInsert = linkForDelete.Select(l => new DbFilesTagLink
                {
                    EntryId = newHashId,
                    Count = l.Count,
                    CreateBy = l.CreateBy,
                    CreateOn = l.CreateOn,
                    EntryType = l.EntryType,
                    TagId = l.TagId,
                    TenantId = l.TenantId
                });

                filesDbContext.RemoveRange(linkForDelete);
                await filesDbContext.AddRangeAsync(linkForInsert);


                var filesSourceForDelete = await Queries.FilesLinksBySourceIdAsync(filesDbContext, _tenantId, oldHashId).ToListAsync();

                var filesSourceForInsert = filesSourceForDelete.Select(l => new DbFilesLink
                {
                    TenantId = l.TenantId,
                    SourceId = newHashId,
                    LinkedId = l.LinkedId,
                    LinkedFor = l.LinkedFor,
                });

                filesDbContext.RemoveRange(filesSourceForDelete);
                await filesDbContext.AddRangeAsync(filesSourceForInsert);

                var filesLinkedForDelete = await Queries.FilesLinksByLinkedIdAsync(filesDbContext, _tenantId, oldHashId).ToListAsync();

                var filesLinkedForInsert = filesLinkedForDelete.Select(l => new DbFilesLink
                {
                    TenantId = l.TenantId,
                    SourceId = l.SourceId,
                    LinkedId = newHashId,
                    LinkedFor = l.LinkedFor,
                });

                filesDbContext.RemoveRange(filesLinkedForDelete);
                await filesDbContext.AddRangeAsync(filesLinkedForInsert);

                await filesDbContext.SaveChangesAsync();
            }

            await tx.CommitAsync();
        });
    }

    protected string MakePath(object entryId)
    {
        var id = Convert.ToString(entryId, CultureInfo.InvariantCulture);

        if (!string.IsNullOrEmpty(id))
        {
            id = Encoding.UTF8.GetString(WebEncoders.Base64UrlDecode(id)).Trim('/');
        }

        return $"/{id}";
    }

    public override string MakeId(string path = null)
    {
        return path;
    }

    protected string MakeId(ICloudFileSystemEntry entry)
    {
        var path = string.Empty;
        if (entry != null && entry is not ErrorEntry)
        {
            try
            {
                path = SharpBoxProviderInfo.Storage.GetFileSystemObjectPath(entry);
            }
            catch (Exception ex)
            {
                _logger.ErrorSharpboxMakeId(ex);
            }
        }
        else if (entry != null)
        {
            path = entry.Id;
        }
        var p = string.IsNullOrEmpty(path) || path == "/" ? "" : ("-" + WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(path)));

        return $"{PathPrefix}{p}";
    }

    protected string MakeTitle(ICloudFileSystemEntry fsEntry)
    {
        if (fsEntry is ICloudDirectoryEntry && IsRoot(fsEntry as ICloudDirectoryEntry))
        {
            return ProviderInfo.CustomerTitle;
        }

        return Global.ReplaceInvalidCharsAndTruncate(fsEntry.Name);
    }

    protected string PathParent(string path)
    {
        if (!string.IsNullOrEmpty(path))
        {
            var index = path.TrimEnd('/').LastIndexOf('/');
            if (index != -1)
            {
                //Cut to it
                return path.Substring(0, index);
            }
        }

        return path;
    }

    protected Folder<string> ToFolder(ICloudDirectoryEntry fsEntry)
    {
        if (fsEntry == null)
        {
            return null;
        }

        if (fsEntry is ErrorEntry)
        {
            //Return error entry
            return ToErrorFolder(fsEntry as ErrorEntry);
        }

        //var childFoldersCount = fsEntry.OfType<ICloudDirectoryEntry>().Count();//NOTE: Removed due to performance isssues
        var isRoot = IsRoot(fsEntry);

        var folder = GetFolder();

        folder.Id = MakeId(fsEntry);
        folder.ParentId = isRoot ? null : MakeId(fsEntry.Parent);
        folder.CreateOn = isRoot ? ProviderInfo.CreateOn : fsEntry.Modified;
        folder.ModifiedOn = isRoot ? ProviderInfo.CreateOn : fsEntry.Modified;
        folder.RootId = RootFolderMakeId();

        folder.Title = MakeTitle(fsEntry);
        folder.FilesCount = 0; /*fsEntry.Count - childFoldersCount NOTE: Removed due to performance isssues*/
        folder.FoldersCount = 0; /*childFoldersCount NOTE: Removed due to performance isssues*/
        folder.Private = ProviderInfo.Private;
        folder.HasLogo = ProviderInfo.HasLogo;
        SetFolderType(folder, isRoot);

        if (folder.CreateOn != DateTime.MinValue && folder.CreateOn.Kind == DateTimeKind.Utc)
        {
            folder.CreateOn = _tenantUtil.DateTimeFromUtc(folder.CreateOn);
        }

        if (folder.ModifiedOn != DateTime.MinValue && folder.ModifiedOn.Kind == DateTimeKind.Utc)
        {
            folder.ModifiedOn = _tenantUtil.DateTimeFromUtc(folder.ModifiedOn);
        }

        return folder;
    }

    private static bool IsRoot(ICloudDirectoryEntry entry)
    {
        if (entry != null && entry.Name != null)
        {
            return string.IsNullOrEmpty(entry.Name.Trim('/'));
        }

        return false;
    }

    private File<string> ToErrorFile(ErrorEntry fsEntry)
    {
        if (fsEntry == null)
        {
            return null;
        }

        var file = GetErrorFile(new Thirdparty.ErrorEntry(fsEntry.Error, null));

        file.Id = MakeId(fsEntry);
        file.CreateOn = fsEntry.Modified;
        file.ModifiedOn = fsEntry.Modified;
        file.RootId = MakeId(null);
        file.Title = MakeTitle(fsEntry);

        return file;
    }

    private Folder<string> ToErrorFolder(ErrorEntry fsEntry)
    {
        if (fsEntry == null)
        {
            return null;
        }

        var folder = GetErrorFolder(new Thirdparty.ErrorEntry(fsEntry.Error, null));

        folder.Id = MakeId(fsEntry);
        folder.CreateOn = fsEntry.Modified;
        folder.ModifiedOn = fsEntry.Modified;
        folder.RootId = MakeId(null);
        folder.Title = MakeTitle(fsEntry);

        return folder;
    }

    protected File<string> ToFile(ICloudFileSystemEntry fsEntry)
    {
        if (fsEntry == null)
        {
            return null;
        }

        if (fsEntry is ErrorEntry)
        {
            //Return error entry

            return ToErrorFile(fsEntry as ErrorEntry);
        }

        var file = GetFile();

        file.Id = MakeId(fsEntry);
        file.ContentLength = fsEntry.Length;
        file.CreateOn = fsEntry.Modified.Kind == DateTimeKind.Utc ? _tenantUtil.DateTimeFromUtc(fsEntry.Modified) : fsEntry.Modified;
        file.ParentId = MakeId(fsEntry.Parent);
        file.ModifiedOn = fsEntry.Modified.Kind == DateTimeKind.Utc ? _tenantUtil.DateTimeFromUtc(fsEntry.Modified) : fsEntry.Modified;
        file.NativeAccessor = fsEntry;
        file.Title = MakeTitle(fsEntry);
        file.RootId = RootFolderMakeId();
        file.Encrypted = ProviderInfo.Private;

        return file;
    }

    private ICloudDirectoryEntry _rootFolder;
    protected ICloudDirectoryEntry RootFolder()
    {
        return _rootFolder ??= SharpBoxProviderInfo.Storage.GetRoot();
    }

    private string _rootFolderId;
    private readonly ILogger<SharpBoxDaoBase> _logger;

    protected string RootFolderMakeId()
    {
        return _rootFolderId ??= MakeId(RootFolder());
    }

    protected ICloudDirectoryEntry GetFolderById(object folderId)
    {
        try
        {
            var path = MakePath(folderId);

            return path == "/"
                       ? RootFolder()
                       : SharpBoxProviderInfo.Storage.GetFolder(path);
        }
        catch (SharpBoxException sharpBoxException)
        {
            if (sharpBoxException.ErrorCode == SharpBoxErrorCodes.ErrorFileNotFound)
            {
                return null;
            }

            return new ErrorEntry(sharpBoxException, folderId);
        }
        catch (Exception ex)
        {
            return new ErrorEntry(ex, folderId);
        }
    }

    protected ICloudFileSystemEntry GetFileById(object fileId)
    {
        try
        {
            return SharpBoxProviderInfo.Storage.GetFile(MakePath(fileId), null);
        }
        catch (SharpBoxException sharpBoxException)
        {
            if (sharpBoxException.ErrorCode == SharpBoxErrorCodes.ErrorFileNotFound)
            {
                return null;
            }

            return new ErrorEntry(sharpBoxException, fileId);
        }
        catch (Exception ex)
        {
            return new ErrorEntry(ex, fileId);
        }
    }

    protected IEnumerable<ICloudFileSystemEntry> GetFolderFiles(object folderId)
    {
        return GetFolderFiles(SharpBoxProviderInfo.Storage.GetFolder(MakePath(folderId)));
    }

    protected IEnumerable<ICloudFileSystemEntry> GetFolderSubfolders(object folderId)
    {
        return GetFolderSubfolders(SharpBoxProviderInfo.Storage.GetFolder(MakePath(folderId)));
    }

    protected IEnumerable<ICloudFileSystemEntry> GetFolderFiles(ICloudDirectoryEntry folder)
    {
        return folder.Where(x => x is not ICloudDirectoryEntry);
    }

    protected IEnumerable<ICloudFileSystemEntry> GetFolderSubfolders(ICloudDirectoryEntry folder)
    {
        return folder.Where(x => x is ICloudDirectoryEntry);
    }

    protected string GetAvailableTitle(string requestTitle, ICloudDirectoryEntry parentFolder, Func<string, ICloudDirectoryEntry, bool> isExist)
    {
        if (!isExist(requestTitle, parentFolder))
        {
            return requestTitle;
        }

        var re = new Regex(@"( \(((?<index>[0-9])+)\)(\.[^\.]*)?)$");
        var match = re.Match(requestTitle);

        if (!match.Success)
        {
            var insertIndex = requestTitle.Length;
            if (requestTitle.LastIndexOf('.') != -1)
            {
                insertIndex = requestTitle.LastIndexOf('.');
            }

            requestTitle = requestTitle.Insert(insertIndex, " (1)");
        }

        while (isExist(requestTitle, parentFolder))
        {
            requestTitle = re.Replace(requestTitle, MatchEvaluator);
        }

        return requestTitle;
    }

    protected async Task<string> GetAvailableTitleAsync(string requestTitle, ICloudDirectoryEntry parentFolder, Func<string, ICloudDirectoryEntry, Task<bool>> isExist)
    {
        if (!await isExist(requestTitle, parentFolder))
        {
            return requestTitle;
        }

        var re = new Regex(@"( \(((?<index>[0-9])+)\)(\.[^\.]*)?)$");
        var match = re.Match(requestTitle);

        if (!match.Success)
        {
            var insertIndex = requestTitle.Length;
            if (requestTitle.LastIndexOf(".", StringComparison.InvariantCulture) != -1)
            {
                insertIndex = requestTitle.LastIndexOf(".", StringComparison.InvariantCulture);
            }

            requestTitle = requestTitle.Insert(insertIndex, " (1)");
        }

        while (await isExist(requestTitle, parentFolder))
        {
            requestTitle = re.Replace(requestTitle, MatchEvaluator);
        }

        return requestTitle;
    }

    public override Task<IEnumerable<string>> GetChildrenAsync(string folderId)
    {
        var subFolders = GetFolderSubfolders(folderId).Select(x => MakeId(x));
        var files = GetFolderFiles(folderId).Select(x => MakeId(x));

        return Task.FromResult(subFolders.Concat(files));
    }

    private string MatchEvaluator(Match match)
    {
        var index = Convert.ToInt32(match.Groups[2].Value);
        var staticText = match.Value.Substring(string.Format(" ({0})", index).Length);

        return string.Format(" ({0}){1}", index + 1, staticText);
    }
}

static file class Queries
{
    public static readonly Func<FilesDbContext, int, string, IAsyncEnumerable<string>> IdsAsync =
        EF.CompileAsyncQuery(
            (FilesDbContext ctx, int tenantId, string idStart) =>
                ctx.ThirdpartyIdMapping
                    .Where(r => r.TenantId == tenantId)
                    .Where(r => r.Id.StartsWith(idStart))
                    .Select(r => r.Id));

    public static readonly Func<FilesDbContext, int, string, IAsyncEnumerable<DbFilesThirdpartyIdMapping>>
        ThirdpartyIdMappingsAsync = EF.CompileAsyncQuery(
            (FilesDbContext ctx, int tenantId, string hashId) =>
                ctx.ThirdpartyIdMapping
                    .Where(r => r.TenantId == tenantId)
                    .Where(r => r.HashId == hashId));

    public static readonly Func<FilesDbContext, int, string, IAsyncEnumerable<DbFilesSecurity>> DbFilesSecuritiesAsync =
        EF.CompileAsyncQuery(
            (FilesDbContext ctx, int tenantId, string entryId) =>
                ctx.Security
                    .Where(r => r.TenantId == tenantId)
                    .Where(r => r.EntryId == entryId));

    public static readonly Func<FilesDbContext, int, string, IAsyncEnumerable<DbFilesTagLink>> DbFilesTagLinksAsync =
        EF.CompileAsyncQuery(
            (FilesDbContext ctx, int tenantId, string entryId) =>
                ctx.TagLink
                    .Where(r => r.TenantId == tenantId)
                    .Where(r => r.EntryId == entryId));

    public static readonly Func<FilesDbContext, int, string, IAsyncEnumerable<DbFilesLink>> FilesLinksBySourceIdAsync =
        EF.CompileAsyncQuery(
            (FilesDbContext ctx, int tenantId, string sourceId) =>
                ctx.FilesLink
                    .Where(r => r.TenantId == tenantId)
                    .Where(l => l.SourceId == sourceId));

    public static readonly Func<FilesDbContext, int, string, IAsyncEnumerable<DbFilesLink>> FilesLinksByLinkedIdAsync =
        EF.CompileAsyncQuery(
            (FilesDbContext ctx, int tenantId, string linkedId) =>
                ctx.FilesLink
                    .Where(r => r.TenantId == tenantId)
                    .Where(l => l.LinkedId == linkedId));
}