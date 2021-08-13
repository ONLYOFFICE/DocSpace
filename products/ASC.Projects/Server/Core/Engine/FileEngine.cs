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
using System.IO;
using System.Linq;

using ASC.Files.Core;
using ASC.Web.Files.Api;
using ASC.Web.Files.Classes;

using FileShare = ASC.Files.Core.Security.FileShare;
using ASC.Core;
using ASC.Files.Core.Security;
using ASC.Common;

namespace ASC.Projects.Engine
{
    [Scope]
    public class FileEngine
    {
        private const string Module = "projects";
        private const string Bunch = "project";
        private SecurityAdapterProvider SecurityAdapterProvider { get; set; }
        private FilesIntegration FilesIntegration { get; set; }
        private IDaoFactory DaoFactory { get; set; }
        private GlobalFolderHelper GlobalFolderHelper { get; set; }
        private SecurityContext SecurityContext { get; set; }

        public FileEngine(FilesIntegration filesIntegration, IDaoFactory daoFactory, GlobalFolderHelper globalFolderHelper, SecurityContext securityContext, SecurityAdapterProvider securityAdapterProvider)
        {
            FilesIntegration = filesIntegration;
            DaoFactory = daoFactory;
            GlobalFolderHelper = globalFolderHelper;
            SecurityContext = securityContext;
            SecurityAdapterProvider = securityAdapterProvider;
        }

        public int GetRoot(int projectId)
        {
            return FilesIntegration.RegisterBunch<int>(Module, Bunch, projectId.ToString(CultureInfo.InvariantCulture));
        }

        public IEnumerable<int> GetRoots(IEnumerable<int> projectIds)
        {
            return FilesIntegration.RegisterBunchFolders<int>(Module, Bunch, projectIds.Select(id => id.ToString(CultureInfo.InvariantCulture)));
        }

        public File<int> GetFile(int id, int version = 1)
        {
            var dao = DaoFactory.GetFileDao<int>();
            var file = 0 < version ? dao.GetFile(id, version) : dao.GetFile(id);
            return file;
        }

        public List<File<int>> GetFiles(IEnumerable<int> id)
        {
            var dao = DaoFactory.GetFileDao<int>();
            return dao.GetFiles(id);
        }

        public File<int> SaveFile(File<int> file, Stream stream)
        {
            var dao = DaoFactory.GetFileDao<int>();
            return dao.SaveFile(file, stream);
        }

        public void RemoveRoot(int projectId)
        {
            var folderId = GetRoot(projectId);

            //requet long operation
            try
            {
                //FileStorageService.DeleteItems("delete", new ItemList<string> { "folder_" + folderId }, true);todo
            }
            catch (Exception)
            {

            }
        }

        public void MoveToTrash(int id)
        {
            var dao = DaoFactory.GetFileDao<int>();
            dao.MoveFile(id, GlobalFolderHelper.FolderTrash);
        }

        public void RegisterFileSecurityProvider()
        {
            FilesIntegration.RegisterFileSecurityProvider(Module, Bunch, SecurityAdapterProvider);
        }

        internal List<Tuple<string, string>> GetFileListInfoHashtable(IEnumerable<File<int>> uploadedFiles)
        {
            if (uploadedFiles == null) return new List<Tuple<string, string>>();

            var fileListInfoHashtable = new List<Tuple<string, string>>();

            foreach (var file in uploadedFiles)
            {
                var fileInfo = string.Format("{0} ({1})", file.Title, Path.GetExtension(file.Title).ToUpper());
                fileListInfoHashtable.Add(new Tuple<string, string>(fileInfo, file.DownloadUrl));
            }

            return fileListInfoHashtable;
        }

        internal FileShare GetFileShare(FileEntry<int> file, int projectId)
        {
            var fileSecurity = SecurityAdapterProvider.GetFileSecurity(projectId);
            var currentUserId = SecurityContext.CurrentAccount.ID;
            if (!fileSecurity.CanRead(file, currentUserId)) return FileShare.Restrict;
            if (!fileSecurity.CanCreate(file, currentUserId) || !fileSecurity.CanEdit(file, currentUserId)) return FileShare.Read;
            if (!fileSecurity.CanDelete(file, currentUserId)) return FileShare.ReadWrite;

            return FileShare.None;
        }
    }
}