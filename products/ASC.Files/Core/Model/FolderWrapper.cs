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
using System.Linq;
using System.Threading.Tasks;

using ASC.Api.Core;
using ASC.Api.Utils;
using ASC.Common;
using ASC.Core;
using ASC.Files.Core;
using ASC.Files.Core.Security;
using ASC.Web.Api.Models;
using ASC.Web.Files.Classes;
using ASC.Web.Files.Utils;

namespace ASC.Api.Documents
{
    /// <summary>
    /// </summary>
    public class FolderWrapper<T> : FileEntryWrapper<T>
    {
        /// <summary>
        /// </summary>
        public T ParentId { get; set; }

        /// <summary>
        /// </summary>
        public int FilesCount { get; set; }

        /// <summary>
        /// </summary>
        public int FoldersCount { get; set; }

        /// <summary>
        /// </summary>
        public bool? IsShareable { get; set; }

        public int New { get; set; }

        /// <summary>
        /// </summary>
        /// <param name="folder"></param>
        public FolderWrapper()
        {
        }

        /// <summary>
        /// </summary>
        /// <returns></returns>
        public static FolderWrapper<int> GetSample()
        {
            return new FolderWrapper<int>
            {
                Access = FileShare.ReadWrite,
                //Updated = ApiDateTime.GetSample(),
                //Created = ApiDateTime.GetSample(),
                //CreatedBy = EmployeeWraper.GetSample(),
                Id = 10,
                RootFolderType = FolderType.BUNCH,
                Shared = false,
                Title = "Some titile",
                //UpdatedBy = EmployeeWraper.GetSample(),
                FilesCount = 5,
                FoldersCount = 7,
                ParentId = 10,
                IsShareable = null
            };
        }
    }

    [Scope]
    public class FolderWrapperHelper : FileEntryWrapperHelper
    {
        private AuthContext AuthContext { get; }
        private IDaoFactory DaoFactory { get; }
        private GlobalFolderHelper GlobalFolderHelper { get; }

        public FolderWrapperHelper(
            ApiDateTimeHelper apiDateTimeHelper,
            EmployeeWraperHelper employeeWrapperHelper,
            AuthContext authContext,
            IDaoFactory daoFactory,
            FileSecurity fileSecurity,
            GlobalFolderHelper globalFolderHelper,
            FileSharingHelper fileSharingHelper)
            : base(apiDateTimeHelper, employeeWrapperHelper, fileSharingHelper, fileSecurity)
        {
            AuthContext = authContext;
            DaoFactory = daoFactory;
            GlobalFolderHelper = globalFolderHelper;
        }

        public async Task<FolderWrapper<T>> GetAsync<T>(Folder<T> folder, List<Tuple<FileEntry<T>, bool>> folders = null)
        {
            var result = await GetFolderWrapperAsync(folder);

            result.ParentId = folder.FolderID;

            if (folder.RootFolderType == FolderType.USER
                && !Equals(folder.RootFolderCreator, AuthContext.CurrentAccount.ID))
            {
                result.RootFolderType = FolderType.SHARE;

                var folderDao = DaoFactory.GetFolderDao<T>();
                FileEntry<T> parentFolder;

                if (folders != null)
                {
                    var folderWithRight = folders.FirstOrDefault(f => f.Item1.ID.Equals(folder.FolderID));
                    if (folderWithRight == null || !folderWithRight.Item2)
                    {
                        result.ParentId = await GlobalFolderHelper.GetFolderShareAsync<T>();
                    }
                }
                else
                {
                    parentFolder = await folderDao.GetFolderAsync(folder.FolderID);
                    var canRead = await FileSecurity.CanReadAsync(parentFolder);
                    if (!canRead)
                    {
                        result.ParentId = await GlobalFolderHelper.GetFolderShareAsync<T>();
                    }
                }
            }

            return result;
        }

        private async Task<FolderWrapper<T>> GetFolderWrapperAsync<T>(Folder<T> folder)
        {
            var result = await GetAsync<FolderWrapper<T>, T>(folder);
            result.FilesCount = folder.TotalFiles;
            result.FoldersCount = folder.TotalSubFolders;
            result.IsShareable = folder.Shareable.NullIfDefault();
            result.New = folder.NewForMe;
            return result;
        }
    }
}