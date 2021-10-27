/*
 *
 * (c) Copyright Ascensio System Limited 2010-2018
 *
 * This program is freeware. You can redistribute it and/or modify it under the terms of the GNU 
 * General Public License (GPL) version 3 as published by the Free Software Foundation (https://www.gnu.org/copyleft/gpl.html). 
 * In accordance with Section 7(a) of the GNU GPL its Section 15 shall be amended to the effect that 
 * Ascensio System SIA expressly excludes the warranty of non-infringement of any third-party rights.
 *
 * THIS PROGRAM IS DISTRIBUTED WITHOUT ANY WARRANTY; WITHOUT EVEN THE IMPLIED WARRANTY OF MERCHANTABILITY OR
 * FITNESS FOR A PARTICULAR PURPOSE. For more details, see GNU GPL at https://www.gnu.org/copyleft/gpl.html
 *
 * You can contact Ascensio System SIA by email at sales@onlyoffice.com
 *
 * The interactive user interfaces in modified source and object code versions of ONLYOFFICE must display 
 * Appropriate Legal Notices, as required under Section 5 of the GNU GPL version 3.
 *
 * Pursuant to Section 7 § 3(b) of the GNU GPL you must retain the original ONLYOFFICE logo which contains 
 * relevant author attributions when distributing the software. If the display of the logo in its graphic 
 * form is not reasonably feasible for technical reasons, you must include the words "Powered by ONLYOFFICE" 
 * in every copy of the program you distribute. 
 * Pursuant to Section 7 § 3(e) we decline to grant you any rights under trademark law for use of our trademarks.
 *
*/


using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Security;
using System.Security.Authentication;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading;

using ASC.Common.Logging;
using ASC.Mail.Clients.Imap;
using ASC.Mail.Configuration;
using ASC.Mail.Core.Engine;
using ASC.Mail.Core.Exceptions;
using ASC.Mail.Enums;
using ASC.Mail.Extensions;
using ASC.Mail.Models;

using MailKit;
using MailKit.Net.Imap;
using MailKit.Net.Pop3;
using MailKit.Net.Smtp;
using MailKit.Search;
using MailKit.Security;

using MimeKit;

using AuthenticationException = MailKit.Security.AuthenticationException;
using MailFolder = ASC.Mail.Models.MailFolder;
using Pop3Client = MailKit.Net.Pop3.Pop3Client;
using SmtpClient = MailKit.Net.Smtp.SmtpClient;

namespace ASC.Mail.Clients
{
    public class MailClient : IDisposable
    {
        public MailBoxData Account { get; private set; }
        public List<ServerFolderAccessInfo> ServerFolderAccessInfos { get; }
        public bool CertificatePermit { get; }
        public FolderEngine FolderEngine { get; }
        public ILog Log { get; set; }

        public ImapClient Imap { get; private set; }
        public Pop3Client Pop { get; private set; }
        public SmtpClient Smtp { get; private set; }

        public bool IsConnected { get; private set; }
        public bool IsAuthenticated { get; private set; }
        public bool IsDisposed { get; private set; }

        private CancellationToken CancelToken { get; set; }
        private CancellationTokenSource StopTokenSource { get; set; }

        private const int CONNECT_TIMEOUT = 15000;
        private const int ENABLE_UTF8_TIMEOUT = 10000;
        private const int LOGIN_TIMEOUT = 30000;

        /// <summary>
        /// Occurs when the client has been successfully authenticated.
        /// </summary>
        /// <remarks>
        /// The <see cref="E:MailClientBase.Authenticated" /> event is raised whenever the client
        /// has been authenticated.
        /// </remarks>
        public event EventHandler<MailClientEventArgs> Authenticated;

        /// <summary>
        /// Occurs when the client has been successfully loaded message.
        /// </summary>
        /// <remarks>
        /// The <see cref="E:MailClientBase.GetMessage" /> event is raised whenever the client
        /// has been loaded message.
        /// </remarks>
        public event EventHandler<MailClientMessageEventArgs> GetMessage;

        /// <summary>
        /// Occurs when the client has been successfully sent message.
        /// </summary>
        /// <remarks>
        /// The <see cref="E:MailClientBase.SendMessage" /> event is raised whenever the client
        /// has been sent message.
        /// </remarks>
        public event EventHandler<MailClientEventArgs> SendMessage;

        protected virtual void OnAuthenticated(string message)
        {
            var eventHandler = Authenticated;
            if (eventHandler != null)
                eventHandler.Invoke(this, new MailClientEventArgs(message, Account));
        }

        protected virtual void OnGetMessage(MimeMessage message, string messageUid, bool unread, MailFolder folder)
        {
            var eventHandler = GetMessage;
            if (eventHandler != null)
                eventHandler.Invoke(this,
                    new MailClientMessageEventArgs(message, messageUid, unread, folder, Account, Log));
        }

        protected virtual void OnSentMessage(string message)
        {
            var eventHandler = SendMessage;
            if (eventHandler != null)
                eventHandler.Invoke(this, new MailClientEventArgs(message, Account));
        }

        #region .Public

        #region .Constructor

        public MailClient(MailBoxData mailbox,
            CancellationToken cancelToken,
            List<ServerFolderAccessInfo> serverFolderAccessInfos,
            int tcpTimeout = 30000,
            bool certificatePermit = false, string protocolLogPath = "",
            ILog log = null, bool skipSmtp = false, bool enableDsn = false)
        {
            var protocolLogger = !string.IsNullOrEmpty(protocolLogPath)
                ? (IProtocolLogger)
                    new ProtocolLogger(protocolLogPath)
                : new NullProtocolLogger();

            Account = mailbox;
            ServerFolderAccessInfos = serverFolderAccessInfos;
            CertificatePermit = certificatePermit;
            Log = log ?? new NullLog();

            StopTokenSource = new CancellationTokenSource();

            CancelToken = CancellationTokenSource.CreateLinkedTokenSource(cancelToken, StopTokenSource.Token).Token;

            Log.Debug($"MailClient: Constructor -> CertificatePermit: {CertificatePermit}.");

            if (Account.Imap)
            {
                Imap = new ImapClient(protocolLogger)
                {
                    Timeout = tcpTimeout
                };

                Imap.ServerCertificateValidationCallback = CertificateValidationCallback;

                Pop = null;
            }
            else
            {
                Pop = new Pop3Client(protocolLogger)
                {
                    Timeout = tcpTimeout
                };
                Imap = null;
            }

            if (skipSmtp)
            {
                Smtp = null;
                return;
            }

            if (enableDsn)
            {
                Smtp = new DsnSmtpClient(protocolLogger,
                    DeliveryStatusNotification.Success |
                    DeliveryStatusNotification.Failure |
                    DeliveryStatusNotification.Delay)
                {
                    Timeout = tcpTimeout
                };
            }
            else
            {
                Smtp = new SmtpClient(protocolLogger)
                {
                    Timeout = tcpTimeout
                };
            }

            IsConnected = false;
            IsAuthenticated = false;
        }

        #endregion

        bool CertificateValidationCallback(object sender, X509Certificate certificate, X509Chain chain, SslPolicyErrors sslPolicyErrors)
        {
            Log.Debug($"CertificateValidationCallback(). Certificate callback: {certificate.Subject}.");

            if (sslPolicyErrors == SslPolicyErrors.None)
            {
                Log.Debug("CertificateValidationCallback(). No Ssl policy errors...");
                return true;
            }

            return CertificatePermit;
        }

        public MimeMessage GetInboxMessage(string uidl)
        {
            if (string.IsNullOrEmpty(uidl))
                throw new ArgumentNullException("uidl");

            if (Account.Imap)
            {
                if (!Imap.IsAuthenticated)
                    LoginImap();

                var elements = uidl.Split('-');

                var index = Convert.ToInt32(elements[0]);
                var folderId = Convert.ToInt32(elements[1]);

                if (folderId != (int)FolderType.Inbox)
                    throw new ArgumentException("Uidl is invalid. Only INBOX folder is supported.");

                var inbox = Imap.Inbox;

                inbox.Open(FolderAccess.ReadOnly);

                var allUids = (Imap.Capabilities & ImapCapabilities.ESearch) != 0
                    ? inbox.Search(SearchOptions.All, SearchQuery.All, CancelToken).UniqueIds
                    : inbox.Fetch(0, -1, MessageSummaryItems.UniqueId, CancelToken).Select(r => r.UniqueId).ToList();

                var uid = allUids.FirstOrDefault(u => u.Id == index);

                if (!uid.IsValid)
                    throw new Exception("IMAP4 uidl not found");

                return Imap.Inbox.GetMessage(uid, CancelToken);
            }
            else
            {
                if (!Pop.IsAuthenticated)
                    LoginPop3();

                var i = 0;
                var uidls =
                    Pop.GetMessageUids(CancelToken)
                        .Select(u => new KeyValuePair<int, string>(i++, u))
                        .ToDictionary(t => t.Key, t => t.Value);

                var uid = uidls.FirstOrDefault(u => u.Value.Equals(uidl, StringComparison.OrdinalIgnoreCase));

                if (uid.Value == null)
                    throw new Exception("POP3 uidl not found");

                return Pop.GetMessage(uid.Key, CancelToken);
            }
        }

        public void Aggregate(MailSettings mailSettings, int limitMessages = -1)
        {
            if (Account.Imap)
                AggregateImap(mailSettings, limitMessages);
            else
                AggregatePop3(limitMessages);
        }

        public void Send(MimeMessage message, bool needCopyToSentFolder = true)
        {
            if (!Smtp.IsConnected)
                LoginSmtp();

            Smtp.Send(message, CancelToken);

            if (!Account.Imap || !needCopyToSentFolder)
                return;

            AppendCopyToSentFolder(message);
        }

        public void Cancel()
        {
            Log.Info("MailClient -> Cancel()");
            StopTokenSource.Cancel();
        }

        public void Dispose()
        {
            Log.Info("MailClient -> Dispose()");

            try
            {
                if (Imap != null)
                {
                    lock (Imap.SyncRoot)
                    {
                        if (Imap.IsConnected)
                        {
                            Log.Debug("Imap -> Disconnect()");
                            Imap.Disconnect(true, CancelToken);
                        }

                        Imap.Dispose();
                        IsDisposed = true;
                    }
                }

                if (Pop != null)
                {
                    lock (Pop.SyncRoot)
                    {
                        if (Pop.IsConnected)
                        {
                            Log.Debug("Pop -> Disconnect()");
                            Pop.Disconnect(true, CancelToken);
                        }

                        Pop.Dispose();
                    }
                }

                if (Smtp != null)
                {
                    lock (Smtp.SyncRoot)
                    {
                        if (Smtp.IsConnected)
                        {
                            Log.Debug("Smtp -> Disconnect()");
                            Smtp.Disconnect(true, CancelToken);
                        }

                        Smtp.Dispose();
                    }
                }

                Authenticated = null;
                SendMessage = null;
                GetMessage = null;

                StopTokenSource.Dispose();

            }
            catch (Exception ex)
            {
                Log.ErrorFormat($"MailClient -> Dispose(MailboxId={Account.MailBoxId} MailboxAddres: '{Account.EMail.Address}')\r\nException: {ex.Message}\r\n");
            }
        }

        public void LoginClient()
        {
            if (Account.Imap)
            {
                if (!Imap.IsAuthenticated)
                    LoginImap();
            }
            else
            {
                if (!Pop.IsAuthenticated)
                    LoginPop3();
            }
        }

        public LoginResult TestLogin()
        {
            var result = new LoginResult
            {
                Imap = Account.Imap
            };

            try
            {
                if (Account.Imap)
                {
                    if (!Imap.IsAuthenticated)
                        LoginImap(false);
                }
                else
                {
                    if (!Pop.IsAuthenticated)
                        LoginPop3(false);
                }

                result.IngoingSuccess = true;
            }
            catch (Exception inEx)
            {
                result.IngoingSuccess = false;
                result.IngoingException = inEx;
            }

            try
            {
                if (!Smtp.IsAuthenticated)
                    LoginSmtp();

                result.OutgoingSuccess = true;
            }
            catch (Exception outEx)
            {
                result.OutgoingSuccess = false;
                result.OutgoingException = outEx;
            }

            return result;
        }

        public static MimeMessage ParseMimeMessage(Stream emlStream)
        {
            var options = new ParserOptions
            {
                AddressParserComplianceMode = RfcComplianceMode.Loose,
                ParameterComplianceMode = RfcComplianceMode.Loose,
                Rfc2047ComplianceMode = RfcComplianceMode.Loose,
                CharsetEncoding = Encoding.UTF8,
                RespectContentLength = false
            };

            var msg = MimeMessage.Load(options, emlStream);

            msg.FixEncodingIssues();

            return msg;
        }

        public static MimeMessage ParseMimeMessage(string emlPath)
        {
            using (var fs = new FileStream(emlPath, FileMode.Open, FileAccess.Read))
            {
                return ParseMimeMessage(fs);
            }
        }

        #endregion

        #region .Private

        #region .IMAP

        private void LoginImap(bool enableUtf8 = true)
        {
            var secureSocketOptions = SecureSocketOptions.Auto;
            var sslProtocols = SslProtocols.None;

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

            Log.DebugFormat($"Imap: Connect({Account.Server}:{Account.Port}, " +
                $"{Enum.GetName(typeof(SecureSocketOptions), secureSocketOptions)})");

            try
            {
                Imap.SslProtocols = sslProtocols;

                Log.InfoFormat("Try connect... to ({1}:{2}) timeout {0} miliseconds", CONNECT_TIMEOUT, Account.Server, Account.Port);

                var t = Imap.ConnectAsync(Account.Server, Account.Port, secureSocketOptions, CancelToken);

                if (!t.Wait(CONNECT_TIMEOUT, CancelToken))
                {
                    Log.Debug("Imap: Failed connect: Timeout.");
                    throw new TimeoutException("Imap: ConnectAsync() timeout.");
                }
                else
                {
                    IsConnected = true;
                    Log.Debug("Imap: Successfull connection. Working on!");
                }

                if (enableUtf8 && (Imap.Capabilities & ImapCapabilities.UTF8Accept) != ImapCapabilities.None)
                {
                    Log.Debug("Imap: EnableUTF8().");

                    t = Imap.EnableUTF8Async(CancelToken);

                    if (!t.Wait(ENABLE_UTF8_TIMEOUT, CancelToken))
                        throw new TimeoutException("Imap: EnableUTF8Async() timeout.");
                }

                Imap.Authenticated += ImapOnAuthenticated;

                if (string.IsNullOrEmpty(Account.OAuthToken))
                {
                    Log.DebugFormat($"Imap: Authentication({Account.Account}).");
                    t = Imap.AuthenticateAsync(Account.Account, Account.Password, CancelToken);
                }
                else
                {
                    Log.DebugFormat("Imap: AuthenticationByOAuth({Account.Account})."); ;

                    var oauth2 = new SaslMechanismOAuth2(Account.Account, Account.AccessToken);

                    t = Imap.AuthenticateAsync(oauth2, CancelToken);
                }

                if (!t.Wait(LOGIN_TIMEOUT, CancelToken))
                {
                    Imap.Authenticated -= ImapOnAuthenticated;
                    Log.Debug("Imap: Failed authentication: Timeout.");
                    throw new TimeoutException("Imap: AuthenticateAsync timeout.");
                }
                else
                {
                    IsAuthenticated = true;
                    Log.Debug("Imap: Successfull authentication.");
                }

                Imap.Authenticated -= ImapOnAuthenticated;
            }
            catch (AggregateException aggEx)
            {
                if (aggEx.InnerException != null)
                {
                    Log.ErrorFormat($"Imap: Exception while logging. See next exception for details.");
                    throw aggEx.InnerException;
                }
                Log.ErrorFormat($"Imap: Exception while logging.");
                throw new Exception("LoginImap failed", aggEx);
            }
            catch (Exception ex)
            {
                Log.ErrorFormat("LoginImap error {0}", ex.Message);
                throw;
            }
            finally
            {
                Imap.Authenticated -= ImapOnAuthenticated;
            }
        }

        private void ImapOnAuthenticated(object sender, AuthenticatedEventArgs authenticatedEventArgs)
        {
            OnAuthenticated(authenticatedEventArgs.Message);
        }

        private void AggregateImap(MailSettings mailSettings, int limitMessages = -1)
        {
            if (!Imap.IsAuthenticated)
                LoginImap();

            try
            {
                var loaded = 0;

                var folders = GetImapFolders();

                foreach (var folder in folders)
                {
                    if (!Imap.IsConnected || CancelToken.IsCancellationRequested)
                        return;

                    var mailFolder = DetectFolder(mailSettings, folder);

                    if (mailFolder == null)
                    {
                        Log.InfoFormat("[folder] x '{0}' (skipped)", folder.Name);
                        continue;
                    }

                    Log.InfoFormat("[folder] >> '{0}' (fId={1}) {2}", folder.Name, mailFolder.Folder,
                        mailFolder.Tags.Any() ? string.Format("tag='{0}'", mailFolder.Tags.FirstOrDefault()) : "");

                    try
                    {
                        folder.Open(FolderAccess.ReadOnly, CancelToken);
                    }
                    catch (OperationCanceledException)
                    {
                        // Skip log error
                        continue;
                    }
                    catch (Exception e)
                    {
                        Log.ErrorFormat("Open faild: {0} Exception: {1}", folder.Name, e.Message);
                        continue;
                    }

                    loaded += LoadFolderMessages(folder, mailFolder, limitMessages, mailSettings.Aggregator.MaxMessageSizeLimit);

                    if (limitMessages <= 0 || loaded < limitMessages)
                        continue;

                    Log.Debug("Limit of maximum number messages per session is exceeded!");
                    break;
                }
            }
            catch (AggregateException aggEx)
            {
                if (aggEx.InnerException != null)
                {
                    throw aggEx.InnerException;
                }
                throw new Exception("AggregateImap failed", aggEx);
            }
        }

        private static bool CompareFolders(IMailFolder f1, IMailFolder f2)
        {
            Func<IMailFolder, bool> isInbox = (f) => f.Attributes.HasFlag(FolderAttributes.Inbox) ||
                                                     f.Name.Equals("inbox", StringComparison.InvariantCultureIgnoreCase);

            Func<IMailFolder, bool> isSent = (f) => f.Attributes.HasFlag(FolderAttributes.Sent) ||
                                                    f.Name.Equals("sent", StringComparison.InvariantCultureIgnoreCase) ||
                                                    f.Name.Equals("sent items", StringComparison.InvariantCultureIgnoreCase);

            Func<IMailFolder, bool> isSpam = (f) => f.Attributes.HasFlag(FolderAttributes.Junk) ||
                                                    f.Name.Equals("spam", StringComparison.InvariantCultureIgnoreCase) ||
                                                    f.Name.Equals("junk", StringComparison.InvariantCultureIgnoreCase) ||
                                                    f.Name.Equals("bulk", StringComparison.InvariantCultureIgnoreCase);

            if (isInbox(f1))
                return true;

            if (isSent(f1) && !isInbox(f2))
                return true;

            return isSpam(f1) && !isInbox(f2) && !isSent(f2);
        }

        private IEnumerable<IMailFolder> GetImapFolders()
        {
            Log.Debug("GetImapFolders()");

            var personal = Imap.GetFolders(Imap.PersonalNamespaces[0], true, CancelToken).ToList();

            if (!personal.Any(mb => mb.Name.Equals("inbox", StringComparison.InvariantCultureIgnoreCase)))
                personal.Add(Imap.Inbox);

            var folders = new List<IMailFolder>(personal);

            foreach (var folder in
                personal.Where(
                    f => f.Attributes.HasFlag(FolderAttributes.HasChildren)))
            {
                folders.AddRange(GetImapSubFolders(folder));
            }

            folders =
                folders.Where(
                    f =>
                        !f.Attributes.HasFlag(FolderAttributes.NoSelect) &&
                        !f.Attributes.HasFlag(FolderAttributes.NonExistent))
                    .Distinct()
                    .ToList();

            if (folders.Count <= 1)
                return folders;

            folders.Sort((f1, f2) => CompareFolders(f1, f2) ? -1 : CompareFolders(f2, f1) ? 1 : 0);

            return folders;
        }

        private IEnumerable<IMailFolder> GetImapSubFolders(IMailFolder folder)
        {
            try
            {
                var subfolders = folder.GetSubfolders(true, CancelToken).ToList();

                if (!subfolders.Any())
                {
                    return subfolders;
                }

                var tempList = new List<IMailFolder>();

                foreach (var subfolder in
                    subfolders.Where(
                        f => f.Attributes.HasFlag(FolderAttributes.HasChildren)))
                {
                    tempList.AddRange(GetImapSubFolders(subfolder));
                }

                subfolders.AddRange(tempList);

                return subfolders;
            }
            catch (Exception ex)
            {
                Log.ErrorFormat("GetImapSubFolders: {0} Exception: {1}", folder.Name, ex.Message);
            }

            return new List<IMailFolder>();
        }

        class TransferProgress : ITransferProgress
        {
            public long BytesTransferred;
            public long TotalSize;

            public void Report(long bytesTransferred, long totalSize)
            {
                BytesTransferred = bytesTransferred;
                TotalSize = totalSize;
            }

            public void Report(long bytesTransferred)
            {
                BytesTransferred = bytesTransferred;
            }
        }

        private int LoadFolderMessages(IMailFolder folder, MailFolder mailFolder, int limitMessages, uint? maxSize)
        {
            var loaded = 0;

            ImapFolderUids folderUids;
            if (!Account.ImapIntervals.TryGetValue(folder.Name, out folderUids))
            {
                Account.ImapFolderChanged = true;
                folderUids = new ImapFolderUids(new List<int> { 1, int.MaxValue }, 1, folder.UidValidity); // by default - mailbox never was processed before
            }
            else
            {
                if (folderUids.UidValidity.HasValue &&
                    folderUids.UidValidity != folder.UidValidity)
                {
                    Log.DebugFormat("Folder '{0}' UIDVALIDITY changed - need refresh folder", folder.Name);
                    folderUids = new ImapFolderUids(new List<int> { 1, int.MaxValue }, 1, folder.UidValidity); // reset folder check history if uidValidity has been changed
                }
            }

            if (!folderUids.UidValidity.HasValue)
            {
                Log.DebugFormat("Folder '{0}' Save UIDVALIDITY = {1}", folder.Name, folder.UidValidity);
                folderUids.UidValidity = folder.UidValidity; // Update UidValidity
            }

            var imapIntervals = new ImapIntervals(folderUids.UnhandledUidIntervals);
            var beginDateUid = folderUids.BeginDateUid;

            var allUids = GetFolderUids(folder);

            foreach (var uidsInterval in imapIntervals.GetUnhandledIntervalsCopy())
            {
                var interval = uidsInterval;
                var uidsCollection =
                    allUids.Select(u => u)
                        .Where(u => u.Id <= interval.To && u.Id >= interval.From)
                        .OrderByDescending(x => x)
                        .ToList();

                if (!uidsCollection.Any())
                {
                    if (!uidsInterval.IsToUidMax())
                        imapIntervals.AddHandledInterval(uidsInterval);
                    continue;
                }

                var first = uidsCollection.First().Id;
                var toUid = (int)(uidsInterval.IsToUidMax()
                    ? first
                    : Math.Max(uidsInterval.To, first));

                var infoList = GetMessagesSummaryInfo(folder,
                    limitMessages > 0 ? uidsCollection.Take(limitMessages * 3).ToList() : uidsCollection);

                foreach (var uid in uidsCollection)
                {
                    try
                    {
                        if (!Imap.IsConnected || CancelToken.IsCancellationRequested)
                        {
                            break;
                        }

                        var messInfo = folder.Fetch(new List<UniqueId>() { uid }, MessageSummaryItems.Size).FirstOrDefault();

                        if (messInfo.Size > maxSize)
                            throw new LimitMessageException(string.Format("Message size ({0}) exceeds fixed maximum message size ({1}). The message will be skipped.",
                                messInfo.Size, maxSize));

                        var progress = new TransferProgress();

                        Log.DebugFormat($"GetMessage(uid='{uid}')");

                        var message = folder.GetMessage(uid, CancelToken, progress);

                        Log.DebugFormat($"BytesTransferred = {progress.BytesTransferred}");

                        var uid1 = uid;
                        var info = infoList.FirstOrDefault(t => t.UniqueId == uid1);

                        message.FixDateIssues(info != null ? info.InternalDate : null, Log);

                        if (message.Date < Account.BeginDate)
                        {
                            Log.DebugFormat("Skip message (Date = {0}) on BeginDate = {1}", message.Date,
                                Account.BeginDate);
                            imapIntervals.SetBeginIndex(toUid);
                            beginDateUid = toUid;
                            break;
                        }

                        var unread = info != null &&
                                     (info.Keywords.Contains("\\Unseen") ||
                                      info.Flags.HasValue && !info.Flags.Value.HasFlag(MessageFlags.Seen));

                        message.FixEncodingIssues(Log);

                        OnGetMessage(message, uid.Id.ToString(), unread, mailFolder);

                        loaded++;
                    }
                    catch (LimitMessageException e)
                    {
                        Log.ErrorFormat(
                            "ProcessMessages() Tenant={0} User='{1}' Account='{2}', MailboxId={3}, UID={4} Exception:\r\n{5}\r\n",
                            Account.TenantId, Account.UserId, Account.EMail.Address, Account.MailBoxId,
                            uid, e);

                        loaded++;
                    }
                    catch (OperationCanceledException)
                    {
                        break;
                    }
                    catch (Exception e)
                    {
                        Log.ErrorFormat(
                            "ProcessMessages() Tenant={0} User='{1}' Account='{2}', MailboxId={3}, UID={4} Exception:\r\n{5}\r\n",
                            Account.TenantId, Account.UserId, Account.EMail.Address, Account.MailBoxId,
                            uid, e);

                        if (uid != uidsCollection.First() && (int)uid.Id != toUid)
                        {
                            imapIntervals.AddHandledInterval(new UidInterval((int)uid.Id + 1, toUid));
                        }
                        toUid = (int)uid.Id - 1;

                        if (e is IOException)
                        {
                            break; // stop checking other mailboxes
                        }

                        continue;
                    }

                    // after successfully message saving - lets update imap intervals state
                    imapIntervals.AddHandledInterval(
                        new UidInterval(
                            uid.Id == uidsCollection.Last().Id && uidsInterval.IsFromUidMin()
                                ? uidsInterval.From
                                : (int)uid.Id, toUid));

                    toUid = (int)uid.Id - 1;

                    if (limitMessages > 0 && loaded >= limitMessages)
                    {
                        break;
                    }
                }

                if (CancelToken.IsCancellationRequested || limitMessages > 0 && loaded >= limitMessages)
                {
                    break;
                }
            }

            var updatedImapFolderUids = new ImapFolderUids(imapIntervals.ToIndexes(), beginDateUid, folder.UidValidity);

            if (!Account.ImapIntervals.Keys.Contains(folder.Name))
            {
                Account.ImapFolderChanged = true;
                Account.ImapIntervals.Add(folder.Name, updatedImapFolderUids);
            }
            else if (Account.ImapIntervals[folder.Name] != updatedImapFolderUids)
            {
                Account.ImapFolderChanged = true;
                Account.ImapIntervals[folder.Name] = updatedImapFolderUids;
            }

            return loaded;
        }

        private List<UniqueId> GetFolderUids(IMailFolder folder)
        {
            List<UniqueId> allUids;

            try
            {
                allUids = folder.Fetch(0, -1, MessageSummaryItems.UniqueId, CancelToken).Select(r => r.UniqueId).ToList();
            }
            catch (ImapCommandException ex)
            {
                Log.WarnFormat("GetFolderUids() Exception: {0}", ex.ToString());

                const int start = 0;
                var end = folder.Count;
                const int increment = 1;

                allUids = Enumerable
                    .Repeat(start, (end - start) / 1 + 1)
                    .Select((tr, ti) => tr + increment * ti)
                    .Select(n => new UniqueId((uint)n))
                    .ToList();
            }

            return allUids;
        }

        private List<IMessageSummary> GetMessagesSummaryInfo(IMailFolder folder, IList<UniqueId> uids)
        {
            var infoList = new List<IMessageSummary>();

            try
            {
                infoList =
                    folder.Fetch(uids,
                        MessageSummaryItems.Flags | MessageSummaryItems.GMailLabels |
                        MessageSummaryItems.InternalDate, CancelToken).ToList();

            }
            catch (ImapCommandException ex)
            {
                Log.WarnFormat("GetMessagesSummaryInfo() Exception: {0}", ex.ToString());
            }

            return infoList;
        }

        private MailFolder DetectFolder(MailSettings mailSettings, IMailFolder folder)
        {
            var folderName = folder.Name.ToLowerInvariant();

            if (mailSettings.SkipImapFlags != null &&
                mailSettings.SkipImapFlags.Any() &&
                mailSettings.SkipImapFlags.Contains(folderName))
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

            if (mailSettings.ImapFlags != null &&
                mailSettings.ImapFlags.Any() &&
                mailSettings.ImapFlags.ContainsKey(folderName))
            {
                folderId = (FolderType)mailSettings.ImapFlags[folderName];
                return new MailFolder(folderId, folder.Name);
            }

            if (mailSettings.SpecialDomainFolders.Any() &&
                mailSettings.SpecialDomainFolders.ContainsKey(Account.Server))
            {
                var domainSpecialFolders = mailSettings.SpecialDomainFolders[Account.Server];

                if (domainSpecialFolders.Any() &&
                    domainSpecialFolders.ContainsKey(folderName))
                {
                    var info = domainSpecialFolders[folderName];
                    return info.skip ? null : new MailFolder(info.folder_id, folder.Name);
                }
            }

            if (mailSettings.DefaultFolders == null || !mailSettings.DefaultFolders.ContainsKey(folderName))
                return new MailFolder(FolderType.Inbox, folder.Name, new[] { folder.FullName });

            folderId = (FolderType)mailSettings.DefaultFolders[folderName];
            return new MailFolder(folderId, folder.Name);
        }

        private IMailFolder GetSentFolder()
        {
            var folders = Imap.GetFolders(Imap.PersonalNamespaces[0], false, CancelToken).ToList();

            if (!folders.Any())
                return null;

            var sendFolder = folders.FirstOrDefault(f => (f.Attributes & FolderAttributes.Sent) != 0);

            if (sendFolder != null)
                return sendFolder;

            if (ServerFolderAccessInfos == null || !ServerFolderAccessInfos.Any() || !ServerFolderAccessInfos.Any(f => f.Server == Account.Server))
                return null;

            foreach (var folder in folders)
            {
                var folderName = folder.Name;

                var serverInfo = ServerFolderAccessInfos.FirstOrDefault(f => f.Server == Account.Server);

                if (serverInfo == null)
                    continue;

                if (!serverInfo.FolderAccessList.TryGetValue(folderName, out ServerFolderAccessInfo.FolderInfo folderInfo))
                    continue;

                if (folderInfo.skip)
                    continue;

                if (folderInfo.folder_id != FolderType.Sent)
                    continue;

                sendFolder = folder;
                break;
            }

            return sendFolder;
        }

        private void AppendCopyToSentFolder(MimeMessage message)
        {
            if (!Account.Imap)
                throw new NotSupportedException("Only Imap is suppoted");

            if (message == null)
                throw new ArgumentNullException("message");

            try
            {
                if (!Imap.IsAuthenticated)
                    LoginImap();

                var sendFolder = GetSentFolder();

                if (sendFolder != null)
                {
                    sendFolder.Open(FolderAccess.ReadWrite);
                    var uid = sendFolder.Append(FormatOptions.Default, message, MessageFlags.Seen, CancelToken);

                    if (uid.HasValue)
                    {
                        Log.InfoFormat("AppendCopyToSenFolder(Mailbox: '{0}', Tenant: {1}, User: '{2}') succeed! (uid:{3})",
                            Account.EMail.Address, Account.TenantId, Account.UserId, uid.Value.Id);
                    }
                    else
                    {
                        Log.ErrorFormat("AppendCopyToSenFolder(Mailbox: '{0}', Tenant: {1}, User: '{2}') failed!",
                            Account.EMail.Address, Account.TenantId, Account.UserId);
                    }
                }
                else
                {
                    Log.DebugFormat(
                        "AppendCopyToSenFolder(Mailbox: '{0}', Tenant: {1}, User: '{2}'): Skip - sent-folder not found",
                        Account.EMail.Address, Account.TenantId, Account.UserId);
                }
            }
            catch (Exception ex)
            {
                Log.ErrorFormat("AppendCopyToSenFolder(Mailbox: '{0}', Tenant: {1}, User: '{2}'): Exception:\r\n{3}\r\n",
                    Account.EMail.Address, Account.TenantId, Account.UserId, ex.ToString());
            }
        }

        #endregion

        #region .POP3

        private void LoginPop3(bool enableUtf8 = true)
        {
            var secureSocketOptions = SecureSocketOptions.Auto;
            var sslProtocols = SslProtocols.None;

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

            Log.DebugFormat("Pop.Connect({0}:{1}, {2})", Account.Server, Account.Port,
                Enum.GetName(typeof(SecureSocketOptions), secureSocketOptions));
            try
            {
                Pop.SslProtocols = sslProtocols;

                var t = Pop.ConnectAsync(Account.Server, Account.Port, secureSocketOptions, CancelToken);

                if (!t.Wait(CONNECT_TIMEOUT, CancelToken))
                {
                    Log.InfoFormat("Pop3: Failed connect: Timeout.");
                    throw new TimeoutException("Pop.ConnectAsync timeout");
                }
                else
                {
                    IsConnected = true;
                    Log.InfoFormat("Imap: Successfull connection. Working on!");
                }

                if (enableUtf8 && (Pop.Capabilities & Pop3Capabilities.UTF8) != Pop3Capabilities.None)
                {
                    Log.Debug("Pop.EnableUTF8");

                    t = Pop.EnableUTF8Async(CancelToken);

                    if (!t.Wait(ENABLE_UTF8_TIMEOUT, CancelToken))
                        throw new TimeoutException("Pop.EnableUTF8Async timeout");
                }

                Pop.Authenticated += PopOnAuthenticated;

                if (string.IsNullOrEmpty(Account.OAuthToken))
                {
                    Log.DebugFormat("Pop.Authentication({0})", Account.Account);

                    t = Pop.AuthenticateAsync(Account.Account, Account.Password, CancelToken);
                }
                else
                {
                    Log.DebugFormat("Pop.AuthenticationByOAuth({0})", Account.Account);

                    var oauth2 = new SaslMechanismOAuth2(Account.Account, Account.AccessToken);

                    t = Pop.AuthenticateAsync(oauth2, CancelToken);
                }

                if (!t.Wait(LOGIN_TIMEOUT, CancelToken))
                {
                    Pop.Authenticated -= PopOnAuthenticated;
                    Log.InfoFormat("Pop: Authentication failed.");
                    throw new TimeoutException("Pop.AuthenticateAsync timeout");
                }
                else
                {
                    IsAuthenticated = true;
                    Log.InfoFormat("Pop: Successfull authentication.");
                }

                Pop.Authenticated -= PopOnAuthenticated;
            }
            catch (AggregateException aggEx)
            {
                if (aggEx.InnerException != null)
                {
                    Log.ErrorFormat($"Pop3: Exception while logging. See next exception for details.");
                    throw aggEx.InnerException;
                }
                Log.ErrorFormat($"Pop3: Exception while logging.");
                throw new Exception("LoginPop3 failed", aggEx);
            }
        }

        private void PopOnAuthenticated(object sender, AuthenticatedEventArgs authenticatedEventArgs)
        {
            OnAuthenticated(authenticatedEventArgs.Message);
        }

        public Func<Dictionary<int, string>, Dictionary<int, string>> FuncGetPop3NewMessagesIDs { get; set; }

        private void AggregatePop3(int limitMessages = -1)
        {
            if (!Pop.IsAuthenticated)
                LoginPop3();

            var mailFolder = new MailFolder(FolderType.Inbox, "INBOX");

            try
            {
                var loaded = 0;
                var i = 0;
                var uidls = Pop.GetMessageUids(CancelToken)
                    .Select(uidl => new KeyValuePair<int, string>(i++, uidl))
                    .ToDictionary(t => t.Key, t => t.Value);

                if (!uidls.Any() || uidls.Count == Account.MessagesCount)
                {
                    Account.MessagesCount = uidls.Count;

                    Log.Debug("New messages not found.\r\n");
                    return;
                }

                var newMessages = FuncGetPop3NewMessagesIDs(uidls);

                if (newMessages.Count == 0)
                {
                    Account.MessagesCount = uidls.Count;

                    Log.Debug("New messages not found.\r\n");
                    return;
                }

                Log.DebugFormat("Found {0} new messages.\r\n", newMessages.Count);

                newMessages = FixPop3UidsOrder(newMessages);

                var skipOnDate = Account.BeginDate != DefineConstants.MinBeginDate;

                foreach (var newMessage in newMessages)
                {
                    if (!Pop.IsConnected || CancelToken.IsCancellationRequested)
                        break;

                    Log.DebugFormat("Processing new message\tUID: {0}\tUIDL: {1}\t",
                        newMessage.Key,
                        newMessage.Value);

                    try
                    {
                        var message = Pop.GetMessage(newMessage.Key, CancelToken);

                        message.FixDateIssues(logger: Log);

                        if (message.Date < Account.BeginDate && skipOnDate)
                        {
                            Log.DebugFormat("Skip message (Date = {0}) on BeginDate = {1}", message.Date,
                                Account.BeginDate);
                            continue;
                        }

                        message.FixEncodingIssues();

                        OnGetMessage(message, newMessage.Value, true, mailFolder);

                        loaded++;

                        if (limitMessages <= 0 || loaded < limitMessages)
                            continue;

                        Log.Debug("Limit of max messages per session is exceeded!");
                        break;
                    }
                    catch (OperationCanceledException)
                    {
                        // Skip log error
                    }
                    catch (Exception e)
                    {
                        Log.ErrorFormat(
                            "ProcessMessages() Tenant={0} User='{1}' Account='{2}', MailboxId={3}, MessageIndex={4}, UIDL='{5}' Exception:\r\n{6}\r\n",
                            Account.TenantId, Account.UserId, Account.EMail.Address, Account.MailBoxId,
                            newMessage.Key, newMessage.Value, e);

                        if (e is IOException)
                        {
                            break; // stop checking other mailboxes
                        }
                    }
                }

                if (loaded < limitMessages)
                {
                    Account.MessagesCount = uidls.Count;
                }
            }
            catch (AggregateException aggEx)
            {
                if (aggEx.InnerException != null)
                {
                    throw aggEx.InnerException;
                }
                throw new Exception("AggregatePop3 failed", aggEx);
            }
        }

        private Dictionary<int, string> FixPop3UidsOrder(Dictionary<int, string> newMessages)
        {
            try
            {
                if (newMessages.Count < 2)
                    return newMessages;

                newMessages = newMessages
                    .OrderBy(item => item.Key)
                    .ToDictionary(id => id.Key, id => id.Value);

                var fstIndex = newMessages.First().Key;
                var lstIndex = newMessages.Last().Key;

                var fstMailHeaders = Pop.GetMessageHeaders(fstIndex, CancelToken).ToList();
                var lstMailHeaders = Pop.GetMessageHeaders(lstIndex, CancelToken).ToList();

                var fstDateHeader =
                    fstMailHeaders.FirstOrDefault(
                        h => h.Field.Equals("Date", StringComparison.InvariantCultureIgnoreCase));

                var lstDateHeader =
                    lstMailHeaders.FirstOrDefault(
                        h => h.Field.Equals("Date", StringComparison.InvariantCultureIgnoreCase));

                DateTime fstDate;
                DateTime lstDate;

                if (fstDateHeader != null && DateTime.TryParse(fstDateHeader.Value, out fstDate) &&
                    lstDateHeader != null &&
                    DateTime.TryParse(lstDateHeader.Value, out lstDate))
                {
                    if (fstDate < lstDate)
                    {
                        Log.DebugFormat("Account '{0}' uids order is DESC", Account.EMail.Address);
                        newMessages = newMessages
                            .OrderByDescending(item => item.Key)
                            .ToDictionary(id => id.Key, id => id.Value);
                        return newMessages;
                    }
                }


                Log.DebugFormat("Account '{0}' uids order is ASC", Account.EMail.Address);
            }
            catch (Exception)
            {
                newMessages = newMessages
                    .OrderByDescending(item => item.Key)
                    .ToDictionary(id => id.Key, id => id.Value);

                Log.WarnFormat("Calculating order skipped! Account '{0}' uids order is DESC", Account.EMail.Address);
            }

            return newMessages;
        }

        #endregion

        #region .SMTP

        private void LoginSmtp()
        {
            var secureSocketOptions = SecureSocketOptions.Auto;
            var sslProtocols = SslProtocols.None;

            switch (Account.SmtpEncryption)
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


            Log.DebugFormat("Smtp.Connect({0}:{1}, {2})", Account.SmtpServer, Account.SmtpPort,
                Enum.GetName(typeof(SecureSocketOptions), secureSocketOptions));
            try
            {
                Smtp.SslProtocols = sslProtocols;

                var t = Smtp.ConnectAsync(Account.SmtpServer, Account.SmtpPort, secureSocketOptions, CancelToken);

                if (!t.Wait(CONNECT_TIMEOUT, CancelToken))
                    throw new TimeoutException("Smtp.ConnectAsync timeout");

                if (!Account.SmtpAuth)
                {
                    if ((Smtp.Capabilities & SmtpCapabilities.Authentication) != 0)
                        throw new AuthenticationException("SmtpAuth is required (setup Authentication Type)");

                    return;
                }

                Smtp.Authenticated += SmtpOnAuthenticated;

                if (string.IsNullOrEmpty(Account.OAuthToken))
                {
                    Log.DebugFormat("Smtp.Authentication({0})", Account.SmtpAccount);

                    t = Smtp.AuthenticateAsync(Account.SmtpAccount, Account.SmtpPassword, CancelToken);
                }
                else
                {
                    Log.DebugFormat("Smtp.AuthenticationByOAuth({0})", Account.SmtpAccount);

                    var oauth2 = new SaslMechanismOAuth2(Account.Account, Account.AccessToken);

                    t = Smtp.AuthenticateAsync(oauth2, CancelToken);
                }

                if (!t.Wait(LOGIN_TIMEOUT, CancelToken))
                {
                    Smtp.Authenticated -= SmtpOnAuthenticated;
                    throw new TimeoutException("Smtp.AuthenticateAsync timeout");
                }

                Smtp.Authenticated -= SmtpOnAuthenticated;
            }
            catch (AggregateException aggEx)
            {
                if (aggEx.InnerException != null)
                {
                    throw aggEx.InnerException;
                }
                throw new Exception("LoginSmtp failed", aggEx);
            }
        }

        private void SmtpOnAuthenticated(object sender, AuthenticatedEventArgs authenticatedEventArgs)
        {
            OnAuthenticated(authenticatedEventArgs.Message);
        }

        #endregion

        #endregion
    }
}