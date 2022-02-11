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

using Box.V2.Models;

using Microsoft.Extensions.Options;

namespace ASC.Files.Thirdparty.Box
{
    internal abstract class BoxDaoBase : ThirdPartyProviderDao<BoxProviderInfo>
    {
        protected override string Id { get => "box"; }

        protected BoxDaoBase(
            IServiceProvider serviceProvider,
            UserManager userManager,
            TenantManager tenantManager,
            TenantUtil tenantUtil,
            DbContextManager<FilesDbContext> dbContextManager,
            SetupInfo setupInfo,
            IOptionsMonitor<ILog> monitor,
            FileUtility fileUtility,
            TempPath tempPath) : base(serviceProvider, userManager, tenantManager, tenantUtil, dbContextManager, setupInfo, monitor, fileUtility, tempPath)
        {
        }


        protected static string MakeBoxId(object entryId)
        {
            var id = Convert.ToString(entryId, CultureInfo.InvariantCulture);
            return string.IsNullOrEmpty(id)
                       ? "0"
                       : id.TrimStart('/');
        }

        protected static string GetParentFolderId(BoxItem boxItem)
        {
            return boxItem == null || boxItem.Parent == null
                       ? null
                       : boxItem.Parent.Id;
        }

        protected string MakeId(BoxItem boxItem)
        {
            var path = string.Empty;
            if (boxItem != null)
            {
                path = boxItem.Id;
            }

            return MakeId(path);
        }

        protected override string MakeId(string path = null)
        {
            var p = string.IsNullOrEmpty(path) || path == "0" ? "" : ("-|" + path.TrimStart('/'));
            return $"{PathPrefix}{p}";
        }

        protected string MakeFolderTitle(BoxFolder boxFolder)
        {
            if (boxFolder == null || IsRoot(boxFolder))
            {
                return ProviderInfo.CustomerTitle;
            }

            return Global.ReplaceInvalidCharsAndTruncate(boxFolder.Name);
        }

        protected string MakeFileTitle(BoxFile boxFile)
        {
            if (boxFile == null || string.IsNullOrEmpty(boxFile.Name))
            {
                return ProviderInfo.ProviderKey;
            }

            return Global.ReplaceInvalidCharsAndTruncate(boxFile.Name);
        }

        protected Folder<string> ToFolder(BoxFolder boxFolder)
        {
            if (boxFolder == null) return null;
            if (boxFolder is ErrorFolder)
            {
                //Return error entry
                return ToErrorFolder(boxFolder as ErrorFolder);
            }

            var isRoot = IsRoot(boxFolder);

            var folder = GetFolder();

            folder.ID = MakeId(boxFolder.Id);
            folder.FolderID = isRoot ? null : MakeId(GetParentFolderId(boxFolder));
            folder.CreateOn = isRoot ? ProviderInfo.CreateOn : (boxFolder.CreatedAt ?? default);
            folder.ModifiedOn = isRoot ? ProviderInfo.CreateOn : (boxFolder.ModifiedAt ?? default);

            folder.Title = MakeFolderTitle(boxFolder);
            folder.TotalFiles = boxFolder.ItemCollection != null ? boxFolder.ItemCollection.Entries.Count(item => item is BoxFile) : 0;
            folder.TotalSubFolders = boxFolder.ItemCollection != null ? boxFolder.ItemCollection.Entries.Count(item => item is BoxFolder) : 0;

            if (folder.CreateOn != DateTime.MinValue && folder.CreateOn.Kind == DateTimeKind.Utc)
                folder.CreateOn = TenantUtil.DateTimeFromUtc(folder.CreateOn);

            if (folder.ModifiedOn != DateTime.MinValue && folder.ModifiedOn.Kind == DateTimeKind.Utc)
                folder.ModifiedOn = TenantUtil.DateTimeFromUtc(folder.ModifiedOn);

            return folder;
        }

        protected static bool IsRoot(BoxFolder boxFolder)
        {
            return boxFolder.Id == "0";
        }

        private File<string> ToErrorFile(ErrorFile boxFile)
        {
            if (boxFile == null) return null;

            var file = GetErrorFile(new ErrorEntry(boxFile.Error, boxFile.ErrorId));

            file.Title = MakeFileTitle(boxFile);

            return file;
        }

        private Folder<string> ToErrorFolder(ErrorFolder boxFolder)
        {
            if (boxFolder == null) return null;

            var folder = GetErrorFolder(new ErrorEntry(boxFolder.Error, boxFolder.ErrorId));

            folder.Title = MakeFolderTitle(boxFolder);

            return folder;
        }

        public File<string> ToFile(BoxFile boxFile)
        {
            if (boxFile == null) return null;

            if (boxFile is ErrorFile)
            {
                //Return error entry
                return ToErrorFile(boxFile as ErrorFile);
            }

            var file = GetFile();

            file.ID = MakeId(boxFile.Id);
            file.ContentLength = boxFile.Size.HasValue ? (long)boxFile.Size : 0;
            file.CreateOn = boxFile.CreatedAt.HasValue ? TenantUtil.DateTimeFromUtc(boxFile.CreatedAt.Value) : default;
            file.FolderID = MakeId(GetParentFolderId(boxFile));
            file.ModifiedOn = boxFile.ModifiedAt.HasValue ? TenantUtil.DateTimeFromUtc(boxFile.ModifiedAt.Value) : default;
            file.NativeAccessor = boxFile;
            file.Title = MakeFileTitle(boxFile);

            return file;
        }

        public async Task<Folder<string>> GetRootFolderAsync(string folderId)
        {
            return ToFolder(await GetBoxFolderAsync("0"));
        }

        protected async Task<BoxFolder> GetBoxFolderAsync(string folderId)
        {
            var boxFolderId = MakeBoxId(folderId);
            try
            {
                var folder = await ProviderInfo.GetBoxFolderAsync(boxFolderId);
                return folder;
            }
            catch (Exception ex)
            {
                return new ErrorFolder(ex, boxFolderId);
            }
        }

        protected ValueTask<BoxFile> GetBoxFileAsync(string fileId)
        {
            var boxFileId = MakeBoxId(fileId);
            try
            {
                var file = ProviderInfo.GetBoxFileAsync(boxFileId);
                return file;
            }
            catch (Exception ex)
            {
                return ValueTask.FromResult<BoxFile>(new ErrorFile(ex, boxFileId));
            }
        }

        protected override async Task<IEnumerable<string>> GetChildrenAsync(string folderId)
        {
            var items = await GetBoxItemsAsync(folderId);
            return items.Select(entry => MakeId(entry.Id));
        }

        protected List<BoxItem> GetBoxItems(string parentId, bool? folder = null)
        {
            var boxFolderId = MakeBoxId(parentId);
            var items = ProviderInfo.GetBoxItemsAsync(boxFolderId).Result;

            if (folder.HasValue)
            {
                if (folder.Value)
                {
                    return items.Where(i => i is BoxFolder).ToList();
                }

                return items.Where(i => i is BoxFile).ToList();
            }

            return items;
        }

        protected async Task<List<BoxItem>> GetBoxItemsAsync(string parentId, bool? folder = null)
        {
            var boxFolderId = MakeBoxId(parentId);
            var items = await ProviderInfo.GetBoxItemsAsync(boxFolderId);

            if (folder.HasValue)
            {
                if (folder.Value)
                {
                    return items.Where(i => i is BoxFolder).ToList();
                }

                return items.Where(i => i is BoxFile).ToList();
            }

            return items;
        }

        protected sealed class ErrorFolder : BoxFolder
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

        protected sealed class ErrorFile : BoxFile
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

        protected string GetAvailableTitle(string requestTitle, string parentFolderId, Func<string, string, bool> isExist)
        {
            if (!isExist(requestTitle, parentFolderId)) return requestTitle;

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

            while (isExist(requestTitle, parentFolderId))
            {
                requestTitle = re.Replace(requestTitle, MatchEvaluator);
            }
            return requestTitle;
        }

        protected async Task<string> GetAvailableTitleAsync(string requestTitle, string parentFolderId, Func<string, string, Task<bool>> isExist)
        {
            if (!await isExist(requestTitle, parentFolderId)) return requestTitle;

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

            while (!await isExist(requestTitle, parentFolderId))
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