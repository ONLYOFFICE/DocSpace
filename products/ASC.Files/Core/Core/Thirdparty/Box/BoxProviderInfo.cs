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

namespace ASC.Files.Thirdparty.Box;

[Transient]
[DebuggerDisplay("{CustomerTitle}")]
internal class BoxProviderInfo : IProviderInfo
{
    private readonly BoxProviderInfoHelper _boxProviderInfoHelper;
    private readonly BoxStorageDisposableWrapper _wrapper;
    private string _rootId;

    public BoxProviderInfo(
        BoxStorageDisposableWrapper wrapper,
        BoxProviderInfoHelper boxProviderInfoHelper)
    {
        _wrapper = wrapper;
        _boxProviderInfoHelper = boxProviderInfoHelper;
    }

    public string BoxRootId
    {
        get
        {
            if (string.IsNullOrEmpty(_rootId))
            {
                var storage = StorageAsync.Result;
                _rootId = storage.GetRootFolderIdAsync().Result;
            }

            return _rootId;
        }
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
    public string RootFolderId => "box-" + ID;
    public FolderType RootFolderType { get; set; }
    public OAuth20Token Token { get; set; }
    internal bool StorageOpened => _wrapper.TryGetStorage(ID, out var storage) && storage.IsOpened;

    internal Task<BoxStorage> StorageAsync
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

    public Task<bool> CheckAccessAsync()
    {
        try
        {
            return Task.FromResult(!string.IsNullOrEmpty(BoxRootId));
        }
        catch (UnauthorizedAccessException)
        {
            return Task.FromResult(false);
        }
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

    internal Task CacheResetAsync(BoxItem boxItem)
    {
        return _boxProviderInfoHelper.CacheResetAsync(ID, boxItem);
    }

    internal Task CacheResetAsync(string boxPath = null, bool? isFile = null)
    {
        return _boxProviderInfoHelper.CacheResetAsync(BoxRootId, ID, boxPath, isFile);
    }

    internal async ValueTask<BoxFile> GetBoxFileAsync(string dropboxFilePath)
    {
        var storage = await StorageAsync;

        return await _boxProviderInfoHelper.GetBoxFileAsync(storage, ID, dropboxFilePath);
    }

    internal async Task<BoxFolder> GetBoxFolderAsync(string dropboxFolderPath)
    {
        var storage = await StorageAsync;

        return await _boxProviderInfoHelper.GetBoxFolderAsync(storage, ID, dropboxFolderPath);
    }

    internal async Task<List<BoxItem>> GetBoxItemsAsync(string dropboxFolderPath)
    {
        var storage = await StorageAsync;

        return await _boxProviderInfoHelper.GetBoxItemsAsync(storage, ID, dropboxFolderPath);
    }

    internal async Task<Stream> GetThumbnailAsync(string boxFileId, int width, int height)
    {
        var storage = await StorageAsync;

        return await _boxProviderInfoHelper.GetThumbnailAsync(storage, boxFileId, width, height);
    }
}

[Transient]
internal class BoxStorageDisposableWrapper : IDisposable
{
    private readonly ConsumerFactory _consumerFactory;
    private readonly OAuth20TokenHelper _oAuth20TokenHelper;
    private readonly IServiceProvider _serviceProvider;
    private readonly TempStream _tempStream;
    private readonly ConcurrentDictionary<int, BoxStorage> _storages =
        new ConcurrentDictionary<int, BoxStorage>();

    public BoxStorageDisposableWrapper(ConsumerFactory consumerFactory, TempStream tempStream, IServiceProvider serviceProvider, OAuth20TokenHelper oAuth20TokenHelper)
    {
        _consumerFactory = consumerFactory;
        _tempStream = tempStream;
        _serviceProvider = serviceProvider;
        _oAuth20TokenHelper = oAuth20TokenHelper;
    }

    public void Dispose()
    {
        foreach (var (key, storage) in _storages)
        {
            storage.Close();
            _storages.Remove(key, out _);
        }
    }

    internal Task<BoxStorage> CreateStorageAsync(OAuth20Token token, int id)
    {
        if (TryGetStorage(id, out var storage) && storage.IsOpened)
        {
            return Task.FromResult(storage);
        }

        return InternalCreateStorageAsync(token, id);
    }

    internal bool TryGetStorage(int providerId, out BoxStorage storage)
    {
        return _storages.TryGetValue(providerId, out storage);
    }

    private Task CheckTokenAsync(OAuth20Token token, int id)
    {
        if (token == null)
        {
            throw new UnauthorizedAccessException("Cannot create Box session with given token");
        }

        return InternalCheckTokenAsync(token, id);
    }

    private async Task InternalCheckTokenAsync(OAuth20Token token, int id)
    {
        if (token.IsExpired)
        {
            token = _oAuth20TokenHelper.RefreshToken<BoxLoginProvider>(_consumerFactory, token);

            var dbDao = _serviceProvider.GetService<ProviderAccountDao>();
            await dbDao.UpdateProviderInfoAsync(id, new AuthData(token: token.ToJson()));
        }
    }

    private async Task<BoxStorage> InternalCreateStorageAsync(OAuth20Token token, int id)
    {
        var boxStorage = new BoxStorage(_tempStream);
        await CheckTokenAsync(token, id);

        boxStorage.Open(token);

        _storages.TryAdd(id, boxStorage);

        return boxStorage;
    }
}

[Singletone]
public class BoxProviderInfoHelper
{
    private readonly ICache _cacheChildItems;
    private readonly TimeSpan _cacheExpiration = TimeSpan.FromMinutes(1);
    private readonly ICache _cacheFile;
    private readonly ICache _cacheFolder;
    private readonly ICacheNotify<BoxCacheItem> _cacheNotify;

    public BoxProviderInfoHelper(ICacheNotify<BoxCacheItem> cacheNotify, ICache cache)
    {
        _cacheFile = cache;
        _cacheFolder = cache;
        _cacheChildItems = cache;
        _cacheNotify = cacheNotify;
        _cacheNotify.Subscribe((i) =>
        {
            if (i.ResetAll)
            {
                _cacheChildItems.Remove(new Regex("^box-" + i.Key + ".*"));
                _cacheFile.Remove(new Regex("^boxf-" + i.Key + ".*"));
                _cacheFolder.Remove(new Regex("^boxd-" + i.Key + ".*"));
            }

            if (!i.IsFileExists)
            {
                _cacheChildItems.Remove("box-" + i.Key);

                _cacheFolder.Remove("boxd-" + i.Key);
            }
            else
            {
                if (i.IsFileExists)
                {
                    _cacheFile.Remove("boxf-" + i.Key);
                }
                else
                {
                    _cacheFolder.Remove("boxd-" + i.Key);
                }
            }
        }, CacheNotifyAction.Remove);
    }

    internal async Task CacheResetAsync(int id, BoxItem boxItem)
    {
        if (boxItem != null)
        {
            await _cacheNotify.PublishAsync(new BoxCacheItem { IsFile = boxItem is BoxFile, Key = id + "-" + boxItem.Id }, CacheNotifyAction.Remove);
        }
    }

    internal async Task CacheResetAsync(string boxRootId, int id, string boxId = null, bool? isFile = null)
    {
        var key = id + "-";
        if (boxId == null)
        {
            await _cacheNotify.PublishAsync(new BoxCacheItem { ResetAll = true, Key = key }, CacheNotifyAction.Remove);
        }
        else
        {
            if (boxId == boxRootId)
            {
                boxId = "0";
            }

            key += boxId;

            await _cacheNotify.PublishAsync(new BoxCacheItem { IsFile = isFile ?? false, IsFileExists = isFile.HasValue, Key = key }, CacheNotifyAction.Remove);
        }
    }

    internal async ValueTask<BoxFile> GetBoxFileAsync(BoxStorage storage, int id, string boxFileId)
    {
        var file = _cacheFile.Get<BoxFile>("boxf-" + id + "-" + boxFileId);
        if (file == null)
        {
            file = await storage.GetFileAsync(boxFileId);
            if (file != null)
            {
                _cacheFile.Insert("boxf-" + id + "-" + boxFileId, file, DateTime.UtcNow.Add(_cacheExpiration));
            }
        }

        return file;
    }

    internal async Task<BoxFolder> GetBoxFolderAsync(BoxStorage storage, int id, string boxFolderId)
    {
        var folder = _cacheFolder.Get<BoxFolder>("boxd-" + id + "-" + boxFolderId);
        if (folder == null)
        {
            folder = await storage.GetFolderAsync(boxFolderId);
            if (folder != null)
            {
                _cacheFolder.Insert("boxd-" + id + "-" + boxFolderId, folder, DateTime.UtcNow.Add(_cacheExpiration));
            }
        }

        return folder;
    }

    internal async Task<List<BoxItem>> GetBoxItemsAsync(BoxStorage storage, int id, string boxFolderId)
    {
        var items = _cacheChildItems.Get<List<BoxItem>>("box-" + id + "-" + boxFolderId);

        if (items == null)
        {
            items = await storage.GetItemsAsync(boxFolderId);
            _cacheChildItems.Insert("box-" + id + "-" + boxFolderId, items, DateTime.UtcNow.Add(_cacheExpiration));
        }

        return items;
    }

    internal async Task<Stream> GetThumbnailAsync(BoxStorage storage, string boxFileId, int width, int height)
    {
        return await storage.GetThumbnailAsync(boxFileId, width, height);
    }
}