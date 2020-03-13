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


using ASC.Api.Core;
using ASC.Core;
using ASC.Core.Common.EF;
using ASC.Mail.Core.Dao.Entities;
using ASC.Mail.Core.Dao.Expressions.UserFolder;
using ASC.Mail.Core.Dao.Interfaces;
using ASC.Mail.Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ASC.Mail.Core.Dao
{
    public class UserFolderTreeDao : BaseDao, IUserFolderTreeDao
    {
        public UserFolderTreeDao(ApiContext apiContext,
            SecurityContext securityContext,
            DbContextManager<MailDbContext> dbContext)
            : base(apiContext, securityContext, dbContext)
        {
        }

        public List<UserFolderTreeItem> Get(IUserFoldersTreeExp exp)
        {
            var list = MailDb.MailUserFolderTree
                .Where(exp.GetExpression())
                .Select(ToUserFolderTreeItem)
                .ToList();

            return list;
        }

        public int Save(UserFolderTreeItem item)
        {
            var tree = new MailUserFolderTree
            {
                FolderId = item.FolderId,
                ParentId = item.ParentId,
                Level = item.Level
            };

            MailDb.MailUserFolderTree.Add(tree);

            var result = MailDb.SaveChanges();

            return result;
        }

        //private readonly string _levelUp = string.Format("{0} + 1", UserFolderTreeTable.Columns.Level);

        //private const string UFT_ALIAS = "uft";

        public int InsertFullPathToRoot(uint folderId, uint parentId)
        {
            var treeItems = MailDb.MailUserFolderTree
                .Where(t => t.FolderId == parentId);

            foreach (var t in treeItems)
            {
                t.Level += 1;
            }

            var result = MailDb.SaveChanges();

            return result;
        }

        public int Remove(IUserFoldersTreeExp exp)
        {
            var deleteQuery = MailDb.MailUserFolderTree
                .Where(exp.GetExpression());

            MailDb.MailUserFolderTree.RemoveRange(deleteQuery);

            var result = MailDb.SaveChanges();

            return result;
        }

        public void Move(uint folderId, uint toFolderId)
        {
            /*var exp = SimpleUserFoldersTreeExp.CreateBuilder()
                .SetParent(folderId)
                .Build();

            var subFolders = Get(exp)
                .ToDictionary(r => r.FolderId, r => r.Level);

            if (!subFolders.Any())
            {
                return;
            }

            var query = new SqlDelete(UserFolderTreeTable.TABLE_NAME)
                .Where(Exp.In(UserFolderTreeTable.Columns.FolderId, subFolders.Keys) &
                       !Exp.In(UserFolderTreeTable.Columns.ParentId, subFolders.Keys));

            // ReSharper disable once NotAccessedVariable
            var result = Db.ExecuteNonQuery(query);

            foreach (var subFolder in subFolders)
            {
                var subQuery = new SqlQuery(UserFolderTreeTable.TABLE_NAME)
                    .Select(subFolder.Key.ToString(CultureInfo.InvariantCulture),
                        UserFolderTreeTable.Columns.ParentId,
                        string.Format("{0} + {1}", _levelUp, subFolder.Value.ToString(CultureInfo.InvariantCulture)))
                    .Where(UserFolderTreeTable.Columns.FolderId, toFolderId);

                var insertQuery = new SqlInsert(UserFolderTreeTable.TABLE_NAME, true)
                    .InColumns(UserFolderTreeTable.Columns.FolderId,
                        UserFolderTreeTable.Columns.ParentId,
                        UserFolderTreeTable.Columns.Level)
                    .Values(subQuery);

                // ReSharper disable once RedundantAssignment
                result = Db.ExecuteNonQuery(insertQuery);
            }*/

            throw new NotImplementedException();
        }

        protected UserFolderTreeItem ToUserFolderTreeItem(MailUserFolderTree r)
        {
            var folder = new UserFolderTreeItem
            {
                FolderId = r.FolderId,
                ParentId = r.ParentId,
                Level = r.Level
            };

            return folder;
        }
    }
}