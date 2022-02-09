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

namespace ASC.Files.Thirdparty.Dropbox
{
    [Transient]
    [DebuggerDisplay("{CustomerTitle}")]
    internal class DropboxProviderInfo : IProviderInfo
    {
        public OAuth20Token Token { get; set; }

        internal DropboxStorage Storage
        {
            get
            {
                if (Wrapper.Storage == null || !Wrapper.Storage.IsOpened)
                {
                    return Wrapper.CreateStorage(Token);
                }
                return Wrapper.Storage;
            }
        }

        internal bool StorageOpened
        {
            get => Wrapper.Storage != null && Wrapper.Storage.IsOpened;
        }

        private DropboxStorageDisposableWrapper Wrapper { get; }
        private DropboxProviderInfoHelper DropboxProviderInfoHelper { get; }
        public int ID { get; set; }

        public Guid Owner { get; set; }

        public string CustomerTitle { get; set; }

        public DateTime CreateOn { get; set; }

        public string RootFolderId
        {
            get { return "dropbox-" + ID; }
        }

        public string ProviderKey { get; set; }

        public FolderType RootFolderType { get; set; }


        public DropboxProviderInfo(
            DropboxStorageDisposableWrapper wrapper,
            DropboxProviderInfoHelper dropboxProviderInfoHelper
            )
        {
            Wrapper = wrapper;
            DropboxProviderInfoHelper = dropboxProviderInfoHelper;
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
                Storage.GetUsedSpace();
            }
            catch (AggregateException)
            {
                return false;
            }
            return true;
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

        internal FolderMetadata GetDropboxFolder(string dropboxFolderPath)
        {
            return DropboxProviderInfoHelper.GetDropboxFolder(Storage, ID, dropboxFolderPath);
        }

        internal FileMetadata GetDropboxFile(string dropboxFilePath)
        {
            return DropboxProviderInfoHelper.GetDropboxFile(Storage, ID, dropboxFilePath);
        }

        internal List<Metadata> GetDropboxItems(string dropboxFolderPath)
        {
            return DropboxProviderInfoHelper.GetDropboxItems(Storage, ID, dropboxFolderPath);
        }

        internal void CacheReset(Metadata dropboxItem)
        {
            DropboxProviderInfoHelper.CacheReset(ID, dropboxItem);
        }

        internal void CacheReset(string dropboxPath = null, bool? isFile = null)
        {
            DropboxProviderInfoHelper.CacheReset(ID, dropboxPath, isFile);
        }
    }

    [Scope]
    internal class DropboxStorageDisposableWrapper : IDisposable
    {
        public DropboxStorage Storage { get; private set; }
        private TempStream TempStream { get; }

        public DropboxStorageDisposableWrapper(TempStream tempStream)
        {
            TempStream = tempStream;
        }

        public DropboxStorage CreateStorage(OAuth20Token token)
        {
            if (Storage != null && Storage.IsOpened) return Storage;

            var dropboxStorage = new DropboxStorage(TempStream);
            dropboxStorage.Open(token);
            return Storage = dropboxStorage;
        }

        public void Dispose()
        {
            Storage?.Close();
            Storage = null;
        }
    }

    [Singletone]
    public class DropboxProviderInfoHelper
    {
        private readonly TimeSpan CacheExpiration;
        private readonly ICache CacheFile;
        private readonly ICache CacheFolder;
        private readonly ICache CacheChildItems;
        private readonly ICacheNotify<DropboxCacheItem> CacheNotify;

        public DropboxProviderInfoHelper(ICacheNotify<DropboxCacheItem> cacheNotify, ICache cache)
        {
            CacheExpiration = TimeSpan.FromMinutes(1);
            CacheFile = cache;
            CacheFolder = cache;
            CacheChildItems = cache;
            CacheNotify = cacheNotify;

            CacheNotify.Subscribe((i) =>
            {
                if (i.ResetAll)
                {
                    CacheFile.Remove(new Regex("^dropboxf-" + i.Key + ".*"));
                    CacheFolder.Remove(new Regex("^dropboxd-" + i.Key + ".*"));
                    CacheChildItems.Remove(new Regex("^dropbox-" + i.Key + ".*"));
                }

                if (!i.IsFileExists)
                {
                    CacheChildItems.Remove("dropbox-" + i.Key);

                    CacheFolder.Remove("dropboxd-" + i.Key);
                }
                else
                {
                    if (i.IsFileExists)
                    {
                        CacheFile.Remove("dropboxf-" + i.Key);
                    }
                    else
                    {
                        CacheFolder.Remove("dropboxd-" + i.Key);
                    }
                }
            }, CacheNotifyAction.Remove);
        }

        internal FolderMetadata GetDropboxFolder(DropboxStorage storage, int id, string dropboxFolderPath)
        {
            var folder = CacheFolder.Get<FolderMetadata>("dropboxd-" + id + "-" + dropboxFolderPath);
            if (folder == null)
            {
                folder = storage.GetFolder(dropboxFolderPath);
                if (folder != null)
                    CacheFolder.Insert("dropboxd-" + id + "-" + dropboxFolderPath, folder, DateTime.UtcNow.Add(CacheExpiration));
            }
            return folder;
        }

        internal FileMetadata GetDropboxFile(DropboxStorage storage, int id, string dropboxFilePath)
        {
            var file = CacheFile.Get<FileMetadata>("dropboxf-" + id + "-" + dropboxFilePath);
            if (file == null)
            {
                file = storage.GetFile(dropboxFilePath);
                if (file != null)
                    CacheFile.Insert("dropboxf-" + id + "-" + dropboxFilePath, file, DateTime.UtcNow.Add(CacheExpiration));
            }
            return file;
        }

        internal List<Metadata> GetDropboxItems(DropboxStorage storage, int id, string dropboxFolderPath)
        {
            var items = CacheChildItems.Get<List<Metadata>>("dropbox-" + id + "-" + dropboxFolderPath);

            if (items == null)
            {
                items = storage.GetItems(dropboxFolderPath);
                CacheChildItems.Insert("dropbox-" + id + "-" + dropboxFolderPath, items, DateTime.UtcNow.Add(CacheExpiration));
            }
            return items;
        }

        internal void CacheReset(int id, Metadata dropboxItem)
        {
            if (dropboxItem != null)
            {
                CacheNotify.Publish(new DropboxCacheItem { IsFile = dropboxItem.AsFolder != null, Key = id + "-" + dropboxItem.PathDisplay }, CacheNotifyAction.Remove);
            }
        }

        internal void CacheReset(int id, string dropboxPath = null, bool? isFile = null)
        {
            var key = id + "-";
            if (dropboxPath == null)
            {
                CacheNotify.Publish(new DropboxCacheItem { ResetAll = true, Key = key }, CacheNotifyAction.Remove);
            }
            else
            {
                key += dropboxPath;

                CacheNotify.Publish(new DropboxCacheItem { IsFile = isFile ?? false, IsFileExists = isFile.HasValue, Key = key }, CacheNotifyAction.Remove);
            }
        }
    }
}