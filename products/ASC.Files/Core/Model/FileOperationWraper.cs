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
using System.Text.Json.Serialization;
using System.Threading.Tasks;

using ASC.Common;
using ASC.Files.Core;
using ASC.Web.Files.Services.WCFService.FileOperations;
using ASC.Web.Studio.Utility;

namespace ASC.Api.Documents
{
    /// <summary>
    /// </summary>
    public class FileOperationWraper
    {
        /// <summary>
        /// </summary>
        public string Id { get; set; }

        /// <summary>
        /// </summary>
        [JsonPropertyName("Operation")]
        public FileOperationType OperationType { get; set; }

        /// <summary>
        /// </summary>
        public int Progress { get; set; }

        /// <summary>
        /// </summary>
        public string Error { get; set; }

        /// <summary>
        /// </summary>
        public string Processed { get; set; }

        /// <summary>
        /// </summary>
        public bool Finished { get; set; }

        /// <summary>
        /// </summary>
        public string Url { get; set; }

        /// <summary>
        /// </summary>
        public List<FileEntryWrapper> Files { get; set; }

        /// <summary>
        /// </summary>
        public List<FileEntryWrapper> Folders { get; set; }

        /// <summary>
        /// </summary>
        public FileOperationWraper()
        {
        }

        /// <summary>
        /// </summary>
        /// <returns></returns>
        public static FileOperationWraper GetSample()
        {
            return new FileOperationWraper
            {
                Id = Guid.NewGuid().ToString(),
                OperationType = FileOperationType.Move,
                Progress = 100,
                //Source = "folder_1,file_1",
                //Result = "folder_1,file_1",
                Error = "",
                Processed = "1",
                Files = new List<FileEntryWrapper> { FileWrapper<int>.GetSample() },
                Folders = new List<FileEntryWrapper> { FolderWrapper<int>.GetSample() }
            };
        }
    }

    [Scope]
    public class FileOperationWraperHelper
    {
        private FolderWrapperHelper FolderWrapperHelper { get; }
        private FileWrapperHelper FilesWrapperHelper { get; }
        private IDaoFactory DaoFactory { get; }
        private CommonLinkUtility CommonLinkUtility { get; }

        public FileOperationWraperHelper(
            FolderWrapperHelper folderWrapperHelper,
            FileWrapperHelper filesWrapperHelper,
            IDaoFactory daoFactory,
            CommonLinkUtility commonLinkUtility)
        {
            FolderWrapperHelper = folderWrapperHelper;
            FilesWrapperHelper = filesWrapperHelper;
            DaoFactory = daoFactory;
            CommonLinkUtility = commonLinkUtility;
        }

        public async Task<FileOperationWraper> GetAsync(FileOperationResult o)
        {
            var result = new FileOperationWraper
            {
                Id = o.Id,
                OperationType = o.OperationType,
                Progress = o.Progress,
                Error = o.Error,
                Processed = o.Processed,
                Finished = o.Finished
            };

            if (!string.IsNullOrEmpty(o.Result) && result.OperationType != FileOperationType.Delete)
            {
                var arr = o.Result.Split(':');
                var folders = arr
                    .Where(s => s.StartsWith("folder_"))
                    .Select(s => s.Substring(7));

                if (folders.Any())
                {
                    var fInt = new List<int>();
                    var fString = new List<string>();

                    foreach (var folder in folders)
                    {
                        if (int.TryParse(folder, out var f))
                        {
                            fInt.Add(f);
                        }
                        else
                        {
                            fString.Add(folder);
                        }
                    }

                    result.Folders = await GetFoldersAsync(folders);
                    result.Folders.AddRange(await GetFoldersAsync(fInt));
                }
                var files = arr
                    .Where(s => s.StartsWith("file_"))
                    .Select(s => s.Substring(5));

                if (files.Any())
                {
                    var fInt = new List<int>();
                    var fString = new List<string>();

                    foreach (var file in files)
                    {
                        if (int.TryParse(file, out var f))
                        {
                            fInt.Add(f);
                        }
                        else
                        {
                            fString.Add(file);
                        }
                    }

                    result.Files = await GetFilesAsync(fString);
                    result.Files.AddRange(await GetFilesAsync(fInt));
                }

                if (result.OperationType == FileOperationType.Download)
                {
                    result.Url = CommonLinkUtility.GetFullAbsolutePath(o.Result);
                }
            }

            return result;

            async Task<List<FileEntryWrapper>> GetFoldersAsync<T>(IEnumerable<T> folders)
            {
                var folderDao = DaoFactory.GetFolderDao<T>();
                var folderEnum = folderDao.GetFoldersAsync(folders).SelectAwait(async r => await FolderWrapperHelper.GetAsync(r)).Cast<FileEntryWrapper>();
                return await folderEnum.ToListAsync();
            }

            async Task<List<FileEntryWrapper>> GetFilesAsync<T>(IEnumerable<T> files)
            {
                var fileDao = DaoFactory.GetFileDao<T>();
                var filesEnum = fileDao.GetFilesAsync(files).SelectAwait(async r => await FilesWrapperHelper.GetAsync(r)).Cast<FileEntryWrapper>();
                return await filesEnum.ToListAsync();
                    ;
            }
        }
    }
}