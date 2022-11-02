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

namespace ASC.Web.Studio.UserControls.CustomNavigation;

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

    private readonly UserPhotoManager _userPhotoManager;
    private readonly StorageFactory _storageFactory;
    private readonly TenantManager _tenantManager;
    private readonly ILogger<StorageHelper> _logger;

    public StorageHelper(UserPhotoManager userPhotoManager, StorageFactory storageFactory, TenantManager tenantManager, ILogger<StorageHelper> logger)
    {
        _userPhotoManager = userPhotoManager;
        _storageFactory = storageFactory;
        _tenantManager = tenantManager;
        _logger = logger;
    }

    public async Task<string> SaveTmpLogo(string tmpLogoPath)
    {
        if (string.IsNullOrEmpty(tmpLogoPath))
        {
            return null;
        }

        try
        {
            byte[] data;

            if (tmpLogoPath.StartsWith(Base64Start))
            {
                data = Convert.FromBase64String(tmpLogoPath.Substring(Base64Start.Length));

                return SaveLogo(Guid.NewGuid() + ".png", data);
            }

            var fileName = Path.GetFileName(tmpLogoPath);

            data = _userPhotoManager.GetTempPhotoData(fileName);

            await _userPhotoManager.RemoveTempPhoto(fileName);

            return SaveLogo(fileName, data);
        }
        catch (Exception ex)
        {
            _logger.ErrorSaveTmpLogo(ex);
            return null;
        }
    }

    public async Task DeleteLogo(string logoPath)
    {
        if (string.IsNullOrEmpty(logoPath))
        {
            return;
        }

        try
        {
            var store = _storageFactory.GetStorage(_tenantManager.GetCurrentTenant().Id, StorageName);

            var fileName = Path.GetFileName(logoPath);

            if (await store.IsFileAsync(fileName))
            {
               await store.DeleteAsync(fileName);
            }
        }
        catch (Exception e)
        {
            _logger.ErrorDeleteLogo(e);
        }
    }

    private string SaveLogo(string fileName, byte[] data)
    {
        var store = _storageFactory.GetStorage(_tenantManager.GetCurrentTenant().Id, StorageName);

        using var stream = new MemoryStream(data);
        stream.Seek(0, SeekOrigin.Begin);
        return store.SaveAsync(fileName, stream).Result.ToString();
    }
}
