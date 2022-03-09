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

namespace ASC.Files.Thirdparty.Dropbox;

[Transient]
[DebuggerDisplay("{CustomerTitle}")]
internal class DropboxProviderInfo : IProviderInfo
{
    public OAuth20Token Token { get; set; }

    internal DropboxStorage Storage
    {
        get
        {
            if (_wrapper.Storage == null || !_wrapper.Storage.IsOpened)
            {
                return _wrapper.CreateStorage(Token);
            }

            return _wrapper.Storage;
        }
    }

    internal bool StorageOpened => _wrapper.Storage != null && _wrapper.Storage.IsOpened;
    public int ID { get; set; }
    public Guid Owner { get; set; }
    public string CustomerTitle { get; set; }
    public DateTime CreateOn { get; set; }
    public string RootFolderId => "dropbox-" + ID;
    public string ProviderKey { get; set; }
    public FolderType RootFolderType { get; set; }

    private readonly DropboxStorageDisposableWrapper _wrapper;
    private readonly DropboxProviderInfoHelper _dropboxProviderInfoHelper;

    public DropboxProviderInfo(
        DropboxStorageDisposableWrapper wrapper,
        DropboxProviderInfoHelper dropboxProviderInfoHelper
        )
    {
        _wrapper = wrapper;
        _dropboxProviderInfoHelper = dropboxProviderInfoHelper;
    }

    public void Dispose()
    {
        if (StorageOpened)
        {
            Storage.Close();
        }
    }

    public async Task<bool> CheckAccessAsync()
    {
        try
        {
            await Storage.GetUsedSpaceAsync().ConfigureAwait(false);
        }
        catch (AggregateException)
        {
            return false;
        }

        return true;
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

    internal Task<FolderMetadata> GetDropboxFolderAsync(string dropboxFolderPath)
    {
        return _dropboxProviderInfoHelper.GetDropboxFolderAsync(Storage, ID, dropboxFolderPath);
    }

    internal ValueTask<FileMetadata> GetDropboxFileAsync(string dropboxFilePath)
    {
        return _dropboxProviderInfoHelper.GetDropboxFileAsync(Storage, ID, dropboxFilePath);
    }

    internal Task<List<Metadata>> GetDropboxItemsAsync(string dropboxFolderPath)
    {
        return _dropboxProviderInfoHelper.GetDropboxItemsAsync(Storage, ID, dropboxFolderPath);
    }

    internal Task CacheResetAsync(Metadata dropboxItem)
    {
        return _dropboxProviderInfoHelper.CacheResetAsync(ID, dropboxItem);
    }

    internal Task CacheResetAsync(string dropboxPath = null, bool? isFile = null)
    {
        return _dropboxProviderInfoHelper.CacheResetAsync(ID, dropboxPath, isFile);
    }
}

[Scope]
internal class DropboxStorageDisposableWrapper : IDisposable
{
    public DropboxStorage Storage { get; private set; }
    private readonly TempStream _tempStream;

    public DropboxStorageDisposableWrapper(TempStream tempStream)
    {
        _tempStream = tempStream;
    }

    public DropboxStorage CreateStorage(OAuth20Token token)
    {
        if (Storage != null && Storage.IsOpened)
        {
            return Storage;
        }

        var dropboxStorage = new DropboxStorage(_tempStream);
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
    private readonly TimeSpan _cacheExpiration;
    private readonly ICache _cacheFile;
    private readonly ICache _cacheFolder;
    private readonly ICache _cacheChildItems;
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

    internal async Task<FolderMetadata> GetDropboxFolderAsync(DropboxStorage storage, int id, string dropboxFolderPath)
    {
        var folder = _cacheFolder.Get<FolderMetadata>("dropboxd-" + id + "-" + dropboxFolderPath);
        if (folder == null)
        {
            folder = await storage.GetFolderAsync(dropboxFolderPath).ConfigureAwait(false);
            if (folder != null)
            {
                _cacheFolder.Insert("dropboxd-" + id + "-" + dropboxFolderPath, folder, DateTime.UtcNow.Add(_cacheExpiration));
            }
        }

        return folder;
    }

    internal async ValueTask<FileMetadata> GetDropboxFileAsync(DropboxStorage storage, int id, string dropboxFilePath)
    {
        var file = _cacheFile.Get<FileMetadata>("dropboxf-" + id + "-" + dropboxFilePath);
        if (file == null)
        {
            file = await storage.GetFileAsync(dropboxFilePath).ConfigureAwait(false);
            if (file != null)
            {
                _cacheFile.Insert("dropboxf-" + id + "-" + dropboxFilePath, file, DateTime.UtcNow.Add(_cacheExpiration));
            }
        }

        return file;
    }

    internal async Task<List<Metadata>> GetDropboxItemsAsync(DropboxStorage storage, int id, string dropboxFolderPath)
    {
        var items = _cacheChildItems.Get<List<Metadata>>("dropbox-" + id + "-" + dropboxFolderPath);

        if (items == null)
        {
            items = await storage.GetItemsAsync(dropboxFolderPath).ConfigureAwait(false);
            _cacheChildItems.Insert("dropbox-" + id + "-" + dropboxFolderPath, items, DateTime.UtcNow.Add(_cacheExpiration));
        }

        return items;
    }

    internal async Task CacheResetAsync(int id, Metadata dropboxItem)
    {
        if (dropboxItem != null)
        {
            await _cacheNotify.PublishAsync(new DropboxCacheItem { IsFile = dropboxItem.AsFolder != null, Key = id + "-" + dropboxItem.PathDisplay }, CacheNotifyAction.Remove).ConfigureAwait(false);
        }
    }

    internal async Task CacheResetAsync(int id, string dropboxPath = null, bool? isFile = null)
    {
        var key = id + "-";
        if (dropboxPath == null)
        {
            await _cacheNotify.PublishAsync(new DropboxCacheItem { ResetAll = true, Key = key }, CacheNotifyAction.Remove).ConfigureAwait(false);
        }
        else
        {
            key += dropboxPath;

            await _cacheNotify.PublishAsync(new DropboxCacheItem { IsFile = isFile ?? false, IsFileExists = isFile.HasValue, Key = key }, CacheNotifyAction.Remove).ConfigureAwait(false);
        }
    }
}
