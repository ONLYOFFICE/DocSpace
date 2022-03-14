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
using System.Globalization;
using System.IO;

using ASC.Common;
using ASC.Common.Logging;
using ASC.Core;
using ASC.Data.Storage;
using ASC.Web.Core.Users;

using Microsoft.Extensions.Options;

namespace ASC.Web.Studio.UserControls.CustomNavigation
{
    //internal class LogoUploader : IFileUploadHandler
    //{
    //    public FileUploadResult ProcessUpload(HttpContext context)
    //    {
    //        var result = new FileUploadResult();
    //        try
    //        {
    //            SecurityContext.DemandPermissions(SecutiryConstants.EditPortalSettings);

    //            var width = Convert.ToInt32(context.Request["size"]);
    //            var size = new Size(width, width);

    //            if (context.Request.Files.Count != 0)
    //            {
    //                const string imgContentType = @"image";

    //                var logo = context.Request.Files[0];
    //                if (!logo.ContentType.StartsWith(imgContentType))
    //                {
    //                    throw new Exception(WhiteLabelResource.ErrorFileNotImage);
    //                }

    //                var data = new byte[logo.InputStream.Length];

    //                var reader = new BinaryReader(logo.InputStream);
    //                reader.Read(data, 0, (int) logo.InputStream.Length);
    //                reader.Close();

    //                using (var stream = new MemoryStream(data))
    //                using (var image = Image.FromStream(stream))
    //                {
    //                    var actualSize = image.Size;
    //                    if (actualSize.Height != size.Height || actualSize.Width != size.Width)
    //                    {
    //                        throw new ImageSizeLimitException();
    //                    }
    //                }

    //                result.Success = true;
    //                result.Message = UserPhotoManager.SaveTempPhoto(data, SetupInfo.MaxImageUploadSize, size.Width,
    //                    size.Height);
    //            }
    //            else
    //            {
    //                result.Success = false;
    //                result.Message = Resource.ErrorEmptyUploadFileSelected;
    //            }
    //        }
    //        catch (ImageWeightLimitException)
    //        {
    //            result.Success = false;
    //            result.Message = Resource.ErrorImageWeightLimit;
    //        }
    //        catch (ImageSizeLimitException)
    //        {
    //            result.Success = false;
    //            result.Message = WhiteLabelResource.ErrorImageSize;
    //        }
    //        catch (Exception ex)
    //        {
    //            result.Success = false;
    //            result.Message = ex.Message.HtmlEncode();
    //        }

    //        return result;
    //    }
    //}

    [Scope]
    public class StorageHelper
    {
        private const string StorageName = "customnavigation";

        private const string Base64Start = "data:image/png;base64,";

        private UserPhotoManager UserPhotoManager { get; }
        private StorageFactory StorageFactory { get; }
        private TenantManager TenantManager { get; }
        public ILog Log { get; set; }

        public StorageHelper(UserPhotoManager userPhotoManager, StorageFactory storageFactory, TenantManager tenantManager, IOptionsMonitor<ILog> options)
        {
            UserPhotoManager = userPhotoManager;
            StorageFactory = storageFactory;
            TenantManager = tenantManager;
            Log = options.CurrentValue;
        }

        public string SaveTmpLogo(string tmpLogoPath)
        {
            if (string.IsNullOrEmpty(tmpLogoPath)) return null;

            try
            {
                byte[] data;

                if (tmpLogoPath.StartsWith(Base64Start))
                {
                    data = Convert.FromBase64String(tmpLogoPath.Substring(Base64Start.Length));

                    return SaveLogo(Guid.NewGuid() + ".png", data);
                }

                var fileName = Path.GetFileName(tmpLogoPath);

                data = UserPhotoManager.GetTempPhotoData(fileName);

                UserPhotoManager.RemoveTempPhoto(fileName);

                return SaveLogo(fileName, data);
            }
            catch (Exception ex)
            {
                Log.Error(ex);
                return null;
            }
        }

        public void DeleteLogo(string logoPath)
        {
            if (string.IsNullOrEmpty(logoPath)) return;

            try
            {
                var store = StorageFactory.GetStorage(TenantManager.GetCurrentTenant().TenantId.ToString(CultureInfo.InvariantCulture), StorageName);

                var fileName = Path.GetFileName(logoPath);

                if (store.IsFileAsync(fileName).Result)
                {
                    store.DeleteAsync(fileName).Wait();
                }
            }
            catch (Exception e)
            {
                Log.Error(e);
            }
        }

        private string SaveLogo(string fileName, byte[] data)
        {
            var store = StorageFactory.GetStorage(TenantManager.GetCurrentTenant().TenantId.ToString(CultureInfo.InvariantCulture), StorageName);

            using var stream = new MemoryStream(data);
            stream.Seek(0, SeekOrigin.Begin);
            return store.SaveAsync(fileName, stream).Result.ToString();
        }
    }
}