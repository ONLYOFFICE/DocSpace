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

namespace ASC.Files.Thirdparty.Dropbox;

[Singletone]
public class DropboxProviderInfoHelper
{
    private readonly ICache _cacheChildItems;
    private readonly TimeSpan _cacheExpiration;
    private readonly ICache _cacheFile;
    private readonly ICache _cacheFolder;
    private readonly ICacheNotify<DropboxCacheItem> _cacheNotify;

    public DropboxProviderInfoHelper(ICacheNotify<DropboxCacheItem> cacheNotify, ICache cache)
    {
        _cacheExpiration = TimeSpan.FromMinutes(1);
        _cacheFile = cache;
        _cacheFolder = cache;
        _cacheChildItems = cache;
        _cacheNotify = cacheNotify;

        _cacheNotify.Subscribe((i) =>
        {
            if (i.ResetAll)
            {
                _cacheFile.Remove(new Regex("^dropboxf-" + i.Key + ".*"));
                _cacheFolder.Remove(new Regex("^dropboxd-" + i.Key + ".*"));
                _cacheChildItems.Remove(new Regex("^dropbox-" + i.Key + ".*"));
            }

            if (!i.IsFileExists)
            {
                _cacheChildItems.Remove("dropbox-" + i.Key);

                _cacheFolder.Remove("dropboxd-" + i.Key);
            }
            else
            {
                if (i.IsFileExists)
                {
                    _cacheFile.Remove("dropboxf-" + i.Key);
                }
                else
                {
                    _cacheFolder.Remove("dropboxd-" + i.Key);
                }
            }
        }, CacheNotifyAction.Remove);
    }

    internal async Task CacheResetAsync(int id, Metadata dropboxItem)
    {
        if (dropboxItem != null)
        {
            await _cacheNotify.PublishAsync(new DropboxCacheItem { IsFile = dropboxItem.AsFolder != null, Key = id + "-" + dropboxItem.PathDisplay }, CacheNotifyAction.Remove);
        }
    }

    internal async Task CacheResetAsync(int id, string dropboxPath = null, bool? isFile = null)
    {
        var key = id + "-";
        if (dropboxPath == null)
        {
            await _cacheNotify.PublishAsync(new DropboxCacheItem { ResetAll = true, Key = key }, CacheNotifyAction.Remove);
        }
        else
        {
            key += dropboxPath;

            await _cacheNotify.PublishAsync(new DropboxCacheItem { IsFile = isFile ?? false, IsFileExists = isFile.HasValue, Key = key }, CacheNotifyAction.Remove);
        }
    }

    internal async ValueTask<FileMetadata> GetDropboxFileAsync(DropboxStorage storage, int id, string dropboxFilePath)
    {
        var file = _cacheFile.Get<FileMetadata>("dropboxf-" + id + "-" + dropboxFilePath);
        if (file == null)
        {
            file = await storage.GetFileAsync(dropboxFilePath);
            if (file != null)
            {
                _cacheFile.Insert("dropboxf-" + id + "-" + dropboxFilePath, file, DateTime.UtcNow.Add(_cacheExpiration));
            }
        }

        return file;
    }

    internal async Task<FolderMetadata> GetDropboxFolderAsync(DropboxStorage storage, int id, string dropboxFolderPath)
    {
        var folder = _cacheFolder.Get<FolderMetadata>("dropboxd-" + id + "-" + dropboxFolderPath);
        if (folder == null)
        {
            folder = await storage.GetFolderAsync(dropboxFolderPath);
            if (folder != null)
            {
                _cacheFolder.Insert("dropboxd-" + id + "-" + dropboxFolderPath, folder, DateTime.UtcNow.Add(_cacheExpiration));
            }
        }

        return folder;
    }
    internal async Task<List<Metadata>> GetDropboxItemsAsync(DropboxStorage storage, int id, string dropboxFolderPath)
    {
        var items = _cacheChildItems.Get<List<Metadata>>("dropbox-" + id + "-" + dropboxFolderPath);

        if (items == null)
        {
            items = await storage.GetItemsAsync(dropboxFolderPath);
            _cacheChildItems.Insert("dropbox-" + id + "-" + dropboxFolderPath, items, DateTime.UtcNow.Add(_cacheExpiration));
        }

        return items;
    }
    internal Task<Stream> GetThumbnailsAsync(DropboxStorage storage, string filePath, int width, int height)
    {
        return storage.GetThumbnailsAsync(filePath, width, height);
    }
}

[Transient]
[DebuggerDisplay("{CustomerTitle}")]
internal class DropboxProviderInfo : IProviderInfo
{
    private readonly DropboxProviderInfoHelper _dropboxProviderInfoHelper;
    private readonly DropboxStorageDisposableWrapper _wrapper;

    public DropboxProviderInfo(
        DropboxStorageDisposableWrapper wrapper,
        DropboxProviderInfoHelper dropboxProviderInfoHelper
        )
    {
        _wrapper = wrapper;
        _dropboxProviderInfoHelper = dropboxProviderInfoHelper;
    }

    public DateTime CreateOn { get; set; }
    public string CustomerTitle { get; set; }
    public string FolderId { get; set; }
    public FolderType FolderType { get; set; }
    public bool HasLogo { get; set; }
    public int ID { get; set; }
    public Guid Owner { get; set; }
    public bool Private { get; set; }
    public string ProviderKey { get; set; }
    public string RootFolderId => "dropbox-" + ID;
    public FolderType RootFolderType { get; set; }
    public OAuth20Token Token { get; set; }
    internal bool StorageOpened => _wrapper.TryGetStorage(ID, out var storage) && storage.IsOpened;

    internal Task<DropboxStorage> StorageAsync
    {
        get
        {
            if (!_wrapper.TryGetStorage(ID, out var storage) || !storage.IsOpened)
            {
                return _wrapper.CreateStorageAsync(Token, ID);
            }

            return Task.FromResult(storage);
        }
    }

    public async Task<bool> CheckAccessAsync()
    {
        try
        {
            await (await StorageAsync).GetUsedSpaceAsync();
        }
        catch (AggregateException)
        {
            return false;
        }

        return true;
    }

    public void Dispose()
    {
        if (StorageOpened)
        {
            StorageAsync.Result.Close();
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

    internal Task CacheResetAsync(Metadata dropboxItem)
    {
        return _dropboxProviderInfoHelper.CacheResetAsync(ID, dropboxItem);
    }

    internal Task CacheResetAsync(string dropboxPath = null, bool? isFile = null)
    {
        return _dropboxProviderInfoHelper.CacheResetAsync(ID, dropboxPath, isFile);
    }

    internal async Task<FileMetadata> GetDropboxFileAsync(string dropboxFilePath)
    {
        var storage = await StorageAsync;
        return await _dropboxProviderInfoHelper.GetDropboxFileAsync(storage, ID, dropboxFilePath);
    }

    internal async Task<FolderMetadata> GetDropboxFolderAsync(string dropboxFolderPath)
    {
        var storage = await StorageAsync;
        return await _dropboxProviderInfoHelper.GetDropboxFolderAsync(storage, ID, dropboxFolderPath);
    }

    internal async Task<List<Metadata>> GetDropboxItemsAsync(string dropboxFolderPath)
    {
        var storage = await StorageAsync;
        return await _dropboxProviderInfoHelper.GetDropboxItemsAsync(storage, ID, dropboxFolderPath);
    }

    internal async Task<Stream> GetThumbnailsAsync(string filePath, int width, int height)
    {
        var storage = await StorageAsync;
        return await _dropboxProviderInfoHelper.GetThumbnailsAsync(storage, filePath, width, height);
    }
}

[Scope(Additional = typeof(DropboxStorageDisposableWrapperExtention))]
internal class DropboxStorageDisposableWrapper : IDisposable
{
    private readonly ConsumerFactory _consumerFactory;
    private readonly OAuth20TokenHelper _oAuth20TokenHelper;
    private readonly IServiceProvider _serviceProvider;
    private readonly ConcurrentDictionary<int, DropboxStorage> _storages =
        new ConcurrentDictionary<int, DropboxStorage>();

    public DropboxStorageDisposableWrapper(IServiceProvider serviceProvider, OAuth20TokenHelper oAuth20TokenHelper, ConsumerFactory consumerFactory)
    {
        _serviceProvider = serviceProvider;
        _oAuth20TokenHelper = oAuth20TokenHelper;
        _consumerFactory = consumerFactory;
    }

    public Task<DropboxStorage> CreateStorageAsync(OAuth20Token token, int id)
    {
        if (TryGetStorage(id, out var storage) && storage.IsOpened)
        {
            return Task.FromResult(storage);
        }

        return InternalCreateStorageAsync(token, id);
    }

    public bool TryGetStorage(int id, out DropboxStorage storage)
    {
        return _storages.TryGetValue(id, out storage);
    }

    public void Dispose()
    {
        foreach (var (key, storage) in _storages)
        {
            storage.Close();
            _storages.Remove(key, out _);
        }
    }

    public async Task<DropboxStorage> InternalCreateStorageAsync(OAuth20Token token, int id)
    {
        var dropboxStorage = ActivatorUtilities.CreateInstance<DropboxStorage>(_serviceProvider);

        await CheckTokenAsync(token, id);

        dropboxStorage.Open(token);

        _storages.TryAdd(id, dropboxStorage);

        return dropboxStorage;
    }

    private Task CheckTokenAsync(OAuth20Token token, int id)
    {
        if (token == null)
        {
            throw new UnauthorizedAccessException("Cannot create Dropbox session with given token");
        }

        return InternalCheckTokenAsync(token, id);
    }

    private async Task InternalCheckTokenAsync(OAuth20Token token, int id)
    {
        if (token.IsExpired)
        {
            token = _oAuth20TokenHelper.RefreshToken<DropboxLoginProvider>(_consumerFactory, token);
            var dbDao = _serviceProvider.GetService<ProviderAccountDao>();
            var authData = new AuthData(token: token.ToJson());
            await dbDao.UpdateProviderInfoAsync(id, authData);
        }
    }
}

public static class DropboxStorageDisposableWrapperExtention
{
    public static void Register(DIHelper dIHelper)
    {
        dIHelper.TryAdd<DropboxStorage>();
    }
}