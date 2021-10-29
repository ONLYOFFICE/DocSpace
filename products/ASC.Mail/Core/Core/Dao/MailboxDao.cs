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
using ASC.Mail.Extensions;
using ASC.Mail.Models;
using ASC.Security.Cryptography;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

namespace ASC.Mail.Core.Dao
{
    [Scope]
    public class MailboxDao : BaseMailDao, IMailboxDao
    {
        private InstanceCrypto InstanceCrypto { get; }
        private MailSettings MailSettings { get; }
        private readonly ILog Log;
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
                 .DistinctBy(b => b.Address)
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

        public int SetMailboxesInProcess(string address)
        {
            return MailDbContext.Database.ExecuteSqlRaw(
                "UPDATE mail_mailbox " +
                "SET is_processed = 1, date_checked = {0} " +
                "WHERE address = {1} AND is_processed = 0 AND is_removed = 0",
                DateTime.UtcNow, address);
        }

        public class MailboxReleasedOptions
        {
            public bool? Enabled = null;
            public bool? QuotaError = null;
            public string OAuthToken = null;
            public bool? ResetImapIntervals = null;
            public string ImapIntervalsJson = null;
            public int? MessageCount = null;
            public long? Size = null;

            public bool DisableBox;
            public int ServerLoginDelay;

            public MailboxReleasedOptions(bool disable, int loginDelay)
            {
                DisableBox = disable;
                ServerLoginDelay = loginDelay;
            }

            public void CleanOptions()
            {
                Enabled = null;
                QuotaError = null;
                OAuthToken = null;
                ResetImapIntervals = null;
                ImapIntervalsJson = null;
                MessageCount = null;
                Size = null;
            }
        }

        public bool ReleaseMailboxes(List<Mailbox> mailboxes, MailBoxData account, bool disable)
        {
            MailboxReleasedOptions rOptions = new MailboxReleasedOptions(disable, account.ServerLoginDelay);

            var boxStr = " WHERE id IN (";
            mailboxes.ForEach(b => boxStr += $"{b.Id}, ");
            boxStr = boxStr.Substring(0, boxStr.Length - 2);
            boxStr += ");";

            var delay = rOptions.ServerLoginDelay > MailSettings.Defines.DefaultServerLoginDelay
                ? DateTime.UtcNow.AddSeconds(rOptions.ServerLoginDelay).ToUniversalTime().ToString("yyyy-MM-dd HH:mm:ss")
                : DateTime.UtcNow.AddSeconds(MailSettings.Defines.DefaultServerLoginDelay).ToUniversalTime().ToString("yyyy-MM-dd HH:mm:ss");

            var query = $"UPDATE mail_mailbox SET is_processed = 0, date_checked = '{DateTime.UtcNow:yyyy-MM-dd HH:mm:ss}', " +
                $"date_login_delay_expires = '{delay}' ";

            if (rOptions.ServerLoginDelay < MailSettings.Defines.DefaultServerLoginDelay)
                rOptions.ServerLoginDelay = MailSettings.Defines.DefaultServerLoginDelay;

            (string, string) enableCase = (", enabled = CASE id", " ELSE enabled END");
            (string, string) msgCountLastCase = (", msg_count_last = CASE id", " ELSE msg_count_last END");
            (string, string) sizeLastCase = (", size_last = CASE id", " ELSE size_last END");
            (string, string) quotaErrorCase = (", quota_error = CASE id", " ELSE quota_error END");
            (string, string) tokenCase = (", token = CASE id ", " ELSE token END");
            (string, string) imapIntervalsCase = (", imap_intervals = CASE id", " ELSE imap_intervals END");

            foreach (var box in mailboxes)
            {
                if (box.Id == account.MailBoxId)
                {

                    if (box.MsgCountLast != account.MessagesCount) rOptions.MessageCount = account.MessagesCount;

                    if (box.SizeLast != account.Size) rOptions.Size = account.Size;

                    if (account.AccessTokenRefreshed)
                        rOptions.OAuthToken = account.OAuthToken;
                }

                if (account.Imap && account.ImapFolderChanged)
                {
                    if (account.BeginDateChanged) { rOptions.ResetImapIntervals = true; }
                    else { rOptions.ImapIntervalsJson = account.ImapIntervalsJson; }
                }
                if (account.AuthErrorDate.HasValue)
                    if (rOptions.DisableBox)
                        rOptions.Enabled = false;

                if (account.QuotaErrorChanged) rOptions.QuotaError = account.QuotaError;

                if (mailboxes.Count == 1)
                {
                    if (rOptions.Enabled.HasValue)
                    {
                        query += $", enabled = {rOptions.Enabled.Value}";
                    }

                    if (rOptions.MessageCount.HasValue)
                    {
                        query += $", msg_count_last = {rOptions.MessageCount.Value}";
                    }

                    if (rOptions.Size.HasValue)
                    {
                        query += $", size_last = {rOptions.Size.Value}";
                    }

                    if (rOptions.QuotaError.HasValue)
                    {
                        query += $", quota_error = {rOptions.QuotaError.Value}";
                    }

                    if (!string.IsNullOrEmpty(rOptions.OAuthToken))
                    {
                        query += $", token = '{InstanceCrypto.Encrypt(rOptions.OAuthToken)}'";
                    }

                    if (rOptions.ResetImapIntervals.HasValue)
                    {
                        query += $", imap_intervals = NULL";
                    }
                    else
                    {
                        if (!string.IsNullOrEmpty(rOptions.ImapIntervalsJson))
                        {
                            var newInterval = "";
                            foreach (var ch in rOptions.ImapIntervalsJson)
                            {
                                if (ch == '{') newInterval += "{{";
                                else if (ch == '}') newInterval += "}}";
                                else newInterval += ch;
                            }

                            query += $", imap_intervals = '{newInterval}'";
                        }
                    }

                    query += $" WHERE id = {box.Id};";

                    break;
                }

                if (rOptions.Enabled.HasValue)
                    enableCase.Item1 += $" WHEN {box.Id} THEN {Convert.ToInt32(rOptions.Enabled.Value)}";

                if (rOptions.QuotaError.HasValue)
                    quotaErrorCase.Item1 += $" WHEN {box.Id} THEN {Convert.ToInt32(rOptions.QuotaError.Value)}";

                if (rOptions.ResetImapIntervals.HasValue)
                {
                    imapIntervalsCase.Item1 += $" WHEN {box.Id} THEN NULL";
                }
                else
                {
                    if (!string.IsNullOrEmpty(rOptions.ImapIntervalsJson))
                    {
                        var newInterval = "";
                        foreach (var ch in rOptions.ImapIntervalsJson)
                        {
                            if (ch == '{') newInterval += "{{";
                            else if (ch == '}') newInterval += "}}";
                            else newInterval += ch;
                        }

                        imapIntervalsCase.Item1 += $" WHEN {box.Id} THEN '{newInterval}'";
                    }
                }

                if (rOptions.MessageCount.HasValue)
                    msgCountLastCase.Item1 += $" WHEN {box.Id} THEN {rOptions.MessageCount.Value}";

                if (rOptions.Size.HasValue)
                    sizeLastCase.Item1 += $" WHEN {box.Id} THEN {rOptions.Size.Value}";

                if (!string.IsNullOrEmpty(rOptions.OAuthToken))
                    tokenCase.Item1 += $" WHEN {box.Id} THEN '{InstanceCrypto.Encrypt(rOptions.OAuthToken)}'";

                rOptions.CleanOptions();
            }

            if (mailboxes.Count == 1)
            {
                return MailDbContext.Database.ExecuteSqlRaw(query) > 0;
            }
            else
            {
                var cases = new List<(string, string)> { enableCase, msgCountLastCase, sizeLastCase, quotaErrorCase, tokenCase, imapIntervalsCase };

                foreach (var c in cases)
                {
                    if (c.Item1.Contains("WHEN"))
                    {
                        query += c.Item1; query += c.Item2;
                    }
                }

                query += boxStr;

                return MailDbContext.Database.ExecuteSqlRaw(query) > 0;
            }
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
                "UPDATE mail_mailbox SET is_processed = 0 " +
                "WHERE is_processed = 1 AND date_checked IS NOT NULL AND {0} - date_checked > {1}",
                DateTime.UtcNow, timeoutInMinutes);

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