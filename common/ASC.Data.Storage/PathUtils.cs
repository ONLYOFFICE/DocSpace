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
using System.IO;

using ASC.Common;
using ASC.Common.Utils;

using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;

namespace ASC.Data.Storage
{
    [Singletone]
    public class PathUtils
    {
        private string StorageRoot { get; }
        private IConfiguration Configuration { get; }
        public IHostEnvironment HostEnvironment { get; }
        private IWebHostEnvironment WebHostEnvironment { get; }

        public PathUtils(IConfiguration configuration, IHostEnvironment hostEnvironment)
        {
            Configuration = configuration;
            HostEnvironment = hostEnvironment;
            StorageRoot = Configuration[Constants.STORAGE_ROOT_PARAM];
        }

        public PathUtils(IConfiguration configuration, IHostEnvironment hostEnvironment, IWebHostEnvironment webHostEnvironment) : this(configuration, hostEnvironment)
        {
            WebHostEnvironment = webHostEnvironment;
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
                if (!string.IsNullOrEmpty(WebHostEnvironment?.WebRootPath) && WebHostEnvironment?.WebRootPath.Length > 1)
                {
                    rootPath = WebHostEnvironment?.WebRootPath.Trim('/');
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

            if (physPath.Contains(Constants.STORAGE_ROOT_PARAM))
            {
                physPath = physPath.Replace(Constants.STORAGE_ROOT_PARAM, StorageRoot ?? storageConfig[Constants.STORAGE_ROOT_PARAM]);
            }

            if (!Path.IsPathRooted(physPath))
            {
                physPath = Path.GetFullPath(CrossPlatform.PathCombine(HostEnvironment.ContentRootPath, physPath.Trim(Path.DirectorySeparatorChar)));
            }
            return physPath;
        }
    }
}
