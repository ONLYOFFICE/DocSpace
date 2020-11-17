using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;

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
        #region Only in TMFileDao

        public void ReassignFiles(string[] fileIds, Guid newOwnerId)
        {
        }

        public List<File<string>> GetFiles(string[] parentIds, FilterType filterType, bool subjectGroup, Guid subjectID, string searchText, bool searchInContent)
        {
            return new List<File<string>>();
        }

        public IEnumerable<File<string>> Search(string text, bool bunch)
        {
            return null;
        }

        public bool IsExistOnStorage(File<string> file)
        {
            return true;
        }

        public void SaveEditHistory(File<string> file, string changes, Stream differenceStream)
        {
            //Do nothing
        }

        public List<EditHistory> GetEditHistory(DocumentServiceHelper documentServiceHelper, string fileId, int fileVersion)
        {
            return null;
        }

        public Stream GetDifferenceStream(File<string> file)
        {
            return null;
        }

        public bool ContainChanges(string fileId, int fileVersion)
        {
            return false;
        }

        public string GetUniqFilePath(File<string> file, string fileTitle)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<(File<int>, SmallShareRecord)> GetFeeds(int tenant, DateTime from, DateTime to)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<int> GetTenantsWithFeeds(DateTime fromTime)
        {
            throw new NotImplementedException();
        }

        public void ReassignFolders(string[] folderIds, Guid newOwnerId)
        {
        }

        public IEnumerable<Folder<string>> SearchFolders(string text, bool bunch)
        {
            return null;
        }

        public string GetFolderID(string module, string bunch, string data, bool createIfNotExists)
        {
            return null;
        }

        public IEnumerable<string> GetFolderIDs(string module, string bunch, IEnumerable<string> data, bool createIfNotExists)
        {
            return new List<string>();
        }

        public string GetFolderIDCommon(bool createIfNotExists)
        {
            return null;
        }

        public string GetFolderIDUser(bool createIfNotExists, Guid? userId)
        {
            return null;
        }

        public string GetFolderIDShare(bool createIfNotExists)
        {
            return null;
        }

        public string GetFolderIDRecent(bool createIfNotExists)
        {
            return null;
        }

        public string GetFolderIDFavorites(bool createIfNotExists)
        {
            return null;
        }

        public string GetFolderIDTemplates(bool createIfNotExists)
        {
            return null;
        }

        public string GetFolderIDPrivacy(bool createIfNotExists, Guid? userId)
        {
            return null;
        }

        public string GetFolderIDTrash(bool createIfNotExists, Guid? userId)
        {
            return null;
        }


        public string GetFolderIDPhotos(bool createIfNotExists)
        {
            return null;
        }

        public string GetFolderIDProjects(bool createIfNotExists)
        {
            return null;
        }

        public string GetBunchObjectID(string folderID)
        {
            return null;
        }

        public Dictionary<string, string> GetBunchObjectIDs(List<string> folderIDs)
        {
            return null;
        }

        public IEnumerable<(Folder<string>, SmallShareRecord)> GetFeedsForFolders(int tenant, DateTime from, DateTime to)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<string> GetTenantsWithFeedsForFolders(DateTime fromTime)
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
        protected FilesDbContext FilesDbContext { get; }
        protected SetupInfo SetupInfo { get; }
        public ILog Log { get; }
        protected FileUtility FileUtility { get; }

        public RegexDaoSelectorBase<T> DaoSelector { get; set; }
        public T ProviderInfo { get; set; }
        public string PathPrefix { get; private set; }

        public abstract string Id { get; }

        public ThirdPartyProviderDao(
            IServiceProvider serviceProvider,
            UserManager userManager,
            TenantManager tenantManager,
            TenantUtil tenantUtil,
            DbContextManager<FilesDbContext> dbContextManager,
            SetupInfo setupInfo,
            IOptionsMonitor<ILog> monitor,
            FileUtility fileUtility)
        {
            ServiceProvider = serviceProvider;
            UserManager = userManager;
            TenantUtil = tenantUtil;
            FilesDbContext = dbContextManager.Get(FileConstant.DatabaseId);
            SetupInfo = setupInfo;
            Log = monitor.CurrentValue;
            FileUtility = fileUtility;
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
            return set.Where(r => r.TenantId == TenantID);
        }

        protected string MappingID(string id, bool saveIfNotExist = false)
        {
            if (id == null) return null;

            string result;
            if (id.StartsWith(Id))
            {
                result = Regex.Replace(BitConverter.ToString(Hasher.Hash(id.ToString(), HashAlg.MD5)), "-", "").ToLower();
            }
            else
            {
                result = FilesDbContext.ThirdpartyIdMapping
                        .Where(r => r.HashId == id)
                        .Select(r => r.Id)
                        .FirstOrDefault();
            }
            if (saveIfNotExist)
            {
                var newMapping = new DbFilesThirdpartyIdMapping
                {
                    Id = id,
                    HashId = result,
                    TenantId = TenantID
                };

                FilesDbContext.ThirdpartyIdMapping.Add(newMapping);
                FilesDbContext.SaveChanges();
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

            folder.ParentFolderID = null;

            return folder;
        }

        protected File<string> GetFile()
        {
            var file = ServiceProvider.GetService<File<string>>();

            InitFileEntry(file);

            file.Access = FileShare.None;
            file.FileStatus = FileStatus.None;
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
}
