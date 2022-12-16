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

internal abstract class SharpBoxDaoBase : ThirdPartyProviderDao<SharpBoxProviderInfo>
{
    protected override string Id => "sbox";

    protected SharpBoxDaoBase(
        IServiceProvider serviceProvider,
        UserManager userManager,
        TenantManager tenantManager,
        TenantUtil tenantUtil,
        IDbContextFactory<FilesDbContext> dbContextManager,
        SetupInfo setupInfo,
        ILogger monitor,
        FileUtility fileUtility,
        TempPath tempPath,
        AuthContext authContext)
        : base(serviceProvider, userManager, tenantManager, tenantUtil, dbContextManager, setupInfo, monitor, fileUtility, tempPath, authContext)
    {
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

    protected Task<string> MappingIDAsync(string id)
    {
        return MappingIDAsync(id, false);
    }

    protected async Task UpdatePathInDBAsync(string oldValue, string newValue)
    {
        if (oldValue.Equals(newValue))
        {
            return;
        }

        using var filesDbContext = _dbContextFactory.CreateDbContext();
        var strategy = filesDbContext.Database.CreateExecutionStrategy();

        await strategy.ExecuteAsync(async () =>
        {
            using var filesDbContext = _dbContextFactory.CreateDbContext();
            using var tx = await filesDbContext.Database.BeginTransactionAsync();
            var oldIDs = await Query(filesDbContext.ThirdpartyIdMapping)
            .Where(r => r.Id.StartsWith(oldValue))
            .Select(r => r.Id)
            .ToListAsync();

            foreach (var oldID in oldIDs)
            {
                var oldHashID = await MappingIDAsync(oldID);
                var newID = oldID.Replace(oldValue, newValue);
                var newHashID = await MappingIDAsync(newID);

                var mappingForDelete = await Query(filesDbContext.ThirdpartyIdMapping)
                    .Where(r => r.HashId == oldHashID).ToListAsync();

                var mappingForInsert = mappingForDelete.Select(m => new DbFilesThirdpartyIdMapping
                {
                    TenantId = m.TenantId,
                    Id = newID,
                    HashId = newHashID
                });

                filesDbContext.RemoveRange(mappingForDelete);
                await filesDbContext.AddRangeAsync(mappingForInsert);

                var securityForDelete = await Query(filesDbContext.Security)
                    .Where(r => r.EntryId == oldHashID).ToListAsync();

                var securityForInsert = securityForDelete.Select(s => new DbFilesSecurity
                {
                    TenantId = s.TenantId,
                    TimeStamp = DateTime.Now,
                    EntryId = newHashID,
                    Share = s.Share,
                    Subject = s.Subject,
                    EntryType = s.EntryType,
                    Owner = s.Owner
                });

                filesDbContext.RemoveRange(securityForDelete);
                await filesDbContext.AddRangeAsync(securityForInsert);

                var linkForDelete = await Query(filesDbContext.TagLink)
                    .Where(r => r.EntryId == oldHashID).ToListAsync();

                var linkForInsert = linkForDelete.Select(l => new DbFilesTagLink
                {
                    EntryId = newHashID,
                    Count = l.Count,
                    CreateBy = l.CreateBy,
                    CreateOn = l.CreateOn,
                    EntryType = l.EntryType,
                    TagId = l.TagId,
                    TenantId = l.TenantId
                });

                filesDbContext.RemoveRange(linkForDelete);
                await filesDbContext.AddRangeAsync(linkForInsert);

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

    protected override string MakeId(string path = null)
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
                path = ProviderInfo.Storage.GetFileSystemObjectPath(entry);
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
        return _rootFolder ??= ProviderInfo.Storage.GetRoot();
    }

    private string _rootFolderId;
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
                       : ProviderInfo.Storage.GetFolder(path);
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
            return ProviderInfo.Storage.GetFile(MakePath(fileId), null);
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
        return GetFolderFiles(ProviderInfo.Storage.GetFolder(MakePath(folderId)));
    }

    protected IEnumerable<ICloudFileSystemEntry> GetFolderSubfolders(object folderId)
    {
        return GetFolderSubfolders(ProviderInfo.Storage.GetFolder(MakePath(folderId)));
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

    protected override Task<IEnumerable<string>> GetChildrenAsync(string folderId)
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
