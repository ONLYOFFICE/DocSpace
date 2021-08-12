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
using System.Data;
using System.Linq;

using ASC.Common;
using ASC.Core;
using ASC.Core.Common.EF;
using ASC.Mail.Core.Dao.Interfaces;
using ASC.Mail.Models;

namespace ASC.Mail.Core.Dao
{
    [Scope]
    public class MailGarbageDao : BaseMailDao, IMailGarbageDao
    {
        public MailGarbageDao(
             TenantManager tenantManager,
             SecurityContext securityContext,
             DbContextManager<MailDbContext> dbContext)
            : base(tenantManager, securityContext, dbContext)
        {
        }

        public int GetMailboxAttachsCount(MailBoxData mailBoxData)
        {
            var count = MailDbContext.MailMail
                .Where(m => m.IdMailbox == mailBoxData.MailBoxId
                    && m.TenantId == mailBoxData.TenantId
                    && m.IdUser == mailBoxData.UserId)
                .Join(MailDbContext.MailAttachment, m => m.Id, a => a.IdMail,
                    (m, a) => new
                    {
                        Mail = m,
                        Attachment = a
                    })
                .Count();

            return count;
        }

        public List<MailAttachGarbage> GetMailboxAttachs(MailBoxData mailBoxData, int limit)
        {
            var list = MailDbContext.MailMail
                .Where(m => m.IdMailbox == mailBoxData.MailBoxId
                    && m.TenantId == mailBoxData.TenantId
                    && m.IdUser == mailBoxData.UserId)
                .Join(MailDbContext.MailAttachment, m => m.Id, a => a.IdMail,
                    (m, a) => new
                    {
                        Mail = m,
                        Attachment = a
                    })
                .Select(r => new MailAttachGarbage(mailBoxData.UserId, r.Attachment.Id,
                    r.Mail.Stream, r.Attachment.FileNumber, r.Attachment.StoredName)
                )
                .Take(limit)
                .ToList();

            return list;
        }

        public void CleanupMailboxAttachs(List<MailAttachGarbage> attachGarbageList)
        {
            if (!attachGarbageList.Any()) return;

            var ids = attachGarbageList.Select(a => a.Id).ToList();

            var deleteQuery = MailDbContext.MailAttachment.Where(m => ids.Contains(m.Id));

            MailDbContext.MailAttachment.RemoveRange(deleteQuery);

            MailDbContext.SaveChanges();
        }

        public int GetMailboxMessagesCount(MailBoxData mailBoxData)
        {
            var count = MailDbContext.MailMail
                .Where(m => m.IdMailbox == mailBoxData.MailBoxId
                    && m.TenantId == mailBoxData.TenantId
                    && m.IdUser == mailBoxData.UserId)
                .Count();

            return count;
        }

        public List<MailMessageGarbage> GetMailboxMessages(MailBoxData mailBoxData, int limit)
        {
            var list = MailDbContext.MailMail
                .Where(m => m.IdMailbox == mailBoxData.MailBoxId
                    && m.TenantId == mailBoxData.TenantId
                    && m.IdUser == mailBoxData.UserId)
                .Select(r => new MailMessageGarbage(mailBoxData.UserId, r.Id, r.Stream))
                .Take(limit)
                .ToList();

            return list;
        }

        public void CleanupMailboxMessages(List<MailMessageGarbage> messageGarbageList)
        {
            if (!messageGarbageList.Any()) return;

            var ids = messageGarbageList.Select(a => a.Id).ToList();

            var deleteQuery = MailDbContext.MailMail.Where(m => ids.Contains(m.Id));

            MailDbContext.MailMail.RemoveRange(deleteQuery);

            MailDbContext.SaveChanges();
        }
    }
}
