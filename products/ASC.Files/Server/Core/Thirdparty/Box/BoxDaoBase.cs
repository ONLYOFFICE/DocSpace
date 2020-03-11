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
using System.Text.RegularExpressions;

using ASC.Core;
using ASC.Core.Common.EF;
using ASC.Core.Tenants;
using ASC.Files.Core;
using ASC.Files.Core.EF;
using ASC.Files.Core.Security;
using ASC.Security.Cryptography;
using ASC.Web.Files.Classes;
using ASC.Web.Studio.Core;

using Box.V2.Models;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace ASC.Files.Thirdparty.Box
{
    internal abstract class BoxDaoBase
    {
        public BoxDaoSelector BoxDaoSelector { get; set; }

        public int TenantID { get; private set; }
        public BoxProviderInfo BoxProviderInfo { get; set; }
        public string PathPrefix { get; private set; }
        public FilesDbContext FilesDbContext { get; }
        public IServiceProvider ServiceProvider { get; }
        public UserManager UserManager { get; }
        public TenantUtil TenantUtil { get; }
        public SetupInfo SetupInfo { get; }

        public BoxDaoBase(
            IServiceProvider serviceProvider,
            UserManager userManager,
            TenantManager tenantManager,
            TenantUtil tenantUtil,
            DbContextManager<FilesDbContext> dbContextManager,
            SetupInfo setupInfo)
        {
            TenantID = tenantManager.GetCurrentTenant().TenantId;
            FilesDbContext = dbContextManager.Get(FileConstant.DatabaseId);
            ServiceProvider = serviceProvider;
            UserManager = userManager;
            TenantUtil = tenantUtil;
            SetupInfo = setupInfo;
        }

        public void Init(BoxDaoSelector.BoxInfo boxInfo, BoxDaoSelector boxDaoSelector)
        {
            BoxProviderInfo = boxInfo.BoxProviderInfo;
            PathPrefix = boxInfo.PathPrefix;
            BoxDaoSelector = boxDaoSelector;
        }

        public void Dispose()
        {
            BoxProviderInfo.Dispose();
        }

        protected string MappingID(string id, bool saveIfNotExist = false)
        {
            if (id == null) return null;

            string result;
            if (id.StartsWith("box"))
            {
                result = Regex.Replace(BitConverter.ToString(Hasher.Hash(id.ToString(), HashAlg.MD5)), "-", "").ToLower();
            }
            else
            {
                result = FilesDbContext.ThirdpartyIdMapping
                        .Where(r => r.HashId == id)
                        .Select(r => r.Id)
                        .FirstOrDefault();
            }
            if (saveIfNotExist)
            {
                var newMapping = new DbFilesThirdpartyIdMapping
                {
                    Id = id,
                    HashId = result,
                    TenantId = TenantID
                };

                FilesDbContext.ThirdpartyIdMapping.Add(newMapping);
                FilesDbContext.SaveChanges();
            }
            return result;
        }

        protected IQueryable<T> Query<T>(DbSet<T> set) where T : class, IDbFile
        {
            return set.Where(r => r.TenantId == TenantID);
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

        protected string MakeId(string path = null)
        {
            return string.Format("{0}{1}", PathPrefix,
                                 string.IsNullOrEmpty(path) || path == "0"
                                     ? "" : ("-|" + path.TrimStart('/')));
        }

        protected string MakeFolderTitle(BoxFolder boxFolder)
        {
            if (boxFolder == null || IsRoot(boxFolder))
            {
                return BoxProviderInfo.CustomerTitle;
            }

            return Global.ReplaceInvalidCharsAndTruncate(boxFolder.Name);
        }

        protected string MakeFileTitle(BoxFile boxFile)
        {
            if (boxFile == null || string.IsNullOrEmpty(boxFile.Name))
            {
                return BoxProviderInfo.ProviderKey;
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

            var folder = ServiceProvider.GetService<Folder<string>>();

            folder.ID = MakeId(boxFolder.Id);
            folder.ParentFolderID = isRoot ? null : MakeId(GetParentFolderId(boxFolder));
            folder.CreateBy = BoxProviderInfo.Owner;
            folder.CreateOn = isRoot ? BoxProviderInfo.CreateOn : (boxFolder.CreatedAt ?? default);
            folder.FolderType = FolderType.DEFAULT;
            folder.ModifiedBy = BoxProviderInfo.Owner;
            folder.ModifiedOn = isRoot ? BoxProviderInfo.CreateOn : (boxFolder.ModifiedAt ?? default);
            folder.ProviderId = BoxProviderInfo.ID;
            folder.ProviderKey = BoxProviderInfo.ProviderKey;
            folder.RootFolderCreator = BoxProviderInfo.Owner;
            folder.RootFolderId = MakeId();
            folder.RootFolderType = BoxProviderInfo.RootFolderType;

            folder.Shareable = false;
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
            var file = ServiceProvider.GetService<File<string>>();

            file.ID = MakeId(boxFile.ErrorId);
            file.CreateBy = BoxProviderInfo.Owner;
            file.CreateOn = TenantUtil.DateTimeNow();
            file.ModifiedBy = BoxProviderInfo.Owner;
            file.ModifiedOn = TenantUtil.DateTimeNow();
            file.ProviderId = BoxProviderInfo.ID;
            file.ProviderKey = BoxProviderInfo.ProviderKey;
            file.RootFolderCreator = BoxProviderInfo.Owner;
            file.RootFolderId = MakeId();
            file.RootFolderType = BoxProviderInfo.RootFolderType;
            file.Title = MakeFileTitle(boxFile);
            file.Error = boxFile.Error;

            return file;
        }

        private Folder<string> ToErrorFolder(ErrorFolder boxFolder)
        {
            if (boxFolder == null) return null;

            var folder = ServiceProvider.GetService<Folder<string>>();

            folder.ID = MakeId(boxFolder.ErrorId);
            folder.ParentFolderID = null;
            folder.CreateBy = BoxProviderInfo.Owner;
            folder.CreateOn = TenantUtil.DateTimeNow();
            folder.FolderType = FolderType.DEFAULT;
            folder.ModifiedBy = BoxProviderInfo.Owner;
            folder.ModifiedOn = TenantUtil.DateTimeNow();
            folder.ProviderId = BoxProviderInfo.ID;
            folder.ProviderKey = BoxProviderInfo.ProviderKey;
            folder.RootFolderCreator = BoxProviderInfo.Owner;
            folder.RootFolderId = MakeId();
            folder.RootFolderType = BoxProviderInfo.RootFolderType;
            folder.Shareable = false;
            folder.Title = MakeFolderTitle(boxFolder);
            folder.TotalFiles = 0;
            folder.TotalSubFolders = 0;
            folder.Error = boxFolder.Error;

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

            var file = ServiceProvider.GetService<File<string>>();

            file.ID = MakeId(boxFile.Id);
            file.Access = FileShare.None;
            file.ContentLength = boxFile.Size.HasValue ? (long)boxFile.Size : 0;
            file.CreateBy = BoxProviderInfo.Owner;
            file.CreateOn = boxFile.CreatedAt.HasValue ? TenantUtil.DateTimeFromUtc(boxFile.CreatedAt.Value) : default;
            file.FileStatus = FileStatus.None;
            file.FolderID = MakeId(GetParentFolderId(boxFile));
            file.ModifiedBy = BoxProviderInfo.Owner;
            file.ModifiedOn = boxFile.ModifiedAt.HasValue ? TenantUtil.DateTimeFromUtc(boxFile.ModifiedAt.Value) : default;
            file.NativeAccessor = boxFile;
            file.ProviderId = BoxProviderInfo.ID;
            file.ProviderKey = BoxProviderInfo.ProviderKey;
            file.Title = MakeFileTitle(boxFile);
            file.RootFolderId = MakeId();
            file.RootFolderType = BoxProviderInfo.RootFolderType;
            file.RootFolderCreator = BoxProviderInfo.Owner;
            file.Shared = false;
            file.Version = 1;

            return file;
        }

        public Folder<string> GetRootFolder(string folderId)
        {
            return ToFolder(GetBoxFolder("0"));
        }

        protected BoxFolder GetBoxFolder(object folderId)
        {
            var boxFolderId = MakeBoxId(folderId);
            try
            {
                var folder = BoxProviderInfo.GetBoxFolder(boxFolderId);
                return folder;
            }
            catch (Exception ex)
            {
                return new ErrorFolder(ex, boxFolderId);
            }
        }

        protected BoxFile GetBoxFile(string fileId)
        {
            var boxFileId = MakeBoxId(fileId);
            try
            {
                var file = BoxProviderInfo.GetBoxFile(boxFileId);
                return file;
            }
            catch (Exception ex)
            {
                return new ErrorFile(ex, boxFileId);
            }
        }

        protected IEnumerable<string> GetChildren(object folderId)
        {
            return GetBoxItems(folderId).Select(entry => MakeId(entry.Id));
        }

        protected List<BoxItem> GetBoxItems(object parentId, bool? folder = null)
        {
            var boxFolderId = MakeBoxId(parentId);
            var items = BoxProviderInfo.GetBoxItems(boxFolderId);

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
                if (requestTitle.LastIndexOf(".", StringComparison.InvariantCulture) != -1)
                {
                    insertIndex = requestTitle.LastIndexOf(".", StringComparison.InvariantCulture);
                }
                requestTitle = requestTitle.Insert(insertIndex, " (1)");
            }

            while (isExist(requestTitle, parentFolderId))
            {
                requestTitle = re.Replace(requestTitle, MatchEvaluator);
            }
            return requestTitle;
        }

        private static string MatchEvaluator(Match match)
        {
            var index = Convert.ToInt32(match.Groups[2].Value);
            var staticText = match.Value.Substring(string.Format(" ({0})", index).Length);
            return string.Format(" ({0}){1}", index + 1, staticText);
        }
    }
}