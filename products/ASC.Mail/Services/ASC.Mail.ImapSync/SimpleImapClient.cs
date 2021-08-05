using System;
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

using MimeKit;

namespace ASC.Mail.ImapSync
{
    public class SimpleImapClient : IDisposable
    {
        private ImapClient imap;

        public ConcurrentQueue<ImapAction> actionsFromImap;
        private ConcurrentQueue<Task> asyncTasks;

        public Task curentTask { get; private set; }

        private readonly ILog _log;
        private readonly MailSettings _mailSettings;
        private readonly MailBoxData Account;

        public event EventHandler<ImapAction> NewActionFromImap;
        public event EventHandler<(MimeMessage, IMessageSummary)> NewMessage;
        public event EventHandler<string> DeleteMessage;
        public event EventHandler MessagesListUpdated;
        public event EventHandler WorkFoldersChanged;

        private CancellationTokenSource DoneToken { get; set; }
        private CancellationToken CancelToken { get; set; }
        private CancellationTokenSource StopTokenSource { get; set; }

        public List<IMailFolder> foldersList { get; private set; }
        public List<IMessageSummary> MessagesList { get; private set; }

        public IMailFolder WorkFolder;

        #region Event from Imap handlers

        private void ImapMessageFlagsChanged(object sender, MessageFlagsChangedEventArgs e)
        {
            if (sender is IMailFolder imap_folder)
            {
                IMessageSummary messageSummary = MessagesList.FirstOrDefault(x => x.Index == e.Index);

                if (messageSummary == null) return;

                if (messageSummary.Flags.HasValue)
                {
                    if (e.Flags == messageSummary.Flags) return;

                    bool oldSeen = messageSummary.Flags.Value.HasFlag(MessageFlags.Seen);
                    bool newSeen = e.Flags.HasFlag(MessageFlags.Seen);

                    if (oldSeen != newSeen)
                    {
                        if (NewActionFromImap != null)
                        {
                            ImapAction imapAction = new ImapAction()
                            {
                                FolderAction = oldSeen ? MailUserAction.SetAsUnread : MailUserAction.SetAsRead,
                                Folder = imap_folder,
                                UniqueId = messageSummary.UniqueId
                            };
                            NewActionFromImap(this, imapAction);
                        }
                    }

                    bool oldImportant = messageSummary.Flags.Value.HasFlag(MessageFlags.Flagged);
                    bool newImportant = e.Flags.HasFlag(MessageFlags.Flagged);

                    if (oldImportant != newImportant)
                    {
                        if (NewActionFromImap != null)
                        {
                            ImapAction imapAction = new ImapAction()
                            {
                                FolderAction = oldImportant ? MailUserAction.SetAsNotImpotant : MailUserAction.SetAsImportant,
                                Folder = imap_folder,
                                UniqueId = messageSummary.UniqueId
                            };
                            NewActionFromImap(this, imapAction);
                        }
                    }

                    AddTask(new Task(() => UpdateMessagesList()));
                }
            }
        }

        private void ImapFolderCountChanged(object sender, EventArgs e)
        {
            AddTask(new Task(() => UpdateMessagesList()));
        }

        #endregion

        public SimpleImapClient(MailBoxData mailbox, CancellationToken cancelToken, MailSettings mailSettings, ILog log)
        {
            Account = mailbox;
            _mailSettings = mailSettings;
            _log = log;

            _log.Name = $"ASC.Mail.SimpleClient.Mbox_{mailbox.MailBoxId}";

            var protocolLogger = !string.IsNullOrEmpty(_mailSettings.ProtocolLogPath)
                ? (IProtocolLogger)
                    new ProtocolLogger(_mailSettings.ProtocolLogPath + $"\\imap_{Account.MailBoxId}_{Thread.CurrentThread.ManagedThreadId}.log", true)
                : new NullProtocolLogger();

            StopTokenSource = new CancellationTokenSource();

            CancelToken = CancellationTokenSource.CreateLinkedTokenSource(cancelToken, StopTokenSource.Token).Token;

            asyncTasks = new ConcurrentQueue<Task>();
            actionsFromImap = new ConcurrentQueue<ImapAction>();

            imap = new ImapClient(protocolLogger)
            {
                Timeout = _mailSettings.TcpTimeout
            };

            curentTask = ConnectToServer().ContinueWith(TaskManager);
        }

        private async Task ConnectToServer()
        {
            await Authenticate();

            await LoadFoldersFromIMAP();

            ChangeFolder(imap.Inbox);
        }

        public void ChangeFolder(IMailFolder newWorkFolder)
        {
            AddTask(new Task(() =>
            {
                try
                {
                    OpenFolder(newWorkFolder);

                    UpdateMessagesList();
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

                //_ = CreateConnection();
            }
            else
            {
                _log.Info("DisconnectedEventArgs.IsRequested=false.");

                DoneToken?.Cancel();

                StopTokenSource.Cancel();
            }
        }

        #region Load Folders from Imap to foldersList

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

            if (!imap.IsConnected)
            {
                await imap.ConnectAsync(Account.Server, Account.Port, secureSocketOptions, CancelToken);

                imap.Disconnected += Imap_Disconnected;
            }

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

        private async Task LoadFoldersFromIMAP()
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

        #endregion

        private void OpenFolder(IMailFolder imapFolder)
        {
            if (imapFolder != WorkFolder)
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
            OpenFolder(WorkFolder);

            var newMessagesList = WorkFolder.Fetch(1, -1, MessageSummaryItems.UniqueId | MessageSummaryItems.Flags).ToList();

            if (MessagesList == null)
            {
                MessagesList = newMessagesList;
            }
            else
            {
                var deletedMessages = MessagesList.Where(x => !newMessagesList.Any(y => y.UniqueId == x.UniqueId)).ToList();
                var newMessages = newMessagesList.Where(x => !MessagesList.Any(y => y.UniqueId == x.UniqueId)).ToList();

                MessagesList = newMessagesList;

                foreach (var message in deletedMessages)
                {
                    if (DeleteMessage != null)
                    {
                        DeleteMessage(this, message.UniqueId.ToString());
                    }
                }

                foreach (var message in newMessages)
                {
                    if (NewMessage != null)
                    {
                        var mimeMessage = WorkFolder.GetMessage(message.UniqueId, CancelToken);

                        NewMessage(this, (mimeMessage, message));
                    }
                }
            }

            if (MessagesListUpdated != null) MessagesListUpdated(this, EventArgs.Empty);
        }

        public void TryGetNewMessage(UniqueId uniqueId)
        {
            AddTask(new Task(() => GetNewMessage(uniqueId)));
        }

        private void GetNewMessage(UniqueId uniqueId)
        {
            var message = MessagesList.FirstOrDefault(x => x.UniqueId == uniqueId);

            if (message == null) return;

            var mimeMessage = WorkFolder.GetMessage(message.UniqueId, CancelToken);

            if(NewMessage!=null) NewMessage(this, (mimeMessage, message));
        }

        private async Task SetIdle()
        {
            if (WorkFolder == null) return;

            try
            {
                OpenFolder(WorkFolder);

                if (imap.Capabilities.HasFlag(ImapCapabilities.Idle))
                {
                    DoneToken = new CancellationTokenSource(new TimeSpan(0, 10, 0));

                    _log.Debug($"Go to Idle.");

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
                _log.Error($"ReturnImapSemaphore, Error:{ex.Message}");
            }
            finally
            {
                DoneToken?.Dispose();

                DoneToken = null;
            }
        }

        public void MoveMessageInImap(IMailFolder sourceFolder, List<UniqueId> uniqueIds, IMailFolder destinationFolder)
        {
            if (uniqueIds.Count == 0 || sourceFolder == null || destinationFolder == null)
            {
                _log.Debug($"MoveMessageInImap: Bad parametrs. Source={sourceFolder?.Name}, Count={uniqueIds.Count}, Destination={destinationFolder?.Name}.");

                return;
            }

            try
            {
                OpenFolder(sourceFolder);

                var returnedUidl = sourceFolder.MoveTo(uniqueIds, destinationFolder);

                UpdateMessagesList();
            }
            catch (Exception ex)
            {
                _log.Error($"MoveMessageInImap: Source={sourceFolder?.Name}, Count={uniqueIds.Count}, Destination={destinationFolder?.Name}.");
                _log.Error($"MoveMessageInImap: {ex.Message}");
            }
        }

        public void TrySetFlagsInImap(IMailFolder folder, List<UniqueId> uniqueIds, MailUserAction action)
        {
            var result = new Task(() => SetFlagsInImap(folder, uniqueIds, action));

            asyncTasks.Enqueue(result);

            DoneToken?.Cancel();
        }

        private bool SetFlagsInImap(IMailFolder folder, List<UniqueId> uniqueIds, MailUserAction action)
        {
            if (folder == null) return false;
            if (uniqueIds.Count == 0) return false;
 
            try
            {
                OpenFolder(folder);

                switch (action)
                {
                    case MailUserAction.SetAsRead:
                        folder.AddFlags(uniqueIds, MessageFlags.Seen, true);
                        break;
                    case MailUserAction.SetAsUnread:
                        folder.RemoveFlags(uniqueIds, MessageFlags.Seen, true);
                        break;
                    case MailUserAction.SetAsImportant:
                        folder.AddFlags(uniqueIds, MessageFlags.Flagged, true);
                        break;
                    case MailUserAction.SetAsNotImpotant:
                        folder.RemoveFlags(uniqueIds, MessageFlags.Flagged, true);
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

        private void TaskManager(Task previosTask)
        {
            if(previosTask.Exception!=null)
            {
                _log.Error($"Task manager: {previosTask.Exception.Message}");
            }

            if(asyncTasks.TryDequeue(out var task))
            {
                curentTask = task.ContinueWith(TaskManager);
                task.Start();

                return;
            }

            curentTask = SetIdle().ContinueWith(TaskManager);
        }

        private void AddTask(Task task)
        {
            asyncTasks.Enqueue(task);

            DoneToken?.Cancel();
        }

        public void Dispose()
        {
            imap.Dispose();
        }
    }
}
