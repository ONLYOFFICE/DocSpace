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
using System.Text.RegularExpressions;

using ASC.Core;
using ASC.Core.Common.EF;
using ASC.Core.Tenants;
using ASC.Files.Core;
using ASC.Files.Core.EF;
using ASC.Files.Core.Security;
using ASC.Files.Core.Thirdparty;
using ASC.Security.Cryptography;
using ASC.Web.Files.Classes;
using ASC.Web.Studio.Core;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.OneDrive.Sdk;

namespace ASC.Files.Thirdparty.OneDrive
{
    internal abstract class OneDriveDaoBase : IThirdPartyProviderDao<OneDriveProviderInfo>
    {
        public RegexDaoSelectorBase<OneDriveProviderInfo> OneDriveDaoSelector { get; set; }

        public int TenantID { get; private set; }
        public OneDriveProviderInfo OneDriveProviderInfo { get; set; }
        public string PathPrefix { get; set; }
        public TenantUtil TenantUtil { get; }
        public FilesDbContext FilesDbContext { get; }
        public SetupInfo SetupInfo { get; }
        public IServiceProvider ServiceProvider { get; }
        public UserManager UserManager { get; }

        public OneDriveDaoBase(
                IServiceProvider serviceProvider,
                UserManager userManager,
                TenantManager tenantManager,
                TenantUtil tenantUtil,
                DbContextManager<FilesDbContext> dbContextManager,
                SetupInfo setupInfo)
        {
            ServiceProvider = serviceProvider;
            UserManager = userManager;
            TenantUtil = tenantUtil;
            TenantID = tenantManager.GetCurrentTenant().TenantId;
            FilesDbContext = dbContextManager.Get(FileConstant.DatabaseId);
            SetupInfo = setupInfo;
        }

        public void Init(BaseProviderInfo<OneDriveProviderInfo> onedriveInfo, RegexDaoSelectorBase<OneDriveProviderInfo> onedriveDaoSelector)
        {
            OneDriveProviderInfo = onedriveInfo.ProviderInfo;
            PathPrefix = onedriveInfo.PathPrefix;
            OneDriveDaoSelector = onedriveDaoSelector;
        }

        public void Dispose()
        {
            OneDriveProviderInfo.Dispose();
        }

        protected string MappingID(string id, bool saveIfNotExist = false)
        {
            if (id == null) return null;

            string result;
            if (id.ToString().StartsWith("onedrive"))
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


        protected static string MakeOneDriveId(string entryId)
        {
            var id = entryId;
            return string.IsNullOrEmpty(id)
                       ? string.Empty
                       : id.TrimStart('/');
        }

        protected static string GetParentFolderId(Item onedriveItem)
        {
            return onedriveItem == null || IsRoot(onedriveItem)
                       ? null
                       : (onedriveItem.ParentReference.Path.Equals(OneDriveStorage.RootPath, StringComparison.InvariantCultureIgnoreCase)
                              ? string.Empty
                              : onedriveItem.ParentReference.Id);
        }

        protected string MakeId(Item onedriveItem)
        {
            var id = string.Empty;
            if (onedriveItem != null)
            {
                id = onedriveItem.Id;
            }

            return MakeId(id);
        }

        protected string MakeId(string id = null)
        {
            return string.Format("{0}{1}", PathPrefix,
                                 string.IsNullOrEmpty(id) || id == ""
                                     ? "" : ("-|" + id.TrimStart('/')));
        }

        public string MakeOneDrivePath(Item onedriveItem)
        {
            return onedriveItem == null || IsRoot(onedriveItem)
                       ? string.Empty
                       : (OneDriveStorage.MakeOneDrivePath(
                           new Regex("^" + OneDriveStorage.RootPath).Replace(onedriveItem.ParentReference.Path, ""),
                           onedriveItem.Name));
        }

        protected string MakeItemTitle(Item onedriveItem)
        {
            if (onedriveItem == null || IsRoot(onedriveItem))
            {
                return OneDriveProviderInfo.CustomerTitle;
            }

            return Global.ReplaceInvalidCharsAndTruncate(onedriveItem.Name);
        }

        protected Folder<string> ToFolder(Item onedriveFolder)
        {
            if (onedriveFolder == null) return null;
            if (onedriveFolder is ErrorItem)
            {
                //Return error entry
                return ToErrorFolder(onedriveFolder as ErrorItem);
            }

            if (onedriveFolder.Folder == null) return null;

            var isRoot = IsRoot(onedriveFolder);

            var folder = ServiceProvider.GetService<Folder<string>>();

            folder.ID = MakeId(isRoot ? string.Empty : onedriveFolder.Id);
            folder.ParentFolderID = isRoot ? null : MakeId(GetParentFolderId(onedriveFolder));
            folder.CreateBy = OneDriveProviderInfo.Owner;
            folder.CreateOn = isRoot ? OneDriveProviderInfo.CreateOn : (onedriveFolder.CreatedDateTime.HasValue ? TenantUtil.DateTimeFromUtc(onedriveFolder.CreatedDateTime.Value.DateTime) : default(DateTime));
            folder.FolderType = FolderType.DEFAULT;
            folder.ModifiedBy = OneDriveProviderInfo.Owner;
            folder.ModifiedOn = isRoot ? OneDriveProviderInfo.CreateOn : (onedriveFolder.LastModifiedDateTime.HasValue ? TenantUtil.DateTimeFromUtc(onedriveFolder.LastModifiedDateTime.Value.DateTime) : default(DateTime));
            folder.ProviderId = OneDriveProviderInfo.ID;
            folder.ProviderKey = OneDriveProviderInfo.ProviderKey;
            folder.RootFolderCreator = OneDriveProviderInfo.Owner;
            folder.RootFolderId = MakeId();
            folder.RootFolderType = OneDriveProviderInfo.RootFolderType;

            folder.Shareable = false;
            folder.Title = MakeItemTitle(onedriveFolder);
            folder.TotalFiles = 0;
            folder.TotalSubFolders = 0;

            return folder;
        }

        protected static bool IsRoot(Item onedriveFolder)
        {
            return onedriveFolder.ParentReference == null || onedriveFolder.ParentReference.Id == null;
        }

        private File<string> ToErrorFile(ErrorItem onedriveFile)
        {
            if (onedriveFile == null) return null;
            var file = ServiceProvider.GetService<File<string>>();

            file.ID = MakeId(onedriveFile.ErrorId);
            file.CreateBy = OneDriveProviderInfo.Owner;
            file.CreateOn = TenantUtil.DateTimeNow();
            file.ModifiedBy = OneDriveProviderInfo.Owner;
            file.ModifiedOn = TenantUtil.DateTimeNow();
            file.ProviderId = OneDriveProviderInfo.ID;
            file.ProviderKey = OneDriveProviderInfo.ProviderKey;
            file.RootFolderCreator = OneDriveProviderInfo.Owner;
            file.RootFolderId = MakeId();
            file.RootFolderType = OneDriveProviderInfo.RootFolderType;
            file.Title = MakeItemTitle(onedriveFile);
            file.Error = onedriveFile.Error;

            return file;
        }

        private Folder<string> ToErrorFolder(ErrorItem onedriveFolder)
        {
            if (onedriveFolder == null) return null;

            var folder = ServiceProvider.GetService<Folder<string>>();

            folder.ID = MakeId(onedriveFolder.ErrorId);
            folder.ParentFolderID = null;
            folder.CreateBy = OneDriveProviderInfo.Owner;
            folder.CreateOn = TenantUtil.DateTimeNow();
            folder.FolderType = FolderType.DEFAULT;
            folder.ModifiedBy = OneDriveProviderInfo.Owner;
            folder.ModifiedOn = TenantUtil.DateTimeNow();
            folder.ProviderId = OneDriveProviderInfo.ID;
            folder.ProviderKey = OneDriveProviderInfo.ProviderKey;
            folder.RootFolderCreator = OneDriveProviderInfo.Owner;
            folder.RootFolderId = MakeId();
            folder.RootFolderType = OneDriveProviderInfo.RootFolderType;
            folder.Shareable = false;
            folder.Title = MakeItemTitle(onedriveFolder);
            folder.TotalFiles = 0;
            folder.TotalSubFolders = 0;
            folder.Error = onedriveFolder.Error;
            return folder;
        }

        public File<string> ToFile(Item onedriveFile)
        {
            if (onedriveFile == null) return null;

            if (onedriveFile is ErrorItem)
            {
                //Return error entry
                return ToErrorFile(onedriveFile as ErrorItem);
            }

            if (onedriveFile.File == null) return null;

            var file = ServiceProvider.GetService<File<string>>();

            file.ID = MakeId(onedriveFile.Id);
            file.Access = FileShare.None;
            file.ContentLength = onedriveFile.Size.HasValue ? (long)onedriveFile.Size : 0;
            file.CreateBy = OneDriveProviderInfo.Owner;
            file.CreateOn = onedriveFile.CreatedDateTime.HasValue ? TenantUtil.DateTimeFromUtc(onedriveFile.CreatedDateTime.Value.DateTime) : default;
            file.FileStatus = FileStatus.None;
            file.FolderID = MakeId(GetParentFolderId(onedriveFile));
            file.ModifiedBy = OneDriveProviderInfo.Owner;
            file.ModifiedOn = onedriveFile.LastModifiedDateTime.HasValue ? TenantUtil.DateTimeFromUtc(onedriveFile.LastModifiedDateTime.Value.DateTime) : default;
            file.NativeAccessor = onedriveFile;
            file.ProviderId = OneDriveProviderInfo.ID;
            file.ProviderKey = OneDriveProviderInfo.ProviderKey;
            file.Title = MakeItemTitle(onedriveFile);
            file.RootFolderId = MakeId();
            file.RootFolderType = OneDriveProviderInfo.RootFolderType;
            file.RootFolderCreator = OneDriveProviderInfo.Owner;
            file.Shared = false;
            file.Version = 1;

            return file;
        }

        public Folder<string> GetRootFolder(string folderId)
        {
            return ToFolder(GetOneDriveItem(""));
        }

        protected Item GetOneDriveItem(string itemId)
        {
            var onedriveId = MakeOneDriveId(itemId);
            try
            {
                return OneDriveProviderInfo.GetOneDriveItem(onedriveId);
            }
            catch (Exception ex)
            {
                return new ErrorItem(ex, onedriveId);
            }
        }

        protected IEnumerable<string> GetChildren(string folderId)
        {
            return GetOneDriveItems(folderId).Select(entry => MakeId(entry.Id));
        }

        protected List<Item> GetOneDriveItems(string parentId, bool? folder = null)
        {
            var onedriveFolderId = MakeOneDriveId(parentId);
            var items = OneDriveProviderInfo.GetOneDriveItems(onedriveFolderId);

            if (folder.HasValue)
            {
                if (folder.Value)
                {
                    return items.Where(i => i.Folder != null).ToList();
                }

                return items.Where(i => i.File != null).ToList();
            }

            return items;
        }

        protected sealed class ErrorItem : Item
        {
            public string Error { get; set; }

            public string ErrorId { get; private set; }


            public ErrorItem(Exception e, object id)
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
            requestTitle = new Regex("\\.$").Replace(requestTitle, "_");
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