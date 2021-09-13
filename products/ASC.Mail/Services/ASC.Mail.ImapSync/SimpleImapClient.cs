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
using ASC.Mail.Core.Entities;
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
        public bool IsReady { get; private set; } = false;

        public Task curentTask { get; private set; }

        public List<MessageDescriptor> ImapMessagesList { get; set; }
        public IMailFolder ImapWorkFolder { get; private set; }
        public List<MailInfo> WorkFolderMails { get; set; }

        private readonly ILog _log;
        private readonly MailSettings _mailSettings;
        public readonly MailBoxData Account;

        public event EventHandler<ImapAction> NewActionFromImap;
        public event EventHandler<(MimeMessage, MessageDescriptor)> NewMessage;
        public event EventHandler<string> DeleteMessage;
        public event EventHandler MessagesListUpdated;
        public event EventHandler<bool> OnCriticalError;
        public event EventHandler<UniqueIdMap> OnUidlsChange;

        private CancellationTokenSource DoneToken { get; set; }
        private CancellationToken CancelToken { get; set; }
        private CancellationTokenSource StopTokenSource { get; set; }

        private ImapClient imap;

        private ConcurrentQueue<Task> asyncTasks;

        private bool IsUpdateMessagesListPlanned = true;

        public Models.MailFolder MailWorkFolder
        {
            get
            {
                return foldersDictionary[ImapWorkFolder];
            }
        }

        public FolderType Folder
        {
            get
            {
                return MailWorkFolder.Folder;
            }
        }
        public Dictionary<IMailFolder, Models.MailFolder> foldersDictionary { get; private set; }

        #region Event from Imap handlers

        private void ImapMessageFlagsChanged(object sender, MessageFlagsChangedEventArgs e)
        {
            if (!IsReady) return;

            if (sender is IMailFolder imap_folder)
            {
                _log.Debug($"ImapMessageFlagsChanged. Folder= {ImapWorkFolder.Name} Index={e.Index}.");

                MessageDescriptor messageSummary = ImapMessagesList?.FirstOrDefault(x => x.Index == e.Index);

                if (messageSummary == null)
                {
                    _log.Warn($"ImapMessageFlagsChanged. No Message summary. Folder= {ImapWorkFolder.Name} Index={e.Index}.");

                    return;
                }

                if (messageSummary.Flags.HasValue)
                {
                    CompareFlags(imap_folder, messageSummary, e.Flags);
                }
            }
        }

        private void ImapFolderCountChanged(object sender, EventArgs e)
        {
            if (!IsReady) return;

            _log.Debug($"ImapFolderCountChanged {ImapWorkFolder.Name} Count={ImapWorkFolder.Count}.");

            if (!IsUpdateMessagesListPlanned)
            {
                AddTask(new Task(() => UpdateMessagesList()));
            }
        }

        #endregion

        public IMailFolder GetImapFolderByType(int folderType) => foldersDictionary.FirstOrDefault(x => x.Value.Folder == (FolderType)folderType).Key;

        public SimpleImapClient(MailBoxData mailbox, CancellationToken cancelToken, MailSettings mailSettings, ILog log)
        {
            Account = mailbox;
            _mailSettings = mailSettings;
            _log = log;

            _log.Name = $"ASC.Mail.SimpleImap_{Account.MailBoxId}";

            var protocolLogger = string.IsNullOrEmpty(_mailSettings.Aggregator.ProtocolLogPath) ? (IProtocolLogger)new NullProtocolLogger() :
                new ProtocolLogger(_mailSettings.Aggregator.ProtocolLogPath + $"/imap_{Account.MailBoxId}_{Thread.CurrentThread.ManagedThreadId}.log", true);

            StopTokenSource = new CancellationTokenSource();

            CancelToken = CancellationTokenSource.CreateLinkedTokenSource(cancelToken, StopTokenSource.Token).Token;

            asyncTasks = new ConcurrentQueue<Task>();

            foldersDictionary = new Dictionary<IMailFolder, Models.MailFolder>();

            imap = new ImapClient(protocolLogger)
            {
                Timeout = _mailSettings.Aggregator.TcpTimeout
            };

            imap.Disconnected += Imap_Disconnected;

            Authenticate();

            LoadFoldersFromIMAP();
        }

        public void ChangeFolder(int folderActivity)
        {
            if (folderActivity != (int)Folder)
            {
                try
                {
                    var newImapFolder = GetImapFolderByType(folderActivity);

                    _log.Debug($"Try change folder to {newImapFolder.Name}.");

                    AddTask(new Task(() => SetNewImapWorkFolder(newImapFolder)));
                }
                catch (Exception ex)
                {
                    _log.Error($"ChangeFolder(New folder={(FolderType)folderActivity})->{ex.Message}");
                }
            }
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

                StopTokenSource?.Cancel();

                OnCriticalError?.Invoke(this, false);
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

                OnCriticalError?.Invoke(this, true);

                return false;
            }

            _log.Debug("Imap logged in.");

            return true;
        }

        internal void Init()
        {
            SetNewImapWorkFolder(imap.Inbox);

            IsReady = true;

            TaskManager(Task.CompletedTask);
        }

        private void LoadFoldersFromIMAP()
        {
            _log.Debug("Load folders from IMAP.");

            try
            {
                var rootFolder = imap.GetFolder(imap.PersonalNamespaces[0].Path);

                var subfolders = GetImapSubFolders(rootFolder);

                var imapFoldersList = subfolders.Where(x => !_mailSettings.SkipImapFlags.Contains(x.Name.ToLowerInvariant()))
                    .Where(x => !x.Attributes.HasFlag(FolderAttributes.NoSelect))
                    .Where(x => !x.Attributes.HasFlag(FolderAttributes.NonExistent))
                    .ToList();

                imapFoldersList.ForEach(x =>
                {
                    var mailFolder = DetectFolder(x);

                    if (mailFolder == null)
                    {
                        _log.Debug($"LoadFoldersFromIMAP-> Skip folder {x.Name}.");
                    }
                    else
                    {
                        foldersDictionary.Add(x, mailFolder);

                        _log.Debug($"LoadFoldersFromIMAP-> Detect folder {x.Name}.");
                    }
                });

                _log.Debug($"Find {foldersDictionary.Count} folders in IMAP.");
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

        private void OpenFolder(IMailFolder folder)
        {
            if (!folder.IsOpen)
            {
                try
                {
                    folder.Open(FolderAccess.ReadWrite);

                    _log.Debug($"OpenFolder: Folder {folder.Name} opened.");
                }
                catch (Exception ex)
                {
                    _log.Error($"OpenFolder {folder.Name}: {ex.Message}");
                }
            }
        }

        private void SetNewImapWorkFolder(IMailFolder imapFolder)
        {
            if (imapFolder != ImapWorkFolder)
            {
                IsUpdateMessagesListPlanned = true;

                try
                {
                    if (ImapWorkFolder != null)
                    {
                        ImapWorkFolder.MessageFlagsChanged -= ImapMessageFlagsChanged;
                        ImapWorkFolder.CountChanged -= ImapFolderCountChanged;
                    }

                    ImapWorkFolder = imapFolder;
                    ImapMessagesList = null;

                    ImapWorkFolder.MessageFlagsChanged += ImapMessageFlagsChanged;
                    ImapWorkFolder.CountChanged += ImapFolderCountChanged;

                    OpenFolder(ImapWorkFolder);

                    UpdateMessagesList();
                }
                catch (Exception ex)
                {
                    _log.Error($"SetNewImapWorkFolder {imapFolder.Name}: {ex.Message}");
                }

                _log.Debug($"SetNewImapWorkFolder: Work folder changed to {imapFolder.Name}.");
            }
        }

        private void UpdateMessagesList()
        {
            List<MessageDescriptor> newMessagesList = new List<MessageDescriptor>();

            _log.Debug($"UpdateMessagesList: Folder {ImapWorkFolder.Name} Count={ImapWorkFolder.Count}.");

            try
            {
                newMessagesList = ImapWorkFolder.Fetch(0, -1, MessageSummaryItems.UniqueId | MessageSummaryItems.Flags).ToMessageDescriptorList();

                _log.Debug($"UpdateMessagesList: New messages count={newMessagesList?.Count}.");
            }
            catch (Exception ex)
            {
                _log.Error($"UpdateMessagesList: Try fetch messages from imap folder={ImapWorkFolder.Name}: {ex.Message}.");

                return;
            }

            IsUpdateMessagesListPlanned = false;

            if (ImapMessagesList == null)
            {
                ImapMessagesList = newMessagesList;

                MessagesListUpdated?.Invoke(this, EventArgs.Empty);
            }
            else
            {
                foreach (var newMessage in newMessagesList)
                {
                    var oldMessage = ImapMessagesList.FirstOrDefault(x => x.UniqueId == newMessage.UniqueId);

                    if (oldMessage == null)
                    {
                        try
                        {
                            var mimeMessage = ImapWorkFolder.GetMessage(newMessage.UniqueId, CancelToken);

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
                        if (newMessage.Flags.HasValue)
                        {
                            CompareFlags(ImapWorkFolder, oldMessage, newMessage.Flags.Value);
                        }
                    }
                }
            }

            ImapMessagesList = newMessagesList;
        }

        public void TryGetNewMessage(UniqueId uniqueId)
        {
            AddTask(new Task(() => GetNewMessage(uniqueId)));
        }

        private void GetNewMessage(UniqueId uniqueId)
        {
            _log.Debug($"GetNewMessage task run: UniqueId={uniqueId}.");

            var message = ImapMessagesList.FirstOrDefault(x => x.UniqueId == uniqueId);

            if (message == null) return;

            try
            {
                var mimeMessage = ImapWorkFolder.GetMessage(message.UniqueId, CancelToken);

                if (NewMessage != null) NewMessage(this, (mimeMessage, message));
            }
            catch (Exception ex)
            {
                _log.Error($"GetNewMessage: Try fetch one mimeMessage from imap with UniqueId={message.UniqueId}: {ex.Message}.");
            }
        }

        private async Task SetIdle()
        {
            if (ImapWorkFolder == null) return;

            if (!imap.IsAuthenticated) return;

            try
            {
                if (imap.Capabilities.HasFlag(ImapCapabilities.Idle))
                {
                    DoneToken = new CancellationTokenSource(new TimeSpan(0, 10, 0));

                    _log.Debug($"Go to Idle. Folder={ImapWorkFolder.Name}.");

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

                OnCriticalError?.Invoke(this, false);
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
            AddTask(new Task(() => MoveMessageInImap(ImapWorkFolder, uniqueIds, destinationFolder)));
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

                OpenFolder(ImapWorkFolder);

                OnUidlsChange?.Invoke(this, returnedUidl);

                ImapMessagesList = ImapMessagesList.Where(x => !uniqueIds.Contains(x.UniqueId)).ToList();
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

            try
            {
                OpenFolder(folder);

                switch (action)
                {
                    case MailUserAction.SetAsRead:
                        folder.AddFlags(uniqueIds, MessageFlags.Seen, true);
                        ImapMessagesList.ForEach(x =>
                        {
                            if (uniqueIds.Contains(x.UniqueId))
                            {
                                x.Flags = x.Flags.Value | MessageFlags.Seen;
                            }
                        });
                        break;
                    case MailUserAction.SetAsUnread:
                        folder.RemoveFlags(uniqueIds, MessageFlags.Seen, true);
                        ImapMessagesList.ForEach(x =>
                        {
                            if (uniqueIds.Contains(x.UniqueId))
                            {
                                x.Flags = x.Flags.Value ^ MessageFlags.Seen;
                            }
                        });
                        break;
                    case MailUserAction.SetAsImportant:
                        folder.AddFlags(uniqueIds, MessageFlags.Flagged, true);
                        ImapMessagesList.ForEach(x =>
                        {
                            if (uniqueIds.Contains(x.UniqueId))
                            {
                                x.Flags = x.Flags.Value ^ MessageFlags.Flagged;
                            }
                        });
                        break;
                    case MailUserAction.SetAsNotImpotant:
                        folder.RemoveFlags(uniqueIds, MessageFlags.Flagged, true);
                        ImapMessagesList.ForEach(x =>
                        {
                            if (uniqueIds.Contains(x.UniqueId))
                            {
                                x.Flags = x.Flags.Value ^ MessageFlags.Flagged;
                            }
                        });
                        break;
                }

                OpenFolder(ImapWorkFolder);
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

            var Uidl = oldMessage.UniqueId.ToUidl(foldersDictionary[imap_folder].Folder);

            var _message = WorkFolderMails.FirstOrDefault(x => x.Uidl == Uidl);

            if (_message == null) return;

            var _message_id = _message.Id;

            _log.Debug($"CompareFlags: {imap_folder.Name} Old flags=({oldMessage.Flags}) New flags {newFlag}.");

            try
            {
                bool oldSeen = oldMessage.Flags.Value.HasFlag(MessageFlags.Seen);
                bool newSeen = newFlag.HasFlag(MessageFlags.Seen);

                if (oldSeen != newSeen)
                {
                    if (NewActionFromImap != null)
                    {
                        ImapAction imapAction = new ImapAction()
                        {
                            FolderAction = oldSeen ? MailUserAction.SetAsUnread : MailUserAction.SetAsRead,
                            message_id = _message_id
                        };
                        NewActionFromImap(this, imapAction);
                    }
                }
            }
            catch (Exception ex)
            {
                _log.Error($"CompareFlags.Seen Uidl={oldMessage.UniqueId}, ImapFolder={imap_folder.Name}->{ex.Message}");
            }

            try
            {
                bool oldImportant = oldMessage.Flags.Value.HasFlag(MessageFlags.Flagged);
                bool newImportant = newFlag.HasFlag(MessageFlags.Flagged);

                if (oldImportant != newImportant)
                {
                    if (NewActionFromImap != null)
                    {
                        ImapAction imapAction = new ImapAction()
                        {
                            FolderAction = oldImportant ? MailUserAction.SetAsNotImpotant : MailUserAction.SetAsImportant,
                            message_id = _message_id
                        };
                        NewActionFromImap(this, imapAction);
                    }
                }
            }
            catch (Exception ex)
            {
                _log.Error($"CompareFlags.Flagged Uidl={oldMessage.UniqueId}, ImapFolder={imap_folder.Name}->{ex.Message}");
            }


            oldMessage.Flags = newFlag;
        }

        private void TaskManager(Task previosTask)
        {
            if (previosTask.Exception != null)
            {
                _log.Error($"Task manager: {previosTask.Exception.Message}");
            }

            if (asyncTasks.TryDequeue(out var task))
            {
                _log.Debug($"TaskManager: new task id={task.Id}.");

                curentTask = task.ContinueWith(TaskManager);

                task.Start();

                return;
            }

            _log.Debug($"TaskManager: no task in queue.");

            if (StopTokenSource != null)
            {
                if (StopTokenSource.IsCancellationRequested)
                {
                    OnCriticalError?.Invoke(this, false);
                }
                else
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
            imap?.Dispose();

            DoneToken?.Cancel();
            DoneToken?.Dispose();

            StopTokenSource?.Cancel();
            StopTokenSource?.Dispose();
        }

        private Models.MailFolder DetectFolder(IMailFolder folder)
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
                return new Models.MailFolder(FolderType.Inbox, folder.Name);
            }
            if ((folder.Attributes & FolderAttributes.Sent) != 0)
            {
                return new Models.MailFolder(FolderType.Sent, folder.Name);
            }
            if ((folder.Attributes & FolderAttributes.Junk) != 0)
            {
                return new Models.MailFolder(FolderType.Spam, folder.Name);
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
                return new Models.MailFolder(folderId, folder.Name);
            }

            if (_mailSettings.SpecialDomainFolders.Any() &&
                _mailSettings.SpecialDomainFolders.ContainsKey(Account.Server))
            {
                var domainSpecialFolders = _mailSettings.SpecialDomainFolders[Account.Server];

                if (domainSpecialFolders.Any() &&
                    domainSpecialFolders.ContainsKey(folderName))
                {
                    var info = domainSpecialFolders[folderName];
                    return info.skip ? null : new Models.MailFolder(info.folder_id, folder.Name);
                }
            }

            if (_mailSettings.DefaultFolders == null || !_mailSettings.DefaultFolders.ContainsKey(folderName))
                return new Models.MailFolder(FolderType.Inbox, folder.Name, new[] { folder.FullName });

            folderId = (FolderType)_mailSettings.DefaultFolders[folderName];
            return new Models.MailFolder(folderId, folder.Name);
        }
    }
}
