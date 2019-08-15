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
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using ASC.Common.Caching;
using ASC.Common.Logging;
using ASC.Common.Threading.Workers;
using ASC.Core;
using ASC.Core.Tenants;
using ASC.Data.Storage;
using ASC.Web.Core.Utility.Skins;
using ASC.Web.Studio.Utility;

namespace ASC.Web.Core.Users
{
    internal class ResizeWorkerItem
    {
        public ResizeWorkerItem(Guid userId, byte[] data, long maxFileSize, Size size, IDataStore dataStore, UserPhotoThumbnailSettings settings)
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
            if (obj.GetType() != typeof(ResizeWorkerItem)) return false;
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
            unchecked
            {
                var result = UserId.GetHashCode();
                result = (result * 397) ^ MaxFileSize.GetHashCode();
                result = (result * 397) ^ Size.GetHashCode();
                return result;
            }
        }
    }

    public class UserPhotoManager
    {
        private static readonly IDictionary<Guid, IDictionary<Size, string>> Photofiles = new Dictionary<Guid, IDictionary<Size, string>>();
        private static readonly ICacheNotify<UserPhotoManagerCacheItem> CacheNotify;

        static UserPhotoManager()
        {
            try
            {
                CacheNotify = new KafkaCache<UserPhotoManagerCacheItem>();

                CacheNotify.Subscribe((data) =>
                {
                    var userId = new Guid(data.UserID);
                    var size = new Size(data.Size.Width, data.Size.Height);

                    lock (Photofiles)
                    {

                        if (!Photofiles.ContainsKey(userId))
                        {
                            Photofiles[userId] = new ConcurrentDictionary<Size, string>();
                        }

                        Photofiles[userId][size] = data.FileName;
                    }
                }, CacheNotifyAction.InsertOrUpdate);

                CacheNotify.Subscribe((data) =>
                {
                    var userId = new Guid(data.UserID);
                    var size = new Size(data.Size.Width, data.Size.Height);

                    try
                    {
                        lock (Photofiles)
                        {
                            Photofiles.Remove(userId);
                        }
                        var storage = GetDataStore();
                        storage.DeleteFiles("", data.UserID + "*.*", false);
                        SetCacheLoadedForTenant(false);
                    }
                    catch { }
                }, CacheNotifyAction.Remove);
            }
            catch (Exception)
            {

            }
        }


        public static string GetDefaultPhotoAbsoluteWebPath()
        {
            return WebImageSupplier.GetAbsoluteWebPath(_defaultAvatar);
        }

        public static string GetRetinaPhotoURL(int tenantId, Guid userID)
        {
            return GetRetinaPhotoURL(tenantId, userID, out _);
        }

        public static string GetRetinaPhotoURL(int tenantId, Guid userID, out bool isdef)
        {
            return GetSizedPhotoAbsoluteWebPath(tenantId, userID, RetinaFotoSize, out isdef);
        }

        public static string GetMaxPhotoURL(int tenantId, Guid userID)
        {
            return GetMaxPhotoURL(tenantId, userID, out _);
        }

        public static string GetMaxPhotoURL(int tenantId, Guid userID, out bool isdef)
        {
            return GetSizedPhotoAbsoluteWebPath(tenantId, userID, MaxFotoSize, out isdef);
        }

        public static string GetBigPhotoURL(int tenantId, Guid userID)
        {
            return GetBigPhotoURL(tenantId, userID, out _);
        }

        public static string GetBigPhotoURL(int tenantId, Guid userID, out bool isdef)
        {
            return GetSizedPhotoAbsoluteWebPath(tenantId, userID, BigFotoSize, out isdef);
        }

        public static string GetMediumPhotoURL(int tenantId, Guid userID)
        {
            return GetMediumPhotoURL(tenantId, userID, out _);
        }

        public static string GetMediumPhotoURL(int tenantId, Guid userID, out bool isdef)
        {
            return GetSizedPhotoAbsoluteWebPath(tenantId, userID, MediumFotoSize, out isdef);
        }

        public static string GetSmallPhotoURL(int tenantId, Guid userID)
        {
            return GetSmallPhotoURL(tenantId, userID, out _);
        }

        public static string GetSmallPhotoURL(int tenantId, Guid userID, out bool isdef)
        {
            return GetSizedPhotoAbsoluteWebPath(tenantId, userID, SmallFotoSize, out isdef);
        }


        public static string GetSizedPhotoUrl(int tenantId, Guid userId, int width, int height)
        {
            return GetSizedPhotoAbsoluteWebPath(tenantId, userId, new Size(width, height));
        }


        public static string GetDefaultSmallPhotoURL()
        {
            return GetDefaultPhotoAbsoluteWebPath(SmallFotoSize);
        }

        public static string GetDefaultMediumPhotoURL()
        {
            return GetDefaultPhotoAbsoluteWebPath(MediumFotoSize);
        }

        public static string GetDefaultBigPhotoURL()
        {
            return GetDefaultPhotoAbsoluteWebPath(BigFotoSize);
        }

        public static string GetDefaultMaxPhotoURL()
        {
            return GetDefaultPhotoAbsoluteWebPath(MaxFotoSize);
        }

        public static string GetDefaultRetinaPhotoURL()
        {
            return GetDefaultPhotoAbsoluteWebPath(RetinaFotoSize);
        }



        public static Size OriginalFotoSize
        {
            get { return new Size(1280, 1280); }
        }

        public static Size RetinaFotoSize
        {
            get { return new Size(360, 360); }
        }

        public static Size MaxFotoSize
        {
            get { return new Size(200, 200); }
        }

        public static Size BigFotoSize
        {
            get { return new Size(82, 82); }
        }

        public static Size MediumFotoSize
        {
            get { return new Size(48, 48); }
        }

        public static Size SmallFotoSize
        {
            get { return new Size(32, 32); }
        }

        private static readonly string _defaultRetinaAvatar = "default_user_photo_size_360-360.png";
        private static readonly string _defaultAvatar = "default_user_photo_size_200-200.png";
        private static readonly string _defaultSmallAvatar = "default_user_photo_size_32-32.png";
        private static readonly string _defaultMediumAvatar = "default_user_photo_size_48-48.png";
        private static readonly string _defaultBigAvatar = "default_user_photo_size_82-82.png";
        private static readonly string _tempDomainName = "temp";


        public static bool UserHasAvatar(Tenant tenant, Guid userID)
        {
            var path = GetPhotoAbsoluteWebPath(tenant, userID);
            var fileName = Path.GetFileName(path);
            return fileName != _defaultAvatar;
        }

        public static string GetPhotoAbsoluteWebPath(Tenant tenant, Guid userID)
        {
            var path = SearchInCache(userID, Size.Empty, out _);
            if (!string.IsNullOrEmpty(path)) return path;

            try
            {
                var data = CoreContext.UserManager.GetUserPhoto(tenant.TenantId, userID);
                string photoUrl;
                string fileName;
                if (data == null || data.Length == 0)
                {
                    photoUrl = GetDefaultPhotoAbsoluteWebPath();
                    fileName = "default";
                }
                else
                {
                    photoUrl = SaveOrUpdatePhoto(tenant, userID, data, -1, new Size(-1, -1), false, out fileName);
                }
                AddToCache(userID, Size.Empty, fileName);

                return photoUrl;
            }
            catch
            {
            }
            return GetDefaultPhotoAbsoluteWebPath();
        }

        internal static Size GetPhotoSize(Tenant tenant, Guid userID)
        {
            var virtualPath = GetPhotoAbsoluteWebPath(tenant, userID);
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

        private static string GetSizedPhotoAbsoluteWebPath(int tenantId, Guid userID, Size size)
        {
            return GetSizedPhotoAbsoluteWebPath(tenantId, userID, size, out _);
        }

        private static string GetSizedPhotoAbsoluteWebPath(int tenantId, Guid userID, Size size, out bool isdef)
        {
            var res = SearchInCache(userID, size, out isdef);
            if (!string.IsNullOrEmpty(res)) return res;

            try
            {
                var data = CoreContext.UserManager.GetUserPhoto(tenantId, userID);

                if (data == null || data.Length == 0)
                {
                    //empty photo. cache default
                    var photoUrl = GetDefaultPhotoAbsoluteWebPath(size);

                    AddToCache(userID, size, "default");
                    isdef = true;
                    return photoUrl;
                }

                //Enqueue for sizing
                SizePhoto(tenantId, userID, data, -1, size);
            }
            catch { }

            isdef = false;
            return GetDefaultPhotoAbsoluteWebPath(size);
        }

        private static string GetDefaultPhotoAbsoluteWebPath(Size size)
        {
            if (size == RetinaFotoSize) return WebImageSupplier.GetAbsoluteWebPath(_defaultRetinaAvatar);
            if (size == MaxFotoSize) return WebImageSupplier.GetAbsoluteWebPath(_defaultAvatar);
            if (size == BigFotoSize) return WebImageSupplier.GetAbsoluteWebPath(_defaultBigAvatar);
            if (size == SmallFotoSize) return WebImageSupplier.GetAbsoluteWebPath(_defaultSmallAvatar);
            if (size == MediumFotoSize) return WebImageSupplier.GetAbsoluteWebPath(_defaultMediumAvatar);
            return GetDefaultPhotoAbsoluteWebPath();
        }

        //Regex for parsing filenames into groups with id's
        private static readonly Regex ParseFile =
                new Regex(@"^(?'module'\{{0,1}([0-9a-fA-F]){8}-([0-9a-fA-F]){4}-([0-9a-fA-F]){4}-([0-9a-fA-F]){4}-([0-9a-fA-F]){12}\}{0,1}){0,1}" +
                    @"(?'user'\{{0,1}([0-9a-fA-F]){8}-([0-9a-fA-F]){4}-([0-9a-fA-F]){4}-([0-9a-fA-F]){4}-([0-9a-fA-F]){12}\}{0,1}){1}" +
                    @"_(?'kind'orig|size){1}_(?'size'(?'width'[0-9]{1,5})-{1}(?'height'[0-9]{1,5})){0,1}\..*", RegexOptions.Compiled);

        private static readonly HashSet<int> TenantDiskCache = new HashSet<int>();
        private static readonly object DiskCacheLoaderLock = new object();

        private static bool IsCacheLoadedForTenant()
        {
            return TenantDiskCache.Contains(TenantProvider.CurrentTenantID);
        }

        private static bool SetCacheLoadedForTenant(bool isLoaded)
        {
            return isLoaded ? TenantDiskCache.Add(TenantProvider.CurrentTenantID) : TenantDiskCache.Remove(TenantProvider.CurrentTenantID);
        }


        private static string SearchInCache(Guid userId, Size size, out bool isDef)
        {
            if (!IsCacheLoadedForTenant())
                LoadDiskCache();

            isDef = false;

            string fileName;
            lock (Photofiles)
            {
                if (!Photofiles.ContainsKey(userId)) return null;
                if (size != Size.Empty && !Photofiles[userId].ContainsKey(size)) return null;

                if (size != Size.Empty)
                    fileName = Photofiles[userId][size];
                else
                    fileName = Photofiles[userId]
                                .Select(x => x.Value)
                                .FirstOrDefault(x => !string.IsNullOrEmpty(x) && x.Contains("_orig_"));
            }
            if (fileName != null && fileName.StartsWith("default"))
            {
                isDef = true;
                return GetDefaultPhotoAbsoluteWebPath(size);
            }

            if (!string.IsNullOrEmpty(fileName))
            {
                var store = GetDataStore();
                return store.GetUri(fileName).ToString();
            }

            return null;
        }


        private static void LoadDiskCache()
        {
            lock (DiskCacheLoaderLock)
            {
                if (!IsCacheLoadedForTenant())
                {
                    try
                    {
                        var listFileNames = GetDataStore().ListFilesRelative("", "", "*.*", false);
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
                                    AddToCache(parsedUserId, size, fileName);
                                }
                            }
                        }
                        SetCacheLoadedForTenant(true);
                    }
                    catch (Exception err)
                    {
                        LogManager.GetLogger("ASC.Web.Photo").Error(err);
                    }
                }
            }
        }

        private static void ClearCache(Guid userID)
        {
            if (CacheNotify != null)
            {
                CacheNotify.Publish(new UserPhotoManagerCacheItem { UserID = userID.ToString() }, CacheNotifyAction.Remove);
            }
        }

        private static void AddToCache(Guid userId, Size size, string fileName)
        {
            if (CacheNotify != null)
            {
                CacheNotify.Publish(new UserPhotoManagerCacheItem { UserID = userId.ToString(), Size = new CacheSize() { Height = size.Height, Width = size.Width }, FileName = fileName }, CacheNotifyAction.InsertOrUpdate);
            }
        }

        public static void ResetThumbnailSettings(Guid userId)
        {
            var thumbSettings = new UserPhotoThumbnailSettings().GetDefault() as UserPhotoThumbnailSettings;
            thumbSettings.SaveForUser(userId);
        }

        public static string SaveOrUpdatePhoto(Tenant tenant, Guid userID, byte[] data)
        {
            return SaveOrUpdatePhoto(tenant, userID, data, -1, OriginalFotoSize, true, out var fileName);
        }

        public static void RemovePhoto(Tenant tenant, Guid idUser)
        {
            CoreContext.UserManager.SaveUserPhoto(tenant, idUser, null);
            ClearCache(idUser);
        }

        private static string SaveOrUpdatePhoto(Tenant tenant, Guid userID, byte[] data, long maxFileSize, Size size, bool saveInCoreContext, out string fileName)
        {
            data = TryParseImage(data, maxFileSize, size, out var imgFormat, out var width, out var height);

            var widening = CommonPhotoManager.GetImgFormatName(imgFormat);
            fileName = string.Format("{0}_orig_{1}-{2}.{3}", userID, width, height, widening);

            if (saveInCoreContext)
            {
                CoreContext.UserManager.SaveUserPhoto(tenant, userID, data);
                SetUserPhotoThumbnailSettings(userID, width, height);
                ClearCache(userID);
            }

            var store = GetDataStore();

            var photoUrl = GetDefaultPhotoAbsoluteWebPath();
            if (data != null && data.Length > 0)
            {
                using (var stream = new MemoryStream(data))
                {
                    photoUrl = store.Save(fileName, stream).ToString();
                }
                //Queue resizing
                SizePhoto(tenant.TenantId, userID, data, -1, SmallFotoSize, true);
                SizePhoto(tenant.TenantId, userID, data, -1, MediumFotoSize, true);
                SizePhoto(tenant.TenantId, userID, data, -1, BigFotoSize, true);
                SizePhoto(tenant.TenantId, userID, data, -1, MaxFotoSize, true);
                SizePhoto(tenant.TenantId, userID, data, -1, RetinaFotoSize, true);
            }
            return photoUrl;
        }

        private static void SetUserPhotoThumbnailSettings(Guid userId, int width, int height)
        {
            var settings = UserPhotoThumbnailSettings.LoadForUser(userId);

            if (!settings.IsDefault) return;

            var max = Math.Max(Math.Max(width, height), SmallFotoSize.Width);
            var min = Math.Max(Math.Min(width, height), SmallFotoSize.Width);

            var pos = (max - min) / 2;

            settings = new UserPhotoThumbnailSettings(
                width >= height ? new Point(pos, 0) : new Point(0, pos),
                new Size(min, min));

            settings.SaveForUser(userId);
        }

        private static byte[] TryParseImage(byte[] data, long maxFileSize, Size maxsize, out ImageFormat imgFormat, out int width, out int height)
        {
            if (data == null || data.Length <= 0) throw new UnknownImageFormatException();
            if (maxFileSize != -1 && data.Length > maxFileSize) throw new ImageSizeLimitException();

            data = ImageHelper.RotateImageByExifOrientationData(data);

            try
            {
                using (var stream = new MemoryStream(data))
                using (var img = new Bitmap(stream))
                {
                    imgFormat = img.RawFormat;
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
                                height = (int)((double)height * (double)maxWidth / (double)width + 0.5);
                                width = maxWidth;
                            }
                            else
                            {
                                width = (int)((double)width * (double)maxHeight / (double)height + 0.5);
                                height = maxHeight;
                            }
                        }

                        if (width > maxWidth && height <= maxHeight)
                        {
                            height = (int)((double)height * (double)maxWidth / (double)width + 0.5);
                            width = maxWidth;
                        }

                        if (width <= maxWidth && height > maxHeight)
                        {
                            width = (int)((double)width * (double)maxHeight / (double)height + 0.5);
                            height = maxHeight;
                        }

                        #endregion

                        using (var b = new Bitmap(width, height))
                        using (var gTemp = Graphics.FromImage(b))
                        {
                            gTemp.InterpolationMode = InterpolationMode.HighQualityBicubic;
                            gTemp.PixelOffsetMode = PixelOffsetMode.HighQuality;
                            gTemp.SmoothingMode = SmoothingMode.HighQuality;
                            gTemp.DrawImage(img, 0, 0, width, height);

                            data = CommonPhotoManager.SaveToBytes(b);
                        }
                    }
                    return data;
                }
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

        //note: using auto stop queue
        private static readonly WorkerQueue<ResizeWorkerItem> ResizeQueue = new WorkerQueue<ResizeWorkerItem>(2, TimeSpan.FromSeconds(30), 1, true);//TODO: configure

        private static string SizePhoto(int tenantId, Guid userID, byte[] data, long maxFileSize, Size size)
        {
            return SizePhoto(tenantId, userID, data, maxFileSize, size, false);
        }

        private static string SizePhoto(int tenantId, Guid userID, byte[] data, long maxFileSize, Size size, bool now)
        {
            if (data == null || data.Length <= 0) throw new UnknownImageFormatException();
            if (maxFileSize != -1 && data.Length > maxFileSize) throw new ImageWeightLimitException();

            var resizeTask = new ResizeWorkerItem(userID, data, maxFileSize, size, GetDataStore(), UserPhotoThumbnailSettings.LoadForUser(userID));

            if (now)
            {
                //Resize synchronously
                ResizeImage(resizeTask);
                return GetSizedPhotoAbsoluteWebPath(tenantId, userID, size);
            }
            else
            {
                if (!ResizeQueue.GetItems().Contains(resizeTask))
                {
                    //Add
                    ResizeQueue.Add(resizeTask);
                    if (!ResizeQueue.IsStarted)
                    {
                        ResizeQueue.Start(ResizeImage);
                    }
                }
                return GetDefaultPhotoAbsoluteWebPath(size);
                //NOTE: return default photo here. Since task will update cache
            }
        }

        private static void ResizeImage(ResizeWorkerItem item)
        {
            try
            {
                var data = item.Data;
                using (var stream = new MemoryStream(data))
                using (var img = Image.FromStream(stream))
                {
                    var imgFormat = img.RawFormat;
                    if (item.Size != img.Size)
                    {
                        using (var img2 = item.Settings.IsDefault ?
                            CommonPhotoManager.DoThumbnail(img, item.Size, true, true, true) :
                            UserPhotoThumbnailManager.GetBitmap(img, item.Size, item.Settings))
                        {
                            data = CommonPhotoManager.SaveToBytes(img2);
                        }
                    }
                    else
                    {
                        data = CommonPhotoManager.SaveToBytes(img);
                    }

                    var widening = CommonPhotoManager.GetImgFormatName(imgFormat);
                    var fileName = string.Format("{0}_size_{1}-{2}.{3}", item.UserId, item.Size.Width, item.Size.Height, widening);

                    using (var stream2 = new MemoryStream(data))
                    {
                        item.DataStore.Save(fileName, stream2).ToString();

                        AddToCache(item.UserId, item.Size, fileName);
                    }
                }
            }
            catch (ArgumentException error)
            {
                throw new UnknownImageFormatException(error);
            }
        }

        public static string GetTempPhotoAbsoluteWebPath(string fileName)
        {
            return GetDataStore().GetUri(_tempDomainName, fileName).ToString();
        }

        public static string SaveTempPhoto(byte[] data, long maxFileSize, int maxWidth, int maxHeight)
        {
            data = TryParseImage(data, maxFileSize, new Size(maxWidth, maxHeight), out var imgFormat, out var width, out var height);

            var fileName = Guid.NewGuid() + "." + CommonPhotoManager.GetImgFormatName(imgFormat);

            var store = GetDataStore();
            using (var stream = new MemoryStream(data))
            {
                return store.Save(_tempDomainName, fileName, stream).ToString();
            }
        }

        public static byte[] GetTempPhotoData(string fileName)
        {
            using (var s = GetDataStore().GetReadStream(_tempDomainName, fileName))
            {
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
        }

        public static string GetSizedTempPhotoAbsoluteWebPath(string fileName, int newWidth, int newHeight)
        {
            var store = GetDataStore();
            if (store.IsFile(_tempDomainName, fileName))
            {
                using (var s = store.GetReadStream(_tempDomainName, fileName))
                using (var img = Image.FromStream(s))
                {
                    var imgFormat = img.RawFormat;
                    byte[] data;

                    if (img.Width != newWidth || img.Height != newHeight)
                    {
                        using (var img2 = CommonPhotoManager.DoThumbnail(img, new Size(newWidth, newHeight), true, true, true))
                        {
                            data = CommonPhotoManager.SaveToBytes(img2);
                        }
                    }
                    else
                    {
                        data = CommonPhotoManager.SaveToBytes(img);
                    }
                    var widening = CommonPhotoManager.GetImgFormatName(imgFormat);
                    var index = fileName.LastIndexOf('.');
                    var fileNameWithoutExt = (index != -1) ? fileName.Substring(0, index) : fileName;

                    var trueFileName = fileNameWithoutExt + "_size_" + newWidth.ToString() + "-" + newHeight.ToString() + "." + widening;
                    using (var stream = new MemoryStream(data))
                    {
                        return store.Save(_tempDomainName, trueFileName, stream).ToString();
                    }
                }
            }
            return GetDefaultPhotoAbsoluteWebPath(new Size(newWidth, newHeight));
        }

        public static void RemoveTempPhoto(string fileName)
        {
            var index = fileName.LastIndexOf('.');
            var fileNameWithoutExt = (index != -1) ? fileName.Substring(0, index) : fileName;
            try
            {
                var store = GetDataStore();
                store.DeleteFiles(_tempDomainName, "", fileNameWithoutExt + "*.*", false);
            }
            catch { };
        }


        public static Bitmap GetPhotoBitmap(int tenantId, Guid userID)
        {
            try
            {
                var data = CoreContext.UserManager.GetUserPhoto(tenantId, userID);
                if (data != null)
                {
                    using (var s = new MemoryStream(data))
                    {
                        return new Bitmap(s);
                    }
                }
            }
            catch { }
            return null;
        }

        public static string SaveThumbnail(Guid userID, Image img, ImageFormat format)
        {
            var moduleID = Guid.Empty;
            var widening = CommonPhotoManager.GetImgFormatName(format);
            var size = img.Size;
            var fileName = string.Format("{0}{1}_size_{2}-{3}.{4}", (moduleID == Guid.Empty ? "" : moduleID.ToString()), userID, img.Width, img.Height, widening);

            var store = GetDataStore();
            string photoUrl;
            using (var s = new MemoryStream(CommonPhotoManager.SaveToBytes(img)))
            {
                img.Dispose();
                photoUrl = store.Save(fileName, s).ToString();
            }

            AddToCache(userID, size, fileName);
            return photoUrl;
        }

        public static byte[] GetUserPhotoData(Guid userId, Size size)
        {
            try
            {
                var pattern = string.Format("{0}_size_{1}-{2}.*", userId, size.Width, size.Height);

                var fileName = GetDataStore().ListFilesRelative("", "", pattern, false).FirstOrDefault();

                if (string.IsNullOrEmpty(fileName)) return null;

                using (var s = GetDataStore().GetReadStream("", fileName))
                {
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
            }
            catch (Exception err)
            {
                LogManager.GetLogger("ASC.Web.Photo").Error(err);
                return null;
            }
        }

        private static IDataStore GetDataStore()
        {
            return StorageFactory.GetStorage(TenantProvider.CurrentTenantID.ToString(), "userPhotos");
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
    public static class ImageHelper
    {
        /// <summary>
        /// Rotate the given image byte array according to Exif Orientation data
        /// </summary>
        /// <param name="data">source image byte array</param>
        /// <param name="updateExifData">set it to TRUE to update image Exif data after rotation (default is TRUE)</param>
        /// <returns>The rotated image byte array. If no rotation occurred, source data will be returned.</returns>
        public static byte[] RotateImageByExifOrientationData(byte[] data, bool updateExifData = true)
        {
            try
            {
                using var stream = new MemoryStream(data);
                using var img = new Bitmap(stream);
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
                LogManager.GetLogger("ASC.Web.Photo").Error(err);
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
            var bmp = new Bitmap(sourceFilePath);
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
            switch (orientation)
            {
                case 1:
                    return RotateFlipType.RotateNoneFlipNone;
                case 2:
                    return RotateFlipType.RotateNoneFlipX;
                case 3:
                    return RotateFlipType.Rotate180FlipNone;
                case 4:
                    return RotateFlipType.Rotate180FlipX;
                case 5:
                    return RotateFlipType.Rotate90FlipX;
                case 6:
                    return RotateFlipType.Rotate90FlipNone;
                case 7:
                    return RotateFlipType.Rotate270FlipX;
                case 8:
                    return RotateFlipType.Rotate270FlipNone;
                default:
                    return RotateFlipType.RotateNoneFlipNone;
            }
        }
    }
}