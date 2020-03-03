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
using System.Runtime.Serialization;

using ASC.Api.Core;
using ASC.Common;
using ASC.Core;
using ASC.Files.Core;
using ASC.Files.Core.Data;
using ASC.Files.Core.Security;
using ASC.Web.Api.Models;
using ASC.Web.Files.Classes;

namespace ASC.Api.Documents
{
    /// <summary>
    /// </summary>
    [DataContract(Name = "folder", Namespace = "")]
    public class FolderWrapper : FileEntryWrapper
    {
        /// <summary>
        /// </summary>
        [DataMember(IsRequired = true, EmitDefaultValue = true)]
        public object ParentId { get; set; }

        /// <summary>
        /// </summary>
        [DataMember(EmitDefaultValue = true, IsRequired = false)]
        public int FilesCount { get; set; }

        /// <summary>
        /// </summary>
        [DataMember(EmitDefaultValue = true, IsRequired = false)]
        public int FoldersCount { get; set; }

        /// <summary>
        /// </summary>
        [DataMember(EmitDefaultValue = false, IsRequired = false)]
        public bool IsShareable { get; set; }

        /// <summary>
        /// </summary>
        /// <param name="folder"></param>
        public FolderWrapper()
        {
        }

        /// <summary>
        /// </summary>
        /// <returns></returns>
        public static FolderWrapper GetSample()
        {
            return new FolderWrapper
            {
                Access = FileShare.ReadWrite,
                //Updated = ApiDateTime.GetSample(),
                //Created = ApiDateTime.GetSample(),
                //CreatedBy = EmployeeWraper.GetSample(),
                Id = new Random().Next(),
                RootFolderType = FolderType.BUNCH,
                Shared = false,
                Title = "Some titile",
                //UpdatedBy = EmployeeWraper.GetSample(),
                FilesCount = new Random().Next(),
                FoldersCount = new Random().Next(),
                ParentId = new Random().Next(),
                IsShareable = false
            };
        }
    }

    public class FolderWrapperHelper : FileEntryWrapperHelper
    {
        public AuthContext AuthContext { get; }
        public IDaoFactory DaoFactory { get; }
        public FileSecurity FileSecurity { get; }
        public GlobalFolderHelper GlobalFolderHelper { get; }

        public FolderWrapperHelper(
            ApiDateTimeHelper apiDateTimeHelper,
            EmployeeWraperHelper employeeWrapperHelper,
            AuthContext authContext,
            IDaoFactory daoFactory,
            FileSecurity fileSecurity,
            GlobalFolderHelper globalFolderHelper)
            : base(apiDateTimeHelper, employeeWrapperHelper)
        {
            AuthContext = authContext;
            DaoFactory = daoFactory;
            FileSecurity = fileSecurity;
            GlobalFolderHelper = globalFolderHelper;
        }

        public FolderWrapper Get<T>(Folder<T> folder)
        {
            var result = Get<FolderWrapper>(folder);
            result.ParentId = folder.ParentFolderID;
            if (folder.RootFolderType == FolderType.USER
                && !Equals(folder.RootFolderCreator, AuthContext.CurrentAccount.ID))
            {
                result.RootFolderType = FolderType.SHARE;

                var folderDao = DaoFactory.GetFolderDao<T>();
                var parentFolder = folderDao.GetFolder(folder.ParentFolderID);
                if (!FileSecurity.CanRead<T>(parentFolder))
                    result.ParentId = GlobalFolderHelper.FolderShare;
            }

            result.FilesCount = folder.TotalFiles;
            result.FoldersCount = folder.TotalSubFolders;
            result.IsShareable = folder.Shareable;

            return result;
        }
    }

    public static class FolderWrapperHelperExtention
    {
        public static DIHelper AddFolderWrapperHelperService(this DIHelper services)
        {
            services.TryAddScoped<FolderWrapperHelper>();

            return services
                .AddFileEntryWrapperHelperService()
                .AddAuthContextService()
                .AddDaoFactoryService()
                .AddFileSecurityService()
                .AddGlobalFolderHelperService();
        }
    }
}