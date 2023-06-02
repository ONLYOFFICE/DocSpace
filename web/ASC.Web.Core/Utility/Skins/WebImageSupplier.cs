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

namespace ASC.Web.Core.Utility.Skins;

[Scope]
public class WebImageSupplier
{
    private readonly string _folderName;
    private readonly WebItemManager _webItemManager;
    private readonly WebPath _webPath;
    private readonly IHttpContextAccessor _httpContextAccessor;

    public WebImageSupplier(WebItemManager webItemManager, WebPath webPath, IConfiguration configuration)
    {
        _webItemManager = webItemManager;
        _webPath = webPath;
        _folderName = configuration["web:images"];
    }
    public WebImageSupplier(WebItemManager webItemManager, WebPath webPath, IHttpContextAccessor httpContextAccessor, IConfiguration configuration)
        : this(webItemManager, webPath, configuration)
    {
        _httpContextAccessor = httpContextAccessor;
    }

    public string GetAbsoluteWebPath(string imgFileName)
    {
        return GetAbsoluteWebPath(imgFileName, Guid.Empty);
    }

    public string GetAbsoluteWebPath(string imgFileName, Guid moduleID)
    {
        return GetImageAbsoluteWebPath(imgFileName, moduleID);
    }


    private string GetImageAbsoluteWebPath(string fileName, Guid partID)
    {
        if (string.IsNullOrEmpty(fileName))
        {
            return string.Empty;
        }
        var filepath = GetPartImageFolderRel(partID) + "/" + fileName;
        return _webPath.GetPathAsync(filepath).Result;
    }

    private string GetPartImageFolderRel(Guid partID)
    {
        var folderName = _folderName;
        string itemFolder = null;
        if (!Guid.Empty.Equals(partID))
        {
            var product = _webItemManager[partID];
            if (product != null && product.Context != null)
            {
                itemFolder = GetAppThemeVirtualPath(product) + "/default/images";
            }

            folderName = itemFolder ?? folderName;
        }
        return folderName.TrimStart('~');
    }

    private static string GetAppThemeVirtualPath(IWebItem webitem)
    {
        if (webitem == null || string.IsNullOrEmpty(webitem.StartURL))
        {
            return string.Empty;
        }

        var dir = webitem.StartURL.Contains('.') ?
                      webitem.StartURL.Substring(0, webitem.StartURL.LastIndexOf('/')) :
                      webitem.StartURL.TrimEnd('/');
        return dir + "/App_Themes";
    }
}
