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


using System.Collections.Generic;
using System.Linq;
using ASC.Common;
using ASC.Core;
using ASC.Core.Common.EF;
using ASC.Mail.Core.Dao.Entities;
using ASC.Mail.Core.Dao.Interfaces;
using ASC.Mail.Enums;

namespace ASC.Mail.Core.Dao
{
    [Scope]
    public class TagMailDao : BaseDao, ITagMailDao
    {
        public TagMailDao(
             TenantManager tenantManager,
             SecurityContext securityContext,
             DbContextManager<MailDbContext> dbContext)
            : base(tenantManager, securityContext, dbContext)
        {
        }

        public void SetMessagesTag(IEnumerable<int> messageIds, int tagId)
        {
            var idMessages = messageIds as IList<int> ?? messageIds.ToList();
            if (!idMessages.Any())
                return;

            var items = new List<MailTagMail>();
            int i, messagessLen;
            for (i = 0, messagessLen = idMessages.Count; i < messagessLen; i++)
            {
                var messageId = idMessages[i];

                items.Add(new MailTagMail
                {
                    Tenant = Tenant,
                    IdUser = UserId,
                    IdMail = messageId,
                    IdTag = tagId
                });

                if ((i % 100 != 0 || i == 0) && i + 1 != messagessLen) 
                    continue;

                MailDb.MailTagMail.AddRange(items);

                MailDb.SaveChanges();

                items = new List<MailTagMail>();
            }
        }

        public int CalculateTagCount(int id)
        {
            var count = MailDb.MailTagMail
                .Where(t => t.Tenant == Tenant && t.IdUser == UserId && t.IdTag == id)
                .Count();

            return count;
        }

        public Dictionary<int, List<int>> GetMailTagsDictionary(List<int> mailIds)
        {
            var dictionary = MailDb.MailTagMail
                .Where(t => t.Tenant == Tenant && t.IdUser == UserId && mailIds.Contains(t.IdMail))
                .Select(t => new { t.IdMail, t.IdTag })
                .GroupBy(t => t.IdMail)
                .ToDictionary(g => g.Key, g => g.Select(t => t.IdTag).ToList());

            return dictionary;
        }

        public List<int> GetTagIds(List<int> mailIds)
        {
            var tagIds = MailDb.MailTagMail
                .Where(t => t.Tenant == Tenant && t.IdUser == UserId && mailIds.Contains(t.IdMail))
                .Select(t => t.IdTag)
                .Distinct()
                .ToList();

            return tagIds;
        }

        public List<int> GetTagIds(int mailboxId)
        {
            var tagIds = MailDb.MailTagMail
                .Join(MailDb.MailMail, tm => tm.IdMail, m => m.Id, 
                (tm, m) => new { 
                    TagMail = tm,
                    Mail = m
                })
                .Where(t => t.Mail.IdMailbox == mailboxId)
                .Select(t => t.TagMail.IdTag)
                .Distinct()
                .ToList();

            return tagIds;
        }

        public string GetChainTags(string chainId, FolderType folder, int mailboxId)
        {
            var tags = MailDb.MailTagMail.Join(MailDb.MailMail, t => t.IdMail, m => m.Id,
                (t, m) => new
                {
                    Tag = t,
                    Mail = m
                })
                .Where(g => g.Mail.ChainId == chainId && g.Mail.IsRemoved == false && g.Mail.Folder == (int)folder && g.Mail.IdMailbox == mailboxId)
                .Where(g => g.Tag.Tenant == Tenant && g.Tag.IdUser == UserId)
                .OrderBy(g => g.Tag.TimeCreated)
                .GroupBy(g => g.Tag.IdTag)
                .Select(g => g.Key)
                .ToList();

            return string.Join(",", tags);
        }

        public int Delete(int tagId, List<int> mailIds)
        {
            var deleteQuery = MailDb.MailTagMail
                .Where(t => t.Tenant == Tenant && t.IdUser == UserId && t.IdTag == tagId)
                .Where(t => mailIds.Contains(t.IdMail));

            MailDb.MailTagMail.RemoveRange(deleteQuery);

            var result = MailDb.SaveChanges();

            return result;
        }

        public int DeleteByTagId(int tagId)
        {
            var deleteQuery = MailDb.MailTagMail
                .Where(t => t.Tenant == Tenant && t.IdUser == UserId)
                .Where(t => t.IdTag == tagId);

            MailDb.MailTagMail.RemoveRange(deleteQuery);

            var result = MailDb.SaveChanges();

            return result;
        }

        public int DeleteByMailboxId(int mailboxId)
        {
            var deleteQuery = MailDb.MailTagMail
               .Join(MailDb.MailMail, tm => tm.IdMail, m => m.Id,
                (tm, m) => new {
                    TagMail = tm,
                    Mail = m
                })
                .Where(t => t.Mail.IdMailbox == mailboxId)
                .Select(t => t.TagMail);

            MailDb.MailTagMail.RemoveRange(deleteQuery);

            var result = MailDb.SaveChanges();

            return result;
        }

        public int DeleteByMailIds(List<int> mailIds)
        {
            var deleteQuery = MailDb.MailTagMail
                .Where(t => t.Tenant == Tenant && t.IdUser == UserId)
                .Where(t => mailIds.Contains(t.IdMail));

            MailDb.MailTagMail.RemoveRange(deleteQuery);

            var result = MailDb.SaveChanges();

            return result;
        }
    }
}