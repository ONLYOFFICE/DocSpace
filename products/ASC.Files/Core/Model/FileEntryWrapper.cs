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


using System.Threading.Tasks;

using ASC.Api.Core;
using ASC.Api.Utils;
using ASC.Common;
using ASC.Files.Core;
using ASC.Files.Core.Security;
using ASC.Web.Api.Models;
using ASC.Web.Files.Utils;

using FileShare = ASC.Files.Core.Security.FileShare;

namespace ASC.Api.Documents
{
    /// <summary>
    /// </summary>
    public abstract class FileEntryWrapper
    {
        /// <summary>
        /// </summary>
        public string Title { get; set; }

        /// <summary>
        /// </summary>
        public FileShare Access { get; set; }

        /// <summary>
        /// </summary>
        public bool Shared { get; set; }

        /// <summary>
        /// </summary>
        public ApiDateTime Created { get; set; }

        /// <summary>
        /// </summary>
        public EmployeeWraper CreatedBy { get; set; }

        private ApiDateTime _updated;

        /// <summary>
        /// </summary>
        public ApiDateTime Updated
        {
            get
            {
                return _updated < Created ? Created : _updated;
            }
            set { _updated = value; }
        }

        /// <summary>
        /// </summary>
        public FolderType RootFolderType { get; set; }

        /// <summary>
        /// </summary>
        public EmployeeWraper UpdatedBy { get; set; }

        /// <summary>
        /// </summary>
        public bool? ProviderItem { get; set; }

        /// <summary>
        /// </summary>
        public string ProviderKey { get; set; }

        /// <summary>
        /// </summary>
        public int? ProviderId { get; set; }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="entry"></param>
        protected FileEntryWrapper(FileEntry entry, EmployeeWraperHelper employeeWraperHelper, ApiDateTimeHelper apiDateTimeHelper)
        {
            Title = entry.Title;
            Access = entry.Access;
            Shared = entry.Shared;
            Created = apiDateTimeHelper.Get(entry.CreateOn);
            CreatedBy = employeeWraperHelper.Get(entry.CreateBy);
            Updated = apiDateTimeHelper.Get(entry.ModifiedOn);
            UpdatedBy = employeeWraperHelper.Get(entry.ModifiedBy);
            RootFolderType = entry.RootFolderType;
            ProviderItem = entry.ProviderEntry.NullIfDefault();
            ProviderKey = entry.ProviderKey;
            ProviderId = entry.ProviderId.NullIfDefault();
        }

        /// <summary>
        /// 
        /// </summary>
        protected FileEntryWrapper()
        {

        }
    }

    /// <summary>
    /// </summary>
    public abstract class FileEntryWrapper<T> : FileEntryWrapper
    {
        /// <summary>
        /// </summary>
        public T Id { get; set; }

        public T RootFolderId { get; set; }

        public bool CanShare { get; set; }

        public bool CanEdit { get; set; }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="entry"></param>
        protected FileEntryWrapper(FileEntry<T> entry, EmployeeWraperHelper employeeWraperHelper, ApiDateTimeHelper apiDateTimeHelper)
            : base(entry, employeeWraperHelper, apiDateTimeHelper)
        {
            Id = entry.ID;
            RootFolderId = entry.RootFolderId;
        }

        /// <summary>
        /// 
        /// </summary>
        protected FileEntryWrapper()
        {

        }
    }

    [Scope]
    public class FileEntryWrapperHelper
    {
        private ApiDateTimeHelper ApiDateTimeHelper { get; }
        private EmployeeWraperHelper EmployeeWraperHelper { get; }
        public FileSharingHelper FileSharingHelper { get; }
        public FileSecurity FileSecurity { get; }

        public FileEntryWrapperHelper(
            ApiDateTimeHelper apiDateTimeHelper,
            EmployeeWraperHelper employeeWraperHelper,
            FileSharingHelper fileSharingHelper, FileSecurity fileSecurity
            )
        {
            ApiDateTimeHelper = apiDateTimeHelper;
            EmployeeWraperHelper = employeeWraperHelper;
            FileSharingHelper = fileSharingHelper;
            FileSecurity = fileSecurity;
        }

        protected internal async Task<T> GetAsync<T, TId>(FileEntry<TId> entry) where T : FileEntryWrapper<TId>, new()
        {
            return new T
            {
                Id = entry.ID,
                Title = entry.Title,
                Access = entry.Access,
                Shared = entry.Shared,
                Created = ApiDateTimeHelper.Get(entry.CreateOn),
                CreatedBy = EmployeeWraperHelper.Get(entry.CreateBy),
                Updated = ApiDateTimeHelper.Get(entry.ModifiedOn),
                UpdatedBy = EmployeeWraperHelper.Get(entry.ModifiedBy),
                RootFolderType = entry.RootFolderType,
                RootFolderId = entry.RootFolderId,
                ProviderItem = entry.ProviderEntry.NullIfDefault(),
                ProviderKey = entry.ProviderKey,
                ProviderId = entry.ProviderId.NullIfDefault(),
                CanShare = await FileSharingHelper.CanSetAccessAsync(entry),
                CanEdit = await FileSecurity.CanEditAsync(entry)
            };
        }
    }
}