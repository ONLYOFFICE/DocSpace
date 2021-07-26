/*
 *
 * (c) Copyright Ascensio System Limited 2010-2020
 * 
 * Licensed under the Apache License, Version 2.0 (the "License");
 * you may not use this file except in compliance with the License.
 * You may obtain a copy of the License at
 * http://www.apache.org/licenses/LICENSE-2.0
 * Unless required by applicable law or agreed to in writing, software
 * distributed under the License is distributed on an "AS IS" BASIS,
 * WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 * See the License for the specific language governing permissions and
 * limitations under the License.
 *
*/


using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Security.Authentication;
using System.Threading;
using System.Threading.Tasks;

using ASC.Common.Logging;
using ASC.Core;
using ASC.Core.Common.Caching;
using ASC.Core.Notify.Signalr;
using ASC.Core.Users;
using ASC.Data.Storage;
using ASC.Mail.Clients.Imap;
using ASC.Mail.Configuration;
using ASC.Mail.Core.Dao;
using ASC.Mail.Core.Dao.Expressions.Message;
using ASC.Mail.Core.Engine;
using ASC.Mail.Core.Entities;
using ASC.Mail.Enums;
using ASC.Mail.Extensions;
using ASC.Mail.Models;
using ASC.Mail.Storage;
using ASC.Mail.Utils;

using MailKit;
using MailKit.Net.Imap;
using MailKit.Security;

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

using MimeKit;

using MailFolder = ASC.Mail.Models.MailFolder;

namespace ASC.Mail.ImapSync
{
    public class MailImapClient : IDisposable
    {
        public ConcurrentDictionary<string, List<MailSieveFilterData>> Filters { get; set; }

        private const int SIGNALR_WAIT_SECONDS = 30;

        private readonly MailEnginesFactory _mailEnginesFactory;
        private readonly MailSettings _mailSettings;
        private readonly MailInfoDao _mailInfoDao;
        private readonly StorageFactory _storageFactory;
        private readonly FolderEngine _folderEngine;
        private readonly UserManager _userManager;
        private readonly SignalrServiceClient _signalrServiceClient;
        private readonly RedisClient _redisClient;
        private readonly ILog _log;
        private readonly IndexEngine _indexEngine;

        const MessageSummaryItems SummaryItems = MessageSummaryItems.UniqueId | MessageSummaryItems.Flags | MessageSummaryItems.Envelope | MessageSummaryItems.BodyStructure;

        readonly bool crmAvailable;
        private bool IMAPEventInProgress;

        public MailBoxData Account { get; private set; }

        protected ImapClient Imap { get; private set; }

        private CancellationTokenSource DoneToken { get; set; }
        private CancellationToken CancelToken { get; set; }
        private CancellationTokenSource StopTokenSource { get; set; }

        private readonly SemaphoreSlim ImapSemaphore;
        private readonly SemaphoreSlim RequestCountSemaphore;

        private Dictionary<IMailFolder, MailFolder> foldersDictionary;

        private bool ClientIsReadyToWork = false;

        private ConcurrentQueue<int> imapMessagesToUpdate;

        public IMailFolder WorkFolder;

        private Timer _deathTimer;

        public event EventHandler DeleteClient;

        public ClientState ClientCurrentState { get; private set; }

        private void ImapMessageFlagsChanged(object sender, MessageFlagsChangedEventArgs e)
        {
            if (ClientIsReadyToWork && sender is IMailFolder imap_folder)
            {
                _log.Debug($"ImapMessageFlagsChanged. Folder={imap_folder.FullName} Index={e.Index} Flags={e.Flags}. imapMessagesToUpdate.Count={imapMessagesToUpdate.Count}");

                _= UpdateMessageFlagInDb(e.Index, WorkFolder);
            }
        }

        private void ImapCountChanged(object sender, EventArgs e)
        {
            if (ClientIsReadyToWork && sender is IMailFolder imap_folder)
            {
                _log.Debug($"ImapCountChanged. Folder={imap_folder.FullName} Count={imap_folder.Count}.");

                _ = SynchronizeMailFolderFromImap(imap_folder);
            }
        }

        public async void CheckRedis(int folderActivity, IEnumerable<int> tags)
        {
            if (foldersDictionary == null) return;

            if (ClientIsReadyToWork)
            {
                var folderKeyValue = foldersDictionary.FirstOrDefault(x => x.Value.Folder == (FolderType)folderActivity);

                if (folderKeyValue.Key != null && WorkFolder != folderKeyValue.Key)
                {
                    WorkFolder = folderKeyValue.Key;

                    await SynchronizeMailFolderFromImap(WorkFolder);

                    _log.Debug($"CheckRedis. WorkFolder change to {WorkFolder.FullName} Count={WorkFolder.Count}.");
                }

                var key = _redisClient.CreateQueueKey(Account.MailBoxId);

                _log.Debug($"ProcessActionFromRedis. Begin read key: {key}");

                int iterationCount = 0;

                while (true)
                {
                    var actionFromCache = await _redisClient.PopFromQueue<CashedMailUserAction>(key);

                    if (actionFromCache == null) break;

                    var actionSplittedList = actionFromCache.Uidls.Select(x => new SplittedUidl(x)).ToList();

                    if (!actionSplittedList.Any()) continue;

                    iterationCount++;

                    if (actionFromCache.Action == MailUserAction.MoveTo)
                    {
                        await MoveMessageInImap(actionSplittedList, actionFromCache.Destination);
                    }
                    else
                    {
                        await SetFlagsInImap(actionSplittedList, actionFromCache.Action);
                    };
                }

                _log.Debug($"ProcessActionFromRedis. {iterationCount} keys readed.");
            }
        }

        public MailImapClient(MailBoxData mailbox, CancellationToken cancelToken, MailSettings mailSettings, IServiceProvider serviceProvider)
        {
            IMAPEventInProgress = false;

            Account = mailbox;
            _mailSettings = mailSettings;

            var clientScope = serviceProvider.CreateScope().ServiceProvider;

            var protocolLogger = !string.IsNullOrEmpty(_mailSettings.ProtocolLogPath)
                ? (IProtocolLogger)
                    new ProtocolLogger(_mailSettings.ProtocolLogPath + $"\\imap_{Account.MailBoxId}_{Thread.CurrentThread.ManagedThreadId}.log", true)
                : new NullProtocolLogger();

            imapMessagesToUpdate = new ConcurrentQueue<int>();
            Filters = new ConcurrentDictionary<string, List<MailSieveFilterData>>();

            var tenantManager = clientScope.GetService<TenantManager>();
            tenantManager.SetCurrentTenant(Account.TenantId);

            var securityContext = clientScope.GetService<SecurityContext>();
            securityContext.AuthenticateMe(new Guid(Account.UserId));

            _mailInfoDao = clientScope.GetService<MailInfoDao>();
            _mailEnginesFactory = clientScope.GetService<MailEnginesFactory>();
            _indexEngine = clientScope.GetService<IndexEngine>();
            _storageFactory = clientScope.GetService<StorageFactory>();
            _folderEngine = clientScope.GetService<FolderEngine>();
            _userManager = clientScope.GetService<UserManager>();
            _signalrServiceClient = clientScope.GetService<IOptionsSnapshot<SignalrServiceClient>>().Get("mail");
            _redisClient= clientScope.GetService<RedisClient>();
            _log= clientScope.GetService<ILog>();
            

            _log.Name = $"ASC.Mail.ImapSync.Mbox_{mailbox.MailBoxId}";

            crmAvailable = false;// mailbox.IsCrmAvailable(_mailSettings.DefaultApiSchema, Log);

            StopTokenSource = new CancellationTokenSource();

            CancelToken = CancellationTokenSource.CreateLinkedTokenSource(cancelToken, StopTokenSource.Token).Token;

            Imap = new ImapClient(protocolLogger)
            {
                Timeout = _mailSettings.TcpTimeout
            };

            if (_redisClient == null)
            {
                _log.Error($"Ctor: _redisClient == null.");

                DeleteClient?.Invoke(this, EventArgs.Empty);

                return;
            }

            //if (certificatePermit)
            //    Imap.ServerCertificateValidationCallback = (sender, certificate, chain, errors) => true;

            ImapSemaphore = new SemaphoreSlim(1, 1);
            RequestCountSemaphore=new SemaphoreSlim(10,10);

            _deathTimer = new Timer(_deathTimer_Elapsed, this, _mailSettings.ImapClienLifeTimeSecond, System.Threading.Timeout.Infinite);

            ClientIsReadyToWork = false;

            _ = CreateConnection();
        }

        private void _deathTimer_Elapsed(object state)
        {
            _log.Debug($"DeathTimer: Death timer elapsed.");

            DoneToken?.Cancel();

            StopTokenSource?.Cancel();

            DeleteClient?.Invoke(this, EventArgs.Empty);
        }

        private async Task CreateConnection(int workFolder = 1)
        {
            await GetImapSemaphore();

            try
            {
                if (!Imap.IsConnected || !Imap.IsAuthenticated) await LoginImap();

                await LoadFoldersFromIMAP(workFolder);
            }
            catch (Exception ex)
            {
                _log.Error($"CreateConnection: {ex.Message}.");
            }
            finally
            {
                await ReturnImapSemaphore(false);
            }

            await SynchronizeMailFolderFromImap(WorkFolder);

            ClientIsReadyToWork = true;

            Imap.Disconnected += Imap_Disconnected;
        }

        private void Imap_Disconnected(object sender, DisconnectedEventArgs e)
        {
            ClientIsReadyToWork = false;

            _log.Debug($"Imap_Disconnected.");

            Imap.Disconnected -= Imap_Disconnected;
            Imap.Inbox.MessageFlagsChanged -= ImapMessageFlagsChanged;
            Imap.Inbox.CountChanged -= ImapCountChanged;

            if (e.IsRequested)
            {
                _log.Info("Try reconnect to IMAP...");

                _ = CreateConnection();
            }
            else
            {
                _log.Info("DisconnectedEventArgs.IsRequested=false.");

                DoneToken?.Cancel();

                StopTokenSource.Cancel();

                DeleteClient?.Invoke(this, EventArgs.Empty);
            }
        }

        public void Dispose()
        {
            _log.Info("Dispose.");

            ClientIsReadyToWork = false;

            DoneToken?.Cancel();
            StopTokenSource?.Cancel();

            try
            {
                if (Imap.IsConnected)
                {
                    Imap.Disconnected -= Imap_Disconnected;

                    Imap.Disconnect(true);
                }

                Imap?.Dispose();

                StopTokenSource?.Dispose();

                DoneToken?.Dispose();
            }
            catch (Exception ex)
            {
                _log.ErrorFormat("Dispose(Mb_Id={0} Mb_Addres: '{1}') Exception: {2}", Account.MailBoxId,
                    Account.EMail.Address, ex.Message);
            }
        }

        public async Task LoginImap(bool enableUtf8 = true)
        {
            if (Imap.IsAuthenticated) return;

            var secureSocketOptions = SecureSocketOptions.Auto;
            var sslProtocols = SslProtocols.Default;

            switch (Account.Encryption)
            {
                case EncryptionType.StartTLS:
                    secureSocketOptions = SecureSocketOptions.StartTlsWhenAvailable;
                    sslProtocols = SslProtocols.Tls | SslProtocols.Tls11 | SslProtocols.Tls12;
                    break;
                case EncryptionType.SSL:
                    secureSocketOptions = SecureSocketOptions.SslOnConnect;
                    sslProtocols = SslProtocols.Tls | SslProtocols.Tls11 | SslProtocols.Tls12;
                    break;
                case EncryptionType.None:
                    secureSocketOptions = SecureSocketOptions.None;
                    sslProtocols = SslProtocols.None;
                    break;
            }

            _log.DebugFormat("Imap.Connect({0}:{1}, {2})", Account.Server, Account.Port,
                Enum.GetName(typeof(SecureSocketOptions), secureSocketOptions));

            Imap.SslProtocols = sslProtocols;

            if (!Imap.IsConnected) await Imap.ConnectAsync(Account.Server, Account.Port, secureSocketOptions, CancelToken);

            try
            {
                if (enableUtf8 && (Imap.Capabilities & ImapCapabilities.UTF8Accept) != ImapCapabilities.None)
                {
                    _log.Debug("Imap.EnableUTF8");

                    await Imap.EnableUTF8Async(CancelToken);
                }

                if (string.IsNullOrEmpty(Account.OAuthToken))
                {
                    _log.DebugFormat("Imap.Authentication({0})", Account.Account);

                    await Imap.AuthenticateAsync(Account.Account, Account.Password, CancelToken);
                }
                else
                {
                    _log.DebugFormat("Imap.AuthenticationByOAuth({0})", Account.Account);

                    var oauth2 = new SaslMechanismOAuth2(Account.Account, Account.AccessToken);

                    await Imap.AuthenticateAsync(oauth2, CancelToken);
                }

                Imap.Inbox.MessageFlagsChanged += ImapMessageFlagsChanged;
                Imap.Inbox.CountChanged += ImapCountChanged;

                SetMailboxAuthError(false);
            }
            catch (AggregateException aggEx)
            {
                SetMailboxAuthError(true);

                if (aggEx.InnerException != null)
                {
                    throw aggEx.InnerException;
                }
                throw new Exception("LoginImap failed", aggEx);
            }

            _log.Debug("Imap logged in.");
        }

        public async Task LoadFoldersFromIMAP(int workFolder = 1)
        {
            _log.Debug("Load folders from IMAP.");

            if (foldersDictionary == null) foldersDictionary = new Dictionary<IMailFolder, MailFolder>();
            else
            {
                _log.Debug("IMAP folders already loaded.");

                return;
            }

            try
            {
                var rootFolder = await Imap.GetFolderAsync(Imap.PersonalNamespaces[0].Path);

                var subfolders = await GetImapSubFolders(rootFolder);

                var foldersIMAP = subfolders.Where(x => !_mailSettings.SkipImapFlags.Contains(x.Name.ToLowerInvariant()))
                    .Where(x => !x.Attributes.HasFlag(FolderAttributes.NoSelect))
                    .Where(x => !x.Attributes.HasFlag(FolderAttributes.NonExistent))
                    .ToList();

                WorkFolder = Imap.Inbox;

                _log.Debug($"Find {foldersIMAP.Count} folders in IMAP.");

                foreach (var folderIMAP in foldersIMAP)
                {
                    if (CancelToken.IsCancellationRequested) return;

                    var dbFolder = DetectFolder(folderIMAP);

                    if (dbFolder == null)
                    {
                        _log.Debug($"{folderIMAP.Name}-> skipped");
                        continue;
                    }
                    else
                    {
                        _log.Debug($"{folderIMAP.Name}-> ready to work.");
                    }

                    foldersDictionary.Add(folderIMAP, dbFolder);
                }

                _log.Debug($"Loaded {foldersDictionary.Count} folders for work.");
            }
            catch (AggregateException aggEx)
            {
                if (aggEx.InnerException != null)
                {
                    throw aggEx.InnerException;
                }
                throw new Exception("LoadFoldersFromIMAP failed", aggEx);
            }
        }

        private async Task<IEnumerable<IMailFolder>> GetImapSubFolders(IMailFolder folder)
        {
            var result = new List<IMailFolder>();
            try
            {
                result = (await folder.GetSubfoldersAsync(true, CancelToken)).ToList();

                if (result.Any())
                {
                    var resultWithSubfolders = result.Where(x => x.Attributes.HasFlag(FolderAttributes.HasChildren)).ToList();

                    foreach (var subfolder in resultWithSubfolders)
                    {
                        result.AddRange(await GetImapSubFolders(subfolder));
                    }

                    return result;
                }
            }
            catch (Exception ex)
            {
                _log.Error($"GetImapSubFolders: {folder.Name} Exception: {ex.Message}");
            }

            return new List<IMailFolder>();
        }

        public async Task UpdateMessageFlagInDb(int index, IMailFolder imapFolder)
        {
            if (index== 0) return;

            if (WorkFolder != imapFolder)
            {
                imapMessagesToUpdate = new ConcurrentQueue<int>();

                _log.Debug($"UpdateMessageFlagInDb: clear queue couse workfolder changed.");
            }

            imapMessagesToUpdate.Enqueue(index);

            if (!IMAPEventInProgress)
            {
                IMAPEventInProgress = true;
            }
            else return;

            await Task.Delay(2000);

            var indexes = new List<int>();

            try
            {
                while(imapMessagesToUpdate.TryDequeue(out int currenValue))
                {
                    indexes.Add(currenValue);
                }
            }

            catch(Exception ex)
            {
                _log.Error($"UpdateMessageFlagInDb({imapFolder.FullName}, Message.Count={indexes.Count})->{ex.Message}");
            }

            IMAPEventInProgress = false;

            await GetImapSemaphore();

            try
            {
                await OpenFolderWithOutWaiter(imapFolder);

                var imap_messages = await imapFolder.FetchAsync(indexes, SummaryItems);

                FolderType folder = foldersDictionary[imapFolder].Folder;

                var imap_SplittedUids = imap_messages.Select(x => new SplittedUidl(folder, x.UniqueId)).ToList();

                var exp = SimpleMessagesExp.CreateBuilder(Account.TenantId, Account.UserId)
                    .SetMessageUids(imap_SplittedUids.Select(x => x.Uidl).ToList())
                    .SetMailboxId(Account.MailBoxId);

                var db_messages = _mailInfoDao.GetMailInfoList(exp.Build()).ToList();

                foreach (var imap_message in imap_messages)
                {
                    var db_message = db_messages.Where(x => x.Uidl.StartsWith(imap_message.UniqueId.ToString())).FirstOrDefault();

                    if (db_message == null) continue;

                    SynchronizeMessageFlagsFromImap(imap_message, db_message);
                }
            }
            catch (Exception ex)
            {
                _log.Error($"UpdateMessageFlagInDb({imapFolder.FullName}, Message.Count={indexes.Count})->{ex.Message}");
            }
            finally
            {
                await ReturnImapSemaphore();
            }
        }

        public async Task<bool> SynchronizeMailFolderFromImap(IMailFolder imapFolder)
        {
            if (imapFolder == null) return false;

            MailFolder dbFolder = foldersDictionary[imapFolder];

            if (dbFolder == null) return false;

            await GetImapSemaphore();

            try
            {
                await OpenFolderWithOutWaiter(imapFolder);

                var imap_messages = (await imapFolder.FetchAsync(1, -1, SummaryItems));//.OrderBy(x=>x.UniqueId.Id).Take(_mailSettings.ImapMessagePerFolder);

                var imap_SplittedUids = imap_messages.Select(x => new SplittedUidl(dbFolder.Folder, x.UniqueId)).ToList();

                var exp = SimpleMessagesExp.CreateBuilder(Account.TenantId, Account.UserId)
                    .SetMessageUids(imap_SplittedUids.Select(x => x.Uidl).ToList())
                    .SetMailboxId(Account.MailBoxId);

                var db_messages = _mailInfoDao.GetMailInfoList(exp.Build()).ToList();

                _log.Debug($"SynchronizeMailFolderFromImap begin: imap_messages={imap_messages.Count()}, db_messages={db_messages.Count()}");

                foreach (var imap_message in imap_messages)
                {
                    _log.Debug($"SynchronizeMailFolderFromImap: imap_message_Uidl={imap_message.UniqueId.Id.ToString()}, folder={imap_message.Folder.Name}.");

                    var db_message = db_messages.FirstOrDefault(x => x.Uidl == SplittedUidl.ToUidl(dbFolder.Folder, imap_message.UniqueId));

                    if (db_message != null)
                    {
                        SynchronizeMessageFlagsFromImap(imap_message, db_message);

                        if (db_message.Folder != dbFolder.Folder)
                        {
                            _log.Debug($"SynchronizeMailFolderFromImap: imap_message_Uidl={imap_message.UniqueId.Id.ToString()}, setFolderTo={dbFolder.Folder}.");

                            _mailEnginesFactory.MessageEngine.SetFolder(new List<int>() { db_message.Id }, dbFolder.Folder);
                        }

                        db_messages.Remove(db_message);

                        continue;
                    }

                    string md5 = await GetMessageMD5FromIMAP(imap_message, imapFolder);

                    _log.Debug($"SynchronizeMailFolderFromImap: imap_message_Uidl={imap_message.UniqueId.Id.ToString()}, Try find by md5={md5}.");

                    var expMd5 = SimpleMessagesExp.CreateBuilder(Account.TenantId, Account.UserId,true)
                        .SetMd5(md5)
                        .SetMailboxId(Account.MailBoxId);

                    db_message = _mailInfoDao.GetMailInfoList(expMd5.Build()).FirstOrDefault();

                    if (db_message == null)
                    {
                        expMd5 = SimpleMessagesExp.CreateBuilder(Account.TenantId, Account.UserId, true)
                            .SetMd5(md5)
                            .SetMailboxId(Account.MailBoxId);

                        _log.Debug($"SynchronizeMailFolderFromImap: imap_message_Uidl={imap_message.UniqueId.Id.ToString()}, Try find in deleted.");

                        db_message = _mailInfoDao.GetMailInfoList(expMd5.Build()).FirstOrDefault();
                        if (db_message == null)
                        {
                            _log.Debug($"SynchronizeMailFolderFromImap: message {imap_message.UniqueId.Id} not found in DB. Try create.");

                            await CreateMessageInDB(imap_message, imapFolder);

                            continue;
                        }

                    }

                    if (db_message.IsRemoved)
                    {
                        _log.Debug($"SynchronizeMailFolderFromImap: message {imap_message.UniqueId.Id} was remove in DB. md5={expMd5}");

                        var restoreQuery = SimpleMessagesExp.CreateBuilder(Account.TenantId, Account.UserId)
                            .SetMessageId(db_message.Id)
                            .Build();

                        _mailInfoDao.SetFieldValue(restoreQuery, "is_removed", false);

                        _mailEnginesFactory.MessageEngine.Restore(new List<int>() { db_message.Id });
                    }

                    if (db_message.Folder != dbFolder.Folder)
                    {
                        _log.Debug($"SynchronizeMailFolderFromImap: imap_message_Uidl={imap_message.UniqueId.Id.ToString()}, setFolderTo={dbFolder.Folder}.");

                        _mailEnginesFactory.MessageEngine.SetFolder(new List<int>() { db_message.Id }, dbFolder.Folder);
                    }

                    var updateQuery = SimpleMessagesExp.CreateBuilder(Account.TenantId, Account.UserId)
                            .SetMessageId(db_message.Id)
                            .Build();

                    _log.Debug($"SynchronizeMailFolderFromImap: imap_message_Uidl={imap_message.UniqueId.Id.ToString()}, setUidlTo={SplittedUidl.ToUidl(dbFolder.Folder, imap_message.UniqueId)}.");

                    _mailInfoDao.SetFieldValue(updateQuery, "Uidl", SplittedUidl.ToUidl(dbFolder.Folder, imap_message.UniqueId));

                    SynchronizeMessageFlagsFromImap(imap_message, db_message);
                }

                var messageToRemove = db_messages.Select(x => x.Id).ToList();

                if (messageToRemove.Any()) _mailEnginesFactory.MessageEngine.SetRemoved(messageToRemove);

            }
            catch (Exception ex)
            {
                _log.Error($"SynchronizeMailFolderFromImap(IMailFolder={imapFolder.Name}, MailFolder={dbFolder.Name})->{ex.Message}");

                return false;
            }
            finally
            {
                await ReturnImapSemaphore();
            }

            return true;
        }

        public bool SynchronizeMessageFlagsFromImap(IMessageSummary imap_message, MailInfo db_message)
        {
            if (imap_message == null || db_message == null) return false;
            try
            {
                _log.Debug("SynchronizeMessageFlagsFromImap begin.");
                _log.Debug($"SynchronizeMessageFlagsFromImap: imap_message_Uidl={imap_message.UniqueId.Id.ToString()}, flag={imap_message.Flags.Value.ToString()}.");
                _log.Debug($"SynchronizeMessageFlagsFromImap: db_message={db_message.Uidl}, folder={db_message.Folder}, IsRemoved={db_message.IsRemoved}.");
                bool unread = !imap_message.Flags.Value.HasFlag(MessageFlags.Seen);
                bool important = imap_message.Flags.Value.HasFlag(MessageFlags.Flagged);
                bool removed = imap_message.Flags.Value.HasFlag(MessageFlags.Deleted);

                if (db_message.IsNew ^ unread) _mailEnginesFactory.MessageEngine.SetUnread(new List<int>() { db_message.Id }, unread, true);
                if (db_message.Importance ^ important) _mailEnginesFactory.MessageEngine.SetImportant(new List<int>() { db_message.Id }, important);
                if (removed) _mailEnginesFactory.MessageEngine.SetRemoved(new List<int>() { db_message.Id });
                _log.Debug("SynchronizeMessageFlagsFromImap finish.");
                return true;
            }
            catch (Exception ex)
            {
                _log.Error($"SynchronizeMessageFlagsFromImap({imap_message.UniqueId},{db_message})->{ex.Message}");

                return false;
            }
        }

        public async Task<bool> MoveMessageInImap(List<SplittedUidl> uidlListSplitted, int destination)
        {
            if (uidlListSplitted.Count == 0) return false;

            await GetImapSemaphore();

            try
            {
                var imapFolder = foldersDictionary.Where(x => x.Value.Folder == uidlListSplitted[0].FolderType).Select(x => x.Key).FirstOrDefault();

                if (imapFolder == null)
                {
                    _log.Debug($"MoveMessageInImap->Don`t found source folder: {uidlListSplitted[0].FolderType}.");

                    return false;
                }

                var imapFolderDestination = foldersDictionary.Where(x => x.Value.Folder == (FolderType)destination).Select(x => x.Key).FirstOrDefault();

                if (imapFolderDestination == null)
                {
                    _log.Debug($"MoveMessageInImap->Don`t found destination folder: {imapFolderDestination}.");

                    return false;
                }

                await OpenFolderWithOutWaiter(imapFolder);

                Imap.Inbox.CountChanged -= ImapCountChanged;
                Imap.Inbox.MessageFlagsChanged -= ImapMessageFlagsChanged;

                var returnedUidl = await imapFolder.MoveToAsync(uidlListSplitted.Select(x => x.UniqueId).ToList(), imapFolderDestination);

                var exp = SimpleMessagesExp.CreateBuilder(Account.TenantId, Account.UserId)
                    .SetMessageUids(uidlListSplitted.Select(x => x.Uidl).ToList())
                    .SetMailboxId(Account.MailBoxId);

                var db_messages = _mailInfoDao.GetMailInfoList(exp.Build()).ToList();

                foreach (var item in returnedUidl)
                {
                    var db_message = db_messages.FirstOrDefault(x => x.Uidl.StartsWith(item.Key.ToString()));

                    if (db_message == null)
                    {
                        _log.Debug($"MoveMessageInImap->Don`t found in DB: {imapFolder}-{item.Key} move to {imapFolderDestination}-{item.Value}.");

                        continue;
                    }

                    var updateQuery = SimpleMessagesExp.CreateBuilder(Account.TenantId, Account.UserId)
                        .SetMessageId(db_message.Id)
                        .Build();

                    _mailInfoDao.SetFieldValue(updateQuery, "uidl", SplittedUidl.ToUidl((FolderType)destination, item.Value));

                    _log.Debug($"MoveMessageInImap->Update in DB: {imapFolder}-{item.Key} move to {imapFolderDestination}-{item.Value}.");
                }
            }
            catch (Exception ex)
            {
                _log.Error($"MoveMessageInImap(messageCount={uidlListSplitted.Count}, destination={destination})->{ex.Message}");

                return false;
            }
            finally
            {
                Imap.Inbox.CountChanged += ImapCountChanged;
                Imap.Inbox.MessageFlagsChanged += ImapMessageFlagsChanged;

                await ReturnImapSemaphore();
            }

            return true;
        }

        public async Task<bool> SetFlagsInImap(List<SplittedUidl> uidlListSplitted, MailUserAction mailUserAction)
        {
            if (uidlListSplitted.Count == 0) return false;

            await GetImapSemaphore();

            try
            {
                var folders = uidlListSplitted.GroupBy(x => x.FolderType).Select(y => y.Key).ToList();

                foreach (var folder in folders)
                {
                    var uids = uidlListSplitted.Where(x => x.FolderType == folder).Select(x => x.UniqueId).ToList();

                    if (uids.Count == 0) continue;

                    IMailFolder imapFolder = foldersDictionary.Where(x => x.Value.Folder == folder).Select(x => x.Key).FirstOrDefault();

                    if (imapFolder == null) continue;

                    await SetFlagsInImap(imapFolder, uids, mailUserAction);
                }
            }
            catch (Exception ex)
            {
                _log.Error($"SetMessageFlagIMAP->{Account.MailBoxId}, {mailUserAction}->{ex.Message}");

                return false;
            }
            finally
            {
                await ReturnImapSemaphore();
            }

            return true;
        }

        public async Task SetFlagsInImap(IMailFolder imapFolder, List<UniqueId> uids, MailUserAction mailUserAction)
        {
            try
            {
                await OpenFolderWithOutWaiter(imapFolder);

                switch (mailUserAction)
                {
                    case MailUserAction.MoveTo:
                    case MailUserAction.Nothing:
                    case MailUserAction.StartImapClient:
                        break;
                    case MailUserAction.SetAsRead:
                        await imapFolder.AddFlagsAsync(uids, MessageFlags.Seen, true);
                        break;
                    case MailUserAction.SetAsUnread:
                        await imapFolder.RemoveFlagsAsync(uids, MessageFlags.Seen, true);
                        break;
                    case MailUserAction.SetAsImportant:
                        await imapFolder.AddFlagsAsync(uids, MessageFlags.Flagged, true);
                        break;
                    case MailUserAction.SetAsNotImpotant:
                        await imapFolder.RemoveFlagsAsync(uids, MessageFlags.Flagged, true);
                        break;
                    case MailUserAction.SetAsDeleted:
                        await imapFolder.AddFlagsAsync(uids, MessageFlags.Deleted, true);
                        break;
                }
                _log.Debug($"SetFlagsInImap. In {imapFolder} set {mailUserAction} for {uids.Count} messages.");
            }
            catch (Exception ex)
            {
                _log.Error($"SetFlagsInImap. In {imapFolder} set {mailUserAction} for {uids.Count} messages. {ex.Message}"); ;
            }
        }

        public List<MailInfo> GetMessageFromDB(MailFolder dbFolder, string md5 = "")
        {
            try
            {
                var exp = SimpleMessagesExp.CreateBuilder(Account.TenantId, Account.UserId)
                    .SetMailboxId(Account.MailBoxId)
                    .SetFolder((int)dbFolder.Folder);

                if (!String.IsNullOrEmpty(md5))
                {
                    exp = exp.SetMd5(md5);
                }

                return _mailInfoDao.GetMailInfoList(exp.Build()).ToList();
            }
            catch (Exception ex)
            {
                _log.Error($"GetMessageFromDB(MailFolder={dbFolder.Name})->{ex.Message}");

                return new List<MailInfo>();
            }
        }

        private async Task<bool> CreateMessageInDB(IMessageSummary imap_message, IMailFolder imapFolder)
        {
            bool result = true;

            Stopwatch watch = null;

            if (_mailSettings.CollectStatistics)
            {
                watch = new Stopwatch();
                watch.Start();
            }

            try
            {
                var message = await imapFolder.GetMessageAsync(imap_message.UniqueId, CancelToken);

                message.FixDateIssues(imap_message?.InternalDate, _log);

                bool unread = false, important = false;

                if ((imap_message != null) && imap_message.Flags.HasValue)
                {
                    unread = !imap_message.Flags.Value.HasFlag(MessageFlags.Seen);
                    important = imap_message.Flags.Value.HasFlag(MessageFlags.Flagged);
                }

                message.FixEncodingIssues(_log);

                var uid = imap_message.UniqueId.ToString();
                var folder = foldersDictionary[imapFolder];
                var uidl = string.Format("{0}-{1}", uid, (int)folder.Folder);

                _log.Info($"Get message (UIDL: '{uidl}', MailboxId = {Account.MailBoxId}, Address = '{Account.EMail}')");

                var messageDB = _mailEnginesFactory.MessageEngine.Save(Account, message, uidl, folder, null, unread, _log);

                if (messageDB == null || messageDB.Id <= 0)
                {
                    _log.Debug("CreateMessageInDB: failed.");
                    result = false;
                    return result;
                }

                _log.Info($"Message saved (id: {messageDB.Id}, From: '{messageDB.From}', Subject: '{messageDB.Subject}', Unread: {messageDB.IsNew})");

                _log.Info("DoOptionalOperations->START");

                DoOptionalOperations(messageDB, message, Account, folder);

                _log.Info("DoOptionalOperations->END");
            }
            catch (Exception ex)
            {
                _log.Error($"CreateMessageInDB Exception:{ex.Message}");

                result = false;
            }
            finally
            {
                if (_mailSettings.CollectStatistics && watch != null)
                {
                    watch.Stop();

                    LogStat("CreateMessageInDB", watch.Elapsed, result);
                }

                _log.Debug($"CreateMessageInDB time={watch.Elapsed.TotalMilliseconds} ms.");
            }

            return result;
        }

        public async Task<string> GetMessageMD5FromIMAP(IMessageSummary message, IMailFolder imapFolder)
        {
            await OpenFolderWithOutWaiter(imapFolder);

            var mimeMessage = await imapFolder.GetMessageAsync(message.UniqueId);

            var md5 = string.Format("{0}|{1}|{2}|{3}",
                mimeMessage.From.Mailboxes.Any() ? mimeMessage.From.Mailboxes.First().Address : "",
                mimeMessage.Subject,
                mimeMessage.Date.UtcDateTime,
                mimeMessage.MessageId).GetMd5();

            return md5;
        }

        private MailFolder DetectFolder(IMailFolder folder)
        {
            var folderName = folder.Name.ToLowerInvariant();

            if (_mailSettings.SkipImapFlags != null &&
                _mailSettings.SkipImapFlags.Any() &&
                _mailSettings.SkipImapFlags.Contains(folderName))
            {
                return null;
            }

            FolderType folderId;

            if ((folder.Attributes & FolderAttributes.Inbox) != 0)
            {
                return new MailFolder(FolderType.Inbox, folder.Name);
            }
            if ((folder.Attributes & FolderAttributes.Sent) != 0)
            {
                return new MailFolder(FolderType.Sent, folder.Name);
            }
            if ((folder.Attributes & FolderAttributes.Junk) != 0)
            {
                return new MailFolder(FolderType.Spam, folder.Name);
            }
            if ((folder.Attributes &
                 (FolderAttributes.All |
                  FolderAttributes.NoSelect |
                  FolderAttributes.NonExistent |
                  FolderAttributes.Trash |
                  FolderAttributes.Archive |
                  FolderAttributes.Drafts |
                  FolderAttributes.Flagged)) != 0)
            {
                return null; // Skip folders
            }

            if (_mailSettings.ImapFlags != null &&
                _mailSettings.ImapFlags.Any() &&
                _mailSettings.ImapFlags.ContainsKey(folderName))
            {
                folderId = (FolderType)_mailSettings.ImapFlags[folderName];
                return new MailFolder(folderId, folder.Name);
            }

            if (_mailSettings.SpecialDomainFolders.Any() &&
                _mailSettings.SpecialDomainFolders.ContainsKey(Account.Server))
            {
                var domainSpecialFolders = _mailSettings.SpecialDomainFolders[Account.Server];

                if (domainSpecialFolders.Any() &&
                    domainSpecialFolders.ContainsKey(folderName))
                {
                    var info = domainSpecialFolders[folderName];
                    return info.skip ? null : new MailFolder(info.folder_id, folder.Name);
                }
            }

            if (_mailSettings.DefaultFolders == null || !_mailSettings.DefaultFolders.ContainsKey(folderName))
                return new MailFolder(FolderType.Inbox, folder.Name, new[] { folder.FullName });

            folderId = (FolderType)_mailSettings.DefaultFolders[folderName];
            return new MailFolder(folderId, folder.Name);
        }

        private async Task OpenFolderWithOutWaiter(IMailFolder imapFolder)
        {
            if (imapFolder.IsOpen) return;

            try
            {
                await imapFolder.OpenAsync(FolderAccess.ReadWrite);

                _log.Debug($"Imap Folder {imapFolder.Name} was open.");
            }
            catch (Exception ex)
            {
                _log.Error($"Imap Folder { imapFolder.Name} didn`t open. {ex.Message}");
            }
        }

        public async Task GetImapSemaphore()
        {
            await RequestCountSemaphore.WaitAsync();

            DoneToken?.Cancel();

            await ImapSemaphore.WaitAsync();
        }

        public async Task ReturnImapSemaphore(bool goToIdle=true)
        {
            RequestCountSemaphore.Release();

            _log.Debug($"ReturnImapSemaphore {RequestCountSemaphore.CurrentCount}.");

            ImapSemaphore.Release();

            if (RequestCountSemaphore.CurrentCount != 10)
            {
                return;
            }

            if (!goToIdle) return;

            DoneToken?.Cancel();

            await ImapSemaphore.WaitAsync();

            try
            {
                await OpenFolderWithOutWaiter(WorkFolder);

                if (Imap.Capabilities.HasFlag(ImapCapabilities.Idle))
                {
                    _deathTimer.Change(_mailSettings.ImapClienLifeTimeSecond, -1);

                    DoneToken = new CancellationTokenSource(new TimeSpan(0, 10, 0));

                    _log.Debug($"Go to Idle.");

                    Imap.Idle(DoneToken.Token);
                }
                else
                {
                    await Task.Delay(new TimeSpan(0, 10, 0));
                    await Imap.NoOpAsync();
                }
            }
            catch (Exception ex)
            {
                _log.Error($"ReturnImapSemaphore, Error:{ex.Message}");
            }
            finally
            {
                DoneToken?.Dispose();

                DoneToken = null;

                if (ImapSemaphore.CurrentCount == 0)
                {
                    ImapSemaphore.Release();

                    _log.Debug($"Return from ReturnImapSemaphore, ImapSemaphore release.");
                }
                else
                {
                    _log.Debug($"Return from ReturnImapSemaphore without ImapSemaphore release.");
                }
            }
        }

        private void LogStat(string method, TimeSpan duration, bool failed)
        {
            if (!_mailSettings.CollectStatistics)
                return;

            _log.DebugWithProps(method,
                new KeyValuePair<string, object>("duration", duration.TotalMilliseconds),
                new KeyValuePair<string, object>("mailboxId", Account.MailBoxId),
                new KeyValuePair<string, object>("address", Account.EMail.ToString()),
                new KeyValuePair<string, object>("isFailed", failed));
        }

        private void DoOptionalOperations(MailMessageData message, MimeMessage mimeMessage, MailBoxData mailbox, MailFolder folder)
        {
            try
            {
                var tagIds = new List<int>();

                if (folder.Tags.Any())
                {
                    _log.Debug("DoOptionalOperations->GetOrCreateTags()");

                    tagIds = _mailEnginesFactory.TagEngine.GetOrCreateTags(mailbox.TenantId, mailbox.UserId, folder.Tags);
                }

                _log.Debug("DoOptionalOperations->IsCrmAvailable()");

                //if (IsCrmAvailable(mailbox, log))
                //{
                //    log.Debug("DoOptionalOperations->GetCrmTags()");

                //    var crmTagIds = mailFactory.TagEngine.GetCrmTags(message.FromEmail);

                //    if (crmTagIds.Any())
                //    {
                //        if (tagIds == null)
                //            tagIds = new List<int>();

                //        tagIds.AddRange(crmTagIds.Select(t => t.Id));
                //    }
                //}

                if (tagIds.Any())
                {
                    if (message.TagIds == null || !message.TagIds.Any())
                        message.TagIds = tagIds;
                    else
                        message.TagIds.AddRange(tagIds);

                    message.TagIds = message.TagIds.Distinct().ToList();
                }

                _log.Debug("DoOptionalOperations->AddMessageToIndex()");

                var wrapper = message.ToMailWrapper(mailbox.TenantId, new Guid(mailbox.UserId));

                _mailEnginesFactory.IndexEngine.Add(wrapper);

                foreach (var tagId in tagIds)
                {
                    try
                    {
                        _log.DebugFormat("DoOptionalOperations->SetMessagesTag(tagId: {0})", tagId);

                        _mailEnginesFactory.TagEngine.SetMessagesTag(new List<int> { message.Id }, tagId);
                    }
                    catch (Exception e)
                    {
                        _log.ErrorFormat(
                            "SetMessagesTag(tenant={0}, userId='{1}', messageId={2}, tagid = {3}) Exception:\r\n{4}\r\n",
                            mailbox.TenantId, mailbox.UserId, message.Id, e.ToString(),
                            tagIds != null ? string.Join(",", tagIds) : "null");
                    }
                }

                _log.Debug("DoOptionalOperations->AddRelationshipEventForLinkedAccounts()");

                _mailEnginesFactory.CrmLinkEngine.AddRelationshipEventForLinkedAccounts(mailbox, message, _mailSettings.DefaultApiSchema);

                _log.Debug("DoOptionalOperations->SaveEmailInData()");

                _mailEnginesFactory.EmailInEngine.SaveEmailInData(mailbox, message, _mailSettings.DefaultApiSchema);

                _log.Debug("DoOptionalOperations->SendAutoreply()");

                _mailEnginesFactory.AutoreplyEngine.SendAutoreply(mailbox, message, _mailSettings.DefaultApiSchema, _log);

                _log.Debug("DoOptionalOperations->UploadIcsToCalendar()");

                if (folder.Folder != Enums.FolderType.Spam)
                {
                    _mailEnginesFactory
                        .CalendarEngine
                        .UploadIcsToCalendar(mailbox, message.CalendarId, message.CalendarUid, message.CalendarEventIcs,
                            message.CalendarEventCharset, message.CalendarEventMimeType, mailbox.EMail.Address,
                            _mailSettings.DefaultApiSchema);
                }

                if (_mailSettings.SaveOriginalMessage)
                {
                    _log.Debug("DoOptionalOperations->StoreMailEml()");
                    StoreMailEml(mailbox.TenantId, mailbox.UserId, message.StreamId, mimeMessage);
                }

                _log.Debug("DoOptionalOperations->ApplyFilters()");

                var filters = GetFilters(_mailEnginesFactory, _log);

                _mailEnginesFactory.FilterEngine.ApplyFilters(message, mailbox, folder, filters);

                _log.Debug("DoOptionalOperations->NotifySignalrIfNeed()");

                SendUnreadUser();
            }
            catch (Exception ex)
            {
                _log.ErrorFormat("DoOptionalOperations() Exception:\r\n{0}\r\n", ex.ToString());
            }
        }

        private void SendUnreadUser()
        {
            try
            {
                var mailFolderInfos = _folderEngine.GetFolders();

                var count = (from mailFolderInfo in mailFolderInfos
                             where mailFolderInfo.id == FolderType.Inbox
                             select mailFolderInfo.unreadMessages)
                    .FirstOrDefault();

                if (Account.UserId != Constants.LostUser.ID.ToString())
                {
                    _signalrServiceClient.SendUnreadUser(Account.TenantId, Account.UserId, count);
                }
            }
            catch (Exception e)
            {
                _log.ErrorFormat("Unknown Error. {0}, {1}", e.ToString(),
                    e.InnerException != null ? e.InnerException.Message : string.Empty);
            }
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

        public string StoreMailEml(int tenant, string user, string streamId, MimeMessage message)
        {
            if (message == null)
                return string.Empty;

            // Using id_user as domain in S3 Storage - allows not to add quota to tenant.
            var savePath = MailStoragePathCombiner.GetEmlKey(user, streamId);

            var storage = _storageFactory.GetMailStorage(tenant);

            try
            {
                using (var stream = new MemoryStream())
                {
                    message.WriteTo(stream);

                    var res = storage.Save(savePath, stream, MailStoragePathCombiner.EML_FILE_NAME).ToString();

                    _log.InfoFormat("StoreMailEml() tenant='{0}', user_id='{1}', save_eml_path='{2}' Result: {3}", tenant, user, savePath, res);

                    return res;
                }
            }
            catch (Exception ex)
            {
                _log.ErrorFormat("StoreMailEml Exception: {0}", ex.ToString());
            }

            return string.Empty;
        }

        private void SetMailboxAuthError(bool state)
        {
            if (Account.AuthErrorDate.HasValue) return;
            try
            {
                if (state)
                {
                    Account.AuthErrorDate = DateTime.UtcNow;
                    _mailEnginesFactory.MailboxEngine.SetMaiboxAuthError(Account.MailBoxId, DateTime.UtcNow);
                }
                else
                {
                    Account.AuthErrorDate = null;
                    _mailEnginesFactory.MailboxEngine.SetMaiboxAuthError(Account.MailBoxId, null);
                }
            }
            catch (Exception ex)
            {
                _log.Error($"SetMailboxAuthError({Account.EMail}): Exception: {ex.Message}");
            }
        }
    }
}