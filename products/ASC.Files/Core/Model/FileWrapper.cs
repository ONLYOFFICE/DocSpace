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
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;

using ASC.Api.Core;
using ASC.Api.Utils;
using ASC.Common;
using ASC.Core;
using ASC.Files.Core;
using ASC.Files.Core.Security;
using ASC.Web.Api.Models;
using ASC.Web.Core.Files;
using ASC.Web.Files.Classes;
using ASC.Web.Files.Utils;
using ASC.Web.Studio.Utility;

using FileShare = ASC.Files.Core.Security.FileShare;
namespace ASC.Api.Documents
{
    /// <summary>
    /// </summary>
    public class FileWrapper<T> : FileEntryWrapper<T>
    {
        /// <summary>
        /// </summary>
        public T FolderId { get; set; }

        /// <summary>
        /// </summary>
        public int Version { get; set; }

        /// <summary>
        /// </summary>
        public int VersionGroup { get; set; }

        /// <summary>
        /// </summary>
        public string ContentLength { get; set; }

        /// <summary>
        /// </summary>
        public long? PureContentLength { get; set; }

        /// <summary>
        /// </summary>
        public FileStatus FileStatus { get; set; }

        /// <summary>
        /// </summary>
        public string ViewUrl { get; set; }

        /// <summary>
        /// </summary>
        public string WebUrl { get; set; }

        /// <summary>
        ///     
        /// </summary>
        public FileType FileType { get; set; }

        /// <summary>
        ///     
        /// </summary>
        public string FileExst { get; set; }

        /// <summary>
        /// </summary>
        public string Comment { get; set; }

        /// <summary>
        /// </summary>
        public bool? Encrypted { get; set; }

        /// <summary>
        /// </summary>
        public string ThumbnailUrl { get; set; }


        public Thumbnail ThumbnailStatus { get; set; }

        /// <summary>
        /// </summary>
        public bool? Locked { get; set; }

        /// <summary>
        /// </summary>
        public string LockedBy { get; set; }

        public bool CanWebRestrictedEditing { get; set; }

        public bool CanFillForms { get; set; }

        /// <summary>
        /// </summary>
        /// <param name="file"></param>
        public FileWrapper()
        {
        }

        /// <summary>
        /// </summary>
        /// <returns></returns>
        public static FileWrapper<int> GetSample()
        {
            return new FileWrapper<int>
            {
                Access = FileShare.ReadWrite,
                //Updated = ApiDateTime.GetSample(),
                //Created = ApiDateTime.GetSample(),
                //CreatedBy = EmployeeWraper.GetSample(),
                Id = 10,
                RootFolderType = FolderType.BUNCH,
                Shared = false,
                Title = "Some titile.txt",
                FileExst = ".txt",
                FileType = FileType.Document,
                //UpdatedBy = EmployeeWraper.GetSample(),
                ContentLength = 12345.ToString(CultureInfo.InvariantCulture),
                FileStatus = FileStatus.IsNew,
                FolderId = 12334,
                Version = 3,
                VersionGroup = 1,
                ViewUrl = "http://www.onlyoffice.com/viewfile?fileid=2221"
            };
        }
    }

    [Scope]
    public class FileWrapperHelper : FileEntryWrapperHelper
    {
        private AuthContext AuthContext { get; }
        private IDaoFactory DaoFactory { get; }
        private GlobalFolderHelper GlobalFolderHelper { get; }
        private CommonLinkUtility CommonLinkUtility { get; }
        private FilesLinkUtility FilesLinkUtility { get; }
        private FileUtility FileUtility { get; }

        public FileWrapperHelper(
            ApiDateTimeHelper apiDateTimeHelper,
            EmployeeWraperHelper employeeWrapperHelper,
            AuthContext authContext,
            IDaoFactory daoFactory,
            FileSecurity fileSecurity,
            GlobalFolderHelper globalFolderHelper,
            CommonLinkUtility commonLinkUtility,
            FilesLinkUtility filesLinkUtility,
            FileUtility fileUtility,
            FileSharingHelper fileSharingHelper)
            : base(apiDateTimeHelper, employeeWrapperHelper, fileSharingHelper, fileSecurity)
        {
            AuthContext = authContext;
            DaoFactory = daoFactory;
            GlobalFolderHelper = globalFolderHelper;
            CommonLinkUtility = commonLinkUtility;
            FilesLinkUtility = filesLinkUtility;
            FileUtility = fileUtility;
        }

        public async Task<FileWrapper<T>> GetAsync<T>(File<T> file, List<Tuple<FileEntry<T>, bool>> folders = null)
        {
            var result = await GetFileWrapperAsync(file);

            result.FolderId = file.FolderID;
            if (file.RootFolderType == FolderType.USER
                && !Equals(file.RootFolderCreator, AuthContext.CurrentAccount.ID))
            {
                result.RootFolderType = FolderType.SHARE;
                var folderDao = DaoFactory.GetFolderDao<T>();
                FileEntry<T> parentFolder;

                if (folders != null)
                {
                    var folderWithRight = folders.FirstOrDefault(f => f.Item1.ID.Equals(file.FolderID));
                    if (folderWithRight == null || !folderWithRight.Item2)
                    {
                        result.FolderId = await GlobalFolderHelper.GetFolderShareAsync<T>();
                    }
                }
                else
                {
                    parentFolder = await folderDao.GetFolderAsync(file.FolderID);
                    if (!await FileSecurity.CanReadAsync(parentFolder))
                    {
                        result.FolderId = await GlobalFolderHelper.GetFolderShareAsync<T>();
                    }
                }
            }


            return result;
        }

        private async Task<FileWrapper<T>> GetFileWrapperAsync<T>(File<T> file)
        {
            var result = await GetAsync<FileWrapper<T>, T>(file);

            result.FileExst = FileUtility.GetFileExtension(file.Title);
            result.FileType = FileUtility.GetFileTypeByExtention(result.FileExst);
            result.Version = file.Version;
            result.VersionGroup = file.VersionGroup;
            result.ContentLength = file.ContentLengthString;
            result.FileStatus = file.FileStatus;
            result.PureContentLength = file.ContentLength.NullIfDefault();
            result.Comment = file.Comment;
            result.Encrypted = file.Encrypted.NullIfDefault();
            result.Locked = file.Locked.NullIfDefault();
            result.LockedBy = file.LockedBy;
            result.CanWebRestrictedEditing = FileUtility.CanWebRestrictedEditing(file.Title);
            result.CanFillForms = await FileSecurity.CanFillFormsAsync(file);

            try
            {
                result.ViewUrl = CommonLinkUtility.GetFullAbsolutePath(file.DownloadUrl);

                result.WebUrl = CommonLinkUtility.GetFullAbsolutePath(FilesLinkUtility.GetFileWebPreviewUrl(FileUtility, file.Title, file.ID, file.Version));

                result.ThumbnailStatus = file.ThumbnailStatus;

                if (file.ThumbnailStatus == Thumbnail.Created)
                {
                    result.ThumbnailUrl = CommonLinkUtility.GetFullAbsolutePath(FilesLinkUtility.GetFileThumbnailUrl(file.ID, file.Version));
                }
            }
            catch (Exception)
            {
                //Don't catch anything here because of httpcontext
            }

            return result;
        }
    }
}