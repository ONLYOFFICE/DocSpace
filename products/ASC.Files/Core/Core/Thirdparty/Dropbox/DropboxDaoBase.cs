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
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

using ASC.Common.Logging;
using ASC.Core;
using ASC.Core.Common.EF;
using ASC.Core.Tenants;
using ASC.Files.Core;
using ASC.Files.Core.EF;
using ASC.Web.Core.Files;
using ASC.Web.Files.Classes;
using ASC.Web.Studio.Core;

using Dropbox.Api.Files;

using Microsoft.Extensions.Options;

namespace ASC.Files.Thirdparty.Dropbox
{
    internal abstract class DropboxDaoBase : ThirdPartyProviderDao<DropboxProviderInfo>
    {
        protected override string Id { get => "dropbox"; }

        protected DropboxDaoBase(IServiceProvider serviceProvider, UserManager userManager, TenantManager tenantManager, TenantUtil tenantUtil, DbContextManager<FilesDbContext> dbContextManager, SetupInfo setupInfo, IOptionsMonitor<ILog> monitor, FileUtility fileUtility, TempPath tempPath) 
            : base(serviceProvider, userManager, tenantManager, tenantUtil, dbContextManager, setupInfo, monitor, fileUtility, tempPath)
        {
        }

        protected static string GetParentFolderPath(Metadata dropboxItem)
        {
            if (dropboxItem == null || IsRoot(dropboxItem.AsFolder))
                return null;

            var pathLength = dropboxItem.PathDisplay.Length - dropboxItem.Name.Length;
            return dropboxItem.PathDisplay.Substring(0, pathLength > 1 ? pathLength - 1 : 0);
        }

        protected static string MakeDropboxPath(object entryId)
        {
            return Convert.ToString(entryId, CultureInfo.InvariantCulture);
        }

        protected string MakeDropboxPath(Metadata dropboxItem)
        {
            string path = null;
            if (dropboxItem != null)
            {
                path = dropboxItem.PathDisplay;
            }

            return path;
        }

        protected string MakeId(Metadata dropboxItem)
        {
            return MakeId(MakeDropboxPath(dropboxItem));
        }

        protected override string MakeId(string path = null)
        {
            var p = string.IsNullOrEmpty(path) || path == "/" ? "" : ("-" + path.Replace('/', '|'));
            return $"{PathPrefix}{p}";
        }

        protected string MakeFolderTitle(FolderMetadata dropboxFolder)
        {
            if (dropboxFolder == null || IsRoot(dropboxFolder))
            {
                return ProviderInfo.CustomerTitle;
            }

            return Global.ReplaceInvalidCharsAndTruncate(dropboxFolder.Name);
        }

        protected string MakeFileTitle(FileMetadata dropboxFile)
        {
            if (dropboxFile == null || string.IsNullOrEmpty(dropboxFile.Name))
            {
                return ProviderInfo.ProviderKey;
            }

            return Global.ReplaceInvalidCharsAndTruncate(dropboxFile.Name);
        }

        protected Folder<string> ToFolder(FolderMetadata dropboxFolder)
        {
            if (dropboxFolder == null) return null;
            if (dropboxFolder is ErrorFolder)
            {
                //Return error entry
                return ToErrorFolder(dropboxFolder as ErrorFolder);
            }

            var isRoot = IsRoot(dropboxFolder);

            var folder = GetFolder();

            folder.ID = MakeId(dropboxFolder);
            folder.FolderID = isRoot ? null : MakeId(GetParentFolderPath(dropboxFolder));
            folder.CreateOn = isRoot ? ProviderInfo.CreateOn : default;
            folder.ModifiedOn = isRoot ? ProviderInfo.CreateOn : default;
            folder.Title = MakeFolderTitle(dropboxFolder);

            if (folder.CreateOn != DateTime.MinValue && folder.CreateOn.Kind == DateTimeKind.Utc)
                folder.CreateOn = TenantUtil.DateTimeFromUtc(folder.CreateOn);

            if (folder.ModifiedOn != DateTime.MinValue && folder.ModifiedOn.Kind == DateTimeKind.Utc)
                folder.ModifiedOn = TenantUtil.DateTimeFromUtc(folder.ModifiedOn);

            return folder;
        }

        protected static bool IsRoot(FolderMetadata dropboxFolder)
        {
            return dropboxFolder != null && dropboxFolder.Id == "/";
        }

        private File<string> ToErrorFile(ErrorFile dropboxFile)
        {
            if (dropboxFile == null) return null;

            var file = GetErrorFile(new ErrorEntry(dropboxFile.ErrorId, dropboxFile.Error));

            file.Title = MakeFileTitle(dropboxFile);

            return file;
        }

        private Folder<string> ToErrorFolder(ErrorFolder dropboxFolder)
        {
            if (dropboxFolder == null) return null;

            var folder = GetErrorFolder(new ErrorEntry(dropboxFolder.Error, dropboxFolder.ErrorId));

            folder.Title = MakeFolderTitle(dropboxFolder);

            return folder;
        }

        public File<string> ToFile(FileMetadata dropboxFile)
        {
            if (dropboxFile == null) return null;

            if (dropboxFile is ErrorFile)
            {
                //Return error entry
                return ToErrorFile(dropboxFile as ErrorFile);
            }

            var file = GetFile();

            file.ID = MakeId(dropboxFile);
            file.ContentLength = (long)dropboxFile.Size;
            file.CreateOn = TenantUtil.DateTimeFromUtc(dropboxFile.ServerModified);
            file.FolderID = MakeId(GetParentFolderPath(dropboxFile));
            file.ModifiedOn = TenantUtil.DateTimeFromUtc(dropboxFile.ServerModified);
            file.NativeAccessor = dropboxFile;
            file.Title = MakeFileTitle(dropboxFile);

            return file;
        }

        public async Task<Folder<string>> GetRootFolderAsync(string folderId)
        {
            return ToFolder(await GetDropboxFolderAsync(string.Empty));
        }

        protected async Task<FolderMetadata> GetDropboxFolderAsync(string folderId)
        {
            var dropboxFolderPath = MakeDropboxPath(folderId);
            try
            {
                var folder = await ProviderInfo.GetDropboxFolderAsync(dropboxFolderPath);
                return folder;
            }
            catch (Exception ex)
            {
                return new ErrorFolder(ex, dropboxFolderPath);
            }
        }

        protected ValueTask<FileMetadata> GetDropboxFileAsync(object fileId)
        {
            var dropboxFilePath = MakeDropboxPath(fileId);
            try
            {
                var file = ProviderInfo.GetDropboxFileAsync(dropboxFilePath);
                return file;
            }
            catch (Exception ex)
            {
                return ValueTask.FromResult<FileMetadata>(new ErrorFile(ex, dropboxFilePath));
            }
        }

        protected override async Task<IEnumerable<string>> GetChildrenAsync(string folderId)
        {
            var items = await GetDropboxItemsAsync(folderId);
            return items.Select(MakeId);
        }

        protected async Task<List<Metadata>> GetDropboxItemsAsync(object parentId, bool? folder = null)
        {
            var dropboxFolderPath = MakeDropboxPath(parentId);
            var items = await ProviderInfo.GetDropboxItemsAsync(dropboxFolderPath);

            if (folder.HasValue)
            {
                if (folder.Value)
                {
                    return items.Where(i => i.AsFolder != null).ToList();
                }

                return items.Where(i => i.AsFile != null).ToList();
            }

            return items;
        }

        protected sealed class ErrorFolder : FolderMetadata
        {
            public string Error { get; set; }

            public string ErrorId { get; private set; }


            public ErrorFolder(Exception e, object id)
            {
                ErrorId = id.ToString();
                if (e != null)
                {
                    Error = e.Message;
                }
            }
        }

        protected sealed class ErrorFile : FileMetadata
        {
            public string Error { get; set; }

            public string ErrorId { get; private set; }


            public ErrorFile(Exception e, object id)
            {
                ErrorId = id.ToString();
                if (e != null)
                {
                    Error = e.Message;
                }
            }
        }

        protected string GetAvailableTitle(string requestTitle, string parentFolderPath, Func<string, string, bool> isExist)
        {
            if (!isExist(requestTitle, parentFolderPath)) return requestTitle;

            var re = new Regex(@"( \(((?<index>[0-9])+)\)(\.[^\.]*)?)$");
            var match = re.Match(requestTitle);

            if (!match.Success)
            {
                var insertIndex = requestTitle.Length;
                if (requestTitle.LastIndexOf('.') != -1)
                {
                    insertIndex = requestTitle.LastIndexOf('.');
                }
                requestTitle = requestTitle.Insert(insertIndex, " (1)");
            }

            while (isExist(requestTitle, parentFolderPath))
            {
                requestTitle = re.Replace(requestTitle, MatchEvaluator);
            }
            return requestTitle;
        }

        protected async Task<string> GetAvailableTitleAsync(string requestTitle, string parentFolderPath, Func<string, string, Task<bool>> isExist)
        {
            if (!await isExist(requestTitle, parentFolderPath)) return requestTitle;

            var re = new Regex(@"( \(((?<index>[0-9])+)\)(\.[^\.]*)?)$");
            var match = re.Match(requestTitle);

            if (!match.Success)
            {
                var insertIndex = requestTitle.Length;
                if (requestTitle.LastIndexOf(".", StringComparison.InvariantCulture) != -1)
                {
                    insertIndex = requestTitle.LastIndexOf(".", StringComparison.InvariantCulture);
                }
                requestTitle = requestTitle.Insert(insertIndex, " (1)");
            }

            while (await isExist(requestTitle, parentFolderPath))
            {
                requestTitle = re.Replace(requestTitle, MatchEvaluator);
            }
            return requestTitle;
        }

        private string MatchEvaluator(Match match)
        {
            var index = Convert.ToInt32(match.Groups[2].Value);
            var staticText = match.Value.Substring(string.Format(" ({0})", index).Length);
            return string.Format(" ({0}){1}", index + 1, staticText);
        }
    }
}