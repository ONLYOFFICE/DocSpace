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

public class LogoUploader
{
    public LogoUploader(RequestDelegate next)
    {

    }

    public async Task Invoke
        (HttpContext context,
        PermissionContext permissionContext,
        SetupInfo setupInfo,
        UserPhotoManager userPhotoManager)
    {
        var result = new FileUploadResult();
        try
        {
            await permissionContext.DemandPermissionsAsync(SecutiryConstants.EditPortalSettings);

            var width = Convert.ToInt32(context.Request.Form["width"]);
            var height = Convert.ToInt32(context.Request.Form["height"]);
            var size = new Size(width, height);

            if (context.Request.Form.Files.Count != 0)
            {
                const string imgContentType = @"image";

                var logo = context.Request.Form.Files[0];
                if (!logo.ContentType.StartsWith(imgContentType))
                {
                    throw new Exception(Resource.ErrorFileNotImage);
                }

                var data = new byte[logo.Length];

                var reader = new BinaryReader(logo.OpenReadStream());
                reader.Read(data, 0, (int)logo.Length);
                reader.Close();

                if (logo.ContentType.Contains("image/x-icon"))
                {
                    result.Success = true;
                    result.Message = await userPhotoManager.SaveTempPhoto(data, setupInfo.MaxImageUploadSize, "ico");
                }
                else if (logo.ContentType.Contains("image/svg+xml"))
                {
                    result.Success = true;
                    result.Message = await userPhotoManager.SaveTempPhoto(data, setupInfo.MaxImageUploadSize, "svg");
                }
                else
                {
                    using (var stream = new MemoryStream(data))
                    using (var image = Image.Load(stream))
                    {
                        var actualSize = image.Size;
                        if (actualSize.Height != size.Height && actualSize.Width != size.Width)
                        {
                            throw new ImageSizeLimitException();
                        }
                    }
                    result.Success = true;
                    result.Message = await userPhotoManager.SaveTempPhoto(data, setupInfo.MaxImageUploadSize, size.Width, size.Height);
                }
            }
            else
            {
                result.Success = false;
                result.Message = Resource.ErrorEmptyUploadFileSelected;
            }
        }
        catch (ImageWeightLimitException)
        {
            result.Success = false;
            result.Message = Resource.ErrorImageWeightLimit;
        }
        catch (ImageSizeLimitException)
        {
            result.Success = false;
            result.Message = Resource.ErrorImageSize;
        }
        catch (Exception ex)
        {
            result.Success = false;
            result.Message = ex.Message.HtmlEncode();
        }
        await context.Response.WriteAsync(System.Text.Json.JsonSerializer.Serialize(result));
    }
}

public static class LogoUploaderExtensions
{
    public static IApplicationBuilder UseLogoUploader(this IApplicationBuilder builder)
    {
        return builder.UseMiddleware<LogoUploader>();
    }
}