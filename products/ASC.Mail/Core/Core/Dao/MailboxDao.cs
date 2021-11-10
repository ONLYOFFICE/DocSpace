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

using ASC.Common;
using ASC.Common.Logging;
using ASC.Core;
using ASC.Core.Common.EF;
using ASC.Mail.Configuration;
using ASC.Mail.Core.Dao.Entities;
using ASC.Mail.Core.Dao.Expressions.Mailbox;
using ASC.Mail.Core.Dao.Interfaces;
using ASC.Mail.Core.Entities;
using ASC.Security.Cryptography;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

using static ASC.Mail.Core.Engine.MailboxEngine;

namespace ASC.Mail.Core.Dao
{
    [Scope]
    public class MailboxDao : BaseMailDao, IMailboxDao
    {
        private InstanceCrypto InstanceCrypto { get; }
        private MailSettings MailSettings { get; }
        private readonly ILog Log;

        private const string mailboxTableName = "mail_mailbox";

        private const string isProcessed_Column = "is_processed";
        private const string dateChecked_Column = "date_checked";
        private const string dateLoginDelayExpires_Column = "date_login_delay_expires";
        private const string enabled_Column = "enabled";
        private const string msgCountLast_Column = "msg_count_last";
        private const string sizeLast_Column = "size_last";
        private const string quotaError_Column = "quota_error";
        private const string token_Column = "token";
        private const string imapIntervals_Column = "imap_intervals";
        private const string isRemoved_Column = "is_removed";
        private const string id_Column = "id";

        public MailboxDao(
             TenantManager tenantManager,
             SecurityContext securityContext,
             IOptionsMonitor<ILog> optionsMonitor,
             DbContextManager<MailDbContext> dbContext,
             InstanceCrypto instanceCrypto,
             MailSettings mailSettings)
            : base(tenantManager, securityContext, dbContext)
        {
            InstanceCrypto = instanceCrypto;
            MailSettings = mailSettings;

            Log = optionsMonitor.Get("ASC.Mail.MailboxDao");
        }

        public Mailbox GetMailBox(IMailboxExp exp)
        {
            var mailbox = MailDbContext.MailMailbox
                .AsNoTracking()
                .Where(exp.GetExpression())
                .Select(ToMailbox)
                .FirstOrDefault();

            return mailbox;
        }

        public List<Mailbox> GetMailBoxes(IMailboxesExp exp)
        {
            var query = MailDbContext.MailMailbox
                 .Where(exp.GetExpression())
                 .Select(ToMailbox)
                 .ToList();

            if (!string.IsNullOrEmpty(exp.OrderBy) && exp.OrderAsc.HasValue)
            {
                if ((bool)exp.OrderAsc)
                {
                    query = query.OrderBy(b => b.DateChecked).ToList();
                }
                else
                {
                    query = query.OrderByDescending(b => b.DateChecked).ToList();
                }

            }

            if (exp.Limit.HasValue)
            {
                query = query.Take(exp.Limit.Value).ToList();
            }

            var mailboxes = query.ToList();

            return mailboxes;
        }
        public List<Mailbox> GetUniqueMailBoxes(IMailboxesExp exp)
        {
            var query = MailDbContext.MailMailbox
                .Where(exp.GetExpression())
                .Select(ToMailbox)
                .ToList();

            if (!string.IsNullOrEmpty(exp.OrderBy) && exp.OrderAsc.HasValue)
            {
                if ((bool)exp.OrderAsc)
                {
                    query = query.OrderBy(b => b.DateChecked).ToList();
                }
                else
                {
                    query = query.OrderByDescending(b => b.DateChecked).ToList();
                }
            }

            if (exp.Limit.HasValue)
            {
                query = query.Take(exp.Limit.Value).ToList();
            }

            var mailboxes = query.ToList();

            return mailboxes;
        }

        public Mailbox GetNextMailBox(IMailboxExp exp)
        {
            var mailbox = MailDbContext.MailMailbox
                 .Where(exp.GetExpression())
                 .OrderBy(mb => mb.Id)
                 .Select(ToMailbox)
                 .Take(1)
                 .SingleOrDefault();

            return mailbox;
        }

        public Tuple<int, int> GetRangeMailboxes(IMailboxExp exp)
        {
            var mbIds = MailDbContext.MailMailbox
                 .Where(exp.GetExpression())
                 .OrderBy(mb => mb.Id)
                 .Select(mb => (int)mb.Id)
                 .ToList();

            var exists = mbIds.Any();

            var min = exists ? mbIds.First() : 0;
            var max = exists ? mbIds.Last() : 0;

            var result = new Tuple<int, int>(min, max);

            return result;
        }

        public List<Tuple<int, string>> GetMailUsers(IMailboxExp exp)
        {
            var list = MailDbContext.MailMailbox
                .Where(exp.GetExpression())
                .Select(mb => new Tuple<int, string>(mb.Tenant, mb.IdUser))
                .ToList();

            return list;
        }

        public int SaveMailBox(Mailbox mailbox)
        {
            var mailMailbox = new MailMailbox
            {
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

            var result = MailDbContext.Entry(mailMailbox);
            result.State = mailMailbox.Id == 0
                ? EntityState.Added
                : EntityState.Modified;

            MailDbContext.SaveChanges();

            return (int)result.Entity.Id;
        }

        public bool SetMailboxRemoved(Mailbox mailbox)
        {
            var mailMailbox = new MailMailbox
            {
                Id = (uint)mailbox.Id,
                IsRemoved = true
            };

            MailDbContext.MailMailbox.Attach(mailMailbox);
            MailDbContext.Entry(mailMailbox).Property(x => x.IsRemoved).IsModified = true;

            var result = MailDbContext.SaveChanges();

            return result > 0;
        }

        public bool RemoveMailbox(Mailbox mailbox, MailDbContext context)
        {
            var mailMailbox = new MailMailbox
            {
                Id = (uint)mailbox.Id
            };

            context.MailMailbox.Remove(mailMailbox);

            var result = context.SaveChanges();

            return result > 0;
        }

        public bool Enable(IMailboxExp exp, bool enabled)
        {
            var mailboxes = MailDbContext.MailMailbox.Where(exp.GetExpression()).ToList();

            if (!mailboxes.Any())
                return false;

            foreach (var mb in mailboxes)
            {
                mb.Enabled = enabled;
                if (enabled)
                {
                    mb.DateAuthError = null;
                }
            }

            var result = MailDbContext.SaveChanges();

            if (result > 0)
            {
                Log.Debug($"Successfuly disabled mailbox(es)");
            }
            else if (result == 0)
            {
                Log.Debug($"Failed to disable mailbox(es)");
            }

            return result > 0;
        }

        public bool SetNextLoginDelay(IMailboxExp exp, TimeSpan delay)
        {
            var mailboxes = MailDbContext.MailMailbox
                .Where(exp.GetExpression());

            if (mailboxes == null)
                return false;

            foreach (var mailbox in mailboxes)
            {
                mailbox.IsProcessed = false;
                mailbox.DateLoginDelayExpires = DateTime.UtcNow.Add(delay);
            }

            var result = MailDbContext.SaveChanges();

            return result > 0;
        }

        public bool SetMailboxEmailIn(Mailbox mailbox, string emailInFolder)
        {
            var mailMailbox = MailDbContext.MailMailbox
                .Where(mb => mb.Id == mailbox.Id
                    && mb.Tenant == mailbox.Tenant
                    && mb.IdUser == mailbox.User
                    && mb.IsRemoved == false)
                .FirstOrDefault();

            if (mailMailbox == null)
                return false;

            mailMailbox.EmailInFolder = "" != emailInFolder ? emailInFolder : null;

            var result = MailDbContext.SaveChanges();

            return result > 0;
        }

        public bool SetMailboxesActivity(int tenant, string user, bool userOnline = true)
        {
            var mailMailbox = MailDbContext.MailMailbox
                .Where(mb => mb.Tenant == tenant
                    && mb.IdUser == user
                    && mb.IsRemoved == false)
                .FirstOrDefault();

            if (mailMailbox == null)
                return false;

            mailMailbox.DateUserChecked = DateTime.UtcNow;
            mailMailbox.UserOnline = userOnline;

            var result = MailDbContext.SaveChanges();

            return result > 0;
        }

        string mySqlUtcNow = "UTC_TIMESTAMP()";

        public bool SetMailboxInProcess(int id)
        {
            return MailDbContext.Database.ExecuteSqlRaw(
                string.Format(
                    "UPDATE {0} SET {1} = 1, {2} = {3} WHERE {4} = {5} AND {6} = 0 AND {7} = 0",
                    mailboxTableName, isProcessed_Column, dateChecked_Column, mySqlUtcNow,
                    id_Column, id, isProcessed_Column, isRemoved_Column)
                ) > 0;
        }

        public bool ReleaseMailbox(Mailbox mailbox, MailboxReleasedOptions rOptions)
        {
            if (rOptions.ServerLoginDelay < MailSettings.Defines.DefaultServerLoginDelay)
                rOptions.ServerLoginDelay = MailSettings.Defines.DefaultServerLoginDelay;

            var delay = rOptions.ServerLoginDelay > MailSettings.Defines.DefaultServerLoginDelay
                ? DateTime.UtcNow.AddSeconds(rOptions.ServerLoginDelay).ToUniversalTime().ToString("yyyy-MM-dd HH:mm:ss")
                : DateTime.UtcNow.AddSeconds(MailSettings.Defines.DefaultServerLoginDelay).ToUniversalTime().ToString("yyyy-MM-dd HH:mm:ss");

            var query = string.Format(
                "UPDATE {0} SET {1} = 0, {2} = {3}, {4} = '{5}', {6} = {7}, {8} = {9}, " +
                "{10} = {11}, {12} = {13}, {14} = {15}, {16} = {17} WHERE {18} = {19}",
                mailboxTableName, isProcessed_Column,
                dateChecked_Column, mySqlUtcNow,
                dateLoginDelayExpires_Column, delay,
                enabled_Column, rOptions.Enabled.HasValue ? rOptions.Enabled.Value : enabled_Column,
                msgCountLast_Column, rOptions.MessageCount.HasValue ? rOptions.MessageCount.Value : msgCountLast_Column,
                sizeLast_Column, rOptions.Size.HasValue ? rOptions.Size.Value : sizeLast_Column,
                quotaError_Column, rOptions.QuotaError.HasValue ? rOptions.QuotaError.Value : quotaError_Column,
                token_Column, !string.IsNullOrEmpty(rOptions.OAuthToken) ? InstanceCrypto.Encrypt(rOptions.OAuthToken) : token_Column,
                imapIntervals_Column, rOptions.ResetImapIntervals.HasValue ? "NULL" : !string.IsNullOrEmpty(rOptions.ImapIntervalsJson) ? rOptions.GetImapIntervalForQuery() : imapIntervals_Column,
                id_Column, mailbox.Id);

            return MailDbContext.Database.ExecuteSqlRaw(query) > 0;
        }

        public bool SetMailboxAuthError(int id, DateTime? authErrorDate)
        {
            var query = MailDbContext.MailMailbox
                .Where(mb => mb.Id == id);

            if (authErrorDate.HasValue)
            {
                query.Where(mb => mb.DateAuthError == null);
            }

            var mailMailbox = query.FirstOrDefault();

            if (mailMailbox == null)
                return false;

            mailMailbox.DateAuthError = authErrorDate;
            mailMailbox.DateChecked = DateTime.UtcNow;

            var result = MailDbContext.SaveChanges();

            return result > 0;
        }

        public List<int> SetMailboxesProcessed(int timeoutInMinutes)
        {
            var mailboxes = MailDbContext.MailMailbox
                .Where(mb => mb.IsProcessed == true
                    && mb.DateChecked != null
                    && EF.Functions.DateDiffMinute(mb.DateChecked, DateTime.UtcNow) > timeoutInMinutes);

            var mbList = mailboxes.ToList();

            if (!mailboxes.Any())
                return new List<int>();

            MailDbContext.Database.ExecuteSqlRaw(
                "UPDATE {0} SET {1} = 0 " +
                "WHERE {1} = 1 AND {2} IS NOT NULL AND {3} - {2} > {4}",
                mailboxTableName, isProcessed_Column, dateChecked_Column, DateTime.UtcNow, timeoutInMinutes);

            return mbList.Select(mb => (int)mb.Id).ToList();
        }

        public bool CanAccessTo(IMailboxExp exp)
        {
            var foundIds = MailDbContext.MailMailbox
               .Where(exp.GetExpression())
               .Select(mb => mb.Id).ToList();

            return foundIds.Any();
        }

        public MailboxStatus GetMailBoxStatus(IMailboxExp exp)
        {
            var status = MailDbContext.MailMailbox
               .Where(exp.GetExpression())
               .Select(ToMailboxStatus)
               .FirstOrDefault();

            return status;
        }

        protected MailboxStatus ToMailboxStatus(MailMailbox r)
        {
            var status = new MailboxStatus
            {
                Id = (int)r.Id,
                IsRemoved = r.IsRemoved,
                Enabled = r.Enabled,
                BeginDate = r.BeginDate
            };

            return status;
        }

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
                SmtpServerId = r.IdSmtpServer,
                ServerId = r.IdInServer,
                EmailInFolder = r.EmailInFolder,
                IsTeamlabMailbox = r.IsServerMailbox,
                DateCreated = r.DateCreated.GetValueOrDefault(),
                DateChecked = r.DateChecked.GetValueOrDefault(),
                DateUserChecked = r.DateUserChecked.GetValueOrDefault(),
                UserOnline = r.UserOnline,
                DateLoginDelayExpires = r.DateLoginDelayExpires,
                DateAuthError = r.DateAuthError
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
}