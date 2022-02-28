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
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

using ASC.Common;
using ASC.Common.Caching;
using ASC.Common.Logging;
using ASC.Core.Common.Configuration;
using ASC.FederatedLogin;
using ASC.FederatedLogin.Helpers;
using ASC.FederatedLogin.LoginProviders;
using ASC.Files.Core;

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

using DriveFile = Google.Apis.Drive.v3.Data.File;

namespace ASC.Files.Thirdparty.GoogleDrive
{
    [Transient]
    [DebuggerDisplay("{CustomerTitle}")]
    internal class GoogleDriveProviderInfo : IProviderInfo
    {
        public OAuth20Token Token { get; set; }
        private string _driveRootId;

        internal Task<GoogleDriveStorage> StorageAsync
        {
            get
            {
                if (Wrapper.Storage == null || !Wrapper.Storage.IsOpened)
                {
                    return Wrapper.CreateStorageAsync(Token, ID);
                }
                return Task.FromResult(Wrapper.Storage);
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
            get { return "drive-" + ID; }
        }

        public string ProviderKey { get; set; }

        public FolderType RootFolderType { get; set; }

        public string DriveRootId
        {
            get
            {
                if (string.IsNullOrEmpty(_driveRootId))
                {
                    try
                    {
                        _driveRootId = StorageAsync.Result.GetRootFolderId();
                    }
                    catch (Exception ex)
                    {
                        Log.Error("GoogleDrive error", ex);
                        return null;
                    }
                }
                return _driveRootId;
            }
        }

        private GoogleDriveStorageDisposableWrapper Wrapper { get; set; }
        private GoogleDriveProviderInfoHelper GoogleDriveProviderInfoHelper { get; }
        public ILog Log { get; }

        public GoogleDriveProviderInfo(
            GoogleDriveStorageDisposableWrapper storageDisposableWrapper,
            GoogleDriveProviderInfoHelper googleDriveProviderInfoHelper,
            IOptionsMonitor<ILog> options)
        {
            Wrapper = storageDisposableWrapper;
            GoogleDriveProviderInfoHelper = googleDriveProviderInfoHelper;
            Log = options.Get("ASC.Files");
        }

        public void Dispose()
        {
            if (StorageOpened)
                StorageAsync.Result.Close();
        }

        public Task<bool> CheckAccessAsync()
        {
            try
            {
                return Task.FromResult(!string.IsNullOrEmpty(DriveRootId));
            }
            catch (UnauthorizedAccessException)
            {
                return Task.FromResult(false);
            }
        }

        public Task InvalidateStorageAsync()
        {
            if (Wrapper != null)
            {
                Wrapper.Dispose();
            }

            return CacheResetAsync();
        }

        public void UpdateTitle(string newtitle)
        {
            CustomerTitle = newtitle;
        }

        internal async Task<DriveFile> GetDriveEntryAsync(string driveId)
        {
            var storage = await StorageAsync;
            return await GoogleDriveProviderInfoHelper.GetDriveEntryAsync(storage, ID, driveId);
        }

        internal async Task<List<DriveFile>> GetDriveEntriesAsync(string parentDriveId, bool? folder = null)
        {
            var storage = await StorageAsync;
            return await GoogleDriveProviderInfoHelper.GetDriveEntriesAsync(storage, ID, parentDriveId, folder);
        }

        internal Task CacheResetAsync(DriveFile driveEntry)
        {
            return GoogleDriveProviderInfoHelper.CacheResetAsync(driveEntry, ID);
        }

        internal Task CacheResetAsync(string driveId = null, bool? childFolder = null)
        {
            return GoogleDriveProviderInfoHelper.CacheResetAsync(DriveRootId, ID, driveId, childFolder);
        }

        internal Task CacheResetChildsAsync(string parentDriveId, bool? childFolder = null)
        {
            return GoogleDriveProviderInfoHelper.CacheResetChildsAsync(ID, parentDriveId, childFolder);
        }
    }

    [Scope(Additional = typeof(GoogleDriveProviderInfoExtention))]
    internal class GoogleDriveStorageDisposableWrapper : IDisposable
    {
        internal GoogleDriveStorage Storage { get; set; }
        internal ConsumerFactory ConsumerFactory { get; }
        internal IServiceProvider ServiceProvider { get; }

        public GoogleDriveStorageDisposableWrapper(ConsumerFactory consumerFactory, IServiceProvider serviceProvider)
        {
            ConsumerFactory = consumerFactory;
            ServiceProvider = serviceProvider;
        }

        public Task<GoogleDriveStorage> CreateStorageAsync(OAuth20Token token, int id)
        {
            if (Storage != null && Storage.IsOpened) return Task.FromResult(Storage);

            return InternalCreateStorageAsync(token, id);
        }

        public async Task<GoogleDriveStorage> InternalCreateStorageAsync(OAuth20Token token, int id)
        {
            var driveStorage = ServiceProvider.GetService<GoogleDriveStorage>();

            await CheckTokenAsync(token, id).ConfigureAwait(false);

            driveStorage.Open(token);
            return Storage = driveStorage;
        }

        private Task CheckTokenAsync(OAuth20Token token, int id)
        {
            if (token == null) throw new UnauthorizedAccessException("Cannot create GoogleDrive session with given token");
            return InternalCheckTokenAsync(token, id);
        }

        private async Task InternalCheckTokenAsync(OAuth20Token token, int id)
        {
            if (token.IsExpired)
            {
                token = OAuth20TokenHelper.RefreshToken<GoogleLoginProvider>(ConsumerFactory, token);

                var dbDao = ServiceProvider.GetService<ProviderAccountDao>();
                var authData = new AuthData(token: token.ToJson());
                await dbDao.UpdateProviderInfoAsync(id, authData).ConfigureAwait(false);
            }
        }

        public void Dispose()
        {
            Storage?.Close();
        }
    }

    [Singletone]
    public class GoogleDriveProviderInfoHelper
    {
        private readonly TimeSpan CacheExpiration;
        private readonly ICache CacheEntry;
        private readonly ICache CacheChildFiles;
        private readonly ICache CacheChildFolders;
        private readonly ICacheNotify<GoogleDriveCacheItem> CacheNotify;

        public GoogleDriveProviderInfoHelper(ICacheNotify<GoogleDriveCacheItem> cacheNotify, ICache cache)
        {
            CacheExpiration = TimeSpan.FromMinutes(1);
            CacheEntry = cache;
            CacheChildFiles = cache;
            CacheChildFolders = cache;

            CacheNotify = cacheNotify;
            CacheNotify.Subscribe((i) =>
            {
                if (i.ResetEntry)
                {
                    CacheEntry.Remove("drive-" + i.Key);
                }
                if (i.ResetAll)
                {
                    CacheEntry.Remove(new Regex("^drive-" + i.Key + ".*"));
                    CacheChildFiles.Remove(new Regex("^drivef-" + i.Key + ".*"));
                    CacheChildFolders.Remove(new Regex("^drived-" + i.Key + ".*"));
                }
                if (i.ResetChilds)
                {
                    if (!i.ChildFolderExist || !i.ChildFolder)
                    {
                        CacheChildFiles.Remove("drivef-" + i.Key);
                    }
                    if (!i.ChildFolderExist || i.ChildFolder)
                    {
                        CacheChildFolders.Remove("drived-" + i.Key);
                    }
                }
            }, CacheNotifyAction.Remove);
        }

        internal async Task<DriveFile> GetDriveEntryAsync(GoogleDriveStorage storage, int id, string driveId)
        {
            var entry = CacheEntry.Get<DriveFile>("drive-" + id + "-" + driveId);
            if (entry == null)
            {
                entry = await storage.GetEntryAsync(driveId).ConfigureAwait(false);
                if (entry != null)
                    CacheEntry.Insert("drive-" + id + "-" + driveId, entry, DateTime.UtcNow.Add(CacheExpiration));
            }
            return entry;
        }

        internal async Task<List<DriveFile>> GetDriveEntriesAsync(GoogleDriveStorage storage, int id, string parentDriveId, bool? folder = null)
        {
            if (folder.HasValue)
            {
                if (folder.Value)
                {
                    var value = CacheChildFolders.Get<List<DriveFile>>("drived-" + id + "-" + parentDriveId);
                    if (value == null)
                    {
                        value = await storage.GetEntriesAsync(parentDriveId, true).ConfigureAwait(false);
                        if (value != null)
                            CacheChildFolders.Insert("drived-" + id + "-" + parentDriveId, value, DateTime.UtcNow.Add(CacheExpiration));
                    }
                    return value;
                }
                else
                {
                    var value = CacheChildFiles.Get<List<DriveFile>>("drivef-" + id + "-" + parentDriveId);
                    if (value == null)
                    {
                        value = await storage.GetEntriesAsync(parentDriveId, false).ConfigureAwait(false);
                        if (value != null)
                            CacheChildFiles.Insert("drivef-" + id + "-" + parentDriveId, value, DateTime.UtcNow.Add(CacheExpiration));
                    }
                    return value;
                }
            }

            if (CacheChildFiles.Get<List<DriveFile>>("drivef-" + id + "-" + parentDriveId) == null &&
                CacheChildFolders.Get<List<DriveFile>>("drived-" + id + "-" + parentDriveId) == null)
            {
                var entries = await storage.GetEntriesAsync(parentDriveId).ConfigureAwait(false);

                CacheChildFiles.Insert("drivef-" + id + "-" + parentDriveId, entries.Where(entry => entry.MimeType != GoogleLoginProvider.GoogleDriveMimeTypeFolder).ToList(), DateTime.UtcNow.Add(CacheExpiration));
                CacheChildFolders.Insert("drived-" + id + "-" + parentDriveId, entries.Where(entry => entry.MimeType == GoogleLoginProvider.GoogleDriveMimeTypeFolder).ToList(), DateTime.UtcNow.Add(CacheExpiration));

                return entries;
            }

            var folders = CacheChildFolders.Get<List<DriveFile>>("drived-" + id + "-" + parentDriveId);
            if (folders == null)
            {
                folders = await storage.GetEntriesAsync(parentDriveId, true).ConfigureAwait(false);
                CacheChildFolders.Insert("drived-" + id + "-" + parentDriveId, folders, DateTime.UtcNow.Add(CacheExpiration));
            }
            var files = CacheChildFiles.Get<List<DriveFile>>("drivef-" + id + "-" + parentDriveId);
            if (files == null)
            {
                files = await storage.GetEntriesAsync(parentDriveId, false).ConfigureAwait(false);
                CacheChildFiles.Insert("drivef-" + id + "-" + parentDriveId, files, DateTime.UtcNow.Add(CacheExpiration));
            }
            return folders.Concat(files).ToList();
        }

        internal async Task CacheResetAsync(DriveFile driveEntry, int id)
        {
            if (driveEntry != null)
            {
                await CacheNotify.PublishAsync(new GoogleDriveCacheItem { ResetEntry = true, Key = id + "-" + driveEntry.Id }, CacheNotifyAction.Remove).ConfigureAwait(false);
            }
        }

        internal async Task CacheResetAsync(string driveRootId, int id, string driveId = null, bool? childFolder = null)
        {
            var key = id + "-";
            if (driveId == null)
            {
                await CacheNotify.PublishAsync(new GoogleDriveCacheItem { ResetAll = true, Key = key }, CacheNotifyAction.Remove).ConfigureAwait(false);
            }
            else
            {
                if (driveId == driveRootId)
                {
                    driveId = "root";
                }
                key += driveId;

                await CacheNotify.PublishAsync(new GoogleDriveCacheItem { ResetEntry = true, ResetChilds = true, Key = key, ChildFolder = childFolder ?? false, ChildFolderExist = childFolder.HasValue }, CacheNotifyAction.Remove).ConfigureAwait(false);
            }
        }

        internal Task CacheResetChildsAsync(int id, string parentDriveId, bool? childFolder = null)
        {
            return CacheNotify.PublishAsync(new GoogleDriveCacheItem { ResetChilds = true, Key = id + "-" + parentDriveId, ChildFolder = childFolder ?? false, ChildFolderExist = childFolder.HasValue }, CacheNotifyAction.Remove);
        }
    }

    public static class GoogleDriveProviderInfoExtention
    {
        public static void Register(DIHelper dIHelper)
        {
            dIHelper.TryAdd<GoogleDriveStorage>();
        }
    }
}