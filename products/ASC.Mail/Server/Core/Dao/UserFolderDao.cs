/*
 *
 * (c) Copyright Ascensio System Limited 2010-2020
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
using ASC.Api.Core;
using ASC.Core;
using ASC.Core.Common.EF;
using ASC.Mail.Core.Dao.Entities;
using ASC.Mail.Core.Dao.Expressions.UserFolder;
using ASC.Mail.Core.Dao.Interfaces;
using ASC.Mail.Core.Entities;
using ASC.Mail.Extensions;

namespace ASC.Mail.Core.Dao
{
    public class UserFolderDao : BaseDao, IUserFolderDao
    {
        public UserFolderDao(ApiContext apiContext,
            SecurityContext securityContext,
            DbContextManager<MailDbContext> dbContext)
            : base(apiContext, securityContext, dbContext)
        {
        }

        public UserFolder Get(uint id)
        {
            /*var query = Query()
                .Where(UserFolderTable.Columns.Tenant, Tenant)
                .Where(UserFolderTable.Columns.User, CurrentUserId)
                .Where(UserFolderTable.Columns.Id, id);

            var result = Db.ExecuteList(query)
                .ConvertAll(ToUserFolder)
                .SingleOrDefault();

            return result;*/

            throw new NotImplementedException();
        }

        public UserFolder GetByMail(uint mailId)
        {
            /*var subQuery = new SqlQuery(UserFoldertXMailTable.TABLE_NAME)
                .Select(UserFoldertXMailTable.Columns.FolderId)
                .Where(UserFoldertXMailTable.Columns.MailId, mailId)
                .Distinct();

            var query = Query()
                .Where(Exp.EqColumns(UserFolderTable.Columns.Id, subQuery));

            var result = Db.ExecuteList(query)
                .ConvertAll(ToUserFolder)
                .SingleOrDefault();

            return result;*/

            throw new NotImplementedException();
        }

        public List<UserFolder> GetList(IUserFoldersExp exp)
        {
            /*var query = Query()
                .Where(exp.GetExpression());

            if (exp.StartIndex.HasValue)
            {
                query.SetFirstResult(exp.StartIndex.Value);
            }

            if (exp.Limit.HasValue)
            {
                query.SetMaxResults(exp.Limit.Value);
            }

            if (!string.IsNullOrEmpty(exp.OrderBy))
            {
                var sortField = UserFolderTable.Columns.Name;

                if (exp.OrderBy == "timeModified")
                {
                    sortField = UserFolderTable.Columns.TimeModified;
                }

                query.OrderBy(sortField, exp.OrderAsc != null && exp.OrderAsc.Value);
            }

            var list = Db.ExecuteList(query)
                .ConvertAll(ToUserFolder);

            return list;*/

            throw new NotImplementedException();
        }

        public UserFolder GetRootFolder(uint folderId)
        {
            /*var subQuery = new SqlQuery(UserFolderTreeTable.TABLE_NAME)
                .Select(UserFolderTreeTable.Columns.ParentId)
                .Where(UserFolderTreeTable.Columns.FolderId, folderId)
                .SetMaxResults(1)
                .OrderBy(UserFolderTreeTable.Columns.Level, false);

            var query = Query()
                .Where(Exp.EqColumns(UserFolderTable.Columns.Id, subQuery));

            var result = Db.ExecuteList(query)
                .ConvertAll(ToUserFolder)
                .SingleOrDefault();

            return result;*/

            throw new NotImplementedException();
        }

        public UserFolder GetRootFolderByMailId(int mailId)
        {
            /*var subSubQuery = new SqlQuery(UserFoldertXMailTable.TABLE_NAME)
                .Select(UserFoldertXMailTable.Columns.FolderId)
                .Where(UserFoldertXMailTable.Columns.MailId, mailId)
                .Distinct();

            var subQuery = new SqlQuery(UserFolderTreeTable.TABLE_NAME)
                .Select(UserFolderTreeTable.Columns.ParentId)
                .Where(Exp.EqColumns(UserFolderTreeTable.Columns.FolderId, subSubQuery))
                .SetMaxResults(1)
                .OrderBy(UserFolderTreeTable.Columns.Level, false);

            var query = Query()
                .Where(Exp.EqColumns(UserFolderTable.Columns.Id, subQuery));

            var result = Db.ExecuteList(query)
                .ConvertAll(ToUserFolder)
                .SingleOrDefault();

            return result;*/

            throw new NotImplementedException();
        }

        public List<UserFolder> GetParentFolders(uint folderId)
        {
            /*const string folder_alias = "f";
            const string folder_tree_alias = "ft";

            var query = Query(folder_alias)
                .InnerJoin(UserFolderTreeTable.TABLE_NAME.Alias(folder_tree_alias),
                    Exp.EqColumns(UserFolderTable.Columns.Id.Prefix(folder_alias),
                        UserFolderTreeTable.Columns.ParentId.Prefix(folder_tree_alias)))
                .Where(UserFolderTreeTable.Columns.FolderId.Prefix(folder_tree_alias), folderId)
                .OrderBy(UserFolderTreeTable.Columns.Level.Prefix(folder_tree_alias), false);

            var list = Db.ExecuteList(query)
                .ConvertAll(ToUserFolder);

            return list;*/

            throw new NotImplementedException();
        }

        public uint Save(UserFolder folder)
        {
            /*var query = new SqlInsert(UserFolderTable.TABLE_NAME, true)
                .InColumnValue(UserFolderTable.Columns.Id, folder.Id)
                .InColumnValue(UserFolderTable.Columns.ParentId, folder.ParentId)
                .InColumnValue(UserFolderTable.Columns.Tenant, folder.Tenant)
                .InColumnValue(UserFolderTable.Columns.User, folder.User)
                .InColumnValue(UserFolderTable.Columns.Name, folder.Name)
                .InColumnValue(UserFolderTable.Columns.FolderCount, folder.FolderCount)
                .InColumnValue(UserFolderTable.Columns.UnreadMessagesCount, folder.UnreadCount)
                .InColumnValue(UserFolderTable.Columns.TotalMessagesCount, folder.TotalCount)
                .InColumnValue(UserFolderTable.Columns.UnreadConversationsCount, folder.UnreadChainCount)
                .InColumnValue(UserFolderTable.Columns.TotalConversationsCount, folder.TotalChainCount)
                .InColumnValue(UserFolderTable.Columns.TimeModified, folder.TimeModified)
                .Identity(0, (uint) 0, true);

            return Db.ExecuteScalar<uint>(query);*/

            throw new NotImplementedException();
        }

        public int Remove(uint id)
        {
            /*var query = new SqlDelete(UserFolderTable.TABLE_NAME)
                .Where(UserFolderTable.Columns.Tenant, Tenant)
                .Where(UserFolderTable.Columns.User, CurrentUserId)
                .Where(UserFolderTable.Columns.Id, id);

            var result = Db.ExecuteNonQuery(query);

            return result;*/

            throw new NotImplementedException();
        }

        public int Remove(IUserFoldersExp exp)
        {
            /*var query = new SqlDelete(UserFolderTable.TABLE_NAME)
                .Where(exp.GetExpression());

            var result = Db.ExecuteNonQuery(query);

            return result;*/

            throw new NotImplementedException();
        }

        //private static readonly string SetFolderCount =
        //    string.Format("{0} = (select count(*) - 1 from {1} where {2} = {3})",
        //        UserFolderTable.Columns.FolderCount, UserFolderTreeTable.TABLE_NAME,
        //        UserFolderTreeTable.Columns.ParentId, UserFolderTable.Columns.Id);

        public void RecalculateFoldersCount(uint id)
        {
            /*var subQuery = new SqlQuery(UserFolderTreeTable.TABLE_NAME)
                .Select(UserFolderTreeTable.Columns.ParentId)
                .Where(UserFolderTreeTable.Columns.FolderId, id);

            var query = new SqlUpdate(UserFolderTable.TABLE_NAME)
                .Set(SetFolderCount)
                .Where(Exp.In(UserFolderTable.Columns.Id, subQuery));

            // ReSharper disable once UnusedVariable
            var result =  Db.ExecuteNonQuery(query);*/

            throw new NotImplementedException();
        }

        //private const string INCR_VALUE_FORMAT = "{0}={0}+({1})";
        //private const string SET_VALUE_FORMAT = "{0}={1}";

        public int SetFolderCounters(uint folderId, int? unreadMess = null, int? totalMess = null,
            int? unreadConv = null, int? totalConv = null)
        {
            /*if (!unreadMess.HasValue
                && !totalMess.HasValue
                && !unreadConv.HasValue
                && !totalConv.HasValue)
            {
                return -1;
            }

            var updateQuery = new SqlUpdate(UserFolderTable.TABLE_NAME)
                .Where(UserFolderTable.Columns.Tenant, Tenant)
                .Where(UserFolderTable.Columns.User, CurrentUserId)
                .Where(UserFolderTable.Columns.Id, folderId);

            Action<string, int?> setColumnValue = (column, item) =>
            {
                if (!item.HasValue)
                    return;

                updateQuery.Set(string.Format(SET_VALUE_FORMAT, column, item.Value));
            };

            setColumnValue(UserFolderTable.Columns.UnreadMessagesCount, unreadMess);

            setColumnValue(UserFolderTable.Columns.TotalMessagesCount, totalMess);

            setColumnValue(UserFolderTable.Columns.UnreadConversationsCount, unreadConv);

            setColumnValue(UserFolderTable.Columns.TotalConversationsCount, totalConv);

            var result = Db.ExecuteNonQuery(updateQuery);

            return result;*/

            throw new NotImplementedException();
        }

        public int ChangeFolderCounters(uint folderId, int? unreadMessDiff = null, int? totalMessDiff = null,
            int? unreadConvDiff = null, int? totalConvDiff = null)
        {
            /*if (!unreadMessDiff.HasValue
                && !totalMessDiff.HasValue
                && !unreadConvDiff.HasValue
                && !totalConvDiff.HasValue)
            {
                return -1;
            }

            var updateQuery = new SqlUpdate(UserFolderTable.TABLE_NAME)
                .Where(UserFolderTable.Columns.Tenant, Tenant)
                .Where(UserFolderTable.Columns.User, CurrentUserId)
                .Where(UserFolderTable.Columns.Id, folderId);

            Action<string, int?> setColumnValue = (column, item) =>
            {
                if (!item.HasValue)
                    return;

                updateQuery.Set(item.Value != 0
                    ? string.Format(INCR_VALUE_FORMAT, column, item.Value)
                    : string.Format(SET_VALUE_FORMAT, column, 0));
            };

            setColumnValue(UserFolderTable.Columns.UnreadMessagesCount, unreadMessDiff);

            setColumnValue(UserFolderTable.Columns.TotalMessagesCount, totalMessDiff);

            setColumnValue(UserFolderTable.Columns.UnreadConversationsCount, unreadConvDiff);

            setColumnValue(UserFolderTable.Columns.TotalConversationsCount, totalConvDiff);

            var result = Db.ExecuteNonQuery(updateQuery);

            return result;*/

            throw new NotImplementedException();
        }

        protected UserFolder ToUserFolder(MailUserFolder r)
        {
            var folder = new UserFolder
            {
                Id = r.Id,
                ParentId = (uint)r.ParentId,
                
                Tenant = r.Tenant,
                User = r.IdUser,
                
                Name = r.Name,
                FolderCount = (int)r.FoldersCount,

                UnreadCount = (int)r.UnreadMessagesCount,
                TotalCount = (int)r.TotalMessagesCount,

                UnreadChainCount = (int)r.UnreadConversationsCount,
                TotalChainCount = (int)r.TotalConversationsCount,

                TimeModified = r.ModifiedOn
            };

            return folder;
        }
    }
}