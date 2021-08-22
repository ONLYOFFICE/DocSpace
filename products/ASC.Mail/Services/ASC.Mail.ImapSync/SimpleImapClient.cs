using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Security.Authentication;
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

using MimeKit;

namespace ASC.Mail.ImapSync
{
    public class SimpleImapClient : IDisposable
    {
        private ImapClient imap;

        private ConcurrentQueue<Task> asyncTasks;

        public Task curentTask { get; private set; }

        private readonly ILog _log;
        private readonly MailSettings _mailSettings;
        private readonly MailBoxData Account;

        public event EventHandler<ImapAction> NewActionFromImap;
        public event EventHandler<(MimeMessage, MessageDescriptor)> NewMessage;
        public event EventHandler<string> DeleteMessage;
        public event EventHandler MessagesListUpdated;
        public event EventHandler WorkFoldersChanged;
        public event EventHandler KillMe;

        private CancellationTokenSource DoneToken { get; set; }
        private CancellationToken CancelToken { get; set; }
        private CancellationTokenSource StopTokenSource { get; set; }

        public List<IMailFolder> foldersList { get; private set; }
        public List<MessageDescriptor> MessagesList { get; private set; }

        public IMailFolder WorkFolder;

        #region Event from Imap handlers

        private void ImapMessageFlagsChanged(object sender, MessageFlagsChangedEventArgs e)
        {
            if (sender is IMailFolder imap_folder)
            {
                _log.Debug($"ImapMessageFlagsChanged {WorkFolder.Name} index={e.Index}");

                MessageDescriptor messageSummary = MessagesList.FirstOrDefault(x => x.Index == e.Index);

                if (messageSummary == null) return;

                if (messageSummary.Flags.HasValue)
                {
                    CompareFlags(imap_folder, messageSummary, e.Flags);
                }
            }
        }

        private void ImapFolderCountChanged(object sender, EventArgs e)
        {
            _log.Debug($"ImapFolderCountChanged {WorkFolder.Name} Count={WorkFolder.Count}.");

            AddTask(new Task(() => UpdateMessagesList()));
        }

        #endregion

        public SimpleImapClient(MailBoxData mailbox, CancellationToken cancelToken, MailSettings mailSettings, ILog log)
        {
            Account = mailbox;
            _mailSettings = mailSettings;
            _log = log;

            _log.Name = $"ASC.Mail.SimpleClient.Mbox_{mailbox.MailBoxId}";

            _log.Debug($"ImapLog: {$"/var/log/appserver/imap_{Account.MailBoxId}_{Thread.CurrentThread.ManagedThreadId}.log"}");

            var protocolLogger = !string.IsNullOrEmpty(_mailSettings.Aggregator.ProtocolLogPath) ? (IProtocolLogger)
                new ProtocolLogger(_mailSettings.Aggregator.ProtocolLogPath + $"/var/log/appserver/imap_{Account.MailBoxId}_{Thread.CurrentThread.ManagedThreadId}.log", true)
            : new ProtocolLogger($"/var/log/appserver/imap_{Account.MailBoxId}_{Thread.CurrentThread.ManagedThreadId}.log", true);

            StopTokenSource = new CancellationTokenSource();

            CancelToken = CancellationTokenSource.CreateLinkedTokenSource(cancelToken, StopTokenSource.Token).Token;

            asyncTasks = new ConcurrentQueue<Task>();

            imap = new ImapClient(protocolLogger)
            {
                Timeout = _mailSettings.Aggregator.TcpTimeout
            };

            imap.Disconnected += Imap_Disconnected;

            curentTask =Task.Run(()=> {
                Authenticate();

                LoadFoldersFromIMAP();

                ChangeFolder(imap.Inbox);
            }).ContinueWith(TaskManager);
        }

        public void ChangeFolder(IMailFolder newWorkFolder)
        {
            _log.Debug($"Try change folder to {newWorkFolder.Name}.");

            AddTask(new Task(() =>
            {
                try
                {
                    OpenFolder(newWorkFolder);

                    UpdateMessagesList();

                    WorkFoldersChanged(this, EventArgs.Empty);

                    _log.Debug($"Folder changed to {WorkFolder.Name}.");
                }
                catch (Exception ex)
                {
                    _log.Error($"ChangeFolder: {ex.Message}");
                }
            }));
        }

        private void Imap_Disconnected(object sender, DisconnectedEventArgs e)
        {
            _log.Debug($"Imap_Disconnected.");

            if (e.IsRequested)
            {
                _log.Info("Try reconnect to IMAP...");

                Authenticate();
            }
            else
            {
                _log.Info("DisconnectedEventArgs.IsRequested=false.");

                DoneToken?.Cancel();

                StopTokenSource.Cancel();

                if(KillMe!=null) KillMe(this, EventArgs.Empty);
            }
        }

        #region Load Folders from Imap to foldersList

        private bool Authenticate(bool enableUtf8 = true)
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

            if (!imap.IsConnected)
            {
                imap.Connect(Account.Server, Account.Port, secureSocketOptions, CancelToken);
            }

            try
            {
                if (enableUtf8 && (imap.Capabilities & ImapCapabilities.UTF8Accept) != ImapCapabilities.None)
                {
                    _log.Debug("Imap.EnableUTF8");

                    imap.EnableUTF8(CancelToken);
                }

                if (string.IsNullOrEmpty(Account.OAuthToken))
                {
                    _log.DebugFormat("Imap.Authentication({0})", Account.Account);

                    imap.Authenticate(Account.Account, Account.Password, CancelToken);
                }
                else
                {
                    _log.DebugFormat("Imap.AuthenticationByOAuth({0})", Account.Account);

                    var oauth2 = new SaslMechanismOAuth2(Account.Account, Account.AccessToken);

                    imap.Authenticate(oauth2, CancelToken);
                }
            }
            catch (Exception ex)
            {
                _log.Error($"Imap.Authentication Error: {ex.Message}");

                if (KillMe != null) KillMe(this, EventArgs.Empty);

                return false;
            }

            _log.Debug("Imap logged in.");

            return true;
        }

        private void LoadFoldersFromIMAP()
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
                var rootFolder = imap.GetFolder(imap.PersonalNamespaces[0].Path);

                var subfolders = GetImapSubFolders(rootFolder);

                foldersList = subfolders.Where(x => !_mailSettings.SkipImapFlags.Contains(x.Name.ToLowerInvariant()))
                    .Where(x => !x.Attributes.HasFlag(FolderAttributes.NoSelect))
                    .Where(x => !x.Attributes.HasFlag(FolderAttributes.NonExistent))
                    .ToList();

                _log.Debug($"Find {foldersList.Count} folders in IMAP.");
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

        private IEnumerable<IMailFolder> GetImapSubFolders(IMailFolder folder)
        {
            var result = new List<IMailFolder>();

            try
            {
                result = folder.GetSubfolders(true, CancelToken).ToList();

                if (result.Any())
                {
                    var resultWithSubfolders = result.Where(x => x.Attributes.HasFlag(FolderAttributes.HasChildren)).ToList();

                    foreach (var subfolder in resultWithSubfolders)
                    {
                        result.AddRange(GetImapSubFolders(subfolder));
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

        #endregion

        private void OpenFolder(IMailFolder imapFolder)
        {
            if (imapFolder != WorkFolder)
            {
                try
                {
                    if (WorkFolder != null)
                    {
                        WorkFolder.MessageFlagsChanged -= ImapMessageFlagsChanged;
                        WorkFolder.CountChanged -= ImapFolderCountChanged;
                    }

                    WorkFolder = imapFolder;
                    MessagesList = null;

                    WorkFolder.MessageFlagsChanged += ImapMessageFlagsChanged;
                    WorkFolder.CountChanged += ImapFolderCountChanged;
                }
                catch(Exception ex)
                {
                    _log.Error($"OpenFolder {imapFolder.Name}: {ex.Message}");
                }

                _log.Debug($"OpenFolder: Work folder changed to {imapFolder.Name}.");
            }

            if (!WorkFolder.IsOpen)
            {
                try
                {
                    imapFolder.Open(FolderAccess.ReadWrite);

                    _log.Debug($"OpenFolder: Folder {imapFolder.Name} opened.");
                }
                catch(Exception ex)
                {
                    _log.Error($"OpenFolder {imapFolder.Name}: {ex.Message}");
                }
            }
        }

        private void UpdateMessagesList()
        {
            List<MessageDescriptor> newMessagesList= new List<MessageDescriptor>();
            _log.Debug($"UpdateMessagesList: Folder {WorkFolder.Name} Count={WorkFolder.Count}.");

            OpenFolder(WorkFolder);

            try
            {
                newMessagesList = WorkFolder.Fetch(1, -1, MessageSummaryItems.UniqueId | MessageSummaryItems.Flags).ToMessageDescriptorList();

                _log.Debug($"UpdateMessagesList: New messages count={newMessagesList?.Count}.");
            }
            catch(Exception ex)
            {
                _log.Error($"UpdateMessagesList: Try fetch messages from imap folder={WorkFolder.Name}: {ex.Message}.");
            }
        
            if (MessagesList == null)
            {
                MessagesList = newMessagesList;

                if (MessagesListUpdated != null) MessagesListUpdated(this, EventArgs.Empty);
            }
            else
            {
                foreach(var newMessage in newMessagesList)
                {
                    var oldMessage = MessagesList.FirstOrDefault(x => x.UniqueId == newMessage.UniqueId);

                    if(oldMessage==null)
                    {
                        try
                        {
                            var mimeMessage = WorkFolder.GetMessage(newMessage.UniqueId, CancelToken);

                            _log.Debug($"UpdateMessagesList: New message detected {newMessage.UniqueId}.");

                            NewMessage(this, (mimeMessage, newMessage));
                        }
                        catch (Exception ex)
                        {
                            _log.Error($"UpdateMessagesList: Try fetch one message from imap with UniqueId={newMessage.UniqueId}: {ex.Message}.");
                        }
                    }
                    else
                    {
                        if(newMessage.Flags.HasValue)
                        {
                            CompareFlags(WorkFolder, oldMessage, newMessage.Flags.Value);
                        }
                    }
                }
            }

            MessagesList = newMessagesList;
        }

        public void TryGetNewMessage(UniqueId uniqueId)
        {
            AddTask(new Task(() => GetNewMessage(uniqueId)));
        }

        private void GetNewMessage(UniqueId uniqueId)
        {
            _log.Debug($"GetNewMessage task run: UniqueId={uniqueId}.");

            var message = MessagesList.FirstOrDefault(x => x.UniqueId == uniqueId);

            if (message == null) return;

            try
            {
                var mimeMessage = WorkFolder.GetMessage(message.UniqueId, CancelToken);

                if (NewMessage != null) NewMessage(this, (mimeMessage, message));
            }
            catch (Exception ex)
            {
                _log.Error($"GetNewMessage: Try fetch one mimeMessage from imap with UniqueId={message.UniqueId}: {ex.Message}.");
            }
        }

        private async Task SetIdle()
        {
            if (WorkFolder == null) return;

            if (!imap.IsAuthenticated) return;

            try
            {
                OpenFolder(WorkFolder);

                if (imap.Capabilities.HasFlag(ImapCapabilities.Idle))
                {
                    DoneToken = new CancellationTokenSource(new TimeSpan(0, 10, 0));
                    
                    _log.Debug($"Go to Idle. Folder={WorkFolder.Name}.");

                    await imap.IdleAsync(DoneToken.Token);
                }
                else
                {
                    await Task.Delay(new TimeSpan(0, 10, 0));
                    await imap.NoOpAsync();
                }
            }
            catch (Exception ex)
            {
                _log.Error($"SetIdle, Error:{ex.Message}");
            }
            finally
            {
                DoneToken?.Dispose();

                DoneToken = null;

                _log.Debug($"Retrurn from Idle.");
            }
        }

        public void TryMoveMessageInImap(IMailFolder sourceFolder, List<UniqueId> uniqueIds, IMailFolder destinationFolder)
        {
            AddTask(new Task(() => MoveMessageInImap(WorkFolder, uniqueIds, destinationFolder)));
        }

        private void MoveMessageInImap(IMailFolder sourceFolder, List<UniqueId> uniqueIds, IMailFolder destinationFolder)
        {
            if (uniqueIds.Count == 0 || sourceFolder == null || destinationFolder == null)
            {
                _log.Debug($"MoveMessageInImap: Bad parametrs. Source={sourceFolder?.Name}, Count={uniqueIds.Count}, Destination={destinationFolder?.Name}.");

                return;
            }

            _log.Debug($"MoveMessageInImap task run: Source={sourceFolder?.Name}, Count={uniqueIds.Count}, Destination={destinationFolder?.Name}.");

            try
            {
                OpenFolder(sourceFolder);

                var returnedUidl = sourceFolder.MoveTo(uniqueIds, destinationFolder);

                MessagesList = MessagesList.Where(x => !uniqueIds.Contains(x.UniqueId)).ToList();
            }
            catch (Exception ex)
            {
                _log.Error($"MoveMessageInImap: {ex.Message}");
            }
        }

        public void TrySetFlagsInImap(IMailFolder folder, List<UniqueId> uniqueIds, MailUserAction action)
        {
            AddTask(new Task(() => SetFlagsInImap(folder, uniqueIds, action)));
        }

        private bool SetFlagsInImap(IMailFolder folder, List<UniqueId> uniqueIds, MailUserAction action)
        {
            if (folder == null) return false;
            if (uniqueIds.Count == 0) return false;

            _log.Debug($"SetFlagsInImap task run: In {folder} set {action} for {uniqueIds.Count} messages.");

            OpenFolder(folder);

            try
            {
                switch (action)
                {
                    case MailUserAction.SetAsRead:
                        folder.AddFlags(uniqueIds, MessageFlags.Seen, true);
                        MessagesList.ForEach(x=>
                        {
                            if (uniqueIds.Contains(x.UniqueId))
                            {
                                x.Flags = x.Flags.Value | MessageFlags.Seen;
                            }
                        });
                        break;
                    case MailUserAction.SetAsUnread:
                        folder.RemoveFlags(uniqueIds, MessageFlags.Seen, true);
                        MessagesList.ForEach(x =>
                        {
                            if (uniqueIds.Contains(x.UniqueId))
                            {
                                x.Flags = x.Flags.Value ^ MessageFlags.Seen;
                            }
                        });
                        break;
                    case MailUserAction.SetAsImportant:
                        folder.AddFlags(uniqueIds, MessageFlags.Flagged, true);
                        MessagesList.ForEach(x =>
                        {
                            if (uniqueIds.Contains(x.UniqueId))
                            {
                                x.Flags = x.Flags.Value ^ MessageFlags.Flagged;
                            }
                        });
                        break;
                    case MailUserAction.SetAsNotImpotant:
                        folder.RemoveFlags(uniqueIds, MessageFlags.Flagged, true);
                        MessagesList.ForEach(x =>
                        {
                            if (uniqueIds.Contains(x.UniqueId))
                            {
                                x.Flags = x.Flags.Value ^ MessageFlags.Flagged;
                            }
                        });
                        break;
                }
            }
            catch (Exception ex)
            {
                _log.Error($"SetMessageFlagIMAP->{folder.Name}, {action}->{ex.Message}");

                return false;
            }

            return true;
        }

        private void CompareFlags(IMailFolder imap_folder, MessageDescriptor oldMessage, MessageFlags newFlag)
        {
            if (newFlag == oldMessage.Flags) return;

            _log.Debug($"CompareFlags: {imap_folder.Name} Old flags=({oldMessage.Flags}) New flags {newFlag}.");

            bool oldSeen = oldMessage.Flags.Value.HasFlag(MessageFlags.Seen);
            bool newSeen = newFlag.HasFlag(MessageFlags.Seen);

            if (oldSeen != newSeen)
            {
                if (NewActionFromImap != null)
                {
                    ImapAction imapAction = new ImapAction()
                    {
                        FolderAction = oldSeen ? MailUserAction.SetAsUnread : MailUserAction.SetAsRead,
                        Folder = imap_folder,
                        UniqueId = oldMessage.UniqueId
                    };
                    NewActionFromImap(this, imapAction);
                }
            }

            bool oldImportant = oldMessage.Flags.Value.HasFlag(MessageFlags.Flagged);
            bool newImportant = newFlag.HasFlag(MessageFlags.Flagged);

            if (oldImportant != newImportant)
            {
                if (NewActionFromImap != null)
                {
                    ImapAction imapAction = new ImapAction()
                    {
                        FolderAction = oldImportant ? MailUserAction.SetAsNotImpotant : MailUserAction.SetAsImportant,
                        Folder = imap_folder,
                        UniqueId = oldMessage.UniqueId
                    };
                    NewActionFromImap(this, imapAction);
                }
            }

            oldMessage.Flags = newFlag;
        }

        private void TaskManager(Task previosTask)
        {
            if (previosTask.Exception!=null)
            {
                _log.Error($"Task manager: {previosTask.Exception.Message}");
            }

            if(asyncTasks.TryDequeue(out var task))
            {
                _log.Debug($"TaskManager: new task id={task.Id}.");

                curentTask = task.ContinueWith(TaskManager);

                task.Start();

                return;
            }

            _log.Debug($"TaskManager: no task in queue.");

            if(StopTokenSource!=null)
            {
                if (!StopTokenSource.IsCancellationRequested)
                {
                    curentTask = SetIdle().ContinueWith(TaskManager);
                }
            }
        }

        private void AddTask(Task task)
        {
            _log.Debug($"AddTask: task id={task.Id} added to queue.");

            asyncTasks.Enqueue(task);

            DoneToken?.Cancel();
        }

        public void Dispose()
        {
            imap.Dispose();

            DoneToken?.Cancel();
            DoneToken.Dispose();

            StopTokenSource.Cancel();
            StopTokenSource.Dispose();
        }
    }
}
