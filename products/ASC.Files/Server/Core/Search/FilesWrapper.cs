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
using System.Linq;

using ASC.Common;
using ASC.Core;
using ASC.ElasticSearch;
using ASC.ElasticSearch.Core;
using ASC.Files.Core;
using ASC.Files.Core.Data;
using ASC.Files.Resources;
using ASC.Web.Core.Files;

using Microsoft.Extensions.DependencyInjection;

namespace ASC.Web.Files.Core.Search
{
    public sealed class FilesWrapper : WrapperWithDoc
    {
        [Column("title", 1)]
        public string Title { get; set; }

        [ColumnLastModified("modified_on")]
        public override DateTime LastModifiedOn { get; set; }

        [ColumnMeta("version", 2)]
        public int Version { get; set; }

        [ColumnCondition("current_version", 3, true)]
        public bool Current { get; set; }

        [ColumnMeta("encrypted", 4)]
        public bool Encrypted { get; set; }

        [ColumnMeta("content_length", 5)]
        public long ContentLength { get; set; }

        [ColumnMeta("create_by", 6)]
        public Guid CreateBy { get; set; }

        [ColumnMeta("create_on", 7)]
        public DateTime CreateOn { get; set; }

        [ColumnMeta("category", 8)]
        public int Category { get; set; }


        [Join(JoinTypeEnum.Sub, "folder_id:folder_id")]
        public List<FilesFoldersWrapper> Folders { get; set; }

        protected override string Table { get { return "files_file"; } }


        public FilesWrapper()
        {

        }

        public FilesWrapper(IServiceProvider serviceProvider, TenantManager tenantManager, FileUtility fileUtility, IDaoFactory daoFactory)
        {
            ServiceProvider = serviceProvider;
            TenantManager = tenantManager;
            FileUtility = fileUtility;
            DaoFactory = daoFactory;
        }

        public static FilesWrapper GetFilesWrapper<T>(IServiceProvider serviceProvider, File<T> d, List<int> parentFolders = null)
        {
            var wrapper = serviceProvider.GetService<FilesWrapper>();
            var tenantManager = serviceProvider.GetService<TenantManager>();

            wrapper.Id = Convert.ToInt32(d.ID);
            wrapper.Title = d.Title;
            wrapper.Version = d.Version;
            wrapper.Encrypted = d.Encrypted;
            wrapper.ContentLength = d.ContentLength;
            wrapper.LastModifiedOn = d.ModifiedOn;
            wrapper.TenantId = tenantManager.GetCurrentTenant().TenantId;

            if (parentFolders != null)
            {
                wrapper.Folders = parentFolders.Select(r => new FilesFoldersWrapper { FolderId = r.ToString() }).ToList();
            }
            return wrapper;
        }

        protected override Stream GetDocumentStream()
        {
            TenantManager.SetCurrentTenant(TenantId);

            if (Encrypted) return null;
            if (!FileUtility.CanIndex(Title)) return null;

            var fileDao = DaoFactory.GetFileDao<int>();
            var file = ServiceProvider.GetService<File<int>>();
            file.ID = Id;
            file.Title = Title;
            file.Version = Version;
            file.ContentLength = ContentLength;

            if (!fileDao.IsExistOnStorage(file)) return null;
            if (file.ContentLength > MaxContentLength) return null;

            return fileDao.GetFileStream(file);
        }

        public override string SettingsTitle
        {
            get { return FilesCommonResource.IndexTitle; }
        }

        private IServiceProvider ServiceProvider { get; }
        private TenantManager TenantManager { get; }
        private FileUtility FileUtility { get; }
        private IDaoFactory DaoFactory { get; }
    }

    public sealed class FilesFoldersWrapper : Wrapper
    {
        [Column("parent_id", 1)]
        public string FolderId { get; set; }

        [ColumnId("")]
        public override int Id { get; set; }

        [ColumnTenantId("")]
        public override int TenantId { get; set; }

        [ColumnLastModified("")]
        public override DateTime LastModifiedOn { get; set; }

        protected override string Table { get { return "files_folder_tree"; } }
    }

    public static class FilesWrapperExtention
    {
        public static DIHelper AddFilesWrapperService(this DIHelper services)
        {
            services.TryAddTransient<FilesWrapper>();
            return services
                .AddTenantManagerService()
                .AddFileUtilityService()
                .AddDaoFactoryService()
                .AddFactoryIndexerService<FilesWrapper>();
        }
    }
}