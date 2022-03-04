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
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;

using ASC.Common;
using ASC.Common.Caching;
using ASC.Common.Logging;
using ASC.Common.Threading;
using ASC.Core;
using ASC.Core.Common.Settings;
using ASC.Core.Tenants;
using ASC.Data.Storage;
using ASC.Web.Core.Utility.Skins;

using Microsoft.Extensions.Options;

using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Formats;
using SixLabors.ImageSharp.Processing;

namespace ASC.Web.Core.Users
{
    [Transient]
    public sealed class ResizeWorkerItem : DistributedTask
    {
        public ResizeWorkerItem()
        {

        }

        public ResizeWorkerItem(Guid userId, byte[] data, long maxFileSize, Size size, IDataStore dataStore, UserPhotoThumbnailSettings settings) : base()
        {
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

        public Guid UserId { get; }

        public UserPhotoThumbnailSettings Settings { get; }

        public override bool Equals(object obj)
        {
            if (obj is null) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (!(obj is ResizeWorkerItem)) return false;
            return Equals((ResizeWorkerItem)obj);
        }

        public bool Equals(ResizeWorkerItem other)
        {
            if (other is null) return false;
            if (ReferenceEquals(this, other)) return true;
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
        private readonly ConcurrentDictionary<CacheSize, ConcurrentDictionary<Guid, string>> Photofiles;
        private readonly ICacheNotify<UserPhotoManagerCacheItem> CacheNotify;
        private readonly HashSet<int> TenantDiskCache;

        public UserPhotoManagerCache(ICacheNotify<UserPhotoManagerCacheItem> notify)
        {
            try
            {
                Photofiles = new ConcurrentDictionary<CacheSize, ConcurrentDictionary<Guid, string>>();
                TenantDiskCache = new HashSet<int>();
                CacheNotify = notify;

                CacheNotify.Subscribe((data) =>
                {
                    var userId = new Guid(data.UserId);
                    Photofiles.GetOrAdd(data.Size, (r) => new ConcurrentDictionary<Guid, string>())[userId] = data.FileName;
                }, CacheNotifyAction.InsertOrUpdate);

                CacheNotify.Subscribe((data) =>
                {
                    var userId = new Guid(data.UserId);

                    try
                    {
                        foreach (var s in (CacheSize[])Enum.GetValues(typeof(CacheSize)))
                        {
                            Photofiles.TryGetValue(s, out var dict);
                            dict?.TryRemove(userId, out _);
                        }
                        SetCacheLoadedForTenant(false, data.TenantId);
                    }
                    catch { }
                }, CacheNotifyAction.Remove);
            }
            catch (Exception)
            {

            }
        }

        public bool IsCacheLoadedForTenant(int tenantId)
        {
            return TenantDiskCache.Contains(tenantId);
        }

        public bool SetCacheLoadedForTenant(bool isLoaded, int tenantId)
        {
            return isLoaded ? TenantDiskCache.Add(tenantId) : TenantDiskCache.Remove(tenantId);
        }

        public void ClearCache(Guid userID, int tenantId)
        {
            if (CacheNotify != null)
            {
                CacheNotify.Publish(new UserPhotoManagerCacheItem { UserId = userID.ToString(), TenantId = tenantId }, CacheNotifyAction.Remove);
            }
        }

        public void AddToCache(Guid userID, Size size, string fileName, int tenantId)
        {
            if (CacheNotify != null)
            {
                CacheNotify.Publish(new UserPhotoManagerCacheItem { UserId = userID.ToString(), Size = UserPhotoManager.ToCache(size), FileName = fileName, TenantId = tenantId }, CacheNotifyAction.InsertOrUpdate);
            }
        }

        public string SearchInCache(Guid userId, Size size)
        {
            string fileName = null;
            Photofiles.TryGetValue(UserPhotoManager.ToCache(size), out var photo);

            if (size != Size.Empty)
            {
                photo?.TryGetValue(userId, out fileName);
            }
            else
            {
                fileName = (photo?.FirstOrDefault(x => x.Key == userId && !string.IsNullOrEmpty(x.Value) && x.Value.Contains("_orig_")))?.Value;
            }

            return fileName;
        }
    }

    [Scope(Additional = typeof(ResizeWorkerItemExtension))]
    public class UserPhotoManager
    {
        //Regex for parsing filenames into groups with id's
        private static readonly Regex ParseFile =
                new Regex(@"^(?'module'\{{0,1}([0-9a-fA-F]){8}-([0-9a-fA-F]){4}-([0-9a-fA-F]){4}-([0-9a-fA-F]){4}-([0-9a-fA-F]){12}\}{0,1}){0,1}" +
                    @"(?'user'\{{0,1}([0-9a-fA-F]){8}-([0-9a-fA-F]){4}-([0-9a-fA-F]){4}-([0-9a-fA-F]){4}-([0-9a-fA-F]){12}\}{0,1}){1}" +
                    @"_(?'kind'orig|size){1}_(?'size'(?'width'[0-9]{1,5})-{1}(?'height'[0-9]{1,5})){0,1}\..*", RegexOptions.Compiled);

        private UserManager UserManager { get; }
        private WebImageSupplier WebImageSupplier { get; }
        private TenantManager TenantManager { get; }
        private StorageFactory StorageFactory { get; }
        private UserPhotoManagerCache UserPhotoManagerCache { get; }
        private SettingsManager SettingsManager { get; }
        private IServiceProvider ServiceProvider { get; }
        public ILog Log { get; }

        private Tenant tenant;
        public Tenant Tenant { get { return tenant ??= TenantManager.GetCurrentTenant(); } }

        //note: using auto stop queue
        private readonly DistributedTaskQueue ResizeQueue;//TODO: configure

        public UserPhotoManager(
            UserManager userManager,
            WebImageSupplier webImageSupplier,
            TenantManager tenantManager,
            StorageFactory storageFactory,
            UserPhotoManagerCache userPhotoManagerCache,
            IOptionsMonitor<ILog> options,
            DistributedTaskQueueOptionsManager optionsQueue,
            SettingsManager settingsManager,
            IServiceProvider serviceProvider)
        {
            ResizeQueue = optionsQueue.Get<ResizeWorkerItem>();
            UserManager = userManager;
            WebImageSupplier = webImageSupplier;
            TenantManager = tenantManager;
            StorageFactory = storageFactory;
            UserPhotoManagerCache = userPhotoManagerCache;
            SettingsManager = settingsManager;
            ServiceProvider = serviceProvider;
            Log = options.Get("ASC.Web.Photo");
        }

        private string _defaultAbsoluteWebPath;
        public string GetDefaultPhotoAbsoluteWebPath()
        {
            return _defaultAbsoluteWebPath ??= WebImageSupplier.GetAbsoluteWebPath(_defaultAvatar);
        }

        public string GetRetinaPhotoURL(Guid userID)
        {
            return GetRetinaPhotoURL(userID, out _);
        }

        public string GetRetinaPhotoURL(Guid userID, out bool isdef)
        {
            return GetSizedPhotoAbsoluteWebPath(userID, RetinaFotoSize, out isdef);
        }

        public string GetMaxPhotoURL(Guid userID)
        {
            return GetMaxPhotoURL(userID, out _);
        }

        public string GetMaxPhotoURL(Guid userID, out bool isdef)
        {
            return GetSizedPhotoAbsoluteWebPath(userID, MaxFotoSize, out isdef);
        }

        public string GetBigPhotoURL(Guid userID)
        {
            return GetBigPhotoURL(userID, out _);
        }

        public string GetBigPhotoURL(Guid userID, out bool isdef)
        {
            return GetSizedPhotoAbsoluteWebPath(userID, BigFotoSize, out isdef);
        }

        public string GetMediumPhotoURL(Guid userID)
        {
            return GetMediumPhotoURL(userID, out _);
        }

        public string GetMediumPhotoURL(Guid userID, out bool isdef)
        {
            return GetSizedPhotoAbsoluteWebPath(userID, MediumFotoSize, out isdef);
        }

        public string GetSmallPhotoURL(Guid userID)
        {
            return GetSmallPhotoURL(userID, out _);
        }

        public string GetSmallPhotoURL(Guid userID, out bool isdef)
        {
            return GetSizedPhotoAbsoluteWebPath(userID, SmallFotoSize, out isdef);
        }


        public string GetSizedPhotoUrl(Guid userId, int width, int height)
        {
            return GetSizedPhotoAbsoluteWebPath(userId, new Size(width, height));
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


        public bool UserHasAvatar(Guid userID)
        {
            var path = GetPhotoAbsoluteWebPath(userID);
            var fileName = System.IO.Path.GetFileName(path);
            return fileName != _defaultAvatar;
        }

        public string GetPhotoAbsoluteWebPath(Guid userID)
        {
            var path = SearchInCache(userID, Size.Empty, out _);
            if (!string.IsNullOrEmpty(path)) return path;

            try
            {
                var data = UserManager.GetUserPhoto(userID);
                string photoUrl;
                string fileName;
                if (data == null || data.Length == 0)
                {
                    photoUrl = GetDefaultPhotoAbsoluteWebPath();
                    fileName = "default";
                }
                else
                {
                    photoUrl = SaveOrUpdatePhoto(userID, data, -1, new Size(-1, -1), false, out fileName);
                }

                UserPhotoManagerCache.AddToCache(userID, Size.Empty, fileName, tenant.TenantId);

                return photoUrl;
            }
            catch
            {
            }
            return GetDefaultPhotoAbsoluteWebPath();
        }

        internal Size GetPhotoSize(Guid userID)
        {
            var virtualPath = GetPhotoAbsoluteWebPath(userID);
            if (virtualPath == null) return Size.Empty;

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

        private string GetSizedPhotoAbsoluteWebPath(Guid userID, Size size)
        {
            return GetSizedPhotoAbsoluteWebPath(userID, size, out _);
        }

        private string GetSizedPhotoAbsoluteWebPath(Guid userID, Size size, out bool isdef)
        {
            var res = SearchInCache(userID, size, out isdef);
            if (!string.IsNullOrEmpty(res)) return res;

            try
            {
                var data = UserManager.GetUserPhoto(userID);

                if (data == null || data.Length == 0)
                {
                    //empty photo. cache default
                    var photoUrl = GetDefaultPhotoAbsoluteWebPath(size);

                    UserPhotoManagerCache.AddToCache(userID, size, "default", tenant.TenantId);
                    isdef = true;
                    return photoUrl;
                }

                //Enqueue for sizing
                SizePhoto(userID, data, -1, size);
            }
            catch { }

            isdef = false;
            return GetDefaultPhotoAbsoluteWebPath(size);
        }

        private string GetDefaultPhotoAbsoluteWebPath(Size size)
            {
            return size switch
            {
                Size(var w, var h) when w == RetinaFotoSize.Width && h == RetinaFotoSize.Height => WebImageSupplier.GetAbsoluteWebPath(_defaultRetinaAvatar),
                Size(var w, var h) when w == MaxFotoSize.Width && h == MaxFotoSize.Height => WebImageSupplier.GetAbsoluteWebPath(_defaultAvatar),
                Size(var w, var h) when w == BigFotoSize.Width && h == BigFotoSize.Height => WebImageSupplier.GetAbsoluteWebPath(_defaultBigAvatar),
                Size(var w, var h) when w == SmallFotoSize.Width && h == SmallFotoSize.Height => WebImageSupplier.GetAbsoluteWebPath(_defaultSmallAvatar),
                Size(var w, var h) when w == MediumFotoSize.Width && h == MediumFotoSize.Height => WebImageSupplier.GetAbsoluteWebPath(_defaultMediumAvatar),
                _ => GetDefaultPhotoAbsoluteWebPath()
            };
        }

        private static readonly HashSet<int> TenantDiskCache = new HashSet<int>();
        private static readonly object DiskCacheLoaderLock = new object();


        private string SearchInCache(Guid userId, Size size, out bool isDef)
        {
            if (!UserPhotoManagerCache.IsCacheLoadedForTenant(Tenant.TenantId))
                LoadDiskCache();

            isDef = false;

            var fileName = UserPhotoManagerCache.SearchInCache(userId, size);

            if (fileName != null && fileName.StartsWith("default"))
            {
                isDef = true;
                return GetDefaultPhotoAbsoluteWebPath(size);
            }

            if (!string.IsNullOrEmpty(fileName))
            {
                var store = GetDataStore();
                return store.GetUriAsync(fileName).Result.ToString();
            }

            return null;
        }

        private void LoadDiskCache()
        {
            lock (DiskCacheLoaderLock)
            {
                if (!UserPhotoManagerCache.IsCacheLoadedForTenant(Tenant.TenantId))
                {
                    try
                    {
                        var listFileNames = GetDataStore().ListFilesRelativeAsync("", "", "*.*", false).ToArrayAsync().Result;
                        foreach (var fileName in listFileNames)
                        {
                            //Try parse fileName
                            if (fileName != null)
                            {
                                var match = ParseFile.Match(fileName);
                                if (match.Success && match.Groups["user"].Success)
                                {
                                    var parsedUserId = new Guid(match.Groups["user"].Value);
                                    var size = Size.Empty;
                                    if (match.Groups["width"].Success && match.Groups["height"].Success)
                                    {
                                        //Parse size
                                        size = new Size(int.Parse(match.Groups["width"].Value), int.Parse(match.Groups["height"].Value));
                                    }
                                    UserPhotoManagerCache.AddToCache(parsedUserId, size, fileName, tenant.TenantId);
                                }
                            }
                        }
                        UserPhotoManagerCache.SetCacheLoadedForTenant(true, Tenant.TenantId);
                    }
                    catch (Exception err)
                    {
                        Log.Error(err);
                    }
                }
            }
        }
        public void ResetThumbnailSettings(Guid userId)
        {
            var thumbSettings = new UserPhotoThumbnailSettings().GetDefault(ServiceProvider) as UserPhotoThumbnailSettings;
            SettingsManager.SaveForUser(thumbSettings, userId);
        }

        public string SaveOrUpdatePhoto(Guid userID, byte[] data)
        {
            return SaveOrUpdatePhoto(userID, data, -1, OriginalFotoSize, true, out _);
        }

        public void RemovePhoto(Guid idUser)
        {
            UserManager.SaveUserPhoto(idUser, null);
            try
            {
                var storage = GetDataStore();
                storage.DeleteFilesAsync("", idUser + "*.*", false).Wait();
            } 
            catch(DirectoryNotFoundException e)
            {
                Log.Error(e);
            }

            UserManager.SaveUserPhoto(idUser, null);
            UserPhotoManagerCache.ClearCache(idUser, tenant.TenantId);
        }

        public void SyncPhoto(Guid userID, byte[] data)
        {
            data = TryParseImage(data, -1, OriginalFotoSize, out _, out int width, out int height);
            UserManager.SaveUserPhoto(userID, data);
            SetUserPhotoThumbnailSettings(userID, width, height);
            UserPhotoManagerCache.ClearCache(userID, tenant.TenantId);
        }


        private string SaveOrUpdatePhoto(Guid userID, byte[] data, long maxFileSize, Size size, bool saveInCoreContext, out string fileName)
        {
            data = TryParseImage(data, maxFileSize, size, out var imgFormat, out var width, out var height);

            var widening = CommonPhotoManager.GetImgFormatName(imgFormat);
            fileName = string.Format("{0}_orig_{1}-{2}.{3}", userID, width, height, widening);

            if (saveInCoreContext)
            {
                UserManager.SaveUserPhoto(userID, data);
                SetUserPhotoThumbnailSettings(userID, width, height);
                UserPhotoManagerCache.ClearCache(userID, tenant.TenantId);

            }

            var store = GetDataStore();

            var photoUrl = GetDefaultPhotoAbsoluteWebPath();
            if (data != null && data.Length > 0)
            {
                using (var stream = new MemoryStream(data))
                {
                    photoUrl = store.SaveAsync(fileName, stream).Result.ToString();
                }
                //Queue resizing
                SizePhoto(userID, data, -1, SmallFotoSize, true);
                SizePhoto(userID, data, -1, MediumFotoSize, true);
                SizePhoto(userID, data, -1, BigFotoSize, true);
                SizePhoto(userID, data, -1, MaxFotoSize, true);
                SizePhoto(userID, data, -1, RetinaFotoSize, true);
            }
            return photoUrl;
        }

        private void SetUserPhotoThumbnailSettings(Guid userId, int width, int height)
        {
            var settings = SettingsManager.LoadForUser<UserPhotoThumbnailSettings>(userId);

            if (!settings.IsDefault) return;

            var max = Math.Max(Math.Max(width, height), SmallFotoSize.Width);
            var min = Math.Max(Math.Min(width, height), SmallFotoSize.Width);

            var pos = (max - min) / 2;

            settings = new UserPhotoThumbnailSettings(
                width >= height ? new Point(pos, 0) : new Point(0, pos),
                new Size(min, min));

            SettingsManager.SaveForUser(settings, userId);
        }

        private byte[] TryParseImage(byte[] data, long maxFileSize, Size maxsize, out IImageFormat imgFormat, out int width, out int height)
        {
            if (data == null || data.Length <= 0) throw new UnknownImageFormatException();
            if (maxFileSize != -1 && data.Length > maxFileSize) throw new ImageSizeLimitException();

            //data = ImageHelper.RotateImageByExifOrientationData(data, Log);

            try
            {
                using var img = Image.Load(data ,out var format);
                imgFormat = format;
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
                    using Image destRound = img.Clone(x => x.Resize(new ResizeOptions
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

        private string SizePhoto(Guid userID, byte[] data, long maxFileSize, Size size)
        {
            return SizePhoto(userID, data, maxFileSize, size, false);
        }

        private string SizePhoto(Guid userID, byte[] data, long maxFileSize, Size size, bool now)
        {
            if (data == null || data.Length <= 0) throw new UnknownImageFormatException();
            if (maxFileSize != -1 && data.Length > maxFileSize) throw new ImageWeightLimitException();

            var resizeTask = new ResizeWorkerItem(userID, data, maxFileSize, size, GetDataStore(), SettingsManager.LoadForUser<UserPhotoThumbnailSettings>(userID));
            var key = $"{userID}{size}";
            resizeTask.SetProperty("key", key);

            if (now)
            {
                //Resize synchronously
                ResizeImage(resizeTask);
                return GetSizedPhotoAbsoluteWebPath(userID, size);
            }
            else
            {
                if (!ResizeQueue.GetTasks<ResizeWorkerItem>().Any(r => r.GetProperty<string>("key") == key))
                {
                    //Add
                    ResizeQueue.QueueTask((a, b) => ResizeImage(resizeTask), resizeTask);
                }
                return GetDefaultPhotoAbsoluteWebPath(size);
                //NOTE: return default photo here. Since task will update cache
            }
        }

        private void ResizeImage(ResizeWorkerItem item)
        {
            try
            {
                var data = item.Data;
                using var stream = new MemoryStream(data);
                using var img = Image.Load(stream, out var format);
                var imgFormat = format;
                if (item.Size != img.Size())
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
                item.DataStore.SaveAsync(fileName, stream2).Result.ToString();

                UserPhotoManagerCache.AddToCache(item.UserId, item.Size, fileName, tenant.TenantId);
            }
            catch (ArgumentException error)
            {
                throw new UnknownImageFormatException(error);
            }
        }

        public string GetTempPhotoAbsoluteWebPath(string fileName)
        {
            return GetDataStore().GetUriAsync(_tempDomainName, fileName).Result.ToString();
        }

        public string SaveTempPhoto(byte[] data, long maxFileSize, int maxWidth, int maxHeight)
        {
            data = TryParseImage(data, maxFileSize, new Size(maxWidth, maxHeight), out var imgFormat, out _, out _);

            var fileName = Guid.NewGuid() + "." + CommonPhotoManager.GetImgFormatName(imgFormat);

            var store = GetDataStore();
            using var stream = new MemoryStream(data);
            return store.SaveAsync(_tempDomainName, fileName, stream).Result.ToString();
        }

        public byte[] GetTempPhotoData(string fileName)
        {
            using var s = GetDataStore().GetReadStreamAsync(_tempDomainName, fileName).Result;
            var data = new MemoryStream();
            var buffer = new byte[1024 * 10];
            while (true)
            {
                var count = s.Read(buffer, 0, buffer.Length);
                if (count == 0) break;
                data.Write(buffer, 0, count);
            }
            return data.ToArray();
        }

        public string GetSizedTempPhotoAbsoluteWebPath(string fileName, int newWidth, int newHeight)
        {
            var store = GetDataStore();
            if (store.IsFileAsync(_tempDomainName, fileName).Result)
            {
                using var s = store.GetReadStreamAsync(_tempDomainName, fileName).Result;
                using var img = Image.Load(s, out var format);
                var imgFormat = format;
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
                return store.SaveAsync(_tempDomainName, trueFileName, stream).Result.ToString();
            }
            return GetDefaultPhotoAbsoluteWebPath(new Size(newWidth, newHeight));
        }

        public void RemoveTempPhoto(string fileName)
        {
            var index = fileName.LastIndexOf('.');
            var fileNameWithoutExt = (index != -1) ? fileName.Substring(0, index) : fileName;
            try
            {
                var store = GetDataStore();
                store.DeleteFilesAsync(_tempDomainName, "", fileNameWithoutExt + "*.*", false).Wait();
            }
            catch { }
        }


        public Image GetPhotoImage(Guid userID, out IImageFormat format)
        {
            try
            {
                var data = UserManager.GetUserPhoto(userID);
                if (data != null)
                {
                    var img = Image.Load(data, out var imgFormat);
                    format = imgFormat;
                    return img;
                }
            }
            catch { }
            format = null;
            return null;
        }

        public string SaveThumbnail(Guid userID, Image img, IImageFormat format)
        {
            var moduleID = Guid.Empty;
            var widening = CommonPhotoManager.GetImgFormatName(format);
            var size = img.Size();
            var fileName = string.Format("{0}{1}_size_{2}-{3}.{4}", moduleID == Guid.Empty ? "" : moduleID.ToString(), userID, img.Width, img.Height, widening);

            var store = GetDataStore();
            string photoUrl;
            using (var s = new MemoryStream(CommonPhotoManager.SaveToBytes(img)))
            {
                img.Dispose();
                photoUrl = store.SaveAsync(fileName, s).Result.ToString();
            }

            UserPhotoManagerCache.AddToCache(userID, size, fileName, tenant.TenantId);
            return photoUrl;
        }

        public byte[] GetUserPhotoData(Guid userId, Size size)
        {
            try
            {
                var pattern = string.Format("{0}_size_{1}-{2}.*", userId, size.Width, size.Height);

                var fileName = GetDataStore().ListFilesRelativeAsync("", "", pattern, false).ToArrayAsync().Result.FirstOrDefault();

                if (string.IsNullOrEmpty(fileName)) return null;

                using var s = GetDataStore().GetReadStreamAsync("", fileName).Result;
                var data = new MemoryStream();
                var buffer = new byte[1024 * 10];
                while (true)
                {
                    var count = s.Read(buffer, 0, buffer.Length);
                    if (count == 0) break;
                    data.Write(buffer, 0, count);
                }
                return data.ToArray();
            }
            catch (Exception err)
            {
                Log.Error(err);
                return null;
            }
        }

        private IDataStore dataStore;
        private IDataStore GetDataStore()
        {
            return dataStore ??= StorageFactory.GetStorage(Tenant.TenantId.ToString(), "userPhotos");
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
            services.AddDistributedTaskQueueService<ResizeWorkerItem>(2);
        }
    }
}