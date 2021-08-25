﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

using ASC.Common;
using ASC.Common.Logging;
using ASC.Core;
using ASC.Core.Notify.Signalr;
using ASC.Core.Users;
using ASC.Mail.Core.Engine;
using ASC.Mail.Enums;
using ASC.Mail.Models;

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace ASC.Mail.Aggregator.CollectionService.Queue
{
    [Singletone]
    public class SignalrWorker : IDisposable
    {
        private readonly Queue<MailBoxData> _processingQueue;
        private Task _workerTask;
        private volatile bool _workerTerminateSignal;
        private readonly EventWaitHandle _waitHandle;
        private readonly TimeSpan _timeSpan;
        public bool StartImmediately { get; set; } = true;

        private ILog Log { get; }
        private SignalrServiceClient SignalrServiceClient { get; }
        private IServiceProvider ServiceProvider { get; }
        private CancellationTokenSource CancellationTokenSource { get; }

        public SignalrWorker(
            IOptionsMonitor<ILog> optionsMonitor,
            IOptionsSnapshot<SignalrServiceClient> optionsSnapshot,
            IServiceProvider serviceProvider)
        {
            Log = optionsMonitor.Get("ASC.Mail.SignalrWorker");
            SignalrServiceClient = optionsSnapshot.Get("mail");
            ServiceProvider = serviceProvider;
            CancellationTokenSource = new CancellationTokenSource();

            _workerTerminateSignal = false;
            _processingQueue = new Queue<MailBoxData>();
            _waitHandle = new EventWaitHandle(false, EventResetMode.AutoReset);
            _timeSpan = TimeSpan.FromSeconds(15);

            _workerTask = new Task(ProcessQueue, CancellationTokenSource.Token);

            if (StartImmediately)
                _workerTask.Start();
        }

        private void ProcessQueue()
        {
            while (!_workerTerminateSignal)
            {
                if (!HasQueuedMailbox)
                {
                    Log.Debug("No items, waiting.");
                    _waitHandle.WaitOne();
                    Log.Debug("Waking up...");
                }

                var mailbox = NextQueuedMailBoxData;
                if (mailbox == null)
                    continue;

                try
                {
                    Log.DebugFormat($"SignalrWorker -> SendUnreadUser(UserId = {mailbox.UserId} TenantId = {mailbox.TenantId})");

                    SendUnreadUser(mailbox.TenantId, mailbox.UserId);
                }
                catch (Exception ex)
                {
                    Log.ErrorFormat($"SignalrWorker -> SendUnreadUser(UserId = {mailbox.UserId} TenantId = {mailbox.TenantId})\r\nException: \r\n{ex}");
                }

                _waitHandle.Reset();
            }
        }

        public int QueueCount
        {
            get
            {
                lock (_processingQueue)
                {
                    return _processingQueue.Count;
                }
            }
        }

        public bool HasQueuedMailbox
        {
            get
            {
                lock (_processingQueue)
                {
                    return _processingQueue.Any();
                }
            }
        }

        public MailBoxData NextQueuedMailBoxData
        {
            get
            {
                if (!HasQueuedMailbox)
                    return null;

                lock (_processingQueue)
                {
                    return _processingQueue.Dequeue();
                }
            }
        }

        public void AddMailbox(MailBoxData item)
        {
            lock (_processingQueue)
            {
                if (!_processingQueue.Contains(item))
                    _processingQueue.Enqueue(item);
            }
            _waitHandle.Set();
        }

        public void Dispose()
        {
            if (_workerTask == null)
                return;

            _workerTerminateSignal = true;
            _waitHandle.Set();

            if (_workerTask.Status == TaskStatus.Running)
            {
                Log.Info("Stop SignalrWorker.");

                if (!_workerTask.Wait(_timeSpan))
                {
                    Log.Info("SignalrWorker busy, cancellation of the task.");
                    CancellationTokenSource.Cancel();
                }
            }

            _workerTask = null;
        }

        private void SendUnreadUser(int tenant, string userId)
        {
            try
            {
                using var scope = ServiceProvider.CreateScope();

                var folderEngine = scope.ServiceProvider.GetService<FolderEngine>();
                var tenantManager = scope.ServiceProvider.GetService<TenantManager>();
                var userManager = scope.ServiceProvider.GetService<UserManager>();

                Log.Debug($"SignalrWorker -> SendUnreadUser(). Try set tenant |{tenant}| for user |{userId}|...");

                tenantManager.SetCurrentTenant(tenant);

                Log.Debug($"SignalrWorker -> SendUnreadUser(). Now current tennant = {tenantManager.GetCurrentTenant().TenantId}");

                var mailFolderInfos = folderEngine.GetFolders();

                var count = (from mailFolderInfo in mailFolderInfos
                             where mailFolderInfo.id == FolderType.Inbox
                             select mailFolderInfo.unreadMessages)
                    .FirstOrDefault();

                var userInfo = userManager.GetUsers(Guid.Parse(userId));
                if (userInfo.ID != Constants.LostUser.ID)
                {
                    // sendMailsCount
                    SignalrServiceClient.SendUnreadUser(tenant, userId, count);
                }
            }
            catch (Exception e)
            {
                Log.ErrorFormat("SignalrWorker -> Unknown Error. {0}, {1}", e.ToString(),
                    e.InnerException != null ? e.InnerException.Message : string.Empty);
            }
        }
    }
}
