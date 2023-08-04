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

namespace ASC.Web.Core.Users;

[Transient]
public sealed class ResizeWorkerItem : DistributedTask
{
    public ResizeWorkerItem()
    {

    }

    public ResizeWorkerItem(int tenantId, Guid userId, byte[] data, long maxFileSize, Size size, IDataStore dataStore, UserPhotoThumbnailSettings settings) : base()
    {
        TenantId = tenantId;
        UserId = userId;
        Data = data;
        MaxFileSize = maxFileSize;
        Size = size;
        DataStore = dataStore;
        Settings = settings;
    }

    public Size Size { get; }

    public IDataStore DataStore { get; }

    public long MaxFileSize { get; }

    public byte[] Data { get; }
    public int TenantId { get; }
    public Guid UserId { get; }

    public UserPhotoThumbnailSettings Settings { get; }

    public override bool Equals(object obj)
    {
        if (obj is null)
        {
            return false;
        }

        if (ReferenceEquals(this, obj))
        {
            return true;
        }

        if (obj is not ResizeWorkerItem)
        {
            return false;
        }

        return Equals((ResizeWorkerItem)obj);
    }

    public bool Equals(ResizeWorkerItem other)
    {
        if (other is null)
        {
            return false;
        }

        if (ReferenceEquals(this, other))
        {
            return true;
        }

        return other.UserId.Equals(UserId) && other.MaxFileSize == MaxFileSize && other.Size.Equals(Size);
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(UserId, MaxFileSize, Size);
    }
}

[Singletone]
public class UserPhotoManagerCache
{
    private readonly ConcurrentDictionary<Guid, ConcurrentDictionary<CacheSize, string>> _photofiles;
    private readonly ICacheNotify<UserPhotoManagerCacheItem> _cacheNotify;
    private readonly HashSet<int> _tenantDiskCache;
    private readonly IServiceScopeFactory _serviceScopeFactory;

    public UserPhotoManagerCache(ICacheNotify<UserPhotoManagerCacheItem> notify, IServiceScopeFactory serviceScopeFactory)
    {
        try
        {
            _photofiles = new ConcurrentDictionary<Guid, ConcurrentDictionary<CacheSize, string>>();
            _tenantDiskCache = new HashSet<int>();
            _cacheNotify = notify;

            _cacheNotify.Subscribe((data) =>
            {
                _photofiles.GetOrAdd(new Guid(data.UserId), (r) => new ConcurrentDictionary<CacheSize, string>())
                              .AddOrUpdate(data.Size, data.FileName, (key, oldValue) => data.FileName);
            }, CacheNotifyAction.InsertOrUpdate);

            _cacheNotify.Subscribe((data) =>
            {
                if (_photofiles.TryRemove(new Guid(data.UserId), out _))
                {

                }

            }, CacheNotifyAction.Remove);
        }
        catch (Exception)
        {

        }
        _serviceScopeFactory = serviceScopeFactory;
    }

    public bool IsCacheLoadedForTenant(int tenantId)
    {
        return _tenantDiskCache.Contains(tenantId);
    }

    public bool SetCacheLoadedForTenant(bool isLoaded, int tenantId)
    {
        return isLoaded ? _tenantDiskCache.Add(tenantId) : _tenantDiskCache.Remove(tenantId);
    }

    public void ClearCache(Guid userID, int tenantId)
    {
        if (_cacheNotify != null)
        {
            _cacheNotify.Publish(new UserPhotoManagerCacheItem { UserId = userID.ToString(), TenantId = tenantId }, CacheNotifyAction.Remove);
        }
    }

    public void AddToCache(Guid userID, Size size, string fileName, int tenantId)
    {
        if (_cacheNotify != null)
        {
            _cacheNotify.Publish(new UserPhotoManagerCacheItem { UserId = userID.ToString(), Size = UserPhotoManager.ToCache(size), FileName = fileName, TenantId = tenantId }, CacheNotifyAction.InsertOrUpdate);
        }
    }

    public string SearchInCache(Guid userId, Size size)
    {
        string fileName = null;

        if (!_photofiles.TryGetValue(userId, out var val))
        {
            return null;
        }

        if (size != Size.Empty && !val.TryGetValue(UserPhotoManager.ToCache(size), out fileName))
        {
            return null;
        }
        else if (String.IsNullOrEmpty(fileName))
        {
            fileName = val.Values.FirstOrDefault(x => !string.IsNullOrEmpty(x) && x.Contains("_orig_"));
        }

        return fileName;
    }
}

[Scope(Additional = typeof(ResizeWorkerItemExtension))]
public class UserPhotoManager
{
    public const string CUSTOM_DISTRIBUTED_TASK_QUEUE_NAME = "user_photo_manager";

    //Regex for parsing filenames into groups with id's
    private static readonly Regex _parseFile =
            new Regex(@"^(?'module'\{{0,1}([0-9a-fA-F]){8}-([0-9a-fA-F]){4}-([0-9a-fA-F]){4}-([0-9a-fA-F]){4}-([0-9a-fA-F]){12}\}{0,1}){0,1}" +
                @"(?'user'\{{0,1}([0-9a-fA-F]){8}-([0-9a-fA-F]){4}-([0-9a-fA-F]){4}-([0-9a-fA-F]){4}-([0-9a-fA-F]){12}\}{0,1}){1}" +
                @"_(?'kind'orig|size){1}_(?'size'(?'width'[0-9]{1,5})-{1}(?'height'[0-9]{1,5})){0,1}\..*", RegexOptions.Compiled);

    private readonly UserManager _userManager;
    private readonly WebImageSupplier _webImageSupplier;
    private readonly TenantManager _tenantManager;
    private readonly StorageFactory _storageFactory;
    private readonly UserPhotoManagerCache _userPhotoManagerCache;
    private readonly SettingsManager _settingsManager;
    private readonly ILogger<UserPhotoManager> _log;

    //note: using auto stop queue
    private readonly DistributedTaskQueue _resizeQueue;//TODO: configure

    public UserPhotoManager(
        UserManager userManager,
        WebImageSupplier webImageSupplier,
        TenantManager tenantManager,
        StorageFactory storageFactory,
        UserPhotoManagerCache userPhotoManagerCache,
        ILogger<UserPhotoManager> logger,
        IDistributedTaskQueueFactory queueFactory,
        SettingsManager settingsManager)
    {
        _resizeQueue = queueFactory.CreateQueue(CUSTOM_DISTRIBUTED_TASK_QUEUE_NAME);
        _userManager = userManager;
        _webImageSupplier = webImageSupplier;
        _tenantManager = tenantManager;
        _storageFactory = storageFactory;
        _userPhotoManagerCache = userPhotoManagerCache;
        _settingsManager = settingsManager;
        _log = logger;
    }

    private string _defaultAbsoluteWebPath;
    public string GetDefaultPhotoAbsoluteWebPath()
    {
        return _defaultAbsoluteWebPath ??= _webImageSupplier.GetAbsoluteWebPath(_defaultAvatar);
    }

    public async Task<string> GetRetinaPhotoURL(Guid userID)
    {
        return await GetSizedPhotoAbsoluteWebPath(userID, RetinaFotoSize);
    }

    public async Task<string> GetMaxPhotoURL(Guid userID)
    {
        return await GetSizedPhotoAbsoluteWebPath(userID, MaxFotoSize);
    }

    public async Task<string> GetBigPhotoURL(Guid userID)
    {
        return await GetSizedPhotoAbsoluteWebPath(userID, BigFotoSize);
    }

    public async Task<string> GetMediumPhotoURL(Guid userID)
    {
        return await GetSizedPhotoAbsoluteWebPath(userID, MediumFotoSize);
    }

    public async Task<string> GetSmallPhotoURL(Guid userID)
    {
        return await GetSizedPhotoAbsoluteWebPath(userID, SmallFotoSize);
    }


    public async Task<string> GetSizedPhotoUrl(Guid userId, int width, int height)
    {
        return await GetSizedPhotoAbsoluteWebPath(userId, new Size(width, height));
    }


    private string _defaultSmallPhotoURL;
    public string GetDefaultSmallPhotoURL()
    {
        return _defaultSmallPhotoURL ??= GetDefaultPhotoAbsoluteWebPath(SmallFotoSize);
    }

    private string _defaultMediumPhotoURL;
    public string GetDefaultMediumPhotoURL()
    {
        return _defaultMediumPhotoURL ??= GetDefaultPhotoAbsoluteWebPath(MediumFotoSize);
    }

    private string _defaultBigPhotoURL;
    public string GetDefaultBigPhotoURL()
    {
        return _defaultBigPhotoURL ??= GetDefaultPhotoAbsoluteWebPath(BigFotoSize);
    }

    private string _defaultMaxPhotoURL;
    public string GetDefaultMaxPhotoURL()
    {
        return _defaultMaxPhotoURL ??= GetDefaultPhotoAbsoluteWebPath(MaxFotoSize);
    }

    private string _defaultRetinaPhotoURL;
    public string GetDefaultRetinaPhotoURL()
    {
        return _defaultRetinaPhotoURL ??= GetDefaultPhotoAbsoluteWebPath(RetinaFotoSize);
    }

    public static Size OriginalFotoSize { get; } = new Size(1280, 1280);

    public static Size RetinaFotoSize { get; } = new Size(360, 360);

    public static Size MaxFotoSize { get; } = new Size(200, 200);

    public static Size BigFotoSize { get; } = new Size(82, 82);

    public static Size MediumFotoSize { get; } = new Size(48, 48);

    public static Size SmallFotoSize { get; } = new Size(32, 32);

    private static readonly string _defaultRetinaAvatar = "default_user_photo_size_360-360.png";
    private static readonly string _defaultAvatar = "default_user_photo_size_200-200.png";
    private static readonly string _defaultSmallAvatar = "default_user_photo_size_32-32.png";
    private static readonly string _defaultMediumAvatar = "default_user_photo_size_48-48.png";
    private static readonly string _defaultBigAvatar = "default_user_photo_size_82-82.png";
    private static readonly string _tempDomainName = "temp";


    public async Task<bool> UserHasAvatar(Guid userID)
    {
        var path = await GetPhotoAbsoluteWebPath(userID);
        var fileName = Path.GetFileName(path);
        return fileName != _defaultAvatar;
    }

    public async Task<string> GetPhotoAbsoluteWebPath(Guid userID)
    {
        var path = await SearchInCache(userID, Size.Empty);
        if (!string.IsNullOrEmpty(path))
        {
            return path;
        }

        try
        {
            var data = await _userManager.GetUserPhotoAsync(userID);
            string photoUrl;
            string fileName;
            if (data == null || data.Length == 0)
            {
                photoUrl = GetDefaultPhotoAbsoluteWebPath();
                fileName = "default";
            }
            else
            {
                (photoUrl, fileName) = await SaveOrUpdatePhotoAsync(userID, data, -1, new Size(-1, -1), false);
            }

            _userPhotoManagerCache.AddToCache(userID, Size.Empty, fileName, _tenantManager.GetCurrentTenant().Id);

            return photoUrl;
        }
        catch
        {
        }
        return GetDefaultPhotoAbsoluteWebPath();
    }

    internal async Task<Size> GetPhotoSize(Guid userID)
    {
        var virtualPath = await GetPhotoAbsoluteWebPath(userID);
        if (virtualPath == null)
        {
            return Size.Empty;
        }

        try
        {
            var sizePart = virtualPath.Substring(virtualPath.LastIndexOf('_'));
            sizePart = sizePart.Trim('_');
            sizePart = sizePart.Remove(sizePart.LastIndexOf('.'));
            return new Size(int.Parse(sizePart.Split('-')[0]), int.Parse(sizePart.Split('-')[1]));
        }
        catch
        {
            return Size.Empty;
        }
    }

    private async Task<string> GetSizedPhotoAbsoluteWebPath(Guid userID, Size size)
    {
        var res = await SearchInCache(userID, size);
        if (!string.IsNullOrEmpty(res))
        {
            return res;
        }

        try
        {
            var data = await _userManager.GetUserPhotoAsync(userID);

            if (data == null || data.Length == 0)
            {
                //empty photo. cache default
                var photoUrl = GetDefaultPhotoAbsoluteWebPath(size);

                _userPhotoManagerCache.AddToCache(userID, size, "default", _tenantManager.GetCurrentTenant().Id);

                return photoUrl;
            }

            //Enqueue for sizing
            await SizePhoto(userID, data, -1, size);
        }
        catch { }

        return GetDefaultPhotoAbsoluteWebPath(size);
    }

    private string GetDefaultPhotoAbsoluteWebPath(Size size)
    {
        return size switch
        {
            Size(var w, var h) when w == RetinaFotoSize.Width && h == RetinaFotoSize.Height => _webImageSupplier.GetAbsoluteWebPath(_defaultRetinaAvatar),
            Size(var w, var h) when w == MaxFotoSize.Width && h == MaxFotoSize.Height => _webImageSupplier.GetAbsoluteWebPath(_defaultAvatar),
            Size(var w, var h) when w == BigFotoSize.Width && h == BigFotoSize.Height => _webImageSupplier.GetAbsoluteWebPath(_defaultBigAvatar),
            Size(var w, var h) when w == SmallFotoSize.Width && h == SmallFotoSize.Height => _webImageSupplier.GetAbsoluteWebPath(_defaultSmallAvatar),
            Size(var w, var h) when w == MediumFotoSize.Width && h == MediumFotoSize.Height => _webImageSupplier.GetAbsoluteWebPath(_defaultMediumAvatar),
            _ => GetDefaultPhotoAbsoluteWebPath()
        };
    }

    private static readonly SemaphoreSlim _semaphore = new SemaphoreSlim(1);


    private async Task<string> SearchInCache(Guid userId, Size size)
    {
        if (!_userPhotoManagerCache.IsCacheLoadedForTenant(_tenantManager.GetCurrentTenant().Id))
        {
            await LoadDiskCache();
        }

        var fileName = _userPhotoManagerCache.SearchInCache(userId, size);

        if (fileName != null && fileName.StartsWith("default"))
        {
            return GetDefaultPhotoAbsoluteWebPath(size);
        }

        if (!string.IsNullOrEmpty(fileName))
        {
            var store = await GetDataStoreAsync();
            var uri = await store.GetUriAsync(fileName);

            return uri.ToString();
        }

        return null;
    }

    private async Task LoadDiskCache()
    {
        await _semaphore.WaitAsync();
        if (!_userPhotoManagerCache.IsCacheLoadedForTenant(_tenantManager.GetCurrentTenant().Id))
        {
            try
            {
                var listFileNames = await (await GetDataStoreAsync()).ListFilesRelativeAsync("", "", "*.*", false).ToArrayAsync();
                foreach (var fileName in listFileNames)
                {
                    //Try parse fileName
                    if (fileName != null)
                    {
                        var match = _parseFile.Match(fileName);
                        if (match.Success && match.Groups["user"].Success)
                        {
                            var parsedUserId = new Guid(match.Groups["user"].Value);
                            var size = Size.Empty;
                            if (match.Groups["width"].Success && match.Groups["height"].Success)
                            {
                                //Parse size
                                size = new Size(int.Parse(match.Groups["width"].Value), int.Parse(match.Groups["height"].Value));
                            }
                            _userPhotoManagerCache.AddToCache(parsedUserId, size, fileName, _tenantManager.GetCurrentTenant().Id);
                        }
                    }
                }
                _userPhotoManagerCache.SetCacheLoadedForTenant(true, _tenantManager.GetCurrentTenant().Id);
            }
            catch (Exception err)
            {
                _log.ErrorLoadDiskCache(err);
            }
        }
        _semaphore.Release();
    }
    public async Task ResetThumbnailSettingsAsync(Guid userId)
    {
        var thumbSettings = _settingsManager.GetDefault<UserPhotoThumbnailSettings>();
        await _settingsManager.SaveAsync(thumbSettings, userId);
    }

    public async Task<(string, string)> SaveOrUpdatePhoto(Guid userID, byte[] data)
    {
        return await SaveOrUpdatePhotoAsync(userID, data, -1, OriginalFotoSize, true);
    }

    public async Task RemovePhotoAsync(Guid idUser)
    {
        await _userManager.SaveUserPhotoAsync(idUser, null);
        try
        {
            var storage = await GetDataStoreAsync();
            await storage.DeleteFilesAsync("", idUser + "*.*", false);
        }
        catch (AggregateException e)
        {
            if (e.InnerException is DirectoryNotFoundException exc)
            {
                _log.ErrorRemovePhoto(exc);
            }
            else
            {
                throw;
            }
        }
        catch (DirectoryNotFoundException e)
        {
            _log.ErrorRemovePhoto(e);
        }

        await _userManager.SaveUserPhotoAsync(idUser, null);
        _userPhotoManagerCache.ClearCache(idUser, await _tenantManager.GetCurrentTenantIdAsync());
    }

    public async Task SyncPhotoAsync(Guid userID, byte[] data)
    {
        data = TryParseImage(data, -1, OriginalFotoSize, out _, out var width, out var height);
        await _userManager.SaveUserPhotoAsync(userID, data);
        await SetUserPhotoThumbnailSettingsAsync(userID, width, height);
        //   _userPhotoManagerCache.ClearCache(userID, _tenantManager.GetCurrentTenant().Id);
    }


    private async Task<(string, string)> SaveOrUpdatePhotoAsync(Guid userID, byte[] data, long maxFileSize, Size size, bool saveInCoreContext)
    {
        data = TryParseImage(data, maxFileSize, size, out var imgFormat, out var width, out var height);

        var widening = CommonPhotoManager.GetImgFormatName(imgFormat);
        var fileName = string.Format("{0}_orig_{1}-{2}.{3}", userID, width, height, widening);

        if (saveInCoreContext)
        {
            await _userManager.SaveUserPhotoAsync(userID, data);
            await SetUserPhotoThumbnailSettingsAsync(userID, width, height);
            // _userPhotoManagerCache.ClearCache(userID, _tenantManager.GetCurrentTenant().Id);
        }

        var store = await GetDataStoreAsync();

        var photoUrl = GetDefaultPhotoAbsoluteWebPath();
        if (data != null && data.Length > 0)
        {
            using (var stream = new MemoryStream(data))
            {
                photoUrl = (await store.SaveAsync(fileName, stream)).ToString();
            }
            //Queue resizing
            var t1 = SizePhoto(userID, data, -1, SmallFotoSize, true);
            var t2 = SizePhoto(userID, data, -1, MediumFotoSize, true);
            var t3 = SizePhoto(userID, data, -1, BigFotoSize, true);
            var t4 = SizePhoto(userID, data, -1, MaxFotoSize, true);
            var t5 = SizePhoto(userID, data, -1, RetinaFotoSize, true);

            await Task.WhenAll(t1, t2, t3, t4, t5);
        }
        return (photoUrl, fileName);
    }

    private async Task SetUserPhotoThumbnailSettingsAsync(Guid userId, int width, int height)
    {
        var settings = await _settingsManager.LoadAsync<UserPhotoThumbnailSettings>(userId);

        if (!settings.IsDefault)
        {
            return;
        }

        var max = Math.Max(Math.Max(width, height), SmallFotoSize.Width);
        var min = Math.Max(Math.Min(width, height), SmallFotoSize.Width);

        var pos = (max - min) / 2;

        settings = new UserPhotoThumbnailSettings(
            width >= height ? new Point(pos, 0) : new Point(0, pos),
            new Size(min, min));

        await _settingsManager.SaveAsync(settings, userId);
    }

    private byte[] TryParseImage(byte[] data, long maxFileSize, Size maxsize, out IImageFormat imgFormat, out int width, out int height)
    {
        if (data == null || data.Length <= 0)
        {
            throw new UnknownImageFormatException();
        }

        if (maxFileSize != -1 && data.Length > maxFileSize)
        {
            throw new ImageSizeLimitException();
        }

        //data = ImageHelper.RotateImageByExifOrientationData(data, Log);

        try
        {
            using var img = Image.Load(data);

            imgFormat = img.Metadata.DecodedImageFormat;
            width = img.Width;
            height = img.Height;
            var maxWidth = maxsize.Width;
            var maxHeight = maxsize.Height;

            if ((maxHeight != -1 && img.Height > maxHeight) || (maxWidth != -1 && img.Width > maxWidth))
            {
                #region calulate height and width

                if (width > maxWidth && height > maxHeight)
                {

                    if (width > height)
                    {
                        height = (int)(height * (double)maxWidth / width + 0.5);
                        width = maxWidth;
                    }
                    else
                    {
                        width = (int)(width * (double)maxHeight / height + 0.5);
                        height = maxHeight;
                    }
                }

                if (width > maxWidth && height <= maxHeight)
                {
                    height = (int)(height * (double)maxWidth / width + 0.5);
                    width = maxWidth;
                }

                if (width <= maxWidth && height > maxHeight)
                {
                    width = (int)(width * (double)maxHeight / height + 0.5);
                    height = maxHeight;
                }

                var tmpW = width;
                var tmpH = height;
                #endregion
                using var destRound = img.Clone(x => x.Resize(new ResizeOptions
                {
                    Size = new Size(tmpW, tmpH),
                    Mode = ResizeMode.Stretch
                }));

                data = CommonPhotoManager.SaveToBytes(destRound);
            }
            return data;
        }
        catch (OutOfMemoryException)
        {
            throw new ImageSizeLimitException();
        }
        catch (ArgumentException error)
        {
            throw new UnknownImageFormatException(error);
        }
    }

    private async Task<string> SizePhoto(Guid userID, byte[] data, long maxFileSize, Size size)
    {
        return await SizePhoto(userID, data, maxFileSize, size, false);
    }

    private async Task<string> SizePhoto(Guid userID, byte[] data, long maxFileSize, Size size, bool now)
    {
        if (data == null || data.Length <= 0)
        {
            throw new UnknownImageFormatException();
        }

        if (maxFileSize != -1 && data.Length > maxFileSize)
        {
            throw new ImageWeightLimitException();
        }

        var resizeTask = new ResizeWorkerItem(await _tenantManager.GetCurrentTenantIdAsync(), userID, data, maxFileSize, size, await GetDataStoreAsync(), await _settingsManager.LoadAsync<UserPhotoThumbnailSettings>(userID));
        var key = $"{userID}{size}";
        resizeTask["key"] = key;

        if (now)
        {
            //Resize synchronously
            await ResizeImage(resizeTask);
            return await GetSizedPhotoAbsoluteWebPath(userID, size);
        }
        else
        {
            if (!_resizeQueue.GetAllTasks<ResizeWorkerItem>().Any(r => r["key"] == key))
            {
                //Add
                _resizeQueue.EnqueueTask(async (a, b) => await ResizeImage(resizeTask), resizeTask);
            }
            return GetDefaultPhotoAbsoluteWebPath(size);
            //NOTE: return default photo here. Since task will update cache
        }
    }

    private async Task ResizeImage(ResizeWorkerItem item)
    {
        try
        {
            await _tenantManager.SetCurrentTenantAsync(item.TenantId);

            var data = item.Data;
            using var stream = new MemoryStream(data);
            using var img = Image.Load(stream);
            var imgFormat = img.Metadata.DecodedImageFormat;

            if (item.Size != img.Size)
            {
                using var img2 = item.Settings.IsDefault ?
                    CommonPhotoManager.DoThumbnail(img, item.Size, true, true, true) :
                    UserPhotoThumbnailManager.GetImage(img, item.Size, item.Settings);
                data = CommonPhotoManager.SaveToBytes(img2);
            }
            else
            {
                data = CommonPhotoManager.SaveToBytes(img);
            }

            var widening = CommonPhotoManager.GetImgFormatName(imgFormat);
            var fileName = string.Format("{0}_size_{1}-{2}.{3}", item.UserId, item.Size.Width, item.Size.Height, widening);

            using var stream2 = new MemoryStream(data);
            await item.DataStore.SaveAsync(fileName, stream2);

            _userPhotoManagerCache.AddToCache(item.UserId, item.Size, fileName, _tenantManager.GetCurrentTenant().Id);
        }
        catch (ArgumentException error)
        {
            throw new UnknownImageFormatException(error);
        }
    }

    public async Task<string> GetTempPhotoAbsoluteWebPath(string fileName)
    {
        return (await (await GetDataStoreAsync()).GetUriAsync(_tempDomainName, fileName)).ToString();
    }

    public async Task<string> SaveTempPhoto(byte[] data, long maxFileSize, int maxWidth, int maxHeight)
    {
        data = TryParseImage(data, maxFileSize, new Size(maxWidth, maxHeight), out var imgFormat, out _, out _);

        var fileName = Guid.NewGuid() + "." + CommonPhotoManager.GetImgFormatName(imgFormat);

        var store = await GetDataStoreAsync();
        using var stream = new MemoryStream(data);
        return (await store.SaveAsync(_tempDomainName, fileName, stream)).ToString();
    }

    public async Task<string> SaveTempPhoto(byte[] data, long maxFileSize, string ext)
    {
        if (maxFileSize != -1 && data.Length > maxFileSize)
        {
            throw new ImageSizeLimitException();
        }

        using var stream = new MemoryStream(data);
        var fileName = Guid.NewGuid() + $".{ext}";
        var store = await GetDataStoreAsync();
        return (await store.SaveAsync(_tempDomainName, fileName, stream)).ToString();
    }

    public async Task<byte[]> GetTempPhotoData(string fileName)
    {
        await using var s = await (await GetDataStoreAsync()).GetReadStreamAsync(_tempDomainName, fileName);
        var data = new MemoryStream();
        var buffer = new byte[1024 * 10];

        while (true)
        {
            var count = await s.ReadAsync(buffer, 0, buffer.Length);
            if (count == 0)
            {
                break;
            }

            await data.WriteAsync(buffer, 0, count);
        }

        return data.ToArray();
    }

    public async Task<string> GetSizedTempPhotoAbsoluteWebPathAsync(string fileName, int newWidth, int newHeight)
    {
        var store = await GetDataStoreAsync();
        if (await store.IsFileAsync(_tempDomainName, fileName))
        {
            await using var s = await store.GetReadStreamAsync(_tempDomainName, fileName);
            using var img = Image.Load(s);

            var imgFormat = img.Metadata.DecodedImageFormat;

            byte[] data;

            if (img.Width != newWidth || img.Height != newHeight)
            {
                using var img2 = CommonPhotoManager.DoThumbnail(img, new Size(newWidth, newHeight), true, true, true);
                data = CommonPhotoManager.SaveToBytes(img2);
            }
            else
            {
                data = CommonPhotoManager.SaveToBytes(img);
            }
            var widening = CommonPhotoManager.GetImgFormatName(imgFormat);
            var index = fileName.LastIndexOf('.');
            var fileNameWithoutExt = (index != -1) ? fileName.Substring(0, index) : fileName;

            var trueFileName = fileNameWithoutExt + "_size_" + newWidth.ToString() + "-" + newHeight.ToString() + "." + widening;
            using var stream = new MemoryStream(data);
            return (await store.SaveAsync(_tempDomainName, trueFileName, stream)).ToString();
        }

        return GetDefaultPhotoAbsoluteWebPath(new Size(newWidth, newHeight));
    }

    public async Task RemoveTempPhotoAsync(string fileName)
    {
        var index = fileName.LastIndexOf('.');
        var fileNameWithoutExt = (index != -1) ? fileName.Substring(0, index) : fileName;
        try
        {
            var store = await GetDataStoreAsync();
            await store.DeleteFilesAsync(_tempDomainName, "", fileNameWithoutExt + "*.*", false);
        }
        catch { }
    }


    public async Task<(Image, IImageFormat)> GetPhotoImageAsync(Guid userID)
    {
        IImageFormat format;
        try
        {
            var data = await _userManager.GetUserPhotoAsync(userID);
            if (data != null)
            {
                var img = Image.Load(data);

                format = img.Metadata.DecodedImageFormat;

                return (img, format);
            }
        }
        catch { }
        format = null;
        return (null, format);
    }

    public async Task<string> SaveThumbnail(Guid userID, Image img, IImageFormat format)
    {
        var moduleID = Guid.Empty;
        var widening = CommonPhotoManager.GetImgFormatName(format);
        var size = img.Size;
        var fileName = string.Format("{0}{1}_size_{2}-{3}.{4}", moduleID == Guid.Empty ? "" : moduleID.ToString(), userID, img.Width, img.Height, widening);

        var store = await GetDataStoreAsync();
        string photoUrl;
        using (var s = new MemoryStream(CommonPhotoManager.SaveToBytes(img)))
        {
            img.Dispose();
            photoUrl = (await store.SaveAsync(fileName, s)).ToString();
        }

        _userPhotoManagerCache.AddToCache(userID, size, fileName, _tenantManager.GetCurrentTenant().Id);
        return photoUrl;
    }

    public async Task<byte[]> GetUserPhotoData(Guid userId, Size size)
    {
        try
        {
            var pattern = string.Format("{0}_size_{1}-{2}.*", userId, size.Width, size.Height);

            var fileName = await (await GetDataStoreAsync()).ListFilesRelativeAsync("", "", pattern, false).FirstOrDefaultAsync();

            if (string.IsNullOrEmpty(fileName))
            {
                return null;
            }

            await using var s = await (await GetDataStoreAsync()).GetReadStreamAsync("", fileName);
            var data = new MemoryStream();
            var buffer = new byte[1024 * 10];
            while (true)
            {
                var count = s.Read(buffer, 0, buffer.Length);
                if (count == 0)
                {
                    break;
                }

                data.Write(buffer, 0, count);
            }
            return data.ToArray();
        }
        catch (Exception err)
        {
            _log.ErrorGetUserPhotoData(err);
            return null;
        }
    }

    private IDataStore _dataStore;
    private async ValueTask<IDataStore> GetDataStoreAsync()
    {
        return _dataStore ??= await _storageFactory.GetStorageAsync(await _tenantManager.GetCurrentTenantIdAsync(), "userPhotos");
    }

    public static CacheSize ToCache(Size size)
    {
        return size switch
        {
            Size(var w, var h) when w == RetinaFotoSize.Width && h == RetinaFotoSize.Height => CacheSize.Retina,
            Size(var w, var h) when w == MaxFotoSize.Width && h == MaxFotoSize.Height => CacheSize.Max,
            Size(var w, var h) when w == BigFotoSize.Width && h == BigFotoSize.Height => CacheSize.Big,
            Size(var w, var h) when w == SmallFotoSize.Width && h == SmallFotoSize.Height => CacheSize.Small,
            Size(var w, var h) when w == MediumFotoSize.Width && h == MediumFotoSize.Height => CacheSize.Medium,
            _ => CacheSize.Original
        };
    }
}

#region Exception Classes

public class UnknownImageFormatException : Exception
{
    public UnknownImageFormatException() : base("unknown image file type") { }

    public UnknownImageFormatException(Exception inner) : base("unknown image file type", inner) { }
}

public class ImageWeightLimitException : Exception
{
    public ImageWeightLimitException() : base("image width is too large") { }
}

public class ImageSizeLimitException : Exception
{
    public ImageSizeLimitException() : base("image size is too large") { }
}

#endregion


/// <summary>
/// Helper class for manipulating images.
/// </summary>
/*public static class ImageHelper
{
    /// <summary>
    /// Rotate the given image byte array according to Exif Orientation data
    /// </summary>
    /// <param name="data">source image byte array</param>
    /// <param name="updateExifData">set it to TRUE to update image Exif data after rotation (default is TRUE)</param>
    /// <returns>The rotated image byte array. If no rotation occurred, source data will be returned.</returns>
    public static byte[] RotateImageByExifOrientationData(byte[] data, ILog Log, bool updateExifData = true)
    {
        try
        {
            using var stream = new MemoryStream(data);
            using var img = Image.Load(stream);

            var fType = RotateImageByExifOrientationData(img, updateExifData);
            if (fType != RotateFlipType.RotateNoneFlipNone)
            {
                using var tempStream = new MemoryStream();
                img.Save(tempStream, System.Drawing.Imaging.ImageFormat.Png);
                data = tempStream.ToArray();
            }
        }
        catch (Exception err)
        {
            Log.Error(err);
        }

        return data;
    }

    /// <summary>
    /// Rotate the given image file according to Exif Orientation data
    /// </summary>
    /// <param name="sourceFilePath">path of source file</param>
    /// <param name="targetFilePath">path of target file</param>
    /// <param name="targetFormat">target format</param>
    /// <param name="updateExifData">set it to TRUE to update image Exif data after rotation (default is TRUE)</param>
    /// <returns>The RotateFlipType value corresponding to the applied rotation. If no rotation occurred, RotateFlipType.RotateNoneFlipNone will be returned.</returns>
    public static RotateFlipType RotateImageByExifOrientationData(string sourceFilePath, string targetFilePath, ImageFormat targetFormat, bool updateExifData = true)
    {
        // Rotate the image according to EXIF data
        using var bmp = new Bitmap(sourceFilePath);
        var fType = RotateImageByExifOrientationData(bmp, updateExifData);
        if (fType != RotateFlipType.RotateNoneFlipNone)
        {
            bmp.Save(targetFilePath, targetFormat);
        }
        return fType;
    }

    /// <summary>
    /// Rotate the given bitmap according to Exif Orientation data
    /// </summary>
    /// <param name="img">source image</param>
    /// <param name="updateExifData">set it to TRUE to update image Exif data after rotation (default is TRUE)</param>
    /// <returns>The RotateFlipType value corresponding to the applied rotation. If no rotation occurred, RotateFlipType.RotateNoneFlipNone will be returned.</returns>
    public static RotateFlipType RotateImageByExifOrientationData(Image img, bool updateExifData = true)
    {
        const int orientationId = 0x0112;
        var fType = RotateFlipType.RotateNoneFlipNone;
        if (img.PropertyIdList.Contains(orientationId))
        {
            var pItem = img.GetPropertyItem(orientationId);
            fType = GetRotateFlipTypeByExifOrientationData(pItem.Value[0]);
            if (fType != RotateFlipType.RotateNoneFlipNone)
            {
                img.RotateFlip(fType);
                if (updateExifData) img.RemovePropertyItem(orientationId); // Remove Exif orientation tag
            }
        }
        return fType;
    }

    /// <summary>
    /// Return the proper System.Drawing.RotateFlipType according to given orientation EXIF metadata
    /// </summary>
    /// <param name="orientation">Exif "Orientation"</param>
    /// <returns>the corresponding System.Drawing.RotateFlipType enum value</returns>
    public static RotateFlipType GetRotateFlipTypeByExifOrientationData(int orientation)
    {
        return orientation switch
        {
            1 => RotateFlipType.RotateNoneFlipNone,
            2 => RotateFlipType.RotateNoneFlipX,
            3 => RotateFlipType.Rotate180FlipNone,
            4 => RotateFlipType.Rotate180FlipX,
            5 => RotateFlipType.Rotate90FlipX,
            6 => RotateFlipType.Rotate90FlipNone,
            7 => RotateFlipType.Rotate270FlipX,
            8 => RotateFlipType.Rotate270FlipNone,
            _ => RotateFlipType.RotateNoneFlipNone,
        };
    }
}*/

public static class SizeExtend
{
    public static void Deconstruct(this Size size, out int w, out int h)
    {
        (w, h) = (size.Width, size.Height);
    }
}

public static class ResizeWorkerItemExtension
{
    public static void Register(DIHelper services)
    {
        services.TryAdd<ResizeWorkerItem>();
        services.Configure<DistributedTaskQueueFactoryOptions>(UserPhotoManager.CUSTOM_DISTRIBUTED_TASK_QUEUE_NAME, options =>
        {
            options.MaxThreadsCount = 2;
        });
    }
}
