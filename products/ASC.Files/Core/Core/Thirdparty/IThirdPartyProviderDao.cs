using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

using ASC.Common.Logging;
using ASC.Core;
using ASC.Core.Common.EF;
using ASC.Core.Tenants;
using ASC.Files.Core;
using ASC.Files.Core.EF;
using ASC.Files.Core.Security;
using ASC.Files.Core.Thirdparty;
using ASC.Security.Cryptography;
using ASC.Web.Core.Files;
using ASC.Web.Files.Services.DocumentService;
using ASC.Web.Studio.Core;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

using FileShare = ASC.Files.Core.Security.FileShare;

namespace ASC.Files.Thirdparty
{
    internal abstract class ThirdPartyProviderDao
    {
        #region FileDao

        public Task ReassignFilesAsync(string[] fileIds, Guid newOwnerId)
        {
            return Task.CompletedTask;
        }

        public Task<List<File<string>>> GetFilesAsync(IEnumerable<string> parentIds, FilterType filterType, bool subjectGroup, Guid subjectID, string searchText, bool searchInContent)
        {
            return Task.FromResult(new List<File<string>>());
        }

        public IAsyncEnumerable<File<string>> SearchAsync(string text, bool bunch)
        {
            return null;
        }

        public Task<bool> IsExistOnStorageAsync(File<string> file)
        {
            return Task.FromResult(true);
        }

        public Task SaveEditHistoryAsync(File<string> file, string changes, Stream differenceStream)
        {
            //Do nothing
            return Task.CompletedTask;
        }

        public Task<List<EditHistory>> GetEditHistoryAsync(DocumentServiceHelper documentServiceHelper, string fileId, int fileVersion)
        {
            return null;
        }

        public Task<Stream> GetDifferenceStreamAsync(File<string> file)
        {
            return null;
        }

        public Task<bool> ContainChangesAsync(string fileId, int fileVersion)
        {
            return Task.FromResult(false);
        }

        public Task SaveThumbnailAsync(File<string> file, Stream thumbnail)
        {
            //Do nothing
            return Task.CompletedTask;
        }

        public Task<Stream> GetThumbnailAsync(File<string> file)
        {
            return Task.FromResult<Stream>(null);
        }

        public virtual Task<Stream> GetFileStreamAsync(File<string> file)
        {
            return null;
        }

        public string GetUniqFilePath(File<string> file, string fileTitle)
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<(File<int>, SmallShareRecord)>> GetFeedsAsync(int tenant, DateTime from, DateTime to)
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<int>> GetTenantsWithFeedsAsync(DateTime fromTime)
        {
            throw new NotImplementedException();
        }

        #endregion
        #region FolderDao

        public Task ReassignFoldersAsync(string[] folderIds, Guid newOwnerId)
        {
            return Task.CompletedTask;
        }

        public IAsyncEnumerable<Folder<string>> SearchFoldersAsync(string text, bool bunch)
        {
            return null;
        }


        public Task<string> GetFolderIDAsync(string module, string bunch, string data, bool createIfNotExists)
        {
            return null;
        }

        public Task<IEnumerable<string>> GetFolderIDsAsync(string module, string bunch, IEnumerable<string> data, bool createIfNotExists)
        {
            return Task.FromResult((IEnumerable<string>)new List<string>());
        }

        public Task<string> GetFolderIDCommonAsync(bool createIfNotExists)
        {
            return null;
        }


        public Task<string> GetFolderIDUserAsync(bool createIfNotExists, Guid? userId)
        {
            return null;
        }

        public Task<string> GetFolderIDShareAsync(bool createIfNotExists)
        {
            return null;
        }


        public Task<string> GetFolderIDRecentAsync(bool createIfNotExists)
        {
            return null;
        }

        public Task<string> GetFolderIDFavoritesAsync(bool createIfNotExists)
        {
            return null;
        }

        public Task<string> GetFolderIDTemplatesAsync(bool createIfNotExists)
        {
            return null;
        }

        public Task<string> GetFolderIDPrivacyAsync(bool createIfNotExists, Guid? userId)
        {
            return null;
        }

        public Task<string> GetFolderIDTrashAsync(bool createIfNotExists, Guid? userId)
        {
            return null;
        }

        public string GetFolderIDPhotos(bool createIfNotExists)
        {
            return null;
        }


        public Task<string> GetFolderIDProjectsAsync(bool createIfNotExists)
        {
            return null;
        }

        public Task<string> GetBunchObjectIDAsync(string folderID)
        {
            return null;
        }

        public Task<Dictionary<string, string>> GetBunchObjectIDsAsync(List<string> folderIDs)
        {
            return null;
        }

        public Task<IEnumerable<(Folder<string>, SmallShareRecord)>> GetFeedsForFoldersAsync(int tenant, DateTime from, DateTime to)
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<string>> GetTenantsWithFeedsForFoldersAsync(DateTime fromTime)
        {
            throw new NotImplementedException();
        }

        #endregion
    }

    internal abstract class ThirdPartyProviderDao<T> : ThirdPartyProviderDao, IDisposable where T : class, IProviderInfo
    {
        public int TenantID { get; private set; }
        protected IServiceProvider ServiceProvider { get; }
        protected UserManager UserManager { get; }
        protected TenantUtil TenantUtil { get; }
        private Lazy<FilesDbContext> LazyFilesDbContext { get; }
        protected FilesDbContext FilesDbContext { get => LazyFilesDbContext.Value; }
        protected SetupInfo SetupInfo { get; }
        protected ILog Log { get; }
        protected FileUtility FileUtility { get; }
        protected TempPath TempPath { get; }
        protected RegexDaoSelectorBase<T> DaoSelector { get; set; }
        protected T ProviderInfo { get; set; }
        protected string PathPrefix { get; private set; }

        protected abstract string Id { get; }

        protected ThirdPartyProviderDao(
            IServiceProvider serviceProvider,
            UserManager userManager,
            TenantManager tenantManager,
            TenantUtil tenantUtil,
            DbContextManager<FilesDbContext> dbContextManager,
            SetupInfo setupInfo,
            IOptionsMonitor<ILog> monitor,
            FileUtility fileUtility,
            TempPath tempPath)
        {
            ServiceProvider = serviceProvider;
            UserManager = userManager;
            TenantUtil = tenantUtil;
            LazyFilesDbContext = new Lazy<FilesDbContext>(() => dbContextManager.Get(FileConstant.DatabaseId));
            SetupInfo = setupInfo;
            Log = monitor.CurrentValue;
            FileUtility = fileUtility;
            TempPath = tempPath;
            TenantID = tenantManager.GetCurrentTenant().TenantId;
        }

        public void Init(BaseProviderInfo<T> providerInfo, RegexDaoSelectorBase<T> selectorBase)
        {
            ProviderInfo = providerInfo.ProviderInfo;
            PathPrefix = providerInfo.PathPrefix;
            DaoSelector = selectorBase;
        }

        protected IQueryable<TSet> Query<TSet>(DbSet<TSet> set) where TSet : class, IDbFile
        {
            return set.AsQueryable().Where(r => r.TenantId == TenantID);
        }

        protected Task<string> MappingIDAsync(string id, bool saveIfNotExist = false)
        {
            if (id == null) return null;

            return InternalMappingIDAsync(id, saveIfNotExist);
        }

        private async Task<string> InternalMappingIDAsync(string id, bool saveIfNotExist = false)
        {
            string result;
            if (id.StartsWith(Id))
            {
                result = Regex.Replace(BitConverter.ToString(Hasher.Hash(id, HashAlg.MD5)), "-", "").ToLower();
            }
            else
            {
                result = await FilesDbContext.ThirdpartyIdMapping
                        .AsQueryable()
                        .Where(r => r.HashId == id)
                        .Select(r => r.Id)
                        .FirstOrDefaultAsync()
                        .ConfigureAwait(false);
            }
            if (saveIfNotExist)
            {
                var newMapping = new DbFilesThirdpartyIdMapping
                {
                    Id = id,
                    HashId = result,
                    TenantId = TenantID
                };

                await FilesDbContext.ThirdpartyIdMapping.AddAsync(newMapping).ConfigureAwait(false);
                await FilesDbContext.SaveChangesAsync().ConfigureAwait(false);
            }
            return result;
        }

        protected Folder<string> GetFolder()
        {
            var folder = ServiceProvider.GetService<Folder<string>>();

            InitFileEntry(folder);

            folder.FolderType = FolderType.DEFAULT;
            folder.Shareable = false;
            folder.TotalFiles = 0;
            folder.TotalSubFolders = 0;

            return folder;
        }

        protected Folder<string> GetErrorFolder(ErrorEntry entry)
        {
            var folder = GetFolder();

            InitFileEntryError(folder, entry);

            folder.FolderID = null;

            return folder;
        }

        protected File<string> GetFile()
        {
            var file = ServiceProvider.GetService<File<string>>();

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
            fileEntry.ProviderId = ProviderInfo.ID;
            fileEntry.ProviderKey = ProviderInfo.ProviderKey;
            fileEntry.RootFolderCreator = ProviderInfo.Owner;
            fileEntry.RootFolderType = ProviderInfo.RootFolderType;
            fileEntry.RootFolderId = MakeId();
        }

        protected void InitFileEntryError(FileEntry<string> fileEntry, ErrorEntry entry)
        {
            fileEntry.ID = MakeId(entry.ErrorId);
            fileEntry.CreateOn = TenantUtil.DateTimeNow();
            fileEntry.ModifiedOn = TenantUtil.DateTimeNow();
            fileEntry.Error = entry.Error;
        }


        protected abstract string MakeId(string path = null);


        #region SecurityDao
        public Task SetShareAsync(FileShareRecord r)
        {
            return Task.CompletedTask;
        }

        public ValueTask<List<FileShareRecord>> GetSharesAsync(IEnumerable<Guid> subjects)
        {
            List<FileShareRecord> result = null;
            return new ValueTask<List<FileShareRecord>>(result);
        }

        public Task<IEnumerable<FileShareRecord>> GetSharesAsync(IEnumerable<FileEntry<string>> entry)
        {
            return null;
        }

        public Task<IEnumerable<FileShareRecord>> GetSharesAsync(FileEntry<string> entry)
        {
            return null;
        }

        public Task RemoveSubjectAsync(Guid subject)
        {
            return Task.CompletedTask;
        }

        public Task<IEnumerable<FileShareRecord>> GetPureShareRecordsAsync(IEnumerable<FileEntry<string>> entries)
        {
            return null;
        }

        public Task<IEnumerable<FileShareRecord>> GetPureShareRecordsAsync(FileEntry<string> entry)
        {
            return null;
        }

        public Task DeleteShareRecordsAsync(IEnumerable<FileShareRecord> records)
        {
            return Task.CompletedTask;
        }

        public ValueTask<bool> IsSharedAsync(object entryId, FileEntryType type)
        {
            throw new NotImplementedException();
        }

        #endregion

        #region TagDao

        public IAsyncEnumerable<Tag> GetTagsAsync(Guid subject, TagType tagType, IEnumerable<FileEntry<string>> fileEntries)
        {
            return AsyncEnumerable.Empty<Tag>();
        }

        public IAsyncEnumerable<Tag> GetTagsAsync(TagType tagType, IEnumerable<FileEntry<string>> fileEntries)
        {
            return AsyncEnumerable.Empty<Tag>();
        }

        public IAsyncEnumerable<Tag> GetTagsAsync(Guid owner, TagType tagType)
        {
            return AsyncEnumerable.Empty<Tag>();
        }

        public IAsyncEnumerable<Tag> GetTagsAsync(string name, TagType tagType)
        {
            return AsyncEnumerable.Empty<Tag>();
        }

        public IAsyncEnumerable<Tag> GetTagsAsync(string[] names, TagType tagType)
        {
            return AsyncEnumerable.Empty<Tag>();
        }

        public Task<IDictionary<object, IEnumerable<Tag>>> GetTagsAsync(Guid subject, IEnumerable<TagType> tagType, IEnumerable<FileEntry<string>> fileEntries)
        {
            return Task.FromResult((IDictionary<object, IEnumerable<Tag>>)new Dictionary<object, IEnumerable<Tag>>());
        }

        public IAsyncEnumerable<Tag> GetNewTagsAsync(Guid subject, IEnumerable<FileEntry<string>> fileEntries)
        {
            return AsyncEnumerable.Empty<Tag>();
        }

        public IAsyncEnumerable<Tag> GetNewTagsAsync(Guid subject, FileEntry<string> fileEntry)
        {
            return AsyncEnumerable.Empty<Tag>();
        }

        public IEnumerable<Tag> SaveTags(IEnumerable<Tag> tag)
        {
            return new List<Tag>();
        }

        public IEnumerable<Tag> SaveTags(Tag tag)
        {
            return new List<Tag>();
        }

        public void UpdateNewTags(IEnumerable<Tag> tag)
        {
        }

        public void UpdateNewTags(Tag tag)
        {
        }

        public void RemoveTags(IEnumerable<Tag> tag)
        {
        }

        public void RemoveTags(Tag tag)
        {
        }


        public IAsyncEnumerable<Tag> GetTagsAsync(string entryID, FileEntryType entryType, TagType tagType)
        {
            return AsyncEnumerable.Empty<Tag>();
        }

        public void MarkAsNew(Guid subject, FileEntry<string> fileEntry)
        {
        }

        public async IAsyncEnumerable<Tag> GetNewTagsAsync(Guid subject, Folder<string> parentFolder, bool deepSearch)
        {
            var folderId = DaoSelector.ConvertId(parentFolder.ID);

            var entryIDs = await FilesDbContext.ThirdpartyIdMapping
                       .AsQueryable()
                       .Where(r => r.Id.StartsWith(parentFolder.ID))
                       .Select(r => r.HashId)
                       .ToListAsync()
                       .ConfigureAwait(false);

            if (!entryIDs.Any()) yield break;

            var q = from r in FilesDbContext.Tag
                    from l in FilesDbContext.TagLink.AsQueryable().Where(a => a.TenantId == r.TenantId && a.TagId == r.Id).DefaultIfEmpty()
                    where r.TenantId == TenantID && l.TenantId == TenantID && r.Flag == TagType.New && entryIDs.Contains(l.EntryId)
                    select new { tag = r, tagLink = l };

            if (subject != Guid.Empty)
            {
                q = q.Where(r => r.tag.Owner == subject);
            }

            var qList = q
                .Distinct()
                .AsAsyncEnumerable();

            var tags = qList
                .SelectAwait(async r => new Tag
                {
                    TagName = r.tag.Name,
                    TagType = r.tag.Flag,
                    Owner = r.tag.Owner,
                    EntryId = await MappingIDAsync(r.tagLink.EntryId).ConfigureAwait(false),
                    EntryType = r.tagLink.EntryType,
                    Count = r.tagLink.TagCount,
                    Id = r.tag.Id
                });


            if (deepSearch)
            {
                await foreach (var e in tags.ConfigureAwait(false))
                    yield return e;
                yield break;
            }

            var folderFileIds = new[] { parentFolder.ID }
                .Concat(await GetChildrenAsync(folderId).ConfigureAwait(false));

            await foreach (var e in tags.Where(tag => folderFileIds.Contains(tag.EntryId.ToString())).ConfigureAwait(false))
                yield return e;
        }

        protected abstract Task<IEnumerable<string>> GetChildrenAsync(string folderId);

        #endregion

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
}
