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


using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text.RegularExpressions;

using ASC.Common;
using ASC.Common.Caching;
using ASC.Core.Common.Configuration;
using ASC.FederatedLogin;
using ASC.FederatedLogin.Helpers;
using ASC.FederatedLogin.LoginProviders;
using ASC.Files.Core;

using Microsoft.Extensions.DependencyInjection;
using Microsoft.OneDrive.Sdk;

namespace ASC.Files.Thirdparty.OneDrive
{
    [Transient]
    [DebuggerDisplay("{CustomerTitle}")]
    internal class OneDriveProviderInfo : IProviderInfo
    {
        public OAuth20Token Token { get; set; }

        internal OneDriveStorage Storage
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
            get { return "onedrive-" + ID; }
        }

        public string ProviderKey { get; set; }

        public FolderType RootFolderType { get; set; }
        private OneDriveStorageDisposableWrapper Wrapper { get; set; }
        private OneDriveProviderInfoHelper OneDriveProviderInfoHelper { get; }

        public OneDriveProviderInfo(
            OneDriveStorageDisposableWrapper wrapper,
            OneDriveProviderInfoHelper oneDriveProviderInfoHelper)
        {
            Wrapper = wrapper;
            OneDriveProviderInfoHelper = oneDriveProviderInfoHelper;
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
                return Storage.CheckAccess();
            }
            catch (AggregateException)
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

        internal Item GetOneDriveItem(string itemId)
        {
            return OneDriveProviderInfoHelper.GetOneDriveItem(Storage, ID, itemId);
        }

        internal List<Item> GetOneDriveItems(string onedriveFolderId)
        {
            return OneDriveProviderInfoHelper.GetOneDriveItems(Storage, ID, onedriveFolderId);
        }

        internal void CacheReset(string onedriveId = null)
        {
            OneDriveProviderInfoHelper.CacheReset(ID, onedriveId);
        }
    }

    [Scope(Additional = typeof(OneDriveProviderInfoExtention))]
    internal class OneDriveStorageDisposableWrapper : IDisposable
    {
        internal OneDriveStorage Storage { get; private set; }
        internal ConsumerFactory ConsumerFactory { get; }
        internal IServiceProvider ServiceProvider { get; }

        public OneDriveStorageDisposableWrapper(ConsumerFactory consumerFactory, IServiceProvider serviceProvider)
        {
            ConsumerFactory = consumerFactory;
            ServiceProvider = serviceProvider;
        }

        public OneDriveStorage CreateStorage(OAuth20Token token, int id)
        {
            if (Storage != null && Storage.IsOpened) return Storage;

            var onedriveStorage = ServiceProvider.GetService<OneDriveStorage>();

            CheckToken(token, id);

            onedriveStorage.Open(token);
            return Storage = onedriveStorage;
        }

        private void CheckToken(OAuth20Token token, int id)
        {
            if (token == null) throw new UnauthorizedAccessException("Cannot create GoogleDrive session with given token");
            if (token.IsExpired)
            {
                token = OAuth20TokenHelper.RefreshToken<OneDriveLoginProvider>(ConsumerFactory, token);

                var dbDao = ServiceProvider.GetService<ProviderAccountDao>();
                var authData = new AuthData(token: token.ToJson());
                dbDao.UpdateProviderInfo(id, authData);
            }
        }

        public void Dispose()
        {
            if (Storage != null && Storage.IsOpened)
            {
                Storage.Close();
                Storage = null;
            }
        }
    }

    [Singletone]
    public class OneDriveProviderInfoHelper
    {
        private readonly TimeSpan CacheExpiration;
        private readonly ICache CacheItem;
        private readonly ICache CacheChildItems;
        private readonly ICacheNotify<OneDriveCacheItem> CacheNotify;

        public OneDriveProviderInfoHelper(ICacheNotify<OneDriveCacheItem> cacheNotify, ICache cache)
        {
            CacheExpiration = TimeSpan.FromMinutes(1);
            CacheItem = cache;
            CacheChildItems = cache;

            CacheNotify = cacheNotify;
            CacheNotify.Subscribe((i) =>
            {
                if (i.ResetAll)
                {
                    CacheChildItems.Remove(new Regex("^onedrivei-" + i.Key + ".*"));
                    CacheItem.Remove(new Regex("^onedrive-" + i.Key + ".*"));
                }
                else
                {
                    CacheChildItems.Remove(new Regex("onedrivei-" + i.Key));
                    CacheItem.Remove("onedrive-" + i.Key);
                }
            }, CacheNotifyAction.Remove);
        }

        internal Item GetOneDriveItem(OneDriveStorage storage, int id, string itemId)
        {
            var file = CacheItem.Get<Item>("onedrive-" + id + "-" + itemId);
            if (file == null)
            {
                file = storage.GetItem(itemId);
                if (file != null)
                    CacheItem.Insert("onedrive-" + id + "-" + itemId, file, DateTime.UtcNow.Add(CacheExpiration));
            }
            return file;
        }

        internal List<Item> GetOneDriveItems(OneDriveStorage storage, int id, string onedriveFolderId)
        {
            var items = CacheChildItems.Get<List<Item>>("onedrivei-" + id + "-" + onedriveFolderId);

            if (items == null)
            {
                items = storage.GetItems(onedriveFolderId);
                CacheChildItems.Insert("onedrivei-" + id + "-" + onedriveFolderId, items, DateTime.UtcNow.Add(CacheExpiration));
            }
            return items;
        }

        internal void CacheReset(int id, string onedriveId = null)
        {
            var key = id + "-";
            if (string.IsNullOrEmpty(onedriveId))
            {
                CacheNotify.Publish(new OneDriveCacheItem { ResetAll = true, Key = key }, CacheNotifyAction.Remove);
            }
            else
            {
                key += onedriveId;

                CacheNotify.Publish(new OneDriveCacheItem { Key = key }, CacheNotifyAction.Remove);
            }
        }
    }
    public static class OneDriveProviderInfoExtention
    {
        public static void Register(DIHelper dIHelper)
        {
            dIHelper.TryAdd<OneDriveStorage>();
        }
    }

}