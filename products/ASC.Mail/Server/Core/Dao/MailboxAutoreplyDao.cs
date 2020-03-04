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
using ASC.Api.Core;
using ASC.Common;
using ASC.Core;
using ASC.Core.Common.EF;
using ASC.Mail.Core.Dao.Entities;
//using ASC.Common.Data;
//using ASC.Common.Data.Sql;
//using ASC.Common.Data.Sql.Expressions;
using ASC.Mail.Core.Dao.Interfaces;
using ASC.Mail.Core.Entities;
//using ASC.Mail.Core.DbSchema;
//using ASC.Mail.Core.DbSchema.Interfaces;
//using ASC.Mail.Core.DbSchema.Tables;
//using ASC.Mail.Core.Entities;
//using ASC.Mail.Extensions;

namespace ASC.Mail.Core.Dao
{
    public class MailboxAutoreplyDao : BaseDao, IMailboxAutoreplyDao
    {
        public MailboxAutoreplyDao(DbContextManager<MailDbContext> dbContext,
            ApiContext apiContext,
            SecurityContext securityContext)
            : base(apiContext, securityContext, dbContext)
        {
        }

        public MailboxAutoreply GetAutoreply(int mailboxId)
        {
            var autoreply = MailDb.MailMailboxAutoreply
                .Where(a => a.Tenant == Tenant && a.IdMailbox == mailboxId)
                .Select(ToAutoreply)
                .DefaultIfEmpty(new MailboxAutoreply
                {
                    MailboxId = mailboxId,
                    Tenant = Tenant,
                    TurnOn = false,
                    OnlyContacts = false,
                    TurnOnToDate = false,
                    FromDate = DateTime.MinValue,
                    ToDate = DateTime.MinValue,
                    Subject = string.Empty,
                    Html = string.Empty
                })
                .Single();

            return autoreply;
        }

        public List<MailboxAutoreply> GetAutoreplies(List<int> mailboxIds)
        {
            var autoreplies = MailDb.MailMailboxAutoreply
                .Where(a => a.Tenant == Tenant && mailboxIds.Contains(a.IdMailbox))
                .Select(ToAutoreply)
                .ToList();

            var notFoundIds = mailboxIds.Where(id => autoreplies.FirstOrDefault(a => a.MailboxId == id) == null).ToList();

            foreach (var id in notFoundIds)
            {
                autoreplies.Add(new MailboxAutoreply
                {
                    MailboxId = id,
                    Tenant = Tenant,
                    TurnOn = false,
                    OnlyContacts = false,
                    TurnOnToDate = false,
                    FromDate = DateTime.MinValue,
                    ToDate = DateTime.MinValue,
                    Subject = string.Empty,
                    Html = string.Empty
                });
            }

            return autoreplies;
        }

        public int SaveAutoreply(MailboxAutoreply autoreply)
        {
            MailDb.MailMailboxAutoreply.Add(new MailMailboxAutoreply
            {
                IdMailbox = autoreply.MailboxId,
                Tenant = autoreply.Tenant,
                TurnOn = autoreply.TurnOn,
                TurnOnToDate = autoreply.TurnOnToDate,
                OnlyContacts = autoreply.OnlyContacts,
                FromDate = autoreply.FromDate,
                ToDate = autoreply.ToDate,
                Subject = autoreply.Subject,
                Html = autoreply.Html
            });

            var count = MailDb.SaveChanges();

            return count;
        }

        public int DeleteAutoreply(int mailboxId)
        {
            using var tr = MailDb.Database.BeginTransaction();

            var range = MailDb.MailMailboxAutoreply
                .Where(r => r.Tenant == Tenant && r.IdMailbox == mailboxId);

            MailDb.MailMailboxAutoreply.RemoveRange(range);

            var count = MailDb.SaveChanges();

            tr.Commit();

            return count;
        }

        protected MailboxAutoreply ToAutoreply(MailMailboxAutoreply r)
        {
            var obj = new MailboxAutoreply
            {
                MailboxId = r.IdMailbox,
                Tenant = r.Tenant,
                TurnOn = r.TurnOn,
                OnlyContacts = r.OnlyContacts,
                TurnOnToDate = r.TurnOnToDate,
                FromDate = r.FromDate,
                ToDate = r.ToDate,
                Subject = r.Subject,
                Html = r.Html
            };

            return obj;
        }
    }

    public static class MailboxAutoreplyDaoExtension
    {
        public static DIHelper AddMailboxAutoreplyDaoService(this DIHelper services)
        {
            services.TryAddScoped<MailboxAutoreplyDao>();

            return services;
        }
    }
}