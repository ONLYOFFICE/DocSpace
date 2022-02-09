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

namespace ASC.Files.Thirdparty.Box
{
    [Transient]
    [DebuggerDisplay("{CustomerTitle}")]
    internal class BoxProviderInfo : IProviderInfo
    {
        public OAuth20Token Token { get; set; }

        private string _rootId;

        internal BoxStorage Storage
        {
            get
            {
                if (Wrapper.Storage == null || !Wrapper.Storage.IsOpened)
                {
                    return Wrapper.CreateStorage(Token, ID);
                }
                return Wrapper.Storage;
            }
        }

        internal bool StorageOpened
        {
            get => Wrapper.Storage != null && Wrapper.Storage.IsOpened;
        }

        public int ID { get; set; }

        public Guid Owner { get; set; }

        public string CustomerTitle { get; set; }

        public DateTime CreateOn { get; set; }

        public string RootFolderId
        {
            get { return "box-" + ID; }
        }

        public string ProviderKey { get; set; }

        public FolderType RootFolderType { get; set; }

        public string BoxRootId
        {
            get
            {
                if (string.IsNullOrEmpty(_rootId))
                {
                    _rootId = Storage.GetRootFolderId();
                }
                return _rootId;
            }
        }

        private BoxStorageDisposableWrapper Wrapper { get; set; }
        private BoxProviderInfoHelper BoxProviderInfoHelper { get; }

        public BoxProviderInfo(
            BoxStorageDisposableWrapper wrapper,
            BoxProviderInfoHelper boxProviderInfoHelper)
        {
            Wrapper = wrapper;
            BoxProviderInfoHelper = boxProviderInfoHelper;
        }

        public void Dispose()
        {
            if (StorageOpened)
                Storage.Close();
        }

        public bool CheckAccess()
        {
            try
            {
                return !string.IsNullOrEmpty(BoxRootId);
            }
            catch (UnauthorizedAccessException)
            {
                return false;
            }
        }

        public void InvalidateStorage()
        {
            if (Wrapper != null)
            {
                Wrapper.Dispose();
            }

            CacheReset();
        }

        public void UpdateTitle(string newtitle)
        {
            CustomerTitle = newtitle;
        }

        internal BoxFolder GetBoxFolder(string dropboxFolderPath)
        {
            return BoxProviderInfoHelper.GetBoxFolder(Storage, ID, dropboxFolderPath);
        }

        internal BoxFile GetBoxFile(string dropboxFilePath)
        {
            return BoxProviderInfoHelper.GetBoxFile(Storage, ID, dropboxFilePath);
        }

        internal List<BoxItem> GetBoxItems(string dropboxFolderPath)
        {
            return BoxProviderInfoHelper.GetBoxItems(Storage, ID, dropboxFolderPath);
        }

        internal void CacheReset(BoxItem boxItem)
        {
            BoxProviderInfoHelper.CacheReset(ID, boxItem);
        }

        internal void CacheReset(string boxPath = null, bool? isFile = null)
        {
            BoxProviderInfoHelper.CacheReset(BoxRootId, ID, boxPath, isFile);
        }
    }

    [Scope]
    internal class BoxStorageDisposableWrapper : IDisposable
    {
        public BoxStorage Storage { get; private set; }
        private ConsumerFactory ConsumerFactory { get; }
        private TempStream TempStream { get; }
        private IServiceProvider ServiceProvider { get; }

        public BoxStorageDisposableWrapper(ConsumerFactory consumerFactory, TempStream tempStream, IServiceProvider serviceProvider)
        {
            ConsumerFactory = consumerFactory;
            TempStream = tempStream;
            ServiceProvider = serviceProvider;
        }

        internal BoxStorage CreateStorage(OAuth20Token token, int id)
        {
            if (Storage != null && Storage.IsOpened) return Storage;

            var boxStorage = new BoxStorage(TempStream);
            CheckToken(token, id);

            boxStorage.Open(token);
            return Storage = boxStorage;
        }

        private void CheckToken(OAuth20Token token, int id)
        {
            if (token == null) throw new UnauthorizedAccessException("Cannot create Box session with given token");
            if (token.IsExpired)
            {
                token = OAuth20TokenHelper.RefreshToken<BoxLoginProvider>(ConsumerFactory, token);

                var dbDao = ServiceProvider.GetService<ProviderAccountDao>();
                dbDao.UpdateProviderInfo(id, new AuthData(token: token.ToJson()));
            }
        }

        public void Dispose()
        {
            if (Storage != null)
            {
                Storage.Close();
                Storage = null;
            }
        }
    }

    [Singletone]
    public class BoxProviderInfoHelper
    {
        private readonly TimeSpan CacheExpiration = TimeSpan.FromMinutes(1);
        private readonly ICache CacheFile;
        private readonly ICache CacheFolder;
        private readonly ICache CacheChildItems;
        private readonly ICacheNotify<BoxCacheItem> CacheNotify;

        public BoxProviderInfoHelper(ICacheNotify<BoxCacheItem> cacheNotify, ICache cache)
        {
            CacheFile = cache;
            CacheFolder = cache;
            CacheChildItems = cache;
            CacheNotify = cacheNotify;
            CacheNotify.Subscribe((i) =>
            {
                if (i.ResetAll)
                {
                    CacheChildItems.Remove(new Regex("^box-" + i.Key + ".*"));
                    CacheFile.Remove(new Regex("^boxf-" + i.Key + ".*"));
                    CacheFolder.Remove(new Regex("^boxd-" + i.Key + ".*"));
                }

                if (!i.IsFileExists)
                {
                    CacheChildItems.Remove("box-" + i.Key);

                    CacheFolder.Remove("boxd-" + i.Key);
                }
                else
                {
                    if (i.IsFileExists)
                    {
                        CacheFile.Remove("boxf-" + i.Key);
                    }
                    else
                    {
                        CacheFolder.Remove("boxd-" + i.Key);
                    }
                }
            }, CacheNotifyAction.Remove);
        }

        internal BoxFolder GetBoxFolder(BoxStorage storage, int id, string boxFolderId)
        {
            var folder = CacheFolder.Get<BoxFolder>("boxd-" + id + "-" + boxFolderId);
            if (folder == null)
            {
                folder = storage.GetFolder(boxFolderId);
                if (folder != null)
                    CacheFolder.Insert("boxd-" + id + "-" + boxFolderId, folder, DateTime.UtcNow.Add(CacheExpiration));
            }
            return folder;
        }

        internal BoxFile GetBoxFile(BoxStorage storage, int id, string boxFileId)
        {
            var file = CacheFile.Get<BoxFile>("boxf-" + id + "-" + boxFileId);
            if (file == null)
            {
                file = storage.GetFile(boxFileId);
                if (file != null)
                    CacheFile.Insert("boxf-" + id + "-" + boxFileId, file, DateTime.UtcNow.Add(CacheExpiration));
            }
            return file;
        }

        internal List<BoxItem> GetBoxItems(BoxStorage storage, int id, string boxFolderId)
        {
            var items = CacheChildItems.Get<List<BoxItem>>("box-" + id + "-" + boxFolderId);

            if (items == null)
            {
                items = storage.GetItems(boxFolderId);
                CacheChildItems.Insert("box-" + id + "-" + boxFolderId, items, DateTime.UtcNow.Add(CacheExpiration));
            }
            return items;
        }

        internal void CacheReset(int id, BoxItem boxItem)
        {
            if (boxItem != null)
            {
                CacheNotify.Publish(new BoxCacheItem { IsFile = boxItem is BoxFile, Key = id + "-" + boxItem.Id }, CacheNotifyAction.Remove);
            }
        }

        internal void CacheReset(string boxRootId, int id, string boxId = null, bool? isFile = null)
        {
            var key = id + "-";
            if (boxId == null)
            {
                CacheNotify.Publish(new BoxCacheItem { ResetAll = true, Key = key }, CacheNotifyAction.Remove);
            }
            else
            {
                if (boxId == boxRootId)
                {
                    boxId = "0";
                }
                key += boxId;

                CacheNotify.Publish(new BoxCacheItem { IsFile = isFile ?? false, IsFileExists = isFile.HasValue, Key = key }, CacheNotifyAction.Remove);
            }
        }
    }
}