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
using ASC.Core.Tenants;
using ASC.Files.Core.EF;
using ASC.Security.Cryptography;

using Autofac;

using Microsoft.EntityFrameworkCore;

namespace ASC.Files.Core.Data
{
    public class AbstractDao
    {
        protected readonly ICache cache;

        public FilesDbContext FilesDbContext { get; }

        internal int TenantID { get; }
        public TenantUtil TenantUtil { get; }

        protected AbstractDao(DbContextManager<FilesDbContext> dbContextManager, TenantManager tenantManager, TenantUtil tenantUtil, string storageKey)
        {
            cache = AscCache.Memory;
            FilesDbContext = dbContextManager.Value;
            TenantID = tenantManager.GetCurrentTenant().TenantId;
            TenantUtil = tenantUtil;
        }


        protected IQueryable<T> Query<T>(Expression<Func<FilesDbContext, DbSet<T>>> func) where T : class, IDbFile
        {
            var compile = func.Compile();
            return compile(FilesDbContext).Where(r => r.TenantId == 1);
        }

        protected IQueryable<DbFile> GetFileQuery(Expression<Func<DbFile, bool>> where)
        {
            return Query(r => r.Files)
                .Where(where);
        }

        protected List<File> FromQuery(IQueryable<DbFile> dbFiles, bool checkShared = true)
        {
            return dbFiles
                .Select(r => new { file = r, root = GetRootFolderType(r), shared = checkShared ? GetSharedQuery(FileEntryType.File, r) : true })
                .ToList()
                .Select(r => new File
                {
                    ID = r.file.Id,
                    Title = r.file.Title,
                    FolderID = r.file.FolderId,
                    CreateOn = TenantUtil.DateTimeFromUtc(r.file.CreateOn),
                    CreateBy = r.file.CreateBy,
                    Version = r.file.Version,
                    VersionGroup = r.file.VersionGroup,
                    ContentLength = r.file.ContentLength,
                    ModifiedOn = TenantUtil.DateTimeFromUtc(r.file.ModifiedOn),
                    ModifiedBy = r.file.ModifiedBy,
                    RootFolderType = ParseRootFolderType(r.root),
                    RootFolderCreator = ParseRootFolderCreator(r.root),
                    RootFolderId = ParseRootFolderId(r.root),
                    Shared = r.shared,
                    ConvertedType = r.file.ConvertedType,
                    Comment = r.file.Comment,
                    Encrypted = r.file.Encrypted,
                    Forcesave = r.file.Forcesave
                }).ToList();
        }

        protected string GetRootFolderType(DbFile file)
        {
            return FilesDbContext.Folders
                .Join(FilesDbContext.FolderTree, a => a.Id, b => b.ParentId, (folder, tree) => new { folder, tree })
                .Where(r => r.folder.TenantId == file.TenantId)
                .Where(r => r.tree.FolderId == file.FolderId)
                .OrderByDescending(r => r.tree.Level)
                .Select(r => r.folder.FolderType + r.folder.CreateBy.ToString() + r.folder.Id.ToString())
                .FirstOrDefault();
        }

        protected FolderType ParseRootFolderType(object v)
        {
            return v != null
                       ? (FolderType)Enum.Parse(typeof(FolderType), v.ToString().Substring(0, 1))
                       : default;
        }

        protected Guid ParseRootFolderCreator(object v)
        {
            return v != null ? new Guid(v.ToString().Substring(1, 36)) : default;
        }

        protected int ParseRootFolderId(object v)
        {
            return v != null ? int.Parse(v.ToString().Substring(1 + 36)) : default;
        }

        protected bool GetSharedQuery(FileEntryType type, DbFile dbFile)
        {
            return
                FilesDbContext.FilesSecurity
                .Where(r => r.EntryType == type)
                .Where(r => r.EntryId == dbFile.Id.ToString())
                .Any();
        }

        protected void GetRecalculateFilesCountUpdate(int folderId)
        {
            var folders = FilesDbContext.Folders
                .Where(r => r.TenantId == TenantID)
                .Where(r => FilesDbContext.FolderTree.Where(r => r.FolderId == folderId).Select(r => r.ParentId).Any(a => a == r.Id));

            foreach (var f in folders)
            {
                var filesCount =
                    FilesDbContext.Files
                    .Join(FilesDbContext.FolderTree, a => a.FolderId, b => b.FolderId, (file, tree) => new { file, tree })
                    .Where(r => r.file.TenantId == f.TenantId)
                    .Where(r => r.tree.ParentId == f.Id)
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
                result = Query(r => r.FilesThirdpartyIdMapping)
                    .Where(r => r.HashId == id.ToString())
                    .Select(r => r.Id)
                    .FirstOrDefault();
            }

            if (saveIfNotExist)
            {
                var newItem = new DbFilesThirdpartyIdMapping
                {
                    Id = id.ToString(),
                    HashId = result.ToString()
                };

                FilesDbContext.AddOrUpdate(r => r.FilesThirdpartyIdMapping, newItem);
            }

            return result;
        }

        protected object MappingID(object id)
        {
            return MappingID(id, false);
        }

        //public static Exp BuildSearch(string column, string text, SqlLike like = SqlLike.AnyWhere)
        //{
        //    return Exp.Like(string.Format("lower({0})", column), text.ToLower().Trim().Replace("%", "\\%").Replace("_", "\\_"), like);
        //}
    }
}