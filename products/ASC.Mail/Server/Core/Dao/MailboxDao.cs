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
using ASC.Common;
using ASC.Core;
using ASC.Core.Common.EF;
using ASC.Mail.Core.Dao.Entities;
using ASC.Mail.Core.Dao.Expressions.Mailbox;
using ASC.Mail.Core.Dao.Interfaces;
using ASC.Mail.Core.Entities;
using ASC.Security.Cryptography;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ASC.Mail.Core.Dao
{
    public class MailboxDao : BaseDao, IMailboxDao
    {
        public InstanceCrypto InstanceCrypto { get; }
        public MailboxDao(ApiContext apiContext,
            SecurityContext securityContext,
            DbContextManager<MailDbContext> dbContext,
            InstanceCrypto instanceCrypto) : 
            base(apiContext, securityContext, dbContext)
        {
            InstanceCrypto = instanceCrypto;
        }

        public Mailbox GetMailBox(IMailboxExp exp)
        {
            var mailbox = MailDb.MailMailbox
                .AsNoTracking()
                .Where(exp.GetExpression())
                .Select(ToMailbox)
                .SingleOrDefault();

            return mailbox;
        }

        public List<Mailbox> GetMailBoxes(IMailboxesExp exp)
        {
            var query = MailDb.MailMailbox
                 .Where(exp.GetExpression())
                 .Select(ToMailbox);

            if (!string.IsNullOrEmpty(exp.OrderBy) && exp.OrderAsc.HasValue)
            {
                //TODO: Fix
                ///query.OrderBy(mb => mb.
                //query.OrderBy(exp.OrderBy, exp.OrderAsc.Value);
            }

            if (exp.Limit.HasValue)
            {
                query.Take(exp.Limit.Value);
            }

            var mailboxes = query.ToList();

            return mailboxes;
        }

        public Mailbox GetNextMailBox(IMailboxExp exp)
        {
            var mailbox = MailDb.MailMailbox
                 .Where(exp.GetExpression())
                 .OrderBy(mb => mb.Id)
                 .Select(ToMailbox)
                 .Take(1)
                 .SingleOrDefault();

            return mailbox;
        }

        public Tuple<int, int> GetRangeMailboxes(IMailboxExp exp)
        {
            var range = MailDb.MailMailbox
                 .Where(exp.GetExpression())
                 .GroupBy(mb => mb.Id)
                 .Select(mb => new
                 {
                     Min = (int)mb.Min(o => o.Id),
                     Max = (int)mb.Max(o => o.Id)
                 })
                 .SingleOrDefault();

            return new Tuple<int, int>(range.Min, range.Max);
        }

        public List<Tuple<int, string>> GetMailUsers(IMailboxExp exp)
        {
            var list = MailDb.MailMailbox
                .Where(exp.GetExpression())
                .Select(mb => new Tuple<int, string>(mb.Tenant, mb.IdUser))
                .ToList();

            return list;
        }

        public int SaveMailBox(Mailbox mailbox)
        {
            var mailMailbox = new MailMailbox { 
                Id = (uint)mailbox.Id,
                Tenant = mailbox.Tenant,
                IdUser = mailbox.User,
                Address = mailbox.Address,
                Name = mailbox.Name,
                Enabled = mailbox.Enabled,
                IsRemoved = mailbox.IsRemoved,
                IsProcessed = mailbox.IsProcessed,
                IsServerMailbox = mailbox.IsTeamlabMailbox,
                Imap = mailbox.Imap,
                UserOnline = mailbox.UserOnline,
                IsDefault = mailbox.IsDefault,
                MsgCountLast = mailbox.MsgCountLast,
                SizeLast = mailbox.SizeLast,
                LoginDelay = mailbox.LoginDelay,
                QuotaError = mailbox.QuotaError,
                ImapIntervals = mailbox.ImapIntervals,
                BeginDate = mailbox.BeginDate,
                EmailInFolder = mailbox.EmailInFolder,
                Pop3Password = InstanceCrypto.Encrypt(mailbox.Password),
                SmtpPassword = !string.IsNullOrEmpty(mailbox.SmtpPassword)
                        ? InstanceCrypto.Encrypt(mailbox.SmtpPassword)
                        : "",
                Token = !string.IsNullOrEmpty(mailbox.OAuthToken)
                        ? InstanceCrypto.Encrypt(mailbox.OAuthToken)
                        : "",
                TokenType = mailbox.OAuthType,
                IdSmtpServer = mailbox.SmtpServerId,
                IdInServer = mailbox.ServerId,
                DateChecked = mailbox.DateChecked,
                DateUserChecked = mailbox.DateUserChecked,
                DateLoginDelayExpires = mailbox.DateLoginDelayExpires,
                DateAuthError = mailbox.DateAuthError,
                DateCreated = mailbox.DateCreated
            };

            var result = MailDb.Entry(mailMailbox);
            result.State = mailMailbox.Id == 0
                ? EntityState.Added
                : EntityState.Modified;

            MailDb.SaveChanges();

            return (int)result.Entity.Id;
        }

        public bool SetMailboxRemoved(Mailbox mailbox)
        {
            var mailMailbox = new MailMailbox
            {
                Id = (uint)mailbox.Id,
                IsRemoved = true
            };

            MailDb.MailMailbox.Attach(mailMailbox);
            MailDb.Entry(mailMailbox).Property(x => x.IsRemoved).IsModified = true;

            var result = MailDb.SaveChanges();

            return result > 0;
        }

        public bool RemoveMailbox(Mailbox mailbox)
        {
            var mailMailbox = new MailMailbox
            {
                Id = (uint)mailbox.Id
            };

            MailDb.MailMailbox.Remove(mailMailbox);

            var result = MailDb.SaveChanges();

            return result > 0;
        }

        /*public bool Enable(IMailboxExp exp, bool enabled)
        {
            var query = new SqlUpdate(MailboxTable.TABLE_NAME)
                .Set(MailboxTable.Columns.Enabled, enabled)
                .Where(exp.GetExpression());

            if (enabled)
                query.Set(MailboxTable.Columns.DateAuthError, null);

            var result = Db.ExecuteNonQuery(query);

            return result > 0;
        }

        public bool SetNextLoginDelay(IMailboxExp exp, TimeSpan delay)
        {
            var query = new SqlUpdate(MailboxTable.TABLE_NAME)
                .Set(MailboxTable.Columns.IsProcessed, false)
                .Set(string.Format(SET_LOGIN_DELAY_EXPIRES, (int)delay.TotalSeconds))
                .Where(exp.GetExpression());

            var result = Db.ExecuteNonQuery(query);

            return result > 0;
        }

        public bool SetMailboxEmailIn(Mailbox mailbox, string emailInFolder)
        {
            var query = new SqlUpdate(MailboxTable.TABLE_NAME)
                .Set(MailboxTable.Columns.EmailInFolder, "" != emailInFolder ? emailInFolder : null)
                .Where(MailboxTable.Columns.Id, mailbox.Id)
                .Where(MailboxTable.Columns.Tenant, mailbox.Tenant)
                .Where(MailboxTable.Columns.User, mailbox.User)
                .Where(MailboxTable.Columns.IsRemoved, false);

            var result = Db.ExecuteNonQuery(query);

            return result > 0;
        }

        public bool SetMailboxesActivity(int tenant, string user, bool userOnline = true)
        {
            var query = new SqlUpdate(MailboxTable.TABLE_NAME)
                .Where(MailboxTable.Columns.Tenant, tenant)
                .Where(MailboxTable.Columns.User, user)
                .Where(MailboxTable.Columns.IsRemoved, false)
                .Set(SET_DATE_USER_CHECKED)
                .Set(MailboxTable.Columns.UserOnline, userOnline);

            var result = Db.ExecuteNonQuery(query);

            return result > 0;
        }

        private const string SET_DATE_CHECKED = MailboxTable.Columns.DateChecked + " = UTC_TIMESTAMP()";
        private const string SET_DATE_USER_CHECKED = MailboxTable.Columns.DateUserChecked + " = UTC_TIMESTAMP()";

        private const string SET_LOGIN_DELAY_EXPIRES =
            MailboxTable.Columns.DateLoginDelayExpires + " = DATE_ADD(UTC_TIMESTAMP(), INTERVAL {0} SECOND)";

        private static readonly string SetDefaultLoginDelayExpires =
            MailboxTable.Columns.DateLoginDelayExpires + " = DATE_ADD(UTC_TIMESTAMP(), INTERVAL " +
            Defines.DefaultServerLoginDelayStr + " SECOND)";

        public bool SetMailboxInProcess(int id)
        {
            var query = new SqlUpdate(MailboxTable.TABLE_NAME)
                .Set(MailboxTable.Columns.IsProcessed, true)
                .Set(SET_DATE_CHECKED)
                .Where(MailboxTable.Columns.Id, id)
                .Where(MailboxTable.Columns.IsProcessed, false)
                .Where(MailboxTable.Columns.IsRemoved, false);

            var result = Db.ExecuteNonQuery(query);

            return result > 0;
        }

        public bool SetMailboxProcessed(Mailbox mailbox, int nextLoginDelay, bool? enabled = null,
            int? messageCount = null, long? size = null, bool? quotaError = null, string oAuthToken = null,
            string imapIntervalsJson = null, bool? resetImapIntervals = false)
        {
            if (nextLoginDelay < Defines.DefaultServerLoginDelay)
                nextLoginDelay = Defines.DefaultServerLoginDelay;

            var query = new SqlUpdate(MailboxTable.TABLE_NAME)
                .Set(MailboxTable.Columns.IsProcessed, false)
                .Set(SET_DATE_CHECKED)
                .Set(nextLoginDelay > Defines.DefaultServerLoginDelay
                    ? string.Format(SET_LOGIN_DELAY_EXPIRES, nextLoginDelay)
                    : SetDefaultLoginDelayExpires)
                .Where(MailboxTable.Columns.Id, mailbox.Id);

            if (enabled.HasValue)
            {
                query.Set(MailboxTable.Columns.Enabled, enabled.Value);
            }

            if (messageCount.HasValue)
            {
                query.Set(MailboxTable.Columns.MsgCountLast, messageCount.Value);
            }

            if (size.HasValue)
            {
                query.Set(MailboxTable.Columns.SizeLast, size.Value);
            }

            if (quotaError.HasValue)
            {
                query.Set(MailboxTable.Columns.QuotaError, quotaError.Value);
            }

            if (!string.IsNullOrEmpty(oAuthToken))
            {
                query.Set(MailboxTable.Columns.OAuthToken, MailUtil.EncryptPassword(oAuthToken));
            }

            if (resetImapIntervals.HasValue)
            {
                query.Set(MailboxTable.Columns.ImapIntervals, null);
            }
            else
            {
                if (!string.IsNullOrEmpty(imapIntervalsJson))
                {
                    query.Set(MailboxTable.Columns.ImapIntervals, imapIntervalsJson);
                }
            }

            var result = Db.ExecuteNonQuery(query);

            return result > 0;
        }

        public bool SetMailboxAuthError(int id, DateTime? authErroDate)
        {
            var query = new SqlUpdate(MailboxTable.TABLE_NAME)
                .Set(MailboxTable.Columns.DateAuthError, authErroDate)
                .Where(MailboxTable.Columns.Id, id);

            if (authErroDate.HasValue)
                query.Where(MailboxTable.Columns.DateAuthError, null);

            var result = Db.ExecuteNonQuery(query);

            return result > 0;
        }

        private const string SET_PROCESS_EXPIRES =
            "TIMESTAMPDIFF(MINUTE, " + MailboxTable.Columns.DateChecked + ", UTC_TIMESTAMP()) > {0}";

        public List<int> SetMailboxesProcessed(int timeoutInMinutes)
        {
            var query = new SqlQuery(MailboxTable.TABLE_NAME)
                .Select(MailboxTable.Columns.Id)
                .Where(MailboxTable.Columns.IsProcessed, true)
                .Where(!Exp.Eq(MailboxTable.Columns.DateChecked, null))
                .Where(string.Format(SET_PROCESS_EXPIRES, timeoutInMinutes));

            var ids = Db.ExecuteList(query)
                .ConvertAll(r => Convert.ToInt32(r[0]));

            if (!ids.Any())
                return ids;

            var update = new SqlUpdate(MailboxTable.TABLE_NAME)
                        .Set(MailboxTable.Columns.IsProcessed, false)
                        .Where(Exp.In(MailboxTable.Columns.Id, ids.ToArray()));

            Db.ExecuteNonQuery(update);

            return ids;
        }

        public bool CanAccessTo(IMailboxExp exp)
        {
            var query = new SqlQuery(MailboxTable.TABLE_NAME)
                .Select(MailboxTable.Columns.Id)
                .Where(exp.GetExpression());

            var foundIds = Db.ExecuteList(query)
                .ConvertAll(res => Convert.ToInt32(res[0]));

            return foundIds.Any();
        }

        public MailboxStatus GetMailBoxStatus(IMailboxExp exp)
        {
            var query = new SqlQuery(MailboxTable.TABLE_NAME)
                .Select(MailboxTable.Columns.Id,
                    MailboxTable.Columns.IsRemoved,
                    MailboxTable.Columns.Enabled,
                    MailboxTable.Columns.BeginDate)
                .Where(exp.GetExpression());

            return Db.ExecuteList(query)
                .ConvertAll(ToMMailboxStatus)
                .SingleOrDefault();
        }

        protected MailboxStatus ToMMailboxStatus(object[] r)
        {
            var status = new MailboxStatus
            {
                Id = Convert.ToInt32(r[0]),
                IsRemoved = Convert.ToBoolean(r[1]),
                Enabled = Convert.ToBoolean(r[2]),
                BeginDate = Convert.ToDateTime(r[3])
            };

            return status;
        }*/

        protected Mailbox ToMailbox(MailMailbox r)
        {
            var mb = new Mailbox
            {
                Id = (int)r.Id,
                User = r.IdUser,
                Tenant = r.Tenant,
                Address = r.Address,
                Enabled = r.Enabled,

                MsgCountLast = r.MsgCountLast,
                SizeLast = r.SizeLast,

                Name = r.Name,
                LoginDelay = r.LoginDelay,
                IsProcessed = r.IsProcessed,
                IsRemoved = r.IsRemoved,
                IsDefault = r.IsDefault,
                QuotaError = r.QuotaError,
                Imap = r.Imap,
                BeginDate = r.BeginDate,
                OAuthType = r.TokenType,

                ImapIntervals = r.ImapIntervals,
                SmtpServerId =r.IdSmtpServer,
                ServerId = r.IdInServer,
                EmailInFolder = r.EmailInFolder,
                IsTeamlabMailbox = r.IsServerMailbox,
                DateCreated = r.DateCreated.GetValueOrDefault(),
                DateChecked = r.DateChecked.GetValueOrDefault(),
                DateUserChecked = r.DateUserChecked.GetValueOrDefault(),
                UserOnline = r.UserOnline,
                DateLoginDelayExpires = r.DateLoginDelayExpires,
                DateAuthError =r.DateAuthError
            };

            string password = r.Pop3Password,
                smtpPassword = r.SmtpPassword,
                oAuthToken = r.Token;

            TryDecryptPassword(password, out password);

            mb.Password = password;

            if (!string.IsNullOrEmpty(smtpPassword))
            {
                TryDecryptPassword(smtpPassword, out smtpPassword);
            }

            mb.SmtpPassword = smtpPassword ?? "";

            TryDecryptPassword(oAuthToken, out oAuthToken);

            mb.OAuthToken = oAuthToken;

            return mb;
        }

        public bool TryDecryptPassword(string encryptedPassword, out string password)
        {
            password = "";
            try
            {
                if (string.IsNullOrEmpty(encryptedPassword))
                    return false;

                password = InstanceCrypto.Decrypt(encryptedPassword);
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }
    }

    public static class MailboxDaoExtension
    {
        public static DIHelper AddMailboxDaoService(this DIHelper services)
        {
            services.TryAddScoped<MailboxDao>();

            return services;
        }
    }
}