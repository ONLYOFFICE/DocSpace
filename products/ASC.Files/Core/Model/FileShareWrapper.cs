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

using ASC.Common;
using ASC.Core;
using ASC.Files.Core;
using ASC.Files.Core.Security;
using ASC.Web.Api.Models;
using ASC.Web.Files.Services.WCFService;

namespace ASC.Api.Documents
{
    /// <summary>
    /// </summary>
    public class FileShareWrapper
    {
        /// <summary>
        /// </summary>
        /// <param name="aceWrapper"></param>
        public FileShareWrapper()
        {

        }

        /// <summary>
        /// </summary>
        public FileShare Access { get; set; }

        /// <summary>
        /// </summary>
        public object SharedTo { get; set; }

        /// <summary>
        /// </summary>
        public bool IsLocked { get; set; }

        /// <summary>
        /// </summary>
        public bool IsOwner { get; set; }

        /// <summary>
        /// </summary>
        /// <returns></returns>
        public static FileShareWrapper GetSample()
        {
            return new FileShareWrapper
            {
                Access = FileShare.ReadWrite,
                IsLocked = false,
                IsOwner = true,
                //SharedTo = EmployeeWraper.GetSample()
            };
        }
    }


    /// <summary>
    /// </summary>
    public class FileShareLink
    {
        /// <summary> 
        /// </summary>
        public Guid Id { get; set; }

        /// <summary> 
        /// </summary>
        public string ShareLink { get; set; }
    }

    [Scope]
    public class FileShareWrapperHelper
    {
        private UserManager UserManager { get; }
        private EmployeeWraperFullHelper EmployeeWraperFullHelper { get; }

        public FileShareWrapperHelper(
            UserManager userManager,
            EmployeeWraperFullHelper employeeWraperFullHelper)
        {
            UserManager = userManager;
            EmployeeWraperFullHelper = employeeWraperFullHelper;
        }

        public FileShareWrapper Get(AceWrapper aceWrapper)
        {
            var result = new FileShareWrapper
            {
                IsOwner = aceWrapper.Owner,
                IsLocked = aceWrapper.LockedRights
            };

            if (aceWrapper.SubjectGroup)
            {
                if (aceWrapper.SubjectId == FileConstant.ShareLinkId)
                {
                    result.SharedTo = new FileShareLink
                    {
                        Id = aceWrapper.SubjectId,
                        ShareLink = aceWrapper.Link
                    };
                }
                else
                {
                    //Shared to group
                    result.SharedTo = new GroupWrapperSummary(UserManager.GetGroupInfo(aceWrapper.SubjectId), UserManager);
                }
            }
            else
            {
                result.SharedTo = EmployeeWraperFullHelper.GetFull(UserManager.GetUsers(aceWrapper.SubjectId));
            }
            result.Access = aceWrapper.Share;

            return result;
        }
    }
}