using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Runtime.Caching;
using System.Threading;

using ASC.Common.Logging;
using ASC.Core;
using ASC.Mail.Aggregator.CollectionService.Queue.Data;
using ASC.Mail.Core;
using ASC.Mail.Core.Dao.Expressions.Mailbox;
using ASC.Mail.Models;
using ASC.Mail.Extensions;

using LiteDB;
using ASC.Mail.Configuration;
using ASC.Common;
using ASC.Mail.Utils;
using ASC.Mail.Core.Engine;

namespace ASC.Mail.Aggregator.CollectionService.Queue
{
    [Scope]
    public class QueueManager : IDisposable
    {
        private readonly int _maxItemsLimit;
        private readonly Queue<MailBoxData> _mailBoxQueue;
        private List<MailBoxData> _lockedMailBoxList;
        private readonly ILog _log;
        private DateTime _loadQueueTime;
        private MemoryCache _tenantMemCache;
        private const string DBC_MAILBOXES = "mailboxes";
        private const string DBC_TENANTS = "tenants";
        private static string _dbcFile;
        private static string _dbcJournalFile;
        private readonly object _locker = new object();
        private LiteDatabase _db;
        private ILiteCollection<MailboxData> _mailboxes;
        private ILiteCollection<TenantData> _tenants;


        private MailSettings MailSettings { get; }
        private TenantManager TenantManager { get; }
        private SecurityContext SecurityContext { get; }
        private ApiHelper ApiHelper { get; }
        private UserManager UserManager { get; }
        private MailboxEngine MailboxEngine { get; }
        private AlertEngine AlertEngine { get; }

        public ManualResetEvent CancelHandler { get; set; }

        public QueueManager(
            MailSettings mailSettings, 
            ILog log,
            TenantManager tenantManager, 
            SecurityContext securityContext,
            ApiHelper apiHelper,
            UserManager userManager,
            MailboxEngine mailboxEngine,
            AlertEngine alertEngine)
        {
            _maxItemsLimit = mailSettings.MaxTasksAtOnce;
            _mailBoxQueue = new Queue<MailBoxData>();
            _lockedMailBoxList = new List<MailBoxData>();

            MailSettings = mailSettings;
            TenantManager = tenantManager;
            SecurityContext = securityContext;
            ApiHelper = apiHelper;
            UserManager = userManager;
            MailboxEngine = mailboxEngine;
            AlertEngine = alertEngine;

            _log = log;
            _loadQueueTime = DateTime.UtcNow;
            _tenantMemCache = new MemoryCache("QueueManagerTenantCache");

            CancelHandler = new ManualResetEvent(false);

            if (MailSettings.UseDump)
            {
                _dbcFile = Path.Combine(Environment.CurrentDirectory, "dump.db");
                _dbcJournalFile = Path.Combine(Environment.CurrentDirectory, "dump-journal.db");

                _log.DebugFormat("Dump file path: {0}", _dbcFile);

                LoadDump();
            }
        }

        #region - public methods -

        public IEnumerable<MailBoxData> GetLockedMailboxes(int needTasks)
        {
            var mbList = new List<MailBoxData>();
            do
            {
                var mailBox = GetLockedMailbox();
                if (mailBox == null)
                    break;

                mbList.Add(mailBox);

            } while (mbList.Count < needTasks);

            return mbList;
        }

        public MailBoxData GetLockedMailbox()
        {
            MailBoxData mailBoxData;

            do
            {
                mailBoxData = GetQueuedMailbox();

            } while (mailBoxData != null && !TryLockMailbox(mailBoxData));

            if (mailBoxData == null)
                return null;

            _lockedMailBoxList.Add(mailBoxData);

            CancelHandler.Reset();

            AddMailboxToDumpDb(mailBoxData.ToMailboxData());

            return mailBoxData;
        }

        public void ReleaseAllProcessingMailboxes()
        {
            if (!_lockedMailBoxList.Any())
                return;

            var cloneCollection = new List<MailBoxData>(_lockedMailBoxList);

            _log.Info("QueueManager->ReleaseAllProcessingMailboxes()");

            foreach (var mailbox in cloneCollection)
            {
                ReleaseMailbox(mailbox);
            }
        }

        public void ReleaseMailbox(MailBoxData mailBoxData)
        {
            try
            {
                if (!_lockedMailBoxList.Contains(mailBoxData))
                {
                    _log.WarnFormat("QueueManager->ReleaseMailbox(Tenant = {0} MailboxId = {1}, Address = '{2}') mailbox not found",
                               mailBoxData.TenantId, mailBoxData.MailBoxId, mailBoxData.EMail);
                    return;
                }

                _log.InfoFormat("QueueManager->ReleaseMailbox(MailboxId = {0} Address '{1}')", mailBoxData.MailBoxId, mailBoxData.EMail);

                TenantManager.SetCurrentTenant(mailBoxData.TenantId);

                MailboxEngine.ReleaseMaibox(mailBoxData, MailSettings);

                _lockedMailBoxList.Remove(mailBoxData);

                DeleteMailboxFromDumpDb(mailBoxData.MailBoxId);
            }
            catch (Exception ex)
            {
                _log.ErrorFormat("QueueManager->ReleaseMailbox(Tenant = {0} MailboxId = {1}, Address = '{2}')\r\nException: {3} \r\n",
                    mailBoxData.TenantId, mailBoxData.MailBoxId, mailBoxData.Account, ex.ToString());
            }
        }

        public int ProcessingCount
        {
            get { return _lockedMailBoxList.Count; }
        }

        public void LoadMailboxesFromDump()
        {
            if (!MailSettings.UseDump)
                return;

            if (_lockedMailBoxList.Any())
                return;

            try
            {
                _log.Debug("LoadMailboxesFromDump()");

                lock (_locker)
                {
                    var list = _mailboxes.FindAll().ToList();

                    _lockedMailBoxList = list.ConvertAll(m => m.ToMailbox()).ToList();
                }
            }
            catch (Exception ex)
            {
                _log.ErrorFormat("LoadMailboxesFromDump: {0}", ex.ToString());

                ReCreateDump();
            }
        }

        public void LoadTenantsFromDump()
        {
            if (!MailSettings.UseDump)
                return;

            try
            {
                _log.Debug("LoadTenantsFromDump()");

                lock (_locker)
                {
                    var list = _tenants.FindAll().ToList();

                    foreach (var tenantData in list)
                    {
                        AddTenantToCache(tenantData, false);
                    }
                }

            }
            catch (Exception ex)
            {
                _log.ErrorFormat("LoadTenantsFromDump: {0}", ex.ToString());

                ReCreateDump();
            }
        }

        #endregion

        #region - private methods -

        private void ReCreateDump()
        {
            if (!MailSettings.UseDump)
                return;

            try
            {
                if (File.Exists(_dbcFile))
                {
                    _log.DebugFormat("Dump file '{0}' exists, trying delete", _dbcFile);

                    File.Delete(_dbcFile);

                    _log.DebugFormat("Dump file '{0}' deleted", _dbcFile);
                }

                if (File.Exists(_dbcJournalFile))
                {
                    _log.DebugFormat("Dump journal file '{0}' exists, trying delete", _dbcJournalFile);

                    File.Delete(_dbcJournalFile);

                    _log.DebugFormat("Dump journal file '{0}' deleted", _dbcJournalFile);
                }

                _db = new LiteDatabase(_dbcFile);

                lock (_locker)
                {
                    _mailboxes = _db.GetCollection<MailboxData>(DBC_MAILBOXES);
                    _tenants = _db.GetCollection<TenantData>(DBC_TENANTS);
                }
            }
            catch (Exception ex)
            {
                _log.ErrorFormat("ReCreateDump() failed Exception: {0}", ex.ToString());
            }
        }

        private void AddMailboxToDumpDb(MailboxData mailboxData)
        {
            if (!MailSettings.UseDump)
                return;

            try
            {
                lock (_locker)
                {
                    var mailbox = _mailboxes.FindOne(Query.EQ("MailboxId", mailboxData.MailboxId));

                    if (mailbox != null)
                        return;

                    _mailboxes.Insert(mailboxData);

                    // Create, if not exists, new index on Name field
                    _mailboxes.EnsureIndex(x => x.MailboxId);
                }
            }
            catch (Exception ex)
            {
                _log.ErrorFormat("AddMailboxToDumpDb(id={0}) Exception: {1}", mailboxData.MailboxId, ex.ToString());

                ReCreateDump();
            }
        }

        private void DeleteMailboxFromDumpDb(int mailBoxId)
        {
            if (!MailSettings.UseDump)
                return;

            try
            {
                lock (_locker)
                {
                    var mailbox = _mailboxes.FindOne(Query.EQ("MailboxId", mailBoxId));

                    if (mailbox == null)
                        return;

                    _mailboxes.DeleteMany(Query.EQ("MailboxId", mailBoxId));
                }
            }
            catch (Exception ex)
            {
                _log.ErrorFormat("DeleteMailboxFromDumpDb(mailboxId={0}) Exception: {1}", mailBoxId, ex.ToString());

                ReCreateDump();
            }
        }

        private void LoadDump()
        {
            if (!MailSettings.UseDump)
                return;

            try
            {
                if (File.Exists(_dbcJournalFile))
                    throw new Exception(string.Format("temp dump journal file exists in {0}", _dbcJournalFile));

                _db = new LiteDatabase(_dbcFile);

                lock (_locker)
                {
                    _tenants = _db.GetCollection<TenantData>(DBC_TENANTS);
                    _mailboxes = _db.GetCollection<MailboxData>(DBC_MAILBOXES);
                }
            }
            catch (Exception ex)
            {
                _log.ErrorFormat("LoadDump() failed Exception: {0}", ex.ToString());

                ReCreateDump();
            }
        }

        private void AddTenantToDumpDb(TenantData tenantData)
        {
            if (!MailSettings.UseDump)
                return;

            try
            {
                lock (_locker)
                {
                    var tenant = _tenants.FindOne(Query.EQ("Tenant", tenantData.Tenant));

                    if (tenant != null)
                        return;

                    _tenants.Insert(tenantData);

                    // Create, if not exists, new index on Name field
                    _tenants.EnsureIndex(x => x.Tenant);
                }
            }
            catch (Exception ex)
            {
                _log.ErrorFormat("AddTenantToDumpDb(tenantId={0}) Exception: {1}", tenantData.Tenant, ex.ToString());

                ReCreateDump();
            }
        }

        private void DeleteTenantFromDumpDb(int tenantId)
        {
            if (!MailSettings.UseDump)
                return;

            try
            {
                lock (_locker)
                {
                    var tenant = _tenants.FindOne(Query.EQ("Tenant", tenantId));

                    if (tenant == null)
                        return;

                    _tenants.DeleteMany(Query.EQ("Tenant", tenantId));
                }
            }
            catch (Exception ex)
            {
                _log.ErrorFormat("DeleteTenantFromDumpDb(tenant={0}) Exception: {1}", tenantId, ex.ToString());

                ReCreateDump();
            }
        }

        private void AddTenantToCache(TenantData tenantData, bool needDump = true)
        {
            var now = DateTime.UtcNow;

            if (tenantData.Expired < now)
            {
                DeleteTenantFromDumpDb(tenantData.Tenant);
                return; // Skip Expired tenant
            }

            var cacheItem = new CacheItem(tenantData.Tenant.ToString(CultureInfo.InvariantCulture), tenantData);

            var nowOffset = tenantData.Expired - now;

            var absoluteExpiration = DateTime.UtcNow.Add(nowOffset);

            var cacheItemPolicy = new CacheItemPolicy
            {
                RemovedCallback = CacheEntryRemove,
                AbsoluteExpiration = absoluteExpiration
            };

            _tenantMemCache.Add(cacheItem, cacheItemPolicy);

            if (!needDump)
                return;

            AddTenantToDumpDb(tenantData);
        }

        private bool QueueIsEmpty
        {
            get { return !_mailBoxQueue.Any(); }
        }

        private bool QueueLifetimeExpired
        {
            get { return DateTime.UtcNow - _loadQueueTime >= MailSettings.QueueLifetime; }
        }

        private void LoadQueue()
        {
            try
            {
                var mbList = MailboxEngine.GetMailboxesForProcessing(MailSettings, _maxItemsLimit);

                ReloadQueue(mbList);
            }
            catch (Exception ex)
            {
                _log.ErrorFormat("QueueManager->LoadQueue()\r\nException: \r\n {0}", ex.ToString());
            }
        }

        private MailBoxData GetQueuedMailbox()
        {
            if (QueueIsEmpty || QueueLifetimeExpired)
            {
                _log.DebugFormat("Queue is {0}. Load new queue.", QueueIsEmpty ? "EMPTY" : "EXPIRED");

                LoadQueue();
            }

            return !QueueIsEmpty ? _mailBoxQueue.Dequeue() : null;
        }

        private void RemoveFromQueue(int tenant)
        {
            var mbList = _mailBoxQueue.Where(mb => mb.TenantId != tenant).Select(mb => mb).ToList();
            ReloadQueue(mbList);
        }

        private void RemoveFromQueue(int tenant, string user)
        {
            var mbList = _mailBoxQueue.Where(mb => mb.TenantId != tenant && mb.UserId != user).Select(mb => mb).ToList();
            ReloadQueue(mbList);
        }

        private void ReloadQueue(IEnumerable<MailBoxData> mbList)
        {
            _mailBoxQueue.Clear();
            _mailBoxQueue.PushRange(mbList);
            _loadQueueTime = DateTime.UtcNow;
        }

        private bool TryLockMailbox(MailBoxData mailbox)
        {
            _log.DebugFormat("TryLockMailbox(MailboxId={0} is {1})", mailbox.MailBoxId, mailbox.Active ? "active" : "inactive");

            try
            {
                var contains = _tenantMemCache.Contains(mailbox.TenantId.ToString(CultureInfo.InvariantCulture));

                if (!contains)
                {
                    _log.DebugFormat("Tenant {0} isn't in cache", mailbox.TenantId);
                    try
                    {
                        var type = mailbox.GetTenantStatus(TenantManager, SecurityContext, ApiHelper, (int)MailSettings.TenantOverdueDays, _log);

                        switch (type)
                        {
                            case DefineConstants.TariffType.LongDead:
                                _log.InfoFormat("Tenant {0} is not paid. Disable mailboxes.", mailbox.TenantId);

                                MailboxEngine.DisableMailboxes(
                                    new TenantMailboxExp(mailbox.TenantId));

                                var userIds =
                                    MailboxEngine.GetMailUsers(new TenantMailboxExp(mailbox.TenantId))
                                        .ConvertAll(t => t.Item2);

                                AlertEngine.CreateDisableAllMailboxesAlert(mailbox.TenantId, userIds);

                                RemoveFromQueue(mailbox.TenantId);

                                return false;

                            case DefineConstants.TariffType.Overdue:
                                _log.InfoFormat("Tenant {0} is not paid. Stop processing mailboxes.", mailbox.TenantId);

                                MailboxEngine.SetNextLoginDelay(new TenantMailboxExp(mailbox.TenantId),
                                    MailSettings.OverdueAccountDelay);

                                RemoveFromQueue(mailbox.TenantId);

                                return false;

                            default:
                                _log.InfoFormat("Tenant {0} is paid.", mailbox.TenantId);

                                var expired = DateTime.UtcNow.Add(MailSettings.TenantCachingPeriod);

                                var tenantData = new TenantData
                                {
                                    Tenant = mailbox.TenantId,
                                    TariffType = type,
                                    Expired = expired
                                };

                                AddTenantToCache(tenantData);

                                break;
                        }
                    }
                    catch (Exception e)
                    {
                        _log.ErrorFormat("TryLockMailbox() -> GetTariffType Exception:\r\n{0}\r\n", e.ToString());
                    }
                }
                else
                {
                    _log.DebugFormat("Tenant {0} is in cache", mailbox.TenantId);
                }

                if (mailbox.IsUserTerminated(TenantManager, UserManager) || mailbox.IsUserRemoved(TenantManager, UserManager))
                {
                    _log.InfoFormat("User '{0}' was terminated. Tenant = {1}. Disable mailboxes for user.",
                              mailbox.UserId,
                              mailbox.TenantId);

                    MailboxEngine.DisableMailboxes(
                        new UserMailboxExp(mailbox.TenantId, mailbox.UserId));

                    AlertEngine.CreateDisableAllMailboxesAlert(mailbox.TenantId,
                        new List<string> { mailbox.UserId });

                    RemoveFromQueue(mailbox.TenantId, mailbox.UserId);

                    return false;
                }

                if (mailbox.IsTenantQuotaEnded(TenantManager, (int)MailSettings.TenantMinQuotaBalance, _log))
                {
                    _log.InfoFormat("Tenant = {0} User = {1}. Quota is ended.", mailbox.TenantId, mailbox.UserId);

                    if (!mailbox.QuotaError)
                        AlertEngine.CreateQuotaErrorWarningAlert(mailbox.TenantId, mailbox.UserId);

                    MailboxEngine.SetNextLoginDelay(new UserMailboxExp(mailbox.TenantId, mailbox.UserId),
                                    MailSettings.QuotaEndedDelay);

                    RemoveFromQueue(mailbox.TenantId, mailbox.UserId);

                    return false;
                }

                return MailboxEngine.LockMaibox(mailbox.MailBoxId);

            }
            catch (Exception ex)
            {
                _log.ErrorFormat("TryLockMailbox(MailboxId={0} is {1}) Exception:\r\n{2}\r\n", mailbox.MailBoxId,
                           mailbox.Active ? "active" : "inactive", ex.ToString());

                return false;
            }

        }

        private void CacheEntryRemove(CacheEntryRemovedArguments arguments)
        {
            if (arguments.RemovedReason == CacheEntryRemovedReason.CacheSpecificEviction)
                return;

            var tenantId = Convert.ToInt32(arguments.CacheItem.Key);

            _log.InfoFormat("Tenant {0} payment cache is expired.", tenantId);

            DeleteTenantFromDumpDb(tenantId);
        }

        #endregion

        public void Dispose()
        {
            _tenantMemCache.Dispose();
            _tenantMemCache = null;

            if (MailSettings.UseDump)
            {
                _db.Dispose();
                _db = null;
            }
        }
    }
}
