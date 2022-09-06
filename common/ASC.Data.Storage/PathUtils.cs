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

namespace ASC.Data.Storage;

[Singletone]
public class PathUtils
{
    public IHostEnvironment HostEnvironment { get; }

    private readonly string _storageRoot;
    private readonly IConfiguration _configuration;
    private readonly IWebHostEnvironment _webHostEnvironment;

    public PathUtils(IConfiguration configuration, IHostEnvironment hostEnvironment)
    {
        _configuration = configuration;
        HostEnvironment = hostEnvironment;
        _storageRoot = _configuration[Constants.StorageRootParam];
    }

    public PathUtils(
        IConfiguration configuration,
        IHostEnvironment hostEnvironment,
        IWebHostEnvironment webHostEnvironment) : this(configuration, hostEnvironment)
    {
        _webHostEnvironment = webHostEnvironment;
    }

    public static string Normalize(string path, bool addTailingSeparator = false)
    {
        path = path
            .Replace('/', Path.DirectorySeparatorChar)
            .Replace('\\', Path.DirectorySeparatorChar)
            .Replace("\\\\", Path.DirectorySeparatorChar.ToString())
            .Replace("//", Path.DirectorySeparatorChar.ToString())
            .TrimEnd(Path.DirectorySeparatorChar);

        return addTailingSeparator && 0 < path.Length ? path + Path.DirectorySeparatorChar : path;
    }

    public string ResolveVirtualPath(string module, string domain)
    {
        var url = $"~/storage/{module}/{(string.IsNullOrEmpty(domain) ? "root" : domain)}/";

        return ResolveVirtualPath(url);
    }

    public string ResolveVirtualPath(string virtPath, bool addTrailingSlash = true)
    {
        if (virtPath == null)
        {
            virtPath = "";
        }

        if (virtPath.StartsWith('~') && !Uri.IsWellFormedUriString(virtPath, UriKind.Absolute))
        {
            var rootPath = "/";
            if (!string.IsNullOrEmpty(_webHostEnvironment?.WebRootPath) && _webHostEnvironment?.WebRootPath.Length > 1)
            {
                rootPath = _webHostEnvironment?.WebRootPath.Trim('/');
            }

            virtPath = virtPath.Replace("~", rootPath);
        }
        if (addTrailingSlash)
        {
            virtPath += "/";
        }
        else
        {
            virtPath = virtPath.TrimEnd('/');
        }

        return virtPath.Replace("//", "/");
    }

    public string ResolvePhysicalPath(string physPath, IDictionary<string, string> storageConfig)
    {
        physPath = Normalize(physPath, false).TrimStart('~');

        if (physPath.Contains(Constants.StorageRootParam))
        {
            physPath = physPath.Replace(Constants.StorageRootParam, _storageRoot ?? storageConfig[Constants.StorageRootParam]);
        }

        if (!Path.IsPathRooted(physPath))
        {
            physPath = Path.GetFullPath(CrossPlatform.PathCombine(HostEnvironment.ContentRootPath, physPath.Trim(Path.DirectorySeparatorChar)));
        }

        return physPath;
    }
}
