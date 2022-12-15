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

using DriveFile = Google.Apis.Drive.v3.Data.File;

namespace ASC.Files.Thirdparty.GoogleDrive;

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
            if (_wrapper.Storage == null || !_wrapper.Storage.IsOpened)
            {
                return _wrapper.CreateStorageAsync(Token, ID);
            }

            return Task.FromResult(_wrapper.Storage);
        }
    }

    internal bool StorageOpened => _wrapper.Storage != null && _wrapper.Storage.IsOpened;

    public int ID { get; set; }
    public Guid Owner { get; set; }
    public string CustomerTitle { get; set; }
    public DateTime CreateOn { get; set; }
    public string RootFolderId => "drive-" + ID;
    public string ProviderKey { get; set; }
    public FolderType RootFolderType { get; set; }
    public FolderType FolderType { get; set; }
    public string FolderId { get; set; }
    public bool Private { get; set; }
    public bool HasLogo { get; set; }
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
                    _logger.ErrorGoogleDrive(ex);

                    return null;
                }
            }

            return _driveRootId;
        }
    }

    private readonly GoogleDriveStorageDisposableWrapper _wrapper;
    private readonly GoogleDriveProviderInfoHelper _googleDriveProviderInfoHelper;
    private readonly ILogger _logger;

    public GoogleDriveProviderInfo(
        GoogleDriveStorageDisposableWrapper storageDisposableWrapper,
        GoogleDriveProviderInfoHelper googleDriveProviderInfoHelper,
        ILoggerProvider options)
    {
        _wrapper = storageDisposableWrapper;
        _googleDriveProviderInfoHelper = googleDriveProviderInfoHelper;
        _logger = options.CreateLogger("ASC.Files");
    }

    public void Dispose()
    {
        if (StorageOpened)
        {
            StorageAsync.Result.Close();
        }
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
        if (_wrapper != null)
        {
            _wrapper.Dispose();
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

        return await _googleDriveProviderInfoHelper.GetDriveEntryAsync(storage, ID, driveId);
    }

    internal async Task<List<DriveFile>> GetDriveEntriesAsync(string parentDriveId, bool? folder = null)
    {
        var storage = await StorageAsync;

        return await _googleDriveProviderInfoHelper.GetDriveEntriesAsync(storage, ID, parentDriveId, folder);
    }

    internal Task CacheResetAsync(DriveFile driveEntry)
    {
        return _googleDriveProviderInfoHelper.CacheResetAsync(driveEntry, ID);
    }

    internal Task CacheResetAsync(string driveId = null, bool? childFolder = null)
    {
        return _googleDriveProviderInfoHelper.CacheResetAsync(DriveRootId, ID, driveId, childFolder);
    }

    internal Task CacheResetChildsAsync(string parentDriveId, bool? childFolder = null)
    {
        return _googleDriveProviderInfoHelper.CacheResetChildsAsync(ID, parentDriveId, childFolder);
    }

    internal async Task<Stream> GetThumbnail(string fileId, int width, int height)
    {
        var storage = await StorageAsync;
        return await storage.GetThumbnail(fileId, width, height);
    }
}

[Scope(Additional = typeof(GoogleDriveProviderInfoExtention))]
internal class GoogleDriveStorageDisposableWrapper : IDisposable
{
    internal GoogleDriveStorage Storage { get; set; }

    private readonly OAuth20TokenHelper _oAuth20TokenHelper;
    private readonly ConsumerFactory _consumerFactory;
    private readonly IServiceProvider _serviceProvider;

    public GoogleDriveStorageDisposableWrapper(ConsumerFactory consumerFactory, IServiceProvider serviceProvider, OAuth20TokenHelper oAuth20TokenHelper)
    {
        _consumerFactory = consumerFactory;
        _serviceProvider = serviceProvider;
        _oAuth20TokenHelper = oAuth20TokenHelper;
    }

    public Task<GoogleDriveStorage> CreateStorageAsync(OAuth20Token token, int id)
    {
        if (Storage != null && Storage.IsOpened)
        {
            return Task.FromResult(Storage);
        }

        return InternalCreateStorageAsync(token, id);
    }

    public async Task<GoogleDriveStorage> InternalCreateStorageAsync(OAuth20Token token, int id)
    {
        var driveStorage = _serviceProvider.GetService<GoogleDriveStorage>();

        await CheckTokenAsync(token, id);

        driveStorage.Open(token);

        return Storage = driveStorage;
    }

    private Task CheckTokenAsync(OAuth20Token token, int id)
    {
        if (token == null)
        {
            throw new UnauthorizedAccessException("Cannot create GoogleDrive session with given token");
        }

        return InternalCheckTokenAsync(token, id);
    }

    private async Task InternalCheckTokenAsync(OAuth20Token token, int id)
    {
        if (token.IsExpired)
        {
            token = _oAuth20TokenHelper.RefreshToken<GoogleLoginProvider>(_consumerFactory, token);

            var dbDao = _serviceProvider.GetService<ProviderAccountDao>();
            var authData = new AuthData(token: token.ToJson());
            await dbDao.UpdateProviderInfoAsync(id, authData);
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
    private readonly TimeSpan _cacheExpiration;
    private readonly ICache _cacheEntry;
    private readonly ICache _cacheChildFiles;
    private readonly ICache _cacheChildFolders;
    private readonly ICacheNotify<GoogleDriveCacheItem> _cacheNotify;

    public GoogleDriveProviderInfoHelper(ICacheNotify<GoogleDriveCacheItem> cacheNotify, ICache cache)
    {
        _cacheExpiration = TimeSpan.FromMinutes(1);
        _cacheEntry = cache;
        _cacheChildFiles = cache;
        _cacheChildFolders = cache;

        _cacheNotify = cacheNotify;
        _cacheNotify.Subscribe((i) =>
        {
            if (i.ResetEntry)
            {
                _cacheEntry.Remove("drive-" + i.Key);
            }
            if (i.ResetAll)
            {
                _cacheEntry.Remove(new Regex("^drive-" + i.Key + ".*"));
                _cacheChildFiles.Remove(new Regex("^drivef-" + i.Key + ".*"));
                _cacheChildFolders.Remove(new Regex("^drived-" + i.Key + ".*"));
            }
            if (i.ResetChilds)
            {
                if (!i.ChildFolderExist || !i.ChildFolder)
                {
                    _cacheChildFiles.Remove("drivef-" + i.Key);
                }
                if (!i.ChildFolderExist || i.ChildFolder)
                {
                    _cacheChildFolders.Remove("drived-" + i.Key);
                }
            }
        }, CacheNotifyAction.Remove);
    }

    internal async Task<DriveFile> GetDriveEntryAsync(GoogleDriveStorage storage, int id, string driveId)
    {
        var entry = _cacheEntry.Get<DriveFile>("drive-" + id + "-" + driveId);
        if (entry == null)
        {
            entry = await storage.GetEntryAsync(driveId);
            if (entry != null)
            {
                _cacheEntry.Insert("drive-" + id + "-" + driveId, entry, DateTime.UtcNow.Add(_cacheExpiration));
            }
        }

        return entry;
    }

    internal async Task<List<DriveFile>> GetDriveEntriesAsync(GoogleDriveStorage storage, int id, string parentDriveId, bool? folder = null)
    {
        if (folder.HasValue)
        {
            if (folder.Value)
            {
                var value = _cacheChildFolders.Get<List<DriveFile>>("drived-" + id + "-" + parentDriveId);
                if (value == null)
                {
                    value = await storage.GetEntriesAsync(parentDriveId, true);
                    if (value != null)
                    {
                        _cacheChildFolders.Insert("drived-" + id + "-" + parentDriveId, value, DateTime.UtcNow.Add(_cacheExpiration));
                    }
                }

                return value;
            }
            else
            {
                var value = _cacheChildFiles.Get<List<DriveFile>>("drivef-" + id + "-" + parentDriveId);
                if (value == null)
                {
                    value = await storage.GetEntriesAsync(parentDriveId, false);
                    if (value != null)
                    {
                        _cacheChildFiles.Insert("drivef-" + id + "-" + parentDriveId, value, DateTime.UtcNow.Add(_cacheExpiration));
                    }
                }

                return value;
            }
        }

        if (_cacheChildFiles.Get<List<DriveFile>>("drivef-" + id + "-" + parentDriveId) == null &&
            _cacheChildFolders.Get<List<DriveFile>>("drived-" + id + "-" + parentDriveId) == null)
        {
            var entries = await storage.GetEntriesAsync(parentDriveId);

            _cacheChildFiles.Insert("drivef-" + id + "-" + parentDriveId, entries.Where(entry => entry.MimeType != GoogleLoginProvider.GoogleDriveMimeTypeFolder).ToList(), DateTime.UtcNow.Add(_cacheExpiration));
            _cacheChildFolders.Insert("drived-" + id + "-" + parentDriveId, entries.Where(entry => entry.MimeType == GoogleLoginProvider.GoogleDriveMimeTypeFolder).ToList(), DateTime.UtcNow.Add(_cacheExpiration));

            return entries;
        }

        var folders = _cacheChildFolders.Get<List<DriveFile>>("drived-" + id + "-" + parentDriveId);
        if (folders == null)
        {
            folders = await storage.GetEntriesAsync(parentDriveId, true);
            _cacheChildFolders.Insert("drived-" + id + "-" + parentDriveId, folders, DateTime.UtcNow.Add(_cacheExpiration));
        }
        var files = _cacheChildFiles.Get<List<DriveFile>>("drivef-" + id + "-" + parentDriveId);
        if (files == null)
        {
            files = await storage.GetEntriesAsync(parentDriveId, false);
            _cacheChildFiles.Insert("drivef-" + id + "-" + parentDriveId, files, DateTime.UtcNow.Add(_cacheExpiration));
        }

        return folders.Concat(files).ToList();
    }

    internal async Task CacheResetAsync(DriveFile driveEntry, int id)
    {
        if (driveEntry != null)
        {
            await _cacheNotify.PublishAsync(new GoogleDriveCacheItem { ResetEntry = true, Key = id + "-" + driveEntry.Id }, CacheNotifyAction.Remove);
        }
    }

    internal async Task CacheResetAsync(string driveRootId, int id, string driveId = null, bool? childFolder = null)
    {
        var key = id + "-";
        if (driveId == null)
        {
            await _cacheNotify.PublishAsync(new GoogleDriveCacheItem { ResetAll = true, Key = key }, CacheNotifyAction.Remove);
        }
        else
        {
            if (driveId == driveRootId)
            {
                driveId = "root";
            }

            key += driveId;

            await _cacheNotify.PublishAsync(new GoogleDriveCacheItem { ResetEntry = true, ResetChilds = true, Key = key, ChildFolder = childFolder ?? false, ChildFolderExist = childFolder.HasValue }, CacheNotifyAction.Remove);
        }
    }

    internal Task CacheResetChildsAsync(int id, string parentDriveId, bool? childFolder = null)
    {
        return _cacheNotify.PublishAsync(new GoogleDriveCacheItem { ResetChilds = true, Key = id + "-" + parentDriveId, ChildFolder = childFolder ?? false, ChildFolderExist = childFolder.HasValue }, CacheNotifyAction.Remove);
    }
}

public static class GoogleDriveProviderInfoExtention
{
    public static void Register(DIHelper dIHelper)
    {
        dIHelper.TryAdd<GoogleDriveStorage>();
    }
}
