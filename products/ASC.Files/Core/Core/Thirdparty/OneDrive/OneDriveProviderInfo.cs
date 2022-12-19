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

namespace ASC.Files.Thirdparty.OneDrive;

[Transient]
[DebuggerDisplay("{CustomerTitle}")]
internal class OneDriveProviderInfo : IProviderInfo
{
    public OAuth20Token Token { get; set; }

    internal Task<OneDriveStorage> StorageAsync
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
    public string RootFolderId => "onedrive-" + ID;
    public string ProviderKey { get; set; }
    public FolderType RootFolderType { get; set; }
    public FolderType FolderType { get; set; }
    public string FolderId { get; set; }
    public bool Private { get; set; }
    public bool HasLogo { get; set; }

    private readonly OneDriveStorageDisposableWrapper _wrapper;
    private readonly OneDriveProviderInfoHelper _oneDriveProviderInfoHelper;

    public OneDriveProviderInfo(
        OneDriveStorageDisposableWrapper wrapper,
        OneDriveProviderInfoHelper oneDriveProviderInfoHelper)
    {
        _wrapper = wrapper;
        _oneDriveProviderInfoHelper = oneDriveProviderInfoHelper;
    }

    public void Dispose()
    {
        if (StorageOpened)
        {
            StorageAsync.Result.Close();
        }
    }

    public async Task<bool> CheckAccessAsync()
    {
        try
        {
            var storage = await StorageAsync;

            return await storage.CheckAccessAsync();
        }
        catch (AggregateException)
        {
            return false;
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

    internal async Task<Item> GetOneDriveItemAsync(string itemId)
    {
        var storage = await StorageAsync;

        return await _oneDriveProviderInfoHelper.GetOneDriveItemAsync(storage, ID, itemId);
    }

    internal async Task<List<Item>> GetOneDriveItemsAsync(string onedriveFolderId)
    {
        var storage = await StorageAsync;

        return await _oneDriveProviderInfoHelper.GetOneDriveItemsAsync(storage, ID, onedriveFolderId);
    }

    internal Task CacheResetAsync(string onedriveId = null)
    {
        return _oneDriveProviderInfoHelper.CacheResetAsync(ID, onedriveId);
    }

    internal async Task<Stream> GetThumbnailAsync(string onedriveId, int width, int height)
    {
        var storage = await StorageAsync;
        return await _oneDriveProviderInfoHelper.GetThumbnailAsync(storage, onedriveId, width, height);
    }
}

[Scope(Additional = typeof(OneDriveProviderInfoExtention))]
internal class OneDriveStorageDisposableWrapper : IDisposable
{
    internal OneDriveStorage Storage { get; private set; }
    private readonly OAuth20TokenHelper _oAuth20TokenHelper;

    internal readonly ConsumerFactory ConsumerFactory;
    internal readonly IServiceProvider ServiceProvider;

    public OneDriveStorageDisposableWrapper(ConsumerFactory consumerFactory, IServiceProvider serviceProvider, OAuth20TokenHelper oAuth20TokenHelper)
    {
        ConsumerFactory = consumerFactory;
        ServiceProvider = serviceProvider;
        _oAuth20TokenHelper = oAuth20TokenHelper;
    }

    public Task<OneDriveStorage> CreateStorageAsync(OAuth20Token token, int id)
    {
        if (Storage != null && Storage.IsOpened)
        {
            return Task.FromResult(Storage);
        }

        return InternalCreateStorageAsync(token, id);
    }

    private async Task<OneDriveStorage> InternalCreateStorageAsync(OAuth20Token token, int id)
    {
        var onedriveStorage = ServiceProvider.GetService<OneDriveStorage>();

        await CheckTokenAsync(token, id);

        onedriveStorage.Open(token);

        return Storage = onedriveStorage;
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
            token = _oAuth20TokenHelper.RefreshToken<OneDriveLoginProvider>(ConsumerFactory, token);

            var dbDao = ServiceProvider.GetService<ProviderAccountDao>();
            var authData = new AuthData(token: token.ToJson());
            await dbDao.UpdateProviderInfoAsync(id, authData);
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
    private readonly TimeSpan _cacheExpiration;
    private readonly ICache _cacheItem;
    private readonly ICache _cacheChildItems;
    private readonly ICacheNotify<OneDriveCacheItem> _cacheNotify;

    public OneDriveProviderInfoHelper(ICacheNotify<OneDriveCacheItem> cacheNotify, ICache cache)
    {
        _cacheExpiration = TimeSpan.FromMinutes(1);
        _cacheItem = cache;
        _cacheChildItems = cache;

        _cacheNotify = cacheNotify;
        _cacheNotify.Subscribe((i) =>
        {
            ResetMemoryCache(i);
            if (i.ResetAll)
            {
                _cacheChildItems.Remove(new Regex("^onedrivei-" + i.Key + ".*"));
                _cacheItem.Remove(new Regex("^onedrive-" + i.Key + ".*"));
            }
            else
            {
                _cacheChildItems.Remove(new Regex("onedrivei-" + i.Key));
                _cacheItem.Remove("onedrive-" + i.Key);
            }
        }, CacheNotifyAction.Remove);
    }

    internal async Task<Item> GetOneDriveItemAsync(OneDriveStorage storage, int id, string itemId)
    {
        var file = _cacheItem.Get<Item>("onedrive-" + id + "-" + itemId);
        if (file == null)
        {
            file = await storage.GetItemAsync(itemId);
            if (file != null)
            {
                _cacheItem.Insert("onedrive-" + id + "-" + itemId, file, DateTime.UtcNow.Add(_cacheExpiration));
            }
        }

        return file;
    }

    internal async Task<List<Item>> GetOneDriveItemsAsync(OneDriveStorage storage, int id, string onedriveFolderId)
    {
        var items = _cacheChildItems.Get<List<Item>>("onedrivei-" + id + "-" + onedriveFolderId);

        if (items == null)
        {
            items = await storage.GetItemsAsync(onedriveFolderId);
            _cacheChildItems.Insert("onedrivei-" + id + "-" + onedriveFolderId, items, DateTime.UtcNow.Add(_cacheExpiration));
        }

        return items;
    }

    internal async Task CacheResetAsync(int id, string onedriveId = null)
    {
        var key = id + "-";
        var item = new OneDriveCacheItem { Key = key };

        if (string.IsNullOrEmpty(onedriveId))
        {
            item.ResetAll = true;
        }
        else
        {
            item.Key += onedriveId;
        }

        ResetMemoryCache(item);
        await _cacheNotify.PublishAsync(item, CacheNotifyAction.Remove);
    }

    private void ResetMemoryCache(OneDriveCacheItem i)
    {
        if (i.ResetAll)
        {
            _cacheChildItems.Remove(new Regex("^onedrivei-" + i.Key + ".*"));
            _cacheItem.Remove(new Regex("^onedrive-" + i.Key + ".*"));
        }
        else
        {
            _cacheChildItems.Remove(new Regex("onedrivei-" + i.Key));
            _cacheItem.Remove("onedrive-" + i.Key);
        }
    }

    internal async Task<Stream> GetThumbnailAsync(OneDriveStorage storage, string onedriveId, int width, int height)
    {
        return await storage.GetThumbnailAsync(onedriveId, width, height);
    }
}
public static class OneDriveProviderInfoExtention
{
    public static void Register(DIHelper dIHelper)
    {
        dIHelper.TryAdd<OneDriveStorage>();
    }
}
