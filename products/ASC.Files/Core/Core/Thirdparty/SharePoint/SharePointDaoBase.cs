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
using System.Text.RegularExpressions;
using System.Threading.Tasks;

using ASC.Common.Logging;
using ASC.Core;
using ASC.Core.Common.EF;
using ASC.Core.Tenants;
using ASC.Files.Core.EF;
using ASC.Web.Core.Files;
using ASC.Web.Studio.Core;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

using Folder = Microsoft.SharePoint.Client.Folder;

namespace ASC.Files.Thirdparty.SharePoint
{
    internal class SharePointDaoBase : ThirdPartyProviderDao<SharePointProviderInfo>
    {
        protected override string Id { get => "spoint"; }

        public SharePointDaoBase(IServiceProvider serviceProvider, UserManager userManager, TenantManager tenantManager, TenantUtil tenantUtil, DbContextManager<FilesDbContext> dbContextManager, SetupInfo setupInfo, IOptionsMonitor<ILog> monitor, FileUtility fileUtility, TempPath tempPath) 
            : base(serviceProvider, userManager, tenantManager, tenantUtil, dbContextManager, setupInfo, monitor, fileUtility, tempPath)
        {
        }

        protected string GetAvailableTitle(string requestTitle, Folder parentFolderID, Func<string, Folder, bool> isExist)
        {
            if (!isExist(requestTitle, parentFolderID)) return requestTitle;

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

            while (isExist(requestTitle, parentFolderID))
            {
                requestTitle = re.Replace(requestTitle, MatchEvaluator);
            }
            return requestTitle;
        }

        protected async Task<string> GetAvailableTitleAsync(string requestTitle, Folder parentFolderID, Func<string, Folder, Task<bool>> isExist)
        {
            if (!await isExist(requestTitle, parentFolderID)) return requestTitle;

            var re = new Regex(@"( \(((?<index>[0-9])+)\)(\.[^\.]*)?)$");
            var match = re.Match(requestTitle);

            if (!match.Success)
            {
                var insertIndex = requestTitle.Length;
                if (requestTitle.LastIndexOf(".", StringComparison.Ordinal) != -1)
                {
                    insertIndex = requestTitle.LastIndexOf(".", StringComparison.Ordinal);
                }
                requestTitle = requestTitle.Insert(insertIndex, " (1)");
            }

            while (await isExist(requestTitle, parentFolderID))
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

        protected Task UpdatePathInDBAsync(string oldValue, string newValue)
        {
            if (oldValue.Equals(newValue)) return Task.CompletedTask;

            return InternalUpdatePathInDBAsync(oldValue, newValue);
        }

        private async Task InternalUpdatePathInDBAsync(string oldValue, string newValue)
        {
            using var tx = FilesDbContext.Database.BeginTransaction();
            var oldIDs = await Query(FilesDbContext.ThirdpartyIdMapping)
                .Where(r => r.Id.StartsWith(oldValue))
                .Select(r => r.Id)
                .ToListAsync();

            foreach (var oldID in oldIDs)
            {
                var oldHashID = await MappingIDAsync(oldID);
                var newID = oldID.Replace(oldValue, newValue);
                var newHashID = await MappingIDAsync(newID);

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

        protected Task<string> MappingIDAsync(string id)
        {
            return MappingIDAsync(id, false);
        }

        protected override string MakeId(string path = null)
        {
            return path;
        }

        protected override async Task<IEnumerable<string>> GetChildrenAsync(string folderId)
        {
            var folders = await ProviderInfo.GetFolderFoldersAsync(folderId);
            var subFolders = folders.Select(x => ProviderInfo.MakeId(x.ServerRelativeUrl));

            var folderFiles = await ProviderInfo.GetFolderFilesAsync(folderId);
            var files = folderFiles.Select(x => ProviderInfo.MakeId(x.ServerRelativeUrl));
            return subFolders.Concat(files);
        }
    }
}
