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


using File = Microsoft.SharePoint.Client.File;
using Folder = Microsoft.SharePoint.Client.Folder;

namespace ASC.Files.Thirdparty.SharePoint
{
    [Transient]
    public class SharePointProviderInfo : IProviderInfo
    {
        private ClientContext clientContext;

        public int ID { get; set; }
        public string ProviderKey { get; set; }
        public Guid Owner { get; set; }
        public FolderType RootFolderType { get; set; }
        public DateTime CreateOn { get; set; }
        public string CustomerTitle { get; set; }
        public string RootFolderId { get { return "spoint-" + ID; } }

        public string SpRootFolderId = "/Shared Documents";

        public SharePointProviderInfo(
            IOptionsMonitor<ILog> options,
            IServiceProvider serviceProvider,
            TenantUtil tenantUtil,
            SharePointProviderInfoHelper sharePointProviderInfoHelper,
            TempStream tempStream)
        {
            Log = options.CurrentValue;
            ServiceProvider = serviceProvider;
            TenantUtil = tenantUtil;
            SharePointProviderInfoHelper = sharePointProviderInfoHelper;
            TempStream = tempStream;
        }

        public bool CheckAccess()
        {
            try
            {
                clientContext.Load(clientContext.Web);
                clientContext.ExecuteQuery();
                return true;
            }
            catch (Exception e)
            {
                Log.Warn("CheckAccess", e);
                return false;
            }
        }

        public void InvalidateStorage()
        {
            if (clientContext != null)
            {
                clientContext.Dispose();
            }

            SharePointProviderInfoHelper.Invalidate();
        }

        public void UpdateTitle(string newtitle)
        {
            CustomerTitle = newtitle;
        }

        public void InitClientContext(AuthData authData)
        {
            var authUrl = authData.Url;
            ICredentials credentials = new NetworkCredential(authData.Login, authData.Password);

            if (authData.Login.EndsWith("onmicrosoft.com"))
            {
                var personalPath = string.Concat("/personal/", authData.Login.Replace("@", "_").Replace(".", "_").ToLower());
                SpRootFolderId = string.Concat(personalPath, "/Documents");

                var ss = new SecureString();
                foreach (var p in authData.Password)
                {
                    ss.AppendChar(p);
                }
                authUrl = string.Concat(authData.Url.TrimEnd('/'), personalPath);
                //TODO
                //credentials = new SharePointOnlineCredentials(authData.Login, ss);

            }

            clientContext = new ClientContext(authUrl)
            {
                AuthenticationMode = ClientAuthenticationMode.Default,
                Credentials = credentials
            };
        }

        #region Files

        public File GetFileById(object id)
        {
            var key = "spointf-" + MakeId(id);
            var file = SharePointProviderInfoHelper.GetFile(key);
            if (file == null)
            {
                file = GetFile(id);
                if (file != null)
                {
                    SharePointProviderInfoHelper.AddFile(key, file);
                }
            }
            return file;
        }

        private File GetFile(object id)
        {
            var file = clientContext.Web.GetFileByServerRelativeUrl((string)id);
            clientContext.Load(file);
            clientContext.Load(file.ListItemAllFields);

            try
            {
                clientContext.ExecuteQuery();
            }
            catch (Exception ex)
            {
                SharePointProviderInfoHelper.PublishFolder(MakeId(GetParentFolderId(id)));
                var serverException = (ServerException)ex;
                if (serverException.ServerErrorTypeName == (typeof(FileNotFoundException)).ToString())
                {
                    return null;
                }
                return new SharePointFileErrorEntry(file.Context, file.Path) { Error = ex.Message, ID = id };
            }

            return file;
        }

        public Stream GetFileStream(object id, int offset = 0)
        {
            var file = GetFileById(id);

            if (file is SharePointFileErrorEntry) return null;
            var fileInfo = File.OpenBinaryDirect(clientContext, (string)id);
            clientContext.ExecuteQuery();

            var tempBuffer = TempStream.Create();
            using (var str = fileInfo.Stream)
            {
                if (str != null)
                {
                    str.CopyTo(tempBuffer);
                    tempBuffer.Flush();
                    tempBuffer.Seek(offset, SeekOrigin.Begin);
                }
            }

            return tempBuffer;
        }

        public File CreateFile(string id, Stream stream)
        {
            byte[] b;

            using (var br = new BinaryReader(stream))
            {
                b = br.ReadBytes((int)stream.Length);
            }

            var file = clientContext.Web.RootFolder.Files.Add(new FileCreationInformation { Content = b, Url = id, Overwrite = true });
            clientContext.Load(file);
            clientContext.Load(file.ListItemAllFields);
            clientContext.ExecuteQuery();

            SharePointProviderInfoHelper.AddFile("spointf-" + MakeId(id), file);
            SharePointProviderInfoHelper.PublishFolder(MakeId(GetParentFolderId(id)));

            return file;
        }

        public void DeleteFile(string id)
        {
            SharePointProviderInfoHelper.PublishFile(MakeId(id), MakeId(GetParentFolderId(id)));

            var file = GetFileById(id);

            if (file is SharePointFileErrorEntry) return;

            file.DeleteObject();
            clientContext.ExecuteQuery();
        }

        public string RenameFile(string id, string newTitle)
        {
            SharePointProviderInfoHelper.PublishFile(MakeId(id), MakeId(GetParentFolderId(id)));

            var file = GetFileById(id);

            if (file is SharePointFileErrorEntry) return MakeId();

            var newUrl = GetParentFolderId(file.ServerRelativeUrl) + "/" + newTitle;
            file.MoveTo(newUrl, MoveOperations.Overwrite);
            clientContext.ExecuteQuery();

            return MakeId(newUrl);
        }

        public string MoveFile(string id, string toFolderId)
        {
            SharePointProviderInfoHelper.PublishFile(MakeId(id), MakeId(GetParentFolderId(id)));
            SharePointProviderInfoHelper.PublishFolder(MakeId(toFolderId));

            var file = GetFileById(id);

            if (file is SharePointFileErrorEntry) return MakeId();

            var newUrl = toFolderId + "/" + file.Name;
            file.MoveTo(newUrl, MoveOperations.Overwrite);
            clientContext.ExecuteQuery();

            return MakeId(newUrl);
        }

        public File CopyFile(string id, string toFolderId)
        {
            SharePointProviderInfoHelper.PublishFolder(MakeId(toFolderId), MakeId(GetParentFolderId(id)));

            var file = GetFileById(id);

            if (file is SharePointFileErrorEntry) return file;

            var newUrl = toFolderId + "/" + file.Name;
            file.CopyTo(newUrl, false);
            clientContext.ExecuteQuery();

            return file;
        }

        public File<string> ToFile(File file)
        {
            if (file == null)
                return null;

            var result = ServiceProvider.GetService<File<string>>();

            if (file is SharePointFileErrorEntry errorFile)
            {
                result.ID = MakeId(errorFile.ID);
                result.FolderID = MakeId(GetParentFolderId(errorFile.ID));
                result.CreateBy = Owner;
                result.CreateOn = DateTime.UtcNow;
                result.ModifiedBy = Owner;
                result.ModifiedOn = DateTime.UtcNow;
                result.ProviderId = ID;
                result.ProviderKey = ProviderKey;
                result.RootFolderCreator = Owner;
                result.RootFolderId = MakeId(RootFolder.ServerRelativeUrl);
                result.RootFolderType = RootFolderType;
                result.Title = MakeTitle(GetTitleById(errorFile.ID));
                result.Error = errorFile.Error;

                return result;
            }

            result.ID = MakeId(file.ServerRelativeUrl);
            result.Access = Core.Security.FileShare.None;
            //ContentLength = file.Length,
            result.CreateBy = Owner;
            result.CreateOn = file.TimeCreated.Kind == DateTimeKind.Utc ? TenantUtil.DateTimeFromUtc(file.TimeCreated) : file.TimeCreated;
            result.FolderID = MakeId(GetParentFolderId(file.ServerRelativeUrl));
            result.ModifiedBy = Owner;
            result.ModifiedOn = file.TimeLastModified.Kind == DateTimeKind.Utc ? TenantUtil.DateTimeFromUtc(file.TimeLastModified) : file.TimeLastModified;
            result.NativeAccessor = file;
            result.ProviderId = ID;
            result.ProviderKey = ProviderKey;
            result.Title = MakeTitle(file.Name);
            result.RootFolderId = MakeId(SpRootFolderId);
            result.RootFolderType = RootFolderType;
            result.RootFolderCreator = Owner;
            result.Shared = false;
            result.Version = 1;

            if (file.IsPropertyAvailable("Length"))
            {
                //TODO
                //result.ContentLength = file.Length;
            }
            else if (file.IsObjectPropertyInstantiated("ListItemAllFields"))
            {
                result.ContentLength = Convert.ToInt64(file.ListItemAllFields["File_x0020_Size"]);
            }

            return result;
        }

        #endregion

        #region Folders

        public Folder RootFolder
        {
            get
            {
                var key = "spointd-" + MakeId();
                var folder = SharePointProviderInfoHelper.GetFolder(key);
                if (folder == null)
                {
                    folder = GetFolderById(SpRootFolderId);
                    SharePointProviderInfoHelper.AddFolder(key, folder);
                }
                return folder;
            }
        }

        public ILog Log { get; }
        private IServiceProvider ServiceProvider { get; }
        private TenantUtil TenantUtil { get; }
        public SharePointProviderInfoHelper SharePointProviderInfoHelper { get; }
        public TempStream TempStream { get; }

        public Folder GetFolderById(object id)
        {
            var key = "spointd-" + MakeId(id);
            var folder = SharePointProviderInfoHelper.GetFolder(key);
            if (folder == null)
            {
                folder = GetFolder(id);
                if (folder != null)
                {
                    SharePointProviderInfoHelper.AddFolder(key, folder);
                }
            }
            return folder;
        }

        private Folder GetFolder(object id)
        {
            if ((string)id == "") id = SpRootFolderId;
            var folder = clientContext.Web.GetFolderByServerRelativeUrl((string)id);
            clientContext.Load(folder);
            clientContext.Load(folder.Files, collection => collection.IncludeWithDefaultProperties(r => r.ListItemAllFields));
            clientContext.Load(folder.Folders);

            try
            {
                clientContext.ExecuteQuery();
            }
            catch (Exception ex)
            {
                SharePointProviderInfoHelper.PublishFolder(MakeId(GetParentFolderId(id)));
                var serverException = (ServerException)ex;
                if (serverException.ServerErrorTypeName == (typeof(FileNotFoundException)).ToString())
                {
                    return null;
                }
                return new SharePointFolderErrorEntry(folder.Context, folder.Path) { Error = ex.Message, ID = id };
            }

            return folder;
        }

        public Folder GetParentFolder(string serverRelativeUrl)
        {
            return GetFolderById(GetParentFolderId(serverRelativeUrl));
        }

        public IEnumerable<File> GetFolderFiles(object id)
        {
            var folder = GetFolderById(id);
            if (folder is SharePointFolderErrorEntry) return new List<File>();

            return GetFolderById(id).Files;
        }

        public IEnumerable<Folder> GetFolderFolders(object id)
        {
            var folder = GetFolderById(id);
            if (folder is SharePointFolderErrorEntry) return new List<Folder>();

            return folder.Folders.ToList().Where(r => r.ServerRelativeUrl != SpRootFolderId + "/" + "Forms");
        }

        public object RenameFolder(object id, string newTitle)
        {
            SharePointProviderInfoHelper.PublishFolder(MakeId(id), MakeId(GetParentFolderId(id)));

            var folder = GetFolderById(id);
            if (folder is SharePointFolderErrorEntry) return MakeId(id);

            return MakeId(MoveFld(folder, GetParentFolderId(id) + "/" + newTitle).ServerRelativeUrl);
        }

        public string MoveFolder(string id, string toFolderId)
        {
            SharePointProviderInfoHelper.PublishFolder(MakeId(id), MakeId(GetParentFolderId(id)), MakeId(toFolderId));

            var folder = GetFolderById(id);
            if (folder is SharePointFolderErrorEntry) return MakeId(id);

            return MakeId(MoveFld(folder, toFolderId + "/" + GetFolderById(id).Name).ServerRelativeUrl);
        }

        public Folder CopyFolder(object id, object toFolderId)
        {
            SharePointProviderInfoHelper.PublishFolder(MakeId(toFolderId));

            var folder = GetFolderById(id);
            if (folder is SharePointFolderErrorEntry) return folder;

            return MoveFld(folder, toFolderId + "/" + GetFolderById(id).Name, false);
        }

        private Folder MoveFld(Folder folder, string newUrl, bool delete = true)
        {
            var newFolder = CreateFolder(newUrl);

            if (delete)
            {
                folder.Folders.ToList().ForEach(r => MoveFolder(r.ServerRelativeUrl, newUrl));
                folder.Files.ToList().ForEach(r => MoveFile(r.ServerRelativeUrl, newUrl));

                folder.DeleteObject();
                clientContext.ExecuteQuery();
            }
            else
            {
                folder.Folders.ToList().ForEach(r => CopyFolder(r.ServerRelativeUrl, newUrl));
                folder.Files.ToList().ForEach(r => CopyFile(r.ServerRelativeUrl, newUrl));
            }

            return newFolder;
        }

        public Folder CreateFolder(string id)
        {
            var folder = clientContext.Web.RootFolder.Folders.Add(id);
            clientContext.Load(folder);
            clientContext.ExecuteQuery();

            SharePointProviderInfoHelper.CreateFolder(id, MakeId(GetParentFolderId(id)), folder);
            return folder;
        }

        public void DeleteFolder(string id)
        {
            SharePointProviderInfoHelper.PublishFolder(MakeId(id), MakeId(GetParentFolderId(id)));

            var folder = GetFolderById(id);

            if (folder is SharePointFolderErrorEntry) return;

            folder.DeleteObject();
            clientContext.ExecuteQuery();
        }

        public Folder<string> ToFolder(Folder folder)
        {
            if (folder == null) return null;

            var result = ServiceProvider.GetService<Folder<string>>();

            if (folder is SharePointFolderErrorEntry errorFolder)
            {
                result.ID = MakeId(errorFolder.ID);
                result.FolderID = null;
                result.CreateBy = Owner;
                result.CreateOn = DateTime.UtcNow;
                result.FolderType = FolderType.DEFAULT;
                result.ModifiedBy = Owner;
                result.ModifiedOn = DateTime.UtcNow;
                result.ProviderId = ID;
                result.ProviderKey = ProviderKey;
                result.RootFolderCreator = Owner;
                result.RootFolderId = MakeId(SpRootFolderId);
                result.RootFolderType = RootFolderType;
                result.Shareable = false;
                result.Title = MakeTitle(GetTitleById(errorFolder.ID));
                result.TotalFiles = 0;
                result.TotalSubFolders = 0;
                result.Error = errorFolder.Error;

                return result;
            }

            var isRoot = folder.ServerRelativeUrl == SpRootFolderId;

            result.ID = MakeId(isRoot ? "" : folder.ServerRelativeUrl);
            result.FolderID = isRoot ? null : MakeId(GetParentFolderId(folder.ServerRelativeUrl));
            result.CreateBy = Owner;
            result.CreateOn = CreateOn;
            result.FolderType = FolderType.DEFAULT;
            result.ModifiedBy = Owner;
            result.ModifiedOn = CreateOn;
            result.ProviderId = ID;
            result.ProviderKey = ProviderKey;
            result.RootFolderCreator = Owner;
            result.RootFolderId = MakeId(RootFolder.ServerRelativeUrl);
            result.RootFolderType = RootFolderType;
            result.Shareable = false;
            result.Title = isRoot ? CustomerTitle : MakeTitle(folder.Name);
            result.TotalFiles = 0;
            result.TotalSubFolders = 0;

            return result;
        }

        #endregion

        public string MakeId(string path = "")
        {
            path = path.Replace(SpRootFolderId, "");
            return string.Format("{0}{1}", "spoint-" + ID, string.IsNullOrEmpty(path) || path == "/" || path == SpRootFolderId ? "" : ("-" + path.Replace('/', '|')));
        }

        private string MakeId(object path)
        {
            return MakeId((string)path);
        }

        protected string MakeTitle(string name)
        {
            return Global.ReplaceInvalidCharsAndTruncate(name);
        }

        protected string GetParentFolderId(string serverRelativeUrl)
        {
            var path = serverRelativeUrl.Split('/');

            return string.Join("/", path.Take(path.Length - 1));
        }

        protected string GetParentFolderId(object serverRelativeUrl)
        {
            return GetParentFolderId((string)serverRelativeUrl);
        }

        protected string GetTitleById(object serverRelativeUrl)
        {
            return ((string)serverRelativeUrl).Split('/').Last();
        }


        public void Dispose()
        {
            clientContext.Dispose();
        }
    }

    [Singletone]
    public class SharePointProviderInfoHelper
    {
        private readonly TimeSpan CacheExpiration;
        private readonly ICache FileCache;
        private readonly ICache FolderCache;
        private readonly ICacheNotify<SharePointProviderCacheItem> Notify;

        public SharePointProviderInfoHelper(ICacheNotify<SharePointProviderCacheItem> notify, ICache cache)
        {
            CacheExpiration = TimeSpan.FromMinutes(1);
            FileCache = cache;
            FolderCache = cache;
            Notify = notify;

            Notify.Subscribe((i) =>
            {
                if (!string.IsNullOrEmpty(i.FileKey))
                {
                    FileCache.Remove("spointf-" + i.FileKey);
                }
                if (!string.IsNullOrEmpty(i.FolderKey))
                {
                    FolderCache.Remove("spointd-" + i.FolderKey);
                }
                if (string.IsNullOrEmpty(i.FileKey) && string.IsNullOrEmpty(i.FolderKey))
                {
                    FileCache.Remove(new Regex("^spointf-.*"));
                    FolderCache.Remove(new Regex("^spointd-.*"));
                }
            }, CacheNotifyAction.Remove);
        }

        public void Invalidate()
        {
            Notify.Publish(new SharePointProviderCacheItem { }, CacheNotifyAction.Remove);
        }
        public void PublishFolder(string id)
        {
            Notify.Publish(new SharePointProviderCacheItem { FolderKey = id }, CacheNotifyAction.Remove);
        }

        public void PublishFolder(string id1, string id2)
        {
            PublishFolder(id1);
            PublishFolder(id2);
        }

        public void PublishFolder(string id1, string id2, string id3)
        {
            PublishFolder(id1, id2);
            PublishFolder(id3);
        }

        public void PublishFile(string fileId, string folderId)
        {
            Notify.Publish(new SharePointProviderCacheItem { FileKey = fileId, FolderKey = folderId }, CacheNotifyAction.Remove);
        }

        public void CreateFolder(string id, string parentFolderId, Folder folder)
        {
            PublishFolder(parentFolderId);
            FolderCache.Insert("spointd-" + id, folder, DateTime.UtcNow.Add(CacheExpiration));
        }

        public Folder GetFolder(string key)
        {
            return FolderCache.Get<Folder>(key);
        }

        public void AddFolder(string key, Folder folder)
        {
            FolderCache.Insert(key, folder, DateTime.UtcNow.Add(CacheExpiration));
        }

        public File GetFile(string key)
        {
            return FileCache.Get<File>(key);
        }

        public void AddFile(string key, File file)
        {
            FileCache.Insert(key, file, DateTime.UtcNow.Add(CacheExpiration));
        }
    }
}