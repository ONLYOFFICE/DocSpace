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
using System.Linq;
using System.Net;
using System.Security;

using ASC.Common.Caching;
using ASC.Common.Logging;
using ASC.Core;
using ASC.Core.Common.EF;
using ASC.Core.Common.EF.Context;
using ASC.Core.Common.EF.Model.Mail;
using ASC.Core.Users;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace ASC.Web.Core.Mail
{
    public class MailServiceHelperStorage
    {
        public ICacheNotify<MailServiceHelperCache> CacheNotify { get; }
        public ICache Cache { get; }
        public MailServiceHelperStorage(ICacheNotify<MailServiceHelperCache> cacheNotify)
        {
            Cache = AscCache.Memory;
            CacheNotify = cacheNotify;
            CacheNotify.Subscribe(r => Cache.Remove(r.Key), CacheNotifyAction.Remove);
        }

        public void Remove()
        {
            CacheNotify.Publish(new MailServiceHelperCache() { Key = MailServiceHelper.CacheKey }, CacheNotifyAction.Remove);
        }
    }

    public class MailServiceHelper
    {
        public const string ConnectionStringFormat = "Server={0};Database={1};User ID={2};Password={3};Pooling=True;Character Set=utf8";
        public const string MailServiceDbId = "mailservice";
        public readonly string DefaultDatabase;
        public const string DefaultUser = "mail_admin";
        public const string DefaultPassword = "Isadmin123";
        public const string DefaultProtocol = "http";
        public const int DefaultPort = 8081;
        public const string DefaultVersion = "v1";

        internal const string CacheKey = "mailserverinfo";

        public UserManager UserManager { get; }
        public AuthContext AuthContext { get; }
        public IConfiguration Configuration { get; }
        public CoreBaseSettings CoreBaseSettings { get; }
        public MailServiceHelperStorage MailServiceHelperStorage { get; }
        public EFLoggerFactory LoggerFactory { get; }
        public MailDbContext MailDbContext { get; }
        public ICache Cache { get; }

        public MailServiceHelper(
            UserManager userManager,
            AuthContext authContext,
            IConfiguration configuration,
            CoreBaseSettings coreBaseSettings,
            MailServiceHelperStorage mailServiceHelperStorage,
            DbContextManager<MailDbContext> dbContext,
            EFLoggerFactory loggerFactory)
        {
            UserManager = userManager;
            AuthContext = authContext;
            Configuration = configuration;
            CoreBaseSettings = coreBaseSettings;
            MailServiceHelperStorage = mailServiceHelperStorage;
            LoggerFactory = loggerFactory;
            MailDbContext = dbContext.Get("webstudio");
            Cache = mailServiceHelperStorage.Cache;
            DefaultDatabase = GetDefaultDatabase();
        }

        private string GetDefaultDatabase()
        {
            var value = Configuration["mail:database-name"];
            return string.IsNullOrEmpty(value) ? "onlyoffice_mailserver" : value;
        }

        private void DemandPermission()
        {
            if (!CoreBaseSettings.Standalone)
                throw new NotSupportedException("Method for server edition only.");

            if (!UserManager.IsUserInGroup(AuthContext.CurrentAccount.ID, Constants.GroupAdmin.ID))
                throw new SecurityException();
        }


        public bool IsMailServerAvailable()
        {
            return _GetMailServerInfo() != null;
        }


        public MailServerInfo GetMailServerInfo()
        {
            DemandPermission();

            return _GetMailServerInfo();
        }

        private MailServerInfo _GetMailServerInfo()
        {
            var cachedData = Cache.Get<Tuple<MailServerInfo>>(CacheKey);

            if (cachedData != null)
                return cachedData.Item1;

            var value = MailDbContext.ServerServer.Select(r => r.ConnectionString).FirstOrDefault();

            cachedData =
                new Tuple<MailServerInfo>(string.IsNullOrEmpty(value)
                                                ? null
                                                : Newtonsoft.Json.JsonConvert.DeserializeObject<MailServerInfo>(value));

            Cache.Insert(CacheKey, cachedData, DateTime.UtcNow.Add(TimeSpan.FromDays(1)));

            return cachedData.Item1;
        }


        public string[] GetDataFromExternalDatabase(string dbid, string connectionString, string ip)
        {
            DemandPermission();

            var dbContextOptionsBuilder = new DbContextOptionsBuilder<MailDbContext>();
            var options = dbContextOptionsBuilder
                .UseMySql(connectionString)
                .UseLoggerFactory(LoggerFactory)
                .Options;

            using var mailDbContext = new MailDbContext(options);

            var token = mailDbContext.ApiKeys
                .Where(r => r.Id == 1)
                .Select(r => r.AccessToken)
                .FirstOrDefault();

            string hostname;

            if (IPAddress.TryParse(ip, out var ipAddress))
            {
                hostname = mailDbContext.GreyListingWhiteList
                    .Where(r => r.Source == "SenderIP:" + ip)
                    .Select(r => r.Comment)
                    .FirstOrDefault();
            }
            else
            {
                hostname = ip;
            }

            return new[] { token, hostname };
        }


        public void UpdateDataFromInternalDatabase(string hostname, MailServerInfo mailServer)
        {
            DemandPermission();

            using var transaction = MailDbContext.Database.BeginTransaction();

            var mailboxProvider = new MailboxProvider
            {
                Id = 0,
                Name = hostname
            };

            var pReq = MailDbContext.MailboxProvider.Add(mailboxProvider);
            MailDbContext.SaveChanges();
            mailboxProvider = pReq.Entity;

            var providerId = mailboxProvider.Id;

            var mailboxServer = new MailboxServer
            {
                Id = 0,
                IdProvider = providerId,
                Type = "smtp",
                Hostname = hostname,
                Port = 587,
                SocketType = "STARTTLS",
                UserName = "%EMAILADDRESS%",
                Authentication = "",
                IsUserData = false
            };

            var req = MailDbContext.MailboxServer.Add(mailboxServer);
            MailDbContext.SaveChanges();

            mailboxServer = req.Entity;

            var smtpServerId = mailboxServer.Id;

            mailboxServer = new MailboxServer
            {
                Id = 0,
                IdProvider = providerId,
                Type = "imap",
                Hostname = hostname,
                Port = 143,
                SocketType = "STARTTLS",
                UserName = "%EMAILADDRESS%",
                Authentication = "",
                IsUserData = false
            };

            req = MailDbContext.MailboxServer.Add(mailboxServer);
            MailDbContext.SaveChanges();

            mailboxServer = req.Entity;

            var imapServerId = mailboxServer.Id;

            var mailServerData = MailDbContext.ServerServer.FirstOrDefault();

            var connectionString = Newtonsoft.Json.JsonConvert.SerializeObject(mailServer);

            var server = new ServerServer
            {
                Id = 0,
                MxRecord = hostname,
                ConnectionString = connectionString,
                ServerType = 2,
                SmtpSettingsId = smtpServerId,
                ImapSettingsId = imapServerId
            };

            MailDbContext.ServerServer.Add(server);
            MailDbContext.SaveChanges();

            if (mailServerData != null)
            {
                server = MailDbContext.ServerServer.Where(r => r.Id == mailServerData.Id).FirstOrDefault();
                MailDbContext.ServerServer.Remove(server);
                MailDbContext.SaveChanges();

                providerId = MailDbContext.MailboxServer
                    .Where(r => r.Id == mailServerData.SmtpSettingsId)
                    .Select(r => r.IdProvider)
                    .FirstOrDefault();

                var providers = MailDbContext.MailboxProvider.Where(r => r.Id == providerId).ToList();
                MailDbContext.MailboxProvider.RemoveRange(providers);
                MailDbContext.SaveChanges();

                var servers = MailDbContext.MailboxServer
                    .Where(r => new[] { mailServerData.SmtpSettingsId, mailServerData.ImapSettingsId }.Any(a => a == r.Id))
                    .ToList();

                MailDbContext.MailboxServer.RemoveRange(servers);
                MailDbContext.SaveChanges();

                var mailboxId = MailDbContext.Mailbox
                    .Where(r => r.IdSmtpServer == mailServerData.SmtpSettingsId)
                    .Where(r => r.IdInServer == mailServerData.ImapSettingsId)
                    .ToArray();

                foreach (var m in mailboxId)
                {
                    m.IdSmtpServer = smtpServerId;
                    m.IdInServer = imapServerId;
                }
                MailDbContext.SaveChanges();
            }

            transaction.Commit();

            MailServiceHelperStorage.Remove();
        }
    }

    public class MailServerInfo
    {
        public string DbConnection { get; set; }
        public MailServerApiInfo Api { get; set; }
    }

    public class MailServerApiInfo
    {
        public string Protocol { get; set; }
        public string Server { get; set; }
        public int Port { get; set; }
        public string Version { get; set; }
        public string Token { get; set; }
    }
}