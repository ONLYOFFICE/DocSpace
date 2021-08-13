/*
 *
 * (c) Copyright Ascensio System Limited 2010-2021
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
 * Pursuant to Section 7 � 3(b) of the GNU GPL you must retain the original ONLYOFFICE logo which contains 
 * relevant author attributions when distributing the software. If the display of the logo in its graphic 
 * form is not reasonably feasible for technical reasons, you must include the words "Powered by ONLYOFFICE" 
 * in every copy of the program you distribute. 
 * Pursuant to Section 7 � 3(e) we decline to grant you any rights under trademark law for use of our trademarks.
 *
*/

using System;

using ASC.Common;
using ASC.Core.Common;

namespace ASC.Projects.Classes
{
    [Scope]
    public class PathProvider
    {
        public string BaseVirtualPath { get; private set; }
        public string BaseAbsolutePath { get; private set; }

        public PathProvider(BaseCommonLinkUtility commonLinkUtility)
        {
            BaseVirtualPath = "~/Products/Projects/";
            try
            {
                BaseAbsolutePath = commonLinkUtility.ToAbsolute(BaseVirtualPath);
            }
            catch (Exception)
            {
                BaseAbsolutePath = BaseVirtualPath;
            }
        }

        //public static string GetFileStaticRelativePath(String fileName)
        //{
        //    var ext = FileUtility.GetFileExtension(fileName);
        //    switch (ext)
        //    {
        //        case ".js":
        //            return VirtualPathUtility.ToAbsolute("~/Products/Projects/js/" + fileName);
        //        case ".png":
        //            return WebPath.GetPath("/Products/Projects/App_Themes/Default/images/" + fileName);
        //        case ".ascx":
        //            return CommonLinkUtility.ToAbsolute("~/Products/Projects/Controls/" + fileName);
        //        case ".css":
        //        case ".less":
        //            return VirtualPathUtility.ToAbsolute("~/Products/Projects/App_Themes/default/css/" + fileName);
        //    }
        //    return fileName;
        //}

        //public static string GetVirtualPath(string physicalPath)
        //{
        //    var rootpath = HttpContext.Current.Server.MapPath("~/");
        //    return "~/" + physicalPath.Replace(rootpath, string.Empty).Replace("\\", "/");
        //}
    }
}