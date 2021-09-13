using System;
using System.Collections.Generic;

using ASC.Common;
using ASC.Common.Utils;
using ASC.Mail.Models;
using ASC.Web.Studio.Core;

using Microsoft.Extensions.Configuration;

using static System.TimeSpan;

namespace ASC.Mail.Configuration
{
    [Singletone]
    public class MailSettings
    {
        public bool NeedProxyHttp { get; set; }

        public MailGarbageEraserConfig Cleaner { get; set; }
        public DefinesConfig Defines { get; set; }
        public AggregatorConfig Aggregator { get; set; }
        public ImapSyncConfig ImapSync { get; set; }
        public WatchdogConfig Watchdog { get; set; }

        public Dictionary<string, int> ImapFlags { get; set; }
        public string[] SkipImapFlags { get; set; }
        public Dictionary<string, Dictionary<string, MailBoxData.MailboxInfo>> SpecialDomainFolders { get; set; }
        public Dictionary<string, int> DefaultFolders { get; set; }

        public static MailSettings Current;
        public MailSettings()
        {
            Current = this;
        }

        public MailSettings(ConfigurationExtension configuration, IConfiguration config)
        {
            var c = configuration.GetSetting<MailSettings>("mail");

            Defines = new DefinesConfig(c.Defines);
            Cleaner = new MailGarbageEraserConfig(c.Cleaner, c.Defines.DefaultApiSchema);
            Aggregator = new AggregatorConfig(c.Aggregator, config);
            Watchdog = new WatchdogConfig(c.Watchdog);
            ImapSync = new ImapSyncConfig(c.ImapSync);

            NeedProxyHttp = SetupInfo.IsVisibleSettings("ProxyHttpContent");

            ImapFlags = new Dictionary<string, int>();
            SkipImapFlags = new string[] { };
            SpecialDomainFolders = new Dictionary<string, Dictionary<string, MailBoxData.MailboxInfo>>();
            DefaultFolders = new Dictionary<string, int>();
        }

        public class MailGarbageEraserConfig
        {
            public string HttpContextScheme { get; set; }
            public int? GarbageOverdueDays { get; set; }
            public int? MaxTasksAtOnce { get; set; }
            public int? MaxFilesToRemoveAtOnce { get; set; }
            public int? TenantCacheDays { get; set; }
            public int? TenantOverdueDays { get; set; }
            public int? TimerWaitMinutes { get; set; }
            public bool DeleteDiscStorageFolders { get; set; }
            public MailGarbageEraserConfig() { }
            public MailGarbageEraserConfig(MailGarbageEraserConfig config, string defaultApiSchema)
            {
                HttpContextScheme = defaultApiSchema == Uri.UriSchemeHttps ? Uri.UriSchemeHttps : Uri.UriSchemeHttp;
                GarbageOverdueDays = config.GarbageOverdueDays ?? 30;
                MaxTasksAtOnce = config.MaxTasksAtOnce ?? 1;
                MaxFilesToRemoveAtOnce = config.MaxFilesToRemoveAtOnce ?? 100;
                TenantCacheDays = config.TenantCacheDays ?? 1;
                TenantOverdueDays = config.TenantOverdueDays ?? 30;
                TimerWaitMinutes = config.TimerWaitMinutes ?? 60;
                DeleteDiscStorageFolders = config.DeleteDiscStorageFolders;
            }
        }
        public class DefinesConfig
        {
            public TimeSpan AuthErrorWarningTimeout { get; set; }
            public TimeSpan AuthErrorDisableMailboxTimeout { get; set; }
            public TimeSpan CheckTimerInterval { get; set; }
            public TimeSpan ActiveInterval { get; set; }
            public List<string> WorkOnUsersOnlyList { get; set; }
            public string WorkOnUsersOnly { get; set; }
            public TimeSpan OverdueAccountDelay { get; set; }
            public TimeSpan QuotaEndedDelay { get; set; }
            public TimeSpan TenantCachingPeriod { get; set; }
            public TimeSpan QueueLifetime { get; set; }
            public bool SaveOriginalMessage { get; set; }
            public bool SslCertificatesErrorsPermit { get; set; }
            public string DefaultApiSchema { get; set; }
            public int DefaultServerLoginDelay { get; set; }
            public int AutoreplyDaysInterval { get; set; }
            public string DefaultServerLoginDelayStr { get; set; }
            public string WebHub { get; set; }
            public string WebHubInternal { get; set; }
            public bool IsSignalRAvailable { get; set; }
            public string MailDaemonEmail { get; set; }
            public int? RecalculateFoldersTimeout { get; set; }
            public int? RemoveDomainTimeout { get; set; }
            public int? RemoveMailboxTimeout { get; set; }
            public int? ServerDomainMailboxPerUserLimit { get; set; }
            public string ServerDnsSpfRecordValue { get; set; }
            public int? ServerDnsMxRecordPriority { get; set; }
            public string ServerDnsDkimSelector { get; set; }
            public string ServerDnsDomainCheckPrefix { get; set; }
            public int ServerDnsDefaultTtl { get; set; }
            public int? MaximumMessageBodySize { get; set; }
            public bool IsAttachmentsGroupOperationsAvailable { get; set; }

            private int _defaultTimeout = 99999;
            public DefinesConfig() { }
            public DefinesConfig(DefinesConfig config)
            {
                DefaultServerLoginDelayStr = config.DefaultServerLoginDelay.ToString();
                WebHub = config.WebHub ?? "";
                IsSignalRAvailable = !string.IsNullOrEmpty(WebHub);
                MailDaemonEmail = config.MailDaemonEmail ?? "mail-daemon@onlyoffice.com";
                RecalculateFoldersTimeout = config.RecalculateFoldersTimeout ?? _defaultTimeout;
                RemoveDomainTimeout = config.RemoveDomainTimeout ?? _defaultTimeout;
                RemoveMailboxTimeout = config.RemoveMailboxTimeout ?? _defaultTimeout;
                ServerDomainMailboxPerUserLimit = config.ServerDomainMailboxPerUserLimit ?? 2;
                ServerDnsSpfRecordValue = config.ServerDnsSpfRecordValue ?? "v=spf1 +mx ~all";
                ServerDnsMxRecordPriority = config.ServerDnsMxRecordPriority ?? 0;
                ServerDnsDkimSelector = config.ServerDnsDkimSelector ?? "dkim";
                ServerDnsDomainCheckPrefix = config.ServerDnsDomainCheckPrefix ?? "onlyoffice-domain";
                ServerDnsDefaultTtl = config.ServerDnsDefaultTtl == default(int) ? 300 : config.ServerDnsDefaultTtl;
                MaximumMessageBodySize = config.MaximumMessageBodySize ?? 524288;
                IsAttachmentsGroupOperationsAvailable = config.IsAttachmentsGroupOperationsAvailable;
                WebHubInternal = config.WebHubInternal == string.Empty ? "http://localhost:9899" : config.WebHubInternal;
                AuthErrorWarningTimeout = config.AuthErrorWarningTimeout == Zero ? FromHours(1) : config.AuthErrorWarningTimeout;
                AuthErrorDisableMailboxTimeout = config.AuthErrorDisableMailboxTimeout == Zero ? FromDays(3) : config.AuthErrorDisableMailboxTimeout;
                SaveOriginalMessage = config.SaveOriginalMessage;
                SslCertificatesErrorsPermit = config.SslCertificatesErrorsPermit;
                DefaultApiSchema = config.DefaultApiSchema == "https" ? config.DefaultApiSchema : Uri.UriSchemeHttp;
                DefaultServerLoginDelay = config.DefaultServerLoginDelay == default(int) ? 30 : config.DefaultServerLoginDelay;
                AutoreplyDaysInterval = config.AutoreplyDaysInterval == default(int) ? 1 : config.AutoreplyDaysInterval;
                OverdueAccountDelay = config.OverdueAccountDelay == Zero ? FromSeconds(600) : config.OverdueAccountDelay;
                QuotaEndedDelay = config.QuotaEndedDelay == Zero ? FromSeconds(600) : config.QuotaEndedDelay;
                TenantCachingPeriod = config.TenantCachingPeriod == Zero ? FromSeconds(86400) : config.TenantCachingPeriod;
                QueueLifetime = config.QueueLifetime == Zero ? FromSeconds(30) : config.QueueLifetime;
                CheckTimerInterval = config.CheckTimerInterval == Zero ? FromSeconds(10) : config.CheckTimerInterval;
                ActiveInterval = config.ActiveInterval == Zero ? FromSeconds(90) : config.ActiveInterval;
                Guid userId;
                WorkOnUsersOnlyList = Guid.TryParse(config.WorkOnUsersOnly, out userId)
                    ? new List<string> { userId.ToString() }
                    : new List<string>();
            }
        }
        public class AggregatorConfig
        {
            public string ApiPrefix { get; set; }
            public string ApiVirtualDirPrefix { get; set; }
            public string ApiPort { get; set; }
            public string ApiHost { get; set; }
            public bool UseDump { get; set; }
            public enum AggregateModeType
            {
                All,
                External,
                Internal
            };
            public AggregateModeType AggregateMode { get; set; }
            public string AggregateModeValue { get; set; }
            public bool EnableSignalr { get; set; }
            public int ChunkOfPop3Uidl { get; set; }
            public int MaxMessagesPerSession { get; set; }
            public int MaxTasksAtOnce { get; set; }
            public double InactiveMailboxesRatio { get; set; }
            public int? TenantOverdueDays { get; set; }
            public long? TenantMinQuotaBalance { get; set; }
            public TimeSpan TaskLifetime { get; set; }
            public TimeSpan TaskCheckState { get; set; }
            public int TcpTimeout { get; set; }
            public string ProtocolLogPath { get; set; }
            public bool CollectStatistics { get; set; }
            public bool ShowMailEngineLogs { get; set; }
            public int? QuotaRest { get; set; }
            public uint? MaxMessageSizeLimit { get; set; }
            public AggregatorConfig() { }
            public AggregatorConfig(AggregatorConfig config, IConfiguration configuration)
            {
                ApiPort = config.ApiPort;
                ApiHost = config.ApiHost;
                ApiPrefix = (configuration["web:api"] ?? "").Trim('~', '/');
                ApiVirtualDirPrefix = (config.ApiVirtualDirPrefix ?? "").Trim('/');
                UseDump = config.UseDump;
                TaskLifetime = config.TaskLifetime == Zero ? FromSeconds(300) : config.TaskLifetime;
                TaskCheckState = config.TaskCheckState == Zero ? FromSeconds(30) : config.TaskCheckState;
                AggregateMode = config.AggregateModeValue == string.Empty
                    ? AggregateModeType.All
                    : config.AggregateModeValue == "external" ? AggregateModeType.External
                    : config.AggregateModeValue == "internal" ? AggregateModeType.Internal
                    : AggregateModeType.All;
                QuotaRest = config.QuotaRest ?? 26214400;
                EnableSignalr = config.EnableSignalr;
                ShowMailEngineLogs = config.ShowMailEngineLogs;
                ChunkOfPop3Uidl = config.ChunkOfPop3Uidl < 1 || config.ChunkOfPop3Uidl > 100 || config.ChunkOfPop3Uidl == 0
                    ? 100
                    : config.ChunkOfPop3Uidl;
                MaxMessagesPerSession = config.MaxMessagesPerSession < 1
                    ? -1
                    : config.MaxMessagesPerSession == 0
                    ? 10
                    : config.MaxMessagesPerSession;
                MaxTasksAtOnce = config.MaxTasksAtOnce < 1
                    ? 1
                    : config.MaxTasksAtOnce == 0
                    ? 10
                    : config.MaxTasksAtOnce;
                InactiveMailboxesRatio = config.InactiveMailboxesRatio < 0
                    ? 0
                    : config.InactiveMailboxesRatio > 100
                    ? 100
                    : config.InactiveMailboxesRatio == 0
                    ? 25
                    : config.InactiveMailboxesRatio;
                TcpTimeout = config.TcpTimeout == default(int) ? 30000 : config.TcpTimeout;
                ProtocolLogPath = config.ProtocolLogPath ?? "";
                CollectStatistics = config.CollectStatistics;
                TenantOverdueDays = config.TenantOverdueDays ?? 10;
                TenantMinQuotaBalance = config.TenantMinQuotaBalance ?? 26214400;
                MaxMessageSizeLimit = config.MaxMessageSizeLimit ?? 67108864;
            }
        }
        public class WatchdogConfig
        {
            public int? TasksTimeoutInMinutes { get; set; }
            public int? TimerIntervalInMinutes { get; set; }
            public WatchdogConfig() { }
            public WatchdogConfig(WatchdogConfig config)
            {
                TasksTimeoutInMinutes = config.TasksTimeoutInMinutes ?? 6;
                TimerIntervalInMinutes = config.TimerIntervalInMinutes ?? 6;
            }
        }

        public class ImapSyncConfig
        {
            public string RedisConnectionString { get; set; }

            public int? AliveTimeInMinutes { get; set; }

            public ImapSyncMode Mode { get; set; }

            public enum ImapSyncMode
            {
                SingleImapClientPerMailBox,
                OneImapClientPerActivFolderInMailBox,
                SingleImapClientPerMailBoxNoFolderChange
            }

            public ImapSyncConfig()
            {

            }
            public ImapSyncConfig(ImapSyncConfig imapSync)
            {
                RedisConnectionString = string.IsNullOrEmpty(imapSync.RedisConnectionString) ? "localhost:6379" : imapSync.RedisConnectionString;

                AliveTimeInMinutes = imapSync.AliveTimeInMinutes ?? 20;
            }

        }
    }


}
