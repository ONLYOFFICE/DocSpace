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
using ASC.Mail.Core.Dao.Expressions.Conversation;
using ASC.Mail.Core.Dao.Interfaces;
using ASC.Mail.Core.Entities;
using ASC.Mail.Enums;

namespace ASC.Mail.Core.Dao
{
    public class ChainDao : BaseDao, IChainDao
    {
        public ChainDao(ApiContext apiContext,
            SecurityContext securityContext,
            DbContextManager<MailDbContext> dbContext)
            : base(apiContext, securityContext, dbContext)
        {
        }

        public List<Chain> GetChains(IConversationsExp exp)
        {
            var chains = MailDb.MailChain
                .Where(exp.GetExpression())
                .Select(ToChain)
                .ToList();

            return chains;
        }

        public Dictionary<int, int> GetChainCount(IConversationsExp exp)
        {
            var dictionary = MailDb.MailChain
                    .Where(exp.GetExpression())
                    .GroupBy(c => c.Folder, (folderId, c) =>
                    new
                    {
                        folder = (int)folderId,
                        count = c.Count()
                    })
                    .ToDictionary(o => o.folder, o => o.count);

            return dictionary;
        }

        //private const string QUERY_COUNT_FORMAT = "SELECT chains.{3}, COUNT(*) FROM " +
        //                                          "(select t.{3}, c.{4} from {0} t " +
        //                                          "inner join {1} m on t.{5} = m.{6} " +
        //                                          "inner join {2} c on m.{7} = c.{4} " +
        //                                          "where t.{8} = @tenant and t.{9} = @user {10}" +
        //                                          "group by t.{3}, c.{4}) as chains " + 
        //                                          "GROUP BY chains.{3};";

        public Dictionary<uint, int> GetChainUserFolderCount(bool? unread = null)
        {
            //TODO: Fix
            /*var query = string.Format(QUERY_COUNT_FORMAT,
                UserFoldertXMailTable.TABLE_NAME,
                MailTable.TABLE_NAME,
                ChainTable.TABLE_NAME,
                UserFoldertXMailTable.Columns.FolderId,
                ChainTable.Columns.Id,
                UserFoldertXMailTable.Columns.MailId,
                MailTable.Columns.Id,
                MailTable.Columns.ChainId,
                UserFoldertXMailTable.Columns.Tenant,
                UserFoldertXMailTable.Columns.User,
                unread.HasValue ? string.Format("and m.{0} = {1} ", MailTable.Columns.Unread, unread.Value ? 1 : 0) : "");

            var result = Db.ExecuteList(query, new { tenant = Tenant, user = CurrentUserId })
                .ConvertAll(r => new
                {
                    folder = Convert.ToUInt32(r[0]),
                    count = Convert.ToInt32(r[1])
                })
                .ToDictionary(o => o.folder, o => o.count);

            return result;*/

            throw new NotImplementedException();
        }

        public Dictionary<uint, int> GetChainUserFolderCount(List<int> userFolderIds, bool? unread = null)
        {
            //TODO: Fix
            /*var query = string.Format(QUERY_COUNT_FORMAT,
                UserFoldertXMailTable.TABLE_NAME,
                MailTable.TABLE_NAME,
                ChainTable.TABLE_NAME,
                UserFoldertXMailTable.Columns.FolderId,
                ChainTable.Columns.Id,
                UserFoldertXMailTable.Columns.MailId,
                MailTable.Columns.Id,
                MailTable.Columns.ChainId,
                UserFoldertXMailTable.Columns.Tenant,
                UserFoldertXMailTable.Columns.User,
                string.Format("and t.{0} in ({1}) {2}", UserFoldertXMailTable.Columns.FolderId, string.Join(",", userFolderIds), 
                    unread.HasValue 
                        ? string.Format("and m.{0} = {1} ", MailTable.Columns.Unread, unread.Value ? 1 : 0) 
                        : "")
                );

            var result = Db.ExecuteList(query, new { tenant = Tenant, user = CurrentUserId })
                .ConvertAll(r => new
                {
                    folder = Convert.ToUInt32(r[0]),
                    count = Convert.ToInt32(r[1])
                })
                .ToDictionary(o => o.folder, o => o.count);

            return result;*/

            throw new NotImplementedException();
        }

        public int SaveChain(Chain chain)
        {
            var mailChain = new MailChain { 
                Id = chain.Id,
                IdMailbox = (uint)chain.MailboxId,
                Tenant = (uint)chain.Tenant,
                IdUser = chain.User,
                Folder = (uint)chain.Folder,
                Length = (uint)chain.Length,
                Unread = chain.Unread,
                HasAttachments = chain.HasAttachments,
                Importance = chain.Importance,
                Tags = chain.Tags
            };

            MailDb.MailChain.Add(mailChain);

            var count = MailDb.SaveChanges();

            return count;
        }

        public int Delete(IConversationsExp exp)
        {
            var query = MailDb.MailChain.Where(exp.GetExpression());

            MailDb.MailChain.RemoveRange(query);

            var count = MailDb.SaveChanges();

            return count;
        }

        public int SetFieldValue<T>(IConversationsExp exp, string field, T value)
        {
            //TODO: Fix
            /*var query =
                new SqlUpdate(ChainTable.TABLE_NAME)
                    .Set(field, value)
                    .Where(exp.GetExpression());

            return Db.ExecuteNonQuery(query);*/

            throw new NotImplementedException();
        }

        protected Chain ToChain(MailChain r)
        {
            var chain = new Chain
            {
                Id = r.Id,
                MailboxId = (int)r.IdMailbox,
                Tenant = (int)r.Tenant,
                User = r.IdUser,
                Folder = (FolderType) r.Folder,
                Length = (int)r.Length,
                Unread = r.Unread,
                HasAttachments = r.HasAttachments,
                Importance = r.Importance,
                Tags = r.Tags
            };

            return chain;
        }
    }
}