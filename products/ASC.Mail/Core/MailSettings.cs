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
        #region From configs

        #region MailGarbageEraserConfig

        public string CleanerHttpContextScheme { get; set; }
        public int? CleanerGarbageOverdueDays { get; set; }
        public int? CleanerMaxTasksAtOnce { get; set; }
        public int? CleanerMaxFilesToRemoveAtOnce { get; set; }
        public int? CleanerTenantCacheDays { get; set; }
        public int? CleanerTenantOverdueDays { get; set; }
        public int? CleanerTimerWaitMinutes { get; set; }
        #endregion

        #region from defines

        public TimeSpan AuthErrorWarningTimeout { get; set; }

        public TimeSpan AuthErrorDisableMailboxTimeout { get; set; }

        public TimeSpan CheckTimerInterval { get; set; }

        public TimeSpan ActiveInterval { get; set; }

        public List<string> WorkOnUsersOnlyList { get; set; }

        public string WorkOnUsersOnly { get; set; }

        public AggregateModeType AggregateMode { get; set; }

        public string AggregateModeValue { get; set; }

        public TimeSpan OverdueAccountDelay { get; set; }

        public TimeSpan QuotaEndedDelay { get; set; }

        public TimeSpan TenantCachingPeriod { get; set; }

        public TimeSpan QueueLifetime { get; set; }

        public bool SaveOriginalMessage { get; set; }

        public bool SslCertificatesErrorsPermit { get; set; }

        public string DefaultApiSchema { get; set; }

        public int DefaultServerLoginDelay { get; set; }

        public int AutoreplyDaysInterval { get; set; }

        public bool NeedProxyHttp { get; set; }

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

        #endregion

        #region from taskConfig

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
        #endregion

        #endregion

        #region Inner

        public Dictionary<string, int> ImapFlags { get; set; }
        public string[] SkipImapFlags { get; set; }
        public Dictionary<string, Dictionary<string, MailBoxData.MailboxInfo>> SpecialDomainFolders { get; set; }
        public Dictionary<string, int> DefaultFolders { get; set; }

        #endregion

        private int _defaultTimeout = 99999;

        public static MailSettings Current;

        public MailSettings()
        {
            Current = this;
        }

        public MailSettings(ConfigurationExtension configuration, IConfiguration config)
        {
            var c = configuration.GetSetting<MailSettings>("mail");

            ApiPort = c.ApiPort;

            ApiHost = c.ApiHost;

            ApiPrefix = (config["web:api"] ?? "").Trim('~', '/');

            ApiVirtualDirPrefix = (c.ApiVirtualDirPrefix ?? "").Trim('/');

            UseDump = c.UseDump;

            CheckTimerInterval = c.CheckTimerInterval == Zero ? FromSeconds(10) : c.CheckTimerInterval;

            ActiveInterval = c.ActiveInterval == Zero ? FromSeconds(90) : c.ActiveInterval;

            TaskLifetime = c.TaskLifetime == Zero ? FromSeconds(300) : c.TaskLifetime;

            TaskCheckState = c.TaskCheckState == Zero ? FromSeconds(30) : c.TaskCheckState;

            OverdueAccountDelay = c.OverdueAccountDelay == Zero ? FromSeconds(600) : c.OverdueAccountDelay;

            QuotaEndedDelay = c.QuotaEndedDelay == Zero ? FromSeconds(600) : c.QuotaEndedDelay;

            TenantCachingPeriod = c.TenantCachingPeriod == Zero ? FromSeconds(86400) : c.TenantCachingPeriod;

            QueueLifetime = c.QueueLifetime == Zero ? FromSeconds(30) : c.QueueLifetime;

            Guid userId;
            WorkOnUsersOnlyList = Guid.TryParse(c.WorkOnUsersOnly, out userId)
                ? new List<string> { userId.ToString() }
                : new List<string>();

            AggregateMode = c.AggregateModeValue == string.Empty
                ? AggregateModeType.All
                : c.AggregateModeValue == "external" ? AggregateModeType.External
                : c.AggregateModeValue == "internal" ? AggregateModeType.Internal
                : AggregateModeType.All;

            QuotaRest = c.QuotaRest ?? 26214400;

            EnableSignalr = c.EnableSignalr;

            ShowMailEngineLogs = c.ShowMailEngineLogs;

            ChunkOfPop3Uidl = c.ChunkOfPop3Uidl < 1 || c.ChunkOfPop3Uidl > 100 || c.ChunkOfPop3Uidl == 0
                ? 100
                : c.ChunkOfPop3Uidl;

            MaxMessagesPerSession = c.MaxMessagesPerSession < 1
                ? -1
                : c.MaxMessagesPerSession == 0
                ? 10
                : c.MaxMessagesPerSession;

            MaxTasksAtOnce = c.MaxTasksAtOnce < 1 //Вытянуть конфиг!
                ? 1
                : c.MaxTasksAtOnce == 0
                ? 10
                : c.MaxTasksAtOnce;


            InactiveMailboxesRatio = c.InactiveMailboxesRatio < 0
                ? 0
                : c.InactiveMailboxesRatio > 100
                ? 100
                : c.InactiveMailboxesRatio == 0
                ? 25
                : c.InactiveMailboxesRatio;

            AuthErrorWarningTimeout = c.AuthErrorWarningTimeout == Zero ? FromHours(1) : c.AuthErrorWarningTimeout;

            AuthErrorDisableMailboxTimeout = c.AuthErrorDisableMailboxTimeout == Zero ? FromDays(3) : c.AuthErrorDisableMailboxTimeout;

            TenantOverdueDays = c.TenantOverdueDays ?? 10;

            TenantMinQuotaBalance = c.TenantMinQuotaBalance ?? 26214400;

            SaveOriginalMessage = c.SaveOriginalMessage;

            SslCertificatesErrorsPermit = c.SslCertificatesErrorsPermit;

            TcpTimeout = c.TcpTimeout == default(int) ? 30000 : c.TcpTimeout;

            ProtocolLogPath = c.ProtocolLogPath ?? "";

            CollectStatistics = c.CollectStatistics;

            DefaultApiSchema = c.DefaultApiSchema == "https" ? c.DefaultApiSchema : Uri.UriSchemeHttp;

            DefaultServerLoginDelay = c.DefaultServerLoginDelay == default(int) ? 30 : c.DefaultServerLoginDelay;

            AutoreplyDaysInterval = c.AutoreplyDaysInterval == default(int) ? 1 : c.AutoreplyDaysInterval;

            NeedProxyHttp = SetupInfo.IsVisibleSettings("ProxyHttpContent");

            DefaultServerLoginDelayStr = DefaultServerLoginDelay.ToString();

            WebHub = c.WebHub ?? "";

            IsSignalRAvailable = !string.IsNullOrEmpty(WebHub);

            MailDaemonEmail = c.MailDaemonEmail ?? "mail-daemon@onlyoffice.com";

            RecalculateFoldersTimeout = c.RecalculateFoldersTimeout ?? _defaultTimeout;

            RemoveDomainTimeout = c.RemoveDomainTimeout ?? _defaultTimeout;

            RemoveMailboxTimeout = c.RemoveMailboxTimeout ?? _defaultTimeout;

            ServerDomainMailboxPerUserLimit = c.ServerDomainMailboxPerUserLimit ?? 2;

            ServerDnsSpfRecordValue = c.ServerDnsSpfRecordValue ?? "v=spf1 +mx ~all";

            ServerDnsMxRecordPriority = c.ServerDnsMxRecordPriority ?? 0;

            ServerDnsDkimSelector = c.ServerDnsDkimSelector ?? "dkim";

            ServerDnsDomainCheckPrefix = c.ServerDnsDomainCheckPrefix ?? "onlyoffice-domain";

            ServerDnsDefaultTtl = c.ServerDnsDefaultTtl == default(int) ? 300 : c.ServerDnsDefaultTtl;

            MaximumMessageBodySize = c.MaximumMessageBodySize ?? 524288;

            IsAttachmentsGroupOperationsAvailable = c.IsAttachmentsGroupOperationsAvailable;

            CleanerHttpContextScheme = DefaultApiSchema == Uri.UriSchemeHttps ? Uri.UriSchemeHttps : Uri.UriSchemeHttp;

            CleanerGarbageOverdueDays = c.CleanerGarbageOverdueDays ?? 30;

            CleanerMaxTasksAtOnce = c.CleanerMaxTasksAtOnce ?? 1;

            CleanerMaxFilesToRemoveAtOnce = c.CleanerMaxFilesToRemoveAtOnce ?? 100;

            CleanerTenantCacheDays = c.CleanerTenantCacheDays ?? 1;

            CleanerTenantOverdueDays = c.CleanerTenantOverdueDays ?? 30;

            CleanerTimerWaitMinutes = c.CleanerTimerWaitMinutes ?? 60;

            WebHubInternal = c.WebHubInternal == string.Empty ? "http://localhost:9899" : c.WebHubInternal;

            //Set values from MailQueueItemSettings later
            ImapFlags = new Dictionary<string, int>();
            SkipImapFlags = new string[] { };
            SpecialDomainFolders = new Dictionary<string, Dictionary<string, MailBoxData.MailboxInfo>>();
            DefaultFolders = new Dictionary<string, int>();
        }

    }
}
