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
using System.Linq.Expressions;
using System.Text.RegularExpressions;

using ASC.Common.Caching;
using ASC.Core;
using ASC.Core.Common.EF;
using ASC.Core.Common.Settings;
using ASC.Core.Tenants;
using ASC.Files.Core.EF;
using ASC.Security.Cryptography;
using ASC.Web.Studio.Core;
using ASC.Web.Studio.UserControls.Statistics;
using ASC.Web.Studio.Utility;

using Microsoft.EntityFrameworkCore;

namespace ASC.Files.Core.Data
{
    public class AbstractDao
    {
        protected readonly ICache cache;

        private Lazy<FilesDbContext> LazyFilesDbContext { get; }
        public FilesDbContext FilesDbContext { get => LazyFilesDbContext.Value; }

        private int tenantID;
        protected internal int TenantID { get => tenantID != 0 ? tenantID : (tenantID = TenantManager.GetCurrentTenant().TenantId); }
        protected UserManager UserManager { get; }
        protected TenantManager TenantManager { get; }
        protected TenantUtil TenantUtil { get; }
        protected SetupInfo SetupInfo { get; }
        protected TenantExtra TenantExtra { get; }
        protected TenantStatisticsProvider TenantStatisticProvider { get; }
        protected CoreBaseSettings CoreBaseSettings { get; }
        protected CoreConfiguration CoreConfiguration { get; }
        protected SettingsManager SettingsManager { get; }
        protected AuthContext AuthContext { get; }
        protected IServiceProvider ServiceProvider { get; }

        protected AbstractDao(
            DbContextManager<FilesDbContext> dbContextManager,
            UserManager userManager,
            TenantManager tenantManager,
            TenantUtil tenantUtil,
            SetupInfo setupInfo,
            TenantExtra tenantExtra,
            TenantStatisticsProvider tenantStatisticProvider,
            CoreBaseSettings coreBaseSettings,
            CoreConfiguration coreConfiguration,
            SettingsManager settingsManager,
            AuthContext authContext,
            IServiceProvider serviceProvider, 
            ICache cache)
        {
            this.cache = cache;
            LazyFilesDbContext = new Lazy<FilesDbContext>(() => dbContextManager.Get(FileConstant.DatabaseId));
            UserManager = userManager;
            TenantManager = tenantManager;
            TenantUtil = tenantUtil;
            SetupInfo = setupInfo;
            TenantExtra = tenantExtra;
            TenantStatisticProvider = tenantStatisticProvider;
            CoreBaseSettings = coreBaseSettings;
            CoreConfiguration = coreConfiguration;
            SettingsManager = settingsManager;
            AuthContext = authContext;
            ServiceProvider = serviceProvider;
        }


        protected IQueryable<T> Query<T>(DbSet<T> set) where T : class, IDbFile
        {
            var tenantId = TenantID;
            return set.Where(r => r.TenantId == tenantId);
        }

        protected internal IQueryable<DbFile> GetFileQuery(Expression<Func<DbFile, bool>> where)
        {
            return Query(FilesDbContext.Files)
                .Where(where);
        }

        protected void GetRecalculateFilesCountUpdate(int folderId)
        {
            var folders = FilesDbContext.Folders
                .Where(r => r.TenantId == TenantID)
                .Where(r => FilesDbContext.Tree.Where(r => r.FolderId == folderId).Select(r => r.ParentId).Any(a => a == r.Id))
                .ToList();

            foreach (var f in folders)
            {
                var filesCount =
                    FilesDbContext.Files
                    .Join(FilesDbContext.Tree, a => a.FolderId, b => b.FolderId, (file, tree) => new { file, tree })
                    .Where(r => r.file.TenantId == f.TenantId)
                    .Where(r => r.tree.ParentId == f.Id)
                    .Select(r=> r.file.Id)
                    .Distinct()
                    .Count();

                f.FilesCount = filesCount;
            }

            FilesDbContext.SaveChanges();
        }

        protected object MappingID(object id, bool saveIfNotExist)
        {
            if (id == null) return null;

            var isNumeric = int.TryParse(id.ToString(), out var n);

            if (isNumeric) return n;

            object result;

            if (id.ToString().StartsWith("sbox")
                || id.ToString().StartsWith("box")
                || id.ToString().StartsWith("dropbox")
                || id.ToString().StartsWith("spoint")
                || id.ToString().StartsWith("drive")
                || id.ToString().StartsWith("onedrive"))
            {
                result = Regex.Replace(BitConverter.ToString(Hasher.Hash(id.ToString(), HashAlg.MD5)), "-", "").ToLower();
            }
            else
            {
                result = Query(FilesDbContext.ThirdpartyIdMapping)
                    .Where(r => r.HashId == id.ToString())
                    .Select(r => r.Id)
                    .FirstOrDefault();
            }

            if (saveIfNotExist)
            {
                var newItem = new DbFilesThirdpartyIdMapping
                {
                    Id = id.ToString(),
                    HashId = result.ToString(),
                    TenantId = TenantID
                };

                FilesDbContext.AddOrUpdate(r => r.ThirdpartyIdMapping, newItem);
            }

            return result;
        }

        protected int MappingID(int id)
        {
            return id;
        }
        protected object MappingID(object id)
        {
            return MappingID(id, false);
        }

        internal static IQueryable<T> BuildSearch<T>(IQueryable<T> query, string text, SearhTypeEnum searhTypeEnum) where T : IDbSearch
        {
            var lowerText = text.ToLower().Trim().Replace("%", "\\%").Replace("_", "\\_");

            return searhTypeEnum switch
            {
                SearhTypeEnum.Start => query.Where(r => r.Title.ToLower().StartsWith(lowerText)),
                SearhTypeEnum.End => query.Where(r => r.Title.ToLower().EndsWith(lowerText)),
                SearhTypeEnum.Any => query.Where(r => r.Title.ToLower().Contains(lowerText)),
                _ => query,
            };
        }

        internal enum SearhTypeEnum
        {
            Start,
            End,
            Any
        }
    }
}