// (c) Copyright Ascensio System SIA 2010-2022
//
// This program is a free software product.
// You can redistribute it and/or modify it under the terms
// of the GNU Affero General Public License (AGPL) version 3 as published by the Free Software
// Foundation. In accordance with Section 7(a) of the GNU AGPL its Section 15 shall be amended
// to the effect that Ascensio System SIA expressly excludes the warranty of non-infringement of
// any third-party rights.
//
// This program is distributed WITHOUT ANY WARRANTY, without even the implied warranty
// of MERCHANTABILITY or FITNESS FOR A PARTICULAR  PURPOSE. For details, see
// the GNU AGPL at: http://www.gnu.org/licenses/agpl-3.0.html
//
// You can contact Ascensio System SIA at Lubanas st. 125a-25, Riga, Latvia, EU, LV-1021.
//
// The  interactive user interfaces in modified source and object code versions of the Program must
// display Appropriate Legal Notices, as required under Section 5 of the GNU AGPL version 3.
//
// Pursuant to Section 7(b) of the License you must retain the original Product logo when
// distributing the program. Pursuant to Section 7(e) we decline to grant you any rights under
// trademark law for use of our trademarks.
//
// All the Product's GUI elements, including illustrations and icon sets, as well as technical writing
// content are licensed under the terms of the Creative Commons Attribution-ShareAlike 4.0
// International. See the License terms at http://creativecommons.org/licenses/by-sa/4.0/legalcode

using Constants = ASC.Core.Users.Constants;

namespace ASC.Web.Core.Mail;

public class MailServiceHelperStorage
{
    private readonly ICacheNotify<MailServiceHelperCache> _cacheNotify;
    private readonly ICache _cache;

    public MailServiceHelperStorage(ICacheNotify<MailServiceHelperCache> cacheNotify, ICache cache)
    {
        _cache = cache;
        _cacheNotify = cacheNotify;
        _cacheNotify.Subscribe(r => _cache.Remove(r.Key), CacheNotifyAction.Remove);
    }

    public void Remove()
    {
        _cacheNotify.Publish(new MailServiceHelperCache() { Key = MailServiceHelper.CacheKey }, CacheNotifyAction.Remove);
    }
}

public class MailServiceHelper
{
    public readonly string ConnectionStringFormat;
    public const string MailServiceDbId = "mailservice";
    public readonly string DefaultDatabase;
    public const string DefaultUser = "mail_admin";
    public const string DefaultPassword = "Isadmin123";
    public const string DefaultProtocol = "http";
    public const int DefaultPort = 8081;
    public const string DefaultVersion = "v1";

    internal const string CacheKey = "mailserverinfo";

    private readonly UserManager _userManager;
    private readonly AuthContext _authContext;
    private readonly IConfiguration _configuration;
    private readonly CoreBaseSettings _coreBaseSettings;
    private readonly MailServiceHelperStorage _mailServiceHelperStorage;
    private readonly EFLoggerFactory _loggerFactory;
    private readonly Lazy<MailDbContext> _lazyMailDbContext;
    private readonly ICache _cache;

    private MailDbContext MailDbContext { get => _lazyMailDbContext.Value; }

    public MailServiceHelper(
        UserManager userManager,
        AuthContext authContext,
        IConfiguration configuration,
        CoreBaseSettings coreBaseSettings,
        MailServiceHelperStorage mailServiceHelperStorage,
        DbContextManager<MailDbContext> dbContext,
        EFLoggerFactory loggerFactory,
        ICache cache)
    {
        ConnectionStringFormat = GetConnectionStringFormat(configuration);
        _userManager = userManager;
        _authContext = authContext;
        _configuration = configuration;
        _coreBaseSettings = coreBaseSettings;
        _mailServiceHelperStorage = mailServiceHelperStorage;
        _loggerFactory = loggerFactory;
        _lazyMailDbContext = new Lazy<MailDbContext>(() => dbContext.Get("webstudio"));
        _cache = cache;
        DefaultDatabase = GetDefaultDatabase();
    }


    private string GetConnectionStringFormat(IConfiguration configuration)
    {
        var value = configuration["mailservice:connection-string-format"];
        return string.IsNullOrEmpty(value) ? "Server={0};Database={1};User ID={2};Password={3};Pooling=true;Character Set=utf8;AutoEnlist=false;SSL Mode=none;AllowPublicKeyRetrieval=true" : value;
    }


    private string GetDefaultDatabase()
    {
        var value = _configuration["mail:database-name"];
        return string.IsNullOrEmpty(value) ? "onlyoffice_mailserver" : value;
    }

    private void DemandPermission()
    {
        if (!_coreBaseSettings.Standalone)
        {
            throw new NotSupportedException("Method for server edition only.");
        }

        if (!_userManager.IsUserInGroup(_authContext.CurrentAccount.ID, Constants.GroupAdmin.ID))
        {
            throw new SecurityException();
        }
    }


    public bool IsMailServerAvailable()
    {
        return InnerGetMailServerInfo() != null;
    }


    public MailServerInfo GetMailServerInfo()
    {
        DemandPermission();

        return InnerGetMailServerInfo();
    }

    private MailServerInfo InnerGetMailServerInfo()
    {
        var cachedData = _cache.Get<Tuple<MailServerInfo>>(CacheKey);

        if (cachedData != null)
        {
            return cachedData.Item1;
        }

        var value = MailDbContext.ServerServer.Select(r => r.ConnectionString).FirstOrDefault();

        cachedData =
            new Tuple<MailServerInfo>(string.IsNullOrEmpty(value)
                                            ? null
                                            : JsonConvert.DeserializeObject<MailServerInfo>(value));

        _cache.Insert(CacheKey, cachedData, DateTime.UtcNow.Add(TimeSpan.FromDays(1)));

        return cachedData.Item1;
    }


    public string GetTokenFromExternalDatabase(string connectionString)
    {
        DemandPermission();

        var dbContextOptionsBuilder = new DbContextOptionsBuilder<MailDbContext>();
        var options = dbContextOptionsBuilder
            //.UseMySql(connectionString)
            .UseNpgsql(connectionString)
            .UseLoggerFactory(_loggerFactory)
            .Options;

        using var mailDbContext = new MailDbContext(options);

        var token = mailDbContext.ApiKeys
            .Where(r => r.Id == 1)
            .Select(r => r.AccessToken)
            .FirstOrDefault();
        return token;
    }

    public string GetHostnameFromExternalDatabase(string connectionString, string ip)
    {
        DemandPermission();

        var dbContextOptionsBuilder = new DbContextOptionsBuilder<MailDbContext>();
        var options = dbContextOptionsBuilder
            //.UseMySql(connectionString)
            .UseNpgsql(connectionString)
            .UseLoggerFactory(_loggerFactory)
            .Options;

        using var mailDbContext = new MailDbContext(options);

        if (!IPAddress.TryParse(ip, out var ipAddress))
        {
            return ip;
        }

        var hostname = mailDbContext.GreyListingWhiteList
            .Where(r => r.Source == "SenderIP:" + ip)
            .Select(r => r.Comment)
            .FirstOrDefault();

        return hostname;
    }


    public void UpdateDataFromInternalDatabase(string hostname, MailServerInfo mailServer)
    {
        DemandPermission();

        var strategy = MailDbContext.Database.CreateExecutionStrategy();

        strategy.Execute(() =>
        {
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

            var connectionString = JsonConvert.SerializeObject(mailServer);

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
        });

        _mailServiceHelperStorage.Remove();
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
