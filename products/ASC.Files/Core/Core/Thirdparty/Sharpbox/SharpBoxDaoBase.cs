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
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

using AppLimit.CloudComputing.SharpBox;
using AppLimit.CloudComputing.SharpBox.Exceptions;

using ASC.Common.Logging;
using ASC.Core;
using ASC.Core.Common.EF;
using ASC.Core.Tenants;
using ASC.Files.Core;
using ASC.Files.Core.EF;
using ASC.Web.Core.Files;
using ASC.Web.Files.Classes;
using ASC.Web.Studio.Core;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

namespace ASC.Files.Thirdparty.Sharpbox
{
    internal abstract class SharpBoxDaoBase : ThirdPartyProviderDao<SharpBoxProviderInfo>
    {
        protected override string Id { get => "sbox"; }

        protected SharpBoxDaoBase(IServiceProvider serviceProvider, UserManager userManager, TenantManager tenantManager, TenantUtil tenantUtil, DbContextManager<FilesDbContext> dbContextManager, SetupInfo setupInfo, IOptionsMonitor<ILog> monitor, FileUtility fileUtility, TempPath tempPath)
            : base(serviceProvider, userManager, tenantManager, tenantUtil, dbContextManager, setupInfo, monitor, fileUtility, tempPath)
        {
        }

        protected class ErrorEntry : ICloudDirectoryEntry
        {
            public ErrorEntry(Exception e, object id)
            {
                if (e != null) Error = e.Message;

                Id = string.IsNullOrEmpty((id ?? "").ToString()) ? "/" : (id ?? "").ToString();
            }

            public string Error { get; set; }

            public string Name
            {
                get { return "/"; }
            }

            public string Id { get; private set; }

            public long Length
            {
                get { return 0; }
            }

            public DateTime Modified
            {
                get { return DateTime.UtcNow; }
            }

            public string ParentID
            {
                get { return ""; }
                set { }
            }

            public ICloudDirectoryEntry Parent
            {
                get { return null; }
                set { }
            }

            public ICloudFileDataTransfer GetDataTransferAccessor()
            {
                return null;
            }

            public string GetPropertyValue(string key)
            {
                return null;
            }

            private readonly List<ICloudFileSystemEntry> _entries = new List<ICloudFileSystemEntry>(0);

            public IEnumerator<ICloudFileSystemEntry> GetEnumerator()
            {
                return _entries.GetEnumerator();
            }

            IEnumerator IEnumerable.GetEnumerator()
            {
                return GetEnumerator();
            }

            public ICloudFileSystemEntry GetChild(string name)
            {
                return null;
            }

            public ICloudFileSystemEntry GetChild(string name, bool bThrowException)
            {
                if (bThrowException) throw new ArgumentNullException(name);
                return null;
            }

            public ICloudFileSystemEntry GetChild(string idOrName, bool bThrowException, bool firstByNameIfNotFound)
            {
                if (bThrowException) throw new ArgumentNullException(idOrName);
                return null;
            }

            public ICloudFileSystemEntry GetChild(int idx)
            {
                return null;
            }

            public int Count
            {
                get { return 0; }
            }

            public nChildState HasChildrens
            {
                get { return nChildState.HasNoChilds; }
            }
        }

        protected Task<string> MappingIDAsync(string id)
        {
            return MappingIDAsync(id, false);
        }

        protected async Task UpdatePathInDBAsync(string oldValue, string newValue)
        {
            if (oldValue.Equals(newValue)) return;

            using var tx = FilesDbContext.Database.BeginTransaction();
            var oldIDs = await Query(FilesDbContext.ThirdpartyIdMapping)
                .Where(r => r.Id.StartsWith(oldValue))
                .Select(r => r.Id)
                .ToListAsync();

            foreach (var oldID in oldIDs)
            {
                var oldHashID = await MappingIDAsync(oldID);
                var newID = oldID.Replace(oldValue, newValue);
                var newHashID = await MappingIDAsync (newID);

                var mappingForUpdate = await Query(FilesDbContext.ThirdpartyIdMapping)
                    .Where(r => r.HashId == oldHashID)
                    .ToListAsync();

                foreach (var m in mappingForUpdate)
                {
                    m.Id = newID;
                    m.HashId = newHashID;
                }

                await FilesDbContext.SaveChangesAsync();

                var securityForUpdate = await Query(FilesDbContext.Security)
                    .Where(r => r.EntryId == oldHashID)
                    .ToListAsync();

                foreach (var s in securityForUpdate)
                {
                    s.EntryId = newHashID;
                }

                await FilesDbContext.SaveChangesAsync();

                var linkForUpdate = await Query(FilesDbContext.TagLink)
                    .Where(r => r.EntryId == oldHashID)
                    .ToListAsync();

                foreach (var l in linkForUpdate)
                {
                    l.EntryId = newHashID;
                }

                await FilesDbContext.SaveChangesAsync();
            }

            await tx.CommitAsync();
        }

        protected string MakePath(object entryId)
        {
            return $"/{Convert.ToString(entryId, CultureInfo.InvariantCulture).Trim('/')}";
        }

        protected override string MakeId(string path = null)
        {
            return path;
        }

        protected string MakeId(ICloudFileSystemEntry entry)
        {
            var path = string.Empty;
            if (entry != null && !(entry is ErrorEntry))
            {
                try
                {
                    path = ProviderInfo.Storage.GetFileSystemObjectPath(entry);
                }
                catch (Exception ex)
                {
                    Log.Error("Sharpbox makeId error", ex);
                }
            }
            else if (entry != null)
            {
                path = entry.Id;
            }
            var p = string.IsNullOrEmpty(path) || path == "/" ? "" : ("-" + path.Replace('/', '|'));
            return $"{PathPrefix}{p}";
        }

        protected string MakeTitle(ICloudFileSystemEntry fsEntry)
        {
            if (fsEntry is ICloudDirectoryEntry && IsRoot(fsEntry as ICloudDirectoryEntry))
            {
                return ProviderInfo.CustomerTitle;
            }

            return Global.ReplaceInvalidCharsAndTruncate(fsEntry.Name);
        }

        protected string PathParent(string path)
        {
            if (!string.IsNullOrEmpty(path))
            {
                var index = path.TrimEnd('/').LastIndexOf('/');
                if (index != -1)
                {
                    //Cut to it
                    return path.Substring(0, index);
                }
            }
            return path;
        }

        protected Folder<string> ToFolder(ICloudDirectoryEntry fsEntry)
        {
            if (fsEntry == null) return null;
            if (fsEntry is ErrorEntry)
            {
                //Return error entry
                return ToErrorFolder(fsEntry as ErrorEntry);
            }

            //var childFoldersCount = fsEntry.OfType<ICloudDirectoryEntry>().Count();//NOTE: Removed due to performance isssues
            var isRoot = IsRoot(fsEntry);

            var folder = GetFolder();

            folder.ID = MakeId(fsEntry);
            folder.FolderID = isRoot ? null : MakeId(fsEntry.Parent);
            folder.CreateOn = isRoot ? ProviderInfo.CreateOn : fsEntry.Modified;
            folder.ModifiedOn = isRoot ? ProviderInfo.CreateOn : fsEntry.Modified;
            folder.RootFolderId = RootFolderMakeId();

            folder.Title = MakeTitle(fsEntry);
            folder.TotalFiles = 0; /*fsEntry.Count - childFoldersCount NOTE: Removed due to performance isssues*/
            folder.TotalSubFolders = 0; /*childFoldersCount NOTE: Removed due to performance isssues*/

            if (folder.CreateOn != DateTime.MinValue && folder.CreateOn.Kind == DateTimeKind.Utc)
                folder.CreateOn = TenantUtil.DateTimeFromUtc(folder.CreateOn);

            if (folder.ModifiedOn != DateTime.MinValue && folder.ModifiedOn.Kind == DateTimeKind.Utc)
                folder.ModifiedOn = TenantUtil.DateTimeFromUtc(folder.ModifiedOn);

            return folder;
        }

        private static bool IsRoot(ICloudDirectoryEntry entry)
        {
            if (entry != null && entry.Name != null)
                return string.IsNullOrEmpty(entry.Name.Trim('/'));
            return false;
        }

        private File<string> ToErrorFile(ErrorEntry fsEntry)
        {
            if (fsEntry == null) return null;

            var file = GetErrorFile(new Thirdparty.ErrorEntry(fsEntry.Error, null));

            file.ID = MakeId(fsEntry);
            file.CreateOn = fsEntry.Modified;
            file.ModifiedOn = fsEntry.Modified;
            file.RootFolderId = MakeId(null);
            file.Title = MakeTitle(fsEntry);

            return file;
        }

        private Folder<string> ToErrorFolder(ErrorEntry fsEntry)
        {
            if (fsEntry == null) return null;
            var folder = GetErrorFolder(new Thirdparty.ErrorEntry(fsEntry.Error, null));

            folder.ID = MakeId(fsEntry);
            folder.CreateOn = fsEntry.Modified;
            folder.ModifiedOn = fsEntry.Modified;
            folder.RootFolderId = MakeId(null);
            folder.Title = MakeTitle(fsEntry);

            return folder;
        }

        protected File<string> ToFile(ICloudFileSystemEntry fsEntry)
        {
            if (fsEntry == null) return null;

            if (fsEntry is ErrorEntry)
            {
                //Return error entry
                return ToErrorFile(fsEntry as ErrorEntry);
            }

            var file = GetFile();

            file.ID = MakeId(fsEntry);
            file.ContentLength = fsEntry.Length;
            file.CreateOn = fsEntry.Modified.Kind == DateTimeKind.Utc ? TenantUtil.DateTimeFromUtc(fsEntry.Modified) : fsEntry.Modified;
            file.FolderID = MakeId(fsEntry.Parent);
            file.ModifiedOn = fsEntry.Modified.Kind == DateTimeKind.Utc ? TenantUtil.DateTimeFromUtc(fsEntry.Modified) : fsEntry.Modified;
            file.NativeAccessor = fsEntry;
            file.Title = MakeTitle(fsEntry);
            file.RootFolderId = RootFolderMakeId();

            return file;
        }

        private ICloudDirectoryEntry _rootFolder;
        protected ICloudDirectoryEntry RootFolder()
        {
            return _rootFolder ??= ProviderInfo.Storage.GetRoot();
        }

        private string _rootFolderId;
        protected string RootFolderMakeId()
        {
            return _rootFolderId ??= MakeId(RootFolder());
        }

        protected ICloudDirectoryEntry GetFolderById(object folderId)
        {
            try
            {
                var path = MakePath(folderId);
                return path == "/"
                           ? RootFolder()
                           : ProviderInfo.Storage.GetFolder(path);
            }
            catch (SharpBoxException sharpBoxException)
            {
                if (sharpBoxException.ErrorCode == SharpBoxErrorCodes.ErrorFileNotFound)
                {
                    return null;
                }
                return new ErrorEntry(sharpBoxException, folderId);
            }
            catch (Exception ex)
            {
                return new ErrorEntry(ex, folderId);
            }
        }

        protected ICloudFileSystemEntry GetFileById(object fileId)
        {
            try
            {
                return ProviderInfo.Storage.GetFile(MakePath(fileId), null);
            }
            catch (SharpBoxException sharpBoxException)
            {
                if (sharpBoxException.ErrorCode == SharpBoxErrorCodes.ErrorFileNotFound)
                {
                    return null;
                }
                return new ErrorEntry(sharpBoxException, fileId);
            }
            catch (Exception ex)
            {
                return new ErrorEntry(ex, fileId);
            }
        }

        protected IEnumerable<ICloudFileSystemEntry> GetFolderFiles(object folderId)
        {
            return GetFolderFiles(ProviderInfo.Storage.GetFolder(MakePath(folderId)));
        }

        protected IEnumerable<ICloudFileSystemEntry> GetFolderSubfolders(object folderId)
        {
            return GetFolderSubfolders(ProviderInfo.Storage.GetFolder(MakePath(folderId)));
        }

        protected IEnumerable<ICloudFileSystemEntry> GetFolderFiles(ICloudDirectoryEntry folder)
        {
            return folder.Where(x => !(x is ICloudDirectoryEntry));
        }

        protected IEnumerable<ICloudFileSystemEntry> GetFolderSubfolders(ICloudDirectoryEntry folder)
        {
            return folder.Where(x => x is ICloudDirectoryEntry);
        }

        protected string GetAvailableTitle(string requestTitle, ICloudDirectoryEntry parentFolder, Func<string, ICloudDirectoryEntry, bool> isExist)
        {
            if (!isExist(requestTitle, parentFolder)) return requestTitle;

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

            while (isExist(requestTitle, parentFolder))
            {
                requestTitle = re.Replace(requestTitle, MatchEvaluator);
            }
            return requestTitle;
        }

        protected async Task<string> GetAvailableTitleAsync(string requestTitle, ICloudDirectoryEntry parentFolder, Func<string, ICloudDirectoryEntry, Task<bool>> isExist)
        {
            if (!await isExist(requestTitle, parentFolder)) return requestTitle;

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

            while (await isExist(requestTitle, parentFolder))
            {
                requestTitle = re.Replace(requestTitle, MatchEvaluator);
            }
            return requestTitle;
        }

        protected override Task<IEnumerable<string>> GetChildrenAsync(string folderId)
        {
            var subFolders = GetFolderSubfolders(folderId).Select(x => MakeId(x));
            var files = GetFolderFiles(folderId).Select(x => MakeId(x));
            return Task.FromResult(subFolders.Concat(files));
        }

        private string MatchEvaluator(Match match)
        {
            var index = Convert.ToInt32(match.Groups[2].Value);
            var staticText = match.Value.Substring(string.Format(" ({0})", index).Length);
            return string.Format(" ({0}){1}", index + 1, staticText);
        }
    }
}