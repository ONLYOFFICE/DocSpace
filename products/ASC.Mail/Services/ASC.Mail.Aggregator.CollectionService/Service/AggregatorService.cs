using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading;
using System.Threading.Tasks;

using ASC.Common;
using ASC.Common.Logging;
using ASC.Common.Utils;
using ASC.Core;
using ASC.Data.Storage;
using ASC.Mail.Aggregator.CollectionService.Console;
using ASC.Mail.Aggregator.CollectionService.Queue;
using ASC.Mail.Aggregator.CollectionService.Queue.Data;
using ASC.Mail.Clients;
using ASC.Mail.Configuration;
using ASC.Mail.Core;
using ASC.Mail.Core.Dao.Expressions.Mailbox;
using ASC.Mail.Core.Engine;
using ASC.Mail.Core.Entities;
using ASC.Mail.Models;
using ASC.Mail.Storage;
using ASC.Mail.Utils;

using MailKit.Net.Imap;
using MailKit.Net.Pop3;
using MailKit.Security;

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

using MimeKit;

using NLog;

using ILog = ASC.Common.Logging.ILog;

namespace ASC.Mail.Aggregator.CollectionService.Service
{
    [Singletone(Additional = typeof(AggregatorServiceExtension))]
    public class AggregatorService
    {
        public const string ASC_MAIL_COLLECTION_SERVICE_NAME = "ASC Mail Collection Service";
        private const string S_FAIL = "error";
        private const string S_OK = "success";
        private const string PROCESS_MESSAGE = "process message";
        private const string PROCESS_MAILBOX = "process mailbox";
        private const string CONNECT_MAILBOX = "connect mailbox";
        private const int SIGNALR_WAIT_SECONDS = 30;

        private readonly TimeSpan TsTaskStateCheckInterval;
        private bool IsFirstTime = true;
        readonly TaskFactory TaskFactory;
        private readonly TimeSpan TaskSecondsLifetime;
        private SignalrWorker SignalrWorker;
        private Timer WorkTimer;
        private readonly CancellationTokenSource CancelTokenSource;

        public Dictionary<string, int> ImapFlags { get; set; }
        public string[] SkipImapFlags { get; set; }
        public Dictionary<string, Dictionary<string, MailBoxData.MailboxInfo>> SpecialDomainFolders { get; set; }
        public Dictionary<string, int> DefaultFolders { get; set; }

        private ILog Log { get; }
        private ILog LogStat { get; }
        private IOptionsMonitor<ILog> LogOptions { get; }
        private MailSettings MailSettings { get; }
        private ConsoleParameters ConsoleParameters { get; }
        private IServiceProvider ServiceProvider { get; }
        private AggregatorServiceScope Scope { get; }

        private DIHelper DIHelper { get; }

        public AggregatorService(
            ConsoleParser consoleParser,
            IOptionsMonitor<ILog> optionsMonitor,
            ConsoleParameters options,
            MailSettings mailSettings,
            IServiceProvider serviceProvider,
            IConfiguration configuration,
            ConfigurationExtension configurationExtension)
        {
            DIHelper = new DIHelper();

            ServiceProvider = serviceProvider;
            ConsoleParameters = consoleParser.GetParsedParameters();

            ConfigureLogNLog(configuration, configurationExtension);

            Log = optionsMonitor.Get("ASC.Mail.MainThread");
            LogStat = optionsMonitor.Get("ASC.Mail.Stat");
            LogOptions = optionsMonitor;
            MailSettings = mailSettings;

            Scope = ServiceProvider.CreateScope().ServiceProvider.GetService<AggregatorServiceScope>();

            var scope = ServiceProvider.CreateScope().ServiceProvider.GetService<AggregatorServiceScope>();
            var (mailQueueItemSettings, signalrWorker) = scope;

            MailSettings.DefaultFolders = scope.MailQueueItemSettings.DefaultFolders;
            MailSettings.ImapFlags = scope.MailQueueItemSettings.ImapFlags;
            MailSettings.SkipImapFlags = scope.MailQueueItemSettings.SkipImapFlags;
            MailSettings.SpecialDomainFolders = scope.MailQueueItemSettings.SpecialDomainFolders;

            TsTaskStateCheckInterval = MailSettings.TaskCheckState;

            if (ConsoleParameters.OnlyUsers != null)
                MailSettings.WorkOnUsersOnlyList.AddRange(ConsoleParameters.OnlyUsers.ToList());

            if (ConsoleParameters.NoMessagesLimit)
                MailSettings.MaxMessagesPerSession = -1;

            TaskSecondsLifetime = MailSettings.TaskLifetime;

            TaskFactory = new TaskFactory();

            CancelTokenSource = new CancellationTokenSource();

            if (MailSettings.EnableSignalr)
                SignalrWorker = scope.SignalrWorker;

            WorkTimer = new Timer(WorkTimerElapsed, CancelTokenSource.Token, Timeout.Infinite, Timeout.Infinite);

            Filters = new ConcurrentDictionary<string, List<MailSieveFilterData>>();

            Log.Info("Service is ready.");
        }

        #region methods

        private void ConfigureLogNLog(IConfiguration configuration, ConfigurationExtension configurationExtension)
        {
            var fileName = CrossPlatform.PathCombine(configuration["pathToNlogConf"], "nlog.config");

            LogManager.Configuration = new NLog.Config.XmlLoggingConfiguration(fileName);
            LogManager.ThrowConfigExceptions = false;

            var settings = configurationExtension.GetSetting<NLogSettings>("log");
            if (!string.IsNullOrEmpty(settings.Name))
            {
                LogManager.Configuration.Variables["name"] = settings.Name;
            }

            if (!string.IsNullOrEmpty(settings.Dir))
            {
                LogManager.Configuration.Variables["dir"] = settings.Dir.TrimEnd('/').TrimEnd('\\') + Path.DirectorySeparatorChar;
            }

            NLog.Targets.Target.Register<SelfCleaningTarget>("SelfCleaning");
        }

        public ConcurrentDictionary<string, List<MailSieveFilterData>> Filters { get; set; }

        internal void StartTimer(bool immediately = false)
        {
            if (WorkTimer == null)
                return;

            Log.DebugFormat("Setup Work timer to {0} seconds", MailSettings.CheckTimerInterval.TotalSeconds);

            if (immediately)
            {
                WorkTimer.Change(0, Timeout.Infinite);
            }
            else
            {
                WorkTimer.Change(MailSettings.CheckTimerInterval, MailSettings.CheckTimerInterval);
            }
        }

        private void StopTimer()
        {
            if (WorkTimer == null)
                return;

            Log.Debug("Setup Work timer to Timeout.Infinite");
            WorkTimer.Change(Timeout.Infinite, Timeout.Infinite);
        }

        internal void StopService()
        {
            if (CancelTokenSource != null)
                CancelTokenSource.Cancel();

            if (Scope.QueueManager != null)
            {
                Scope.QueueManager.CancelHandler.WaitOne();
            }

            StopTimer();
            DisposeWorkers();
        }

        private void WorkTimerElapsed(object state)
        {
            Log.Debug("Timer -> WorkTimer_Elapsed");

            var cancelToken = state as CancellationToken? ?? new CancellationToken();

            try
            {
                if (IsFirstTime)
                {
                    Scope.QueueManager.LoadMailboxesFromDump();

                    if (Scope.QueueManager.ProcessingCount > 0)
                    {
                        Log.InfoFormat("Found {0} tasks to release", Scope.QueueManager.ProcessingCount);

                        Scope.QueueManager.ReleaseAllProcessingMailboxes();
                    }

                    Scope.QueueManager.LoadTenantsFromDump();

                    IsFirstTime = false;
                }

                if (cancelToken.IsCancellationRequested)
                {
                    Log.Debug("Timer -> WorkTimer_Elapsed: IsCancellationRequested. Quit.");
                    return;
                }

                StopTimer();

                var tasks = CreateTasks(MailSettings.MaxTasksAtOnce, cancelToken);

                // ***Add a loop to process the tasks one at a time until none remain. 
                while (tasks.Any())
                {
                    // Identify the first task that completes.
                    var indexTask = Task.WaitAny(tasks.Select(t => t.Task).ToArray(), (int)TsTaskStateCheckInterval.TotalMilliseconds, cancelToken);
                    if (indexTask > -1)
                    {
                        // ***Remove the selected task from the list so that you don't 
                        // process it more than once.
                        var outTask = tasks[indexTask];

                        FreeTask(outTask, tasks);
                    }
                    else
                    {
                        Log.InfoFormat("Task.WaitAny timeout. Tasks count = {0}\r\nTasks:\r\n{1}", tasks.Count,
                            string.Join("\r\n", tasks.Select(t =>
                                        $"Id: {t.Task.Id} Status: {t.Task.Status}, MailboxId: {t.Mailbox.MailBoxId} Address: '{t.Mailbox.EMail}'")));
                    }

                    var tasks2Free =
                        tasks.Where(
                            t =>
                            t.Task.Status == TaskStatus.Canceled || t.Task.Status == TaskStatus.Faulted ||
                            t.Task.Status == TaskStatus.RanToCompletion).ToList();

                    if (tasks2Free.Any())
                    {
                        Log.InfoFormat("Need free next tasks = {0}: ({1})", tasks2Free.Count,
                                  string.Join(",",
                                              tasks2Free.Select(t => t.Task.Id.ToString(CultureInfo.InvariantCulture))));

                        tasks2Free.ForEach(task => FreeTask(task, tasks));
                    }

                    var difference = MailSettings.MaxTasksAtOnce - tasks.Count;

                    if (difference <= 0) continue;

                    var newTasks = CreateTasks(difference, cancelToken);

                    tasks.AddRange(newTasks);

                    Log.InfoFormat("Total tasks count = {0} ({1}).", tasks.Count,
                              string.Join(",", tasks.Select(t => t.Task.Id)));
                }

                Log.Info("All mailboxes were processed. Go back to timer.");
            }
            catch (Exception ex)
            {
                if (ex is AggregateException)
                {
                    ex = ((AggregateException)ex).GetBaseException();
                }

                if (ex is TaskCanceledException || ex is OperationCanceledException)
                {
                    Log.Info("Execution was canceled.");

                    Scope.QueueManager.ReleaseAllProcessingMailboxes();

                    Scope.QueueManager.CancelHandler.Set();

                    return;
                }

                Log.ErrorFormat("Timer -> WorkTimer_Elapsed. Exception:\r\n{0}\r\n", ex.ToString());

                if (Scope.QueueManager.ProcessingCount != 0)
                {
                    Scope.QueueManager.ReleaseAllProcessingMailboxes();
                }
            }

            Scope.QueueManager.CancelHandler.Set();

            StartTimer();
        }

        private void DisposeWorkers()
        {
            if (WorkTimer != null)
            {
                WorkTimer.Dispose();
                WorkTimer = null;
            }

            if (Scope.QueueManager != null)
                Scope.QueueManager.Dispose();

            if (SignalrWorker != null)
                SignalrWorker.Dispose();
        }

        private void NotifySignalrIfNeed(MailBoxData mailbox, ILog log)
        {
            if (!MailSettings.EnableSignalr)
            {
                log.Debug("Skip NotifySignalrIfNeed: EnableSignalr == false");

                return;
            }

            var now = DateTime.UtcNow;

            try
            {
                if (mailbox.LastSignalrNotify.HasValue &&
                    !((now - mailbox.LastSignalrNotify.Value).TotalSeconds > SIGNALR_WAIT_SECONDS))
                {
                    mailbox.LastSignalrNotifySkipped = true;

                    log.InfoFormat(
                        "Skip NotifySignalrIfNeed: last notification has occurend less then {0} seconds ago",
                        SIGNALR_WAIT_SECONDS);

                    return;
                }

                if (SignalrWorker == null)
                    throw new NullReferenceException("SignalrWorker");

                SignalrWorker.AddMailbox(mailbox);

                log.InfoFormat("NotifySignalrIfNeed(UserId = {0} TenantId = {1}) has been succeeded",
                    mailbox.UserId, mailbox.TenantId);
            }
            catch (Exception ex)
            {
                log.ErrorFormat("NotifySignalrIfNeed(UserId = {0} TenantId = {1}) Exception: {2}", mailbox.UserId,
                    mailbox.TenantId, ex.ToString());
            }

            mailbox.LastSignalrNotify = now;
            mailbox.LastSignalrNotifySkipped = false;
        }

        public List<TaskData> CreateTasks(int needCount, CancellationToken cancelToken)
        {
            Log.InfoFormat("CreateTasks(need {0} tasks).", needCount);

            var mailboxes = Scope.QueueManager.GetLockedMailboxes(needCount).ToList();

            var tasks = new List<TaskData>();

            foreach (var mailbox in mailboxes)
            {
                var timeoutCancel = new CancellationTokenSource(TaskSecondsLifetime);

                var commonCancelToken =
                    CancellationTokenSource.CreateLinkedTokenSource(cancelToken, timeoutCancel.Token).Token;

                var taskLogger = LogOptions.Get("");
                taskLogger.Name = string.Empty;
                taskLogger.Name += $"Mbox_{mailbox.MailBoxId}";

                var client = CreateMailClient(mailbox, taskLogger, commonCancelToken);

                if (client == null)
                {
                    taskLogger.InfoFormat("ReleaseMailbox(Tenant = {0} MailboxId = {1}, Address = '{2}')",
                               mailbox.TenantId, mailbox.MailBoxId, mailbox.EMail);
                    ReleaseMailbox(mailbox);
                    continue;
                }

                var task = TaskFactory.StartNew(() => ProcessMailbox(client, MailSettings),
                    commonCancelToken);

                tasks.Add(new TaskData(mailbox, task));
            }

            if (tasks.Any())
                Log.InfoFormat("Created {0} tasks.", tasks.Count);
            else
                Log.Info("No more mailboxes for processing.");

            return tasks;
        }

        private MailClient CreateMailClient(MailBoxData mailbox, ILog log, CancellationToken cancelToken)
        {
            MailClient client = null;

            var connectError = false;
            var stopClient = false;

            Stopwatch watch = null;

            if (MailSettings.CollectStatistics)
            {
                watch = new Stopwatch();
                watch.Start();
            }

            try
            {
                var serverFolderAccessInfos = Scope.DaoFactory.ImapSpecialMailboxDao.GetServerFolderAccessInfoList();
                client = new MailClient(mailbox, cancelToken, serverFolderAccessInfos, MailSettings.TcpTimeout,
                   mailbox.IsTeamlab || MailSettings.SslCertificatesErrorPermit, MailSettings.ProtocolLogPath, log, true);

                log.DebugFormat("MailClient.LoginImapPop(Tenant = {0}, MailboxId = {1} Address = '{2}')",
                    mailbox.TenantId, mailbox.MailBoxId, mailbox.EMail);

                if (!mailbox.Imap)
                {
                    client.FuncGetPop3NewMessagesIDs =
                        uidls => MessageEngine.GetPop3NewMessagesIDs(Scope.DaoFactory, mailbox, uidls, MailSettings.ChunkOfPop3Uidl);
                }

                client.Authenticated += ClientOnAuthenticated;

                client.LoginImapPop();
            }
            catch (System.TimeoutException exTimeout)
            {
                log.WarnFormat(
                    "[TIMEOUT] CreateTasks->client.LoginImapPop(Tenant = {0}, MailboxId = {1}, Address = '{2}') Exception: {3}",
                    mailbox.TenantId, mailbox.MailBoxId, mailbox.EMail, exTimeout.ToString());

                connectError = true;
                stopClient = true;
            }
            catch (OperationCanceledException)
            {
                log.InfoFormat(
                    "[CANCEL] CreateTasks->client.LoginImapPop(Tenant = {0}, MailboxId = {1}, Address = '{2}')",
                    mailbox.TenantId, mailbox.MailBoxId, mailbox.EMail);
                stopClient = true;
            }
            catch (AuthenticationException authEx)
            {
                log.ErrorFormat(
                    "CreateTasks->client.LoginImapPop(Tenant = {0}, MailboxId = {1}, Address = '{2}')\r\nException: {3}\r\n",
                    mailbox.TenantId, mailbox.MailBoxId, mailbox.EMail, authEx.ToString());

                connectError = true;
                stopClient = true;
            }
            catch (WebException webEx)
            {
                log.ErrorFormat(
                    "CreateTasks->client.LoginImapPop(Tenant = {0}, MailboxId = {1}, Address = '{2}')\r\nException: {3}\r\n",
                    mailbox.TenantId, mailbox.MailBoxId, mailbox.EMail, webEx.ToString());

                connectError = true;
                stopClient = true;
            }
            catch (Exception ex)
            {
                log.ErrorFormat(
                    "CreateTasks->client.LoginImapPop(Tenant = {0}, MailboxId = {1}, Address = '{2}')\r\nException: {3}\r\n",
                    mailbox.TenantId, mailbox.MailBoxId, mailbox.EMail,
                    ex is ImapProtocolException || ex is Pop3ProtocolException ? ex.Message : ex.ToString());

                stopClient = true;
            }
            finally
            {
                if (connectError)
                {
                    SetMailboxAuthError(mailbox, log);
                }

                if (stopClient)
                {
                    CloseMailClient(client, mailbox, log);
                }

                if (MailSettings.CollectStatistics && watch != null)
                {
                    watch.Stop();

                    LogStatistic(CONNECT_MAILBOX, mailbox, watch.Elapsed, connectError);
                }
            }

            return client;
        }

        private void SetMailboxAuthError(MailBoxData mailbox, ILog log)
        {
            try
            {
                if (mailbox.AuthErrorDate.HasValue)
                    return;

                mailbox.AuthErrorDate = DateTime.UtcNow;

                //var engine = new EngineFactory(mailbox.TenantId);
                Scope.MailEnginesFactory.MailboxEngine.SetMaiboxAuthError(mailbox.MailBoxId, mailbox.AuthErrorDate.Value);
            }
            catch (Exception ex)
            {
                log.ErrorFormat(
                    "CreateTasks->SetMailboxAuthError(Tenant = {0}, MailboxId = {1}, Address = '{2}') Exception: {3}",
                    mailbox.TenantId, mailbox.MailBoxId, mailbox.EMail, ex.Message);
            }
        }

        private void CloseMailClient(MailClient client, MailBoxData mailbox, ILog log)
        {
            if (client == null)
                return;

            try
            {
                client.Authenticated -= ClientOnAuthenticated;
                client.GetMessage -= ClientOnGetMessage;

                client.Cancel();
                client.Dispose();
            }
            catch (Exception ex)
            {
                log.ErrorFormat(
                    "CloseMailClient(Tenant = {0}, MailboxId = {1}, Address = '{2}') Exception: {3}",
                    mailbox.TenantId, mailbox.MailBoxId, mailbox.EMail, ex.Message);
            }
        }

        private void ProcessMailbox(MailClient client, MailSettings mailSettings)
        {
            var scope
                    = ServiceProvider.CreateScope().ServiceProvider.GetService<AggregatorServiceScope>();

            var mailbox = client.Account;

            Stopwatch watch = null;

            if (mailSettings.CollectStatistics)
            {
                watch = new Stopwatch();
                watch.Start();
            }

            var failed = false;

            var taskLogger = LogOptions.Get("");
            taskLogger.Name = string.Empty;
            taskLogger.Name += $"ASC.Mail Mbox_{mailbox.MailBoxId} Task_{Task.CurrentId}";

            taskLogger.InfoFormat(
                "ProcessMailbox(Tenant = {0}, MailboxId = {1} Address = '{2}') Is {3}",
                mailbox.TenantId, mailbox.MailBoxId,
                mailbox.EMail, mailbox.Active ? "Active" : "Inactive");

            try
            {
                client.Log = taskLogger;

                client.GetMessage += ClientOnGetMessage;

                client.Aggregate(mailSettings, mailSettings.MaxMessagesPerSession);
            }
            catch (OperationCanceledException)
            {
                taskLogger.InfoFormat(
                    "[CANCEL] ProcessMailbox(Tenant = {0}, MailboxId = {1}, Address = '{2}')",
                    mailbox.TenantId, mailbox.MailBoxId, mailbox.EMail);

                NotifySignalrIfNeed(mailbox, taskLogger);
            }
            catch (Exception ex)
            {
                taskLogger.ErrorFormat(
                    "ProcessMailbox(Tenant = {0}, MailboxId = {1}, Address = '{2}')\r\nException: {3}\r\n",
                    mailbox.TenantId, mailbox.MailBoxId, mailbox.EMail,
                    ex is ImapProtocolException || ex is Pop3ProtocolException ? ex.Message : ex.ToString());

                failed = true;
            }
            finally
            {
                CloseMailClient(client, mailbox, Log);

                if (MailSettings.CollectStatistics && watch != null)
                {
                    watch.Stop();

                    LogStatistic(PROCESS_MAILBOX, mailbox, watch.Elapsed, failed);
                }
            }

            var state = GetMailboxState(mailbox, taskLogger, scope.MailEnginesFactory);

            switch (state)
            {
                case MailboxState.NoChanges:
                    taskLogger.InfoFormat("MailBox with id={0} not changed.", mailbox.MailBoxId);
                    break;
                case MailboxState.Disabled:
                    taskLogger.InfoFormat("MailBox with id={0} is deactivated.", mailbox.MailBoxId);
                    break;
                case MailboxState.Deleted:
                    taskLogger.InfoFormat("MailBox with id={0} is removed.", mailbox.MailBoxId);

                    try
                    {
                        taskLogger.InfoFormat("RemoveMailBox(id={0}) >> Try clear new data from removed mailbox", mailbox.MailBoxId);

                        scope.MailEnginesFactory.MailboxEngine.RemoveMailBox(mailbox);
                    }
                    catch (Exception exRem)
                    {
                        taskLogger.InfoFormat(
                            "[REMOVE] ProcessMailbox->RemoveMailBox(Tenant = {0}, MailboxId = {1}, Address = '{2}') Exception: {3}",
                            mailbox.TenantId, mailbox.MailBoxId, mailbox.EMail, exRem.Message);
                    }
                    break;
                case MailboxState.DateChanged:
                    taskLogger.InfoFormat("MailBox with id={0}: beginDate was changed.", mailbox.MailBoxId);
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            taskLogger.InfoFormat("Mailbox '{0}' has been processed.", mailbox.EMail);
        }

        private void ClientOnAuthenticated(object sender, MailClientEventArgs mailClientEventArgs)
        {
            if (!mailClientEventArgs.Mailbox.AuthErrorDate.HasValue)
                return;

            mailClientEventArgs.Mailbox.AuthErrorDate = null;

            //var engine = new EngineFactory(mailClientEventArgs.Mailbox.TenantId);
            Scope.MailEnginesFactory.MailboxEngine.SetMaiboxAuthError(mailClientEventArgs.Mailbox.MailBoxId, mailClientEventArgs.Mailbox.AuthErrorDate);
        }

        private void ClientOnGetMessage(object sender, MailClientMessageEventArgs mailClientMessageEventArgs)
        {
            var log = Log;

            Stopwatch watch = null;

            if (MailSettings.CollectStatistics)
            {
                watch = new Stopwatch();
                watch.Start();
            }

            var failed = false;

            var mailbox = mailClientMessageEventArgs.Mailbox;
            try
            {
                var mimeMessage = mailClientMessageEventArgs.Message;
                var uid = mailClientMessageEventArgs.MessageUid;
                var folder = mailClientMessageEventArgs.Folder;
                var unread = mailClientMessageEventArgs.Unread;
                log = mailClientMessageEventArgs.Logger;

                var uidl = mailbox.Imap ? $"{uid}-{(int)folder.Folder}" : uid;

                log.InfoFormat("Found message (UIDL: '{0}', MailboxId = {1}, Address = '{2}')",
                    uidl, mailbox.MailBoxId, mailbox.EMail);

                var (tenantManager, securityContext, mailEnginesFactory)
                    = ServiceProvider.CreateScope().ServiceProvider.GetService<AggregatorServiceScope>();

                tenantManager.SetCurrentTenant(mailbox.TenantId);
                securityContext.AuthenticateMe(new Guid(mailbox.UserId));

                var message = mailEnginesFactory.MessageEngine.Save(mailbox, mimeMessage, uidl, folder, null, unread, log);

                if (message == null || message.Id <= 0)
                {
                    return;
                }

                log.InfoFormat("Message saved (id: {0}, From: '{1}', Subject: '{2}', Unread: {3})",
                    message.Id, message.From, message.Subject, message.IsNew);

                log.Info("DoOptionalOperations->START");

                DoOptionalOperations(message, mimeMessage, mailbox, folder, log, mailEnginesFactory);

                log.Info("DoOptionalOperations->END");
            }
            catch (Exception ex)
            {
                Log.ErrorFormat("[ClientOnGetMessage] Exception:\r\n{0}\r\n", ex.ToString());
                failed = true;

                throw ex;
            }
            finally
            {
                if (MailSettings.CollectStatistics && watch != null)
                {
                    watch.Stop();

                    LogStatistic(PROCESS_MESSAGE, mailbox, watch.Elapsed, failed);
                }
            }

        }

        enum MailboxState
        {
            NoChanges,
            Disabled,
            Deleted,
            DateChanged
        }

        private MailboxState GetMailboxState(MailBoxData mailbox, ILog log, MailEnginesFactory mailFactory)
        {
            try
            {
                log.Debug("GetMailBoxState()");

                //var engine = new EngineFactory(-1);
                var status = (MailboxStatus)Scope.MailEnginesFactory.MailboxEngine.GetMailboxStatus(new СoncreteUserMailboxExp(mailbox.MailBoxId, mailbox.TenantId, mailbox.UserId, null));

                if (mailbox.BeginDate != status.BeginDate)
                {
                    mailbox.BeginDateChanged = true;
                    mailbox.BeginDate = status.BeginDate;

                    return MailboxState.DateChanged;
                }

                if (status.IsRemoved)
                    return MailboxState.Deleted;

                if (!status.Enabled)
                    return MailboxState.Disabled;
            }
            catch (Exception exGetMbInfo)
            {
                log.ErrorFormat(
                    $"GetMailBoxState(Tenant = {mailbox.TenantId}, " +
                    $"MailboxId = {mailbox.MailBoxId}, " +
                    $"Address = '{mailbox.EMail}') " +
                    $"Exception: {exGetMbInfo.Message} \n{exGetMbInfo.StackTrace}");
            }

            return MailboxState.NoChanges;
        }

        private readonly ConcurrentDictionary<string, bool> _userCrmAvailabeDictionary = new ConcurrentDictionary<string, bool>();
        private readonly object _locker = new object();

        //TODO Change ApiHelper
        private bool IsCrmAvailable(MailBoxData mailbox, ILog log)
        {
            /*bool crmAvailable;

            lock (_locker)
            {
                if (_userCrmAvailabeDictionary.TryGetValue(mailbox.UserId, out crmAvailable))
                    return crmAvailable;

                crmAvailable = mailbox.IsCrmAvailable(_scope.TenantManager, _scope.SecurityContext, _scope.ApiHelper, log);
                _userCrmAvailabeDictionary.GetOrAdd(mailbox.UserId, crmAvailable);
            }

            return crmAvailable;*/
            return false;
        }

        private List<MailSieveFilterData> GetFilters(MailEnginesFactory factory, ILog log)
        {
            var user = factory.UserId;

            if (string.IsNullOrEmpty(user))
                return new List<MailSieveFilterData>();

            try
            {
                if (Filters.ContainsKey(user)) return Filters[user];

                var filters = factory.FilterEngine.GetList();

                Filters.TryAdd(user, filters);

                return filters;
            }
            catch (Exception ex)
            {
                log.Error("GetFilters failed", ex);
            }

            return new List<MailSieveFilterData>();
        }

        private void DoOptionalOperations(MailMessageData message, MimeMessage mimeMessage, MailBoxData mailbox, MailFolder folder, ILog log, MailEnginesFactory mailFactory)
        {
            try
            {
                var tagIds = new List<int>();

                if (folder.Tags.Any())
                {
                    log.Debug("DoOptionalOperations->GetOrCreateTags()");

                    tagIds = mailFactory.TagEngine.GetOrCreateTags(mailbox.TenantId, mailbox.UserId, folder.Tags);
                }

                log.Debug("DoOptionalOperations->IsCrmAvailable()");

                if (IsCrmAvailable(mailbox, log))
                {
                    log.Debug("DoOptionalOperations->GetCrmTags()");

                    var crmTagIds = mailFactory.TagEngine.GetCrmTags(message.FromEmail);

                    if (crmTagIds.Any())
                    {
                        if (tagIds == null)
                            tagIds = new List<int>();

                        tagIds.AddRange(crmTagIds.Select(t => t.Id));
                    }
                }

                if (tagIds.Any())
                {
                    if (message.TagIds == null || !message.TagIds.Any())
                        message.TagIds = tagIds;
                    else
                        message.TagIds.AddRange(tagIds);

                    message.TagIds = message.TagIds.Distinct().ToList();
                }

                log.Debug("DoOptionalOperations->AddMessageToIndex()");

                var wrapper = message.ToMailWrapper(mailbox.TenantId, new Guid(mailbox.UserId));

                mailFactory.IndexEngine.Add(wrapper);

                foreach (var tagId in tagIds)
                {
                    try
                    {
                        log.DebugFormat("DoOptionalOperations->SetMessagesTag(tagId: {0})", tagId);

                        mailFactory.TagEngine.SetMessagesTag(new List<int> { message.Id }, tagId);
                    }
                    catch (Exception e)
                    {
                        log.ErrorFormat(
                            "SetMessagesTag(tenant={0}, userId='{1}', messageId={2}, tagid = {3}) Exception:\r\n{4}\r\n",
                            mailbox.TenantId, mailbox.UserId, message.Id, e.ToString(),
                            tagIds != null ? string.Join(",", tagIds) : "null");
                    }
                }

                log.Debug("DoOptionalOperations->AddRelationshipEventForLinkedAccounts()");

                mailFactory.CrmLinkEngine.AddRelationshipEventForLinkedAccounts(mailbox, message, MailSettings.DefaultApiSchema);

                log.Debug("DoOptionalOperations->SaveEmailInData()");

                mailFactory.EmailInEngine.SaveEmailInData(mailbox, message, MailSettings.DefaultApiSchema);

                log.Debug("DoOptionalOperations->SendAutoreply()");

                mailFactory.AutoreplyEngine.SendAutoreply(mailbox, message, MailSettings.DefaultApiSchema, log);

                log.Debug("DoOptionalOperations->UploadIcsToCalendar()");

                if (folder.Folder != Enums.FolderType.Spam)
                {
                    mailFactory
                        .CalendarEngine
                        .UploadIcsToCalendar(mailbox, message.CalendarId, message.CalendarUid, message.CalendarEventIcs,
                            message.CalendarEventCharset, message.CalendarEventMimeType, mailbox.EMail.Address,
                            MailSettings.DefaultApiSchema);
                }

                if (MailSettings.SaveOriginalMessage)
                {
                    log.Debug("DoOptionalOperations->StoreMailEml()");
                    StoreMailEml(mailbox.TenantId, mailbox.UserId, message.StreamId, mimeMessage, log);
                }

                log.Debug("DoOptionalOperations->ApplyFilters()");

                var filters = GetFilters(mailFactory, log);

                mailFactory.FilterEngine.ApplyFilters(message, mailbox, folder, filters);

                log.Debug("DoOptionalOperations->NotifySignalrIfNeed()");

                NotifySignalrIfNeed(mailbox, log);
            }
            catch (Exception ex)
            {
                log.ErrorFormat("DoOptionalOperations() Exception:\r\n{0}\r\n", ex.ToString());
            }
        }

        public string StoreMailEml(int tenant, string user, string streamId, MimeMessage message, ILog log)
        {
            if (message == null)
                return string.Empty;

            // Using id_user as domain in S3 Storage - allows not to add quota to tenant.
            var savePath = MailStoragePathCombiner.GetEmlKey(user, streamId);
            var storage = Scope.StorageFactory.GetMailStorage(tenant);

            try
            {
                using (var stream = new MemoryStream())
                {
                    message.WriteTo(stream);

                    var res = storage.Save(savePath, stream, MailStoragePathCombiner.EML_FILE_NAME).ToString();

                    log.InfoFormat("StoreMailEml() tenant='{0}', user_id='{1}', save_eml_path='{2}' Result: {3}", tenant, user, savePath, res);

                    return res;
                }
            }
            catch (Exception ex)
            {
                log.ErrorFormat("StoreMailEml Exception: {0}", ex.ToString());
            }

            return string.Empty;
        }

        public void FreeTask(TaskData taskData, ICollection<TaskData> tasks)
        {
            try
            {
                Log.DebugFormat("End Task {0} with status = '{1}'.", taskData.Task.Id, taskData.Task.Status);

                if (!tasks.Remove(taskData))
                    Log.Error("Task not exists in tasks array.");

                var mailbox = taskData.Mailbox;

                ReleaseMailbox(mailbox);

                taskData.Task.Dispose();
            }
            catch (Exception ex)
            {
                Log.ErrorFormat("FreeTask(id:'{0}', email:'{1}'): Exception:\r\n{2}\r\n", taskData.Mailbox.MailBoxId, taskData.Mailbox.EMail, ex.ToString());
            }
        }

        private void ReleaseMailbox(MailBoxData mailbox)
        {
            if (mailbox == null)
                return;

            if (mailbox.LastSignalrNotifySkipped)
                NotifySignalrIfNeed(mailbox, Log);

            Scope.QueueManager.ReleaseMailbox(mailbox);

            if (!Filters.ContainsKey(mailbox.UserId))
                return;

            List<MailSieveFilterData> filters;
            if (!Filters.TryRemove(mailbox.UserId, out filters))
            {
                Log.Error("Try forget Filters for user failed");
            }
        }

        private void LogStatistic(string method, MailBoxData mailBoxData, TimeSpan duration, bool failed)
        {
            if (!MailSettings.CollectStatistics)
                return;

            LogStat.DebugWithProps(method,
                new KeyValuePair<string, object>("duration", duration.TotalMilliseconds),
                new KeyValuePair<string, object>("mailboxId", mailBoxData.MailBoxId),
                new KeyValuePair<string, object>("address", mailBoxData.EMail.ToString()),
                new KeyValuePair<string, object>("status", failed ? S_FAIL : S_OK));
        }
        #endregion
    }

    [Scope]
    internal class AggregatorServiceScope
    {
        internal TenantManager TenantManager { get; }
        internal CoreBaseSettings CoreBaseSettings { get; }
        internal QueueManager QueueManager { get; }
        internal MailQueueItemSettings MailQueueItemSettings { get; }
        internal StorageFactory StorageFactory { get; }
        internal MailEnginesFactory MailEnginesFactory { get; }
        internal SecurityContext SecurityContext { get; }
        internal ApiHelper ApiHelper { get; }
        internal SignalrWorker SignalrWorker { get; }
        internal DaoFactory DaoFactory { get; }

        public AggregatorServiceScope(
            TenantManager tenantManager,
            CoreBaseSettings coreBaseSettings,
            MailQueueItemSettings mailQueueItemSettings,
            StorageFactory storageFactory,
            MailEnginesFactory mailEnginesFactory,
            SecurityContext securityContext,
            ApiHelper apiHelper,
            QueueManager queueManager,
            SignalrWorker signalrWorker,
            DaoFactory daoFactory)
        {
            TenantManager = tenantManager;
            MailQueueItemSettings = mailQueueItemSettings;
            CoreBaseSettings = coreBaseSettings;
            StorageFactory = storageFactory;
            MailEnginesFactory = mailEnginesFactory;
            SecurityContext = securityContext;
            ApiHelper = apiHelper;
            QueueManager = queueManager;
            SignalrWorker = signalrWorker;
            DaoFactory = daoFactory;
        }

        public void Deconstruct(
            out TenantManager tenantManager,
            out SecurityContext securityContext,
            out MailEnginesFactory mailEnginesFactory
            )
        {
            tenantManager = TenantManager;
            mailEnginesFactory = MailEnginesFactory;
            securityContext = SecurityContext;
        }

        public void Deconstruct(
            out MailQueueItemSettings mailQueueItemSettings,
            out SignalrWorker signalrWorker)
        {
            mailQueueItemSettings = MailQueueItemSettings;
            signalrWorker = SignalrWorker;
        }
    }

    public class AggregatorServiceExtension
    {
        public static void Register(DIHelper services)
        {
            services.TryAdd<AggregatorServiceScope>();
        }
    }
}
