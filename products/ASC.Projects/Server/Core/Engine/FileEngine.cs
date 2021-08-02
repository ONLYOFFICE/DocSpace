/*
 *
 * (c) Copyright Ascensio System Limited 2010-2021
 * 
 * Licensed under the Apache License, Version 2.0 (the "License");
 * you may not use this file except in compliance with the License.
 * You may obtain a copy of the License at
 * http://www.apache.org/licenses/LICENSE-2.0
 * Unless required by applicable law or agreed to in writing, software
 * distributed under the License is distributed on an "AS IS" BASIS,
 * WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 * See the License for the specific language governing permissions and
 * limitations under the License.
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
        private IFileSecurityProvider FileSecurityProvider { get; set; }

        public FileEngine(FilesIntegration filesIntegration, IDaoFactory daoFactory, GlobalFolderHelper globalFolderHelper, SecurityContext securityContext, SecurityAdapterProvider securityAdapterProvider, IFileSecurityProvider fileSecurityProvider)
        {
            FilesIntegration = filesIntegration;
            DaoFactory = daoFactory;
            GlobalFolderHelper = globalFolderHelper;
            SecurityContext = securityContext;
            SecurityAdapterProvider = securityAdapterProvider;
            FileSecurityProvider = fileSecurityProvider;
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
            FilesIntegration.RegisterFileSecurityProvider(Module, Bunch, FileSecurityProvider);
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