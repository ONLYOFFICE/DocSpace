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
using System.Linq;
using System.Text.RegularExpressions;

using ASC.Core;
using ASC.Core.Common.EF;
using ASC.Core.Tenants;
using ASC.Files.Core;
using ASC.Files.Core.EF;
using ASC.Files.Core.Thirdparty;
using ASC.Security.Cryptography;
using ASC.Web.Studio.Core;

using Microsoft.EntityFrameworkCore;

using Folder = Microsoft.SharePoint.Client.Folder;

namespace ASC.Files.Thirdparty.SharePoint
{
    internal class SharePointDaoBase : IThirdPartyProviderDao<SharePointProviderInfo>
    {
        public SharePointProviderInfo ProviderInfo { get; private set; }
        public RegexDaoSelectorBase<SharePointProviderInfo> SharePointDaoSelector { get; private set; }

        public SharePointDaoBase(
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

        public void Init(BaseProviderInfo<SharePointProviderInfo> sharePointInfo, RegexDaoSelectorBase<SharePointProviderInfo> sharePointDaoSelector)
        {
            ProviderInfo = sharePointInfo.ProviderInfo;
            SharePointDaoSelector = sharePointDaoSelector;
        }

        protected int TenantID { get; set; }

        public IServiceProvider ServiceProvider { get; }
        public UserManager UserManager { get; }
        public TenantUtil TenantUtil { get; }
        public FilesDbContext FilesDbContext { get; }
        public SetupInfo SetupInfo { get; }

        protected IQueryable<T> Query<T>(DbSet<T> set) where T : class, IDbFile
        {
            return set.Where(r => r.TenantId == TenantID);
        }

        protected string GetAvailableTitle(string requestTitle, Folder parentFolderID, Func<string, Folder, bool> isExist)
        {
            if (!isExist(requestTitle, parentFolderID)) return requestTitle;

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

            while (isExist(requestTitle, parentFolderID))
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

        protected string MappingID(string id, bool saveIfNotExist)
        {
            if (id == null) return null;

            string result;

            if (id.ToString().StartsWith("spoint"))
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

        protected void UpdatePathInDB(string oldValue, string newValue)
        {
            if (oldValue.Equals(newValue)) return;

            using (var tx = FilesDbContext.Database.BeginTransaction())
            {
                var oldIDs = Query(FilesDbContext.ThirdpartyIdMapping)
                    .Where(r => r.Id.StartsWith(oldValue))
                    .Select(r => r.Id)
                    .ToList();

                foreach (var oldID in oldIDs)
                {
                    var oldHashID = MappingID(oldID);
                    var newID = oldID.Replace(oldValue, newValue);
                    var newHashID = MappingID(newID);

                    var mappingForUpdate = Query(FilesDbContext.ThirdpartyIdMapping)
                        .Where(r => r.HashId == oldHashID)
                        .ToList();

                    foreach (var m in mappingForUpdate)
                    {
                        m.Id = newID;
                        m.HashId = newHashID;
                    }

                    FilesDbContext.SaveChanges();

                    var securityForUpdate = Query(FilesDbContext.Security)
                        .Where(r => r.EntryId == oldHashID)
                        .ToList();

                    foreach (var s in securityForUpdate)
                    {
                        s.EntryId = newHashID;
                    }

                    FilesDbContext.SaveChanges();

                    var linkForUpdate = Query(FilesDbContext.TagLink)
                        .Where(r => r.EntryId == oldHashID)
                        .ToList();

                    foreach (var l in linkForUpdate)
                    {
                        l.EntryId = newHashID;
                    }

                    FilesDbContext.SaveChanges();
                }

                tx.Commit();
            }
        }

        protected string MappingID(string id)
        {
            return MappingID(id, false);
        }

        public void Dispose()
        {
            if (ProviderInfo != null)
            {
                ProviderInfo.Dispose();
            }
        }
    }
}
