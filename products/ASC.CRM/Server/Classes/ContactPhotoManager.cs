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
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Net;

using ASC.Common;
using ASC.Common.Caching;
using ASC.Common.Logging;
using ASC.Common.Threading;
using ASC.CRM.Resources;
using ASC.Data.Storage;
using ASC.Web.Core;
using ASC.Web.Core.Utility.Skins;
using ASC.Web.CRM.Configuration;

using Microsoft.Extensions.Options;

namespace ASC.Web.CRM.Classes
{
    public class ResizeWorkerItem : DistributedTask
    {
        public int ContactID { get; set; }

        public bool UploadOnly { get; set; }

        public String TmpDirName { get; set; }

        public Size[] RequireFotoSize { get; set; }

        public byte[] ImageData { get; set; }

        public IDataStore DataStore { get; set; }

        public override bool Equals(object obj)
        {
            if (!(obj is ResizeWorkerItem)) return false;

            var item = (ResizeWorkerItem)obj;

            return item.ContactID.Equals(ContactID) && RequireFotoSize.Equals(item.RequireFotoSize) && ImageData.Length == item.ImageData.Length;
        }

        public override int GetHashCode()
        {
            return ContactID ^ RequireFotoSize.GetHashCode() ^ ImageData.Length;
        }
    }

    [Scope]
    public class ContactPhotoManager
    {
        public readonly ILog _logger;
        public readonly Global _global;
        public readonly WebImageSupplier _webImageSupplier;
        private readonly DistributedTaskQueue _resizeQueue;
        private readonly ICacheNotify<ContactPhotoManagerCacheItem> _cacheNotify;
        private readonly ICache _cache;

        private const string PhotosBaseDirName = "photos";
        private const string PhotosDefaultTmpDirName = "temp";

        private static readonly Size _oldBigSize = new Size(145, 145);

        private readonly Size _bigSize = new Size(200, 200);
        private readonly Size _mediumSize = new Size(82, 82);
        private readonly Size _smallSize = new Size(40, 40);

        private readonly object locker = new object();


        public ContactPhotoManager(Global global,
                                   WebImageSupplier webImageSupplier,
                                   IOptionsMonitor<ILog> logger,
                                   ICache cache,
                                   ICacheNotify<ContactPhotoManagerCacheItem> cacheNotify,
                                   DistributedTaskQueueOptionsManager optionsQueue)
        {
            _global = global;
            _webImageSupplier = webImageSupplier;
            _cacheNotify = cacheNotify;
            _cache = cache;
            _resizeQueue = optionsQueue.Get<ResizeWorkerItem>();
            _logger = logger.Get("ASC.CRM");

            _cacheNotify.Subscribe((x) =>
            {
                _cache.Remove($"contact_photo_cache_key_id_{x.Id}");

            }, CacheNotifyAction.Remove);
        }

        #region Cache and DataStore Methods

        private String FromCache(int contactID, Size photoSize)
        {
            var cacheItem = _cache.Get<Dictionary<Size, String>>($"contact_photo_cache_key_id_{contactID}");

            if (cacheItem is null) return String.Empty;

            if (cacheItem.ContainsKey(photoSize)) return cacheItem[photoSize];

            return String.Empty;
        }

        private void ToCache(int contactID, String photoUri, Size photoSize)
        {
            var photoUriBySize = _cache.Get<Dictionary<Size, String>>($"contact_photo_cache_key_id_{contactID}");

            if (photoUriBySize.ContainsKey(photoSize))
                photoUriBySize[photoSize] = photoUri;
            else
                photoUriBySize.Add(photoSize, photoUri);

            _cache.Insert($"contact_photo_cache_key_id_{contactID}", photoUriBySize, TimeSpan.FromMinutes(15));
        }

        private String FromDataStore(int contactID, Size photoSize)
        {
            return FromDataStore(contactID, photoSize, false, null);
        }

        private String FromDataStore(int contactID, Size photoSize, Boolean isTmpDir, String tmpDirName)
        {
            var directoryPath = !isTmpDir
                                    ? BuildFileDirectory(contactID)
                                    : (String.IsNullOrEmpty(tmpDirName) ? BuildFileTmpDirectory(contactID) : BuildFileTmpDirectory(tmpDirName));

            var filesURI = _global.GetStore().ListFiles(directoryPath, BuildFileName(contactID, photoSize) + "*", false);

            if (filesURI.Length == 0 && photoSize == _bigSize)
            {
                filesURI = _global.GetStore().ListFiles(directoryPath, BuildFileName(contactID, _oldBigSize) + "*", false);
            }

            if (filesURI.Length == 0)
            {
                return String.Empty;
            }

            return filesURI[0].ToString();
        }

        private String FromDataStoreRelative(int contactID, Size photoSize, Boolean isTmpDir, String tmpDirName)
        {
            var directoryPath = !isTmpDir
                                    ? BuildFileDirectory(contactID)
                                    : (String.IsNullOrEmpty(tmpDirName) ? BuildFileTmpDirectory(contactID) : BuildFileTmpDirectory(tmpDirName));

            var filesPaths = _global.GetStore().ListFilesRelative("", directoryPath, BuildFileName(contactID, photoSize) + "*", false);

            if (filesPaths.Length == 0 && photoSize == _bigSize)
            {
                filesPaths = _global.GetStore().ListFilesRelative("", directoryPath, BuildFileName(contactID, _oldBigSize) + "*", false);
            }

            if (filesPaths.Length == 0)
            {
                return String.Empty;
            }

            return Path.Combine(directoryPath, filesPaths[0]);
        }

        private PhotoData FromDataStore(Size photoSize, String tmpDirName)
        {
            var directoryPath = BuildFileTmpDirectory(tmpDirName);

            if (!_global.GetStore().IsDirectory(directoryPath))
                return null;

            var filesURI = _global.GetStore().ListFiles(directoryPath, BuildFileName(0, photoSize) + "*", false);

            if (filesURI.Length == 0) return null;

            return new PhotoData { Url = filesURI[0].ToString(), Path = tmpDirName };
        }

        #endregion

        #region Private Methods

        private String GetPhotoUri(int contactID, bool isCompany, Size photoSize)
        {
            var photoUri = FromCache(contactID, photoSize);

            if (!String.IsNullOrEmpty(photoUri)) return photoUri;

            photoUri = FromDataStore(contactID, photoSize);

            if (String.IsNullOrEmpty(photoUri))
                photoUri = GetDefaultPhoto(isCompany, photoSize);

            ToCache(contactID, photoUri, photoSize);

            return photoUri;
        }

        private String BuildFileDirectory(int contactID)
        {
            var s = contactID.ToString("000000");

            return String.Concat(PhotosBaseDirName, "/", s.Substring(0, 2), "/",
                                 s.Substring(2, 2), "/",
                                 s.Substring(4), "/");
        }

        private String BuildFileTmpDirectory(int contactID)
        {
            return String.Concat(BuildFileDirectory(contactID), PhotosDefaultTmpDirName, "/");
        }

        private String BuildFileTmpDirectory(string tmpDirName)
        {
            return String.Concat(PhotosBaseDirName, "/", tmpDirName.TrimEnd('/'), "/");
        }

        private String BuildFileName(int contactID, Size photoSize)
        {
            return String.Format("contact_{0}_{1}_{2}", contactID, photoSize.Width, photoSize.Height);
        }

        private String BuildFilePath(int contactID, Size photoSize, String imageExtension)
        {
            if (photoSize.IsEmpty || contactID == 0)
                throw new ArgumentException();

            return String.Concat(BuildFileDirectory(contactID), BuildFileName(contactID, photoSize), imageExtension);
        }

        private String BuildFileTmpPath(int contactID, Size photoSize, String imageExtension, String tmpDirName)
        {
            if (photoSize.IsEmpty || (contactID == 0 && String.IsNullOrEmpty(tmpDirName)))
                throw new ArgumentException();

            return String.Concat(
                String.IsNullOrEmpty(tmpDirName)
                    ? BuildFileTmpDirectory(contactID)
                    : BuildFileTmpDirectory(tmpDirName),
                BuildFileName(contactID, photoSize), imageExtension);
        }

        private void ExecResizeImage(ResizeWorkerItem resizeWorkerItem)
        {
            foreach (var fotoSize in resizeWorkerItem.RequireFotoSize)
            {
                var data = resizeWorkerItem.ImageData;
                using (var stream = new MemoryStream(data))
                using (var img = new Bitmap(stream))
                {
                    var imgFormat = img.RawFormat;
                    if (fotoSize != img.Size)
                    {
                        using (var img2 = CommonPhotoManager.DoThumbnail(img, fotoSize, false, false, false))
                        {
                            data = CommonPhotoManager.SaveToBytes(img2, Global.GetImgFormatName(imgFormat));
                        }
                    }
                    else
                    {
                        data = Global.SaveToBytes(img);
                    }

                    var fileExtension = String.Concat("." + Global.GetImgFormatName(imgFormat));

                    var photoPath = !resizeWorkerItem.UploadOnly
                                        ? BuildFilePath(resizeWorkerItem.ContactID, fotoSize, fileExtension)
                                        : BuildFileTmpPath(resizeWorkerItem.ContactID, fotoSize, fileExtension, resizeWorkerItem.TmpDirName);

                    using (var fileStream = new MemoryStream(data))
                    {
                        var photoUri = resizeWorkerItem.DataStore.Save(photoPath, fileStream).ToString();
                        photoUri = String.Format("{0}?cd={1}", photoUri, DateTime.UtcNow.Ticks);

                        if (!resizeWorkerItem.UploadOnly)
                        {
                            ToCache(resizeWorkerItem.ContactID, photoUri, fotoSize);
                        }
                    }
                }
            }
        }

        private String GetDefaultPhoto(bool isCompany, Size photoSize)
        {
            int contactID;

            if (isCompany)
                contactID = -1;
            else
                contactID = -2;

            var defaultPhotoUri = FromCache(contactID, photoSize);

            if (!String.IsNullOrEmpty(defaultPhotoUri)) return defaultPhotoUri;

            if (isCompany)
                defaultPhotoUri = _webImageSupplier.GetAbsoluteWebPath(String.Format("empty_company_logo_{0}_{1}.png", photoSize.Height, photoSize.Width), ProductEntryPoint.ID);
            else
                defaultPhotoUri = _webImageSupplier.GetAbsoluteWebPath(String.Format("empty_people_logo_{0}_{1}.png", photoSize.Height, photoSize.Width), ProductEntryPoint.ID);

            ToCache(contactID, defaultPhotoUri, photoSize);

            return defaultPhotoUri;
        }

        #endregion

        #region Delete Methods

        public void DeletePhoto(int contactID)
        {
            DeletePhoto(contactID, false, null, true);
        }

        public void DeletePhoto(int contactID, bool isTmpDir, string tmpDirName, bool recursive)
        {
            if (contactID == 0)
                throw new ArgumentException();

            lock (locker)
            {
                _resizeQueue.GetTasks<ResizeWorkerItem>().Where(item => item.ContactID == contactID)
                           .All(item =>
                               {
                                   _resizeQueue.RemoveTask(item.Id);

                                   return true;
                               });

                var photoDirectory = !isTmpDir
                                         ? BuildFileDirectory(contactID)
                                         : (String.IsNullOrEmpty(tmpDirName) ? BuildFileTmpDirectory(contactID) : BuildFileTmpDirectory(tmpDirName));
                var store = _global.GetStore();

                if (store.IsDirectory(photoDirectory))
                {
                    store.DeleteFiles(photoDirectory, "*", recursive);
                    if (recursive)
                    {
                        store.DeleteDirectory(photoDirectory);
                    }
                }

                if (!isTmpDir)
                {
                    _cache.Remove($"contact_photo_cache_key_id_{contactID}");
                    _cacheNotify.Publish(new ContactPhotoManagerCacheItem { Id = contactID }, CacheNotifyAction.Remove);
                }
            }
        }

        public void DeletePhoto(string tmpDirName)
        {
            lock (locker)
            {

                var photoDirectory = BuildFileTmpDirectory(tmpDirName);
                var store = _global.GetStore();

                if (store.IsDirectory(photoDirectory))
                {
                    store.DeleteFiles(photoDirectory, "*", false);
                }
            }
        }

        #endregion

        public void TryUploadPhotoFromTmp(int contactID, bool isNewContact, string tmpDirName)
        {
            var directoryPath = BuildFileDirectory(contactID);
            var dataStore = _global.GetStore();

            try
            {
                if (dataStore.IsDirectory(directoryPath))
                {
                    DeletePhoto(contactID, false, null, false);
                }
                foreach (var photoSize in new[] { _bigSize, _mediumSize, _smallSize })
                {
                    var photoTmpPath = FromDataStoreRelative(isNewContact ? 0 : contactID, photoSize, true, tmpDirName);
                    if (string.IsNullOrEmpty(photoTmpPath)) throw new Exception("Temp phono not found");

                    var imageExtension = Path.GetExtension(photoTmpPath);

                    var photoPath = String.Concat(directoryPath, BuildFileName(contactID, photoSize), imageExtension).TrimStart('/');

                    byte[] data;
                    using (var photoTmpStream = dataStore.GetReadStream(photoTmpPath))
                    {
                        data = Global.ToByteArray(photoTmpStream);
                    }
                    using (var fileStream = new MemoryStream(data))
                    {
                        var photoUri = dataStore.Save(photoPath, fileStream).ToString();
                        photoUri = String.Format("{0}?cd={1}", photoUri, DateTime.UtcNow.Ticks);
                        ToCache(contactID, photoUri, photoSize);
                    }
                }
                DeletePhoto(contactID, true, tmpDirName, true);
            }
            catch (Exception ex)
            {
                _logger.ErrorFormat("TryUploadPhotoFromTmp for contactID={0} failed witth error: {1}", contactID, ex);

                return;
            }
        }

        #region Get Photo Methods

        public String GetSmallSizePhoto(int contactID, bool isCompany)
        {
            if (contactID <= 0)
                return GetDefaultPhoto(isCompany, _smallSize);

            return GetPhotoUri(contactID, isCompany, _smallSize);
        }

        public String GetMediumSizePhoto(int contactID, bool isCompany)
        {
            if (contactID <= 0)
                return GetDefaultPhoto(isCompany, _mediumSize);

            return GetPhotoUri(contactID, isCompany, _mediumSize);
        }

        public String GetBigSizePhoto(int contactID, bool isCompany)
        {
            if (contactID <= 0)
                return GetDefaultPhoto(isCompany, _bigSize);

            return GetPhotoUri(contactID, isCompany, _bigSize);
        }

        #endregion

        private PhotoData ResizeToBigSize(byte[] imageData, string tmpDirName)
        {
            return ResizeToBigSize(imageData, 0, true, tmpDirName);
        }

        private PhotoData ResizeToBigSize(byte[] imageData, int contactID, bool uploadOnly, string tmpDirName)
        {
            var resizeWorkerItem = new ResizeWorkerItem
            {
                ContactID = contactID,
                UploadOnly = uploadOnly,
                RequireFotoSize = new[] { _bigSize },
                ImageData = imageData,
                DataStore = _global.GetStore(),
                TmpDirName = tmpDirName
            };

            ExecResizeImage(resizeWorkerItem);

            if (!uploadOnly)
            {
                return new PhotoData { Url = FromCache(contactID, _bigSize) };
            }
            else if (String.IsNullOrEmpty(tmpDirName))
            {
                return new PhotoData { Url = FromDataStore(contactID, _bigSize, true, null), Path = PhotosDefaultTmpDirName };
            }
            else
            {
                return FromDataStore(_bigSize, tmpDirName);
            }
        }

        private void ExecGenerateThumbnail(byte[] imageData, int contactID, bool uploadOnly)
        {
            ExecGenerateThumbnail(imageData, contactID, uploadOnly, null);
        }

        private void ExecGenerateThumbnail(byte[] imageData, string tmpDirName)
        {
            ExecGenerateThumbnail(imageData, 0, true, tmpDirName);
        }


        private void ExecGenerateThumbnail(byte[] imageData, int contactID, bool uploadOnly, string tmpDirName)
        {
            var resizeWorkerItem = new ResizeWorkerItem
            {
                ContactID = contactID,
                UploadOnly = uploadOnly,
                RequireFotoSize = new[] { _mediumSize, _smallSize },
                ImageData = imageData,
                DataStore = _global.GetStore(),
                TmpDirName = tmpDirName
            };

            if (!_resizeQueue.GetTasks<ResizeWorkerItem>().Contains(resizeWorkerItem))
            {
                //Add
                _resizeQueue.QueueTask((a, b) => ExecResizeImage(resizeWorkerItem), resizeWorkerItem);
            }
        }

        private byte[] ToByteArray(Stream inputStream, int streamLength)
        {
            using (var br = new BinaryReader(inputStream))
            {
                return br.ReadBytes(streamLength);
            }
        }

        #region UploadPhoto Methods

        public PhotoData UploadPhoto(String imageUrl, int contactID, bool uploadOnly, bool checkFormat = true)
        {
            var request = (HttpWebRequest)WebRequest.Create(imageUrl);
            using (var response = request.GetResponse())
            {
                using (var inputStream = response.GetResponseStream())
                {
                    var imageData = ToByteArray(inputStream, (int)response.ContentLength);
                    return UploadPhoto(imageData, contactID, uploadOnly, checkFormat);
                }
            }
        }

        public PhotoData UploadPhoto(Stream inputStream, int contactID, bool uploadOnly, bool checkFormat = true)
        {
            var imageData = Global.ToByteArray(inputStream);
            return UploadPhoto(imageData, contactID, uploadOnly, checkFormat);
        }

        public PhotoData UploadPhoto(byte[] imageData, int contactID, bool uploadOnly, bool checkFormat = true)
        {
            if (contactID == 0)
                throw new ArgumentException();

            if (checkFormat)
                CheckImgFormat(imageData);

            DeletePhoto(contactID, uploadOnly, null, false);

            ExecGenerateThumbnail(imageData, contactID, uploadOnly);

            return ResizeToBigSize(imageData, contactID, uploadOnly, null);
        }


        public PhotoData UploadPhotoToTemp(String imageUrl, String tmpDirName, bool checkFormat = true)
        {
            var request = (HttpWebRequest)WebRequest.Create(imageUrl);
            using (var response = request.GetResponse())
            {
                using (var inputStream = response.GetResponseStream())
                {
                    var imageData = ToByteArray(inputStream, (int)response.ContentLength);
                    if (string.IsNullOrEmpty(tmpDirName))
                    {
                        tmpDirName = Guid.NewGuid().ToString();
                    }
                    return UploadPhotoToTemp(imageData, tmpDirName, checkFormat);
                }
            }
        }

        public PhotoData UploadPhotoToTemp(Stream inputStream, String tmpDirName, bool checkFormat = true)
        {
            var imageData = Global.ToByteArray(inputStream);
            return UploadPhotoToTemp(imageData, tmpDirName, checkFormat);
        }

        public PhotoData UploadPhotoToTemp(byte[] imageData, String tmpDirName, bool checkFormat = true)
        {
            if (checkFormat)
                CheckImgFormat(imageData);

            DeletePhoto(tmpDirName);

            ExecGenerateThumbnail(imageData, tmpDirName);

            return ResizeToBigSize(imageData, tmpDirName);
        }

        public ImageFormat CheckImgFormat(byte[] imageData)
        {
            using (var stream = new MemoryStream(imageData))
            using (var img = new Bitmap(stream))
            {
                var format = img.RawFormat;

                if (!format.Equals(ImageFormat.Png) && !format.Equals(ImageFormat.Jpeg))
                    throw new Exception(CRMJSResource.ErrorMessage_NotImageSupportFormat);

                return format;
            }
        }

        public class PhotoData
        {
            public string Url;
            public string Path;
        }

        #endregion
    }
}