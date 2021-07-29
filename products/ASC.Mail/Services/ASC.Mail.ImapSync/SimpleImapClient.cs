﻿using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Security.Authentication;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

using ASC.Common.Logging;
using ASC.Core.Common.Caching;
using ASC.Mail.Configuration;
using ASC.Mail.Enums;
using ASC.Mail.Models;

using MailKit;
using MailKit.Net.Imap;
using MailKit.Security;

namespace ASC.Mail.ImapSync
{
    public class SimpleImapClient : IDisposable
    {
        private ImapClient imap;

        private ConcurrentQueue<CashedMailUserAction> actionsToImap;
        private ConcurrentQueue<ImapAction> actionsFromImap;

        private readonly ILog _log;
        private readonly MailSettings _mailSettings;
        private readonly MailBoxData Account;

        private CancellationTokenSource DoneToken { get; set; }
        private CancellationToken CancelToken { get; set; }
        private CancellationTokenSource StopTokenSource { get; set; }

        private List<IMailFolder> foldersList;
        public List<IMessageSummary> MessagesList { get; private set; }

        public IMailFolder WorkFolder;

        public ClientState State { get; private set; }

        private void ImapMessageFlagsChanged(object sender, MessageFlagsChangedEventArgs e)
        {
            if (sender is IMailFolder imap_folder)
            {
                IMessageSummary messageSummary = MessagesList.FirstOrDefault(x => x.Index == e.Index);

                if (messageSummary == null) return;

                if(messageSummary.Flags.HasValue)
                {
                    if (e.Flags == messageSummary.Flags) return;

                    bool oldSeen = messageSummary.Flags.Value.HasFlag(MessageFlags.Seen);
                    bool newSeen = e.Flags.HasFlag(MessageFlags.Seen);

                    if(oldSeen!=newSeen)
                    {
                        ImapAction imapAction = new ImapAction()
                        {
                            FolderAction = oldSeen ? ImapAction.Action.SetAsUnread : ImapAction.Action.SetAsRead,
                             Folder= imap_folder,
                              UniqueId= messageSummary.UniqueId

                        };
                        actionsFromImap.Enqueue(imapAction);
                    }

                    bool oldImportant = messageSummary.Flags.Value.HasFlag(MessageFlags.Flagged);
                    bool newImportant = e.Flags.HasFlag(MessageFlags.Flagged);

                    if (oldImportant != newImportant)
                    {
                        ImapAction imapAction = new ImapAction()
                        {
                            FolderAction = oldImportant ? ImapAction.Action.SetAsNotImpotant : ImapAction.Action.SetAsImportant,
                            Folder = imap_folder,
                            UniqueId = messageSummary.UniqueId

                        };
                        actionsFromImap.Enqueue(imapAction);
                    }
                }
            }
        }

        public SimpleImapClient(MailBoxData mailbox, CancellationToken cancelToken, MailSettings mailSettings, ILog log)
        {
            Account = mailbox;
            _mailSettings = mailSettings;
            _log = log;

            _log.Name = $"ASC.Mail.SimpleClient.Mbox_{mailbox.MailBoxId}";

            State = ClientState.Creating;

            var protocolLogger = !string.IsNullOrEmpty(_mailSettings.ProtocolLogPath)
                ? (IProtocolLogger)
                    new ProtocolLogger(_mailSettings.ProtocolLogPath + $"\\imap_{Account.MailBoxId}_{Thread.CurrentThread.ManagedThreadId}.log", true)
                : new NullProtocolLogger();

            StopTokenSource = new CancellationTokenSource();

            CancelToken = CancellationTokenSource.CreateLinkedTokenSource(cancelToken, StopTokenSource.Token).Token;

            imap = new ImapClient(protocolLogger)
            {
                Timeout = _mailSettings.TcpTimeout
            };
        }

        public async Task<List<IMailFolder>> ConnectToServer(IMailFolder folder=null)
        {
            if (!(await Authenticate())) return null;

            imap.Disconnected += Imap_Disconnected;

            await TryChangeFolder(imap.Inbox);

            await LoadFoldersFromIMAP();

            return foldersList;
        }

        public bool TrySendToIMAP(CashedMailUserAction actionToIMAP)
        {
            actionsToImap.Enqueue(actionToIMAP);

            return true;
        }

        public async Task<List<IMessageSummary>> TryChangeFolder(IMailFolder newWorkFolder)
        {
            WorkFolder.MessageFlagsChanged -= ImapMessageFlagsChanged;

            WorkFolder = newWorkFolder;

            await OpenFolder(WorkFolder);

            MessagesList = (await WorkFolder.FetchAsync(1, -1, MessageSummaryItems.UniqueId | MessageSummaryItems.Flags)).ToList();//.OrderBy(x=>x.UniqueId.Id).Take(_mailSettings.ImapMessagePerFolder);

            WorkFolder.MessageFlagsChanged += ImapMessageFlagsChanged;

            return MessagesList;
        }

        private void Imap_Disconnected(object sender, DisconnectedEventArgs e)
        {
            _log.Debug($"Imap_Disconnected.");

            if (e.IsRequested)
            {
                _log.Info("Try reconnect to IMAP...");

                //_ = CreateConnection();
            }
            else
            {
                _log.Info("DisconnectedEventArgs.IsRequested=false.");

                DoneToken?.Cancel();

                StopTokenSource.Cancel();
            }
        }

        public async Task<bool> Authenticate(bool enableUtf8 = true)
        {
            if (imap.IsAuthenticated) return true;

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

            imap.SslProtocols = sslProtocols;

            if (!imap.IsConnected) await imap.ConnectAsync(Account.Server, Account.Port, secureSocketOptions, CancelToken);

            try
            {
                if (enableUtf8 && (imap.Capabilities & ImapCapabilities.UTF8Accept) != ImapCapabilities.None)
                {
                    _log.Debug("Imap.EnableUTF8");

                    await imap.EnableUTF8Async(CancelToken);
                }

                if (string.IsNullOrEmpty(Account.OAuthToken))
                {
                    _log.DebugFormat("Imap.Authentication({0})", Account.Account);

                    await imap.AuthenticateAsync(Account.Account, Account.Password, CancelToken);
                }
                else
                {
                    _log.DebugFormat("Imap.AuthenticationByOAuth({0})", Account.Account);

                    var oauth2 = new SaslMechanismOAuth2(Account.Account, Account.AccessToken);

                    await imap.AuthenticateAsync(oauth2, CancelToken);
                }
            }
            catch (Exception ex)
            {
                _log.Error($"Imap.Authentication Error: {ex.Message}");
                return false;
            }

            _log.Debug("Imap logged in.");

            return true;
        }

        public async Task LoadFoldersFromIMAP(int workFolder = 1)
        {
            _log.Debug("Load folders from IMAP.");

            if (foldersList == null)
            {
                foldersList = new List<IMailFolder>();
            }
            else
            {
                _log.Debug("IMAP folders already loaded.");

                return;
            }

            try
            {
                var rootFolder = await imap.GetFolderAsync(imap.PersonalNamespaces[0].Path);

                var subfolders = await GetImapSubFolders(rootFolder);

                var foldersIMAP = subfolders.Where(x => !_mailSettings.SkipImapFlags.Contains(x.Name.ToLowerInvariant()))
                    .Where(x => !x.Attributes.HasFlag(FolderAttributes.NoSelect))
                    .Where(x => !x.Attributes.HasFlag(FolderAttributes.NonExistent))
                    .ToList();

                WorkFolder = imap.Inbox;

                _log.Debug($"Find {foldersIMAP.Count} folders in IMAP.");


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

        private async Task OpenFolder(IMailFolder imapFolder)
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

        private async Task SetIdle()
        {
            try
            {
                await OpenFolder(WorkFolder);

                if (imap.Capabilities.HasFlag(ImapCapabilities.Idle))
                {
                    DoneToken = new CancellationTokenSource(new TimeSpan(0, 10, 0));

                    _log.Debug($"Go to Idle.");

                    imap.Idle(DoneToken.Token);
                }
                else
                {
                    await Task.Delay(new TimeSpan(0, 10, 0));
                    await imap.NoOpAsync();
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
            }
        }

        public async Task<bool> MoveMessageInImap(List<SplittedUidl> uidlListSplitted, int destination)
        {
            //if (uidlListSplitted.Count == 0) return false;

            //try
            //{
            //    var imapFolder = foldersDictionary.Where(x => x.Value.Folder == uidlListSplitted[0].FolderType).Select(x => x.Key).FirstOrDefault();

            //    if (imapFolder == null)
            //    {
            //        _log.Debug($"MoveMessageInImap->Don`t found source folder: {uidlListSplitted[0].FolderType}.");

            //        return false;
            //    }

            //    var imapFolderDestination = foldersDictionary.Where(x => x.Value.Folder == (FolderType)destination).Select(x => x.Key).FirstOrDefault();

            //    if (imapFolderDestination == null)
            //    {
            //        _log.Debug($"MoveMessageInImap->Don`t found destination folder: {imapFolderDestination}.");

            //        return false;
            //    }

            //    await OpenFolderWithOutWaiter(imapFolder);

            //    Imap.Inbox.CountChanged -= ImapCountChanged;
            //    Imap.Inbox.MessageFlagsChanged -= ImapMessageFlagsChanged;

            //    var returnedUidl = await imapFolder.MoveToAsync(uidlListSplitted.Select(x => x.UniqueId).ToList(), imapFolderDestination);

            //    var exp = SimpleMessagesExp.CreateBuilder(Account.TenantId, Account.UserId)
            //        .SetMessageUids(uidlListSplitted.Select(x => x.Uidl).ToList())
            //        .SetMailboxId(Account.MailBoxId);

            //    var db_messages = _mailInfoDao.GetMailInfoList(exp.Build()).ToList();

            //    foreach (var item in returnedUidl)
            //    {
            //        var db_message = db_messages.FirstOrDefault(x => x.Uidl.StartsWith(item.Key.ToString()));

            //        if (db_message == null)
            //        {
            //            _log.Debug($"MoveMessageInImap->Don`t found in DB: {imapFolder}-{item.Key} move to {imapFolderDestination}-{item.Value}.");

            //            continue;
            //        }

            //        var updateQuery = SimpleMessagesExp.CreateBuilder(Account.TenantId, Account.UserId)
            //            .SetMessageId(db_message.Id)
            //            .Build();

            //        _mailInfoDao.SetFieldValue(updateQuery, "uidl", SplittedUidl.ToUidl((FolderType)destination, item.Value));

            //        _log.Debug($"MoveMessageInImap->Update in DB: {imapFolder}-{item.Key} move to {imapFolderDestination}-{item.Value}.");
            //    }
            //}
            //catch (Exception ex)
            //{
            //    _log.Error($"MoveMessageInImap(messageCount={uidlListSplitted.Count}, destination={destination})->{ex.Message}");

            //    return false;
            //}
            //finally
            //{
            //    Imap.Inbox.CountChanged += ImapCountChanged;
            //    Imap.Inbox.MessageFlagsChanged += ImapMessageFlagsChanged;

            //    _ = ReturnImapSemaphore();
            //}

            return true;
        }

        public async Task<bool> SetFlagsInImap(IMailFolder folder, List<UniqueId> uniqueIds, ImapAction.Action action)
        {
            if (folder == null) return false;
            if (uniqueIds.Count == 0) return false;

            try
            {
                await OpenFolder(folder);

                switch (action)
                {
                    case ImapAction.Action.SetAsRead:
                        await folder.AddFlagsAsync(uniqueIds, MessageFlags.Seen, true);
                        break;
                    case ImapAction.Action.SetAsUnread:
                        await folder.RemoveFlagsAsync(uniqueIds, MessageFlags.Seen, true);
                        break;
                    case ImapAction.Action.SetAsImportant:
                        await folder.AddFlagsAsync(uniqueIds, MessageFlags.Flagged, true);
                        break;
                    case ImapAction.Action.SetAsNotImpotant:
                        await folder.RemoveFlagsAsync(uniqueIds, MessageFlags.Flagged, true);
                        break;

                }
                _log.Debug($"SetFlagsInImap. In {folder} set {action} for {uniqueIds.Count} messages.");

            }
            catch (Exception ex)
            {
                _log.Error($"SetMessageFlagIMAP->{folder.Name}, {action}->{ex.Message}");

                return false;
            }

            return true;
        }

        public void Dispose()
        {
            throw new NotImplementedException();
        }
    }
}
