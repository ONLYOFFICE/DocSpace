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


#region Import

using System;

using ASC.Common;
using ASC.Web.Studio.Utility;

#endregion

namespace ASC.Web.CRM
{
    [Scope]
    public class PathProvider
    {
//        private HttpContext _httpContext;
        private CommonLinkUtility _commonLinkUtility;
//        private WebPath _webPath;

        public PathProvider(CommonLinkUtility commonLinkUtility)
        {
            _commonLinkUtility = commonLinkUtility;
//            _httpContext = httpContextAccessor.HttpContext;

            BaseAbsolutePath = _commonLinkUtility.ToAbsolute(BaseVirtualPath);
        }


        public readonly String BaseVirtualPath = "/products/crm/";
        public readonly String BaseAbsolutePath;

        public String StartURL()
        {
            return BaseVirtualPath;
        }

        //public string BaseSiteUrl
        //{
        //    get
        //    {               
        //        string baseUrl = HttpContext.Request.GetUrlRewriter().Scheme + "://" + HttpContext.Request.GetUrlRewriter().Authority + HttpContext.Request.ApplicationPath.TrimEnd('/') + '/';
        //        return baseUrl;
        //    }
        //}

        //public  string GetVirtualPath(string physicalPath)
        //{
        //    string rootpath = HttpContext.Server.MapPath("~/");
        //    physicalPath = physicalPath.Replace(rootpath, "");
        //    physicalPath = physicalPath.Replace("\\", "/");

        //    return "~/" + physicalPath;
        //}

        // TODO: Remove GetFileStaticRelativePath method
        //public  String GetFileStaticRelativePath(String fileName)
        //{
        //    if (fileName.EndsWith(".js"))
        //    {
        //        //Attention: Only for ResourceBundleControl
        //        return VirtualPathUtility.ToAbsolute("~/Products/CRM/js/" + fileName);
        //    }
        //    if (fileName.EndsWith(".ascx"))
        //    {
        //        return VirtualPathUtility.ToAbsolute("~/Products/CRM/Controls/" + fileName);
        //    }
        //    if (fileName.EndsWith(".css") || fileName.EndsWith(".less"))
        //    {
        //        //Attention: Only for ResourceBundleControl
        //        return VirtualPathUtility.ToAbsolute("~/Products/CRM/App_Themes/default/css/" + fileName);
        //    }
        //    if (fileName.EndsWith(".png") || fileName.EndsWith(".gif") || fileName.EndsWith(".jpg"))
        //    {
        //        return WebPath.GetPath("/Products/CRM/App_Themes/default/images/" + fileName);
        //    }

        //    return fileName;
        //}

    }

}