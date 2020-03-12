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
using ASC.Api.Core;
using ASC.Core;
using ASC.Core.Common.EF;
using ASC.Mail.Core.Dao.Entities;
using ASC.Mail.Core.Dao.Expressions.Message;
using ASC.Mail.Core.Dao.Interfaces;
using ASC.Mail.Core.Entities;
using ASC.Mail.Enums;

namespace ASC.Mail.Core.Dao
{
    public class MailInfoDao : BaseDao, IMailInfoDao
    {
        public MailInfoDao(ApiContext apiContext,
            SecurityContext securityContext,
            DbContextManager<MailDbContext> dbContext)
            : base(apiContext, securityContext, dbContext) { 
        }

        //private const string MM_ALIAS = "mm";
        //private const string MTM_ALIAS = "tm";
        //private const string UFXM_ALIAS = "ufxm";

        //private static readonly string CountMailId = "count(" + MailTable.Columns.Id.Prefix(MM_ALIAS) + ")";

        //private static readonly string ConcatTagIds =
        //    string.Format(
        //        "(SELECT CAST(group_concat({4}.{0} ORDER BY {4}.{3} SEPARATOR ',') AS CHAR) from {1} as {4} WHERE {4}.{2} = {5}.{6}) tagIds",
        //        TagMailTable.Columns.TagId,
        //        TagMailTable.TABLE_NAME,
        //        TagMailTable.Columns.MailId,
        //        TagMailTable.Columns.TimeCreated,
        //        MTM_ALIAS,
        //        MM_ALIAS,
        //        MailTable.Columns.Id);

        public List<MailInfo> GetMailInfoList(IMessagesExp exp, bool skipSelectTags = false)
        {
            /*var query = new SqlQuery(MailTable.TABLE_NAME.Alias(MM_ALIAS))
                .Select(MailTable.Columns.Id.Prefix(MM_ALIAS),
                    MailTable.Columns.From.Prefix(MM_ALIAS),
                    MailTable.Columns.To.Prefix(MM_ALIAS),
                    MailTable.Columns.Cc.Prefix(MM_ALIAS),
                    MailTable.Columns.Reply.Prefix(MM_ALIAS),
                    MailTable.Columns.Subject.Prefix(MM_ALIAS),
                    MailTable.Columns.Importance.Prefix(MM_ALIAS),
                    MailTable.Columns.DateSent.Prefix(MM_ALIAS),
                    MailTable.Columns.Size.Prefix(MM_ALIAS),
                    MailTable.Columns.AttachCount.Prefix(MM_ALIAS),
                    MailTable.Columns.Unread.Prefix(MM_ALIAS),
                    MailTable.Columns.IsAnswered.Prefix(MM_ALIAS),
                    MailTable.Columns.IsForwarded.Prefix(MM_ALIAS),
                    skipSelectTags ? "\"\" as tagIds" : ConcatTagIds,
                    MailTable.Columns.FolderRestore.Prefix(MM_ALIAS),
                    MailTable.Columns.Folder.Prefix(MM_ALIAS),
                    MailTable.Columns.ChainId.Prefix(MM_ALIAS),
                    MailTable.Columns.ChainDate.Prefix(MM_ALIAS),
                    MailTable.Columns.MailboxId.Prefix(MM_ALIAS),
                    MailTable.Columns.CalendarUid.Prefix(MM_ALIAS),
                    MailTable.Columns.Stream.Prefix(MM_ALIAS),
                    MailTable.Columns.Uidl.Prefix(MM_ALIAS),
                    MailTable.Columns.IsRemoved.Prefix(MM_ALIAS),
                    MailTable.Columns.Introduction.Prefix(MM_ALIAS));

            if (exp.TagIds != null && exp.TagIds.Any())
            {
                query
                    .InnerJoin(TagMailTable.TABLE_NAME.Alias(MTM_ALIAS),
                        Exp.EqColumns(MailTable.Columns.Id.Prefix(MM_ALIAS),
                            TagMailTable.Columns.MailId.Prefix(MTM_ALIAS)))
                    .Where(Exp.In(TagMailTable.Columns.TagId.Prefix(MTM_ALIAS), exp.TagIds))
                    .GroupBy(1)
                    .Having(Exp.Eq(CountMailId, exp.TagIds.Count));
            }

            if (exp.UserFolderId.HasValue)
            {
                query
                    .InnerJoin(UserFoldertXMailTable.TABLE_NAME.Alias(UFXM_ALIAS),
                        Exp.EqColumns(MailTable.Columns.Id.Prefix(MM_ALIAS),
                            UserFoldertXMailTable.Columns.MailId.Prefix(UFXM_ALIAS)))
                    .Where(UserFoldertXMailTable.Columns.FolderId.Prefix(UFXM_ALIAS), exp.UserFolderId.Value);
            }

            query.Where(exp.GetExpression());

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
                var sortField = MailTable.Columns.DateSent.Prefix(MM_ALIAS);

                if (exp.OrderBy == Defines.ORDER_BY_SUBJECT)
                {
                    sortField = MailTable.Columns.Subject.Prefix(MM_ALIAS);
                }
                else if (exp.OrderBy == Defines.ORDER_BY_SENDER)
                {
                    sortField = MailTable.Columns.From.Prefix(MM_ALIAS);
                }
                else if (exp.OrderBy == Defines.ORDER_BY_DATE_CHAIN)
                {
                    sortField = MailTable.Columns.ChainDate.Prefix(MM_ALIAS);
                }

                query.OrderBy(sortField, exp.OrderAsc != null && exp.OrderAsc.Value);
            }

            var list = Db.ExecuteList(query)
                .ConvertAll(ToMailInfo);

            return list;*/

            throw new NotImplementedException();
        }

        public long GetMailInfoTotal(IMessagesExp exp)
        {
            /*long total;

            var query = new SqlQuery(MailTable.TABLE_NAME.Alias(MM_ALIAS))
                .SelectCount(MailTable.Columns.Id.Prefix(MM_ALIAS));

            if (exp.TagIds != null && exp.TagIds.Any())
            {
                query
                    .InnerJoin(TagMailTable.TABLE_NAME.Alias(MTM_ALIAS),
                        Exp.EqColumns(MailTable.Columns.Id.Prefix(MM_ALIAS),
                            TagMailTable.Columns.MailId.Prefix(MTM_ALIAS)))
                    .Where(Exp.In(TagMailTable.Columns.TagId.Prefix(MTM_ALIAS), exp.TagIds))
                    .GroupBy(MailTable.Columns.Id.Prefix(MM_ALIAS))
                    .Having(Exp.Eq(CountMailId, exp.TagIds.Count));
            }

            if (exp.UserFolderId.HasValue)
            {
                query
                    .InnerJoin(UserFoldertXMailTable.TABLE_NAME.Alias(UFXM_ALIAS),
                        Exp.EqColumns(MailTable.Columns.Id.Prefix(MM_ALIAS),
                            UserFoldertXMailTable.Columns.MailId.Prefix(UFXM_ALIAS)))
                    .Where(UserFoldertXMailTable.Columns.FolderId.Prefix(UFXM_ALIAS), exp.UserFolderId.Value);
            }

            query.Where(exp.GetExpression());

            if (exp.TagIds != null && exp.TagIds.Any())
            {
                var queryTempCount = new SqlQuery()
                    .SelectCount()
                    .From(query, "tbl");

                total = Db.ExecuteScalar<long>(queryTempCount);
            }
            else
            {
                total = Db.ExecuteScalar<long>(query);
            }

            return total;*/

            throw new NotImplementedException();
        }

        public Dictionary<int, int> GetMailCount(IMessagesExp exp)
        {
            /*var query = new SqlQuery(MailTable.TABLE_NAME.Alias(MM_ALIAS))
                .Select(MailTable.Columns.Folder.Prefix(MM_ALIAS))
                .SelectCount()
                .Where(exp.GetExpression())
                .GroupBy(MailTable.Columns.Folder.Prefix(MM_ALIAS));

            return Db.ExecuteList(query)
                .ConvertAll(r => new
                {
                    folder = Convert.ToInt32(r[0]),
                    count = Convert.ToInt32(r[1])
                })
                .ToDictionary(o => o.folder, o => o.count);*/

            throw new NotImplementedException();
        }

        public Dictionary<uint, int> GetMailUserFolderCount(List<int> userFolderIds, bool? unread = null)
        {
            /*var exp = Exp.Eq(UserFoldertXMailTable.Columns.Tenant.Prefix(UFXM_ALIAS), Tenant) &
                      Exp.Eq(UserFoldertXMailTable.Columns.User.Prefix(UFXM_ALIAS), User) &
                      Exp.In(UserFoldertXMailTable.Columns.FolderId.Prefix(UFXM_ALIAS), userFolderIds);

            if (unread.HasValue)
            {
                exp = exp & Exp.Eq(MailTable.Columns.Unread.Prefix(MM_ALIAS), unread.Value);
            }

            var query = new SqlQuery(UserFoldertXMailTable.TABLE_NAME.Alias(UFXM_ALIAS))
                .InnerJoin(MailTable.TABLE_NAME.Alias(MM_ALIAS),
                    Exp.EqColumns(
                        UserFoldertXMailTable.Columns.MailId.Prefix(UFXM_ALIAS),
                        MailTable.Columns.Id.Prefix(MM_ALIAS)))
                .Select(UserFoldertXMailTable.Columns.FolderId.Prefix(UFXM_ALIAS))
                .SelectCount()
                .Where(exp)
                .GroupBy(UserFoldertXMailTable.Columns.FolderId.Prefix(UFXM_ALIAS));

            var result = Db.ExecuteList(query)
                .ConvertAll(r => new
                {
                    folder = Convert.ToUInt32(r[0]),
                    count = Convert.ToInt32(r[1])
                })
                .ToDictionary(o => o.folder, o => o.count);

            return result;*/

            throw new NotImplementedException();
        }

        public Dictionary<uint, int> GetMailUserFolderCount(bool? unread = null)
        {
            /*var exp = Exp.Eq(UserFoldertXMailTable.Columns.Tenant.Prefix(UFXM_ALIAS), Tenant) &
                      Exp.Eq(UserFoldertXMailTable.Columns.User.Prefix(UFXM_ALIAS), User);

            if (unread.HasValue)
            {
                exp = exp & Exp.Eq(MailTable.Columns.Unread.Prefix(MM_ALIAS), unread.Value);
            }

            var query = new SqlQuery(UserFoldertXMailTable.TABLE_NAME.Alias(UFXM_ALIAS))
                .InnerJoin(MailTable.TABLE_NAME.Alias(MM_ALIAS),
                    Exp.EqColumns(
                        UserFoldertXMailTable.Columns.MailId.Prefix(UFXM_ALIAS),
                        MailTable.Columns.Id.Prefix(MM_ALIAS)))
                .Select(UserFoldertXMailTable.Columns.FolderId.Prefix(UFXM_ALIAS))
                .SelectCount()
                .Where(exp)
                .GroupBy(UserFoldertXMailTable.Columns.FolderId.Prefix(UFXM_ALIAS));

            var result = Db.ExecuteList(query)
                .ConvertAll(r => new
                {
                    folder = Convert.ToUInt32(r[0]),
                    count = Convert.ToInt32(r[1])
                })
                .ToDictionary(o => o.folder, o => o.count);

            return result;*/

            throw new NotImplementedException();
        }

        public Tuple<int, int> GetRangeMails(IMessagesExp exp)
        {
            /*var query = new SqlQuery(MailTable.TABLE_NAME.Alias(MM_ALIAS))
                .SelectMin(MailTable.Columns.Id.Prefix(MM_ALIAS))
                .SelectMax(MailTable.Columns.Id.Prefix(MM_ALIAS))
                .Where(exp.GetExpression());

            var range = Db.ExecuteList(query)
                .ConvertAll(r => new Tuple<int, int>(Convert.ToInt32(r[0]), Convert.ToInt32(r[1])))
                .SingleOrDefault();

            return range;*/

            throw new NotImplementedException();
        }

        public T GetFieldMaxValue<T>(IMessagesExp exp, string field)
        {
            /*var fieldQuery = new SqlQuery(MailTable.TABLE_NAME.Alias(MM_ALIAS))
                .SelectMax(field.Prefix(MM_ALIAS))
                .Where(exp.GetExpression());

            var fieldVal = Db.ExecuteScalar<T>(fieldQuery);

            return fieldVal;*/
            throw new NotImplementedException();
        }

        public int SetFieldValue<T>(IMessagesExp exp, string field, T value)
        {
            /*var query =
                new SqlUpdate(MailTable.TABLE_NAME.Alias(MM_ALIAS))
                    .Set(field.Prefix(MM_ALIAS), value)
                    .Where(exp.GetExpression());

            var result = Db.ExecuteNonQuery(query);

            return result;*/

            throw new NotImplementedException();
        }

        public int SetFieldsEqual(IMessagesExp exp, string fieldFrom, string fieldTo)
        {
            /*var query =
                new SqlUpdate(MailTable.TABLE_NAME.Alias(MM_ALIAS))
                    .Set(string.Format("{0}={1}", fieldTo.Prefix(MM_ALIAS), fieldFrom.Prefix(MM_ALIAS)))
                    .Where(exp.GetExpression());

            var result = Db.ExecuteNonQuery(query);

            return result;*/

            throw new NotImplementedException();
        }

        protected MailInfo ToMailInfo(MailMail r)
        {
            var mailInfo = new MailInfo
            {
                Id = r.Id,
                From = r.FromText,
                To = r.ToText,
                Cc = r.Cc,
                ReplyTo = r.ReplyTo,
                Subject = r.Subject,
                Importance = r.Importance,
                DateSent = r.DateSent,
                Size = r.Size,
                HasAttachments = r.AttachmentsCount > 0,
                IsNew = r.Unread,
                IsAnswered = r.IsAnswered,
                IsForwarded = r.IsForwarded,
                LabelsString = "", //TODO: fix Convert.ToString(r[13]),
                FolderRestore = (FolderType) r.Folder,
                Folder = (FolderType) r.FolderRestore,
                ChainId = r.ChainId,
                ChainDate = r.ChainDate,
                MailboxId = r.IdMailbox,
                CalendarUid = string.IsNullOrEmpty(r.CalendarUid) ? null : r.CalendarUid,
                Stream = r.Stream,
                Uidl = r.Uidl,
                IsRemoved = r.IsRemoved,
                Intoduction = r.Introduction
            };

            return mailInfo;
        }
    }
}